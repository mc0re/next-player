Imports Common


Friend Class TestTimeService
    Implements ITimeService

    Private Shared ReadOnly BaseDate As Date = DateTimeOffset.FromUnixTimeMilliseconds(0).UtcDateTime

    Public Shared Property CurrentTime As Double


    ''' <inheritdoc/>
    Public Function GetCurrentTime() As Double Implements ITimeService.GetCurrentTime
        Return CurrentTime
    End Function


    ''' <inheritdoc/>
    Public Function ToDateTime(millis As Double) As Date Implements ITimeService.ToDateTime
        Return DateTimeOffset.FromUnixTimeMilliseconds(CLng(millis)).UtcDateTime
    End Function


    Public Function GetTime(ts As Date) As Double Implements ITimeService.GetTime
        Return GetTime(ts.Year, ts.Month, ts.Day, ts.Hour, ts.Minute, ts.Second)
    End Function


    Public Function GetTime(year As Integer, month As Integer, day As Integer, hour As Integer, minute As Integer, second As Integer) As Double
        Return (New Date(year, month, day, hour, minute, second) - BaseDate).TotalMilliseconds
    End Function

End Class
