''' <summary>
''' Configuration for voice control commands.
''' </summary>
Public Interface IVoiceConfiguration

	Property IsVoiceControlEnabled As Boolean


	ReadOnly Property VoiceCommands As VoiceCommandConfigItemCollection


	ReadOnly Property VoiceControlFeedbackChannel As Integer


	ReadOnly Property VoiceControlFeedbackVoice As String

End Interface
