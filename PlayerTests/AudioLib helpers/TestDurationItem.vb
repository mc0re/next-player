Imports System.Xml.Serialization
Imports AudioPlayerLibrary
Imports Common


Public Class TestDurationItem
	Implements IInputFile, IDurationElement

#Region " FileName property "

	Private ReadOnly mFileName As String


	Public ReadOnly Property FileName As String Implements IInputFile.FileName
		Get
			Return mFileName
		End Get
	End Property

#End Region


	<XmlIgnore>
	Public Property LastRootPath As String Implements IInputFile.LastRootPath


	Public Property FileTimestamp As Date Implements IInputFile.FileTimestamp


	Public Property IsLoadingFailed As Boolean Implements IInputFile.IsLoadingFailed


	Public Property HasDuration As Boolean Implements IDurationElement.HasDuration


	Public Property Duration As TimeSpan Implements IDurationElement.Duration


#Region " Init and clean-up "

	Public Sub New(fileName As String)
		mFileName = fileName
	End Sub

#End Region


	Public Sub AfterLoad(ByVal rootPath As String) Implements IInputFile.AfterLoad
		Throw New NotImplementedException()
	End Sub


	Public Sub BeforeSave(ByVal rootPath As String) Implements IInputFile.BeforeSave
		Throw New NotImplementedException()
	End Sub

End Class
