Imports System.ComponentModel
Imports System.Windows


Public Class ElementSpy
	Inherits Freezable

#Region " Element dependency property "

	Public Shared ReadOnly ElementProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Element), GetType(FrameworkElement), GetType(ElementSpy))


	<Category("Common Properties"), Description("Set and get the bound element")>
	Public Property Element As FrameworkElement
		Get
			Return DirectCast(GetValue(ElementProperty), FrameworkElement)
		End Get
		Set(value As FrameworkElement)
			SetValue(ElementProperty, value)
		End Set
	End Property

#End Region


#Region " Freezable overrides "

	Protected Overrides Function CreateInstanceCore() As Freezable
		Return New ElementSpy()
	End Function

#End Region

End Class
