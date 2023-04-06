Imports System.Reflection


Public Class ReportTemplateManager

#Region " ReportRoot read-only property "

	Public Shared ReadOnly Property ReportRoot As String
		Get
			Return System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
		End Get
	End Property

#End Region


#Region " Collect report templates "

	Public Shared Function GetTemplates(root As String) As IEnumerable(Of ReportTemplateItem)
		Return (
			From r In System.IO.Directory.EnumerateFiles(root, "*.rdlc", IO.SearchOption.AllDirectories)
			Select New ReportTemplateItem() With {
				.FilePath = r,
				.ShowName = System.IO.Path.GetFileNameWithoutExtension(r)
			})
	End Function

#End Region

End Class
