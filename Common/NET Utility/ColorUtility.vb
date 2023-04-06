Imports System.Windows.Media
Imports System.Windows


''' <summary>
''' How the colour is modified to be lighter / darker.
''' </summary>
Public Enum ColorModeType
	' ReSharper disable InconsistentNaming
	RGB

	HSL
	' ReSharper restore InconsistentNaming

	Opacity
End Enum


Public Module ColorUtility

#Region " Colour conversions "

	' ReSharper disable once MemberCanBePrivate.Global

	''' <summary>
	''' Converts WinForms System.Drawing.Color to WPF System.Windows.Media.Color.
	''' </summary>
	Public Function DrawingColorToWpfColor(drawingColor As System.Drawing.Color) As Color
		Return Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B)
	End Function


	' ReSharper disable once MemberCanBeInternal

	''' <summary>
	''' Converts WPF System.Windows.Media.Color to WinForms System.Drawing.Color.
	''' </summary>
	Public Function WpfColorToDrawingColor(wpfColor As Color) As System.Drawing.Color
		Return System.Drawing.Color.FromArgb(wpfColor.A, wpfColor.R, wpfColor.G, wpfColor.B)
	End Function


	''' <summary>
	''' Returns the Color which is represented by the name passed.
	''' </summary>
	''' <param name="colorName">The name of the Color to retrieve.</param>
	''' <returns>Nullable color - either the found Color or null.</returns>
	<CodeAnalysis.SuppressMessage("Design", "CC0004:Catch block cannot be empty", Justification:="Swallow exception")>
	Public Function GetColorByName(colorName As String) As Color
		Dim clrResult = Colors.Black

		Try
			Dim value As Object = ColorConverter.ConvertFromString(colorName)
			If value IsNot Nothing Then
				clrResult = DirectCast(value, Color)
			End If
		Catch ex As FormatException
			' Incorrect color definition - ignore
		End Try

		Return clrResult
	End Function


	''' <summary>
	''' Convert any color type (String, System.Drawing.Color or System.Windows.Media.Color) to System.Windows.Media.Color.
	''' </summary>
	Public Function ColorToWpfColor(value As Object) As Color
		Dim clr As Color

		If value Is DependencyProperty.UnsetValue Then
			' This can happen, for instance, during ClearData
			Return Colors.Transparent
		ElseIf TypeOf value Is String Then
			clr = GetColorByName(value.ToString())
		ElseIf TypeOf value Is System.Drawing.Color Then
			clr = DrawingColorToWpfColor(CType(value, System.Drawing.Color))
		ElseIf TypeOf value Is Color Then
			clr = CType(value, Color)
		Else
			Throw New ArgumentException("Wrong parameter type to ColorToWpfColor")
		End If

		Return clr
	End Function

#End Region


#Region " Utility functions "

	''' <summary>
	''' Return the value trimmed to Byte type.
	''' </summary>
	Private Function TrimToByte(value As Double) As Byte
		If value < Byte.MinValue Then value = Byte.MinValue
		If value > Byte.MaxValue Then value = Byte.MaxValue
		Return CByte(value)
	End Function


	''' <summary>
	''' Return the value trimmed to 0..1 range.
	''' </summary>
	Private Function TrimToPercent(value As Double) As Double
		If value < 0 Then value = 0
		If value > 1 Then value = 1
		Return value
	End Function


	''' <summary>
	''' Return a linear approximation between two Byte values.
	''' </summary>
	Private Function AdjustByte(orig As Byte, percent As Double) As Byte
		Return TrimToByte(orig + CInt(Byte.MaxValue * percent))
	End Function

#End Region


#Region " Colour adjustment functions "


	''' <summary>
	''' Change a colour to make it lighter or darker.
	''' The used mode is HSL.
	''' </summary>
	''' <param name="wpfColor">Original colour</param>
	''' <param name="percent">-100% is very dark (black), +100% is very light (white).
	''' The whole scala (0-255) is 100%, percentage is taken from this.
	''' </param>
	Public Function ChangeColor(wpfColor As Color, percent As Double) As Color
		Return ChangeColor(wpfColor, percent, ColorModeType.HSL)
	End Function


	''' <summary>
	''' Change a colour to make it lighter or darker.
	''' </summary>
	''' <param name="wpfColor">Original colour</param>
	''' <param name="percent">-100% is very dark (black), +100% is very light (white).
	''' The whole scala (0-255) is 100%, percentage is taken from this.
	''' </param>
	''' <param name="mode">Conversion mode.</param>
	Public Function ChangeColor(wpfColor As Color, percent As Double,
	mode As ColorModeType) As Color

		Select Case mode
			Case ColorModeType.RGB
				Return ChangeColorRgb(wpfColor, percent)

			Case ColorModeType.HSL
				Return ChangeColorHsl(wpfColor, percent)

			Case Else ' ColorModeType.Opacity
				Return ChangeColorOpacity(wpfColor, percent)
		End Select
	End Function


	''' <summary>
	''' RGB mode: find a "line" from orig to 000000 or FFFFFF and modify the values linearly.
	''' Alpha is kept intact.
	''' </summary>
	Private Function ChangeColorRgb(orig As Color, percent As Double) As Color
		Dim newR = AdjustByte(orig.R, percent)
		Dim newG = AdjustByte(orig.G, percent)
		Dim newB = AdjustByte(orig.B, percent)
		Return Color.FromArgb(orig.A, newR, newG, newB)
	End Function


	''' <summary>
	''' HSL mode: convert RGB to HSL, adjust L and convert back.
	''' Alpha is kept intact.
	''' </summary>
	Private Function ChangeColorHsl(orig As Color, percent As Double) As Color
		Dim origHsl = ColorModelHSL.FromRgb(orig.R, orig.G, orig.B)
		origHsl.Lightness = TrimToPercent(origHsl.Lightness + percent)

		Dim res = origHsl.ToColor(orig.A)
		Return res
	End Function


	''' <summary>
	''' Opacity mode: modify only Alpha channel.
	''' </summary>
	Private Function ChangeColorOpacity(orig As Color, percent As Double) As Color
		Dim newA = AdjustByte(orig.A, percent)
		Return Color.FromArgb(newA, orig.R, orig.G, orig.B)
	End Function

#End Region


#Region " Colour interpolation functions "


	''' <summary>
	''' Interpolate (animate) between two colours.
	''' The mode used is RGB.
	''' </summary>
	''' <param name="colorA">From-color</param>
	''' <param name="colorB">To-color</param>
	''' <param name="progress">0 = A, 1 = B, number in (0..1) - in between A and B</param>
	Public Function InterpolateColor(colorA As Color, colorB As Color, progress As Double) As Color
		Return InterpolateColor(colorA, colorB, progress, ColorModeType.RGB)
	End Function


	''' <summary>
	''' Interpolate (animate) between two colours.
	''' </summary>
	''' <param name="colorA">From-color</param>
	''' <param name="colorB">To-color</param>
	''' <param name="progress">0 = A, 1 = B, number in (0..1) - in between A and B</param>
	''' <param name="mode">Conversion mode.</param>
	Public Function InterpolateColor(colorA As Color,
	colorB As Color, progress As Double,
	mode As ColorModeType) As Color

		Select Case mode
			Case ColorModeType.RGB
				Return InterpolateColorRgb(colorA, colorB, progress)

			Case ColorModeType.HSL
				Return InterpolateColorHsl(colorA, colorB, progress)

			Case Else ' ColorModeType.Opacity
				Return InterpolateColorRgb(colorA, colorB, progress)
		End Select
	End Function


	''' <summary>
	''' Interpolate (animate) between two colours in RGB space.
	''' </summary>
	''' <param name="colorA">From-color</param>
	''' <param name="colorB">To-color</param>
	''' <param name="progress">0 = A, 1 = B, number in (0..1) - in between A and B</param>
	Private Function InterpolateColorRgb(colorA As Color,
	colorB As Color, progress As Double) _
	As Color

		Dim resA = TrimToByte(LinearInterpolation(colorA.A, colorB.A, progress))
		Dim resR = TrimToByte(LinearInterpolation(colorA.R, colorB.R, progress))
		Dim resG = TrimToByte(LinearInterpolation(colorA.G, colorB.G, progress))
		Dim resB = TrimToByte(LinearInterpolation(colorA.B, colorB.B, progress))

		Return Color.FromArgb(resA, resR, resG, resB)
	End Function


	''' <summary>
	''' Interpolate (animate) between two colours in HSL space.
	''' </summary>
	''' <param name="colorA">From-color</param>
	''' <param name="colorB">To-color</param>
	''' <param name="progress">0 = A, 1 = B, number in (0..1) - in between A and B</param>
	Private Function InterpolateColorHsl(colorA As Color,
	colorB As Color, progress As Double) _
	As Color

		Dim resA = LinearInterpolation(colorA.A, colorB.A, progress)
		Dim hslA = ColorModelHSL.FromRgb(colorA.R, colorA.G, colorA.B)
		Dim hslB = ColorModelHSL.FromRgb(colorB.R, colorB.G, colorB.B)

		If hslA.Hue - hslB.Hue > 180 Then
			hslB.Hue += 360
		ElseIf hslB.Hue - hslA.Hue > 180 Then
			hslA.Hue += 360
		End If

		Dim resH = LinearInterpolation(hslA.Hue, hslB.Hue, progress)
		If resH < 0 Then
			resH += 360
		ElseIf resH >= 360 Then
			resH -= 360
		End If

		Dim resS = LinearInterpolation(hslA.Saturation, hslB.Saturation, progress)
		Dim resL = LinearInterpolation(hslA.Lightness, hslB.Lightness, progress)

		Dim resHsl = New ColorModelHSL() With {.Hue = resH, .Saturation = resS, .Lightness = resL}
		Return resHsl.ToColor(TrimToByte(resA))
	End Function

#End Region

End Module
