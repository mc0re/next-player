Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports AudioChannelLibrary
Imports AudioPlayerLibrary
Imports Common


<TemplatePart(Name:="PART_Scroller", Type:=GetType(TextBox))>
Public Class MessageLogControl
    Inherits Control
    Implements IMessageLog

    <Flags>
    Private Enum LogDestinations
        MessageBox = 1
        Speech = 2
        All = 3
    End Enum


#Region " Fields "

    ''' <summary>
    ''' Last output line, to avoid obvious duplicates.
    ''' </summary>
    Private mLastLine As String


    Private mScroller As TextBox

#End Region


#Region " OfflineText property "

    Public Property OfflineText As String

#End Region


#Region " VoiceConfig property "

    Private mVoiceConfig As IVoiceConfiguration


    Public ReadOnly Property VoiceConfig As IVoiceConfiguration
        Get
            If mVoiceConfig Is Nothing Then
                mVoiceConfig = InterfaceMapper.GetImplementation(Of IVoiceConfiguration)()
            End If

            Return mVoiceConfig
        End Get
    End Property

#End Region


#Region " Speaker property "

    Private mSpeaker As ISpeechSynthesizer


    Public ReadOnly Property Speaker As ISpeechSynthesizer
        Get
            If mSpeaker Is Nothing Then
                mSpeaker = InterfaceMapper.GetImplementation(Of ISpeechSynthesizer)()
            End If

            Return mSpeaker
        End Get
    End Property

#End Region


#Region " FileCacheSize read-only dependency property "

    Private Shared ReadOnly FileCacheSizePropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(FileCacheSize), GetType(Integer), GetType(MessageLogControl), New PropertyMetadata(0))


    Public Shared ReadOnly FileCacheSizeProperty As DependencyProperty = FileCacheSizePropertyKey.DependencyProperty


    <Category("Common Properties"), Description("Current file cache size, in bytes")>
    Public Property FileCacheSize As Integer
        Get
            Return CInt(GetValue(FileCacheSizeProperty))
        End Get
        Private Set(value As Integer)
            SetValue(FileCacheSizePropertyKey, value)
        End Set
    End Property

#End Region


#Region " SampleCacheSize read-only dependency property "

    Private Shared ReadOnly SampleCacheSizePropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(SampleCacheSize), GetType(Integer), GetType(MessageLogControl), New PropertyMetadata(0))


    Public Shared ReadOnly SampleCacheSizeProperty As DependencyProperty = SampleCacheSizePropertyKey.DependencyProperty


    <Category("Common Properties"), Description("Current sample cache size, in bytes")>
    Public Property SampleCacheSize As Integer
        Get
            Return CInt(GetValue(SampleCacheSizeProperty))
        End Get
        Private Set(value As Integer)
            SetValue(SampleCacheSizePropertyKey, value)
        End Set
    End Property

#End Region


#Region " FigureCacheSize read-only dependency property "

    Private Shared ReadOnly FigureCacheSizePropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(FigureCacheSize), GetType(Long), GetType(MessageLogControl), New PropertyMetadata(0L))


    Public Shared ReadOnly FigureCacheSizeProperty As DependencyProperty = FigureCacheSizePropertyKey.DependencyProperty


    <Category("Common Properties"), Description("Current Geometry figure cache size, in bytes")>
    Public Property FigureCacheSize As Long
        Get
            Return CLng(GetValue(FigureCacheSizeProperty))
        End Get
        Private Set(value As Long)
            SetValue(FigureCacheSizePropertyKey, value)
        End Set
    End Property

#End Region


#Region " TriggerCount read-only dependency property "

    Private Shared ReadOnly TriggerCountPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(TriggerCount), GetType(Integer), GetType(MessageLogControl), New PropertyMetadata(0))


    Public Shared ReadOnly TriggerCountProperty As DependencyProperty = TriggerCountPropertyKey.DependencyProperty


    <Category("Common Properties"), Description("Amount of set triggers")>
    Public Property TriggerCount As Integer
        Get
            Return CInt(GetValue(TriggerCountProperty))
        End Get
        Private Set(value As Integer)
            SetValue(TriggerCountPropertyKey, value)
        End Set
    End Property

#End Region


#Region " NextTriggerList read-only dependency property "

    Private Shared ReadOnly NextTriggerListPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(NextTriggerList), GetType(ObservableCollection(Of TriggerSummary)), GetType(MessageLogControl),
        New PropertyMetadata(New ObservableCollection(Of TriggerSummary)()))


    Public Shared ReadOnly NextTriggerListProperty As DependencyProperty = NextTriggerListPropertyKey.DependencyProperty


    <Category("Common Properties"), Description("A list of set triggers")>
    Public Property NextTriggerList As ObservableCollection(Of TriggerSummary)
        Get
            Return CType(GetValue(NextTriggerListProperty), ObservableCollection(Of TriggerSummary))
        End Get
        Private Set(value As ObservableCollection(Of TriggerSummary))
            SetValue(NextTriggerListPropertyKey, value)
        End Set
    End Property

#End Region


#Region " NextTriggerName read-only dependency property "

    Private Shared ReadOnly NextTriggerNamePropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(NextTriggerName), GetType(String), GetType(MessageLogControl), New PropertyMetadata(String.Empty))


    Public Shared ReadOnly NextTriggerNameProperty As DependencyProperty = NextTriggerNamePropertyKey.DependencyProperty


    <Category("Common Properties"), Description("Name of the next triggered action")>
    Public Property NextTriggerName As String
        Get
            Return CStr(GetValue(NextTriggerNameProperty))
        End Get
        Private Set(value As String)
            SetValue(NextTriggerNamePropertyKey, value)
        End Set
    End Property

#End Region


#Region " IsDurationBusy read-only dependency property "

    Private Shared ReadOnly IsDurationBusyPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(IsDurationBusy), GetType(Boolean), GetType(MessageLogControl), New PropertyMetadata(False))


    Public Shared ReadOnly IsDurationBusyProperty As DependencyProperty = IsDurationBusyPropertyKey.DependencyProperty


    <Category("Common Properties"), Description("Whether duration calculation is ongoing")>
    Public Property IsDurationBusy As Boolean
        Get
            Return CBool(GetValue(IsDurationBusyProperty))
        End Get
        Private Set(value As Boolean)
            SetValue(IsDurationBusyPropertyKey, value)
        End Set
    End Property

#End Region


#Region " IsWaveformBusy read-only dependency property "

    Private mWaveformWorkerCount As Integer


    Private Shared ReadOnly IsWaveformBusyPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(IsWaveformBusy), GetType(Boolean), GetType(MessageLogControl), New PropertyMetadata(False))


    Public Shared ReadOnly IsWaveformBusyProperty As DependencyProperty = IsWaveformBusyPropertyKey.DependencyProperty


    <Category("Common Properties"), Description("Whether waveform calculation is ongoing")>
    Public Property IsWaveformBusy As Boolean
        Get
            Return CBool(GetValue(IsWaveformBusyProperty))
        End Get
        Private Set(value As Boolean)
            If IsWaveformBusy = value Then Return
            SetValue(IsWaveformBusyPropertyKey, value)
        End Set
    End Property

#End Region


#Region " Text read-only dependency property "

    Private Shared ReadOnly TextPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(Text), GetType(String), GetType(MessageLogControl), New PropertyMetadata(""))


    Public Shared ReadOnly TextProperty As DependencyProperty = TextPropertyKey.DependencyProperty


    <Category("Common Properties"), Description("Log as full text")>
    Public Property Text As String
        Get
            Return CStr(GetValue(TextProperty))
        End Get
        Private Set(value As String)
            If Text = value Then Return
            SetValue(TextPropertyKey, value)
        End Set
    End Property

#End Region


#Region " ClearLog command "

    Public ReadOnly Property ClearLogCommand As New DelegateCommand(AddressOf ClearLogCommandExecuted)


    Private Sub ClearLogCommandExecuted(param As Object)
        Text = String.Empty
    End Sub

#End Region


#Region " Init and clean-up "

    Private Sub LoadedHandler() Handles Me.Loaded
        Text = OfflineText
        mScroller = CType(Template.FindName("PART_Scroller", Me), TextBox)
    End Sub

#End Region


#Region " IMessageLog implementation "

    Public Sub ClearLog(reason As String, shortReason As String) Implements IMessageLog.ClearLog
        AddText(LogDestinations.MessageBox, "--- " & reason)
        AddText(LogDestinations.Speech, shortReason)
    End Sub


    Public Sub LogKeyError(format As String, ParamArray args() As Object) Implements IMessageLog.LogKeyError
        AddText(format, args)
    End Sub


    Public Sub LogFileError(format As String, ParamArray args() As Object) Implements IMessageLog.LogFileError
        AddText(format, args)
    End Sub


    Public Sub LogAudioError(format As String, ParamArray args() As Object) Implements IMessageLog.LogAudioError
        AddText(format, args)
    End Sub


    Public Sub LogLicenseWarning(format As String, ParamArray args() As Object) Implements IMessageLog.LogLicenseWarning
        AddText(format, args)
    End Sub


    Public Sub LogVoiceInfo(format As String, ParamArray args() As Object) Implements IMessageLog.LogVoiceInfo
        AddText(format, args)
    End Sub


    ''' <summary>
    ''' As this method executes in another thread, send a thread-safe copy of the list.
    ''' </summary>
    Public Sub LogTriggerInfo(triggerList As IEnumerable(Of TriggerSummary)) Implements IMessageLog.LogTriggerInfo
        Dispatcher.BeginInvoke(
            Sub()
                Dim tlst = triggerList.ToList()
                TriggerCount = tlst.Count
                Dim lst = NextTriggerList
                lst.Clear()

                If TriggerCount > 0 Then
                    NextTriggerName = tlst.First().NextAction
                    For Each tr In tlst
                        lst.Add(tr)
                    Next
                End If
            End Sub)
    End Sub


    Public Sub LogDurationWork(ByVal working As Boolean) Implements IMessageLog.LogDurationWork
        Dispatcher.BeginInvoke(Sub() IsDurationBusy = working)
    End Sub


    Public Sub LogWaveformWork(ByVal working As Boolean) Implements IMessageLog.LogWaveformWork
        Dispatcher.BeginInvoke(
            Sub()
                mWaveformWorkerCount += If(working, 1, -1)
                IsWaveformBusy = (mWaveformWorkerCount > 0)
            End Sub)
    End Sub


    Public Sub LogTriggerMessage(format As String, ParamArray args() As Object) Implements IMessageLog.LogTriggerMessage
        AddText(format, args)
    End Sub


    Public Sub LogPowerPointError(format As String, ParamArray args() As Object) Implements IMessageLog.LogPowerPointError
        AddText(format, args)
    End Sub


    Public Sub LogFileCacheInfo(size As Integer) Implements IMessageLog.LogFileCacheInfo
        Dispatcher.BeginInvoke(Sub() FileCacheSize = size)
    End Sub


    Public Sub LogSampleCacheInfo(size As Integer) Implements IMessageLog.LogSampleCacheInfo
        Dispatcher.BeginInvoke(Sub() SampleCacheSize = size)
    End Sub


    Public Sub LogFigureCacheInfo(size As Long) Implements IMessageLog.LogFigureCacheInfo
        Dispatcher.BeginInvoke(Sub() FigureCacheSize = size)
    End Sub

#End Region


#Region " Utility "

    ''' <summary>
    ''' Post the text onto the UI and to the voice feedback.
    ''' </summary>
    Private Sub AddText(format As String, ParamArray args() As Object)
        AddText(LogDestinations.All, format, args)
    End Sub


    ''' <summary>
    ''' Post the text onto the UI and/or to the voice feedback, depending on <paramref name="mode"/>.
    ''' </summary>
    Private Sub AddText(mode As LogDestinations, format As String, ParamArray args() As Object)
        Dim str = String.Format(format, args)
        If str = mLastLine Then Return

        mLastLine = str

        If mode.HasFlag(LogDestinations.MessageBox) Then
            Dispatcher.BeginInvoke(
            Sub()
                If IsLoaded Then
                    Text = Text & str & vbCrLf
                    mScroller?.ScrollToEnd()
                Else
                    OfflineText = OfflineText & str & vbCrLf
                End If
            End Sub)
        End If

        If mode.HasFlag(LogDestinations.Speech) Then
            Speaker?.Speak(str)
        End If
    End Sub

#End Region

End Class
