Imports System.Globalization
Imports System.Windows.Media
Imports System.Windows.Interop
Imports System.Windows.Media.Imaging
Imports Common


<ValueConversion(GetType(String), GetType(ImageSource))>
Public Class IconPathConverter
	Implements IValueConverter

#Region " Win32 declaration "

	Private Declare Unicode Function ExtractIcon Lib "shell32.dll" Alias "ExtractIconW" _
		(hInst As IntPtr, lpszExeFileName As String, nIconIndex As Integer) As IntPtr

#End Region


#Region " IValueConverter implementation "

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
		If targetType IsNot GetType(ImageSource) Then Return Binding.DoNothing

		Try
			Dim fileName = CStr(value).Split(","c)
			If fileName.Length <> 2 Then Throw New ArgumentException("Expecting 2 parts of icon path")

			Dim hIcon = ExtractIcon(Process.GetCurrentProcess().Handle, fileName(0), Integer.Parse(fileName(1)))
			If hIcon = IntPtr.Zero Then Return Nothing

			Dim ret = Imaging.CreateBitmapSourceFromHIcon(hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions())
			Return ret

		Catch ex As Exception
			Dim logger = InterfaceMapper.GetImplementation(Of IMessageLog)()
			logger.LogLoadingError("Cannot load icon '{0}': {1}", value, ex.Message)
			Return Nothing
		End Try
	End Function


	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
		Throw New NotImplementedException()
	End Function

#End Region

End Class
