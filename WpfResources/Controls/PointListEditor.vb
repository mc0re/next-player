Imports System.ComponentModel
Imports System.Globalization
Imports Common
Imports PlayerActions


''' <summary>
''' Editing surface for the automation points.
''' </summary>
Public Class PointListEditor
    Inherits ItemsControl

#Region " Duration calculator "

    Private Shared mMaxPosConv As New MaxPositionConverter()

#End Region


#Region " HasDuration dependency property "

    Public Shared ReadOnly HasDurationProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(HasDuration), GetType(Boolean), GetType(PointListEditor),
        New PropertyMetadata(False, New PropertyChangedCallback(AddressOf DrawingDurationPropertyDependencyChanged)))


    <Category("Common Properties"), Description("Whether the duration is defined (if not, use maximum value of points).")>
    Public Property HasDuration As Boolean
        Get
            Return CBool(GetValue(HasDurationProperty))
        End Get
        Set(value As Boolean)
            SetValue(HasDurationProperty, value)
        End Set
    End Property

#End Region


#Region " Duration dependency property "

    Public Shared ReadOnly DurationProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(Duration), GetType(TimeSpan), GetType(PointListEditor),
        New PropertyMetadata(InterfaceMapper.GetImplementation(Of IEffectDurationConfiguration)().DefaultDuration,
                             New PropertyChangedCallback(AddressOf DrawingDurationPropertyDependencyChanged)))


    <Category("Common Properties"), Description("Defined duration.")>
    Public Property Duration As TimeSpan
        Get
            Return CType(GetValue(DurationProperty), TimeSpan)
        End Get
        Set(value As TimeSpan)
            SetValue(DurationProperty, value)
        End Set
    End Property

#End Region


#Region " NoDurationFactor dependency property "

    Public Shared ReadOnly NoDurationFactorProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(NoDurationFactor), GetType(Double), GetType(PointListEditor),
        New PropertyMetadata(1.0, New PropertyChangedCallback(AddressOf DrawingDurationPropertyDependencyChanged)))


    <Category("Common Properties"), Description("Defined duration.")>
    Public Property NoDurationFactor As Double
        Get
            Return CDbl(GetValue(NoDurationFactorProperty))
        End Get
        Set(value As Double)
            SetValue(NoDurationFactorProperty, value)
        End Set
    End Property

#End Region


#Region " DrawingDuration read-only dependency property "

    Private Shared DrawingDurationPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(DrawingDuration), GetType(Double), GetType(PointListEditor),
        New PropertyMetadata(InterfaceMapper.GetImplementation(Of IEffectDurationConfiguration)().DefaultDuration.TotalMilliseconds))


    Public Shared DrawingDurationProperty As DependencyProperty = DrawingDurationPropertyKey.DependencyProperty


    <Category("Appearance"), Description("The duration used to draw and scale, milliseconds")>
    Public Property DrawingDuration As Double
        Get
            Return CType(GetValue(DrawingDurationProperty), Double)
        End Get
        Private Set(value As Double)
            SetValue(DrawingDurationPropertyKey, value)
        End Set
    End Property


    ''' <summary>
    ''' Start calculation from DependencyProperty changed event.
    ''' </summary>
    Private Shared Sub DrawingDurationPropertyDependencyChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
        Dim this = CType(obj, PointListEditor)
        this.CalculateDrawingDuration()
    End Sub


    ''' <summary>
    ''' Actual calculation of the duration.
    ''' </summary>
    Private Sub CalculateDrawingDuration()
        Dim points = TryCast(ItemsSource, AutomationPointCollection)

        mMaxPosConv.DefaultMaximum = InterfaceMapper.GetImplementation(Of IEffectDurationConfiguration)().
            DefaultDuration.TotalMilliseconds

        Dim lastXobj = mMaxPosConv.Convert(
            {points, HasDuration, Duration, NoDurationFactor},
            GetType(Double), Nothing, CultureInfo.InvariantCulture)

        If lastXobj Is Binding.DoNothing Then
            ' This covers all 
            Return
        End If

        DrawingDuration = CDbl(lastXobj)
    End Sub

#End Region


#Region " Init and clean-up "

    Shared Sub New()
        ItemsSourceProperty.OverrideMetadata(GetType(PointListEditor),
            New FrameworkPropertyMetadata(New PropertyChangedCallback(AddressOf DrawingDurationPropertyDependencyChanged)))
    End Sub


    Private Sub InitHandler() Handles Me.Initialized
        Me.AddHandler(RelativePositionContainer.RemovePointEvent, New RoutedEventHandler(AddressOf RemovePointHandler))
    End Sub

#End Region


#Region " ItemsControl overrides "

    Protected Overrides Function GetContainerForItemOverride() As DependencyObject
        Return New RelativePositionContainer()
    End Function


    Protected Overrides Function IsItemItsOwnContainerOverride(item As Object) As Boolean
        Return TypeOf item Is RelativePositionContainer
    End Function

#End Region


#Region " Mouse events handlers: add, move, remove "

    Private mMoveOngoing As Boolean = False


    Private mMovingAutoPoint As AutomationPoint


    Private mMouseEnterCoords As Point


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
        If tt IsNot Nothing Then
            If isShown Then
                Dim mouseCoord = args.GetPosition(Me) - mMouseEnterCoords
                tt.IsOpen = True
                tt.HorizontalOffset = mouseCoord.X
                tt.VerticalOffset = mouseCoord.Y
            Else
                tt.IsOpen = False
            End If
        End If
    End Sub


    ''' <summary>
    ''' Click on an empty place results in adding an element.
    ''' Click on an existing element starts moving.
    ''' </summary>
    Private Sub MouseLeftDownHandler(sender As Object, args As MouseButtonEventArgs) Handles Me.MouseLeftButtonDown
        Dim curPos = args.GetPosition(Me)
        Dim elem = FindVisualParent(Of RelativePositionContainer)(CType(args.OriginalSource, DependencyObject))

        If elem Is Nothing Then
            ' Add a new point
            mMovingAutoPoint = AddPoint(curPos)
        Else
            Dim curAutoPoint = TryCast(elem.DataContext, AutomationPoint)
            If curAutoPoint Is Nothing Then Return
            mMovingAutoPoint = curAutoPoint
        End If

        SetTooltipOrigin(args)
        ShowElementTooltip(args, True)
        mMoveOngoing = True
        args.Handled = True
    End Sub


    Private Sub MouseMoveHandler(sender As Object, args As MouseEventArgs) Handles Me.MouseMove
        If Not mMoveOngoing Then Return

        ShowElementTooltip(args, True)
        Dim curPos = args.GetPosition(Me)
        mMovingAutoPoint.X = ConvertCanvasXToPoint(curPos.X)
        mMovingAutoPoint.Y = CSng(ConvertCanvasYToPoint(curPos.Y))
        args.Handled = True
    End Sub


    Private Sub MouseLeftUpHandler(sender As Object, args As MouseButtonEventArgs) Handles Me.MouseLeftButtonUp
        If Not mMoveOngoing Then Return
        mMoveOngoing = False

        ShowElementTooltip(args, False)
        Dim curPos = args.GetPosition(Me)
        mMovingAutoPoint.X = ConvertCanvasXToPoint(curPos.X)
        mMovingAutoPoint.Y = CSng(ConvertCanvasYToPoint(curPos.Y))
        CalculateDrawingDuration()
        args.Handled = True
    End Sub


    ''' <summary>
    ''' Mouse entered - remember the enter coordinates.
    ''' </summary>
    Private Sub MouseEnterHandler(sender As Object, args As MouseEventArgs) Handles Me.MouseEnter
        SetTooltipOrigin(args)
        args.Handled = True
    End Sub


    Private Sub MouseLeaveHandler(sender As Object, args As MouseEventArgs) Handles Me.MouseLeave
        mMoveOngoing = False
        ShowElementTooltip(args, False)
        args.Handled = True
    End Sub


    ''' <summary>
    ''' Handles RemovePoint event sent from the container.
    ''' </summary>
    Private Sub RemovePointHandler(sender As Object, args As RoutedEventArgs)
        Dim targs = CType(args, RemovePointEventArgs)
        Dim points = TryCast(ItemsSource, AutomationPointCollection)
        points.Remove(targs.PointToRemove)
        targs.Handled = True
    End Sub

#End Region


#Region " Utility "

    ''' <summary>
    ''' Convert canvas coordinate to point coordinate.
    ''' </summary>
    ''' <param name="canvasCoord">Coordinate on the canvas</param>
    ''' <param name="canvasSize">Maximum coordinate on the canvas</param>
    ''' <param name="outputSize">Maximum output coordinate</param>
    ''' <param name="precision">The number of decimal points in the result</param>
    ''' <returns>Converter and rounded coordinate</returns>
    Private Shared Function ConvertCanvasToPoint(
        canvasCoord As Double, canvasSize As Double,
        outputSize As Double, precision As Integer
    ) As Double

        ' Limit by the given borders
        If canvasCoord > canvasSize Then
            canvasCoord = canvasSize
        ElseIf canvasCoord < 0 Then
            canvasCoord = 0
        End If

        ' Relative conversion
        Dim outCoord = canvasCoord / canvasSize * outputSize

        ' Rounding
        If precision < 0 Then
            Dim scaled = outCoord / Math.Pow(10, -precision)
            Return Math.Round(scaled) * Math.Pow(10, -precision)
        Else
            Return Math.Round(outCoord, precision)
        End If
    End Function


    ''' <summary>
    ''' A set of predefined arguments for X coordinate conversion.
    ''' </summary>
    Private Function ConvertCanvasXToPoint(canvasX As Double) As Double
        Return ConvertCanvasToPoint(canvasX, ActualWidth, DrawingDuration, -1)
    End Function


    ''' <summary>
    ''' A set of predefined arguments for Y coordinate conversion.
    ''' </summary>
    Private Function ConvertCanvasYToPoint(canvasY As Double) As Double
        Return 1 - ConvertCanvasToPoint(canvasY, ActualHeight, 1, 2)
    End Function


    ''' <summary>
    ''' Add a new automation point at the given coordinates.
    ''' </summary>
    ''' <remarks>
    ''' The coordinates are rounded to integer milliseconds and 0.00 for volume.
    ''' The Y coordinate is reversed.
    ''' </remarks>
    Private Function AddPoint(coords As Point) As AutomationPoint
        Dim points = TryCast(ItemsSource, AutomationPointCollection)
        Dim relX = ConvertCanvasXToPoint(coords.X)
        Dim relY = CSng(ConvertCanvasYToPoint(coords.Y))
        Dim ap = New AutomationPoint(relX, relY)
        points.Add(ap)
        Return ap
    End Function

#End Region

End Class
