Imports System.Globalization


''' <summary>
''' Convert any object to itself.
''' In case of multibinding-converter, is called for every change
''' in all properties, although returns the first one.
''' </summary>
Public Class PassThroughConverter
	Implements IValueConverter, IMultiValueConverter

#Region " IValueConverter implementation "

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
		If value Is DependencyProperty.UnsetValue Then Return Binding.DoNothing

		Return value
	End Function


	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
		Throw New NotImplementedException()
	End Function

#End Region


#Region " IMultiValueConverter implementation "

	Public Function ConvertMulti(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
		If values.Count = 0 Then
			Throw New ArgumentException("PassThroughConverter shall have at least one value to pass")
		End If

		If values(0) Is DependencyProperty.UnsetValue Then Return Binding.DoNothing

		Return values(0)
	End Function


	Public Function ConvertMultiBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
		Throw New NotImplementedException()
	End Function

#End Region

End Class
