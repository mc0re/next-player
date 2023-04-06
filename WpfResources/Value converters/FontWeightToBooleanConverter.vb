Public Class FontWeightToBooleanConverter
	Implements IValueConverter

#Region " IValueConverter implementation "

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
		If value Is DependencyProperty.UnsetValue Then Return Binding.DoNothing

		Dim fw = CType(value, FontWeight)
		Return fw <> FontWeights.Normal
	End Function


	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
		Dim isBold = CBool(value)
		Return If(isBold, FontWeights.Bold, FontWeights.Normal)
	End Function

#End Region

End Class
