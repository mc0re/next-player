Imports System.Diagnostics.CodeAnalysis
''' <summary>
''' Mini-class for storing the notification information.
''' </summary>
Public Class NotificationInfo

#Region " Properties "

    ''' <summary>
    ''' Action to trigger.
    ''' </summary>
    Public ReadOnly Property Action As IPlayerAction


    ''' <summary>
    ''' THe reference point for the trigger.
    ''' </summary>
    Public ReadOnly Property Trigger As TriggerReference


    ''' <summary>
    ''' When to trigger the event, absolute or playlist time, in milliseconds.
    ''' </summary>
    Public ReadOnly Property Position As Double


    ''' <summary>
    ''' Whether <see cref="Position"/> refers to wall clock or playlist clock.
    ''' </summary>
    Public ReadOnly Property IsAbsolute As Boolean


    ''' <summary>
    ''' Whether the action has been triggered (so it only happens once).
    ''' </summary>
    Public Property IsTriggered As Boolean

#End Region


#Region " Init and clean-up "

    Public Sub New(trigger As TriggerReference, position As Double, isAbs As Boolean, action As IPlayerAction)
        Me.Trigger = trigger
        Me.Position = position
        IsAbsolute = isAbs
        Me.Action = action
    End Sub

#End Region


#Region " API "

    ''' <summary>
    ''' Whether the notification is scheduled after the given time.
    ''' </summary>
    ''' <param name="wallTick">Absolute (wall clock) time</param>
    ''' <param name="playTick">Playlist time</param>
    Public Function IsAfter(wallTick As Double, playTick As Double) As Boolean
        Return If(IsAbsolute, Position > wallTick, Position > playTick)
    End Function


    ''' <summary>
    ''' Update trigger timer.
    ''' </summary>
    ''' <param name="startTime">New trigger time</param>
    Public Sub UpdatePosition(startTime As Double)
        _Position = startTime
    End Sub


    ''' <summary>
    ''' For debugging.
    ''' </summary>
    <ExcludeFromCodeCoverage>
    Public Overrides Function ToString() As String
        Return String.Format("{0} {1} {2}: {3}",
                             If(IsTriggered, "!"c, "-"c),
                             If(IsAbsolute, "W", "P"),
                             Position, Action)
    End Function

#End Region

End Class
