Imports Common

<TestCategory("Bucket list")>
<TestClass>
Public Class BucketListTest

    <TestMethod>
    Public Sub BucketList_GetFromEmpty()
        Dim sut = New BucketList(Of TestStorageItem)()
        Dim item As TestStorageItem = Nothing

        ' Need to update
        Assert.IsFalse(sut.TryGetForWidth(200, item, Function() New TestStorageItem()))
        Assert.IsNotNull(item)
        Assert.IsNull(item.Name)

        Assert.AreEqual(1L, sut.GetSize(Function(el) 1))
    End Sub


    <TestMethod>
    Public Sub BucketList_GetExactMatch()
        Dim sut = New BucketList(Of TestStorageItem)()
        sut.AddOrUpdate(200, Function(orig) New TestStorageItem With {.Name = "200px"}, Nothing)

        Dim item As TestStorageItem = Nothing

        ' No need to update
        Assert.IsTrue(sut.TryGetForWidth(200, item, Nothing))
        Assert.IsNotNull(item)
        Assert.AreEqual("200px", item.Name)

        Assert.AreEqual(1L, sut.GetSize(Function(el) 1))

        ' Make sure we got the original
        item.Name = "New 200px"
        Dim item2 As TestStorageItem = Nothing
        Assert.IsTrue(sut.TryGetForWidth(200, item2, Nothing))
        Assert.AreEqual("New 200px", item2.Name)
    End Sub


    <TestMethod>
    Public Sub BucketList_AskIncreaseSameBucket()
        Dim sut = New BucketList(Of TestStorageItem)()
        sut.AddOrUpdate(200, Function(orig) New TestStorageItem With {.Name = "200px"}, Nothing)

        Dim item As TestStorageItem = Nothing

        ' Need to update
        Assert.IsFalse(sut.TryGetForWidth(210, item, Function() New TestStorageItem()))
        Assert.IsNotNull(item)
        Assert.AreEqual("200px", item.Name)

        Assert.AreEqual(1L, sut.GetSize(Function(el) 1))

        ' Make sure we got the original to update
        item.Name = "210px"
        Dim item2 As TestStorageItem = Nothing
        Assert.IsTrue(sut.TryGetForWidth(200, item2, Nothing))
        Assert.AreEqual("210px", item2.Name)
    End Sub


    <TestMethod>
    Public Sub BucketList_AskIncreaseAnotherBucket()
        Dim sut = New BucketList(Of TestStorageItem)()
        sut.AddOrUpdate(200, Function(orig) New TestStorageItem With {.Name = "200px"}, Nothing)

        Dim item As TestStorageItem = Nothing

        ' Need to update
        Assert.IsFalse(sut.TryGetForWidth(300, item, Function(orig) New TestStorageItem With {.Name = orig?.Name}))
        Assert.IsNotNull(item)
        Assert.AreEqual("200px", item.Name)

        Assert.AreEqual(2L, sut.GetSize(Function(el) 1))

        ' Make sure we got a copy from the new bucket
        item.Name = "300px"
        Dim item2 As TestStorageItem = Nothing
        Assert.IsTrue(sut.TryGetForWidth(200, item2, Nothing))
        Assert.AreEqual("200px", item2.Name)
    End Sub


    <TestMethod>
    Public Sub BucketList_AskDecreaseSameBucket()
        Dim sut = New BucketList(Of TestStorageItem)()
        sut.AddOrUpdate(200, Function(orig) New TestStorageItem With {.Name = "200px"}, Nothing)

        Dim item As TestStorageItem = Nothing

        ' Need to update
        Assert.IsTrue(sut.TryGetForWidth(190, item, Nothing))
        Assert.IsNotNull(item)
        Assert.AreEqual("200px", item.Name)

        Assert.AreEqual(1L, sut.GetSize(Function(el) 1))

        ' Make sure we got the original to update
        item.Name = "190px"
        Dim item2 As TestStorageItem = Nothing
        Assert.IsTrue(sut.TryGetForWidth(200, item2, Nothing))
        Assert.AreEqual("190px", item2.Name)
    End Sub


    <TestMethod>
    Public Sub BucketList_AskDecreaseAnotherBucket()
        Dim sut = New BucketList(Of TestStorageItem)()
        sut.AddOrUpdate(300, Function(orig) New TestStorageItem With {.Name = "300px"}, Nothing)

        Dim item As TestStorageItem = Nothing

        ' Need to update
        Assert.IsFalse(sut.TryGetForWidth(200, item, Function(orig) New TestStorageItem With {.Name = orig?.Name}))
        Assert.IsNotNull(item)
        Assert.AreEqual("300px", item.Name)

        Assert.AreEqual(2L, sut.GetSize(Function(el) 1))

        ' Make sure we got a copy from the new bucket
        item.Name = "200px"
        Dim item2 As TestStorageItem = Nothing
        Assert.IsTrue(sut.TryGetForWidth(300, item2, Nothing))
        Assert.AreEqual("300px", item2.Name)
    End Sub

End Class
