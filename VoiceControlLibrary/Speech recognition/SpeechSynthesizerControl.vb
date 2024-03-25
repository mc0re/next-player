Imports System.IO
Imports System.Speech.AudioFormat
Imports System.Speech.Synthesis
Imports Common


Public NotInheritable Class SpeechSynthesizerControl
    Implements ISpeechSynthesizer

#Region " Fields "

    Private mSynth As New SpeechSynthesizer


    Private mPlayer As IVoicePlayer


    Private mVoiceConfig As IVoiceConfiguration

#End Region


#Region " SynthesizedVoices property "

    Private mSynthesizedVoices As IEnumerable(Of String)


    Public ReadOnly Property SynthesizedVoices As IEnumerable(Of String) Implements ISpeechSynthesizer.SynthesizedVoices
        Get
            If mSynthesizedVoices Is Nothing Then
                mSynthesizedVoices = mSynth.GetInstalledVoices().
                    Where(Function(v) v.Enabled).
                    Select(Function(v) v.VoiceInfo.Name).
                    ToList()
            End If

            Return mSynthesizedVoices
        End Get
    End Property

#End Region


    ''' <inheritdoc/>
    Public Sub Setup() Implements ISpeechSynthesizer.Setup
        mVoiceConfig = InterfaceMapper.GetImplementation(Of IVoiceConfiguration)()
        mPlayer = Nothing

        Try
            mSynth.SelectVoice(mVoiceConfig.VoiceControlFeedbackVoice)
        Catch
            ' Use default
        End Try
    End Sub


    ''' <inheritdoc/>
    Public Sub Speak(text As String) Implements ISpeechSynthesizer.Speak
        If Not mVoiceConfig.IsVoiceControlEnabled Then Return

        If mPlayer Is Nothing Then
            mPlayer = InterfaceMapper.GetImplementation(Of IVoicePlayer)()
        End If

        Dim audioStream As New MemoryStream
        Dim outputFormat = New SpeechAudioFormatInfo(44100, AudioBitsPerSample.Sixteen, AudioChannel.Mono)

        mSynth.SetOutputToAudioStream(audioStream, outputFormat)
        mSynth.Speak(text)
        mSynth.SetOutputToNull()
        audioStream.Seek(0, SeekOrigin.Begin)
        mPlayer.PlayAndForget(audioStream, mVoiceConfig.VoiceControlFeedbackChannel)
    End Sub

End Class
