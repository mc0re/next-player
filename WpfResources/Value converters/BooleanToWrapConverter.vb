''' <summary>
''' Convert True to Wrap, False to NoWrap.
''' </summary>
<ValueConversion(GetType(Boolean), GetType(TextWrapping))>
Public Class BooleanToWrapConverter
	Implements IValueConverter

#Region " IValueConverter implementation "

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
		If value Is DependencyProperty.UnsetValue Then Return Binding.DoNothing

		Return If(CBool(value), TextWrapping.Wrap, TextWrapping.NoWrap)
	End Function


	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
		Throw New NotImplementedException()
	End Function

#End Region

End Class
