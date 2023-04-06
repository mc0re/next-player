Imports Common
Imports RoomDivisionLibrary


<TestClass>
Public Class Room2DLayoutTests

    Private Shared ReadOnly Origin As IPoint2D = Point2DHelper.Create(0, 0)


    <TestMethod>
    Public Sub Room2D_CutByOnePoint()
        Dim room As New Room2D(8, 6, 2, 2)
        Dim spk = {
            Point2DHelper.Create(0, 0, {1}),
            Point2DHelper.Create(0, 0, {2})
        }

        Dim layout = New Room2DLayouter(Of Integer)()
        layout.PrepareLayout(room, spk)

        Assert.AreEqual(1, layout.GetPolygons().Count)

        TestAssert.AreEqual({1, 2}, layout.GetReferences(Origin))
        TestAssert.AreEqual(New Integer() {}, layout.GetReferences(Point2DHelper.Create(5, 5)))
    End Sub


    <TestMethod>
    Public Sub Room2D_CutByTwoCornerPoints()
        Dim room As New Room2D(8, 6, 2, 2)
        Dim spk = {
            Point2DHelper.Create(-4, -3, {1}),
            Point2DHelper.Create(4, 3, {2})
        }

        Dim layout = New Room2DLayouter(Of Integer)()
        layout.PrepareLayout(room, spk)

        Assert.AreEqual(1, layout.GetPolygons().Count)

        TestAssert.AreEqual({1, 2}, layout.GetReferences(Origin))
        TestAssert.AreEqual({1, 2}, layout.GetReferences(Point2DHelper.Create(-3, 2)))
        TestAssert.AreEqual({1, 2}, layout.GetReferences(Point2DHelper.Create(3, -2)))
    End Sub


    <TestMethod>
    <DataRow(-1)>
    <DataRow(0)>
    <DataRow(1)>
    Public Sub Room2D_CutByTwoPoints(y As Double)
        Dim room As New Room2D(8, 6, 2, 2)
        Dim spk = {
            Point2DHelper.Create(-2, 0, {1}),
            Point2DHelper.Create(2, 0, {2})
        }

        Dim layout = New Room2DLayouter(Of Integer)()
        layout.PrepareLayout(room, spk)

        Assert.AreEqual(3, layout.GetPolygons().Count)

        Assert.AreEqual(0, layout.GetReferences(Point2DHelper.Create(-5, y)).Count)
        TestAssert.AreEqual({1}, layout.GetReferences(Point2DHelper.Create(-3, y)))
        TestAssert.AreEqual({1}, layout.GetReferences(Point2DHelper.Create(-2, y)))
        TestAssert.AreEqual({1, 2}, layout.GetReferences(Point2DHelper.Create(0, y)))
        TestAssert.AreEqual({2}, layout.GetReferences(Point2DHelper.Create(2, y)))
        TestAssert.AreEqual({2}, layout.GetReferences(Point2DHelper.Create(3, y)))
        Assert.AreEqual(0, layout.GetReferences(Point2DHelper.Create(5, y)).Count)
    End Sub


    <TestMethod>
    <DataRow(-1)>
    <DataRow(0)>
    <DataRow(1)>
    Public Sub Room2D_CutByThreeColinearPoints(y As Double)
        Dim room As New Room2D(8, 6, 2, 2)
        Dim spk = {
            Point2DHelper.Create(-2, 0, {1}),
            Point2DHelper.Create(0, 0, {2}),
            Point2DHelper.Create(2, 0, {3})
        }

        Dim layout = New Room2DLayouter(Of Integer)()
        layout.PrepareLayout(room, spk)

        Assert.AreEqual(4, layout.GetPolygons().Count)

        Assert.AreEqual(0, layout.GetReferences(Point2DHelper.Create(-5, y)).Count)
        TestAssert.AreEqual({1}, layout.GetReferences(Point2DHelper.Create(-3, y)))
        TestAssert.AreEqual({1}, layout.GetReferences(Point2DHelper.Create(-2, y)))
        TestAssert.AreEqual({1, 2}, layout.GetReferences(Point2DHelper.Create(-1, y)))
        TestAssert.AreEqual({2}, layout.GetReferences(Point2DHelper.Create(0, y)))
        TestAssert.AreEqual({2, 3}, layout.GetReferences(Point2DHelper.Create(1, y)))
        TestAssert.AreEqual({3}, layout.GetReferences(Point2DHelper.Create(2, 0)))
        TestAssert.AreEqual({3}, layout.GetReferences(Point2DHelper.Create(3, 0)))
        Assert.AreEqual(0, layout.GetReferences(Point2DHelper.Create(5, 0)).Count)
    End Sub


    <TestMethod>
    <DataRow(-1)>
    <DataRow(0)>
    <DataRow(1)>
    Public Sub Room2D_CutByColinearColocatedPoints(y As Double)
        Dim room As New Room2D(8, 6, 2, 2)
        Dim spk = {
            Point2DHelper.Create(-2, 0, {1}),
            Point2DHelper.Create(0, 0, {2}),
            Point2DHelper.Create(0, 0, {3}),
            Point2DHelper.Create(2, 0, {4})
        }

        Dim layout = New Room2DLayouter(Of Integer)()
        layout.PrepareLayout(room, spk)

        Assert.AreEqual(4, layout.GetPolygons().Count)

        TestAssert.AreEqual({1}, layout.GetReferences(Point2DHelper.Create(-3, y)))
        TestAssert.AreEqual({1}, layout.GetReferences(Point2DHelper.Create(-2, y)))
        TestAssert.AreEqual({1, 2, 3}, layout.GetReferences(Point2DHelper.Create(-1, y)))
        TestAssert.AreEqual({2, 3}, layout.GetReferences(Point2DHelper.Create(0, y)))
        TestAssert.AreEqual({2, 3, 4}, layout.GetReferences(Point2DHelper.Create(1, y)))
        TestAssert.AreEqual({4}, layout.GetReferences(Point2DHelper.Create(2, y)))
        TestAssert.AreEqual({4}, layout.GetReferences(Point2DHelper.Create(3, y)))
    End Sub


    <TestMethod>
    Public Sub Room2D_CutByThreePoints()
        Dim room As New Room2D(8, 6, 2, 2)
        Dim spk = {
            Point2DHelper.Create(-2, 0, {1}),
            Point2DHelper.Create(0, 1, {2}),
            Point2DHelper.Create(2, 0, {3})
        }

        Dim layout = New Room2DLayouter(Of Integer)()
        layout.PrepareLayout(room, spk)

        Assert.AreEqual(6, layout.GetPolygons().Count)

        ' Points match
        TestAssert.AreEqual({1}, layout.GetReferences(Point2DHelper.Create(-2, 0)))
        TestAssert.AreEqual({2}, layout.GetReferences(Point2DHelper.Create(0, 1)))
        TestAssert.AreEqual({3}, layout.GetReferences(Point2DHelper.Create(2, 0)))

        ' Segments match
        TestAssert.AreEqual({1, 2}, layout.GetReferences(Point2DHelper.Create(-1, 0.5)))
        TestAssert.AreEqual({2, 3}, layout.GetReferences(Point2DHelper.Create(1, 0.5)))
        TestAssert.AreEqual({3, 1}, layout.GetReferences(Point2DHelper.Create(0, 0)))
        TestAssert.AreEqual({1}, layout.GetReferences(Point2DHelper.Create(-3, 0)))
        TestAssert.AreEqual({2}, layout.GetReferences(Point2DHelper.Create(0, 3)))
        TestAssert.AreEqual({3}, layout.GetReferences(Point2DHelper.Create(3, 0)))

        ' Points inside polygons
        TestAssert.AreEqual({1, 2, 3}, layout.GetReferences(Point2DHelper.Create(0, 0.5)))
        TestAssert.AreEqual({1}, layout.GetReferences(Point2DHelper.Create(-3.9, -0.3)))
        TestAssert.AreEqual({1, 2}, layout.GetReferences(Point2DHelper.Create(-3, 1)))
        TestAssert.AreEqual({2, 3}, layout.GetReferences(Point2DHelper.Create(3, 1)))
        TestAssert.AreEqual({3}, layout.GetReferences(Point2DHelper.Create(3.9, -0.3)))
        TestAssert.AreEqual({3, 1}, layout.GetReferences(Point2DHelper.Create(0, -1)))
    End Sub


    <TestMethod>
    Public Sub Room2D_CutByFourPointsOutsideAudience()
        Dim room As New Room2D(8, 6, 2, 2)
        Dim spk = {
            Point2DHelper.Create(-3, 0, {1}),
            Point2DHelper.Create(0, 2, {2}),
            Point2DHelper.Create(0, -2, {3}),
            Point2DHelper.Create(3, 0, {4})
        }

        Dim layout = New Room2DLayouter(Of Integer)()
        layout.PrepareLayout(room, spk)

        Assert.AreEqual(6, layout.GetPolygons().Count)

        ' Points match
        TestAssert.AreEqual({1}, layout.GetReferences(Point2DHelper.Create(-3, 0)))
        TestAssert.AreEqual({2}, layout.GetReferences(Point2DHelper.Create(0, 2)))
        TestAssert.AreEqual({3}, layout.GetReferences(Point2DHelper.Create(0, -2)))
        TestAssert.AreEqual({4}, layout.GetReferences(Point2DHelper.Create(3, 0)))

        ' Segment match
        TestAssert.AreEqual({3, 2}, layout.GetReferences(Origin))
        TestAssert.AreEqual({1, 2}, layout.GetReferences(Point2DHelper.Create(-1.5, 1)))
        TestAssert.AreEqual({1, 2, 3}, layout.GetReferences(Point2DHelper.Create(-1, 0)))

        TestAssert.AreEqual({1}, layout.GetReferences(Point2DHelper.Create(-3.5, 0)))
    End Sub


    <TestMethod>
    <DataRow(-1, 0, 1, 0)>
    Public Sub Room2D_ThreePointsCrashTest_Ok(
        x1 As Double, y1 As Double,
        x2 As Double, y2 As Double
    )
        Dim room As New Room2D(3, 2, 0.4, 0.4)

        For x3 = room.Borders.Left To room.Borders.Right Step 0.1
            For y3 = room.Borders.Back To room.Borders.Front Step 0.1
                Dim rs As I2DLayouter(Of String) = New Room2DLayouter(Of String)()

                Dim channelList As New List(Of Point2D(Of String)) From {
                        Point2DHelper.Create(x1, y1, {"A"}),
                        Point2DHelper.Create(x2, y2, {"B"}),
                        Point2DHelper.Create(x3, y3, {"C"})
                    }

                rs.PrepareLayout(room, channelList)
                Dim visList = rs.GetPolygons()

                Assert.IsNotNull(visList)
                Assert.IsTrue(visList.Count > 0)
            Next
        Next
    End Sub

End Class
