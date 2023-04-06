Imports System.Collections.Specialized


''' <summary>
''' A collection of <see cref="IChannel"/> elements - base interface.
''' </summary>
''' <remarks>
''' There should be only one entry with a given channel number.
''' 
''' Inherits <see cref="ICollection"/> to avoid generics in XAML.
''' </remarks>
<CLSCompliant(True)>
Public Interface IChannelCollection(Of TElem As IChannel)
	Inherits ICollection, IEnumerable(Of TElem), INotifyCollectionChanged

#Region " Properties "

	''' <summary>
	''' Retrieve an element by channel number.
	''' </summary>
	ReadOnly Property Channel(channelNr As Integer) As TElem


	''' <summary>
	''' Needed for serialization.
	''' </summary>
	Default Property Item(idx As Integer) As TElem

#End Region


#Region " Collection API "

	''' <summary>
	''' Make sure there is at least one channel.
	''' </summary>
	Sub AfterLoad()


	''' <summary>
	''' Create a new channel configuration of type <typeparamref name="TElem"/>,
	''' add it to the list.
	''' </summary>
	''' <remarks>Channel number is "first free"</remarks>
	Function CreateNewChannel() As TElem


	''' <summary>
	''' Create a new channel configuration of the given type <typeparamref name="TCh"/>,
	''' add it to the list.
	''' </summary>
	''' <remarks>Channel number is "first free"</remarks>
	Function CreateNewChannel(Of TCh As TElem)() As TElem


	''' <summary>
	''' Needed for serialization.
	''' </summary>
	Sub Add(elem As TElem)


	''' <summary>
	''' Replace all items in this collection with <paramref name="origin"/>.
	''' </summary>
	''' <remarks>
	''' The items are cloned.
	''' 
	''' If there is more than one channel with the same channel number,
	''' raises a <see cref="DuplicateChannelNumberException"/> exception
	''' and proceeds with import.
	''' </remarks>
	Sub CopyFrom(origin As IEnumerable(Of TElem))


	''' <summary>
	''' If already exists, do nothing.
	''' </summary>
	Sub AddIfMissing(elem As TElem)


	''' <summary>
	''' Remove from playlist.
	''' </summary>
	Sub RemoveChannel(channelNr As Integer)

#End Region

End Interface
