Imports System.Globalization
Imports Common
Imports System.Windows.Media
Imports System.Windows.Media.Imaging


Public Module WpfUtility

#Region " Convert string to the given value "

	''' <summary>
	''' Convert a string representation of a value to the given type.
	''' </summary>
	''' <exception cref="ParseException">Parsing error occured</exception>
	Public Function ConvertValue(value As String, targetType As Type, targetParameterName As String, line As Integer, position As Integer) As Object
		If targetType Is Nothing Then Throw New ArgumentNullException(NameOf(targetType))

		Try
			If targetType = GetType(String) Then
				Return value
			ElseIf targetType.IsEnum Then
				Try
					Return [Enum].Parse(targetType, value)
				Catch ex As ArgumentException
					Dim names = [Enum].GetNames(targetType)
					Throw New ParseException(String.Format(CultureInfo.InvariantCulture, "Passed enum value '{0}' for '{1}' not recognized.", value, targetParameterName),
											 String.Format(CultureInfo.InvariantCulture, "Allowed values are: '{0}'.", String.Join("', '", names)),
											 line, position)
				End Try

			ElseIf targetType = GetType(Integer) Then
				Return ParseInteger(value)
			ElseIf targetType = GetType(Double) Then
				Return ParseDouble(value)
			ElseIf targetType = GetType(Boolean) Then
				Return Boolean.Parse(value)
			ElseIf targetType = GetType(Brush) Then
				Return BrushToStringConverter.ConvertFromString(value)
			ElseIf targetType = GetType(Thickness) Then
				Return CType(New ThicknessConverter().ConvertFromInvariantString(value), Thickness)
			End If

			Throw New ParseException(String.Format(CultureInfo.InvariantCulture, "No conversion to type '{0}'.", targetType.Name), "Contact developer.", line, position)

		Catch ex As Exception
			If TypeOf ex Is ParseException Then
				Throw
			Else
				Throw New ParseException(ex.Message, String.Format(CultureInfo.InvariantCulture, "Input: '{0}'.", value), line, position)
			End If
		End Try
	End Function

#End Region


#Region " Size functions "

	''' <summary>
	''' 2 digits are chosen based on screen coordinates precision and font calculation precision.
	''' </summary>
	Private Const NumberOfRoundingDigits = 2


	''' <summary>
	''' Compares the two screen sizes.
	''' </summary>
	''' <returns>
	'''  * -1 if sz1 &lt; sz2
	'''  * 0 if sz1 = sz2
	'''  * 1 if sz1 &gt; sz2
	''' </returns>
	''' <remarks>
	''' The function is necessary due to rounding errors during size calculations.
	''' </remarks>
	Public Function CompareSizes(size1 As Double, size2 As Double) As Integer
		Dim res = Math.Round(size1 - size2, NumberOfRoundingDigits)
		Return If(res = 0, 0, If(res < 0, -1, 1))
	End Function


	''' <summary>
	''' Perform a division of two sizes.
	''' </summary>
	''' <param name="totalSize">Nominator</param>
	''' <param name="partSize">Denominator</param>
	''' <param name="rounder">Post-processing function (Math.Floor, Math.Ceiling, ...)</param>
	''' <returns>Post-processed division, converter to Integer</returns>
	''' <remarks>
	''' Due to rounding errors, sz1 can be slightly smaller or larger than sz2
	''' (say, sz1 = 26.666666666666657 and sz2 = 26.666666666666668.
	''' Therefore we need rounding the division result.
	''' </remarks>
	Public Function DivideSizes(totalSize As Double, partSize As Double, rounder As Func(Of Double, Double)) As Integer
		If rounder Is Nothing Then Throw New ArgumentNullException(NameOf(rounder), "DivideSizes shall have a rounding function")
		Return CInt(rounder(Math.Round(totalSize / partSize, NumberOfRoundingDigits)))
	End Function


	''' <summary>
	''' Convert relative coordinates (0-1) to absolute.
	''' The resulting coordinates are rounded (for X and Y - up, for Width and Height - down).
	''' </summary>
	Public Function RelativeToAbsoluteCoordinates(absoluteSize As Size, relativeBox As Rect) As Rect
		Dim startX = Math.Ceiling(absoluteSize.Width * relativeBox.X)
		Dim startY = Math.Ceiling(absoluteSize.Height * relativeBox.Y)
		Dim w = Math.Floor(absoluteSize.Width * relativeBox.Width)
		Dim h = Math.Floor(absoluteSize.Height * relativeBox.Height)

		Return New Rect(startX, startY, w, h)
	End Function

#End Region


#Region " Visual and logical tree functions "

	''' <summary>
	''' Find the first visual child of the given type.
	''' </summary>
	Public Function FindVisualChild(Of TChildItem)(ByVal obj As DependencyObject) As TChildItem
		Dim elem = TryCast(obj, FrameworkElement)
		If elem IsNot Nothing Then
			elem.ApplyTemplate()
		End If

		For i As Integer = 0 To VisualTreeHelper.GetChildrenCount(obj) - 1
			Dim child As Object = VisualTreeHelper.GetChild(obj, i)
			If child IsNot Nothing AndAlso TypeOf child Is TChildItem Then
				Return DirectCast(child, TChildItem)
			Else
				Dim childOfChild As TChildItem = FindVisualChild(Of TChildItem)(CType(child, DependencyObject))
				If childOfChild IsNot Nothing Then
					Return childOfChild
				End If
			End If
		Next i

		Return Nothing
	End Function


	''' <summary>
	''' Find the first visual child of the given type.
	''' </summary>
	Public Function FindVisualChild(childType As Type, obj As DependencyObject) As FrameworkElement
		Dim elem = TryCast(obj, FrameworkElement)
		If elem IsNot Nothing Then
			elem.ApplyTemplate()
		End If

		For i As Integer = 0 To VisualTreeHelper.GetChildrenCount(obj) - 1
			Dim child = VisualTreeHelper.GetChild(obj, i)
			If child IsNot Nothing AndAlso child.GetType() = childType Then
				Return CType(child, FrameworkElement)
			Else
				Dim childOfChild As FrameworkElement = FindVisualChild(childType, child)
				If childOfChild IsNot Nothing Then
					Return childOfChild
				End If
			End If
		Next i

		Return Nothing
	End Function


	''' <summary>
	''' Find the first visual child of the given type.
	''' </summary>
	Public Function FindMeOrVisualChild(childType As Type, obj As DependencyObject) As FrameworkElement
		Dim elem = TryCast(obj, FrameworkElement)
		If elem IsNot Nothing Then
			elem.ApplyTemplate()
			If elem.GetType() = childType Then Return elem
		End If

		For i As Integer = 0 To VisualTreeHelper.GetChildrenCount(obj) - 1
			Dim child = VisualTreeHelper.GetChild(obj, i)
			If child IsNot Nothing AndAlso child.GetType() = childType Then
				Return CType(child, FrameworkElement)
			Else
				Dim childOfChild As FrameworkElement = FindVisualChild(childType, child)
				If childOfChild IsNot Nothing Then
					Return childOfChild
				End If
			End If
		Next i

		Return Nothing
	End Function


	''' <summary>
	''' Find the first visual child with the given condition.
	''' </summary>
	Public Function FindMeOrVisualChild(obj As DependencyObject, condition As Func(Of DependencyObject, Boolean)) As UIElement
		Dim elem = TryCast(obj, FrameworkElement)
		If elem IsNot Nothing Then
			elem.ApplyTemplate()
			If condition(elem) Then Return elem
		End If

		For i As Integer = 0 To VisualTreeHelper.GetChildrenCount(obj) - 1
			Dim child = VisualTreeHelper.GetChild(obj, i)
			If child IsNot Nothing Then
				Dim foundChild = FindMeOrVisualChild(child, condition)
				If foundChild IsNot Nothing Then
					Return foundChild
				End If
			End If
		Next i

		Return Nothing
	End Function


	''' <summary>
	''' Find the all visual children of the given type, including the parent itself.
	''' </summary>
	Public Function FindMeAndVisualChildren(Of TChildItem As DependencyObject)(parent As DependencyObject) As IEnumerable(Of TChildItem)
		Dim res As New List(Of TChildItem)

		If parent Is Nothing Then Return res

		Dim elem = TryCast(parent, FrameworkElement)
		If elem IsNot Nothing Then
			elem.ApplyTemplate()
		End If

		Dim iparent = TryCast(parent, TChildItem)
		If iparent IsNot Nothing Then
			res.Add(iparent)
		End If

		For i As Integer = 0 To VisualTreeHelper.GetChildrenCount(parent) - 1
			Dim child = VisualTreeHelper.GetChild(parent, i)
			If child IsNot Nothing Then
				res.AddRange(FindMeAndVisualChildren(Of TChildItem)(child))
			End If
		Next i

		Return res
	End Function


	''' <summary>
	''' Find all leaf visual children (that is, those without own children).
	''' </summary>
	Public Function FindLeafVisualChildren(ByVal obj As DependencyObject) As IEnumerable(Of FrameworkElement)
		Dim children As New List(Of FrameworkElement)

		For i As Integer = 0 To VisualTreeHelper.GetChildrenCount(obj) - 1
			Dim child As DependencyObject = VisualTreeHelper.GetChild(obj, i)
			If child Is Nothing Then Continue For

			If VisualTreeHelper.GetChildrenCount(child) = 0 Then
				children.Add(CType(child, FrameworkElement))
			Else
				children.AddRange(FindLeafVisualChildren(child))
			End If
		Next i

		Return children
	End Function


	''' <summary>
	''' Find a parent of a given control of the given type.
	''' </summary>
	Public Function FindVisualParent(Of T As Visual)(child As DependencyObject) As T
		If child Is Nothing Then Return Nothing

		Dim parent = VisualTreeHelper.GetParent(child)

		While parent IsNot Nothing AndAlso Not TypeOf parent Is T
			parent = VisualTreeHelper.GetParent(parent)
		End While

		Return TryCast(parent, T)
	End Function


	''' <summary>
	''' If the provided element is of type T, return it,
	''' otherwise search for visual parent of type T.
	''' </summary>
	Public Function FindMeOrParent(Of T As Visual)(child As Visual) As T
		Dim elem = TryCast(child, T)
		If elem Is Nothing Then
			elem = FindVisualParent(Of T)(child)
		End If

		Return elem
	End Function

#End Region


#Region " Image conversion functions "

	''' <summary>
	''' Convert an Image instance to ImageSource instance.
	''' </summary>
	Public Function ConvertDrawingImageToWpfImage(drawingImage As System.Drawing.Image) As ImageSource
		If drawingImage Is Nothing Then Throw New ArgumentNullException(NameOf(drawingImage), NameOf(ConvertDrawingImageToWpfImage))
		Dim bi As New BitmapImage

		Using ms As New IO.MemoryStream
			drawingImage.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp)
			ms.Seek(0, IO.SeekOrigin.Begin)

			bi.BeginInit()
			bi.StreamSource = ms
			bi.EndInit()
		End Using

		Return bi
	End Function


	Public Function CreateBitmapAndDraw(width As Integer, height As Integer, dpi As Double, render As Action(Of DrawingContext)) As BitmapSource
		If render Is Nothing Then Throw New ArgumentNullException(NameOf(render), "CreateBitmapAndDraw must have a rendering function")
		Dim drawingVisual As New DrawingVisual()
		Using drawingContext As DrawingContext = drawingVisual.RenderOpen()
			render(drawingContext)
		End Using
		Dim bitmap As New RenderTargetBitmap(width, height, dpi, dpi, PixelFormats.[Default])
		bitmap.Render(drawingVisual)

		Return bitmap
	End Function

#End Region

End Module
