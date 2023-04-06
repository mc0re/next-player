Public NotInheritable Class Line3DHelper

#Region " Init and clean-up "

	Private Sub New()
		' Do nothing
	End Sub

#End Region


#Region " Factory methods "

    Public Shared Function Create(a As IPoint3D, b As IPoint3D) As ILine3D
        Return New Line3D(Of NoRef)(a, b, NoRef.Empty)
    End Function


    Public Shared Function Create(a As IPoint3D, v As Vector3D) As ILine3D
        Return New Line3D(Of NoRef)(a, v, NoRef.Empty)
    End Function


    Public Shared Function Create(Of TRef)(
        a As IPoint3D, b As IPoint3D, ref As IEnumerable(Of TRef)
    ) As Line3D(Of TRef)

        Return New Line3D(Of TRef)(a, b, ref)
    End Function


    Public Shared Function Create(Of TRef)(
        a As IPoint3D, v As Vector3D, ref As IEnumerable(Of TRef)
    ) As Line3D(Of TRef)

        Return New Line3D(Of TRef)(a, v, ref)
    End Function

#End Region


#Region " API "

    ''' <summary>
    ''' Get the point of intersection of two lines, if they intersect,
    ''' or the two points defining the shortest line between these lines.
    ''' </summary>
    Public Shared Function ClosestPoints(line1 As ILine3D, line2 As ILine3D) As IReadOnlyList(Of IPoint3D)
        Dim v1 = line1.Vector
        Dim v2 = line2.Vector
        Dim line1Point = line1.Point
        Dim line2Point = line2.Point

        If Vector3D.IsSame(v1.Unit, v2.Unit) Then
            ' Lines are parallel, pick a random point between them,
            ' but not right between the pivot points to have a triangle
            Return {Point3DHelper.Average(v1.From(line1Point), line2Point)}
        End If

        ' The direction of the shortest vector is perpendicular both v1 and v2
        Dim shortest = v1.CrossProduct(v2)
        Dim diff = Vector3D.CreateA2B(line1Point, line2Point)

        ' Solve the system of linear equations
        Dim det = Math3D.Determinant(v1.X, -v2.X, shortest.X, v1.Y, -v2.Y, shortest.Y, v1.Z, -v2.Z, shortest.Z)
        Debug.Assert(Sign(det) <> 0, "Degenerated case")

        Dim det1 = Math3D.Determinant(diff.X, -v2.X, shortest.X, diff.Y, -v2.Y, shortest.Y, diff.Z, -v2.Z, shortest.Z)
        Dim det2 = Math3D.Determinant(v1.X, diff.X, shortest.X, v1.Y, diff.Y, shortest.Y, v1.Z, diff.Z, shortest.Z)

        ' Point on line 1 closest to line 2
        Dim p1 = v1.Multiply(det1 / det).From(line1Point)

        ' Point on line 2 closest to line 1
        Dim p2 = v2.Multiply(det2 / det).From(line2Point)

        Return If(Point3DHelper.IsSame(p1, p2), {p1}, {p1, p2})
    End Function

#End Region

End Class
