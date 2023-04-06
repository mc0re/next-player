Imports System.Windows.Threading


Class Application

    Private Sub UnhandledException(sender As Object, e As DispatcherUnhandledExceptionEventArgs) Handles Me.DispatcherUnhandledException
        Debugger.Break()
    End Sub

End Class
