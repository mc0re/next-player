Imports System.ComponentModel
Imports System.Globalization
Imports System.Speech.Recognition
Imports Common


''' <summary>
''' Start voice recognition engine, listen to commands.
''' </summary>
Public Class SpeechRecognitionControl
    Implements IDisposable, INotifyPropertyChanged

#Region " INotifyPropertyChanged implementation "

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged


    ''' <summary>
    ''' Raise PropertyChanged event.
    ''' </summary>
    Private Sub RaisePropertyChanged(propName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propName))
    End Sub


    ''' <summary>
    ''' Helper function to raise the event.
    ''' NB. BindingList does not propagate properties of derived objects.
    ''' </summary>
    Protected Sub RaisePropertyChanged(Of T)(prop As Expressions.Expression(Of Func(Of T)))
        RaisePropertyChanged(PropertyChangedHelper.GetPropertyName(prop))
    End Sub

#End Region


#Region " Fields "

    ''' <summary>
    ''' Instance of speech recognizer.
    ''' </summary>
    Private WithEvents mSpeechRecognizer As New SpeechRecognitionEngine()


    ''' <summary>
    ''' A list of commands the player can understand.
    ''' The "numeric" commands are "unrolled".
    ''' Key is recognition text.
    ''' </summary>
    Private ReadOnly mVoiceControls As New Dictionary(Of String, VoiceOperation)()


    ''' <summary>
    ''' Common target control for all commands.
    ''' </summary>
    Private ReadOnly mCommandTarget As Control

#End Region


#Region " IsSoundComing read-only notifying property "

    Private mIsSoundComing As Boolean


    ''' <summary>
    ''' Whether sound is coming to speech recognizer.
    ''' </summary>
    Public Property IsSoundComing As Boolean
        Get
            Return mIsSoundComing
        End Get
        Set(value As Boolean)
            mIsSoundComing = value
            RaisePropertyChanged(Function() IsSoundComing)
        End Set
    End Property

#End Region


#Region " VoiceOperationList property "

    ''' <summary>
    ''' A list of command settings.
    ''' </summary>
    Private ReadOnly mVoiceSettingList As New VoiceCommandSettingItemCollection()


    Public ReadOnly Property VoiceOperationList As VoiceCommandSettingItemCollection
        Get
            Return mVoiceSettingList
        End Get
    End Property

#End Region


#Region " MessageLog property "

    Private mMessageLog As IMessageLog


    Private Property MessageLog As IMessageLog
        Get
            If mMessageLog Is Nothing Then
                mMessageLog = InterfaceMapper.GetImplementation(Of IMessageLog)()
            End If

            Return mMessageLog
        End Get
        Set
            mMessageLog = Value
        End Set
    End Property

#End Region


#Region " Init and clean-up "

    Public Sub New(cmdTarget As Control)
        mCommandTarget = cmdTarget
        LoadSettings()

        Try
            mSpeechRecognizer.SetInputToDefaultAudioDevice()
            AddHandler mSpeechRecognizer.AudioStateChanged, AddressOf AudioStateChangedHanlder
            AddHandler mSpeechRecognizer.SpeechRecognized, AddressOf OnSpeechRecognized
            AddHandler mSpeechRecognizer.SpeechRecognitionRejected, AddressOf OnSpeechRejected

        Catch ex As Exception
            ' Probably no audio found, ignore
            InterfaceMapper.GetImplementation(Of IMessageLog)().LogFileError("Recognizer error: {0}", ex.Message)
        End Try
    End Sub

#End Region


#Region " Settings "

    ''' <summary>
    ''' Copy settings from Configuration.VoiceCommands to mVoiceSettingList.
    ''' </summary>
    ''' <remarks>
    ''' To allow smooth editing, Setting points directly to the actual setting, saved in app.config.
    ''' </remarks>
    Public Sub LoadSettings()
        mVoiceControls.Clear()
        Dim voice = InterfaceMapper.GetImplementation(Of IVoiceConfiguration)()

        For Each cmdDef In voice.VoiceCommands
            Dim rcmd = TryCast(Application.Current.FindResource(cmdDef.CommandName), RoutedCommand)
            If rcmd Is Nothing Then
                MessageLog.LogVoiceInfo(VoiceMessages.CommandNotFound, cmdDef.CommandName)
                Continue For
            End If

            Dim cmdDesc = GetCommandDescription(cmdDef.CommandName)
            Dim cmd As New CommandSettingItem() With {
                    .Setting = cmdDef,
                    .Command = rcmd,
                    .Definition = cmdDesc
                }
            VoiceOperationList.Add(cmd)
        Next
    End Sub


    ''' <summary>
    ''' Re-populate the grammar.
    ''' </summary>
    Public Sub Restart(maxPars As Integer, maxItems As Integer)
        StartListening(maxPars, maxItems)
    End Sub

#End Region


#Region " Recognition "

    ''' <summary>
    ''' Create grammar for recognition.
    ''' </summary>
    Private Function PrepareGrammar(maxPars As Integer, maxItems As Integer) As Grammar
        ' Voice control disabled
        Dim voice = InterfaceMapper.GetImplementation(Of IVoiceConfiguration)()
        If Not voice.IsVoiceControlEnabled Then Return Nothing

        ' No commands
        If Not VoiceOperationList.Any() Then Return Nothing

        Dim commandFound = False

        ' Fill mVoiceControls
        mVoiceControls.Clear()

        For Each cmdDef In VoiceOperationList
            If Not cmdDef.Setting.IsEnabled Then Continue For
            commandFound = True

            If cmdDef.Definition.ParameterType = CommandParameterTypes.None Then
                Dim cmd As New VoiceOperation() With {
                    .Setting = cmdDef.Setting,
                    .Command = cmdDef.Command,
                    .RecognizedText = cmdDef.Setting.RecognitionText
                }

                mVoiceControls.Add(cmd.RecognizedText, cmd)
            Else
                Dim maxIndex As Integer
                If cmdDef.Definition.ParameterType = CommandParameterTypes.ParallelIndex Then
                    maxIndex = maxPars
                Else
                    maxIndex = maxItems
                End If

                For Each idx In Enumerable.Range(1, maxIndex)
                    Dim cmd As New VoiceOperation() With {
                        .Setting = cmdDef.Setting,
                        .Command = cmdDef.Command,
                        .Parameter = idx,
                        .RecognizedText = String.Format("{0} {1}", cmdDef.Setting.RecognitionText, idx).Trim()
                    }

                    mVoiceControls.Add(cmd.RecognizedText, cmd)
                Next
            End If
        Next

        ' All commands are disabled
        If Not commandFound Then Return Nothing

        ' Create grammar
        Try
            Dim gb As New GrammarBuilder(New Choices(mVoiceControls.Keys.ToArray())) With {
                .Culture = New CultureInfo("en-GB")
            }
            Return New Grammar(gb)

        Catch ex As Exception
            MessageLog.LogVoiceInfo(VoiceMessages.ErrorInGrammar, ex.Message)
            Return Nothing
        End Try
    End Function


    ''' <summary>
    ''' Collect command list for voice recognition from mVoiceSettingList to mVoiceControls.
    ''' </summary>
    ''' <returns>False if no commands, True if commands are set up, and the engine can be started</returns>
    Public Function StartListening(maxPars As Integer, maxItems As Integer) As Boolean
        Try
            If mSpeechRecognizer Is Nothing Then Return False

            Dim gr = PrepareGrammar(maxPars, maxItems)
            RemoveHandler mSpeechRecognizer.RecognizerUpdateReached, AddressOf SpeechRecognitionOffHandler
            RemoveHandler mSpeechRecognizer.RecognizerUpdateReached, AddressOf SpeechRecognitionOnHandler

            If gr Is Nothing Then
                AddHandler mSpeechRecognizer.RecognizerUpdateReached, AddressOf SpeechRecognitionOffHandler
            Else
                AddHandler mSpeechRecognizer.RecognizerUpdateReached, AddressOf SpeechRecognitionOnHandler
            End If

            mSpeechRecognizer.RequestRecognizerUpdate(gr)
            Return gr IsNot Nothing

        Catch ex As Exception
            MessageLog.LogVoiceInfo(VoiceMessages.ErrorInStartListening, ex.Message)
            Return False
        End Try
    End Function


    Private Sub SpeechRecognitionOffHandler(sender As Object, e As RecognizerUpdateReachedEventArgs)
        mSpeechRecognizer.UnloadAllGrammars()
        StopListening()
        MessageLog.LogVoiceInfo(VoiceMessages.RecognitionStopped)
    End Sub


    Private Sub SpeechRecognitionOnHandler(sender As Object, e As RecognizerUpdateReachedEventArgs)
        Dim gr = CType(e.UserToken, Grammar)
        Dim hasGr = mSpeechRecognizer.Grammars.Any()

        mSpeechRecognizer.UnloadAllGrammars()
        mSpeechRecognizer.UpdateRecognizerSetting("CFGConfidenceRejectionThreshold", 20)
        mSpeechRecognizer.UpdateRecognizerSetting("HighConfidenceThreshold", 80)
        mSpeechRecognizer.UpdateRecognizerSetting("NormalConfidenceThreshold", 50)
        mSpeechRecognizer.UpdateRecognizerSetting("LowConfidenceThreshold", 20)
        mSpeechRecognizer.LoadGrammarAsync(gr)

        If Not hasGr Then
            mSpeechRecognizer.SetInputToDefaultAudioDevice()
            mSpeechRecognizer.RecognizeAsync(RecognizeMode.Multiple)

            MessageLog.LogVoiceInfo(VoiceMessages.RecognitionStarted)
        Else
            MessageLog.LogVoiceInfo(VoiceMessages.RecognitionUpdated)
        End If
    End Sub


    ''' <summary>
    ''' Stop listening and recognition.
    ''' </summary>
    Public Sub StopListening()
        mSpeechRecognizer?.RecognizeAsyncStop()
    End Sub


    ''' <summary>
    ''' Triggered when engine is stopped.
    ''' </summary>
    Private Sub AudioStateChangedHanlder(sender As Object, args As AudioStateChangedEventArgs)
        IsSoundComing = (args.AudioState = AudioState.Speech)
    End Sub


    ''' <summary>
    ''' Command is recognized, execute.
    ''' </summary>
    Private Sub OnSpeechRecognized(sender As Object, args As SpeechRecognizedEventArgs)
        Dim cmd As VoiceOperation = Nothing
        If Not mVoiceControls.TryGetValue(args.Result.Text, cmd) Then
            MessageLog.LogVoiceInfo(VoiceMessages.CommandNotInList, args.Result.Text)
            Return
        End If

        Dim response = IIf(cmd.Setting.Definition.Flags.HasFlag(CommandFlags.Confirm), VoiceMessages.CommandRecognized, VoiceMessages.CommandRecognizedNoConfirmation)
        MessageLog.LogVoiceInfo(response, args.Result.Text, args.Result.Confidence)

        If cmd.Command.CanExecute(cmd.Parameter, mCommandTarget) Then
            cmd.Command.Execute(cmd.Parameter, mCommandTarget)
        Else
            MessageLog.LogVoiceInfo(VoiceMessages.NoItemSelected)
        End If
    End Sub


    ''' <summary>
    ''' Unrecognized command.
    ''' </summary>
    Private Sub OnSpeechRejected(sender As Object, args As SpeechRecognitionRejectedEventArgs)
        MessageLog.LogVoiceInfo(VoiceMessages.CommandRejected, args.Result.Text, args.Result.Confidence)
    End Sub

#End Region


#Region " IDisposable implementation "

    Private mIsDisposed As Boolean


    ''' <inheritdoc/>
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not mIsDisposed Then
            If disposing Then
                RemoveHandler mSpeechRecognizer.AudioStateChanged, AddressOf AudioStateChangedHanlder
                RemoveHandler mSpeechRecognizer.SpeechRecognized, AddressOf OnSpeechRecognized
                RemoveHandler mSpeechRecognizer.SpeechRecognitionRejected, AddressOf OnSpeechRejected
                mSpeechRecognizer.Dispose()
                mSpeechRecognizer = Nothing
            End If
        End If

        mIsDisposed = True
    End Sub


    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

#End Region

End Class
