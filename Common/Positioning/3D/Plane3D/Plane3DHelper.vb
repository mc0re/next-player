

Public NotInheritable Class Plane3DHelper

#Region " Init and clean-up "

    Private Sub New()
        ' Do nothing
    End Sub

#End Region


#Region " Factory methods "

    ''' <summary>
    ''' Create a plane going through 3 points.
    ''' In the right-hand coordinate system, and the points
    ''' given in clockwise direction, the positive sign of the "distance"
    ''' goes "out".
    ''' </summary>
    Public Shared Function Create3Points(Of TRef)(
        a As IPoint3D, b As IPoint3D, c As IPoint3D, refList As IEnumerable(Of TRef)
    ) As Plane3D(Of TRef)

        Dim ba = Vector3D.CreateA2B(a, b)
        Dim ac = Vector3D.CreateA2B(c, a)
        Dim cb = Vector3D.CreateA2B(b, c)

        If ba.IsZero Then
            Throw New ArgumentException("Cannot build a plane from colliding points A and B")
        End If

        If ac.IsZero Then
            Throw New ArgumentException("Cannot build a plane from colliding points A and C")
        End If

        If cb.IsZero Then
            Throw New ArgumentException("Cannot build a plane from colliding points B and C")
        End If

        ' Throws if error
        Return New Plane3D(Of TRef)(a, ba.CrossProduct(ac), refList)
    End Function


    ''' <summary>
    ''' Create a plane going through point <paramref name="a"/> and having
    ''' a normal vector <paramref name="normal"/>.
    ''' In the right-hand coordinate system, the positive distance
    ''' is along the normal.
    ''' </summary>
    ''' <param name="a">A point on a plane</param>
    ''' <param name="normal">Normal vector</param>
    Public Shared Function CreatePointNormal(Of TRef)(
        a As IPoint3D, normal As Vector3D, refList As IEnumerable(Of TRef)
    ) As Plane3D(Of TRef)

        Return New Plane3D(Of TRef)(a, normal, refList)
    End Function


    ''' <summary>
    ''' Create a plane going through line <paramref name="along"/>
    ''' and having a normal vector in the same half-plane
    ''' as <paramref name="normalDir"/>.
    ''' In the right-hand coordinate system, the positive distance
    ''' is along the normal.
    ''' </summary>
    ''' <param name="pivot">A point on a plane</param>
    ''' <param name="along">A vector, which the plane shall go along</param>
    ''' <param name="normalDir">In which direction the normal vector goes</param>
    Public Shared Function CreateAlongLine(Of TRef)(
        pivot As IPoint3D, along As Vector3D, normalDir As Vector3D, refList As IEnumerable(Of TRef)
    ) As Plane3D(Of TRef)

        If normalDir.IsZero Then
            Throw New ArgumentException("Cannot build a plane from zero normal-direction")
        End If

        ' Find the normal as a rejection of normalDir onto line
        Dim projSize = normalDir.DotProduct(along) / along.DotProduct(along)
        Dim normal = normalDir.Minus(along.Multiply(projSize))

        If normal.IsZero Then
            ' Special case, normalDir is parallel to the line,
            ' so keep the "along" and choose any fitting normal.
            normal = along.AnyPerpendicular
        End If

        ' Now we know a point and a normal, build a plane
        Return New Plane3D(Of TRef)(pivot, normal, refList)
    End Function


    ''' <summary>
    ''' Create a border with the same references as the given plane.
    ''' </summary>
    Public Shared Function CreateBorder(Of TRef)(
        plane As Plane3D(Of TRef), inside As Integer
    ) As Border3D(Of TRef)

        Return CreateBorder(plane, inside, plane.References)
    End Function


    ''' <summary>
    ''' Create a border with the given references. Not a usual case.
    ''' </summary>
    Public Shared Function CreateBorder(Of TRef)(
        plane As IPlane3D, inside As Integer, refList As IEnumerable(Of TRef)
    ) As Border3D(Of TRef)

        Return New Border3D(Of TRef)(plane, inside, refList)
    End Function


    ''' <summary>
    ''' Create a border with an empty references list.
    ''' </summary>
    Public Shared Function CreateBorderNoRef(Of TRef)(plane As IPlane3D, inside As Integer) As Border3D(Of TRef)

        Return New Border3D(Of TRef)(plane, inside, Enumerable.Empty(Of TRef))
    End Function

#End Region


#Region " API "

    ''' <summary>
    ''' Check whether the two planes are essentially the same.
    ''' </summary>
    ''' <remarks>
    ''' Planes are the same if they are parallel (i.e. their Normals are parallel),
    ''' and the distance between them is 0.
    ''' </remarks>
    Public Shared Function IsSame(a As IPlane3D, b As IPlane3D) As Boolean
        Return Vector3D.IsSame(a.Normal.Unit, b.Normal.Unit) AndAlso
               a.IsPointOnPlane(b.Point)
    End Function

#End Region

End Class
