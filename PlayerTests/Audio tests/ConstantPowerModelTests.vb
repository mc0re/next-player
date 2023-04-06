Imports Common


<TestClass>
<TestCategory("ConstantPower panning model")>
Public Class ConstantPowerModelTests

#Region " Init and clean-up "

    <ClassInitialize>
    Public Shared Sub InitClass(ctx As TestContext)
        SetupTestIo()
        InterfaceMapper.SetInstance(Of IVolumeConfiguration)(New TestConfig())
    End Sub


    <TestInitialize()>
    Public Sub InitTests()
        InterfaceMapper.SetInstance(Of IMessageLog)(New TestLogger())
    End Sub

#End Region


#Region " Playback tests: mono input, panning 0 "

    <TestMethod>
    Public Sub PlayConstantPower_MonoTo1x1_PanningZero_Ok()
        TestPlayback(New PlayerSetupConstantPower(),
                     "0.7", 1, "1 x 1000", 1, "0.7 x 1000")
        TestPlayback(New PlayerSetupConstantPower With {.Volume = 0.8},
                     "0.5", 1, "0.5 x 1000", 1, "0.2 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayConstantPower_MonoTo1x1_SpecialFlags_Ok()
        TestPlayback(New PlayerSetupConstantPower With {.Mute = True},
                     "0.7", 1, "1 x 1000", 1, "0 x 1000")
        TestPlayback(New PlayerSetupConstantPower(),
                     "0.7:0:+:reversed", 1, "1 x 1000", 1, "-0.7 x 1000")
        TestPlayback(New PlayerSetupConstantPower(),
                     "0.7:0:+:disabled", 1, "1 x 1000", 1, "0 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayConstantPower_MonoTo1x2_PanningZero_Ok()
        TestPlayback(New PlayerSetupConstantPower(),
                     "0.6", 1, "1 x 1000", 2, "0.6,0 x 1000")
        TestPlayback(New PlayerSetupConstantPower With {.Volume = 0.8},
                     "0.5", 1, "0.5 x 1000", 2, "0.2,0 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayConstantPower_MonoTo2x1_PanningZero_Ok()
        TestPlayback(New PlayerSetupConstantPower(),
                     "0.6; 0.4", 1, "1 x 1000", 1, "0.6 x 1000", "0.4 x 1000")
        TestPlayback(New PlayerSetupConstantPower With {.Volume = 0.8},
                     "0.5; 0.5", 1, "0.5 x 1000", 1, "0.2 x 1000", "0.2 x 1000")
    End Sub

#End Region


#Region " Playback tests: mono input, panning non-0 "

    <TestMethod>
    Public Sub PlayConstantPower_MonoTo1x1_PanningNonZero_Ok()
        TestPlayback(New PlayerSetupConstantPower With {.PlaybackPanning = 0.4},
                     "0.7", 1, "1 x 1000", 1, "0.56 x 1000")
        TestPlayback(New PlayerSetupConstantPower With {.Volume = 0.8, .PlaybackPanning = -0.6},
                     "0.5", 1, "0.5 x 1000", 1, "0.14 x 1000")

        ' Panning beyond 2.0 mutes the sound
        TestPlayback(New PlayerSetupConstantPower With {.PlaybackPanning = 2},
                     "0.7", 1, "1,0 x 1000", 1, "0,0 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayConstantPower_MonoTo1x2_PanningNonZero_Ok()
        TestPlayback(New PlayerSetupConstantPower With {.PlaybackPanning = 0.4},
                     "0.7", 1, "1 x 1000", 2, "0.56,0 x 1000")
        TestPlayback(New PlayerSetupConstantPower With {.Volume = 0.8, .PlaybackPanning = -0.6},
                     "0.5", 1, "1 x 1000", 2, "0.28,0 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayConstantPower_MonoTo1x3_PanningNonZero_Ok()
        TestPlayback(New PlayerSetupConstantPower With {.PlaybackPanning = 0.4},
                     "0.7", 1, "1 x 1000", 3, "0.56,0,0 x 1000")
        TestPlayback(New PlayerSetupConstantPower With {.Volume = 0.8, .PlaybackPanning = -0.6},
                     "0.5", 1, "1 x 1000", 3, "0.28,0,0 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayConstantPower_MonoTo2x1_PanningNonZero_Ok()
        TestPlayback(New PlayerSetupConstantPower With {.Volume = 0.8, .PlaybackPanning = -0.6},
                     "0.7; 0.5", 1, "1 x 1000", 1, "0.392 x 1000", "0.28 x 1000")
        TestPlayback(New PlayerSetupConstantPower With {.PlaybackPanning = 0.4},
                     "0.6; 0.4", 1, "1 x 1000", 1, "0.48 x 1000", "0.32 x 1000")
    End Sub

#End Region


#Region " Playback tests: stereo input "

    <TestMethod>
    <TestCategory("NAudio player")>
    Public Sub PlayConstantPower_StereoTo1x1_PanningZero_Ok()
        TestPlayback(New PlayerSetupConstantPower(),
                     "0.7:0:+-", 2, "1,0.8 x 1000", 1, "0.7 x 1000")
        TestPlayback(New PlayerSetupConstantPower(),
                     "0.7:0:++", 2, "1,0.8 x 1000", 1, "0.8910 x 1000")
        TestPlayback(New PlayerSetupConstantPower(),
                     "0.7:0:--", 2, "1,0.8 x 1000", 1, "0 x 1000")
    End Sub


    <TestMethod>
    <TestCategory("NAudio player")>
    Public Sub PlayConstantPower_StereoTo2x1_PanningZero_Ok()
        TestPlayback(New PlayerSetupConstantPower(),
                     "0.7:0:+-; 0.5:0:-+", 2, "1,0.8 x 1000", 1, "0.7 x 1000", "0.4 x 1000")
        ' Mix both input channels
        TestPlayback(New PlayerSetupConstantPower(),
                     "0.7:0:++; 0.5:0:++", 2, "1,0.8 x 1000", 1, "0.8910 x 1000", "0.6364 x 1000")
    End Sub


    <TestMethod>
    <TestCategory("NAudio player")>
    Public Sub PlayConstantPower_StereoTo1x1_PanningNonZero_Ok()
        TestPlayback(New PlayerSetupConstantPower With {.PlaybackPanning = -2.0},
                     "1:0:++", 2, "0.8,0.4 x 1000", 1, "0 x 1000")
        TestPlayback(New PlayerSetupConstantPower With {.PlaybackPanning = 2.0},
                     "1:0:++", 2, "0.8,0.2 x 1000", 1, "0 x 1000")

        TestPlayback(New PlayerSetupConstantPower With {.PlaybackPanning = 0.4},
                     "0.7:0:+-", 2, "1,0.8 x 1000", 1, "0.56 x 1000")
        TestPlayback(New PlayerSetupConstantPower With {.PlaybackPanning = -0.6},
                     "0.7:0:++", 2, "1,0.8 x 1000", 1, "0.6237 x 1000")
        TestPlayback(New PlayerSetupConstantPower With {.PlaybackPanning = 0.5},
                     "0.7:0:--", 2, "1,0.8 x 1000", 1, "0 x 1000")
    End Sub


    <TestMethod>
    <TestCategory("NAudio player")>
    Public Sub PlayConstantPower_StereoTo2x1_PanningNonZero_Ok()
        TestPlayback(New PlayerSetupConstantPower With {.PlaybackPanning = 0.5},
                     "0.7:0:+-; 0.5:0:-+", 2, "1,0.2 x 1000", 1, "0.525 x 1000", "0.075 x 1000")
        TestPlayback(New PlayerSetupConstantPower With {.PlaybackPanning = 1.5},
                     "0.7:0:+-; 0.5:0:-+", 2, "1,0.2 x 1000", 1, "0.175 x 1000", "0.025 x 1000")
        TestPlayback(New PlayerSetupConstantPower With {.PlaybackPanning = -1.5},
                     "0.7:0:+-; 0.5:0:-+", 2, "1,0.2 x 1000", 1, "0.175 x 1000", "0.025 x 1000")
    End Sub

#End Region


#Region " Playback tests: 3-channel input "

    <TestMethod>
    <TestCategory("NAudio player")>
    Public Sub PlayConstantPower_TripleTo1x1_PanningZero_Ok()
        TestPlayback(New PlayerSetupConstantPower(),
                     "0.7:0:+++", 3, "1,0.8,0.3 x 1000", 1, "0.6249 x 1000")
    End Sub


    <TestMethod>
    <TestCategory("NAudio player")>
    Public Sub PlayConstantPower_TripleTo3x1_PanningZero_Ok()
        TestPlayback(New PlayerSetupConstantPower(),
                     "0.6:0:+--; 0.5:0:-+-; 0.4:0:--+",
                     3, "1,0.8,0.4 x 1000",
                     1, "0.6 x 1000", "0.4 x 1000", "0.16 x 1000")
    End Sub


    <TestMethod>
    <TestCategory("NAudio player")>
    Public Sub PlayConstantPower_TripleTo3x1_PanningNonZero_Ok()
        TestPlayback(New PlayerSetupConstantPower With {.PlaybackPanning = 0.4},
                     "1:0:+--; 1:0:-+-; 1:0:--+",
                     3, "1,0.8,0.4 x 1000",
                     1, "0.8 x 1000", "0.64 x 1000", "0.32 x 1000")
        TestPlayback(New PlayerSetupConstantPower With {.PlaybackPanning = -0.4},
                     "1:0:+--; 1:0:-+-; 1:0:--+",
                     3, "1,0.8,0.4 x 1000",
                     1, "0.8 x 1000", "0.64 x 1000", "0.32 x 1000")
    End Sub

#End Region

End Class
