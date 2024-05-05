Imports System.Globalization
Imports System.Windows.Media
Imports PlayerActions


''' <summary>
''' Parameters:
''' - Points (collection of AutomationPoint)
''' - HasDuration flag
''' - Duration
''' - NoDurationFactor
''' </summary>
''' <remarks>
''' As Y=0 is top and Y=1 bottom, they need to be reversed in UI.
''' This is done by ScaleTransform(ScaleY = -1).
''' </remarks>
<ValueConversion(GetType(IEnumerable(Of AutomationPoint)), GetType(Geometry))>
Public Class PointsToPathConverter
	Implements IMultiValueConverter

#Region " DefaultDuration property "

	''' <summary>
	''' Used when there is no duration and no points or just one point.
	''' </summary>
	Public Property DefaultDuration As TimeSpan = TimeSpan.FromSeconds(1)

#End Region


#Region " Duration calculator "

	Private Shared ReadOnly MaxPosConv As New MaxPositionConverter()

#End Region


#Region " IMultiValueConverter implementation "

	Public Function Convert(values As Object(), targetType As Type, parameter As Object, culture As CultureInfo) As Object Implements IMultiValueConverter.Convert
		' Get arguments
		If values.Count < 4 Then Throw New ArgumentException("PointsToPathConverter must have 3 arguments")

		MaxPosConv.DefaultMaximum = DefaultDuration.TotalMilliseconds

		Dim lastXobj = MaxPosConv.Convert(values, targetType, parameter, culture)
		If lastXobj Is Binding.DoNothing Then
			' This covers all 
			Return Binding.DoNothing
		End If

		Dim pointList = TryCast(values(0), IEnumerable(Of AutomationPoint)).ToList()

		' Start with the first point.
		' If no points, (0,1) is the default.
		Dim firstPoint = If(pointList.Any(), New Point(pointList.First().X, pointList.First().Y), New Point(0, 1))
		Dim lastPt = firstPoint
		Dim lastX = CDbl(lastXobj)

		Dim lines As New List(Of LineSegment)()
		lines.Add(New LineSegment(firstPoint, False))

		For Each nextPt In pointList.Skip(1).Select(Function(ap) New Point(ap.X, ap.Y))
			Dim revPt = nextPt
			If revPt.X > lastX Then Exit For

			lines.Add(New LineSegment(revPt, True))
			lastPt = revPt
		Next

		' Continue the line until the end of canvas.
		lines.Add(New LineSegment(New Point(lastX, lastPt.Y), True))

		' Make sure the line meets Y=0.
		If lastPt.Y <> 0 Then
			lines.Add(New LineSegment(New Point(lastX, 0), False))
		End If

		Dim geom As New PathGeometry()
		geom.Figures.Add(New PathFigure(New Point(0, 0), lines, True))

		Return geom
	End Function


	Public Function ConvertBack(value As Object, targetTypes As Type(), parameter As Object, culture As CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
		Throw New NotImplementedException()
	End Function

#End Region

End Class
