Imports Common


''' <summary>
''' A single face of the outer shell.
''' </summary>
Public Class ShellFace(Of TRef)

#Region " Properties "

    Public Property Points As ICollection(Of Point3D(Of TRef))


    Public Property Outside As Vector3D

#End Region


#Region " Init and clean-up "

    Public Sub New(pointList As ICollection(Of Point3D(Of TRef)), normalOutside As Vector3D)
        Points = pointList
        Outside = normalOutside
    End Sub

#End Region

End Class
