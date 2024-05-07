Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Xml.Serialization
Imports AudioPlayerLibrary
Imports Common


''' <summary>
''' An automation base class, regulating a parameter over time.
''' </summary>
<Serializable>
<XmlInclude(GetType(PlayerActionPanningAutomation))>
<XmlInclude(GetType(PlayerActionEffect))>
Public MustInherit Class PlayerActionAutomation
    Inherits PlayerAction
    Implements ISoundAutomation

#Region " OperationType notifying property "

    Private mOperationType As EffectOperationTypes = EffectOperationTypes.ChainMultiply


    ''' <summary>
    ''' Effect type.
    ''' </summary>
    Public Property OperationType As EffectOperationTypes
        Get
            Return mOperationType
        End Get
        Set(value As EffectOperationTypes)
            mOperationType = value
            RaisePropertyChanged(Function() OperationType)
        End Set
    End Property


#End Region


#Region " EffectTargetMain notifying property "

    Private mEffectTargetMain As EffectTargets = EffectTargets.All


    ''' <summary>
    ''' Effect target for main line.
    ''' </summary>
    <AffectsStructure>
    <Description("Which action(s) this effect is applied for in the main line.")>
    Public Property EffectTargetMain As EffectTargets Implements ISoundAutomation.EffectTargetMain
        Get
            Return mEffectTargetMain
        End Get
        Set(value As EffectTargets)
            mEffectTargetMain = value
            RaisePropertyChanged(Function() EffectTargetMain)
        End Set
    End Property


#End Region


#Region " EffectTargetParallel notifying property "

    Private mEffectTargetParallel As EffectTargets = EffectTargets.All


    ''' <summary>
    ''' Effect target for parallel line.
    ''' </summary>
    <AffectsStructure>
    <Description("Which action(s) this effect is applied for in the parallel line.")>
    Public Property EffectTargetParallel As EffectTargets Implements ISoundAutomation.EffectTargetParallel
        Get
            Return mEffectTargetParallel
        End Get
        Set(value As EffectTargets)
            mEffectTargetParallel = value
            RaisePropertyChanged(Function() EffectTargetParallel)
        End Set
    End Property


#End Region


#Region " TargetList non-persistent notifying property "

    Private WithEvents mTargetList As New ObservableCollection(Of ISoundProducer)


    <Description("Which actions this effect is applied for. Regulated by EffectTargetMain / Parallel.")>
    <XmlIgnore()>
    Public Property TargetList As IList(Of ISoundProducer) Implements ISoundAutomation.TargetList
        Get
            Return mTargetList
        End Get
        Set(value As IList(Of ISoundProducer))
            mTargetList.Clear()

            If value IsNot Nothing Then
                For Each item In value
                    mTargetList.Add(item)
                Next
            End If

            RaisePropertyChanged(Function() TargetList)
        End Set
    End Property


    ''' <summary>
    ''' When the collection is changed during playback, check,
    ''' whether the "main timeline" has changed.
    ''' </summary>
    Private Sub TargetListChanged(sender As Object, args As NotifyCollectionChangedEventArgs) Handles mTargetList.CollectionChanged
        If IsPlaying Then
            AssignTimeline()
        End If

        RaisePropertyChanged(Function() TargetList)
    End Sub

#End Region


#Region " TimelineTarget non-persistent read-only property "

    Private mTimelineTarget As ISoundProducer


    ''' <summary>
    ''' Reference sound producer, to which effect's timeline is synchronized.
    ''' </summary>
    <XmlIgnore()>
    <IgnoreForReport()>
    Public Property TimelineTarget As ISoundProducer
        Get
            Return mTimelineTarget
        End Get
        Private Set(value As ISoundProducer)
            If mTimelineTarget Is value Then Return

            mTimelineTarget = value

            If value IsNot Nothing Then
                TimelineStart = value.PlayPosition - PlayPosition
            End If
        End Set
    End Property

#End Region


#Region " TimelineStart non-persistent read-only property "

    Private mTimelineStart As TimeSpan


    ''' <summary>
    ''' To calculate the current position, a reference point to a producer is needed.
    ''' </summary>
    <XmlIgnore()>
    <IgnoreForReport()>
    Public Property TimelineStart As TimeSpan
        Get
            Return mTimelineStart
        End Get
        Set(value As TimeSpan)
            mTimelineStart = value
        End Set
    End Property

#End Region


#Region " PlayPosition-related functionality "

    ''' <summary>
    ''' Calculate current position and apply the effect.
    ''' </summary>
    Public Overrides Sub UpdatePlayPosition()
        If TimelineTarget Is Nothing Then
            AssignTimeline()
        End If

        If TimelineTarget Is Nothing OrElse Not TimelineTarget.IsPlaying Then
            MyBase.Stop(True)
            SetPlayPosition(TimeSpan.Zero)
            Return
        End If

        Dim relPos = TimelineTarget.PlayPosition - TimelineStart
        SetPlayPosition(relPos)
        ApplyEffect(OperationType)
    End Sub

#End Region


#Region " PlayerAction overrides "

    ''' <summary>
    ''' Check, which producer is the "main timeline".
    ''' </summary>
    ''' <returns>True if the timeline was found, False otherwise</returns>
    ''' <remarks>The one playing longest from now is the main timeline.</remarks>
    Private Function AssignTimeline() As Boolean
        Dim targets = TargetList.ToList()
        Dim tline = If((
            From target In targets
            Where target.IsPlaying AndAlso target.ExecutionType <> ExecutionTypes.Parallel
            Order By target.Duration - target.PlayPosition Descending
        ).FirstOrDefault(),
        (
            From target In targets
            Where target.IsPlaying AndAlso target.ExecutionType = ExecutionTypes.Parallel
            Order By target.Duration - target.PlayPosition Descending
        ).FirstOrDefault())

        TimelineTarget = tline
        Return TimelineTarget IsNot Nothing
    End Function


    ''' <summary>
    ''' Find main timeline, assign reference point.
    ''' </summary>
    Public Overrides Sub Start()
        If AssignTimeline() Then
            TimelineStart = TimelineTarget.PlayPosition
            ApplyEffect(OperationType)
            MyBase.Start()
        End If
    End Sub


    ''' <summary>
    ''' When actively stopped, remove the effect.
    ''' </summary>
    Public Overrides Sub [Stop](intendedResume As Boolean)
        MyBase.Stop(intendedResume)

        If Not intendedResume Then
            ApplyEffect(EffectOperationTypes.Bypass)
        End If
    End Sub

#End Region


#Region " Effect functionality "

    ''' <summary>
    ''' Set targets' volume to the interpolated value.
    ''' </summary>
    Protected MustOverride Sub ApplyEffect(op As EffectOperationTypes)

#End Region


#Region " Utility "

    ''' <summary>
    ''' Get interpolated value.
    ''' </summary>
    Protected Function GetAnimationValue(pos As Double, pointList As AutomationPointCollection) As Double
        Dim prevSet = False
        Dim prevPt As AutomationPoint = pointList.FirstOrDefault()
        Dim nextSet = False
        Dim nextPt As AutomationPoint = pointList.LastOrDefault()

        For Each pt In pointList
            If pt.X > pos Then
                nextPt = pt
                nextSet = True
                Exit For
            End If

            prevPt = pt
            prevSet = True
        Next

        If Not prevSet Then
            prevPt = New AutomationPoint(pos, If(pointList.Any(), prevPt.Y, 1))
        End If

        If Not nextSet Then
            Return prevPt.Y
        ElseIf prevPt.X = nextPt.X Then
            Return prevPt.Y
        Else
            Return (nextPt.Y - prevPt.Y) / (nextPt.X - prevPt.X) * (pos - prevPt.X) + prevPt.Y
        End If
    End Function

#End Region

End Class
