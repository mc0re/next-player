Imports System.Globalization
Imports Common
Imports PlayerActions


''' <summary>
''' Parameters:
''' - Points (collection of AutomationPoint)
''' - HasDuration flag
''' - Duration
''' - NoDurationFactor
''' </summary>
<ValueConversion(GetType(IEnumerable(Of AutomationPoint)), GetType(Double))>
Public Class MaxPositionConverter
	Implements IMultiValueConverter

#Region " Properties "

	''' <summary>
	''' Maximum duration (in milliseconds).
	''' </summary>
	Public Property DefaultMaximum As Double = 1 * MillisecondsInSecond

#End Region


#Region " IMultiValueConverter implementation "

	Public Function Convert(values As Object(), targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
		If values.Count < 4 Then Throw New ArgumentException("MaxPositionConverter must have 3 arguments")

		' Check for UnsetValue
		For Each v In values
			If v Is DependencyProperty.UnsetValue Then
				Return Binding.DoNothing
			End If
		Next

		Dim pointList = TryCast(values(0), IEnumerable(Of AutomationPoint))
		If pointList Is Nothing Then Return Binding.DoNothing

		Dim hasDuration = CBool(values(1))
		Dim duration = CType(values(2), TimeSpan).TotalMilliseconds()
		Dim noDurationFactor = CDbl(values(3))

		Dim calcDur = CalculateDuration(hasDuration, duration, noDurationFactor, pointList)
		If targetType Is GetType(TimeSpan) Then
			Return TimeSpan.FromMilliseconds(calcDur)
		Else
			Return calcDur
		End If
	End Function


	Public Function ConvertBack(value As Object, targetTypes As Type(), parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
		Throw New NotImplementedException()
	End Function

#End Region


#Region " Conversion "

	Private Function CalculateDuration(
		hasDuration As Boolean,
		duration As Double,
		noDurationFactor As Double,
		pointList As IEnumerable(Of AutomationPoint)
	) As Double

		If hasDuration Then
			Return Math.Max(duration, DefaultMaximum)
		ElseIf Not pointList.Any() Then
			Return DefaultMaximum
		Else
			Dim maxX = (From pt In pointList Select pt.X).Max()
			Return Math.Max(maxX * noDurationFactor, DefaultMaximum)
		End If
	End Function

#End Region

End Class
