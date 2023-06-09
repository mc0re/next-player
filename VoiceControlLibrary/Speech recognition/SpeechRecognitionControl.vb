﻿Imports System.ComponentModel
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
	''' A list of commands.
	''' Key is recognition text.
	''' </summary>
	Private ReadOnly mVoiceControls As New Dictionary(Of String, VoiceOperation)()


	''' <summary>
	''' Common target control for all commands.
	''' </summary>
	Private mCommandTarget As Control

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


#Region " Init and clean-up "

	Public Sub New(cmdTarget As Control)
		mCommandTarget = cmdTarget
		LoadSettings()

		Try
			mSpeechRecognizer.SetInputToDefaultAudioDevice()

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
				InterfaceMapper.GetImplementation(Of IMessageLog)().
					LogVoiceInfo("Command '{0}' not found", cmdDef.CommandName)
				Continue For
			End If

			Dim cmdDesc = CommandList.GetCommandDescription(cmdDef.CommandName)
			Dim cmd As New CommandSettingItem() With {
					.Setting = cmdDef,
					.Command = rcmd,
					.ParameterType = cmdDesc.ParameterType,
					.Description = cmdDesc.Description
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
		Dim commandChoices As New Choices()

		' Fill mVoiceControls
		mVoiceControls.Clear()

		For Each cmdDef In VoiceOperationList
			If Not cmdDef.Setting.IsEnabled Then Continue For
			commandFound = True

			If cmdDef.ParameterType = CommandParameterTypes.None Then
				Dim cmd As New VoiceOperation() With {
					.Setting = cmdDef.Setting,
					.Command = cmdDef.Command,
					.RecognizedText = cmdDef.Setting.RecognitionText
				}

				mVoiceControls.Add(cmd.RecognizedText, cmd)
			Else
				Dim maxIndex As Integer
				If cmdDef.ParameterType = CommandParameterTypes.ParallelIndex Then
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
		For Each cmdText In mVoiceControls.Keys
			commandChoices.Add(cmdText)
		Next

		Try
			Dim gb As New GrammarBuilder(commandChoices)
			Return New Grammar(gb)

		Catch ex As Exception
			InterfaceMapper.GetImplementation(Of IMessageLog)().LogVoiceInfo(
				"Error when building grammar: " & ex.Message)
			Return Nothing
		End Try
	End Function


	''' <summary>
	''' Collect command list for voice recognition from mVoiceSettingList to mVoiceControls.
	''' </summary>
	''' <returns>False if no commands, True if commands are set up, and the engine can be started</returns>
	Public Function StartListening(maxPars As Integer, maxItems As Integer) As Boolean
		Try
			Dim hasGr = mSpeechRecognizer.Grammars.Any()
			Dim gr = PrepareGrammar(maxPars, maxItems)
			If gr Is Nothing Then Return False

			mSpeechRecognizer.RequestRecognizerUpdate()
			mSpeechRecognizer.UnloadAllGrammars()
			mSpeechRecognizer.LoadGrammarAsync(gr)

			If Not hasGr Then
				mSpeechRecognizer.RecognizeAsync(RecognizeMode.Multiple)

				InterfaceMapper.GetImplementation(Of IMessageLog)().LogVoiceInfo(
					"Voice recognition started")
			Else
				InterfaceMapper.GetImplementation(Of IMessageLog)().LogVoiceInfo(
					"Voice commands updated")
			End If

			Return True

		Catch ex As Exception
			InterfaceMapper.GetImplementation(Of IMessageLog)().LogVoiceInfo(
				"Error when initializing voice recognition: " & ex.Message)
			Return False
		End Try
	End Function


	''' <summary>
	''' Stop listening and recognition.
	''' </summary>
	Public Sub StopListening()
		mSpeechRecognizer.RecognizeAsyncStop()
	End Sub


	''' <summary>
	''' Triggered when engine is stopped.
	''' </summary>
	Private Sub AudioStateChangedHanlder(sender As Object, args As AudioStateChangedEventArgs) Handles mSpeechRecognizer.AudioStateChanged
		IsSoundComing = (args.AudioState = AudioState.Speech)
	End Sub


	''' <summary>
	''' Command is recognized, execute.
	''' </summary>
	Private Sub OnSpeechRecognized(sender As Object, args As SpeechRecognizedEventArgs) Handles mSpeechRecognizer.SpeechRecognized
		Dim cmdText = String.Join(" ", From w In args.Result.Words Select w.Text)
		InterfaceMapper.GetImplementation(Of IMessageLog)().
			LogVoiceInfo("Voice command recognized '{0}'", cmdText)

		Dim cmd As VoiceOperation = Nothing
		If Not mVoiceControls.TryGetValue(cmdText, cmd) Then
			InterfaceMapper.GetImplementation(Of IMessageLog)().
				LogVoiceInfo("Voice command '{0}' not found", cmdText)
			Return
		End If

		cmd.Command.Execute(cmd.Parameter, mCommandTarget)
	End Sub


	''' <summary>
	''' Unrecognized command.
	''' </summary>
	Private Sub OnSpeechRejected(sender As Object, args As SpeechRecognitionRejectedEventArgs) Handles mSpeechRecognizer.SpeechRecognitionRejected
		InterfaceMapper.GetImplementation(Of IMessageLog)().
			LogVoiceInfo("Voice command '{0}' not found", args.Result.Text)
	End Sub

#End Region


#Region " IDisposable implementation "

	Private disposedValue As Boolean ' To detect redundant calls


	' IDisposable
	Protected Overridable Sub Dispose(disposing As Boolean)
		If Not Me.disposedValue Then
			If disposing Then
				' TODO: dispose managed state (managed objects).
				mSpeechRecognizer.Dispose()
			End If

			' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
			' TODO: set large fields to null.
		End If
		Me.disposedValue = True
	End Sub


	' This code added by Visual Basic to correctly implement the disposable pattern.
	Public Sub Dispose() Implements IDisposable.Dispose
		' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
		Dispose(True)
		GC.SuppressFinalize(Me)
	End Sub

#End Region

End Class
