Imports System.Globalization


<ValueConversion(GetType(String), GetType(Boolean), ParameterType:=GetType(String))>
Public Class StringComparerConverter
	Implements IValueConverter

#Region " Properties "

	Public Property NegateResult As Boolean = False

#End Region


#Region " IValueConverter implementation "

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
		If value Is DependencyProperty.UnsetValue Then Return Binding.DoNothing

		Dim valStr = CStr(value)
		Dim parStr = CStr(parameter)

		Dim eq = String.Compare(valStr, parStr, culture, CompareOptions.None) = 0
		Return If(NegateResult, Not eq, eq)
	End Function


	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
		If value Is DependencyProperty.UnsetValue Then Return Binding.DoNothing

		If CBool(value) Then
			Return parameter
		Else
			Return Binding.DoNothing
		End If
	End Function

#End Region

End Class
