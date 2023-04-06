Imports System.ComponentModel


''' <summary>
''' Different models for panning calculation.
''' </summary>
<CLSCompliant(True)>
Public Enum PanningModels

    <Description("No panning, each channel has a coefficient of 1/N")>
    Fixed

    <Description("Panning defines between-the-channels displacement, constant volume")>
    ConstantVolume

    <Description("Panning defines between-the-channels displacement, constant intensity")>
    ConstantPower

    <Description("Panning defines an angle of turning")>
    Angular

    <Description("3D positioning")>
    Coordinates

End Enum
