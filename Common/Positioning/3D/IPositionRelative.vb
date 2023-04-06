''' <summary>
''' Defines 3D coordinates of a point, relative to the room.
''' </summary>
<CLSCompliant(True)>
Public Interface IPositionRelative

    ''' <summary>
    ''' -1 (full left) .. 1 (full right)
    ''' </summary>
    Property X As Single


    ''' <summary>
    ''' -1 (full back) .. 1 (full front)
    ''' </summary>
    Property Y As Single


    ''' <summary>
    ''' -1 (full bottom) .. 1 (full top)
    ''' </summary>
    Property Z As Single

End Interface
