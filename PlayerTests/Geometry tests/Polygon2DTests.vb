Imports Common


<TestClass>
<TestCategory("Polygon 2D")>
Public Class Polygon2DTests

#Region " Polygon2DSide tests "

    <TestMethod>
    Public Sub SideIntersectionParallelSame()
        ' Horizontal line, inside direction is down
        Dim s = Polygon2DSideHelper.CreateInfinite(
            Line2DHelper.Create(Point2DHelper.Create(0, 0),
                                 Point2DHelper.Create(1, 0)),
            Line2DDirections.Right, {"A", "B"})
        ' Identical line
        Dim l = Line2DHelper.Create(Point2DHelper.Create(0, 0),
                                     Point2DHelper.Create(1, 0))
        Dim br = Line2DHelper.CreateBorder(l, Line2DDirections.Right)
        Dim bl = Line2DHelper.CreateBorder(l, Line2DDirections.Left)

        Dim res1 = s.CutByBorder(br)
        Dim res2 = s.CutByBorder(bl)

        Assert.AreEqual(s, res1)
        Assert.IsNull(res2)
    End Sub


    <TestMethod>
    Public Sub SideIntersectionParallelOutsideMerge()
        ' Horizontal line, inside direction is down
        Dim s = Polygon2DSideHelper.CreateInfinite(
            Line2DHelper.Create(Point2DHelper.Create(0, 0),
                                 Point2DHelper.Create(1, 0)),
            Line2DDirections.Right, {"A", "B"})
        ' Parallel line outside
        Dim l = Line2DHelper.Create(Point2DHelper.Create(0, 1),
                                     Point2DHelper.Create(1, 1))
        Dim br = Line2DHelper.CreateBorder(l, Line2DDirections.Right)

        Dim res = s.CutByBorder(br)

        Assert.AreEqual(s, res)
    End Sub


    <TestMethod>
    Public Sub SideIntersectionParallelOutsideEliminate()
        ' Horizontal line, inside direction is down
        Dim s = Polygon2DSideHelper.CreateInfinite(
            Line2DHelper.Create(Point2DHelper.Create(0, 0),
                                 Point2DHelper.Create(1, 0)),
            Line2DDirections.Right, {"A", "B"})
        ' Parallel line outside
        Dim l = Line2DHelper.Create(Point2DHelper.Create(0, 1),
                                     Point2DHelper.Create(1, 1))
        Dim bl = Line2DHelper.CreateBorder(l, Line2DDirections.Left)

        Dim res = s.CutByBorder(bl)

        Assert.IsNull(res)
    End Sub


    <TestMethod>
    Public Sub SideIntersectionParallelInside()
        ' Horizontal line, inside direction is down
        Dim s = Polygon2DSideHelper.CreateInfinite(
            Line2DHelper.Create(Point2DHelper.Create(0, 0),
                                 Point2DHelper.Create(1, 0)),
            Line2DDirections.Right, {"A", "B"})
        ' Parallel line inside
        Dim l = Line2DHelper.Create(Point2DHelper.Create(0, -1),
                                     Point2DHelper.Create(1, -1))
        Dim br = Line2DHelper.CreateBorder(l, Line2DDirections.Right)

        Dim res = s.CutByBorder(br)

        Assert.IsNull(res)
    End Sub


    <TestMethod>
    Public Sub SideIntersectionCutToRight()
        ' Horizontal line, inside direction is down
        Dim s = Polygon2DSideHelper.CreateInfinite(
            Line2DHelper.Create(Point2DHelper.Create(0, 0),
                                 Point2DHelper.Create(1, 0)),
            Line2DDirections.Right, {"A", "B"})
        ' Perpendicular line
        Dim l = Line2DHelper.Create(Point2DHelper.Create(0, 0),
                                     Point2DHelper.Create(0, 1))
        Dim br = Line2DHelper.CreateBorder(l, Line2DDirections.Right)

        Dim res = s.CutByBorder(br)

        Assert.AreEqual(s.Line, res.Line)
        Assert.AreEqual(s.InsideDirection, res.InsideDirection)
        Assert.AreEqual(0.0, res.CoordinateA)
        Assert.AreEqual(Double.PositiveInfinity, res.CoordinateB)
    End Sub


    <TestMethod>
    Public Sub SideIntersectionCutToLeft()
        ' Horizontal line, inside direction is down
        Dim s = Polygon2DSideHelper.CreateInfinite(
            Line2DHelper.Create(Point2DHelper.Create(0, 0),
                                 Point2DHelper.Create(1, 0)),
            Line2DDirections.Right, {"A", "B"})
        ' Perpendicular line
        Dim l = Line2DHelper.Create(Point2DHelper.Create(0, 0),
                                     Point2DHelper.Create(0, 1))
        Dim bl = Line2DHelper.CreateBorder(l, Line2DDirections.Left)

        Dim res = s.CutByBorder(bl)

        Assert.AreEqual(s.Line, res.Line)
        Assert.AreEqual(s.InsideDirection, res.InsideDirection)
        Assert.AreEqual(Double.NegativeInfinity, res.CoordinateA)
        Assert.AreEqual(0.0, res.CoordinateB)
    End Sub


    <TestMethod>
    Public Sub SideIntersectionCutToRightReverse()
        ' Horizontal line, inside direction is down
        Dim s = Polygon2DSideHelper.CreateInfinite(
            Line2DHelper.Create(Point2DHelper.Create(0, 0),
                                 Point2DHelper.Create(1, 0)),
            Line2DDirections.Right, {"A", "B"})
        ' Perpendicular line
        Dim l = Line2DHelper.Create(Point2DHelper.Create(0, 0),
                                     Point2DHelper.Create(0, -1))
        Dim br = Line2DHelper.CreateBorder(l, Line2DDirections.Right)

        Dim res = s.CutByBorder(br)

        Assert.AreEqual(s.Line, res.Line)
        Assert.AreEqual(s.InsideDirection, res.InsideDirection)
        Assert.AreEqual(Double.NegativeInfinity, res.CoordinateA)
        Assert.AreEqual(0.0, res.CoordinateB)
    End Sub


    <TestMethod>
    Public Sub SideIntersectionCutToLeftReverse()
        ' Horizontal line, inside direction is down
        Dim s = Polygon2DSideHelper.CreateInfinite(
            Line2DHelper.Create(Point2DHelper.Create(0, 0),
                                 Point2DHelper.Create(1, 0)),
            Line2DDirections.Right, {"A", "B"})
        ' Perpendicular line
        Dim l = Line2DHelper.Create(Point2DHelper.Create(0, 0),
                                     Point2DHelper.Create(0, -1))
        Dim bl = Line2DHelper.CreateBorder(l, Line2DDirections.Left)

        Dim res = s.CutByBorder(bl)

        Assert.AreEqual(s.Line, res.Line)
        Assert.AreEqual(s.InsideDirection, res.InsideDirection)
        Assert.AreEqual(0.0, res.CoordinateA)
        Assert.AreEqual(Double.PositiveInfinity, res.CoordinateB)
    End Sub

#End Region


#Region " Polygon2D cut tests "

    <TestMethod>
    Public Sub Polygon2DCut_HalfInfiniteLeft()
        Dim p = Polygon2DHelper.CreateInfinite({1})
        Dim res = p.CutByLine(Line2DHelper.Create(Point2DHelper.Create(-1, 0),
                                                  Point2DHelper.Create(1, 0)),
                              Line2DDirections.Left)

        Assert.IsTrue(res)
        Assert.AreEqual(1, p.Sides.Count)

        Dim side1 = p.Sides.First()
        Assert.AreEqual(2.0, side1.Line.Vector.X)
        Assert.AreEqual(0.0, side1.Line.Vector.Y)
        Assert.AreEqual(Double.NegativeInfinity, side1.CoordinateA)
        Assert.AreEqual(Double.PositiveInfinity, side1.CoordinateB)
    End Sub


    <TestMethod>
    Public Sub Polygon2DCut_QuarterInfinite()
        Dim p = Polygon2DHelper.CreateInfinite({1})
        Dim res1 = p.CutByLine(Line2DHelper.Create(Point2DHelper.Create(-1, 0),
                                                   Point2DHelper.Create(1, 0)),
                              Line2DDirections.Left)
        Dim res2 = p.CutByLine(Line2DHelper.Create(Point2DHelper.Create(0, -1),
                                                   Point2DHelper.Create(0, 1)),
                               Line2DDirections.Right)

        Assert.IsTrue(res1)
        Assert.IsTrue(res2)
        Assert.AreEqual(2, p.Sides.Count)
    End Sub


    <TestMethod>
    Public Sub Polygon2DCut_TriangleClockwise()
        Dim p = Polygon2DHelper.CreateInfinite({"Main"})
        Dim res1 = p.CutByLine(Line2DHelper.Create(Point2DHelper.Create(1, 0),
                                                   Point2DHelper.Create(-1, 0)),
                               Line2DDirections.Right)
        Dim res2 = p.CutByLine(Line2DHelper.Create(Point2DHelper.Create(0, -1),
                                                   Point2DHelper.Create(0, 1)),
                               Line2DDirections.Right)
        Dim res3 = p.CutByLine(Line2DHelper.Create(Point2DHelper.Create(0, 2),
                                                   Point2DHelper.Create(2, 0)),
                               Line2DDirections.Right)

        Assert.IsTrue(res1)
        Assert.IsTrue(res2)
        Assert.IsTrue(res3)
        Assert.AreEqual(3, p.Sides.Count)

        Dim side1 = p.Sides(0)
        Dim s1a = side1.Line.GetPoint(side1.CoordinateA)
        Dim s1b = side1.Line.GetPoint(side1.CoordinateB)
        Assert.AreEqual(2.0, s1a.X)
        Assert.AreEqual(0.0, s1a.Y)
        Assert.AreEqual(0.0, s1b.X)
        Assert.AreEqual(0.0, s1b.Y)

        Dim side2 = p.Sides(1)
        Dim s2a = side2.Line.GetPoint(side2.CoordinateA)
        Dim s2b = side2.Line.GetPoint(side2.CoordinateB)
        Assert.AreEqual(0.0, s2a.X)
        Assert.AreEqual(0.0, s2a.Y)
        Assert.AreEqual(0.0, s2b.X)
        Assert.AreEqual(2.0, s2b.Y)

        Dim side3 = p.Sides(2)
        Dim s3a = side3.Line.GetPoint(side3.CoordinateA)
        Dim s3b = side3.Line.GetPoint(side3.CoordinateB)
        Assert.AreEqual(0.0, s3a.X)
        Assert.AreEqual(2.0, s3a.Y)
        Assert.AreEqual(2.0, s3b.X)
        Assert.AreEqual(0.0, s3b.Y)
    End Sub


    <TestMethod>
    Public Sub Polygon2DCut_CutAway()
        Dim p = Polygon2DHelper.CreateInfinite({1})
        Dim res1 = p.CutByLine(Line2DHelper.Create(Point2DHelper.Create(-1, 0),
                                                    Point2DHelper.Create(1, 0)),
                               Line2DDirections.Left)
        Dim res2 = p.CutByLine(Line2DHelper.Create(Point2DHelper.Create(0, -1),
                                                    Point2DHelper.Create(0, 1)),
                               Line2DDirections.Right)
        Dim res3 = p.CutByLine(Line2DHelper.Create(Point2DHelper.Create(0, 2),
                                                    Point2DHelper.Create(2, 0)),
                               Line2DDirections.Right)
        Dim res4 = p.CutByLine(Line2DHelper.Create(Point2DHelper.Create(0, 3),
                                                    Point2DHelper.Create(3, 0)),
                               Line2DDirections.Left)

        Assert.IsTrue(res1)
        Assert.IsTrue(res2)
        Assert.IsTrue(res3)
        Assert.IsFalse(res4)
    End Sub


    <TestMethod>
    Public Sub Polygon2DCut_DegeneratePolygonEliminated()
        Dim p = Polygon2DHelper.Create({
            Point2DHelper.Create(-1, -1, {"A"}),
            Point2DHelper.Create(-1, 1, {"B"}),
            Point2DHelper.Create(1, 1, {"C"}),
            Point2DHelper.Create(1, -1, {"D"})})
        Dim res1 = p.CutByLine(Line2DHelper.Create(Point2DHelper.Create(-1, 0), Vector2DHelper.Create(0, -1)),
                               Line2DDirections.Right)

        Assert.IsFalse(res1)
    End Sub


    <TestMethod>
    Public Sub Polygon2DCut_ByOwnSides()
        Dim p = Polygon2DHelper.Create({
            Point2DHelper.Create(-1, -1, {"A"}),
            Point2DHelper.Create(-1, 1, {"B"}),
            Point2DHelper.Create(1, 1, {"C"}),
            Point2DHelper.Create(1, -1, {"D"})})
        Dim res1 = p.CutByLine(Line2DHelper.Create(Point2DHelper.Create(-1, 0), Vector2DHelper.Create(0, 1)),
                              Line2DDirections.Right)
        Dim res2 = p.CutByLine(Line2DHelper.Create(Point2DHelper.Create(0, 1), Vector2DHelper.Create(1, 0)),
                              Line2DDirections.Right)
        Dim res3 = p.CutByLine(Line2DHelper.Create(Point2DHelper.Create(1, 0), Vector2DHelper.Create(0, -1)),
                              Line2DDirections.Right)
        Dim res4 = p.CutByLine(Line2DHelper.Create(Point2DHelper.Create(0, -1), Vector2DHelper.Create(-1, 0)),
                              Line2DDirections.Right)

        Assert.IsTrue(res1)
        Assert.IsTrue(res2)
        Assert.IsTrue(res3)
        Assert.IsTrue(res4)
        Assert.AreEqual(4, p.Sides.Count)
    End Sub


    <TestMethod>
    Public Sub Polygon2DCut_ByParallel()
        Dim p = Polygon2DHelper.Create({
            Point2DHelper.Create(-1, -0.5, {"A"}),
            Point2DHelper.Create(-1, 2, {"B"}),
            Point2DHelper.Create(0.7882353, 2, {"C"}),
            Point2DHelper.Create(0.9352941, -0.5, {"D"})})
        Dim cutLine = Line2DHelper.Create(
            Point2DHelper.Create(-0.8, 0), Vector2DHelper.Create(-0.058722, 0.99827437))

        Dim res = p.CutByLine(cutLine, Line2DDirections.Right)

        Assert.IsTrue(res)
        Assert.AreEqual(4, p.Sides.Count)
    End Sub


    <TestMethod>
    Public Sub Polygon2DCut_ParallelFarAway()
        Dim p = Polygon2DHelper.Create({
            Point2DHelper.Create(-1.06, -0.61, {"A"}),
            Point2DHelper.Create(0, 1.22, {"B"}),
            Point2DHelper.Create(1.06, -0.61, {"C"})})
        Dim cutLine = Line2DHelper.Create(
            Point2DHelper.Create(0, 4.29), Vector2DHelper.Create(1.41, 0))

        Dim res = p.CutByLine(cutLine, Line2DDirections.Right)

        Assert.IsTrue(res)
        Assert.AreEqual(3, p.Sides.Count)
    End Sub

#End Region


#Region " Polygon2D contain tests "

    <TestMethod>
    Public Sub Polygon2D_InfiniteContainsAll()
        Dim p = Polygon2DHelper.CreateInfinite({1})

        Dim res = p.Contains(Point2DHelper.Create(1, 0))

        Assert.IsTrue(res)
    End Sub


    <TestMethod>
    Public Sub Polygon2D_HalfInfiniteLeft()
        Dim p = Polygon2DHelper.CreateInfinite({1})
        Dim res = p.CutByLine(Line2DHelper.Create(Point2DHelper.Create(-1, 0),
                                                  Point2DHelper.Create(1, 0)),
                              Line2DDirections.Left)

        Assert.IsTrue(res)
        Assert.IsTrue(p.Contains(Point2DHelper.Create(0, 1)))
        Assert.IsFalse(p.Contains(Point2DHelper.Create(0, -1)))
    End Sub


    <TestMethod>
    Public Sub Polygon2D_HalfInfiniteRight()
        Dim p = Polygon2DHelper.CreateInfinite({1})
        Dim res = p.CutByLine(Line2DHelper.Create(Point2DHelper.Create(-1, 0),
                                                   Point2DHelper.Create(1, 0)),
                              Line2DDirections.Right)

        Assert.IsTrue(res)
        Assert.IsTrue(p.Contains(Point2DHelper.Create(0, -1)))
        Assert.IsFalse(p.Contains(Point2DHelper.Create(0, 1)))
    End Sub


    <TestMethod>
    Public Sub Polygon2D_QuarterInfinite()
        Dim p = Polygon2DHelper.CreateInfinite({1})
        Dim res1 = p.CutByLine(Line2DHelper.Create(Point2DHelper.Create(-1, 0),
                                                   Point2DHelper.Create(1, 0)),
                              Line2DDirections.Left)
        Dim res2 = p.CutByLine(Line2DHelper.Create(Point2DHelper.Create(0, -1),
                                                    Point2DHelper.Create(0, 1)),
                               Line2DDirections.Right)

        Assert.IsTrue(res1)
        Assert.IsTrue(res2)
        Assert.IsTrue(p.Contains(Point2DHelper.Create(1, 1)))
        Assert.IsTrue(p.Contains(Point2DHelper.Create(0.01, 0.01)))
        Assert.IsFalse(p.Contains(Point2DHelper.Create(-1, 1)))
        Assert.IsFalse(p.Contains(Point2DHelper.Create(1, -1)))
        Assert.IsFalse(p.Contains(Point2DHelper.Create(-1, -1)))
    End Sub


    <TestMethod>
    Public Sub Polygon2D_TriangleContains()
        Dim p = Polygon2DHelper.CreateInfinite({"Main"})
        Dim res1 = p.CutByLine(Line2DHelper.Create(Point2DHelper.Create(-1, 0),
                                                    Point2DHelper.Create(1, 0)),
                               Line2DDirections.Left)
        Dim res2 = p.CutByLine(Line2DHelper.Create(Point2DHelper.Create(0, -1),
                                                    Point2DHelper.Create(0, 1)),
                               Line2DDirections.Right)
        Dim res3 = p.CutByLine(Line2DHelper.Create(Point2DHelper.Create(0, 2),
                                                    Point2DHelper.Create(2, 0)),
                               Line2DDirections.Right)

        Assert.IsTrue(res1)
        Assert.IsTrue(res2)
        Assert.IsTrue(res3)
        Assert.IsTrue(p.Contains(Point2DHelper.Create(0.5, 0.5)))

        ' On the boundary = not inside
        Assert.IsFalse(p.Contains(Point2DHelper.Create(1, 1)))
        Assert.IsFalse(p.Contains(Point2DHelper.Create(0, 2)))
        Assert.IsFalse(p.Contains(Point2DHelper.Create(2, 0)))
        Assert.IsFalse(p.Contains(Point2DHelper.Create(-1, 1)))
        Assert.IsFalse(p.Contains(Point2DHelper.Create(1, -1)))
        Assert.IsFalse(p.Contains(Point2DHelper.Create(-1, -1)))
        Assert.IsFalse(p.Contains(Point2DHelper.Create(2, 2)))
    End Sub

#End Region

End Class
