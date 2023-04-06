Imports Common


<TestClass>
<TestCategory("ConstantVolume panning model")>
Public Class ConstantVolumeModelTests

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


#Region " Mono input, panning 0 "

    <TestMethod>
    Public Sub PlayConstantVolume_MonoTo1x1_PanningZero_Ok()
        TestPlayback(New PlayerSetupConstantVolume(),
                     "0.7", 1, "1 x 1000", 1, "0.7 x 1000")
        TestPlayback(New PlayerSetupConstantVolume With {.Volume = 0.8},
                     "0.5", 1, "0.5 x 1000", 1, "0.2 x 1000")
        TestPlayback(New PlayerSetupConstantVolume With {.Volume = 0.8},
                     "0.5:0:-", 1, "0.5 x 1000", 1, "0 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayConstantVolume_MonoTo1x1_SpecialFlags_Ok()
        TestPlayback(New PlayerSetupConstantVolume With {.Mute = True},
                     "0.7", 1, "1 x 1000", 1, "0 x 1000")
        TestPlayback(New PlayerSetupConstantVolume(),
                     "0.7:0:+:reversed", 1, "1 x 1000", 1, "-0.7 x 1000")
        TestPlayback(New PlayerSetupConstantVolume(),
                     "0.7:0:+:disabled", 1, "1 x 1000", 1, "0 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayConstantVolume_MonoTo1x2_PanningZero_Ok()
        TestPlayback(New PlayerSetupConstantVolume(),
                     "0.6", 1, "1 x 1000", 2, "0.6,0 x 1000")
        TestPlayback(New PlayerSetupConstantVolume With {.Volume = 0.8},
                     "0.5", 1, "0.5 x 1000", 2, "0.2,0 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayConstantVolume_MonoTo2x1_PanningZero_Ok()
        TestPlayback(New PlayerSetupConstantVolume(),
                     "0.6; 0.4", 1, "1 x 1000", 1, "0.6 x 1000", "0.4 x 1000")
        TestPlayback(New PlayerSetupConstantVolume With {.Volume = 0.8},
                     "0.5; 0.5", 1, "0.5 x 1000", 1, "0.2 x 1000", "0.2 x 1000")
    End Sub

#End Region


#Region " Mono input, playback panning non-0 "

    <TestMethod>
    Public Sub PlayConstantVolume_MonoTo1x1_PanningNonZero_Ok()
        TestPlayback(New PlayerSetupConstantVolume With {.PlaybackPanning = 0.4},
                     "0.7", 1, "1 x 1000", 1, "0.5009 x 1000")
        TestPlayback(New PlayerSetupConstantVolume With {.PlaybackPanning = -0.4},
                     "0.7", 1, "1 x 1000", 1, "0.5009 x 1000")
        TestPlayback(New PlayerSetupConstantVolume With {.Volume = 0.8, .PlaybackPanning = 0.4},
                     "0.5", 1, "1 x 1000", 1, "0.2862 x 1000")

        ' Panning beyond 2.0 mutes the sound
        TestPlayback(New PlayerSetupConstantVolume With {.PlaybackPanning = 2},
                     "0.7", 1, "1,0 x 1000", 1, "0,0 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayConstantVolume_MonoTo1x2_PanningNonZero_Ok()
        TestPlayback(New PlayerSetupConstantVolume With {.PlaybackPanning = 0.4},
                     "0.7", 1, "1 x 1000", 2, "0.5009,0 x 1000")
        TestPlayback(New PlayerSetupConstantVolume With {.Volume = 0.8, .PlaybackPanning = 0.4},
                     "0.5", 1, "1 x 1000", 2, "0.2862,0 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayConstantVolume_MonoTo1x3_PanningNonZero_Ok()
        TestPlayback(New PlayerSetupConstantVolume With {.PlaybackPanning = 0.4},
                     "0.7", 1, "1 x 1000", 3, "0.5009,0,0 x 1000")
        TestPlayback(New PlayerSetupConstantVolume With {.Volume = 0.8, .PlaybackPanning = 0.4},
                     "0.5", 1, "1 x 1000", 3, "0.2862,0,0 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayConstantVolume_MonoTo2x1_PanningNonZero_Ok()
        TestPlayback(New PlayerSetupConstantVolume With {.Volume = 0.8, .PlaybackPanning = -0.6},
                     "0.5; 0.5", 1, "1 x 1000", 1, "0.2343 x 1000", "0.2343 x 1000")
        TestPlayback(New PlayerSetupConstantVolume With {.PlaybackPanning = 0.4},
                     "0.6; 0.4", 1, "1 x 1000", 1, "0.4293 x 1000", "0.2862 x 1000")
    End Sub

#End Region


#Region " Stereo input "

    <TestMethod>
    Public Sub PlayConstantVolume_StereoTo1x1_PanningZero_Ok()
        TestPlayback(New PlayerSetupConstantVolume(),
                     "0.6:0:+-", 2, "1,0.8 x 1000", 1, "0.6 x 1000")
        TestPlayback(New PlayerSetupConstantVolume(),
                     "0.6:0:-+", 2, "1,0.8 x 1000", 1, "0.48 x 1000")
        TestPlayback(New PlayerSetupConstantVolume(),
                     "0.6:0:++", 2, "1,0.8 x 1000", 1, "0.7637 x 1000")
        TestPlayback(New PlayerSetupConstantVolume(),
                     "0.6:0:--", 2, "1,0.8 x 1000", 1, "0 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayConstantVolume_StereoTo2x1_PanningZero_Ok()
        TestPlayback(New PlayerSetupConstantVolume(),
                     "0.6:0:+-; 0.5:0:-+", 2, "1,0.8 x 1000", 1, "0.6 x 1000", "0.4 x 1000")
        ' Mix both input channels
        TestPlayback(New PlayerSetupConstantVolume(),
                     "0.6:0:++; 0.5:0:++", 2, "1,0.8 x 1000", 1, "0.7637 x 1000", "0.6364 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayConstantVolume_StereoTo1x1_PanningNonZero_Ok()
        TestPlayback(New PlayerSetupConstantVolume With {.PlaybackPanning = -2.0},
                     "1:0:++", 2, "1,0.8 x 1000", 1, "0.0 x 1000")
        TestPlayback(New PlayerSetupConstantVolume With {.PlaybackPanning = 2.0},
                     "1:0:++", 2, "1,0.8 x 1000", 1, "0.0 x 1000")

        TestPlayback(New PlayerSetupConstantVolume With {.PlaybackPanning = 0.4},
                     "0.6:0:+-", 2, "1,0.8 x 1000", 1, "0.4293 x 1000")
        TestPlayback(New PlayerSetupConstantVolume With {.PlaybackPanning = 0.4},
                     "0.6:0:-+", 2, "1,0.8 x 1000", 1, "0.3435 x 1000")
        TestPlayback(New PlayerSetupConstantVolume With {.PlaybackPanning = 0.4},
                     "0.6:0:++", 2, "1,0.8 x 1000", 1, "0.5464 x 1000")
        TestPlayback(New PlayerSetupConstantVolume With {.PlaybackPanning = 0.5},
                     "0.6:0:--", 2, "1,0.8 x 1000", 1, "0 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayConstantVolume_StereoTo2x1_PanningNonZero_Ok()
        TestPlayback(New PlayerSetupConstantVolume With {.PlaybackPanning = 0.5},
                     "0.6:0:+-; 0.5:0:-+", 2, "1,0.8 x 1000", 1, "0.3897 x 1000", "0.2598 x 1000")
        TestPlayback(New PlayerSetupConstantVolume With {.PlaybackPanning = 1},
                     "0.6:0:+-; 0.5:0:-+", 2, "1,0.8 x 1000", 1, "0.2121 x 1000", "0.1414 x 1000")
        TestPlayback(New PlayerSetupConstantVolume With {.PlaybackPanning = 1.5},
                     "0.6:0:+-; 0.5:0:-+", 2, "1,0.8 x 1000", 1, "0.0750 x 1000", "0.0500 x 1000")
    End Sub

#End Region


#Region " 3-channel input "

    <TestMethod>
    Public Sub PlayConstantVolume_TripleTo1x1_PanningZero_Ok()
        TestPlayback(New PlayerSetupConstantVolume(),
                     "0.6:0:+++", 3, "1,0.8,0.3 x 1000", 1, "0.5356 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayConstantVolume_TripleTo3x1_PanningZero_Ok()
        TestPlayback(New PlayerSetupConstantVolume(),
                     "0.6:0:+--; 0.5:0:-+-; 0.4:0:--+",
                     3, "1,0.8,0.4 x 1000",
                     1, "0.6 x 1000", "0.4 x 1000", "0.16 x 1000")
    End Sub


    <TestMethod>
    Public Sub PlayConstantVolume_TripleTo3x1_PanningNonZero_Ok()
        TestPlayback(New PlayerSetupConstantVolume With {.PlaybackPanning = 0.4},
                     "1:0:+--; 1:0:-+-; 1:0:--+",
                     3, "1,0.8,0.4 x 1000",
                     1, "0.7155 x 1000", "0.5724 x 1000", "0.2862 x 1000")
        TestPlayback(New PlayerSetupConstantVolume With {.PlaybackPanning = -0.4},
                     "1:0:+--; 1:0:-+-; 1:0:--+",
                     3, "1,0.8,0.4 x 1000",
                     1, "0.7155 x 1000", "0.5724 x 1000", "0.2862 x 1000")
    End Sub

#End Region

End Class
