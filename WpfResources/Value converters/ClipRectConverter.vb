Imports System.Globalization
Imports System.Windows.Media


''' <summary>
''' Single value: create a rectangle with the defined height (default 1) and width equal to the given value.
''' Multi value:
''' - Value (Min-Max)
''' - Min value
''' - Max value
''' - Maximum output width
''' - Height
''' </summary>
<ValueConversion(GetType(TimeSpan), GetType(Geometry))>
<ValueConversion(GetType(Double), GetType(Geometry))>
Public Class ClipRectConverter
	Implements IValueConverter, IMultiValueConverter

#Region " Height property "

	Public Property Height As Double = 1

#End Region


#Region " WidthFactor property "

	Public Property WidthFactor As Double = 1

#End Region


#Region " IValueConverter implementation "

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
		Dim pos As Double
		If TypeOf value Is TimeSpan Then
			pos = CType(value, TimeSpan).TotalMilliseconds
		Else
			pos = CDbl(value)
		End If

		Return New RectangleGeometry(New Rect(0, 0, Math.Max(pos * WidthFactor, 0), Math.Max(Height, 0)))
	End Function


	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
		Throw New NotImplementedException()
	End Function

#End Region


#Region " IMultiValueConverter implementation "

	Public Function ConvertMulti(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
		If values.Any(Function(v) v Is DependencyProperty.UnsetValue) Then
			Return Binding.DoNothing
		End If

		If values.Count < 5 Then Throw New ArgumentException("ClipRectConverter must have 5 values")
		Dim actualValue = CDbl(values(0))
		Dim minValue = CDbl(values(1))
		Dim maxValue = CDbl(values(2))
		Dim relValue = If((maxValue - minValue) = 0, actualValue - minValue, (actualValue - minValue) / (maxValue - minValue))

		Dim maxWidth = Math.Max(CDbl(values(3)), 0)
		Dim maxHeight = CDbl(values(4))

		Return New RectangleGeometry(New Rect(0, 0, relValue * maxWidth * WidthFactor, maxHeight))
	End Function


	Public Function ConvertMultiBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
		Throw New NotImplementedException()
	End Function

#End Region

End Class
