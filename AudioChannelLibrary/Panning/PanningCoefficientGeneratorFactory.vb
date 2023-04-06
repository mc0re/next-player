Imports Common


''' <summary>
''' Get the proper coefficient generator for the given logical channel
''' based on the panning model.
''' </summary>
Public Class PanningCoefficientGeneratorFactory
    Implements ICoefficientGeneratorFactory

#Region " Fields "

    ''' <summary>
    ''' Keep the precalculated coefficients for all physical channels.
    ''' </summary>
    Private ReadOnly mDestInfo As New List(Of SourcePanningInfo(Of Single))()

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Prepare whatever we can to speed up further calculations.
    ''' </summary>
    Public Sub New(logicalChNr As Integer, audioConfig As IAudioEnvironmentStorage, nofSourceChannels As Integer)
        Dim links = audioConfig.GetLinks(logicalChNr)
        If Not links.Any() Then Return

        Dim enabled = links.Where(Function(l) l.Link.IsEnabled).ToList()
        If Not enabled.Any() Then Return

        Dim xList = enabled.Select(Function(l) l.Physical.X).ToList()
        Dim xMin = xList.Min()
        Dim xMax = xList.Max()

        For Each lnk In enabled
            ' If there is only one output, it's at 0.
            ' Otherwise, the outputs are distributed within -0.5..0.5 range.
            Dim pos = If(IsEqual(xMin, xMax), 0,
                CSng((lnk.Physical.X - xMin) / (xMax - xMin) - 0.5))
            Dim info = CreatePanningInfo(nofSourceChannels, lnk, pos)

            mDestInfo.Add(info)
        Next
    End Sub

#End Region


#Region " ICoefficientGenerator implementation "

    ''' <inheritdoc/>
    Public Function Create(playback As AudioPlaybackInfo) As ICoefficientGenerator Implements ICoefficientGeneratorFactory.Create
        Return New PanningCoefficientGenerator(playback, playback, mDestInfo)
    End Function

#End Region

End Class
