Public Class BooleanOrConverter
	Implements IMultiValueConverter

#Region " IMultiValueConverter implementation "

	Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IMultiValueConverter.Convert
		If values.Any(Function(v) v Is DependencyProperty.UnsetValue) Then Return Binding.DoNothing

		Return values.Any(Function(v) CBool(v) = True)
	End Function


	Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As Globalization.CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
		Throw New NotImplementedException()
	End Function

#End Region

End Class
