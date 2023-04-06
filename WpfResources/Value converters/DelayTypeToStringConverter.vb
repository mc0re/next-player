Imports System.Globalization
Imports AudioPlayerLibrary


Public Class DelayTypeToStringConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If value Is DependencyProperty.UnsetValue Then Return Binding.DoNothing

        Dim dt = CType(value, DelayTypes)

        Select Case dt
            Case DelayTypes.TimedFromStart
                Return "after start of"
            Case DelayTypes.TimedBeforeEnd
                Return "before end of"
            Case DelayTypes.TimedAfterEnd
                Return "after end of"
            Case Else
                Throw New ArgumentException($"Unknown delay type '{dt}'.")
        End Select
    End Function


    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function

End Class
