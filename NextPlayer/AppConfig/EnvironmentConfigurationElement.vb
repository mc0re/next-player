Imports System.ComponentModel
Imports System.Configuration
Imports Common


''' <summary>
''' A set of settings, which depend on the environment.
''' </summary>
Public Class EnvironmentConfigurationElement
	Inherits ConfigurationElement

#Region " Name property "

	<ConfigurationProperty(NameOf(Name), IsKey:=True, DefaultValue:="Default")>
	Public Property Name As String
		Get
			Return CStr(Me(NameOf(Name)))
		End Get
		Set(value As String)
			Me(NameOf(Name)) = value
		End Set
	End Property

#End Region


#Region " IsEditEnabled property "

	<ConfigurationProperty(NameOf(IsEditEnabled), DefaultValue:=True)>
	Public Property IsEditEnabled As Boolean
		Get
			Return CBool(Me(NameOf(IsEditEnabled)))
		End Get
		Set(value As Boolean)
			Me(NameOf(IsEditEnabled)) = value
		End Set
	End Property

#End Region


#Region " IsPositionChangeEnabled property "

	<ConfigurationProperty(NameOf(IsPositionChangeEnabled))>
	Public Property IsPositionChangeEnabled As Boolean
		Get
			Return CBool(Me(NameOf(IsPositionChangeEnabled)))
		End Get
		Set(value As Boolean)
			Me(NameOf(IsPositionChangeEnabled)) = value
		End Set
	End Property

#End Region


#Region " IsVoiceControlEnabled property "

	<ConfigurationProperty(NameOf(IsVoiceControlEnabled))>
	Public Property IsVoiceControlEnabled As Boolean
		Get
			Return CBool(Me(NameOf(IsVoiceControlEnabled)))
		End Get
		Set(value As Boolean)
			Me(NameOf(IsVoiceControlEnabled)) = value
		End Set
	End Property

#End Region


#Region " VoiceControlFeedbackChannel property "

	<ConfigurationProperty(NameOf(VoiceControlFeedbackChannel))>
	Public Property VoiceControlFeedbackChannel As Integer
		Get
			Return CInt(Me(NameOf(VoiceControlFeedbackChannel)))
		End Get
		Set(value As Integer)
			Me(NameOf(VoiceControlFeedbackChannel)) = value
		End Set
	End Property

#End Region


#Region " VoiceControlFeedbackVoice property "

	<ConfigurationProperty(NameOf(VoiceControlFeedbackVoice))>
	Public Property VoiceControlFeedbackVoice As String
		Get
			Return CStr(Me(NameOf(VoiceControlFeedbackVoice)))
		End Get
		Set(value As String)
			Me(NameOf(VoiceControlFeedbackVoice)) = value
		End Set
	End Property

#End Region


#Region " UseNAudio property "

	<ConfigurationProperty(NameOf(UseNAudio), DefaultValue:=True)>
	Public Property UseNAudio As Boolean
		Get
			Return CBool(Me(NameOf(UseNAudio)))
		End Get
		Set(value As Boolean)
			Me(NameOf(UseNAudio)) = value
		End Set
	End Property

#End Region


#Region " PlayerWindowPosition property "

	<ConfigurationProperty(NameOf(PlayerWindowPosition))>
	Public Property PlayerWindowPositionInternal As WindowPositionConfigurationElement
		Get
			Dim wnd = TryCast(Me(NameOf(PlayerWindowPosition)), WindowPositionConfigurationElement)
			Return If(wnd, WindowPositionConfigurationElement.FromModel(New WindowPosition()))
		End Get
		Set(value As WindowPositionConfigurationElement)
			Me(NameOf(PlayerWindowPosition)) = value
		End Set
	End Property


	Public Property PlayerWindowPosition As WindowPosition
		Get
			Return PlayerWindowPositionInternal.ToModel()
		End Get
		Set(value As WindowPosition)
			PlayerWindowPositionInternal = WindowPositionConfigurationElement.FromModel(value)
		End Set
	End Property

#End Region


#Region " MainWindowSplit property "

	<ConfigurationProperty(NameOf(MainWindowSplit), DefaultValue:="310")>
	<TypeConverter(GetType(GridLengthConfigurationConverter))>
	Public Property MainWindowSplit As GridLength
		Get
			Return CType(Me(NameOf(MainWindowSplit)), GridLength)
		End Get
		Set(value As GridLength)
			Me(NameOf(MainWindowSplit)) = value
		End Set
	End Property

#End Region


#Region " StatusWindowSplit property "

	<ConfigurationProperty(NameOf(StatusWindowSplit), DefaultValue:="150")>
	<TypeConverter(GetType(GridLengthConfigurationConverter))>
	Public Property StatusWindowSplit As GridLength
		Get
			Return CType(Me(NameOf(StatusWindowSplit)), GridLength)
		End Get
		Set(value As GridLength)
			Me(NameOf(StatusWindowSplit)) = value
		End Set
	End Property

#End Region

End Class
