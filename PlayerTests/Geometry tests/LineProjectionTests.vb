Imports Common
Imports RoomDivisionLibrary


<TestClass>
Public Class LineProjectionTests

#Region " Layout steps tests: line projection "

    <TestMethod>
    Public Sub Layouter3D_CalcProjectionLine_TwoPoints_Ok()
        Dim spk = {
            Point3DHelper.Create(-1, 0, 0, {"A"}),
            Point3DHelper.Create(1, 0, 0, {"B"})
        }

        Dim projList As IList(Of PointAsVertex1D(Of String)) = Nothing
        Dim res = PolyhedronFromLinearCreator.CalcProjectionLine(spk, projList)

        Assert.IsTrue(res)
        Assert.AreEqual(2, projList.Count)
        Dim h = projList.First().Line.GetCoordinate(Point3DHelper.Origin)
        Assert.IsTrue(h.HasValue)

        ' The first point is the projection's origins
        Assert.AreEqual(0.0, projList(0).Projection)
        Assert.AreEqual(spk(0), projList(0).OriginalPoint)
        Assert.AreEqual(spk(1), projList(1).OriginalPoint)
    End Sub


    <TestMethod>
    Public Sub Layouter3D_CalcProjectionLine_ThreeOnLine_Ok()
        Dim spk = {
            Point3DHelper.Create(-1, 0, 1, {"A"}),
            Point3DHelper.Create(1, 0, 1, {"B"}),
            Point3DHelper.Create(0, 0, 1, {"C"})
        }

        Dim projList As IList(Of PointAsVertex1D(Of String)) = Nothing
        Dim res = PolyhedronFromLinearCreator.CalcProjectionLine(spk, projList)

        Assert.IsTrue(res)
        Assert.AreEqual(3, projList.Count)

        ' The first point is the projection's origins
        Assert.AreEqual(0.0, projList(0).Projection)
        Assert.AreEqual(spk(0), projList(0).OriginalPoint)
        Assert.AreEqual(spk(2), projList(1).OriginalPoint)
        Assert.AreEqual(spk(1), projList(2).OriginalPoint)
    End Sub


    <TestMethod>
    Public Sub Layouter3D_CalcProjectionLine_FourOnLine_Ok()
        Dim spk = {
            Point3DHelper.Create(-1, 0, 1, {"A"}),
            Point3DHelper.Create(1, 0, 1, {"B"}),
            Point3DHelper.Create(2, 0, 1, {"C"}),
            Point3DHelper.Create(0, 0, 1, {"D"})
        }

        Dim projList As IList(Of PointAsVertex1D(Of String)) = Nothing
        Dim res = PolyhedronFromLinearCreator.CalcProjectionLine(spk, projList)

        Assert.IsTrue(res)
        Assert.AreEqual(4, projList.Count)

        ' The first point is the projection's origins
        Assert.AreEqual(0.0, projList(0).Projection)
        Assert.AreEqual(spk(0), projList(0).OriginalPoint)
        Assert.AreEqual(spk(3), projList(1).OriginalPoint)
        Assert.AreEqual(spk(1), projList(2).OriginalPoint)
        Assert.AreEqual(spk(2), projList(3).OriginalPoint)
    End Sub


    <TestMethod>
    Public Sub Layouter3D_CalcProjectionLine_ThreeNotOnLine_Ok()
        Dim spk = {
            Point3DHelper.Create(-1, 0, 1, {"A"}),
            Point3DHelper.Create(1, 0, 1, {"B"}),
            Point3DHelper.Create(0, 1, 1, {"C"})
        }

        Dim projList As IList(Of PointAsVertex1D(Of String)) = Nothing
        Dim res = PolyhedronFromLinearCreator.CalcProjectionLine(spk, projList)

        Assert.IsFalse(res)
    End Sub

#End Region

End Class
