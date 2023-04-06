Imports AudioChannelLibrary
Imports Common


<TestClass>
<TestCategory("3D position coefficients")>
Public Class PositionModelTests

#Region " Fields "

    Private Shared mFactory As ICoefficientGeneratorFactory

#End Region


#Region " Init and clean-up "

    <ClassInitialize>
    Public Shared Sub InitClass(ctx As TestContext)
        SetupTestIo()
        InterfaceMapper.SetInstance(Of IVolumeConfiguration)(New TestConfig())
    End Sub

#End Region


#Region " Test utility "

    ''' <summary>
    ''' Create audio setup for testing.
    ''' </summary>
    ''' <param name="room"></param>
    ''' <param name="source">Virtual source's coordinates</param>
    ''' <param name="volume">Virtual source's volume</param>
    ''' <returns>
    ''' A <see cref="PositionCoefficientGenerator"/> instance.
    ''' </returns>
    Private Shared Function CreateGenerator(
        room As Room3D,
        channels As IPoint3D(),
        source As IPoint3D,
        Optional volume As Single = 1.0F
    ) As ICoefficientGenerator

        Dim linkDef = String.Join(";", Enumerable.Repeat("1", channels.Length))
        Dim env As New TestAudioEnvironmentStorage(linkDef)

        For chIdx = 0 To channels.Length - 1
            Dim ph = env.Physical(chIdx)
            ph.X = channels(chIdx).X
            ph.Y = channels(chIdx).Y
            ph.Z = channels(chIdx).Z
        Next

        env.SetRoom(room)
        mFactory = New PositionCoefficientGeneratorFactory(env.LastLogicalChannel, env, 1)

        Dim pl As New AudioPlaybackInfo With {
            .PanningModel = PanningModels.Coordinates,
            .X = CSng(source.X), .Y = CSng(source.Y), .Z = CSng(source.Z),
            .Volume = volume
        }

        Return mFactory.Create(pl)
    End Function


    Private Shared Function MoveSource(
        source As IPoint3D, Optional volume As Single = 1.0F
    ) As ICoefficientGenerator

        Dim pl As New AudioPlaybackInfo With {
            .PanningModel = PanningModels.Coordinates,
            .X = CSng(source.X), .Y = CSng(source.Y), .Z = CSng(source.Z),
            .Volume = volume
        }

        Return mFactory.Create(pl)
    End Function

#End Region


#Region " Tests "

    <TestMethod>
    Public Sub PositionCoef_1ChAt0_SrcAt0_Ok()
        Dim room As New Room3D()
        Dim g = CreateGenerator(room, {Point3DHelper.Create(0, 0, 0)}, Point3DHelper.Create(0, 0, 0))
        Dim c = g.Generate(1)
        Assert.AreEqual(6.25, c(0).Volume(0), VolumePrecision)
    End Sub


    <TestMethod>
    Public Sub PositionCoef_1ChAt1_SrcAt1_Ok()
        Dim room As New Room3D()
        Dim g = CreateGenerator(room, {Point3DHelper.Create(1, 0, 0)}, Point3DHelper.Create(1, 0, 0))
        Dim c = g.Generate(1)
        Assert.AreEqual(1, c(0).Volume(0), VolumePrecision)
    End Sub


    <TestMethod>
    Public Sub PositionCoef_3ChBelowSrc_SrcAt1_Ok()
        Dim room As New Room3D()
        Dim g = CreateGenerator(room,
                                {
                                    Point3DHelper.Create(-1, 0, 0.5),
                                    Point3DHelper.Create(1, -1, 0.5),
                                    Point3DHelper.Create(1, 1, 0.5)
                                }, Point3DHelper.Create(0, 0, 1))
        Dim c1 = g.Generate(1)
        Assert.AreEqual(0.5178, c1(0).Volume(0), VolumePrecision)
        Dim c2 = g.Generate(2)
        Assert.AreEqual(0.659, c2(0).Volume(0), VolumePrecision)
        Dim c3 = g.Generate(3)
        Assert.AreEqual(0.659, c3(0).Volume(0), VolumePrecision)
    End Sub


    <TestMethod>
    Public Sub PositionCoef_2Ch_Switch()
        Dim room As New Room3D()
        Dim g = CreateGenerator(room,
                                {
                                    Point3DHelper.Create(-0.8, 0.5, 0.5),
                                    Point3DHelper.Create(0.8, 0.5, 0.5)
                                }, Point3DHelper.Create(-0.9, 0.5, 0.5))
        Dim a1 = g.Generate(1)
        Assert.AreEqual(0.8702, a1(0).Volume(0), VolumePrecision)
        Dim a2 = g.Generate(2)
        Assert.AreEqual(0, a2.Count)

        g = MoveSource(Point3DHelper.Create(-0.8, 0.5, 0.5))
        Dim b1 = g.Generate(1)
        Assert.AreEqual(1, b1(0).Volume(0), VolumePrecision)
        Dim b2 = g.Generate(2)
        Assert.AreEqual(0, b2.Count)

        g = MoveSource(Point3DHelper.Create(-0.7, 0.5, 0.5))
        Dim c1 = g.Generate(1)
        Assert.AreEqual(1.1515, c1(0).Volume(0), VolumePrecision)
        Dim c2 = g.Generate(2)
        Assert.AreEqual(0, c2.Count)

        g = MoveSource(Point3DHelper.Create(0.9, 0.5, 0.5))
        Dim d1 = g.Generate(1)
        Assert.AreEqual(0, d1.Count)
        Dim d2 = g.Generate(2)
        Assert.AreEqual(0.8702, d2(0).Volume(0), VolumePrecision)
    End Sub

#End Region

End Class
