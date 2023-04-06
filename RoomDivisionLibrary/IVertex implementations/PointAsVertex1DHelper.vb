Imports Common


''' <summary>
''' Convenience class.
''' </summary>
Public NotInheritable Class PointAsVertex1DHelper

#Region " Init and clean-up "

	Private Sub New()
		' Do nothing
	End Sub

#End Region


#Region " Factory method "

    Public Shared Function Create(Of TRef)(
        line As ILine3D, coord As Double, origPoint As Point3D(Of TRef)
    ) As PointAsVertex1D(Of TRef)

        Return New PointAsVertex1D(Of TRef)(line, coord, origPoint)
    End Function

#End Region

End Class
