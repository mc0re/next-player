Imports AudioPlayerLibrary


<TestClass>
Public Class PlaylistLibraryShould

#Region " Tests "

    <TestMethod>
    Public Sub PlaylistStructure_GetFirstAction()
        Dim list = TestPlaylistUtility.CreatePlaylist("PMP1 EME1 PMP2")

        Dim fa = PlaylistStructureLibrary.GetFirstAction(list)

        Assert.AreEqual("MP1", fa.Name)
    End Sub


    <TestMethod>
    Public Sub PlaylistStructure_GetFirstAction_FirstEffect()
        Dim list = TestPlaylistUtility.CreatePlaylist("EME1 PMP1 PMP2")

        Dim fa = PlaylistStructureLibrary.GetFirstAction(list)

        Assert.AreEqual("MP1", fa.Name)
    End Sub


    <TestMethod>
    Public Sub PlaylistStructure_GetFirstAction_WithGlobals()
        Dim list = TestPlaylistUtility.CreatePlaylist("pGP1 eGE1 PMP1 PMP2")

        Dim fa = PlaylistStructureLibrary.GetFirstAction(list)

        Assert.AreEqual("MP1", fa.Name)
    End Sub


    <TestMethod>
    Public Sub PlaylistStructure_GetGlobalParallels_Empty()
        Dim list = TestPlaylistUtility.CreatePlaylist("PMP1 ePE1 pPP1 PMP2 ePE2")

        Dim gp = PlaylistStructureLibrary.GetGlobalParallels(list)

        Assert.AreEqual(0, gp.Count)
    End Sub


    <TestMethod>
    Public Sub PlaylistStructure_GetGlobalParallels()
        Dim list = TestPlaylistUtility.CreatePlaylist("pGP1 eGE1 PMP1 ePE1 PMP2 ePE2 EME1")

        Dim gp = PlaylistStructureLibrary.GetGlobalParallels(list)

        Assert.AreEqual(2, gp.Count)
        Assert.AreEqual("GP1", gp(0).Name)
        Assert.AreEqual("GE1", gp(1).Name)
    End Sub


    <TestMethod>
    Public Sub PlaylistStructure_MainIndexing()
        Dim list = TestPlaylistUtility.CreatePlaylist("pGP1 eGE1-s PMP1 ePE1 PMP2 ePE2-s EME1")

        Dim data = PlaylistStructureLibrary.ArrangeStructure(list)

        Assert.IsNotNull(data)
        Assert.AreEqual(7, data.MaxActions)
        Assert.AreEqual(1, list(0).Index)
        Assert.AreEqual(2, list(1).Index)
        Assert.AreEqual(3, list(2).Index)
        Assert.AreEqual(4, list(3).Index)
        Assert.AreEqual(5, list(4).Index)
        Assert.AreEqual(6, list(5).Index)
        Assert.AreEqual(7, list(6).Index)
    End Sub


    <TestMethod>
    Public Sub PlaylistStructure_ParallelIndexing()
        Dim list = TestPlaylistUtility.CreatePlaylist("pGP1 eGE1-s PMP1 ePE1 PMP2 ePE2-s EME1")

        Dim data = PlaylistStructureLibrary.ArrangeStructure(list)

        Assert.IsNotNull(data)
        Assert.AreEqual(1, data.GlobalParallelCount)
        Assert.AreEqual(2, data.MaxParallels)

        Assert.AreEqual(True, list(0).IsGlobalParallel)
        Assert.AreEqual(True, list(0).HasParallelIndex)
        Assert.AreEqual(1, list(0).ParallelIndex)

        Assert.AreEqual(True, list(1).IsGlobalParallel)
        Assert.AreEqual(False, list(1).HasParallelIndex)

        Assert.AreEqual(False, list(2).IsGlobalParallel)
        Assert.AreEqual(False, list(2).HasParallelIndex)

        Assert.AreEqual(False, list(3).IsGlobalParallel)
        Assert.AreEqual(True, list(3).HasParallelIndex)
        Assert.AreEqual(2, list(3).ParallelIndex)

        Assert.AreEqual(False, list(4).IsGlobalParallel)
        Assert.AreEqual(False, list(4).HasParallelIndex)

        Assert.AreEqual(False, list(5).IsGlobalParallel)
        Assert.AreEqual(False, list(5).HasParallelIndex)

        Assert.AreEqual(False, list(6).IsGlobalParallel)
        Assert.AreEqual(False, list(6).HasParallelIndex)
    End Sub


    <TestMethod>
    Public Sub PlaylistStructure_ReferencesGlobalParallels()
        Dim list = TestPlaylistUtility.CreatePlaylist("pGP1-w eGE1-s eGE2-S pGP2-s")

        Dim data = PlaylistStructureLibrary.ArrangeStructure(list)

        Assert.IsNotNull(data)

        Assert.AreEqual(Nothing, list(0).ReferenceAction)
        Assert.AreEqual(list(0), list(1).ReferenceAction)
        Assert.AreEqual(list(0), list(2).ReferenceAction)
        Assert.AreEqual(list(2), list(3).ReferenceAction)
    End Sub


    <TestMethod>
    Public Sub PlaylistStructure_ReferencesOnMainLine()
        Dim list = TestPlaylistUtility.CreatePlaylist("pGP1 PMP1 PMP2-s EME1-s EME2-S PMP3-S")

        Dim data = PlaylistStructureLibrary.ArrangeStructure(list)

        Assert.IsNotNull(data)

        Assert.AreEqual(Nothing, list(0).ReferenceAction)
        Assert.AreEqual(Nothing, list(1).ReferenceAction)
        Assert.AreEqual(list(1), list(2).ReferenceAction)
        Assert.AreEqual(list(2), list(3).ReferenceAction)
        Assert.AreEqual(list(2), list(4).ReferenceAction)
        Assert.AreEqual(list(2), list(5).ReferenceAction)
    End Sub

#End Region

End Class
