Public Class Math3D

	''' <summary>
	''' Calculate determinant of a 3x3 matrix.
	''' </summary>
	''' <param name="a11">Row 1, column 1</param>
	''' <param name="a12">Row 1, column 2</param>
	''' <param name="a13">Row 1, column 3</param>
	''' <param name="a21">Row 2, column 1</param>
	''' <param name="a22">Row 2, column 2</param>
	''' <param name="a23">Row 2, column 3</param>
	''' <param name="a31">Row 3, column 1</param>
	''' <param name="a32">Row 3, column 2</param>
	''' <param name="a33">Row 3, column 3</param>
	Public Shared Function Determinant(
		a11 As Double, a12 As Double, a13 As Double,
		a21 As Double, a22 As Double, a23 As Double,
		a31 As Double, a32 As Double, a33 As Double
	) As Double

		Return a11 * (a22 * a33 - a23 * a32) -
			   a12 * (a21 * a33 - a23 * a31) +
			   a13 * (a21 * a32 - a22 * a31)
	End Function

End Class
