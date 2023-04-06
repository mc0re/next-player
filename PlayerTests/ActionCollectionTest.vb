Imports PlayerActions


<TestClass>
Public Class ActionCollectionTest

    <TestMethod()>
    <TestCategory("Playlist handling")>
    Public Sub ActionCollection_Arrange_IndexOk()
        Dim coll As New PlayerActionCollection()
        Dim m1 As New PlayerActionFile With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.MainStopAll}
        Dim m2 As New PlayerActionFile With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.Parallel}
        Dim m3 As New PlayerActionFile With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.MainContinuePrev}

        coll.Items.Add(m1)
        coll.Items.Add(m2)
        coll.Items.Add(m3)

        Assert.AreEqual(1, m1.Index)
        Assert.AreEqual(2, m2.Index)
        Assert.AreEqual(3, m3.Index)

        m1.Index = 0
        m2.Index = 0
        m3.Index = 0
        m2.IsEnabled = False

        Assert.AreEqual(1, m1.Index)
        Assert.AreEqual(2, m2.Index)
        Assert.AreEqual(3, m3.Index)
    End Sub


    <TestMethod()>
    <TestCategory("Playlist handling")>
    Public Sub ActionCollection_Arrange_ParallelIndexOk()
        Dim coll As New PlayerActionCollection()
        Dim com As New PlayerActionComment()
        Dim mg As New PlayerActionFile With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.Parallel}
        Dim m1 As New PlayerActionFile With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.MainStopAll}
        Dim m2 As New PlayerActionFile With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.Parallel}
        Dim m3 As New PlayerActionFile With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.Parallel}
        Dim m4 As New PlayerActionFile With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.MainContinuePrev}

        coll.Items.Add(com)
        coll.Items.Add(mg)
        coll.Items.Add(m1)
        coll.Items.Add(m2)
        coll.Items.Add(m3)
        coll.Items.Add(m4)

        Assert.AreNotEqual(1, mg.Index)
        Assert.AreEqual(1, mg.ParallelIndex)
        Assert.AreEqual(2, m2.ParallelIndex)
        Assert.AreEqual(3, m3.ParallelIndex)
    End Sub


    <TestMethod()>
    <TestCategory("Playlist handling")>
    Public Sub ActionCollection_Arrange_EffectTargetOk()
        Dim coll As New PlayerActionCollection()

        ' Global parallels
        Dim ge1 As New PlayerActionEffect With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.Parallel, .EffectTargetMain = AudioPlayerLibrary.EffectTargets.All, .EffectTargetParallel = AudioPlayerLibrary.EffectTargets.None, .Name = "Global effect Main"}
        Dim ge2 As New PlayerActionEffect With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.Parallel, .EffectTargetParallel = AudioPlayerLibrary.EffectTargets.All, .Name = "Global effect Par"}
        Dim gf1 As New PlayerActionFile With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.Parallel, .Name = "Global file"}

        ' Main line, part 1
        Dim mf0 As New PlayerActionFile With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.MainStopAll, .Name = "Main file 0"}
        Dim mf1 As New PlayerActionFile With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.MainStopAll, .Name = "Main file 1"}
        Dim mf2 As New PlayerActionFile With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.MainContinuePrev, .Name = "Main file 2"}
        Dim me1 As New PlayerActionEffect With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.MainContinuePrev, .EffectTargetMain = AudioPlayerLibrary.EffectTargets.All, .EffectTargetParallel = AudioPlayerLibrary.EffectTargets.None, .Name = "Main effect All"}
        Dim me2 As New PlayerActionEffect With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.MainContinuePrev, .EffectTargetMain = AudioPlayerLibrary.EffectTargets.Last, .EffectTargetParallel = AudioPlayerLibrary.EffectTargets.None, .Name = "Main effect Last"}

        ' Parallels
        Dim pf1 As New PlayerActionFile With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.Parallel, .Name = "Par file 1"}
        Dim pf2 As New PlayerActionFile With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.Parallel, .Name = "Par file 2"}
        Dim pe1 As New PlayerActionEffect With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.Parallel, .EffectTargetMain = AudioPlayerLibrary.EffectTargets.Last, .EffectTargetParallel = AudioPlayerLibrary.EffectTargets.None, .Name = "Par effect MainLast"}
        Dim pe2 As New PlayerActionEffect With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.Parallel, .EffectTargetMain = AudioPlayerLibrary.EffectTargets.All, .EffectTargetParallel = AudioPlayerLibrary.EffectTargets.None, .Name = "Par effect MainAll"}
        Dim pe3 As New PlayerActionEffect With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.Parallel, .EffectTargetMain = AudioPlayerLibrary.EffectTargets.None, .EffectTargetParallel = AudioPlayerLibrary.EffectTargets.Last, .Name = "Par effect ParLast"}
        Dim pe4 As New PlayerActionEffect With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.Parallel, .EffectTargetMain = AudioPlayerLibrary.EffectTargets.None, .EffectTargetParallel = AudioPlayerLibrary.EffectTargets.All, .Name = "Par effect ParAll"}

        ' Main line, part 2
        Dim me3 As New PlayerActionEffect With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.MainContinuePrev, .EffectTargetMain = AudioPlayerLibrary.EffectTargets.Last, .EffectTargetParallel = AudioPlayerLibrary.EffectTargets.Last, .Name = "Main effect MainParLast"}
        Dim me4 As New PlayerActionEffect With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.MainContinuePrev, .EffectTargetMain = AudioPlayerLibrary.EffectTargets.All, .EffectTargetParallel = AudioPlayerLibrary.EffectTargets.All, .Name = "Main effect MainParAll"}
        Dim mf3 As New PlayerActionFile With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.MainContinuePrev, .Name = "Main file 3"}
        Dim me5 As New PlayerActionEffect With {.ExecutionType = AudioPlayerLibrary.ExecutionTypes.MainContinuePrev, .EffectTargetMain = AudioPlayerLibrary.EffectTargets.All, .EffectTargetParallel = AudioPlayerLibrary.EffectTargets.All, .Name = "Main effect MainParAll2"}

        coll.Items.Add(ge1) ' | -ge1
        coll.Items.Add(ge2) ' | -ge2
        coll.Items.Add(gf1) ' | -gf1
        coll.Items.Add(mf0) ' -mf0
        coll.Items.Add(mf1) ' -mf1
        coll.Items.Add(mf2) ' -mf2
        coll.Items.Add(me1) ' -me1
        coll.Items.Add(me2) ' -me2
        coll.Items.Add(pf1) ' | -pf1
        coll.Items.Add(pf2) ' | -pf2
        coll.Items.Add(pe1) ' | -pe1
        coll.Items.Add(pe2) ' | -pe2
        coll.Items.Add(pe3) ' | -pe3
        coll.Items.Add(pe4) ' | -pe4
        coll.Items.Add(me3) ' -me3
        coll.Items.Add(me4) ' -me4
        coll.Items.Add(mf3) ' -mf3
        coll.Items.Add(me5) ' -me5

        ' Check globals
        Assert.AreEqual(0, ge1.TargetList.Count)
        Assert.AreEqual(0, ge2.TargetList.Count)

        ' Check main line 1
        Assert.AreEqual(2, me1.TargetList.Count)
        Assert.AreSame(mf1, me1.TargetList(0))
        Assert.AreSame(mf2, me1.TargetList(1))
        Assert.AreEqual(1, me2.TargetList.Count)
        Assert.AreSame(mf2, me2.TargetList(0))

        ' Check parallel line
        Assert.AreEqual(1, pe1.TargetList.Count)
        Assert.AreSame(mf2, pe1.TargetList(0))
        Assert.AreEqual(2, pe2.TargetList.Count)
        Assert.AreSame(mf1, pe2.TargetList(0))
        Assert.AreSame(mf2, pe2.TargetList(1))
        Assert.AreEqual(1, pe3.TargetList.Count)
        Assert.AreSame(pf2, pe3.TargetList(0))
        Assert.AreEqual(2, pe4.TargetList.Count)
        Assert.AreSame(pf1, pe4.TargetList(0))
        Assert.AreSame(pf2, pe4.TargetList(1))

        ' Check main line 2.
        Assert.AreEqual(2, me3.TargetList.Count)
        Assert.AreSame(mf2, me3.TargetList(0))
        Assert.AreSame(pf2, me3.TargetList(1))
    End Sub

End Class
