﻿Imports System.Collections.ObjectModel
Imports System.ComponentModel
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
        AddMessage(LogDestinations.MessageBox, "--- " & reason)
        AddMessage(LogDestinations.Speech, shortReason)
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


    Public Sub LogVoiceInfo(message As VoiceMessages, ParamArray args() As Object) Implements IMessageLog.LogVoiceInfo
        Select Case message
            Case VoiceMessages.ErrorInStartListening
                AddTextMessage(String.Format("Error when preparing voice recognition: {0}.", args))
                AddVoiceMessage("Error when preparing voice recognition")

            Case VoiceMessages.ErrorInGrammar
                AddTextMessage(String.Format("Error when building grammar: {0}.", args))
                AddVoiceMessage("Error when building grammar")

            Case VoiceMessages.CommandNotFound
                AddTextMessage(String.Format("Command '{0}' not found.", args))
                AddVoiceMessage(String.Format("Command '{0}' not found.", args))

            Case VoiceMessages.RecognitionStarted
                AddTextMessage("Voice recognition started.")
                AddVoiceMessage("Voice recognition started.")

            Case VoiceMessages.RecognitionStopped
                AddTextMessage("Voice recognition stopped.")
                AddVoiceMessage("Voice recognition stopped.")

            Case VoiceMessages.RecognitionUpdated
                AddTextMessage("Voice commands updated.")
                AddVoiceMessage("Voice commands updated.")

            Case VoiceMessages.CommandRecognized
                AddTextMessage(String.Format("Voice command recognized '{0}', confidence {1:F2}.", args))
                AddVoiceMessage(CStr(args(0)))

            Case VoiceMessages.CommandRecognizedNoConfirmation
                AddTextMessage(String.Format("Voice command recognized '{0}', confidence {1:F2}.", args))

            Case VoiceMessages.CommandNotInList
                AddTextMessage(String.Format("Voice command '{0}' not found in the list.", args))
                AddVoiceMessage(String.Format("'{0}' not found.", args))

            Case VoiceMessages.CommandRejected
                AddTextMessage(String.Format("Voice command '{0}' rejected, confidence {1:F2}.", args))
                AddVoiceMessage(String.Format("'{0}' rejected.", args))

            Case VoiceMessages.YieldCommandList
                AddVoiceMessage(String.Format("Possible commands: {0}", args))

            Case VoiceMessages.YieldTriggerList
                AddVoiceMessage(String.Format("Current triggers: {0}", GenerateTriggersList()))

            Case VoiceMessages.NoItemSelected
                AddVoiceMessage("No item is selected in the playlist, command ignored.")

            Case Else
                AddTextMessage(String.Format("Unknown voice message '{0}'.", message))
        End Select
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


    Public Sub LogCommandExecuted(message As CommandMessages, ParamArray args() As Object) Implements IMessageLog.LogCommandExecuted
        Select Case message
            Case CommandMessages.VolumeSet
                AddVoiceMessage(String.Format("Volume {0:F2}.", args))

            Case CommandMessages.PanningSet
                AddVoiceMessage(String.Format("Panning {0:F2}.", args))

            Case CommandMessages.CoordinateXSet
                AddVoiceMessage(String.Format("X {0:F2}.", args))

            Case CommandMessages.CoordinateYSet
                AddVoiceMessage(String.Format("Y {0:F2}.", args))

            Case CommandMessages.CoordinateZSet
                AddVoiceMessage(String.Format("Z {0:F2}.", args))

            Case CommandMessages.Started
                AddVoiceMessage(String.Format("Playing {0}.", args))

            Case CommandMessages.StartPassive
                AddVoiceMessage("Waiting for command or time.")

            Case CommandMessages.Stopped
                AddVoiceMessage(String.Format("Stopped {0}.", args))

            Case CommandMessages.StoppedAll
                AddVoiceMessage("Playlist stopped.")

            Case CommandMessages.Selected
                AddVoiceMessage(String.Format("Selected {0}, {1}.", args))

            Case Else
                AddTextMessage(String.Format("Unknown command message '{0}'.", message))
        End Select
    End Sub

#End Region


#Region " Utility "

    ''' <summary>
    ''' Post the text onto the UI and to the voice feedback.
    ''' </summary>
    Private Sub AddText(format As String, ParamArray args() As Object)
        Dispatcher.BeginInvoke(Sub() AddMessage(LogDestinations.All, format, args))
    End Sub


    ''' <summary>
    ''' Post the text onto the UI and/or to the voice feedback, depending on <paramref name="mode"/>.
    ''' </summary>
    Private Sub AddMessage(mode As LogDestinations, format As String, ParamArray args() As Object)
        Dim str = String.Format(format, args)
        If mode.HasFlag(LogDestinations.MessageBox) Then
            AddTextMessage(str)
        End If

        If mode.HasFlag(LogDestinations.Speech) Then
            AddVoiceMessage(str)
        End If
    End Sub


    Private Sub AddTextMessage(message As String)
        If message = mLastLine Then Return

        mLastLine = message

        Dispatcher.BeginInvoke(
            Sub()
                If IsLoaded Then
                    Text = Text & message & vbCrLf
                    mScroller?.ScrollToEnd()
                Else
                    OfflineText = OfflineText & message & vbCrLf
                End If
            End Sub)
    End Sub


    Private Sub AddVoiceMessage(str As String)
        Dispatcher.BeginInvoke(
            Sub()
                Speaker?.Speak(str)
            End Sub)
    End Sub


    Private Function GenerateTriggersList() As Object
        If NextTriggerList.Count = 0 Then Return "None"

        Dim lst = NextTriggerList.Select(Function(t) $"{t.NextAction} at {TimeToHuman(t.IsAbsolute, t.NextTime)}")
        Return String.Join(", ", lst)
    End Function


    Private Function TimeToHuman(isAbsolute As Boolean, time As Date) As String
        If isAbsolute Then
            Return "Exactly " + time.ToString("T")
        Else
            Dim hr = If(time.Hour > 0, $"{time.Hour} hour ", "")
            Dim ms = $"{time.Minute}:{time.Second}"
            Return hr + ms
        End If
    End Function

#End Region

End Class
