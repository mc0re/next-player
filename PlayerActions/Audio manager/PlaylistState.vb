Imports AudioPlayerLibrary


Public Class PlaylistState

    ''' <summary>
    ''' The action on the main line currently producing sound.
    ''' </summary>
    Public Property ActiveMainProducer As IPlayerAction


    ''' <summary>
    ''' The last started action on the main line.
    ''' </summary>
    Public Property ActiveMainAction As IPlayerAction


    ''' <summary>
    ''' The action on the main line producing the sound or produced it last.
    ''' </summary>
    ''' <returns></returns>
    Public Property LastMainProducer As IPlayerAction


    ''' <summary>
    ''' The active parallel action list.
    ''' </summary>
    Public Property ManualParallels As ICollection(Of IPlayerAction)

    ''' <summary>
    ''' The "next" action.
    ''' </summary>
    Public Property NextAction As IPlayerAction

End Class
