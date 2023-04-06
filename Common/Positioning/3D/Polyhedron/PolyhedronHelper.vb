Public Class PolyhedronHelper

#Region " API "

    ''' <summary>
    ''' Create a polyhedron that has the given sides.
    ''' </summary>
    Public Shared Function CreateFromBorders(Of TRef)(
        planesInfo As IReadOnlyCollection(Of Border3D(Of TRef))
    ) As Polyhedron(Of TRef)

        Dim refs = ReferencesHelper.MergeReferences(planesInfo)
        Dim sides = CalculateSides(planesInfo)

        Return New Polyhedron(Of TRef)(sides, refs)
    End Function


    ''' <summary>
    ''' Create a polyhedron that has the given sides.
    ''' </summary>
    Public Shared Function CreateFromBorders(Of TRef)(
        planesInfo As IReadOnlyCollection(Of Border3D(Of TRef)),
        refList As IEnumerable(Of TRef)
    ) As Polyhedron(Of TRef)

        Dim refs = refList.Concat(ReferencesHelper.MergeReferences(planesInfo))
        Dim sides = CalculateSides(planesInfo)

        Return New Polyhedron(Of TRef)(sides, refs)
    End Function

#End Region


#Region " Utility "

    ''' <summary>
    ''' Calculate the polygons for each side of the polyhedron.
    ''' </summary>
    ''' <remarks>
    ''' A polyhedron is given by a number of sides (plane + direction).
    ''' Each side is limited by all other sides.
    ''' Calculate the intersection points.
    ''' </remarks>
    Private Shared Function CalculateSides(Of TRef)(
        planesInfo As IReadOnlyCollection(Of Border3D(Of TRef))
    ) As List(Of PolyhedronSide(Of TRef))

        Dim res As New List(Of PolyhedronSide(Of TRef))()

        ' Cut each of the polyhedron sides by other sides,
        ' unless one of them is essentially the same plane.
        For Each side In planesInfo
            ' Create initial polygon large enough to contain whole room
            Dim poly As New Polygon3D(Of TRef)(side.Plane)

            ' Cut it by the room and by other sides, unless they are duplicated
            Dim sameSidesAsLoopVar = planesInfo.
                Where(Function(r) Plane3DHelper.IsSame(r.Plane, side.Plane)).
                ToList()
            Dim allSidesButLoopVar = planesInfo.Except(sameSidesAsLoopVar)
            Dim eliminated = CutPolygon(poly, allSidesButLoopVar)

            If Not eliminated Then
                res.Add(PolyhedronSideHelper.Create(poly, side.Inside))
            End If
        Next

        '' Find out, which room sides are relevant
        '' (which contain at least one edge of the resulting polyhedron).
        'Dim touchingSides = (
        '    From roomSide In roomSides
        '    Where Not (From side In res Where Plane3DHelper.IsSame(side.Plane, roomSide.Plane)).Any()
        '    Where (From side In res Where side.Intersect(roomSide.Plane) = SegmentIntersectResults.Contains).Any()
        '    Select roomSide
        ').ToList()

        'For Each side In touchingSides
        '    ' Create initial polygon large enough to contain whole room
        '    Dim poly As New Polygon3D(Of TRef)(side.Plane)

        '    ' Cut by other room sides
        '    Dim eliminated = CutPolygon(poly, roomSides.Union(mSides).Except({side}))

        '    If Not eliminated Then
        '        res.Add(poly)
        '    End If
        'Next

        Return res
    End Function


    Private Shared Function CutPolygon(Of TRef)(
        poly As Polygon3D(Of TRef),
        cutSides As IEnumerable(Of Border3D(Of TRef))
    ) As Boolean

        Dim eliminated = False

        ' Cut by room and other sides
        For Each cutPlane In cutSides
            If Not poly.Cut(cutPlane.Plane, cutPlane.Inside) Then
                eliminated = True
                Exit For
            End If
        Next

        Return eliminated
    End Function

#End Region

End Class