Imports System.Windows.Media
Imports System.Globalization


''' <summary>
''' HSL storage and conversion routines.
''' See http://en.wikipedia.org/wiki/HSL_color_space
''' </summary>
<CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId:="HSL")>
Public Structure ColorModelHsl

#Region " Public properties "

    ' In degrees, 0..359
    Public Property Hue As Double

    ' 0..1
    Public Property Saturation As Double

    ' 0..1
    Public Property Lightness As Double

#End Region


#Region " Equality overrides "

    Public Overrides Function GetHashCode() As Integer
        Return Hue.GetHashCode() Xor Saturation.GetHashCode() Xor Lightness.GetHashCode()
    End Function


    Public Overrides Function Equals(obj As Object) As Boolean
        Dim other = DirectCast(obj, ColorModelHsl)

        Return IsEqual(Hue, other.Hue) AndAlso
               IsEqual(Saturation, other.Saturation) AndAlso
               IsEqual(Lightness, other.Lightness)
    End Function


    Public Shared Operator =(value1 As ColorModelHsl, value2 As ColorModelHsl) As Boolean
        Return value1.Equals(value2)
    End Operator


    Public Shared Operator <>(value1 As ColorModelHsl, value2 As ColorModelHsl) As Boolean
        Return Not value1.Equals(value2)
    End Operator

#End Region


#Region " Conversion "

    ''' <summary>
    ''' Create an HSL structure from RGB.
    ''' </summary>
    <CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId:="Rgb")>
    <CodeAnalysis.SuppressMessage("Style", "CC0014:Use Ternary operator.", Justification:="'If blue > green' is more understandible here")>
    Public Shared Function FromRgb(red As Byte, green As Byte, blue As Byte) As ColorModelHsl
        Dim res As ColorModelHsl
        Dim max = Math.Max(Math.Max(red, green), blue)
        Dim min = Math.Min(Math.Min(red, green), blue)

        Dim chroma = max - min
        res.Lightness = (CInt(max) + min) / 510  ' Maximum 1.0

        If chroma <> 0 Then
            res.Saturation = chroma / (1 - Math.Abs(2 * res.Lightness - 1)) / 255
            Dim q = 60 / chroma

            Select Case max
                Case red
                    If blue > green Then
                        res.Hue = q * (CInt(green) - blue) + 360
                    Else
                        res.Hue = q * (green - blue)
                    End If

                Case green
                    res.Hue = q * (CInt(blue) - red) + 120

                Case blue
                    res.Hue = q * (CInt(red) - green) + 240
            End Select
        End If

        Return res
    End Function


    ''' <summary>
    ''' Convert HSL to RGB with the given Alpha.
    ''' </summary>
    Public Function ToColor(alpha As Byte) As Color
        Dim red As Double, green As Double, blue As Double
        Dim chroma = Saturation * (1 - Math.Abs(2 * Lightness - 1))
        Dim x = chroma * (1 - Math.Abs(Hue / 60 Mod 2 - 1))

        Select Case Hue
            Case 0 To 60
                red = chroma : green = x : blue = 0
            Case 60 To 120
                red = x : green = chroma : blue = 0
            Case 120 To 180
                red = 0 : green = chroma : blue = x
            Case 180 To 240
                red = 0 : green = x : blue = chroma
            Case 240 To 300
                red = x : green = 0 : blue = chroma
            Case 300 To 360
                red = chroma : green = 0 : blue = x
        End Select

        Dim add = Lightness - chroma / 2
        Return Color.FromArgb(alpha, CByte((red + add) * 255), CByte((green + add) * 255), CByte((blue + add) * 255))
    End Function

#End Region


#Region " ToString "

    ''' <summary>
    ''' Debugging.
    ''' </summary>
    Public Overrides Function ToString() As String
        Return String.Format(CultureInfo.InvariantCulture, "H={0}, S={1}, L={2}", Hue, Saturation, Lightness)
    End Function

#End Region

End Structure
