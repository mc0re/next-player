Imports AudioPlayerLibrary
Imports Common


<TestClass>
Public Class AudioManagerTest

#Region " Fields "

    Private mAlib As AudioPlayerLibrary

    Private mTriggerList As IList(Of NotificationInfo)

    Private mMainEffect As TestAction

    Private mTriggerAfterStart As TestAction

    Private mTriggerBeforeEnd As TestAction

    Private mTriggerAfterEnd As TestAction

#End Region


    <TestInitialize>
    Public Sub Initialize()
        InterfaceMapper.SetInstance(Of IEffectDurationConfiguration)(New TestEffectDurationConfiguration())
        InterfaceMapper.SetInstance(Of IMessageLog)(New TestMessageLog())

        mAlib = New AudioPlayerLibrary()
        mAlib.Reset()

        ' Make sure the static AudioPlayerLibrary constructor is executed before GetImplementation()
        mTriggerList = InterfaceMapper.GetImplementation(Of IList(Of NotificationInfo))()

        ' Set up actions
        mMainEffect = New TestActionProducer With {.Duration = TimeSpan.FromSeconds(60), .HasDuration = True}
        mTriggerAfterStart = New TestAction With {
            .DelayType = DelayTypes.TimedFromStart, .DelayReference = DelayReferences.LastProducer, .DelayBefore = TimeSpan.FromSeconds(1)}
        mTriggerBeforeEnd = New TestAction With {
            .DelayType = DelayTypes.TimedBeforeEnd, .DelayReference = DelayReferences.LastProducer, .DelayBefore = TimeSpan.FromSeconds(2)}
        mTriggerAfterEnd = New TestAction With {
            .DelayType = DelayTypes.TimedAfterEnd, .DelayReference = DelayReferences.LastProducer, .DelayBefore = TimeSpan.FromSeconds(3)}
    End Sub


#Region " Notification tests "

    <TestMethod(), TestCategory("Audio library notifications")>
    Public Sub Notifications_AddRemoveTriggers()
        ' Manually started, ignore
        Dim mTriggerManual = New TestAction With {.DelayType = DelayTypes.Manual}

        ' Already playing, ignore
        Dim mTriggerAlreadyPlaying = New TestAction With {
            .DelayType = DelayTypes.TimedFromStart, .DelayReference = DelayReferences.LastProducer,
            .DelayBefore = TimeSpan.FromSeconds(0.5), .IsPlaying = True}

        ' Delay reference is not playing, cannot set up time, ignore
        Dim mTriggerRefNotPlaying = New TestAction With {
            .DelayType = DelayTypes.TimedFromStart, .DelayReference = DelayReferences.LastProducer,
            .DelayBefore = TimeSpan.FromSeconds(0.5), .DelayReferenceAction = mTriggerBeforeEnd}

        Assert.AreEqual(0, mTriggerList.Count)

        mAlib.PlayMain(mMainEffect, ExecutionTypes.MainContinuePrev)
        mAlib.SetNotification(mTriggerManual)
        mAlib.SetNotification(mTriggerAlreadyPlaying)
        mAlib.SetNotification(mTriggerRefNotPlaying)

        Assert.AreEqual(0, mTriggerList.Count)

        mAlib.SetNotification(mTriggerAfterStart)

        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreSame(mTriggerAfterStart, mTriggerList.Last.Action)
        Assert.AreEqual(1000.0, mTriggerList.Last.Position)

        mAlib.SetNotification(mTriggerBeforeEnd)

        Assert.AreEqual(2, mTriggerList.Count)
        Assert.AreSame(mTriggerBeforeEnd, mTriggerList.Last.Action)
        Assert.AreEqual(58000.0, mTriggerList.Last.Position)

        mAlib.SetNotification(mTriggerAfterEnd)

        Assert.AreEqual(3, mTriggerList.Count)
        Assert.AreSame(mTriggerAfterEnd, mTriggerList.Last.Action)
        Assert.AreEqual(63000.0, mTriggerList.Last.Position)

        mAlib.ClearNotification(mTriggerBeforeEnd)

        Assert.AreEqual(2, mTriggerList.Count)
        Assert.AreSame(mTriggerAfterStart, mTriggerList.First.Action)
        Assert.AreSame(mTriggerAfterEnd, mTriggerList.Last.Action)
    End Sub


    <TestMethod(), TestCategory("Audio library notifications")>
    Public Sub Notifications_AddTriggerNoEnd()
        Assert.AreEqual(0, mTriggerList.Count)

        mMainEffect.HasDuration = False
        mAlib.PlayMain(mMainEffect, ExecutionTypes.MainContinuePrev)

        mAlib.SetNotification(mTriggerBeforeEnd)

        Assert.AreEqual(0, mTriggerList.Count)

        mAlib.SetNotification(mTriggerAfterEnd)

        Assert.AreEqual(0, mTriggerList.Count)
    End Sub


    <TestMethod(), TestCategory("Audio library notifications")>
    Public Sub Notifications_ChangeDelayDuringPlayback()
        Assert.AreEqual(0, mTriggerList.Count)

        mAlib.PlayMain(mMainEffect, ExecutionTypes.MainContinuePrev)
        mAlib.SetNotification(mTriggerAfterStart)

        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreSame(mTriggerAfterStart, mTriggerList.Last.Action)
        Assert.AreEqual(1000.0, mTriggerList.Last.Position)

        mTriggerAfterStart.DelayBefore = TimeSpan.FromSeconds(4)
        mAlib.SetNotification(mTriggerAfterStart)

        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreSame(mTriggerAfterStart, mTriggerList.Last.Action)
        Assert.AreEqual(4000.0, mTriggerList.Last.Position)
    End Sub


    <TestMethod(), TestCategory("Audio library notifications")>
    Public Sub Notifications_ChangeTypeDuringPlayback()
        Assert.AreEqual(0, mTriggerList.Count)

        mAlib.PlayMain(mMainEffect, ExecutionTypes.MainContinuePrev)
        mAlib.SetNotification(mTriggerAfterStart)

        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreSame(mTriggerAfterStart, mTriggerList.Last.Action)
        Assert.AreEqual(1000.0, mTriggerList.Last.Position)

        mTriggerAfterStart.DelayType = DelayTypes.TimedBeforeEnd
        mAlib.SetNotification(mTriggerAfterStart)

        Assert.AreEqual(1, mTriggerList.Count)
        Assert.AreSame(mTriggerAfterStart, mTriggerList.Last.Action)
        Assert.AreEqual(59000.0, mTriggerList.Last.Position)
    End Sub

#End Region


#Region " PlayMain / PlayParallel tests "

    <TestMethod, TestCategory("Audio library")>
    Public Sub InterruptType_MainContinueAsPlanned_Ok()
        Dim firstPlaying As Boolean
        Dim secondPlaying As Boolean
        Dim parPlaying As Boolean

        Dim first As New TestActionProducer(Sub() firstPlaying = True, Sub() firstPlaying = False) With {.Name = "First"}
        Dim par As New TestActionProducer(Sub() parPlaying = True, Sub() parPlaying = False) With {
            .Name = "Parallel", .ExecutionType = ExecutionTypes.Parallel}

        Assert.IsFalse(mAlib.PlaybackStatus.IsActive)

        mAlib.PlayMain(first, ExecutionTypes.MainStopAll)
        mAlib.PlayParallel(par)

        Assert.IsTrue(firstPlaying)
        Assert.IsTrue(parPlaying)

        Dim second As New TestActionProducer(Sub() secondPlaying = True, Sub() secondPlaying = False) With {.Name = "Second"}
        second.ExecutionType = ExecutionTypes.MainContinuePrev
        mAlib.PlayMain(second, ExecutionTypes.MainContinuePrev)

        Assert.IsTrue(firstPlaying)
        Assert.IsTrue(secondPlaying)
        Assert.IsTrue(parPlaying)
    End Sub


    <TestMethod, TestCategory("Audio library")>
    Public Sub InterruptType_MainStopPrevAsPlanned_Ok()
        Dim firstPlaying As Boolean
        Dim secondPlaying As Boolean
        Dim parPlaying As Boolean

        Dim first As New TestActionProducer(Sub() firstPlaying = True, Sub() firstPlaying = False) With {.Name = "First"}
        Dim par As New TestActionProducer(Sub() parPlaying = True, Sub() parPlaying = False) With {
            .Name = "Parallel", .ExecutionType = ExecutionTypes.Parallel}

        Assert.IsFalse(mAlib.PlaybackStatus.IsActive)

        mAlib.PlayMain(first, ExecutionTypes.MainStopAll)
        mAlib.PlayParallel(par)

        Assert.IsTrue(firstPlaying)
        Assert.IsTrue(parPlaying)

        Dim second As New TestActionProducer(Sub() secondPlaying = True, Sub() secondPlaying = False) With {
            .Name = "Second", .ExecutionType = ExecutionTypes.MainStopPrev}
        mAlib.PlayMain(second, ExecutionTypes.MainContinuePrev)

        Assert.IsFalse(firstPlaying)
        Assert.IsTrue(secondPlaying)
        Assert.IsTrue(parPlaying)
    End Sub


    <TestMethod, TestCategory("Audio library")>
    Public Sub InterruptType_MainStopAllAsPlanned_Ok()
        Dim firstPlaying As Boolean
        Dim secondPlaying As Boolean
        Dim parPlaying As Boolean

        Dim first As New TestActionProducer(Sub() firstPlaying = True, Sub() firstPlaying = False) With {.Name = "First"}
        Dim par As New TestActionProducer(Sub() parPlaying = True, Sub() parPlaying = False) With {
            .Name = "Parallel", .ExecutionType = ExecutionTypes.Parallel}

        Assert.IsFalse(mAlib.PlaybackStatus.IsActive)

        mAlib.PlayMain(first, ExecutionTypes.MainStopAll)
        mAlib.PlayParallel(par)

        Assert.IsTrue(firstPlaying)
        Assert.IsTrue(parPlaying)

        Dim second As New TestActionProducer(Sub() secondPlaying = True, Sub() secondPlaying = False) With {
            .Name = "Second", .ExecutionType = ExecutionTypes.MainStopAll}
        mAlib.PlayMain(second, ExecutionTypes.MainContinuePrev)

        Assert.IsFalse(firstPlaying)
        Assert.IsTrue(secondPlaying)
        Assert.IsFalse(parPlaying)
    End Sub

#End Region

End Class
