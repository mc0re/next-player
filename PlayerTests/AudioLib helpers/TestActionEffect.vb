Imports AudioPlayerLibrary


Friend Class TestActionEffect
    Inherits TestAction
    Implements ISoundAutomation

    Public Property EffectTargetMain As EffectTargets Implements ISoundAutomation.EffectTargetMain

    Public Property EffectTargetParallel As EffectTargets Implements ISoundAutomation.EffectTargetParallel

    Public Property TargetList As IList(Of ISoundProducer) = New List(Of ISoundProducer)() Implements ISoundAutomation.TargetList

End Class
