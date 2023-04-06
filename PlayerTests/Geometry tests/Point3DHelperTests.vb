Imports Common


<TestClass>
Public Class Point3DHelperTests

    <TestMethod>
    Public Sub Point3DEqualityComparer_Trivial()
        Dim pt = New List(Of Point3D(Of String)) From {
            Point3DHelper.Create(0, 0, 0, {"A"}),
            Point3DHelper.Create(1, 1, 1, {"B"}),
            Point3DHelper.Create(0, 0, 0, {"A"}),
            Point3DHelper.Create(1, 1, 1, {"B"})
            }

        Dim unique = pt.Distinct(Point3DHelper.EqualityComparer).ToList()

        Assert.AreEqual(2, unique.Count)
    End Sub


    <TestMethod>
    Public Sub Point3DEqualityComparer_AlmostSameNumbers()
        Dim pt = New List(Of Point3D(Of String)) From {
            Point3DHelper.Create(0, 0, 0, {"A"}),
            Point3DHelper.Create(1, 1, 1, {"B"}),
            Point3DHelper.Create(0, 0, 1.5e-14, {"A"}),
            Point3DHelper.Create(1, 1, 1.00000000001, {"B"})
            }

        Dim unique = pt.Distinct(Point3DHelper.EqualityComparer).ToList()

        Assert.AreEqual(2, unique.Count)
    End Sub

End Class
