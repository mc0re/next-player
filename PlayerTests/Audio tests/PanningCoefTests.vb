Imports AudioChannelLibrary
Imports Common


<TestClass>
Public Class PanningCoefTests

#Region " Init and clean-up "

    <ClassInitialize>
    Public Shared Sub InitClass(ctx As TestContext)
        SetupTestIo()
        InterfaceMapper.SetInstance(Of IVolumeConfiguration)(New TestConfig())
    End Sub

#End Region


#Region " Test utility "

    Private Shared Function CreateGenerator(
        destPan As Single, sourcePan As Single, nofSource As Integer,
        Optional mapAll As Char = "+"c,
        Optional mapDef As String = Nothing,
        Optional volume As Single = 1.0F,
        Optional model As PanningModels = PanningModels.ConstantVolume
    ) As ICoefficientGenerator
        If String.IsNullOrEmpty(mapDef) Then
            mapDef = String.Empty.PadRight(nofSource, mapAll)
        End If

        Dim env As New TestAudioEnvironmentStorage("1:0:" & mapDef & "|" & destPan)
        Dim f As New PanningCoefficientGeneratorFactory(
            env.LastLogicalChannel, env, nofSource)
        Dim pl As New AudioPlaybackInfo With {
            .PanningModel = model,
            .Panning = sourcePan,
            .Volume = volume
        }
        Dim g = f.Create(pl)

        Return g
    End Function


    Private Shared Sub TestMonoPanning(destPan As Single, sourcePan As Single, coef As Single)
        Dim g = CreateGenerator(destPan, sourcePan, 1)
        Dim c = g.Generate(1)
        Assert.AreEqual(coef, c(0).Volume(0), VolumePrecision)
    End Sub


    Private Shared Sub TestStereoPanning(destPan As Single, sourcePan As Single, coefL As Single, coefR As Single)
        Dim g = CreateGenerator(destPan, sourcePan, 2)
        Dim c = g.Generate(1)
        Assert.AreEqual(coefL, c(0).Volume(0), VolumePrecision)
        Assert.AreEqual(coefR, c(0).Volume(1), VolumePrecision)
    End Sub

#End Region


#Region " Coefficient size and no links tests "

    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_1Inp_NoLinks_Ok()
        Dim g = CreateGenerator(0, 0, 1, "-"c)
        Dim c = g.Generate(1)
        Assert.AreEqual(1, c(0).SourceChannelCount)
        Assert.AreEqual(0.0F, c(0).Volume(0))
    End Sub


    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_1Inp_NoLinks_BeyondSize()
        Dim g = CreateGenerator(0, 0, 1, "-"c)
        Dim c = g.Generate(1)
        Assert.AreEqual(1, c(0).SourceChannelCount)
        Assert.AreEqual(0.0F, c(0).Volume(1))
    End Sub


    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_2Inp_NoLinks_Ok()
        Dim g = CreateGenerator(0, 0, 2, "-"c)
        Dim c = g.Generate(1)
        Assert.AreEqual(2, c(0).SourceChannelCount)
        Assert.AreEqual(0.0F, c(0).Volume(0))
        Assert.AreEqual(0.0F, c(0).Volume(1))
    End Sub


    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_2Inp_NoLinks_BeyondSize()
        Dim g = CreateGenerator(0, 0, 2, "-"c)
        Dim c = g.Generate(1)
        Assert.AreEqual(2, c(0).SourceChannelCount)
        Assert.AreEqual(0.0F, c(0).Volume(2))
    End Sub


    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_3Inp_NoLinks_Ok()
        Dim g = CreateGenerator(0, 0, 3, "-"c)
        Dim c = g.Generate(1)
        Assert.AreEqual(3, c(0).SourceChannelCount)
        Assert.AreEqual(0.0F, c(0).Volume(0))
        Assert.AreEqual(0.0F, c(0).Volume(1))
        Assert.AreEqual(0.0F, c(0).Volume(2))
    End Sub


    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_3Inp_NoLinks_BeyondSize()
        Dim g = CreateGenerator(0, 0, 3, "-"c)
        Dim c = g.Generate(1)
        Assert.AreEqual(3, c(0).SourceChannelCount)
        Assert.AreEqual(0.0F, c(0).Volume(3))
    End Sub


    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_4Inp_NoLinks_Ok()
        Dim g = CreateGenerator(0, 0, 4, "-"c)
        Dim c = g.Generate(1)
        Assert.AreEqual(4, c(0).SourceChannelCount)
        Assert.AreEqual(0.0F, c(0).Volume(0))
        Assert.AreEqual(0.0F, c(0).Volume(1))
        Assert.AreEqual(0.0F, c(0).Volume(2))
        Assert.AreEqual(0.0F, c(0).Volume(3))
    End Sub


    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_4Inp_NoLinks_BeyondSize()
        Dim g = CreateGenerator(0, 0, 4, "-"c)
        Dim c = g.Generate(1)
        Assert.AreEqual(4, c(0).SourceChannelCount)
        Assert.AreEqual(0.0F, c(0).Volume(4))
    End Sub

#End Region


#Region " Linear coefficient calculation 1-of-1 input tests "

    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_Linear_1Inp_DestMiddleSrcVariablePanning_Ok()
        TestMonoPanning(0, -1.0, 0.3536)
        TestMonoPanning(0, -0.9, 0.4079)
        TestMonoPanning(0, -0.8, 0.4648)
        TestMonoPanning(0, -0.7, 0.524)
        TestMonoPanning(0, -0.6, 0.5857)
        TestMonoPanning(0, -0.5, 0.6495)
        TestMonoPanning(0, -0.4, 0.7155)
        TestMonoPanning(0, -0.3, 0.7837)
        TestMonoPanning(0, -0.2, 0.8538)
        TestMonoPanning(0, -0.1, 0.9259)
        TestMonoPanning(0, 0, 1)
        TestMonoPanning(0, 0.1, 0.9259)
        TestMonoPanning(0, 0.2, 0.8538)
        TestMonoPanning(0, 0.3, 0.7837)
        TestMonoPanning(0, 0.4, 0.7155)
        TestMonoPanning(0, 0.5, 0.6495)
        TestMonoPanning(0, 0.6, 0.5857)
        TestMonoPanning(0, 0.7, 0.524)
        TestMonoPanning(0, 0.8, 0.4648)
        TestMonoPanning(0, 0.9, 0.4079)
        TestMonoPanning(0, 1.0, 0.3536)
    End Sub


    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_Linear_1Inp_DestVariableSrcMiddlePanning_Ok()
        TestMonoPanning(-1.0, 0, 1)
        TestMonoPanning(-0.9, 0, 1)
        TestMonoPanning(-0.8, 0, 1)
        TestMonoPanning(-0.7, 0, 1)
        TestMonoPanning(-0.6, 0, 1)
        TestMonoPanning(-0.5, 0, 1)
        TestMonoPanning(-0.4, 0, 1)
        TestMonoPanning(-0.3, 0, 1)
        TestMonoPanning(-0.2, 0, 1)
        TestMonoPanning(-0.1, 0, 1)
        TestMonoPanning(0.0, 0, 1)
        TestMonoPanning(0.1, 0, 1)
        TestMonoPanning(0.2, 0, 1)
        TestMonoPanning(0.3, 0, 1)
        TestMonoPanning(0.4, 0, 1)
        TestMonoPanning(0.5, 0, 1)
        TestMonoPanning(0.6, 0, 1)
        TestMonoPanning(0.7, 0, 1)
        TestMonoPanning(0.8, 0, 1)
        TestMonoPanning(0.9, 0, 1)
        TestMonoPanning(1.0, 0, 1)
    End Sub


    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_Linear_1Inp_DestVariableSrcVariablePanning_Ok()
        TestMonoPanning(-1.0, -1.0, 0.3536)
        TestMonoPanning(-0.9, -0.9, 0.4079)
        TestMonoPanning(-0.8, -0.8, 0.4648)
        TestMonoPanning(-0.7, -0.7, 0.524)
        TestMonoPanning(-0.6, -0.6, 0.5857)
        TestMonoPanning(-0.5, -0.5, 0.6495)
        TestMonoPanning(-0.4, -0.4, 0.7155)
        TestMonoPanning(-0.3, -0.3, 0.7837)
        TestMonoPanning(-0.2, -0.2, 0.8538)
        TestMonoPanning(-0.1, -0.1, 0.9259)
        TestMonoPanning(0.0, 0.0, 1)
        TestMonoPanning(0.1, 0.1, 0.9259)
        TestMonoPanning(0.2, 0.2, 0.8538)
        TestMonoPanning(0.3, 0.3, 0.7837)
        TestMonoPanning(0.4, 0.4, 0.7155)
        TestMonoPanning(0.5, 0.5, 0.6495)
        TestMonoPanning(0.6, 0.6, 0.5857)
        TestMonoPanning(0.7, 0.7, 0.524)
        TestMonoPanning(0.8, 0.8, 0.4648)
        TestMonoPanning(0.9, 0.9, 0.4079)
        TestMonoPanning(1.0, 1.0, 0.3536)
    End Sub


    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_Linear_1Inp_PanningBeyond_Ok()
        TestMonoPanning(0, -2, 0)
        TestMonoPanning(0, 2, 0)
    End Sub


    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_Linear_1Inp_Volume_Ok()
        Dim g = CreateGenerator(0, 0, 1, volume:=0.7)
        Dim c = g.Generate(1)
        Assert.AreEqual(0.7F, c(0).Volume(0))
    End Sub


    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_Linear_1Inp_VolumePhase_Ok()
        Dim g = CreateGenerator(0, 0, 1, volume:=-0.3)
        Dim c = g.Generate(1)
        Assert.AreEqual(-0.3F, c(0).Volume(0))
    End Sub

#End Region


#Region " Linear coefficient calculation 1-of-2 tests "

    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_Linear_2Inp_LeftLink_Ok()
        Dim g = CreateGenerator(0, 0, 2, mapDef:="+-")
        Dim c = g.Generate(1)
        Assert.AreEqual(1.0F, c(0).Volume(0))
        Assert.AreEqual(0.0F, c(0).Volume(1))
    End Sub


    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_Linear_2Inp_LeftLinkOwnPanningFullLeft_Ok()
        Dim g = CreateGenerator(-1, 0, 2, mapDef:="+-")
        Dim c = g.Generate(1)
        Assert.AreEqual(1.0F, c(0).Volume(0))
        Assert.AreEqual(0.0F, c(0).Volume(1))
    End Sub


    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_Linear_2Inp_LeftLinkPanningFullLeft_Ok()
        Dim g = CreateGenerator(0, -1, 2, mapDef:="+-")
        Dim c = g.Generate(1)
        Assert.AreEqual(0.3536F, c(0).Volume(0), VolumePrecision)
        Assert.AreEqual(0.0F, c(0).Volume(1))
    End Sub


    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_Linear_2Inp_LeftLinkPanningFullRight_Ok()
        Dim g = CreateGenerator(0, 1, 2, mapDef:="+-")
        Dim c = g.Generate(1)
        Assert.AreEqual(0.3536F, c(0).Volume(0), VolumePrecision)
        Assert.AreEqual(0.0F, c(0).Volume(1))
    End Sub


    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_Linear_2Inp_RightLink_Ok()
        Dim g = CreateGenerator(0, 0, 2, mapDef:="-+")
        Dim c = g.Generate(1)
        Assert.AreEqual(0.0F, c(0).Volume(0))
        Assert.AreEqual(1.0F, c(0).Volume(1))
    End Sub


    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_Linear_2Inp_RightLinkOwnPanning_Ok()
        Dim g = CreateGenerator(1, 0, 2, mapDef:="-+")
        Dim c = g.Generate(1)
        Assert.AreEqual(0.0F, c(0).Volume(0))
        Assert.AreEqual(1.0F, c(0).Volume(1))
    End Sub


    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_Linear_2Inp_RightLinkPanningFullLeft_Ok()
        Dim g = CreateGenerator(0, -1, 2, mapDef:="-+")
        Dim c = g.Generate(1)
        Assert.AreEqual(0.0F, c(0).Volume(0))
        Assert.AreEqual(0.3536F, c(0).Volume(1), VolumePrecision)
    End Sub


    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_Linear_2Inp_RightLinkPanningFullRight_Ok()
        Dim g = CreateGenerator(0, 1, 2, mapDef:="-+")
        Dim c = g.Generate(1)
        Assert.AreEqual(0.0F, c(0).Volume(0))
        Assert.AreEqual(0.3536F, c(0).Volume(1), VolumePrecision)
    End Sub

#End Region


#Region " Linear coefficient calculation stereo-to-mono tests "

    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_Linear_2Inp_DestMiddleSrcVariablePanning_Ok()
        TestStereoPanning(0, -1, 0.25, 0.25)
        TestStereoPanning(0, -0.9, 0.2884, 0.2884)
        TestStereoPanning(0, -0.8, 0.3286, 0.3286)
        TestStereoPanning(0, -0.7, 0.3706, 0.3706)
        TestStereoPanning(0, -0.6, 0.4141, 0.4141)
        TestStereoPanning(0, -0.5, 0.4593, 0.4593)
        TestStereoPanning(0, -0.4, 0.506, 0.506)
        TestStereoPanning(0, -0.3, 0.5541, 0.5541)
        TestStereoPanning(0, -0.2, 0.6037, 0.6037)
        TestStereoPanning(0, -0.1, 0.6547, 0.6547)
        TestStereoPanning(0, 0, 0.7071, 0.7071)
        TestStereoPanning(0, 0.1, 0.6547, 0.6547)
        TestStereoPanning(0, 0.2, 0.6037, 0.6037)
        TestStereoPanning(0, 0.3, 0.5541, 0.5541)
        TestStereoPanning(0, 0.4, 0.506, 0.506)
        TestStereoPanning(0, 0.5, 0.4593, 0.4593)
        TestStereoPanning(0, 0.6, 0.4141, 0.4141)
        TestStereoPanning(0, 0.7, 0.3706, 0.3706)
        TestStereoPanning(0, 0.8, 0.3286, 0.3286)
        TestStereoPanning(0, 0.9, 0.2884, 0.2884)
        TestStereoPanning(0, 1, 0.25, 0.25)
    End Sub


    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_Linear_2Inp_DestVariableSrcMiddlePanning_Ok()
        TestStereoPanning(-1.0, 0, 1, 0)
        TestStereoPanning(-0.9, 0, 0.9747, 0.2236)
        TestStereoPanning(-0.8, 0, 0.9487, 0.3162)
        TestStereoPanning(-0.7, 0, 0.922, 0.3873)
        TestStereoPanning(-0.6, 0, 0.8944, 0.4472)
        TestStereoPanning(-0.5, 0, 0.866, 0.5)
        TestStereoPanning(-0.4, 0, 0.8367, 0.5477)
        TestStereoPanning(-0.3, 0, 0.8062, 0.5916)
        TestStereoPanning(-0.2, 0, 0.7746, 0.6325)
        TestStereoPanning(-0.1, 0, 0.7416, 0.6708)
        TestStereoPanning(0.0, 0, 0.7071, 0.7071)
        TestStereoPanning(0.1, 0, 0.6708, 0.7416)
        TestStereoPanning(0.2, 0, 0.6325, 0.7746)
        TestStereoPanning(0.3, 0, 0.5916, 0.8062)
        TestStereoPanning(0.4, 0, 0.5477, 0.8367)
        TestStereoPanning(0.5, 0, 0.5, 0.866)
        TestStereoPanning(0.6, 0, 0.4472, 0.8944)
        TestStereoPanning(0.7, 0, 0.3873, 0.922)
        TestStereoPanning(0.8, 0, 0.3162, 0.9487)
        TestStereoPanning(0.9, 0, 0.2236, 0.9747)
        TestStereoPanning(1.0, 0, 0, 1)
    End Sub


    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_Linear_2Inp_BothLinksExtremePanning_Ok()
        TestStereoPanning(-1, -1, 0.3536, 0)
        TestStereoPanning(1, 1, 0, 0.3536)
    End Sub


    <TestMethod>
    <TestCategory("NAudio player panning coef")>
    Public Sub NAudioPlayerCoef_Linear_2Inp_BothLinksVolume_Ok()
        Dim g = CreateGenerator(0, 0, 2, volume:=0.7)
        Dim c = g.Generate(1)
        Assert.AreEqual(0.495, c(0).Volume(0), VolumePrecision)
        Assert.AreEqual(0.495, c(0).Volume(1), VolumePrecision)
    End Sub

#End Region

End Class
