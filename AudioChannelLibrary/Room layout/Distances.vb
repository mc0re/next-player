Public Class Distances

    ''' <summary>
    ''' Whether the channel's output shall be used.
    ''' </summary>
    ''' <remarks>It shouldn't when the channel is on the opposite side</remarks>
    Public Property UseChannel As Boolean()


    ''' <summary>
    ''' Distances from the used channels to source projection point.
    ''' </summary>
    Public Property ChToListener As Single()


    ''' <summary>
    ''' Distances from used channels to source projection point,
    ''' as seen by the listener.
    ''' </summary>
    Public Property ChToSourceProj As Single()

End Class
