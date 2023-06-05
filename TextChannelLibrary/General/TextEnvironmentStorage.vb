Imports Common


<Serializable>
Public Class TextEnvironmentStorage
	Inherits ChannelEnvironmentStorage(Of TextPhysicalChannel, TextChannelLink)
	Implements ITextEnvironmentStorage

	Public Sub HideAll() Implements ITextEnvironmentStorage.HideAll
		For Each ph In Physical
			ph.SendText(Nothing)
		Next
	End Sub

End Class
