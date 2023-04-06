Imports System.Globalization


<ValueConversion(GetType(Boolean), GetType(Boolean))>
Public Class BooleanNotConverter
	Implements IValueConverter

#Region " IValueConverter implementation "

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
		Return Not CBool(value)
	End Function


	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
		Return Not CBool(value)
	End Function

#End Region

End Class
