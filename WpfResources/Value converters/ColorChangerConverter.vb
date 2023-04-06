Imports System.Globalization
Imports System.Windows.Media
Imports Common


''' <summary>
''' Make a colour darker or lighter.
''' </summary>
''' <remarks>
''' As a single-value converter, it accepts a parameter of type Single, value -1.0..+1.0 (0..-1 is darker, 0..+1 is lighter).
''' As a multi-value converter, the change percentage comes as a second value.
''' If the percent is not specified (omitted), value from ChangePercent property is used (default +0.2).
''' </remarks>
<ValueConversion(GetType(System.Windows.Media.Color), GetType(System.Windows.Media.Color), ParameterType:=GetType(Single))>
<ValueConversion(GetType(System.Drawing.Color), GetType(System.Windows.Media.Color), ParameterType:=GetType(Single))>
<ValueConversion(GetType(System.Windows.Media.Color), GetType(System.Windows.Media.Brush), ParameterType:=GetType(Single))>
<ValueConversion(GetType(System.Drawing.Color), GetType(System.Windows.Media.Brush), ParameterType:=GetType(Single))>
Public Class ColorChangerConverter
Implements IValueConverter, IMultiValueConverter

#Region " Properties "

	''' <summary>
	''' Default change value.
	''' </summary>
	Public Property ChangePercent As Double = 0

#End Region


#Region " IValueConverter implementation "

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
		Dim orig = ColorToWpfColor(value)
		Dim percent = If(parameter Is Nothing, ChangePercent, ParseDouble(CStr(parameter)))

		Return ChangeColorByPercent(orig, percent, targetType)
	End Function


	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
		Dim orig = ColorToWpfColor(value)
		Dim percent = -If(parameter Is Nothing, ChangePercent, ParseDouble(CStr(parameter)))

		Return ChangeColorByPercent(orig, percent, targetType)
	End Function

#End Region


#Region " IMultiValueConverter implementation "

	Public Function ConvertMultiple(values() As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements Windows.Data.IMultiValueConverter.Convert
		If values Is Nothing Then Throw New ArgumentNullException(NameOf(values), NameOf(ColorChangerConverter))
		Dim orig = ColorToWpfColor(values(0))
		Dim percent = If(values.Count = 1, ChangePercent, ParseDouble(CStr(values(1))))

		Return ChangeColorByPercent(orig, percent, targetType)
	End Function


	<CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId:=NameOf(ColorChangerConverter))>
	Public Function ConvertMultipleBack(value As Object, targetTypes() As Type, parameter As Object, culture As CultureInfo) As Object() Implements Windows.Data.IMultiValueConverter.ConvertBack
		Throw New NotImplementedException("ColorChangerConverter conversion not reversible")
	End Function

#End Region


#Region " Actual functionality "

	Private Shared Function ChangeColorByPercent(orig As Color, percent As Double, targetType As Type) As Object
		Dim clr = ChangeColor(orig, percent)

		Select Case targetType
			Case GetType(Color)
				Return clr

			Case GetType(Brush)
				Return New SolidColorBrush(clr)

			Case Else
				Throw New ArgumentException("Unknown target type for ColorChangerConverter")
		End Select
	End Function

#End Region

End Class
