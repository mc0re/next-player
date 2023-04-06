Imports Common
Imports RoomDivisionLibrary


Public Class PositionCoefficientGeneratorFactory
    Implements ICoefficientGeneratorFactory

#Region " Fields "

    ''' <summary>
    ''' Keeps the room subdivision.
    ''' </summary>
    Private ReadOnly mLayouter As I3DLayouter(Of AudioPhysicalChannel)


    ''' <summary>
    ''' Keep the precalculated coefficients for all physical channels.
    ''' </summary>
    Private ReadOnly mDestInfo As New SourcePanning3DCollection()

#End Region


#Region " Init and clean-up "

    Public Sub New(logicalChNr As Integer, audioConfig As IAudioEnvironmentStorage, nofSourceChannels As Integer)
        mLayouter = audioConfig.GetLayouter(logicalChNr)

        Dim links = audioConfig.GetLinks(logicalChNr)
        Dim enabled = links.Where(Function(l) l.Link.IsEnabled)

        For Each lnk In enabled
            Dim info = CreatePanningInfo(nofSourceChannels, lnk, Point3DHelper.Create(lnk.Physical))
            mDestInfo.Add(info)
        Next
    End Sub

#End Region


#Region " ICoefficientGeneratorFactory implementation "

    Public Function Create(playback As AudioPlaybackInfo) As ICoefficientGenerator Implements ICoefficientGeneratorFactory.Create
        Return New PositionCoefficientGenerator(playback, playback, mLayouter, mDestInfo)
    End Function

#End Region

End Class
