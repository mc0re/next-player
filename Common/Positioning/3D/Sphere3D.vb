Imports System.Diagnostics.CodeAnalysis


''' <summary>
''' Represents a sphere in 3D space.
''' </summary>
<CLSCompliant(True)>
<Serializable>
Public Class Sphere3D

#Region " Properties "

    Public ReadOnly Center As IPoint3D

    Public ReadOnly Radius As Double

#End Region


#Region " Init and clean-up "

    Public Sub New(center As IPoint3D, radius As Double)
        Me.Center = center
        Me.Radius = radius
    End Sub

#End Region


#Region " ToString "

    <ExcludeFromCodeCoverage>
    Public Overrides Function ToString() As String
        Return String.Format("{0} r {1}", Center, Radius)
    End Function

#End Region

End Class
