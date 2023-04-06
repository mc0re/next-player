Imports System.Diagnostics.CodeAnalysis


''' <summary>
''' Represents a line.
''' </summary>
<CLSCompliant(True)>
<Serializable>
Public Class Line3D(Of TRef)
    Implements ILine3D, IRefKeeper(Of TRef)

#Region " Fields "

    ' Using the separated fields to speed up access

    Private ReadOnly mAx As Double

    Private ReadOnly mAy As Double

    Private ReadOnly mAz As Double

    Private ReadOnly mDx As Double

    Private ReadOnly mDy As Double

    Private ReadOnly mDz As Double

    Private ReadOnly mCoefX As Double

    Private ReadOnly mCoefY As Double

    Private ReadOnly mCoefZ As Double

#End Region


#Region " ILine3D properties "

    ''' <inheritdoc/>
    Public ReadOnly Property Point As IPoint3D Implements ILine3D.Point


    ''' <inheritdoc/>
    Public ReadOnly Property Vector As Vector3D Implements ILine3D.Vector

#End Region


#Region " IRefKeeper properties "

    Private ReadOnly mReferences As New List(Of TRef)()


    Public ReadOnly Property References As IReadOnlyCollection(Of TRef) Implements IRefKeeper(Of TRef).References
        Get
            Return mReferences
        End Get
    End Property

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Create a line through two points.
    ''' </summary>
    ''' <remarks>
    ''' The distance from point to line is |AC - b * AB|, where b = AC * AB / AB^2.
    ''' The check code is the same as in <see cref="Point3DHelper.Distance"/>.
    ''' </remarks>
    Friend Sub New(a As IPoint3D, b As IPoint3D, ref As IEnumerable(Of TRef))
        mAx = a.X
        mAy = a.Y
        mAz = a.Z

        mDx = b.X - a.X
        mDy = b.Y - a.Y
        mDz = b.Z - a.Z

        Dim denom = mDx * mDx + mDy * mDy + mDz * mDz

        If IsEqual(Math.Sqrt(denom), 0) Then
            Throw New ArgumentException("Cannot build a line from colliding points")
        End If

        Point = a
        Vector = New Vector3D(mDx, mDy, mDz)

        mCoefX = mDx / denom
        mCoefY = mDy / denom
        mCoefZ = mDz / denom

        mReferences.AddRange(ref)
    End Sub


    ''' <summary>
    ''' Create a line defined by a point and a vector.
    ''' </summary>
    Friend Sub New(a As IPoint3D, v As Vector3D, ref As IEnumerable(Of TRef))
        Point = a
        Vector = v

        If v.IsZero Then
            Throw New ArgumentException("Cannot build a line from null vector")
        End If

        mAx = a.X
        mAy = a.Y
        mAz = a.Z

        mDx = v.X
        mDy = v.Y
        mDz = v.Z

        Dim denom = v.Square

        mCoefX = mDx / denom
        mCoefY = mDy / denom
        mCoefZ = mDz / denom

        mReferences.AddRange(ref)
    End Sub

#End Region


#Region " API "

    ''' <inheritdoc/>
    Public Function GetCoordinate(c As IPoint3D) As Double? Implements ILine3D.GetCoordinate
        Dim dx = c.X - mAx
        Dim dy = c.Y - mAy
        Dim dz = c.Z - mAz

        Dim t = dx * mCoefX + dy * mCoefY + dz * mCoefZ

        Dim isOnLine = IsEqual(dx - t * mDx, 0) AndAlso
                       IsEqual(dy - t * mDy, 0) AndAlso
                       IsEqual(dz - t * mDz, 0)

        Return If(isOnLine, t, CType(Nothing, Double?))
    End Function


    ''' <inheritdoc/>
    Public Function GetPoint(u As Double) As IPoint3D Implements ILine3D.GetPoint
        Return Point3DHelper.Create(mAx + mDx * u, mAy + mDy * u, mAz + mDz * u)
    End Function


    ''' <inheritdoc/>
    Public Function CutByPlane(plane As IPlane3D) As LineCutResult Implements ILine3D.CutByPlane
        Dim denom = Vector.DotProduct(plane.Normal)

        ' Line is parallel to the plane, no cut
        If IsEqual(denom, 0) Then
            Return Nothing
        End If

        Return New LineCutResult With {
            .Coordinate = (-plane.Offset - plane.Normal.DotProduct(Point)) / denom,
            .VectorSign = Sign(denom)
        }
    End Function


    ''' <inheritdoc/>
    Public Function CutBySphere(sphere As Sphere3D) As LineCutResult Implements ILine3D.CutBySphere
        Dim oc = Vector3D.CreateA2B(sphere.Center, Point)
        Dim loc = Vector.Unit.DotProduct(oc)
        Dim det = loc * loc - oc.Square + sphere.Radius * sphere.Radius

        ' Intersection in 0 or 1 points
        If Sign(det) <= 0 Then
            Return Nothing
        End If

        Dim root = Math.Sqrt(det)
        Dim t = -loc - root

        If t < 0 Then
            t = -loc + root
        End If

        If t <= 0 Then
            Return Nothing
        End If

        Return New LineCutResult With {
            .Coordinate = t / Vector.Length,
            .VectorSign = 1
        }
    End Function

#End Region


#Region " ToString "

    ''' <summary>
    ''' For debugging.
    ''' </summary>
    <ExcludeFromCodeCoverage>
    Public Overrides Function ToString() As String
        Return String.Format("({0:F2}, {1:F2}) {2} / {3}",
                             Point.X, Point.Y, Vector, ReferencesHelper.AsString(References))
    End Function

#End Region

End Class
