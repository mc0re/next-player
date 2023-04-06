Public Interface ISoundAutomation
    Inherits IPlayerAction

    Property EffectTargetMain As EffectTargets


    Property EffectTargetParallel As EffectTargets


    Property TargetList As IList(Of ISoundProducer)

End Interface
