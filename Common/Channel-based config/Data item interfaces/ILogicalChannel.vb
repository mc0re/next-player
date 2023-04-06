''' <summary>
''' Logical channel description.
''' </summary>
''' <remarks>
''' Logical channels are stored generally in the playlist.
''' 
''' There is no practical point of having this interface
''' other than structural separation between logical and physical channels.
''' </remarks>
<CLSCompliant(True)>
Public Interface ILogicalChannel
	Inherits IChannel

	Function GetPhysicalChannels(Of TPhys As IPhysicalChannel,
									 TLink As IChannelLink,
									 TEnv As IChannelEnvironmentStorage(Of TPhys, TLink))(
	) As IList(Of LinkInfo(Of TPhys, TLink))

End Interface
