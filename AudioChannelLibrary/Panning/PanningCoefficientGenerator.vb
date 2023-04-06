Imports Common


''' <summary>
''' Helper class that generates channel coefficients,
''' taking panning and position into account.
''' </summary>
''' <remarks>
''' The coefficients provide factors for mapping between the source channels
''' and the destination channel.
''' 
''' All source channels, where the mapping is set, are taken and evenly
''' distributed in -1..1. If there is only one channel, it gets position 0.
''' 
''' The coefficients are to be used directly on the source samples
''' for the sake of speed, thereforethe volume is also included.
''' </remarks>
Public Class PanningCoefficientGenerator
    Implements ICoefficientGenerator

#Region " Constants "

    Private Shared ReadOnly sEmptyCollection As New ChannelModifierCollection()

#End Region


#Region " Fields "

    Private ReadOnly mModifiers As New Dictionary(Of Integer, ChannelModifierCollection)()

#End Region


#Region " Init and clean-up "

    Public Sub New(
        panControl As ISimplePanning,
        volumeControl As ISimpleVolume,
        sourceList As List(Of SourcePanningInfo(Of Single)))

        For Each pan In sourceList
            Dim coefList As Single()

            If pan.IsEnabled AndAlso Not volumeControl.IsMuted Then
                ' Two displacements: virtual mono source vs source channels (playback panning)
                ' and speaker vs virtual mono source (position).
                Dim displ = GetModelValue(panControl.PanningModel, Math.Abs(panControl.Panning) / 2) *
                            GetModelValue(PanningModels.ConstantPower, Math.Abs(pan.SpeakerPosition - panControl.Panning / 2))

                coefList = MultiplyCoef(pan.EnabledMap, volumeControl.Volume * displ, pan.SourceToMono)
            Else
                coefList = New Single(pan.EnabledMap.SourceCount - 1) {}
            End If

            ' There are no reflections involved in panning
            mModifiers.Add(
                pan.PhysicalNr,
                New ChannelModifierCollection From {New ChannelModifier(pan.Delay, coefList)})
        Next
    End Sub

#End Region


#Region " ICoefficientGenerator implementation "

    Public Function Generate(phChNr As Integer) As ChannelModifierCollection Implements ICoefficientGenerator.Generate
        Dim mc As ChannelModifierCollection = Nothing

        If mModifiers.TryGetValue(phChNr, mc) Then
            Return mc
        End If

        Return sEmptyCollection
    End Function

#End Region

End Class
