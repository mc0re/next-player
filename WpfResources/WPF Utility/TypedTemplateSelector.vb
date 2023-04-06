Imports System.Windows.Markup


''' <summary>
''' A single entry.
''' </summary>
Public Class TypedTemplateDefinition

	Public Property DataType As Type

	Public Property Template As DataTemplate

End Class


Public Class TypedTemplateDefinitionList
	Inherits List(Of TypedTemplateDefinition)

End Class


''' <summary>
''' Select a template based on data type.
''' </summary>
<ContentProperty("DefinitionList")>
Public Class TypedTemplateSelector
	Inherits DataTemplateSelector

#Region " A list of templates "

	Public Property DefinitionList As New TypedTemplateDefinitionList()

#End Region


#Region " DefaultTemplate property "

	''' <summary>
	''' If the type was not found.
	''' </summary>
	Public Property DefaultTemplate As DataTemplate

#End Region


#Region " DataTemplateSelector override "

	Public Overrides Function SelectTemplate(item As Object, container As DependencyObject) As DataTemplate
		Dim objType As Type = Nothing
		If item IsNot Nothing Then
			objType = item.GetType()
		End If

		Dim templ = (
			From def In DefinitionList
			Where def.DataType Is objType Select def.Template
		).FirstOrDefault()

		Return If(templ, DefaultTemplate)
	End Function

#End Region

End Class
