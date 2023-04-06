Imports Common


''' <summary>
''' Layouter for 3D with visualizations.
''' </summary>
''' <typeparam name="TRef"></typeparam>
<CLSCompliant(True)>
Public Interface I3DLayouter(Of TRef)
    Inherits ILayouter(Of IPoint3D, Point3D(Of TRef), Room3D, TRef)

#Region " API "

    ''' <summary>
    ''' For visualization.
    ''' </summary>
    ''' <param name="pt"></param>
    ''' <returns></returns>
    Function GetContainingPolyhedron(pt As IPoint3D) As Polyhedron(Of TRef)


    ''' <summary>
    ''' For visualization.
    ''' </summary>
    ''' <returns></returns>
    Function GetPolyhedrons() As IEnumerable(Of Polyhedron(Of TRef))

#End Region

End Interface
