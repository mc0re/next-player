Imports AudioChannelLibrary


''' <summary>
''' Show and control the volume.
''' </summary>
<TemplatePart(Name:="PART_Indicator", Type:=GetType(FrameworkElement))>
<TemplatePart(Name:="PART_Track", Type:=GetType(FrameworkElement))>
Public Class VolumeControl
    Inherits PositionedRangeBase

#Region " Overrides "

    Protected Overrides Sub ProgressValueChanged(progress As Double)
        Dim action = CType(DataContext, IVolumeController)
        progress = Math.Round(progress, 2)
        action.Volume = CSng(progress)
    End Sub

#End Region

End Class
