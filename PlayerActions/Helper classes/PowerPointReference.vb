Imports System.IO
Imports PPt = NetOffice.PowerPointApi
Imports Common
Imports AudioPlayerLibrary
Imports NetOffice.OfficeApi.Enums

''' <summary>
''' Establishes a reference to PowerPoint application.
''' </summary>
''' <remarks>
''' Uses Microsoft.Interop from VSTO, get at http://www.microsoft.com/en-us/download/details.aspx?id=44074
''' </remarks>
Public Class PowerPointReference
	Implements IPresenterReference

#Region " Fields "

	Private mPresenterIndex As Integer


	Private mCurrentSlide As PPt.Slide

#End Region


#Region " Events "

	Public Event PresenterChanged(presenterIndex As Integer) Implements IPresenterReference.PresenterChanged


	''' <summary>
	''' Change has occured, notify subscribers.
	''' </summary>
	Private Sub UpdateVersion()
		RaiseEvent PresenterChanged(mPresenterIndex)
	End Sub

#End Region


#Region " PresentationFile read-only property "

	Private mPresentationFileName As String


	Public Property PresentationFileName As String
		Get
			Return mPresentationFileName
		End Get
		Set(value As String)
			If mPresentationFileName = value Then Return

			mPresentationFileName = value
			GenerateThumbnails(False)
			RefreshReferences()
		End Set
	End Property

#End Region


#Region " Last action used this presentation "

	Private mLastAction As IPlayerAction


	Private Sub SetCurrentAction(controller As IPlayerAction)
		If controller IsNot Nothing AndAlso Not mLastAction Is controller Then
			If mLastAction IsNot Nothing Then mLastAction.Stop(False)
			mLastAction = controller
		End If
	End Sub

#End Region


#Region " PptApplication read-only property "

	Private mPptApplication As PPt.Application


	Public ReadOnly Property PptApplication As PPt.Application
		Get
			Return mPptApplication
		End Get
	End Property

#End Region


#Region " SlideCollection read-only property "

	Private mSlideCollection As PPt.Slides


	Public ReadOnly Property SlideCollection As PPt.Slides
		Get
			Return mSlideCollection
		End Get
	End Property

#End Region


#Region " TextCollection read-only property "

	Private mTextCollection As New Dictionary(Of Integer, String)()


	''' <summary>
	''' A list of found text per slide.
	''' </summary>
	Public ReadOnly Property TextCollection As IDictionary(Of Integer, String)
		Get
			Return mTextCollection
		End Get
	End Property

#End Region


#Region " IsInitialized read-only property "

	Private mIsInitialized As Boolean


	Public ReadOnly Property IsInitialized As Boolean Implements IPresenterReference.IsInitialized
		Get
			Return mIsInitialized
		End Get
	End Property


	Private WriteOnly Property IsInitializedWritable As Boolean
		Set(value As Boolean)
			If mIsInitialized = value Then Return

			mIsInitialized = value
			UpdateVersion()
		End Set
	End Property

#End Region


#Region " MinSlideNumber read-only property "

	Public ReadOnly Property MinSlideNumber As Integer Implements IPresenterReference.MinSlideNumber
		Get
			Return 1
		End Get
	End Property

#End Region


#Region " MaxSlideNumber read-only property "

	Public ReadOnly Property MaxSlideNumber As Integer Implements IPresenterReference.MaxSlideNumber
		Get
			If Not IsInitialized Then Return 0
			Return SlideCollection.Count
		End Get
	End Property

#End Region


#Region " CurrentSlideNumber read-only property "

	Public ReadOnly Property CurrentSlideNumber As Integer Implements IPresenterReference.CurrentSlideNumber
		Get
			Return mCurrentSlide.SlideIndex
		End Get
	End Property

#End Region


#Region " Init and clean-up "

	''' <summary>
	''' Create a new instance of PowerPoint presenter.
	''' </summary>
	Public Sub New(index As Integer)
		mPresenterIndex = index
	End Sub


	''' <summary>
	''' Get PowerPoint application reference.
	''' </summary>
	''' <returns>False if not found</returns>
	Public Function GetReferences() As Boolean
		Try
			If mCurrentSlide IsNot Nothing Then Return True

			If GetAppReference() AndAlso GetSlideCollection() AndAlso GetCurrentSlide() Then
				UpdateVersion()
				IsInitializedWritable = True
			Else
				CleanReferences()
				IsInitializedWritable = False
			End If

		Catch ex As Exception
			CleanReferences()
			IsInitializedWritable = False
		End Try

		Return IsInitialized
	End Function


	''' <summary>
	''' Clean up COM references.
	''' </summary>
	Public Sub CleanReferences()
		mCurrentSlide = Nothing
		mSlideCollection = Nothing
		mPptApplication = Nothing

		IsInitializedWritable = False
	End Sub

#End Region


#Region " Thumbnails "

	''' <summary>
	''' Generate filename for the given slide thumbnail.
	''' </summary>
	Public Function GetThumbnailName(allAsMask As Boolean, slideIndex As Integer) As String Implements IPresenterReference.GetThumbnailName
		Dim slideNr = If(allAsMask, "*", slideIndex.ToString())
		Return IO.Path.Combine(IO.Path.GetTempPath(),
							   IO.Path.GetFileNameWithoutExtension(PresentationFileName) & "." & slideNr & ".jpg")
	End Function


	''' <summary>
	''' Generate thumbnails in system temp path.
	''' Get slide texts.
	''' </summary>
	<CodeAnalysis.SuppressMessage("Design", "CC0004:Catch block cannot be empty", Justification:="<Pending>")>
	Public Sub GenerateThumbnails(forceUpdate As Boolean)
		If String.IsNullOrWhiteSpace(PresentationFileName) Then Return

		Dim app As PPt.Application = Nothing
		Dim quitApp = True
		Dim pptPresentation As PPt.Presentation = Nothing

		Dim ofalse = MsoTriState.msoFalse
		Dim otrue = MsoTriState.msoTrue

		Try
			Dim presDate = File.GetLastWriteTime(PresentationFileName)
			app = New PPt.Application()
			quitApp = app.Presentations.Count = 0
			pptPresentation = app.Presentations.Open(PresentationFileName, otrue, ofalse, ofalse)

			If forceUpdate Then
				Dim slideMask = GetThumbnailName(True, 0)
				For Each fname In New DirectoryInfo(Path.GetDirectoryName(slideMask)).GetFiles(Path.GetFileName(slideMask))
					Try
						fname.Delete()
					Catch ex As Exception
						' Cannot delete slide, may be in use, ignore
					End Try
				Next
			End If

			TextCollection.Clear()

			For Each slide In pptPresentation.Slides.Cast(Of PPt.Slide)()
				Dim sfName = GetThumbnailName(False, slide.SlideNumber)
				Dim slideDate = File.GetLastWriteTime(sfName)

				If forceUpdate OrElse slideDate < presDate Then
					Try
						slide.Export(sfName, "JPG")
					Catch ex As Exception
						' Cannot overwrite slide, may be in use, ignore
					End Try
				End If

				' Get texts
				Dim str = GetTextFromShapes(slide.Shapes)

				If String.IsNullOrWhiteSpace(str) Then
					str = GetTextFromShapes(slide.NotesPage.Shapes)
				End If

				TextCollection(slide.SlideNumber) = str
			Next

		Catch ex As ArgumentException
			InterfaceMapper.GetImplementation(Of IMessageLog)().LogPowerPointError(
				"PowerPoint is not installed.")

		Finally
			If pptPresentation IsNot Nothing Then pptPresentation.Close()
			If app IsNot Nothing AndAlso quitApp Then app.Quit()

			' Trying to close PowerPoint application
			GC.Collect()
			GC.WaitForPendingFinalizers()

			UpdateVersion()
		End Try
	End Sub

#End Region


#Region " Utility "

	''' <summary>
	''' Establish a connection to the PowerPoint application.
	''' </summary>
	''' <returns>True if Ok, False if cannot make a connection</returns>
	Private Function GetAppReference() As Boolean
		Try
			If mSlideCollection IsNot Nothing AndAlso mSlideCollection.Count >= 0 Then
				Return True
			End If
		Catch ex As Exception
			InterfaceMapper.GetImplementation(Of IMessageLog)().LogPowerPointError(
				"PowerPoint was closed, reestablishing connection.")
		End Try

		Dim isRunning = True

		Try
			' Get Running PowerPoint Application object 
			mPptApplication = PPt.Application.GetActiveInstance()
		Catch ex As Exception
			isRunning = False
		End Try

		If mPptApplication Is Nothing OrElse Not isRunning Then
			InterfaceMapper.GetImplementation(Of IMessageLog)().LogPowerPointError(
				"PowerPoint is not running.")
			Return False
		End If

		Return True
	End Function


	''' <summary>
	''' Get a collection of slides.
	''' </summary>
	''' <returns>True if Ok, False in case of error</returns>
	Private Function GetSlideCollection() As Boolean
		If mPptApplication.Presentations.Count = 0 Then
			If mPptApplication.ProtectedViewWindows.Count = 0 Then
				InterfaceMapper.GetImplementation(Of IMessageLog)().LogPowerPointError(
					"Presentation is not opened.")
			Else
				InterfaceMapper.GetImplementation(Of IMessageLog)().LogPowerPointError(
					"Presentation is opened in protected mode, enable editing.")
			End If

			Return False
		End If

		Try
			For Each presIdx In Enumerable.Range(1, mPptApplication.Presentations.Count)
				Dim pres = mPptApplication.Presentations(presIdx)
				If pres.FullName = PresentationFileName Then
					mSlideCollection = mPptApplication.Presentations(presIdx).Slides
					Return True
				End If
			Next

			InterfaceMapper.GetImplementation(Of IMessageLog)().LogPowerPointError(
				"Presentation is not opened: " & PresentationFileName)
			Return False

		Catch ex As Exception
			InterfaceMapper.GetImplementation(Of IMessageLog)().LogPowerPointError(
				"Presentation is not active: " & ex.Message)
			Return False
		End Try
	End Function


	''' <summary>
	''' Get a currently shown slide.
	''' </summary>
	''' <returns>True if Ok, False in case of error</returns>
	Private Function GetCurrentSlide() As Boolean
		Dim showWindowList = mPptApplication.SlideShowWindows
		If showWindowList.Count = 0 Then
			InterfaceMapper.GetImplementation(Of IMessageLog)().LogPowerPointError(
				"Presentation is in normal mode, run slide show.")

			Try
				' Get selected slide object in normal view 
				mCurrentSlide = mSlideCollection(mPptApplication.ActiveWindow.Selection.SlideRange.SlideNumber)
			Catch ex As Exception
				InterfaceMapper.GetImplementation(Of IMessageLog)().LogPowerPointError(
					"Problems getting current slide (no slides?): " & ex.Message)
				Return False
			End Try
		Else
			' Get selected slide object in reading view 
			mCurrentSlide = showWindowList(1).View.Slide
		End If

		Return True
	End Function


	''' <summary>
	''' Get the first text in the shape list.
	''' </summary>
	<CodeAnalysis.SuppressMessage("Design", "CC0004:Catch block cannot be empty", Justification:="<Pending>")>
	Private Shared Function GetTextFromShapes(shapeList As PPt.Shapes) As String
		For Each shapeIdx In Enumerable.Range(1, shapeList.Count)
			Try
				Dim tf = shapeList(shapeIdx).TextFrame
				If tf.HasText <> MsoTriState.msoTrue Then Continue For
				Return tf.TextRange.Text

			Catch ex As Exception
				' Something is wrong, proceed to next shape
			End Try
		Next

		Return Nothing
	End Function


	Private Sub RefreshReferences()
		CleanReferences()
		GetReferences()
	End Sub

#End Region


#Region " IPresenterReference implementation "

	''' <summary>
	''' Set current slide by index (1-based).
	''' </summary>
	Public Sub SetSlideByNumber(slideNumber As Integer, action As IPlayerAction) Implements IPresenterReference.SetSlideByNumber
		Dim slideCount As Integer

		' Check that we're still online
		Try
			slideCount = SlideCollection.Count
		Catch ex As Exception
			' Cannot access the app, try restarting it
			InterfaceMapper.GetImplementation(Of IMessageLog)().LogPowerPointError(
				"Reestablishing connection.")
			OpenApplication()

			Try
				slideCount = SlideCollection.Count
			Catch ex2 As Exception
				' We cannot proceed
				Return
			End Try
		End Try

		' Verify slide number
		If slideNumber < 1 Then
			InterfaceMapper.GetImplementation(Of IMessageLog)().LogPowerPointError(
				"Slide index must be 1..{0}, trying to set {1}. Truncated to {0}.",
				slideCount, slideNumber)

			slideNumber = 1
		End If

		If slideNumber > slideCount Then
			InterfaceMapper.GetImplementation(Of IMessageLog)().LogPowerPointError(
				"Slide index must be 1..{0}, trying to set {1}. Truncated to {0}.",
				slideCount, slideNumber)

			slideNumber = slideCount
		End If

		' Get reference to PPT
		Dim showWindowList As PPt.SlideShowWindows
		Try
			showWindowList = PptApplication.SlideShowWindows
		Catch ex As Exception
			If Not GetReferences() Then Return
			showWindowList = PptApplication.SlideShowWindows
		End Try

		' Everything is fine, switch
		SetCurrentAction(action)

		If showWindowList Is Nothing OrElse showWindowList.Count = 0 Then
			' This is normal mode
			Try
				mCurrentSlide = SlideCollection(slideNumber)
				mCurrentSlide.[Select]()
			Catch ex As Exception
				InterfaceMapper.GetImplementation(Of IMessageLog)().LogPowerPointError(
					"Error in normal mode: {0}", ex.Message)
			End Try
		Else
			' This is presenter mode
			Dim showView = showWindowList(1).View
			Try
				showView.GotoSlide(slideNumber)
			Catch ex As Exception
				InterfaceMapper.GetImplementation(Of IMessageLog)().LogPowerPointError(
					"Error in pres mode: {0}", ex.Message)
			End Try
			mCurrentSlide = showView.Slide
		End If
	End Sub


	''' <summary>
	''' Get slide text.
	''' </summary>
	Public Function GetSlideText(slide As Integer) As String Implements IPresenterReference.GetSlideText
		Dim slideText = String.Empty
		TextCollection.TryGetValue(slide, slideText)
		Return slideText
	End Function


	Public Sub UpdateThumbnails(forceUpdate As Boolean) Implements IPresenterReference.UpdateThumbnails
		GenerateThumbnails(forceUpdate)
		RefreshReferences()
	End Sub


	Public Sub OpenApplication() Implements IPresenterReference.OpenApplication
		Try
			Dim app As PPt.Application

			Try
				app = PPt.Application.GetActiveInstance()

				If app Is Nothing Then
					app = New PPt.Application()
				End If
			Catch ex As Exception
				app = New PPt.Application()
			End Try

			Dim pres = app.Presentations.Open(PresentationFileName)
			With pres.SlideShowSettings
				.ShowMediaControls = MsoTriState.msoFalse
				.ShowType = PPt.Enums.PpSlideShowType.ppShowTypeWindow
				.Run()
			End With

			RefreshReferences()

		Catch ex As Exception
			InterfaceMapper.GetImplementation(Of IMessageLog)().LogPowerPointError(
				"PowerPoint cannot be opened with file '{0}'. Not installed?", PresentationFileName)
		End Try
	End Sub

#End Region

End Class
