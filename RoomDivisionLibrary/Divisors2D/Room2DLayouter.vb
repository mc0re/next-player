Imports Common


Partial Public Class Room2DLayouter(Of TRef)
    Implements I2DLayouter(Of TRef)

#Region " Events "

    Public Event LayoutChanged() Implements I2DLayouter(Of TRef).LayoutChanged

#End Region


#Region " Fields "

    ''' <summary>
    ''' When looking for a match, first check exact match with points.
    ''' </summary>
    Private ReadOnly mPoints As New List(Of Point2D(Of TRef))()


    ''' <summary>
    ''' When looking for a match, second check if the given point is exactly between a pair.
    ''' </summary>
    Private ReadOnly mSegments As New List(Of LineSegment2D(Of TRef))()


    ''' <summary>
    ''' When looking for a match, finally look for a polygon.
    ''' </summary>
    Private ReadOnly mPolygons As New List(Of Polygon2D(Of TRef))()

#End Region


#Region " ILayouter properties "

    Private mRoom As Room2D


    Public ReadOnly Property Room As Room2D Implements I2DLayouter(Of TRef).Room
        Get
            Return mRoom
        End Get
    End Property

#End Region


#Region " ILayouter API "

    Public Sub PrepareLayout(room As Room2D, vertices As IEnumerable(Of Point2D(Of TRef))) Implements I2DLayouter(Of TRef).PrepareLayout
        mRoom = room
        mPoints.Clear()
        mSegments.Clear()
        mPolygons.Clear()

        mPoints.AddRange(Point2DHelper.MergePoints(vertices))

        If SingleDivider2D.IsSingle(mPoints) Then
            SingleDivider2D.SplitSingle(room, mPoints, mPolygons)

        ElseIf LinearDivider2D.AreColinear(mPoints) Then
            LinearDivider2D.SplitLinear(room, mPoints, mSegments, mPolygons)

        Else
            PlaneDivider2D.SplitPlanar(room, mPoints, mSegments, mPolygons)
        End If
    End Sub


    ''' <summary>
    ''' Retrieve a list of references to be used for the given point.
    ''' </summary>
    ''' <param name="location"></param>
    ''' <returns>0 or more references</returns>
    Public Function GetReferences(location As IPoint2D) As IReadOnlyCollection(Of TRef) Implements I2DLayouter(Of TRef).GetReferences
        ' 0. Outside the room?
        If Not mRoom.Borders.Contains(location) Then
            Return New List(Of TRef)()
        End If

        ' 1. Check for exact point match
        Dim exact = mPoints.Where(Function(p) Point2DHelper.IsSame(p, location)).ToList()
        If exact.Any() Then
            Return exact.Single().References
        End If

        ' 2. Check for segments
        Dim pair = mSegments.FirstOrDefault(Function(p) p.Contains(location))
        If pair IsNot Nothing Then
            Return pair.References
        End If

        ' 3. Check polygons
        Dim inside = mPolygons.FirstOrDefault(Function(p) p.Contains(location))

        If inside Is Nothing Then
            Return New TRef() {}
        Else
            Return inside.References.ToList()
        End If
    End Function


#End Region


#Region " Visualization and test API "

    Public Function GetPolygons() As IEnumerable(Of Polygon2D(Of TRef)) Implements I2DLayouter(Of TRef).GetPolygons
        Return mPolygons
    End Function


    Public Function GetContainingPolygon(location As IPoint2D) As Polygon2D(Of TRef) Implements I2DLayouter(Of TRef).GetContainingPolygon
        Return mPolygons.FirstOrDefault(Function(p) p.Contains(location))
    End Function

#End Region

End Class
