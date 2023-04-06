Public Class FontStyleToBooleanConverter
	Implements IValueConverter

#Region " IValueConverter implementation "

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
		If value Is DependencyProperty.UnsetValue Then Return Binding.DoNothing

		Dim fs = CType(value, FontStyle)
		Return fs <> FontStyles.Normal
	End Function


	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
		Dim isItalic = CBool(value)
		Return If(isItalic, FontStyles.Italic, FontStyles.Normal)
	End Function

#End Region

End Class
