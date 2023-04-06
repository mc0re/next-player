Imports Common


''' <summary>
''' User-friendly representation of a command.
''' </summary>
Public Class ReportVoiceCommandSetting

#Region " Helper classes "

	''' <summary>
	''' Representation of VoiceCommandConfigItem.
	''' </summary>
	Private Class ReportVoiceCommandConfigItem
		Inherits ReportDataMirror(Of VoiceCommandConfigItem)

		Public Property CommandName As String

		Public Property IsEnabled As Boolean

		Public Property RecognitionText As String


		Public Sub Copy(config As VoiceCommandConfigItem)
			CopyFields(config)
		End Sub

	End Class


	''' <summary>
	''' Representation of VoiceCommandDescription.
	''' </summary>
	Private Class ReportVoiceCommandDescription
		Inherits ReportDataMirror(Of VoiceCommandDescription)

		Public Property ParameterType As String	' CommandParameterTypes

		Public Property Description As String


		Public Sub Copy(desc As VoiceCommandDescription)
			CopyFields(desc)
		End Sub

	End Class

#End Region


#Region " Backing fields "

	Private ReadOnly mConfig As New ReportVoiceCommandConfigItem()

	Private ReadOnly mDesc As New ReportVoiceCommandDescription()

#End Region


#Region " VoiceCommandConfigItem properties "

	Public ReadOnly Property CommandName As String
		Get
			Return mConfig.CommandName
		End Get
	End Property


	Public ReadOnly Property IsEnabled As Boolean
		Get
			Return mConfig.IsEnabled
		End Get
	End Property


	Public ReadOnly Property RecognitionText As String
		Get
			Return mConfig.RecognitionText
		End Get
	End Property

#End Region


#Region " VoiceCommandDescription properties "

	Public ReadOnly Property ParameterType As String	' CommandParameterTypes
		Get
			Return mDesc.ParameterType
		End Get
	End Property


	Public ReadOnly Property Description As String
		Get
			Return mDesc.Description
		End Get
	End Property

#End Region


#Region " Init and clean-up "

	Public Sub New(config As VoiceCommandConfigItem, desc As VoiceCommandDescription)
		mConfig.Copy(config)
		mDesc.Copy(desc)
	End Sub

#End Region

End Class
