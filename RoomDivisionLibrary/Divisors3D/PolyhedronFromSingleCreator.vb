Imports Common


''' <summary>
''' Utility class for creating polyhedrons.
''' </summary>
Public Class PolyhedronFromSingleCreator

    ''' <summary>
    ''' Check whether the provided points are collide to one.
    ''' </summary>
    Public Shared Function AreOnePoint(Of TRef)(
        spkList As ICollection(Of Point3D(Of TRef))
    ) As Boolean

        Dim singlePoint = spkList.Distinct(Point3DHelper.EqualityComparer).ToList()

        Return singlePoint.Count <= 1
    End Function


    ''' <summary>
    ''' Create a "fill everything" polyhedron with all provided reference points.
    ''' </summary>
    ''' <remarks>Shell is empty in this case</remarks>
    Public Shared Function Create(Of TRef)(
        spkList As IEnumerable(Of Point3D(Of TRef)),
        room As Room3D
    ) As DivisionResult(Of TRef)

        Dim res As New DivisionResult(Of TRef)(room)

        res.Core.Add(PolyhedronHelper.CreateFromBorders(
            room.AllSideBorders(Of TRef)(),
            ReferencesHelper.MergeReferences(spkList)))

        Return res
    End Function

End Class
