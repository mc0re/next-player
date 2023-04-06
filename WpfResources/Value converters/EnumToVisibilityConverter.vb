Imports System.Globalization


''' <summary>
''' Compare the bound value to the one defined by parameter.
''' </summary>
<ValueConversion(GetType([Enum]), GetType(Visibility), ParameterType:=GetType(String))>
Public Class EnumToVisibilityConverter
    Implements IValueConverter

#Region " Properties "

    Public Property MatchValue As Visibility = Visibility.Visible


    Public Property NonMatchValue As Visibility = Visibility.Collapsed

#End Region


#Region " IValueConverter implementation "

    Public Function Convert(value As Object, targetType As System.Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim enumValue = [Enum].Parse(value.GetType(), CStr(parameter))

        If value.Equals(enumValue) Then
            Return MatchValue
        Else
            Return NonMatchValue
        End If
    End Function


    Public Function ConvertBack(value As Object, targetType As System.Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function

#End Region

End Class
