Imports Common


Public Interface ITestDataWriter

	Sub Write(
		ph As IChannel,
		buffer() As Single, offset As Integer, requestedCount As Integer,
		producedCount As Integer)

End Interface
