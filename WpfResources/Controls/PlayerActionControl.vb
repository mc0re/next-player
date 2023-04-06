Imports System.ComponentModel
Imports AudioPlayerLibrary


''' <summary>
''' Custom control for displaying and editing PlayerAction.
''' </summary>
Public Class PlayerActionControl
	Inherits ContentControl

#Region " Action dependency property "

	Public Shared ReadOnly ActionProperty As DependencyProperty = DependencyProperty.Register(
	 NameOf(Action), GetType(IPlayerAction), GetType(PlayerActionControl))


	<Category("Common Properties"), Description("Action being shown and edited")>
	Public Property Action As IPlayerAction
		Get
			Return CType(GetValue(ActionProperty), IPlayerAction)
		End Get
		Set(value As IPlayerAction)
			SetValue(ActionProperty, value)
		End Set
	End Property

#End Region


#Region " ActionHeaderText dependency property "

	Public Shared ReadOnly ActionHeaderTextProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(ActionHeaderText), GetType(String), GetType(PlayerActionControl))


	<Category("Appearance"), Description("Text, related to the action being shown")>
	Public Property ActionHeaderText As String
		Get
			Return CStr(GetValue(ActionHeaderTextProperty))
		End Get
		Set(value As String)
			SetValue(ActionHeaderTextProperty, value)
		End Set
	End Property

#End Region

End Class
