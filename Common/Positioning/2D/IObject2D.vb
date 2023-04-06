<CLSCompliant(True)>
Public Interface IObject2D

    ''' <summary>
    ''' Whether the given point is contained within the object,
    ''' not on object's boundaries.
    ''' </summary>
    Function Contains(p As IPoint2D) As Boolean

End Interface
