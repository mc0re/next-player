Imports Common


''' <summary>
''' Convenience class.
''' </summary>
Public NotInheritable Class Vertex2DHelper

#Region " Init and clean-up "

    Private Sub New()
        ' Do nothing
    End Sub

#End Region


#Region " Factory method "

    Public Shared Function Create(Of TRef)(
        plane As IPlane3D, coord As IPoint2D, ref As Point3D(Of TRef)
    ) As PointAsVertex2D(Of TRef)

        Return New PointAsVertex2D(Of TRef)(plane, coord, ref)
    End Function

#End Region

End Class
