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


    ''' <summary>
    ''' Which flags are applied for the command.
    ''' </summary>
    <IgnoreForReport()>
    Public Property Flags As CommandFlags


#Region " ToString "

    Public Overrides Function ToString() As String
        Return String.Format("{0} [{1}]", CommandName, DefaultText)
    End Function

#End Region

End Class
