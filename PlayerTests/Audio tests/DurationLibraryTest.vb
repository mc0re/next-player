Imports System.Threading
Imports Common
Imports AudioPlayerLibrary
Imports System.Windows


<TestClass>
Public Class DurationLibraryTest

#Region " Needed classes "

    Private Class TestDurationPlayer
        Implements IDurationPlayer

#Region " Events "

        Public Event MediaFailed(sender As Object, args As MediaFailedEventArgs) Implements IDurationPlayer.MediaFailed

        Public Event MediaOpened(sender As Object, args As EventArgs) Implements IDurationPlayer.MediaOpened

#End Region


#Region " Fields "

        Private mItemIndex As Integer

        Private ReadOnly mOutcomeList As IList(Of Boolean)

        Private ReadOnly mDurationList As IList(Of Duration)

#End Region


#Region " NaturalDuration read-only property "

        Private mDuration As Duration


        Public ReadOnly Property NaturalDuration As Duration Implements IDurationPlayer.NaturalDuration
            Get
                Return mDuration
            End Get
        End Property

#End Region


#Region " Init and clean-up "

        Public Sub New(outcomeList As IList(Of Boolean), durationList As IList(Of Double))
            mOutcomeList = outcomeList
            mDurationList = (From dur In durationList Select New Duration(TimeSpan.FromSeconds(dur))).ToList()
            mItemIndex = 0
        End Sub

#End Region


#Region " API "

        Public Sub Open(fileName As String, channelNo As Integer) Implements IDurationPlayer.Open
            Dim opened As New Thread(
                Sub()
                    Thread.Sleep(100)

                    If mOutcomeList(mItemIndex) Then
                        mDuration = mDurationList(mItemIndex)
                        RaiseEvent MediaOpened(Me, Nothing)
                        OpenedEvent.Set()
                    Else
                        RaiseEvent MediaFailed(Me, Nothing)
                        FailedEvent.Set()
                    End If

                    mItemIndex += 1
                End Sub)

            opened.Start()
        End Sub


        Public Sub Close() Implements IDurationPlayer.Close
            ' Nothing
        End Sub

#End Region


#Region " IDisposable implementation "

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do nothing
        End Sub

#End Region

    End Class

#End Region


#Region " Asynchronous handling events "

    Private Shared ReadOnly OpenedEvent As New AutoResetEvent(False)

    Private Shared ReadOnly FailedEvent As New AutoResetEvent(False)

#End Region


#Region " Asynchronous constants "

    Private Const FileOpenDelay As Integer = 500

    Private Const NoNewOpenDelay As Integer = 200

#End Region


#Region " Tests "

    <TestInitialize()>
    Public Sub InitTests()
        InterfaceMapper.SetInstance(Of IMessageLog)(New TestLogger())
    End Sub


    <TestMethod(), TestCategory("Duration")>
    Public Sub Duration_OneFile_Ok()
        ' Get the file
        Dim info As New TestDurationItem(GetTestFilePath("Electronic [Nokia].mp3"))

        ' Action
        Dim inst = New TestDurationPlayer({True}, {5})
        InterfaceMapper.SetInstance(Of IDurationPlayer)(inst)
        Assert.IsFalse(info.HasDuration)

        DurationLibrary.GetDuration(info)

        Dim isOpened = OpenedEvent.WaitOne(FileOpenDelay)
        Assert.IsTrue(isOpened)
        Assert.IsTrue(info.HasDuration)
		Assert.AreEqual(5.0, info.Duration.TotalSeconds)
        Assert.IsFalse(OpenedEvent.WaitOne(NoNewOpenDelay))
    End Sub


	<TestMethod(), TestCategory("Duration")>
	Public Sub Duration_TwoFiles_Ok()
		' Get the files
		Dim info1 As New TestDurationItem(GetTestFilePath("Electronic [Nokia].mp3"))
		Dim info2 As New TestDurationItem(GetTestFilePath("Ericcson ringing tone [].mp3"))

		' Action
		Dim inst = New TestDurationPlayer({True, True}, {5, 4})
		InterfaceMapper.SetInstance(Of IDurationPlayer)(inst)
		DurationLibrary.GetDuration(info1)
		DurationLibrary.GetDuration(info2)

		' Both shall come
		Assert.IsFalse(info1.HasDuration)
		Assert.IsFalse(info2.HasDuration)

        Assert.IsTrue(OpenedEvent.WaitOne(FileOpenDelay))
        Assert.IsTrue(info1.HasDuration)
		Assert.AreEqual(5.0, info1.Duration.TotalSeconds)
		Assert.IsFalse(info2.HasDuration)

        Assert.IsTrue(OpenedEvent.WaitOne(FileOpenDelay))
        Assert.IsTrue(info2.HasDuration)
		Assert.AreEqual(4.0, info2.Duration.TotalSeconds)

        Assert.IsFalse(OpenedEvent.WaitOne(NoNewOpenDelay))
    End Sub


	<TestMethod(), TestCategory("Duration")>
	Public Sub Duration_OneFile_Error()
		' Get the file
		Dim info As New TestDurationItem(GetTestFilePath("WrongMp3.mp3"))

		' Action
		Dim inst = New TestDurationPlayer({False}, {5})
		InterfaceMapper.SetInstance(Of IDurationPlayer)(inst)
		DurationLibrary.GetDuration(info)

		Assert.IsFalse(info.HasDuration)
		Assert.AreEqual(0.0, info.Duration.TotalSeconds)

        Assert.IsTrue(FailedEvent.WaitOne(FileOpenDelay))
        Assert.IsFalse(info.HasDuration)
		Assert.IsTrue(info.IsLoadingFailed)

        Assert.IsFalse(OpenedEvent.WaitOne(NoNewOpenDelay))
        Assert.IsFalse(FailedEvent.WaitOne(50))
    End Sub


	<TestMethod(), TestCategory("Duration")>
	Public Sub Duration_TwoFile_ErrorOk()
		' Get the file
		Dim info1 As New TestDurationItem(GetTestFilePath("WrongMp3.mp3"))
		Dim info2 As New TestDurationItem(GetTestFilePath("Electronic [Nokia].mp3"))

		' Action
		Dim inst = New TestDurationPlayer({False, True}, {3, 5})
		InterfaceMapper.SetInstance(Of IDurationPlayer)(inst)
		DurationLibrary.GetDuration(info1)
		DurationLibrary.GetDuration(info2)

		Assert.IsFalse(info1.HasDuration)
		Assert.IsFalse(info1.IsLoadingFailed)

		Assert.IsTrue(FailedEvent.WaitOne(500))
		Assert.IsFalse(info1.HasDuration)
		Assert.IsTrue(info1.IsLoadingFailed)
		Assert.IsFalse(info2.HasDuration)
		Assert.IsFalse(info2.IsLoadingFailed)

        Assert.IsTrue(OpenedEvent.WaitOne(FileOpenDelay))
        Assert.IsFalse(info2.IsLoadingFailed)
		Assert.IsTrue(info2.HasDuration)
		Assert.AreEqual(5.0, info2.Duration.TotalSeconds)

        Assert.IsFalse(OpenedEvent.WaitOne(NoNewOpenDelay))
        Assert.IsFalse(FailedEvent.WaitOne(50))
	End Sub

#End Region

End Class
