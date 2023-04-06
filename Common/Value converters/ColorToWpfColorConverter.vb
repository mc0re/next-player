Imports System.Globalization
Imports System.Windows.Data
Imports System.Windows
Imports System.Windows.Media


''' <summary>
''' Convert a System.Drawing.Color (ordinary colour type)
''' to Windows.Media.Color (WPF colour type).
''' If ConverterParameter is True, remove the alpha component (set to fully opaque).
''' In case of multi-value converter, argument #2 defines the alpha component (as 0-1).
''' </summary>
<ValueConversion(GetType(System.Drawing.Color), GetType(Color), ParameterType:=GetType(Boolean))>
<ValueConversion(GetType(System.Drawing.Color), GetType(Brush), ParameterType:=GetType(Boolean))>
<ValueConversion(GetType(Color), GetType(Brush), ParameterType:=GetType(Boolean))>
Public Class ColorToWpfColorConverter
	Implements IValueConverter, IMultiValueConverter

#Region " IValueConverter implementation "

#Disable Warning CC0108 ' You should use nameof instead of the parameter element name string
	<CodeAnalysis.SuppressMessage("Design", "CC0021:You should use nameof instead of the parameter element name string", Justification:="Style is not a name")>
	<CodeAnalysis.SuppressMessage("Style", "CC0013:Use Ternary operator.", Justification:="To avoid casting issues")>
	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
#Enable Warning CC0108 ' You should use nameof instead of the parameter element name string
		If value Is Nothing Then Return Binding.DoNothing
		Dim cOut = ColorToWpfColor(value)

		Dim removeAlpha = (parameter IsNot Nothing) AndAlso CBool(parameter)
		If removeAlpha Then
			cOut.A = Byte.MaxValue
		End If

		If targetType Is GetType(Brush) Then
			Return New SolidColorBrush(cOut)
		Else
			Return cOut
		End If
	End Function


#Disable Warning CC0108 ' You should use nameof instead of the parameter element name string
	<CodeAnalysis.SuppressMessage("Design", "CC0021:You should use nameof instead of the parameter element name string", Justification:="Style is not a name")>
	<CodeAnalysis.SuppressMessage("Style", "CC0013:Use Ternary operator.", Justification:="To avoid casting issues")>
	<CodeAnalysis.SuppressMessage("Style", "CC0014:Use Ternary operator.", Justification:="If is more readable here")>
	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
#Enable Warning CC0108 ' You should use nameof instead of the parameter element name string
		If value Is Nothing Then Return DependencyProperty.UnsetValue

		Dim clr As System.Drawing.Color

		If TypeOf value Is System.Drawing.Color Then
			clr = CType(value, System.Drawing.Color)
		Else
			clr = WpfColorToDrawingColor(CType(value, Color))
		End If

		If targetType Is GetType(Brush) Then
			Return New SolidColorBrush(ColorToWpfColor(clr))
		Else
			Return clr
		End If
	End Function

#End Region


#Region " IMultiValueConverter implementation "

#Disable Warning CC0108 ' You should use nameof instead of the parameter element name string
	<CodeAnalysis.SuppressMessage("Design", "CC0021:You should use nameof instead of the parameter element name string", Justification:="Style is not a name")>
	<CodeAnalysis.SuppressMessage("Style", "CC0013:Use Ternary operator.", Justification:="To avoid casting issues")>
	Public Function ConvertMultiple(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
#Enable Warning CC0108 ' You should use nameof instead of the parameter element name string
		If values Is Nothing Then Return Binding.DoNothing
		If values.Count <> 2 Then Throw New ArgumentException("Needs exactly two parameters", NameOf(values))

		Dim cOut = ColorToWpfColor(values(0))
		cOut.A = CByte(Byte.MaxValue * ConvertArgumentToDouble(values(1), 1, Me.GetType().Name))

		If targetType Is GetType(Brush) Then
			Return New SolidColorBrush(cOut)
		Else
			Return cOut
		End If
	End Function


	<CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId:=NameOf(ColorToWpfColorConverter))>
	Public Function ConvertMultipleBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
		Throw New NotImplementedException("ColorToWpfColorConverter doesn't convert back")
	End Function

#End Region

End Class
