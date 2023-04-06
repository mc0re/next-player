Imports System.ComponentModel
Imports System.Windows.Media


''' <summary>
''' Helps editing a set of font definition features.
''' </summary>
Public Class FontEditorControl
	Inherits Control

#Region " Description dependency property "

	Public Shared ReadOnly DescriptionProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Description), GetType(String), GetType(FontEditorControl))


	<Category("Common Properties"), Description("Set description")>
	Public Property Description As String
		Get
			Return CStr(GetValue(DescriptionProperty))
		End Get
		Set(value As String)
			SetValue(DescriptionProperty, value)
		End Set
	End Property

#End Region


#Region " BackColorEditMode dependency property "

	Public Shared ReadOnly BackColorEditModeProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(BackColorEditMode), GetType(EditModes), GetType(FontEditorControl),
		New PropertyMetadata(EditModes.Edit))


	<Category("Common Properties"), Description("How the background colour is presented")>
	Public Property BackColorEditMode As EditModes
		Get
			Return CType(GetValue(BackColorEditModeProperty), EditModes)
		End Get
		Set(value As EditModes)
			SetValue(BackColorEditModeProperty, value)
		End Set
	End Property

#End Region


#Region " BackColor dependency property "

	Public Shared ReadOnly BackColorProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(BackColor), GetType(Color), GetType(FontEditorControl),
		New FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault))


	<Category("Common Properties"), Description("Background color")>
	Public Property BackColor As Color
		Get
			Return CType(GetValue(BackColorProperty), Color)
		End Get
		Set(value As Color)
			SetValue(BackColorProperty, value)
		End Set
	End Property

#End Region


#Region " FrameColorEditMode dependency property "

	Public Shared ReadOnly FrameColorEditModeProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(FrameColorEditMode), GetType(EditModes), GetType(FontEditorControl),
		New PropertyMetadata(EditModes.Edit))


	<Category("Common Properties"), Description("How the frame colour is presented")>
	Public Property FrameColorEditMode As EditModes
		Get
			Return CType(GetValue(FrameColorEditModeProperty), EditModes)
		End Get
		Set(value As EditModes)
			SetValue(FrameColorEditModeProperty, value)
		End Set
	End Property

#End Region


#Region " FrameColor dependency property "

	Public Shared ReadOnly FrameColorProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(FrameColor), GetType(Color), GetType(FontEditorControl),
		New FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault))


	<Category("Common Properties"), Description("Frame color")>
	Public Property FrameColor As Color
		Get
			Return CType(GetValue(FrameColorProperty), Color)
		End Get
		Set(value As Color)
			SetValue(FrameColorProperty, value)
		End Set
	End Property

#End Region


#Region " ForeColorEditMode dependency property "

	Public Shared ReadOnly ForeColorEditModeProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(ForeColorEditMode), GetType(EditModes), GetType(FontEditorControl),
		New PropertyMetadata(EditModes.Edit))


	<Category("Common Properties"), Description("How the foreground colour is presented")>
	Public Property ForeColorEditMode As EditModes
		Get
			Return CType(GetValue(ForeColorEditModeProperty), EditModes)
		End Get
		Set(value As EditModes)
			SetValue(ForeColorEditModeProperty, value)
		End Set
	End Property

#End Region


#Region " ForeColor dependency property "

	Public Shared ReadOnly ForeColorProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(ForeColor), GetType(Color), GetType(FontEditorControl),
		New FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault))


	<Category("Common Properties"), Description("Foreground color")>
	Public Property ForeColor As Color
		Get
			Return CType(GetValue(ForeColorProperty), Color)
		End Get
		Set(value As Color)
			SetValue(ForeColorProperty, value)
		End Set
	End Property

#End Region


#Region " EditFontMode dependency property "

	Public Shared ReadOnly EditFontModeProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(EditFontMode), GetType(EditModes), GetType(FontEditorControl))


	<Category("Common Properties"), Description("Whether font weight is to be edited")>
	Public Property EditFontMode As EditModes
		Get
			Return CType(GetValue(EditFontModeProperty), EditModes)
		End Get
		Set(value As EditModes)
			SetValue(EditFontModeProperty, value)
		End Set
	End Property

#End Region


#Region " EditedWeight dependency property "

	Public Shared ReadOnly EditedWeightProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(EditedWeight), GetType(FontWeight), GetType(FontEditorControl),
		New FrameworkPropertyMetadata(FontWeights.Normal, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault))


	<Category("Common Properties"), Description("Edited font weight")>
	Public Property EditedWeight As FontWeight
		Get
			Return CType(GetValue(EditedWeightProperty), FontWeight)
		End Get
		Set(value As FontWeight)
			SetValue(EditedWeightProperty, value)
		End Set
	End Property

#End Region


#Region " EditedStyle dependency property "

	Public Shared ReadOnly EditedStyleProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(EditedStyle), GetType(FontStyle), GetType(FontEditorControl),
		New FrameworkPropertyMetadata(FontStyles.Normal, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault))


	<Category("Common Properties"), Description("Edited font style")>
	Public Property EditedStyle As FontStyle
		Get
			Return CType(GetValue(EditedStyleProperty), FontStyle)
		End Get
		Set(value As FontStyle)
			SetValue(EditedStyleProperty, value)
		End Set
	End Property

#End Region

End Class
