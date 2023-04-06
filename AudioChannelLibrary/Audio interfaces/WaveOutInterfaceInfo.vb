Imports Common


''' <summary>
''' Information about a single WaveOut device.
''' </summary>
Public Class WaveOutInterfaceInfo
	Inherits PropertyChangedHelper

#Region " DeviceNumber notifying property "

	Private mDeviceNumber As Integer


	Public Property DeviceNumber As Integer
		Get
			Return mDeviceNumber
		End Get
		Set(value As Integer)
			SetField(mDeviceNumber, value, NameOf(DeviceNumber))
		End Set
	End Property

#End Region


#Region " Name notifying property "

	Private mName As String


	Public Property Name As String
		Get
			Return mName
		End Get
		Set(value As String)
			SetField(mName, value, NameOf(Name))
		End Set
	End Property

#End Region


#Region " Channels notifying property "

	Private mChannels As Integer


	Public Property Channels As Integer
		Get
			Return mChannels
		End Get
		Set(value As Integer)
			SetField(mChannels, value, NameOf(Channels))
		End Set
	End Property

#End Region

End Class
