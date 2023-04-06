Imports System.Diagnostics.CodeAnalysis


''' <summary>
''' Represents a plane in 3D space.
''' </summary>
''' <remarks>
''' Not every point used to create the plane is a reference to keep,
''' therefore the references are a separated list.
''' </remarks>
<CLSCompliant(True)>
<Serializable>
Public Class Plane3D(Of TRef)
    Implements IPlane3D, IRefKeeper(Of TRef)

#Region " IPlane3D properties "

    ''' <inheritdoc/>
    Public ReadOnly Property Point As IPoint3D Implements IPlane3D.Point


    ''' <inheritdoc/>
    Public ReadOnly Property Normal As Vector3D Implements IPlane3D.Normal


    ''' <inheritdoc/>
    Public ReadOnly Property Offset As Double Implements IPlane3D.Offset

#End Region


#Region " IRefKeeper property "

    Public ReadOnly Property References As IReadOnlyCollection(Of TRef) Implements IRefKeeper(Of TRef).References

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Create a plane going through point <paramref name="a"/> and having
    ''' a normal vector <paramref name="normal"/>.
    ''' In the right-hand coordinate system, the positive distance
    ''' is along the normal.
    ''' </summary>
    ''' <param name="a">A point on a plane</param>
    ''' <param name="normal">Normal vector</param>
    Friend Sub New(a As IPoint3D, normal As Vector3D, refList As IEnumerable(Of TRef))
        If normal.IsZero Then
            Throw New ArgumentException("Cannot build a plane from zero normal")
        End If

        Point = a
        Me.Normal = normal
        Offset = -normal.DotProduct(a)
        References = refList.ToList()
    End Sub


    ''' <summary>
    ''' Create a new plane by shifting this plane
    ''' by <paramref name="byOffset"/> along its normal.
    ''' References are not passed along.
    ''' </summary>
    Public Function Shift(byOffset As Double) As IPlane3D Implements IPlane3D.Shift
        Return New Plane3D(Of TRef)(
            Normal.Unit.Multiply(byOffset).From(Point),
            Normal,
            Enumerable.Empty(Of TRef))
    End Function

#End Region


#Region " IPlane3D implementation "

    ''' <inheritdoc/>
    Public Function Intersect(byPlane As IPlane3D, inside As Integer) As PlaneIntersectionResult Implements IPlane3D.Intersect
        ' Vector along the intersection line
        Dim v = Normal.CrossProduct(byPlane.Normal)
        Dim vSquared = v.Square

        If IsEqual(vSquared, 0) Then
            ' The planes are parallel, check their relative position
            If Sign(byPlane.GetDistanceToPoint(Point)) = -inside Then
                Return PlaneIntersectionResult.Eliminated
            Else
                Return PlaneIntersectionResult.Kept
            End If
        End If

        ' Find a point on the intersection line.
        ' See http://geomalgorithms.com/a05-_intersect-1.html solution (C)
        Dim t1 = New Vector3D(
            byPlane.Offset * Normal.X - Offset * byPlane.Normal.X,
            byPlane.Offset * Normal.Y - Offset * byPlane.Normal.Y,
            byPlane.Offset * Normal.Z - Offset * byPlane.Normal.Z)
        Dim t2 = t1.CrossProduct(v)
        Dim intersectPoint = Point3DHelper.Create(t2.X / vSquared, t2.Y / vSquared, t2.Z / vSquared)

        ' Intersection line as projection on this plane
        Dim a2D = GetProjection(intersectPoint)
        Dim b2D = GetProjection(v.From(intersectPoint))
        Dim line = Line2DHelper.Create(a2D, b2D)

        ' Inside direction is determined by byPlane
        Dim insidePoint = byPlane.Normal.Multiply(inside).From(intersectPoint)
        Dim c = GetProjection(insidePoint)
        Dim inside2D = Sign(line.GetDistanceToLine(c))

        Return New PlaneIntersectionResult(
            line, If(inside2D > 0, Line2DDirections.Right, Line2DDirections.Left))
    End Function


    ''' <inheritdoc/>
    Public Function GetDistanceToPoint(c As IPoint3D) As Double Implements IPlane3D.GetDistanceToPoint
        Return c.X * Normal.X + c.Y * Normal.Y + c.Z * Normal.Z + Offset
    End Function


    ''' <inheritdoc/>
    Public Function IsPointOnPlane(c As IPoint3D) As Boolean Implements IPlane3D.IsPointOnPlane
        Return IsEqual(GetDistanceToPoint(c), 0)
    End Function


    ''' <inheritdoc/>
    Public Function GetProjection(c As IPoint3D) As IPoint2D Implements IPlane3D.GetProjection
        Dim cv = Vector3D.CreateA2B(Point, c)
        Dim u = Normal.AnyPerpendicular.DotProduct(cv)
        Dim v = Normal.OtherPerpendicular.DotProduct(cv)

        Return Point2DHelper.Create(u, v)
    End Function


    ''' <inheritdoc/>
    Public Function GetProjectionIfOnPlane(c As IPoint3D) As IPoint2D Implements IPlane3D.GetProjectionIfOnPlane
        If Not IsPointOnPlane(c) Then Return Nothing

        Return GetProjection(c)
    End Function



    ''' <inheritdoc/>
    Public Function GetPoint(x As Double, y As Double) As IPoint3D Implements IPlane3D.GetPoint
        Dim u = Normal.AnyPerpendicular.Multiply(x).From(Point)
        Dim v = Normal.OtherPerpendicular.Multiply(y).From(u)

        Return v
    End Function


    ''' <inheritdoc/>
    Public Function GetPoint(p As IPoint2D) As IPoint3D Implements IPlane3D.GetPoint
        Return GetPoint(p.X, p.Y)
    End Function

#End Region


#Region " ToString "

    ''' <summary>
    ''' For debugging.
    ''' </summary>
    <ExcludeFromCodeCoverage>
    Public Overrides Function ToString() As String
        Return String.Format("Point {0}, normal {1}", Point, Normal)
    End Function

#End Region

End Class
