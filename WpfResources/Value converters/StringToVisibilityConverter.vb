Imports System.Globalization


Public Class StringToVisibilityConverter
    Implements IValueConverter

#Region " Properties "

    Public Property EmptyValue As Visibility = Visibility.Collapsed

    Public Property NonEmptyValue As Visibility = Visibility.Visible

#End Region


#Region " IValueConverter implementation "

    Public Function Convert(value As Object, targetType As System.Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim str = CStr(value)
        If String.IsNullOrEmpty(str) Then
            Return EmptyValue
        Else
            Return NonEmptyValue
        End If
    End Function


    Public Function ConvertBack(value As Object, targetType As System.Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function

#End Region

End Class
