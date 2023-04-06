Imports System.ComponentModel
Imports AudioChannelLibrary
Imports Common


Public Class AudioPositionEditorWindow

#Region " Room dependency property "

	Public Shared ReadOnly RoomProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Room), GetType(Room3D), GetType(AudioPositionEditorWindow))


	<Description("Defines the room, in which the channels are placed")>
	Public Property Room As Room3D
		Get
			Return CType(GetValue(RoomProperty), Room3D)
		End Get
		Set(value As Room3D)
			SetValue(RoomProperty, value)
		End Set
	End Property

#End Region


#Region " Channels dependency property "

	Public Shared ReadOnly ChannelsProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Channels), GetType(ChannelCollection(Of AudioPhysicalChannel)), GetType(AudioPositionEditorWindow))


	<Description("A collection of channels to edit")>
	Public Property Channels As ChannelCollection(Of AudioPhysicalChannel)
		Get
			Return CType(GetValue(ChannelsProperty), ChannelCollection(Of AudioPhysicalChannel))
		End Get
		Set(value As ChannelCollection(Of AudioPhysicalChannel))
			SetValue(ChannelsProperty, value)
		End Set
	End Property

#End Region

End Class
