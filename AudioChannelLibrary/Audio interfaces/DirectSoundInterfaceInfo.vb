Imports Common


''' <summary>
''' Information about a single DirectSound device.
''' </summary>
Public Class DirectSoundInterfaceInfo
	Inherits PropertyChangedHelper

#Region " DeviceGuid notifying property "

	Private mDeviceGuid As Guid


	Public Property DeviceGuid As Guid
		Get
			Return mDeviceGuid
		End Get
		Set(value As Guid)
			SetField(mDeviceGuid, value, NameOf(DeviceGuid))
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

End Class
