''' <summary>
''' The implementing class is a 2-nd phase generator.
''' 
''' It is created by calling <see cref="ICoefficientGeneratorFactory.Create"/> method.
''' 
''' Its <see cref="ICoefficientGenerator.Generate"/> method is called from within
''' the sample providers.
''' </summary>
Public Interface ICoefficientGenerator

    ''' <summary>
    ''' Generate a set of channel modifiers for the given physical channel.
    ''' </summary>
    ''' <remarks>
    ''' Each item in the collection contains a set of volume coefficients
    ''' for a particular delay. In panning there is only one delay,
    ''' but in 3D positioning can be more (especially considering reflections).
    ''' </remarks>
    Function Generate(phChNr As Integer) As ChannelModifierCollection

End Interface
