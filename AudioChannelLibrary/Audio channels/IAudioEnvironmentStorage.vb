Imports Common
Imports RoomDivisionLibrary


Public Interface IAudioEnvironmentStorage
    Inherits IChannelEnvironmentStorage(Of AudioPhysicalChannel, AudioChannelLink)

    ''' <summary>
    ''' Stop all test sounds.
    ''' </summary>
    Sub StopAllTests()


    ''' <summary>
    ''' Get room information for the given logical channel.
    ''' </summary>
    Function GetLayouter(logCh As Integer) As I3DLayouter(Of AudioPhysicalChannel)

End Interface
