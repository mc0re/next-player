Imports System.Diagnostics.CodeAnalysis
Imports Common


<Serializable>
Public Class TestPoint3D
    Implements IPoint3D

#Region " IPoint3D implementation "

    Public Property X As Double Implements IPoint3D.X


    Public Property Y As Double Implements IPoint3D.Y


    Public Property Z As Double Implements IPoint3D.Z

#End Region


#Region " Init and clean-up "

    Public Sub New(x As Double, y As Double, z As Double)
        Me.X = x
        Me.Y = y
        Me.Z = z
    End Sub

#End Region


#Region " ToString "

    <ExcludeFromCodeCoverage>
    Public Overrides Function ToString() As String
        Return $"({X}, {Y}, {Z}) TP"
    End Function

#End Region

End Class
