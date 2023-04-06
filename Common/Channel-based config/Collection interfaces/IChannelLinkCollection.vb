Public Interface IChannelLinkCollection(Of TLink As IChannelLink)
	Inherits IChannelLinkCollectionBase

	Function GetForLogical(logicalNr As Integer) As IEnumerable(Of TLink)

End Interface
