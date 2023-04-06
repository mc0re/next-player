Imports System.ComponentModel
Imports AudioChannelLibrary


Public Class AudioChannelEditorWindow

#Region " Channel dependency property "

	Public Shared ReadOnly ChannelProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Channel), GetType(AudioPhysicalChannel), GetType(AudioChannelEditorWindow))


	<Description("The channel to edit")>
	Public Property Channel As AudioPhysicalChannel
		Get
			Return CType(GetValue(ChannelProperty), AudioPhysicalChannel)
		End Get
		Set(value As AudioPhysicalChannel)
			SetValue(ChannelProperty, value)
		End Set
	End Property

#End Region

End Class
