Public Class MediaEndedEventArgs
	Inherits EventArgs

	''' <summary>
	''' File being stopped.
	''' </summary>
	Public Property FileName As String


	''' <summary>
	''' In case of error, the message is not empty.
	''' </summary>
	Public Property ErrorMessage As String

End Class
