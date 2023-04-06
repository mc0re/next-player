''' <summary>
''' Describes a configuration of a single presentation.
''' </summary>
Public Class PowerPointConfiguration
	Inherits PropertyChangedHelper

	''' <summary>
	''' Presenter index (0-based).
	''' </summary>
	Public Property Index As Integer


	''' <summary>
	''' Full path to the slide deck.
	''' </summary>
	Public Property FilePath As String


	''' <summary>
	''' Whether to update slides, based on PowerPointUpdateInterval.
	''' </summary>
	Public Property UseUpdateTimer As Boolean


	''' <summary>
	''' PowerPoint slide update [minutes].
	''' </summary>
	Public Property PowerPointUpdateInterval As Integer

End Class
