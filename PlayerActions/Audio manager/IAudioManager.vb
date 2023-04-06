Imports AudioPlayerLibrary


''' <summary>
''' API for controlling the playback of the playlist actions.
''' </summary>
Public Interface IAudioManager

#Region " Playback events "

    ''' <summary>
    ''' The given action has started playback.
    ''' </summary>
    Event PlaybackStarted(sender As IAudioManager, action As IPlayerAction)


    ''' <summary>
    ''' The given action has stopped or paused playback.
    ''' </summary>
    Event PlaybackEnded(sender As IAudioManager, action As IPlayerAction)


    ''' <summary>
    ''' Playlist state is changed - such as what is currently playing and what's next.
    ''' </summary>
    Event StateChanged(sender As IAudioManager, state As PlaylistState)


    ''' <summary>
    ''' Playlist time is changed.
    ''' </summary>
    Event PlaybackTimeChanged(sender As IAudioManager, time As TimeSpan)

#End Region


#Region " Properties "

    ReadOnly Property IsPlaying As Boolean

    ReadOnly Property IsActive As Boolean

    ReadOnly Property IsManuallyPaused As Boolean

#End Region


#Region " Structure API "

    Sub SetPlaylist(value As PlayerActionCollection)

#End Region


#Region " Playback API "

    ''' <summary>
    ''' Reset the list state.
    ''' </summary>
    Sub Reset()


    ''' <summary>
    ''' Request playing the given action on the timeline defined by the item.
    ''' </summary>
    Sub Play(item As IPlayerAction, interrupt As ExecutionTypes)


    ''' <summary>
    ''' Start the playlist without actually playing anything at the moment.
    ''' </summary>
    Sub StartWaiting()


    ''' <summary>
    ''' Request stopping the given parallel action.
    ''' </summary>
    Sub StopSingle(item As IPlayerAction)


    ''' <summary>
    ''' Pause all playback (stops the parallel timelines).
    ''' </summary>
    Sub PauseAll()


    ''' <summary>
    ''' Resume playback on the main timeline.
    ''' </summary>
    Sub ResumeMain()

#End Region

End Interface
