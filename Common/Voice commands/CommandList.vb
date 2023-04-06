Public Module CommandList

#Disable Warning CC0108 ' You should use nameof instead of the parameter element name string
    <CodeAnalysis.SuppressMessage("Design", "CC0021:You should use nameof instead of the parameter element name string",
                                  Justification:="These are strings, not names")>
    Public ReadOnly AppCommandList As New VoiceCommandDescriptionCollection() From {
        {"PlayNextCommand", "Play next action", "Next"},
        {"PlayAgainCommand", "Replay the active action", "Replay again"},
        {"ResetPlaylistCommand", "Reset playlist position", "Reset"},
        {"StartPlaylistCommand", "Start playlist in passive mode", "Passive"},
        {"StopCommand", "Stop all actions", "Stop"},
        {"ResumeCommand", "Resume stopped main actions", "Resume"},
        {"PlayStopParallelCommand", "Play and stop parallel action; index appended", "", CommandParameterTypes.ParallelIndex},
        {"SelectActionCommand", "Select action by number; index appended", "Choose", CommandParameterTypes.ItemIndex},
        {"ExecuteActionCommand", "Select and execute action by number; index appended", "Execute", CommandParameterTypes.ItemIndex},
        {"SetActiveCommand", "Set action active", "Play this"},
        {"StopActionCommand", "Stop action", "Stop this"},
        {"SetNextCommand", "Set action as next", "Set next"},
        {"SelectActiveCommand", "Select active action in the playlist", "Select active"},
        {"SelectNextCommand", "Select next action in the playlist", "Select next"},
 _
        {"LoadPlaylistCommand", "Load playlist", "Load list"},
        {"SavePlaylistCommand", "Save playlist in a new file", "Save list"},
        {"NewPlaylistCommand", "Create a new playlist", "New list"},
        {"AddFilesCommand", "Add file(s) to playlist", "Add file"},
        {"AddAutoVolumeCommand", "Add a volume automation to playlist", "Add volume automation"},
        {"AddCommentCommand", "Add command to playlist", "Add comment"},
        {"AddPowerPointCommand", "Add slide control to playlist", "Add slide"},
        {"AddTextCommand", "Add text control to playlist", "Add text"},
        {"DeleteFileCommand", "Delete selected action", "Delete this"},
        {"ListItemUpCommand", "Move action up the playlist", "Move up"},
        {"ListItemDownCommand", "Move action down the playlist", "Move down"},
 _
        {"RelativeVolumeUpCommand", "Decrease volume of all actions but this", "Emphasize up"},
        {"RelativeVolumeDownCommand", "Increase volume of all actions but this", "Emphasize down"},
        {"PlaySampleCommand", "Play test sound from the selected channel", "Play sample"},
        {"MaxVolumeCommand", "Set volume to the maximum before clipping occurs", "Volume maximum"},
        {"VolumeUpCommand", "Increase volume of this action", "Volume up"},
        {"VolumeDownCommand", "Decrease volume of this action", "Volume down"},
        {"PanningLeftCommand", "Pan the sound a bit to the left", "Panning left"},
        {"PanningRightCommand", "Pan the sound a bit to the right", "Panning right"},
        {"CoordinateXDownCommand", "Move the sound location to the left", "Position left"},
        {"CoordinateXUpCommand", "Move the sound location to the right", "Position right"},
        {"CoordinateYDownCommand", "Move the sound location forward", "Position forward"},
        {"CoordinateYUpCommand", "Move the sound location backwards", "Position back"},
        {"CoordinateZDownCommand", "Move the sound location up", "Position down"},
        {"CoordinateZUpCommand", "Move the sound location down", "Position up"},
 _
        {"ReplaceFileCommand", "Replace file in the action", "Replace file"},
        {"UpdateFileCommand", "Update file information", "Update file"},
        {"ShowSettingsCommand", "Show settings dialog", "Show settings"},
        {"OpenPptCommand", "Open presentation in PowerPoint", "Open power point"},
        {"ShowTextWindowCommand", "Show or hide text", "Show text"},
        {"PrintCommand", "Show print dialog", "Show print"},
        {"ShowAboutCommand", "Show about dialog", "Show about"},
        {"CloseWindowCommand", "Accept changes and close a dialog", "Close dialog"},
        {"CancelWindowCommand", "Reject changes and close a dialog", "Cancel dialog"}
    }
#Enable Warning CC0108 ' You should use nameof instead of the parameter element name string


    ''' <summary>
    ''' Get command description.
    ''' </summary>
    ''' <param name="cmdName">System-used command name (e.g. StopCommand)</param>
    Public Function GetCommandDescription(cmdName As String) As VoiceCommandDescription
        Return AppCommandList.GetDescription(cmdName)
    End Function

End Module
