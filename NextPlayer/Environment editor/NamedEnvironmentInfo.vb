Imports System.ComponentModel
Imports Common


''' <summary>
''' This class represents a whole row in the environment editor,
''' along with local and playlist flags.
''' </summary>
Public Class NamedEnvironmentInfo
	Inherits PropertyChangedHelper

#Region " Name notifying property "

	Private mName As String


	Public Property Name As String
		Get
			Return mName
		End Get
		Set(value As String)
			If String.IsNullOrWhiteSpace(value) Then Return

			ConfigurationManager.Rename(mName, value)
			SetField(mName, value, Function() Name)
		End Set
	End Property

#End Region


#Region " HasLocal notifying property "

	Private mHasLocal As Boolean


	Public Property HasLocal As Boolean
		Get
			Return mHasLocal
		End Get
		Set(value As Boolean)
			If mHasLocal = value Then Return

			If value Then
				ConfigurationManager.CreateLocal(Name)
			Else
				ConfigurationManager.DeleteLocal(Name)
			End If

			SetField(mHasLocal, value, Function() HasLocal)
		End Set
	End Property

#End Region


#Region " Playlist read-only property "

	Private mPlaylist As New BindingList(Of PlaylistEnvironmentDescription)


	Public ReadOnly Property Playlist As BindingList(Of PlaylistEnvironmentDescription)
		Get
			Return mPlaylist
		End Get
	End Property

#End Region


#Region " Init and clean-up "

	Public Sub New(envName As String, isLocal As Boolean)
		mName = envName
		mHasLocal = isLocal
	End Sub

#End Region


#Region " ToString "

	Public Overrides Function ToString() As String
		Dim str = String.Format("{0}, {1} machines in playlist", Name, Playlist.Count)

		If HasLocal Then
			str &= " + local"
		End If

		Return str
	End Function

#End Region

End Class
