Public Interface ITextRenderer

    ''' <summary>
    ''' Text scrolling position, if applicable [0-1].
    ''' </summary>
    Property ScrollPosition As Double


    ''' <summary>
    ''' Show a window with the given text.
    ''' </summary>
    Sub Show(text As String)


    ''' <summary>
    ''' Hide the window.
    ''' </summary>
    Sub Hide()

End Interface
