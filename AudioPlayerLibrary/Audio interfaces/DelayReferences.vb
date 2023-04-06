Imports System.ComponentModel


''' <summary>
''' What the delay is measured from.
''' </summary>
Public Enum DelayReferences

    <Description("Playlist clock")>
    MainClock

    <Description("Action directly above the item")>
    LastAction

    <Description("Sound producer directly above the item")>
    LastProducer

    <Description("Time of day")>
    WallClock
End Enum
