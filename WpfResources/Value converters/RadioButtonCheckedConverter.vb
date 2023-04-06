''' <summary>
''' Comparison converter for radio buttons.
''' </summary>
<ValueConversion(GetType(Boolean), GetType(Boolean), ParameterType:=GetType(Boolean))>
<ValueConversion(GetType([Enum]), GetType(Boolean), ParameterType:=GetType([Enum]))>
Public Class RadioButtonCheckedConverter
	Implements IValueConverter

#Region " IValueConverter implementation "

	Public Function Convert(value As Object, targetType As System.Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
		Return value.Equals(parameter)
	End Function


	Public Function ConvertBack(value As Object, targetType As System.Type, parameter As Object, culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
		Return If(value.Equals(True), parameter, Binding.DoNothing)
	End Function

#End Region

End Class
