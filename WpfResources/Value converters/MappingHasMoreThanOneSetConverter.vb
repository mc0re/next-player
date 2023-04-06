Imports System.Globalization
Imports AudioChannelLibrary


Public Class MappingHasMoreThanOneSetConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        If value Is DependencyProperty.UnsetValue Then Return Binding.DoNothing

        Dim list = TryCast(value, IList(Of AudioChannelMappingItem))
        If list Is Nothing Then Throw New ArgumentException("Expecting a list of AudioChannelMappingItem")

        Return list.Where(Function(m) m.IsSet).Count > 1
    End Function


    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function

End Class
