Imports System.Diagnostics.CodeAnalysis
Imports AudioPlayerLibrary
Imports Common


<TestClass>
<TestCategory("Audio library notifications")>
Public Class NotificationCollectionTest

#Region " TriggerLogger "

    <ExcludeFromCodeCoverage>
    Private Class TriggerLogger
        Implements IMessageLog

        Public ReadOnly mTriggerList As List(Of TriggerSummary)


        Public Sub New(triggerList As List(Of TriggerSummary))
            mTriggerList = triggerList
        End Sub


        Public Sub ClearLog(reason As String) Implements IMessageLog.ClearLog
        End Sub

        Public Sub LogFileCacheInfo(size As Integer) Implements IMessageLog.LogFileCacheInfo
        End Sub

        Public Sub LogSampleCacheInfo(size As Integer) Implements IMessageLog.LogSampleCacheInfo
        End Sub

        Public Sub LogFigureCacheInfo(size As Long) Implements IMessageLog.LogFigureCacheInfo
        End Sub

        Public Sub LogTriggerInfo(triggerList As IEnumerable(Of TriggerSummary)) Implements IMessageLog.LogTriggerInfo
            mTriggerList.Clear()
            mTriggerList.AddRange(triggerList)
            mReported = True
        End Sub

        Public Sub LogDurationWork(working As Boolean) Implements IMessageLog.LogDurationWork
        End Sub

        Public Sub LogWaveformWork(working As Boolean) Implements IMessageLog.LogWaveformWork
        End Sub

        Public Sub LogTriggerMessage(format As String, ParamArray args() As Object) Implements IMessageLog.LogTriggerMessage
        End Sub

        Public Sub LogVoiceInfo(format As String, ParamArray args() As Object) Implements IMessageLog.LogVoiceInfo
        End Sub

        Public Sub LogKeyError(format As String, ParamArray args() As Object) Implements IMessageLog.LogKeyError
        End Sub

        Public Sub LogLicenseWarning(format As String, ParamArray args() As Object) Implements IMessageLog.LogLicenseWarning
        End Sub

        Public Sub LogLoadingError(format As String, ParamArray args() As Object) Implements IMessageLog.LogLoadingError
        End Sub

        Public Sub LogAudioError(format As String, ParamArray args() As Object) Implements IMessageLog.LogAudioError
        End Sub

        Public Sub LogPowerPointError(format As String, ParamArray args() As Object) Implements IMessageLog.LogPowerPointError
        End Sub
    End Class

#End Region


#Region " Fields "

    Private Shared mReported As Boolean

    Private ReadOnly mTriggerList As New List(Of TriggerSummary)()

    Private mTimeService As TestTimeService

    Private mNotif As NotificationCollection

    Private mStatus As PlaybackStatus

    Private mActionTriggeredBeforeEnd As TestAction

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Set up <see cref="InterfaceMapper"/> to use the stub classes.
    ''' Set up producers and effects, 1 of each on each line.
    ''' Initialize <see cref="PlaybackStatus"/> and 3 helper actions.
    ''' </summary>
    <TestInitialize>
    Public Sub Initialize()
        InterfaceMapper.SetInstance(Of IEffectDurationConfiguration)(New TestEffectDurationConfiguration())
        InterfaceMapper.SetInstance(Of IMessageLog)(New TriggerLogger(mTriggerList))

        mTimeService = New TestTimeService()
        InterfaceMapper.SetInstance(Of ITimeService)(mTimeService)
        TestTimeService.CurrentTime = mTimeService.GetTime(2019, 10, 5, 12, 0, 0)

        mReported = False
        mStatus = New PlaybackStatus()
        mStatus.Reset()

        mNotif = New NotificationCollection(mStatus)

        mActionTriggeredBeforeEnd = New TestActionEffect With {
            .Name = "Action BE",
            .DelayType = DelayTypes.TimedBeforeEnd,
            .DelayReference = DelayReferences.LastProducer,
            .DelayBefore = TimeSpan.FromSeconds(2)}
    End Sub

#End Region


#Region " Trigger time tests "

    <TestMethod>
    Public Sub Notifications_AddManualTrigger()
        ' Manually started, ignore
        Dim manualTrigger = New TestActionProducer With {.DelayType = DelayTypes.Manual}

        mNotif.SetNotification(0, 0, manualTrigger)

        Assert.IsTrue(mReported)
        Assert.AreEqual(0, mTriggerList.Count)
    End Sub


    <TestMethod>
    Public Sub Notifications_AddPlayingTrigger()
        ' Already playing, ignore
        Dim alreadyPlaying = New TestActionProducer With {
            .DelayType = DelayTypes.TimedFromStart,
            .DelayReference = DelayReferences.LastProducer,
            .DelayBefore = TimeSpan.FromSeconds(0.5),
            .IsPlaying = True}

        mNotif.SetNotification(0, 0, alreadyPlaying)

        Assert.IsTrue(mReported)
        Assert.AreEqual(0, mTriggerList.Count)
    End Sub


    <TestMethod>
    Public Sub Notifications_AddStoppedReferenceTrigger()
        Dim refNotPlaying = New TestActionProducer With {
            .DelayType = DelayTypes.TimedFromStart,
            .DelayReference = DelayReferences.LastProducer,
            .DelayBefore = TimeSpan.FromSeconds(0.5),
            .ReferenceAction = mActionTriggeredBeforeEnd,
            .Name = "Test"}
        mStatus.SetMainProducer(Nothing)

        ' Delay reference is not playing, but as the SetNotification is called,
        ' for some reason this reference is about to start, so the notification
        ' is added - albeit it does not look correct in this test case.
        mNotif.SetNotification(0, 0, refNotPlaying)

        Assert.IsTrue(mReported)
        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreEqual("Test", mTriggerList.Last.NextAction)
        Assert.AreEqual(New Date(1970, 1, 1, 0, 0, 0, 500), mTriggerList.Last.NextTime)
    End Sub


    <TestMethod>
    Public Sub Notifications_AddOnlyFutureTriggers()
        Dim list = TestPlaylistUtility.CreatePlaylist("PP1 EE1-S")
        PlaylistStructureLibrary.ArrangeStructure(list)

        mNotif.SetNotification(0, 0, list(1))

        Assert.IsTrue(mReported)
        Assert.AreEqual(0, mTriggerList.Count)
    End Sub


    <TestMethod>
    Public Sub Notifications_AddAfterStartTrigger()
        Dim list = TestPlaylistUtility.CreatePlaylist("PP1 EE1-S1")
        PlaylistStructureLibrary.ArrangeStructure(list)

        mNotif.SetNotification(0, 0, list(1))

        Assert.IsTrue(mReported)
        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreEqual("E1", mTriggerList.Last.NextAction)
        Assert.AreEqual(New Date(1970, 1, 1, 0, 0, 1), mTriggerList.Last.NextTime)
    End Sub


    <TestMethod>
    Public Sub Notifications_AddBeforeEndTriggerNoDuration()
        mStatus.SetMainProducer(New TestActionProducer With {.HasDuration = False})

        mNotif.SetNotification(0, 0, mActionTriggeredBeforeEnd)

        Assert.IsTrue(mReported)
        Assert.AreEqual(0, mTriggerList.Count)
    End Sub


    <TestMethod>
    Public Sub Notifications_AddBeforeEndTrigger()
        Dim list = TestPlaylistUtility.CreatePlaylist("PP1 EE1-E-2")
        PlaylistStructureLibrary.ArrangeStructure(list)

        mNotif.SetNotification(0, 0, list(1))

        Assert.IsTrue(mReported)
        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreEqual("E1", mTriggerList.Last.NextAction)
        Assert.AreEqual(TimeSpan.FromSeconds(10), list(0).Duration)
        Assert.AreEqual(New Date(1970, 1, 1, 0, 0, 8), mTriggerList.Last.NextTime)
    End Sub


    <TestMethod>
    Public Sub Notifications_AddAfterEndTrigger()
        Dim list = TestPlaylistUtility.CreatePlaylist("PP1 EE1-E2")
        PlaylistStructureLibrary.ArrangeStructure(list)

        mNotif.SetNotification(0, 0, list(1))

        Assert.IsTrue(mReported)
        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreEqual("E1", mTriggerList.Last.NextAction)
        Assert.AreEqual(TimeSpan.FromSeconds(10), list(0).Duration)
        Assert.AreEqual(New Date(1970, 1, 1, 0, 0, 12), mTriggerList.Last.NextTime)
    End Sub


    <TestMethod>
    Public Sub Notifications_AddTriggerNoEnd()
        mStatus.SetMainProducer(New TestActionProducer With {.HasDuration = False})

        mNotif.SetNotification(0, 0, mActionTriggeredBeforeEnd)

        Assert.AreEqual(0, mTriggerList.Count)

        mNotif.SetNotification(0, 0, New TestActionEffect With {
            .Name = "Action AE",
            .DelayType = DelayTypes.TimedAfterEnd,
            .DelayReference = DelayReferences.LastProducer,
            .DelayBefore = TimeSpan.FromSeconds(3)}
)

        Assert.AreEqual(0, mTriggerList.Count)
    End Sub

#End Region


#Region " Trigger list tests "

    <TestMethod>
    Public Sub Notifications_ClearTrigger()
        Dim list = TestPlaylistUtility.CreatePlaylist("PP1 EE1-S5")
        PlaylistStructureLibrary.ArrangeStructure(list)

        mNotif.SetNotification(0, 0, list(1))

        Assert.IsTrue(mReported)
        Assert.AreEqual(1, mTriggerList.Count)

        mNotif.ClearNotification(list(1))

        Assert.AreEqual(0, mTriggerList.Count)
    End Sub


    <TestMethod>
    Public Sub Notifications_ClearAll()
        Dim list = TestPlaylistUtility.CreatePlaylist("PP1 EE1-S5")
        PlaylistStructureLibrary.ArrangeStructure(list)

        mNotif.SetNotification(0, 0, list(1))

        Assert.IsTrue(mReported)
        Assert.AreEqual(1, mTriggerList.Count)

        mNotif.Clear()

        Assert.AreEqual(0, mTriggerList.Count)
    End Sub


    <TestMethod>
    Public Sub Notifications_CheckTriggers()
        Dim list = TestPlaylistUtility.CreatePlaylist("PP1 EE1-S1 EE2-E")
        PlaylistStructureLibrary.ArrangeStructure(list)

        mNotif.SetNotification(0, 0, list(1))
        mNotif.SetNotification(0, 0, list(2))

        Assert.IsTrue(mReported)
        Assert.AreEqual(2, mTriggerList.Count)

        Dim time0 = mTimeService.GetCurrentTime()
        Dim toTrigger = mNotif.CheckNotifications(time0, time0, 0, 0)
        Assert.AreEqual(0, toTrigger.Count)

        ' 0.5 second passed, no changes
        mReported = False
        toTrigger = mNotif.CheckNotifications(time0, time0 + 500, 0, 500)
        Assert.IsFalse(mReported)
        Assert.AreEqual(0, toTrigger.Count)

        ' 1 second passed, 1 trigger
        mReported = False
        toTrigger = mNotif.CheckNotifications(time0 + 500, time0 + 1000, 0, 1000)
        Assert.IsTrue(mReported)
        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreEqual(1, toTrigger.Count)
        Assert.AreEqual("E1", toTrigger.First().Name)
        Assert.AreEqual(list(1), toTrigger.First())

        ' Clean up call
        mReported = False
        mNotif.RemoveTriggeredNotifications()
        Assert.IsTrue(mReported)
        Assert.AreEqual(1, mTriggerList.Count)

        ' 1 minute passed, 1 more trigger
        mReported = False
        toTrigger = mNotif.CheckNotifications(time0 + 1000, time0 + 60000, 1000, 59000)
        Assert.IsTrue(mReported)
        Assert.AreEqual(0, mTriggerList.Count)
        Assert.AreEqual(1, toTrigger.Count)
        Assert.AreEqual("E2", toTrigger.First().Name)
        Assert.AreEqual(list(2), toTrigger.First())

        ' Clean up call
        mReported = False
        mNotif.RemoveTriggeredNotifications()
        Assert.IsTrue(mReported)
        Assert.AreEqual(0, mTriggerList.Count)
    End Sub

#End Region


#Region " Trigger reference tests "

    <TestMethod>
    Public Sub NotificationReference_LastProducerOnMain()
        Dim list = TestPlaylistUtility.CreatePlaylist("PP1 EE1 pP2 eE2 PT")
        With list.Last()
            .DelayType = DelayTypes.TimedAfterEnd
            .DelayReference = DelayReferences.LastProducer ' P1
            .DelayBefore = TimeSpan.FromSeconds(1)
        End With
        PlaylistStructureLibrary.ArrangeStructure(list)

        mNotif.SetNotification(0, 0, list.Last())

        Assert.IsTrue(mReported)
        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreEqual("T", mTriggerList.Last.NextAction)
        Assert.AreEqual(New Date(1970, 1, 1, 0, 0, 11), mTriggerList.Last.NextTime)
    End Sub


    <TestMethod>
    Public Sub NotificationReference_LastProducerOnParallel()
        Dim list = TestPlaylistUtility.CreatePlaylist("PP1 EE1 pP2 eE2 pT")
        With list.Last()
            .DelayType = DelayTypes.TimedAfterEnd
            .DelayReference = DelayReferences.LastProducer ' P2
            .DelayBefore = TimeSpan.FromSeconds(1)
        End With
        PlaylistStructureLibrary.ArrangeStructure(list)

        mNotif.SetNotification(0, 0, list.Last())

        Assert.IsTrue(mReported)
        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreEqual("T", mTriggerList.Last.NextAction)
        Assert.AreEqual(New Date(1970, 1, 1, 0, 0, 41), mTriggerList.Last.NextTime)
    End Sub


    <TestMethod>
    Public Sub NotificationReference_LastProducerOnParallel_FoundMain()
        Dim list = TestPlaylistUtility.CreatePlaylist("PP1 EE1 eE2 pT")
        With list.Last()
            .DelayType = DelayTypes.TimedAfterEnd
            .DelayReference = DelayReferences.LastProducer ' P1
            .DelayBefore = TimeSpan.FromSeconds(1)
        End With
        PlaylistStructureLibrary.ArrangeStructure(list)

        mNotif.SetNotification(0, 0, list.Last())

        Assert.IsTrue(mReported)
        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreEqual("T", mTriggerList.Last.NextAction)
        Assert.AreEqual(New Date(1970, 1, 1, 0, 0, 11), mTriggerList.Last.NextTime)
    End Sub


    <TestMethod>
    Public Sub NotificationReference_LastActionOnMain()
        Dim list = TestPlaylistUtility.CreatePlaylist("PP1 EE1 pP2 eE2 PT")
        With list.Last()
            .DelayReference = DelayReferences.LastAction ' E1
            .DelayType = DelayTypes.TimedAfterEnd
            .DelayBefore = TimeSpan.FromSeconds(1)
        End With
        PlaylistStructureLibrary.ArrangeStructure(list)

        mNotif.SetNotification(0, 0, list.Last())

        Assert.IsTrue(mReported)
        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreEqual("T", mTriggerList.Last.NextAction)
        Assert.AreEqual(New Date(1970, 1, 1, 0, 0, 21), mTriggerList.Last.NextTime)
    End Sub


    <TestMethod>
    Public Sub NotificationReference_LastActionOnMain_FoundProducer()
        Dim list = TestPlaylistUtility.CreatePlaylist("PP1 pP2 eE1 PT")
        With list.Last()
            .DelayReference = DelayReferences.LastAction ' P1
            .DelayType = DelayTypes.TimedAfterEnd
            .DelayBefore = TimeSpan.FromSeconds(1)
        End With
        PlaylistStructureLibrary.ArrangeStructure(list)

        mNotif.SetNotification(0, 0, list.Last())

        Assert.IsTrue(mReported)
        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreEqual("T", mTriggerList.Last.NextAction)
        Assert.AreEqual(New Date(1970, 1, 1, 0, 0, 11), mTriggerList.Last.NextTime)
    End Sub


    <TestMethod>
    Public Sub NotificationReference_LastActionOnParallel()
        Dim list = TestPlaylistUtility.CreatePlaylist("PP1 EE1 pP2 eE2 pT")
        With list.Last()
            .DelayType = DelayTypes.TimedAfterEnd
            .DelayReference = DelayReferences.LastAction ' E2
            .DelayBefore = TimeSpan.FromSeconds(1)
        End With
        PlaylistStructureLibrary.ArrangeStructure(list)

        mNotif.SetNotification(0, 0, list.Last())

        Assert.IsTrue(mReported)
        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreEqual("T", mTriggerList.Last.NextAction)
        Assert.AreEqual(New Date(1970, 1, 1, 0, 1, 21), mTriggerList.Last.NextTime)
    End Sub


    <TestMethod>
    Public Sub NotificationReference_LastActionOnParallel_FoundProducerOnParallel()
        Dim list = TestPlaylistUtility.CreatePlaylist("PP1 EE1 pP2 pT")
        With list.Last()
            .DelayType = DelayTypes.TimedAfterEnd
            .DelayReference = DelayReferences.LastAction ' P2
            .DelayBefore = TimeSpan.FromSeconds(1)
        End With
        PlaylistStructureLibrary.ArrangeStructure(list)

        mNotif.SetNotification(0, 0, list.Last())

        Assert.IsTrue(mReported)
        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreEqual("T", mTriggerList.Last.NextAction)
        Assert.AreEqual(New Date(1970, 1, 1, 0, 0, 41), mTriggerList.Last.NextTime)
    End Sub


    <TestMethod>
    Public Sub NotificationReference_LastActionOnParallel_FoundActionOnMain()
        Dim list = TestPlaylistUtility.CreatePlaylist("PP1 EE1 pT")
        With list.Last()
            .DelayType = DelayTypes.TimedAfterEnd
            .DelayReference = DelayReferences.LastAction ' E1
            .DelayBefore = TimeSpan.FromSeconds(1)
        End With
        PlaylistStructureLibrary.ArrangeStructure(list)

        mNotif.SetNotification(0, 0, list.Last())

        Assert.IsTrue(mReported)
        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreEqual("T", mTriggerList.Last.NextAction)
        Assert.AreEqual(New Date(1970, 1, 1, 0, 0, 21), mTriggerList.Last.NextTime)
    End Sub


    <TestMethod>
    Public Sub NotificationReference_LastActionOnParallel_FoundProducerOnMain()
        Dim list = TestPlaylistUtility.CreatePlaylist("PP1 pT")
        With list.Last()
            .DelayType = DelayTypes.TimedAfterEnd
            .DelayReference = DelayReferences.LastAction ' P1
            .DelayBefore = TimeSpan.FromSeconds(1)
        End With
        PlaylistStructureLibrary.ArrangeStructure(list)

        mNotif.SetNotification(0, 0, list.Last())

        Assert.IsTrue(mReported)
        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreEqual("T", mTriggerList.Last.NextAction)
        Assert.AreEqual(New Date(1970, 1, 1, 0, 0, 11), mTriggerList.Last.NextTime)
    End Sub


    <TestMethod>
    Public Sub NotificationReference_PlayerClockReference()
        mNotif.SetNotification(0, 0, New TestActionProducer With {
            .Name = "PC",
            .DelayType = DelayTypes.TimedFromStart,
            .DelayReference = DelayReferences.MainClock,
            .DelayBefore = TimeSpan.FromSeconds(10)})

        Assert.IsTrue(mReported)
        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreEqual("PC", mTriggerList.Last.NextAction)
        Assert.AreEqual(New Date(1970, 1, 1, 0, 0, 10), mTriggerList.Last.NextTime)
    End Sub


    <TestMethod>
    Public Sub NotificationReference_WallClockReference()
        mNotif.SetNotification(0, 0, New TestActionProducer With {
            .Name = "WC",
            .DelayType = DelayTypes.TimedFromStart,
            .DelayReference = DelayReferences.WallClock,
            .DelayBefore = New TimeSpan(12, 10, 0)})

        Assert.IsTrue(mReported)
        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreEqual("WC", mTriggerList.Last.NextAction)
        Assert.AreEqual(New Date(2019, 10, 5, 12, 10, 0), mTriggerList.Last.NextTime)
    End Sub

#End Region


#Region " Trigger changes tests "

    <TestMethod>
    Public Sub Notifications_ChangeDelayDuringPlayback()
        Dim list = TestPlaylistUtility.CreatePlaylist("PP1 EE1-S1")
        PlaylistStructureLibrary.ArrangeStructure(list)

        mNotif.SetNotification(0, 0, list(1))

        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreEqual("E1", mTriggerList.Last.NextAction)
        Assert.AreEqual(New Date(1970, 1, 1, 0, 0, 1), mTriggerList.Last.NextTime)

        list(1).DelayBefore = TimeSpan.FromSeconds(4)
        mNotif.SetNotification(0, 0, list(1))

        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreEqual("E1", mTriggerList.Last.NextAction)
        Assert.AreEqual(New Date(1970, 1, 1, 0, 0, 4), mTriggerList.Last.NextTime)
    End Sub


    <TestMethod>
    Public Sub Notifications_ChangeTypeDuringPlayback()
        Dim list = TestPlaylistUtility.CreatePlaylist("PP1 EE1-S1")
        PlaylistStructureLibrary.ArrangeStructure(list)

        mNotif.SetNotification(0, 0, list(1))

        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreEqual("E1", mTriggerList.Last.NextAction)
        Assert.AreEqual(New Date(1970, 1, 1, 0, 0, 1), mTriggerList.Last.NextTime)

        list(1).DelayType = DelayTypes.TimedBeforeEnd
        mNotif.SetNotification(0, 0, list(1))

        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreEqual("E1", mTriggerList.Last.NextAction)
        Assert.AreEqual(New Date(1970, 1, 1, 0, 0, 9), mTriggerList.Last.NextTime)
    End Sub

#End Region


#Region " Trigger batch tests "

    <TestMethod>
    Public Sub Notifications_SetMultipleTriggers()
        Dim list = TestPlaylistUtility.CreatePlaylist("PP1 EE1-E EE2-E5")
        PlaylistStructureLibrary.ArrangeStructure(list)

        mNotif.SetNotificationTriggers(0, 0, list)

        Assert.IsTrue(mReported)
        Assert.AreEqual(2, mTriggerList.Count)

        ' Clean up call
        mReported = False
        mNotif.Clear()
        Assert.IsTrue(mReported)
        Assert.AreEqual(0, mTriggerList.Count)
    End Sub

#End Region

End Class
