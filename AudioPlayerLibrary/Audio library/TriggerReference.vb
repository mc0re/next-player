''' <summary>
''' Used for anchoring automatic delays.
''' </summary>
Public NotInheritable Class TriggerReference

#Region " Properties "

    ''' <summary>
    ''' Whether the time refers to wall clock or playlist clock.
    ''' The playlist can be paused, the wall clock cannot.
    ''' </summary>
    Public ReadOnly Property IsAbsolute As Boolean


    ''' <summary>
    ''' Whether the action was started.
    ''' </summary>
    Public ReadOnly Property IsStarted As Boolean


    ''' <summary>
    ''' When the action was started.
    ''' </summary>
    Public ReadOnly Property StartTime As Double


    ''' <summary>
    ''' Whether the duration is set.
    ''' </summary>
    Public ReadOnly Property HasDuration As Boolean


    ''' <summary>
    ''' When the action will end.
    ''' Only valid if <see cref="HasDuration"/> is true.
    ''' </summary>
    Public ReadOnly Property EndTime As Double


    ''' <summary>
    ''' THe actual action behind the reference.
    ''' </summary>
    Public ReadOnly Property Action As IPlayerAction

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Convenience constructor.
    ''' </summary>
    ''' <param name="isAbsolute">True if the anchor refers to absolute time, False - if playlist time</param>
    Public Sub New(isAbsolute As Boolean)
        Me.IsAbsolute = isAbsolute
    End Sub


    ''' <summary>
    ''' Convenience constructor.
    ''' </summary>
    ''' <param name="act">
    ''' There is a number of checks before this method is called.
    ''' So we assume the given action is correctly identified, and thus is about to start.
    ''' </param>
    Public Sub New(act As IPlayerAction)
        IsAbsolute = False
        IsStarted = True
        StartTime = act.StartTime
        Action = act
        CalculateEndTime(act.StartTime)
    End Sub

#End Region


#Region " API "

    ''' <summary>
    ''' Setup this trigger for the given action.
    ''' </summary>
    Public Sub SetTimeAnchor(currentTime As Double)
        _StartTime = currentTime
        _IsStarted = True
        _HasDuration = False
    End Sub


    ''' <summary>
    ''' Calculate anchor's end time.
    ''' </summary>
    ''' <param name="currentTime">Current, usually playlist, time</param>
    Public Sub CalculateEndTime(currentTime As Double)
        If Action?.HasDuration Then
            Dim leftToPlay = Action.Duration.TotalMilliseconds -
                             Action.StopPosition.TotalMilliseconds -
                             Action.PlayPosition.TotalMilliseconds

            _EndTime = currentTime + leftToPlay
            _HasDuration = True
        Else
            _HasDuration = False
        End If
    End Sub


    ''' <summary>
    ''' Reset this trigger.
    ''' </summary>
    Public Sub ResetAnchor()
        _StartTime = 0
        _IsStarted = False
        _HasDuration = False
    End Sub

#End Region

End Class
