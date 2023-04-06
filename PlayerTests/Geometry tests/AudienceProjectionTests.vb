Imports Common


<TestClass>
Public Class AudienceProjectionTests

#Region " Utility "

    Private Shared Sub TestCubeProjection(
        room As Room3D,
        x As Double, y As Double, z As Double,
        audX As Double, audY As Double,
        projX As Double, projY As Double, projZ As Double
    )
        Dim p = room.ProjectPoint(New TestPoint3D(x, y, z), ProjectionModes.Cube)

        Assert.AreEqual(audX, p.PointA.X, AbsoluteCoordPrecision, "Audience projection X")
        Assert.AreEqual(audY, p.PointA.Y, AbsoluteCoordPrecision, "Audience projection Y")
        Assert.AreEqual(0.0, p.PointA.Z, AbsoluteCoordPrecision, "Audience projection Z")

        Assert.AreEqual(projX, p.PointB.X, AbsoluteCoordPrecision, "Room projection X")
        Assert.AreEqual(projY, p.PointB.Y, AbsoluteCoordPrecision, "Room projection Y")
        Assert.AreEqual(projZ, p.PointB.Z, AbsoluteCoordPrecision, "Room projection Z")
    End Sub


    Private Shared Sub TestSphereProjection(
        room As Room3D,
        x As Double, y As Double, z As Double,
        audX As Double, audY As Double,
        projX As Double, projY As Double, projZ As Double
    )
        Dim p = room.ProjectPoint(New TestPoint3D(x, y, z), ProjectionModes.Sphere)

        Assert.AreEqual(audX, p.PointA.X, AbsoluteCoordPrecision, "Audience projection X")
        Assert.AreEqual(audY, p.PointA.Y, AbsoluteCoordPrecision, "Audience projection Y")
        Assert.AreEqual(0.0, p.PointA.Z, AbsoluteCoordPrecision, "Audience projection Z")

        Assert.AreEqual(projX, p.PointB.X, AbsoluteCoordPrecision, "Room projection X")
        Assert.AreEqual(projY, p.PointB.Y, AbsoluteCoordPrecision, "Room projection Y")
        Assert.AreEqual(projZ, p.PointB.Z, AbsoluteCoordPrecision, "Room projection Z")
    End Sub


    Private Shared Sub TestConvertedCoord(room As Room3D, x As Single, y As Single, z As Single, ax As Double, ay As Double, az As Double)
        Dim p As New PositionRelative(x, y, z)
        Dim c = room.ConvertRelative(p)
        Assert.AreEqual(ax, c.X, AbsoluteCoordPrecision, "X coordinate")
        Assert.AreEqual(ay, c.Y, AbsoluteCoordPrecision, "Y coordinate")
        Assert.AreEqual(az, c.Z, AbsoluteCoordPrecision, "Z coordinate")
    End Sub

#End Region


#Region " RoomDefinition.ProjectToAudience tests "

    <TestMethod>
    <DataRow(0, 0, 0, 0, 0)>
    <DataRow(1, 0, 0, 0, 0)>
    <DataRow(0, 1, 0, 0, 0)>
    <DataRow(0, 0, 1, 0, 0)>
    <DataRow(-1, -1, -1, 0, 0)>
    Public Sub RoomDefinition_ProjectToAudience_ZeroAudience_Ok(
        cx As Double, cy As Double, cz As Double,
        px As Double, py As Double
    )
        Dim room As New Room3D With {
            .XLeft = 2, .XRight = 2, .YBack = 2, .YFront = 2,
            .ZBelow = 2, .ZAbove = 2
        }

        Dim c = Point3DHelper.Create(cx, cy, cz)
        Dim p = room.ProjectToAudience(c)

        Assert.AreEqual(px, p.X, AbsoluteCoordPrecision, "X")
        Assert.AreEqual(py, p.Y, AbsoluteCoordPrecision, "Y")
        Assert.AreEqual(0, p.Z, AbsoluteCoordPrecision, "Z")
    End Sub


    <TestMethod>
    <DataRow(0, 0, 0, 0, 0)>
    <DataRow(-1.1, 0, 0, -1, 0)>
    <DataRow(-1, 0, 0, -1, 0)>
    <DataRow(-0.9, 0, 0, -0.9, 0)>
    <DataRow(1, 0, 0, 1, 0)>
    <DataRow(1.1, 0, 0, 1.1, 0)>
    <DataRow(1.2, 0, 0, 1.1, 0)>
    <DataRow(0, -1.3, 0, 0, -1.2)>
    <DataRow(0, -1.2, 0, 0, -1.2)>
    <DataRow(0, -1.1, 0, 0, -1.1)>
    <DataRow(0, 1.2, 0, 0, 1.2)>
    <DataRow(0, 1.3, 0, 0, 1.3)>
    <DataRow(0, 1.4, 0, 0, 1.3)>
    Public Sub RoomDefinition_ProjectToAudience_NonZeroAudience_Ok(
        cx As Double, cy As Double, cz As Double,
        px As Double, py As Double
    )
        Dim room As New Room3D With {
            .XLeft = 2, .XRight = 2, .YBack = 2, .YFront = 2,
            .ZBelow = 2, .ZAbove = 2,
            .AudienceLeft = 1, .AudienceRight = 1.1, .AudienceBack = 1.2, .AudienceFront = 1.3
        }

        Dim c = Point3DHelper.Create(cx, cy, cz)
        Dim p = room.ProjectToAudience(c)

        Assert.AreEqual(px, p.X, AbsoluteCoordPrecision, "X")
        Assert.AreEqual(py, p.Y, AbsoluteCoordPrecision, "Y")
        Assert.AreEqual(0, p.Z, AbsoluteCoordPrecision, "Z")
    End Sub

#End Region


#Region " RoomDefinition.GetProjectedPoint tests "

    <TestMethod>
    Public Sub RoomProjCube_ZeroAudienceOneCoord_Ok()
        ' The test room is 2 meters from 0 in all directions
        Dim room As New Room3D With {
            .XLeft = 2, .XRight = 2, .YBack = 2, .YFront = 2, .ZBelow = 2, .ZAbove = 2
        }

        TestCubeProjection(room, 0, 0, 0, 0, 0, 0, 0, 2)

        TestCubeProjection(room, 1, 0, 0, 0, 0, 2, 0, 0)
        TestCubeProjection(room, 0, 1, 0, 0, 0, 0, 2, 0)
        TestCubeProjection(room, 0, 0, 1, 0, 0, 0, 0, 2)

        TestCubeProjection(room, -1, 0, 0, 0, 0, -2, 0, 0)
        TestCubeProjection(room, 0, -1, 0, 0, 0, 0, -2, 0)
        TestCubeProjection(room, 0, 0, -1, 0, 0, 0, 0, -2)
    End Sub


    <TestMethod>
    Public Sub RoomProjCube_ZeroAudienceTwoCoord_Ok()
        ' The test room is 2 meters from 0 in all directions
        Dim room As New Room3D With {
            .XLeft = 2, .XRight = 2, .YBack = 2, .YFront = 2, .ZBelow = 2, .ZAbove = 2
        }

        TestCubeProjection(room, 1, 0.5, 0, 0, 0, 2, 1, 0)
        TestCubeProjection(room, 0.5, 1, 0, 0, 0, 1, 2, 0)
        TestCubeProjection(room, 1, 0, 0.5, 0, 0, 2, 0, 1)
        TestCubeProjection(room, 0.5, 0, 1, 0, 0, 1, 0, 2)
        TestCubeProjection(room, 0, 1, 0.5, 0, 0, 0, 2, 1)
        TestCubeProjection(room, 0, 0.5, 1, 0, 0, 0, 1, 2)

        TestCubeProjection(room, -1, -0.5, 0, 0, 0, -2, -1, 0)
        TestCubeProjection(room, -0.5, -1, 0, 0, 0, -1, -2, 0)
        TestCubeProjection(room, -1, 0, -0.5, 0, 0, -2, 0, -1)
        TestCubeProjection(room, -0.5, 0, -1, 0, 0, -1, 0, -2)
        TestCubeProjection(room, 0, -1, -0.5, 0, 0, 0, -2, -1)
        TestCubeProjection(room, 0, -0.5, -1, 0, 0, 0, -1, -2)
    End Sub


    <TestMethod>
    Public Sub RoomProjCube_ZeroAudienceThreeCoord_Ok()
        ' The test room is 2 meters from 0 in all directions
        Dim room As New Room3D With {
            .XLeft = 2, .XRight = 2, .YBack = 2, .YFront = 2, .ZBelow = 2, .ZAbove = 2
        }

        TestCubeProjection(room, 1, 1, 1, 0, 0, 2, 2, 2)
        TestCubeProjection(room, 1, 1, 0.5, 0, 0, 2, 2, 1)
        TestCubeProjection(room, 1, 0.5, 1, 0, 0, 2, 1, 2)
        TestCubeProjection(room, 0.5, 1, 1, 0, 0, 1, 2, 2)

        TestCubeProjection(room, 0.5, 0.5, 0.5, 0, 0, 2, 2, 2)
        TestCubeProjection(room, 0.5, 0.5, 1, 0, 0, 1, 1, 2)
        TestCubeProjection(room, 0.5, 1, 0.5, 0, 0, 1, 2, 1)
        TestCubeProjection(room, 1, 0.5, 0.5, 0, 0, 2, 1, 1)
    End Sub


    <TestMethod>
    Public Sub RoomProjSphere_ZeroAudienceOneCoord_Ok()
        ' The test room is 2 meters from 0 in all directions
        Dim room As New Room3D With {
            .XLeft = 2, .XRight = 2, .YBack = 2, .YFront = 2, .ZBelow = 2, .ZAbove = 2
        }

        Assert.AreEqual(3.464, room.RoomSize, AbsoluteCoordPrecision)
        TestSphereProjection(room, 0, 0, 0, 0, 0, 0, 0, 3.464)

        TestSphereProjection(room, 1, 0, 0, 0, 0, 3.464, 0, 0)
        TestSphereProjection(room, 0, 1, 0, 0, 0, 0, 3.464, 0)
        TestSphereProjection(room, 0, 0, 1, 0, 0, 0, 0, 3.464)

        TestSphereProjection(room, -1, 0, 0, 0, 0, -3.464, 0, 0)
        TestSphereProjection(room, 0, -1, 0, 0, 0, 0, -3.464, 0)
        TestSphereProjection(room, 0, 0, -1, 0, 0, 0, 0, -3.464)
    End Sub


    <TestMethod>
    Public Sub RoomProjSphere_NonZeroAudienceOneCoord_Ok()
        ' The test room is 2 meters from 0 in all directions
        Dim room As New Room3D With {
            .XLeft = 2, .XRight = 2, .YBack = 2, .YFront = 2, .ZBelow = 2, .ZAbove = 2,
            .AudienceLeft = 1, .AudienceRight = 1, .AudienceBack = 1, .AudienceFront = 1
        }

        Assert.AreEqual(3.464, room.RoomSize, AbsoluteCoordPrecision)
        TestSphereProjection(room, 0, 0, 0, 0, 0, 0, 0, 3.464)

        TestSphereProjection(room, 2, 0, 0, 1, 0, 3.464, 0, 0)
        TestSphereProjection(room, 0, 2, 0, 0, 1, 0, 3.464, 0)
        TestSphereProjection(room, 0, 0, 2, 0, 0, 0, 0, 3.464)

        TestSphereProjection(room, -2, 0, 0, -1, 0, -3.464, 0, 0)
        TestSphereProjection(room, 0, -2, 0, 0, -1, 0, -3.464, 0)
        TestSphereProjection(room, 0, 0, -2, 0, 0, 0, 0, -3.464)
    End Sub


    <TestMethod>
    Public Sub RoomProjSphere_NonZeroAudienceTwoCoord_Ok()
        ' The test room is 2 meters from 0 in all directions
        Const audsz = 0.2
        Dim room As New Room3D With {
            .XLeft = 2, .XRight = 2, .YBack = 2, .YFront = 2, .ZBelow = 2, .ZAbove = 2,
            .AudienceLeft = audsz, .AudienceRight = audsz, .AudienceBack = audsz, .AudienceFront = audsz
        }

        Assert.AreEqual(3.464, room.RoomSize, AbsoluteCoordPrecision)
        TestSphereProjection(room, 0, 0, 0, 0, 0, 0, 0, 3.464)

        TestSphereProjection(room, 1, 0.5, 0, audsz, audsz, 3.201, 1.325, 0)
        TestSphereProjection(room, 0.5, 1, 0, audsz, audsz, 1.325, 3.201, 0)
        TestSphereProjection(room, 1, 0, 0.5, audsz, 0, 2.992, 0, 1.745)
        TestSphereProjection(room, 0.5, 0, 1, audsz, 0, 1.177, 0, 3.258)
        TestSphereProjection(room, 0, 1, 0.5, 0, audsz, 0, 2.992, 1.745)
        TestSphereProjection(room, 0, 0.5, 1, 0, audsz, 0, 1.177, 3.258)
    End Sub


    <TestMethod>
    Public Sub RoomProjUnknown_Throws()
        ' The test room is 2 meters from 0 in all directions
        Dim room As New Room3D With {
            .XLeft = 2, .XRight = 2, .YBack = 2, .YFront = 2, .ZBelow = 2, .ZAbove = 2
        }

        Dim ex = Assert.ThrowsException(Of ArgumentException)(
            Sub() room.ProjectPoint(Point3DHelper.Origin, CType(999, ProjectionModes)))
        Assert.AreEqual("Unsupported projection mode 999", ex.Message)
    End Sub


    <TestMethod>
    Public Sub RoomDefinition_Convert_Ok()
        Dim room As New Room3D With {
            .XLeft = 1, .XRight = 2, .YBack = 3, .YFront = 4, .ZBelow = 5, .ZAbove = 6
        }

        TestConvertedCoord(room, 0, 0, 0, 0, 0, 0)
        TestConvertedCoord(room, 0.5, 0, 0, 1, 0, 0)
        TestConvertedCoord(room, 0, 0.5, 0, 0, 2, 0)
        TestConvertedCoord(room, 0, 0, 0.5, 0, 0, 3)
        TestConvertedCoord(room, -0.5, 0, 0, -0.5, 0, 0)
        TestConvertedCoord(room, 0, -0.5, 0, 0, -1.5, 0)
        TestConvertedCoord(room, 0, 0, -0.5, 0, 0, -2.5)
    End Sub


    <TestMethod>
    Public Sub RoomDefinition_SettersSameValue_Ok()
        Dim room As New Room3D With {
            .XLeft = 1, .XRight = 2, .YBack = 3, .YFront = 4, .ZBelow = 5, .ZAbove = 6
        }

        AddHandler room.PropertyChanged, Sub(s, e) Assert.Fail(String.Format("Unexpected event on property {0}", e.PropertyName))

        room.XLeft = 1
        room.XRight = 2
        room.YBack = 3
        room.YFront = 4
        room.ZBelow = 5
        room.ZAbove = 6
    End Sub


    <TestMethod>
    Public Sub RoomDefinition_SettersNegativeValue_Ok()
        Dim room As New Room3D With {
            .XLeft = 1, .XRight = 2, .YBack = 3, .YFront = 4, .ZBelow = 5, .ZAbove = 6
        }

        room.XLeft = -1
        room.XRight = -2
        room.YBack = -3
        room.YFront = -4
        room.ZBelow = -5
        room.ZAbove = -6

        Assert.AreEqual(0.0, room.XLeft)
        Assert.AreEqual(0.0, room.XRight)
        Assert.AreEqual(0.0, room.YBack)
        Assert.AreEqual(0.0, room.YFront)
        Assert.AreEqual(0.0, room.ZBelow)
        Assert.AreEqual(0.0, room.ZAbove)

        room.AudienceLeft = -1
        room.AudienceRight = -1
        room.AudienceBack = -1
        room.AudienceFront = -1

        Assert.AreEqual(0.0, room.AudienceLeft)
        Assert.AreEqual(0.0, room.AudienceRight)
        Assert.AreEqual(0.0, room.AudienceBack)
        Assert.AreEqual(0.0, room.AudienceFront)
    End Sub

#End Region

End Class
