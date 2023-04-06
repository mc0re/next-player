Imports Common


<TestClass>
<TestCategory("Fixed panning model")>
Public Class FixedModelTests

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


#Region " Mono input, all panning 0 "

    <TestMethod>
    Public Sub PlayFixed_MonoTo1x1_PanningZero_Ok()
        TestPlayback(New PlayerSetupFixed(),
                     "0.8", 1, "1 x 1000", 1, "0.8 x 1000")
        TestPlayback(New PlayerSetupFixed With {.Volume = 0.8},
                     "0.5", 1, "0.5 x 1000", 1, "0.2 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayFixed_MonoTo1x1_SpecialFlags_Ok()
        TestPlayback(New PlayerSetupFixed With {.Mute = True},
                     "0.8", 1, "1 x 1000", 1, "0 x 1000")
        TestPlayback(New PlayerSetupFixed(),
                     "0.8:0:+:reversed", 1, "1 x 1000", 1, "-0.8 x 1000")
        TestPlayback(New PlayerSetupFixed(),
                     "0.8:0:+:disabled", 1, "1 x 1000", 1, "0 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayFixed_MonoTo1x2_PanningZero_Ok()
        TestPlayback(New PlayerSetupFixed(),
                     "0.8", 1, "1 x 1000", 2, "0.8,0 x 1000")
        TestPlayback(New PlayerSetupFixed With {.Volume = 0.8},
                     "0.5", 1, "0.5 x 1000", 2, "0.2,0 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayFixed_MonoTo2x1_PanningZero_Ok()
        TestPlayback(New PlayerSetupFixed(),
                     "0.8; 0.4", 1, "1,0 x 1000", 1, "0.8,0 x 1000", "0.4,0 x 1000")
        TestPlayback(New PlayerSetupFixed With {.Volume = 0.8},
                     "0.5; 0.5", 1, "1,0 x 1000", 1, "0.4,0 x 1000", "0.4,0 x 1000")
    End Sub

#End Region


#Region " Mono input, source panning non-0 "

    <TestMethod>
    Public Sub PlayFixed_MonoTo1x1_SourcePanningNonZero_Ok()
        ' Source panning has no effect on mono sources
        TestPlayback(New PlayerSetupFixed(),
                     "0.8:0:+|-1", 1, "1 x 1000", 1, "0.8 x 1000")
        TestPlayback(New PlayerSetupFixed With {.Volume = 0.8},
                     "0.5:0:+|1", 1, "0.5 x 1000", 1, "0.2 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayFixed_MonoTo1x2_SourcePanningNonZero_Ok()
        ' Source panning has no effect on mono sources
        TestPlayback(New PlayerSetupFixed(),
                     "0.8:0:+|-1", 1, "1 x 1000", 2, "0.8,0 x 1000")
        TestPlayback(New PlayerSetupFixed With {.Volume = 0.8},
                     "0.5:0:+|1", 1, "0.5 x 1000", 2, "0.2,0 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayFixed_MonoTo1x3_SourcePanningNonZero_Ok()
        ' Source panning has no effect on mono sources
        TestPlayback(New PlayerSetupFixed(),
                     "0.8:0:+|-1", 1, "1 x 1000", 3, "0.8,0,0 x 1000")
        TestPlayback(New PlayerSetupFixed With {.Volume = 0.8},
                     "0.5:0:+|1", 1, "0.5 x 1000", 3, "0.2,0,0 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayFixed_MonoTo2x1_SourcePanningNonZero_Ok()
        ' Source panning has no effect on mono sources
        TestPlayback(New PlayerSetupFixed(),
                     "0.8:0:+|-1; 0.4:0:+|1", 1, "1 x 1000", 1, "0.8 x 1000", "0.4 x 1000")
        TestPlayback(New PlayerSetupFixed With {.Volume = 0.8},
                     "0.5:0:+|-1; 0.5:0:+|1", 1, "1 x 1000", 1, "0.4 x 1000", "0.4 x 1000")
    End Sub

#End Region


#Region " Mono input, playback panning non-0 "

    <TestMethod>
    Public Sub PlayFixed_MonoTo1x1_PanningNonZero_Ok()
        TestPlayback(New PlayerSetupFixed With {.Volume = 0.5, .PlaybackPanning = 0.4},
                     "0.8", 1, "1 x 1000", 1, "0.3578 x 1000")
        ' Too large panning reduces the output to 0
        TestPlayback(New PlayerSetupFixed With {.PlaybackPanning = -2},
                     "0.5", 1, "0.5 x 1000", 1, "0 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayFixed_MonoTo1x2_PanningNonZero_Ok()
        ' The sign of panning is not important
        TestPlayback(New PlayerSetupFixed With {.PlaybackPanning = -0.5},
                     "0.8", 1, "1 x 1000", 2, "0.6928,0 x 1000")
        ' 0.6928 / 0.8 * 0.5 (source volume) / 1 * 0.5 (source wave) * 0.8 (play volume) = 0.1732
        TestPlayback(New PlayerSetupFixed With {.Volume = 0.8, .PlaybackPanning = 0.5},
                     "0.5", 1, "0.5 x 1000", 2, "0.1732,0 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayFixed_MonoTo1x3_PanningNonZero_Ok()
        TestPlayback(New PlayerSetupFixed With {.PlaybackPanning = 0.4},
                     "0.8", 1, "1 x 1000", 3, "0.7155,0,0 x 1000")
        TestPlayback(New PlayerSetupFixed With {.Volume = 0.8, .PlaybackPanning = -0.6},
                     "0.5", 1, "0.5 x 1000", 3, "0.1673,0,0 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayFixed_MonoTo2x1_PanningNonZero_Ok()
        ' PlaybackPanning shifts the virtual mono source from the source channels
        TestPlayback(New PlayerSetupFixed With {.PlaybackPanning = 0.4},
                     "0.8; 0.4", 1, "1 x 1000", 1, "0.7155 x 1000", "0.3578 x 1000")
        TestPlayback(New PlayerSetupFixed With {.Volume = 0.8, .PlaybackPanning = -0.6},
                     "0.5; 0.5", 1, "1 x 1000", 1, "0.3347 x 1000", "0.3347 x 1000")
    End Sub

#End Region


#Region " Stereo input "

    <TestMethod>
    Public Sub PlayFixed_StereoTo1x1_PanningZero_Ok()
        ' Skip the right channel
        TestPlayback(New PlayerSetupFixed(),
                     "0.8:0:+-", 2, "1,0.5 x 1000", 1, "0.8 x 1000")
        ' Skip the left channel
        TestPlayback(New PlayerSetupFixed(),
                     "0.8:0:-+", 2, "1,0.5 x 1000", 1, "0.4 x 1000")
        ' Mix both channels
        TestPlayback(New PlayerSetupFixed(),
                     "0.8:0:++", 2, "1,0.5 x 1000", 1, "0.8485 x 1000")
        ' No channels are used
        TestPlayback(New PlayerSetupFixed(),
                     "0.8:0:--", 2, "1,0.5 x 1000", 1, "0 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayFixed_StereoTo2x1_PanningZero_Ok()
        TestPlayback(New PlayerSetupFixed(),
                     "0.8:0:+-; 0.5:0:-+", 2, "1,0.5 x 1000", 1, "0.8 x 1000", "0.25 x 1000")
        ' Mix both input channels
        TestPlayback(New PlayerSetupFixed(),
                     "1.0:0:++; 0.4:0:++", 2, "1,0.5 x 1000", 1, "1 x 1000", "0.4243 x 1000")
        ' TODO: 95% link volume still maxes out the signal
        TestPlayback(New PlayerSetupFixed(),
                     "0.95:0:++; 0.4:0:++", 2, "1,0.5 x 1000", 1, "1 x 1000", "0.4243 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayFixed_StereoTo1x1_PanningNonZero_Ok()
        ' Full panning
        TestPlayback(New PlayerSetupFixed With {.PlaybackPanning = -1},
                     "0.6:0:++", 2, "1,0.5 x 1000", 1, "0.45 x 1000")
        TestPlayback(New PlayerSetupFixed With {.PlaybackPanning = 1},
                     "0.6:0:++", 2, "1,0.5 x 1000", 1, "0.45 x 1000")

        ' Somewhere in between
        TestPlayback(New PlayerSetupFixed With {.PlaybackPanning = 0.5},
                     "0.6:0:+-", 2, "1,0.5 x 1000", 1, "0.5196 x 1000")
        TestPlayback(New PlayerSetupFixed With {.PlaybackPanning = -0.5},
                     "0.6:0:++", 2, "1,0.5 x 1000", 1, "0.5511 x 1000")
        TestPlayback(New PlayerSetupFixed With {.PlaybackPanning = 0.3},
                     "0.6:0:--", 2, "1,0.5 x 1000", 1, "0 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayFixed_StereoTo2x1_PanningNonZero_Ok()
        TestPlayback(New PlayerSetupFixed With {.PlaybackPanning = 0.5},
                     "0.6:0:+-; 0.5:0:-+", 2, "1,0.5 x 1000", 1, "0.5196 x 1000", "0.2165 x 1000")
        ' Both output channels are located at 0, so panning's sign plays no role
        TestPlayback(New PlayerSetupFixed With {.PlaybackPanning = 1},
                     "0.6:0:+-; 0.5:0:-+", 2, "1,0.5 x 1000", 1, "0.4243 x 1000", "0.1768 x 1000")
        TestPlayback(New PlayerSetupFixed With {.PlaybackPanning = -1},
                     "0.6:0:+-; 0.5:0:-+", 2, "1,0.5 x 1000", 1, "0.4243 x 1000", "0.1768 x 1000")
    End Sub

#End Region


#Region " Playback tests: 3-channel input "

    <TestMethod>
    Public Sub PlayFixed_TripleTo1x1_PanningZero_Ok()
        ' Input channels are always mapped using ConstantPower
        TestPlayback(New PlayerSetupFixed(),
                     "0.6:0:+++", 3, "1,0.8,0.3 x 1000", 1, "0.5356 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayFixed_TripleTo3x1_PanningZero_Ok()
        TestPlayback(New PlayerSetupFixed(),
                     "0.6:0:+--; 0.5:0:-+-; 0.4:0:--+",
                     3, "1,0.8,0.4 x 1000",
                     1, "0.6 x 1000", "0.4 x 1000", "0.16 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayFixed_TripleTo3x1_PanningNonZero_Ok()
        ' Both output channels are located at 0, so panning's sign plays no role
        TestPlayback(New PlayerSetupFixed With {.PlaybackPanning = 0.4},
                     "1:0:+--; 1:0:-+-; 1:0:--+",
                     3, "1,0.8,0.4 x 1000",
                     1, "0.8944 x 1000", "0.7155 x 1000", "0.3578 x 1000")
        TestPlayback(New PlayerSetupFixed With {.PlaybackPanning = -0.4},
                     "1:0:+--; 1:0:-+-; 1:0:--+",
                     3, "1,0.8,0.4 x 1000",
                     1, "0.8944 x 1000", "0.7155 x 1000", "0.3578 x 1000")
    End Sub

#End Region

End Class
