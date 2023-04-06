Imports AudioChannelLibrary
Imports NAudio.Wave


''' <summary>
''' Just create a simple PCM multi-channel player.
''' </summary>
<Serializable>
Public Class TestTripleOutputInterface
    Inherits AudioOutputInterfaceBase

    Public Sub New()
        Channels = 3
    End Sub


    Public Overrides Function CreatePlayer() As IWavePlayer
        Return New TestWavePlayer(Channels)
    End Function

End Class
