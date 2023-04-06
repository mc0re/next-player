Imports Common
Imports MIConvexHull


<CLSCompliant(True)>
Public Class RoomLayouterOld

#Region " Events "

	Public Event LayoutChanged()

#End Region


#Region " Fields "

	Private mRoom As RoomDefinition


	Private mPolyList As RoomPolyhedronList

#End Region


#Region " Coordinate conversion API "

	''' <summary>
	''' Convert the given relative coordinates to absolute according to the room.
	''' </summary>
	Public Function ConvertCoords(c As IPositionRelative) As Point3D
		Return mRoom.Convert(c)
	End Function


	''' <summary>
	''' Project the given point to the audience (at plane Z=0).
	''' </summary>
	Public Function ProjectToAudience(c As IPoint3D) As IPoint3D
		Return mRoom.ProjectToAudience(c)
	End Function

#End Region


#Region " Layout API "

	''' <summary>
	''' Process room and physical outputs information to speed up the playback calculations.
	''' </summary>
	''' <remarks>
	''' Project all speakers from audience to room walls or a sphere around room center.
	''' Find the closest speaker projections by creating a convex hull.
	''' Use pairs of speakers from the hull faces to build planes to separate the space.
	''' </remarks>
	Public Sub PrepareLayout(room As RoomDefinition, phList As IEnumerable(Of AudioPhysicalChannel))
		SetRoom(room)
		mPolyList = New RoomPolyhedronList(room)
		mPolyList.Clear()

		' Create a list of projection points from the original channels
		Dim spkProj = (
				From ch In phList
				Select mRoom.GetProjectedPoint(ch, ProjectionModes.Sphere)
			).ToList()

		spkProj = MergePointsAlongLine(spkProj)

		' Check layout type and create the layout
		Dim pl As IList(Of ProjectionRoomToPlane) = Nothing

		If spkProj.Count = 0 Then
			' No speakers

		ElseIf spkProj.Count = 1 Then
			' Single speaker, take whole room
			mPolyList.Add(New RoomPolyhedron(spkProj.First(), room))

		ElseIf CalcProjectionPlane(spkProj, pl) Then
			mPolyList.AddRange(CreatePolyhedronsFromPlanar(pl))

		Else
			mPolyList.AddRange(CreateFrom3d(spkProj))
		End If

		mPolyList.CollectAllSpeakers()
		RaiseEvent LayoutChanged()
	End Sub


	''' <summary>
	''' Get a list of output channels to be used for the given simulated point.
	''' </summary>
	''' <remarks>
	''' Get all real physical channels, and only those that should be used.
	''' Disabled channels are also returned.
	''' </remarks>
	Public Function GetLayout(c As IPoint3D) As IList(Of AudioPhysicalChannel)
		Dim st = mPolyList.FindProjections(c)

		Return st.Aggregate(New List(Of AudioPhysicalChannel)(),
					 Function(list, p)
						 list.AddRange(p.Speakers.Cast(Of AudioPhysicalChannel)())
						 Return list
					 End Function).
				ToList()
	End Function

#End Region


#Region " Visualization API "

	''' <summary>
	''' Get a list of polyhedrons for this layout (might be empty).
	''' </summary>
	Public Function GetPolyhedrons() As IReadOnlyList(Of RoomPolyhedron(Of AudioPhysicalChannel))
		Dim res As New List(Of RoomPolyhedron(Of AudioPhysicalChannel))()

		' If none, this layout covers whole room
		Return If(mPolyList, New List(Of RoomPolyhedron(Of AudioPhysicalChannel))())
	End Function


	Public Function GetContainingPolyhedron(c As IPoint3D) As RoomPolyhedron(Of AudioPhysicalChannel)
		Return (
			From p In mPolyList Where p.CheckPoint(c) IsNot Nothing
		).FirstOrDefault()
	End Function

#End Region


#Region " Layout utility: Common "

	''' <summary>
	''' This method needs to be called before room is referenced
	''' by <see cref="CreatePolyhedronsFromPlanar"/>.
	''' </summary>
	Friend Sub SetRoom(room As RoomDefinition)
		mRoom = room
	End Sub


	''' <summary>
	''' Check co-placement of projection points, merge if needed.
	''' </summary>
	Private Shared Function MergePointsAlongLine(spkProj As List(Of ProjectionToRoom)) As List(Of ProjectionToRoom)
		If spkProj.Count <= 1 Then
			Return spkProj
		End If

		Dim mergedProj As New List(Of ProjectionToRoom)()
		Dim untested = spkProj

		While untested.Any()
			Dim pivotSpk = untested.First()

			untested = (
				From testSpk In untested.Skip(1)
				Where Not pivotSpk.Merge(testSpk)
				Select testSpk
			).ToList()

			mergedProj.Add(pivotSpk)
		End While

		Return mergedProj
	End Function

#End Region


#Region " Layout utility: Plane "

	Private Class PlaneInfo
		Public Plane As Plane3D
		Public Point As ProjectionToRoom
	End Class


	''' <summary>
	''' Check if all projections lie on a single plane.
	''' If so, calculate the projections.
	''' </summary>
	''' <returns>False if the points are not on the same plane</returns>
	''' <remarks>
	''' The function puts the result into its argument.
	''' This is done to chain the checks in the main method.
	''' </remarks>
	Friend Function CalcProjectionPlane(
		pts As IList(Of ProjectionToRoom),
		ByRef planeProjections As IList(Of ProjectionRoomToPlane)
	) As Boolean
		Debug.Assert(pts.Count >= 2)
		Debug.Assert(Not Point3DHelper.IsSame(pts(0), pts(1)))

		Dim planeAllPoints As Plane3D

		' Some first points might be on the same line,
		' so they don't define a plane yet.
		' Any 3 non-colinear points would do.
		Dim ln As New Line3D(pts(0), pts(1))
		Dim pNotOnLine As IPoint3D = Nothing

		For Each pt In pts.Skip(2)
			If Not ln.GetPointOnLine(pt).HasValue Then
				pNotOnLine = pt
				Exit For
			End If
		Next

		If pNotOnLine IsNot Nothing Then
			planeAllPoints = Plane3D.Create3Points(pts(0), pts(1), pNotOnLine)
		Else
			' Points are colinear, so create a plane,
			' which goes through the all these points,
			' and is perpendicular to a vector MIDDLE_POINT -> 0.
			' If the vector is a 0-vector, the plane must be perpendicular
			' to the line between these points.
			' NB. For spherical projection, no 3 or more points
			' can lie on the same line.
			Dim vectTo0 = Vector3D.CreateA2B(
				Point3D.Average(pts.ToArray()), Point3DHelper.Origin)
			Dim normal = If(
				Not vectTo0.IsZero,
				vectTo0,
				New Line3D(pts(0), pts(1)).Vector.AnyPerpendicular)

			planeAllPoints = Plane3D.CreateAlongLine(
				pts(0), New Line3D(pts(0), pts(1)), normal)
		End If

		Dim res As New List(Of ProjectionRoomToPlane)()

		For Each pt In pts
			Dim proj = planeAllPoints.GetPointOnPlane(pt)

			If proj Is Nothing Then Return False
			res.Add(New ProjectionRoomToPlane(planeAllPoints, proj, pt))
		Next

		planeProjections = res
		Return True
	End Function


	''' <summary>
	''' For the given list of coplanar projected points,
	''' create a collection of polyhedrons,
	''' dividing the room.
	''' </summary>
	''' <remarks>
	''' The points must be vertices of a convex polygon.
	''' They must not be colinear, unless there are only 2 points.
	''' 
	''' First, create a closed polygon from all points and check,
	''' whether it contains the origin. If not, open the polygon.
	''' 
	''' For each pair of points (A, B), the polyhedrons sides are:
	''' - a plane going through point A
	'''   along a projection line from A to the audience plane and
	'''   perpendicular to the common projection plane;
	''' - same for point B;
	''' - if the audience plane is not cut by the speakers plane,
	'''   a plane parallel to the speakers plane going through the
	'''   audience plane.
	'''   
	''' If the origin is outside the polygon, also take into consideration
	''' a plane parallel to the projection plane, but going through the
	''' closest point of the audience rectangle.
	''' </remarks>
	Friend Function CreatePolyhedronsFromPlanar(
		points As IList(Of ProjectionRoomToPlane)
	) As IEnumerable(Of RoomPolyhedron)
		Debug.Assert(points.Count > 0)

		Dim projPlane = points.First().Plane
		Dim audIntersPlane = IntersectWithAudience(projPlane)
		Dim speakerSide = If(
			audIntersPlane Is projPlane, 0, GetPointsSide(audIntersPlane, points))
		Dim poly = CreatePolygon(points)
		Dim planeList = CreatePlanes(poly)

		Dim res As New List(Of RoomPolyhedron)()

		' Create polyhedrons, corresponding to each pair of division planes
		For plIdx = 0 To planeList.Count - 2
			Dim sideA = planeList(plIdx).Plane
			Dim sideB = planeList(plIdx + 1).Plane
			Dim pointA = planeList(plIdx).Point
			Dim pointB = planeList(plIdx + 1).Point

			Dim signAToB = Sign(sideA.GetDistanceToPoint(sideB.Point))
			Debug.Assert(signAToB <> 0)

			Dim sideList As New List(Of PolyhedronSide)()

			If sideA.IsSame(sideB) Then
				' If sides A and B are the same
				sideList.Add(New PolyhedronSide(sideA, signAToB, {pointA, pointB}))
			Else
				sideList.Add(New PolyhedronSide(sideA, signAToB, {pointA}))
				sideList.Add(New PolyhedronSide(sideB, -signAToB, {pointB}))

				If speakerSide <> 0 Then
					sideList.Add(New PolyhedronSide(audIntersPlane, speakerSide, {}))
				End If
			End If

			res.Add(New RoomPolyhedron(sideList, mRoom))
		Next

		Return res
	End Function


	''' <summary>
	''' Get intersection of <paramref name="projPlane"/> with audience area.
	''' </summary>
	''' <returns>
	''' A new plane parallel to <paramref name="projPlane"/> or
	''' <paramref name="projPlane"/>, if the audience lies within it
	''' </returns>
	Private Function IntersectWithAudience(projPlane As Plane3D) As Plane3D
		' Find the distance to all audience area corners
		Dim distToAud = (
			From ap In mRoom.AudiencePoints
			Let offset = projPlane.GetDistanceToPoint(ap)
			Let sign = Sign(offset)
		).ToList()

		Dim res As Plane3D
		Dim minSign = distToAud.Min(Function(d) d.sign)

		If distToAud.Max(Function(d) d.sign) <> minSign Then
			' Projection plane crosses the audience,
			' so all the sounds shall be audible
			res = projPlane

		ElseIf minSign = 0 Then
			' If the distance is 0, polygon lies in the audience plane;
			' so all the sounds shall be audible
			res = projPlane

		Else
			' Projection plane does not cross the audience
			' Move to the closest audience point
			Dim minDist = distToAud.Min(
				Function(d) Math.Abs(d.offset / projPlane.Normal.Length)) * minSign

			res = projPlane.CreateShifted(minDist)
		End If

		Return res
	End Function


	''' <summary>
	''' On which side from the given plane lie the given points;
	''' 0 if this shouldn't be considered as a separation.
	''' </summary>
	Private Function GetPointsSide(
		plane As Plane3D,
		points As IList(Of ProjectionRoomToPlane)
	) As Integer

		Return Sign(plane.GetDistanceToPoint(points.First().RoomProj))
	End Function


	''' <summary>
	''' Create an opened or a closed polygon with the given vertices.
	''' </summary>
	Private Shared Function CreatePolygon(
		points As IList(Of ProjectionRoomToPlane)
	) As Polygon2D(Of ProjectionRoomToPlane)
		Debug.Assert(points.Count >= 2)

		If points.Count > 3 Then
			' Make sure the points are listed along a circle
			points = ReorderPlanarPoints(points)
		End If

		' Create a closed polygon
		Dim poly As New Polygon2D(Of ProjectionRoomToPlane)(points)

		' Check whether the origin is enclosed in the created polygon;
		' if not so, open the polygon
		Dim projPlane = points.First().Plane
		Dim origin = projPlane.ProjectToPlane(Point3DHelper.Origin)
		poly.Break(origin)

		Return poly
	End Function


	''' <summary>
	''' Reorder the given points to that they lie along a circle.
	''' </summary>
	Private Shared Function ReorderPlanarPoints(
		points As IList(Of ProjectionRoomToPlane)
	) As IList(Of ProjectionRoomToPlane)

		' The projections are coplanar - i.e. they form a circle,
		' reorder them into a convex hull.
		Dim hull = ConvexHull.Create(points, AbsoluteCoordPrecision)

		' The hull provides the line segments, but they are unordered
		Dim res As New List(Of ProjectionRoomToPlane)()

		Dim firstSegm = hull.Faces.First()
		Dim segm = firstSegm
		Do
			res.Add(segm.Vertices(0))
			segm = segm.Adjacency(0) ' Shared point is #1
		Loop Until segm Is firstSegm

		Return res
	End Function


	''' <summary>
	''' Calculate planes through polygon's vertices.
	''' </summary>
	''' <param name="poly">Polygon to traverse</param>
	''' <remarks>
	''' If the polygon is closed, the first plane is added as the last one
	''' as well to allow looking at pairs of planes.
	''' </remarks>
	Private Shared Function CreatePlanes(
		poly As Polygon2D(Of ProjectionRoomToPlane)
	) As IList(Of PlaneInfo)
		Dim planeList = (
			From vIdx In Enumerable.Range(0, poly.Vertices.Count)
			Let pt = poly.Vertices(vIdx).RoomProj
			Let prevPt =
				If(vIdx > 0, poly.Vertices(vIdx - 1).RoomProj,
				If(poly.IsOpened, Nothing,
				poly.Vertices.Last().RoomProj))
			Let nextPt =
				If(vIdx < poly.Vertices.Count - 1, poly.Vertices(vIdx + 1).RoomProj,
				If(poly.IsOpened, Nothing,
				poly.Vertices.First().RoomProj))
			Let v = Vector3D.AverageVector(prevPt, pt, nextPt)
			Select New PlaneInfo With {
				.Plane = Plane3D.CreateAlongLine(pt, pt.LineFromAudience, v),
				.Point = pt
			}
		).ToList()

		If Not poly.IsOpened Then
			planeList.Add(planeList.First())
		End If

		Return planeList
	End Function

#End Region


#Region " Layout utility: 3D "

	''' <summary>
	''' Create room division for a non-planar colocation of points.
	''' </summary>
	Friend Function CreateFrom3d(
		spkProj As IList(Of ProjectionToRoom)
	) As IEnumerable(Of RoomPolyhedron)
		Dim res As New List(Of RoomPolyhedron)()
		Dim hull = ConvexHull.Create(spkProj, AbsoluteCoordPrecision)

		' If it does not contain origin, remove the faces looking away from it
		' NB. This depends on Helix library to not mess up the order of vertices
		Dim points =
			From f In hull.Faces
			Let p0 = f.Vertices(0)
			Let p1 = f.Vertices(1)
			Let p2 = f.Vertices(2)
			Let signs = (
				From vi In Enumerable.Range(0, 3)
				Let s = Sign(f.Vertices(vi).Z)
				Order By s
				Select s)
			Let isSameSign = (signs.Last() = signs.First())
			Let fplane = Plane3D.Create3Points(p0, p1, p2)
			Where Sign(fplane.GetDistanceToPoint(Point3DHelper.Origin)) > 0
			Select New With {.face = f, .isSameSign = isSameSign, .sign = signs.First()}

		For Each f In points
			' 3 points for each face => 3 side planes
			Dim ab = CreateSidesFromEdge(f.face, 0, 1, 2)
			Dim bc = CreateSidesFromEdge(f.face, 1, 2, 0)
			Dim ca = CreateSidesFromEdge(f.face, 2, 0, 1)

			Dim sides = ab.Concat(bc).Concat(ca).ToList()

			If f.isSameSign Then
				sides.Add(New PolyhedronSide(
					Plane3D.CreatePointNormal(Point3DHelper.Origin, Vector3D.AlongZ),
					f.sign, {}))
			End If

			res.Add(New RoomPolyhedron(sides, mRoom))
		Next

		Return res
	End Function


	''' <summary>
	''' Create a set of polyhedron sides for the given face edge.
	''' </summary>
	''' <param name="face">Face definition</param>
	''' <param name="idx1">Point 1 on the edge</param>
	''' <param name="idx2">Point 2 on the edge</param>
	''' <param name="idxInside">Point not on the edge</param>
	Private Shared Function CreateSidesFromEdge(
		face As DefaultConvexFace(Of ProjectionToRoom), idx1 As Integer, idx2 As Integer, idxInside As Integer
	) As IReadOnlyList(Of PolyhedronSide)

		Dim proj1 = face.Vertices(idx1)
		Dim proj2 = face.Vertices(idx2)

		' 1 or 2 points on the audience plane
		Dim planes As New List(Of Plane3D)()

		If Point3DHelper.IsSame(proj1.SpeakerToAudience, proj2.SpeakerToAudience) Then
			planes.Add(Plane3D.Create3Points(proj1, proj2, proj1.SpeakerToAudience))
		Else
			Dim bottom1 = proj1.SpeakerToAudience
			Dim bottom2 = Point3D.Average(proj1.SpeakerToAudience, proj2.SpeakerToAudience)
			Dim bottom3 = proj2.SpeakerToAudience

			planes.Add(Plane3D.Create3Points(proj1, bottom1, bottom2))
			planes.Add(Plane3D.Create3Points(proj1, proj2, bottom2))
			planes.Add(Plane3D.Create3Points(proj2, bottom2, bottom3))
		End If

		' If the face goes along the plane towards audience, ignore this plane
		Return (
			From pl In planes
			Let inside = Sign(pl.GetDistanceToPoint(face.Vertices(idxInside)))
			Where inside <> 0
			Select New PolyhedronSide(pl, inside, {proj1, proj2})
		).ToList()
	End Function

#End Region

End Class
