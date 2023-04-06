<CLSCompliant(True)>
Public MustInherit Class LogicalChannelBase
	Inherits ChannelBase
	Implements ILogicalChannel

#Region " Get physical channels "

	''' <summary>
	''' Get all links and related physical channels,
	''' including the disabled ones (because they could be enabled during the playback).
	''' </summary>
	Public Function GetPhysicalChannels(Of TPhys As IPhysicalChannel, TLink As IChannelLink, TEnv As IChannelEnvironmentStorage(Of TPhys, TLink))(
	) As IList(Of LinkInfo(Of TPhys, TLink)) Implements ILogicalChannel.GetPhysicalChannels
		Dim config = InterfaceMapper.GetImplementation(Of TEnv)()
        Dim links = config.Links.GetForLogical(Channel)

        Dim phys = (
			From l In links
			Let p = config.Physical.Channel(l.Physical)
			Where p IsNot Nothing
			Select New LinkInfo(Of TPhys, TLink) With {.Physical = p, .Link = l}
			).ToList()

		Return phys
	End Function

#End Region

End Class
