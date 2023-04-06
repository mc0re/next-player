Public Structure TimerTickResult

    ''' <summary>
    ''' Time since the playback has started.
    ''' </summary>
    Public Property PlaybackTime As Double


    ''' <summary>
    ''' Whether there are new actions to perform.
    ''' </summary>
    Public Property HasActions As Boolean


    ''' <summary>
    ''' Actions to perform.
    ''' </summary>
    Public Property Actions As AudioActions

End Structure
