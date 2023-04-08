Imports System.Threading.Tasks
Imports NAudio.Wave


Friend Class TestWavePlayer
    Implements IWavePlayer

#Region " Events "

    Public Event PlaybackStopped As EventHandler(Of StoppedEventArgs) Implements IWavePlayer.PlaybackStopped

#End Region


#Region " Fields "

    Private mNofChannels As Integer

    Private mPlaybackState As PlaybackState

    Private mProvider As IWaveProvider

#End Region


#Region " Init and clean-up "

    Public Sub New(nofChannels As Integer)
        mNofChannels = nofChannels
    End Sub

#End Region


#Region " IWavePlayer properties "

    Public Property Volume As Single Implements IWavePlayer.Volume


    Public ReadOnly Property PlaybackState As PlaybackState Implements IWavePlayer.PlaybackState
        Get
            Return mPlaybackState
        End Get
    End Property


    Public ReadOnly Property OutputWaveFormat As WaveFormat Implements IWavePlayer.OutputWaveFormat
        Get
            Return New WaveFormat()
        End Get
    End Property

#End Region


#Region " IWavePlayer API "

    Public Sub Init(waveProvider As IWaveProvider) Implements IWavePlayer.Init
        If (waveProvider.WaveFormat.Channels <> mNofChannels) Then
            Throw New ArgumentException($"Expected {mNofChannels} channels in the test player, got {waveProvider.WaveFormat.Channels}.")
        End If

        mProvider = waveProvider
    End Sub


    Public Sub Play() Implements IWavePlayer.Play
        mPlaybackState = PlaybackState.Playing
        Dim buffer = New Byte(999) {}
        Dim tmr = Task.Run(
            Sub()
                While mProvider.Read(buffer, 0, 1000) > 0
                End While
            End Sub)
        tmr.ContinueWith(
            Sub(t) RaiseEvent PlaybackStopped(Me, New StoppedEventArgs()))
    End Sub


    Public Sub [Stop]() Implements IWavePlayer.Stop
        mPlaybackState = PlaybackState.Stopped
        RaiseEvent PlaybackStopped(Me, New StoppedEventArgs())
    End Sub


    Public Sub Pause() Implements IWavePlayer.Pause
        mPlaybackState = PlaybackState.Paused
    End Sub


    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do nothing
    End Sub

#End Region

End Class
