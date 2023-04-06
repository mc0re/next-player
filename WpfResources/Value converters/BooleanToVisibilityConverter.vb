Imports System.Globalization


<ValueConversion(GetType(Boolean), GetType(Visibility), ParameterType:=GetType(String))>
<ValueConversion(GetType(Object), GetType(Visibility), ParameterType:=GetType(String))>
Public Class BooleanToVisibilityConverter
    Implements IValueConverter, IMultiValueConverter

#Region " Properties "

    Public Property TrueValue As Visibility = Visibility.Visible


    Public Property FalseValue As Visibility = Visibility.Collapsed

#End Region


#Region " IValueConverter implementation "

    <CodeAnalysis.SuppressMessage("Design", "CC0021:You should use nameof instead of the parameter element name string", Justification:="Style is not a name")>
    <CodeAnalysis.SuppressMessage("Style", "CC0013:Use Ternary operator.", Justification:="To avoid casting issues")>
    <CodeAnalysis.SuppressMessage("Style", "CC0014:Use Ternary operator.", Justification:="If is more readable here")>
    Public Function Convert(value As Object, targetType As System.Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
        Dim neg = (String.Compare(CStr(parameter), "Not", True) = 0)
        Dim boolVal As Boolean

        boolVal = ToBool(value)

        Return If(boolVal Xor neg, TrueValue, FalseValue)
    End Function


    Public Function ConvertBack(value As Object, targetType As System.Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function

#End Region


#Region " IMultiValueConverter implementation "

    Public Function ConvertMulti(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
        If values Is DependencyProperty.UnsetValue Then Return Binding.DoNothing

        Dim allTrue = values.All(Function(v) ToBool(v))
        Return If(allTrue, TrueValue, FalseValue)
    End Function


    Public Function ConvertMultiBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotImplementedException()
    End Function

#End Region


#Region " Utility "

    Private Shared Function ToBool(value As Object) As Boolean
        Dim boolVal As Boolean

        If TypeOf value Is Boolean Then
            boolVal = CBool(value)
        ElseIf TypeOf value Is Double Then
            boolVal = CDbl(value) <> 0
        ElseIf TypeOf value Is Integer Then
            boolVal = CInt(value) <> 0
        Else
            boolVal = value IsNot Nothing
        End If

        Return boolVal
    End Function

#End Region

End Class
