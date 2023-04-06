''' <summary>
''' This class is used for storing the setting for each command in app.config.
''' </summary>
<Serializable>
Public Class VoiceCommandConfigItem

#Region " CommandName property "

	Public Property CommandName As String

#End Region


#Region " IsEnabled property "

	Public Property IsEnabled As Boolean

#End Region


#Region " RecognitionText property "

	Public Property RecognitionText As String

#End Region


#Region " Init and clean-up "

	' ReSharper disable once MemberCanBePrivate.Global

	''' <summary>
	''' Default constructor for serialization.
	''' </summary>
	Public Sub New()
		' Do nothing
	End Sub


	''' <summary>
	''' Create a new disabled setting.
	''' </summary>
	''' <param name="cmdName">System-defined command name (e.g. StopCommand)</param>
	''' <param name="recText">Text for voice recognition</param>
	Public Sub New(cmdName As String, recText As String)
		CommandName = cmdName
		RecognitionText = recText
		IsEnabled = False
	End Sub

#End Region


#Region " ToString "

	Public Overrides Function ToString() As String
		Return String.Format("{0}{1} = {2}",
							 If(IsEnabled, "+", "-"), CommandName, RecognitionText)
	End Function

#End Region

End Class
