
Imports System.Windows


Public Class SingleItemStorage
	Inherits FrameworkElement

#Region " Source dependency property "

	Public Shared ReadOnly SourceProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Source), GetType(DependencyObject), GetType(SingleItemStorage))


	''' <summary>
	''' Source is the underlying object.
	''' </summary> 
	Public Property Source As DependencyObject
		Get
			Return CType(GetValue(SourceProperty), DependencyObject)
		End Get
		Set(value As DependencyObject)
			SetValue(SourceProperty, value)
		End Set
	End Property

#End Region

End Class
