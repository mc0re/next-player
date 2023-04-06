Public Class TestAssert

    Public Shared Sub AreEqual(Of T)(expected As IEnumerable(Of T), actual As IEnumerable(Of T))
        Dim expList = expected.ToList()
        Dim actList = actual.ToList()

        Assert.AreEqual(expList.Count, actList.Count, "Size of the collections")

        For i = 0 To expList.Count-1
            Assert.AreEqual(expList(i), actList(i), $"Element #{i}")
        Next
    End Sub

End Class
