Imports System.ComponentModel


''' <summary>
''' Configuration interface.
''' </summary>
Public Interface IConfiguration
	Inherits IVolumeConfiguration, IEffectDurationConfiguration

	ReadOnly Property Skin As ISkinConfiguration

	Property CurrentActionCollection As IPlaylist

	Property CurrentSkinPath As String

	Property CurrentEnvironment As INotifyPropertyChanged


	''' <summary>
	''' Adjust the list of environments.
	''' </summary>
	Sub SetEnvironments(environmentList As IEnumerable(Of IPlaylistConfigurationItem))


	''' <summary>
	''' The name of the current environment.
	''' </summary>
	Property EnvironmentName As String

End Interface
