Imports System.Diagnostics.CodeAnalysis
Imports Common
Imports MIConvexHull


''' <summary>
''' Keeps information about the projection of a point onto a 3D plane.
''' </summary>
<CLSCompliant(True)>
Public Class ProjectionRoomToPlane
	Implements IVertex, IPoint2D

#Region " Properties "

	''' <summary>
	''' The plane all room projections lie on.
	''' </summary>
	Public ReadOnly Property Plane As Plane3D


	''' <summary>
	''' The coordinates of this projection on the plane.
	''' </summary>
	Public ReadOnly Property Projection As Point2D


	''' <summary>
	''' Actual projection (speaker to room wall, 3D) being projected onto the plane.
	''' </summary>
	Public ReadOnly Property RoomProj As ProjectionToRoom

#End Region


#Region " IVertex implementation "

	Private ReadOnly mPosition As Double()


	''' <summary>
	''' <see cref="IVertex"/> position for convex hull.
	''' </summary>
	Public ReadOnly Property Position As Double() Implements IVertex.Position
		Get
			Return mPosition
		End Get
	End Property

#End Region


#Region " IPoint2D implementation "

	Public ReadOnly Property X As Double Implements IPoint2D.X
		Get
			Return Projection.X
		End Get
	End Property


	Public ReadOnly Property Y As Double Implements IPoint2D.Y
		Get
			Return Projection.Y
		End Get
	End Property

#End Region


#Region " Init and clean-up "

	Public Sub New(plane As Plane3D, coord As Point2D, ref As ProjectionToRoom)
		Me.Plane = plane
		Me.Projection = coord
		Me.RoomProj = ref

		mPosition = New Double() {coord.X, coord.Y}
	End Sub

#End Region


#Region " ToString "

	<ExcludeFromCodeCoverage>
	Public Overrides Function ToString() As String
		Return String.Format("{0} -> {1}", RoomProj.ToString(), Projection)
	End Function

#End Region

End Class
