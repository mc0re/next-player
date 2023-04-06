''' <summary>
''' Type of delay between actions.
''' </summary>
Public Enum DelayTypes

    ''' <summary>
    ''' The user must press a key to start the action.
    ''' </summary>
    Manual


    ''' <summary>
	''' The action starts automatically after a certain delay after the main action start.
	''' </summary>
	TimedFromStart


	''' <summary>
	''' The action starts automatically for a certain delay before the main action end.
	''' </summary>
	TimedBeforeEnd


	''' <summary>
	''' The action starts automatically after a certain delay after the main action end.
	''' </summary>
	TimedAfterEnd

End Enum
