Imports System.Threading
Imports AudioChannelLibrary
Imports Common
Imports NextPlayer


Module PlaybackTestUtility

#Region " Constants "

    Public Const VolumePrecision = 0.0001F

#End Region


#Region " API "

    ''' <summary>
    ''' Test the produced numbers.
    ''' </summary>
    ''' <param name="setup">Set up object</param>
    ''' <param name="linkDef">A string defining the links' parameters</param>
    ''' <param name="inputChannels">The number of input channels</param>
    ''' <param name="inputDef">Input samples definition</param>
    ''' <param name="outputChannes">The number of output channels</param>
    ''' <param name="outputDef">Output samples definition</param>
    Friend Sub TestPlayback(
        setup As PlayerSetup,
        linkDef As String,
        inputChannels As Integer, inputDef As String, outputChannes As Integer,
        ParamArray outputDef As String()
    )
        Select Case outputChannes
            Case 1
                InterfaceMapper.SetType(Of IAudioOutputInterface, TestMonoOutputInterface)()
            Case 2
                InterfaceMapper.SetType(Of IAudioOutputInterface, TestStereoOutputInterface)()
            Case 3
                InterfaceMapper.SetType(Of IAudioOutputInterface, TestTripleOutputInterface)()
            Case Else
                Throw New ArgumentException("Unsupported number of output channels")
        End Select

        Dim dw As New TestAudioWriter()
        Dim storage As New TestAudioEnvironmentStorage(linkDef)
        Assert.AreEqual(outputDef.Length, storage.Links.Count)

        InterfaceMapper.SetInstance(Of IInputStreamProvider)(
            New TestInputStreamProvider(inputChannels, inputDef))
        InterfaceMapper.SetInstance(Of IAudioEnvironmentStorage)(storage)
        InterfaceMapper.SetInstance(Of ITestDataWriter)(dw)

        Dim okEvent As New AutoResetEvent(False)
        Dim failedEvent As New AutoResetEvent(False)
        Dim finishedEvent As New AutoResetEvent(False)

        Dim pl As New NAudioPlayer()
        pl.PlaybackInfo.Volume = setup.Volume
        pl.PlaybackInfo.PanningModel = setup.PanningModel
        pl.PlaybackInfo.Panning = setup.PlaybackPanning
        pl.PlaybackInfo.IsMuted = setup.Mute

        Dim errMessage As String = ""
        AddHandler pl.MediaOpened, Sub(obj, args) okEvent.Set()
        AddHandler pl.MediaFailed, Sub(obj, args)
                                       errMessage = args.Reason
                                       failedEvent.Set()
                                   End Sub
        AddHandler pl.MediaEnded, Sub(obj, args) finishedEvent.Set()

        pl.Open("Any name.wav", storage.Logical(0).Channel)

        Dim waitRes = WaitHandle.WaitAny({okEvent, failedEvent}, 5000)
        Assert.AreEqual(0, waitRes, errMessage)

        pl.Play()
        Assert.IsTrue(finishedEvent.WaitOne(60000))

        pl.Close()

        For outIdx = 0 To outputDef.Length - 1
            Dim ch = storage.Physical(outIdx)
            Assert.IsNotNull(dw.DataBuffer(ch.Channel),
                             $"Missing channel {ch.Channel} - exist channels {String.Join(", ", dw.Channels)}.")
            CompareStreams(
                New TestDataBuffer(outputDef(outIdx)).Data,
                dw.DataBuffer(ch.Channel).Data, $"output channel {ch.Channel}")
        Next

        Assert.IsNull(dw.DataBuffer(outputDef.Length + 1), "More outputs are defined than expected.")
    End Sub

#End Region


#Region " Utility "

    Private Sub CompareStreams(expected As IList(Of Single), actual As IList(Of Single), text As String)
        Assert.AreEqual(expected.Count, actual.Count, "Stream size.")

        For idx = 0 To expected.Count - 1
            Assert.AreEqual(expected(idx), actual(idx), VolumePrecision, $"Value #{idx} of {text}")
        Next
    End Sub

#End Region

End Module
