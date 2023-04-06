Imports Common


Public MustInherit Class PlayerSetup

    ''' <summary>
    ''' Playback panning model.
    ''' </summary>
    Public MustOverride ReadOnly Property PanningModel As PanningModels


    ''' <summary>
    ''' Playback panning (balance).
    ''' </summary>
    Public Property PlaybackPanning As Single = 0


    ''' <summary>
    ''' Playback volume.
    ''' </summary>
    Public Property Volume As Single = 1


    ''' <summary>
    ''' Whether the sound source is muted.
    ''' </summary>
    Public Property Mute As Boolean = False

End Class


Public Class PlayerSetupFixed
    Inherits PlayerSetup

    Public Overrides ReadOnly Property PanningModel As PanningModels = PanningModels.Fixed

End Class


Public Class PlayerSetupConstantVolume
    Inherits PlayerSetup

    Public Overrides ReadOnly Property PanningModel As PanningModels = PanningModels.ConstantVolume

End Class


Public Class PlayerSetupConstantPower
    Inherits PlayerSetup

    Public Overrides ReadOnly Property PanningModel As PanningModels = PanningModels.ConstantPower

End Class
