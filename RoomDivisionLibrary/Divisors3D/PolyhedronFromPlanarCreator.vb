Imports MIConvexHull
Imports Common


''' <summary>
''' Utility class for creating polyhedrons.
''' </summary>
Public Class PolyhedronFromPlanarCreator

#Region " API "

    ''' <summary>
    ''' Check if all points lie on a single plane.
    ''' If so, calculate the projections.
    ''' </summary>
    ''' <returns>False if the points are not on the same plane</returns>
    ''' <remarks>
    ''' The function puts the result into its argument.
    ''' This is done to chain the checks in the caller method.
    ''' </remarks>
    Public Shared Function CalcProjectionPlane(Of TRef)(
        pts As IList(Of Point3D(Of TRef)),
        ByRef planeProjections As IList(Of PointAsVertex2D(Of TRef))
    ) As Boolean

        Debug.Assert(pts.Count >= 2)

        Dim planeAllPoints As IPlane3D

        ' Some first points might be on the same line,
        ' so they don't define a plane yet.
        ' Any 3 non-colinear points would do.
        Dim ln = Line3DHelper.Create(pts(0), pts(1))
        Dim pNotOnLine As IPoint3D = Nothing

        For Each pt In pts.Skip(2)
            If Not ln.GetCoordinate(pt).HasValue Then
                pNotOnLine = pt
                Exit For
            End If
        Next

        If pNotOnLine IsNot Nothing Then
            ' Found 3 non-colinear points, make a plane through them
            planeAllPoints = Plane3DHelper.Create3Points(
                pts(0), pts(1), pNotOnLine, NoRef.Empty)
        Else
            ' All points are colinear, so create a plane,
            ' which goes through all these points,
            ' and is perpendicular to a vector MIDDLE_POINT -> 0.
            ' If the vector is a 0-vector, the plane goes through origin;
            ' pick any direction.
            Dim vectTo0 = Vector3D.CreateA2B(
                Point3DHelper.Average(pts.Cast(Of IPoint3D)().ToArray()), Point3DHelper.Origin)

            Dim normal = If(
                Not vectTo0.IsZero,
                vectTo0,
                Vector3D.CreateA2B(pts(0), pts(1)).AnyPerpendicular)

            planeAllPoints = Plane3DHelper.CreateAlongLine(
                pts(0), Vector3D.CreateA2B(pts(0), pts(1)), normal, NoRef.Empty)
        End If

        Dim res As New List(Of PointAsVertex2D(Of TRef))()

        For Each pt In pts
            Dim proj = planeAllPoints.GetProjectionIfOnPlane(pt)

            If proj Is Nothing Then Return False

            res.Add(Vertex2DHelper.Create(planeAllPoints, Point2DHelper.Create(proj, {pt}), pt))
        Next

        planeProjections = res

        Return True
    End Function


    ''' <summary>
    ''' For the given list of coplanar projected points,
    ''' create a collection of polyhedrons,
    ''' dividing the room.
    ''' </summary>
    ''' <remarks>
    ''' The points must be vertices of a convex polygon.
    ''' They must not be colinear, unless there are only 2 points.
    ''' 
    ''' First, use triangulation to divide the space into polygons.
    ''' 
    ''' If the convex hull does not contain audience,
    ''' create divisions from the audience to the speakers,
    ''' based on projections.
    ''' 
    ''' Then, within the projection of the convex hull to the audience,
    ''' continue room separation into infinity.
    ''' 
    ''' For each pair of points (A, B), the polyhedrons sides are:
    ''' - a plane going through point A
    '''   along a projection line from A to the audience plane and
    '''   perpendicular to the common projection plane;
    ''' - same for point B;
    ''' - if the audience plane is not cut by the speakers plane,
    '''   a plane parallel to the speakers plane going through the
    '''   audience plane.
    '''   
    ''' If the origin is outside the polygon, also take into consideration
    ''' a plane parallel to the projection plane, but going through the
    ''' closest point of the audience rectangle.
    ''' </remarks>
    Public Shared Function Create(Of TRef)(
        points As IList(Of PointAsVertex2D(Of TRef)),
        room As Room3D
    ) As DivisionResult(Of TRef)

        ' Any 2 points are colinear, so if we are here, we've got more than 2
        Debug.Assert(points.Count > 2)

        Dim shell As New List(Of ShellFace(Of TRef))()

        If points.Count = 3 Then
            ' Triangulation cannot handle the trivial case of 3 points,
            ' do the job manually
            Dim pts = points.Select(Function(p) p.OriginalPoint).ToList()
            shell.Add(CreateFromPoints(Of TRef)(pts(0), pts(1), pts(2)))
        Else
            Dim normal = New Vector3D(1, 1, 1) ' points.First().Plane.Normal
            Dim div = Triangulation.CreateDelaunay(points, AbsoluteCoordPrecision)

            For Each f In div.Cells
                Dim pts = f.Vertices.Select(Function(v) v.OriginalPoint).ToList()
                shell.Add(New ShellFace(Of TRef)(pts, normal))
            Next
        End If

        Dim res As New DivisionResult(Of TRef)(room)

        ' Duplicate the faces with opposite normal
        For Each sh In shell
            res.Shell.Add(sh)
            res.Shell.Add(New ShellFace(Of TRef)(sh.Points, sh.Outside.Negate))
        Next

        Return res
    End Function

#End Region


#Region " Utility "

    ''' <summary>
    ''' Create a <see cref="ShellFace"/> from 3 points. Calculate normal vector.
    ''' </summary>
    ''' <remarks>
    ''' The direction of the normal vector is not important, as both it and its opposite
    ''' will be added to the shell.
    ''' </remarks>
    Private Shared Function CreateFromPoints(Of TRef)(
        a As Point3D(Of TRef), b As Point3D(Of TRef), c As Point3D(Of TRef)
    ) As ShellFace(Of TRef)

        Dim normal =
            Vector3D.CreateA2B(a, b).
            CrossProduct(
            Vector3D.CreateA2B(a, c))

        Return New ShellFace(Of TRef)({a, b, c}, normal)
    End Function

#End Region

End Class
