Imports System.ComponentModel


Public Class CoordinateEditorControl
	Inherits Control

#Region " Label dependency property "

	Public Shared ReadOnly LabelProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Label), GetType(String), GetType(CoordinateEditorControl))


	<Description("Coordinate name")>
	Public Property Label As String
		Get
			Return CStr(GetValue(LabelProperty))
		End Get
		Set(value As String)
			SetValue(LabelProperty, value)
		End Set
	End Property

#End Region


#Region " Value dependency property "

	Public Shared ReadOnly ValueProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Value), GetType(Single), GetType(CoordinateEditorControl),
		New FrameworkPropertyMetadata(0.0F, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault))


	<Description("Coordinate value")>
	Public Property Value As Single
		Get
			Return CSng(GetValue(ValueProperty))
		End Get
		Set(value As Single)
			SetValue(ValueProperty, value)
		End Set
	End Property

#End Region


#Region " CanBeNegative dependency property "

	Public Shared ReadOnly CanBeNegativeProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(CanBeNegative), GetType(Boolean), GetType(CoordinateEditorControl))


	<Description("Coordinate value")>
	Public Property CanBeNegative As Boolean
		Get
			Return CBool(GetValue(CanBeNegativeProperty))
		End Get
		Set(value As Boolean)
			SetValue(CanBeNegativeProperty, value)
		End Set
	End Property

#End Region

End Class
