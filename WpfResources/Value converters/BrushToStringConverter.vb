Imports System.Globalization
Imports System.Windows.Media


''' <summary>
''' Two-way conversion between a Brush and its string representation.
''' </summary>
<ValueConversion(GetType(Brush), GetType(String))>
Public Class BrushToStringConverter
	Implements IValueConverter

#Region " Utility functions "

	''' <summary>
	''' Convert a brush to a color, if possible.
	''' </summary>
	Private Shared Function ConvertToColor(br As Brush) As Color
		Dim sbr = TryCast(br, SolidColorBrush)

		If sbr IsNot Nothing Then
			Return sbr.Color
		End If

		Dim lbr = TryCast(br, LinearGradientBrush)

		If lbr IsNot Nothing Then
			Return lbr.GradientStops(0).Color
		End If

		Return Colors.Transparent
	End Function


	''' <summary>
	''' Convert a brush to a string, if possible.
	''' </summary>
	Private Shared Function ConvertToString(br As Brush) As String
		Dim sbr = TryCast(br, SolidColorBrush)

		If sbr IsNot Nothing Then
			' Returns format #aarrggbb
			Return sbr.Color.ToString()
		End If

		Dim lbr = TryCast(br, LinearGradientBrush)

		If lbr IsNot Nothing Then
			Dim str = String.Join(" ", From grad In lbr.GradientStops Select grad.Color)
			Return str
		End If

		Return String.Empty
	End Function


	''' <summary>
	''' Convert Color to brush.
	''' </summary>
	Private Shared Function ConvertFromColor(clr As Color) As Brush
		Return New SolidColorBrush(clr)
	End Function


	''' <summary>
	''' Convert String to brush.
	''' </summary>
	Public Shared Function ConvertFromString(inputValue As String) As Brush
		If String.IsNullOrEmpty(inputValue) Then Throw New ArgumentNullException(NameOf(inputValue))

		If {"-"c, "/"c, "|"c, "\"c}.Contains(inputValue(0)) Then
			' Linear gradient
			Dim colorList =
				From clr In inputValue.Substring(1).Split(" "c)
				Select CType(ColorConverter.ConvertFromString(clr), Color)

			Dim br = New LinearGradientBrush()
			Select Case inputValue(0)
				Case "-"c
					br.StartPoint = New Point(0, 0.5)
					br.EndPoint = New Point(1, 0.5)
				Case "/"c
					br.StartPoint = New Point(0, 1)
					br.EndPoint = New Point(1, 0)
				Case "|"c
					br.StartPoint = New Point(0.5, 0)
					br.EndPoint = New Point(0.5, 1)
				Case "\"c
					br.StartPoint = New Point(0, 0)
					br.EndPoint = New Point(1, 1)
			End Select

			Dim nofIntervals = colorList.Count - 1.0
			Dim idx = 0
			For Each clr In colorList
				br.GradientStops.Add(New GradientStop(clr, idx / nofIntervals))
				idx += 1
			Next

			Return br
		Else
			' Single colour
			Dim colorArgb = CType(ColorConverter.ConvertFromString(inputValue), Color)
			Dim sbr = New SolidColorBrush(colorArgb)
			Return sbr
		End If
	End Function

#End Region


#Region " IValueConverter implementation "

	Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.Convert
		Dim br = TryCast(value, Brush)
		If br Is Nothing Then Return String.Empty

		Return ConvertToString(br)
	End Function


	Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IValueConverter.ConvertBack
		Return ConvertFromString(CStr(value))
	End Function

#End Region

End Class
