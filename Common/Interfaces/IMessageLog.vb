Public Interface IMessageLog

    ''' <summary>
    ''' Start a new log section, possibly cleaning up the previous entries.
    ''' </summary>
    Sub ClearLog(reason As String, shortReason As String)


    ''' <summary>
    ''' Report current file cache size (in bytes).
    ''' </summary>
    Sub LogFileCacheInfo(size As Integer)


    ''' <summary>
    ''' Report current sample cache size (in bytes).
    ''' </summary>
    Sub LogSampleCacheInfo(size As Integer)


    ''' <summary>
    ''' Report current figure cache size (in bytes).
    ''' </summary>
    Sub LogFigureCacheInfo(size As Long)


    ''' <summary>
    ''' Report auto-trigger information.
    ''' </summary>
    Sub LogTriggerInfo(triggerList As IEnumerable(Of TriggerSummary))


    ''' <summary>
    ''' Whether the duration library is working.
    ''' </summary>
    Sub LogDurationWork(working As Boolean)


    ''' <summary>
    ''' Whether the waveform library is working.
    ''' There can be multiple threads sending these notifications,
    ''' so a counter is needed.
    ''' </summary>
    Sub LogWaveformWork(working As Boolean)


    Sub LogTriggerMessage(format As String, ParamArray args() As Object)

    Sub LogVoiceInfo(message As VoiceMessages, ParamArray args() As Object)

    Sub LogKeyError(format As String, ParamArray args() As Object)

    Sub LogLicenseWarning(format As String, ParamArray args() As Object)


    ''' <summary>
    ''' Report a file loading or saving error.
    ''' </summary>
    ''' <param name="format">Message string containing references to <paramref name="args"/> like <c>{0}</c></param>
    ''' <param name="args">Arguments to the message string</param>
    Sub LogFileError(format As String, ParamArray args() As Object)

    Sub LogAudioError(format As String, ParamArray args() As Object)

    Sub LogPowerPointError(format As String, ParamArray args() As Object)


    ''' <summary>
    ''' Report that a command is executed successfully. Pass on parameters if needed.
    ''' </summary>
    Sub LogCommandExecuted(message As CommandMessages, ParamArray args() As Object)

End Interface
