Imports System.ComponentModel
Imports Common


<Description("Allows editing a point in 3D")>
Public Class CoordinatesEditorControl
	Inherits Control

#Region " Point dependency property "

	Public Shared ReadOnly PointProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Point), GetType(IPositionRelative), GetType(CoordinatesEditorControl))


	<Description("The point to be edited")>
	Public Property Point As IPositionRelative
		Get
			Return CType(GetValue(PointProperty), IPositionRelative)
		End Get
		Set(value As IPositionRelative)
			SetValue(PointProperty, value)
		End Set
	End Property

#End Region

End Class
