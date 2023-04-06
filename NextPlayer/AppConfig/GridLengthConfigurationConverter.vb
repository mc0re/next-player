Imports System.ComponentModel
Imports System.Configuration
Imports System.Globalization


''' <summary>
''' Convert GridLength from and to String.
''' </summary>
Public Class GridLengthConfigurationConverter
	Inherits ConfigurationConverterBase

	Public Property DefaultLength As Double = 300


	''' <summary>
	''' From string.
	''' </summary>
	Public Overrides Function ConvertFrom(
		context As ITypeDescriptorContext,
		culture As CultureInfo,
		value As Object
	) As Object

		Dim str = CStr(value)
		Return If(str = "Auto",
				New GridLength(DefaultLength),
				New GridLength(Double.Parse(str)))
	End Function


	''' <summary>
	''' To string.
	''' </summary>
	Public Overrides Function ConvertTo(
		context As ITypeDescriptorContext,
		culture As CultureInfo,
		value As Object,
		destinationType As Type
	) As Object

		Dim len = CType(value, GridLength)
		Return If(len.IsAbsolute, len.Value.ToString(), "Auto")
	End Function

End Class
