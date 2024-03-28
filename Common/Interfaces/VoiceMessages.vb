Public Enum VoiceMessages

    ''' <summary>
    ''' Parameter: inner exception message.
    ''' </summary>
    ErrorInStartListening

    ''' <summary>
    ''' Parameter: inner exception message.
    ''' </summary>
    ErrorInGrammar

    ''' <summary>
    ''' Voice command is not found.
    ''' Parameter: recognized command.
    ''' </summary>
    CommandNotFound

    ''' <summary>
    ''' Parameters: none.
    ''' </summary>
    RecognitionStarted

    ''' <summary>
    ''' Parameters: none.
    ''' </summary>
    RecognitionUpdated

    ''' <summary>
    ''' Parameters: command and confidence level.
    ''' </summary>
    CommandRecognized

    ''' <summary>
    ''' Parameters: command.
    ''' </summary>
    CommandNotInList

    ''' <summary>
    ''' Parameters: command and confidence level.
    ''' </summary>
    CommandRejected

    ''' <summary>
    ''' Parameter: a string with commands.
    ''' </summary>
    YieldCommandList
End Enum
