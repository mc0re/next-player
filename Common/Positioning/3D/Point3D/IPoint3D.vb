''' <summary>
''' Defines 3D coordinates of a point in absolute units (e.g. meters).
''' </summary>
''' <remarks>
''' Use this interface as parameter type, when you don't care, what does this
''' point represent, and only care about its coordinates.
''' Use one of the implementing types, if you need to care about the point's meaning.
''' </remarks>
<CLSCompliant(True)>
Public Interface IPoint3D

	ReadOnly Property X As Double

	ReadOnly Property Y As Double

	ReadOnly Property Z As Double

End Interface
