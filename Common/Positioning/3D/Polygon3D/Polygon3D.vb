''' <summary>
''' Represents a flat polygon in 3D space.
''' </summary>
''' <remarks>
''' As the polygon is cut, the original points are not preserved.
''' Therefore this class is not generic.
''' </remarks>
<CLSCompliant(True)>
<Serializable>
Public Class Polygon3D(Of TRef)
    Implements IPolygon3D, IRefKeeper(Of TRef)

#Region " Fields "

    ''' <summary>
    ''' Flat representation of the polygon on <see cref="Plane"/>.
    ''' </summary>
    ''' <remarks>
    ''' All geomettrical calculations are passed on to this polygon.
    ''' </remarks>
    Private ReadOnly mPolygon2D As Polygon2D(Of TRef)

#End Region


#Region " IRefKeeper.References read-only property "

    Private ReadOnly mReferences As New List(Of TRef)()


    Public ReadOnly Property References As IReadOnlyCollection(Of TRef) Implements IRefKeeper(Of TRef).References
        Get
            Return mReferences
        End Get
    End Property

#End Region


#Region " IPolygon3D properties "

    Public ReadOnly Property Plane As IPlane3D Implements IPolygon3D.Plane


    Public ReadOnly Property Vertices As IReadOnlyCollection(Of IPoint3D) Implements IPolygon3D.Vertices
        Get
            Return (
                From v In mPolygon2D.Vertices
                Select Plane.GetPoint(v)
                ).ToList()
        End Get
    End Property


    Public ReadOnly Property IPolygon3DSides As IReadOnlyCollection(Of ILineSegment3D) Implements IPolygon3D.Sides
        Get
            Return (
                From s In mPolygon2D.Sides
                Select LineSegment3DHelper.Create(
                    Plane.GetPoint(s.Line.GetPoint(s.CoordinateA)),
                    Plane.GetPoint(s.Line.GetPoint(s.CoordinateB)))
                ).ToList()
        End Get
    End Property

#End Region


#Region " Sides read-only property "

    Public ReadOnly Property Sides As IReadOnlyCollection(Of LineSegment3D(Of TRef))
        Get
            Return (
                From side In mPolygon2D.Sides
                Select LineSegment3DHelper.Create(
                    Plane.GetPoint(side.Line.GetPoint(side.CoordinateA)),
                    Plane.GetPoint(side.Line.GetPoint(side.CoordinateB)),
                    side.References)
                ).ToList()
        End Get
    End Property

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Create an infinite polygon, which lies on the given plane.
    ''' </summary>
    Public Sub New(plane As IPlane3D)
        Me.Plane = plane
        mPolygon2D = Polygon2DHelper.CreateInfinite(Of TRef)()
    End Sub


    ''' <summary>
    ''' Create a polygon, which lies on the given plane
    ''' (to reduce calculations when the plane is known)
    ''' and has the given vertices.
    ''' </summary>
    ''' <param name="vertices">
    ''' The vertices must be defined in hull-order,
    ''' though the direction is not important.
    ''' </param>
    Public Sub New(plane As IPlane3D, vertices As IEnumerable(Of Point3D(Of TRef)))
        Me.Plane = plane
        mPolygon2D = Polygon2DHelper.Create(
            From v In vertices
            Select Point2DHelper.Create(plane.GetProjection(v), v.References))
    End Sub

#End Region


#Region " IPolygon3D implementation "

    Public Function Contains(c As IPoint3D) As Boolean Implements IObject3D.Contains
        Dim p = Plane.GetProjectionIfOnPlane(c)
        If p Is Nothing Then Return False

        Return mPolygon2D.Contains(p)
    End Function

#End Region


#Region " API "

    ''' <summary>
    ''' Check the relative position of this polygon and the given plane.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' If the plane Contains one of the sides, it definitely Touches
    ''' two other sides. So we have to find the right answer
    ''' among all sides.
    ''' </remarks>
    Public Function Intersect(byPlane As IPlane3D) As SegmentIntersectResults
        Dim sideRes As New List(Of SegmentIntersectResults)()

        For Each side In Sides
            sideRes.Add(side.Intersect(byPlane).Action)
        Next

        Return sideRes.Min()
    End Function


    ''' <summary>
    ''' Cut the polygon by the given plane, keep the inside part.
    ''' </summary>
    ''' <returns>False if the polygon was eliminated by the cut</returns>
    Public Function Cut(byPlane As IPlane3D, inside As Integer) As Boolean Implements IPolygon3D.Cut
        Dim res = Plane.Intersect(byPlane, inside)

        Select Case res.State
            Case PlaneIntersectionResults.Eliminated
                Return False

            Case PlaneIntersectionResults.Kept
                Return True

            Case Else
                Return mPolygon2D.CutByLine(Line2DHelper.CreateBorder(res.Line, res.Inside))
        End Select
    End Function

#End Region

End Class
