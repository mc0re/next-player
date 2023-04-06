''' <summary>
''' Used primarily for visualization.
''' </summary>
<CLSCompliant(True)>
Public Interface IPolygon3D
    Inherits IObject3D

    ReadOnly Property Plane As IPlane3D

    ReadOnly Property Vertices As IReadOnlyCollection(Of IPoint3D)

    ReadOnly Property Sides As IReadOnlyCollection(Of ILineSegment3D)


    ''' <summary>
    ''' Cut a polygon by the given plane.
    ''' </summary>
    ''' <returns>False, if it was eliminated</returns>
    Function Cut(byPlane As IPlane3D, inside As Integer) As Boolean

End Interface
