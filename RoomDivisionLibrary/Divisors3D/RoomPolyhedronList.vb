Imports Common


Public Class RoomPolyhedronList(Of TRef)
    Inherits List(Of Polyhedron(Of TRef))

#Region " Fields "

    ''' <summary>
    ''' A list of all speakers in the room.
    ''' </summary>
    Private mVertices As New HashSet(Of Point3D(Of TRef))(Point3DHelper.EqualityComparer)


    ''' <summary>
    ''' Reference to the room.
    ''' </summary>
    Public ReadOnly Property Vertices As IReadOnlyCollection(Of Point3D(Of TRef))
        Get
            Return mVertices
        End Get
    End Property

#End Region


#Region " API "

    ''' <summary>
    ''' Call this when all sides are collected.
    ''' </summary>
    ''' <param name="refs">Initial list of references</param>
    Public Sub CollectAllVertices(refs As IEnumerable(Of Point3D(Of TRef)))
        mVertices.Clear()

        For each ref In refs
            mVertices.Add(ref)
        Next

        For Each polyhedron In Me
            For Each polygon In polyhedron.Sides
                For Each v In polygon.Polygon.Vertices.OfType(Of Point3D(Of TRef))()
                    mVertices.Add(v)
                Next
            Next
        Next
    End Sub


    ''' <summary>
    ''' Find speakers to use for the given point.
    ''' </summary>
    ''' <returns>A list of speaker projections, can be empty</returns>
    Public Function FindRefForPoint(c As IPoint3D) As IReadOnlyCollection(Of TRef)
        ' Check exact match for a vertex.
        Dim exactMatch = (From v In mVertices Where Point3DHelper.IsSame(v, c)).ToList()

        If exactMatch.Any() Then
            Return exactMatch.SelectMany(Function(m) m.References).ToList()
        End If

        ' Find a matching polyhedron
        Dim polyMatch = (
            From p In Me Where p.Contains(c)
            ).FirstOrDefault()

        If polyMatch IsNot Nothing Then
            Return polyMatch.References
        End If

        Return New List(Of TRef)()
    End Function

#End Region

End Class
