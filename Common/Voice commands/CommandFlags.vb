''' <summary>
''' Which flags apply to the command.
''' By default it is a combination defined by <see cref="[Default]"/>. 
''' </summary>
Public Enum CommandFlags

    ''' <summary>
    ''' Confirm the command recognition by repeating its name.
    ''' If not set, the command is confirmed by its action.
    ''' </summary>
    Confirm = 1


    ''' <summary>
    ''' Do not show this command in "what can I say" list.
    ''' </summary>
    HideFromList = 2


    ''' <summary>
    ''' Default settings:
    ''' - No confirm
    ''' </summary>
    [Default] = 0

End Enum
