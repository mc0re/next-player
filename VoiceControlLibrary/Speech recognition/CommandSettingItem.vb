Imports System.ComponentModel
Imports Common


''' <summary>
''' Used for displaying the command in Settings dialog.
''' </summary>
Public Class CommandSettingItem

#Region " Setting property "

	Public Property Setting As VoiceCommandConfigItem

#End Region


#Region " Command property "

	Public Property Command As RoutedCommand

#End Region


#Region " ParameterType property "

	Public Property ParameterType As CommandParameterTypes

#End Region


#Region " Description property "

	Public Property Description As String

#End Region


#Region " ToString "

	Public Overrides Function ToString() As String
		Return Setting.ToString()
	End Function

#End Region

End Class


Public Class VoiceCommandSettingItemCollection
	Inherits BindingList(Of CommandSettingItem)

	Public Sub New()
		' Do nothing
	End Sub


	Public Sub New(ops As IEnumerable(Of CommandSettingItem))
		For Each op In ops
			Me.Add(op)
		Next
	End Sub

End Class