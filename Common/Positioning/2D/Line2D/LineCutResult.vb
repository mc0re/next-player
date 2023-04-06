''' <summary>
''' The result of cutting a line.
''' Returned by a number of functions for 2D and 3D space.
''' If the line was not cut, Nothing is returned.
''' </summary>
<CLSCompliant(True)>
Public Class LineCutResult

    ''' <summary>
    ''' The coordinate of the crossing point in line's coordinate system.
    ''' </summary>
    Public Property Coordinate As Double


    ''' <summary>
    ''' When cut by a line in 2D space,
    ''' whether the "right" direction of the cutting line's vector
    ''' goes along this line's vector (positive - it does).
    ''' 
    ''' When cut by a surface in 3D space,
    ''' whether the normal of this surface
    ''' goes along this line's vector (positive - it does).
    ''' </summary>
    Public Property VectorSign As Integer

End Class
