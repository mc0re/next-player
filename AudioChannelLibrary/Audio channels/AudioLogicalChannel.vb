Imports Common


''' <summary>
''' A single audio output channel configuration.
''' </summary>
<CLSCompliant(True)>
Public Class AudioLogicalChannel
	Inherits LogicalChannelBase

#Region " Init and clean-up "

	Public Sub New()
		Description = "Audio channel"
	End Sub

#End Region


#Region " API "

	Public Sub PlayTestSound()
		For Each def In GetPhysicalChannels(Of AudioPhysicalChannel, AudioChannelLink, IAudioEnvironmentStorage)()
			def.Physical.PlayTestSound(def.Link)
		Next

		IsActive = True
	End Sub


	Public Sub StopTestSound()
		For Each def In GetPhysicalChannels(Of AudioPhysicalChannel, AudioChannelLink, IAudioEnvironmentStorage)()
			def.Physical.StopTestSound()
		Next

		IsActive = False
	End Sub

#End Region

End Class
