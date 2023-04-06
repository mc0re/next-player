Imports System.Windows
Imports System.ComponentModel
Imports System.IO
Imports Microsoft.Reporting.WinForms
Imports Common
Imports PlayerActions


Public Class ReportCreatorControl

#Region " Playlist dependency property "

	Public Shared ReadOnly PlaylistProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(Playlist), GetType(PlayerActionCollection), GetType(ReportCreatorControl),
		New PropertyMetadata(New PropertyChangedCallback(AddressOf PlaylistChanged)))


	<Category("Common Properties"), Description("A list of all actions to use for printing")>
	Public Property Playlist As PlayerActionCollection
		Get
			Return CType(GetValue(PlaylistProperty), PlayerActionCollection)
		End Get
		Set(value As PlayerActionCollection)
			SetValue(PlaylistProperty, value)
		End Set
	End Property


	Private Shared Sub PlaylistChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		Dim this = CType(obj, ReportCreatorControl)
		this.GenerateReport()
	End Sub

#End Region


#Region " VoiceCommandList dependency property "

	Public Shared ReadOnly VoiceCommandListProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(VoiceCommandList), GetType(VoiceCommandConfigItemCollection), GetType(ReportCreatorControl),
		New PropertyMetadata(New PropertyChangedCallback(AddressOf VoiceCommandListChanged)))


	<Category("Common Properties"), Description("A list of assigned voice commands")>
	Public Property VoiceCommandList As VoiceCommandConfigItemCollection
		Get
			Return CType(GetValue(VoiceCommandListProperty), VoiceCommandConfigItemCollection)
		End Get
		Set(value As VoiceCommandConfigItemCollection)
			SetValue(VoiceCommandListProperty, value)
		End Set
	End Property


	Private Shared Sub VoiceCommandListChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		Dim this = CType(obj, ReportCreatorControl)
		this.GenerateReport()
	End Sub

#End Region


#Region " ReportTemplate dependency property "

	Public Shared ReadOnly ReportTemplateProperty As DependencyProperty = DependencyProperty.Register(
		NameOf(ReportTemplate), GetType(ReportTemplateItem), GetType(ReportCreatorControl),
		New PropertyMetadata(New PropertyChangedCallback(AddressOf ReportTemplateChanged)))


	<Category("Common Properties"), Description("A report template to apply")>
	Public Property ReportTemplate As ReportTemplateItem
		Get
			Return CType(GetValue(ReportTemplateProperty), ReportTemplateItem)
		End Get
		Set(value As ReportTemplateItem)
			SetValue(ReportTemplateProperty, value)
		End Set
	End Property


	Private Shared Sub ReportTemplateChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
		Dim this = CType(obj, ReportCreatorControl)
		this.GenerateReport()
	End Sub

#End Region


#Region " Init and clean-up "

	''' <summary>
	''' Upon control loading, set the report viewer colours.
	''' </summary>
	Private Sub ControlLoadHandler(sender As Object, args As EventArgs) Handles Me.Loaded
		'If Me.Background IsNot Nothing Then
		'	Dim bgColor = CType(Me.Background, Media.SolidColorBrush).Color
		'	PlaylistReportViewer.BackColor = System.Drawing.Color.FromArgb(bgColor.R, bgColor.G, bgColor.B)
		'End If

		'If Me.Foreground IsNot Nothing Then
		'	Dim fgColor = CType(Me.Foreground, Media.SolidColorBrush).Color
		'	PlaylistReportViewer.ForeColor = System.Drawing.Color.FromArgb(fgColor.R, fgColor.G, fgColor.B)
		'End If

		PlaylistReportViewer.ProcessingMode = ProcessingMode.Local
	End Sub

#End Region


#Region " Report generation "

	<CodeAnalysis.SuppressMessage("Design", "CC0021:You should use nameof instead of the parameter element name string", Justification:="<Pending>")>
	Private Sub GenerateReport()
		If Playlist Is Nothing Then Return
		Dim report = PlaylistReportViewer.LocalReport

		Try
			report.ReportPath = ReportTemplate.FilePath
			'Using strm = File.OpenRead(ReportTemplate.FilePath)
			'	report.LoadReportDefinition(strm)
			'End Using

			report.DataSources.Clear()
			For Each dsn In report.GetDataSourceNames()
				Dim dataSource As IEnumerable(Of Object) = Nothing

				Select Case dsn
					Case "Playlist"
						dataSource = {Playlist}
					Case "PlayerActionList"
						dataSource = (From item In Playlist.Items.Cast(Of PlayerAction)() Select New ReportPlayerAction(item)).ToList()
					Case "VoiceCommandList"
						dataSource = (From item In VoiceCommandList
									  Select New ReportVoiceCommandSetting(item, CommandList.GetCommandDescription(item.CommandName))).ToList()
				End Select

				If dataSource Is Nothing Then
					MessageBox.Show(String.Format("Unknown data source '{0}'", dsn), "NexT Player")
					Continue For
				End If

				report.DataSources.Add(New ReportDataSource(dsn, dataSource))
			Next

			PlaylistReportViewer.RefreshReport()

		Catch ex As Exception
			Trace.TraceError(ex.Message)
		End Try
	End Sub


	''' <summary>
	''' Export the generated report to a PDF file and open it.
	''' </summary>
	Public Sub ExportToPdf(fileName As String)
		Dim bytes = PlaylistReportViewer.LocalReport.Render("PDF")

		Using fs As New FileStream(fileName, FileMode.Create)
			fs.Write(bytes, 0, bytes.Length)
			fs.Close()
		End Using

		System.Diagnostics.Process.Start(fileName)
	End Sub


	Public Function GetDirName(path As String) As String
		Dim sidx = path.LastIndexOfAny({"\"c, "/"c})
		Return path.Substring(0, sidx)
	End Function


	Public Function GetFileName(path As String) As String
		Dim sidx = path.LastIndexOfAny({"\"c, "/"c})
		Return path.Substring(sidx + 1)
	End Function

#End Region

End Class
