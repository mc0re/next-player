Imports System.Runtime.Remoting.Contexts
Imports Common


''' <summary>
''' A set of methods handling playlist mutations during the playback,
''' on a level "command -> list of actions".
''' </summary>
''' <remarks>Must be protected against multi-threading.</remarks>
<Synchronization>
Public Class AudioPlaybackLibrary
    Inherits ContextBoundObject

#Region " Fields "

    ''' <summary>
    ''' A list of all actions to be played.
    ''' </summary>
    ''' <remarks>
    ''' The action list actually belongs to the UI thread.
    ''' We can only read item properties, and cannot modify.
    ''' </remarks>
    Private mActionList As IReadOnlyList(Of IPlayerAction) = New List(Of IPlayerAction)()


    ''' <summary>
    ''' A list of currently active (playing or paused) actions.
    ''' </summary>
    Private ReadOnly mPlayingList As New List(Of IPlayerAction)()


    ''' <summary>
    ''' A list of auto-generated actions, not to be saved.
    ''' </summary>
    Private ReadOnly mGeneratedList As New List(Of IPlayerAction)()


    ''' <summary>
    ''' To generate fade-in / fade-out effects.
    ''' </summary>
    Private mEffectGenerator As IEffectGenerator


    Private ReadOnly mTimeService As ITimeService


    ''' <summary>
    ''' A list of upcoming triggers.
    ''' </summary>
    Private ReadOnly mNotifications As NotificationCollection


    ''' <summary>
    ''' When was the last time (wall clock) <see cref="AdvancePlaylistTime"/> was called.
    ''' </summary>
    ''' <remarks>
    ''' Used to make sure we do not run the same action twice.
    ''' </remarks>
    Private mLastWallTick As Double


    ''' <summary>
    ''' When was the last time (playlist clock) <see cref="AdvancePlaylistTime"/> was called.
    ''' </summary>
    ''' <remarks>
    ''' Used to make sure we do not run the same action twice.
    ''' </remarks>
    Private mLastPlayTick As Double

#End Region


#Region " PlaybackStatus property "

    ''' <summary>
    ''' The object the library is using for operation.
    ''' </summary>
    Public ReadOnly Property PlaybackStatus As New PlaybackStatus()

#End Region


#Region " Init and clean-up "

    Public Sub New()
        mEffectGenerator = InterfaceMapper.GetImplementation(Of IEffectGenerator)()
        mTimeService = InterfaceMapper.GetImplementation(Of ITimeService)()
        mNotifications = New NotificationCollection(PlaybackStatus)
    End Sub

#End Region


#Region " Set-up API "

    ''' <summary>
    ''' Set the current a playlist.
    ''' </summary>
    ''' <returns>A list of individual stop actions to perform</returns>
    Public Function SetPlaylist(actionList As IReadOnlyList(Of IPlayerAction)) As AudioActions
        mActionList = actionList

        Return Reset()
    End Function


    ''' <summary>
    ''' The playlist structure has changed, update active actions.
    ''' </summary>
    Public Sub SetActiveActions()
        Dim act = PlaybackStatus.ActiveMainAction

        If act Is Nothing Then
            PlaybackStatus.NextAction = PlaylistStructureLibrary.GetFirstAction(mActionList)
        Else
            PlaybackStatus.NextAction = act.NextAction
        End If

        SetParallelActions(act)
    End Sub


    ''' <summary>
    ''' The playlist structure has changed, update active actions.
    ''' </summary>
    Public Sub SetParallelActions(act As IPlayerAction)
        If act Is Nothing Then
            PlaybackStatus.ActiveParallels = PlaylistStructureLibrary.GetGlobalParallels(mActionList)
        Else
            PlaybackStatus.ActiveParallels = act.Parallels.Concat(PlaylistStructureLibrary.GetGlobalParallels(mActionList)).ToList()
        End If
    End Sub


    ''' <summary>
    ''' Reset all properties and internal states.
    ''' </summary>
    ''' <returns>A list of individual stop actions to perform</returns>
    Public Function Reset() As AudioActions
        Dim res = New AudioActions()

        StopAllSounds(res)
        mNotifications.Clear()
        mPlayingList.Clear()
        mGeneratedList.Clear()

        PlaybackStatus.Reset()
        SetActiveActions()

        Return res
    End Function

#End Region


#Region " Playback control API "

    ''' <summary>
    ''' Start playback of the given item as specified in that item.
    ''' </summary>
    ''' <param name="act">
    ''' Play action to start, or Nothing, if there is nothing to play, but we need to wait for a trigger
    ''' </param>
    ''' <returns>A list of individual start/stop actions to perform</returns>
    Public Function Play(act As IPlayerAction) As AudioActions
        Return Play(act, act.ExecutionType)
    End Function


    ''' <summary>
    ''' Start playback of the given item in the given way.
    ''' </summary>
    ''' <param name="act">
    ''' Play action to start, or Nothing, if there is nothing to play, but we need to wait for a trigger
    ''' </param>
    ''' <param name="interruptType">How the start of this item influences the already playing items</param>
    ''' <returns>A list of individual start/stop actions to perform</returns>
    Public Function Play(act As IPlayerAction, interruptType As ExecutionTypes) As AudioActions
        PlaybackStatus.IsManuallyPaused = False

        Dim playActions As AudioActions

        If Not PlaybackStatus.IsActive Then
            PlaybackStatus.Reset()
            SetActiveActions()
            PlaybackStatus.IsActive = True
            mLastWallTick = mTimeService.GetCurrentTime() - 1
            mLastPlayTick = -1
        End If

        If act Is Nothing Then
            playActions = StartWaiting()
        ElseIf act.ExecutionType = ExecutionTypes.Parallel Then
            playActions = PlayParallel(act)
            act.StartTime = PlaybackStatus.PlaylistTime
        Else
            playActions = PlayMain(act, interruptType)
            act.StartTime = PlaybackStatus.PlaylistTime
        End If

        AddTriggers(playActions.GetStartingActions())
        playActions.Merge(GetTriggeredActions())

        VerifyPlayingStatus()

        Return playActions
    End Function


    ''' <summary>
    ''' Stop playback of the given action without affecting other playing actions.
    ''' </summary>
    ''' <returns>A list of individual stop actions to perform</returns>
    Public Function StopSingle(act As IPlayerAction) As AudioActions
        Dim res = New AudioActions()

        res.Stopping.Add(act)
        mPlayingList.Remove(act)
        VerifyPlayingStatus()

        Return res
    End Function


    ''' <summary>
    ''' Pause all main playbacks. Stop and remove parallel playbacks. Pause triggers.
    ''' </summary>
    ''' <returns>A list of individual stop actions to perform</returns>
    Public Function PauseAll() As AudioActions
        PlaybackStatus.IsManuallyPaused = True

        Dim res = New AudioActions()

        For Each act In mPlayingList.ToList()
            If act.ExecutionType = ExecutionTypes.Parallel Then
                res.Stopping.Add(act)
                mPlayingList.Remove(act)
            Else
                res.Pausing.Add(act)
            End If
        Next

        Return res
    End Function


    ''' <summary>
    ''' Resume all main playbacks.
    ''' </summary>
    ''' <returns>A list of individual start actions to perform</returns>
    Public Function ResumeMain() As AudioActions
        PlaybackStatus.IsManuallyPaused = False

        Dim res = New AudioActions()

        For Each act In mPlayingList
            res.Resuming.Add(act)
        Next

        VerifyPlayingStatus()

        Return res
    End Function


    ''' <summary>
    ''' Update positions, check notifications.
    ''' </summary>
    ''' <returns>Time since the playback has started</returns>
    ''' <remarks>Shall be executed in the worker thread.</remarks>
    Public Function AdvancePlaylistTime(timeStep As Double) As TimerTickResult
        ' Update the PlayPosition property and send notifications.
        ' First for automations, then for audio, to make sure the automations are applied.
        Dim playingListCopy = mPlayingList.ToList()

        For Each act In playingListCopy.Where(Function(a) TryCast(a, ISoundProducer) Is Nothing)
            act.UpdatePlayPosition()
        Next

        For Each act In playingListCopy.Where(Function(a) TryCast(a, ISoundProducer) IsNot Nothing)
            act.UpdatePlayPosition()
        Next

        ' Update current time of the main line producer
        PlaybackStatus.PlaylistTime = AdjustPlaybackTime(PlaybackStatus, timeStep)

        ' Check what actions shall be performed now
        Dim actions = GetTriggeredActions()
        VerifyPlayingStatus()

        Return New TimerTickResult With {
            .PlaybackTime = PlaybackStatus.PlaylistTime,
            .HasActions = Not actions.IsEmpty(),
            .Actions = actions
        }
    End Function


    ''' <summary>
    ''' The playback of a certain sound is naturally ended.
    ''' </summary>
    Public Function NotifyEndReached(act As ISoundProducer) As AudioActions
        Dim res As New AudioActions()

        If act Is PlaybackStatus.ActiveMainProducer Then
            Dim producers = From a In mPlayingList.OfType(Of ISoundProducer)()
                            Where a.ExecutionType <> ExecutionTypes.Parallel AndAlso a.IsPlaying
            PlaybackStatus.SetMainProducer(producers.LastOrDefault())

        ElseIf act Is PlaybackStatus.ActiveMainAction Then
            Dim actions = From a In mPlayingList
                          Where a.ExecutionType <> ExecutionTypes.Parallel AndAlso a.IsPlaying
            PlaybackStatus.SetMainAction(actions.LastOrDefault(), True)
        End If

        If Not mGeneratedList.Remove(act) Then
            res.Stopping.Add(act)
        End If

        mPlayingList.Remove(act)

        VerifyPlayingStatus()

        Return res
    End Function


    ''' <summary>
    ''' Remove all notifications relying on the given item.
    ''' </summary>
    Public Sub ClearNotification(item As IPlayerAction)
        mNotifications.ClearNotification(item)
    End Sub


    ''' <summary>
    ''' Recalculate triggers due to play position changes.
    ''' </summary>
    Public Sub RecalculateNotifications()
        Dim curTime = mTimeService.GetCurrentTime()
        mNotifications.RecalculateTriggers(curTime, PlaybackStatus.PlaylistTime)
    End Sub

#End Region


#Region " Structural utility "

    ''' <summary>
    ''' Set IsPlaying, depending on the action.IsPlaying statuses.
    ''' </summary>
    Private Sub VerifyPlayingStatus()
        PlaybackStatus.IsPlaying = (From act In mPlayingList Where act.IsPlaying).Any()
    End Sub

#End Region


#Region " Playback utility "

    ''' <summary>
    ''' Start waiting for the triggers.
    ''' </summary>
    Private Function StartWaiting() As AudioActions
        ' Do nothing, this is a placeholder
        Return New AudioActions()
    End Function


    ''' <summary>
    ''' Prepare for playing back a file or automation on the main line as defined by action.
    ''' </summary>
    ''' <param name="interruptType">
    ''' Defines intervention type. Can be:
    ''' - <see cref="ExecutionTypes.MainStopAll"/> (used for Again command)
    ''' - <see cref="ExecutionTypes.MainStopPrev"/> (used for double-click)
    ''' - <see cref="ExecutionTypes.MainContinuePrev"/> (meaning "goes as planned"),
    ''' but also action's <see cref="IPlayerAction.ExecutionType"/> is taken into account.
    ''' </param>
    ''' <remarks>
    ''' THe action is added to <see cref="mPlayingList"/>, if it is a producer.
    ''' 
    ''' If this is a sound producer, and we're interrupting from UI, stop
    ''' all sounds on the main line.
    ''' 
    ''' If this is an automation, don't stop the producers.
    ''' </remarks>
    Private Function PlayMain(act As IPlayerAction, interruptType As ExecutionTypes) As AudioActions
        Dim res = New AudioActions()

        If Not act.CanExecute Then Return res

        Dim asProducer = TryCast(act, ISoundProducer)
        Dim isProducer = asProducer IsNot Nothing
        Dim nextProducer = TryCast(asProducer?.NextAction, ISoundProducer)

        ' Check what to stop.
        ' If a cross-fade action is started before time, stop the previous mains.
        Dim StopAllSoundsTypes = {ExecutionTypes.MainStopAll}
        Dim StopMainKeepParTypes = {ExecutionTypes.MainStopPrev, ExecutionTypes.MainCrossFade}

        If StopAllSoundsTypes.Contains(interruptType) OrElse StopAllSoundsTypes.Contains(act.ExecutionType) Then
            StopAllSounds(res)
            mNotifications.Clear()

        ElseIf StopMainKeepParTypes.Contains(interruptType) OrElse StopMainKeepParTypes.Contains(act.ExecutionType) Then
            StopMainSounds(res)
            mNotifications.Clear()
        End If

        ' Only add audio-related actions to the internal list
        If isProducer OrElse TryCast(act, ISoundAutomation) IsNot Nothing Then
            mPlayingList.Add(act)
        End If

        ' Generate in-out effects
        If isProducer AndAlso act.ExecutionType = ExecutionTypes.MainCrossFade Then
            Dim fadeIn = CreateFadeInEffect(asProducer)
            mGeneratedList.Add(fadeIn)
        End If

        If isProducer AndAlso nextProducer IsNot Nothing AndAlso nextProducer.ExecutionType = ExecutionTypes.MainCrossFade Then
            Dim fadeOut = CreateFadeOutEffects(asProducer, nextProducer.DelayBefore)
            mGeneratedList.AddRange(fadeOut)
            mNotifications.RemoveTriggeredNotifications()
        End If

        ' Update actions
        If isProducer Then
            res.StartingProducers.Add(asProducer)
            PlaybackStatus.SetMainProducer(asProducer)
        Else
            res.StartingNonProducers.Add(act)
            PlaybackStatus.SetMainAction(act, True)
        End If

        Return res
    End Function


    ''' <summary>
    ''' Play a file in parallel with the main line.
    ''' </summary>
    ''' <remarks>
    ''' Add the action to <see cref="mPlayingList"/>.
    ''' </remarks>
    Private Function PlayParallel(act As IPlayerAction) As AudioActions
        Dim res = New AudioActions()

        ' Not playable, ignore
        If Not act.CanExecute Then Return res

        mPlayingList.Add(act)

        Dim asProducer = TryCast(act, ISoundProducer)
        If asProducer IsNot Nothing Then
            res.StartingProducers.Add(asProducer)
        Else
            res.StartingNonProducers.Add(act)
        End If

        Return res
    End Function


    ''' <summary>
    ''' Adjust the playlist time as accurately as possible.
    ''' </summary>
    ''' <param name="status">Source of information</param>
    ''' <param name="defaultTimeStep">How much to add if there is no sound been played [milliseconds]</param>
    ''' <returns>New playlist time [milliseconds]</returns>
    Private Shared Function AdjustPlaybackTime(status As PlaybackStatus, defaultTimeStep As Double) As Double
        With status
            If .ActiveMainProducer?.IsPlaying Then
                Dim duration = .ActiveMainProducer.PlayPosition.TotalMilliseconds - .ActiveMainProducer.StartPosition.TotalMilliseconds
                Return .ActiveMainProducer.StartTime + duration

            ElseIf .IsActive And Not .IsManuallyPaused Then
                Return .PlaylistTime + defaultTimeStep

            Else
                Return .PlaylistTime
            End If
        End With
    End Function


    Private Sub StopAllSounds(res As AudioActions)
        For Each act In mPlayingList.ToList()
            res.Stopping.Add(act)
            mPlayingList.Remove(act)
        Next

        For Each act In mGeneratedList.ToList()
            mGeneratedList.Remove(act)
        Next
    End Sub


    Private Sub StopMainSounds(res As AudioActions)
        For Each mainAct In mPlayingList.Where(Function(a) a.ExecutionType <> ExecutionTypes.Parallel).ToList()
            res.Stopping.Add(mainAct)
            mPlayingList.Remove(mainAct)
        Next

        For Each act In mGeneratedList.ToList()
            mGeneratedList.Remove(act)
        Next
    End Sub


    ''' <summary>
    ''' Create a fade-in effect for the given producer.
    ''' </summary>
    Private Function CreateFadeInEffect(asProducer As ISoundProducer) As IPlayerAction
        Dim fadeIn = mEffectGenerator.Generate(
            GeneratedVolumeAutomationTypes.FadeIn, asProducer, asProducer.DelayBefore)

        Return fadeIn
    End Function


    ''' <summary>
    ''' Create fade-out effects for all main-line producers.
    ''' </summary>
    Private Function CreateFadeOutEffects(ref As ISoundProducer, duration As TimeSpan) As IEnumerable(Of IPlayerAction)
        Dim fadeOutList = (
            From mainAct In mPlayingList.OfType(Of ISoundProducer)()
            Where mainAct.ExecutionType <> ExecutionTypes.Parallel
            Select mEffectGenerator.Generate(GeneratedVolumeAutomationTypes.FadeOut, ref, duration)
            ).ToList()

        Return fadeOutList
    End Function

#End Region


#Region " Notification utility "

    ''' <summary>
    ''' Check notifications for the global and starting actions.
    ''' </summary>
    ''' <param name="starting">A list of actions to check for references</param>
    ''' <remarks>
    ''' A list of potential triggers are all actions that have one of <paramref name="starting"/>
    ''' as a reference.
    ''' 
    ''' Global parallels and wall-clock dependent items
    ''' are also added to the list of potential triggers.
    ''' 
    ''' Potential, because those already in <paramref name="starting"/> shall not come twice.
    ''' </remarks>
    Private Sub AddTriggers(starting As ICollection(Of IPlayerAction))
        Dim triggerList = mActionList.
                          Concat(mGeneratedList).
                          Where(Function(a) starting IsNot Nothing AndAlso starting.Contains(a.ReferenceAction) OrElse
                                            a.DelayReference = DelayReferences.WallClock OrElse
                                            a.DelayReference = DelayReferences.MainClock).
                          Except(starting).
                          ToList()

        mNotifications.SetNotificationTriggers(mLastWallTick, mLastPlayTick, triggerList)
    End Sub


    ''' <summary>
    ''' Check which actions had to be started since the last check due to triggers.
    ''' </summary>
    Private Function GetTriggeredActions() As AudioActions
        Dim actions = New AudioActions()
        Dim curTime = mTimeService.GetCurrentTime()

        Do
            ' Iterative check:
            ' - Check the trigger list
            ' - Start what's needed
            ' - Starting may add to trigger list
            Dim notif = mNotifications.CheckNotifications(mLastWallTick, curTime, mLastPlayTick, PlaybackStatus.PlaylistTime)
            If Not notif.Any() Then Exit Do

            For Each newAction In notif
                If newAction.IsActive Then
                    ' Ignore already started actions
                    InterfaceMapper.GetImplementation(Of IMessageLog)().LogAudioError(
                        $"Action '{newAction.Name}' is already playing, ignore the trigger.")
                Else
                    actions.Merge(Play(newAction))
                End If
            Next
        Loop

        mLastWallTick = curTime
        mLastPlayTick = PlaybackStatus.PlaylistTime

        Return actions
    End Function

#End Region

End Class
