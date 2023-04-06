Imports System.Windows
Imports System.Windows.Data


''' <summary>
''' Convert input values into a string.
''' In single-value mode format is given as ConverterParameter.
''' In multi-mode the format is ConverterParameter or (if not supplied) the first value.
''' </summary>
<ValueConversion(GetType(Object), GetType(String), ParameterType:=GetType(String))>
Public Class StringFormatConverter
	Implements IValueConverter, IMultiValueConverter

#Region " IValueConverter implementation "

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
		If value Is DependencyProperty.UnsetValue Then Return Binding.DoNothing

		Dim fmt = If(parameter Is Nothing, "{0}", CStr(parameter))
		fmt = fmt.Replace("\n", vbCrLf)

		Return String.Format(fmt, value)
	End Function


	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
		Throw New NotImplementedException()
	End Function

#End Region


#Region " IMultiValueConverter implementation "

	Public Function ConvertMulti(values() As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IMultiValueConverter.Convert
		For Each v In values
			If v Is DependencyProperty.UnsetValue Then
				Return Binding.DoNothing
			End If
		Next

		Dim idx = 0
		Dim fmt As String
		If parameter Is Nothing Then
			If values.Count = 0 Then Return String.Empty
			fmt = CStr(values(0))
			idx += 1
		Else
			fmt = CStr(parameter)
		End If

		fmt = fmt.Replace("\n", vbCrLf)

		Return String.Format(fmt, values.Skip(idx).ToArray())
	End Function


	Public Function ConvertMultiBack(value As Object, targetTypes() As Type, parameter As Object, culture As Globalization.CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
		Throw New NotImplementedException()
	End Function

#End Region

End Class
