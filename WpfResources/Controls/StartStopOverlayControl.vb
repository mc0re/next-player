Public Class StartStopOverlayControl
	Inherits Control

#Region " StartPosition dependency property "

	Public Shared ReadOnly StartPositionProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(StartPosition), GetType(TimeSpan), GetType(StartStopOverlayControl),
		New FrameworkPropertyMetadata(TimeSpan.Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
									  New PropertyChangedCallback(AddressOf StartPositionChanged)))


	Public Property StartPosition As TimeSpan
		Get
			Return CType(GetValue(StartPositionProperty), TimeSpan)
		End Get
		Set(value As TimeSpan)
			SetValue(StartPositionProperty, value)
		End Set
	End Property


	Private Shared Sub StartPositionChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		Dim this = CType(obj, StartStopOverlayControl)
		this.StartGridLength = ConvertPositionToLength(this.StartPosition, this.Duration, this.ActualWidth)
	End Sub

#End Region


#Region " StopPosition dependency property "

	Public Shared ReadOnly StopPositionProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(StopPosition), GetType(TimeSpan), GetType(StartStopOverlayControl),
		New FrameworkPropertyMetadata(TimeSpan.Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
									  New PropertyChangedCallback(AddressOf StopPositionChanged)))


	Public Property StopPosition As TimeSpan
		Get
			Return CType(GetValue(StopPositionProperty), TimeSpan)
		End Get
		Set(value As TimeSpan)
			SetValue(StopPositionProperty, value)
		End Set
	End Property


	Private Shared Sub StopPositionChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		Dim this = CType(obj, StartStopOverlayControl)
		this.StopGridLength = ConvertPositionToLength(this.StopPosition, this.Duration, this.ActualWidth)
	End Sub

#End Region


#Region " Duration dependency property "

	Public Shared ReadOnly DurationProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Duration), GetType(TimeSpan), GetType(StartStopOverlayControl),
		New PropertyMetadata(New PropertyChangedCallback(AddressOf DurationChanged)))


	Public Property Duration As TimeSpan
		Get
			Return CType(GetValue(DurationProperty), TimeSpan)
		End Get
		Set(value As TimeSpan)
			SetValue(DurationProperty, value)
		End Set
	End Property


	Private Shared Sub DurationChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		StartPositionChanged(obj, args)
		StopPositionChanged(obj, args)
	End Sub

#End Region


#Region " StartGridLength dependency property "

	Public Shared ReadOnly StartGridLengthProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(StartGridLength), GetType(GridLength), GetType(StartStopOverlayControl),
		New FrameworkPropertyMetadata(New GridLength(0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
									  New PropertyChangedCallback(AddressOf StartGridLengthChanged)))


	Public Property StartGridLength As GridLength
		Get
			Return CType(GetValue(StartGridLengthProperty), GridLength)
		End Get
		Set(value As GridLength)
			SetValue(StartGridLengthProperty, value)
		End Set
	End Property


	Private Shared Sub StartGridLengthChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		Dim this = CType(obj, StartStopOverlayControl)
		this.StartPosition = ConvertLengthToPosition(this.StartGridLength, this.ActualWidth, this.Duration)
	End Sub

#End Region


#Region " StopGridLength dependency property "

	Public Shared ReadOnly StopGridLengthProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(StopGridLength), GetType(GridLength), GetType(StartStopOverlayControl),
		New FrameworkPropertyMetadata(New GridLength(0), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
									  New PropertyChangedCallback(AddressOf StopGridLengthChanged)))


	Public Property StopGridLength As GridLength
		Get
			Return CType(GetValue(StopGridLengthProperty), GridLength)
		End Get
		Set(value As GridLength)
			SetValue(StopGridLengthProperty, value)
		End Set
	End Property


	Private Shared Sub StopGridLengthChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		Dim this = CType(obj, StartStopOverlayControl)
		this.StopPosition = ConvertLengthToPosition(this.StopGridLength, this.ActualWidth, this.Duration)
	End Sub

#End Region


#Region " UI event handlers "

	Private Sub SizeChangedHandler() Handles Me.SizeChanged
		StartPositionChanged(Me, Nothing)
		StopPositionChanged(Me, Nothing)
	End Sub

#End Region


#Region " Utility methods "

	Private Shared Function ConvertPositionToLength(pos As TimeSpan, maxPos As TimeSpan, maxSize As Double) As GridLength
		Dim maxMs = maxPos.TotalMilliseconds
		If maxMs = 0 Then Return New GridLength(0)

		Dim sz = pos.TotalMilliseconds / maxMs * maxSize
		Return New GridLength(sz)
	End Function


	Private Shared Function ConvertLengthToPosition(len As GridLength, maxSize As Double, maxPos As TimeSpan) As TimeSpan
		If maxSize = 0 Then Throw New NullReferenceException("Size is zero")
		If len.GridUnitType <> GridUnitType.Pixel Then Throw New ArgumentException("Non-pixel length")

		Dim maxMs = maxPos.TotalMilliseconds
		Dim ms = len.Value / maxSize * maxPos.TotalMilliseconds
		Return TimeSpan.FromMilliseconds(ms)
	End Function

#End Region

End Class
