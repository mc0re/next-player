Imports System.Xml.Serialization


Public Interface IInputFile

	''' <summary>
	''' Last used root; used for cloning.
	''' </summary>
	<XmlIgnore>
	Property LastRootPath As String


	''' <summary>
	''' Full path to the file.
	''' </summary>
	ReadOnly Property FileName As String


	''' <summary>
	''' Timestamp of the last known file version.
	''' </summary>
	Property FileTimestamp As DateTime


	''' <summary>
	''' If the file is not found, or failed to load, set to True.
	''' </summary>
	Property IsLoadingFailed As Boolean


	''' <summary>
	''' If the file name is stored as relative, convert to absolute.
	''' </summary>
	Sub AfterLoad(rootPath As String)


	''' <summary>
	''' If the file name should be stored as relative, convert to relative.
	''' </summary>
	Sub BeforeSave(rootPath As String)

End Interface
