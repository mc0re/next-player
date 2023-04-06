Imports System.Globalization


<ValueConversion(GetType(String), GetType(FontWeight), ParameterType:=GetType(String))>
Public Class StringToFontWeightConverter
	Implements IValueConverter

#Region " IValueConverter implementation "

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
		If value Is DependencyProperty.UnsetValue Then Return Binding.DoNothing

		Dim txt = CStr(value)
		Return If(txt = CStr(parameter), FontWeights.Bold, FontWeights.Normal)
	End Function


	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
		Throw New NotImplementedException("StringToFontWeightConverter cannot convert back")
	End Function

#End Region

End Class
