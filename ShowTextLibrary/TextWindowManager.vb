Imports System.Windows


Public NotInheritable Class TextWindowManager

#Region " Fields "

	Private mWindowList As New Dictionary(Of Integer, Window)

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

	Public Sub ShowText(windowIndex As Integer, text As String)
		Dim wnd As Window = Nothing
		If Not mWindowList.TryGetValue(windowIndex, wnd) Then
			wnd = New Window()
			mWindowList.Add(windowIndex, wnd)
		End If

		'Dim cfg = Configuration.Instance
		wnd.Show()
	End Sub


	Public Sub HideText(windowIndex As Integer)
		Dim wnd As Window = Nothing
		If mWindowList.TryGetValue(windowIndex, wnd) Then
			wnd.Hide()
		End If
	End Sub

#End Region

End Class
