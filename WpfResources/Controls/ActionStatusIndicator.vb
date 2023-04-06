Imports System.ComponentModel
Imports PlayerActions


''' <summary>
''' Show the status of an action.
''' </summary>
Public Class ActionStatusIndicator
	Inherits Control

#Region " Action dependency property "

	Public Shared ReadOnly ActionProperty As DependencyProperty = DependencyProperty.Register(
	 NameOf(Action), GetType(PlayerAction), GetType(ActionStatusIndicator))


	<Category("Common Properties"), Description("Action being shown")>
	Public Property Action As PlayerAction
		Get
			Return CType(GetValue(ActionProperty), PlayerAction)
		End Get
		Set(value As PlayerAction)
			SetValue(ActionProperty, value)
		End Set
	End Property

#End Region

End Class
