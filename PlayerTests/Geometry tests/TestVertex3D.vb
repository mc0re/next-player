Imports MIConvexHull


''' <summary>
''' Trivial <see cref="IVertex"/> implementation for unit tests.
''' </summary>
Public Class TestVertex3D
	Implements IVertex

#Region " Name read-only property "

	Public ReadOnly Property Name As String

#End Region


#Region " IVertex.Position read-only property "

	Public ReadOnly Property Position As Double() Implements IVertex.Position

#End Region


#Region " Init anc clean-up "

	Public Sub New(pointName As String, x As Double, y As Double, z As Double)
		Position = {x, y, z}
		Name = pointName
	End Sub

#End Region


#Region " ToString "

	''' <summary>
	''' For debugging.
	''' </summary>
	Public Overrides Function ToString() As String
		Return String.Format("{0}: ({1:F2}, {2:F2}, {3:F2})", Name, Position(0), Position(1), Position(2))
	End Function

#End Region

End Class
