Imports System.ComponentModel


Public Interface ITextOutputInterface
    Inherits INotifyPropertyChanged

    ''' <summary>
    ''' Send the given text to the output.
    ''' <paramref name="text"/> can be Nothing, meaning
    ''' the previous text shall be cleared, if possible.
    ''' </summary>
    Sub SendText(text As String)


    ''' <summary>
    ''' Set text playback position, if applicable [0-1].
    ''' </summary>
    Sub SetPosition(position As Double)

End Interface
