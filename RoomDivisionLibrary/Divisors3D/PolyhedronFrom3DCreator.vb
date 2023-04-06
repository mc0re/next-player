Imports Common
Imports MIConvexHull


''' <summary>
''' Utility class for creating polyhedrons.
''' </summary>
Public Class PolyhedronFrom3DCreator

#Region " API "

    ''' <summary>
    ''' Create room division for a non-planar colocation of points.
    ''' </summary>
    Public Shared Function Create(Of TRef)(
        points As IList(Of Point3D(Of TRef)),
        room As Room3D
    ) As DivisionResult(Of TRef)

        ' Any 3 points are coplanar, so if we are here, we've got more than 3
        Debug.Assert(points.Count > 3)

        Dim vertices = (
                From ch In points
                Select PointAsVertex3DHelper.Create(ch, ch) Distinct
            ).ToList()
        Dim res As New DivisionResult(Of TRef)(room)

        If vertices.Count = 4 Then
            ' Tetrahedration does not work for a trivial case of 4 points,
            ' do the job manually
            res.Core.Add(CreateFromPoints(
                vertices(0), vertices(1), vertices(2), vertices(3), room))
        Else
            ' Divide the room "inside speakers".
            ' Use AbsoluteCoordPrecision when becomes possible.
            Dim div = Triangulation.CreateDelaunay(vertices, AbsoluteCoordPrecision)
            For Each cell In div.Cells
                Dim v = cell.Vertices
                res.Core.Add(CreateFromPoints(v(0), v(1), v(2), v(3), room))
            Next
        End If

        ' Now find faces on the outer shell
        Dim hull = ConvexHull.Create(vertices, AbsoluteCoordPrecision)
        For Each f In hull.Result.Faces
            Dim sh As New ShellFace(Of TRef)(
                (From v In f.Vertices Select v.Ref).ToList(),
                New Vector3D(f.Normal(0), f.Normal(1), f.Normal(2)))
            res.Shell.Add(sh)
        Next

        Return res
    End Function

#End Region


#Region " Utility "

    ''' <summary>
    ''' Create trivial division from 4 points.
    ''' </summary>
    Private Shared Function CreateFromPoints(Of TRef)(
        a As PointAsVertex3D(Of Point3D(Of TRef)),
        b As PointAsVertex3D(Of Point3D(Of TRef)),
        c As PointAsVertex3D(Of Point3D(Of TRef)),
        d As PointAsVertex3D(Of Point3D(Of TRef)),
        room As Room3D
    ) As Polyhedron(Of TRef)

        Dim pl = {
            CreateSide(a, b, c, d),
            CreateSide(a, b, d, c),
            CreateSide(a, c, d, b),
            CreateSide(b, c, d, a)
        }

        Return PolyhedronHelper.CreateFromBorders(pl)
    End Function


    ''' <summary>
    ''' Create a side of a polyhedron, which goes through the given 3 points,
    ''' and <paramref name="internal"/> is towards inside the polyhedron been built.
    ''' </summary>
    Private Shared Function CreateSide(Of TRef)(
        a As PointAsVertex3D(Of Point3D(Of TRef)),
        b As PointAsVertex3D(Of Point3D(Of TRef)),
        c As PointAsVertex3D(Of Point3D(Of TRef)),
        internal As IPoint3D
    ) As Border3D(Of TRef)

        Dim refList = {a, b, c}.SelectMany(Function(p) p.Ref.References)
        Dim pl = Plane3DHelper.Create3Points(a, b, c, refList)

        Return New Border3D(Of TRef)(
            pl, Sign(pl.GetDistanceToPoint(internal)), pl.References)
    End Function

#End Region

End Class
