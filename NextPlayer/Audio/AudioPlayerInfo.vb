Imports NAudio.Wave
Imports AudioChannelLibrary


''' <summary>
''' Information about a single connection channel -> output device.
''' </summary>
Friend Class AudioPlayerInfo

#Region " Public properties "

	Public Property Provider As VolumeProvider

	Public Property Channel As AudioPhysicalChannel

	Public Property Link As AudioChannelLink

	Public Property Player As IWavePlayer

	Public Property Reader As WaveStream

	Public Property HasStopped As Boolean

#End Region


#Region " ToString "

	Public Overrides Function ToString() As String
		Return $"{Channel.Description}"
	End Function

#End Region

End Class
