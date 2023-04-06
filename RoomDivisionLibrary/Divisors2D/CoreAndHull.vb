Imports Common


''' <summary>
''' Represents the triangulated set of points.
''' </summary>
''' <typeparam name="TRef">Reference type</typeparam>
Public Class CoreAndHull(Of TRef)

    ''' <summary>
    ''' A list of trangles.
    ''' </summary>
    Public Property Core As IList(Of Polygon2D(Of TRef))


    ''' <summary>
    ''' A list of outer sides of the set.
    ''' </summary>
    Public Property Hull As IList(Of HullSide(Of TRef))

End Class
