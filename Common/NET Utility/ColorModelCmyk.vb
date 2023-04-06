Imports System.Windows.Media


' ReSharper disable once InconsistentNaming

''' <summary>
''' CMYK storage and conversion.
''' </summary>
''' <remarks>
''' See http://www.easycalculation.com/colorconverter/rgb-color-converter.php
''' and ImageMagick source code.
''' 
''' CMYK color examples:
'''   Black   (0,0,0,1)
'''   Blue    (1,1,0,0)
'''   Cyan    (1,0,0,0)
'''   Green   (1,0,1,0)
'''   Magenta (0,1,0,0)
'''   Red     (0,1,1,0)
'''   White   (0,0,0,0)
'''   Yellow  (0,0,1,0)
''' </remarks>
<CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId:="CMYK")>
Public Structure ColorModelCMYK

#Region " Public fields "

	' 0..1
	Public Property Cyan As Double

	' 0..1
	Public Property Magenta As Double

	' 0..1
	Public Property Yellow As Double

	' 0..1
	Public Property Black As Double

#End Region


#Region " Equality overrides "

	Public Overrides Function GetHashCode() As Integer
		Return Cyan.GetHashCode() Xor Magenta.GetHashCode() Xor Yellow.GetHashCode() Xor Black.GetHashCode()
	End Function


	Public Overrides Function Equals(obj As Object) As Boolean
		Dim other = DirectCast(obj, ColorModelCMYK)

		Return Cyan = other.Cyan AndAlso
			   Magenta = other.Magenta AndAlso
			   Yellow = other.Yellow AndAlso
			   Black = other.Black
	End Function


	Public Shared Operator =(value1 As ColorModelCMYK, value2 As ColorModelCMYK) As Boolean
		Return value1.Equals(value2)
	End Operator


	Public Shared Operator <>(value1 As ColorModelCMYK, value2 As ColorModelCMYK) As Boolean
		Return Not value1.Equals(value2)
	End Operator

#End Region


#Region " Conversion "

	''' <summary>
	''' Convert to RGB.
	''' </summary>
	Public Function ToColor() As Color
		' CMYK -> CMY
		' Formulas from http://orion.math.iastate.edu/burkardt/f_src/colors/colors.html
		'Dim c = Cyan + Black
		'Dim m = Magenta + Black
		'Dim y = Yellow + Black

		' Formulas from EasyRgb.com
		Dim c = Cyan * (1 - Black) + Black
		Dim m = Magenta * (1 - Black) + Black
		Dim y = Yellow * (1 - Black) + Black

		' CMY -> RGB
		Dim redi = Math.Round(255 * EncodePixelGamma(1 - c))
		Dim greeni = Math.Round(255 * EncodePixelGamma(1 - m))
		Dim bluei = Math.Round(255 * EncodePixelGamma(1 - y))

		Return Color.FromRgb(CByte(redi), CByte(greeni), CByte(bluei))
	End Function


	''' <summary>
	''' Convert one RGB channel from "v-compound" to "V".
	''' </summary>
	Private Shared Function EncodePixelGamma(v As Double) As Double
		Dim tmp As Double

		If v <= 0.0031306684425005883 Then
			tmp = v * 12.92
		Else
			tmp = 1.055 * v ^ (1 / 2.4) - 0.055
		End If

		Return If(tmp < 0, 0, If(tmp > 1, 1, tmp))
	End Function

#End Region

End Structure
