Public Class DataGridTemplateBoundColumn
	Inherits DataGridBoundColumn

#Region " CellTemplate dependency property "

	Public Shared ReadOnly CellTemplateProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(CellTemplate), GetType(DataTemplate), GetType(DataGridTemplateBoundColumn))


	Public Property CellTemplate As DataTemplate
		Get
			Return CType(GetValue(CellTemplateProperty), DataTemplate)
		End Get
		Set(value As DataTemplate)
			SetValue(CellTemplateProperty, value)
		End Set
	End Property

#End Region


#Region " CellTemplateSelector dependency property "

	Public Shared ReadOnly CellTemplateSelectorProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(CellTemplateSelector), GetType(DataTemplateSelector), GetType(DataGridTemplateBoundColumn))


	Public Property CellTemplateSelector As DataTemplateSelector
		Get
			Return CType(GetValue(CellTemplateSelectorProperty), DataTemplateSelector)
		End Get
		Set(value As DataTemplateSelector)
			SetValue(CellTemplateSelectorProperty, value)
		End Set
	End Property

#End Region


#Region " DataGridBoundColumn overrides "

	Protected Overrides Function GenerateElement(cell As DataGridCell, dataItem As Object) As FrameworkElement
		Return LoadTemplateContent(dataItem, cell)
	End Function


	Protected Overrides Function GenerateEditingElement(cell As DataGridCell, dataItem As Object) As FrameworkElement
		Return LoadTemplateContent(dataItem, cell)
	End Function


	''' <summary>
	''' Override which handles property change for template properties.
	''' </summary>
	Protected Overrides Sub RefreshCellContent(element As FrameworkElement, propertyName As String)
		'Dim cell As DataGridCell = TryCast(element, DataGridCell)
		'If cell IsNot Nothing Then
		'	Dim isCellEditing As Boolean = cell.IsEditing

		'	If (Not isCellEditing AndAlso ((String.Compare(propertyName, "CellTemplate", StringComparison.Ordinal) = 0) OrElse (String.Compare(propertyName, "CellTemplateSelector", StringComparison.Ordinal) = 0))) OrElse (isCellEditing AndAlso ((String.Compare(propertyName, "CellEditingTemplate", StringComparison.Ordinal) = 0) OrElse (String.Compare(propertyName, "CellEditingTemplateSelector", StringComparison.Ordinal) = 0))) Then
		'		cell.BuildVisualTree()
		'		Return
		'	End If
		'End If

		'MyBase.RefreshCellContent(element, propertyName)
	End Sub

#End Region


#Region " Visual tree generation "

	''' <summary>
	'''     Creates the visual tree that will become the content of a cell. 
	''' </summary>
	''' <param name="dataItem">The data item for the cell.</param>
	''' <param name="cell">The cell container that will receive the tree.</param> 
	Private Function LoadTemplateContent(dataItem As Object, cell As DataGridCell) As FrameworkElement
		Dim template = CellTemplate
		Dim templateSelector = CellTemplateSelector

		If template Is Nothing AndAlso templateSelector Is Nothing Then
			Return Nothing
		End If

		Dim generated = CType(template.LoadContent(), FrameworkElement)

		'Dim cp As New ContentPresenter()
		'cp.ContentTemplate = template
		'cp.ContentTemplateSelector = templateSelector

		Dim b = Binding
		If b Is Nothing Then
			BindingOperations.ClearBinding(generated, Control.DataContextProperty)
		Else
			BindingOperations.SetBinding(generated, Control.DataContextProperty, b)
		End If

		Return generated
	End Function

#End Region

End Class
