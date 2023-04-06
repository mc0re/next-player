
''' <summary>
''' Represents a flat polygon.
''' </summary>
<CLSCompliant(True)>
Public Interface IPolygon2D
    Inherits IObject2D

    ''' <summary>
    ''' A list of polygon's vertices in a consequent order.
    ''' </summary>
    ReadOnly Property Vertices As IReadOnlyCollection(Of IPoint2D)


    ''' <summary>
    ''' Cut this polygon by a new border.
    ''' </summary>
    Function CutByLine(border As IBorder2D) As Boolean


    ''' <summary>
    ''' Cut this polygon by a new border.
    ''' </summary>
    Function CutByLine(line As ILine2D, inside As Line2DDirections) As Boolean

End Interface
