Imports System.Threading
Imports AudioChannelLibrary
Imports Common
Imports NextPlayer


<TestClass>
Public Class NAudioPlayerTest

#Region " Init and clean-up "

    <ClassInitialize>
    Public Shared Sub InitClass(ctx As TestContext)
        SetupTestIo()
        InterfaceMapper.SetInstance(Of IVolumeConfiguration)(New TestConfig())
    End Sub


    <ClassCleanup>
    Public Shared Sub CleanupClass()
        InterfaceMapper.SetInstance(Of IInputStreamProvider)(FileCache.Instance)
    End Sub


    <TestInitialize()>
    Public Sub InitTests()
        InterfaceMapper.SetInstance(Of IMessageLog)(New TestLogger())
    End Sub

#End Region


#Region " Open file tests "

    <TestMethod>
    <TestCategory("NAudio player")>
    Public Sub NAudioPlayer_Open_Ok()
        Dim storage As New TestAudioEnvironmentStorage("1")
        InterfaceMapper.SetInstance(Of IAudioEnvironmentStorage)(storage)
        InterfaceMapper.SetInstance(Of IInputStreamProvider)(
            New TestInputStreamProvider(1, ""))

        Dim okEvent As New AutoResetEvent(False)
        Dim failedEvent As New AutoResetEvent(False)

        Dim pl As New NAudioPlayer()
        AddHandler pl.MediaOpened, Sub(obj, args) okEvent.Set()
        AddHandler pl.MediaFailed, Sub(obj, args) failedEvent.Set()

        pl.Open("Any name.wav", storage.Logical(0).Channel)

        Dim waitRes = WaitHandle.WaitAny({okEvent, failedEvent}, 1000)
        Assert.AreEqual(0, waitRes)

        pl.Close()
    End Sub


    <TestMethod>
    <TestCategory("NAudio player")>
    Public Sub NAudioPlayer_OpenUnexisting_Error()
        Dim storage As New TestAudioEnvironmentStorage("1")
        InterfaceMapper.SetInstance(Of IAudioEnvironmentStorage)(storage)
        InterfaceMapper.SetInstance(Of IInputStreamProvider)(
            New TestInputStreamProvider())

        Dim okEvent As New AutoResetEvent(False)
        Dim failedEvent As New AutoResetEvent(False)

        Dim pl As New NAudioPlayer()
        AddHandler pl.MediaOpened, Sub(obj, args) okEvent.Set()
        AddHandler pl.MediaFailed, Sub(obj, args) failedEvent.Set()

        pl.Open("Any name.wav", storage.Logical(0).Channel)

        Dim waitRes = WaitHandle.WaitAny({okEvent, failedEvent}, 1000)
        Assert.AreEqual(1, waitRes)

        pl.Close()
    End Sub

#End Region

End Class
