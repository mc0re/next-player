Imports AudioChannelLibrary

<TestClass>
Public Class AudioChannelLinkMappingTests

	<TestMethod>
	<TestCategory("Link mapping")>
	Public Sub LinkMapping_Create_Empty_Ok()
		Dim m As New AudioChannelLinkMapping()
		Assert.AreEqual(0, m.NofChannels)
		Assert.AreEqual(0, m.MappingList.Count)
		Assert.AreEqual(0.0F, m.Panning)
	End Sub


	<TestMethod>
	<TestCategory("Link mapping")>
	Public Sub LinkMapping_Create_1x1_Ok()
		Dim m As New AudioChannelLinkMapping(1, 0, 1)
		Assert.AreEqual(1, m.NofChannels)
		Assert.AreEqual(1, m.MappingList.Count)
		Assert.IsTrue(m.MappingList(0).IsSet)
		Assert.AreEqual(0.0F, m.Panning)
	End Sub


	<TestMethod>
	<TestCategory("Link mapping")>
	Public Sub LinkMapping_Create_2x1_Ok()
		Dim m As New AudioChannelLinkMapping(2, 0, 1)
		Assert.AreEqual(2, m.NofChannels)
		Assert.AreEqual(2, m.MappingList.Count)
		Assert.IsTrue(m.MappingList(0).IsSet)
		Assert.IsTrue(m.MappingList(1).IsSet)
		Assert.AreEqual(0.0F, m.Panning)
	End Sub


	<TestMethod>
	<TestCategory("Link mapping")>
	Public Sub LinkMapping_Create_2x2_Ok()
		Dim m As New AudioChannelLinkMapping(2, 0, 2)
		Assert.AreEqual(2, m.NofChannels)
		Assert.AreEqual(2, m.MappingList.Count)
		Assert.IsTrue(m.MappingList(0).IsSet)
		Assert.IsTrue(m.MappingList(1).IsSet)
		Assert.AreEqual(-1.0F, m.Panning)

		m = New AudioChannelLinkMapping(2, 1, 2)
		Assert.AreEqual(2, m.NofChannels)
		Assert.AreEqual(2, m.MappingList.Count)
		Assert.IsTrue(m.MappingList(0).IsSet)
		Assert.IsTrue(m.MappingList(1).IsSet)
		Assert.AreEqual(1.0F, m.Panning)
	End Sub

End Class
