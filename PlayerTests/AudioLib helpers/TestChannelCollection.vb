Imports System.Collections.Specialized
Imports Common


Public Class TestChannelCollection(Of TElem As IChannel)
	Inherits List(Of TElem)
	Implements IChannelCollection(Of TElem)

#Region " Events "

	Public Event CollectionChanged As NotifyCollectionChangedEventHandler Implements INotifyCollectionChanged.CollectionChanged

#End Region


#Region " Properties "

	Public ReadOnly Property Channel(channelNr As Integer) As TElem Implements IChannelCollection(Of TElem).Channel
		Get
			Return (From c In Me Where c.Channel = channelNr).FirstOrDefault()
		End Get
	End Property


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


	Public Shadows ReadOnly Property Count As Integer Implements ICollection.Count
		Get
			Throw New NotImplementedException()
		End Get
	End Property


	Public ReadOnly Property IsSynchronized As Boolean Implements ICollection.IsSynchronized
		Get
			Throw New NotImplementedException()
		End Get
	End Property


	Public ReadOnly Property SyncRoot As Object Implements ICollection.SyncRoot
		Get
			Throw New NotImplementedException()
		End Get
	End Property

#End Region

	Public Shadows Sub Add(elem As TElem) Implements IChannelCollection(Of TElem).Add
		MyBase.Add(elem)
	End Sub


	Public Sub AddIfMissing(elem As TElem) Implements IChannelCollection(Of TElem).AddIfMissing
		Throw New NotImplementedException()
	End Sub


	Public Sub AfterLoad() Implements IChannelCollection(Of TElem).AfterLoad
		Throw New NotImplementedException()
	End Sub


	Public Sub CopyFrom(origin As IEnumerable(Of TElem)) Implements IChannelCollection(Of TElem).CopyFrom
		Throw New NotImplementedException()
	End Sub


	Public Shadows Sub CopyTo(array As Array, index As Integer) Implements ICollection.CopyTo
		Throw New NotImplementedException()
	End Sub


	Public Sub RemoveChannel(channelNr As Integer) Implements IChannelCollection(Of TElem).RemoveChannel
		Throw New NotImplementedException()
	End Sub


	Public Function CreateNewChannel() As TElem Implements IChannelCollection(Of TElem).CreateNewChannel
		Dim elem = Activator.CreateInstance(Of TElem)()
		Add(elem)

		Return elem
	End Function


	Public Function CreateNewChannel(Of TCh As TElem)() As TElem Implements IChannelCollection(Of TElem).CreateNewChannel
		Throw New NotImplementedException()
	End Function

End Class
