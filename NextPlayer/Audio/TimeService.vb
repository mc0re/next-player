Imports Common


Public Class TimeService
    Implements ITimeService

#Region " Constants "

    Private Shared ReadOnly BaseDate As Date = DateTimeOffset.FromUnixTimeMilliseconds(0).UtcDateTime

#End Region


#Region " API "

    ''' <inheritdoc/>
    Private Function GetCurrentTime() As Double Implements ITimeService.GetCurrentTime
        Return GetTime(Date.Now)
    End Function


    ''' <inheritdoc/>
    Public Function ToDateTime(millis As Double) As Date Implements ITimeService.ToDateTime
        Return DateTimeOffset.FromUnixTimeMilliseconds(CLng(millis)).UtcDateTime
    End Function


    ''' <inheritdoc/>
    Public Function GetTime(ts As Date) As Double Implements ITimeService.GetTime
        Return (New Date(ts.Year, ts.Month, ts.Day, ts.Hour, ts.Minute, ts.Second) - BaseDate).TotalMilliseconds
    End Function

#End Region

End Class
