''' <summary>
''' This class is used for defining, how commands are shown in UI.
''' </summary>
''' <summary>
''' A collection of command definitions for UI (Settings dialog).
''' </summary>
Public Class VoiceCommandDescriptionCollection
    Inherits List(Of VoiceCommandDescription)

    Public Property Title As String


#Region " Convenience methods to build a collection in code "

    Public Overloads Sub Add(cmdName As String, cmdUserName As String, defText As String, Optional flags As CommandFlags = CommandFlags.Default)
        Add(cmdName, cmdUserName, defText, CommandParameterTypes.None, flags)
    End Sub


    Public Overloads Sub Add(
            cmdName As String,
            cmdDescription As String,
            defText As String,
            paramType As CommandParameterTypes,
            Optional flags As CommandFlags = CommandFlags.Default
            )
        Add(New VoiceCommandDescription() With {
            .CommandName = cmdName,
            .ParameterType = paramType,
            .Description = cmdDescription,
            .DefaultText = defText,
            .Flags = flags
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
