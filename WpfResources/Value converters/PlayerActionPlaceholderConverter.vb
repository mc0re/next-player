Imports System.Globalization
Imports PlayerActions


''' <summary>
''' Return a default object, if the bound value is Nothing.
''' </summary>
<ValueConversion(GetType(PlayerAction), GetType(PlayerAction))>
Public Class PlayerActionPlaceholderConverter
	Implements IValueConverter

#Region " IValueConverter implementation "

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
		If value Is Nothing Then
			Return PlayerAction.PlaceHolder
		Else
			Return value
		End If
	End Function


	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
		Throw New NotImplementedException()
	End Function

#End Region

End Class
