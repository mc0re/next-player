Imports System.ComponentModel


Public Class Plane3DVisualizerWindow

    Protected Overrides Sub OnClosing(e As CancelEventArgs)
        VisualizerControl.Dispatcher.InvokeShutdown()
        MyBase.OnClosing(e)
    End Sub

End Class