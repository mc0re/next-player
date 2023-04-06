Imports System.Diagnostics.CodeAnalysis
Imports Common
Imports MIConvexHull


Public Class Point2DVertex(Of TRef)
    Implements IVertex2D, IVertex

#Region " Properties "

    Public ReadOnly Property Point As Point2D(Of TRef)

    Public ReadOnly Property X As Double Implements IVertex2D.X

    Public ReadOnly Property Y As Double Implements IVertex2D.Y

    Public ReadOnly Property Position As Double() Implements IVertex.Position

#End Region


#Region " Init and clean-up "

    Public Sub New()

    End Sub


    Public Sub New(point As Point2D(Of TRef))
        X = point.X
        Y = point.Y
        Position = {point.X, point.Y}
        Me.Point = point
    End Sub

#End Region


#Region " ToString "

    <ExcludeFromCodeCoverage>
    Public Overrides Function ToString() As String
        Return Point.ToString()
    End Function

#End Region

End Class
