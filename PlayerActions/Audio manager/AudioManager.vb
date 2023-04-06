Imports System.Collections.Specialized
Imports System.Threading
Imports System.Timers
Imports AudioPlayerLibrary


''' <summary>
''' Offload the UI with audio playback management.
''' The playback control runs on a dedicated thread.
''' </summary>
Public Class AudioManager
    Implements IAudioManager

#Region " Events "

#Region " PlaybackStarted event "

    ''' <summary>
    ''' Playback of a new item has started.
    ''' This event is triggered on the creator's thread.
    ''' </summary>
    Public Event PlaybackStarted(sender As IAudioManager, action As IPlayerAction) Implements IAudioManager.PlaybackStarted


    Private Sub RaisePlaybackStarted(action As IPlayerAction)
        mCallbackSc.Post(Sub() RaiseEvent PlaybackStarted(Me, action), Nothing)
    End Sub

#End Region


#Region " PlaybackEnded event "

    ''' <summary>
    ''' Playback of an item has ended or was stopped.
    ''' This event is triggered on the creator's thread.
    ''' </summary>
    Public Event PlaybackEnded(sender As IAudioManager, action As IPlayerAction) Implements IAudioManager.PlaybackEnded


    Private Sub RaisePlaybackEnded(action As IPlayerAction)
        mCallbackSc.Post(Sub() RaiseEvent PlaybackEnded(Me, action), Nothing)
    End Sub

#End Region


#Region " StateChanged event "

    ''' <summary>
    ''' Playlist state is changed.
    ''' This event is triggered on the creator's thread.
    ''' </summary>
    Public Event StateChanged(sender As IAudioManager, state As PlaylistState) Implements IAudioManager.StateChanged


    Private Sub RaiseStateChanged(state As PlaylistState)
        mCallbackSc.Post(Sub() RaiseEvent StateChanged(Me, state), Nothing)
    End Sub

#End Region


#Region " PlaybackTimeChanged event "

    ''' <summary>
    ''' Playlist time is changed.
    ''' This event is triggered on the creator's thread.
    ''' </summary>
    Public Event PlaybackTimeChanged(sender As IAudioManager, time As TimeSpan) Implements IAudioManager.PlaybackTimeChanged


    Private Sub RaisePlaybackTimeChanged(time As TimeSpan)
        mCallbackSc.Post(Sub() RaiseEvent PlaybackTimeChanged(Me, time), Nothing)
    End Sub

#End Region

#End Region


#Region " Fields "

    ''' <summary>
    ''' Which thread to use for events.
    ''' </summary>
    Private ReadOnly mCallbackSc As SynchronizationContext


    '''' <summary>
    '''' The working thread of the audio manager.
    '''' </summary>
    'Private ReadOnly mWorkerSc As SynchronizationContext


    ''' <summary>
    ''' A set of methods to operate on the playlist.
    ''' </summary>
    Private WithEvents mAudioLib As AudioPlaybackLibrary


    ''' <summary>
    ''' A list of all actions to be played. Used for change notifications.
    ''' </summary>
    ''' <remarks>
    ''' The action list actually belongs to the UI thread.
    ''' We can only read item properties, and cannot modify.
    ''' </remarks>
    Private WithEvents mActionList As PlayerActionCollection

#End Region


#Region " IAudioManager properties "

    ''' <summary>
    ''' Whether any sound is being played or expected to be played (on pause).
    ''' </summary>
    Public ReadOnly Property IsActive As Boolean Implements IAudioManager.IsActive
        Get
            Return mAudioLib.PlaybackStatus.IsActive
        End Get
    End Property



    ''' <summary>
    ''' Whether any action is being played.
    ''' </summary>
    Public ReadOnly Property IsPlaying As Boolean Implements IAudioManager.IsPlaying
        Get
            Return mAudioLib.PlaybackStatus.IsPlaying
        End Get
    End Property


    ''' <summary>
    ''' Whether the playback is temporarily stalled.
    ''' </summary>
    Public ReadOnly Property IsManuallyPaused As Boolean Implements IAudioManager.IsManuallyPaused
        Get
            Return mAudioLib.PlaybackStatus.IsManuallyPaused
        End Get
    End Property

#End Region


#Region " Init and clean-up "

    Public Sub New(playbackTick As Integer, actionList As PlayerActionCollection)
        mCallbackSc = SynchronizationContext.Current
        mAudioLib = New AudioPlaybackLibrary()
        'mWorkerSc = Task.Run(Function() SynchronizationContext.Current).GetAwaiter().GetResult()

        mActionList = actionList
        mTickTimer = New Timers.Timer(playbackTick)
    End Sub

#End Region


#Region " IAudioManager API "

    ''' <summary>
    ''' Set a new playlist to operate on. Resets the status.
    ''' </summary>
    Public Sub SetPlaylist(actionList As PlayerActionCollection) Implements IAudioManager.SetPlaylist
        mActionList = actionList

        StartOperation(Sub()
                           ProcessActions(mAudioLib.SetPlaylist(actionList.Items), False)
                           RaiseStateChanged(CreateState(mAudioLib.PlaybackStatus))
                       End Sub)
    End Sub


    ''' <summary>
    ''' Reset the current playlist and all triggers.
    ''' </summary>
    Public Sub Reset() Implements IAudioManager.Reset
        StopProgressTimer()

        StartOperation(Sub()
                           ProcessActions(mAudioLib.Reset(), False)
                           RaiseStateChanged(CreateState(mAudioLib.PlaybackStatus))
                           RaisePlaybackTimeChanged(TimeSpan.Zero)
                       End Sub)
    End Sub


    ''' <inheritdoc/>
    Public Sub Play(item As IPlayerAction, interrupt As ExecutionTypes) Implements IAudioManager.Play
        StartOperation(Sub()
                           ProcessActions(mAudioLib.Play(item, interrupt))
                           RaiseStateChanged(CreateState(mAudioLib.PlaybackStatus))
                       End Sub)
    End Sub


    ''' <inheritdoc/>
    Public Sub StartWaiting() Implements IAudioManager.StartWaiting
        StartOperation(Sub()
                           ProcessActions(mAudioLib.Play(Nothing, ExecutionTypes.MainStopAll))
                           RaiseStateChanged(CreateState(mAudioLib.PlaybackStatus))
                       End Sub)
    End Sub


    Public Sub StopSingle(item As IPlayerAction) Implements IAudioManager.StopSingle
        StartOperation(Sub()
                           ProcessActions(mAudioLib.StopSingle(item))
                           RaiseStateChanged(CreateState(mAudioLib.PlaybackStatus))
                       End Sub)
    End Sub


    Public Sub PauseAll() Implements IAudioManager.PauseAll
        StartOperation(Sub()
                           ProcessActions(mAudioLib.PauseAll())
                           RaiseStateChanged(CreateState(mAudioLib.PlaybackStatus))
                       End Sub)
    End Sub


    Public Sub ResumeMain() Implements IAudioManager.ResumeMain
        StartOperation(Sub()
                           ProcessActions(mAudioLib.ResumeMain())
                           RaiseStateChanged(CreateState(mAudioLib.PlaybackStatus))
                       End Sub)
    End Sub

#End Region


#Region " Action list event handlers "

    Private Sub ActionListChangedHandler(sender As Object, args As NotifyCollectionChangedEventArgs) Handles mActionList.CollectionChanged
        Dim pargs = CType(args, PlayerNotifyCollectionChangedEventArgs)

        If PlayerActionCollection.CanAffectTriggers(pargs) Then
            ' Remove possibly modified triggers
            For Each item In pargs.OldItems.Cast(Of PlayerAction)()
                mAudioLib.ClearNotification(item)
            Next

            mAudioLib.SetActiveActions()
            RaiseStateChanged(CreateState(mAudioLib.PlaybackStatus))

        ElseIf PlayerActionCollection.CanAffectStructure(pargs) Then
            mAudioLib.SetActiveActions()
            RaiseStateChanged(CreateState(mAudioLib.PlaybackStatus))
        End If
    End Sub

#End Region


#Region " Progress timer "

    ''' <summary>
    ''' How often the progress is updated during playback.
    ''' </summary>
    Private WithEvents mTickTimer As Timers.Timer


    ''' <summary>
    ''' Start the progress timer.
    ''' </summary>
    Private Sub StartProgressTimer()
        If mTickTimer.Enabled Then Return

        mTickTimer.Start()
    End Sub


    ''' <summary>
    ''' Stop the progress timer.
    ''' </summary>
    Private Sub StopProgressTimer()
        If Not mTickTimer.Enabled Then Return

        mTickTimer.Stop()
    End Sub


    ''' <summary>
    ''' Synchronize playback progress.
    ''' Check notifications.
    ''' </summary>
    Private Sub OnProgressTickEvent(sender As Object, args As ElapsedEventArgs) Handles mTickTimer.Elapsed
        StartOperation(Sub()
                           Dim upd = mAudioLib.AdvancePlaylistTime(mTickTimer.Interval)
                           RaisePlaybackTimeChanged(TimeSpan.FromMilliseconds(upd.PlaybackTime))

                           If upd.HasActions Then
                               ProcessActions(upd.Actions)
                               RaiseStateChanged(CreateState(mAudioLib.PlaybackStatus))
                           End If
                       End Sub)
    End Sub

#End Region


#Region " Utility "

    ''' <summary>
    ''' Start a library operation.
    ''' </summary>
    Private Sub StartOperation(oper As Action)
        'mWorkerSc.Post(p, Nothing)
        ThreadPool.QueueUserWorkItem(Sub(s) oper.Invoke())
    End Sub


    Private Function CreateState(playbackStatus As PlaybackStatus) As PlaylistState
        Return New PlaylistState With {
            .ActiveMainAction = playbackStatus.ActiveMainAction,
            .ActiveMainProducer = playbackStatus.ActiveMainProducer,
            .LastMainProducer = playbackStatus.LastMainProducer,
            .ManualParallels = (From a In playbackStatus.ActiveParallels Where a.DelayType = DelayTypes.Manual).ToList(),
            .NextAction = playbackStatus.NextAction
            }
    End Function


    Private Sub ProcessActions(playActions As AudioActions, Optional startTimer As Boolean = True)
        mCallbackSc.Post(Sub()
                             For Each act In playActions.Pausing
                                 act.Stop(True)
                                 RaisePlaybackEnded(act)
                             Next

                             For Each act In playActions.Stopping
                                 act.Stop(False)
                                 RaisePlaybackEnded(act)
                             Next

                             ' First prepare to start the producers, so non-producers have references
                             For Each act In playActions.StartingProducers
                                 AddHandler act.PositionChanged, AddressOf PositionChangedHandler
                                 AddHandler act.EndReached, AddressOf EndReachedHandler
                                 act.PrepareStart()
                             Next

                             ' Start the non-producers, so their effect is applied at once
                             For Each act In playActions.StartingNonProducers
                                 act.PrepareStart()
                                 act.Start()
                                 RaisePlaybackStarted(act)
                             Next

                             ' Finally start the producers
                             For Each act In playActions.StartingProducers
                                 act.Start()
                                 RaisePlaybackStarted(act)
                             Next

                             ' Start the producers with applied automations
                             For Each act In playActions.Resuming
                                 act.Start()
                                 RaisePlaybackStarted(act)
                             Next

                             RaiseStateChanged(CreateState(mAudioLib.PlaybackStatus))
                         End Sub, Nothing)

        If startTimer Then
            OnProgressTickEvent(Nothing, Nothing)
            StartProgressTimer()
        End If
    End Sub


    ''' <summary>
    ''' Event handler for sudden position change.
    ''' </summary>
    Private Sub PositionChangedHandler(sender As ISoundProducer)
        StartOperation(Sub() mAudioLib.RecalculateNotifications())
    End Sub


    ''' <summary>
    ''' Event handler for natural end of the media.
    ''' </summary>
    Private Sub EndReachedHandler(act As ISoundProducer)
        RemoveHandler act.EndReached, AddressOf EndReachedHandler

        StartOperation(Sub() ProcessActions(mAudioLib.NotifyEndReached(act)))
    End Sub

#End Region

End Class
