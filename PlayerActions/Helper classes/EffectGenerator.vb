Imports AudioPlayerLibrary


Public Class EffectGenerator
    Implements IEffectGenerator

    Public Function Generate(
        genType As GeneratedVolumeAutomationTypes,
        target As ISoundProducer,
        duration As TimeSpan
    ) As IPlayerAction Implements IEffectGenerator.Generate

        Dim eff As New PlayerActionEffect(genType, duration) With {
            .TargetList = {target},
            .DelayType = If(genType = GeneratedVolumeAutomationTypes.FadeIn, DelayTypes.TimedFromStart, DelayTypes.TimedBeforeEnd),
            .DelayReference = DelayReferences.LastProducer,
            .ReferenceAction = target,
            .ExecutionType = ExecutionTypes.Parallel
        }

        If genType = GeneratedVolumeAutomationTypes.FadeOut Then
            eff.DelayBefore = duration
        End If

        target.GeneratedEffects.Add(eff)

        Return eff
    End Function

End Class
