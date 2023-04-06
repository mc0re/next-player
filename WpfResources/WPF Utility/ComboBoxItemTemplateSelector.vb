Imports System.Windows.Media


Public Class ComboBoxItemTemplateSelector
	Inherits DataTemplateSelector

	''' <summary>
	''' Template for an item in the closed box
	''' </summary>
	Public Property SelectedTemplate As DataTemplate


	''' <summary>
	''' Template for an item in the opened list
	''' </summary>
	Public Property DropDownTemplate As DataTemplate


	Public Overrides Function SelectTemplate(item As Object, container As DependencyObject) As DataTemplate

		While container IsNot Nothing
			container = VisualTreeHelper.GetParent(container)

			If TypeOf container Is ComboBoxItem Then
				Return DropDownTemplate
			End If
		End While

		Return SelectedTemplate
	End Function

End Class
