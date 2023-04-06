''' <summary>
''' This is a placeholder for an action,
''' to make sure the UI gets no NULLs.
''' </summary>
Public Class PlayerActionPlaceholder
	Inherits PlayerAction

#Region " Init and clean-up "

	Public Sub New()
		CanExecute = False
	End Sub

#End Region


#Region " ToString "

	''' <summary>
	''' For debugging.
	''' </summary>
	Public Overrides Function ToString() As String
		Return "No action"
	End Function

#End Region

End Class
