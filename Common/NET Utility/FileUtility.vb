Imports System.IO


Public Class FileUtility

	''' <summary>
	''' Convert relative path to absolute.
	''' Also check the last modification timestamp.
	''' </summary>
	''' <param name="relPath">File name and relative path (e.g. "..\mp3\audio.mp3")</param>
	''' <param name="rootPath">Root path (where the playlist file resides; e.g. "C:\Playlists\Perf\Player")</param>
	''' <param name="modifDate">OUT. Last modification date. If file not found, set to '12:00 midnight, January 1, 1601 A.D. (C.E.) Coordinated Universal Time (UTC), adjusted to local time.'</param>
	''' <returns>Absolute path (e.g. "C:\Playlists\Perf\mp3\audio.mp3")</returns>
	Public Shared Function RelPathToAbsolute(relPath As String, rootPath As String, ByRef modifDate As DateTime) As String
		If String.IsNullOrWhiteSpace(relPath) Then Return String.Empty
		If String.IsNullOrEmpty(rootPath) Then Return relPath

		Dim absPath = Path.GetFullPath(Path.Combine(rootPath, relPath))

		Try
			modifDate = File.GetLastWriteTime(absPath)

		Catch ex As UnauthorizedAccessException
			InterfaceMapper.GetImplementation(Of IMessageLog)().LogFileError(
				"No permission to open '{0}': {1}", absPath, ex.Message)

		Catch ex As PathTooLongException
			InterfaceMapper.GetImplementation(Of IMessageLog)().LogFileError(
				"Path too long (max 260 chars): '{0}'", absPath)
		End Try

		Return absPath
	End Function


	''' <summary>
	''' Convert absolute path to relative (relative to rootPath).
	''' </summary>
	Public Shared Function AbsPathToRelative(absPath As String, rootPath As String) As String
		Static ds As String = Path.DirectorySeparatorChar

		If String.IsNullOrWhiteSpace(absPath) Then Return String.Empty

		If Not rootPath.EndsWith(ds) Then rootPath = rootPath & ds
		Dim uriRoot As New Uri(rootPath)
		Dim uriFile As New Uri(absPath)
		Dim relativeUri = uriRoot.MakeRelativeUri(uriFile)
		Return Uri.UnescapeDataString(relativeUri.ToString())
	End Function

End Class
