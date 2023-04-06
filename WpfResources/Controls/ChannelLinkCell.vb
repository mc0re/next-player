Imports System.ComponentModel
Imports Common


''' <summary>
''' Helper class to pass on parameters for ShowLinkEditorCommand.
''' </summary>
Public Class ShowLinkEditorCommandArgs

	''' <summary>
	''' Actual link object to edit.
	''' </summary>
	Public Link As IChannelLink

	''' <summary>
	''' Editing template (for specific parts).
	''' If Nothing, the window shouldn't be shown -
	''' the link should just be deleted.
	''' </summary>
	Public Template As DataTemplate

	''' <summary>
	''' A command to use for deleting a link.
	''' </summary>
	Public DeleteCommand As ICommand

End Class


''' <summary>
''' Shown in every cell of the link matrix.
''' Expected to be a visual descendant of ChannelEditorControlBase.
''' </summary>
Public Class ChannelLinkCell
	Inherits Button

#Region " Commands "

	Public Property ClickCommand As New DelegateCommand(AddressOf ClickCommandExecuted)


	Public Property DeleteLinkCommand As New DelegateCommand(AddressOf DeleteLinkCommandExecuted, AddressOf AddEditingCommandCanExecute)

#End Region


#Region " LogicalChannel attached dependency property "

	Public Shared ReadOnly LogicalChannelProperty As DependencyProperty = DependencyProperty.RegisterAttached(
		NameOf(LogicalChannel), GetType(Integer),
		GetType(ChannelLinkCell),
		New FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.Inherits, New PropertyChangedCallback(AddressOf OnChannelChanged)))


	<Category("Common Properties"), Description("Logical channel (to be) linked")>
	Public Property LogicalChannel As Integer
		Get
			Return CInt(GetValue(LogicalChannelProperty))
		End Get
		Set(value As Integer)
			SetValue(LogicalChannelProperty, value)
		End Set
	End Property


	Public Shared Sub SetLogicalChannel(element As UIElement, value As Integer)
		element.SetValue(LogicalChannelProperty, value)
	End Sub


	Public Shared Function GetLogicalChannel(element As UIElement) As Integer
		Return CInt(element.GetValue(LogicalChannelProperty))
	End Function


	Private Shared Sub OnChannelChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		Dim this = TryCast(obj, ChannelLinkCell)
		If this Is Nothing Then Return

		Dim logChannel = this.LogicalChannel
		Dim physChannel = this.PhysicalChannel

		If logChannel > 0 And physChannel > 0 Then
			Dim st = ChannelEditorControlBase.GetChannelStorage(this)
			this.Link = st.Links.GetLink(logChannel, physChannel)
		End If
	End Sub

#End Region


#Region " PhysicalChannel dependency property "

	Public Shared ReadOnly PhysicalChannelProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(PhysicalChannel), GetType(Integer),
		GetType(ChannelLinkCell),
		New FrameworkPropertyMetadata(New PropertyChangedCallback(AddressOf OnChannelChanged)))


	<Category("Common Properties"), Description("Physical channel (to be) linked")>
	Public Property PhysicalChannel As Integer
		Get
			Return CInt(GetValue(PhysicalChannelProperty))
		End Get
		Set(value As Integer)
			SetValue(PhysicalChannelProperty, value)
		End Set
	End Property

#End Region


#Region " Link dependency property "

	Public Shared ReadOnly LinkProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Link), GetType(IChannelLink), GetType(ChannelLinkCell))


	<Category("Common Properties"), Description("Link object or Nothing")>
	Public Property Link As IChannelLink
		Get
			Return CType(GetValue(LinkProperty), IChannelLink)
		End Get
		Set(value As IChannelLink)
			SetValue(LinkProperty, value)
			HasLink = value IsNot Nothing
		End Set
	End Property

#End Region


#Region " HasLink read-only dependency property "

	Private Shared ReadOnly HasLinkPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
		NameOf(HasLink), GetType(Boolean), GetType(ChannelLinkCell),
		New FrameworkPropertyMetadata(False))


	Public Shared ReadOnly HasLinkProperty As DependencyProperty = HasLinkPropertyKey.DependencyProperty


	<Category("Common Properties"), Description("Whether link object is not Nothing")>
	Public Property HasLink As Boolean
		Get
			Return CBool(GetValue(HasLinkProperty))
		End Get
		Private Set(value As Boolean)
			SetValue(HasLinkPropertyKey, value)
		End Set
	End Property

#End Region


#Region " Command checkers "

	Private Function AddEditingCommandCanExecute(sender As Object) As Boolean
		Return Not ChannelEditorControlBase.GetIsReadOnly(Me)
	End Function

#End Region


#Region " Command handlers "

	''' <summary>
	''' Open link editor. Add a new link if not present.
	''' </summary>
	''' <param name="param">Ignored</param>
	Private Sub ClickCommandExecuted(param As Object)
		Dim editorTemplate = ChannelEditorControlBase.GetLinkEditorTemplate(Me)

		' If no template, just switch the linkage on/off
		If Link IsNot Nothing AndAlso editorTemplate Is Nothing Then
			DeleteLinkCommand.Execute(Link)
			Return
		End If

		If Link Is Nothing Then
			Dim st = ChannelEditorControlBase.GetChannelStorage(Me)
			Link = st.Links.CreateNewLink(LogicalChannel, PhysicalChannel)
		End If

		' If no command, just switch the linkage on/off
		Dim cmd = ChannelEditorControlBase.GetShowLinkEditorCommand(Me)
		If cmd Is Nothing Then Return

		Dim cparam As New ShowLinkEditorCommandArgs() With {
			.Link = Link,
			.Template = editorTemplate,
			.DeleteCommand = DeleteLinkCommand
		}
		cmd.Execute(cparam)
	End Sub


	''' <summary>
	''' Add a new logical channel.
	''' </summary>
	''' <param name="param">Ignored</param>
	Private Sub DeleteLinkCommandExecuted(param As Object)
		Debug.Assert(param Is Link)
		Dim st = ChannelEditorControlBase.GetChannelStorage(Me)
		st.Links.DeleteLink(Link)
		Link = Nothing
	End Sub

#End Region

End Class
