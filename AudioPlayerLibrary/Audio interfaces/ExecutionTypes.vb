''' <summary>
''' How to execute an action.
''' </summary>
Public Enum ExecutionTypes

    ''' <summary>
    ''' Main line, stop all main-line previous actions.
    ''' </summary>
    MainStopPrev

    ''' <summary>
    ''' Main line, stop all previous actions.
    ''' </summary>
    MainStopAll

    ''' <summary>
    ''' Main line, continue all previous actions.
    ''' </summary>
    MainContinuePrev

    ''' <summary>
    ''' Main line, cross-fade with previous main. Should be no effects in between them.
    ''' </summary>
    MainCrossFade

    ''' <summary>
    ''' Parallel line, continue all previous actions.
    ''' </summary>
    Parallel

End Enum
