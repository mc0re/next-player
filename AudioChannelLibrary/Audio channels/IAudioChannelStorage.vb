Imports Common


Public Interface IAudioChannelStorage

	ReadOnly Property Logical As IChannelCollection(Of AudioLogicalChannel)


	''' <summary>
	''' Stop all test sounds.
	''' </summary>
	Sub StopAllTests()

End Interface
