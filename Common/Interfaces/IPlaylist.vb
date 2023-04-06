Imports System.Collections.Specialized
Imports System.ComponentModel


Public Interface IPlaylist
	Inherits IInputFile, INotifyPropertyChanged, INotifyCollectionChanged

	Property EnvironmentName As String

	ReadOnly Property CurrentEnvironment As Object

End Interface
