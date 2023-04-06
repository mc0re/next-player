''' <summary>
''' This interface combines all functionality for storing
''' environment-base channel configurations.
''' </summary>
<CLSCompliant(True)>
Public Interface IChannelEnvironmentStorage(Of TPhys As IPhysicalChannel, TLink As IChannelLink)
    Inherits ILinkStorageBase

#Region " Properties "

    ''' <summary>
    ''' A list of physical channels.
    ''' </summary>
    ReadOnly Property Physical As IChannelCollection(Of TPhys)


    ''' <summary>
    ''' A collection of link objects.
    ''' </summary>
    ''' <returns></returns>
    Shadows ReadOnly Property Links As ChannelLinkCollection(Of TLink)

#End Region


#Region " Methods "

    ''' <summary>
    ''' Sanity check after loading.
    ''' </summary>
    Sub AfterLoad()


    ''' <summary>
    ''' Get a list of links and connected channels for the given logical channel.
    ''' </summary>
    ''' <remarks>
    ''' All links are returned, also disabled - in case they are enabled during playback.
    ''' </remarks>
    Function GetLinks(logicalNr As Integer) As IReadOnlyCollection(Of LinkResult(Of TLink, TPhys))

#End Region

End Interface
