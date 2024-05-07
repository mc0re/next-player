Imports System.ComponentModel
Imports TextChannelLibrary

''' <summary>
''' Show text in a coloured window.
''' </summary>
<TemplatePart(Name:="PART_ToolTipContainer")>
Public Class TextWindow
	Implements ITextRenderer

#Region " Events "

	Private Event DragStarted(initialMove As Vector, mouseCoords As Point)

	Private Event Drag(moveDistance As Vector, mouseCoords As Point)

	Private Event DragEnded()


	Private Event ResizeStarted(initialMove As Vector, mouseCoords As Point)

	Private Event Resize(moveDistance As Vector, mouseCoords As Point)

	Private Event ResizeEnded()

#End Region


#Region " Fields "

	Private mToolTipContainer As FrameworkElement

#End Region


#Region " IsWindowDragged read-only dependency property "

	Private Shared ReadOnly IsWindowDraggedPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
		NameOf(IsWindowDragged), GetType(Boolean), GetType(TextWindow),
		New FrameworkPropertyMetadata(False))


	Public Shared ReadOnly IsWindowDraggedProperty As DependencyProperty = IsWindowDraggedPropertyKey.DependencyProperty


	Public Property IsWindowDragged As Boolean
		Get
			Return CBool(GetValue(IsWindowDraggedProperty))
		End Get
		Set(value As Boolean)
			SetValue(IsWindowDraggedPropertyKey, value)
		End Set
	End Property

#End Region


#Region " Channel dependency property "

	Public Shared ReadOnly ChannelProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Channel), GetType(TextPhysicalChannel), GetType(TextWindow))


	Public Property Channel As TextPhysicalChannel
		Get
			Return CType(GetValue(ChannelProperty), TextPhysicalChannel)
		End Get
		Set(value As TextPhysicalChannel)
			SetValue(ChannelProperty, value)
		End Set
	End Property

#End Region


#Region " Configuration dependency property "

	Public Shared ReadOnly ConfigurationProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Configuration), GetType(RenderTextInterface), GetType(TextWindow))


	Public Property Configuration As RenderTextInterface
		Get
			Return CType(GetValue(ConfigurationProperty), RenderTextInterface)
		End Get
		Set(value As RenderTextInterface)
			SetValue(ConfigurationProperty, value)
			AddHandler value.PropertyChanged, AddressOf ConfigurationPropertyChanged
		End Set
	End Property


	Private Sub ConfigurationPropertyChanged(sender As Object, args As PropertyChangedEventArgs)
		' Break assignment loop
		If IsWindowDragged Then Return

		With Configuration
			Left = .Left
			Top = .Top
			Width = .Width
			Height = .Height
		End With
	End Sub

#End Region


#Region " Text dependency property "

	Public Shared ReadOnly TextProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Text), GetType(String), GetType(TextWindow))


	Public Property Text As String
		Get
			Return CStr(GetValue(TextProperty))
		End Get
		Set(value As String)
			SetValue(TextProperty, value)
		End Set
	End Property

#End Region


#Region " ScrollPosition dependency property "

	Public Shared ReadOnly ScrollPositionProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(ScrollPosition), GetType(Double), GetType(TextWindow))


	Public Property ScrollPosition As Double Implements ITextRenderer.ScrollPosition
		Get
			Return CDbl(GetValue(ScrollPositionProperty))
		End Get
		Set(value As Double)
			SetValue(ScrollPositionProperty, value)
		End Set
	End Property

#End Region


#Region " Init and clean-up "

	Public Sub New()
		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		mToolTipContainer = CType(FindName(NameOf(PART_ToolTipContainer)), FrameworkElement)
	End Sub

#End Region


#Region " ITextRenderer API "

	Private Sub ShowText(text As String) Implements ITextRenderer.Show
		' To switch focus back
		Dim oldWin = Application.Current.Windows.OfType(Of Window)().SingleOrDefault(Function(w) w.IsActive)

		Me.Text = text

		If Not IsVisible Then
			Show()
		End If

		Topmost = True
		Channel.IsActive = True

		' Set the focus back to main window
		oldWin?.Focus()
	End Sub


	Private Sub HideText() Implements ITextRenderer.Hide
		Text = String.Empty
		Hide()
		Channel.IsActive = False
	End Sub

#End Region


#Region " Drag / resize detection "

	''' <summary>
	''' Coordinates where the mouse was put down
	''' </summary>
	Private mMouseDownCoords As Point


	Private Sub MouseDownHandler(sender As Object, args As MouseButtonEventArgs) Handles Me.MouseDown
		Try
			mMouseDownCoords = Mouse.GetPosition(Application.Current.MainWindow)
		Catch
			' Swallow
		End Try
	End Sub


	Private Sub MouseMoveHandler(sender As Object, args As MouseEventArgs) Handles Me.MouseMove
		Dim mouseCoords As Point
		Try
			mouseCoords = Mouse.GetPosition(Application.Current.MainWindow)
		Catch
			Return
		End Try

		Dim moved = mouseCoords - mMouseDownCoords

		If (args.LeftButton <> MouseButtonState.Pressed AndAlso
			args.RightButton <> MouseButtonState.Pressed) OrElse
		   (Math.Abs(moved.X) <= SystemParameters.MinimumHorizontalDragDistance AndAlso
			Math.Abs(moved.Y) <= SystemParameters.MinimumVerticalDragDistance) Then

			Return
		End If

		If Not IsWindowDragged Then
			' Start the operation
			IsWindowDragged = True

			If args.LeftButton = MouseButtonState.Pressed Then
				RaiseEvent DragStarted(moved, mouseCoords)
			Else
				RaiseEvent ResizeStarted(moved, mouseCoords)
			End If
		Else
			' Operation is ongoing already
			If args.LeftButton = MouseButtonState.Pressed Then
				RaiseEvent Drag(moved, mouseCoords)
			Else
				RaiseEvent Resize(moved, mouseCoords)
			End If
		End If
	End Sub


	Private Sub MouseUpHandler(sender As Object, args As MouseButtonEventArgs) Handles Me.MouseUp
		IsWindowDragged = False

		If args.ChangedButton = MouseButton.Left Then
			RaiseEvent DragEnded()
		Else
			RaiseEvent ResizeEnded()
		End If
	End Sub

#End Region


#Region " Tooltip handling "

	Private Sub OperationStartedHandler(initialMove As Vector, mouseCoords As Point) Handles Me.DragStarted, Me.ResizeStarted
		Dim ttObj = TryCast(mToolTipContainer.ToolTip, ToolTip)
		If ttObj Is Nothing Then Return

		ttObj.DataContext = Me
		ttObj.IsEnabled = True
		ttObj.Placement = Primitives.PlacementMode.Absolute
		ttObj.IsOpen = True
		OperationOngoingHandler(initialMove, mouseCoords)
	End Sub


	Private Sub OperationOngoingHandler(moved As Vector, mouseCoords As Point) Handles Me.Drag, Me.Resize
		Dim ttObj = TryCast(mToolTipContainer.ToolTip, ToolTip)
		If ttObj Is Nothing Then Return

		ttObj.HorizontalOffset = mouseCoords.X
		ttObj.VerticalOffset = mouseCoords.Y
	End Sub


	Private Sub OperationEndedHandler() Handles Me.DragEnded, Me.ResizeEnded
		Dim ttObj = TryCast(mToolTipContainer.ToolTip, ToolTip)
		If ttObj Is Nothing Then Return

		ttObj.IsEnabled = False
		ttObj.IsOpen = False
	End Sub


#End Region


#Region " Drag handling "

	''' <summary>
	''' Original window position
	''' </summary>
	Private mOriginalPosition As Point


	Private Sub DragStartedHandler(initialMove As Vector, mouseCoords As Point) Handles Me.DragStarted
		' Compensate for the initial move (due to minimal distance)
		mOriginalPosition = New Point(Left - initialMove.X, Top - initialMove.Y)
	End Sub


	Private Sub DragHandler(moveDistance As Vector, mouseCoords As Point) Handles Me.Drag
		Left = mOriginalPosition.X + moveDistance.X
		Top = mOriginalPosition.Y + moveDistance.Y

		With Configuration
			.Left = CInt(Left)
			.Top = CInt(Top)
		End With
	End Sub

#End Region


#Region " Resize handling "

	''' <summary>
	''' Original window position
	''' </summary>
	Private mOriginalSize As Point


	Private Sub ResizeStartedHandler(initialMove As Vector, mouseCoords As Point) Handles Me.ResizeStarted
		' Compensate for the initial move (due to minimal distance)
		mOriginalSize = New Point(Width - initialMove.X, Height - initialMove.Y)
	End Sub


	Private Sub ResizeHandler(moveDistance As Vector, mouseCoords As Point) Handles Me.Resize
		Width = mOriginalSize.X + moveDistance.X
		Height = mOriginalSize.Y + moveDistance.Y

		With Configuration
			.Width = CInt(Width)
			.Height = CInt(Height)
		End With
	End Sub

#End Region

End Class
