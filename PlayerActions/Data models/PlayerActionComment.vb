Public Class PlayerActionComment
	Inherits PlayerAction

#Region " Description notifying property "

	Private mDescription As String


	''' <summary>
	''' Description.
	''' </summary>
	Public Property Description As String
		Get
			Return mDescription
		End Get
		Set(value As String)
			mDescription = value
			RaisePropertyChanged(Function() Description)
		End Set
	End Property

#End Region


#Region " Init and clean-up "

	Public Sub New()
		CanExecute = False
		Name = "Comment"
	End Sub

#End Region

End Class
