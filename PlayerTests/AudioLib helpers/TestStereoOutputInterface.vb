Imports AudioChannelLibrary
Imports NAudio.Wave


''' <summary>
''' Just create a simple PCM stereo player.
''' </summary>
<Serializable>
Public Class TestStereoOutputInterface
    Inherits AudioOutputInterfaceBase

    Public Sub New()
        Channels = 2
    End Sub


    Public Overrides Function CreatePlayer() As IWavePlayer
        Return New TestWavePlayer(Channels)
    End Function

End Class
