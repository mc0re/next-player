''' <summary>
''' Types of the generated effects.
''' </summary>
''' <remarks>
''' IEffectGenerator implementation should be able to generate all these types.
''' </remarks>
Public Enum GeneratedVolumeAutomationTypes
	FadeIn
	FadeOut
End Enum


Public Interface IEffectGenerator

	''' <summary>
	''' Generate an auto-effect.
	''' </summary>
	''' <param name="genType">Type of the generated effect</param>
	''' <param name="target">Effect target</param>
	''' <param name="duration">Nothing means default duration</param>
	Function Generate(genType As GeneratedVolumeAutomationTypes, target As ISoundProducer, duration As TimeSpan) As IPlayerAction

End Interface
