Imports System.ComponentModel
Imports PlayerActions


''' <summary>
''' Show and control playback position.
''' </summary>
<TemplatePartAttribute(Name:="PART_Indicator", Type:=GetType(FrameworkElement))>
<TemplatePartAttribute(Name:="PART_Track", Type:=GetType(FrameworkElement))>
Public Class PlayPositionControl
	Inherits PositionedRangeBase

#Region " Position dependency property "

	Public Shared ReadOnly PositionProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Position), GetType(TimeSpan), GetType(PlayPositionControl),
		New PropertyMetadata(New PropertyChangedCallback(AddressOf PositionChanged)))


	<Category("Common Properties"), Description("Playback position; setting also changes progress.")>
	Property Position As TimeSpan
		Get
			Return CType(GetValue(PositionProperty), TimeSpan)
		End Get
		Set(value As TimeSpan)
			SetValue(PositionProperty, value)
		End Set
	End Property


	Private Shared Sub PositionChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		Dim this = CType(obj, PlayPositionControl)
		Dim newPos = CType(args.NewValue, TimeSpan)

		this.Value = newPos.TotalMilliseconds
	End Sub

#End Region


#Region " Overrides "

	''' <summary>
	''' Change position based on the mouse click.
	''' </summary>
	''' <remarks>Not possible if the control is not enabled</remarks>
	Protected Overrides Sub ProgressValueChanged(progress As Double)
		Dim res = TimeSpan.FromMilliseconds(progress)
		Dim action = CType(DataContext, PlayerAction)
		action.PlayPosition = res
	End Sub

#End Region

End Class
