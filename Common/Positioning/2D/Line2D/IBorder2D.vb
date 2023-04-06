''' <summary>
''' Represents a 2D border, i.e. a line and a direction "inside the border".
''' </summary>
<CLSCompliant(True)>
Public Interface IBorder2D

    ''' <summary>
    ''' Line defining the border.
    ''' </summary>
    ReadOnly Property Line As ILine2D


    ''' <summary>
    ''' Which direction is "inside the border".
    ''' </summary>
    ReadOnly Property Inside As Line2DDirections

End Interface
