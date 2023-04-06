''' <summary>
''' Physical channel description.
''' </summary>
''' <remarks>
''' Physical channels are stored in the machine-specific part
''' of the playlist.
''' 
''' There is no practical point of having this interface
''' other than structural separation between logical and physical channels.
''' </remarks>
<CLSCompliant(True)>
Public Interface IPhysicalChannel
	Inherits IChannel

End Interface
