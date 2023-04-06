Imports System.Globalization
Imports System.Windows


Public Module ParseUtility

#Region " Culture-independant parsing "

    ''' <summary>
    ''' Parse an integer from a string in a culture-independent way.
    ''' </summary>
    ''' <exception cref="ArgumentNullException">Input string is Nothing</exception>
    ''' <exception cref="FormatException">Wrong input string</exception>
    ''' <exception cref="OverflowException">Too large a number</exception>
    <CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId:="integer")>
    Public Function ParseInteger(input As String) As Integer
		Return Integer.Parse(input, CultureInfo.InvariantCulture)
	End Function


    ''' <summary>
    ''' Get an integer from an object in a culture-independent way.
    ''' The object can be of type Integer or String, suitable for XAML.
    ''' </summary>
    ''' <exception cref="ArgumentNullException">Input string is Nothing</exception>
    ''' <exception cref="FormatException">Wrong input string</exception>
    ''' <exception cref="OverflowException">Too large a number</exception>
    <CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId:="integer")>
    Public Function GetAsInteger(input As Object) As Integer
		If input Is DependencyProperty.UnsetValue Then
			Trace.TraceWarning("Unset value passed to GetAsInteger")
			Return 0
		ElseIf TypeOf input Is Integer Then
			Return CInt(input)
		Else
			Return ParseInteger(CStr(input))
		End If
	End Function


	''' <summary>
	''' Parse a double from a string in a culture-independent way.
	''' </summary>
	Public Function ParseDouble(input As String) As Double
		Return Double.Parse(input, CultureInfo.InvariantCulture)
	End Function


	''' <summary>
	''' Parse a byte from a string in a culture-independent way.
	''' </summary>
	Public Function ParseByte(input As String) As Byte
		Return Byte.Parse(input, CultureInfo.InvariantCulture)
	End Function


	''' <summary>
	''' Parse enum value name from a string.
	''' </summary>
	Public Function ParseEnum(Of T)(input As String) As T
		Return CType([Enum].Parse(GetType(T), input), T)
	End Function


	''' <summary>
	''' Convert a string or a number to Double.
	''' </summary>
	Public Function ConvertArgumentToDouble(value As Object, argumentIndex As Integer, caller As String) As Double
		If value Is DependencyProperty.UnsetValue Then
			Return 0
		ElseIf TypeOf value Is Double OrElse TypeOf value Is Integer Then
			Return CDbl(value)
		ElseIf TypeOf value Is String Then
			Return ParseDouble(CStr(value))
		Else
			Throw New ArgumentException(String.Format(CultureInfo.InvariantCulture,
				"Incorrect type of argument {0} to {1}", argumentIndex, caller))
		End If
	End Function

#End Region

End Module
