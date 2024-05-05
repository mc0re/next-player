Imports System.ComponentModel
Imports System.IO
Imports System.Threading
Imports System.Windows.Threading
Imports AudioChannelLibrary
Imports AudioPlayerLibrary
Imports Common
Imports NAudio.Wave


''' <summary>
''' Player using NAudio.
''' </summary>
''' <remarks>
''' A single player for the given source file and logical audio channel.
''' </remarks>
Public NotInheritable Class NAudioPlayer
    Implements IAudioPlayer

#Region " Events "

    Public Event MediaOpened(ByVal sender As Object, ByVal args As EventArgs) Implements IAudioPlayer.MediaOpened

    Public Event MediaFailed(ByVal sender As Object, ByVal args As MediaFailedEventArgs) Implements IAudioPlayer.MediaFailed

    Public Event MediaEnded(sender As Object, args As MediaEndedEventArgs) Implements IAudioPlayer.MediaEnded

#End Region


#Region " Fields "

    Private WithEvents mAudioConfig As IAudioEnvironmentStorage


    Private mFileName As String


    Private mNofSourceChannels As Integer


    Private mCoefficientFactory As ICoefficientGeneratorFactory


    ''' <summary>
    ''' A list of mini-players, one for each link in the logical channel setup.
    ''' Each such player outputs only into the defined physical channel,
    ''' the other channels are set to 0.
    ''' </summary>
    Private ReadOnly mInfoList As New List(Of AudioPlayerInfo)()


    ''' <summary>
    ''' All playback will be run on this thread.
    ''' </summary>
    Private mThread As Thread


    ''' <summary>
    ''' A dispatcher to access mThread.
    ''' </summary>
    Private mDispatcher As Dispatcher

#End Region


#Region " NaturalDuration read-only property "

    ''' <summary>
    ''' Does not return duration if no devices are attached.
    ''' </summary>
    Public ReadOnly Property NaturalDuration As Duration Implements IAudioPlayer.NaturalDuration
        Get
            Dim info = mInfoList.FirstOrDefault()
            Return New Duration(If(info?.Reader Is Nothing, TimeSpan.Zero, info.Reader.TotalTime))
        End Get
    End Property

#End Region


#Region " Channel read-only property "

    Private mLogicalChannelNr As Integer


    Public ReadOnly Property Channel As Integer Implements IAudioPlayer.Channel
        Get
            Return mLogicalChannelNr
        End Get
    End Property

#End Region


#Region " Position property "

    ''' <summary>
    ''' Gets or sets playback position.
    ''' </summary>
    ''' <remarks>
    ''' The player must be opened for this to have effect.
    ''' </remarks>
    Public Property Position As TimeSpan Implements IAudioPlayer.Position
        Get
            Dim info = mInfoList.FirstOrDefault()
            Return If(info Is Nothing OrElse info.Reader Is Nothing, TimeSpan.Zero, info.Reader.CurrentTime)
        End Get
        Set(value As TimeSpan)
            BeginExecute(Sub() SetPositionUnsafe(value))
        End Set
    End Property


    Private Sub SetPositionUnsafe(value As TimeSpan)
        For Each info In mInfoList
            info.Reader.CurrentTime = value
        Next
    End Sub

#End Region


#Region " PlaybackInfo property "

    Private WithEvents mPlaybackInfo As New AudioPlaybackInfo()


    ''' <summary>
    ''' When setting, set all underlying players with the same link.
    ''' </summary>
    Public Property PlaybackInfo As AudioPlaybackInfo Implements IAudioPlayer.PlaybackInfo
        Get
            Return mPlaybackInfo
        End Get
        Set(value As AudioPlaybackInfo)
            mPlaybackInfo = value
            CreateGeneratorFactory()

            Execute(Sub()
                        For Each info In mInfoList
                            info.Provider.PlaybackInfo = value
                        Next
                    End Sub)
        End Set
    End Property

#End Region


#Region " Init and clean-up "

    Public Sub New()
        mAudioConfig = InterfaceMapper.GetImplementation(Of IAudioEnvironmentStorage)()
        Dim dispatcherReadyEvent = New ManualResetEvent(False)

        mThread = New Thread(
            Sub()
                mDispatcher = Dispatcher.CurrentDispatcher
                dispatcherReadyEvent.Set()
                Dispatcher.Run()
            End Sub
        ) With {
            .Name = "Player thread",
            .IsBackground = True,
            .Priority = ThreadPriority.AboveNormal
        }

        mThread.Start()
        dispatcherReadyEvent.WaitOne()
    End Sub

#End Region


#Region " Event listeners "

    Private Sub PlaybackInfoChangedHandler(sender As Object, args As PropertyChangedEventArgs) _
    Handles mPlaybackInfo.PropertyChanged
        ' The channel will be set later, so ignore for now
        If Channel = 0 Then Return

        Select Case args.PropertyName
            Case NameOf(AudioPlaybackInfo.CoefficientGenerator)
                Return

            Case NameOf(AudioPlaybackInfo.PanningModel)
                CreateGeneratorFactory()

            Case Else
                CreateGenerator()
        End Select
    End Sub


    Private Sub AudioConfigChangedHandler(sender As Object, args As PropertyChangedEventArgs) _
    Handles mAudioConfig.PropertyChanged
        CreateGeneratorFactory()
    End Sub

#End Region


#Region " Utility "

    ''' <summary>
    ''' Execute the given action on the <see cref="mDispatcher"/> thread some time later.
    ''' </summary>
    Private Sub BeginExecute(act As Action)
        If mDispatcher Is Nothing Then Return
        mDispatcher.BeginInvoke(act)
    End Sub


    ''' <summary>
    ''' Create a new factory when the panning model is changed,
    ''' or the audio environment is changed.
    ''' 
    ''' Also create the actual generator.
    ''' </summary>
    Private Sub CreateGeneratorFactory()
        If mLogicalChannelNr = 0 Then Return

        Select Case mPlaybackInfo.PanningModel
            Case PanningModels.Coordinates
                mCoefficientFactory = New PositionCoefficientGeneratorFactory(
                    mLogicalChannelNr, mAudioConfig, mNofSourceChannels)

            Case Else
                mCoefficientFactory = New PanningCoefficientGeneratorFactory(
                    mLogicalChannelNr, mAudioConfig, mNofSourceChannels)
        End Select

        CreateGenerator()
    End Sub


    ''' <summary>
    ''' Update <see cref="AudioPlaybackInfo.CoefficientGenerator"/> with the new generator,
    ''' corresponding to the changes in the playback information.
    ''' </summary>
    Private Sub CreateGenerator()
        mPlaybackInfo.CoefficientGenerator = mCoefficientFactory.Create(mPlaybackInfo)
    End Sub

#End Region


#Region " Data API "

    Public Sub Open(fileName As String, channelNo As Integer) Implements IAudioPlayer.Open
        BeginExecute(Sub() OpenUnsafe(fileName, channelNo))
    End Sub


    Public Sub Close() Implements IAudioPlayer.Close
        BeginExecute(Sub() CloseUnsafe(False))
    End Sub


    Public Sub PlayAndForget(strm As Stream, channelNo As Integer) Implements IVoicePlayer.PlayAndForget
        BeginExecute(Sub() PlayUnsafe(strm, channelNo))
    End Sub


    ''' <summary>
    ''' Execute the given action on the <see cref="mDispatcher"/> thread and wait for it to finish.
    ''' </summary>
    Public Sub Execute(action As Action) Implements IAudioPlayer.Execute
        If mDispatcher Is Nothing Then Return

        Try
            mDispatcher.Invoke(action)
        Catch ex As Exception
            ' Ignore
        End Try
    End Sub

#End Region


#Region " Data methods "

    ''' <summary>
    ''' Open the given file as audio file.
    ''' </summary>
    Private Sub OpenUnsafe(fileName As String, channelNo As Integer)
        CloseUnsafe(False)
        Dim streamProvider = InterfaceMapper.GetImplementation(Of IInputStreamProvider)()

        Try
            Using f = streamProvider.CreateStream(fileName)
                mNofSourceChannels = f.WaveFormat.Channels
            End Using

            If channelNo = 0 Then
                channelNo = 1
            End If

            mLogicalChannelNr = channelNo
            CreateGeneratorFactory()
            CollectLinks(Function() streamProvider.CreateStream(fileName))

            mFileName = fileName
            RaiseEvent MediaOpened(Me, EventArgs.Empty)

        Catch ex As Exception
            CloseUnsafe(False)
            RaiseEvent MediaFailed(Me, New MediaFailedEventArgs With {.FileName = fileName, .Reason = ex.Message})
        End Try
    End Sub


    ''' <summary>
    ''' Close the currently opened file.
    ''' If <paramref name="dispose"/>, also clean up the player and dispatcher.
    ''' </summary>
    ''' <param name="dispose">Whether to dispose the player and other resources.</param>
    Private Sub CloseUnsafe(dispose As Boolean)
        For Each info In mInfoList
            If info.Reader IsNot Nothing Then
                Try
                    info.Reader.Dispose()
                Catch
                    ' Sometimes a NullReferenceException occurs. Ignore.
                End Try

                info.Reader = Nothing
            End If

            If dispose AndAlso info.Player IsNot Nothing Then
                info.Player.Stop()
                RemoveHandler info.Player.PlaybackStopped, AddressOf PlaybackStoppedHandler
                info.Player.Dispose()
                info.Player = Nothing
            End If

            info.Provider = Nothing
        Next

        If dispose Then
            mInfoList.Clear()
        End If

        If dispose AndAlso mDispatcher IsNot Nothing Then
            mDispatcher.InvokeShutdown()
            mDispatcher = Nothing
            'mThread.Abort()
            mThread = Nothing
        End If

        mFileName = Nothing
    End Sub


    Private Sub PlayUnsafe(strm As Stream, channelNo As Integer)
        If channelNo = 0 Then
            channelNo = 1
        End If

        Try
            mLogicalChannelNr = channelNo
            mNofSourceChannels = 1
            mFileName = "Synthesized"
            CreateGeneratorFactory()

            ' The WaveFormat must match the generated one in SpeechSynthesizerControl
            CollectLinks(Function() New RawSourceWaveStream(strm, New WaveFormat(44100, 16, 1)))
            Play()

            RaiseEvent MediaOpened(Me, EventArgs.Empty)

        Catch ex As Exception
            CloseUnsafe(False)
            RaiseEvent MediaFailed(Me, New MediaFailedEventArgs With {.FileName = mFileName, .Reason = ex.Message})
        End Try
    End Sub


    ''' <summary>
    ''' Collect all assigned links and connected physical channels.
    ''' For each physical channel create a player with a shared mPlaybackInfo.
    ''' </summary>
    ''' <param name="generateStreamFn">
    ''' A function to generate a <see cref="WaveStream"/> for each linked physical channel.
    ''' </param>
    Private Sub CollectLinks(generateStreamFn As Func(Of WaveStream))
        If mInfoList.IsEmpty() Then
            For Each chOutput In mAudioConfig.GetLinks(mLogicalChannelNr)
                Dim ph = chOutput.Physical
                Dim mpl = ph.AudioInterface.CreatePlayer()

                ' Error creating a player, skip
                If mpl Is Nothing Then Continue For

                AddHandler mpl.PlaybackStopped, AddressOf PlaybackStoppedHandler
                mInfoList.Add(New AudioPlayerInfo With {
                              .Channel = ph, .Link = chOutput.Link, .Player = mpl
                              })
            Next
        End If

        For Each info In mInfoList
            info.HasStopped = False
            info.Reader = generateStreamFn.Invoke()
            info.Provider = New VolumeProvider(
                    info.Reader.ToSampleProvider(), mPlaybackInfo, info.Channel, info.Link)
            info.Player.Init(info.Provider)
        Next
    End Sub

#End Region


#Region " Playback API "

    Public Sub Play() Implements IAudioPlayer.Play
        BeginExecute(
            Sub()
                For Each info In mInfoList
                    info.Player.Play()
                Next
            End Sub)
    End Sub


    Public Sub Pause() Implements IAudioPlayer.Pause
        BeginExecute(
            Sub()
                For Each info In mInfoList
                    info.Player.Pause()
                Next
            End Sub)
    End Sub


    Public Sub [Stop]() Implements IAudioPlayer.Stop
        BeginExecute(
            Sub()
                For Each info In mInfoList
                    If info.Player IsNot Nothing Then
                        info.Player.Stop()
                    End If
                Next
            End Sub)
    End Sub

#End Region


#Region " Event listeners "

    ''' <summary>
    ''' Raise the MediaEnded event, when all players have stopped.
    ''' </summary>
    Private Sub PlaybackStoppedHandler(sender As Object, args As StoppedEventArgs)
        Dim pl = CType(sender, IWavePlayer)
        Dim info = mInfoList.FirstOrDefault(Function(i) i.Player Is pl)

        info.HasStopped = True

        If Not mInfoList.Any(Function(i) Not i.HasStopped) Then
            Dim endArgs As New MediaEndedEventArgs With {.FileName = mFileName}

            If args.Exception IsNot Nothing Then
                endArgs.ErrorMessage = args.Exception.Message
                ' Maybe, send MediaFailed in this case?
            End If

            RaiseEvent MediaEnded(Me, endArgs)
        End If
    End Sub

#End Region


#Region " IDisposable implementation "

    Public Sub Dispose() Implements IDisposable.Dispose
        Execute(Sub() CloseUnsafe(True))
    End Sub

#End Region

End Class
