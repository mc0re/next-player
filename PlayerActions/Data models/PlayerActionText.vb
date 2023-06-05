﻿Imports System.Windows.Threading
Imports System.Xml.Serialization
Imports AudioPlayerLibrary
Imports Common
Imports TextChannelLibrary


''' <summary>
''' Show text in a prepared window.
''' </summary>
Public Class PlayerActionText
    Inherits PlayerAction

#Region " Text notifying property "

    Private mText As String


    ''' <summary>
    ''' Text to show.
    ''' </summary>
    Public Property Text As String
        Get
            Return mText
        End Get
        Set(value As String)
            mText = value
            RaisePropertyChanged(Function() Text)
        End Set
    End Property

#End Region


#Region " Channel notifying property "

    Private mChannel As Integer = 1


    ''' <summary>
    ''' Channel to use, 1-based.
    ''' </summary>
    Public Property Channel As Integer
        Get
            Return mChannel
        End Get
        Set(value As Integer)
            mChannel = value
            RaisePropertyChanged(Function() Channel)
        End Set
    End Property

#End Region


#Region " Scroll notifying property "

    Private mScroll As Boolean


    ''' <summary>
    ''' Whether to scroll the contents if does not fit.
    ''' The scroll direction is defined by each physical channel separately.
    ''' </summary>
    Public Property Scroll As Boolean
        Get
            Return mScroll
        End Get
        Set(value As Boolean)
            mScroll = value
            RaisePropertyChanged(Function() Scroll)
        End Set
    End Property

#End Region


#Region " ScrollStart notifying property "

    Private mScrollStart As TimeSpan


    ''' <summary>
    ''' If <see cref="Scroll"/> is set, how long should we wait
    ''' before star scrolling the text.
    ''' </summary>
    <XmlIgnore, SerializedAs>
    Public Property ScrollStart As TimeSpan
        Get
            Return mScrollStart
        End Get
        Set(value As TimeSpan)
            mScrollStart = value
            RaisePropertyChanged(Function() ScrollStart)
        End Set
    End Property


    ''' <summary>
    ''' If <see cref="Scroll"/> is set, how long should we wait
    ''' before star scrolling the text.
    ''' </summary>
    ''' <remarks>To work around inability to serialize TimeSpan.</remarks>
    <XmlElement(NameOf(ScrollStart))>
    <IgnoreForReport()>
    Public Property ScrollStartSerialized As Double
        Get
            Return ScrollStart.TotalMilliseconds()
        End Get
        Set(value As Double)
            ScrollStart = TimeSpan.FromMilliseconds(value)
        End Set
    End Property

#End Region


#Region " ScrollDuration notifying property "

    Private mScrollDuration As TimeSpan


    ''' <summary>
    ''' If <see cref="Scroll"/> is set, how long should it take
    ''' to scroll the entire text.
    ''' </summary>
    <XmlIgnore, SerializedAs>
    Public Property ScrollDuration As TimeSpan
        Get
            Return mScrollDuration
        End Get
        Set(value As TimeSpan)
            mScrollDuration = value
            RaisePropertyChanged(Function() ScrollDuration)
        End Set
    End Property


    ''' <summary>
    ''' If <see cref="Scroll"/> is set, how long should it take
    ''' to scroll the entire text.
    ''' </summary>
    ''' <remarks>To work around inability to serialize TimeSpan.</remarks>
    <XmlElement(NameOf(ScrollDuration))>
    <IgnoreForReport()>
    Public Property ScrollDurationSerialized As Double
        Get
            Return ScrollDuration.TotalMilliseconds()
        End Get
        Set(value As Double)
            ScrollDuration = TimeSpan.FromMilliseconds(value)
        End Set
    End Property

#End Region


#Region " ScrollPosition notifying property "

    Private mScrollPosition As Double


    ''' <summary>
    ''' Scrolling position (0-1).
    ''' Not for storing, playback-only property.
    ''' </summary>
    <XmlIgnore>
    Public Property ScrollPosition As Double
        Get
            Return mScrollPosition
        End Get
        Set(value As Double)
            If value < 0 Then
                value = 0
            ElseIf value > 1 Then
                value = 1
            End If

            If mScrollPosition = value Then Return
            mScrollPosition = value
            RaisePropertyChanged(Function() ScrollPosition)
            Dim storage = InterfaceMapper.GetImplementation(Of ITextChannelStorage)()
            Dim ch = storage.Logical.Channel(Channel)
            ch.SetPosition(value)
        End Set
    End Property

#End Region


#Region " Init and clean-up "

    Public Sub New()
        Name = "Show text"
        HasDuration = True
        ExecutionType = ExecutionTypes.MainContinuePrev
    End Sub

#End Region


#Region " PlayerAction overrides "

    Public Overrides Sub PrepareStart()
        MyBase.PrepareStart()
        ScrollPosition = 0
    End Sub


    Public Overrides Sub Start()
        MyBase.Start()
        Dim storage = InterfaceMapper.GetImplementation(Of ITextChannelStorage)()
        Dim ch = storage.Logical.Channel(Channel)

        ' Avoid occasional Null-reference exceptions
        If ch Is Nothing Then Return

        If String.IsNullOrEmpty(Text) Then
            ch.HideText()
            MyBase.Stop(False)
        Else
            ch.ShowText(Text)

            If Scroll And ScrollDuration.Ticks > 0 Then
                StartPositionTimer()
            Else
                MyBase.Stop(False)
            End If
        End If
    End Sub


    Public Overrides Sub [Stop](intendedResume As Boolean)
        MyBase.Stop(intendedResume)
        StopPositionTimer()

        If Not intendedResume Then
            ScrollPosition = 0
        End If
    End Sub

#End Region


#Region " Additional actions "

    Public Shared Sub Reset()
        Dim chList = InterfaceMapper.GetImplementation(Of ITextChannelStorage)()
        chList.HideAll()
        Dim storage = InterfaceMapper.GetImplementation(Of ITextEnvironmentStorage)()
        storage.HideAll()
    End Sub

#End Region


#Region " Timer "

    ''' <summary>
    ''' Interval between timer ticks [milliseconds]
    ''' </summary>
    Private Const TimerStep As Double = 100

    Private mTimer As DispatcherTimer

    ''' <summary>
    ''' Start delay [milliseconds].
    ''' </summary>
    Private mTimerInitialDelay As Double

    ''' <summary>
    ''' Position increment for every passed millisecond.
    ''' </summary>
    Private mTimerStep As Double

    ''' <summary>
    ''' Previous tick timestamp.
    ''' </summary>
    Private mLastTickTime As DateTime


    Private Sub StartPositionTimer()
        mTimer = New DispatcherTimer With {
            .Interval = TimeSpan.FromMilliseconds(TimerStep)
        }
        AddHandler mTimer.Tick, AddressOf PositionTimerTick
        mTimerInitialDelay = ScrollStart.TotalMilliseconds
        mTimerStep = 1.0 / ScrollDuration.TotalMilliseconds
        mLastTickTime = Date.UtcNow
        mTimer.Start()
    End Sub


    Private Sub StopPositionTimer()
        mTimer?.Stop()
        mTimer = Nothing
    End Sub


    Private Sub PositionTimerTick(sender As Object, e As EventArgs)
        Dim now = Date.UtcNow
        Dim passed = (now - mLastTickTime).TotalMilliseconds
        mLastTickTime = now

        If mTimerInitialDelay > 0 Then
            mTimerInitialDelay -= passed
        Else
            ScrollPosition += mTimerStep * passed

            If ScrollPosition >= 1 Then
                MyBase.Stop(False)
            End If
        End If
    End Sub

#End Region


#Region " ToString "

    Public Overrides Function ToString() As String
        Return String.Format("Text '{0}'", Text)
    End Function

#End Region

End Class
