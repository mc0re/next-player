Imports Common


Public Module GeometryTestUtility

#Region " Test utility "

    Public Sub TestPoint(actual As IPoint2D, x As Double, y As Double)
		Assert.AreEqual(x, actual.X, AbsoluteCoordPrecision, "X")
		Assert.AreEqual(y, actual.Y, AbsoluteCoordPrecision, "Y")
	End Sub


	Public Sub TestPoint(actual As IPoint3D, x As Double, y As Double, z As Double)
		Assert.AreEqual(x, actual.X, AbsoluteCoordPrecision, "X")
		Assert.AreEqual(y, actual.Y, AbsoluteCoordPrecision, "Y")
		Assert.AreEqual(z, actual.Z, AbsoluteCoordPrecision, "Z")
	End Sub


	Public Sub TestPoint(actual As IPoint3D, expected As IPoint3D)
		Assert.AreEqual(expected.X, actual.X, AbsoluteCoordPrecision, "X")
		Assert.AreEqual(expected.Y, actual.Y, AbsoluteCoordPrecision, "Y")
		Assert.AreEqual(expected.Z, actual.Z, AbsoluteCoordPrecision, "Z")
	End Sub


	Public Sub TestVector(actual As Vector3D, x As Double, y As Double, z As Double)
		Assert.AreEqual(x, actual.X, AbsoluteCoordPrecision, "X")
		Assert.AreEqual(y, actual.Y, AbsoluteCoordPrecision, "Y")
		Assert.AreEqual(z, actual.Z, AbsoluteCoordPrecision, "Z")
	End Sub

#End Region

End Module
