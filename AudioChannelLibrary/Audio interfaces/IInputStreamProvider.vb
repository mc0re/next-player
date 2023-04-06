Imports NAudio.Wave


Public Interface IInputStreamProvider

	''' <summary>
	''' Create a new stream each time, as the read position is not to be reused.
	''' </summary>
	Function CreateStream(fileName As String) As WaveStream

End Interface
