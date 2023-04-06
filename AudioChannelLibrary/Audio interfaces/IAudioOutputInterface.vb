Imports System.ComponentModel
Imports NAudio.Wave


Public Interface IAudioOutputInterface
	Inherits INotifyPropertyChanged

	''' <summary>
	''' Create a player for this interface.
	''' </summary>
	Function CreatePlayer() As IWavePlayer

End Interface
