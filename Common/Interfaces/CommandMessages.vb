Public Enum CommandMessages

    ''' <summary>
    ''' Volume was changed.
    ''' Parameter: new volume.
    ''' </summary>
    VolumeSet

    ''' <summary>
    ''' Panniong was changed.
    ''' Parameter: new panning.
    ''' </summary>
    PanningSet

    ''' <summary>
    ''' X coordinate was changed,
    ''' Parameter: new X coordinate,
    ''' </summary>
    CoordinateXSet

    ''' <summary>
    ''' Y coordinate was changed,
    ''' Parameter: new Y coordinate,
    ''' </summary>
    CoordinateYSet

    ''' <summary>
    ''' Z coordinate was changed,
    ''' Parameter: new Z coordinate,
    ''' </summary>
    CoordinateZSet

    ''' <summary>
    ''' Playback has started.
    ''' Parameter: action name.
    ''' </summary>
    Started

    ''' <summary>
    ''' Playback has stopped.
    ''' Parameter: action name.
    ''' </summary>
    Stopped

    ''' <summary>
    ''' Start in passive (waiting) mode.
    ''' </summary>
    StartPassive

    ''' <summary>
    ''' Playback has stopped for all items..
    ''' </summary>
    StoppedAll

    ''' <summary>
    ''' A list item is selected.
    ''' Parameters: index, name.
    ''' </summary>
    Selected

End Enum
