''' <summary>
''' Sum up all given values.
''' </summary>
Public Class SumConverter
    Implements IMultiValueConverter

#Region " IMultiValueConverter implementation "

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IMultiValueConverter.Convert
        Dim sum = 0L

        For Each value In values
            If TypeOf value Is Integer Then
                sum += CInt(value)
            ElseIf TypeOf value Is Long Then
                sum += CLng(value)
            Else
                Continue For
            End If
        Next

        Return sum
    End Function


    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As Globalization.CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotImplementedException("SumConverter cannot convert back")
    End Function

#End Region

End Class
