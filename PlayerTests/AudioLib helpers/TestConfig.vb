Imports Common


Public Class TestConfig
	Implements IVolumeConfiguration

	Public Property BalancePrecision As Single = 0.01F Implements IVolumeConfiguration.BalancePrecision


	Public Property VolumePrecision As Single = 0.01F Implements IVolumeConfiguration.VolumePrecision

End Class
