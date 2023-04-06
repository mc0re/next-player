''' <summary>
''' This class is used for defining, how commands are shown in UI.
''' </summary>
Public Class VoiceCommandDescription

	''' <summary>
	''' The internal command name.
	''' </summary>
	Public Property CommandName As String


	''' <summary>
	''' Command parameter type.
	''' </summary>
	Public Property ParameterType As CommandParameterTypes


	''' <summary>
	''' The user-friendly description.
	''' </summary>
	Public Property Description As String


	''' <summary>
	''' Default text for speech recognition.
	''' </summary>
	<IgnoreForReport()>
	Public Property DefaultText As String


#Region " ToString "

	Public Overrides Function ToString() As String
		Return String.Format("{0} [{1}]", CommandName, DefaultText)
	End Function

#End Region

End Class


''' <summary>
''' A collection of command definitions for UI (Settings dialog).
''' </summary>
Public Class VoiceCommandDescriptionCollection
	Inherits List(Of VoiceCommandDescription)

#Region " Convenience methods to build a collection in code "

	Public Overloads Sub Add(cmdName As String, cmdUserName As String, defText As String)
        Add(cmdName, cmdUserName, defText, CommandParameterTypes.None)
    End Sub


	Public Overloads Sub Add(cmdName As String, cmdUserName As String, defText As String, paramType As CommandParameterTypes)
		Add(New VoiceCommandDescription() With {
			.CommandName = cmdName,
			.ParameterType = paramType,
			.Description = cmdUserName,
			.DefaultText = defText
		})
	End Sub

#End Region


#Region " Getters "

	''' <summary>
	''' Check whether a command with the given name exists in the collection.
	''' </summary>
	''' <param name="cmdName">System-defined command name (e.g. StopCommand)</param>
	Public Overloads Function Contains(cmdName As String) As Boolean
		For Each desc In Me
			If desc.CommandName = cmdName Then
				Return True
			End If
		Next

		Return False
	End Function


	''' <summary>
	''' Get user-friendly description for the given command.
	''' </summary>
	''' <param name="cmdName">System-defined command name (e.g. StopCommand)</param>
	Public Function GetDescription(cmdName As String) As VoiceCommandDescription
		For Each desc In Me
			If desc.CommandName = cmdName Then
				Return desc
			End If
		Next

		Return Nothing
	End Function

#End Region

End Class