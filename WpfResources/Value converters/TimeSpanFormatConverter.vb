''' <summary>
''' Convert TimeSpan to String.
''' Format is defined by ConverterParameter. There,
'''  {0} - The value
'''  {1} - Minus if the TimeSpan is negative, nothing otherwise
'''  {2} - Minus if the TimeSpan is negative, plus otherwise
''' </summary>
''' <remarks></remarks>
<ValueConversion(GetType(TimeSpan), GetType(String), ParameterType:=GetType(String))>
Public Class TimeSpanFormatConverter
    Implements IValueConverter

#Region " IValueConverter implementation "

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        If Not TypeOf value Is TimeSpan Then
            Throw New ArgumentException("Must be TimeSpan")
        End If

        Dim ts = CType(value, TimeSpan)
        Return TimeSpanToString(ts, parameter)
    End Function


    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Dim ts As TimeSpan

        Dim vstr = CStr(value)
        While vstr.Count(Function(c) c = ":"c) < 2
            vstr = "00:" & vstr
        End While

        If TimeSpan.TryParse(vstr, ts) Then
            Return ts
        Else
            Return Binding.DoNothing
        End If
    End Function

#End Region


#Region " Actual conversion "

    Public Shared Function TimeSpanToString(ts As TimeSpan, fmt As Object) As String
        Dim signAlways = If(ts.TotalMilliseconds < 0, "-", "+")
        Dim signNeg = If(ts.TotalMilliseconds < 0, "-", String.Empty)
        Dim fmtStr = If(fmt IsNot Nothing, CStr(fmt),
                     If(ts.TotalHours >= 1, "{1}{0:hh\:mm\:ss\.fff}",
                     "{1}{0:mm\:ss\.fff}"))

        Return String.Format(fmtStr, ts, signNeg, signAlways)
    End Function

#End Region

End Class
