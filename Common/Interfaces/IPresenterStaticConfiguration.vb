Public Interface IPresenterStaticConfiguration

	''' <summary>
	''' Whether to update slides, based on PowerPointUpdateInterval.
	''' </summary>
	Property UseUpdateTimer As Boolean


	''' <summary>
	''' PowerPoint slide update [minutes].
	''' </summary>
	Property PowerPointUpdateInterval As Integer


	''' <summary>
	''' Property oscillating when a presenter reports changes.
	''' </summary>
	Property PresenterVersion As Integer

End Interface
