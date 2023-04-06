Imports System.ComponentModel
Imports Common


''' <summary>
''' Base class for channel-based editor, non-generic to be used in XAML.
''' </summary>
Public MustInherit Class ChannelEditorControlBase
	Inherits Control

#Region " IsReadOnly attached dependency property "

	Public Shared ReadOnly IsReadOnlyProperty As DependencyProperty = DependencyProperty.RegisterAttached(
		NameOf(IsReadOnly), GetType(Boolean),
		GetType(ChannelEditorControlBase),
		New FrameworkPropertyMetadata(False, FrameworkPropertyMetadataOptions.Inherits))


	<Category("Common Properties"), Description("Whether the channel configurations are read-only")>
	Public Property IsReadOnly As Boolean
		Get
			Return CBool(GetValue(IsReadOnlyProperty))
		End Get
		Set(value As Boolean)
			SetValue(IsReadOnlyProperty, value)
		End Set
	End Property


	Public Shared Sub SetIsReadOnly(element As UIElement, value As Boolean)
		element.SetValue(IsReadOnlyProperty, value)
	End Sub


	Public Shared Function GetIsReadOnly(element As UIElement) As Boolean
		Return CBool(element.GetValue(IsReadOnlyProperty))
	End Function

#End Region


#Region " LogicalChannels dependency property "

	Public Shared ReadOnly LogicalChannelsProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(LogicalChannels), GetType(ICollection),
		GetType(ChannelEditorControlBase))


	<Category("Common Properties"), Description("Data collection of type ILogicalChannel")>
	Public Property LogicalChannels As ICollection
		Get
			Return CType(GetValue(LogicalChannelsProperty), ICollection)
		End Get
		Set(value As ICollection)
			SetValue(LogicalChannelsProperty, value)
		End Set
	End Property

#End Region


#Region " ChannelStorage attached dependency property "

	Public Shared ReadOnly ChannelStorageProperty As DependencyProperty = DependencyProperty.RegisterAttached(
		NameOf(ChannelStorage), GetType(ILinkStorageBase),
		GetType(ChannelEditorControlBase),
		New FrameworkPropertyMetadata(Nothing, FrameworkPropertyMetadataOptions.Inherits))


	<Category("Common Properties"), Description("Data collection of type IChannelStorage")>
	Public Property ChannelStorage As ILinkStorageBase
		Get
			Return CType(GetValue(ChannelStorageProperty), ILinkStorageBase)
		End Get
		Set(value As ILinkStorageBase)
			SetValue(ChannelStorageProperty, value)
		End Set
	End Property


	Public Shared Sub SetChannelStorage(element As UIElement, value As ILinkStorageBase)
		element.SetValue(ChannelStorageProperty, value)
	End Sub


	Public Shared Function GetChannelStorage(element As UIElement) As ILinkStorageBase
		Return CType(element.GetValue(ChannelStorageProperty), ILinkStorageBase)
	End Function

#End Region


#Region " ShowLinkEditorCommand attached dependency property "

	Public Shared ReadOnly ShowLinkEditorCommandProperty As DependencyProperty = DependencyProperty.RegisterAttached(
		NameOf(ShowLinkEditorCommand), GetType(ICommand),
		GetType(ShowLinkEditorCommandArgs),
		New FrameworkPropertyMetadata(Nothing, FrameworkPropertyMetadataOptions.Inherits))


	<Category("Common Properties"), Description("Loose coupling: command to show link editor")>
	Public Property ShowLinkEditorCommand As ICommand
		Get
			Return CType(GetValue(ShowLinkEditorCommandProperty), ICommand)
		End Get
		Set(value As ICommand)
			SetValue(ShowLinkEditorCommandProperty, value)
		End Set
	End Property


	Public Shared Sub SetShowLinkEditorCommand(element As UIElement, value As ICommand)
		element.SetValue(ShowLinkEditorCommandProperty, value)
	End Sub


	Public Shared Function GetShowLinkEditorCommand(element As UIElement) As ICommand
		Return CType(element.GetValue(ShowLinkEditorCommandProperty), ICommand)
	End Function

#End Region


#Region " LogicalItemSetupTemplate dependency property "

	Public Shared ReadOnly LogicalItemSetupTemplateProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(LogicalItemSetupTemplate), GetType(DataTemplate),
		GetType(ChannelEditorControlBase))


	<Category("Common Properties"), Description("DataTemplate for additional setup for logical channels")>
	Public Property LogicalItemSetupTemplate As DataTemplate
		Get
			Return CType(GetValue(LogicalItemSetupTemplateProperty), DataTemplate)
		End Get
		Set(value As DataTemplate)
			SetValue(LogicalItemSetupTemplateProperty, value)
		End Set
	End Property

#End Region


#Region " PhysicalHeaderTemplate dependency property "

	Public Shared ReadOnly PhysicalHeaderTemplateProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(PhysicalHeaderTemplate), GetType(DataTemplate),
		GetType(ChannelEditorControlBase))


	<Category("Common Properties"), Description("DataTemplate for table header for physical channels")>
	Public Property PhysicalHeaderTemplate As DataTemplate
		Get
			Return CType(GetValue(PhysicalHeaderTemplateProperty), DataTemplate)
		End Get
		Set(value As DataTemplate)
			SetValue(PhysicalHeaderTemplateProperty, value)
		End Set
	End Property

#End Region


#Region " PhysicalFooterTemplate dependency property "

	Public Shared ReadOnly PhysicalFooterTemplateProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(PhysicalFooterTemplate), GetType(DataTemplate),
		GetType(ChannelEditorControlBase))


	<Category("Common Properties"), Description("DataTemplate for table footer for physical channels")>
	Public Property PhysicalFooterTemplate As DataTemplate
		Get
			Return CType(GetValue(PhysicalFooterTemplateProperty), DataTemplate)
		End Get
		Set(value As DataTemplate)
			SetValue(PhysicalFooterTemplateProperty, value)
		End Set
	End Property

#End Region


#Region " LinkCellTemplate attached dependency property "

	Public Shared ReadOnly LinkCellTemplateProperty As DependencyProperty = DependencyProperty.RegisterAttached(
		NameOf(LinkCellTemplate), GetType(DataTemplate),
		GetType(ChannelEditorControlBase),
		New FrameworkPropertyMetadata(Nothing, FrameworkPropertyMetadataOptions.Inherits))


	<Category("Common Properties"), Description("DataTemplate for channel link in link matrix")>
	Public Property LinkCellTemplate As DataTemplate
		Get
			Return CType(GetValue(LinkCellTemplateProperty), DataTemplate)
		End Get
		Set(value As DataTemplate)
			SetValue(LinkCellTemplateProperty, value)
		End Set
	End Property


	Public Shared Sub SetLinkCellTemplate(element As UIElement, value As DataTemplate)
		element.SetValue(LinkCellTemplateProperty, value)
	End Sub


	Public Shared Function GetLinkCellTemplate(element As UIElement) As DataTemplate
		Return CType(element.GetValue(LinkCellTemplateProperty), DataTemplate)
	End Function

#End Region


#Region " LinkEditorTemplate attached dependency property "

	Public Shared ReadOnly LinkEditorTemplateProperty As DependencyProperty = DependencyProperty.RegisterAttached(
		NameOf(LinkEditorTemplate), GetType(DataTemplate),
		GetType(ChannelEditorControlBase),
		New FrameworkPropertyMetadata(Nothing, FrameworkPropertyMetadataOptions.Inherits))


	<Category("Common Properties"), Description("DataTemplate for channel link in editor window")>
	Public Property LinkEditorTemplate As DataTemplate
		Get
			Return CType(GetValue(LinkEditorTemplateProperty), DataTemplate)
		End Get
		Set(value As DataTemplate)
			SetValue(LinkEditorTemplateProperty, value)
		End Set
	End Property


	Public Shared Sub SetLinkEditorTemplate(element As UIElement, value As DataTemplate)
		element.SetValue(LinkEditorTemplateProperty, value)
	End Sub


	Public Shared Function GetLinkEditorTemplate(element As UIElement) As DataTemplate
		Return CType(element.GetValue(LinkEditorTemplateProperty), DataTemplate)
	End Function

#End Region


#Region " Commands "

	Public Property AddLogicalCommand As New DelegateCommand(AddressOf AddLogicalCommandExecuted, AddressOf AddLogicalCommandCanExecute)


	Public Property DeleteLogicalCommand As New DelegateCommand(AddressOf DeleteLogicalCommandExecuted, AddressOf DeleteLogicalCommandCanExecute)


	Public Property TestLogicalCommand As New DelegateCommand(AddressOf TestLogicalCommandExecuted)


	Public Property AddPhysicalCommand As New DelegateCommand(AddressOf AddPhysicalCommandExecuted)


	Public Property DeletePhysicalCommand As New DelegateCommand(AddressOf DeletePhysicalCommandExecuted)


	Public Property TestPhysicalCommand As New DelegateCommand(AddressOf TestPhysicalCommandExecuted)

#End Region


#Region " Methods to override "

#Region " Logical channels "

	Protected MustOverride Sub AddLogicalCommandOverride()


	Protected MustOverride Sub DeleteLogicalCommandOverride(logChannel As ILogicalChannel)


	Protected MustOverride Sub TestLogicalCommandOverride(logChannel As ILogicalChannel)

#End Region


#Region " Physical channels "

	Protected MustOverride Sub AddPhysicalCommandOverride()


	Protected MustOverride Sub DeletePhysicalCommandOverride(physChannel As IPhysicalChannel)


	Protected MustOverride Sub TestPhysicalCommandOverride(physChannel As IPhysicalChannel)

#End Region

#End Region


#Region " Command checkers "

	Private Function AddLogicalCommandCanExecute(sender As Object) As Boolean
		Return True
	End Function


	Private Function DeleteLogicalCommandCanExecute(sender As Object) As Boolean
		Return AddLogicalCommandCanExecute(sender) AndAlso LogicalChannels.Count > 1
	End Function

#End Region


#Region " Command handlers "

	''' <summary>
	''' Add a new logical channel.
	''' </summary>
	''' <param name="param">Ignored</param>
	Private Sub AddLogicalCommandExecuted(param As Object)
		AddLogicalCommandOverride()
	End Sub


	''' <summary>
	''' Delete the logical channel (command parameter).
	''' </summary>
	''' <param name="param">Channel to delete</param>
	Private Sub DeleteLogicalCommandExecuted(param As Object)
		Dim logChannel = CType(param, ILogicalChannel)
		DeleteLogicalCommandOverride(logChannel)
	End Sub


	''' <summary>
	''' Test a logical channel (command parameter).
	''' </summary>
	''' <param name="param">Channel to test</param>
	Private Sub TestLogicalCommandExecuted(param As Object)
		Dim logChannel = CType(param, ILogicalChannel)
		TestLogicalCommandOverride(logChannel)
	End Sub


	''' <summary>
	''' Add a new physical channel.
	''' </summary>
	''' <param name="param">Ignored</param>
	Private Sub AddPhysicalCommandExecuted(param As Object)
		AddPhysicalCommandOverride()
	End Sub


	''' <summary>
	''' Delete the physical channel (command parameter).
	''' </summary>
	''' <param name="param">Channel to delete</param>
	Private Sub DeletePhysicalCommandExecuted(param As Object)
		Dim physChannel = CType(param, IPhysicalChannel)
		DeletePhysicalCommandOverride(physChannel)
	End Sub


	''' <summary>
	''' Test a physical channel (command parameter).
	''' </summary>
	''' <param name="param">Channel to test</param>
	Private Sub TestPhysicalCommandExecuted(param As Object)
		Dim physChannel = CType(param, IPhysicalChannel)
		TestPhysicalCommandOverride(physChannel)
	End Sub

#End Region

End Class
