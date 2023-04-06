Imports System.ComponentModel
Imports PlayerActions
Imports Reports


Public Class PrintListWindow

#Region " Playlist dependency property "

	Public Shared ReadOnly PlaylistProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Playlist), GetType(PlayerActionCollection), GetType(PrintListWindow))


	<Category("Common Properties"), Description("A list of all actions to use for printing")>
	Public Property Playlist As PlayerActionCollection
		Get
			Return CType(GetValue(PlaylistProperty), PlayerActionCollection)
		End Get
		Set(value As PlayerActionCollection)
			SetValue(PlaylistProperty, value)
		End Set
	End Property

#End Region


#Region " ReportTemplateList read-only dependency property "

	Private Shared ReadOnly ReportTemplateListPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
		NameOf(ReportTemplateList), GetType(IEnumerable(Of ReportTemplateItem)), GetType(PrintListWindow),
		New PropertyMetadata(Nothing))


	Public Shared ReadOnly ReportTemplateListProperty As DependencyProperty = ReportTemplateListPropertyKey.DependencyProperty


	<Category("Common Properties"), Description("A list of found report templates")>
	Public Property ReportTemplateList As IEnumerable(Of ReportTemplateItem)
		Get
			Return CType(GetValue(ReportTemplateListProperty), IEnumerable(Of ReportTemplateItem))
		End Get
		Private Set(value As IEnumerable(Of ReportTemplateItem))
			SetValue(ReportTemplateListPropertyKey, value)
		End Set
	End Property

#End Region


#Region " Init and clean-up "

	Public Sub New()
		' This call is required by the designer.
		InitializeComponent()

		' Collect report templates
		Dim root = ReportTemplateManager.ReportRoot
		Dim templateList = ReportTemplateManager.GetTemplates(root)
		ReportTemplateList = templateList.ToList()
	End Sub

#End Region


#Region " Command handlers "

	''' <summary>
	''' Close this window.
	''' </summary>
	Private Sub CloseWindowCommandExecuted(sender As Object, e As ExecutedRoutedEventArgs)
		DialogResult = True
	End Sub

#End Region

End Class
