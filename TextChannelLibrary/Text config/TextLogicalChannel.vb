Imports Common


''' <summary>
''' Specific type for text logical channels.
''' </summary>
<Serializable>
Public Class TextLogicalChannel
	Inherits LogicalChannelBase

#Region " Init and clean-up "

	Public Sub New()
		Description = "Text channel"
	End Sub

#End Region


#Region " Show API "

	Public Sub ShowText(text As String)
		For Each def In GetPhysicalChannels(Of TextPhysicalChannel, TextChannelLink, ITextEnvironmentStorage)()
			If def.Link.IsEnabled AndAlso def.Physical.IsEnabled Then
				def.Physical.ShowText(text)
			End If
		Next

		IsActive = True
	End Sub


	Public Sub HideText()
		For Each def In GetPhysicalChannels(Of TextPhysicalChannel, TextChannelLink, ITextEnvironmentStorage)()
			If def.Link.IsEnabled AndAlso def.Physical.IsEnabled Then
				def.Physical.HideText()
			End If
		Next

		IsActive = False
	End Sub

#End Region

End Class
