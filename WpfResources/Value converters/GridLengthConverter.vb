Imports System.Globalization


<ValueConversion(GetType(Double), GetType(GridLength))>
Public Class GridLengthConverter
	Implements IValueConverter

#Region " IValueConverter implementation "

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
		If value Is DependencyProperty.UnsetValue Then Return Binding.DoNothing

		Dim v = CDbl(value)
		Dim res As New GridLength(v)

		Return res
	End Function


	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
		If value Is DependencyProperty.UnsetValue Then Return Binding.DoNothing

		Dim g = CType(value, GridLength)
		Return g.Value
	End Function

#End Region

End Class
