Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Globalization
Imports System.IO
Imports System.Reflection
Imports System.Threading
Imports System.Windows.Controls.Primitives
Imports System.Xml.Serialization
Imports AudioChannelLibrary
Imports AudioPlayerLibrary
Imports Common
Imports PlayerActions
Imports Serilog
Imports TextChannelLibrary
Imports TextWindowLibrary
Imports VoiceControlLibrary
Imports WpfResources


Class PlayerWindow
    Inherits Window

#Region " Constants "

    Private Const PlaylistExtensionFilter = "NexTPlayer playlists (.tpl)|*.tpl|All files (.*)|*.*"


    Private Const AudiofileExtensionFilter =
        "MP3 files (.mp3)|*.mp3|WAV files (.wav)|*.wav|Audio files|*.mp3;*.wav;*.aiff|All files (.*)|*.*"

    Private Const AudiofileDefaultExtension = ".mp3"


    Friend Const PresentationExtensionFilter = "PowerPoint files (.pptx)|*.pptx|All files (.*)|*.*"

    Friend Const PresentationDefaultExtension = ".pptx"


    Private Shared ReadOnly mVoiceRecognitionCulture As New CultureInfo("en-GB")

#End Region


#Region " Fields "

    ''' <summary>
    ''' Keep track of the opened dialogs to close them upon exit.
    ''' </summary>
    Private ReadOnly mDialogWindows As New List(Of Window)()


    ''' <summary>
    ''' Speech recognition controller.
    ''' </summary>
    Private WithEvents mVoiceControl As SpeechRecognitionControl


    Private mSynthesizer As ISpeechSynthesizer


    ''' <summary>
    ''' Audio manager to handle multiple playbacks.
    ''' </summary>
    Private WithEvents mAudioMgr As IAudioManager


    ''' <summary>
    ''' Logger.
    ''' </summary>
    Private ReadOnly mLogger As ILogger

#End Region


#Region " Properties "

#Region " AppName read-only property "

    Public ReadOnly Property AppName As String
        Get
            Return Assembly.GetExecutingAssembly().GetCustomAttribute(Of AssemblyProductAttribute)().Product
        End Get
    End Property

#End Region


#Region " ActionList read-only dependency property "

    Private Shared ReadOnly ActionListPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(ActionList), GetType(PlayerActionCollection), GetType(PlayerWindow),
        New PropertyMetadata(Nothing, New PropertyChangedCallback(AddressOf ActionListChanged)))


    Public Shared ReadOnly ActionListProperty As DependencyProperty = ActionListPropertyKey.DependencyProperty


    <Category("Common Properties"), Description("Currently loaded playlist")>
    Public Property ActionList As PlayerActionCollection
        Get
            Return CType(GetValue(ActionListProperty), PlayerActionCollection)
        End Get
        Private Set(value As PlayerActionCollection)
            AppConfiguration.Instance.CurrentActionCollection = value
            SetValue(ActionListPropertyKey, value)
        End Set
    End Property


    Private WithEvents mActionList As PlayerActionCollection = CType(ActionListProperty.DefaultMetadata.DefaultValue, PlayerActionCollection)


    Private Shared Sub ActionListChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
        Dim this = CType(obj, PlayerWindow)
        Dim value = CType(args.NewValue, PlayerActionCollection)

        this.mActionList = value
        this.mAudioMgr.SetPlaylist(value)

        If this.mVoiceControl IsNot Nothing Then
            this.mVoiceControl.Restart(value.MaxParallels, value.Items.Count)
        End If
    End Sub

#End Region


#Region " IsListModified read-only dependency property "

    Private Shared ReadOnly IsListModifiedPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(IsListModified), GetType(Boolean), GetType(PlayerWindow),
        New PropertyMetadata(False))


    Public Shared ReadOnly IsListModifiedProperty As DependencyProperty = IsListModifiedPropertyKey.DependencyProperty


    <Category("Common Properties"), Description("Whether the playlist was modified since last save")>
    Public Property IsListModified As Boolean
        Get
            Return CBool(GetValue(IsListModifiedProperty))
        End Get
        Private Set(value As Boolean)
            SetValue(IsListModifiedPropertyKey, value)
        End Set
    End Property

#End Region


#Region " ActiveMainAction read-only dependency property "

    Private Shared ReadOnly ActiveMainActionPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(ActiveMainAction), GetType(IPlayerAction), GetType(PlayerWindow),
        New PropertyMetadata(Nothing))


    Public Shared ReadOnly ActiveMainActionProperty As DependencyProperty = ActiveMainActionPropertyKey.DependencyProperty


    <Category("Common Properties"), Description("Currently active action on the main line")>
    Public Property ActiveMainAction As IPlayerAction
        Get
            Return CType(GetValue(ActiveMainActionProperty), IPlayerAction)
        End Get
        Private Set(value As IPlayerAction)
            SetValue(ActiveMainActionPropertyKey, value)
        End Set
    End Property

#End Region


#Region " ActiveTime read-only dependency property "

    Private Shared ReadOnly ActiveTimePropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(ActiveTime), GetType(TimeSpan), GetType(PlayerWindow),
        New PropertyMetadata(New TimeSpan()))


    Public Shared ReadOnly ActiveTimeProperty As DependencyProperty = ActiveTimePropertyKey.DependencyProperty


    <Category("Common Properties"), Description("Playback time")>
    Public Property ActiveTime As TimeSpan
        Get
            Return CType(GetValue(ActiveTimeProperty), TimeSpan)
        End Get
        Private Set(value As TimeSpan)
            SetValue(ActiveTimePropertyKey, value)
        End Set
    End Property

#End Region


#Region " ActiveMainProducer read-only dependency property "

    Private Shared ReadOnly ActiveMainProducerPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(ActiveMainProducer), GetType(IPlayerAction), GetType(PlayerWindow),
        New PropertyMetadata(Nothing))


    Public Shared ReadOnly ActiveMainProducerProperty As DependencyProperty = ActiveMainProducerPropertyKey.DependencyProperty


    <Category("Common Properties"), Description("Current producer on the main line")>
    Public Property ActiveMainProducer As IPlayerAction
        Get
            Return CType(GetValue(ActiveMainProducerProperty), IPlayerAction)
        End Get
        Private Set(value As IPlayerAction)
            SetValue(ActiveMainProducerPropertyKey, value)
        End Set
    End Property

#End Region


#Region " ManualParallels read-only dependency property "

    Private Shared ReadOnly ManualParallelsPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(ManualParallels), GetType(ICollection(Of IPlayerAction)), GetType(PlayerWindow),
        New PropertyMetadata(Enumerable.Empty(Of IPlayerAction), New PropertyChangedCallback(AddressOf ManualParallelsChanged)))


    Public Shared ReadOnly ManualParallelsProperty As DependencyProperty = ManualParallelsPropertyKey.DependencyProperty


    <Category("Common Properties"), Description("Available manual parallel actions")>
    Public Property ManualParallels As ICollection(Of IPlayerAction)
        Get
            Return CType(GetValue(ManualParallelsProperty), ICollection(Of IPlayerAction))
        End Get
        Private Set(value As ICollection(Of IPlayerAction))
            SetValue(ManualParallelsPropertyKey, value)
        End Set
    End Property


    Private Shared Sub ManualParallelsChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
        Dim oldList = TryCast(args.OldValue, ICollection(Of IPlayerAction))
        Dim newList = TryCast(args.NewValue, ICollection(Of IPlayerAction))

        For Each oldItem In oldList.Except(newList)
            oldItem.IsActiveParallel = False
        Next

        For Each newItem In newList
            newItem.IsActiveParallel = True
        Next
    End Sub

#End Region


#Region " ReplayAction read-only dependency property "

    Private Shared ReadOnly ReplayActionPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(ReplayAction), GetType(IPlayerAction), GetType(PlayerWindow),
        New PropertyMetadata(Nothing))


    Public Shared ReadOnly ReplayActionProperty As DependencyProperty = ReplayActionPropertyKey.DependencyProperty


    <Category("Common Properties"), Description("Action for Play Again command")>
    Public Property ReplayAction As IPlayerAction
        Get
            Return CType(GetValue(ReplayActionProperty), IPlayerAction)
        End Get
        Private Set(value As IPlayerAction)
            SetValue(ReplayActionPropertyKey, value)
        End Set
    End Property

#End Region


#Region " NextAction read-only dependency property "

    Private Shared ReadOnly NextActionPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(NextAction), GetType(IPlayerAction), GetType(PlayerWindow),
        New PropertyMetadata(Nothing, New PropertyChangedCallback(AddressOf NextActionChanged)))


    Public Shared ReadOnly NextActionProperty As DependencyProperty = NextActionPropertyKey.DependencyProperty


    <Category("Common Properties"), Description("Action to be executed next")>
    Public Property NextAction As IPlayerAction
        Get
            Return CType(GetValue(NextActionProperty), IPlayerAction)
        End Get
        Private Set(value As IPlayerAction)
            SetValue(NextActionPropertyKey, value)
        End Set
    End Property


    Private Shared Sub NextActionChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
        Dim oldNext = CType(args.OldValue, IPlayerAction)
        If oldNext IsNot Nothing Then
            oldNext.IsNext = False
        End If

        Dim newNext = CType(args.NewValue, IPlayerAction)
        If newNext IsNot Nothing Then
            newNext.IsNext = True
            Dim this = CType(obj, PlayerWindow)
            this.Playlist.BringItemToView(newNext)
        End If
    End Sub

#End Region


#Region " FollowActionType dependency property "

    Private Shared ReadOnly FollowActionTypeProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(FollowActionType), GetType(FollowActionTypes), GetType(PlayerWindow),
        New PropertyMetadata(New PropertyChangedCallback(AddressOf FollowActionTypeChanged)))


    <Category("Common Properties"), Description("Whether the details view follows the playlist during playback")>
    Public Property FollowActionType As FollowActionTypes
        Get
            Return CType(GetValue(FollowActionTypeProperty), FollowActionTypes)
        End Get
        Private Set(value As FollowActionTypes)
            SetValue(FollowActionTypeProperty, value)
        End Set
    End Property


    Private Shared Sub FollowActionTypeChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
        Dim this = CType(obj, PlayerWindow)
        this.AssignFollowedAction()
        this.SetPlaylistInFocus()
    End Sub


    ''' <summary>
    ''' Set the followed action.
    ''' </summary>
    Private Sub AssignFollowedAction()
        Select Case FollowActionType
            Case FollowActionTypes.ActiveAction
                ActionControl.Action = ActiveMainAction

            Case FollowActionTypes.ActiveProducer
                ActionControl.Action = ActiveMainProducer

            Case FollowActionTypes.NextAction
                ActionControl.Action = NextAction
        End Select
    End Sub

#End Region


#Region " IsSoundComing read-only dependency property "

    Private Shared ReadOnly IsSoundComingPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(IsSoundComing), GetType(Boolean), GetType(PlayerWindow),
        New PropertyMetadata(False))


    Public Shared ReadOnly IsSoundComingProperty As DependencyProperty = IsSoundComingPropertyKey.DependencyProperty


    <Category("Common Properties"), Description("Whether any sound is coming to speech recognizer")>
    Public Property IsSoundComing As Boolean
        Get
            Return CType(GetValue(IsSoundComingProperty), Boolean)
        End Get
        Private Set(value As Boolean)
            SetValue(IsSoundComingPropertyKey, value)
        End Set
    End Property

#End Region


#Region " MessageLog property "

    Private mMessageLog As IMessageLog


    Private ReadOnly Property MessageLog As IMessageLog
        Get
            If mMessageLog Is Nothing Then
                mMessageLog = InterfaceMapper.GetImplementation(Of IMessageLog)(True)
            End If

            Return mMessageLog
        End Get
    End Property

#End Region

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Process configuration options.
    ''' </summary>
    Public Sub New()
        mLogger = InterfaceMapper.GetImplementation(Of ILogger)()
        InterfaceMapper.SetInstance(Of ISpeechSynthesizer)(New SpeechSynthesizerControl)

        ' Assume that configuration files are loaded
        AppConfiguration.SetUpAudioLib()
        AppConfiguration.SetUpTextLib()
        InterfaceMapper.SetInstance(Of IConfiguration)(AppConfiguration.Instance)
        InterfaceMapper.SetInstance(Of IVolumeConfiguration)(AppConfiguration.Instance)
        InterfaceMapper.SetInstance(Of IEffectDurationConfiguration)(AppConfiguration.Instance)
        InterfaceMapper.SetInstance(Of IPresenterStaticConfiguration)(AppConfiguration.Instance)
        InterfaceMapper.SetInstance(Of IInputStreamProvider)(FileCache.Instance)
        InterfaceMapper.SetInstance(Of ITimeService)(New TimeService())
        InterfaceMapper.SetInstance(Of IEffectGenerator)(New EffectGenerator())

        ' Initialize audio library
        mAudioMgr = New AudioManager(AppConfiguration.Instance.PlaybackTick, ActionList)

        ' Initally empty playlist - to avoid a lot of Null-checks
        ActionList = PlayerActionCollection.CreateEmpty()

        ' This call is required by the designer.
        InitializeComponent()
    End Sub


    ''' <summary>
    ''' Start up non-UI modules such as license module, voice recognition.
    ''' </summary>
    Private Sub OnWindowInitialized() Handles Me.Initialized

        ' Set up interface mappings
        InterfaceMapper.SetInstance(Of IMessageLog)(MessageLogger)
        InterfaceMapper.SetInstance(Of IDurationPlayer)(New NAudioPlayer())

        ' Start recognizer
        mVoiceControl = New SpeechRecognitionControl(Me)

        ' Add extra serialized types
        PlayerActionCollection.AddSerializedType(GetType(RenderTextInterface))
    End Sub


    ''' <summary>
    ''' Start speech recognition engine.
    ''' Load last file.
    ''' </summary>
    Private Sub LoadedHandler() Handles Me.Loaded
        Thread.CurrentThread.CurrentCulture = mVoiceRecognitionCulture
        Thread.CurrentThread.CurrentUICulture = mVoiceRecognitionCulture

        Dim fileToOpen = AppConfiguration.Instance.LastPlaylistFile

        Dim cmdArgs = Environment.GetCommandLineArgs().Skip(1).ToList()
        If cmdArgs.Any() Then
            fileToOpen = cmdArgs.First()
        End If

        Try
            If Not String.IsNullOrWhiteSpace(fileToOpen) AndAlso Not LoadPlaylist(fileToOpen) Then
                AppConfiguration.Instance.LastPlaylistFile = String.Empty
            End If

        Catch ex As Exception
            MessageBox.Show(String.Format("Error when loading file '{1}'.{0}{2}",
                                          vbCrLf, fileToOpen, ex.Message),
                            AppConfiguration.AppName,
                            MessageBoxButton.OK, MessageBoxImage.Error)
        End Try

        SetPlaylistInFocus()

        Dim vc = InterfaceMapper.GetImplementation(Of IVoiceConfiguration)()
        If vc.IsVoiceControlEnabled Then
            mVoiceControl.StartListening(ActionList.MaxParallels, ActionList.Items.Count)
        End If
    End Sub


    ''' <summary>
    ''' Clean-up.
    ''' </summary>
    Private Sub OnWindowClosing() Handles Me.Closing
        SaveSilentlyOrAsk(True)

        AppConfiguration.Instance.SaveSettings()
        mVoiceControl.StopListening()
        mVoiceControl.Dispose()
        mAudioMgr.Reset()
        InterfaceMapper.GetImplementation(Of IDurationPlayer)().Dispose()
        DurationLibrary.Shutdown()
        WaveformStorage.Cleanup()
    End Sub

#End Region


#Region " Background saving utility "

    Private WithEvents mSaveTimer As New Timers.Timer() With {.AutoReset = False}

    ''' <summary>
    ''' If the playlist is assigned to a file, save it.
    ''' Otherwise, ask for file name.
    ''' </summary>
    Private Sub SaveSilentlyOrAsk(force As Boolean)
        Dispatcher.BeginInvoke(
            Sub()
                If My.Settings.SavePlaylistOnChange Then
                    If Not ActionList.DoesNameExist OrElse String.IsNullOrWhiteSpace(AppConfiguration.Instance.LastPlaylistFile) Then
                        SavePlaylistInteractive()
                    ElseIf force Then
                        mSaveTimer.Stop()
                        If ActionList.IsSaveNeeded Then
                            SavePlaylist(AppConfiguration.Instance.LastPlaylistFile)
                        End If
                    Else
                        mSaveTimer.Interval = AppConfiguration.Instance.SaveTick
                        mSaveTimer.Start()
                    End If
                End If
            End Sub)
    End Sub


    Private Sub SaveTimerElapsedHandler() Handles mSaveTimer.Elapsed
        SaveSilentlyOrAsk(True)
    End Sub

#End Region


#Region " Command utility "

    ''' <summary>
    ''' Get the currently active item - from the command parameter
    ''' or from the active pane.
    ''' </summary>
    ''' <typeparam name="T">Return type</typeparam>
    ''' <param name="obj">Supplied bound command parameter</param>
    Private Function GetSelectedItem(Of T As Class)(obj As Object) As T
        ' Parameter should be the action being modified.
        Dim item = TryCast(obj, T)

        ' If not, it should be the action being viewed in the details pane.
        ' This is the case when a voice command is executed.
        If item Is Nothing Then item = TryCast(ActionControl.Action, T)

        Return item
    End Function

#End Region


#Region " Playlist load and save commands "

    ''' <summary>
    ''' Check, whether any modification was made.
    ''' If so, ask the user, whether the changes shall be saved.
    ''' If so, open the Save dialog.
    ''' </summary>
    ''' <returns>
    ''' True if the content replacement modification can be performed.
    ''' </returns>
    Private Function CanProceedWithReplacement() As Boolean
        If Not IsListModified Then Return True

        Dim dlg As New ContentModifiedWindow(Me) With {
            .ContentName = Title
        }
        dlg.ShowDialog()

        ' Cancelled pressed, do not replace / close
        If dlg.DialogResult <> True Then Return False

        If dlg.ShallSave Then
            ' Saving cancelled, do not replace / close
            If Not SavePlaylistInteractive() Then Return False
        End If

        Return True
    End Function


    ''' <summary>
    ''' Load the given playlist.
    ''' </summary>
    ''' <param name="fileName">
    ''' File name to load. If succeeded, the name is written into the configuration file.
    ''' </param>
    Private Function LoadPlaylist(fileName As String) As Boolean
        FileCache.Instance.Clear()
        MessageLog.ClearLog("Loading " & fileName, "Loading playlist")

        ActionList = PlayerActionCollection.LoadFromFile(fileName)
        If ActionList Is Nothing Then
            mLogger.Information($"Loaded playlist '{fileName}', empty.")
            Return False
        End If

        mLogger.Information($"Loaded playlist '{fileName}', {ActionList.Items.Count} items.")
        IsListModified = False
        ResetPlayer()
        AppConfiguration.Instance.LastPlaylistFile = fileName

        Return True
    End Function


    ''' <summary>
    ''' Returns False if loading failed or was cancelled.
    ''' </summary>
    Private Sub LoadPlaylistInteractive()
        Dim dlg As New Microsoft.Win32.OpenFileDialog() With {
            .DefaultExt = PlayerActionCollection.PlaylistDefaultExtension,
            .Filter = PlaylistExtensionFilter
        }

        ' Show open file dialog box 
        Dim result? As Boolean = dlg.ShowDialog()

        ' Process open file dialog box results 
        If result <> True Then Return

        LoadPlaylist(dlg.FileName)
    End Sub


    ''' <summary>
    ''' Save the current playlist to the given file.
    ''' </summary>
    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CC0004:Catch block cannot be empty", Justification:="<Pending>")>
    Private Function SavePlaylist(fileName As String) As Boolean
        Try
            If File.Exists(fileName) Then
                Dim backupName As String = fileName + ".bak"
                File.Delete(backupName)
                File.Move(fileName, backupName)
            End If

            Using writer = File.Open(fileName, FileMode.Create, FileAccess.Write)
                ActionList.Save(writer, fileName)

                If IsListModified Then
                    IsListModified = False
                    mLogger.Information($"Saved playlist '{fileName}'.")
                End If

                Return True
            End Using

        Catch ex As Exception
            MessageLogger.LogFileError("Error saving playlist: {0}", ex.Message)
            mLogger.Warning(ex, $"Saving playlist '{fileName}' failed.")
        End Try

        Return False
    End Function


    ''' <summary>
    ''' Return False if saving was cancelled or failed.
    ''' </summary>
    Private Function SavePlaylistInteractive() As Boolean
        Static isSaving As Boolean = False

        ' Reentrance protection
        If isSaving Then Return False
        isSaving = True

        ' Configure save file dialog box
        Dim dlg As New Microsoft.Win32.SaveFileDialog() With {
            .FileName = ActionList.Name,
            .DefaultExt = PlayerActionCollection.PlaylistDefaultExtension,
            .Filter = PlaylistExtensionFilter
        }

        ' Show save file dialog box
        Dim result? As Boolean = dlg.ShowDialog()
        isSaving = False

        ' Process save file dialog box results 
        If result <> True Then Return False

        AppConfiguration.Instance.LastPlaylistFile = dlg.FileName
        Return SavePlaylist(dlg.FileName)
    End Function


    Private Sub LoadPlaylistCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        If Not CanProceedWithReplacement() Then Return

        LoadPlaylistInteractive()
        SetPlaylistInFocus()
    End Sub


    Private Sub SavePlaylistCommandCanExecute(sender As Object, args As CanExecuteRoutedEventArgs)
        args.CanExecute = True
    End Sub


    Private Sub SavePlaylistCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        SavePlaylistInteractive()
        SetPlaylistInFocus()
    End Sub

#End Region


#Region " Playlist modification utility "

    ''' <summary>
    ''' Upon an action property change, save the playlist if required.
    ''' </summary>
    ''' <remarks>
    ''' Can come from a non-UI thread.
    ''' </remarks>
    Private Sub ActionListPropertyChangedHandler(sender As Object, args As PropertyChangedEventArgs) Handles mActionList.PropertyChanged
        Dim pargs = CType(args, PlayerNotifyPropertyChangedEventArgs)
        If pargs.PropInfo.CustomAttributes.OfType(Of XmlIgnoreAttribute)().Any() Then Return

        Dispatcher.BeginInvoke(Sub()
                                   If My.Settings.SavePlaylistOnChange Then
                                       SaveSilentlyOrAsk(False)
                                   Else
                                       IsListModified = True
                                   End If
                               End Sub)
    End Sub


    ''' <summary>
    ''' Upon any change, save the playlist if required and adjust it.
    ''' </summary>
    ''' <remarks>
    ''' Can come from a non-UI thread.
    ''' </remarks>
    Private Sub ActionListChangedHandler(sender As Object, args As NotifyCollectionChangedEventArgs) Handles mActionList.CollectionChanged
        Dim pargs = CType(args, PlayerNotifyCollectionChangedEventArgs)

        Dispatcher.BeginInvoke(
            Sub()
                If ActionList.IsSaveNeeded AndAlso PlayerActionCollection.CanAffectXml(pargs) Then
                    If My.Settings.SavePlaylistOnChange Then
                        SaveSilentlyOrAsk(False)
                    Else
                        IsListModified = True
                    End If
                End If

                If PlayerActionCollection.CanAffectStructure(pargs) Then
                    mVoiceControl.Restart(ActionList.MaxParallels, ActionList.Items.Count)
                End If
            End Sub)
    End Sub


    ''' <summary>
    ''' If idx is greater or equal to zero, add the item at the specified index
    ''' (that corresponds to inserting it before the cursor).
    ''' If less than zero, add to the end (no selection).
    ''' </summary>
    Private Sub AddItemToList(idx As Integer, act As PlayerAction)
        If idx < 0 Then
            ActionList.Items.Add(act)
        Else
            ActionList.Items.Insert(idx, act)
        End If

        Playlist.SelectedItem = act
    End Sub


    ''' <summary>
    ''' Returns Trus if the list was modified.
    ''' </summary>
    Private Function AddFilesToPlaylist() As Boolean
        Dim dlg As New Microsoft.Win32.OpenFileDialog() With {
            .DefaultExt = AudiofileDefaultExtension,
            .Filter = AudiofileExtensionFilter,
            .Multiselect = True
        }

        ' Show open file dialog box 
        Dim result? As Boolean = dlg.ShowDialog()

        ' Process open file dialog box results 
        If result <> True Then Return False

        Dim curIdx = Playlist.SelectedIndex

        For Each act In From fileName In dlg.FileNames Select New PlayerActionFile(fileName)
            act.AfterLoad(String.Empty)
            AddItemToList(curIdx, act)
            mLogger.Information($"Added file item '{act.FileToPlay} at index {curIdx}'.")
            curIdx += 1
        Next

        Return True
    End Function


    ''' <summary>
    ''' Add a simple action to the playlist.
    ''' </summary>
    Private Sub AddActionToPlaylist(action As PlayerAction)
        AddItemToList(Playlist.SelectedIndex, action)
        mLogger.Information($"Added item '{action.Name} at index {Playlist.SelectedIndex}'.")
    End Sub

#End Region


#Region " Playlist modification commands: move, remove, change elements "

    ''' <summary>
    ''' List modifications cannot be executed,
    ''' if there is no selection, or no license.
    ''' </summary>
    Private Sub ListModificationCommandCanExecute(sender As Object, args As CanExecuteRoutedEventArgs)
        HasSelectionCommandCanExecute(args)
    End Sub


    Private Sub DeleteFileCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim delIdx = Playlist.SelectedIndex
        Dim delList = New List(Of PlayerAction)(Playlist.SelectedItems.Cast(Of PlayerAction)())
        Dim delNames = String.Join(vbCrLf, From di In delList Select di.Name)
        Dim str = String.Format("Delete this action?{0}{0}{1}", vbCrLf, delNames)

        If MessageBox.Show(str, AppConfiguration.AppName, MessageBoxButton.YesNo) <> MessageBoxResult.Yes Then
            Return
        End If

        For Each delElem In delList
            ActionList.Items.Remove(delElem)
            mLogger.Information($"Deleted item '{delElem.Name}'.")

            Dim asFile = TryCast(delElem, PlayerActionFile)
            If asFile IsNot Nothing Then
                FileCache.Instance.EraseEntry(asFile.AbsFileToPlay)
            End If
        Next

        If delIdx >= Playlist.Items.Count Then delIdx = Playlist.Items.Count - 1
        Playlist.SelectedIndex = delIdx
    End Sub


    Private Sub ListItemUpCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim curIdx = Playlist.SelectedIndex
        If curIdx > 0 Then
            Dim elem = ActionList.Items(curIdx)
            ActionList.Items.RemoveAt(curIdx)
            ActionList.Items.Insert(curIdx - 1, elem)
            Playlist.SelectedIndex = curIdx - 1
            mLogger.Information($"Moved item '{elem.Name}' from index {curIdx} to {curIdx - 1}.")
        End If
    End Sub


    Private Sub ListItemDownCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim curIdx = Playlist.SelectedIndex
        If curIdx < ActionList.Items.Count - 1 Then
            Dim elem = ActionList.Items(curIdx)
            ActionList.Items.RemoveAt(curIdx)
            ActionList.Items.Insert(curIdx + 1, elem)
            Playlist.SelectedIndex = curIdx + 1
            mLogger.Information($"Moved item '{elem.Name}' from index {curIdx} to {curIdx + 1}.")
        End If
    End Sub


    Private Sub ReplaceFileCommandCanExecute(sender As Object, args As CanExecuteRoutedEventArgs)
        args.CanExecute = True
    End Sub


    ''' <summary>
    ''' Open a replacement dialog, replace file upon success.
    ''' </summary>
    Private Sub ReplaceFileCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim item = GetSelectedItem(Of PlayerActionFile)(args.Parameter)
        If item Is Nothing Then Return

        Dim dlg As New Microsoft.Win32.OpenFileDialog() With {
            .DefaultExt = AudiofileDefaultExtension,
            .Filter = AudiofileExtensionFilter
        }

        If Not String.IsNullOrEmpty(item.AbsFileToPlay) Then
            dlg.InitialDirectory = Path.GetDirectoryName(item.AbsFileToPlay)
            dlg.FileName = item.ShortFileName
        End If

        ' Show open file dialog box 
        Dim result? As Boolean = dlg.ShowDialog()

        ' Process open file dialog box results 
        If result <> True Then Return

        FileCache.Instance.EraseEntry(dlg.FileName)
        item.FileToPlay = dlg.FileName
        item.FileTimestamp = Date.MinValue
        item.IsLoadingFailed = True ' To re-calculate duration
        mLogger.Information($"Replaced file in item '{item.Name}' to '{dlg.FileName}'.")
        item.AfterLoad(String.Empty)

        WaveformStorage.ForceUpdate(dlg.FileName, Dispatcher)
    End Sub

#End Region


#Region " Playlist modification commands: add elements "

    Private Sub AddFilesCommandCanExecute(sender As Object, args As CanExecuteRoutedEventArgs)
        args.CanExecute = True
    End Sub


    Private Sub AddFilesCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        If AddFilesToPlaylist() AndAlso Not My.Settings.SavePlaylistOnChange Then
            IsListModified = True
        End If
    End Sub


    Private Sub AddAutoVolumeCommandCanExecute(sender As Object, args As CanExecuteRoutedEventArgs)
        args.CanExecute = True
    End Sub


    Private Sub AddAutoVolumeCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        AddActionToPlaylist(New PlayerActionEffect(GeneratedVolumeAutomationTypes.FadeOut))

        If Not My.Settings.SavePlaylistOnChange Then
            IsListModified = True
        End If
    End Sub


    Private Sub AddPowerPointCommandCanExecute(sender As Object, args As CanExecuteRoutedEventArgs)
        args.CanExecute = True
    End Sub


    Private Sub AddPowerPointCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        AddActionToPlaylist(New PlayerActionPowerPoint())

        If Not My.Settings.SavePlaylistOnChange Then
            IsListModified = True
        End If
    End Sub


    Private Sub AddTextCommandCanExecute(sender As Object, args As CanExecuteRoutedEventArgs)
        args.CanExecute = True
    End Sub


    Private Sub AddTextCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        AddActionToPlaylist(New PlayerActionText())

        If Not My.Settings.SavePlaylistOnChange Then
            IsListModified = True
        End If
    End Sub


    Private Sub AddCommentCommandCanExecute(sender As Object, args As CanExecuteRoutedEventArgs)
        args.CanExecute = True
    End Sub


    Private Sub AddCommentCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        AddActionToPlaylist(New PlayerActionComment())

        If Not My.Settings.SavePlaylistOnChange Then
            IsListModified = True
        End If
    End Sub

#End Region


#Region " Playlist modification commands: volume and location "

    ''' <summary>
    ''' Whether a volume command may be executed.
    ''' </summary>
    Private Sub VolumeCommandCanExecute(sender As Object, args As CanExecuteRoutedEventArgs)
        HasSelectionCommandCanExecute(args)
    End Sub


    ''' <summary>
    ''' Whether a panning command may be executed.
    ''' </summary>
    Private Sub PanningCommandCanExecute(sender As Object, args As CanExecuteRoutedEventArgs)
        HasSelectionCommandCanExecute(args)
    End Sub


    ''' <summary>
    ''' Whether a 3D positioning command may be executed.
    ''' </summary>
    Private Sub CoordinateCommandCanExecute(sender As Object, args As CanExecuteRoutedEventArgs)
        HasSelectionCommandCanExecute(args)
    End Sub


    ''' <summary>
    ''' Sink all volumes but selected, effectively making the current action relatively louder.
    ''' </summary>
    Private Sub RelativeVolumeUpCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim curAct = GetSelectedItem(Of IPlayerAction)(Playlist.SelectedItem)
        If curAct Is Nothing Then Return

        For Each act In Playlist.Items.OfType(Of IPlayerAction)().Where(Function(a) a IsNot curAct)
            act.ModifyVolume(-AppConfiguration.Instance.VolumeStep)
        Next
    End Sub


    ''' <summary>
    ''' Raise all volumes but selected, effectively making the current action relatively softer.
    ''' </summary>
    Private Sub RelativeVolumeDownCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim curAct = GetSelectedItem(Of IPlayerAction)(Playlist.SelectedItem)
        If curAct Is Nothing Then Return

        For Each act In Playlist.Items.OfType(Of IPlayerAction)().Where(Function(a) a IsNot curAct)
            act.ModifyVolume(AppConfiguration.Instance.VolumeStep)
        Next
    End Sub


    ''' <summary>
    ''' Test a logical channel (command parameter).
    ''' </summary>
    Private Sub PlaySampleCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim logCh = CType(args.Parameter, Integer)
        If logCh = 0 Then Return

        Dim ch = InterfaceMapper.GetImplementation(Of IAudioChannelStorage)().Logical.Channel(logCh)
        If ch Is Nothing Then Return

        If ch.IsActive Then
            ch.StopTestSound()
        Else
            ch.PlayTestSound()
        End If
    End Sub


    ''' <summary>
    ''' Set the volume to the maximum before clipping.
    ''' </summary>
    Private Sub MaxVolumeCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim item = GetSelectedItem(Of PlayerActionFile)(Playlist.SelectedItem)
        If item Is Nothing Then Return

        Dim maxSample = WaveformStorage.GetMaxSample(item.AbsFileToPlay)
        item.Volume = If(maxSample = 0, 1, 1 / maxSample)
    End Sub


    ''' <summary>
    ''' Raise the volume.
    ''' </summary>
    Private Sub VolumeUpCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim item = GetSelectedItem(Of IVolumeController)(Playlist.SelectedItem)
        If item Is Nothing Then Return

        item.Volume += AppConfiguration.Instance.VolumeStep
        MessageLog.LogCommandExecuted(CommandMessages.VolumeSet, item.Volume)
    End Sub


    ''' <summary>
    ''' Lower the volume.
    ''' </summary>
    Private Sub VolumeDownCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim item = GetSelectedItem(Of IVolumeController)(Playlist.SelectedItem)
        If item Is Nothing Then Return

        item.Volume -= AppConfiguration.Instance.VolumeStep
        MessageLog.LogCommandExecuted(CommandMessages.VolumeSet, item.Volume)
    End Sub


    ''' <summary>
    ''' Move panning to the left.
    ''' </summary>
    Private Sub PanningLeftCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim item = GetSelectedItem(Of PlayerActionFile)(Playlist.SelectedItem)
        If item Is Nothing Then Return

        item.SoundPositionMode = SoundPositionModes.Panning
        item.Balance -= AppConfiguration.Instance.PanningStep
        MessageLog.LogCommandExecuted(CommandMessages.PanningSet, item.Balance)
    End Sub


    ''' <summary>
    ''' Move panning to the right.
    ''' </summary>
    Private Sub PanningRightCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim item = GetSelectedItem(Of PlayerActionFile)(Playlist.SelectedItem)
        If item Is Nothing Then Return

        item.SoundPositionMode = SoundPositionModes.Panning
        item.Balance += AppConfiguration.Instance.PanningStep
        MessageLog.LogCommandExecuted(CommandMessages.PanningSet, item.Balance)
    End Sub


    ''' <summary>
    ''' Move position to the left.
    ''' </summary>
    Private Sub CoordinateXDownCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim item = GetSelectedItem(Of PlayerActionFile)(Playlist.SelectedItem)
        If item Is Nothing Then Return

        item.SoundPositionMode = SoundPositionModes.Coordinates
        item.X -= AppConfiguration.Instance.CoordinateStep
        MessageLog.LogCommandExecuted(CommandMessages.CoordinateXSet, item.X)
    End Sub


    ''' <summary>
    ''' Move position to the right.
    ''' </summary>
    Private Sub CoordinateXUpCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim item = GetSelectedItem(Of PlayerActionFile)(Playlist.SelectedItem)
        If item Is Nothing Then Return

        item.SoundPositionMode = SoundPositionModes.Coordinates
        item.X += AppConfiguration.Instance.CoordinateStep
        MessageLog.LogCommandExecuted(CommandMessages.CoordinateXSet, item.X)
    End Sub


    ''' <summary>
    ''' Move position forward.
    ''' </summary>
    Private Sub CoordinateYDownCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim item = GetSelectedItem(Of PlayerActionFile)(Playlist.SelectedItem)
        If item Is Nothing Then Return

        item.SoundPositionMode = SoundPositionModes.Coordinates
        item.Y -= AppConfiguration.Instance.CoordinateStep
        MessageLog.LogCommandExecuted(CommandMessages.CoordinateYSet, item.Y)
    End Sub


    ''' <summary>
    ''' Move position backwards.
    ''' </summary>
    Private Sub CoordinateYUpCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim item = GetSelectedItem(Of PlayerActionFile)(Playlist.SelectedItem)
        If item Is Nothing Then Return

        item.SoundPositionMode = SoundPositionModes.Coordinates
        item.Y += AppConfiguration.Instance.CoordinateStep
        MessageLog.LogCommandExecuted(CommandMessages.CoordinateYSet, item.Y)
    End Sub


    ''' <summary>
    ''' Move position up.
    ''' </summary>
    Private Sub CoordinateZDownCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim item = GetSelectedItem(Of PlayerActionFile)(Playlist.SelectedItem)
        If item Is Nothing Then Return

        item.SoundPositionMode = SoundPositionModes.Coordinates
        item.Z -= AppConfiguration.Instance.CoordinateStep
        MessageLog.LogCommandExecuted(CommandMessages.CoordinateZSet, item.Z)
    End Sub


    ''' <summary>
    ''' Move position down.
    ''' </summary>
    Private Sub CoordinateZUpCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim item = GetSelectedItem(Of PlayerActionFile)(Playlist.SelectedItem)
        If item Is Nothing Then Return

        item.SoundPositionMode = SoundPositionModes.Coordinates
        item.Z += AppConfiguration.Instance.CoordinateStep
        MessageLog.LogCommandExecuted(CommandMessages.CoordinateZSet, item.Z)
    End Sub

#End Region


#Region " Playlist modification commands "

    Private Sub HasSelectionCommandCanExecute(args As CanExecuteRoutedEventArgs)
        args.CanExecute = (Playlist IsNot Nothing AndAlso Playlist.SelectedItems.Count > 0)
    End Sub


    Private Sub IsSelectionPlayableCommandCanExecute(sender As Object, args As CanExecuteRoutedEventArgs)
        HasSelectionCommandCanExecute(args)

        If args.CanExecute Then
            Dim act = TryCast(Playlist.SelectedItem, PlayerAction)
            args.CanExecute = act IsNot Nothing AndAlso act.CanExecute
        End If
    End Sub


    Private Sub IsSelectionPlayableMainCommandCanExecute(sender As Object, args As CanExecuteRoutedEventArgs)
        IsSelectionPlayableCommandCanExecute(sender, args)

        If args.CanExecute Then
            args.CanExecute = CType(Playlist.SelectedItem, PlayerAction).ExecutionType <> ExecutionTypes.Parallel
        End If
    End Sub


    Private Sub HasActiveActionCommandCanExecute(sender As Object, args As CanExecuteRoutedEventArgs)
        args.CanExecute = ReplayAction IsNot Nothing
    End Sub


    Private Sub HasNextActionCommandCanExecute(sender As Object, args As CanExecuteRoutedEventArgs)
        args.CanExecute = NextAction IsNot Nothing OrElse ActionList.GlobalParallelList.Any()
    End Sub


    Private Sub IsPlayingCommandCanExecute(sender As Object, args As CanExecuteRoutedEventArgs)
        args.CanExecute = mAudioMgr.IsPlaying
    End Sub


    Private Sub IsResumeCommandCanExecute(sender As Object, args As CanExecuteRoutedEventArgs)
        args.CanExecute = mAudioMgr.IsActive AndAlso mAudioMgr.IsManuallyPaused
    End Sub


    Private Sub NewPlaylistCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        SaveSilentlyOrAsk(True)
        ActionList = New PlayerActionCollection()
        AppConfiguration.Instance.LastPlaylistFile = String.Empty
        FileCache.Instance.Clear()
        ResetPlayer()
    End Sub


    Private Sub SetActiveCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim item = CType(Playlist.SelectedItem, PlayerAction)
        SetItemActiveAndPlay(item, ExecutionTypes.MainStopPrev)
        MessageLog.LogCommandExecuted(CommandMessages.Started, item.Name)
    End Sub


    Private Sub StopActionCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim item = CType(Playlist.SelectedItem, PlayerAction)
        mAudioMgr.StopSingle(item)
        MessageLog.LogCommandExecuted(CommandMessages.Stopped, item.Name)
    End Sub


    Private Sub SetNextCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim item = CType(Playlist.SelectedItem, PlayerAction)
        NextAction = item
    End Sub


    ''' <summary>
    ''' Select action by index.
    ''' </summary>
    Private Sub SelectActionCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim selIdx = Integer.Parse(CStr(args.Parameter)) - 1
        If selIdx < 0 OrElse selIdx >= Playlist.Items.Count() Then Return

        Playlist.SelectedIndex = selIdx
        Dim act As PlayerAction = CType(Playlist.SelectedItem, PlayerAction)
        Playlist.BringItemToView(act)
        MessageLog.LogCommandExecuted(CommandMessages.Selected, selIdx + 1, act.Name)
    End Sub


    ''' <summary>
    ''' Execute action by index.
    ''' </summary>
    Private Sub ExecuteActionCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        SelectActionCommandExecuted(sender, args)
        SetActiveCommandExecuted(sender, args)
    End Sub


    ''' <summary>
    ''' Handle inserts from drag-and-drop.
    ''' </summary>
    Private Sub InsertItemHandler(sender As Object, args As InsertItemsEventArgs) Handles Playlist.InsertItems
        Dim idx = args.InsertIndex

        For Each item In args.ItemList
            If ActionList.Items.Contains(item) Then
                ' Move - delete it first
                Dim fromIdx = ActionList.Items.IndexOf(item)
                ActionList.Items.RemoveAt(fromIdx)

                If fromIdx < idx Then
                    idx -= 1
                End If
            End If

            AddItemToList(idx, item)
            ' Index -1 shall stay -1
            If idx >= 0 Then idx += 1
        Next
    End Sub


    ''' <summary>
    ''' Handle selection change - show the selected action in details.
    ''' </summary>
    Private Sub SelectionChangedHandler(sender As Object, args As SelectionChangedEventArgs) Handles Playlist.SelectionChanged
        FollowActionType = FollowActionTypes.None

        If args.AddedItems.IsEmpty() Then
            ActionControl.Action = PlayerAction.PlaceHolder
        Else
            ActionControl.Action = args.AddedItems.OfType(Of PlayerAction)().LastOrDefault()
        End If
    End Sub


    Private Sub WhatCanISayCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim commands = mVoiceControl.VoiceOperationList.
            Where(Function(v) v.Setting.IsEnabled AndAlso Not v.Definition.Flags.HasFlag(CommandFlags.HideFromList)).
            Select(Function(v)
                       If (v.Definition.ParameterType = CommandParameterTypes.ItemIndex) Then
                           Return IIf(ActionList.Items.Count > 0, $"{v.Setting.RecognitionText} 1 to {ActionList.Items.Count}", "")
                       ElseIf (v.Definition.ParameterType = CommandParameterTypes.ParallelIndex) Then
                           Return IIf(ActionList.MaxParallels > 0, $"{v.Setting.RecognitionText} 1 to {ActionList.MaxParallels}", "")
                       Else
                           Return v.Setting.RecognitionText
                       End If
                   End Function)

        Dim str = String.Join(", ", commands)
        MessageLog.LogVoiceInfo(VoiceMessages.YieldCommandList, str)
    End Sub


    Private Sub ListTriggersCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        MessageLog.LogVoiceInfo(VoiceMessages.YieldTriggerList)
    End Sub

#End Region


#Region " Focus "

    ''' <summary>
    ''' Make sure Playlist is in focus to accept key commands.
    ''' </summary>
    Private Sub SetPlaylistInFocus()
        Playlist.SetFocusToMe()
    End Sub

#End Region


#Region " Speech recognition events "

    ''' <summary>
    ''' Change IsSoundComing.
    ''' </summary>
    Private Sub VoiceControlPropertyChangedHandler(sender As Object, args As PropertyChangedEventArgs) Handles mVoiceControl.PropertyChanged
        If args.PropertyName <> PropertyChangedHelper.GetPropertyName(Function() mVoiceControl.IsSoundComing) Then Return

        Dispatcher.BeginInvoke(Sub() IsSoundComing = mVoiceControl.IsSoundComing)
    End Sub


    Private Sub RestartVoiceControl()
        mVoiceControl.Restart(ActionList.MaxParallels, ActionList.Items.Count)
    End Sub

#End Region


#Region " Mouse event handlers "

    Private Shared Function GetActionFromEventSource(sender As Object) As PlayerAction
        Dim dc As Object = Nothing

        If TypeOf sender Is FrameworkElement Then
            dc = TryCast(sender, FrameworkElement).DataContext
        ElseIf TypeOf sender Is FrameworkContentElement Then
            dc = TryCast(sender, FrameworkContentElement).DataContext
        End If

        Return TryCast(dc, PlayerAction)
    End Function


    ''' <summary>
    ''' Handle mouse click - show the selected action in details
    ''' and turn off following.
    ''' </summary>
    Private Sub MouseDownHandler(sender As Object, args As MouseButtonEventArgs) Handles Playlist.PreviewMouseDown
        Dim item = GetActionFromEventSource(args.OriginalSource)

        ' Clicked in another area - ignore
        If item Is Nothing Then Return

        Playlist.SelectedItem = item

        If ActionControl.Action Is item Then Return

        FollowActionType = FollowActionTypes.None
        ActionControl.Action = item
    End Sub


    ''' <summary>
    ''' Set the selected action to play.
    ''' </summary>
    Private Sub ActionList_MouseDoubleClick(sender As Object, args As MouseButtonEventArgs)
        Dim item = GetActionFromEventSource(args.OriginalSource)

        SetItemActiveAndPlay(item, ExecutionTypes.MainStopPrev, item.Equals(ReplayAction))
    End Sub

#End Region


#Region " Playlist operation commands "

    ''' <summary>
    ''' Reset the player's actions.
    ''' </summary>
    Private Sub ResetPlayer()
        mAudioMgr.Reset()

        ActionList.ArrangeStructure()
        FollowActionType = AppConfiguration.Instance.DefaultFollowAction
        PlayerActionText.Reset()
    End Sub


    ''' <summary>
    ''' If the item is on the main line, set it to Active and start the playback.
    ''' If parallel, just start it.
    ''' </summary>
    ''' <param name="interrupt">
    ''' Whether the action is coming from UI and shall interrupt previous actions.
    ''' It is not, when it comes from Next button or a trigger.
    ''' </param>
    Private Sub SetItemActiveAndPlay(
        item As IPlayerAction,
        interrupt As ExecutionTypes,
        Optional restart As Boolean = False
        )
        If item Is Nothing Then Return

        If restart Then
            mAudioMgr.Restart(item, ExecutionTypes.MainStopPrev)
        Else
            mAudioMgr.Play(item, interrupt)
        End If
    End Sub


    ''' <summary>
    ''' Play next main action.
    ''' Parallel actions are left untouched.
    ''' </summary>
    Private Sub PlayNextCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        If NextAction IsNot Nothing Then
            SetItemActiveAndPlay(NextAction, ExecutionTypes.MainContinuePrev)
            MessageLog.LogCommandExecuted(CommandMessages.Started, NextAction.Name)
            SetPlaylistInFocus()
        ElseIf ActionList.GlobalParallelList.Any() Then
            mAudioMgr.StartWaiting()
            MessageLog.LogCommandExecuted(CommandMessages.StartPassive)
        End If
    End Sub


    ''' <summary>
    ''' Play the current main action again (after pause).
    ''' Parallel actions are left untouched.
    ''' </summary>
    Private Sub PlayAgainCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        If ReplayAction Is Nothing Then Return

        SetItemActiveAndPlay(ReplayAction, ExecutionTypes.MainStopAll, True)
        MessageLog.LogCommandExecuted(CommandMessages.Started, ReplayAction.Name)
        SetPlaylistInFocus()
    End Sub


    ''' <summary>
    ''' Pause main and stop all parallel sounds.
    ''' </summary>
    Private Sub StopCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        mAudioMgr.PauseAll()
        MessageLog.LogCommandExecuted(CommandMessages.StoppedAll)
        SetPlaylistInFocus()
    End Sub


    ''' <summary>
    ''' Resume main sounds.
    ''' </summary>
    Private Sub ResumeCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        mAudioMgr.ResumeMain()
        SetPlaylistInFocus()
    End Sub


    ''' <summary>
    ''' Stop all sounds, reset the player state.
    ''' </summary>
    Private Sub ResetPlaylistCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        MessageLog.ClearLog("Reset playlist", "Playlist reset")
        ResetPlayer()
        SetPlaylistInFocus()
    End Sub


    ''' <summary>
    ''' Start the playlist in passive mode, without anything been played back
    ''' unless specified by a clock or manual starts.
    ''' </summary>
    Private Sub StartPlaylistCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        MessageLog.ClearLog("Start playlist in passive mode", "Passive mode, waiting")
        mAudioMgr.StartWaiting()
        SetPlaylistInFocus()
    End Sub


    ''' <summary>
    ''' Start or stop the given parallel action.
    ''' </summary>
    Private Sub PlayStopParallelCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim parIdx = Integer.Parse(CStr(args.Parameter))
        If parIdx < 0 OrElse parIdx > ManualParallels.Count() Then Return

        If parIdx = 0 Then
            PlayNextCommandExecuted(sender, args)
        Else
            Dim act = ManualParallels(parIdx - 1)

            If act.IsPlaying Then
                mAudioMgr.StopSingle(act)
                MessageLog.LogCommandExecuted(CommandMessages.Stopped, act.Name)
            Else
                mAudioMgr.Play(act, ExecutionTypes.Parallel)
                MessageLog.LogCommandExecuted(CommandMessages.Started, act.Name)
            End If
        End If
    End Sub

#End Region


#Region " Menu commands "

    ''' <summary>
    ''' Close the application.
    ''' </summary>
    ''' <remarks>
    ''' Reusing Application.Close command.
    ''' Registration is performed in XAML, otherwise the command is always disabled.
    ''' </remarks>
    Private Sub CloseCommandExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Close()
    End Sub


    ''' <summary>
    ''' Show Settings dialog
    ''' </summary>
    Private Sub ShowSettingsCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim wnd As New SettingsWindow() With {
            .VoiceCommands = mVoiceControl.VoiceOperationList,
            .OnAccept = AddressOf RestartVoiceControl
        }
        AddHandler wnd.Closed, AddressOf DialogClosedHandler

        mDialogWindows.Add(wnd)
        wnd.Show()
    End Sub


    ''' <summary>
    ''' Whether may show Print dialog
    ''' </summary>
    Private Sub PrintCommandCanExecute(sender As Object, args As CanExecuteRoutedEventArgs)
        args.CanExecute = True
    End Sub


    ''' <summary>
    ''' Show Print dialog
    ''' </summary>
    Private Sub PrintCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim wnd As New PrintListWindow() With {
            .Playlist = ActionList
        }
        AddHandler wnd.Closed, AddressOf DialogClosedHandler

        mDialogWindows.Add(wnd)
        wnd.ShowDialog()
    End Sub


    ''' <summary>
    ''' Show Export dialog
    ''' </summary>
    Private Sub ExportCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim wnd As New ExportListWindow() With {
            .Playlist = ActionList
        }
        AddHandler wnd.Closed, AddressOf DialogClosedHandler

        mDialogWindows.Add(wnd)
        wnd.ShowDialog()
    End Sub


    ''' <summary>
    ''' Show About dialog
    ''' </summary>
    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CC0004:Catch block cannot be empty", Justification:="<Pending>")>
    Private Sub ShowAboutCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim wnd As New AboutWindow()
        AddHandler wnd.Closed, AddressOf DialogClosedHandler

        mDialogWindows.Add(wnd)

        Try
            wnd.ShowDialog()
        Catch ex As Exception
            ' Ignore
        End Try
    End Sub


    ''' <summary>
    ''' Preview text window.
    ''' </summary>
    Private Sub ShowTextWindowCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        Dim act = CType(args.Parameter, PlayerActionText)
        Dim btn = CType(args.OriginalSource, ToggleButton)

        If btn.IsChecked Then
            InterfaceMapper.GetImplementation(Of ITextChannelStorage).Logical.Channel(act.Channel).ShowText(act.Text)
        Else
            InterfaceMapper.GetImplementation(Of ITextChannelStorage).Logical.Channel(act.Channel).HideText()
        End If
    End Sub


    ''' <summary>
    ''' Close an opened dialog.
    ''' </summary>
    Private Sub CloseWindowCommandExecuted(sender As Object, e As ExecutedRoutedEventArgs)
        If mDialogWindows.IsEmpty() Then Return

        Dim wnd = mDialogWindows.Last()
        CType(e.Command, RoutedCommand).Execute(e.Parameter, wnd)
    End Sub


    ''' <summary>
    ''' Remove reference, so the Close command does no harm.
    ''' </summary>
    Private Sub DialogClosedHandler(sender As Object, args As EventArgs)
        mDialogWindows.Remove(CType(sender, Window))
    End Sub


    ''' <summary>
    ''' Select active action in the playlist
    ''' </summary>
    Private Sub SelectActiveCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        If ReplayAction Is Nothing Then Return

        Playlist.SelectedItem = ReplayAction
        Playlist.BringItemToView(ReplayAction)
    End Sub


    ''' <summary>
    ''' Select next action in the playlist
    ''' </summary>
    Private Sub SelectNextCommandExecuted(sender As Object, args As ExecutedRoutedEventArgs)
        If NextAction Is Nothing Then Return

        Playlist.SelectedItem = NextAction
        Playlist.BringItemToView(NextAction)
    End Sub

#End Region


#Region " Playback handlers "

    ''' <summary>
    ''' The action has started.
    ''' </summary>
    Private Sub PlaybackStartedHandler(sender As IAudioManager, action As IPlayerAction) Handles mAudioMgr.PlaybackStarted
        action.IsActive = True
        AssignFollowedAction()
    End Sub


    ''' <summary>
    ''' The action has finished naturally.
    ''' </summary>
    Private Sub PlaybackEndedHandler(sender As IAudioManager, action As IPlayerAction) Handles mAudioMgr.PlaybackEnded
        action.IsActive = False
    End Sub


    ''' <summary>
    ''' The playback state is changed.
    ''' </summary>
    ''' <remarks>To be called on UI thread</remarks>
    Private Sub StateChangedHandler(sender As IAudioManager, state As PlaylistState) Handles mAudioMgr.StateChanged
        ManualParallels = state.ManualParallels
        ActiveMainAction = state.ActiveMainAction
        ActiveMainProducer = state.ActiveMainProducer
        ReplayAction = If(state.ActiveMainProducer, state.LastMainProducer)
        NextAction = state.NextAction

        AssignFollowedAction()
    End Sub


    ''' <summary>
    ''' The playback time is changed.
    ''' </summary>
    ''' <remarks>To be called on UI thread</remarks>
    Private Sub PlaybackTimeChangedHandler(sender As IAudioManager, time As TimeSpan) Handles mAudioMgr.PlaybackTimeChanged
        ActiveTime = time
    End Sub

#End Region

End Class
