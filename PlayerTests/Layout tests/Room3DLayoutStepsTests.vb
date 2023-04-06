Imports Common
Imports MIConvexHull
Imports RoomDivisionLibrary


<TestClass>
<TestCategory("3D positioning")>
Public Class Room3DLayoutStepsTests

#Region " MIConvexHull sanity tests "

    <TestMethod>
    <ExpectedException(GetType(ConvexHullGenerationException))>
    Public Sub MiConvexHull_Triang_Four2dPoints_WasTooFew_Ok()
        Dim points = {
            New TestVertex2D("A", 0, 0),
            New TestVertex2D("B", 0, -2),
            New TestVertex2D("C", -0.71, -1.5),
            New TestVertex2D("D", -0.71, -0.5)
        }

        ' Currently this generates an exception due to a bug in MIConvexHull
        ' See https://github.com/DesignEngrLab/MIConvexHull/issues/30
        Dim div = Triangulation.CreateDelaunay(points)

        Assert.AreEqual(2, div.Cells.Count)
    End Sub


    <TestMethod>
    <ExpectedException(GetType(NullReferenceException))>
    Public Sub MiConvexHull_Triang_Four2dPoints_AroundZero_Ok()
        Dim points = {
            New TestVertex2D("A", -1, -1),
            New TestVertex2D("B", -1, 1),
            New TestVertex2D("C", 1, -1),
            New TestVertex2D("D", 1, 1)
        }

        ' Currently this generates an exception due to a bug in MIConvexHull
        ' See https://github.com/DesignEngrLab/MIConvexHull/issues/30
        Dim div = Triangulation.CreateDelaunay(points)

        Assert.AreEqual(2, div.Cells.Count)
    End Sub


    <TestMethod>
    <ExpectedException(GetType(ConvexHullGenerationException))>
    Public Sub MiConvexHull_Triang_Four2dPoints_TwoCells_Ok()
        Dim points = {
            New TestVertex2D("A", 0, 0),
            New TestVertex2D("B", 0, -2),
            New TestVertex2D("C", -0.70710678118654746, -1.5),
            New TestVertex2D("D", -0.70710678118654746, -0.5)
        }

        ' Currently this generates an exception due to a bug in MIConvexHull
        ' See https://github.com/DesignEngrLab/MIConvexHull/issues/30
        Dim div = Triangulation.CreateDelaunay(points)

        Assert.AreEqual(2, div.Cells.Count)
    End Sub


    <TestMethod>
    Public Sub MiConvexHull_Triang_Four3dPoints_CannotDelaunay_Throws()
        Dim points = {
            New TestVertex3D("A", -0.9, 0.7, 0),
            New TestVertex3D("B", 0.9, 0.6, 0.1),
            New TestVertex3D("C", 0.5, 0.3, 0.5),
            New TestVertex3D("D", 0, 0.9, 0.7)
        }

        Dim ex = Assert.ThrowsException(Of ConvexHullGenerationException)(
            Sub() Triangulation.CreateDelaunay(points))
        Assert.AreEqual("Exception of type 'MIConvexHull.ConvexHullGenerationException' was thrown.", ex.Message)
    End Sub


    <TestMethod>
    Public Sub MiConvexHull_Hull_Four3dPoints_Ok()
        Dim points = {
            New TestVertex3D("A", -0.9, 0.7, 0),
            New TestVertex3D("B", 0.9, 0.6, 0.1),
            New TestVertex3D("C", 0.5, 0.3, 0.5),
            New TestVertex3D("D", 0, 0.9, 0.7)
        }

        Dim hull = ConvexHull.Create(points, AbsoluteCoordPrecision)
        Assert.AreEqual(4, hull.Result.Faces.Count)
    End Sub


    <TestMethod>
    Public Sub MiConvexHull_Triang_Five3dPoints_TwoCells_Ok()
        Dim points = {
            New TestVertex3D("A", -0.9, 0.7, 0),
            New TestVertex3D("B", 0.9, 0.6, 0.1),
            New TestVertex3D("C", 0.5, 0.3, 0.5),
            New TestVertex3D("D", 0, 0.9, 0.7),
            New TestVertex3D("E", -0.5, 0.3, 0.2)
        }

        Dim div = Triangulation.CreateDelaunay(points)
        Assert.AreEqual(2, div.Cells.Count)

        Dim hull = ConvexHull.Create(points, AbsoluteCoordPrecision)
        Assert.AreEqual(6, hull.Result.Faces.Count)
    End Sub

#End Region


#Region " Layout steps tests: single point "

    <TestMethod>
    Public Sub Layouter3D_CreateSingle_NoSpeaker_Ok()
        Dim room As New Room3D()
        room.SetAllSides(2)
        Dim spk = Point3DHelper.Create(1, 1, 0, {"A"})

        Dim res = PolyhedronFromSingleCreator.Create({spk}, room)

        ' 1 polyhedron in the core, no shell
        Assert.IsNotNull(res)
        Assert.AreEqual(1, res.Core.Count)
        Assert.AreEqual(0, res.Shell.Count)

        ' All-filling polyhedron
        TestAssert.AreEqual({"A"}, res.Core.FindRefForPoint(Point3DHelper.Origin))

        ' Beyond the room - none
        Assert.AreEqual(0, res.Core.FindRefForPoint(Point3DHelper.Create(2.1, 0, 0)).Count)
        Assert.AreEqual(0, res.Core.FindRefForPoint(Point3DHelper.Create(-2.1, 0, 0)).Count)
        Assert.AreEqual(0, res.Core.FindRefForPoint(Point3DHelper.Create(0, 2.1, 0)).Count)
        Assert.AreEqual(0, res.Core.FindRefForPoint(Point3DHelper.Create(0, -2.1, 0)).Count)
        Assert.AreEqual(0, res.Core.FindRefForPoint(Point3DHelper.Create(0, 0, 2.1)).Count)
        Assert.AreEqual(0, res.Core.FindRefForPoint(Point3DHelper.Create(0, 0, -2.1)).Count)
    End Sub


    <TestMethod>
    Public Sub Layouter3D_CreateSingle_OneSpeaker_Ok()
        Dim room As New Room3D()
        room.SetAllSides(2)

        Dim spk = Point3DHelper.Create(1, 1, 0, {"A"})
        Dim res = PolyhedronFromSingleCreator.Create({spk}, room)

        ' 1 polyhedron in the core, no shell
        Assert.IsNotNull(res)
        Assert.AreEqual(1, res.Core.Count)
        Assert.AreEqual(0, res.Shell.Count)

        ' Direct match
        Dim refList = res.Core.FindRefForPoint(spk)
        TestAssert.AreEqual({"A"}, refList)

        ' The speaker is returned as a result of polyhedron match
        refList = res.Core.FindRefForPoint(Point3DHelper.Origin)
        TestAssert.AreEqual({"A"}, refList)
    End Sub

#End Region


#Region " Layout steps tests: linear colocation "

    <TestMethod>
    <DataRow(0.1, 0.5, DisplayName:="Y = 0.1, Z = 0.5")>
    <DataRow(1.0, 0.5, DisplayName:="Y = 1.0, Z = 0.5")>
    <DataRow(0.5, 1.0, DisplayName:="Y = 0.5, Z = 1.0")>
    Public Sub Layouter3D_CreateLinear_TwoInFront_Ok(y As Double, z As Double)
        Dim room As New Room3D()
        room.SetAudienceSides(0)
        room.SetAllSides(2)

        Dim spk = {
            Point3DHelper.Create(-1, y, z, {"A"}),
            Point3DHelper.Create(1, y, z, {"B"})
        }

        Dim projList As IList(Of PointAsVertex1D(Of String)) = Nothing
        Assert.IsTrue(PolyhedronFromLinearCreator.CalcProjectionLine(spk, projList))
        Dim res = PolyhedronFromLinearCreator.Create(projList, room)

        ' Separation completed - no shell
        Assert.AreEqual(3, res.Core.Count)
        Assert.AreEqual(0, res.Shell.Count)

        ' Exact match; for it to work CollectAllSpeakers needs to be called
        res.Core.CollectAllVertices(spk)
        TestAssert.AreEqual({"A"}, res.Core.FindRefForPoint(spk(0)))
        TestAssert.AreEqual({"B"}, res.Core.FindRefForPoint(spk(1)))

        ' In-between - two speakers
        TestAssert.AreEqual({"A", "B"}, res.Core.FindRefForPoint(Point3DHelper.Create(0, y, z)))

        ' Beyond the speakers
        TestAssert.AreEqual({"A"}, res.Core.FindRefForPoint(Point3DHelper.Create(-1.1, y, z)))
        TestAssert.AreEqual({"B"}, res.Core.FindRefForPoint(Point3DHelper.Create(1.1, y, z)))

        ' Beyond the room - no speakers
        Assert.AreEqual(0, res.Core.FindRefForPoint(Point3DHelper.Create(-2.1, 0, 0)).Count())
        Assert.AreEqual(0, res.Core.FindRefForPoint(Point3DHelper.Create(2.1, 0, 0)).Count())
        Assert.AreEqual(0, res.Core.FindRefForPoint(Point3DHelper.Create(0, 0, 2.1)).Count())
    End Sub


    <TestMethod>
    Public Sub Layouter3D_CreateLinear_TwoAsideBeyondAudience_Ok()
        Dim room As New Room3D()
        room.SetAudienceSides(9)
        room.SetAllSides(2)

        Dim spk = {
            Point3DHelper.Create(-1, 0, 0, {"A"}),
            Point3DHelper.Create(1, 0, 0, {"B"})
        }

        Dim projList As IList(Of PointAsVertex1D(Of String)) = Nothing
        Assert.IsTrue(PolyhedronFromLinearCreator.CalcProjectionLine(spk, projList))
        Dim res = PolyhedronFromLinearCreator.Create(projList, room)

        ' Separation completed - no shell
        Assert.AreEqual(3, res.Core.Count)
        Assert.AreEqual(6, res.Core(0).Sides.Count)
        Assert.AreEqual(6, res.Core(1).Sides.Count)
        Assert.AreEqual(6, res.Core(2).Sides.Count)
        Assert.AreEqual(0, res.Shell.Count)

        ' Exact match; for it to work CollectAllSpeakers needs to be called
        res.Core.CollectAllVertices(spk)
        Assert.AreEqual(2, res.Core.Vertices.Count)
        TestAssert.AreEqual({"A"}, res.Core.FindRefForPoint(spk(0)))
        TestAssert.AreEqual({"B"}, res.Core.FindRefForPoint(spk(1)))

        ' In-between - two speakers
        TestAssert.AreEqual({"A", "B"}, res.Core.FindRefForPoint(Point3DHelper.Create(0, 0, 0)))
        TestAssert.AreEqual({"A", "B"}, res.Core.FindRefForPoint(Point3DHelper.Create(0, -1, -1)))

        ' Beyond the speakers
        TestAssert.AreEqual({"A"}, res.Core.FindRefForPoint(Point3DHelper.Create(-1.1, 0, 0)))
        TestAssert.AreEqual({"B"}, res.Core.FindRefForPoint(Point3DHelper.Create(1.1, 0, 0)))

        ' Beyond the room - no speakers
        Assert.AreEqual(0, res.Core.FindRefForPoint(Point3DHelper.Create(-2.1, 0, 0)).Count())
        Assert.AreEqual(0, res.Core.FindRefForPoint(Point3DHelper.Create(2.1, 0, 0)).Count())
        Assert.AreEqual(0, res.Core.FindRefForPoint(Point3DHelper.Create(0, 0, 2.1)).Count())
    End Sub


    <TestMethod>
    <DataRow(0.2, 0.2, DisplayName:="Y = 0.2, Z = 0.2")>
    <DataRow(1.0, 0.2, DisplayName:="Y = 1.0, Z = 0.2")>
    Public Sub Layouter3D_CreateLinear_ThreeInFront_Ok(y As Double, z As double)
        Dim room As New Room3D()
        room.SetAudienceSides(0)
        room.SetAllSides(2)

        Dim spk = {
            Point3DHelper.Create(-0.9, y, z, {"A"}),
            Point3DHelper.Create(0, y, z, {"B"}),
            Point3DHelper.Create(0.9, y, z, {"C"})
        }

        Dim projList As IList(Of PointAsVertex1D(Of String)) = Nothing
        Assert.IsTrue(PolyhedronFromLinearCreator.CalcProjectionLine(spk, projList))
        Dim res = PolyhedronFromLinearCreator.Create(projList, room)

        Assert.AreEqual(4, res.Core.Count)
        Assert.AreEqual(0, res.Shell.Count)
        res.Core.CollectAllVertices(spk)

        ' Exact match
        TestAssert.AreEqual({"A"}, res.Core.FindRefForPoint(spk(0)))
        TestAssert.AreEqual({"B"}, res.Core.FindRefForPoint(spk(1)))
        TestAssert.AreEqual({"C"}, res.Core.FindRefForPoint(spk(2)))

        ' In-between - two speakers
        TestAssert.AreEqual({"A", "B"}, res.Core.FindRefForPoint(Point3DHelper.Create(-0.5, y, z)))
        TestAssert.AreEqual({"B", "C"}, res.Core.FindRefForPoint(Point3DHelper.Create(0.5, y, z)))

        ' Beyond the speakers
        TestAssert.AreEqual({"A"}, res.Core.FindRefForPoint(Point3DHelper.Create(-1, y, z)))
        TestAssert.AreEqual({"C"}, res.Core.FindRefForPoint(Point3DHelper.Create(1, y, z)))

        ' Below the audience - none
        Assert.AreEqual(0, res.Core.FindRefForPoint(Point3DHelper.Create(0, 0, -0.1)).Count())

        ' Beyond the room - none
        Assert.AreEqual(0, res.Core.FindRefForPoint(Point3DHelper.Create(-2.1, 0, 0)).Count())
        Assert.AreEqual(0, res.Core.FindRefForPoint(Point3DHelper.Create(2.1, 0, 0)).Count())
        Assert.AreEqual(0, res.Core.FindRefForPoint(Point3DHelper.Create(0, -2.1, 0)).Count())
        Assert.AreEqual(0, res.Core.FindRefForPoint(Point3DHelper.Create(0, 2.1, 0)).Count())
        Assert.AreEqual(0, res.Core.FindRefForPoint(Point3DHelper.Create(0, 0, -2.1)).Count())
        Assert.AreEqual(0, res.Core.FindRefForPoint(Point3DHelper.Create(0, 0, 2.1)).Count())
    End Sub

#End Region


#Region " Layout steps tests: planar colocation "

    <TestMethod>
    <DataRow(0.0, DisplayName:="Zero audience")>
    <DataRow(0.2, DisplayName:="Non-zero audience 0.2")>
    <DataRow(1.0, DisplayName:="Non-zero audience 1.0")>
    Public Sub Layouter3D_CreatePlanar_ThreeInFront_Ok(audSize As Double)
        Dim room As New Room3D With {
            .XLeft = 2, .XRight = 2, .YBack = 2, .YFront = 2, .ZBelow = 2, .ZAbove = 2,
            .AudienceLeft = audSize, .AudienceRight = audSize, .AudienceFront = audSize, .AudienceBack = audSize
        }

        Dim spk = {
            Point3DHelper.Create(-1, 0.5, 0, {"A"}),
            Point3DHelper.Create(0, 0.5, 1, {"B"}),
            Point3DHelper.Create(1, 0.5, 0, {"C"})
        }

        Dim projList As IList(Of PointAsVertex2D(Of String)) = Nothing
        Assert.IsTrue(PolyhedronFromPlanarCreator.CalcProjectionPlane(spk, projList))
        Dim res = PolyhedronFromPlanarCreator.Create(projList, room)

        ' 3 points => 1 face, 2 in the Shell
        Assert.AreEqual(0, res.Core.Count)
        Assert.AreEqual(2, res.Shell.Count)
    End Sub


    <TestMethod>
    <DataRow(0.0, DisplayName:="Zero audience")>
    <DataRow(0.2, DisplayName:="Non-zero audience 0.2")>
    <DataRow(1.0, DisplayName:="Non-zero audience 1.0")>
    Public Sub Layouter3D_CreatePlanar_FourInFront_Ok(audSize As Double)
        Dim room As New Room3D With {
            .XLeft = 2, .XRight = 2, .YBack = 2, .YFront = 2, .ZBelow = 2, .ZAbove = 2,
            .AudienceLeft = audSize, .AudienceRight = audSize, .AudienceFront = audSize, .AudienceBack = audSize
        }

        ' Do not center at (0,0), see https://github.com/DesignEngrLab/MIConvexHull/issues/30
        Dim spk = {
            Point3DHelper.Create(-1.1, 1, 0.5, {"A"}),
            Point3DHelper.Create(1, 1, 0.5, {"B"}),
            Point3DHelper.Create(0.5, 0.5, 1, {"C"}),
            Point3DHelper.Create(-0.5, 0.5, 1, {"D"})
        }

        Dim projList As IList(Of PointAsVertex2D(Of String)) = Nothing
        Assert.IsTrue(PolyhedronFromPlanarCreator.CalcProjectionPlane(spk, projList))
        Dim res = PolyhedronFromPlanarCreator.Create(projList, room)

        ' 4 points => 2 faces, 4 in the Shell
        Assert.AreEqual(0, res.Core.Count)
        Assert.AreEqual(4, res.Shell.Count)
    End Sub


    <TestMethod>
    <DataRow(0.0, DisplayName:="Zero audience")>
    <DataRow(0.2, DisplayName:="Non-zero audience 0.2")>
    <DataRow(1.0, DisplayName:="Non-zero audience 1.0")>
    Public Sub Layouter3D_CreatePlanar_FourAround_Ok(audSize As Double)
        Dim room As New Room3D With {
            .XLeft = 2, .XRight = 2, .YBack = 2, .YFront = 2, .ZBelow = 2, .ZAbove = 2,
            .AudienceLeft = audSize, .AudienceRight = audSize, .AudienceFront = audSize, .AudienceBack = audSize
        }

        ' Do not center at (0,0), see https://github.com/DesignEngrLab/MIConvexHull/issues/30
        Dim spk = {
            Point3DHelper.Create(-1.1, 1, 0, {"A"}),
            Point3DHelper.Create(1, 1, 0, {"B"}),
            Point3DHelper.Create(-1, -0.5, 0, {"C"}),
            Point3DHelper.Create(1, -0.5, 0, {"D"})
        }

        Dim projList As IList(Of PointAsVertex2D(Of String)) = Nothing
        Assert.IsTrue(PolyhedronFromPlanarCreator.CalcProjectionPlane(spk, projList))
        Dim res = PolyhedronFromPlanarCreator.Create(projList, room)

        ' 4 points => 2 faces, 4 in the Shell
        Assert.AreEqual(0, res.Core.Count)
        Assert.AreEqual(4, res.Shell.Count)
    End Sub

#End Region


#Region " Layout steps tests: 3D "

    <TestMethod>
    Public Sub Layouter3D_Create3d_ZeroAudience_FourInFront_Ok()
        Dim room As New Room3D With {
            .XLeft = 1, .XRight = 1, .YFront = 1, .YBack = 0.5, .ZAbove = 1, .ZBelow = 0.2
        }

        Dim spk = {
            Point3DHelper.Create(0, 0.5, 0, {"A"}),
            Point3DHelper.Create(-1, 1, 0.5, {"B"}),
            Point3DHelper.Create(1, 1, 0.5, {"C"}),
            Point3DHelper.Create(0, 1, 1, {"D"})
        }

        Dim res = PolyhedronFrom3DCreator.Create(spk, room)

        ' 4 points => 1 tetrahedron in core, 4 triangles in the shell
        Assert.IsNotNull(res)
        Assert.AreEqual(1, res.Core.Count)
        Assert.AreEqual(4, res.Shell.Count)
    End Sub


    <TestMethod>
    Public Sub Layouter3D_Create3d_ZeroAudience_FiveInFront_Ok()
        Dim room As New Room3D With {
            .XLeft = 1, .XRight = 1, .YFront = 1, .YBack = 0.5, .ZAbove = 1, .ZBelow = 0.2
        }

        ' Do not center at (0,0), see https://github.com/DesignEngrLab/MIConvexHull/issues/30
        Dim spk = {
            Point3DHelper.Create(-1.1, 1, 0, {"A"}),
            Point3DHelper.Create(1, 1, 0, {"B"}),
            Point3DHelper.Create(-0.5, 1, 0.5, {"C"}),
            Point3DHelper.Create(0.5, 1, 0.5, {"D"}),
            Point3DHelper.Create(0, 0, 1, {"E"})
        }

        Dim res = PolyhedronFrom3DCreator.Create(spk, room)

        ' 5 points => 2 tetrahedrons
        Assert.IsNotNull(res)
        Assert.AreEqual(2, res.Core.Count)
        Assert.AreEqual(6, res.Shell.Count)
    End Sub

#End Region


#Region " Layout step tests: shell "

#End Region

End Class
