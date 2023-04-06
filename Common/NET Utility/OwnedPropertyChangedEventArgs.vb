Imports System.ComponentModel


Public Class OwnedPropertyChangedEventArgs
	Inherits PropertyChangedEventArgs

#Region " PropertyOwner read-only property "

	Public ReadOnly Property PropertyOwner As Object

#End Region


#Region " Init and clean-up "

	Public Sub New(propName As String, propOwner As Object)
		MyBase.New(propName)
		PropertyOwner = propOwner
	End Sub

#End Region

End Class
