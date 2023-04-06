Public Interface IPresenterConfiguration

	''' <summary>
	''' How many presenters are defined.
	''' </summary>
	ReadOnly Property Count As Integer


	ReadOnly Property PresentationFile As IInputFile


	''' <summary>
	''' Create a <see cref="PresentationFile"/> for the given file path
	''' </summary>
	''' <param name="fileName"></param>
	Sub SetPresentation(fileName As String)


	''' <summary>
	''' Get configuration by index.
	''' </summary>
	Function GetPresenter(index As Integer) As PowerPointConfiguration

End Interface
