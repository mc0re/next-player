Public Interface IPlayerAction
    Inherits IDurationElement

#Region " Essential properties "

    ''' <summary>
    ''' Display name.
    ''' </summary>
    Property Name As String


    ''' <summary>
    ''' Whether an action can be executed by calling Start.
    ''' E.g. comments can not.
    ''' </summary>
    Property CanExecute As Boolean


    ''' <summary>
    ''' True when the action is enabled.
    ''' </summary>
    Property IsEnabled As Boolean


    ''' <summary>
    ''' Current position in playback / automation.
    ''' </summary>
    Property PlayPosition As TimeSpan


    ''' <summary>
    ''' At which PlayPosition the playback started.
    ''' Usually 0, unless the playback is cut.
    ''' </summary>
    ReadOnly Property StartPosition As TimeSpan


    ''' <summary>
    ''' At which PlayPosition the playback stops, offset from the end.
    ''' Usually 0, unless the playback is cut.
    ''' </summary>
    ReadOnly Property StopPosition As TimeSpan


    ''' <summary>
    ''' Whether to stop the previous sound.
    ''' </summary>
    Property ExecutionType As ExecutionTypes


    ''' <summary>
    ''' How the action's execution is related to the previous actions.
    ''' </summary>
    Property DelayType As DelayTypes


    ''' <summary>
    ''' Where to measure the delay from.
    ''' </summary>
    Property DelayReference As DelayReferences


    ''' <summary>
    ''' Value used if <see cref="DelayType"/> requires one, or for cross-fade.
    ''' </summary>
    Property DelayBefore As TimeSpan

#End Region


#Region " Assistive properties, only used during playback "

    ''' <summary>
    ''' Action's index in the playlist (1..).
    ''' </summary>
    Property Index As Integer


    ''' <summary>
    ''' Whether the action is "global parallel".
    ''' </summary>
    ''' <returns></returns>
    Property IsGlobalParallel As Boolean


    ''' <summary>
    ''' The actual reference action, described in <see cref="DelayReference"/>.
    ''' Calculated upon structure change by <see cref="PlaylistStructureLibrary.ArrangeStructure"/>.
    ''' </summary>
    Property ReferenceAction As IPlayerAction


    ''' <summary>
    ''' Whether the action has been modified.
    ''' The flag is regulated by PlayerActionCollection.
    ''' </summary>
    Property IsSaveNeeded As Boolean


    ''' <summary>
    ''' True when playback or pause. UI-only.
    ''' </summary>
    Property IsActive As Boolean


    ''' <summary>
    ''' True when the playback is ongoing.
    ''' </summary>
    Property IsPlaying As Boolean


    ''' <summary>
    ''' Whether this action is a parallel to the currently executing main action.
    ''' </summary>
    Property IsActiveParallel As Boolean


    ''' <summary>
    ''' If this is a parallel action, this property identifies its parent.
    ''' </summary>
    Property ParallelParent As IPlayerAction


    ''' <summary>
    ''' If this is a parallel action, this property is its number in the list (1..).
    ''' </summary>
    Property ParallelIndex As Integer


    ''' <summary>
    ''' Whether the parallel index is applicable (shown in UI).
    ''' </summary>
    Property HasParallelIndex As Boolean


    ''' <summary>
    ''' A list of parallel actions, to which this one is a parent.
    ''' </summary>
    Property Parallels As IList(Of IPlayerAction)


    ''' <summary>
    ''' Whether this action is the next one after the currently playing one.
    ''' </summary>
    Property IsNext As Boolean


    ''' <summary>
    ''' Next action in after this one.
    ''' </summary>
    Property NextAction As IPlayerAction


    ''' <summary>
    ''' When was the action started, in playlist time.
    ''' </summary>
    Property StartTime As Double

#End Region


#Region " Methods "

    ''' <summary>
    ''' Prepare before the Start (when starting, not resuming).
    ''' </summary>
    Sub PrepareStart()


    ''' <summary>
    ''' Start or resume the sound producing / automation.
    ''' </summary>
    Sub Start()


    ''' <summary>
    ''' Stop producing the sound / automation and any timers.
    ''' </summary>
    ''' <param name="intendedResume">Whether a resume might be expected. If not, shut down.</param>
    Sub [Stop](intendedResume As Boolean)


    ''' <summary>
    ''' Used to update the playback position from inside the action.
    ''' </summary>
    Sub UpdatePlayPosition()


    ''' <summary>
    ''' Raise or sink the volume by the given amount in percent.
    ''' </summary>
    ''' <param name="delta">Negative - sink, positive - raise; 0.1 is 10% higher, -0.1 is 10% lower</param>
    ''' <remarks>Non-0 volume must remain non-0, limit is VolumePrecision</remarks>
    Sub ModifyVolume(delta As Single)

#End Region

End Interface
