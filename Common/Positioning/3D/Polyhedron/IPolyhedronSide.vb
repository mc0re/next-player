<CLSCompliant(True)>
Public Interface IPolyhedronSide

    ''' <summary>
    ''' Polygonal representation of the side.
    ''' </summary>
    ReadOnly Property Polygon As IPolygon3D


    ''' <summary>
    ''' Which way is inside, according to the plane.
    ''' </summary>
    ReadOnly Property Inside As Integer

End Interface
