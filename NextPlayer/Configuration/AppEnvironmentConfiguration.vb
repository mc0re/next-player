Imports System.ComponentModel
Imports Common


''' <summary>
''' A set of application-wide settings, which change in different environments.
''' </summary>
Public Class AppEnvironmentConfiguration
	Inherits PropertyChangedHelper

#Region " Name notifying property "

	Private mName As String = "Default"


	<Category("Common Properties"), Description("Environment name")>
	Public Property Name As String
		Get
			Return mName
		End Get
		Set(value As String)
			SetField(mName, value, Function() Name)
		End Set
	End Property

#End Region


#Region " IsEditEnabled notifying property "

	Private mIsEditEnabled As Boolean = True


	<Category("Common Properties"), Description("Whether the playlist is currently editable")>
	Public Property IsEditEnabled As Boolean
		Get
			Return mIsEditEnabled
		End Get
		Set(value As Boolean)
			SetField(mIsEditEnabled, value, Function() IsEditEnabled)
		End Set
	End Property

#End Region


#Region " IsPositionChangeEnabled notifying property "

	Private mIsPositionChangeEnabled As Boolean


	<Category("Common Properties"), Description("Whether clicking on the progress indicator changes the playback position")>
	Public Property IsPositionChangeEnabled As Boolean
		Get
			Return mIsPositionChangeEnabled
		End Get
		Set(value As Boolean)
			SetField(mIsPositionChangeEnabled, value, Function() IsPositionChangeEnabled)
		End Set
	End Property

#End Region


#Region " IsVoiceControlEnabled notifying property "

	Private mIsVoiceControlEnabled As Boolean


	<Category("Common Properties"), Description("Whether voice control is enabled")>
	Public Property IsVoiceControlEnabled As Boolean
		Get
			Return mIsVoiceControlEnabled
		End Get
		Set(value As Boolean)
			SetField(mIsVoiceControlEnabled, value, Function() IsVoiceControlEnabled)
		End Set
	End Property

#End Region


#Region " VoiceControlFeedbackChannel notifying property "

	Private mVoiceControlFeedbackChannel As Integer


	<Category("Common Properties"), Description("Channel for voice feedback")>
	Public Property VoiceControlFeedbackChannel As Integer
		Get
			Return mVoiceControlFeedbackChannel
		End Get
		Set(value As Integer)
			SetField(mVoiceControlFeedbackChannel, value, Function() VoiceControlFeedbackChannel)
		End Set
	End Property

#End Region


#Region " VoiceControlFeedbackVoice notifying property "

	Private mVoiceControlFeedbackVoice As String


	<Category("Common Properties"), Description("Voice name for voice feedback")>
	Public Property VoiceControlFeedbackVoice As String
		Get
			Return mVoiceControlFeedbackVoice
		End Get
		Set(value As String)
			SetField(mVoiceControlFeedbackVoice, value, Function() VoiceControlFeedbackVoice)
		End Set
	End Property

#End Region


#Region " UseNAudio notifying property "

	Private mUseNAudio As Boolean = True


	''' <summary>
	''' Which playback library to use.
	''' </summary>
	Public Property UseNAudio As Boolean
		Get
			Return mUseNAudio
		End Get
		Set(value As Boolean)
			SetField(mUseNAudio, value, Function() UseNAudio)
			AppConfiguration.SetUpAudioLib()
		End Set
	End Property

#End Region


#Region " PlayerWindowPosition notifying property "

	Private WithEvents mPlayerWindowPosition As New WindowPosition()


	<Category("Common Properties"), Description("Player window position")>
	Public Property PlayerWindowPosition As WindowPosition
		Get
			Return mPlayerWindowPosition
		End Get
		Set(value As WindowPosition)
			SetField(mPlayerWindowPosition, value, Function() PlayerWindowPosition)
		End Set
	End Property


	Private Sub PlayerWindowPositionPropertyChanged(sender As Object, args As PropertyChangedEventArgs) Handles mPlayerWindowPosition.PropertyChanged
		RaisePropertyChanged(Me, args.PropertyName, sender)
	End Sub

#End Region


#Region " MainWindowSplit notifying property "

	Private mMainWindowSplit As GridLength


	<Category("Common Properties"), Description("Player window main splitter")>
	Public Property MainWindowSplit As GridLength
		Get
			Return mMainWindowSplit
		End Get
		Set(value As GridLength)
			SetField(mMainWindowSplit, value, Function() MainWindowSplit)
		End Set
	End Property

#End Region


#Region " StatusWindowSplit notifying property "

	Private mStatusWindowSplit As GridLength


	<Category("Common Properties"), Description("Player window status panel splitter")>
	Public Property StatusWindowSplit As GridLength
		Get
			Return mStatusWindowSplit
		End Get
		Set(value As GridLength)
			SetField(mStatusWindowSplit, value, Function() StatusWindowSplit)
		End Set
	End Property

#End Region


#Region " ToString "

	''' <summary>
	''' For debugging.
	''' </summary>
	Public Overrides Function ToString() As String
		Return Name
	End Function

#End Region

End Class
