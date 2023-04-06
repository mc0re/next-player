Imports AudioChannelLibrary
Imports NAudio.Wave


''' <summary>
''' Just create a simple PCM mono player.
''' </summary>
<Serializable>
Public Class TestMonoOutputInterface
    Inherits AudioOutputInterfaceBase

    Public Sub New()
        Channels = 1
    End Sub


    Public Overrides Function CreatePlayer() As IWavePlayer
        Return New TestWavePlayer(Channels)
    End Function

End Class
