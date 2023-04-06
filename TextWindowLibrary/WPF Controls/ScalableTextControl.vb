Public Class ScalableTextControl
	Inherits Control

#Region " Text dependency property "

	Public Shared ReadOnly TextProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Text), GetType(String), GetType(ScalableTextControl))


	Public Property Text As String
		Get
			Return CStr(GetValue(TextProperty))
		End Get
		Set(value As String)
			SetValue(TextProperty, value)
		End Set
	End Property

#End Region

End Class
