Imports Common


Public Class TestLogger
    Implements IMessageLog

    Public Sub ClearLog(reason As String) Implements IMessageLog.ClearLog

    End Sub

    Public Sub LogKeyError(format As String, ParamArray args() As Object) Implements IMessageLog.LogKeyError

    End Sub

    Public Sub LogLicenseWarning(format As String, ParamArray args() As Object) Implements IMessageLog.LogLicenseWarning

    End Sub

    Public Sub LogLoadingError(format As String, ParamArray args() As Object) Implements IMessageLog.LogLoadingError

    End Sub

    Public Sub LogAudioError(format As String, ParamArray args() As Object) Implements IMessageLog.LogAudioError

    End Sub

    Public Sub LogPowerPointError(format As String, ParamArray args() As Object) Implements IMessageLog.LogPowerPointError

    End Sub

    Public Sub LogTriggerInfo(triggerList As IEnumerable(Of TriggerSummary)) Implements IMessageLog.LogTriggerInfo

    End Sub

    Public Sub LogDurationWork(ByVal working As Boolean) Implements IMessageLog.LogDurationWork

    End Sub

    Public Sub LogWaveformWork(ByVal working As Boolean) Implements IMessageLog.LogWaveformWork

    End Sub

    Public Sub LogTriggerMessage(format As String, ParamArray args() As Object) Implements IMessageLog.LogTriggerMessage

    End Sub

    Public Sub LogVoiceInfo(format As String, ParamArray args() As Object) Implements IMessageLog.LogVoiceInfo

    End Sub

    Public Sub LogFileCacheInfo(size As Integer) Implements IMessageLog.LogFileCacheInfo

    End Sub

    Public Sub LogSampleCacheInfo(size As Integer) Implements IMessageLog.LogSampleCacheInfo

    End Sub

    Public Sub LogFigureCacheInfo(size As Long) Implements IMessageLog.LogFigureCacheInfo

    End Sub

End Class
