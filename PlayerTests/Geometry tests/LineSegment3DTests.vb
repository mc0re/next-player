Imports Common


<TestClass>
<TestCategory("3D geometry")>
Public Class LineSegment3DTests

#Region " LineSegment3D creation tests "

	<TestMethod>
	Public Sub LineSegment3D_CreateOrdinary_Ok()
		Dim a = Point3DHelper.Create(0, 0, 0)
		Dim b = Point3DHelper.Create(3, 4, 12)
        Dim segm = LineSegment3DHelper.Create(a, b)

        Assert.IsNotNull(segm)
        Assert.AreEqual(13.0, segm.Length)
    End Sub


    <TestMethod>
    Public Sub LineSegment3D_CreateEmpty_Ok()
        Dim a = Point3DHelper.Create(1, 0, 0)
        Dim segm = LineSegment3DHelper.Create(a, a)

        Assert.IsNull(segm)
    End Sub

#End Region


#Region " LineSegment3D intersection tests "

    <TestMethod>
    Public Sub LineSegment3D_IntersectParallel_Ok()
        Dim a = Point3DHelper.Create(1, 0, 0)
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim segm = LineSegment3DHelper.Create(a, b)

        Dim pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(0, 0, 0), Vector3D.AlongZ, {"Pa"})
        Assert.AreEqual(SegmentIntersectResults.Contains, segm.Intersect(pl).Action)

        pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(0, 0, 0), New Vector3D(1, 1, 0), {"Pb"})
        Assert.AreEqual(SegmentIntersectResults.Beyond, segm.Intersect(pl).Action)
    End Sub


    <TestMethod>
    Public Sub LineSegment3D_IntersectAcross_Ok()
        Dim a = Point3DHelper.Create(1, 0, 0)
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim segm = LineSegment3DHelper.Create(a, b)

        Dim pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(0, 0, 0), New Vector3D(-1, 1, 0), {"Pa"})
        Assert.AreEqual(SegmentIntersectResults.Intersects, segm.Intersect(pl).Action)
    End Sub


    <TestMethod>
    Public Sub LineSegment3D_IntersectOnEnds_Ok()
        Dim a = Point3DHelper.Create(1, 0, 0)
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim segm = LineSegment3DHelper.Create(a, b)

        Dim pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(1, 0, 0), New Vector3D(-1, 0, 0), {"Pa"})
        Assert.AreEqual(SegmentIntersectResults.Touches, segm.Intersect(pl).Action)

        pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(0, 1, 0), New Vector3D(0, -1, 0), {"Pb"})
        Assert.AreEqual(SegmentIntersectResults.Touches, segm.Intersect(pl).Action)
    End Sub


    <TestMethod>
    Public Sub LineSegment3D_IntersectBeyond_Ok()
        Dim a = Point3DHelper.Create(1, 0, 0)
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim segm = LineSegment3DHelper.Create(a, b)

        Dim pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(-1, 0, 0), Vector3D.AlongX, {"Pa"})
        Assert.AreEqual(SegmentIntersectResults.Beyond, segm.Intersect(pl).Action)

        pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(0, -1, 0), Vector3D.AlongY, {"Pb"})
        Assert.AreEqual(SegmentIntersectResults.Beyond, segm.Intersect(pl).Action)
    End Sub

#End Region


#Region " LineSegment3D cut info tests "

    <TestMethod>
    Public Sub LineSegment3D_CutParallel_Ok()
        Dim a = Point3DHelper.Create(1, 0, 0)
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim segm = LineSegment3DHelper.Create(a, b)

        Dim pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(0, 0, 0), Vector3D.AlongZ, {"Pa"})
        Assert.AreEqual(SegmentCutResults.Contains, segm.GetCutInfo(pl, 1).Action)

        pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(0, 0, 0), New Vector3D(1, 1, 0), {"Pb"})
        Assert.AreEqual(SegmentCutResults.Beyond, segm.GetCutInfo(pl, 1).Action)

        Assert.AreEqual(SegmentCutResults.EliminateFromA, segm.GetCutInfo(pl, -1).Action)
    End Sub


    <TestMethod>
    Public Sub LineSegment3D_CutOnEnds_Ok()
        Dim a = Point3DHelper.Create(1, 0, 0)
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim segm = LineSegment3DHelper.Create(a, b)
        Dim positiveDir = Vector3D.AlongX
        Dim negativeDir = Vector3D.AlongX.Negate

        ' None of these cuttings actually modify the segment,
        ' so it is reused and not recreated
        Dim pl = Plane3DHelper.CreatePointNormal(a, positiveDir, {"Pa"})
        Assert.AreEqual(SegmentCutResults.EliminateFromA, segm.GetCutInfo(pl, 1).Action)

        pl = Plane3DHelper.CreatePointNormal(a, negativeDir, {"Pb"})
        Assert.AreEqual(SegmentCutResults.Beyond, segm.GetCutInfo(pl, 1).Action)

        pl = Plane3DHelper.CreatePointNormal(b, negativeDir, {"Pc"})
        Assert.AreEqual(SegmentCutResults.EliminateFromB, segm.GetCutInfo(pl, 1).Action)

        pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(0, 0, 0), positiveDir, {"Pd"})
        Assert.AreEqual(SegmentCutResults.Beyond, segm.GetCutInfo(pl, 1).Action)

        pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(0, 2, 0), positiveDir, {"Pe"})
        Assert.AreEqual(SegmentCutResults.Beyond, segm.GetCutInfo(pl, 1).Action)
    End Sub


    <TestMethod>
    Public Sub LineSegment3D_CutCloseToEnds_Ok()
        Dim a = Point3DHelper.Create(-1, 0, 0)
        Dim b = Point3DHelper.Create(1, 0, 0)
        Dim segm = LineSegment3DHelper.Create(a, b)

        Dim pl = Plane3DHelper.CreatePointNormal(
            Point3DHelper.Create(-1 + AbsoluteCoordPrecision / 2, 0, 0), Vector3D.AlongX, {"Pa"})
        Assert.AreEqual(SegmentCutResults.Beyond, segm.GetCutInfo(pl, 1).Action)

        pl = Plane3DHelper.CreatePointNormal(
            Point3DHelper.Create(1 - AbsoluteCoordPrecision / 2, 0, 0), New Vector3D(-1, 0, 0), {"Pb"})
        Assert.AreEqual(SegmentCutResults.Beyond, segm.GetCutInfo(pl, 1).Action)
    End Sub


    <TestMethod>
    Public Sub LineSegment3D_CutBeyond_Ok()
        Dim a = Point3DHelper.Create(1, 0, 0)
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim segm = LineSegment3DHelper.Create(a, b)

        Dim pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(2, 0, 0), New Vector3D(-1, 0, 0), {"Pa"})
        Assert.AreEqual(SegmentCutResults.Beyond, segm.GetCutInfo(pl, 1).Action)
        Assert.AreEqual(SegmentCutResults.EliminateFromA, segm.GetCutInfo(pl, -1).Action)

        pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(-1, 2, 0), Vector3D.AlongX, {"Pb"})
        Assert.AreEqual(SegmentCutResults.Beyond, segm.GetCutInfo(pl, 1).Action)
        Assert.AreEqual(SegmentCutResults.EliminateFromA, segm.GetCutInfo(pl, -1).Action)
    End Sub


    <TestMethod>
    Public Sub LineSegment3D_CutDiagonal_Ok()
        Dim a = Point3DHelper.Create(1, 0, 0)
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim ln = Line3DHelper.Create(a, b)

        Dim segm = LineSegment3DHelper.Create(a, b)
        Dim pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(0, 0, 0), New Vector3D(1, -1, 0), {"Pa"})
        Dim cmd = segm.GetCutInfo(pl, 1)
        Assert.AreEqual(SegmentCutResults.KeepA, cmd.Action)
        Assert.AreEqual(0.5, cmd.Position)

        segm = LineSegment3DHelper.Create(a, b)
        pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(2, 2, 0), New Vector3D(1, -1, 0), {"Pb"})
        cmd = segm.GetCutInfo(pl, -1)
        Assert.AreEqual(SegmentCutResults.KeepB, cmd.Action)
        Assert.AreEqual(0.5, cmd.Position)

        segm = LineSegment3DHelper.Create(a, b)
        pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(0, 0, 0), New Vector3D(1, -1, 0), {"Pc"})
        cmd = segm.GetCutInfo(pl, -1)
        Assert.AreEqual(SegmentCutResults.KeepB, cmd.Action)
        Assert.AreEqual(0.5, cmd.Position)
    End Sub


    <TestMethod>
    Public Sub LineSegment3D_CutAlongX_Ok()
        Dim a = Point3DHelper.Create(3, 0, 0)
        Dim b = Point3DHelper.Create(0, 3, 0)

        Dim segm = LineSegment3DHelper.Create(a, b)
        Dim pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(0, 1, 0), Vector3D.AlongY, {"Pa"})
        Assert.AreEqual(SegmentCutResults.KeepB, segm.GetCutInfo(pl, 1).Action)

        segm = LineSegment3DHelper.Create(a, b)
        pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(0, 1, 0), New Vector3D(0, -1, 0), {"Pb"})
        Assert.AreEqual(SegmentCutResults.KeepB, segm.GetCutInfo(pl, -1).Action)
    End Sub


    <TestMethod>
    Public Sub LineSegment3D_CutAlongY_Ok()
        Dim a = Point3DHelper.Create(3, 0, 0)
        Dim b = Point3DHelper.Create(0, 3, 0)

        Dim segm = LineSegment3DHelper.Create(a, b)
        Dim pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(1, 0, 0), Vector3D.AlongX, {"Pa"})
        Assert.AreEqual(SegmentCutResults.KeepA, segm.GetCutInfo(pl, 1).Action)

        segm = LineSegment3DHelper.Create(a, b)
        pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(1, 0, 0), New Vector3D(-1, 0, 0), {"Pb"})
        Assert.AreEqual(SegmentCutResults.KeepA, segm.GetCutInfo(pl, -1).Action)
    End Sub


    <TestMethod>
    Public Sub LineSegment3D_PointsOnly_CutMiddle_Ok()
        Dim a = Point3DHelper.Create(1, 0, 0)
        Dim b = Point3DHelper.Create(0, 1, 0)
        Dim segm = LineSegment3DHelper.Create(a, b)

        Dim pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(0, 0, 0), New Vector3D(1, -1, 0), {"Pa"})
        Assert.AreEqual(SegmentCutResults.KeepA, segm.GetCutInfo(pl, 1).Action)

        segm = LineSegment3DHelper.Create(a, b)
        pl = Plane3DHelper.CreatePointNormal(Point3DHelper.Create(0, 0, 0), New Vector3D(1, -1, 0), {"Pb"})
        Assert.AreEqual(SegmentCutResults.KeepB, segm.GetCutInfo(pl, -1).Action)
    End Sub


    <TestMethod>
    Public Sub LineSegment3D_CutPlaneTooClose_Ok()
        Dim a = Point3DHelper.Create(-1, 0, 0)
        Dim b = Point3DHelper.Create(1, 0, 0)
        Dim segm = LineSegment3DHelper.Create(a, b)

        ' The plane is almost along the line
        Dim pl = Plane3DHelper.CreatePointNormal(
            Point3DHelper.Create(-0.99, 0, 0), New Vector3D(0.01, 1, 0), {"Pa"})
        Assert.AreEqual(SegmentCutResults.KeepB, segm.GetCutInfo(pl, 1).Action)
    End Sub


    <TestMethod>
    <ExpectedException(GetType(ArgumentException))>
    Public Sub LineSegment3D_CutIncorrectInfo_Throws()
        Dim a = Point3DHelper.Create(-1, 0, 0)
        Dim b = Point3DHelper.Create(1, 0, 0)
        Dim segm = LineSegment3DHelper.Create(a, b)

        Dim info As New SegmentIntersectInfo With {
			.Action = CType(-1, SegmentIntersectResults)
		}
        segm.ConvertToCutInfo(Nothing, 1, info)
    End Sub

#End Region

End Class
