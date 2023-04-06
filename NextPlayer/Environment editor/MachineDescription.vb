Imports LicenseLibrary


Public Class MachineDescription

#Region " Properties "

	Public Property Name As String

	Public Property FingerPrint As MachineFingerPrint

	Public Property IsThisMachine As Boolean

#End Region


#Region " ToString "

	Public Overrides Function ToString() As String
		Dim str = Name

		If IsThisMachine Then
			str &= " (this)"
		End If

		Return str
	End Function

#End Region

End Class
