Imports AudioPlayerLibrary

''' <summary>
''' Operations for slide presenter.
''' </summary>
Public Interface IPresenterReference

	''' <summary>
	''' Something has changed in the given presenter.
	''' </summary>
	Event PresenterChanged(presenterIndex As Integer)


	''' <summary>
	''' Whether the presenter is initialized.
	''' If so, all other properties are defined.
	''' </summary>
	ReadOnly Property IsInitialized As Boolean


	''' <summary>
	''' Minimum slide number, usually 1.
	''' </summary>
	ReadOnly Property MinSlideNumber As Integer


	''' <summary>
	''' Maximum slide number.
	''' </summary>
	ReadOnly Property MaxSlideNumber As Integer


	''' <summary>
	''' Index of the currently shown slide.
	''' </summary>
	ReadOnly Property CurrentSlideNumber As Integer


	''' <summary>
	''' Set slide. <paramref name="action"/> is needed to be able to stop the
	''' previous presenter action.
	''' </summary>
	Sub SetSlideByNumber(slide As Integer, action As IPlayerAction)


	''' <summary>
	''' Get identifying text for the slide.
	''' </summary>
	Function GetSlideText(slide As Integer) As String


	''' <summary>
	''' Get the file name (in TEMP path) for a thumbnail for the given slide.
	''' </summary>
	Function GetThumbnailName(allAsMask As Boolean, slide As Integer) As String


	''' <summary>
	''' Create / update slide thumbnails.
	''' </summary>
	''' <param name="forceUpdate">Overwrite all existing</param>
	Sub UpdateThumbnails(forceUpdate As Boolean)


	''' <summary>
	''' Start the application, which will show the slides.
	''' </summary>
	Sub OpenApplication()

End Interface
