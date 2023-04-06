Imports System.ComponentModel
Imports System.Windows.Controls.Primitives
Imports System.Windows.Media


''' <summary>
''' Base class for tracking mouse events - movements and clicks.
''' </summary>
<TemplatePart(Name:="PART_Indicator", Type:=GetType(FrameworkElement))>
<TemplatePart(Name:="PART_Track", Type:=GetType(FrameworkElement))>
Public MustInherit Class PositionedRangeBase
    Inherits RangeBase

#Region " Fields "

    ''' <summary>
    ''' The track, mouse clicks on which change the play position.
    ''' </summary>
    Private mTrack As FrameworkElement


    ''' <summary>
    ''' The indicator, which width is regulated by progress.
    ''' </summary>
    Private mIndicator As FrameworkElement

#End Region


#Region " MouseOverValue read-only dependency property "

    Private Shared ReadOnly MouseOverValuePropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(MouseOverValue), GetType(Double), GetType(PositionedRangeBase),
        New PropertyMetadata(0.0))


    Public Shared ReadOnly MouseOverValueProperty As DependencyProperty = MouseOverValuePropertyKey.DependencyProperty


    <Category("Common Properties"), Description("progress under mouse")>
    Public Property MouseOverValue As Double
        Get
            Return CDbl(GetValue(MouseOverValueProperty))
        End Get
        Private Set(newValue As Double)
            SetValue(MouseOverValuePropertyKey, newValue)
        End Set
    End Property

#End Region


#Region " Orientation dependency property "

    Public Shared ReadOnly OrientationProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(Orientation), GetType(Orientation), GetType(PositionedRangeBase),
        New FrameworkPropertyMetadata(Orientation.Horizontal))


    <Category("Appearance"), Description("Orientation of the bar")>
    Public Property Orientation As Orientation
        Get
            Return CType(GetValue(OrientationProperty), Orientation)
        End Get
        Set(newValue As Orientation)
            SetValue(OrientationProperty, newValue)
        End Set
    End Property

#End Region


#Region " DefaultValue dependency property "

    Public Shared ReadOnly DefaultValueProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(DefaultValue), GetType(Double), GetType(PositionedRangeBase))


    <Category("Common Properties"), Description("What is the default value")>
    Public Property DefaultValue As Double
        Get
            Return CDbl(GetValue(DefaultValueProperty))
        End Get
        Set(newValue As Double)
            SetValue(DefaultValueProperty, newValue)
        End Set
    End Property

#End Region


#Region " TrackGeometry dependency property "

    Public Shared ReadOnly TrackGeometryProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(TrackGeometry), GetType(Geometry), GetType(PositionedRangeBase))


    <Category("Appearance"), Description("Geometry to use for showing track")>
    Public Property TrackGeometry As Geometry
        Get
            Return CType(GetValue(TrackGeometryProperty), Geometry)
        End Get
        Set(newValue As Geometry)
            SetValue(TrackGeometryProperty, newValue)
        End Set
    End Property

#End Region


#Region " ForegroundBorderBrush dependency property "

    Public Shared ReadOnly ForegroundBorderBrushProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(ForegroundBorderBrush), GetType(Brush), GetType(PositionedRangeBase))


    <CodeAnalysis.SuppressMessage("Design", "CC0021:You should use nameof instead of the parameter element name string", Justification:="Brushes is a category")>
    <Category("Brushes"), Description("An outline color for foreground part")>
    Public Property ForegroundBorderBrush As Brush
        Get
            Return CType(GetValue(ForegroundBorderBrushProperty), Brush)
        End Get
        Set(newValue As Brush)
            SetValue(ForegroundBorderBrushProperty, newValue)
        End Set
    End Property

#End Region


#Region " ShowIndicator dependency property "

    Public Shared ReadOnly ShowIndicatorProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(ShowIndicator), GetType(Boolean), GetType(PositionedRangeBase),
        New PropertyMetadata(True))


    <Category("Appearance"), Description("Whether to show part of the track as indicator")>
    Public Property ShowIndicator As Boolean
        Get
            Return CBool(GetValue(ShowIndicatorProperty))
        End Get
        Set(newValue As Boolean)
            SetValue(ShowIndicatorProperty, newValue)
        End Set
    End Property

#End Region


#Region " IsIndicatorMoveable dependency property "

    Public Shared ReadOnly IsIndicatorMoveableProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(IsIndicatorMoveable), GetType(Boolean), GetType(PositionedRangeBase))


    Public Property IsIndicatorMoveable As Boolean
        Get
            Return CType(GetValue(IsIndicatorMoveableProperty), Boolean)
        End Get
        Set(newValue As Boolean)
            SetValue(IsIndicatorMoveableProperty, newValue)
        End Set
    End Property

#End Region


#Region " TrackOverlay dependency property "

    Public Shared ReadOnly TrackOverlayProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(TrackOverlay), GetType(DataTemplate), GetType(PositionedRangeBase))


    <Category("Appearance"), Description("Layed over track (which is a Grid)")>
    Public Property TrackOverlay As DataTemplate
        Get
            Return CType(GetValue(TrackOverlayProperty), DataTemplate)
        End Get
        Set(newValue As DataTemplate)
            SetValue(TrackOverlayProperty, newValue)
        End Set
    End Property

#End Region


#Region " IndicatorOverlay dependency property "

    Public Shared ReadOnly IndicatorOverlayProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(IndicatorOverlay), GetType(DataTemplate), GetType(PositionedRangeBase))


    <Category("Appearance"), Description("Layed over indicator (which is a Grid)")>
    Public Property IndicatorOverlay As DataTemplate
        Get
            Return CType(GetValue(IndicatorOverlayProperty), DataTemplate)
        End Get
        Set(newValue As DataTemplate)
            SetValue(IndicatorOverlayProperty, newValue)
        End Set
    End Property

#End Region


#Region " AfterIndicatorOverlay dependency property "

    Public Shared ReadOnly AfterIndicatorOverlayProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(AfterIndicatorOverlay), GetType(DataTemplate), GetType(PositionedRangeBase))


    <Category("Appearance"), Description("Layed over right after indicator")>
    Public Property AfterIndicatorOverlay As DataTemplate
        Get
            Return CType(GetValue(AfterIndicatorOverlayProperty), DataTemplate)
        End Get
        Set(newValue As DataTemplate)
            SetValue(AfterIndicatorOverlayProperty, newValue)
        End Set
    End Property

#End Region


#Region " MouseOverToolTip dependency property "

    Public Shared ReadOnly MouseOverToolTipProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(MouseOverToolTip), GetType(String), GetType(PositionedRangeBase))


    <Category("Appearance"), Description("What tooltip to show when the mouse is moving over the track")>
    Public Property MouseOverToolTip As String
        Get
            Return CStr(GetValue(MouseOverToolTipProperty))
        End Get
        Set(newValue As String)
            SetValue(MouseOverToolTipProperty, newValue)
        End Set
    End Property

#End Region


#Region " Init and clean-up "

    Public Overrides Sub OnApplyTemplate()
        MyBase.OnApplyTemplate()
        mTrack = CType(Template.FindName("PART_Track", Me), FrameworkElement)
        mIndicator = CType(Template.FindName("PART_Indicator", Me), FrameworkElement)
        ValueChangedHandler()
    End Sub

#End Region


#Region " ValueChanged handler "

    Private Sub ValueChangedHandler() Handles Me.ValueChanged, Me.SizeChanged
        If mIndicator Is Nothing OrElse mTrack Is Nothing Then Return

        Dim relPos = (Value - Minimum) / (Maximum - Minimum)
        mIndicator.Width = relPos * mTrack.ActualWidth
    End Sub

#End Region


#Region " Mouse events "

    Private mButtonPressed As Boolean


    ''' <summary>
    ''' Where the mouse tracking has started; needed for ToolTip to follow the mouse.
    ''' </summary>
    Private mMouseEnterCoords As Point


    ''' <summary>
    ''' Last opened tooltip.
    ''' </summary>
    Private mLastToolTip As ToolTip


    ''' <summary>
    ''' Set original tooltip coordinates.
    ''' </summary>
    Private Sub SetTooltipOrigin(args As MouseEventArgs)
        mMouseEnterCoords = args.GetPosition(Me)
    End Sub


    ''' <summary>
    ''' Show or hide the tooltip for the event's OriginalSource.
    ''' </summary>
    ''' <remarks>The ToolTip must be an explicitly created ToolTip object.</remarks>
    Private Sub ShowElementTooltip(args As MouseEventArgs, isShown As Boolean)
        Dim tt = TryCast(TryCast(args.OriginalSource, FrameworkElement).ToolTip, ToolTip)
        If tt IsNot Nothing AndAlso isShown Then
            mLastToolTip = tt
            Dim mouseCoord = args.GetPosition(Me) - mMouseEnterCoords
            tt.IsOpen = True
            tt.HorizontalOffset = mouseCoord.X
            tt.VerticalOffset = mouseCoord.Y
        ElseIf Not isShown Then
            If tt Is Nothing Then tt = mLastToolTip
            If tt IsNot Nothing Then tt.IsOpen = False
        End If
    End Sub


    ''' <summary>
    ''' If the mouse button is still depressed, track position.
    ''' </summary>
    Private Sub MouseMoveHandler(sender As Object, args As MouseEventArgs) Handles Me.MouseMove
        Dim res = TrackMousePosition(sender, args)
        MouseOverValue = res
        ShowElementTooltip(args, True)

        If mButtonPressed Then
            ProgressValueChanged(res)
        End If

        args.Handled = True
    End Sub


    ''' <summary>
    ''' Mouse entered - remember the enter coordinates.
    ''' </summary>
    Private Sub MouseEnterHandler(sender As Object, args As MouseEventArgs) Handles Me.MouseEnter
        SetTooltipOrigin(args)
        args.Handled = True
    End Sub


    ''' <summary>
    ''' No more tracking.
    ''' </summary>
    Private Sub MouseLeaveHandler(sender As Object, args As MouseEventArgs) Handles Me.MouseLeave, Me.MouseUp
        mButtonPressed = False
        ShowElementTooltip(args, False)
        args.Handled = True
    End Sub


    ''' <summary>
    ''' Change position based on the mouse click.
    ''' </summary>
    ''' <remarks>Not possible if the control is not enabled</remarks>
    Private Sub MouseDownHandler(sender As Object, args As MouseButtonEventArgs) Handles Me.MouseDown
        If Not IsIndicatorMoveable Then Return

        ' Right-click: set default
        If args.ChangedButton = MouseButton.Right AndAlso args.ButtonState = MouseButtonState.Pressed Then
            Value = DefaultValue
            Return
        End If

        mButtonPressed = True
        Dim res = TrackMousePosition(sender, args)
        MouseOverValue = res
        ProgressValueChanged(res)
        ShowElementTooltip(args, True)
    End Sub


    ''' <summary>
    ''' When applicable, calculate the mouse position and convert to progress
    ''' between 0 and Maximum.
    ''' </summary>
    Private Function TrackMousePosition(sender As Object, args As MouseEventArgs) As Double
        Dim hitPoint = args.MouseDevice.GetPosition(mTrack)
        Dim hitCoord = If(Orientation = Controls.Orientation.Horizontal, hitPoint.X, hitPoint.Y)
        Dim maxCoord = If(Orientation = Controls.Orientation.Horizontal, mTrack.ActualWidth, mTrack.ActualHeight)
        If maxCoord = 0 Then Return 0

        Dim relCoord = hitCoord / maxCoord

        Return (Maximum - Minimum) * relCoord + Minimum
    End Function


    ''' <summary>
    ''' Called with progress Minimum..Maximum, when the mouse clicks.
    ''' </summary>
    Protected MustOverride Sub ProgressValueChanged(progress As Double)

#End Region

End Class
