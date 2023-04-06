''' <summary>
''' Defines 2D coordinates of a point in absolute units (e.g. meters).
''' </summary>
<CLSCompliant(True)>
Public Interface IPoint2D
    Inherits IObject2D

#Region " Properties "

    ReadOnly Property X As Double

    ReadOnly Property Y As Double

#End Region

End Interface
