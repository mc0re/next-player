Imports Common
Imports MIConvexHull


Public Class PlaneDivider2D

#Region " API "

    ''' <summary>
    ''' Split the room by the given vertices.
    ''' </summary>
    ''' <param name="room">Room limiting the space</param>
    ''' <param name="verticeList">A list of points to process</param>
    ''' <remarks>
    ''' The points in the faces in core are clockwise.
    ''' The points on the sides of the hull are also clockwise.
    ''' </remarks>
    Public Shared Sub SplitPlanar(Of TRef)(
        room As Room2D,
        verticeList As IList(Of Point2D(Of TRef)),
        segmentList As List(Of LineSegment2D(Of TRef)),
        polyList As List(Of Polygon2D(Of TRef))
    )
        Dim res = FindCoreAndHull(room, verticeList)

        polyList.AddRange(res.Core)
        segmentList.AddRange(CollectSegments(res.Core))
        AddHullPolygons(room, segmentList, polyList, res.Hull)
    End Sub

#End Region


#Region " Utility "

    Private Shared Function FindCoreAndHull(Of TRef)(
        room As Room2D,
        verticeList As IList(Of Point2D(Of TRef))
    ) As CoreAndHull(Of TRef)

        Dim sortedList As IList(Of Point2D(Of TRef)) = Nothing

        If verticeList.Count = 3 Then
            ' Triangulation cannot handle the trivial case of 3 points on a plane
            Return SplitTrivialCase(verticeList)
        ElseIf IsSquare(verticeList, sortedList) Then
            Return SplitSquare(sortedList)
        Else
            Return SplitUsingTriangulation(verticeList)
        End If
    End Function


    ''' <summary>
    ''' The trivial case of 3 points on a 3D plane
    ''' is not supported by MIConvexHull library.
    ''' So create a special handler.
    ''' </summary>
    ''' <typeparam name="TRef">Reference type</typeparam>
    ''' <param name="verticeList">A list of vertices</param>
    ''' <returns>Core (a single triangle) and hull definitions</returns>
    Private Shared Function SplitTrivialCase(Of TRef)(
        verticeList As IList(Of Point2D(Of TRef))
    ) As CoreAndHull(Of TRef)

        Dim core = {Polygon2DHelper.Create(verticeList)}

        ' Order the points by angle from the center of masses
        Dim centerX = verticeList.Average(Function(v) v.X)
        Dim centerY = verticeList.Average(Function(v) v.Y)
        Dim center = Point2DHelper.Create(centerX, centerY)
        Dim orderedVertices = (
            From v In verticeList
            Let angle = Vector2DHelper.Create(center, v).Angle
            Order By angle
            Select v
            ).ToList()

        ' Close the polygon
        orderedVertices.Add(orderedVertices.First())

        ' Create outer sides in the right (clockwise) order
        Dim hull = New List(Of HullSide(Of TRef))()

        For i = 0 To orderedVertices.Count - 2
            Dim a = orderedVertices(i)
            Dim b = orderedVertices(i + 1)

            hull.Add(New HullSide(Of TRef) With {
                .PointA = a, .PointB = b,
                .Normal = Vector2DHelper.Create(b, a).Perpendicular()
                })
        Next

        ' Collect adjustant normals
        For i = 0 To orderedVertices.Count - 2
            Dim nextI = If(i = orderedVertices.Count - 2, 0, i + 1)
            hull(nextI).NormalAdjA = hull(i).Normal
            hull(i).NormalAdjB = hull(nextI).Normal
        Next

        Return New CoreAndHull(Of TRef) With {.Core = core, .Hull = hull}
    End Function


    ''' <summary>
    ''' There is a special case of a square, where MIConvexHull library crashes,
    ''' see https://github.com/DesignEngrLab/MIConvexHull/issues/30
    ''' So check for it and handle manually.
    ''' </summary>
    ''' <typeparam name="TRef">Reference type</typeparam>
    ''' <param name="verticeList">A list of vertices</param>
    ''' <returns>True if the points represent a square</returns>
    Private Shared Function IsSquare(Of TRef)(
        verticeList As IList(Of Point2D(Of TRef)),
        ByRef sortedList As IList(Of Point2D(Of TRef))) As Boolean

        If verticeList.Count <> 4 Then Return False

        ' Name the points A, B, C, D; O is their center
        Dim o = Point2DHelper.Average(verticeList)

        Dim allFromO As IEnumerable(Of (Vector2D, Point2D(Of TRef))) =
            verticeList.Select(Function(p) (Vector2DHelper.Create(o, p), p)).ToList()
        If allFromO.Any(Function(t) t.Item1.IsZero) Then Return False

        Dim dist = allFromO(0).Item1.Length
        If Not allFromO.Skip(1).All(Function(t) IsEqual(t.Item1.Length, dist)) Then
            Return False
        End If

        sortedList = allFromO.OrderBy(Function(t) t.Item1.Angle).
                              Select(Function(t) t.Item2).
                              ToList()
        Return True
    End Function


    ''' <summary>
    ''' There is a special case of a square, where MIConvexHull library crashes,
    ''' see https://github.com/DesignEngrLab/MIConvexHull/issues/30
    ''' So check for it and handle manually.
    ''' </summary>
    ''' <typeparam name="TRef">Reference type</typeparam>
    ''' <param name="vertices">A list of vertices, enclosing the square, in clockwise order</param>
    ''' <returns>Core (two triangles) and hull definitions</returns>
    Private Shared Function SplitSquare(Of TRef)(
        vertices As IList(Of Point2D(Of TRef))
    ) As CoreAndHull(Of TRef)

        Dim core = {Polygon2DHelper.Create({vertices(0), vertices(1), vertices(2)}),
                    Polygon2DHelper.Create({vertices(2), vertices(3), vertices(0)})}

        ' Close the polygon
        Dim sideVertices = vertices.Append(vertices.First()).ToList()

        ' Create outer sides in the right (clockwise) order
        Dim hull = New List(Of HullSide(Of TRef))()

        For i = 0 To sideVertices.Count - 2
            Dim a = sideVertices(i)
            Dim b = sideVertices(i + 1)

            hull.Add(New HullSide(Of TRef) With {
                .PointA = a, .PointB = b,
                .Normal = Vector2DHelper.Create(b, a).Perpendicular()
                })
        Next

        ' Collect adjustant normals
        For i = 0 To sideVertices.Count - 2
            Dim nextI = If(i = sideVertices.Count - 2, 0, i + 1)
            hull(nextI).NormalAdjA = hull(i).Normal
            hull(i).NormalAdjB = hull(nextI).Normal
        Next

        Return New CoreAndHull(Of TRef) With {.Core = core, .Hull = hull}
    End Function


    Private Shared Function SplitUsingTriangulation(Of TRef)(
        verticeList As IList(Of Point2D(Of TRef))
    ) As CoreAndHull(Of TRef)

        Dim chVertices = (From v In verticeList Select Point2DVertexFactory.Create(v)).ToList()

        Dim coreCells = Triangulation.CreateDelaunay(chVertices, AbsoluteCoordPrecision).Cells
        Dim core = (From cell In coreCells
                    Let pointList = cell.Vertices.Select(Function(v) v.Point).ToList()
                    Select Polygon2DHelper.Create(pointList)).ToList()

        ' Returns points in counter-clockwise order
        Dim hullResult = ConvexHull.Create2D(chVertices, AbsoluteCoordPrecision)

        If hullResult.Outcome <> ConvexHullCreationResultOutcome.Success Then
            Throw New ArgumentException(hullResult.ErrorMessage)
        End If

        Dim hull As New List(Of HullSide(Of TRef))

        ' The 2D intreface only yields points, not sides.
        ' First, create all sides and their normals.
        For sideIdx = 0 To hullResult.Result.Count - 1
            Dim a = hullResult.Result(sideIdx).Point
            Dim b = hullResult.Result(NextIndex(sideIdx, hullResult.Result.Count)).Point
            Dim sideNormal = Vector2DHelper.Create(a, b).Perpendicular

            hull.Add(New HullSide(Of TRef) With
            {
                .PointA = a,
                .PointB = b,
                .Normal = sideNormal
            })
        Next

        ' Then, find out neighbours' normals.
        '   prev(A, B) -> current(A, B) -> next(A, B)
        '   current.A = prev.B
        '   current.B = next.A
        For sideIdx = 0 To hull.Count - 1
            Dim prevIdx = PreviousIndex(sideIdx, hull.Count)
            Dim nextIdx = NextIndex(sideIdx, hull.Count)
            hull(sideIdx).NormalAdjA = hull(prevIdx).Normal
            hull(sideIdx).NormalAdjB = hull(nextIdx).Normal
        Next

        Return New CoreAndHull(Of TRef) With {.Core = core, .Hull = hull}
    End Function


    Private Shared Function NextIndex(sideIdx As Integer, collectionSize As Integer) As Integer
        Return If(sideIdx = collectionSize - 1, 0, sideIdx + 1)
    End Function


    Private Shared Function PreviousIndex(sideIdx As Integer, collectionSize As Integer) As Integer
        Return If(sideIdx = 0, collectionSize - 1, sideIdx - 1)
    End Function


    ''' <summary>
    ''' Core polygons are just added along with their intra-connections.
    ''' All unique segments are collected in the provided list.
    ''' </summary>
    ''' <typeparam name="TRef">Point reference type</typeparam>
    ''' <param name="core">A list of polygons to traverse</param>
    Private Shared Function CollectSegments(Of TRef)(
        core As ICollection(Of Polygon2D(Of TRef))
    ) As ICollection(Of LineSegment2D(Of TRef))

        Dim segmentList = core.
            SelectMany(Function(p) p.Sides.Cast(Of LineSegment2D(Of TRef))()).
            Distinct(LineSegment2DHelper.EqualityComparer).
            ToList()

        Return segmentList
    End Function


    ''' <summary>
    ''' Hull sides are processed to create polygons towards the room borders.
    ''' </summary>
    ''' <typeparam name="TRef">Type of point references</typeparam>
    ''' <param name="room">Room limiting the space</param>
    ''' <param name="polyList">[out] List to add polygons to</param>
    ''' <param name="segmentList">[out] List to add separating segments to</param>
    ''' <param name="hull">Information about segments on the convex hull</param>
    ''' <remarks>
    ''' Split the hull segments if necessary (cross-checking with <paramref name="segmentList"/>).
    ''' 
    ''' From each vertex create an "average normal" line and a "audience projection" line.
    ''' If they do not match, create a "blind area", where only one vertex is active.
    ''' Also add both lines as segments.
    ''' </remarks>
    Private Shared Sub AddHullPolygons(Of TRef)(
        room As Room2D,
        segmentList As List(Of LineSegment2D(Of TRef)),
        polyList As IList(Of Polygon2D(Of TRef)),
        hull As IList(Of HullSide(Of TRef))
    )
        ' THe center mass of the hull
        Dim midPoint = Point2DHelper.Average(hull.SelectMany(Function(h) {h.PointA, h.PointB}))

        ' Split the segments, flip them
        hull = SplitSegments(hull, segmentList)
        hull = FlipSegments(hull, midPoint)

        ' Calculate rays for each unique point, add as segments
        Dim rayCollection = hull.ToDictionary(
            Function(s) s.PointA,
            Function(s) GetDividingRays(room, s.PointA, s.Normal, s.NormalAdjA),
            Point2DHelper.EqualityComparer)
        segmentList.AddRange(rayCollection.SelectMany(Function(r) r.Value))

        ' Create polygon(s) for each hull side
        For Each side In hull
            Dim raysA = rayCollection(side.PointA)
            Dim raysB = rayCollection(side.PointB)

            If raysA.Count > 1 Then
                ' Create a blind zone starting at point A by cutting the room by the rays
                ' Point B will be taken care of by another segment
                Dim blind = Polygon2DHelper.CreateInfinite(
                    ReferencesHelper.MergeReferences(raysA))
                If room.CutByRoom(blind) AndAlso
                   blind.CutByLine(raysA.First().Line, Line2DDirections.Right) AndAlso
                   blind.CutByLine(raysA.Last().Line, Line2DDirections.Left) Then

                    polyList.Add(blind)
                End If
            End If

            ' Create a polygon for the hull segment
            Dim poly = Polygon2DHelper.CreateInfinite(
                ReferencesHelper.MergeReferences({side.PointA, side.PointB}))

            ' a-b is always clockwise
            If room.CutByRoom(poly) AndAlso
               poly.CutByLine(raysA.Last().Line, Line2DDirections.Right) AndAlso
               poly.CutByLine(raysB.First().Line, Line2DDirections.Left) AndAlso
               poly.CutByLine(Line2DHelper.Create(side.PointA, side.PointB), Line2DDirections.Left) Then

                polyList.Add(poly)
            End If
        Next
    End Sub


    ''' <summary>
    ''' In lack of the corresponding MIConvexHull functionality,
    ''' we need to manually find the minimal segments
    ''' </summary>
    ''' <typeparam name="TRef">Type of point references</typeparam>
    ''' <param name="hull">Original hull segments to split</param>
    ''' <param name="coreSegments">Core segments to extract pivot points</param>
    ''' <returns></returns>
    Private Shared Function SplitSegments(Of TRef)(
        hull As IList(Of HullSide(Of TRef)),
        coreSegments As IList(Of LineSegment2D(Of TRef))
    ) As IList(Of HullSide(Of TRef))

        Dim splitPointList = coreSegments.
            SelectMany(Function(s) {s.PointA, s.PointB}).
            Distinct(Point2DHelper.EqualityComparer).
            ToList()

        ' Split hull segments if needed
        For Each splitPoint In splitPointList
            Dim hullCopy = hull.ToList()

            For Each hullSide In hull
                Dim hullSegm = LineSegment2DHelper.Create(hullSide.PointA, hullSide.PointB)

                If hullSegm.Contains(splitPoint) Then
                    hullCopy.Remove(hullSide)
                    hullCopy.Add(New HullSide(Of TRef) With {
                        .PointA = hullSide.PointA,
                        .PointB = splitPoint,
                        .Normal = hullSide.Normal,
                        .NormalAdjA = hullSide.NormalAdjA,
                        .NormalAdjB = hullSide.Normal})
                    hullCopy.Add(New HullSide(Of TRef) With {
                        .PointA = splitPoint,
                        .PointB = hullSide.PointB,
                        .Normal = hullSide.Normal,
                        .NormalAdjA = hullSide.Normal,
                        .NormalAdjB = hullSide.NormalAdjB})
                End If
            Next

            hull = hullCopy
        Next

        Return hull
    End Function


    ''' <summary>
    ''' Rotate the segments so point A is before point B (clockwise).
    ''' </summary>
    ''' <typeparam name="TRef">Type of point references</typeparam>
    ''' <param name="hull">Segments to process</param>
    ''' <param name="midPoint">Point to use as a center of eyesight</param>
    Private Shared Function FlipSegments(Of TRef)(hull As IList(Of HullSide(Of TRef)), midPoint As IPoint2D) As IList(Of HullSide(Of TRef))
        Dim res = New List(Of HullSide(Of TRef))()

        For Each side In hull
            Dim rayA = Vector2DHelper.Create(midPoint, side.PointA)
            Dim rayB = Vector2DHelper.Create(midPoint, side.PointB)

            If Vector2DHelper.AngleComparer.Compare(rayA.Angle, rayB.Angle) > 0 Then
                res.Add(New HullSide(Of TRef) With {
                    .PointA = side.PointB,
                    .PointB = side.PointA,
                    .Normal = side.Normal,
                    .NormalAdjA = side.NormalAdjB,
                    .NormalAdjB = side.NormalAdjA})
            Else
                res.Add(side)
            End If
        Next

        Return res
    End Function


    ''' <summary>
    ''' Get average normal and projection lines in the clockwise order,
    ''' if they are different lines.
    ''' </summary>
    ''' <remarks>
    ''' The line segments are technically rays (only have one end).
    ''' </remarks>
    Private Shared Function GetDividingRays(Of TRef)(
        room As Room2D,
        vertex As Point2D(Of TRef),
        normal As Vector2D,
        adjNormal As Vector2D
    ) As IList(Of LineSegment2D(Of TRef))

        Dim projVector = CreateProjectionLine(room, vertex, normal, adjNormal).Vector
        Dim normalVector = normal.Plus(adjNormal)
        Dim res As New List(Of LineSegment2D(Of TRef))()

        Dim projToNorm = Vector2DHelper.CompareAngles(projVector.Angle, normalVector.Angle)

        If projToNorm = 0 Then
            res.Add(LineSegment2DHelper.Create(vertex, normalVector, vertex.References))
        Else
            res.AddRange(
                From v In If(projToNorm < 0, {projVector, normalVector}, {normalVector, projVector})
                Let line = Line2DHelper.Create(vertex, v)
                Select LineSegment2DHelper.Create(line, 0, Double.PositiveInfinity, vertex.References))
        End If

        Return res
    End Function


    ''' <summary>
    ''' Create a line from the point's projection towards the point itself.
    ''' </summary>
    Private Shared Function CreateProjectionLine(
        room As Room2D,
        point As IPoint2D,
        normal As Vector2D,
        adjNormal As Vector2D
    ) As ILine2D

        Dim proj = room.ProjectToAudience(point)

        If Not Point2DHelper.IsSame(point, proj) Then
            Return Line2DHelper.Create(proj, point)
        End If

        ' No direction, peek average normal with the adjustant side
        Dim v = If(adjNormal Is Nothing, normal, normal.Plus(adjNormal))

        Return Line2DHelper.Create(proj, v)
    End Function

#End Region

End Class
