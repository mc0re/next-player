Public Class EnvironmentDescription

#Region " Properties "

	Public Property Name As String


	Public Property HasLocal As Boolean

#End Region


#Region " ToString "

	Public Overrides Function ToString() As String
		Dim str = Name

		If HasLocal Then
			str &= " (local)"
		End If

		Return str
	End Function

#End Region

End Class
