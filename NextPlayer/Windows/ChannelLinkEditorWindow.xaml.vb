Imports System.ComponentModel
Imports Common


Public Class ChannelLinkEditorWindow
	Inherits Window

#Region " DeleteLinkCommand dependency property "

	Public Shared ReadOnly DeleteLinkCommandProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(DeleteLinkCommand), GetType(ICommand), GetType(ChannelLinkEditorWindow))


	<Category("Common Properties"), Description("Command used to delete the link. Parameter is the link to delete.")>
	Public Property DeleteLinkCommand As ICommand
		Get
			Return CType(GetValue(DeleteLinkCommandProperty), ICommand)
		End Get
		Set(value As ICommand)
			SetValue(DeleteLinkCommandProperty, value)
		End Set
	End Property

#End Region


#Region " Link dependency property "

	Public Shared ReadOnly LinkProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Link), GetType(IChannelLink), GetType(ChannelLinkEditorWindow))


	<Category("Common Properties"), Description("Link object to edit")>
	Public Property Link As IChannelLink
		Get
			Return CType(GetValue(LinkProperty), IChannelLink)
		End Get
		Set(value As IChannelLink)
			SetValue(LinkProperty, value)
		End Set
	End Property

#End Region


#Region " LinkEditorTemplate dependency property "

	Public Shared ReadOnly LinkEditorTemplateProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(LinkEditorTemplate), GetType(DataTemplate), GetType(ChannelLinkEditorWindow))


	<Category("Common Properties"), Description("DataTemplate for channel link in editor window")>
	Public Property LinkEditorTemplate As DataTemplate
		Get
			Return CType(GetValue(LinkEditorTemplateProperty), DataTemplate)
		End Get
		Set(value As DataTemplate)
			SetValue(LinkEditorTemplateProperty, value)
		End Set
	End Property

#End Region


#Region " Command handlers "

	Private Sub DeleteLinkClick(sender As Object, e As RoutedEventArgs)
		DeleteLinkCommand.Execute(Link)
		Close()
	End Sub

#End Region

End Class
