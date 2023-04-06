<CLSCompliant(True)>
Public Interface ILineSegment2D
    Inherits IObject2D

    ''' <summary>
    ''' The line both points are on.
    ''' </summary>
    ReadOnly Property Line As ILine2D


    ''' <summary>
    ''' Coordinate of end point A in line coordinates or negative infinity.
    ''' </summary>
    ReadOnly Property CoordinateA As Double


    ''' <summary>
    ''' Coordinate of end point B in line coordinates or positive infinity.
    ''' </summary>
    ReadOnly Property CoordinateB As Double

End Interface
