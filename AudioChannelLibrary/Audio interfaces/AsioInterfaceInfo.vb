Imports Common


Public Class AsioInterfaceInfo
	Inherits PropertyChangedHelper

#Region " DriverName notifying property "

	Private mDriverName As String


	Public Property DriverName As String
		Get
			Return mDriverName
		End Get
		Set(value As String)
			SetField(mDriverName, value, NameOf(DriverName))
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
