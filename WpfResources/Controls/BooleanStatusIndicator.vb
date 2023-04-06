Public Class BooleanStatusIndicator
	Inherits Control

#Region " Status dependency property "

	Public Shared ReadOnly StatusProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Status), GetType(Boolean), GetType(BooleanStatusIndicator))


	Public Property Status As Boolean
		Get
			Return CBool(GetValue(StatusProperty))
		End Get
		Set(value As Boolean)
			SetValue(StatusProperty, value)
		End Set
	End Property

#End Region

End Class
