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


#Region " Definition property "

    Public Property Definition As VoiceCommandDescription

#End Region


#Region " ToString "

    Public Overrides Function ToString() As String
        Return Setting.ToString()
    End Function

#End Region

End Class
