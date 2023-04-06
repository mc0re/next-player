Imports Common


''' <summary>
''' Convenience class to avoid generics specification.
''' </summary>
Public NotInheritable Class PointAsVertex3DHelper

#Region " Init and clean-up "

	Private Sub New()
		' Do nothing
	End Sub

#End Region


#Region " Factory method "

	''' <summary>
	''' Create a new PointAsVertex3D instance.
	''' </summary>
	Public Shared Function Create(Of TCookie)(
		point As IPoint3D, payload As TCookie
	) As PointAsVertex3D(Of TCookie)

		Return New PointAsVertex3D(Of TCookie)(point, payload)
	End Function

#End Region

End Class
