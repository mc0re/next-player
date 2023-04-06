Imports Common


<TestClass>
<TestCategory("2D geometry")>
Public Class Geometry2DTests

#Region " Constants "

    Private Shared ReadOnly Origin As IPoint2D = Point2DHelper.Create(0, 0)

#End Region


#Region " Vector2D tests "

    <TestMethod>
    Public Sub Vector2D_Zero_Ok()
        Dim v1 = Vector2DHelper.Create(0, 0)

        Assert.IsTrue(v1.IsZero)
        Assert.AreEqual(0.0, v1.Angle)
    End Sub


    <TestMethod>
    Public Sub Vector2D_Perpendicular_Ok()
        Dim v1 = Vector2DHelper.Create(2, 0)
        Dim v2 = v1.Perpendicular

        Assert.AreEqual(0.0, v2.X)
        Assert.AreEqual(-1.0, v2.Y)
    End Sub


    <TestMethod>
    Public Sub Vector2D_AngleSame()
        Dim v1 = Vector2DHelper.Create(1, 1)

        Assert.AreEqual(0, Vector2DHelper.CompareAngles(v1.Angle, v1.Angle))
    End Sub


    <TestMethod>
    Public Sub Vector2D_AngleInQ1Q1()
        Dim v1 = Vector2DHelper.Create(1, 1)
        Dim v2 = Vector2DHelper.Create(1, 0)

        Assert.AreEqual(-1, Vector2DHelper.CompareAngles(v1.Angle, v2.Angle))
        Assert.AreEqual(1, Vector2DHelper.CompareAngles(v2.Angle, v1.Angle))
    End Sub


    <TestMethod>
    Public Sub Vector2D_AngleInQ1Q2()
        Dim v1 = Vector2DHelper.Create(1, 1)
        Dim v2 = Vector2DHelper.Create(1, -1)

        Assert.AreEqual(-1, Vector2DHelper.CompareAngles(v1.Angle, v2.Angle))
        Assert.AreEqual(1, Vector2DHelper.CompareAngles(v2.Angle, v1.Angle))
    End Sub


    <TestMethod>
    Public Sub Vector2D_AngleInQ2Q3()
        Dim v1 = Vector2DHelper.Create(1, -1)
        Dim v2 = Vector2DHelper.Create(-1, -1)

        Assert.AreEqual(-1, Vector2DHelper.CompareAngles(v1.Angle, v2.Angle))
        Assert.AreEqual(1, Vector2DHelper.CompareAngles(v2.Angle, v1.Angle))
    End Sub


    <TestMethod>
    Public Sub Vector2D_AngleInQ3Q4()
        Dim v1 = Vector2DHelper.Create(-1, -1)
        Dim v2 = Vector2DHelper.Create(-1, 1)

        Assert.AreEqual(-1, Vector2DHelper.CompareAngles(v1.Angle, v2.Angle))
        Assert.AreEqual(1, Vector2DHelper.CompareAngles(v2.Angle, v1.Angle))
    End Sub


    <TestMethod>
    Public Sub Vector2D_AngleInQ4Q1()
        Dim v1 = Vector2DHelper.Create(-1, 1)
        Dim v2 = Vector2DHelper.Create(1, 1)

        Assert.AreEqual(-1, Vector2DHelper.CompareAngles(v1.Angle, v2.Angle))
        Assert.AreEqual(1, Vector2DHelper.CompareAngles(v2.Angle, v1.Angle))
    End Sub

#End Region


#Region " Line2D tests "

    <TestMethod>
    Public Sub Line2D_Colliding_Throws()
        Dim a = Point2DHelper.Create(1, 1)

        Dim ex = Assert.ThrowsException(Of ArgumentException)(
            Sub() Line2DHelper.Create(a, a))

        Assert.AreEqual("Cannot create a line from colliding points", ex.Message)
    End Sub


    <TestMethod>
    Public Sub Line2D_AlongXGetDistance_Ok()
        Dim a = Point2DHelper.Create(0, 1)
        Dim b = Point2DHelper.Create(4, 1)
        Dim l = Line2DHelper.Create(a, b)

        Assert.AreEqual(0.0, l.GetDistanceToLine(Point2DHelper.Create(0, 1)))
        Assert.AreEqual(0.0, l.GetDistanceToLine(Point2DHelper.Create(2, 1)))
        Assert.AreEqual(-1.0, l.GetDistanceToLine(Point2DHelper.Create(0, 2)))
        Assert.AreEqual(2.0, l.GetDistanceToLine(Point2DHelper.Create(3, -1)))
    End Sub


    <TestMethod>
    Public Sub Line2D_AlongYGetDistance_Ok()
        Dim a = Point2DHelper.Create(1, 0)
        Dim b = Point2DHelper.Create(1, 4)
        Dim l = Line2DHelper.Create(a, b)

        Assert.AreEqual(0.0, l.GetDistanceToLine(Point2DHelper.Create(1, 0)))
        Assert.AreEqual(0.0, l.GetDistanceToLine(Point2DHelper.Create(1, 6)))
        Assert.AreEqual(-1.0, l.GetDistanceToLine(Point2DHelper.Create(0, 2)))
        Assert.AreEqual(2.0, l.GetDistanceToLine(Point2DHelper.Create(3, 0)))
    End Sub


    <TestMethod>
    Public Sub Line2D_DiagonalGetDistance_Ok()
        Dim a = Point2DHelper.Create(1, 1)
        Dim b = Point2DHelper.Create(2, 2)
        Dim l = Line2DHelper.Create(a, b)

        Assert.AreEqual(0.0, l.GetDistanceToLine(Point2DHelper.Create(0, 0)))
        Assert.AreEqual(0.0, l.GetDistanceToLine(Point2DHelper.Create(0.5, 0.5)))
        Assert.AreEqual(0.0, l.GetDistanceToLine(Point2DHelper.Create(1, 1)))

        Assert.AreEqual(Math.Sqrt(0.5), l.GetDistanceToLine(Point2DHelper.Create(1, 0)), AbsoluteCoordPrecision)
        Assert.AreEqual(-Math.Sqrt(0.5), l.GetDistanceToLine(Point2DHelper.Create(0, 1)), AbsoluteCoordPrecision)
    End Sub


    <TestMethod>
    Public Sub Line2D_AlongXGetAlongLine_Ok()
        Dim a = Point2DHelper.Create(0, 1)
        Dim b = Point2DHelper.Create(4, 1)
        Dim l = Line2DHelper.Create(a, b)

        Assert.AreEqual(0.0, l.GetCoordinate(Point2DHelper.Create(0, 1)))
        Assert.AreEqual(1.0, l.GetCoordinate(Point2DHelper.Create(4, 1)))
        Assert.AreEqual(-0.5, l.GetCoordinate(Point2DHelper.Create(-2, 1)))
        Assert.AreEqual(0.5, l.GetCoordinate(Point2DHelper.Create(2, 1)))
        Assert.AreEqual(1.5, l.GetCoordinate(Point2DHelper.Create(6, 1)))
    End Sub


    <TestMethod>
    Public Sub Line2D_AlongYGetAlongLine_Ok()
        Dim a = Point2DHelper.Create(1, 0)
        Dim b = Point2DHelper.Create(1, 4)
        Dim l = Line2DHelper.Create(a, b)

        Assert.AreEqual(0.0, l.GetCoordinate(Point2DHelper.Create(1, 0)))
        Assert.AreEqual(1.0, l.GetCoordinate(Point2DHelper.Create(1, 4)))
        Assert.AreEqual(-0.5, l.GetCoordinate(Point2DHelper.Create(1, -2)))
        Assert.AreEqual(0.5, l.GetCoordinate(Point2DHelper.Create(1, 2)))
        Assert.AreEqual(1.5, l.GetCoordinate(Point2DHelper.Create(1, 6)))
    End Sub


    <TestMethod>
    Public Sub Line2D_DiagonalGetAlongLine_Ok()
        Dim a = Point2DHelper.Create(0, 1)
        Dim b = Point2DHelper.Create(4, 5)
        Dim l = Line2DHelper.Create(a, b)

        Assert.AreEqual(0.0, l.GetCoordinate(Point2DHelper.Create(0, 1)))
        Assert.AreEqual(1.0, l.GetCoordinate(Point2DHelper.Create(4, 5)))
        Assert.AreEqual(-0.5, l.GetCoordinate(Point2DHelper.Create(-2, -1)))
        Assert.AreEqual(0.5, l.GetCoordinate(Point2DHelper.Create(2, 3)))
        Assert.AreEqual(1.5, l.GetCoordinate(Point2DHelper.Create(6, 7)))
    End Sub

#End Region


#Region " LineSegment2D tests "

    <TestMethod>
    Public Sub LineSegment2D_Colliding_Throws()
        Dim a = Point2DHelper.Create(1, 1, {"A"})

        Dim ex = Assert.ThrowsException(Of ArgumentException)(
            Sub() LineSegment2DHelper.Create(a, a))

        Assert.AreEqual("Cannot create a line from colliding points", ex.Message)
    End Sub


    <TestMethod>
    Public Sub LineSegment2D_Positive_Ok()
        Dim a = Point2DHelper.Create(0, 1, {"A"})
        Dim b = Point2DHelper.Create(4, 1, {"B"})
        Dim l = LineSegment2DHelper.Create(a, b)

        Dim p = l.Line.GetPoint(0)
        Assert.AreEqual(a.X, p.X)
        Assert.AreEqual(a.Y, p.Y)
    End Sub

#End Region


#Region " Polygon2D tests "

    <TestMethod>
    Public Sub Polygon2D_OnePoints_Throws()
        Dim a = Point2DHelper.Create(1, 0, "A")
        Dim ex = Assert.ThrowsException(Of ArgumentException)(Sub() Polygon2DHelper.Create({a}))
        Assert.AreEqual("Polygon must have 3 or more vertices.", ex.Message)
    End Sub


    <TestMethod>
    Public Sub Polygon2D_TwoPoints_Throws()
        Dim a = Point2DHelper.Create(-1, 0, "A")
        Dim b = Point2DHelper.Create(1, 0, "A")
        Dim ex = Assert.ThrowsException(Of ArgumentException)(Sub() Polygon2DHelper.Create({a, b}))
        Assert.AreEqual("Polygon must have 3 or more vertices.", ex.Message)
    End Sub


    <TestMethod>
    Public Sub Polygon2D_ThreePointsAroundOrigin_Contains_Ok()
        Dim a = Point2DHelper.Create(0, 1, "A")
        Dim b = Point2DHelper.Create(1, -1, "B")
        Dim c = Point2DHelper.Create(-1, -1, "C")
        Dim p = Polygon2DHelper.Create({a, b, c})

        Assert.AreEqual(3, p.Sides.Count)

        Dim s0a = p.Sides(0).Line.GetPoint(0)
        Dim s0b = p.Sides(0).Line.GetPoint(1)
        Assert.AreEqual(a.X, s0a.X)
        Assert.AreEqual(a.Y, s0a.Y)
        Assert.AreEqual(b.X, s0b.X)
        Assert.AreEqual(b.Y, s0b.Y)

        Dim s1a = p.Sides(1).Line.GetPoint(0)
        Dim s1b = p.Sides(1).Line.GetPoint(1)
        Assert.AreEqual(b.X, s1a.X)
        Assert.AreEqual(b.Y, s1a.Y)
        Assert.AreEqual(c.X, s1b.X)
        Assert.AreEqual(c.Y, s1b.Y)

        Dim s2a = p.Sides(2).Line.GetPoint(0)
        Dim s2b = p.Sides(2).Line.GetPoint(1)
        Assert.AreEqual(c.X, s2a.X)
        Assert.AreEqual(c.Y, s2a.Y)
        Assert.AreEqual(a.X, s2b.X)
        Assert.AreEqual(a.Y, s2b.Y)

        Dim res = p.Contains(Origin)
        Assert.IsTrue(res)
    End Sub


    <TestMethod>
    Public Sub Polygon2D_ThreePointsTouchOrigin_NotContains_Ok()
        Dim a = Point2DHelper.Create(-1, 0, "A")
        Dim b = Point2DHelper.Create(1, 0, "B")
        Dim c = Point2DHelper.Create(0, 1, "C")
        Dim p = Polygon2DHelper.Create({a, b, c})

        Dim res = p.Contains(Origin)
        Assert.IsFalse(res)
    End Sub


    <TestMethod>
    Public Sub Polygon2D_ThreePoints_NotContains_Ok()
        Dim a = Point2DHelper.Create(-1, 0, "A")
        Dim b = Point2DHelper.Create(1, 0, "B")
        Dim c = Point2DHelper.Create(0, 1, "C")
        Dim p = Polygon2DHelper.Create({a, b, c})

        Assert.IsFalse(p.Contains(Point2DHelper.Create(0, -1)))
        Assert.IsFalse(p.Contains(Point2DHelper.Create(1, 1)))
        Assert.IsFalse(p.Contains(Point2DHelper.Create(-1, -1)))
    End Sub

#End Region

End Class
