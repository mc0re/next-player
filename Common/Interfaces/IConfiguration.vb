Imports System.ComponentModel


''' <summary>
''' Configuration interface.
''' </summary>
Public Interface IConfiguration
	Inherits IVolumeConfiguration, IEffectDurationConfiguration

	ReadOnly Property Skin As Object    ' SkinConfiguration

	Property CurrentActionCollection As IPlaylist

	Property CurrentSkinPath As String

	Property CurrentEnvironment As INotifyPropertyChanged

	Property EnvironmentName As String

End Interface
