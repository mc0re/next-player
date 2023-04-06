''' <summary>
''' Bind to application settings.
''' </summary>
Public Class SettingBindingExtension
	Inherits Binding

#Region " Init and clean-up "

	Public Sub New()
		Initialize()
	End Sub


	Public Sub New(path As String)
		MyBase.New(path)
		Initialize()
	End Sub

#End Region


#Region " Utility "

	Private Sub Initialize()
		Source = My.Settings

		If Mode = BindingMode.Default Then
			Mode = BindingMode.TwoWay
		End If
	End Sub

#End Region

End Class
