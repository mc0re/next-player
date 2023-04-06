Imports System.Reflection


''' <summary>
''' Metadata identifying the player.
''' </summary>
<Serializable>
Public Class PlaylistMetadata

#Region " Helper field "

	Private ReadOnly mEntryAsm As Assembly = Assembly.GetEntryAssembly()

#End Region


#Region " Properties "

	Public Property PlayerName As String = If(mEntryAsm Is Nothing,
											  "Test",
											  mEntryAsm.GetCustomAttribute(Of AssemblyProductAttribute)().Product)


#Region " PlayerVersion property "

	Private mPlayerVersion As Version = If(mEntryAsm Is Nothing,
										   New Version(),
										   mEntryAsm.GetName().Version)


	Public Property PlayerVersion As String
		Get
			Return mPlayerVersion.ToString()
		End Get
		Set(value As String)
			Version.TryParse(value, mPlayerVersion)
		End Set
	End Property

#End Region


	Public Property PlayerLink As String = "http://nextplayer.nikitins.dk"


	Public Property FileType As String

#End Region


#Region " Init and clean-up "

	''' <summary>
	''' Serialization constructor.
	''' </summary>
	Public Sub New()
		' Do nothing
	End Sub


	Public Sub New(fileType As String)
		Me.FileType = fileType
	End Sub

#End Region

End Class
