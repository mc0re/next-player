Imports System.ComponentModel
Imports System.Reflection
Imports System.Xml.Serialization
Imports AudioPlayerLibrary
Imports Common


''' <summary>
''' Base class for player action.
''' </summary>
''' <remarks>
''' When adding a new action type, remember:
''' - It shall be called PlayerActionXxx, then the ActionType will be "Xxx"
''' - Add XmlInclude to PlayerAction definition (else comes an exception during playlist load / save)
''' - Add missing properties to ReportPlayerAction (else comes an exception during report generation)
''' - Add a DataTemplate to PlaylistControl
''' - Add a DataTemplate to PlayerActionControl
''' - Add an adding graphics, RoutedCommand, handler and button; remember AppCommandList
''' - Add specific report line(s) to standard templates
''' </remarks>
<Serializable()>
<XmlInclude(GetType(PlayerActionAutomation))>
<XmlInclude(GetType(PlayerActionComment))>
<XmlInclude(GetType(PlayerActionEffect))>
<XmlInclude(GetType(PlayerActionFile))>
<XmlInclude(GetType(PlayerActionPowerPoint))>
<XmlInclude(GetType(PlayerActionText))>
Public MustInherit Class PlayerAction
    Implements INotifyPropertyChanged, IDurationElement, IPlayerAction, ICloneable

#Region " INotifyPropertyChanged implementation "

    <NonSerialized()>
    Public Event PropertyChanged(sender As Object, args As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged


    ''' <summary>
    ''' Helper function to raise the event.
    ''' NB. BindingList does not propagate properties of derived objects.
    ''' </summary>
    Protected Sub RaisePropertyChanged(propName As String)
        Dim pi = Me.GetType().GetProperty(propName)
        Dim ign = pi.GetCustomAttribute(Of XmlIgnoreAttribute)()
        Dim ser = pi.GetCustomAttribute(Of SerializedAsAttribute)()
        If ign Is Nothing OrElse ser IsNot Nothing Then IsSaveNeeded = True

        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propName))
    End Sub


    ''' <summary>
    ''' Helper function to raise the event.
    ''' NB. BindingList does not propagate properties of derived objects.
    ''' </summary>
    Protected Sub RaisePropertyChanged(Of T)(prop As Expressions.Expression(Of Func(Of T)))
        RaisePropertyChanged(PropertyChangedHelper.GetPropertyName(prop))
    End Sub

#End Region


#Region " Constants "

    Public Shared ReadOnly PlaceHolder As PlayerAction = New PlayerActionPlaceholder()

#End Region


#Region " Persistent properties "

#Region " IsEnabled notifying property "

    Private mIsEnabled As Boolean = True


    ''' <summary>
    ''' Whether the action is enabled (otherwise skip it).
    ''' </summary>
    <AffectsStructure()>
    <AffectsTriggers>
    Public Property IsEnabled As Boolean Implements IPlayerAction.IsEnabled
        Get
            Return mIsEnabled
        End Get
        Set(value As Boolean)
            mIsEnabled = value
            RaisePropertyChanged(Function() IsEnabled)
        End Set
    End Property

#End Region


#Region " Name notifying property "

    Private mName As String


    ''' <summary>
    ''' User-defined action name.
    ''' </summary>
    Public Property Name As String Implements IPlayerAction.Name
        Get
            Return mName
        End Get
        Set(value As String)
            mName = value
            RaisePropertyChanged(Function() Name)
        End Set
    End Property


#End Region


#Region " ExecutionType notifying property "

    Private mExecutionType As ExecutionTypes = ExecutionTypes.MainStopPrev


    ''' <summary>
    ''' Whether to stop the previous sound.
    ''' </summary>
    <AffectsStructure()>
    Public Property ExecutionType As ExecutionTypes Implements IPlayerAction.ExecutionType
        Get
            Return mExecutionType
        End Get
        Set(value As ExecutionTypes)
            If mExecutionType = value Then Return

            mExecutionType = value
            RaisePropertyChanged(Function() ExecutionType)

            If value = ExecutionTypes.MainCrossFade Then
                DelayReference = DelayReferences.LastProducer
                DelayType = DelayTypes.TimedBeforeEnd
                If DelayBefore = TimeSpan.Zero Then
                    Dim sett = InterfaceMapper.GetImplementation(Of IEffectDurationConfiguration)()
                    DelayBefore = sett.DefaultDuration
                End If
            End If
        End Set
    End Property

#End Region


#Region " DelayType notifying property "

    Private mDelayType As DelayTypes = DelayTypes.Manual


    ''' <summary>
    ''' Type of delay before the playback.
    ''' </summary>
    <AffectsTriggers>
    Public Property DelayType As DelayTypes Implements IPlayerAction.DelayType
        Get
            Return mDelayType
        End Get
        Set(value As DelayTypes)
            If mDelayType = value Then Return

            mDelayType = value
            RaisePropertyChanged(Function() DelayType)
        End Set
    End Property

#End Region


#Region " DelayReference notifying property "

    Private mDelayReference As DelayReferences = DelayReferences.LastProducer


    ''' <summary>
    ''' What to delay from.
    ''' </summary>
    <AffectsStructure()>
    <AffectsTriggers>
    Public Property DelayReference As DelayReferences Implements IPlayerAction.DelayReference
        Get
            Return mDelayReference
        End Get
        Set(value As DelayReferences)
            If mDelayReference = value Then Return

            mDelayReference = value
            RaisePropertyChanged(Function() DelayReference)
        End Set
    End Property

#End Region


#Region " DelayBefore notifying property "

    Private mDelayBefore As TimeSpan


    ''' <summary>
    ''' Delay before starting the playback for Timed delays.
    ''' Must be non-negative.
    ''' </summary>
    <XmlIgnore, SerializedAs>
    <AffectsTriggers>
    Public Property DelayBefore As TimeSpan Implements IPlayerAction.DelayBefore
        Get
            Return mDelayBefore
        End Get
        Set(value As TimeSpan)
            mDelayBefore = value
            RaisePropertyChanged(Function() DelayBefore)
        End Set
    End Property


    ''' <summary>
    ''' Delay before starting the playback for Timed delays.
    ''' Must be non-negative.
    ''' </summary>
    ''' <remarks>To work around inability to serialize TimeSpan.</remarks>
    <XmlElement(NameOf(DelayBefore))>
    <IgnoreForReport()>
    Public Property DelayBeforeSerialized As Double
        Get
            Return DelayBefore.TotalMilliseconds()
        End Get
        Set(value As Double)
            DelayBefore = TimeSpan.FromMilliseconds(value)
        End Set
    End Property

#End Region


#Region " HasDuration notifying property "

    Private mHasDuration As Boolean


    ''' <summary>
    ''' Whether the action has duration.
    ''' For an effect, this is whether to stop the target(s) after the <see cref="Duration"/> is reached.
    ''' </summary>
    Public Property HasDuration As Boolean Implements IPlayerAction.HasDuration
        Get
            Return mHasDuration
        End Get
        Set(value As Boolean)
            If mHasDuration = value Then Return

            mHasDuration = value
            RaisePropertyChanged(Function() HasDuration)
        End Set
    End Property

#End Region


#Region " Duration notifying property "

    Private mDuration As TimeSpan


    ''' <summary>
    ''' Action duration.
    ''' </summary>
    <XmlIgnore, SerializedAs>
    Public Property Duration As TimeSpan Implements IPlayerAction.Duration
        Get
            Return mDuration
        End Get
        Set(value As TimeSpan)
            If mDuration = value Then Return

            mDuration = value
            RaisePropertyChanged(Function() Duration)
        End Set
    End Property


    ''' <summary>
    ''' Serializable duration.
    ''' </summary>
    ''' <remarks>To work around inability to serialize TimeSpan.</remarks>
    <XmlElement(NameOf(Duration))>
    <IgnoreForReport()>
    Public Property DurationSerialized As Double
        Get
            Return Duration.TotalMilliseconds()
        End Get
        Set(value As Double)
            Duration = TimeSpan.FromMilliseconds(value)
            RaisePropertyChanged(Function() DurationSerialized)
        End Set
    End Property

#End Region

#End Region


#Region " Structure-related non-persistent, mainly non-notifying properties "

    <XmlIgnore()>
    Public Property Index As Integer Implements IPlayerAction.Index

    <XmlIgnore()>
    Public Property IsGlobalParallel As Boolean Implements IPlayerAction.IsGlobalParallel

    <XmlIgnore()>
    Public Property CanExecute As Boolean = True Implements IPlayerAction.CanExecute


    ''' <summary>
    ''' Used for reports.
    ''' </summary>
    <XmlIgnore()>
    Public ReadOnly Property ActionType As String
        Get
            Dim t = Me.GetType().Name
            Dim bt = GetType(PlayerAction).Name

            If t.StartsWith(bt, StringComparison.Ordinal) Then
                t = t.Substring(bt.Length)
            End If

            Return t
        End Get
    End Property


    <XmlIgnore()>
    Public Property NextAction As IPlayerAction Implements IPlayerAction.NextAction


    <XmlIgnore()>
    Public Property ParallelParent As IPlayerAction Implements IPlayerAction.ParallelParent


    <XmlIgnore()>
    Public Property Parallels As IList(Of IPlayerAction) = New List(Of IPlayerAction)() Implements IPlayerAction.Parallels


    <XmlIgnore()>
    Public Property StartTime As Double Implements IPlayerAction.StartTime


#Region " DelayReferenceAction notifying property "

    Private mDelayReferenceAction As IPlayerAction


    ''' <summary>
    ''' Shouldn't be used in the report directly.
    ''' </summary>
    <XmlIgnore()>
    Public Property ReferenceAction As IPlayerAction Implements IPlayerAction.ReferenceAction
        Get
            Return mDelayReferenceAction
        End Get
        Set(value As IPlayerAction)
            If mDelayReferenceAction Is value Then Return

            mDelayReferenceAction = value
            RaisePropertyChanged(Function() ReferenceAction)
            RaisePropertyChanged(Function() DelayReferenceName)
        End Set
    End Property

#End Region


#Region " DelayReferenceName read-only notifying property "

    <XmlIgnore()>
    Public ReadOnly Property DelayReferenceName As String
        Get
            If ReferenceAction IsNot Nothing Then
                Return ReferenceAction.Name
            Else
                Return GetEnumAttribute(Of DescriptionAttribute)(DelayReference).Description
            End If
        End Get
    End Property

#End Region

#End Region


#Region " UI-related non-persistent notifying properties "

#Region " ParallelIndex notifying property "

    Private mParallelIndex As Integer


    ''' <summary>
    ''' Index (1..) for several parallel actions.
    ''' </summary>
    <XmlIgnore()>
    Public Property ParallelIndex As Integer Implements IPlayerAction.ParallelIndex
        Get
            Return mParallelIndex
        End Get
        Set(value As Integer)
            If mParallelIndex = value Then Return

            mParallelIndex = value
            RaisePropertyChanged(Function() ParallelIndex)
        End Set
    End Property

#End Region


#Region " HasParallelIndex notifying property "

    Private mHasParallelIndex As Boolean


    ''' <summary>
    ''' Whether <see cref="ParallelIndex"/> is applicable.
    ''' </summary>
    <XmlIgnore()>
    Public Property HasParallelIndex As Boolean Implements IPlayerAction.HasParallelIndex
        Get
            Return mHasParallelIndex
        End Get
        Set(value As Boolean)
            If mHasParallelIndex = value Then Return

            mHasParallelIndex = value
            RaisePropertyChanged(Function() HasParallelIndex)
        End Set
    End Property

#End Region


#Region " IsPlaying read-only property "

    Private mIsPlaying As Boolean


    <XmlIgnore()>
    <IgnoreForReport()>
    <Description("Whether this action is being played (timer running)")>
    Public Property IsPlaying As Boolean Implements IPlayerAction.IsPlaying
        Get
            Return mIsPlaying
        End Get
        Private Set(value As Boolean)
            If mIsPlaying = value Then Return

            mIsPlaying = value
            RaisePropertyChanged(Function() IsPlaying)
        End Set
    End Property

#End Region


#Region " IsActive property "

    Private mIsActive As Boolean


    <XmlIgnore()>
    <IgnoreForReport()>
    <Description("Whether this is a currently active (executing or paused) main action")>
    Public Property IsActive As Boolean Implements IPlayerAction.IsActive
        Get
            Return mIsActive
        End Get
        Set(value As Boolean)
            If mIsActive = value Then Return

            mIsActive = value
            RaisePropertyChanged(Function() IsActive)
        End Set
    End Property

#End Region


#Region " IsActiveParallel property "

    Private mIsActiveParallel As Boolean


    <XmlIgnore()>
    <IgnoreForReport()>
    <Description("Whether this is a parallel to the currently executing main action")>
    Public Property IsActiveParallel As Boolean Implements IPlayerAction.IsActiveParallel
        Get
            Return mIsActiveParallel
        End Get
        Set(value As Boolean)
            If mIsActiveParallel = value Then Return

            mIsActiveParallel = value
            RaisePropertyChanged(Function() IsActiveParallel)
        End Set
    End Property

#End Region


#Region " IsNext property "

    Private mIsNext As Boolean


    <XmlIgnore()>
    <IgnoreForReport()>
    <Description("Whether this action is set as next main action")>
    Public Property IsNext As Boolean Implements IPlayerAction.IsNext
        Get
            Return mIsNext
        End Get
        Set(value As Boolean)
            If mIsNext = value Then Return

            mIsNext = value
            RaisePropertyChanged(Function() IsNext)
        End Set
    End Property

#End Region


#Region " PlayPosition non-persistent notifying property and related overrides "

    Private mPlayPosition As TimeSpan


    ''' <summary>
    ''' Current playing position.
    ''' </summary>
    <XmlIgnore()>
    <IgnoreForReport()>
    Public Property PlayPosition As TimeSpan Implements IPlayerAction.PlayPosition
        Get
            Return mPlayPosition
        End Get
        Set(value As TimeSpan)
            If mPlayPosition = value Then Return

            SetPlayPosition(value)
            OnPlayPositionChanged()
        End Set
    End Property


    ''' <summary>
    ''' Sets the property value and raises the event.
    ''' </summary>
    Protected Sub SetPlayPosition(value As TimeSpan)
        mPlayPosition = value
        RaisePropertyChanged(Function() PlayPosition)
        RaisePropertyChanged(Function() PlayRemaining)
    End Sub


    ''' <summary>
    ''' Override it to update the playback position from inside the action (by playback).
    ''' Use SetPlayPosition(...) to set the value and raise the PropertyChanged.
    ''' </summary>
    Public Overridable Sub UpdatePlayPosition() Implements IPlayerAction.UpdatePlayPosition
        ' Do nothing in the base version
    End Sub


    ''' <summary>
    ''' Override it to perform an action when the PlayPosition has been changed
    ''' from outside the action (e.g. UI).
    ''' </summary>
    ''' <remarks>
    ''' Called from PlayPosition setter.
    ''' </remarks>
    Protected Overridable Sub OnPlayPositionChanged()
        ' Do nothing in the base version
    End Sub

#End Region


#Region " PlayRemaining non-persistent read-only notifying property "

    ''' <summary>
    ''' How much time is left until the end.
    ''' </summary>
    <XmlIgnore()>
    <IgnoreForReport()>
    Public ReadOnly Property PlayRemaining As TimeSpan
        Get
            Return If(HasDuration, Duration - PlayPosition, Duration)
        End Get
    End Property

#End Region


#Region " StartPosition non-persistent property "

    <XmlIgnore()>
    <IgnoreForReport()>
    Public Overridable ReadOnly Property StartPositionReadOnly As TimeSpan Implements IPlayerAction.StartPosition
        Get
            Return TimeSpan.Zero
        End Get
    End Property

#End Region


#Region " StopPosition non-persistent property "

    <XmlIgnore()>
    <IgnoreForReport()>
    Public Overridable ReadOnly Property StopPositionReadOnly As TimeSpan Implements IPlayerAction.StopPosition
        Get
            Return TimeSpan.Zero
        End Get
    End Property

#End Region


#Region " IsSaveNeeded non-notifying non-persistant read-only property "

    Private mIsSaveNeeded As Boolean


    <XmlIgnore()>
    <IgnoreForReport()>
    <Description("Whether this action has been modified such that it needs saving")>
    Public Property IsSaveNeeded As Boolean Implements IPlayerAction.IsSaveNeeded
        Get
            Return mIsSaveNeeded
        End Get
        Set(value As Boolean)
            If mIsSaveNeeded = value Then Return

            mIsSaveNeeded = value
        End Set
    End Property

#End Region

#End Region


#Region " ICloneable implementation "

    Public Function Clone() As Object Implements ICloneable.Clone
        Dim copy = Activator.CreateInstance(Me.GetType())
        Dim propList =
            From pi In Me.GetType().GetProperties()
            Where (pi.CanRead AndAlso pi.CanWrite AndAlso
                   pi.GetCustomAttribute(Of XmlIgnoreAttribute)() Is Nothing)

        For Each pi In propList
            Dim value = pi.GetValue(Me)
            pi.SetValue(copy, value)
        Next

        Dim origAsFile = TryCast(Me, IInputFile)
        If origAsFile IsNot Nothing Then
            Dim copyAsFile = TryCast(copy, IInputFile)
            copyAsFile.AfterLoad(origAsFile.LastRootPath)
        End If

        Return copy
    End Function

#End Region


#Region " IPlayerAction overrides "

    ''' <summary>
    ''' Prepare for playback.
    ''' </summary>
    ''' <remarks>
    ''' Overrides should call the base method to set IsPlaying.
    ''' </remarks>
    Public Overridable Sub PrepareStart() Implements IPlayerAction.PrepareStart
        IsPlaying = True
    End Sub


    ''' <summary>
    ''' Start playback of sound or effect.
    ''' </summary>
    ''' <remarks>
    ''' Overrides should call the base method.
    ''' Used for resume functionality as well (if called without <see cref="PrepareStart"/>).
    ''' </remarks>
    Public Overridable Sub Start() Implements IPlayerAction.Start
        IsPlaying = True
    End Sub


    ''' <summary>
    ''' Stop playback or reset effect.
    ''' </summary>
    ''' <remarks>
    ''' Overrides should call the base method to set IsPlaying.
    ''' </remarks>
    Public Overridable Sub [Stop](intendedResume As Boolean) Implements IPlayerAction.Stop
        IsPlaying = False

        If Not intendedResume Then
            PlayPosition = TimeSpan.Zero
        End If
    End Sub


    ''' <summary>
    ''' Modify the volume.
    ''' </summary>
    Public Overridable Sub ModifyVolume(delta As Single) Implements IPlayerAction.ModifyVolume
        ' Do nothing in the base version.
    End Sub

#End Region


#Region " Utility "

    ''' <summary>
    ''' Calculate new volume based on the old voume and given delta.
    ''' See ModifyVolume.
    ''' </summary>
    Protected Shared Function CalculateVolume(oldVolume As Single, delta As Single) As Single
        Dim newVolume = oldVolume * (1 + delta)
        Dim conf = InterfaceMapper.GetImplementation(Of IVolumeConfiguration)()

        If newVolume > 1 Then
            newVolume = 1
        ElseIf oldVolume > 0 AndAlso newVolume < conf.VolumePrecision Then
            newVolume = conf.VolumePrecision
        End If

        Return newVolume
    End Function

#End Region


#Region " ToString "

    Public Overrides Function ToString() As String
        Dim atype = If(String.IsNullOrWhiteSpace(ActionType), String.Empty, ActionType & " ")
        Return String.Format("{0}'{1}'", atype, Name)
    End Function

#End Region

End Class
