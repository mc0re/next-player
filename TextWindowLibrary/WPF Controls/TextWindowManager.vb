Imports Common
Imports TextChannelLibrary


Public NotInheritable Class TextWindowManager
	Implements ITextWindowManager

#Region " Fields "

	Private mWindowList As New List(Of TextWindow)

#End Region


#Region " Singleton "

	Private Shared mInstance As New TextWindowManager()


	Public Shared ReadOnly Property Instance As TextWindowManager
		Get
			Return mInstance
		End Get
	End Property


	Private Sub New()
		' Do nothing
	End Sub

#End Region


#Region " API "

	''' <summary>
	''' Show a single window with the given text.
	''' </summary>
	Public Sub ShowPhysical(physChannel As Integer, text As String) Implements ITextWindowManager.ShowPhysical
		Dim confProvider = InterfaceMapper.GetImplementation(Of ITextConfigurationStorage)()
		Dim conf = confProvider.PerMachine.Physical.InConfig.Channel(physChannel)
		If conf Is Nothing Then Return

		ShowTextWindows({conf}, text)
	End Sub


	''' <summary>
	''' Show all windows for the given logical channel with the given text.
	''' </summary>
	Public Sub ShowLogical(logChannel As Integer, text As String) Implements ITextWindowManager.ShowLogical
		ShowTextWindows(GetPhysChannels(logChannel), text)
	End Sub


	''' <summary>
	''' Hide text windows by physical channel.
	''' </summary>
	Public Sub HidePhysical(physChannel As Integer) Implements ITextWindowManager.HidePhysical
		Dim wndList = (From tw In mWindowList Where tw.Configuration.Channel = physChannel).ToList()

		For Each wnd In wndList
			wnd.Hide()
			wnd.Configuration.IsActive = False
		Next
	End Sub


	''' <summary>
	''' Hide text windows by logical channel.
	''' </summary>
	Public Sub HideLogical(logChannel As Integer) Implements ITextWindowManager.HideLogical
		Dim confList = GetPhysChannels(logChannel).ToList()
		Dim wndList = From tw In mWindowList Where confList.Any(Function(c) c.Channel = tw.Configuration.Channel)

		For Each wnd In wndList
			wnd.Hide()
			wnd.Configuration.IsActive = False
		Next
	End Sub


	''' <summary>
	''' Hide all shown windows.
	''' </summary>
	Public Sub HideAll() Implements ITextWindowManager.HideAll
		For Each wnd In mWindowList
			wnd.Hide()
			wnd.Configuration.IsActive = False
		Next
	End Sub

#End Region


#Region " Utility "

	Private Shared Function GetPhysChannels(logChannel As Integer) As IEnumerable(Of TextPhysicalChannel)
		Dim confProvider = InterfaceMapper.GetImplementation(Of ITextConfigurationStorage)()
		Dim conf = confProvider.Logical.InConfig.Channel(logChannel)
		If conf Is Nothing Then Return {}

		Dim confList =
			From lnk In conf.PhysicalLinkList
			Let ph = confProvider.Physical.InConfig.Channel(lnk.PhysicalChannel)
			Where ph IsNot Nothing
			Select ph

		Return confList
	End Function


	''' <summary>
	''' Show windows with the given text.
	''' </summary>
	Private Sub ShowTextWindows(confList As IEnumerable(Of TextPhysicalChannel), text As String)
		Dim oldWin = Application.Current.Windows.OfType(Of Window)().SingleOrDefault(Function(w) w.IsActive)

		' Check for already created windows
		Dim wndList = (From tw In mWindowList Where confList.Contains(tw.Configuration)).ToList()

		Dim isAllowed = confList.Any()

		' Not allowed, hide and leave
		If Not isAllowed Then
			For Each tw In wndList
				tw.Hide()
				tw.Configuration.IsActive = False
			Next

			Return
		End If

		' Allowed, create if needed
		For Each conf In confList
			If (From tcw In wndList Where tcw.Configuration Is conf).Any() Then Continue For

			Dim tw As New TextWindow()
			tw.Configuration = conf
			mWindowList.Add(tw)
			wndList.Add(tw)
		Next

		' Show all needed windows
		For Each wnd In wndList
			If Not wnd.IsVisible Then
				wnd.Show()
			End If

			wnd.Text = text
			wnd.Topmost = True
			wnd.Configuration.IsActive = True
		Next

		' Set the focus back to main window
		If oldWin IsNot Nothing Then
			oldWin.Focus()
		End If
	End Sub

#End Region

End Class
