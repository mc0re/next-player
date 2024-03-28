''' <summary>
''' A group of command definitions for UI (Settings dialog).
''' </summary>
Public Class VoiceCommandDescriptionGroup
    Inherits List(Of VoiceCommandDescriptionCollection)

#Region " Convenience methods to build a collection in code "

    Public Overloads Sub Add(title As String, commands As VoiceCommandDescriptionCollection)
        commands.Title = title
        Add(commands)
    End Sub

#End Region


#Region " Getters "

    ''' <summary>
    ''' Check whether a command with the given name exists in any collection.
    ''' </summary>
    ''' <param name="cmdName">System-defined command name (e.g. StopCommand)</param>
    Public Overloads Function Contains(cmdName As String) As Boolean
        For Each group In Me
            If group.Contains(cmdName) Then
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
        For Each group In Me
            Dim desc = group.GetDescription(cmdName)

            If desc IsNot Nothing Then
                Return desc
            End If
        Next

        Return Nothing
    End Function

#End Region

End Class
