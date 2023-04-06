Imports Common
Imports RoomDivisionLibrary


<TestClass>
<TestCategory("3D positioning")>
Public Class Room3DFlattenedLayoutTests

#Region " Init and clean-up "

    <ClassInitialize>
    Public Shared Sub InitClass(ctx As TestContext)
        SetupTestIo()
        InterfaceMapper.SetInstance(Of IVolumeConfiguration)(New TestConfig())
    End Sub

#End Region


#Region " Trivial layout tests "

    <TestMethod>
    Public Sub LayoutFlattened_Point_NoSpeakers_NoOutput_Ok()
        Dim t = LayoutTester.CreateFlattened({}, 1)
        t.Test({0.0, 0.0, 0.0}, {})
    End Sub


    <TestMethod>
    Public Sub LayoutFlattened_Point_OneSpeaker_AlwaysPlays_Ok()
        Dim t0 = LayoutTester.CreateFlattened({New Double() {0, 0, 0}}, 1)
        t0.Test({0.0, 0.0, 0.0}, {0})

        Dim t1 = LayoutTester.CreateFlattened({New Double() {0, 1, 0}}, 1)
        t1.Test({0.0, 0.0, 0.0}, {0})

        ' No sound outside the room
        t0.Test({-10.0, -10.0, -10.0}, {})
        t0.Test({10.0, 10.0, 10.0}, {})
    End Sub


    <TestMethod>
    Public Sub LayoutFlattened_Point_TwoSpeakersColocated_Ok()
        Dim t = LayoutTester.CreateFlattened(
            {New Double() {0, 0, 0}, New Double() {0, 0, 0}}, 1)

        t.Test({-1.0, 1, 1}, {0, 1})
        t.Test({0.0, 0, 0}, {0, 1})
        t.Test({0.0, 1, 1}, {0, 1})
    End Sub


    <TestMethod>
    Public Sub LayoutFlattened_Line_TwoSpeakersBySidesOfZeroAudience_Ok()
        Dim t = LayoutTester.CreateFlattened(
            {New Double() {-0.5, 0, 0}, New Double() {0.5, 0, 0}}, 3)

        t.Test({-1.0, 1, 1}, {0})
        t.Test({0.0, 0, 0}, {0, 1})
        t.Test({0.0, 1, 1}, {0, 1})
        t.Test({1.0, -1, -1}, {1})
    End Sub


    <TestMethod>
    Public Sub LayoutFlattened_Line_ThreeSpeakersBySidesOfZeroAudience_Ok()
        Dim t = LayoutTester.CreateFlattened({
            New Double() {-0.5, 0, 0},
            New Double() {0.5, 0, 0},
            New Double() {1.5, 0, 0}
        }, 4)

        t.Test({-1.0, 1, 1}, {0})
        t.Test({0.0, 0, 0}, {0, 1})
        t.Test({0.0, 1, 1}, {0, 1})
        t.Test({1.0, 0, 0}, {1, 2})
        t.Test({1.8, 0, 0}, {2})
    End Sub


    <TestMethod>
    Public Sub LayoutFlattened_Line_TwoSpeakersSameXy_Ok()
        ' One sound source is below the audience plane,
        ' and one is above it.
        ' Flattened layout doesn't care about Z axis, so it's as 1 point
        Dim t = LayoutTester.CreateFlattened(
            {New Double() {0.0, 0, -0.5}, New Double() {0.0, 0, 1}}, 1)

        ' No sound beyond the room
        t.Test({-3.0, 0, 0}, {})
        t.Test({3.0, 0, 0}, {})
        t.Test({0.0, -3, 0}, {})
        t.Test({0.0, 3, 0}, {})

        t.Test({0.0, 0, -3}, {0, 1})
        t.Test({0.0, 0, 3}, {0, 1})

        ' Inside the room
        t.Test({0.0, 0, -0.4}, {0, 1})
        t.Test({0.0, 0, 0.4}, {0, 1})
        t.Test({0.0, 0.0, 0.0}, {0, 1})
    End Sub


    <TestMethod>
    Public Sub LayoutFlattened_Line_TwoSpeakersAcrossAudience_Ok()
        Dim t = LayoutTester.CreateFlattened(
            {New Double() {-1.0, 0, -0.5}, New Double() {1.0, 0, 1}}, 3)

        ' No sound beyond the room
        t.Test({-3.0, 0, 0}, {})
        t.Test({3.0, 0, 0}, {})
        t.Test({0.0, -3, 0}, {})
        t.Test({0.0, 3, 0}, {})

        ' Flattened layout doesn't care about Z axis
        t.Test({0.0, 0, -3}, {0, 1})
        t.Test({0.0, 0, 3}, {0, 1})

        ' Inside
        t.Test({0.0, 0, -0.4}, {0, 1})
        t.Test({0.0, 0, 0.4}, {0, 1})
        t.Test({0.0, 0.0, 0.0}, {0, 1})
        t.Test({-1.5, 0, 0}, {0})
        t.Test({1.5, 0, 0}, {1})
    End Sub


    <TestMethod>
    Public Sub LayoutFlattened_Line_TwoSpeakersOnAudiencePlane_Ok()
        Dim t = LayoutTester.CreateFlattened(
            {New Double() {-0.5, 0.5, 0}, New Double() {0.5, 0.5, 0}}, 3)

        ' No sound beyond the room
        t.Test({-3.0, 0, 0}, {})
        t.Test({3.0, 0, 0}, {})
        t.Test({0.0, -3, 0}, {})
        t.Test({0.0, 3, 0}, {})

        ' Flattened layout doesn't care about Z axis
        t.Test({0.0, 0, -3}, {0, 1})
        t.Test({0.0, 0, 3}, {0, 1})

        ' Beyond left source
        t.Test({-1.0, 0, 0}, {0})
        t.Test({-1.0, 1, 1}, {0})

        ' Left source
        t.Test({-0.5, 0, 0}, {0})
        t.Test({-0.5, 0.5, 0}, {0})
        t.Test({-0.5, 1, 1}, {0})

        ' In between on the line
        t.Test({-0.4, 0, 0}, {0, 1})
        t.Test({-0.2, 0, 0}, {0, 1})
        t.Test({0.0, 0, 0}, {0, 1})
        t.Test({0.2, 0, 0}, {0, 1})
        t.Test({0.4, 0, 0}, {0, 1})

        ' In between not on the line
        t.Test({0.0, 1, 1}, {0, 1})
        t.Test({0.0, -1, -1}, {0, 1})
        t.Test({-0.4, 1, 1}, {0, 1})
        t.Test({0.4, -1, -1}, {0, 1})

        ' Right source
        t.Test({0.5, 0, 0}, {1})
        t.Test({0.5, 0.5, 0}, {1})
        t.Test({0.5, 1, 1}, {1})

        ' Beyond right source
        t.Test({1.0, 0, 0}, {1})
        t.Test({1.0, 1, 1}, {1})
    End Sub


    <TestMethod>
    Public Sub LayoutFlattened_Line_ThreeSpeakers_Ok()
        Dim t = LayoutTester.CreateFlattened({
            New Double() {-0.5, 0, 1}, New Double() {0.5, 0, 1},
            New Double() {0, 0, 0, 5}
        }, 4)

        t.Test({-1.0, 0, 0}, {0})
        t.Test({-1.0, 1, 1}, {0})

        t.Test({-0.5, 0, 0}, {0})
        t.Test({-0.5, 1, 1}, {0})

        t.Test({-0.2, 0, 0.5}, {0, 2})
        t.Test({-0.2, 1, 1}, {0, 2})

        t.Test({0.0, 0, 0}, {2})
        t.Test({0.0, 1, 1}, {2})

        t.Test({0.2, 0, 0.5}, {1, 2})
        t.Test({0.2, 1, 1}, {1, 2})

        t.Test({0.5, 0, 0}, {1})
        t.Test({0.5, 1, 1}, {1})

        t.Test({1.0, 0, 0}, {1})
        t.Test({1.0, 1, 1}, {1})
    End Sub

#End Region


#Region " Flat layout tests "

    <TestMethod>
    Public Sub LayoutFlattened_Plane_ThreeSpeakers_Ok()
        Dim t = LayoutTester.CreateFlattened(
            {New Double() {-1, 0, 0}, New Double() {0, 1, 0}, New Double() {1, 0, 0}},
            6)

        ' Inside the triangle
        t.Test({0, 0.5, 0.5}, {0, 1, 2})

        ' Vertices
        t.Test({-1, 0, 0}, {0})
        t.Test({0, 1, 0}, {1})
        t.Test({1, 0, 0}, {2})

        ' Edges
        t.Test({-0.5, 0.5, 0}, {0, 1})
        t.Test({0.5, 0.5, 0}, {1, 2})
        t.Test({0.0, 0.0, 0.0}, {0, 2})

        ' Outside the edges
        t.Test({-0.7, 0.7, 0}, {0, 1})
        t.Test({0.7, 0.7, 0}, {1, 2})
        t.Test({0, -0.5, 0}, {0, 2})
    End Sub


    <TestMethod>
    Public Sub LayoutFlattened_Plane_FourSpeakers_Ok()
        Dim t = LayoutTester.CreateFlattened({
            New Double() {-1, 1, 0}, New Double() {0, 2, 0},
            New Double() {1, 2, 0}, New Double() {0, 0, 0}}, 6)

        ' Points
        t.Test({-1, 1, 0}, {0})
        t.Test({0, 2, 0}, {1})
        t.Test({1, 2, 0}, {2})
        t.Test({0, 0, 0}, {3})

        ' Internal edges
        t.Test({-0.5, 1.5, 0}, {0, 1})
        t.Test({0.5, 2.0, 0}, {1, 2})
        t.Test({0.5, 1.0, 0}, {2, 3})
        t.Test({-0.5, 0.5, 0}, {0, 3})

        ' External edges
        t.Test({-1.5, 1.0, 0}, {0})
        t.Test({-1.5, 1.5, 0}, {0})

        ' Inside the core
        t.Test({-0.5, 1.0, 0}, {0, 1, 3})
        t.Test({0.5, 1.5, 0}, {1, 2, 3})

        ' Outside the core
        t.Test({-1.5, 1.2, 0}, {0})
        t.Test({-1.0, 1.5, 0}, {0, 1})
        t.Test({1, 0, 0}, {2, 3})
        t.Test({-1, 0, 0}, {0, 3})
    End Sub


    <TestMethod>
    Public Sub LayoutFlattened_Plane_FourSpeakersAroundZero_Ok()
        Dim t = LayoutTester.CreateFlattened({
            New Double() {-1, 0, 0}, New Double() {0, 1, 0},
            New Double() {1, 0, 0}, New Double() {0, -1, 0}}, 6)

        ' Points
        t.Test({-1, 0, 0}, {0})
        t.Test({0, 1, 0}, {1})
        t.Test({1, 0, 0}, {2})
        t.Test({0, -1, 0}, {3})

        ' Edges
        t.Test({-0.5, 0.5, 0}, {0, 1})
        t.Test({0.5, 0.5, 0}, {1, 2})
        t.Test({0.5, -0.5, 0}, {2, 3})
        t.Test({-0.5, -0.5, 0}, {0, 3})

        ' Inside - several solutions are valid
        t.Test({0.0, 0.0, 0.0}, {0, 2})
        t.Test({-0.2, 0.2, 0}, {0, 1, 2})
        t.Test({0.2, 0.2, 0}, {0, 1, 2})
        t.Test({0.2, -0.2, 0}, {0, 2, 3})
        t.Test({-0.2, -0.2, 0}, {0, 2, 3})
    End Sub


    <TestMethod>
    Public Sub LayoutFlattened_Space_FourSpeakers_Ok()
        Dim t = LayoutTester.CreateFlattened({
            New Double() {-1, 0, 0}, New Double() {0, 1, 0},
            New Double() {1, 0, 0}, New Double() {0, 0, 1}},
            8)

        ' Points
        t.Test({-1, 0, 0}, {0})
        t.Test({0, 1, 0}, {1})
        t.Test({1, 0, 0}, {2})
        t.Test({0, 0, 0}, {3})

        ' Internal edges
        t.Test({-0.5, 0.5, 0}, {0, 1})
        t.Test({0.5, 0.5, 0}, {1, 2})
        t.Test({0.5, 0, 0}, {2, 3})
        t.Test({-0.5, 0, 0}, {0, 3})

        ' External edges
        t.Test({-1.5, 0, 0}, {0})
        t.Test({0, 1.5, 0}, {1})
        t.Test({1.5, 0, 0}, {2})
        t.Test({0, -1.5, 0}, {3})

        ' Inside
        t.Test({-0.2, 0.2, 0}, {0, 1, 3})
        t.Test({0.2, 0.2, 0}, {1, 2, 3})

        ' Outside the core
        t.Test({1.5, 1.5, 0}, {1, 2})
        t.Test({1.5, -0.1, 0}, {2})
        t.Test({1.5, -1.5, 0}, {2, 3})
        t.Test({-1.5, -1.5, 0}, {0, 3})
        t.Test({-1.5, -0.1, 0}, {0})
        t.Test({-1.5, 1.5, 0}, {0, 1})
    End Sub

#End Region


#Region " Visualization tests "

    <TestMethod>
    Public Sub LayoutFlattened_Visualization_ColocatedPoints_Ok()
        Dim room As New Room3D With {
            .AudienceLeft = 0.2, .AudienceRight = 0.2, .AudienceFront = 0.2, .AudienceBack = 0.2
        }
        room.SetAllSides(2)

        Dim rs As New Room3DFlattenedLayouter(Of String)()

        Dim spk = {
            Point3DHelper.Create(0.0, 0.0, 0.0, {"A"}),
            Point3DHelper.Create(0.0, -0.0000000000000001388, -2.776E-17, {"B"}),
            Point3DHelper.Create(-0.0000000000000001388, -2.776E-17, 0.0, {"C"})
        }

        rs.PrepareLayout(room, spk)
        Dim visList = rs.GetPolyhedrons()

        Assert.IsNotNull(visList)
        Assert.AreEqual(1, visList.Count)

        ' Only the first speaker is on
        TestAssert.AreEqual({"A", "B", "C"}, rs.GetReferences(Point3DHelper.Create(1, 1, 1)))
    End Sub


    <TestMethod>
    <DataRow(0.9, 0.1, 0.2, -0.6, 0.1, -0.2, DisplayName:="Point 2 lies on plane 1 across audience")>
    <DataRow(0.9, 0.1, 0.2, -0.8, 0.0, -0.1, DisplayName:="One plane matches the audience")>
    Public Sub LayoutFlattened_Visualization_TwoSpecialPoints_Ok(
        x1 As Double, y1 As Double, z1 As Double,
        x2 As Double, y2 As Double, z2 As Double)

        Dim room As New Room3D With {
            .XLeft = 1, .XRight = 1, .YFront = 2, .YBack = 0.5, .ZAbove = 1, .ZBelow = 0.2,
            .AudienceLeft = 0.2, .AudienceRight = 0.2, .AudienceFront = 0.2, .AudienceBack = 0.2
        }

        Dim rs As New Room3DFlattenedLayouter(Of String)()

        Dim spk = {
            Point3DHelper.Create(x1, y1, z1, {"A"}),
            Point3DHelper.Create(x2, y2, z2, {"B"})
        }

        rs.PrepareLayout(room, spk)
        Dim visList = rs.GetPolyhedrons()

        Assert.IsNotNull(visList)
        Assert.IsTrue(visList.Count > 0)

        ' Test exact match
        TestAssert.AreEqual({"A"}, rs.GetReferences(spk(0)))

        If Not Point3DHelper.IsSame(spk(0), spk(1)) Then
            TestAssert.AreEqual({"B"}, rs.GetReferences(spk(1)))
        End If
    End Sub


    <TestMethod>
    Public Sub LayoutFlattened_Visualization_CutToRoomEdge_Ok()
        Dim room As New Room3D With {
            .XLeft = 2, .XRight = 2, .YFront = 2, .YBack = 2, .ZAbove = 2, .ZBelow = 2,
            .AudienceLeft = 0.2, .AudienceRight = 0.2, .AudienceFront = 0.2, .AudienceBack = 0.2
        }

        Dim n As New Vector3D(0, -1, 1)
        Dim spkProj = Point3DHelper.Create(0, -1, 1, {"A"})
        Dim dist = 0.0

        While dist < Math.Min(room.YBack, room.ZAbove)
            Dim o = n.Multiply(dist).From(Point3DHelper.Origin)

            Dim ph = PolyhedronHelper.CreateFromBorders(
                room.AllSideBorders(Of String).Concat({
                Plane3DHelper.CreateBorder(
                    Plane3DHelper.CreatePointNormal(o, n, {"Pa"}), 1, spkProj.References)
                }).ToList())
            Dim pList = ph.Sides

            Assert.AreEqual(5, pList.Count)

            Dim cuttingPoly = pList.Last()
            Assert.IsTrue(Vector3D.IsSame(n, cuttingPoly.Polygon.Plane.Normal))

            Dim origCut = cuttingPoly.Polygon.Sides
            Assert.AreEqual(4, origCut.Count)

            ' Check that Y and Z coordinates are not beyond the room
            Assert.AreEqual(room.ZAbove,
                origCut.SelectMany(Function(s) {s.PointA.Z, s.PointB.Z}).Max(),
                AbsoluteCoordPrecision)
            Assert.AreEqual(-room.YBack,
                origCut.SelectMany(Function(s) {s.PointA.Y, s.PointB.Y}).Min(),
                AbsoluteCoordPrecision)

            dist += 0.1
        End While
    End Sub


    <TestMethod>
    Public Sub LayoutFlattened_Visualization_CutToRoomVertice_Ok()
        Dim room As New Room3D With {
            .XLeft = 2, .XRight = 2, .YFront = 2, .YBack = 2, .ZAbove = 2, .ZBelow = 2,
            .AudienceLeft = 0.2, .AudienceRight = 0.2, .AudienceFront = 0.2, .AudienceBack = 0.2
        }

        Dim n As New Vector3D(1, -1, 1)
        Dim spkProj = Point3DHelper.Create(0, -1, 1, {"A"})
        Dim dist = 0.0

        While dist < 2
            Dim o = n.Multiply(dist).From(Point3DHelper.Origin)

            Dim ph = PolyhedronHelper.CreateFromBorders(
                room.AllSideBorders(Of String).Concat({
                Plane3DHelper.CreateBorder(
                    Plane3DHelper.CreatePointNormal(o, n, {"Pa"}), 1, spkProj.References)
                }).ToList())
            Dim pList = ph.Sides

            Dim polygons = If(dist < 0.7, 7, 4)
            Dim origSides = If(dist < 0.7, 6, 3)

            Assert.AreEqual(polygons, pList.Count, "At distance " & dist)

            Dim cuttingPoly = pList.Last()
            Assert.IsTrue(Vector3D.IsSame(n, cuttingPoly.Polygon.Plane.Normal))

            Dim origCut = cuttingPoly.Polygon.Sides
            Assert.AreEqual(origSides, origCut.Count, "At distance " & dist)

            ' Check that X, Y and Z coordinates are not beyond the room
            Assert.AreEqual(room.XRight,
                origCut.SelectMany(Function(s) {s.PointA.X, s.PointB.X}).Max(),
                AbsoluteCoordPrecision)
            Assert.AreEqual(-room.YBack,
                origCut.SelectMany(Function(s) {s.PointA.Y, s.PointB.Y}).Min(),
                AbsoluteCoordPrecision)
            Assert.AreEqual(room.ZAbove,
                origCut.SelectMany(Function(s) {s.PointA.Z, s.PointB.Z}).Max(),
                AbsoluteCoordPrecision)

            dist += 0.1
        End While
    End Sub


    <TestMethod>
    <DataRow(0, 0, 0)>
    <DataRow(0.9, 0.1, 0.2)>
    Public Sub LayoutFlattened_Visualization_TwoPointsCrashTest_Ok(
        x1 As Double, y1 As Double, z1 As Double
    )
        Dim room As New Room3D With {
            .XLeft = 1, .XRight = 1, .YFront = 2, .YBack = 0.5, .ZAbove = 1, .ZBelow = 0.2,
            .AudienceLeft = 0.2, .AudienceRight = 0.2, .AudienceFront = 0.2, .AudienceBack = 0.2
        }

        For x2 = -room.XLeft To room.XRight Step 0.1
            For y2 = -room.YBack To room.YFront Step 0.1
                For z2 = -room.ZBelow To room.ZAbove Step 0.1
                    Dim rs As I3DLayouter(Of String) = New Room3DFlattenedLayouter(Of String)()

                    Dim spk = {
                        Point3DHelper.Create(x1, y1, z1, {"A"}),
                        Point3DHelper.Create(x2, y2, z2, {"B"})
                    }

                    rs.PrepareLayout(room, spk)
                    Dim visList = rs.GetPolyhedrons()

                    Assert.IsNotNull(visList)
                    Assert.IsTrue(visList.Count > 0, $"x2 = {x2}, y2 = {y2}, z2 = {z2}")

                    ' Test exact match
                    If IsEqual(spk(0).X, spk(1).X) AndAlso IsEqual(spk(0).Y, spk(1).Y) Then
                        TestAssert.AreEqual({"A", "B"}, rs.GetReferences(spk(0)))
                    Else
                        TestAssert.AreEqual({"A"}, rs.GetReferences(spk(0)))
                        TestAssert.AreEqual({"B"}, rs.GetReferences(spk(1)))
                    End If
                Next
            Next
        Next
    End Sub


    <TestMethod>
    <DataRow(-1, 0, 0, 1, 0, 0)>
    Public Sub LayoutFlattened_Visualization_ThreePointsCrashTest_Ok(
        x1 As Double, y1 As Double, z1 As Double,
        x2 As Double, y2 As Double, z2 As Double
    )
        Dim room As New Room3D With {
            .XLeft = 1, .XRight = 1, .YFront = 2, .YBack = 0.5, .ZAbove = 1, .ZBelow = 0.2
        }
        room.SetAudienceSides(0.2)

        For x3 = -room.XLeft To room.XRight Step 0.1
            For y3 = -room.YBack To room.YFront Step 0.1
                For z3 = -room.ZBelow To room.ZAbove Step 0.1
                    Dim rs As I3DLayouter(Of String) = New Room3DFlattenedLayouter(Of String)()

                    Dim channelList As New List(Of Point3D(Of String)) From {
                        Point3DHelper.Create(x1, y1, z1, {"A"}),
                        Point3DHelper.Create(x2, y2, z2, {"B"}),
                        Point3DHelper.Create(x3, y3, z3, {"C"})
                    }

                    rs.PrepareLayout(room, channelList)
                    Dim visList = rs.GetPolyhedrons()

                    Assert.IsNotNull(visList)
                    Assert.IsTrue(visList.Count > 0, $"x2 = {x2}, y2 = {y2}, z2 = {z2}")
                Next
            Next
        Next
    End Sub


    <TestMethod>
    Public Sub Room3DFlat_CutByTwoCornerPoints()
        Dim room As New Room3D With {.XLeft = 4, .XRight = 4, .YBack = 3, .YFront = 3, .ZBelow = 2, .ZAbove = 2}
        Dim spk = {
            Point3DHelper.Create(-4, -3, -2, {"A"}),
            Point3DHelper.Create(4, 3, 2, {"B"})
        }

        Dim layout = New Room3DFlattenedLayouter(Of String)()
        layout.PrepareLayout(room, spk)

        Assert.AreEqual(1, layout.GetPolyhedrons().Count)

        TestAssert.AreEqual({"A", "B"}, layout.GetReferences(Point3DHelper.Origin))
        TestAssert.AreEqual({"A", "B"}, layout.GetReferences(Point3DHelper.Create(-3, 2, 1)))
        TestAssert.AreEqual({"A", "B"}, layout.GetReferences(Point3DHelper.Create(3, -2, -1)))
    End Sub


    <TestMethod>
    <DataRow(-1)>
    <DataRow(0)>
    <DataRow(1)>
    Public Sub Room3DFlat_CutByTwoPoints(y As Double)
        Dim room As New Room3D With {.XLeft = 4, .XRight = 4, .YBack = 3, .YFront = 3, .ZBelow = 2, .ZAbove = 2}
        Dim spk = {
            Point3DHelper.Create(-2, 0, 0, {"A"}),
            Point3DHelper.Create(2, 0, 0, {"B"})
        }

        Dim layout = New Room3DFlattenedLayouter(Of String)()
        layout.PrepareLayout(room, spk)

        Assert.AreEqual(3, layout.GetPolyhedrons().Count)

        Assert.AreEqual(0, layout.GetReferences(Point3DHelper.Create(-5, y, 0)).Count)
        TestAssert.AreEqual({"A"}, layout.GetReferences(Point3DHelper.Create(-3, y, 0)))
        TestAssert.AreEqual({"A"}, layout.GetReferences(Point3DHelper.Create(-2, y, 0)))
        TestAssert.AreEqual({"A", "B"}, layout.GetReferences(Point3DHelper.Create(0, y, 0)))
        TestAssert.AreEqual({"B"}, layout.GetReferences(Point3DHelper.Create(2, y, 0)))
        TestAssert.AreEqual({"B"}, layout.GetReferences(Point3DHelper.Create(3, y, 0)))
        Assert.AreEqual(0, layout.GetReferences(Point3DHelper.Create(5, y, 0)).Count)
    End Sub


    <TestMethod>
    <DataRow(-1, 0, 0, 1, 0, 0)>
    Public Sub Room3DFlat_ThreePointsCrashTest_Ok(
        x1 As Double, y1 As Double, z1 As Double,
        x2 As Double, y2 As Double, z2 As Double
    )
        Dim room As New Room3D With {
            .XLeft = 1, .XRight = 1, .YFront = 2, .YBack = 0.5, .ZAbove = 1, .ZBelow = 0.2,
            .AudienceLeft = 0.2, .AudienceRight = 0.2, .AudienceFront = 0.2, .AudienceBack = 0.2
        }

        For x3 = -room.XLeft To room.XRight Step 0.1
            For y3 = -room.YBack To room.YFront Step 0.1
                For z3 = -room.ZBelow To room.ZAbove Step 0.1
                    Dim rs As I3DLayouter(Of String) = New Room3DFlattenedLayouter(Of String)()

                    Dim channelList As New List(Of Point3D(Of String)) From {
                        Point3DHelper.Create(x1, y1, z1, {"A"}),
                        Point3DHelper.Create(x2, y2, z2, {"B"}),
                        Point3DHelper.Create(x3, y3, z3, {"C"})
                    }

                    rs.PrepareLayout(room, channelList)
                    Dim visList = rs.GetPolyhedrons()

                    Assert.IsNotNull(visList)
                    Assert.IsTrue(visList.Count > 0)
                Next
            Next
        Next
    End Sub

#End Region

End Class
