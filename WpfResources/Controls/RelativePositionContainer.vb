Imports PlayerActions


''' <summary>
''' Definition of event arguments.
''' </summary>
Public Class RemovePointEventArgs
    Inherits RoutedEventArgs

    Public Property PointToRemove As AutomationPoint


    Public Sub New(evt As RoutedEvent)
        MyBase.New(evt)
    End Sub

End Class


''' <summary>
''' Delegate type for handling RemovePoint event.
''' </summary>
Public Delegate Sub RemovePointEventHandler(sender As Object, args As RemovePointEventArgs)


Public Class RelativePositionContainer
    Inherits ContentPresenter

#Region " RemovePoint routed event "

    Public Shared ReadOnly RemovePointEvent As RoutedEvent = EventManager.RegisterRoutedEvent(
        NameOf(RemovePoint), RoutingStrategy.Bubble, GetType(RemovePointEventHandler), GetType(RelativePositionContainer))


    Public Custom Event RemovePoint As RemovePointEventHandler
        AddHandler(ByVal value As RemovePointEventHandler)
            Me.AddHandler(RemovePointEvent, value)
        End AddHandler

        RemoveHandler(ByVal value As RemovePointEventHandler)
            Me.RemoveHandler(RemovePointEvent, value)
        End RemoveHandler

        RaiseEvent(ByVal sender As Object, ByVal e As RemovePointEventArgs)
            Me.RaiseEvent(e)
        End RaiseEvent
    End Event


    Private Sub RaiseRemovePoint()
        Dim insArgs = New RemovePointEventArgs(RemovePointEvent) With {.PointToRemove = CType(DataContext, AutomationPoint)}
        RaiseEvent RemovePoint(Me, insArgs)
    End Sub

#End Region


#Region " MaxInputX attached dependency property "

    Public Shared ReadOnly MaxInputXProperty As DependencyProperty = DependencyProperty.RegisterAttached(
        NameOf(MaxInputX), GetType(Double), GetType(RelativePositionContainer),
        New FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.Inherits))


    Public Shared Sub SetMaxInputX(element As UIElement, value As Double)
        element.SetValue(MaxInputXProperty, value)
    End Sub


    Public Shared Function GetMaxInputX(element As UIElement) As Double
        Return CDbl(element.GetValue(MaxInputXProperty))
    End Function


    Public Property MaxInputX As Double
        Get
            Return CDbl(GetValue(MaxInputXProperty))
        End Get
        Set(value As Double)
            SetValue(MaxInputXProperty, value)
        End Set
    End Property

#End Region


#Region " MaxOutputX attached dependency property "

    Public Shared ReadOnly MaxOutputXProperty As DependencyProperty = DependencyProperty.RegisterAttached(
        NameOf(MaxOutputX), GetType(Double), GetType(RelativePositionContainer),
        New FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.Inherits))


    Public Shared Sub SetMaxOutputX(element As UIElement, value As Double)
        element.SetValue(MaxOutputXProperty, value)
    End Sub


    Public Shared Function GetMaxOutputX(element As UIElement) As Double
        Return CDbl(element.GetValue(MaxOutputXProperty))
    End Function


    Public Property MaxOutputX As Double
        Get
            Return CDbl(GetValue(MaxOutputXProperty))
        End Get
        Set(value As Double)
            SetValue(MaxOutputXProperty, value)
        End Set
    End Property

#End Region


#Region " MaxInputY dependency property "

    Public Shared ReadOnly MaxInputYProperty As DependencyProperty = DependencyProperty.RegisterAttached(
        NameOf(MaxInputY), GetType(Double), GetType(RelativePositionContainer),
        New FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.Inherits))


    Public Shared Sub SetMaxInputY(element As UIElement, value As Double)
        element.SetValue(MaxInputYProperty, value)
    End Sub


    Public Shared Function GetMaxInputY(element As UIElement) As Double
        Return CDbl(element.GetValue(MaxInputYProperty))
    End Function


    Public Property MaxInputY As Double
        Get
            Return CDbl(GetValue(MaxInputYProperty))
        End Get
        Set(value As Double)
            SetValue(MaxInputYProperty, value)
        End Set
    End Property

#End Region


#Region " MaxOutputY dependency property "

    Public Shared ReadOnly MaxOutputYProperty As DependencyProperty = DependencyProperty.RegisterAttached(
        NameOf(MaxOutputY), GetType(Double), GetType(RelativePositionContainer),
        New FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.Inherits))


    Public Shared Sub SetMaxOutputY(element As UIElement, value As Double)
        element.SetValue(MaxOutputYProperty, value)
    End Sub


    Public Shared Function GetMaxOutputY(element As UIElement) As Double
        Return CDbl(element.GetValue(MaxOutputYProperty))
    End Function


    Public Property MaxOutputY As Double
        Get
            Return CDbl(GetValue(MaxOutputYProperty))
        End Get
        Set(value As Double)
            SetValue(MaxOutputYProperty, value)
        End Set
    End Property

#End Region


#Region " Mouse event handlers: remove this item "

    Private Sub MouseRightUpHandler(sender As Object, args As MouseButtonEventArgs) Handles Me.MouseRightButtonUp
        RaiseRemovePoint()
        args.Handled = True
    End Sub

#End Region

End Class
