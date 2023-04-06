Imports Common


<TestClass>
<TestCategory("3D geometry")>
Public Class PolyhedronTests

    <TestMethod>
    Public Sub Polyhedron_Room()
        Dim room As New Room3D()
        Dim roomPlanes = room.AllSideBorders(Of Integer)()
        Assert.AreEqual(6, roomPlanes.Count)

        Dim ph = PolyhedronHelper.CreateFromBorders(roomPlanes)

        Dim psides = ph.Sides
        Assert.AreEqual(6, psides.Count)
        For Each side In psides
            Dim contains = roomPlanes.Single(
                Function(p) Plane3DHelper.IsSame(p.Plane, side.Polygon.Plane))
            Assert.IsNotNull(contains)
        Next
    End Sub


    <TestMethod>
    Public Sub Polyhedron_Tube()
        Dim pl1 = Plane3DHelper.CreatePointNormal(New TestPoint3D(-1, -0.5, 0), New Vector3D(1, 1, 1), {"Pa"})
        Dim pl2 = Plane3DHelper.CreatePointNormal(New TestPoint3D(1, 1, 1), New Vector3D(-1, -1, -1), {"Pb"})
        Dim pl3 = Plane3DHelper.CreatePointNormal(New TestPoint3D(-1, 1, 0), New Vector3D(1, -1, 0), {"Pc"})
        Dim pl4 = Plane3DHelper.CreatePointNormal(New TestPoint3D(1, -0.5, 0), New Vector3D(-1, 1, 0), {"Pd"})

        Dim ph = PolyhedronHelper.CreateFromBorders(
            {
                Plane3DHelper.CreateBorder(pl1, 1, {1}),
                Plane3DHelper.CreateBorder(pl2, 1, {2}),
                Plane3DHelper.CreateBorder(pl3, 1, {3}),
                Plane3DHelper.CreateBorder(pl4, 1, {4})
            })

        Assert.AreEqual(4, ph.Sides.Count)
        Assert.AreEqual(4, ph.References.Count)

        Assert.IsTrue(ph.Contains(New TestPoint3D(0, 0, 0)))
        Assert.IsFalse(ph.Contains(New TestPoint3D(0, 0, -2)))
        Assert.IsFalse(ph.Contains(New TestPoint3D(0, 0, 4)))
        Assert.IsFalse(ph.Contains(New TestPoint3D(-1, -1, 0)))
        Assert.IsFalse(ph.Contains(New TestPoint3D(2, 2, 0)))
    End Sub


    <TestMethod>
    Public Sub Polyhedron_RoomCutByPlaneToCorner()
        Dim room As New Room3D()
        room.SetAllSides(2)
        Dim roomPlanes = room.AllSideBorders(Of String)()
        Assert.AreEqual(6, roomPlanes.Count)

        Dim borders = roomPlanes.Concat({
            Plane3DHelper.CreateBorder(
                Plane3DHelper.CreatePointNormal(
                    Point3DHelper.Create(1.5, 1.5, 1.5, {"A"}),
                    New Vector3D(1, 1, 1),
                    {"A"}), 1)}).
            ToList()

        Dim ph = PolyhedronHelper.CreateFromBorders(borders)

        Assert.AreEqual(4, ph.Sides.Count)
        Assert.AreEqual(3, ph.Sides(0).Polygon.Sides.Count)
        Assert.AreEqual(3, ph.Sides(1).Polygon.Sides.Count)
        Assert.AreEqual(3, ph.Sides(2).Polygon.Sides.Count)
        Assert.AreEqual(3, ph.Sides(3).Polygon.Sides.Count)
        Assert.AreEqual(1, ph.References.Count)
    End Sub


    <TestMethod>
    Public Sub Polyhedron_RoomCutByPlaneFromCorner()
        Dim room As New Room3D()
        room.SetAllSides(2)
        Dim roomPlanes = room.AllSideBorders(Of String)()
        Assert.AreEqual(6, roomPlanes.Count)

        Dim borders = roomPlanes.Concat({
            Plane3DHelper.CreateBorder(
                Plane3DHelper.CreatePointNormal(
                    Point3DHelper.Create(1.5, 1.5, 1.5, {"A"}),
                    New Vector3D(1, 1, 1),
                    {"A"}), -1)}).
            ToList()

        Dim ph = PolyhedronHelper.CreateFromBorders(borders)

        Assert.AreEqual(7, ph.Sides.Count)
        Assert.AreEqual(1, ph.References.Count)
    End Sub

End Class
