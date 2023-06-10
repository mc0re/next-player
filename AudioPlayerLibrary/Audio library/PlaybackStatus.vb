Imports Common

Public Class PlaybackStatus

#Region " Fields "

    Private ReadOnly mTimeService As ITimeService

#End Region


#Region " Simple properties "

    ''' <summary>
    ''' Whether any sound is being played or expected to be played (on pause or awaiting a trigger).
    ''' </summary>
    Public Property IsActive As Boolean


    ''' <summary>
    ''' Whether any action is being played.
    ''' </summary>
    Public Property IsPlaying As Boolean


    ''' <summary>
    ''' Whether the playback is temporarily stalled.
    ''' </summary>
    Public Property IsManuallyPaused As Boolean


    ''' <summary>
    ''' Time since the the playback has been started [milliseconds].
    ''' If he playback is paused, keeps its value.
    ''' </summary>
    Public Property PlaylistTime As Double


    ''' <summary>
    ''' Last (possibly active) producer on the main line.
    ''' Used for replay.
    ''' </summary>
    Public Property LastMainProducer As ISoundProducer


    ''' <summary>
    ''' Action to start on the Next command.
    ''' </summary>
    Public Property NextAction As IPlayerAction


    ''' <summary>
    ''' Starts with the timer, so StartTime is 00:00.
    ''' </summary>
    Public ReadOnly ActiveTimeAnchor As New TriggerReference(False)


    ''' <summary>
    ''' Follows the real time, so StartTime is Now.
    ''' </summary>
    Public ReadOnly WallTimeAnchor As New TriggerReference(True)

#End Region


#Region " ActiveParallels property "

    Private mActiveParallels As IList(Of IPlayerAction) = New List(Of IPlayerAction)


    ''' <summary>
    ''' A list of active parallel actions.
    ''' </summary>
    Public Property ActiveParallels As IList(Of IPlayerAction)
        Get
            Return mActiveParallels
        End Get
        Set(value As IList(Of IPlayerAction))
            mActiveParallels = If(value, New List(Of IPlayerAction))
        End Set
    End Property

#End Region


#Region " ActiveMainProducer read-only property "

    Private mActiveMainProducer As ISoundProducer


    ''' <summary>
    ''' Last started and still running main-line producer action.
    ''' </summary>
    Public Property ActiveMainProducer As ISoundProducer
        Get
            Return mActiveMainProducer
        End Get
        Private Set(value As ISoundProducer)
            mActiveMainProducer = value
        End Set
    End Property

#End Region


#Region " ActiveMainAction read-only property "

    Private mActiveMainAction As IPlayerAction


    ''' <summary>
    ''' Last started and still running main-line action (producer or automation).
    ''' </summary>
    Public Property ActiveMainAction As IPlayerAction
        Get
            Return mActiveMainAction
        End Get
        Private Set(value As IPlayerAction)
            mActiveMainAction = value
        End Set
    End Property

#End Region


#Region " Init and clean-up "

    Public Sub New()
        mTimeService = InterfaceMapper.GetImplementation(Of ITimeService)()
    End Sub

#End Region


#Region " API "

    ''' <summary>
    ''' Reset the whole playback
    ''' </summary>
    Public Sub Reset()
        IsActive = False
        IsPlaying = False
        IsManuallyPaused = False

        ActiveMainProducer = Nothing
        ActiveMainAction = Nothing
        LastMainProducer = Nothing
        NextAction = Nothing
        ActiveParallels.Clear()

        PlaylistTime = 0
        ActiveTimeAnchor.ResetAnchor()
        ActiveTimeAnchor.SetTimeAnchor(0)

        ' Wall time starts at midnight
        WallTimeAnchor.ResetAnchor()
        Dim curMs = mTimeService.GetCurrentTime()
        Dim dayStart = mTimeService.GetTime(mTimeService.ToDateTime(curMs).Date)
        WallTimeAnchor.SetTimeAnchor(dayStart)
    End Sub


    ''' <summary>
    ''' Set a new active main line producer.
    ''' </summary>
    Public Sub SetMainProducer(producer As ISoundProducer)
        If producer IsNot Nothing AndAlso Not producer.Equals(ActiveMainAction) Then
            LastMainProducer = producer
        End If

        ActiveMainProducer = producer

        If producer IsNot Nothing Then
            NextAction = producer.NextAction
        End If

        SetMainAction(producer, False)
    End Sub


    ''' <summary>
    ''' Set a new active main line action.
    ''' </summary>
    ''' <param name="overwrite">When true, overwrites the data; when False, only writes the data if was empty</param>
    Public Sub SetMainAction(act As IPlayerAction, overwrite As Boolean)
        If overwrite OrElse ActiveMainAction Is Nothing Then
            ActiveMainAction = act

            If act IsNot Nothing Then
                NextAction = act.NextAction
            End If
        End If
    End Sub

#End Region

End Class
