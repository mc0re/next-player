Imports System.Globalization


''' <summary>
''' If the passed object is not Nothing, return Visible, otherwise - Collapsed.
''' </summary>
<ValueConversion(GetType(Object), GetType(Visibility))>
Public Class ObjectToVisibilityConverter
	Implements IValueConverter

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
		If value Is DependencyProperty.UnsetValue Then Return Visibility.Collapsed
		If value Is Nothing Then Return Visibility.Collapsed

		Return Visibility.Visible
	End Function


	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
		Throw New NotImplementedException()
	End Function

End Class
