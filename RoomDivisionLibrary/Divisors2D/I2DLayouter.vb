Imports Common


''' <summary>
''' Layouter for 3D with visualizations.
''' </summary>
''' <typeparam name="TRef"></typeparam>
<CLSCompliant(True)>
Public Interface I2DLayouter(Of TRef)
    Inherits ILayouter(Of IPoint2D, Point2D(Of TRef), Room2D, TRef)

#Region " API "

    ''' <summary>
    ''' For visualization.
    ''' </summary>
    ''' <param name="pt"></param>
    Function GetContainingPolygon(pt As IPoint2D) As Polygon2D(Of TRef)


    ''' <summary>
    ''' For visualization.
    ''' </summary>
    Function GetPolygons() As IEnumerable(Of Polygon2D(Of TRef))

#End Region

End Interface
