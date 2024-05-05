Imports System.Globalization


<ValueConversion(GetType(Double), GetType(Thickness))>
Public Class DoubleToMarginConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If value Is DependencyProperty.UnsetValue Then Return value

        Dim margin = CDbl(value)
        Return New Thickness(margin)
    End Function


    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function

End Class
