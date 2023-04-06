''' <summary>
''' Convert dates to and from scalar units, which happen to be milliseconds.
''' </summary>
Public Interface ITimeService

    ''' <summary>
    ''' Convert milliseconds to a <see cref="Date"/>.
    ''' </summary>
    Function ToDateTime(millis As Double) As Date


    ''' <summary>
    ''' Get current time [milliseconds].
    ''' </summary>
    Function GetCurrentTime() As Double


    ''' <summary>
    ''' Get time for the given timestamp.
    ''' </summary>
    ''' <param name="ts"></param>
    ''' <returns></returns>
    Function GetTime(ts As Date) As Double

End Interface
