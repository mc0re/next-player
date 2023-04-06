''' <summary>
''' Used primarily for visualization.
''' </summary>
<CLSCompliant(True)>
Public Interface IPolyhedron
    Inherits IObject3D

    ReadOnly Property Sides As IReadOnlyCollection(Of IPolyhedronSide)

    ReadOnly Property Vertices As IReadOnlyCollection(Of IPoint3D)

End Interface
