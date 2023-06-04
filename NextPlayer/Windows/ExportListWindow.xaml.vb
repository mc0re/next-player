Imports System.ComponentModel
Imports System.IO
Imports System.IO.Compression
Imports PlayerActions
Imports WpfResources


Public Class ExportListWindow

#Region " Constants "

	Private Const ExportFileExtensionFilter = "Zip archive (.zip)|*.zip|All files (.*)|*.*"

	Private Const ExportFileExtension = ".zip"

#End Region


#Region " Playlist dependency property "

	Public Shared ReadOnly PlaylistProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Playlist), GetType(PlayerActionCollection), GetType(ExportListWindow))


	<Category("Common Properties"), Description("A list of all actions to export")>
	Public Property Playlist As PlayerActionCollection
		Get
			Return CType(GetValue(PlaylistProperty), PlayerActionCollection)
		End Get
		Set(value As PlayerActionCollection)
			SetValue(PlaylistProperty, value.Clone())
			FillCounters()
		End Set
	End Property


	Private Sub FillCounters()
		Dim actList = (
			From a In Playlist.Items
			Let fa = TryCast(a, PlayerActionFile)
			Where fa IsNot Nothing
			Select fa
		).ToList()

		NumberOfFileActions = actList.Count

		NumberOfFiles = (
				From a In actList
				Where File.Exists(a.AbsFileToPlay)
				Let n = If(
					AddActionNumber,
					a.Index.ToString(),
					If(MatchFileNames, a.Name, Path.GetFileName(a.AbsFileToPlay)))
				Select n Distinct
				).Count()
	End Sub

#End Region


#Region " MatchFileNames dependency property "

	Public Shared ReadOnly MatchFileNamesProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(MatchFileNames), GetType(Boolean), GetType(ExportListWindow),
		New PropertyMetadata(True, New PropertyChangedCallback(AddressOf MatchFileNamesChanged)))


	<Category("Common Properties"), Description("Whether to rename files")>
	Public Property MatchFileNames As Boolean
		Get
			Return CBool(GetValue(MatchFileNamesProperty))
		End Get
		Set(value As Boolean)
			SetValue(MatchFileNamesProperty, value)
		End Set
	End Property


	Private Shared Sub MatchFileNamesChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		Dim this = CType(obj, ExportListWindow)
		this.FillCounters()
	End Sub

#End Region


#Region " AddActionNumber dependency property "

	Public Shared ReadOnly AddActionNumberProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(AddActionNumber), GetType(Boolean), GetType(ExportListWindow),
		New PropertyMetadata(New PropertyChangedCallback(AddressOf AddActionNumberChanged)))


	<Category("Common Properties"), Description("Whether to add action number to file names")>
	Public Property AddActionNumber As Boolean
		Get
			Return CBool(GetValue(AddActionNumberProperty))
		End Get
		Set(value As Boolean)
			SetValue(AddActionNumberProperty, value)
		End Set
	End Property


	Private Shared Sub AddActionNumberChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		Dim this = CType(obj, ExportListWindow)
		this.FillCounters()
	End Sub

#End Region


#Region " NumberOfFileActions read-only dependency property "

	Private Shared ReadOnly NumberOfFileActionsPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
		NameOf(NumberOfFileActions), GetType(Integer), GetType(ExportListWindow),
		New PropertyMetadata(0))


	Public Shared ReadOnly NumberOfFileActionsProperty As DependencyProperty = NumberOfFileActionsPropertyKey.DependencyProperty


	<Category("Common Properties"), Description("The number of file actions in the playlist")>
	Public Property NumberOfFileActions As Integer
		Get
			Return CInt(GetValue(NumberOfFileActionsProperty))
		End Get
		Private Set(value As Integer)
			SetValue(NumberOfFileActionsPropertyKey, value)
		End Set
	End Property

#End Region


#Region " NumberOfFiles read-only dependency property "

	Private Shared ReadOnly NumberOfFilesPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
		NameOf(NumberOfFiles), GetType(Integer), GetType(ExportListWindow),
		New PropertyMetadata(0))


	Public Shared ReadOnly NumberOfFilesProperty As DependencyProperty = NumberOfFilesPropertyKey.DependencyProperty


	<Category("Common Properties"), Description("The number of unique files to be writen")>
	Public Property NumberOfFiles As Integer
		Get
			Return CInt(GetValue(NumberOfFilesProperty))
		End Get
		Private Set(value As Integer)
			SetValue(NumberOfFilesPropertyKey, value)
		End Set
	End Property

#End Region


#Region " UI handlers "

	Private Sub ExportButton_Click(sender As Object, e As RoutedEventArgs)
		Dim dlg As New Microsoft.Win32.SaveFileDialog() With {
			.InitialDirectory = My.Settings.LastExportDir,
			.FileName = Playlist.Name,
			.DefaultExt = ExportFileExtension,
			.Filter = ExportFileExtensionFilter,
			.OverwritePrompt = True
		}

		' Show save file dialog box
		Dim result? As Boolean = dlg.ShowDialog()

		' Process save file dialog box results 
		If result <> True Then Return

		My.Settings.LastExportDir = Path.GetDirectoryName(dlg.FileName)
		ExportPlaylist(dlg.FileName)
		Close()
	End Sub

#End Region


#Region " Utility "

	Private Sub ExportPlaylist(filename As String)
		Dim savedFiles As New HashSet(Of String)()

		Try
			Using New WaitCursor()
				If File.Exists(filename) Then
					File.Delete(filename)
				End If

				Using arch = ZipFile.Open(filename, ZipArchiveMode.Create)
					For Each itm In Playlist.Items
						If TypeOf itm Is PlayerActionFile Then
							Dim fact = CType(itm, PlayerActionFile)

							If File.Exists(fact.AbsFileToPlay) Then
								fact.FileToPlay = ArchiveAction(itm, fact.AbsFileToPlay, savedFiles, arch)

								' Set AbsFileToPlay
								fact.AfterLoad(String.Empty)
							End If
						End If
					Next

					If Playlist.PresenterCount = 1 Then
						Dim pname = Playlist.PresentationFile.AbsFileName

						If File.Exists(pname) Then
							Playlist.PresentationFile.FileName = ArchiveAction(Nothing, pname, savedFiles, arch)

							' Set AbsFileName
							Playlist.PresentationFile.AfterLoad(String.Empty)
						End If
					End If

					Using ms As New MemoryStream
						Dim e = arch.CreateEntry(Playlist.Name & PlayerActionCollection.PlaylistDefaultExtension)

						Using sw = e.Open()
							Playlist.Save(sw)
						End Using
					End Using
				End Using
			End Using

		Catch ex As Exception
			MessageBox.Show(ex.Message, "Exception during export")
		End Try
	End Sub


	''' <summary>
	''' Archive the given file. Rename if required by flags.
	''' </summary>
	''' <returns>New (renamed and clean) file name</returns>
	Private Function ArchiveAction(
		itm As PlayerAction, fileName As String,
		savedFiles As HashSet(Of String), arch As ZipArchive
	) As String
		Dim newName = Path.GetFileName(fileName)

		If itm IsNot Nothing Then
			If MatchFileNames Then
				newName = itm.Name & Path.GetExtension(newName)
			End If

			If AddActionNumber Then
				newName = itm.Index & " " & newName
			End If
		End If

		If savedFiles.Add(newName) Then
			ArchiveFile(fileName, newName, arch)
		End If

		Return newName
	End Function


	Private Sub ArchiveFile(fileName As String, saveName As String, arch As ZipArchive)
		arch.CreateEntryFromFile(fileName, saveName)
	End Sub

#End Region

End Class
