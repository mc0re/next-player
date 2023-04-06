''' <summary>
''' Used for drop-down menus to supply an action instead of a choice.
''' </summary>
Public Class NamedCommand
	Inherits DependencyObject

#Region " Name dependency property "

	Public Shared ReadOnly NameProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Name), GetType(String), GetType(NamedCommand))


	Public Property Name As String
		Get
			Return CStr(GetValue(NameProperty))
		End Get
		Set(value As String)
			SetValue(NameProperty, value)
		End Set
	End Property

#End Region


#Region " Command dependency property "

	Public Shared ReadOnly CommandProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Command), GetType(ICommand), GetType(NamedCommand))


	Public Property Command As ICommand
		Get
			Return CType(GetValue(CommandProperty), ICommand)
		End Get
		Set(value As ICommand)
			SetValue(CommandProperty, value)
		End Set
	End Property

#End Region

End Class
