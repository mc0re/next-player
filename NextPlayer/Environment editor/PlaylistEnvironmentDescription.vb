Imports Common


''' <summary>
''' Represents an existance of a given configuration for the given machine
''' in the playlist.
''' </summary>
Public Class PlaylistEnvironmentDescription
	Inherits PropertyChangedHelper

#Region " Properties "

	Public Property EnvName As String

	Public Property MachineName As String

#End Region


#Region " Exists notifying property "

	Private mExists As Boolean


	Public Property Exists As Boolean
		Get
			Return mExists
		End Get
		Set(value As Boolean)
			If mExists = value Then Return

			If value Then
				ConfigurationManager.CreatePlaylist(EnvName, MachineName)
			Else
				ConfigurationManager.DeletePlaylist(EnvName, MachineName)
			End If

			SetField(mExists, value, Function() Exists)
		End Set
	End Property

#End Region


#Region " Init and clean-up "

	Public Sub New(eName As String, machine As String, ex As Boolean)
		EnvName = eName
		MachineName = machine
		mExists = ex
	End Sub

#End Region

End Class
