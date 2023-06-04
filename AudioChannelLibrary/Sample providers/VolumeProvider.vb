Imports System.Runtime.InteropServices
Imports Common
Imports NAudio.Wave


''' <summary>
''' An ISampleProvider implementation, which allows changing volume and panning.
''' </summary>
''' <remarks>Not reusable.</remarks>
Public Class VolumeProvider
    Implements ISampleProvider

#Region " Constants "

    ''' <summary>
    ''' Maximum allowed delay, to pre-allocate storage buffers.
    ''' </summary>
    Public Const MaxDelayMilliseconds As Single = 1000


    ''' <summary>
    ''' Standard head size (m)
    ''' </summary>
    Public Const HeadSize As Single = 0.215

#End Region


#Region " Fields "

    Private WithEvents mAudioLink As AudioChannelLink


    Private ReadOnly mSource As ISampleProvider


    ''' <summary>
    ''' The number of channels in the source data.
    ''' </summary>
    Private ReadOnly mNofSourceChannels As Integer


    Private WithEvents mDestChannel As AudioPhysicalChannel


    Private ReadOnly mDestChannelIndex As Integer


    ''' <summary>
    ''' The number of channels in the output.
    ''' </summary>
    Private ReadOnly mNofDestChannels As Integer


    Private ReadOnly mReader As New DelayedBuffer()


    Private ReadOnly mTestWriter As ITestDataWriter


    ''' <summary>
    ''' A list of modification coefficients.
    ''' </summary>
    Private mCurrentCoef As ChannelModifierCollection


    ''' <summary>
    ''' Current play position [frames].
    ''' </summary>
    Private mCurPosition As Long = -1


    ''' <summary>
    ''' Previous read's modification coefficients for smooth transition.
    ''' </summary>
    Private mOldCoef As ChannelModifierCollection

#End Region


#Region " PlaybackInfo property "

    ''' <summary>
    ''' A set of information, regulating the playback.
    ''' </summary>
    Private WithEvents mPlaybackInfo As New AudioPlaybackInfo()


    Public Property PlaybackInfo As AudioPlaybackInfo
        Get
            Return mPlaybackInfo
        End Get
        Set(value As AudioPlaybackInfo)
            mPlaybackInfo = value
        End Set
    End Property

#End Region


#Region " WaveFormat property "

    Public ReadOnly Property WaveFormat As WaveFormat Implements ISampleProvider.WaveFormat

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Create a new wave provider.
    ''' </summary>
    ''' <param name="source">Sample provider from the input data</param>
    ''' <param name="phCh">The output physical channel</param>
    ''' <param name="link">Link settings</param>
    ''' <remarks>
    ''' The input comes from <paramref name="source"/>, which has a number of channels
    ''' (defined by <paramref name="source"/>.WaveFormat.Channels).
    ''' 
    ''' <paramref name="link"/> defines, which input channels are taken,
    ''' by <see cref="AudioChannelLink.MappingCollection"/>.
    ''' Each link has its own Volume, Delay and Phase.
    ''' 
    ''' The output goes to the buffer, supplied to <see cref="Read"/> method.
    ''' The buffer is expected to be used by <paramref name="phCh"/> physical channel,
    ''' which has AudioInterface.Channels actual output mono channels,
    ''' and shall only use the one with number <see cref="AudioPhysicalChannel.DeviceChannel"/>.
    ''' That is, the output is always mono.
    ''' 
    ''' Mixing of the single channels is done by the sound driver.
    ''' </remarks>
    Public Sub New(source As ISampleProvider, info As AudioPlaybackInfo, phCh As AudioPhysicalChannel, link As AudioChannelLink)
        mSource = source
        mNofSourceChannels = mSource.WaveFormat.Channels
        mPlaybackInfo = info

        mDestChannel = phCh
        mDestChannelIndex = phCh.DeviceChannel - 1
        mNofDestChannels = phCh.AudioInterface.Channels

        WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(
            mSource.WaveFormat.SampleRate, mNofDestChannels)
        mAudioLink = link
        mReader.MaxDelay = DelayToSamples(MaxDelayMilliseconds)
        mTestWriter = InterfaceMapper.GetImplementation(Of ITestDataWriter)(True)

        UpdateCoefficients()
    End Sub

#End Region


#Region " Listeners "

    Private Sub PropertyChangedHandler() _
    Handles mAudioLink.PropertyChanged, mDestChannel.PropertyChanged, mPlaybackInfo.PropertyChanged
        UpdateCoefficients()
    End Sub

#End Region


#Region " ISampleProvider implementation "

    ''' <summary>
    ''' Read a chunk of audio data from source, process it
    ''' and write the result to the provided buffer.
    ''' </summary>
    Public Function Read(buffer() As Single, offset As Integer, count As Integer) As Integer Implements ISampleProvider.Read
        Dim framesRequested = count \ mNofDestChannels
        Dim samplesRequired = framesRequested * mNofSourceChannels

        ' Read original samples into mReader's buffer
        Dim samplesRead As Integer

        Try
            samplesRead = mSource.Read(mReader.ProvideBuffer(samplesRequired), 0, samplesRequired)
        Catch ex As Exception
            ' If something happened to the file reader
            Dim log = InterfaceMapper.GetImplementation(Of IMessageLog)()
            log.LogAudioError(ex.Message)
            Return 0
        End Try

        mReader.SetSamplesWritten(samplesRead)

        ' Calculate the amount of read data and to be produced data
        If samplesRead Mod mNofSourceChannels <> 0 Then
            Throw New ArgumentException("Read samples must be a multiple of channels.")
        End If

        Dim framesRead = samplesRead \ mNofSourceChannels
        Dim samplesProduced = framesRead * mNofDestChannels

        ' Zero-out the whole array.
        ' If it is not fully zeroed-out, residual sounds appear.
        Array.Clear(buffer, offset, buffer.Length - offset)

        If Not mPlaybackInfo.IsMuted AndAlso mAudioLink.IsEnabled AndAlso mDestChannel.IsEnabled Then
            ProcessAndWriteSamples(framesRead, buffer, offset)
        End If

#If DEBUG Then
        If mTestWriter IsNot Nothing Then
            mTestWriter.Write(mDestChannel, buffer, offset, count, samplesProduced)
        End If
#End If

        Return samplesProduced
    End Function

#End Region


#Region " Stream utility methods "

    ''' <summary>
    ''' Modify the samples and write them out
    ''' Consider both output channel settings and link (effect) settings.
    ''' </summary>
    ''' <param name="framesRead">The amount of read frames (1 sample for each source channel)</param>
    ''' <param name="buffer">Buffer to fill in</param>
    ''' <param name="offset">From which index to fill in the buffer</param>
    ''' <remarks>
    ''' Only the values for the destination channel are written in the output <paramref name="buffer"/>,
    ''' the other channels are not touched (and assumed to be zeros).
    ''' </remarks>
    Private Sub ProcessAndWriteSamples(framesRead As Integer, buffer As Single(), offset As Integer)
        For frameIdx = 0 To framesRead - 1
            ' To not have jumps in volume and panning, a smooth transition is applied
            ' of a 1-read length.
            Dim coefList = mCurrentCoef.Transition(mOldCoef, CSng(frameIdx) / framesRead)

            For Each modif In coefList
                ' There could be a delay applied to the link
                mReader.SetReadDelay(DelayToSamples(modif.Delay))

                Dim readIdx = frameIdx * mNofSourceChannels
                Dim writeIdx = offset + frameIdx * mNofDestChannels

                For destIdx = 0 To mNofDestChannels - 1
                    If destIdx <> mDestChannelIndex Then Continue For

                    Dim inputSample As Single = 0

                    For srcIdx = 0 To mNofSourceChannels - 1
                        Dim volume = modif.Volume(srcIdx)

                        If Math.Abs(volume) < RelativeVolumePrecision Then Continue For

                        Dim srcSample = mReader.Read(readIdx + srcIdx)
                        inputSample += srcSample * volume
                    Next

                    buffer(writeIdx + destIdx) = TrimSample(inputSample)
                Next
            Next
        Next

        ' Only valid for 1 read chunk
        mOldCoef = Nothing
        mCurPosition += framesRead
    End Sub

#End Region


#Region " Calculation utility methods "

    ''' <summary>
    ''' Calculate the coefficients for this channel.
    ''' </summary>
    ''' <remarks>
    ''' Also keeps the previous coefficients for a smooth transition effect
    ''' during playback (not start-up).
    ''' </remarks>
    Private Sub UpdateCoefficients()
        If mOldCoef Is Nothing AndAlso mCurPosition > 0 Then
            mOldCoef = mCurrentCoef
        End If

        mCurrentCoef = mPlaybackInfo.CoefficientGenerator.Generate(mDestChannel.Channel)
    End Sub


    ''' <summary>
    ''' Calculate the number of samples to meet the desired delay.
    ''' </summary>
    ''' <param name="delay">Delay in milliseconds</param>
    ''' <returns>Delay in samples, adjusted to the number of source channels</returns>
    Private Function DelayToSamples(delay As Single) As Integer
        Dim samp = CInt(delay * WaveFormat.SampleRate *
                    mNofSourceChannels / MillisecondsInSecond)
        samp = samp - samp Mod mNofSourceChannels

        Return samp
    End Function

#End Region

End Class
