Imports Common


''' <summary>
''' Result of room division.
''' </summary>
''' <typeparam name="TRef">Payload type</typeparam>
Public Class DivisionResult(Of TRef)

#Region " Properties "

    ''' <summary>
    ''' Polyhedrons in the area surrounded by the given points.
    ''' </summary>
    Public ReadOnly Core As RoomPolyhedronList(Of TRef)


    ''' <summary>
    ''' Triangles representing the outer shell of the structure.
    ''' </summary>
    Public ReadOnly Shell As IList(Of ShellFace(Of TRef)) =
        New List(Of ShellFace(Of TRef))()

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Initialises read-only fields.
    ''' </summary>
    ''' <param name="room">Needed for polyhedron list creation</param>
    Public Sub New(room As Room3D)
        Core = New RoomPolyhedronList(Of TRef)()
    End Sub

#End Region

End Class
