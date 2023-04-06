Imports System.Collections.Specialized
Imports System.ComponentModel


<Serializable>
<CLSCompliant(True)>
Public NotInheritable Class ChannelLinkCollection(Of TLink As IChannelLink)
	Inherits BindingList(Of TLink)
	Implements IChannelLinkCollection(Of TLink)

#Region " INotifyCollectionChanged implementation "

	Public Event CollectionChanged As NotifyCollectionChangedEventHandler Implements INotifyCollectionChanged.CollectionChanged


	Private Sub RaiseCollectionChanged()
		RaiseEvent CollectionChanged(Me, New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))
	End Sub


	Private Sub HandleCollectionChanged() Handles MyBase.ListChanged
		RaiseCollectionChanged()
	End Sub

#End Region


#Region " IChannelLinkCollectionBase implementation "

	Public Function CreateNewLink(logChannel As Integer, physChannel As Integer, ParamArray args() As Object) As IChannelLink Implements IChannelLinkCollectionBase.CreateNewLink
		Dim newLink = DirectCast(Activator.CreateInstance(GetType(TLink), args), TLink)

		newLink.Logical = logChannel
		newLink.Physical = physChannel
		Add(newLink)

		Return newLink
	End Function


	Public Function GetLink(logChannel As Integer, physChannel As Integer) As IChannelLink Implements IChannelLinkCollectionBase.GetLink
		Return Where(Function(lnk) lnk.Logical = logChannel AndAlso lnk.Physical = physChannel).FirstOrDefault()
	End Function


	Public Sub DeleteLink(link As IChannelLink) Implements IChannelLinkCollectionBase.DeleteLink
		Remove(CType(link, TLink))
	End Sub

#End Region


#Region " IChannelLinkCollection implementation "

	Public Function GetForLogical(logicalNr As Integer) As IEnumerable(Of TLink) Implements IChannelLinkCollection(Of TLink).GetForLogical
		Return Where(Function(lnk) lnk.Logical = logicalNr).ToList()
	End Function

#End Region


#Region " CopyFrom method "

	''' <summary>
	''' Replace all items in this collection with <paramref name="origin"/>.
	''' </summary>
	Public Sub CopyFrom(origin As IEnumerable(Of TLink))
		RaiseListChangedEvents = False
		Clear()

		For Each elem In origin
			Add(elem.Clone(Of TLink)())
		Next

		RaiseListChangedEvents = True
		OnListChanged(New ListChangedEventArgs(ListChangedType.Reset, 0))
	End Sub

#End Region

End Class
