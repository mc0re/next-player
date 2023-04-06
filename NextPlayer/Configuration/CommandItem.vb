Imports System.ComponentModel


''' <summary>
''' A single itme in the commands collection.
''' </summary>
Public Class CommandItem
	Inherits DependencyObject

#Region " Id dependency property "

	Public Shared ReadOnly IdProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Id), GetType(String), GetType(CommandItem))


	<Category("Common Properties"), Description("Command ID for saving and text retrieval")>
	Public Property Id As String
		Get
			Return CStr(GetValue(IdProperty))
		End Get
		Set(value As String)
			SetValue(IdProperty, value)
		End Set
	End Property

#End Region


#Region " Command dependency property "

	Public Shared ReadOnly CommandProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Command), GetType(RoutedCommand), GetType(CommandItem))


	<Category("Common Properties"), Description("Command being defined")>
	Public Property Command As RoutedCommand
		Get
			Return CType(GetValue(CommandProperty), RoutedCommand)
		End Get
		Set(value As RoutedCommand)
			SetValue(CommandProperty, value)
		End Set
	End Property

#End Region

	Public Property AssignedKey As Key

End Class
