Imports Common
Imports MIConvexHull


<CLSCompliant(True)>
Public Class Room3DLayouter(Of TRef)
    Implements I3DLayouter(Of TRef)

#Region " Events "

    Public Event LayoutChanged() Implements I3DLayouter(Of TRef).LayoutChanged

#End Region


#Region " Fields "

    Private mPolyList As RoomPolyhedronList(Of TRef)

#End Region


#Region " ILayouter properties "

    Private mRoom As Room3D


    Public ReadOnly Property Room As Room3D Implements I3DLayouter(Of TRef).Room
        Get
            Return mRoom
        End Get
    End Property

#End Region


#Region " Layout API "

    ''' <summary>
    ''' Process room and physical outputs information to speed up the playback calculations.
    ''' </summary>
    ''' <remarks>
    ''' Project all speakers from audience to room walls or a sphere around room center.
    ''' Find the closest speaker projections by creating a triangulation.
    ''' For the areas outside the convex hull, use pairs of speakers
    ''' from the faces to build up planes to separate the space.
    ''' </remarks>
    Public Sub PrepareLayout(room As Room3D, speakers As IEnumerable(Of Point3D(Of TRef))) Implements I3DLayouter(Of TRef).PrepareLayout
        mRoom = room
        mPolyList = New RoomPolyhedronList(Of TRef)()
        Dim spkList = speakers.ToList()

        ' Check layout type and create the layout
        Dim div As New DivisionResult(Of TRef)(room)
        Dim projToLine As IList(Of PointAsVertex1D(Of TRef)) = Nothing
        Dim projToPlane As IList(Of PointAsVertex2D(Of TRef)) = Nothing

        If PolyhedronFromSingleCreator.AreOnePoint(spkList) Then
            ' No speakers or single speaker - take whole room
            div = PolyhedronFromSingleCreator.Create(spkList, room)

        ElseIf PolyhedronFromLinearCreator.CalcProjectionLine(spkList, projToLine) Then
            ' All are colinear
            div = PolyhedronFromLinearCreator.Create(projToLine, room)

        ElseIf PolyhedronFromPlanarCreator.CalcProjectionPlane(Of TRef)(spkList, projToPlane) Then
            ' All are coplanar
            div = PolyhedronFromPlanarCreator.Create(projToPlane, room)

        Else
            ' Real 3D
            div = PolyhedronFrom3DCreator.Create(spkList, room)
        End If

        mPolyList.AddRange(div.Core)

        ' Now cover the rest of the room space
        Dim hulls = CreateToAndFromAudience(div.Shell, room, ProjectionModes.Sphere)
        mPolyList.AddRange(hulls.TowardsAudience)
        mPolyList.AddRange(hulls.AwayFromAudience)

        ' We might need to create a look-up tree to speed up the search
        mPolyList.CollectAllVertices(spkList)

        RaiseEvent LayoutChanged()
    End Sub


    ''' <summary>
    ''' Get a list of output channels to be used for the given simulated point.
    ''' </summary>
    ''' <remarks>
    ''' Get all real physical channels, and only those that should be used.
    ''' Disabled channels are also returned.
    ''' </remarks>
    Public Function GetReferences(c As IPoint3D) As IReadOnlyCollection(Of TRef) Implements I3DLayouter(Of TRef).GetReferences
        Return mPolyList.FindRefForPoint(c).ToList()
    End Function

#End Region


#Region " Layout utility "

    ''' <summary>
    ''' Process the shell and create polyhedrons towards the audience and
    ''' away from it.
    ''' </summary>
    Private Shared Function CreateToAndFromAudience(
        faces As ICollection(Of ShellFace(Of TRef)),
        room As Room3D,
        mode As ProjectionModes
    ) As DivisionHull(Of TRef)

        Dim res As New DivisionHull(Of TRef)()

        ' We calculate room projection for all points in all faces,
        ' thus we're bound to calculate the same points multiple times.
        ' This is deemed easier to maintain than a more complex alternative,
        ' and shouldn't be too time-consuming anyway.
        Dim faceProjections = (
            From f In faces
            Let facePlane = Plane3DHelper.CreatePointNormal(f.Points.First(), f.Outside, f.Points.First().References)
            Let projs = (From p In f.Points Select room.ProjectPoint(p, mode)).ToList()
            Let dirs = (From prj In projs Select Sign(facePlane.GetDistanceToPoint(prj.PointA))).ToList()
            Let allPositive = dirs.All(Function(s) s > 0)
            Let allNegative = dirs.All(Function(s) s < 0)
            Let pi = New PlaneInfo(Of TRef) With {
                .Points = f.Points.Cast(Of IPoint3D)().ToList(),
                .PointsPlane = facePlane,
                .Projections = projs,
                .Direction =
                    If(allPositive, FaceDirections.ToAudience,
                    If(allNegative, FaceDirections.FromAudience,
                    FaceDirections.Unknown))
                }
            Select pi
        ).ToList()

        'Dim projPlane = points.First().Plane
        'Dim audIntersPlane = IntersectWithAudience(projPlane, room)
        'Dim speakerSide = If(
        '	audIntersPlane Is projPlane, 0, GetPointsSide(audIntersPlane, points))
        'Dim poly = CreatePolygon(points)
        'Dim planeList = CreatePlanes(poly)

        '' Create polyhedrons, corresponding to each pair of division planes
        'For plIdx = 0 To planeList.Count - 2
        '	Dim sideA = planeList(plIdx).PointsPlane
        '	Dim sideB = planeList(plIdx + 1).PointsPlane
        '	Dim pointA = planeList(plIdx).Point
        '	Dim pointB = planeList(plIdx + 1).Point

        '	Dim signAToB = Sign(sideA.GetDistanceToPoint(sideB.Point))
        '	Debug.Assert(signAToB <> 0)

        '	Dim sideList As New List(Of PolyhedronSide(Of TPoint))()

        '	If Plane3DHelper.IsSame(sideA, sideB) Then
        '		' If sides A and B are the same
        '		sideList.Add(New PolyhedronSide(Of TPoint)(sideA, signAToB, {pointA.Ref, pointB.Ref}))
        '	Else
        '		sideList.Add(New PolyhedronSide(Of TPoint)(sideA, signAToB, {pointA.Ref}))
        '		sideList.Add(New PolyhedronSide(Of TPoint)(sideB, -signAToB, {pointB.Ref}))

        '		If speakerSide <> 0 Then
        '			sideList.Add(New PolyhedronSide(Of TPoint)(audIntersPlane, speakerSide, {}))
        '		End If
        '	End If

        '	res.Core.Add(PolyhedronHelper.CreateFromPlanes(sideList, room))
        'Next

        Return res
    End Function


    ''' <summary>
    ''' Get intersection of <paramref name="projPlane"/> with audience area.
    ''' </summary>
    ''' <returns>
    ''' A new plane parallel to <paramref name="projPlane"/> or
    ''' <paramref name="projPlane"/>, if the audience lies within it
    ''' </returns>
    Private Shared Function IntersectWithAudience(
        projPlane As IPlane3D, room As Room3D
    ) As IPlane3D

        ' Find the distance to all audience area corners
        Dim distToAud = (
            From ap In room.AudiencePoints
            Let offset = projPlane.GetDistanceToPoint(ap)
            Let sign = Sign(offset)
        ).ToList()

        Dim res As IPlane3D
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

            res = projPlane.Shift(minDist)
        End If

        Return res
    End Function


    ''' <summary>
    ''' On which side from the given plane lie the given points;
    ''' 0 if this shouldn't be considered as a separation.
    ''' </summary>
    Private Shared Function GetPointsSide(
        plane As IPlane3D,
        points As IList(Of PointAsVertex2D(Of TRef))
    ) As Integer

        'Return Sign(plane.GetDistanceToPoint(points.First().Original))
        Return 0
    End Function


    ''' <summary>
    ''' Create an opened or a closed polygon with the given vertices.
    ''' </summary>
    Private Shared Function CreatePolygon(
        points As IList(Of PointAsVertex2D(Of TRef))
    ) As Polygon2D(Of TRef)
        Debug.Assert(points.Count >= 2)

        If points.Count > 3 Then
            ' Make sure the points are listed along a circle
            points = ReorderPlanarPoints(points)
        End If

        ' Create a closed polygon
        'Dim poly = Polygon2DFactory.Create(points)

        '' Check whether the origin is enclosed in the created polygon;
        '' if not so, open the polygon
        'Dim projPlane = points.First().Plane
        'Dim origin = projPlane.GetProjection(Point3DHelper.Origin)
        'poly.Break(origin)

        'Return poly
        Return Nothing
    End Function


    ''' <summary>
    ''' Reorder the given points to that they lie along a circle.
    ''' </summary>
    Private Shared Function ReorderPlanarPoints(
        points As IList(Of PointAsVertex2D(Of TRef))
    ) As IList(Of PointAsVertex2D(Of TRef))

        ' The projections are coplanar - i.e. they form a circle,
        ' reorder them into a convex hull.
        Dim hull = ConvexHull.Create(points, AbsoluteCoordPrecision)

        ' The hull provides the line segments, but they are unordered
        Dim res As New List(Of PointAsVertex2D(Of TRef))()

        Dim firstSegm = hull.Result.Faces.First()
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
    Private Shared Function CreatePlanes(Of TCookie)(
        poly As Polygon2D(Of PointAsVertex2D(Of TCookie))
    ) As IList(Of PlaneInfo(Of TRef))
        'Dim planeList = (
        '	From vIdx In Enumerable.Range(0, poly.Vertices.Count)
        '	Let pt = poly.Vertices(vIdx).RoomProj
        '	Let prevPt =
        '		If(vIdx > 0, poly.Vertices(vIdx - 1).RoomProj,
        '		If(poly.IsOpened, Nothing,
        '		poly.Vertices.Last().RoomProj))
        '	Let nextPt =
        '		If(vIdx < poly.Vertices.Count - 1, poly.Vertices(vIdx + 1).RoomProj,
        '		If(poly.IsOpened, Nothing,
        '		poly.Vertices.First().RoomProj))
        '	Let v = Vector3D.AverageVector(prevPt, pt, nextPt)
        '	Select New PlaneInfo With {
        '		.Plane = Plane3D.CreateAlongLine(pt, pt.LineFromAudience, v),
        '		.Point = pt
        '	}
        ').ToList()

        'If Not poly.IsOpened Then
        '	planeList.Add(planeList.First())
        'End If

        'Return planeList
        Return {}
    End Function

#End Region


#Region " Visualization API "

    ''' <summary>
    ''' Get a list of polyhedrons for this layout (might be empty).
    ''' </summary>
    Public Function GetPolyhedrons() As IEnumerable(Of Polyhedron(Of TRef)) Implements I3DLayouter(Of TRef).GetPolyhedrons
        Dim res As New List(Of Polyhedron(Of TRef))()

        ' If none, this layout covers whole room
        Return If(mPolyList, New List(Of Polyhedron(Of TRef))())
    End Function


    Public Function GetContainingPolyhedron(c As IPoint3D) As Polyhedron(Of TRef) Implements I3DLayouter(Of TRef).GetContainingPolyhedron
        Return (
            From p In mPolyList Where p.Contains(c)
        ).FirstOrDefault()
    End Function

#End Region

End Class
