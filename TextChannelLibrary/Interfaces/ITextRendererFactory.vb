Public Interface ITextRendererFactory

    ''' <summary>
    ''' Create a text renderer.
    ''' </summary>
    Function Create(config As RenderTextInterface, channel As TextPhysicalChannel) As ITextRenderer

End Interface
