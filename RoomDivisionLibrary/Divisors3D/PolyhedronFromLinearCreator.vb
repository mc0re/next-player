Imports Common


''' <summary>
''' Utility class for creating polyhedrons.
''' </summary>
Public Class PolyhedronFromLinearCreator

#Region " API "

    ''' <summary>
    ''' Check if all points lie on a single line.
    ''' If so, calculate the projections.
    ''' </summary>
    ''' <returns>False if the points are not on the same line</returns>
    ''' <remarks>
    ''' The function puts the result into its argument.
    ''' This is done to chain the checks in the caller method.
    ''' </remarks>
    Public Shared Function CalcProjectionLine(Of TRef)(
        pointList As ICollection(Of Point3D(Of TRef)),
        ByRef projToLine As IList(Of PointAsVertex1D(Of TRef))
    ) As Boolean
        Debug.Assert(pointList.Count >= 2)

        Dim ln = Line3DHelper.Create(pointList(0), pointList(1))
        Dim res As New List(Of PointAsVertex1D(Of TRef))

        ' Check colinearity
        For Each pt In pointList
            Dim ptCoord = ln.GetCoordinate(pt)

            If Not ptCoord.HasValue Then Return False

            res.Add(PointAsVertex1DHelper.Create(Of TRef)(ln, ptCoord.Value, pt))
        Next

        ' Sort by the single projected coordinate
        projToLine = res.OrderBy(Function(a) a.Projection).ToList()

        Return True
    End Function


    ''' <summary>
    ''' For the given list of colinear projected points,
    ''' create a collection of polyhedron sides,
    ''' dividing the room in slices.
    ''' </summary>
    ''' <remarks>
    ''' We'll create the core division right here,
    ''' so <see cref="DivisionResult(Of TPoint).Shell"/> will be empty.
    ''' </remarks>
    Public Shared Function Create(Of TRef)(
        projectionLine As IList(Of PointAsVertex1D(Of TRef)),
        room As Room3D
    ) As DivisionResult(Of TRef)

        ' Make sure there are some points to work with
        Debug.Assert(projectionLine.Count >= 2)

        Dim separationPlanes = CreateSeparationPlanes(projectionLine, room)
        Dim audPlane = room.GetAudiencePlane()
        Dim sidePairs = (
            From idx In Enumerable.Range(0, separationPlanes.Count - 1)
            Let pl1 = separationPlanes(idx)
            Let pl2 = separationPlanes(idx + 1)
            Select CreatePair(pl1, pl2, audPlane)
        ).ToList()

        Dim polyList = sidePairs.
            SelectMany(
                Function(pair) CreatePolyhedrons(pair, audPlane, room)).
            ToList()

        ' Create left-most and right-most polyhedrons
        Dim leftMost = sidePairs.First()
        polyList.Insert(0, PolyhedronHelper.CreateFromBorders(
                     room.AllSideBorders(Of TRef).Concat({
                     Plane3DHelper.CreateBorder(leftMost.Plane1, -leftMost.Direction1To2)}).
                     ToList()))
        Dim rightMost = sidePairs.Last()
        polyList.Add(PolyhedronHelper.CreateFromBorders(
                     room.AllSideBorders(Of TRef).Concat({
                     Plane3DHelper.CreateBorder(rightMost.Plane2, -rightMost.Direction2To1)}).
                     ToList()))

        Dim res As New DivisionResult(Of TRef)(room)

        If polyList.Any(Function(p) Not p.Sides.Any()) Then
            res.Core.Add(PolyhedronHelper.CreateFromBorders(
                room.AllSideBorders(Of TRef),
                projectionLine.SelectMany(Function(pr) pr.OriginalPoint.References)))
        Else
            res.Core.AddRange(polyList)
        End If

        Return res
    End Function


    ''' <summary>
    ''' Create planes along the line, perpendicular to it.
    ''' The audience location is ignored, as this is not a normal 3D case,
    ''' and we can cut some corners.
    ''' </summary>
    Private Shared Function CreateSeparationPlanes(Of TRef)(
        projToLine As IList(Of PointAsVertex1D(Of TRef)),
        room As Room3D
    ) As IReadOnlyList(Of Plane3D(Of TRef))

        Dim ln = projToLine.First().Line

        ' For each point create a plane along the projection line,
        ' having that point as a reference object.
        Return (
            From p In projToLine
            Select Plane3DHelper.CreatePointNormal(p.OriginalPoint, ln.Vector, p.OriginalPoint.References)
        ).ToList()
    End Function


    ''' <summary>
    ''' Collect information for a pair of planes needed to create polyhedrons between and outside them.
    ''' </summary>
    ''' <typeparam name="TRef">Type of point references</typeparam>
    ''' <param name="plane1">Plane 1 "left"</param>
    ''' <param name="plane2">Plane 2 "right"</param>
    ''' <param name="audiencePlane">Audience plane</param>
    Private Shared Function CreatePair(Of TRef)(
        plane1 As Plane3D(Of TRef), plane2 As Plane3D(Of TRef), audiencePlane As IPlane3D
    ) As Plane3DPair(Of TRef)
        ' Get direction between the planes.
        ' Usually, as they were created along the same normal direction,
        ' direction from plane 2 to plane 1 is the opposite of 1 to 2.
        ' But one or both of them could be 0, if a point lies on the other plane -
        ' for instance, when they both are on the audience plane.
        ' Get directions from the audience plane to the side planes.
        Dim res = New Plane3DPair(Of TRef) With {
            .Plane1 = plane1,
            .Plane2 = plane2,
            .Direction1To2 = Sign(plane1.GetDistanceToPoint(plane2.Point)),
            .Direction2To1 = Sign(plane2.GetDistanceToPoint(plane1.Point)),
            .DirectionAudTo1 = Sign(audiencePlane.GetDistanceToPoint(plane1.Point)),
            .DirectionAudTo2 = Sign(audiencePlane.GetDistanceToPoint(plane2.Point)),
            .References = ReferencesHelper.MergeReferences({ .Plane1, .Plane2})
        }

        Return res
    End Function


    ''' <summary>
    ''' Create polyhedrons generated by the two audio planes and audience plane,
    ''' separating the room.
    ''' </summary>
    ''' <remarks>
    ''' The room is cut into pieces by the audience plane and the given planes.
    ''' Each piece is a polyhedron, combining references from the separation planes.
    ''' </remarks>
    Private Shared Function CreatePolyhedrons(Of TRef)(
        pair As Plane3DPair(Of TRef),
        audPlane As IPlane3D,
        room As Room3D
    ) As IReadOnlyList(Of Polyhedron(Of TRef))

        ' If any of the points lie on the other plane, they are considered
        ' "colocated", and a "filling" polyhedron is returned.
        ' The handling could have been more forgiving, but this is a corner case,
        ' probably only occuring during room design phase.
        If pair.Direction1To2 = 0 OrElse pair.Direction2To1 = 0 Then
            Return {PolyhedronHelper.CreateFromBorders(
                        room.AllSideBorders(Of TRef)(), pair.References)}
        End If

        ' If the audience plane separates the points, create two polyhedrons -
        ' one above the audience plane and one below it.
        ' Even thought this is an oversimplification (this should only
        ' be done if the points are on two different sides of the audience horizontally,
        ' not only vertically), this is not a real use-case, so it doesn't matter
        ' much, just it behaves somewhat normally.
        If pair.DirectionAudTo1 <> 0 AndAlso pair.DirectionAudTo2 = -pair.DirectionAudTo1 Then
            Dim poly1 = PolyhedronHelper.CreateFromBorders(
                    room.AllSideBorders(Of TRef).Concat({
                    Plane3DHelper.CreateBorder(pair.Plane1, pair.Direction1To2),
                    Plane3DHelper.CreateBorderNoRef(Of TRef)(audPlane, pair.DirectionAudTo1)}).
                    ToList())
            Dim poly2 = PolyhedronHelper.CreateFromBorders(
                    room.AllSideBorders(Of TRef).Concat({
                    Plane3DHelper.CreateBorder(pair.Plane2, pair.Direction2To1),
                    Plane3DHelper.CreateBorderNoRef(Of TRef)(audPlane, pair.DirectionAudTo2)}).
                    ToList())
            Return {poly1, poly2}
        End If

        Dim sides = 
            room.AllSideBorders(Of TRef).Concat({
            Plane3DHelper.CreateBorder(pair.Plane1, pair.Direction1To2),
            Plane3DHelper.CreateBorder(pair.Plane2, pair.Direction2To1)
        }).ToList()

        ' If the audience plane needs to be included,
        ' make sure it contains the points, which belong to it.
        If pair.DirectionAudTo1 <> 0 AndAlso pair.DirectionAudTo2 <> 0 Then
            Dim audPlaneRef = (
                From p In pair.References
                Where audPlane.IsPointOnPlane(Point3DHelper.Origin)
            ).ToList()

            sides.Add(Plane3DHelper.CreateBorder(audPlane, 1, audPlaneRef))
        End If

        Return {PolyhedronHelper.CreateFromBorders(sides)}
    End Function

#End Region

End Class
