''' <summary>
''' Helper class for division visualization.
''' </summary>
Public Class RoomDivisionTriangle

#Region " Properties "

	Public ReadOnly A As PhysicalChannelLocation

	Public ReadOnly B As PhysicalChannelLocation

	Public ReadOnly C As PhysicalChannelLocation

	Public ReadOnly Speakers As IList(Of Integer)


	Public ReadOnly Property ContainsSpeaker As Boolean
		Get
			Return A.LocationNumber > 0 OrElse B.LocationNumber > 0 OrElse C.LocationNumber > 0
		End Get
	End Property

#End Region


#Region " Init and clean-up "

	Private Sub New(
					p1 As PhysicalChannelLocation, p2 As PhysicalChannelLocation, p3 As PhysicalChannelLocation,
					spList As IList(Of Integer)
					)
		A = p1
		B = p2
		C = p3
		Speakers = spList
	End Sub

#End Region


#Region " Factory "

	Public Shared Function CreateFrom(tetrah As RoomPolyhedron) As IList(Of RoomDivisionTriangle)
		Dim res As New List(Of RoomDivisionTriangle)()
		Dim vertList = tetrah.GetVertices()

		If vertList.Count < 3 Then Return res

		Dim spList = (From v In vertList Where v.LocationNumber > 0 Select v.LocationNumber).ToList()

		If vertList.Count = 3 Then
			res.Add(New RoomDivisionTriangle(vertList(0), vertList(1), vertList(2), spList))
		Else
			' 4 points, 4 triangles
			Debug.Assert(vertList.Count = 4)

			res.Add(New RoomDivisionTriangle(vertList(0), vertList(1), vertList(2), spList))
			res.Add(New RoomDivisionTriangle(vertList(0), vertList(1), vertList(3), spList))
			res.Add(New RoomDivisionTriangle(vertList(0), vertList(2), vertList(3), spList))
			res.Add(New RoomDivisionTriangle(vertList(1), vertList(2), vertList(3), spList))
		End If

		Return res
	End Function

#End Region


#Region " Comparison "

	''' <summary>
	''' Compare the two triangles by locations and speakers.
	''' </summary>
	Public Function IsSameAs(other As RoomDivisionTriangle) As Boolean
		If A.LocationNumber <> other.A.LocationNumber OrElse
		   B.LocationNumber <> other.B.LocationNumber OrElse
		   C.LocationNumber <> other.C.LocationNumber Then Return False

		If Speakers.Count <> other.Speakers.Count Then Return False

		For spIdx = 0 To Speakers.Count - 1
			If Speakers(spIdx) <> other.Speakers(spIdx) Then Return False
		Next

		Return True
	End Function

#End Region

End Class
