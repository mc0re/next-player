Imports Common


''' <summary>
''' How to divide the room if all given points collide into one.
''' </summary>
Public NotInheritable Class SingleDivider2D

    ''' <summary>
    ''' Check whether the point set can be represented by a single polygon.
    ''' </summary>
    Public Shared Function IsSingle(mPoints As IEnumerable(Of IPoint2D)) As Boolean
        Return mPoints.Count <= 1
    End Function


    ''' <summary>
    ''' If all points collide into one, only the room will be the boundary.
    ''' </summary>
    ''' <param name="room">Room definition</param>
    Public Shared Sub SplitSingle(Of TRef)(
        room As Room2D,
        points As IList(Of Point2D(Of TRef)),
        polyList As IList(Of Polygon2D(Of TRef))
    )

        Dim refList = ReferencesHelper.MergeReferences(points)

        Dim poly = Polygon2DHelper.CreateInfinite(refList)
        If room.CutByRoom(poly) Then
            polyList.Add(poly)
        End If
    End Sub

End Class
