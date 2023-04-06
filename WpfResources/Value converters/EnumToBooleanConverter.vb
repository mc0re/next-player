Imports System.Globalization


Public Class EnumToBooleanConverter
	Implements IValueConverter

#Region " Properties "

	Public Property MatchValue As Boolean = True


	Public Property NonMatchValue As Boolean = False


	Public Property NonMatchEnumValue As Object = Binding.DoNothing

#End Region


#Region " IValueConverter implementation "

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
		If value Is DependencyProperty.UnsetValue Then Return Binding.DoNothing

		Dim enumValue As Object

		If TypeOf parameter Is String Then
			enumValue = [Enum].Parse(value.GetType(), CStr(parameter))
		Else
			enumValue = parameter
		End If

		If value.Equals(enumValue) Then
			Return MatchValue
		Else
			Return NonMatchValue
		End If
	End Function


	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
		Dim enumValue As Object

		If TypeOf parameter Is String Then
			enumValue = [Enum].Parse(targetType, CStr(parameter))
		Else
			enumValue = parameter
		End If

		If value.Equals(MatchValue) Then
			Return enumValue
		Else
			Return NonMatchEnumValue
		End If
	End Function

#End Region

End Class
