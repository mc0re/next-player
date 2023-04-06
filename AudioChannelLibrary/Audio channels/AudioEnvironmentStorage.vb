Imports System.Collections.Concurrent
Imports Common
Imports RoomDivisionLibrary


<Serializable>
<CLSCompliant(True)>
Public Class AudioEnvironmentStorage
    Inherits ChannelEnvironmentStorage(Of AudioPhysicalChannel, AudioChannelLink)
    Implements IAudioEnvironmentStorage

#Region " Fields "

    Private mIsLoaded As Boolean


    Private ReadOnly mLayoutters As New ConcurrentDictionary(Of Integer, I3DLayouter(Of AudioPhysicalChannel))()

#End Region


#Region " General API "

    ''' <summary>
    ''' If no physical channels present, create default setup.
    ''' </summary>
    Public Overrides Sub AfterLoad()
        If Physical.Count = 0 Then
            Dim firstCh = Physical.CreateNewChannel()
            firstCh.Description = "Default device"

            Dim nofDeviceChannels = firstCh.AudioInterface.Channels

            ' First position and distance between channels in default location
            Dim xFirst = If(nofDeviceChannels = 1, 0, -1)
            Dim xDist = If(nofDeviceChannels = 1, 1, 2.0F / (nofDeviceChannels - 1))
            firstCh.X = xFirst

            ' The first channel (#0) is already created, therefore from 1
            For extraCh = 1 To nofDeviceChannels - 1
                Dim nextCh = Physical.CreateNewChannel()
                nextCh.Description = firstCh.Description
                nextCh.DeviceChannel = firstCh.DeviceChannel + extraCh
                nextCh.X = xFirst + xDist * extraCh
            Next

            Links.Clear()
        End If

        If Links.Count = 0 Then
            Dim physCount = Physical.Count

            For Each ch In Physical
                Links.CreateNewLink(1, ch.Channel, ch.Channel - 1, physCount)
            Next
        End If

        For Each lnk In Links
            lnk.AfterLoad()
        Next

        mIsLoaded = True
    End Sub


    ''' <summary>
    ''' Stop all test sounds.
    ''' </summary>
    Public Sub StopAllTests() Implements IAudioEnvironmentStorage.StopAllTests
        For Each ph In Physical
            ph.StopTestSound()
        Next
    End Sub

#End Region


#Region " Room API "

    ''' <summary>
    ''' Pre-calculate 3D parameters to speed up later usage during playback.
    ''' </summary>
    ''' <param name="room">Room characteristic</param>
    ''' <remarks>
    ''' <see cref="AfterLoad"/> shall be called before this method.
    ''' 
    ''' Based on the list of physical channels and their coordinates in <see cref="IAudioEnvironmentStorage.Physical"/>,
    ''' calculate for the whole room parameters to be used, when the list of links
    ''' will be requested for a particular position.
    ''' 
    ''' Calculation is done for each logical channel.
    ''' </remarks>
    Public Sub SetRoom(room As Room3D)
        ' Not ready yet, will be called later
        If room Is Nothing Then Return
        If Not mIsLoaded Then Return

        Dim logList = (From l In Links Select l.Logical Distinct).ToList()

        For Each logCh In logList
            Dim logChNr = logCh
            Dim layoutter = mLayoutters.GetOrAdd(logChNr, Function(ch) New Room3DFlattenedLayouter(Of AudioPhysicalChannel)())
            Dim spkList = (
                From ch In Links.GetForLogical(logChNr)
                Let ph = Physical.Channel(ch.Physical)
                Where ph IsNot Nothing
                Select Point3DHelper.Create(ph.X, ph.Y, ph.Z, {ph})
                ).ToList()

            layoutter.PrepareLayout(room, spkList)
        Next
    End Sub


    ''' <summary>
    ''' Get room layout engine for the given logical channel.
    ''' </summary>
    Public Function GetLayouter(logCh As Integer) As I3DLayouter(Of AudioPhysicalChannel) Implements IAudioEnvironmentStorage.GetLayouter
        Dim res As I3DLayouter(Of AudioPhysicalChannel) = Nothing
        Dim found = mLayoutters.TryGetValue(logCh, res)

        If Not found Then
            Throw New ArgumentException($"Cannot find layout for logical channel {logCh}.")
        End If

        Return res
    End Function

#End Region

End Class
