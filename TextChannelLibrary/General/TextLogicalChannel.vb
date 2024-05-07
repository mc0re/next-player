Imports Common


''' <summary>
''' Specific type for text logical channels.
''' </summary>
<Serializable>
Public Class TextLogicalChannel
	Inherits LogicalChannelBase

#Region " Fields "

	Private mClient As IPlaylistAction

#End Region


#Region " Init and clean-up "

	Public Sub New()
		Description = "Text channel"
	End Sub

#End Region


#Region " Show API "

	''' <summary>
	''' Set the current user of the channel.
	''' As only one user makes sense at a time, the previous one should be stopped.
	''' </summary>
	Public Sub SetClient(client As IPlaylistAction)
		mClient?.Stop(False)
		mClient = client
	End Sub


	''' <summary>
	''' Show the text in the window.
	''' </summary>
	Public Sub ShowText(text As String)
		For Each def In GetPhysicalChannels(Of TextPhysicalChannel, TextChannelLink, ITextEnvironmentStorage)()
			If def.Link.IsEnabled AndAlso def.Physical.IsEnabled Then
				def.Physical.SendText(text)
			End If
		Next

		IsActive = True
	End Sub


	''' <summary>
	''' Hide the window.
	''' </summary>
	Public Sub HideText()
		For Each def In GetPhysicalChannels(Of TextPhysicalChannel, TextChannelLink, ITextEnvironmentStorage)()
			If def.Link.IsEnabled AndAlso def.Physical.IsEnabled Then
				def.Physical.SendText(Nothing)
			End If
		Next

		IsActive = False
	End Sub


	''' <summary>
	''' If the text is scrollable, set its position.
	''' </summary>
	''' <param name="position">0-1</param>
	Public Sub SetPosition(position As Double)
		For Each def In GetPhysicalChannels(Of TextPhysicalChannel, TextChannelLink, ITextEnvironmentStorage)()
			If def.Link.IsEnabled AndAlso def.Physical.IsEnabled Then
				def.Physical.SetPosition(position)
			End If
		Next

		IsActive = False
	End Sub

#End Region

End Class
