''' <summary>
''' Configuration for voice control commands.
''' </summary>
Public Interface IVoiceConfiguration

	Property IsVoiceControlEnabled As Boolean


	ReadOnly Property VoiceCommands As VoiceCommandConfigItemCollection

End Interface
