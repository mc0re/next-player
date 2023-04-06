Imports System.ComponentModel
Imports System.Reflection


''' <summary>
''' Derived class to add property info.
''' </summary>
Public Class PlayerNotifyPropertyChangedEventArgs
	Inherits PropertyChangedEventArgs

	Public ReadOnly PropInfo As PropertyInfo


	Public Sub New(propName As String, propInfo As PropertyInfo)
		MyBase.New(propName)
		Me.PropInfo = propInfo
	End Sub

End Class
