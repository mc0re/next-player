Imports Common


''' <summary>
''' A collection of methods for calculating channel coefficients.
''' </summary>
Public Module SoundPositionUtility

#Region " Constants "

    ''' <summary>
    ''' Speed of sound in air at room temperature [m/s]
    ''' </summary>
    Public Const SpeedOfSound As Single = 343


    ''' <summary>
    ''' The distance [meter] at which sound with volume = 1 has correct loudness.
    ''' If the desired distance is less than nominal, the volume is NOT increased.
    ''' </summary>
    Public Const NominalDistance As Single = 1.0F


    Private ReadOnly RelativeCoordPrecision As Single


    Public ReadOnly RelativeVolumePrecision As Single

#End Region


#Region " Init and clean-up "

    Sub New()
        Dim config = InterfaceMapper.GetImplementation(Of IVolumeConfiguration)()
        RelativeCoordPrecision = config.BalancePrecision
        RelativeVolumePrecision = config.VolumePrecision
    End Sub

#End Region


#Region " Panning model "

    ''' <summary>
    ''' Get panned channel influence, according to the model.
    ''' </summary>
    ''' <param name="relDist">
    ''' Relative distance between the single-point and one of the multi-point channels, 0..1
    ''' </param>
    ''' <returns>Value 0..1</returns>
    ''' <remarks>
    ''' It should always be that model(0) = 1.
    ''' Usually model(-1) = model(1) = 0.
    ''' The function must be smooth in the definition interval.
    ''' </remarks>
    Public Function GetModelValue(model As PanningModels, relDist As Single) As Single
        If relDist < 0 OrElse relDist > 1 Then Return 0

        Dim cf As Single

        Select Case model
            Case PanningModels.Fixed
                cf = 1

            Case PanningModels.ConstantVolume
                ' This is linear panning.
                cf = 1 - relDist

            Case PanningModels.ConstantPower
                ' This is square-law panning,
                ' correct for 2 outputs, less so for more.
                cf = CSng(Math.Sqrt(1.0F - relDist))

            Case PanningModels.Angular
                ' This is sine-law panning.
                cf = CSng(Math.Cos(relDist * Math.PI / 2))

            Case Else
                Throw New ArgumentException("Unsupported panning model")
        End Select

        Return cf
    End Function


    ''' <summary>
    ''' Get the maximum possible sum of coefficients for the given model
    ''' and the given number of channels.
    ''' </summary>
    ''' <remarks>
    ''' There are <paramref name="nofCh"/> channels, evenly distributed
    ''' in range [-1..1]. Consider all pannings (see <see cref="GetModelValue"/>)
    ''' from 0 to 1, and for the given model find out the maximum of the sum
    ''' of the model values for all channels.
    ''' </remarks>
    Private Function GetMaxModelValue(model As PanningModels, nofCh As Integer) As Single
        If nofCh <= 1 Then Return 1

        Dim cf As Single

        Select Case model
            Case PanningModels.Fixed
                cf = nofCh

            Case PanningModels.ConstantVolume
                cf = If(
                    nofCh Mod 2 = 0,
                    nofCh - (nofCh + 1.0F) / 4,
                    nofCh - nofCh * nofCh / (nofCh - 1.0F) / nofCh)

            Case PanningModels.ConstantPower
                ' Approximation
                cf = If(nofCh <= 2, 1, 0.642F * nofCh)

            Case PanningModels.Angular
                ' Rough approximation
                cf = nofCh - 0.5F

            Case Else
                Throw New ArgumentException("Unsupported panning model")
        End Select

        Return cf
    End Function

#End Region


#Region " Utility "

    ''' <summary>
    ''' Count enabled channels.
    ''' </summary>
    Public Function CountEnabledChannels(map As AudioChannelLinkMapping, allDisabled As Boolean) As EnabledChannelsCollection
        Dim res As New EnabledChannelsCollection() With {
            .SourceCount = map.MappingList.Count,
            .IsEnabled = (From m In map.MappingList Select If(allDisabled, False, m.IsSet)).ToList()
        }

        ' It's always faster with indices than with a Linq query calling lambda functions.
        Dim enabledSrcCount = 0

        For srcIdx = 0 To res.SourceCount - 1
            If res.IsEnabled(srcIdx) Then
                enabledSrcCount += 1
            End If
        Next

        res.EnabledCount = enabledSrcCount

        Return res
    End Function


    ''' <summary>
    ''' Calculate distances between the enabled source channels
    ''' and the single mono channel, offset by <paramref name="srcPanning"/>.
    ''' </summary>
    ''' <param name="srcPanning">
    ''' Location of the source on the line, where the source channels are distributed.
    ''' </param>
    ''' <param name="normalize">
    ''' If True, make sure the sum of all coefficients is never above 1
    ''' </param>
    ''' <returns>An array of distances, 0 where disabled</returns>
    ''' <remarks>
    ''' The channels in the source data are evenly distributed
    ''' within a line, centered at 0 and having a length of 1.
    ''' 
    ''' The outer channels are thus placed at -0.5 and 0.5;
    ''' if there is only one channel, it's location is 0.
    ''' 
    ''' Source panning is only applicable if there is more than one
    ''' enabled source channel.
    ''' </remarks>
    Public Function CalculatePanningCoefficients(
        model As PanningModels,
        srcPanning As Single,
        chInfo As EnabledChannelsCollection,
        normalize As Boolean
    ) As Single()
        Dim res = New Single(chInfo.SourceCount - 1) {}
        Dim enabledSrcIdx = 0
        Dim maxValue = If(normalize, GetMaxModelValue(model, chInfo.EnabledCount), 1)

        ' If there is only one channel, its location is 0
        If chInfo.EnabledCount = 1 Then
            srcPanning = 0
        End If

        For srcIdx = 0 To chInfo.SourceCount - 1
            If Not chInfo.IsEnabled(srcIdx) Then Continue For

            ' A value in [-0.5..0.5], 0 if only 1 input channel is enabled
            Dim srcChPos = If(chInfo.EnabledCount = 1, 0,
                              -1.0F / 2 + enabledSrcIdx / (chInfo.EnabledCount - 1.0F))

            ' Distance between the given sound source and the current sound sink
            Dim dist = Math.Abs(srcChPos - srcPanning)

            Dim cf As Single

            If dist < RelativeCoordPrecision Then
                cf = 1
            ElseIf dist > 1.0F - RelativeCoordPrecision Then
                cf = 0
            Else
                cf = GetModelValue(model, dist)
            End If

            res(srcIdx) = cf / maxValue

            enabledSrcIdx += 1
        Next

        Return res
    End Function


    ''' <summary>
    ''' Multiply the given arrays, then multiply by <paramref name="volume"/>.
    ''' </summary>
    Public Function MultiplyCoef(
        chInfo As EnabledChannelsCollection, volume As Single,
        ParamArray srcToCh As Single()()
    ) As Single()
        Dim res = New Single(chInfo.SourceCount - 1) {}

        For srcIdx = 0 To chInfo.SourceCount - 1
            If Not chInfo.IsEnabled(srcIdx) Then Continue For

            Dim mult = volume

            For Each arr In srcToCh
                mult *= arr(srcIdx)
            Next

            res(srcIdx) = mult
        Next

        Return res
    End Function


    ''' <summary>
    ''' Trim the value to be within the given boundaries.
    ''' </summary>
    Public Function Trim(value As Double, min As Double, max As Double) As Double
        If value < min Then
            Return min
        ElseIf value > max Then
            Return max
        Else
            Return value
        End If
    End Function


    ''' <summary>
    ''' Adjust volume of the sample and eventually clip it.
    ''' </summary>
    Public Function TrimSample(sample As Single) As Single
        If sample < -1 Then
            sample = -1
        ElseIf sample > 1 Then
            sample = 1
        End If

        Return sample
    End Function


    ''' <summary>
    ''' Create coefficients for a physical channel and the corresponding link.
    ''' </summary>
    ''' <remarks>
    ''' Panning is calculated based on the X-coordinate of a speaker,
    ''' where the left-most is -1, right-most - +1.
    ''' 
    ''' If <see cref="SourcePanningInfo.IsEnabled"/> is False,
    ''' the array of coefficients is not created.
    ''' </remarks>
    Public Function CreatePanningInfo(Of TPosition)(
        nofSourceChannels As Integer,
        lnk As LinkResult(Of AudioChannelLink, AudioPhysicalChannel),
        position As TPosition
    ) As SourcePanningInfo(Of TPosition)
        Dim link = lnk.Link
        Dim physical = lnk.Physical

        Dim info As New SourcePanningInfo(Of TPosition) With {
            .PhysicalNr = physical.Channel,
            .IsEnabled = link.IsEnabled AndAlso physical.IsEnabled,
            .SpeakerPosition = position
        }

        Dim map = link.GetOrCreateMapping(nofSourceChannels)
        info.EnabledMap = CountEnabledChannels(map, Not info.IsEnabled)

        If info.EnabledMap.EnabledCount = 0 Then
            ' None enabled, act as if the link is disabled
            info.IsEnabled = False
        End If

        If Not info.IsEnabled Then
            Return info
        End If

        Dim volume = link.Volume * physical.Volume
        If link.ReversedPhase Xor physical.ReversedPhase Then
            volume = -volume
        End If

        ' The link's panning defines the source mono point location.
        ' Normalize to make sure the end value is <= 1.
        info.SourceToMono =
            MultiplyCoef(info.EnabledMap, volume,
            CalculatePanningCoefficients(map.PanningModel, map.Panning / 2, info.EnabledMap, True))

        info.Delay = link.Delay + physical.Delay

        Return info
    End Function

#End Region

End Module
