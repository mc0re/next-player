Public Interface IPlaylistAction

    ''' <summary>
    ''' Display name.
    ''' </summary>
    Property Name As String


    ''' <summary>
    ''' Start or resume the sound producing / automation.
    ''' </summary>
    Sub Start()


    ''' <summary>
    ''' Stop producing the sound / automation and any timers.
    ''' </summary>
    ''' <param name="intendedResume">Whether a resume might be expected. If not, shut down.</param>
    Sub [Stop](intendedResume As Boolean)

End Interface
