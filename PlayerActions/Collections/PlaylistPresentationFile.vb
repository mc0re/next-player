Imports Common
Imports System.Xml.Serialization
Imports System.IO
Imports System.ComponentModel


''' <summary>
''' Stores information about a presentation (PPT) file used in the playlist.
''' </summary>
''' <remarks></remarks>
<Serializable()>
Public Class PlaylistPresentationFile
	Inherits PropertyChangedHelper
	Implements IInputFile

#Region " FileName property "

	Private mFileName As String


	''' <summary>
	''' Relative path to PPT file.
	''' </summary>
	Public Property FileName As String
		Get
			Return mFileName
		End Get
		Set(value As String)
			mFileName = value
			SetField(mFileName, value, Function() FileName)
			ShortFileName = Path.GetFileNameWithoutExtension(value)
		End Set
	End Property

#End Region


#Region " LastRootPath property "

	<XmlIgnore>
	Public Property LastRootPath As String Implements IInputFile.LastRootPath

#End Region


#Region " FileTimestamp notifying property "

	Private mFileTimestamp As DateTime


	''' <summary>
	''' File modified timestamp, to check for updates.
	''' </summary>
	<IgnoreForReport()>
	Public Property FileTimestamp As DateTime Implements IInputFile.FileTimestamp
		Get
			Return mFileTimestamp
		End Get
		Set(value As DateTime)
			SetField(mFileTimestamp, value, Function() FileTimestamp)
		End Set
	End Property


#End Region


#Region " AbsFileName property "

	''' <summary>
	''' Absolute path to the file.
	''' </summary>
	<XmlIgnore()>
	Public Property AbsFileName As String


	<XmlIgnore()>
	<IgnoreForReport()>
	Public ReadOnly Property AbsFileNameReadOnly As String Implements IInputFile.FileName
		Get
			Return AbsFileName
		End Get
	End Property

#End Region


#Region " ShortFileName read-only notifying non-persistent property  "

	Private mShortFileName As String


	''' <summary>
	''' Get user-friendly name of the file.
	''' </summary>
	<XmlIgnore()>
	Public Property ShortFileName As String
		Get
			Return mShortFileName
		End Get
		Set(value As String)
			SetField(mShortFileName, value, Function() ShortFileName)
		End Set
	End Property

#End Region


#Region " IsLoadingFailed non-persistent notifying property "

	Private mIsLoadingFailed As Boolean


	<XmlIgnore()>
	<Description("Whether there was a problem loading the file")>
	Public Property IsLoadingFailed As Boolean Implements IInputFile.IsLoadingFailed
		Get
			Return mIsLoadingFailed
		End Get
		Set(value As Boolean)
			SetField(mIsLoadingFailed, value, Function() IsLoadingFailed)
		End Set
	End Property

#End Region


#Region " API "

	''' <summary>
	''' Set AbsFileName to absolute, according to the load path.
	''' Check modification date.
	''' </summary>
	Public Sub AfterLoad(rootPath As String) Implements IInputFile.AfterLoad
		If String.IsNullOrWhiteSpace(rootPath) Then
			AbsFileName = FileName
			Return
		End If

		Dim lastDate As DateTime
		AbsFileName = FileUtility.RelPathToAbsolute(FileName, rootPath, lastDate)
		LastRootPath = rootPath

		Dim presRef = PresenterFactory.GetReference(0)
		If presRef Is Nothing Then Return

		presRef.UpdateThumbnails(False)
		FileTimestamp = lastDate
	End Sub


	''' <summary>
	''' Set FileName to relative, according to the new save path.
	''' </summary>
	Public Sub BeforeSave(rootPath As String) Implements IInputFile.BeforeSave
		FileName = FileUtility.AbsPathToRelative(AbsFileName, rootPath)
		LastRootPath = rootPath
	End Sub


	Public Shadows Function Clone() As PlaylistPresentationFile
		Dim res = MyBase.Clone(Of PlaylistPresentationFile)()

		res.AbsFileName = AbsFileName

		Return res
	End Function

#End Region

End Class
