﻿Imports System.Text.RegularExpressions
''' <summary>
''' Remove all CR/LFs.
''' </summary>
<ValueConversion(GetType(String), GetType(String))>
Public Class StringToOneLineConverter
	Implements IValueConverter

	Private Shared sNewLines As New Regex($"[{vbCr}{vbLf}]+[ {vbCr}{vbLf}]*")

	Public Property NewLine As String = " | "


#Region " IValueConverter implementation "

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
		If value Is DependencyProperty.UnsetValue Then Return Binding.DoNothing

		Dim str = CStr(value)
		If String.IsNullOrEmpty(str) Then Return str

		str = sNewLines.Replace(str, NewLine)

		Return str
	End Function


	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
		Throw New NotImplementedException()
	End Function

#End Region

End Class
