Public Class BooleanAndConverter
	Implements IMultiValueConverter

#Region " Properties "

	Public Property NegateResult As Boolean = False

#End Region


#Region " IMultiValueConverter implementation "

	Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IMultiValueConverter.Convert
		If values.Any(Function(v) v Is DependencyProperty.UnsetValue) Then Return Binding.DoNothing

		Dim res = values.Any(Function(v) CBool(v) = False)

		Return If(NegateResult, res, Not res)
	End Function


	Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As Globalization.CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
		Throw New NotImplementedException()
	End Function

#End Region

End Class
