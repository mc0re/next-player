<TemplatePart(Name:="PART_ScrollViewer", Type:=GetType(ScrollViewer))>
Public Class ScalableTextControl
	Inherits Control

	Private mScroller As ScrollViewer


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


#Region " Position dependency property "

	Public Shared ReadOnly PositionProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Position), GetType(Double), GetType(ScalableTextControl),
		New PropertyMetadata(AddressOf PositionChangedHandler))


	''' <summary>
	''' Scroll position, 0-1.
	''' </summary>
	Public Property Position As Double
		Get
			Return CDbl(GetValue(PositionProperty))
		End Get
		Set(value As Double)
			SetValue(PositionProperty, value)
		End Set
	End Property


	Private Shared Sub PositionChangedHandler(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
		Dim self = CType(d, ScalableTextControl)

		If self.mScroller IsNot Nothing Then
			self.mScroller.ScrollToHorizontalOffset(self.mScroller.ScrollableWidth * self.Position)
			self.mScroller.ScrollToVerticalOffset(self.mScroller.ScrollableHeight * self.Position)
		End If
	End Sub

#End Region


	Public Overrides Sub OnApplyTemplate()
		MyBase.OnApplyTemplate()
		mScroller = CType(Template.FindName("PART_ScrollViewer", Me), ScrollViewer)
	End Sub

End Class
