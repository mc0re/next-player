Imports System.Threading.Tasks
Imports System.Windows.Threading
Imports Common
Imports Serilog


Class Application

    Private mLogger As ILogger


    Protected Overrides Sub OnStartup(e As StartupEventArgs)
        mLogger = New LoggerConfiguration().
            ReadFrom.AppSettings().
            CreateLogger()
        mLogger.Information("Starting NextPlayer")
        InterfaceMapper.SetInstance(mLogger)
        MyBase.OnStartup(e)
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf UnhandledAppDomainException
        AddHandler TaskScheduler.UnobservedTaskException, AddressOf UnhandledTaskException
    End Sub


    Private Sub UnhandledDispatcherException(sender As Object, e As DispatcherUnhandledExceptionEventArgs) Handles Me.DispatcherUnhandledException
        mLogger.Error(e.Exception, "UnhandledDispatcherException")
        Debugger.Break()
    End Sub


    Private Sub UnhandledAppDomainException(sender As Object, e As UnhandledExceptionEventArgs)
        mLogger.Error(TryCast(e.ExceptionObject, Exception), "UnhandledAppDomainException")
        Debugger.Break()
    End Sub


    Private Sub UnhandledTaskException(sender As Object, e As UnobservedTaskExceptionEventArgs)
        mLogger.Error(e.Exception, "UnhandledTaskException")
        e.SetObserved()
        Debugger.Break()
    End Sub

End Class
