Imports Common
Imports Serilog


''' <summary>
''' A list of notifications.
''' </summary>
Public Class NotificationCollection

#Region " Fields "

    Private ReadOnly mTimeService As ITimeService

    ''' <summary>
    ''' A list of set triggers - when and what to trigger.
    ''' </summary>
    Private ReadOnly mNotificationList As New List(Of NotificationInfo)

    Private ReadOnly mPlaybackStatus As PlaybackStatus

#End Region


#Region " Uiogger property "

    Private mUiLogger As IMessageLog


    Public ReadOnly Property UiLogger As IMessageLog
        Get
            If mUiLogger Is Nothing Then
                mUiLogger = InterfaceMapper.GetImplementation(Of IMessageLog)(True)
            End If

            Return mUiLogger
        End Get
    End Property

#End Region


#Region " Logger lazy property "

    Private mLogger As ILogger


    Private ReadOnly Property Logger As ILogger
        Get
            If mLogger Is Nothing Then
                mLogger = InterfaceMapper.GetImplementation(Of ILogger)(True)
            End If

            Return mLogger
        End Get
    End Property

#End Region


#Region " Init and clean-up "

    Public Sub New(status As PlaybackStatus)
        mPlaybackStatus = status
        mTimeService = InterfaceMapper.GetImplementation(Of ITimeService)()
    End Sub

#End Region


#Region " API "

    ''' <summary>
    ''' Remove all triggers.
    ''' </summary>
    Public Sub Clear()
        mNotificationList.Clear()
        ReportTriggers()
    End Sub


    ''' <summary>
    ''' Remove all triggers relying on the given action.
    ''' </summary>
    ''' <return>True if the trigger was removed, False if was not found</return>
    Public Sub ClearNotification(toRemove As IPlayerAction)
        Dim delList = (
            From notif In mNotificationList Where notif.Action.Equals(toRemove)
            ).ToList()

        For Each delNotif In delList
            mNotificationList.Remove(delNotif)
            Logger.Information($"Notification for {delNotif.Action.Name} removed from the list upon request.")
        Next

        ReportTriggers()
    End Sub


    ''' <summary>
    ''' Remove all triggers relying on the given action.
    ''' </summary>
    ''' <return>True if the trigger was removed, False if was not found</return>
    Public Sub ClearDependentNotification(toRemove As IPlayerAction)
        Dim delList = (
            From notif In mNotificationList Where notif.Trigger.Action.Equals(toRemove)
            ).ToList()

        For Each delNotif In delList
            mNotificationList.Remove(delNotif)
            Logger.Information($"Notification for '{delNotif.Action.Name}' removed as dependant on '{toRemove.Name}'.")
        Next

        ReportTriggers()
    End Sub


    ''' <summary>
    ''' Remove all passed triggers.
    ''' </summary>
    Public Sub RemoveTriggeredNotifications()
        Dim delList = (
            From notif In mNotificationList Where notif.IsTriggered
            ).ToList()

        For Each delNotif In delList
            mNotificationList.Remove(delNotif)
            Logger.Information($"Notification for {delNotif.Action.Name} removed from the list because it's triggered.")
        Next

        ReportTriggers()
    End Sub


    ''' <summary>
    ''' Check notification list against the current time.
    ''' </summary>
    ''' <remarks>
    ''' Marks future notifications as "not triggered".
    ''' Does not change the notification list.
    ''' </remarks>
    ''' <returns>A list of actions to start at the current moment</returns>
    Public Function CheckNotifications(
        prevWallTick As Double, curWallTick As Double, prevPlayTick As Double, curPlayTick As Double
    ) As ICollection(Of IPlayerAction)

        Dim res = New List(Of IPlayerAction)

        For Each notifInfo In mNotificationList.ToList()
            If Not notifInfo.IsTriggered AndAlso
               notifInfo.IsAfter(prevWallTick, prevPlayTick) AndAlso
               Not notifInfo.IsAfter(curWallTick, curPlayTick) Then

                ' Actual but not yet executed
                If notifInfo.Trigger.IsAbsolute Then
                    Logger.Information($"Trigger '{notifInfo.Action.Name}' activated because wall time is between {prevWallTick} and {curWallTick}, must be {notifInfo.Position}.")
                Else
                    Logger.Information($"Trigger '{notifInfo.Action.Name}' activated because '{notifInfo.Trigger.Action.Name}' position is between {prevPlayTick} and {curPlayTick}, must be {notifInfo.Position}.")
                End If
                notifInfo.IsTriggered = True
                ReportTriggers()
                res.Add(notifInfo.Action)
            End If
        Next

        Return res
    End Function


    ''' <summary>
    ''' Set triggers for automatic playback of the given actions.
    ''' </summary>
    Public Sub SetNotificationTriggers(
        lastWallTick As Double,
        lastPlayTick As Double,
        actions As IEnumerable(Of IPlayerAction))

        For Each act In actions
            SetNotification(lastWallTick, lastPlayTick, act)
        Next
    End Sub


    ''' <summary>
    ''' Recalculate triggers.
    ''' </summary>
    Public Sub RecalculateTriggers(curWallTime As Double, curPlayTime As Double)
        For Each ni In mNotificationList
            If ni.Trigger.IsAbsolute Then
                ni.Trigger.CalculateEndTime(If(ni.IsAbsolute, curWallTime, curPlayTime))
            End If

            ni.UpdatePosition(GetStartTime(ni.Action, ni.Trigger))
        Next

        ReportTriggers()
    End Sub

#End Region


#Region " Utility "

    ''' <summary>
    ''' Set a notification to be fired for the given action to be triggered.
    ''' Do not do that for a manually started actions, and for already playing actions.
    ''' </summary>
    Friend Sub SetNotification(lastWallTick As Double, lastPlayTick As Double, act As IPlayerAction)
        Dim notif = GetTriggerInfo(act)

        ' Only add future triggers
        If notif IsNot Nothing AndAlso notif.IsAfter(lastWallTick, lastPlayTick) Then
            ' Clean-up the list
            RemoveTriggeredNotifications()
            ClearNotification(act)

            mNotificationList.Add(notif)
        End If

        ReportTriggers()
    End Sub


    ''' <summary>
    ''' Establish a trigger reference for the given reference action.
    ''' </summary>
    ''' <param name="refType">Anchor type</param>
    ''' <param name="refAction">
    ''' Action used as an anchor point. There is a number of checks (e.g. for WallClock)
    ''' before this method is called, so we assume it is called with the correct action.
    ''' </param>
    Private Function GetReferencePoint(refType As DelayReferences, refAction As IPlayerAction) As TriggerReference
        Dim refPoint As TriggerReference

        Select Case refType
            Case DelayReferences.MainClock
                refPoint = mPlaybackStatus.ActiveTimeAnchor

            Case DelayReferences.WallClock
                refPoint = mPlaybackStatus.WallTimeAnchor

            Case Else
                refPoint = If(refAction Is Nothing, Nothing, New TriggerReference(refAction))
        End Select

        Return refPoint
    End Function


    ''' <summary>
    ''' Calculate if and when the given action must be triggered.
    ''' </summary>
    ''' <param name="triggeredAction">Action to check</param>
    ''' <returns>If the trigger point is found, its info. Nothing if the action shall not be triggered.</returns>
    Private Function GetTriggerInfo(triggeredAction As IPlayerAction) As NotificationInfo
        ' No trigger for a manual actions
        If triggeredAction.DelayType = DelayTypes.Manual Then Return Nothing

        ' Cannot set a trigger for an already playing actions
        If triggeredAction.IsPlaying Then Return Nothing

        Dim refPoint = GetReferencePoint(triggeredAction.DelayReference, triggeredAction.ReferenceAction)

        ' The reference point must be valid
        If refPoint Is Nothing Then Return Nothing

        ' The referenced action must be started
        If Not refPoint.IsStarted Then Return Nothing

        ' Check request validity
        If Not refPoint.HasDuration AndAlso triggeredAction.DelayType = DelayTypes.TimedBeforeEnd Then
            UiLogger?.LogTriggerMessage("Trying to set before-end-trigger with no end time for '{0}'",
                                      triggeredAction)
            Return Nothing
        End If

        If Not refPoint.HasDuration AndAlso triggeredAction.DelayType = DelayTypes.TimedAfterEnd Then
            UiLogger?.LogTriggerMessage("Trying to set after-end-trigger with no end time for '{0}'",
                                      triggeredAction)
            Return Nothing
        End If

        ' Calculate start time
        Return New NotificationInfo(refPoint,
                                    GetStartTime(triggeredAction, refPoint),
                                    refPoint.IsAbsolute, triggeredAction)
    End Function


    Private Function GetStartTime(
        triggeredAction As IPlayerAction, refPoint As TriggerReference
    ) As Double
        Dim delay = triggeredAction.DelayBefore.TotalMilliseconds()

        Select Case triggeredAction.DelayType
            Case DelayTypes.TimedFromStart
                Return refPoint.StartTime + delay

            Case DelayTypes.TimedBeforeEnd
                Return refPoint.EndTime - delay

            Case DelayTypes.TimedAfterEnd
                Return refPoint.EndTime + delay

            Case Else
                Throw New ArgumentException(String.Format("Unknown delay type {0}", triggeredAction.DelayType))
        End Select
    End Function


    ''' <summary>
    ''' Report triger status to the logger.
    ''' </summary>
    ''' <remarks>
    ''' List all not yet started triggers.
    ''' </remarks>
    Private Sub ReportTriggers()
        Dim seq =
            From ni In mNotificationList
            Where Not ni.IsTriggered
            Let tm = mTimeService.ToDateTime(ni.Position)
            Order By tm
            Select New TriggerSummary With {
                .NextAction = ni.Action.Name,
                .NextTime = tm,
                .IsAbsolute = ni.IsAbsolute
            }

        UiLogger?.LogTriggerInfo(seq.ToList())
    End Sub

#End Region

End Class
