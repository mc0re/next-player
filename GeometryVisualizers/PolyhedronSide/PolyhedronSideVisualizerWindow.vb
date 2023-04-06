Imports System.ComponentModel


Public Class PolyhedronSideVisualizerWindow

    Protected Overrides Sub OnClosing(e As CancelEventArgs)
        VisualizerControl.Dispatcher.InvokeShutdown()
        MyBase.OnClosing(e)
    End Sub

End Class