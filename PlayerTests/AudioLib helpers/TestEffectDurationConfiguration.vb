Imports Common


Friend Class TestEffectDurationConfiguration
	Implements IEffectDurationConfiguration

	Public Property DefaultDuration As TimeSpan Implements IEffectDurationConfiguration.DefaultDuration
		Get
			Return TimeSpan.FromSeconds(1)
		End Get
		Set(value As TimeSpan)

		End Set
	End Property

End Class
