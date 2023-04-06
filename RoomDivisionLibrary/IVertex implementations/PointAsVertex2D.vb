Imports Common
Imports MIConvexHull


''' <summary>
''' Represents a point for MIConvexHull triangulation algorithms.
''' </summary>
<CLSCompliant(True)>
Public Class PointAsVertex2D(Of TRef)
    Implements IVertex

#Region " IVertex.Position read-only property "

    ''' <summary>
    ''' 2D coordinate for <see cref="IVertex"/>.
    ''' </summary>
    Public ReadOnly Property Position As Double() Implements IVertex.Position

#End Region


#Region " Own properties "

    ''' <summary>
    ''' Projection plane.
    ''' </summary>
    Public Property Plane As IPlane3D


    ''' <summary>
    ''' The original point.
    ''' </summary>
    Public ReadOnly Property OriginalPoint As Point3D(Of TRef)

#End Region


#Region " Init and clean-up "

    Public Sub New(plane As IPlane3D, coord As IPoint2D, original As Point3D(Of TRef))
        Position = New Double() {coord.X, coord.Y}
        Me.Plane = plane
        OriginalPoint = original
    End Sub

#End Region

End Class
