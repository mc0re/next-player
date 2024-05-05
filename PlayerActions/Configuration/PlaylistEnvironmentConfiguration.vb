Imports Common
Imports AudioChannelLibrary
Imports TextChannelLibrary
Imports LicenseLibrary


''' <summary>
''' A set of playlist settings, which change in different environments.
''' </summary>
<Serializable>
Public Class PlaylistEnvironmentConfiguration
	Inherits PropertyChangedHelper

#Region " Fields "

	Private mIsLoaded As Boolean

#End Region


#Region " Name notifying property "

	Private mName As String


	''' <summary>
	''' Configuration name.
	''' </summary>
	Public Property Name As String
		Get
			Return mName
		End Get
		Set(value As String)
			SetField(mName, value, Function() Name)
		End Set
	End Property

#End Region


#Region " MachineId property "

	Public Property MachineId As String

#End Region


#Region " MachineName property "

	Public Property MachineName As String

#End Region


#Region " AudioOutput notifying property "

	Private WithEvents mAudioOutput As AudioEnvironmentStorage


	Public Property AudioOutput As AudioEnvironmentStorage
		Get
			Return mAudioOutput
		End Get
		Set(value As AudioEnvironmentStorage)
			SetField(mAudioOutput, value, Function() AudioOutput)
		End Set
	End Property


	Private Sub AudioOutputPropertyChangedHandler() Handles mAudioOutput.PropertyChanged
		RaisePropertyChanged(NameOf(AudioOutput))
		If Not mIsLoaded Then Return

		' Make sure defaults are created
		AudioOutput.AfterLoad()
		AudioOutput.SetRoom(mRoom)
	End Sub

#End Region


#Region " TextOutput notifying property "

	Private WithEvents mTextOutput As TextEnvironmentStorage


	Public Property TextOutput As TextEnvironmentStorage
		Get
			Return mTextOutput
		End Get
		Set(value As TextEnvironmentStorage)
			SetField(mTextOutput, value, Function() TextOutput)
		End Set
	End Property


	Private Sub TextOutputPropertyChangedHandler() Handles mTextOutput.PropertyChanged
		RaisePropertyChanged(NameOf(TextOutput))
		If Not mIsLoaded Then Return

		' Make sure defaults are created
		TextOutput.AfterLoad()
	End Sub

#End Region


#Region " Room notifying property "

	Private WithEvents mRoom As Room3D


	Public Property Room As Room3D
		Get
			Return mRoom
		End Get
		Set(value As Room3D)
			SetField(mRoom, value, Function() Room)
		End Set
	End Property


	Private Sub RoomPropertyChangedHandler() Handles mRoom.PropertyChanged
		RaisePropertyChanged(NameOf(Room))
		If Not mIsLoaded Then Return

		If AudioOutput IsNot Nothing Then
			AudioOutput.SetRoom(mRoom)
		End If
	End Sub

#End Region


#Region " Init and clean-up "

	Public Sub New()
		Name = "Default"
		MachineId = MachineFingerPrint.MachineInstance.FingerPrint
		MachineName = Environment.MachineName
		AudioOutput = New AudioEnvironmentStorage()
		TextOutput = New TextEnvironmentStorage()
		Room = New Room3D()
	End Sub

#End Region


#Region " Ensure the correct setup "

	Public Sub AfterLoad()
		AudioOutput.AfterLoad()
		AudioOutput.SetRoom(Room)
		TextOutput.AfterLoad()
		InterfaceMapper.GetImplementation(Of ISpeechSynthesizer)().Setup()

		mIsLoaded = True
	End Sub

#End Region


#Region " Clone "

	Public Shadows Function Clone() As PlaylistEnvironmentConfiguration
		' Create a shallow copy
		Dim cl = MyBase.Clone(Of PlaylistEnvironmentConfiguration)

		' Take a deep copy of complex objects
		cl.AudioOutput = AudioOutput.Clone(Of AudioEnvironmentStorage)()
		cl.TextOutput = TextOutput.Clone(Of TextEnvironmentStorage)()
		cl.Room = Room.Clone(Of Room3D)()

		Return cl
	End Function

#End Region


#Region " ToString "

	''' <summary>
	''' For debugging.
	''' </summary>
	Public Overrides Function ToString() As String
		Return String.Format("{0} on {1}", Name, MachineName)
	End Function

#End Region

End Class
