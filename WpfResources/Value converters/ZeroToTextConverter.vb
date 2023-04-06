Imports System.Globalization


''' <summary>
''' If the provided Integer is not zero, it is converted to String.
''' If it iz zero, ConverterParameter is returned.
''' </summary>
<ValueConversion(GetType(Integer), GetType(String), ParameterType:=GetType(String))>
Public Class ZeroToTextConverter
	Implements IValueConverter

#Region " IValueConverter implementation "

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
		If value Is DependencyProperty.UnsetValue Then Return Binding.DoNothing

		Dim intValue = CInt(value)
		If intValue = 0 Then
			Return CStr(parameter)
		Else
			Return intValue.ToString(CultureInfo.InvariantCulture)
		End If
	End Function


	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
		Throw New NotImplementedException()
	End Function

#End Region

End Class
