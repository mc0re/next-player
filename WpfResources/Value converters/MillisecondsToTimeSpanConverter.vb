Imports System.Globalization


<ValueConversion(GetType(Double), GetType(TimeSpan), ParameterType:=GetType(String))>
Public Class MillisecondsToTimeSpanConverter
	Implements IValueConverter

#Region " IValueConverter implementation "

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
		If value Is DependencyProperty.UnsetValue Then Return Binding.DoNothing

		Dim ms = CDbl(value)
		Dim ts = TimeSpan.FromMilliseconds(ms)

		If targetType Is GetType(TimeSpan) Then
			Return ts
		End If

		Return TimeSpanFormatConverter.TimeSpanToString(ts, parameter)
	End Function


	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
		Throw New NotImplementedException()
	End Function

#End Region

End Class
