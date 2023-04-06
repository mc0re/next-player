Imports Common


Public Class LinearDivider2D

#Region " API "

    ''' <summary>
    ''' Check whether all give points are colinear.
    ''' </summary>
    ''' <param name="points">A list of points</param>
    Public Shared Function AreColinear(points As IEnumerable(Of IPoint2D)) As Boolean
        Dim pointList = points.ToList()
        Dim line = Line2DHelper.Create(pointList(0), pointList(1))

        For Each point In pointList.Skip(2)
            If Not line.Contains(point) Then
                Return False
            End If
        Next

        Return True
    End Function


    ''' <summary>
    ''' Create perpendiculars in the vertices and cut the room by them.
    ''' </summary>
    ''' <param name="room"></param>
    ''' <param name="verticeList"></param>
    Public Shared Sub SplitLinear(Of TRef)(
        room As Room2D,
        verticeList As IList(Of Point2D(Of TRef)),
        segmentList As IList(Of LineSegment2D(Of TRef)),
        polyList As IList(Of Polygon2D(Of TRef))
    )
        Dim commonLine = Line2DHelper.Create(verticeList.First(), verticeList.Last())

        ' Calculate coordinates and perpendiculars
        Dim coordList = (
            From v In verticeList
            Select vc = New ColinearCoordinate(Of TRef)(
                commonLine.GetCoordinate(v),
                commonLine.Perpendicular(v),
                v)
            Order By vc.Coordinate
            Select vc
        ).ToList()

        ' Process perpendiculars as segments
        For i = 0 To coordList.Count - 1
            segmentList.Add(LineSegment2DHelper.Create(
                            coordList(i).Perpendicular, coordList(i).References))
        Next

        ' Process pairs of points
        For i = 0 To coordList.Count
            ProcessPair(segmentList, polyList, room,
                        If(i > 0, coordList(i - 1), Nothing),
                        If(i < coordList.Count, coordList(i), Nothing))
        Next
    End Sub

#End Region


#Region " Utility "

    ''' <summary>
    ''' Process two adjustent points and create:
    ''' - A line segment between them
    ''' - A polygon, where the room is cut by the perpendiculars
    ''' </summary>
    ''' <param name="room">Room definition</param>
    ''' <param name="p0">Start point or Nothing</param>
    ''' <param name="p1">End point or Nothing</param>
    Private Shared Sub ProcessPair(Of TRef)(
        segmentList As IList(Of LineSegment2D(Of TRef)),
        polyList As IList(Of Polygon2D(Of TRef)),
        room As Room2D,
        p0 As ColinearCoordinate(Of TRef),
        p1 As ColinearCoordinate(Of TRef)
    )
        ' Create a list of references from one or two points
        Dim ref As New List(Of TRef)()

        If p0 IsNot Nothing Then
            ref.AddRange(p0.References)
        End If

        If p1 IsNot Nothing Then
            ref.AddRange(p1.References)
        End If

        ' Set up a pair of points for in-between match
        If p0 IsNot Nothing AndAlso p1 IsNot Nothing Then
            segmentList.Add(LineSegment2DHelper.Create(p0.Point, p1.Point))
        End If

        AddPolygon(polyList, room, p0, p1, ref)
    End Sub


    ''' <summary>
    ''' Add a cut polygon to the layout, if it is not empty.
    ''' </summary>
    Private Shared Sub AddPolygon(Of TRef)(
        polyList As IList(Of Polygon2D(Of TRef)),
        room As Room2D,
        p0 As ColinearCoordinate(Of TRef),
        p1 As ColinearCoordinate(Of TRef),
        ref As List(Of TRef)
    )
        Dim poly = Polygon2DHelper.CreateInfinite(ref)

        ' There can;t be a situation when an infinite polygon disappears
        ' when cutting by a finite room. So no check for the retun value.
        room.CutByRoom(poly)

        ' If not first, cut by the closest line
        If p0 IsNot Nothing AndAlso Not poly.CutByLine(p0.Perpendicular, Line2DDirections.Left) Then
            Return
        End If

        ' If not last, cut by the more distant line
        If p1 IsNot Nothing AndAlso Not poly.CutByLine(p1.Perpendicular, Line2DDirections.Right) Then
            Return
        End If

        polyList.Add(poly)
    End Sub

#End Region

End Class
