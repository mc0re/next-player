''' <summary>
''' Configuration for voice control commands.
''' </summary>
Public Interface IVoiceConfiguration

	ReadOnly Property VoiceCommands As VoiceCommandConfigItemCollection


	Property IsVoiceControlEnabled As Boolean


	Property VoiceControlFeedbackChannel As Integer


	Property VoiceControlFeedbackVoice As String

End Interface
