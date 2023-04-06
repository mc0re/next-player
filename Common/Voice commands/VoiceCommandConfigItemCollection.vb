Imports System.ComponentModel
Imports System.Configuration


<Serializable>
<SettingsSerializeAs(SettingsSerializeAs.Xml)>
Public Class VoiceCommandConfigItemCollection
	Inherits BindingList(Of VoiceCommandConfigItem)

#Region " Additional methods "

	''' <summary>
	''' Check whether a command with the given name exists in the collection.
	''' </summary>
	''' <param name="cmdName">System-defined command name (e.g. StopCommand)</param>
	Public Overloads Function Contains(cmdName As String) As Boolean
		Return Any(Function(cmdSetting) cmdSetting.CommandName = cmdName)
	End Function


	Public Sub CopyFrom(original As ICollection(Of VoiceCommandConfigItem))
		Clear()

		If original Is Nothing Then Return

		For Each origItem In original
			Add(origItem)
		Next
	End Sub

#End Region

End Class
