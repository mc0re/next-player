''' <summary>
''' Prepared information independent from the playback per physical channel.
''' </summary>
Public Class SourcePanningInfo(Of TPosition)

    ''' <summary>
    ''' Destination physical channel.
    ''' </summary>
    Public PhysicalNr As Integer


    ''' <summary>
    ''' Whether the output to that channel is enabled.
    ''' </summary>
    Public IsEnabled As Boolean


    ''' <summary>
    ''' Which inputs are enabled.
    ''' </summary>
    Public EnabledMap As EnabledChannelsCollection


    ''' <summary>
    ''' Link delay.
    ''' </summary>
    Public Delay As Single


    ''' <summary>
    ''' Source-to-physical coefficients.
    ''' The coefficients are to be used as:
    '''   mono = SUM ( sourceData[i] * coef[i] )
    ''' </summary>
    Public SourceToMono As Single()


    ''' <summary>
    ''' Where the channel is located.
    ''' For panning this would be a single number -0.5..0.5.
    ''' For 3D positioning - a point definition.
    ''' </summary>
    Public SpeakerPosition As TPosition

End Class
