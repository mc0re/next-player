Imports Common
Imports AudioChannelLibrary
Imports System.Xml.Serialization
Imports System.IO


Public Class LayoutConfiguration

#Region " Fields "

	Private Shared Serializer As New XmlSerializer(GetType(LayoutConfiguration))

#End Region


#Region " Properties "

	Public Property Room As Room3D


	Public Property Channels As ChannelCollection(Of AudioPhysicalChannel)

#End Region


#Region " API "

	''' <summary>
	''' Write the data into the given file.
	''' </summary>
	Public Sub Export(fileName As String)
		Using f = File.OpenWrite(fileName)
			Serializer.Serialize(f, Me)
		End Using
	End Sub

#End Region

End Class
