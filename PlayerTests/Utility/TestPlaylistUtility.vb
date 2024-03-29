﻿Imports AudioPlayerLibrary

Public Class TestPlaylistUtility

    ''' <summary>
    ''' Create a list of actions according to the definition in <paramref name="def"/>.
    ''' </summary>
    ''' <param name="def">
    ''' Space-separated list of actions.
    ''' 
    ''' The first letter defines the action type:
    ''' <list type="bullet">
    '''   <item><term>P</term> <description>main line producer</description></item>
    '''   <item><term>E</term> <description>main line effect</description></item>
    '''   <item><term>p</term> <description>parallel line producer</description></item>
    '''   <item><term>e</term> <description>parallel line effect</description></item>
    ''' </list>
    ''' 
    ''' <para>
    ''' Then follows the action name until <c>-</c> or space.
    ''' 
    ''' </para>
    ''' If present, the next section (after <c>-</c>) defines the delay type. Absent assumes manual.
    ''' The first letter can be:
    ''' <list type="bullet">
    '''   <item><term>S</term> <description>start of the last producer (default delay = 0)</description></item>
    '''   <item><term>E</term> <description>end of the last producer (default delay = 0)</description></item>
    '''   <item><term>s</term> <description>start of the last action (default delay = 0)</description></item>
    '''   <item><term>e</term> <description>end of the last action (default delay = 0)</description></item>
    '''   <item><term>p</term> <description>start with playlist clock (at 1 hour)</description></item>
    '''   <item><term>w</term> <description>start with wall clock (at 12:00)</description></item>
    '''   <item><term>m</term> <description>manual start</description></item>
    ''' </list>
    ''' 
    ''' If present, the next number is delay in seconds.
    ''' </param>
    ''' <remarks>
    ''' All actions have a duration. It starts with 10 seconds and doubles for each consequent action.
    ''' </remarks>
    Public Shared Function CreatePlaylist(def As String) As List(Of IPlayerAction)
        Dim res = New List(Of IPlayerAction)()
        Dim duration = TimeSpan.FromSeconds(10)

        For Each itemDef In def.Split(" "c)
            Dim nameEtc = itemDef.Substring(1).Split("-".ToCharArray(), 2) ' The delay can be negative
            Dim name = nameEtc(0)
            Dim ref = If(nameEtc.Length > 1, nameEtc(1), "m")

            Dim dt As DelayTypes
            Dim dr As DelayReferences
            Dim delay = If(ref.Length = 1, TimeSpan.Zero, TimeSpan.FromSeconds(Integer.Parse(ref.Substring(1))))

            Select Case ref(0)
                Case "m"c : dt = DelayTypes.Manual
                Case "S"c : dt = DelayTypes.TimedFromStart : dr = DelayReferences.LastProducer
                Case "E"c : dt = DelayTypes.TimedAfterEnd : dr = DelayReferences.LastProducer
                Case "s"c : dt = DelayTypes.TimedFromStart : dr = DelayReferences.LastAction
                Case "e"c : dt = DelayTypes.TimedAfterEnd : dr = DelayReferences.LastAction
                Case "p"c : dt = DelayTypes.TimedFromStart : dr = DelayReferences.MainClock : delay = TimeSpan.FromHours(1)
                Case "w"c : dt = DelayTypes.TimedFromStart : dr = DelayReferences.WallClock : delay = TimeSpan.FromHours(12)
            End Select


            Select Case itemDef(0)
                Case "P"c
                    res.Add(New TestActionProducer With {
                            .Name = name, .ExecutionType = ExecutionTypes.MainContinuePrev,
                            .HasDuration = True, .Duration = duration,
                            .DelayType = dt, .DelayReference = dr, .DelayBefore = delay})

                Case "p"c
                    res.Add(New TestActionProducer With {
                            .Name = name, .ExecutionType = ExecutionTypes.Parallel,
                            .HasDuration = True, .Duration = duration,
                            .DelayType = dt, .DelayReference = dr, .DelayBefore = delay})

                Case "E"c
                    res.Add(New TestActionEffect With {
                            .Name = name, .ExecutionType = ExecutionTypes.MainContinuePrev,
                            .HasDuration = True, .Duration = duration,
                            .DelayType = dt, .DelayReference = dr, .DelayBefore = delay})

                Case "e"c
                    res.Add(New TestActionEffect With {
                            .Name = name, .ExecutionType = ExecutionTypes.Parallel,
                            .HasDuration = True, .Duration = duration,
                            .DelayType = dt, .DelayReference = dr, .DelayBefore = delay})
            End Select

            duration = TimeSpan.FromSeconds(duration.TotalSeconds * 2)
        Next

        Return res
    End Function

End Class
