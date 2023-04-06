Imports System.ComponentModel
Imports System.Windows.Media


Public Enum EditModes
	Hide
	[ReadOnly]
	Edit
End Enum


Public Class ColorEditorControl
	Inherits Control

#Region " EditedColor dependency property "

	Public Shared ReadOnly EditedColorProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(EditedColor), GetType(Color), GetType(ColorEditorControl),
		New FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault))


	<Category("Common Properties"), Description("Color to edit")>
	Public Property EditedColor As Color
		Get
			Return CType(GetValue(EditedColorProperty), Color)
		End Get
		Set(value As Color)
			SetValue(EditedColorProperty, value)
		End Set
	End Property

#End Region


#Region " EditMode dependency property "

	Public Shared ReadOnly EditModeProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(EditMode), GetType(EditModes), GetType(ColorEditorControl),
		New PropertyMetadata(EditModes.Edit))


	<Category("Common Properties"), Description("How the colour is presented")>
	Public Property EditMode As EditModes
		Get
			Return CType(GetValue(EditModeProperty), EditModes)
		End Get
		Set(value As EditModes)
			SetValue(EditModeProperty, value)
		End Set
	End Property

#End Region


#Region " Description dependency property "

	Public Shared ReadOnly DescriptionProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Description), GetType(String), GetType(ColorEditorControl))


	<Category("Common Properties"), Description("Colour short description for tooltip")>
	Public Property Description As String
		Get
			Return CStr(GetValue(DescriptionProperty))
		End Get
		Set(value As String)
			SetValue(DescriptionProperty, value)
		End Set
	End Property

#End Region

End Class
