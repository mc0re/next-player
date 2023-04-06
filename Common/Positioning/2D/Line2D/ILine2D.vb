<CLSCompliant(True)>
Public Interface ILine2D
    Inherits IObject2D

#Region " Properties "

    ''' <summary>
    ''' Line equation parameters.
    ''' </summary>
    ''' <remarks>
    ''' The line is represented as an equation:
    '''   <see cref="FactorX"/> * x + <see cref="FactorY"/> * Y + <see cref="Offset"/> = 0
    ''' </remarks>
    ReadOnly Property FactorX As Double


    ''' <summary>
    ''' Line equation parameters.
    ''' </summary>
    ''' <remarks>
    ''' The line is represented as an equation:
    '''   <see cref="FactorX"/> * x + <see cref="FactorY"/> * Y + <see cref="Offset"/> = 0
    ''' </remarks>
    ReadOnly Property FactorY As Double


    ''' <summary>
    ''' Line equation parameters.
    ''' </summary>
    ''' <remarks>
    ''' The line is represented as an equation:
    '''   <see cref="FactorX"/> * x + <see cref="FactorY"/> * Y + <see cref="Offset"/> = 0
    ''' </remarks>
    ReadOnly Property Offset As Double


    ''' <summary>
    ''' Unit vector for the line.
    ''' </summary>
    ReadOnly Property Vector As Vector2D

#End Region


#Region " API "

    ''' <summary>
    ''' Get absolute distance from <paramref name="c"/> to the line.
    ''' </summary>
    ''' <remarks>
    ''' The distance from point to line is |AC - b * AB|, where b = AC * AB / AB^2.
    ''' The distance is in the coordinate units,
    ''' and can be positive (the point is to the right of the vector)
    ''' or negative (the point is to the left of the vector),
    ''' as well as zero.
    ''' </remarks>
    Function GetDistanceToLine(c As IPoint2D) As Double


    ''' <summary>
    ''' Get the coordinate of the given point in line's coordinate system, if it is on the line.
    ''' </summary>
    ''' <returns>
    ''' Distance of point A is 0, point B - 1.
    ''' If the point is not on the line, the result is undefined.
    ''' </returns>
    Function GetCoordinate(c As IPoint2D) As Double


    ''' <summary>
    ''' Determine a point from the coordinate in line's coordinate system.
    ''' </summary>
    ''' <param name="u">Coordinate</param>
    Function GetPoint(u As Double) As IPoint2D


    ''' <summary>
    ''' Find a coordinate of the cut point.
    ''' </summary>
    ''' <param name="cutLine"></param>
    ''' <returns>Nothing if cannot be cut (cutting line is parallel)</returns>
    ''' <remarks>The line is not modified.</remarks>
    Function CutByLine(cutLine As ILine2D) As LineCutResult


    ''' <summary>
    ''' Create a new line perpendicular to this line in the given point,
    ''' that is 90 degrees to the right.
    ''' </summary>
    ''' <param name="pt">Pivot point</param>
    Function Perpendicular(pt As IPoint2D) As ILine2D


    ''' <summary>
    ''' Create a colocated line, which vector points into the opposite direction.
    ''' </summary>
    Function Revert() As ILine2D

#End Region

End Interface
