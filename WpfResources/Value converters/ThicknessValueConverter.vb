<ValueConversion(GetType(Double), GetType(Thickness))>
Public Class ThicknessValueConverter
	Implements IValueConverter

#Region " IValueConverter implementation "

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
		If value Is DependencyProperty.UnsetValue Then Return Binding.DoNothing

		Dim thick = CDbl(value)
		Return New Thickness(thick)
	End Function


	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
		Throw New NotImplementedException()
	End Function

#End Region

End Class
