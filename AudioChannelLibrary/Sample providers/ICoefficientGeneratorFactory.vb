''' <summary>
''' The implementing class is a 1-st phase generator.
''' 
''' It is used from within the player, and created upon player startup,
''' when panning model is changed, or when the audio environment is changed
''' (links are modified, physical channels are moved, etc.).
''' 
''' Its <see cref="ICoefficientGeneratorFactory.Create"/> method is called
''' in the player upon changes in the playback (panning, coordinates, volume).
''' </summary>
Public Interface ICoefficientGeneratorFactory

    Function Create(playback As AudioPlaybackInfo) As ICoefficientGenerator

End Interface
