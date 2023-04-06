Imports Common
Imports RoomDivisionLibrary


<TestClass>
Public Class PlaneProjectionTests

#Region " Utility "

    ''' <summary>
    ''' Test that the distance between projections is the same
    ''' as the distance between the original points.
    ''' </summary>
    Private Shared Sub TestDistances(Of TRef)(
        a As PointAsVertex2D(Of TRef), b As PointAsVertex2D(Of TRef))

        Assert.AreEqual(
            Point3DHelper.Distance(a.OriginalPoint, b.OriginalPoint),
            Point2DHelper.Distance(Point2DHelper.Create(a.Position(0), a.Position(1)),
                                   Point2DHelper.Create(b.Position(0), b.Position(1))),
            AbsoluteCoordPrecision)
    End Sub

#End Region


#Region " Layout steps tests: plane projection "

    <TestMethod>
    Public Sub Layouter3D_CalcProjectionPlane_TwoAcrossAudience_Ok()
        Dim spk = {
            Point3DHelper.Create(-1, 0, 0, {"A"}),
            Point3DHelper.Create(1, 0, 0, {"B"})
        }

        Dim projList As IList(Of PointAsVertex2D(Of String)) = Nothing
        Dim res = PolyhedronFromPlanarCreator.CalcProjectionPlane(spk, projList)

        Assert.IsTrue(res)
        Assert.AreEqual(2, projList.Count)
        Dim h = projList.First().Plane.GetDistanceToPoint(Point3DHelper.Origin)
        Assert.AreEqual(0, h, AbsoluteCoordPrecision)

        ' The first point is the projection's origins,
        ' and all distances are as in the original points
        'TestPoint(projList(0), 0, 0)
        TestDistances(projList(0), projList(1))
    End Sub


    <TestMethod>
    Public Sub Layouter3D_CalcProjectionPlane_TwoAsideAudience_Ok()
        Dim spk = {
            Point3DHelper.Create(-1, 0.5, 0.5, {"A"}),
            Point3DHelper.Create(1, 0.5, 0.5, {"B"})
        }

        Dim projList As IList(Of PointAsVertex2D(Of String)) = Nothing
        Dim res = PolyhedronFromPlanarCreator.CalcProjectionPlane(spk, projList)

        Assert.IsTrue(res)
        Assert.AreEqual(2, projList.Count)

        Dim h = projList.First().Plane.GetDistanceToPoint(Point3DHelper.Origin)
        Assert.AreEqual(0.5, h, AbsoluteCoordPrecision)

        ' The first point is the projection's origins,
        ' and all distances are as in the original points
        'TestPoint(projList(0), 0, 0)
        TestDistances(projList(0), projList(1))
    End Sub


    <TestMethod>
    Public Sub Layouter3D_CalcProjectionPlane_ThreeOnLine_Ok()
        Dim spk = {
            Point3DHelper.Create(-1, 0, 1.2, {"A"}),
            Point3DHelper.Create(0, 0, 1.2, {"B"}),
            Point3DHelper.Create(1, 0, 1.2, {"C"})
        }

        Dim projList As IList(Of PointAsVertex2D(Of String)) = Nothing
        Dim res = PolyhedronFromPlanarCreator.CalcProjectionPlane(spk, projList)

        Assert.IsTrue(res)
        Assert.AreEqual(3, projList.Count)

        Dim h = projList.First().Plane.GetDistanceToPoint(Point3DHelper.Origin)
        Assert.AreEqual(1.44, h, AbsoluteCoordPrecision)

        ' The first point is the projection's origins,
        ' and all distances are as in the original points
        'TestPoint(projList(0), 0, 0)
        TestDistances(projList(0), projList(1))
        TestDistances(projList(0), projList(2))
        TestDistances(projList(1), projList(2))
    End Sub


    <TestMethod>
    Public Sub Layouter3D_CalcProjectionPlane_ThreeAcrossAudience_Ok()
        Dim spk = {
            Point3DHelper.Create(-1, 0, 0, {"A"}),
            Point3DHelper.Create(0, 1, 0, {"B"}),
            Point3DHelper.Create(1, 0, 0, {"C"})
        }

        Dim projList As IList(Of PointAsVertex2D(Of String)) = Nothing
        Dim res = PolyhedronFromPlanarCreator.CalcProjectionPlane(spk, projList)

        Assert.IsTrue(res)
        Assert.AreEqual(3, projList.Count)
        Dim h = projList.First().Plane.GetDistanceToPoint(Point3DHelper.Origin)
        Assert.AreEqual(0, h, AbsoluteCoordPrecision)

        ' The first point is the projection's origins,
        ' and all distances are as in the original points
        'TestPoint(projList(0), 0, 0)
        TestDistances(projList(0), projList(1))
        TestDistances(projList(0), projList(2))
        TestDistances(projList(1), projList(2))
    End Sub


    <TestMethod>
    Public Sub Layouter3D_CalcProjectionPlane_ThreeInFrontOfAudience_Ok()
        Dim spk = {
            Point3DHelper.Create(-1, 2, 1, {"A"}),
            Point3DHelper.Create(0, 2, 1, {"B"}),
            Point3DHelper.Create(1, 2, 1, {"C"})
        }

        Dim projList As IList(Of PointAsVertex2D(Of String)) = Nothing
        Dim res = PolyhedronFromPlanarCreator.CalcProjectionPlane(spk, projList)

        Assert.IsTrue(res)
        Assert.AreEqual(3, projList.Count)
        Dim h = projList.First().Plane.GetDistanceToPoint(Point3DHelper.Origin)
        Assert.AreEqual(5.0, h, AbsoluteCoordPrecision)

        ' The first point is the projection's origins,
        ' and all distances are as in the original points
        'TestPoint(projList(0), 0, 0)
        TestDistances(projList(0), projList(1))
        TestDistances(projList(0), projList(2))
        TestDistances(projList(1), projList(2))
    End Sub


    <TestMethod>
    Public Sub Layouter3D_CalcProjectionPlane_FourNotPlane_Ok()
        Dim spk = {
            Point3DHelper.Create(-1, 0, 0, {"A"}),
            Point3DHelper.Create(0, 1, 0, {"B"}),
            Point3DHelper.Create(0, 0, 1, {"C"}),
            Point3DHelper.Create(1, 0, 0, {"D"})
        }

        Dim projList As IList(Of PointAsVertex2D(Of String)) = Nothing
        Dim res = PolyhedronFromPlanarCreator.CalcProjectionPlane(spk, projList)

        Assert.IsFalse(res)
        Assert.IsNull(projList)
    End Sub

#End Region

End Class