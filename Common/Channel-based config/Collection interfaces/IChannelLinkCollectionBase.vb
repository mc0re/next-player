Imports System.Collections.Specialized


<CLSCompliant(True)>
Public Interface IChannelLinkCollectionBase
	Inherits INotifyCollectionChanged

	''' <summary>
	''' Add a new link with default settings.
	''' </summary>
	''' <param name="logChannel">Logical channel number</param>
	''' <param name="physChannel">Physical channel number</param>
	''' <param name="args">Arguments to <see cref="ChannelLink"/> constructor</param>
	''' <returns>Newly created link</returns>
	Function CreateNewLink(logChannel As Integer, physChannel As Integer, ParamArray args() As Object) As IChannelLink


	''' <summary>
	''' Get a link between the given channels.
	''' </summary>
	''' <returns>Link object or Nothing, if not found</returns>
	Function GetLink(logChannel As Integer, physChannel As Integer) As IChannelLink


	''' <summary>
	''' Delete this link.
	''' </summary>
	Sub DeleteLink(link As IChannelLink)

End Interface
