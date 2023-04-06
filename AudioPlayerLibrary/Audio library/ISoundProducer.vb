Imports AudioChannelLibrary


''' <summary>
''' The bare minimum for playing and applying effects on audio.
''' </summary>
Public Interface ISoundProducer
    Inherits IPlayerAction, IVolumeController

    ''' <summary>
    ''' The playback position was changed from outside the action, e.g. from UI.
    ''' </summary>
    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")>
    Event PositionChanged(sender As ISoundProducer)


    ''' <summary>
    ''' THe natural end of playback has been reached.
    ''' </summary>
    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")>
    Event EndReached(sender As ISoundProducer)


    ''' <summary>
    ''' To keep the original volume setting intact.
    ''' </summary>
    Property IsMuted As Boolean


    ''' <summary>
    ''' Effective (animated) volume (0-1) for UI.
    ''' </summary>
    ReadOnly Property EffectiveVolume As Double


    ''' <summary>
    ''' Stereo balance (-1 - 1).
    ''' </summary>
    Property Balance As Double


    ''' <summary>
    ''' Effects applied to this producer.
    ''' </summary>
    Property Effects As IList(Of ISoundAutomation)


    ''' <summary>
    ''' Automatically generated effects, not visible in the UI.
    ''' </summary>
    ReadOnly Property GeneratedEffects As IList(Of ISoundAutomation)


    ''' <summary>
    ''' Get currently effective volume for the producer,
    ''' when asked by an automation.
    ''' The first automation always gets the playback volume.
    ''' </summary>
    Function GetEffectiveVolume() As Double


    ''' <summary>
    ''' Set effective volume for the producer.
    ''' If there are several automations involved, only after the last one has set
    ''' the volume, it is effectuated.
    ''' </summary>
    Sub SetEffectiveVolume(sender As ISoundAutomation, newVolume As Double)


    ''' <summary>
    ''' Simulate that the producer naturally reached the end of sound.
    ''' </summary>
    ''' <remarks>Used for stop-effects.</remarks>
    Sub SimulateEndReached()

End Interface
