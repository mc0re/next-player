Imports System.ComponentModel
Imports Common


''' <summary>
''' Used for actual command recognition.
''' </summary>
Public Class VoiceOperation

#Region " Setting property "

	Public Property Setting As VoiceCommandConfigItem

#End Region


#Region " RecognizedText property "

	Public Property RecognizedText As String

#End Region


#Region " Command property "

	Public Property Command As RoutedCommand

#End Region


#Region " Parameter property "

	Public Property Parameter As Object

#End Region


#Region " ToString "

	Public Overrides Function ToString() As String
		Return Setting.ToString()
	End Function

#End Region

End Class


Public Class VoiceOperationCollection
	Inherits BindingList(Of VoiceOperation)

	Public Sub New()
		' Do nothing
	End Sub


	Public Sub New(ops As IEnumerable(Of VoiceOperation))
		For Each op In ops
			Me.Add(op)
		Next
	End Sub

End Class