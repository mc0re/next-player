''' <summary>
''' Parameters are:
''' - Original coordinate (0-1 for Y, 0+ for X)
''' - Maximum value for that coordinate
''' - Maximum value for the result
''' - Twice the offset (to center)
''' </summary>
''' <remarks></remarks>
<ValueConversion(GetType(Double), GetType(Double), ParameterType:=GetType(String))>
Public Class ScaleCoordinateConverter
	Implements IMultiValueConverter

#Region " IMultiValueConverter implementation "

	Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IMultiValueConverter.Convert
		' Sanity check
		For Each v In values
			If v Is DependencyProperty.UnsetValue Then
				Return Binding.DoNothing
			End If
		Next

		Dim c = CDbl(values(0))
		Dim maxIn As Double

		If TypeOf values(1) Is TimeSpan Then
			maxIn = CType(values(1), TimeSpan).TotalMilliseconds
		Else
			maxIn = CDbl(values(1))
		End If

		Dim maxOut = CDbl(values(2))
		Dim offset = CDbl(values(3))

		If maxIn = 0 Then
			Return c
		End If

		Return c / maxIn * maxOut - offset / 2
	End Function


	Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As Globalization.CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
		Throw New NotImplementedException()
	End Function

#End Region

End Class
