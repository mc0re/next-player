Imports System.Timers
Imports Common
Imports AudioPlayerLibrary


''' <summary>
''' Sets a given slide on a running PowerPoint instance.
''' </summary>
<Serializable()>
Public Class PlayerActionPowerPoint
	Inherits PlayerAction
	Implements ICommandIssuer

#Region " Fields "

	Private WithEvents mRefreshTimer As New Timer With {.AutoReset = True}

#End Region


#Region " PresenterIndex notifying property "

	Private mPresenterIndex As Integer


	''' <summary>
	''' Index of the presenter as specified in Settings.
	''' </summary>
	Public Property PresenterIndex As Integer
		Get
			Return mPresenterIndex
		End Get
		Set(value As Integer)
			If mPresenterIndex = value Then Return

			mPresenterIndex = value
			RaisePropertyChanged(Function() PresenterIndex)
		End Set
	End Property

#End Region


#Region " SetSlideAction notifying property "

	Private mSetSlideAction As SetSlideActions = SetSlideActions.SetByIndex


	Public Property SetSlideAction As SetSlideActions
		Get
			Return mSetSlideAction
		End Get
		Set(value As SetSlideActions)
			mSetSlideAction = value
			RaisePropertyChanged(Function() SetSlideAction)
		End Set
	End Property

#End Region


#Region " SlideIndex notifying property "

	Private mSlideIndex As Integer = 1


	''' <summary>
	''' Used if slide action is SetByIndex.
	''' </summary>
	''' <value>1-N</value>
	Public Property SlideIndex As Integer
		Get
			Return mSlideIndex
		End Get
		Set(value As Integer)
			If mSlideIndex = value Then Return

			Dim pres As IPresenterReference

			Try
				pres = GetReferences()
			Catch ex As Exception
				' Connection to the presenter was aborted
				pres = Nothing
			End Try

			Try
				If pres IsNot Nothing AndAlso pres.IsInitialized Then
					value = Math.Max(value, pres.MinSlideNumber)
					value = Math.Min(value, pres.MaxSlideNumber)
				End If

			Catch ex As Exception
				' Connection to the presenter was aborted, save as is
			End Try

			mSlideIndex = value
			RaisePropertyChanged(Function() SlideIndex)
		End Set
	End Property

#End Region


#Region " Init and clean-up "

	Public Sub New()
		Name = "Show PowerPoint slide"
		HasDuration = True
		ExecutionType = ExecutionTypes.MainContinuePrev
	End Sub

#End Region


#Region " Utility "

	''' <summary>
	''' Get reference for the given presenter (by index).
	''' </summary>
	Private Function GetReferences() As IPresenterReference
		Return PresenterFactory.GetReference(PresenterIndex)
	End Function


	''' <summary>
	''' Set current slide by index (1-based).
	''' </summary>
	Private Sub SetSlideByNumber(slideNumber As Integer)
		Dim pres = GetReferences()
		Debug.Assert(pres IsNot Nothing AndAlso pres.IsInitialized)

		pres.SetSlideByNumber(slideNumber, Me)
	End Sub


	''' <summary>
	''' Advance to the next slide.
	''' </summary>
	Private Sub SetNextSlide()
		Dim pres = GetReferences()
		Debug.Assert(pres IsNot Nothing AndAlso pres.IsInitialized)

		SlideIndex = pres.CurrentSlideNumber + 1
		SetSlideByNumber(SlideIndex)
	End Sub


	''' <summary>
	''' Go to the previous slide.
	''' </summary>
	Private Sub SetPreviousSlide()
		Dim pres = GetReferences()
		Debug.Assert(pres IsNot Nothing AndAlso pres.IsInitialized)

		SlideIndex = pres.CurrentSlideNumber - 1
		SetSlideByNumber(SlideIndex)
	End Sub

#End Region


#Region " PlayerAction overrides "

	Public Overrides Sub Start()
		MyBase.Start()

		Dim pres = GetReferences()
		If pres Is Nothing Then Return

		If Not pres.IsInitialized Then pres.OpenApplication()
		If Not pres.IsInitialized Then Return

		Select Case SetSlideAction
			Case SetSlideActions.SetByIndex
				SetSlideByNumber(SlideIndex)

			Case SetSlideActions.SetNext
				SetNextSlide()

			Case SetSlideActions.SetPrevious
				SetPreviousSlide()
		End Select

		Dim config = InterfaceMapper.GetImplementation(Of IPresenterConfiguration)().GetPresenter(PresenterIndex)
		If config Is Nothing OrElse Not config.UseUpdateTimer Then Return

		' Minutes to milliseconds
		mRefreshTimer.Interval = config.PowerPointUpdateInterval * SecondsInMinute * MillisecondsInSecond
		mRefreshTimer.Start()
	End Sub


	Public Overrides Sub [Stop](intendedResume As Boolean)
		mRefreshTimer.Stop()
		MyBase.[Stop](intendedResume)
	End Sub

#End Region


#Region " Refresh slides "

	''' <summary>
	''' Refresh timer elapsed, refresh slides so that kiosk mode
	''' does not timeout.
	''' </summary>
	Private Sub RefreshTimerElapsed() Handles mRefreshTimer.Elapsed
		RefreshCurrentSlide()
	End Sub


	''' <summary>
	''' Prevent timeout in kiosk mode.
	''' </summary>
	Private Sub RefreshCurrentSlide()
		Dim origSlide = SlideIndex
		Dim tempSlide = origSlide - 1
		If tempSlide < 1 Then tempSlide = origSlide + 1

		SetSlideByNumber(tempSlide)
		SetSlideByNumber(origSlide)
	End Sub

#End Region


#Region " ToString "

	Public Overrides Function ToString() As String
		Dim action = SetSlideAction.ToString()

		If SetSlideAction = SetSlideActions.SetByIndex Then
			action = String.Format("{0} {1}", action, SlideIndex)
		End If

		Return String.Format("{0} {1}", MyBase.ToString(), action)
	End Function

#End Region

End Class
