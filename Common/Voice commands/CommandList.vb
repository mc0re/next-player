Public Module CommandList

#Disable Warning CC0108 ' You should use nameof instead of the parameter element name string
    <CodeAnalysis.SuppressMessage("Design", "CC0021:You should use nameof instead of the parameter element name string",
                                  Justification:="These are string constants, not names")>
    Public ReadOnly AppCommandList As New VoiceCommandDescriptionGroup() From {
        {"Playlist control", New VoiceCommandDescriptionCollection() From {
            {"PlayNextCommand", "Play next action", "Next"},
            {"PlayAgainCommand", "Replay the active action", "Replay"},
            {"ResetPlaylistCommand", "Reset playlist position", "Reset playlist"},
            {"StartPlaylistCommand", "Start playlist in passive mode", "Start passive"},
            {"StopCommand", "Stop all actions", "Stop"},
            {"ResumeCommand", "Resume stopped main actions", "Resume"},
            {"PlayStopParallelCommand", "Play and stop parallel action; index appended", "", CommandParameterTypes.ParallelIndex}
        }},
        {"Item control", New VoiceCommandDescriptionCollection() From {
            {"ExecuteActionCommand", "Select and execute action by number; index appended", "Execute", CommandParameterTypes.ItemIndex},
            {"SelectActionCommand", "Select action by number; index appended", "Select", CommandParameterTypes.ItemIndex},
            {"SetActiveCommand", "Set action active", "Play this"},
            {"StopActionCommand", "Stop action", "Stop this"},
            {"SetNextCommand", "Set action as next", "Set next", CommandFlags.Confirm},
            {"SelectActiveCommand", "Select active action in the playlist", "Select active", CommandFlags.Confirm},
            {"SelectNextCommand", "Select 'next' action in the playlist", "Select next", CommandFlags.Confirm}
        }},
        {"Item editing", New VoiceCommandDescriptionCollection() From {
            {"RelativeVolumeUpCommand", "Decrease volume of all actions but this", "Emphasize up", CommandFlags.Confirm},
            {"RelativeVolumeDownCommand", "Increase volume of all actions but this", "Emphasize down", CommandFlags.Confirm},
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
            {"UpdateFileCommand", "Update (reload) file information", "Update file", CommandFlags.Confirm}
        }},
        {"Playlist editing", New VoiceCommandDescriptionCollection() From {
            {"ReplaceFileCommand", "Replace file in the action", "Replace file"},
            {"LoadPlaylistCommand", "Load playlist", "Load list"},
            {"SavePlaylistCommand", "Save playlist in a new file", "Save list"},
            {"NewPlaylistCommand", "Create a new playlist", "New list", CommandFlags.Confirm},
            {"AddFilesCommand", "Add file(s) to playlist", "Add file"},
            {"AddAutoVolumeCommand", "Add a volume automation to playlist", "Add volume automation"},
            {"AddCommentCommand", "Add command to playlist", "Add comment"},
            {"AddPowerPointCommand", "Add slide control to playlist", "Add slide"},
            {"AddTextCommand", "Add text control to playlist", "Add text"},
            {"DeleteFileCommand", "Delete selected action", "Delete this"},
            {"ListItemUpCommand", "Move action up the playlist", "Move up", CommandFlags.Confirm},
            {"ListItemDownCommand", "Move action down the playlist", "Move down", CommandFlags.Confirm}
        }},
        {"System control", New VoiceCommandDescriptionCollection() From {
            {"WhatCanISayCommand", "List available voice commands", "What can I say", CommandFlags.HideFromList},
            {"ListTriggersCommand", "List active triggers", "List triggers"},
            {"PlaySampleCommand", "Play test sound from the selected channel", "Play sample"},
            {"TestVoiceFeedbackCommand", "Play test sound for voice feedback", "Test voice"},
            {"ShowSettingsCommand", "Show settings dialog", "Show settings", CommandFlags.Confirm},
            {"OpenPptCommand", "Open presentation in PowerPoint", "Open power point", CommandFlags.Confirm},
            {"ShowTextWindowCommand", "Show or hide text", "Show text", CommandFlags.Confirm},
            {"PrintCommand", "Show print dialog", "Show print"},
            {"ShowAboutCommand", "Show about dialog", "Show about"},
            {"CloseWindowCommand", "Accept changes and close a dialog", "Close dialog"},
            {"CancelWindowCommand", "Reject changes and close a dialog", "Cancel dialog"}
        }}
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
