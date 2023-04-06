Imports System.Diagnostics.CodeAnalysis
Imports Common
Imports MIConvexHull


<CLSCompliant(True)>
Public Class PointAsVertex1D(Of TRef)
    Implements IVertex

#Region " IVertex.Position read-only property "

    ''' <summary>
    ''' 1D coordinate for <see cref="IVertex"/>.
    ''' </summary>
    Public ReadOnly Property Position As Double() Implements IVertex.Position

#End Region


#Region " OriginalPoint read-only property "

    ''' <summary>
    ''' LIst of references, passed around.
    ''' </summary>
    Public ReadOnly Property OriginalPoint As Point3D(Of TRef)

#End Region


#Region " Projection properties "

    ''' <summary>
    ''' The line all points lie on.
    ''' </summary>
    Public ReadOnly Property Line As ILine3D


    ''' <summary>
    ''' The coordinates of this projection on the line.
    ''' </summary>
    Public ReadOnly Property Projection As Double

#End Region


#Region " Init and clean-up "

    Public Sub New(line As ILine3D, coord As Double, original As Point3D(Of TRef))
        Projection = coord
        Me.Line = line
        Position = New Double() {coord}
        OriginalPoint = original
    End Sub

#End Region


#Region " ToString "

    <ExcludeFromCodeCoverage>
    Public Overrides Function ToString() As String
        Return String.Format("{0}: {1}", Me.Projection, OriginalPoint)
    End Function

#End Region

End Class
