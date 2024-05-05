Imports TextChannelLibrary


Public Class TextRendererFactory
    Implements ITextRendererFactory

    Private mWindows As New Dictionary(Of Integer, TextWindow)()


    Public Function Create(config As RenderTextInterface, channel As TextPhysicalChannel) As ITextRenderer Implements ITextRendererFactory.Create
        Dim window As TextWindow = Nothing

        If mWindows.TryGetValue(channel.Channel, window) Then
            window.Configuration = config
            Return window
        End If

        window = New TextWindow() With {
            .Channel = channel,
            .Configuration = config
        }
        mWindows(channel.Channel) = window

        Return window
    End Function

End Class
