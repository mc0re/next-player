Imports System.Collections.Specialized
Imports System.ComponentModel


''' <summary>
''' Base class for channel-based configurations.
''' </summary>
''' <typeparam name="TElem">Item type for the collection</typeparam>
''' <remarks>
''' There should be only one entry with a given channel number.
''' </remarks>
<Serializable>
Public NotInheritable Class ChannelCollection(Of TElem As IChannel)
	Inherits BindingList(Of TElem)
	Implements IChannelCollection(Of TElem)

#Region " INotifyCollectionChanged implementation "

	Public Event CollectionChanged As NotifyCollectionChangedEventHandler Implements INotifyCollectionChanged.CollectionChanged


	Protected Sub RaiseCollectionChanged()
		RaiseEvent CollectionChanged(Me, New NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))
	End Sub


	Private Sub HandleCollectionChanged() Handles MyBase.ListChanged
		RaiseCollectionChanged()
	End Sub

#End Region


#Region " Channel read-only indexer property "

	''' <summary>
	''' Get a channel configuration by channel number.
	''' </summary>
	''' <remarks>There should only be one configuration for a given channel number</remarks>
	Public ReadOnly Property Channel(channelNr As Integer) As TElem Implements IChannelCollection(Of TElem).Channel
		Get
			Return (From def In Items Where def.Channel = channelNr).SingleOrDefault()
		End Get
	End Property

#End Region


#Region " Default accessor "

	Default Public Shadows Property Item(idx As Integer) As TElem Implements IChannelCollection(Of TElem).Item
		Get
			Return MyBase.Item(idx)
		End Get
		Set(value As TElem)
			MyBase.Item(idx) = value
		End Set
	End Property

#End Region


#Region " IChannelCollection methods "

	''' <summary>
	''' In case no channels were loaded, create a default one,
	''' if it is possible.
	''' </summary>
	Public Sub AfterLoad() Implements IChannelCollection(Of TElem).AfterLoad
		If Count = 0 AndAlso GetType(TElem).GetConstructor(Type.EmptyTypes) IsNot Nothing Then
			CreateNewChannel(Of TElem)()
		End If
	End Sub


	''' <summary>
	''' Create a new channel configuration, add it to the list.
	''' </summary>
	Public Function CreateNewChannel() As TElem Implements IChannelCollection(Of TElem).CreateNewChannel
		Return CreateNewChannel(Of TElem)()
	End Function


	''' <summary>
	''' Create a new channel configuration, add it to the list.
	''' </summary>
	Public Function CreateNewChannel(Of TCh As TElem)() As TElem Implements IChannelCollection(Of TElem).CreateNewChannel
		Dim newCh = Activator.CreateInstance(Of TCh)()
		Dim chNr = 1

		While Any(Function(ch) ch.Channel = chNr)
			chNr += 1
		End While

		newCh.Channel = chNr
		Add(newCh)

		Return newCh
	End Function


	Public Shadows Sub Add(elem As TElem) Implements IChannelCollection(Of TElem).Add
		MyBase.Add(elem)
	End Sub


	''' <summary>
	''' Replace all items in this collection with clones of <paramref name="origin"/>.
	''' </summary>
	Public Sub CopyFrom(origin As IEnumerable(Of TElem)) Implements IChannelCollection(Of TElem).CopyFrom
		RaiseListChangedEvents = False
		Clear()

		For Each elem In origin
			Add(elem.Clone(Of TElem)())
		Next

		RaiseListChangedEvents = True
		OnListChanged(New ListChangedEventArgs(ListChangedType.Reset, 0))
	End Sub


	Public Sub AddIfMissing(elem As TElem) Implements IChannelCollection(Of TElem).AddIfMissing
		If Contains(elem) Then Return
		Add(elem)
	End Sub


	Public Sub RemoveChannel(channelNr As Integer) Implements IChannelCollection(Of TElem).RemoveChannel
		Dim toDel = Items.Where(Function(w) w.Channel = channelNr).ToList()

		For Each plDef In toDel
			Remove(plDef)
		Next
	End Sub

#End Region

End Class
