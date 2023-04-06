Imports Common


''' <summary>
''' Base class for text output operations.
''' </summary>
<Serializable>
Public MustInherit Class TextPhysicalChannel
	Inherits ChannelBase
	Implements IPhysicalChannel

	Protected Sub New()
		Description = "Text output"
	End Sub


	Public MustOverride Sub ShowText(text As String)


	Public MustOverride Sub HideText()

End Class
