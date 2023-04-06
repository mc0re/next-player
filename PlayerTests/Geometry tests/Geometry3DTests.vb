Imports Common


<TestClass>
<TestCategory("3D geometry")>
Public Class Geometry3DTests

#Region " Point3D tests "

    <TestMethod>
    Public Sub Point3D_CreateCoordinates_Ok()
        Dim p = Point3DHelper.Create(1, 2, 3)

        Assert.AreEqual(1.0, p.X)
        Assert.AreEqual(2.0, p.Y)
        Assert.AreEqual(3.0, p.Z)
    End Sub


    <TestMethod>
    Public Sub Point3D_CreateCopy_Ok()
        Dim p = Point3DHelper.Create(1, 2, 3)
        Dim p1 = Point3DHelper.Create(p)

        Assert.AreEqual(1.0, p1.X)
        Assert.AreEqual(2.0, p1.Y)
        Assert.AreEqual(3.0, p1.Z)
    End Sub


    <TestMethod>
    Public Sub Point3D_IsSame_Ok()
        Dim a = Point3DHelper.Create(1, 2, 3)
        Dim b = Point3DHelper.Create(1, 2, 4)

        Assert.IsTrue(Point3DHelper.IsSame(a, a))
        Assert.IsFalse(Point3DHelper.IsSame(a, b))
    End Sub

#End Region


#Region " Vector3D tests "

    <TestMethod>
    Public Sub Vector3D_AlongXPerpendiculars_Ok()
        Dim v As New Vector3D(4, 0, 0)

        Assert.AreEqual(0.0, v.DotProduct(v.AnyPerpendicular))
        Assert.AreEqual(0.0, v.DotProduct(v.OtherPerpendicular))
    End Sub


    <TestMethod>
    Public Sub Vector3D_AlongYPerpendiculars_Ok()
        Dim v As New Vector3D(0, 4, 0)

        Assert.AreEqual(0.0, v.DotProduct(v.AnyPerpendicular))
        Assert.AreEqual(0.0, v.DotProduct(v.OtherPerpendicular))
    End Sub


    <TestMethod>
    Public Sub Vector3D_AlongZPerpendiculars_Ok()
        Dim v As New Vector3D(0, 0, 4)

        Assert.AreEqual(0.0, v.DotProduct(v.AnyPerpendicular))
        Assert.AreEqual(0.0, v.DotProduct(v.OtherPerpendicular))
    End Sub


    <TestMethod>
    Public Sub Vector3D_UnalignedPerpendiculars_Ok()
        Dim v As New Vector3D(2, 3, 4)

        Assert.AreEqual(0.0, v.DotProduct(v.AnyPerpendicular))
        Assert.AreEqual(0.0, v.DotProduct(v.OtherPerpendicular))
    End Sub


    <TestMethod>
    Public Sub Vector3D_Average_AllGiven_Ok()
        Dim a = Point3DHelper.Create(0, 0, 1)
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim c = Point3DHelper.Create(1, 0, 0)

        Dim v = Vector3D.AverageVector(a, b, c)
        TestVector(v, 1, 0, -1)
    End Sub


    <TestMethod>
    Public Sub Vector3D_Average_ANotGiven_Ok()
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim c = Point3DHelper.Create(1, 0, 0)

        Dim v = Vector3D.AverageVector(Nothing, b, c)
        TestVector(v, 1, -1, 0)
    End Sub


    <TestMethod>
    Public Sub Vector3D_Average_CNotGiven_Ok()
        Dim a = Point3DHelper.Create(0, 0, 1)
        Dim b = Point3DHelper.Create(0, 1, 0)

        Dim v = Vector3D.AverageVector(a, b, Nothing)
        TestVector(v, 0, 1, -1)
    End Sub


    <TestMethod>
    Public Sub Vector3D_Average_BNotGiven_Throws()
        Dim a = Point3DHelper.Create(0, 0, 1)
        Dim c = Point3DHelper.Create(1, 0, 0)

        Dim ex = Assert.ThrowsException(Of ArgumentException)(
            Sub() Vector3D.AverageVector(a, Nothing, c))

        Assert.AreEqual("B cannot be absent to find an average", ex.Message)
    End Sub


    <TestMethod>
    Public Sub Vector3D_Average_ACNotGiven_Throws()
        Dim b = Point3DHelper.Create(0, 1, 0)

        Dim ex = Assert.ThrowsException(Of ArgumentException)(
            Sub() Vector3D.AverageVector(Nothing, b, Nothing))

        Assert.AreEqual("Both A and C cannot be absent to find an average", ex.Message)
    End Sub


    <TestMethod>
    Public Sub Vector3D_Average_ACSame_Ok()
        Dim a = Point3DHelper.Create(0, 0, 1)
        Dim b = Point3DHelper.Create(0, 1, 0)

        Dim v = Vector3D.AverageVector(a, b, a)
        TestVector(v, 0, 1, -1)
    End Sub

#End Region


#Region " Line3D tests "

    <TestMethod>
    Public Sub Line3D_Colliding_Throws()
        Dim a = Point3DHelper.Create(1, 1, 1)

        Dim ex = Assert.ThrowsException(Of ArgumentException)(
            Sub() Line3DHelper.Create(a, a))

        Assert.AreEqual("Cannot build a line from colliding points", ex.Message)
    End Sub


    <TestMethod>
    Public Sub Line3D_NullVector_Throws()
        Dim a = Point3DHelper.Create(1, 1, 1)

        Dim ex = Assert.ThrowsException(Of ArgumentException)(
            Sub() Line3DHelper.Create(a, New Vector3D(0, 0, 0)))

        Assert.AreEqual("Cannot build a line from null vector", ex.Message)
    End Sub


    <TestMethod>
    Public Sub Line3D_AlongXGetPoint_Ok()
        Dim a = Point3DHelper.Create(0, 1, 1)
        Dim b = Point3DHelper.Create(4, 1, 1)
        Dim l = Line3DHelper.Create(a, b)

        Assert.AreEqual(0.0, l.GetCoordinate(Point3DHelper.Create(0, 1, 1)))
        Assert.AreEqual(0.5, l.GetCoordinate(Point3DHelper.Create(2, 1, 1)))
        Assert.AreEqual(1.0, l.GetCoordinate(Point3DHelper.Create(4, 1, 1)))
        Assert.AreEqual(2.0, l.GetCoordinate(Point3DHelper.Create(8, 1, 1)))
        Assert.AreEqual(-1.0, l.GetCoordinate(Point3DHelper.Create(-4, 1, 1)))

        Assert.IsNull(l.GetCoordinate(Point3DHelper.Create(0, 0, 1)))
        Assert.IsNull(l.GetCoordinate(Point3DHelper.Create(0, 1, 0)))
    End Sub


    <TestMethod>
    Public Sub Line3D_AlongYGetPoint_Ok()
        Dim a = Point3DHelper.Create(1, 0, 1)
        Dim b = Point3DHelper.Create(1, 4, 1)
        Dim l = Line3DHelper.Create(a, b)

        Assert.AreEqual(0.0, l.GetCoordinate(Point3DHelper.Create(1, 0, 1)))
        Assert.AreEqual(0.5, l.GetCoordinate(Point3DHelper.Create(1, 2, 1)))
        Assert.AreEqual(1.0, l.GetCoordinate(Point3DHelper.Create(1, 4, 1)))
        Assert.AreEqual(2.0, l.GetCoordinate(Point3DHelper.Create(1, 8, 1)))
        Assert.AreEqual(-1.0, l.GetCoordinate(Point3DHelper.Create(1, -4, 1)))

        Assert.IsNull(l.GetCoordinate(Point3DHelper.Create(0, 0, 1)))
        Assert.IsNull(l.GetCoordinate(Point3DHelper.Create(1, 0, 0)))
    End Sub


    <TestMethod>
    Public Sub Line3D_AlongZGetPoint_Ok()
        Dim a = Point3DHelper.Create(1, 1, 0)
        Dim b = Point3DHelper.Create(1, 1, 4)
        Dim l = Line3DHelper.Create(a, b)

        Assert.AreEqual(0.0, l.GetCoordinate(Point3DHelper.Create(1, 1, 0)))
        Assert.AreEqual(0.5, l.GetCoordinate(Point3DHelper.Create(1, 1, 2)))
        Assert.AreEqual(1.0, l.GetCoordinate(Point3DHelper.Create(1, 1, 4)))
        Assert.AreEqual(2.0, l.GetCoordinate(Point3DHelper.Create(1, 1, 8)))
        Assert.AreEqual(-1.0, l.GetCoordinate(Point3DHelper.Create(1, 1, -4)))

        Assert.IsNull(l.GetCoordinate(Point3DHelper.Create(0, 1, 0)))
        Assert.IsNull(l.GetCoordinate(Point3DHelper.Create(1, 0, 0)))
    End Sub


    <TestMethod>
    Public Sub Line3D_DiagonalGetPoint_Ok()
        Dim a = Point3DHelper.Create(0, 0, 0)
        Dim b = Point3DHelper.Create(1, 1, 1)
        Dim l = Line3DHelper.Create(a, b)

        Assert.AreEqual(0.0, l.GetCoordinate(Point3DHelper.Create(0, 0, 0)))
        Assert.AreEqual(0.5, l.GetCoordinate(Point3DHelper.Create(0.5, 0.5, 0.5)))
        Assert.AreEqual(1.0, l.GetCoordinate(Point3DHelper.Create(1, 1, 1)))
        Assert.AreEqual(2.0, l.GetCoordinate(Point3DHelper.Create(2, 2, 2)))
        Assert.AreEqual(-1.0, l.GetCoordinate(Point3DHelper.Create(-1, -1, -1)))

        Assert.IsNull(l.GetCoordinate(Point3DHelper.Create(0, 0, 1)))
        Assert.IsNull(l.GetCoordinate(Point3DHelper.Create(0, 1, 0)))
        Assert.IsNull(l.GetCoordinate(Point3DHelper.Create(1, 0, 0)))
    End Sub


    <TestMethod>
    Public Sub Line3D_AlongXCutByPlane_Ok()
        Dim a = Point3DHelper.Create(0, 1, 1)
        Dim b = Point3DHelper.Create(4, 1, 1)
        Dim l = Line3DHelper.Create(a, b)

        Dim plA = Plane3DHelper.CreatePointNormal(a, Vector3D.AlongX, {"Pa"})
        Dim cutA = l.CutByPlane(plA)
        Assert.AreEqual(0.0, cutA.Coordinate)
        Assert.AreEqual(1, cutA.VectorSign)

        Dim plB = Plane3DHelper.CreatePointNormal(b, New Vector3D(1, 1, 1), {"Pb"})
        Dim cutB = l.CutByPlane(plB)
        Assert.AreEqual(1.0, cutB.Coordinate)
        Assert.AreEqual(1, cutB.VectorSign)

        Dim plC = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(2, 2, 2), Vector3D.AlongX, {"Pc"})
        Dim cutC = l.CutByPlane(plC)
        Assert.AreEqual(0.5, cutC.Coordinate)
        Assert.AreEqual(1, cutC.VectorSign)

        Dim plD = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(2, 1, 1), Vector3D.AlongY, {"Pd"})
        Assert.IsNull(l.CutByPlane(plD))

        Dim plE = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(2, 1, 1), Vector3D.AlongZ, {"Pe"})
        Assert.IsNull(l.CutByPlane(plE))
    End Sub


    <TestMethod>
    Public Sub Line3D_DiagonalCutByPlane_Ok()
        Dim a = Point3DHelper.Create(0, 0, 0)
        Dim b = Point3DHelper.Create(1, 1, 1)
        Dim l = Line3DHelper.Create(a, b)

        Dim plA = Plane3DHelper.CreatePointNormal(a, Vector3D.AlongX, {"Pa"})
        Dim cutA = l.CutByPlane(plA)
        Assert.AreEqual(0.0, cutA.Coordinate)
        Assert.AreEqual(1, cutA.VectorSign)

        Dim plB = Plane3DHelper.CreatePointNormal(b, Vector3D.AlongY, {"Pb"})
        Dim cutB = l.CutByPlane(plB)
        Assert.AreEqual(1.0, cutB.Coordinate)
        Assert.AreEqual(1, cutB.VectorSign)

        Dim plC = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(2, 2, 2), Vector3D.AlongX, {"Pc"})
        Dim cutC = l.CutByPlane(plC)
        Assert.AreEqual(2.0, cutC.Coordinate)
        Assert.AreEqual(1, cutC.VectorSign)

        Dim plD = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(2, 1, 1), New Vector3D(1, -1, 0), {"Pd"})
        Assert.IsNull(l.CutByPlane(plD))
    End Sub


    <TestMethod>
    Public Sub Line3D_AlongXCutBySphere_Ok()
        Dim a = Point3DHelper.Create(0, 1, 1)
        Dim b = Point3DHelper.Create(4, 1, 1)
        Dim l = Line3DHelper.Create(a, b)

        Dim cutA = l.CutBySphere(New Sphere3D(a, 1))
        Assert.AreEqual(0.25, cutA.Coordinate)
        Assert.AreEqual(1, cutA.VectorSign)

        Dim cutB = l.CutBySphere(New Sphere3D(b, 1))
        Assert.AreEqual(0.75, cutB.Coordinate)
        Assert.AreEqual(1, cutB.VectorSign)

        Assert.IsNull(l.CutBySphere(New Sphere3D(Point3DHelper.Create(4, 4, 4), 1)))
        Assert.IsNull(l.CutBySphere(New Sphere3D(Point3DHelper.Create(-1, 1, 1), 1)))
        Assert.IsNull(l.CutBySphere(New Sphere3D(Point3DHelper.Create(1, 1, 1), 1)))
    End Sub


    <TestMethod>
    Public Sub Line3D_ClosestPoint_ParallelLines_Ok()
        Dim v As New Vector3D(1, 1, 1)
        Dim l1 = Line3DHelper.Create(Point3DHelper.Create(0, 1, 1), v)
        Dim l2 = Line3DHelper.Create(Point3DHelper.Create(2, 1, 3), v)

        Dim pi = Line3DHelper.ClosestPoints(l1, l2)
        Assert.AreEqual(1, pi.Count)
        Assert.AreEqual(1.5, pi(0).X)
        Assert.AreEqual(1.5, pi(0).Y)
        Assert.AreEqual(2.5, pi(0).Z)
    End Sub


    <TestMethod>
    Public Sub Line3D_ClosestPoint_ParallelUnequalLines_Ok()
        Dim l1 = Line3DHelper.Create(Point3DHelper.Create(0, 1, 1), New Vector3D(1, 1, 1))
        Dim l2 = Line3DHelper.Create(Point3DHelper.Create(2, 1, 3), New Vector3D(2, 2, 2))

        Dim pi = Line3DHelper.ClosestPoints(l1, l2)
        Assert.AreEqual(1, pi.Count)
        Assert.AreEqual(1.5, pi(0).X)
        Assert.AreEqual(1.5, pi(0).Y)
        Assert.AreEqual(2.5, pi(0).Z)
    End Sub


    <TestMethod>
    Public Sub Line3D_ClosestPoint_SamePoint_Ok()
        Dim p = Point3DHelper.Create(1, 1, 1)
        Dim l1 = Line3DHelper.Create(p, Point3DHelper.Create(2, 1, 1))
        Dim l2 = Line3DHelper.Create(p, Point3DHelper.Create(1, 1, 2))

        Dim pi = Line3DHelper.ClosestPoints(l1, l2)
        Assert.AreEqual(1, pi.Count)
        Assert.AreEqual(p.X, pi(0).X)
        Assert.AreEqual(p.Y, pi(0).Y)
        Assert.AreEqual(p.Z, pi(0).Z)
    End Sub


    <TestMethod>
    Public Sub Line3D_ClosestPoint_Crossing_Ok()
        Dim l1 = Line3DHelper.Create(Point3DHelper.Create(0, 0, 0), Point3DHelper.Create(2, 2, 0))
        Dim l2 = Line3DHelper.Create(Point3DHelper.Create(0, 2, 0), Point3DHelper.Create(2, 0, 0))

        Dim pi = Line3DHelper.ClosestPoints(l1, l2)
        Assert.AreEqual(1, pi.Count)
        Assert.AreEqual(1.0, pi(0).X)
        Assert.AreEqual(1.0, pi(0).Y)
        Assert.AreEqual(0.0, pi(0).Z)
    End Sub


    <TestMethod>
    Public Sub Line3D_ClosestPoint_Skew_Ok()
        Dim l1 = Line3DHelper.Create(Point3DHelper.Create(0, 0, 0), Point3DHelper.Create(2, 2, 0))
        Dim l2 = Line3DHelper.Create(Point3DHelper.Create(0, 2, 4), Point3DHelper.Create(2, 0, 4))

        Dim pi = Line3DHelper.ClosestPoints(l1, l2)
        Assert.AreEqual(2, pi.Count)
        Assert.AreEqual(1.0, pi(0).X)
        Assert.AreEqual(1.0, pi(0).Y)
        Assert.AreEqual(0.0, pi(0).Z)
        Assert.AreEqual(1.0, pi(1).X)
        Assert.AreEqual(1.0, pi(1).Y)
        Assert.AreEqual(4.0, pi(1).Z)
    End Sub

#End Region


#Region " Plane3D tests "

    <TestMethod>
    <ExpectedException(GetType(ArgumentException))>
    Public Sub Plane3D_ABC_ABColliding_Throws()
        Dim a = Point3DHelper.Create(1, 1, 1)
        Dim b = Point3DHelper.Create(2, 2, 2)

        Dim p = Plane3DHelper.Create3Points(a, a, b, {"Pa"})
    End Sub


    <TestMethod>
    <ExpectedException(GetType(ArgumentException))>
    Public Sub Plane3D_ABC_ACColliding_Throws()
        Dim a = Point3DHelper.Create(1, 1, 1)
        Dim b = Point3DHelper.Create(2, 2, 2)
        Dim p = Plane3DHelper.Create3Points(a, b, a, {"Pa"})
    End Sub


    <TestMethod>
    <ExpectedException(GetType(ArgumentException))>
    Public Sub Plane3D_ABC_BCColliding_Throws()
        Dim a = Point3DHelper.Create(1, 1, 1)
        Dim b = Point3DHelper.Create(2, 2, 2)
        Dim p = Plane3DHelper.Create3Points(a, b, b, {"Pa"})
    End Sub


    <TestMethod>
    <ExpectedException(GetType(ArgumentException))>
    Public Sub Plane3D_ABC_Colinear_Throws()
        Dim a = Point3DHelper.Create(1, 1, 1)
        Dim b = Point3DHelper.Create(2, 2, 2)
        Dim c = Point3DHelper.Create(3, 3, 3)
        Dim p = Plane3DHelper.Create3Points(a, b, c, {"Pa"})
    End Sub


    <TestMethod>
    Public Sub Plane3D_ABC_Ok()
        ' Give in clockwise direction
        Dim a = Point3DHelper.Create(1, 1, 2)
        Dim b = Point3DHelper.Create(1, 2, 1)
        Dim c = Point3DHelper.Create(2, 1, 1)
        Dim p = Plane3DHelper.Create3Points(a, b, c, {"Pa"})

        Assert.AreEqual(0.0, p.GetDistanceToPoint(a))
        Assert.AreEqual(0.0, p.GetDistanceToPoint(b))
        Assert.AreEqual(0.0, p.GetDistanceToPoint(c))

        Assert.AreEqual(0.0, p.GetDistanceToPoint(Point3DHelper.Create(0, 3, 1)))
        Assert.AreEqual(0.0, p.GetDistanceToPoint(Point3DHelper.Create(3, 0, 1)))
        Assert.AreEqual(0.0, p.GetDistanceToPoint(Point3DHelper.Create(1, 0, 3)))

        Assert.IsTrue(p.GetDistanceToPoint(Point3DHelper.Create(0, 0, 0)) < 0)
        Assert.IsTrue(p.GetDistanceToPoint(Point3DHelper.Create(2, 2, 2)) > 0)
    End Sub


    <TestMethod>
    <ExpectedException(GetType(ArgumentException))>
    Public Sub Plane3D_ANormal_NormZero_Throws()
        Dim a = Point3DHelper.Create(1, 1, 1)
        Dim v As New Vector3D(0, 0, 0)
        Dim p = Plane3DHelper.CreatePointNormal(a, v, {"Pa"})
    End Sub


    <TestMethod>
    Public Sub Plane3D_ANormal_Ok()
        Dim a = Point3DHelper.Create(1, 1, 1)
        Dim v As New Vector3D(2, 2, 2)
        Dim p = Plane3DHelper.CreatePointNormal(a, v, {"Pa"})

        Assert.AreEqual(0.0, p.GetDistanceToPoint(a))

        Assert.IsTrue(p.GetDistanceToPoint(Point3DHelper.Create(0, 0, 0)) < 0)
        Assert.IsTrue(p.GetDistanceToPoint(Point3DHelper.Create(2, 2, 2)) > 0)
    End Sub


    <TestMethod>
    <ExpectedException(GetType(ArgumentException))>
    Public Sub Plane3D_ALineNormal_NormZero_Throws()
        Dim a = Point3DHelper.Create(1, 1, 1)
        Dim b = Point3DHelper.Create(3, 0, 0)
        Dim l = Vector3D.CreateA2B(a, b)
        Dim v As New Vector3D(0, 0, 0)
        Dim p = Plane3DHelper.CreateAlongLine(a, l, v, {"Pa"})
    End Sub


    <TestMethod>
    Public Sub Plane3D_ALineNormal_Ok()
        Dim a = Point3DHelper.Create(1, 1, 1)
        Dim b = Point3DHelper.Create(3, 0, 0)
        Dim l = Vector3D.CreateA2B(a, b)
        Dim v As New Vector3D(2, 2, 2)
        Dim p = Plane3DHelper.CreateAlongLine(a, l, v, {"Pa"})

        Assert.AreEqual(0.0, p.GetDistanceToPoint(a))
        Assert.AreEqual(0.0, p.GetDistanceToPoint(b))

        Assert.IsTrue(p.GetDistanceToPoint(Point3DHelper.Create(0, 0, 0)) < 0)
        Assert.IsTrue(p.GetDistanceToPoint(Point3DHelper.Create(2, 2, 2)) > 0)
    End Sub


    <TestMethod>
    Public Sub Plane3D_ALineNormal_RejZero_Ok()
        Dim a = Point3DHelper.Create(1, 0, 0)
        Dim b = Point3DHelper.Create(3, 0, 0)
        Dim l = Vector3D.CreateA2B(a, b)
        Dim p = Plane3DHelper.CreateAlongLine(a, l, Vector3D.AlongX, {"Pa"})

        ' Both points are still on the plane
        Assert.AreEqual(0.0, p.GetDistanceToPoint(a))
        Assert.AreEqual(0.0, p.GetDistanceToPoint(b))

        Assert.IsTrue(p.GetDistanceToPoint(Point3DHelper.Create(2, 2, 2)) > 0)
    End Sub


    <TestMethod>
    Public Sub Plane3D_Offset_Ok()
        Dim orig = Plane3DHelper.CreatePointNormal(
            Point3DHelper.Create(1, 2, 3), Vector3D.AlongZ, {"Pa"})
        Dim distTo0 = orig.GetDistanceToPoint(Point3DHelper.Origin)

        Dim off = orig.Shift(2)
        Dim offDistTo0 = off.GetDistanceToPoint(Point3DHelper.Origin)

        Assert.AreEqual(distTo0 - 2, offDistTo0, AbsoluteCoordPrecision)
    End Sub


    <TestMethod>
    Public Sub Plane3D_GetProjectionIfOnPlane_YZ_Ok()
        Dim a = Point3DHelper.Create(1, 1, 1)
        Dim p = Plane3DHelper.CreatePointNormal(a, Vector3D.AlongX, {"Pa"})

        Dim r = p.GetProjectionIfOnPlane(a)
        Assert.AreEqual(0.0, r.X)
        Assert.AreEqual(0.0, r.Y)

        r = p.GetProjectionIfOnPlane(Point3DHelper.Create(1, 1, 1))
        Assert.AreEqual(0.0, r.X)
        Assert.AreEqual(0.0, r.Y)

        r = p.GetProjectionIfOnPlane(Point3DHelper.Create(3, 1, 1))
        Assert.IsNull(r)

        r = p.GetProjectionIfOnPlane(Point3DHelper.Create(1, 2, 2))
        Assert.AreEqual(1.0, r.X)
        Assert.AreEqual(-1.0, r.Y)

        r = p.GetProjectionIfOnPlane(Point3DHelper.Create(1, 0, -2))
        Assert.AreEqual(-3.0, r.X)
        Assert.AreEqual(1.0, r.Y)
    End Sub


    <TestMethod>
    Public Sub Plane3D_IsSame_Ok()
        Dim pl1 = Plane3DHelper.CreatePointNormal(
            Point3DHelper.Create(1, 2, 3), Vector3D.AlongZ, {"Pa"})
        Dim pl2 = Plane3DHelper.CreatePointNormal(
            Point3DHelper.Create(5, 4, 3), New Vector3D(0, 0, 2), {"Pb"})

        Assert.IsTrue(Plane3DHelper.IsSame(pl1, pl2))
    End Sub


    <TestMethod>
    Public Sub Plane3D_IsNotSame_Skew_Ok()
        Dim p = Point3DHelper.Create(1, 2, 3)
        Dim pl1 = Plane3DHelper.CreatePointNormal(p, Vector3D.AlongZ, {"Pa"})
        Dim pl2 = Plane3DHelper.CreatePointNormal(p, Vector3D.AlongY, {"Pb"})

        Assert.IsFalse(Plane3DHelper.IsSame(pl1, pl2))
    End Sub


    <TestMethod>
    Public Sub Plane3D_IsNotSame_Parallel_Ok()
        Dim pl1 = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(1, 2, 3), Vector3D.AlongZ, {"Pa"})
        Dim pl2 = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(1, 0, 0), Vector3D.AlongZ, {"Pb"})

        Assert.IsFalse(Plane3DHelper.IsSame(pl1, pl2))
    End Sub

#End Region


#Region " Plane3D intersection tests "

    <TestMethod>
    Public Sub Plane3D_Cut_IntersectAtOrigin()
        Dim pl1 = Plane3DHelper.CreatePointNormal(Point3DHelper.Origin, Vector3D.AlongZ, {"Pa"})
        Dim pl2 = Plane3DHelper.CreatePointNormal(Point3DHelper.Origin, New Vector3D(1, 0, 1), {"Pb"})

        Dim r = pl1.Intersect(pl2, 1)

        Assert.AreEqual(PlaneIntersectionResults.Line, r.State)
        Assert.AreEqual(Line2DDirections.Right, r.Inside)

        Dim v = r.Line.Vector
        Assert.AreEqual(1.0, v.X)
        Assert.AreEqual(0.0, v.Y)

        Dim p0 = r.Line.GetPoint(0)
        Assert.AreEqual(0.0, p0.X)
        Assert.AreEqual(0.0, p0.Y)
    End Sub


    <TestMethod>
    Public Sub Plane3D_Cut_IntersectInsidePositive()
        Dim pl1 = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(0, 0, 1), Vector3D.AlongZ, {"Pa"})
        Dim pl2 = Plane3DHelper.CreatePointNormal(Point3DHelper.Origin, New Vector3D(1, 0, 1), {"Pb"})

        Dim r = pl1.Intersect(pl2, 1)

        Assert.AreEqual(PlaneIntersectionResults.Line, r.State)
        Assert.AreEqual(Line2DDirections.Right, r.Inside)

        Dim v = r.Line.Vector
        Assert.AreEqual(1.0, v.X)
        Assert.AreEqual(0.0, v.Y)

        Dim p0 = r.Line.GetPoint(0)
        Assert.AreEqual(0.0, p0.X)
        Assert.AreEqual(1.0, p0.Y)
    End Sub


    <TestMethod>
    Public Sub Plane3D_Cut_IntersectInsideNegative()
        Dim pl1 = Plane3DHelper.CreatePointNormal(Point3DHelper.Origin, Vector3D.AlongZ, {"Pa"})
        Dim pl2 = Plane3DHelper.CreatePointNormal(Point3DHelper.Origin, New Vector3D(1, 0, 1), {"Pb"})

        Dim r = pl1.Intersect(pl2, -1)

        Assert.AreEqual(PlaneIntersectionResults.Line, r.State)
        Assert.AreEqual(Line2DDirections.Left, r.Inside)

        Dim v = r.Line.Vector
        Assert.AreEqual(1.0, v.X)
        Assert.AreEqual(0.0, v.Y)

        Dim p0 = r.Line.GetPoint(0)
        Assert.AreEqual(0.0, p0.X)
        Assert.AreEqual(0.0, p0.Y)
    End Sub

#End Region

End Class
