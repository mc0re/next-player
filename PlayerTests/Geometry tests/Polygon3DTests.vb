Imports Common


<TestClass>
<TestCategory("3D geometry")>
Public Class Polygon3DTests

#Region " Polygon3D creation tests "

    <TestMethod>
    Public Sub Polygon3D_CreateInfinteXY_Ok()
        Dim pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Origin, Vector3D.AlongZ, {"Pa"})

        Dim poly = Polygon3DHelper.Create(pl)

        Assert.AreEqual(0, poly.Sides.Count)
        Assert.IsTrue(poly.Contains(Point3DHelper.Create(1, 0, 0)))
        Assert.IsFalse(poly.Contains(Point3DHelper.Create(1, 1, 1)))
    End Sub


    <TestMethod>
    Public Sub Polygon3D_CreateTooFewPoints_Throws()
        Dim a = Point3DHelper.Create(1, 0, 0)
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Origin, Vector3D.AlongZ, {"Pa"})

        Dim ex = Assert.ThrowsException(Of ArgumentException)(Sub() Polygon3DHelper.Create(pl, a, b))
        Assert.AreEqual("Polygon must have 3 or more vertices.", ex.Message)
    End Sub

#End Region


#Region " Polygon3D cut tests: no cut "

    <TestMethod>
    Public Sub Polygon3D_CutInfinite_Ok()
        Dim a = Point3DHelper.Create(1, 0, 0, {"A"})
        Dim pl = Plane3DHelper.CreatePointNormal(a, Vector3D.AlongZ, {"Pa"})

        Dim poly = Polygon3DHelper.Create(pl)

        Dim cutPlane = Plane3DHelper.CreatePointNormal(a, New Vector3D(0, -1, 0), {"Pb"})
        Assert.IsTrue(poly.Cut(cutPlane, 1))

        Assert.AreEqual(1, poly.Sides.Count)
    End Sub


    <TestMethod>
    Public Sub Polygon3D_CutBeyond_Ok()
        Dim a = Point3DHelper.Create(1, 0, 0, {"A"})
        Dim b = Point3DHelper.Create(0, 1, 0, {"B"})
        Dim c = Point3DHelper.Create(2, 2, 0, {"C"})
        Dim pl = Plane3DHelper.Create3Points(a, b, c, {"Pa"})

        Dim poly = Polygon3DHelper.Create(pl, a, b, c)

        Dim cutPlane = Plane3DHelper.CreatePointNormal(c, New Vector3D(0, -1, 0), {"Pb"})
        Assert.IsTrue(poly.Cut(cutPlane, 1))

        Assert.AreEqual(3, poly.Sides.Count)
    End Sub


    <TestMethod>
    Public Sub Polygon3D_CutAlong_Ok()
        Dim a = Point3DHelper.Create(0, 0, 0)
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim c = Point3DHelper.Create(1, 1, 0)
        Dim pl = Plane3DHelper.Create3Points(a, b, c, {"Pa"})

        Dim poly = Polygon3DHelper.Create(pl, a, b, c)

        Dim cutPlane = Plane3DHelper.CreatePointNormal(c, New Vector3D(0, -1, 0), {"Pb"})
        Assert.IsTrue(poly.Cut(cutPlane, 1))

        Assert.AreEqual(3, poly.Sides.Count)
    End Sub


    <TestMethod>
    Public Sub Polygon3D_CutContains_Ok()
        Dim a = Point3DHelper.Create(0, 0, 0)
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim c = Point3DHelper.Create(1, 1, 0)
        Dim pl = Plane3DHelper.Create3Points(a, b, c, {"Pa"})

        Dim poly = Polygon3DHelper.Create(pl, a, b, c)

        Dim cutPlane = Plane3DHelper.CreatePointNormal(c, Vector3D.AlongZ, {"Pa"})
        Assert.IsTrue(poly.Cut(cutPlane, 1))
        Assert.IsTrue(poly.Cut(cutPlane, -1))

        Assert.AreEqual(3, poly.Sides.Count)
    End Sub

#End Region


#Region " Polygon3D cut tests: through segments "

    <TestMethod>
    Public Sub Polygon3D_CutAddsSides_Ok()
        Dim a = Point3DHelper.Create(2, 0, 0)
        Dim b = Point3DHelper.Create(0, 2, 0)
        Dim c = Point3DHelper.Create(-2, 0, 0)
        Dim d = Point3DHelper.Create(0, -2, 0)
        Dim pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Origin, Vector3D.AlongZ, {"Pa"})
        Dim poly = Polygon3DHelper.Create(pl, a, b, c, d)

        Dim e = Point3DHelper.Create(1, 0, 0)
        Dim cutPlane = Plane3DHelper.CreatePointNormal(e, Vector3D.AlongX, {"Pb"})
        Assert.IsTrue(poly.Cut(cutPlane, 1))

        Assert.AreEqual(3, poly.Sides.Count)

        Dim ver = poly.Vertices
        Assert.IsTrue(ver.Contains(a, Point3DHelper.EqualityComparer))
        Assert.IsTrue(ver.Contains(Point3DHelper.Create(1, -1, 0), Point3DHelper.EqualityComparer))
        Assert.IsTrue(ver.Contains(Point3DHelper.Create(1, 1, 0), Point3DHelper.EqualityComparer))
    End Sub


    <TestMethod>
    Public Sub Polygon3D_CutRemovesSides_Ok()
        Dim a = Point3DHelper.Create(2, 0, 0)
        Dim b = Point3DHelper.Create(0, 2, 0)
        Dim c = Point3DHelper.Create(-2, 0, 0)
        Dim d = Point3DHelper.Create(0, -2, 0)
        Dim pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Origin, Vector3D.AlongZ, {"Pa"})
        Dim poly = Polygon3DHelper.Create(pl, a, b, c, d)

        Dim e = Point3DHelper.Create(1, 0, 0)
        Dim cutPlane = Plane3DHelper.CreatePointNormal(e, Vector3D.AlongX, {"Pb"})
        Assert.IsTrue(poly.Cut(cutPlane, -1))

        Assert.AreEqual(5, poly.Sides.Count)

        Dim ver = poly.Vertices
        Assert.IsTrue(ver.Contains(b, Point3DHelper.EqualityComparer))
        Assert.IsTrue(ver.Contains(c, Point3DHelper.EqualityComparer))
        Assert.IsTrue(ver.Contains(d, Point3DHelper.EqualityComparer))
        Assert.IsTrue(ver.Contains(Point3DHelper.Create(1, -1, 0), Point3DHelper.EqualityComparer))
        Assert.IsTrue(ver.Contains(Point3DHelper.Create(1, 1, 0), Point3DHelper.EqualityComparer))
    End Sub


    <TestMethod>
    Public Sub Polygon3D_CutPlaneTooClose_Ok()
        Dim a = Point3DHelper.Create(1, 1, 0)
        Dim b = Point3DHelper.Create(-1, 1, 0)
        Dim c = Point3DHelper.Create(-1, -1, 0)
        Dim d = Point3DHelper.Create(1, -1, 0)
        Dim polyPlane = Plane3DHelper.Create3Points(a, b, c, {"Pa"})
        Dim poly = Polygon3DHelper.Create(polyPlane, a, b, c, d)

        Dim cutPlane = Plane3DHelper.CreatePointNormal(
            Point3DHelper.Create(0, 1, 0), New Vector3D(AbsoluteCoordPrecision / 2, 1, 0), {"Pb"})
        Assert.IsTrue(poly.Cut(cutPlane, -1))

        Assert.AreEqual(4, poly.Sides.Count)
    End Sub


    <TestMethod>
    Public Sub Polygon3D_CutRemovesPolygon_Ok()
        Dim a = Point3DHelper.Create(0, 0, 0)
        Dim b = Point3DHelper.Create(1, -1, 0)
        Dim c = Point3DHelper.Create(1, 1, 0)
        Dim polyPlane = Plane3DHelper.Create3Points(a, b, c, {"Pa"})
        Dim poly = Polygon3DHelper.Create(polyPlane, a, b, c)

        Dim cutPlane = Plane3DHelper.CreatePointNormal(
            Point3DHelper.Create(AbsoluteCoordPrecision / 2, 0, 0), Vector3D.AlongX, {"Pb"})
        Assert.IsFalse(poly.Cut(cutPlane, -1))

        Assert.AreEqual(0, poly.Sides.Count)
    End Sub

#End Region


#Region " Polygon3D cut tests: through one vertice, remove middle "

    <TestMethod>
    Public Sub Polygon3D_CutVertice_23_Remove3FromA_Ok()
        Dim a = Point3DHelper.Create(0, 0, 0)
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim c = Point3DHelper.Create(0, 1, 1)
        Dim d = Point3DHelper.Create(0, 1, 2)
        Dim e = Point3DHelper.Create(0, 0, 2)
        Dim f = Point3DHelper.Create(0, 0, 1)

        Dim polyPlane = Plane3DHelper.Create3Points(a, b, c, {"Pa"})
        Dim cutPlane = Plane3DHelper.CreatePointNormal(c, New Vector3D(0, 0, -1), {"Pb"})

        Dim poly = Polygon3DHelper.Create(polyPlane, a, b, c, d, e)
        Assert.IsTrue(poly.Cut(cutPlane, 1))

        Assert.AreEqual(4, poly.Sides.Count)
        TestPoint(poly.Vertices(0), a)
        TestPoint(poly.Vertices(1), b)
        TestPoint(poly.Vertices(2), c)
        TestPoint(poly.Vertices(3), f)
    End Sub


    <TestMethod>
    Public Sub Polygon3D_CutVertice_34_Remove3FromB_Ok()
        Dim a = Point3DHelper.Create(0, 0, 0)
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim c = Point3DHelper.Create(0, 1, 1)
        Dim d = Point3DHelper.Create(0, 1, 2)
        Dim e = Point3DHelper.Create(0, 0, 2)
        Dim f = Point3DHelper.Create(0, 0, 1)

        Dim polyPlane = Plane3DHelper.Create3Points(a, b, c, {"Pa"})
        Dim cutPlane = Plane3DHelper.CreatePointNormal(c, New Vector3D(0, 0, -1), {"Pb"})

        Dim poly = Polygon3DHelper.Create(polyPlane, a, e, d, c, b)
        Assert.IsTrue(poly.Cut(cutPlane, 1))

        Assert.AreEqual(4, poly.Sides.Count)
        TestPoint(poly.Vertices(0), a)
        TestPoint(poly.Vertices(1), f)
        TestPoint(poly.Vertices(2), c)
        TestPoint(poly.Vertices(3), b)
    End Sub


    <TestMethod>
    Public Sub Polygon3D_CutVertice_51_Remove1FromA_Ok()
        Dim a = Point3DHelper.Create(0, 0, 0)
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim c = Point3DHelper.Create(0, 1, 1)
        Dim d = Point3DHelper.Create(0, 1, 2)
        Dim e = Point3DHelper.Create(0, 0, 2)
        Dim f = Point3DHelper.Create(0, 0, 1)

        Dim polyPlane = Plane3DHelper.Create3Points(a, b, c, {"Pa"})
        Dim cutPlane = Plane3DHelper.CreatePointNormal(c, New Vector3D(0, 0, -1), {"Pb"})

        Dim poly = Polygon3DHelper.Create(polyPlane, c, d, e, a, b)
        Assert.IsTrue(poly.Cut(cutPlane, 1))

        Assert.AreEqual(4, poly.Sides.Count)
        TestPoint(poly.Vertices(0), f)
        TestPoint(poly.Vertices(1), a)
        TestPoint(poly.Vertices(2), b)
        TestPoint(poly.Vertices(3), c)
    End Sub


    <TestMethod>
    Public Sub Polygon3D_CutVertice_51_Remove5FromB_Ok()
        Dim a = Point3DHelper.Create(0, 0, 0)
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim c = Point3DHelper.Create(0, 1, 1)
        Dim d = Point3DHelper.Create(0, 1, 2)
        Dim e = Point3DHelper.Create(0, 0, 2)
        Dim f = Point3DHelper.Create(0, 0, 1)

        Dim polyPlane = Plane3DHelper.Create3Points(a, b, c, {"Pa"})
        Dim cutPlane = Plane3DHelper.CreatePointNormal(c, New Vector3D(0, 0, -1), {"Pb"})

        Dim poly = Polygon3DHelper.Create(polyPlane, c, b, a, e, d)
        Assert.IsTrue(poly.Cut(cutPlane, 1))

        Assert.AreEqual(4, poly.Sides.Count)
        TestPoint(poly.Vertices(0), c)
        TestPoint(poly.Vertices(1), b)
        TestPoint(poly.Vertices(2), a)
        TestPoint(poly.Vertices(3), f)
    End Sub

#End Region


#Region " Polygon3D cut tests: through one vertice, remove ends "

    <TestMethod>
    Public Sub Polygon3D_CutVertice_34_Remove4FromA_Ok()
        Dim a = Point3DHelper.Create(0, 0, 0)
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim c = Point3DHelper.Create(0, 1, 1)
        Dim d = Point3DHelper.Create(0, 1, 2)
        Dim e = Point3DHelper.Create(0, 0, 2)
        Dim cut = Point3DHelper.Create(0, 0, 1)

        Dim polyPlane = Plane3DHelper.Create3Points(a, b, c, {"Pa"})
        Dim cutPlane = Plane3DHelper.CreatePointNormal(c, New Vector3D(0, 0, -1), {"Pb"})

        Dim poly = Polygon3DHelper.Create(polyPlane, e, a, b, c, d)
        Assert.IsTrue(poly.Cut(cutPlane, 1))

        Assert.AreEqual(4, poly.Sides.Count)
        TestPoint(poly.Sides(0).PointA, cut)
        TestPoint(poly.Sides(1).PointA, a)
        TestPoint(poly.Sides(2).PointA, b)
        TestPoint(poly.Sides(3).PointA, c)
    End Sub


    <TestMethod>
    Public Sub Polygon3D_CutVertice_23_Remove2FromB_Ok()
        Dim a = Point3DHelper.Create(0, 0, 0)
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim c = Point3DHelper.Create(0, 1, 1)
        Dim d = Point3DHelper.Create(0, 1, 2)
        Dim e = Point3DHelper.Create(0, 0, 2)
        Dim f = Point3DHelper.Create(0, 0, 1)

        Dim polyPlane = Plane3DHelper.Create3Points(a, b, c, {"Pa"})
        Dim cutPlane = Plane3DHelper.CreatePointNormal(c, New Vector3D(0, 0, -1), {"Pb"})

        Dim poly = Polygon3DHelper.Create(polyPlane, e, d, c, b, a)
        Assert.IsTrue(poly.Cut(cutPlane, 1))

        Assert.AreEqual(4, poly.Sides.Count)
        TestPoint(poly.Vertices(0), c)
        TestPoint(poly.Vertices(1), b)
        TestPoint(poly.Vertices(2), a)
        TestPoint(poly.Vertices(3), f)
    End Sub


    <TestMethod>
    Public Sub Polygon3D_CutVertice_45_Remove5FromA_Ok()
        Dim a = Point3DHelper.Create(0, 0, 0)
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim c = Point3DHelper.Create(0, 1, 1)
        Dim d = Point3DHelper.Create(0, 1, 2)
        Dim e = Point3DHelper.Create(0, 0, 2)
        Dim cut = Point3DHelper.Create(0, 0, 1)

        Dim polyPlane = Plane3DHelper.Create3Points(a, b, c, {"Pa"})
        Dim cutPlane = Plane3DHelper.CreatePointNormal(c, New Vector3D(0, 0, -1), {"Pb"})

        Dim poly = Polygon3DHelper.Create(polyPlane, d, e, a, b, c)
        Assert.IsTrue(poly.Cut(cutPlane, 1))

        Assert.AreEqual(4, poly.Sides.Count)
        TestPoint(poly.Sides(0).PointA, cut)
        TestPoint(poly.Sides(1).PointA, a)
        TestPoint(poly.Sides(2).PointA, b)
        TestPoint(poly.Sides(3).PointA, c)
    End Sub


    <TestMethod>
    Public Sub Polygon3D_CutVertice_12_Remove1FromB_Ok()
        Dim a = Point3DHelper.Create(0, 0, 0)
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim c = Point3DHelper.Create(0, 1, 1)
        Dim d = Point3DHelper.Create(0, 1, 2)
        Dim e = Point3DHelper.Create(0, 0, 2)
        Dim f = Point3DHelper.Create(0, 0, 1)

        Dim polyPlane = Plane3DHelper.Create3Points(a, b, c, {"Pa"})
        Dim cutPlane = Plane3DHelper.CreatePointNormal(c, New Vector3D(0, 0, -1), {"Pb"})

        Dim poly = Polygon3DHelper.Create(polyPlane, d, c, b, a, e)
        Assert.IsTrue(poly.Cut(cutPlane, 1))

        Assert.AreEqual(4, poly.Sides.Count)
        TestPoint(poly.Vertices(0), c)
        TestPoint(poly.Vertices(1), b)
        TestPoint(poly.Vertices(2), a)
        TestPoint(poly.Vertices(3), f)
    End Sub

#End Region


#Region " Polygon3D cut tests: through two vertices "

    <TestMethod>
    Public Sub Polygon3D_CutTwoVertices_Middle_Ok()
        Dim a = Point3DHelper.Create(0, 0, 0)
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim c = Point3DHelper.Create(0, 1, 1)
        Dim d = Point3DHelper.Create(0, 1, 2)
        Dim e = Point3DHelper.Create(0, 0, 2)
        Dim f = Point3DHelper.Create(0, 0, 1)

        Dim polyPlane = Plane3DHelper.Create3Points(a, b, c, {"Pa"})
        Dim cutPlane = Plane3DHelper.CreatePointNormal(c, New Vector3D(0, 0, -1), {"Pb"})

        Dim poly = Polygon3DHelper.Create(polyPlane, a, b, c, d, e, f)
        Assert.IsTrue(poly.Cut(cutPlane, 1))

        Assert.AreEqual(4, poly.Sides.Count)
        TestPoint(poly.Vertices(0), a)
        TestPoint(poly.Vertices(1), b)
        TestPoint(poly.Vertices(2), c)
        TestPoint(poly.Vertices(3), f)
    End Sub


    <TestMethod>
    Public Sub Polygon3D_CutTwoVertices_StartEnd_Ok()
        Dim a = Point3DHelper.Create(0, 0, 0)
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim c = Point3DHelper.Create(0, 1, 1)
        Dim d = Point3DHelper.Create(0, 1, 2)
        Dim e = Point3DHelper.Create(0, 0, 2)
        Dim f = Point3DHelper.Create(0, 0, 1)

        Dim polyPlane = Plane3DHelper.Create3Points(a, b, c, {"Pa"})
        Dim cutPlane = Plane3DHelper.CreatePointNormal(c, New Vector3D(0, 0, -1), {"Pb"})

        Dim poly = Polygon3DHelper.Create(polyPlane, e, f, a, b, c, d)
        Assert.IsTrue(poly.Cut(cutPlane, 1))

        Assert.AreEqual(4, poly.Sides.Count)
        TestPoint(poly.Vertices(0), f)
        TestPoint(poly.Vertices(1), a)
        TestPoint(poly.Vertices(2), b)
        TestPoint(poly.Vertices(3), c)
    End Sub

#End Region

End Class
