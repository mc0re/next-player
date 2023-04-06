Imports Common
Imports System.ComponentModel

''' <summary>
''' Show value, edit on click.
''' </summary>
Public Class NumberEditor
	Inherits Control

#Region " Commands "

	Public Property StartEditCommand As New DelegateCommand(AddressOf StartEditCommandExecuted)

	Public Property StopEditCommand As New DelegateCommand(AddressOf StopEditCommandExecuted)

#End Region


#Region " TextBlockStyle dependency property "

	Public Shared ReadOnly TextBlockStyleProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(TextBlockStyle), GetType(Style), GetType(NumberEditor))

	Public Property TextBlockStyle As Style
		Get
			Return CType(GetValue(TextBlockStyleProperty), Style)
		End Get
		Set(value As Style)
			SetValue(TextBlockStyleProperty, value)
		End Set
	End Property

#End Region


#Region " TextBoxStyle dependency property "

	Public Shared ReadOnly TextBoxStyleProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(TextBoxStyle), GetType(Style), GetType(NumberEditor))

	Public Property TextBoxStyle As Style
		Get
			Return CType(GetValue(TextBoxStyleProperty), Style)
		End Get
		Set(value As Style)
			SetValue(TextBoxStyleProperty, value)
		End Set
	End Property

#End Region


#Region " Number dependency property "

	Public Shared ReadOnly NumberProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Number), GetType(Double), GetType(NumberEditor),
		New FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault))


	<Category("Common Properties"), Description("What value is being edited")>
	Public Property Number As Double
		Get
			Return CDbl(GetValue(NumberProperty))
		End Get
		Set(value As Double)
			SetValue(NumberProperty, value)
		End Set
	End Property

#End Region


#Region " ViewerFormat dependency property "

	Public Shared ReadOnly ViewerFormatProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(ViewerFormat), GetType(String), GetType(NumberEditor))

	Public Property ViewerFormat As String
		Get
			Return CStr(GetValue(ViewerFormatProperty))
		End Get
		Set(value As String)
			SetValue(ViewerFormatProperty, value)
		End Set
	End Property

#End Region


#Region " IsEditMode dependency property "

	Public Shared ReadOnly IsEditModeProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(IsEditMode), GetType(Boolean), GetType(NumberEditor))

	Public Property IsEditMode As Boolean
		Get
			Return CBool(GetValue(IsEditModeProperty))
		End Get
		Set(value As Boolean)
			SetValue(IsEditModeProperty, value)
		End Set
	End Property

#End Region


#Region " Commands handling "

	Private Sub StartEditCommandExecuted(parameter As Object)
		IsEditMode = True
	End Sub


	Private Sub StopEditCommandExecuted(parameter As Object)
		IsEditMode = False
	End Sub

#End Region

End Class
