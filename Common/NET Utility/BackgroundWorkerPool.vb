Imports System.Collections.Concurrent
Imports System.Diagnostics.CodeAnalysis
Imports System.Threading


''' <summary>
''' This class contains a pool of background threads, which execute queued work.
''' </summary>
Public NotInheritable Class BackgroundWorkerPool(Of TItem)
    Implements IBackgroundWorkerPool(Of TItem)

#Region " Fields "

    ''' <summary>
    ''' A queue of incoming items to process.
    ''' </summary>
    Private ReadOnly mWorkQueue As New BlockingCollection(Of TItem)()


    ''' <summary>
    ''' A collection of the items in the queue and in the works.
    ''' </summary>
    Private ReadOnly mWorkItemList As New ConcurrentBag(Of TItem)()


    ''' <summary>
    ''' A common processing function.
    ''' </summary>
    Private ReadOnly mWorkerMethod As Action(Of TItem, CancellationToken)


    ''' <summary>
    ''' Worker tasks.
    ''' </summary>
    Private ReadOnly mWorkerList As Task()


    ''' <summary>
    ''' To catch redundant calls to dispose
    ''' </summary>
    Private mDisposed As Boolean

#End Region


#Region " MaxDegreeOfParallelism read-only property "

    Private mMaxDegreeOfParallelism As Integer


    ''' <summary>
    ''' The maximum amount of tasks, executed in parallel.
    ''' </summary>
    Public ReadOnly Property MaxDegreeOfParallelism As Integer Implements IBackgroundWorkerPool(Of TItem).MaxDegreeOfParallelism
        Get
            Return mMaxDegreeOfParallelism
        End Get
    End Property

#End Region


#Region " ShutdownToken read-only property "

    ''' <summary>
    ''' A cancellation token that is set when the worker has to close.
    ''' </summary>
    Private ReadOnly mShutdownToken As New CancellationTokenSource()


    ''' <summary>
    ''' A cancellation token that is set when the worker has to close.
    ''' </summary>
    Public ReadOnly Property ShutdownToken As CancellationToken Implements IBackgroundWorkerPool(Of TItem).ShutdownToken
        Get
            Return mShutdownToken.Token
        End Get
    End Property

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Create the default amount of workers (CPU - 1, but at least 1).
    ''' </summary>
    Public Sub New(method As Action(Of TItem, CancellationToken))
        Me.New(Math.Max(1, Environment.ProcessorCount - 1), method)
    End Sub


    ''' <summary>
    ''' Create the given amount of workers.
    ''' </summary>
    ''' <param name="nofTasks">The number of parallel tasks</param>
    Public Sub New(nofTasks As Integer, method As Action(Of TItem, CancellationToken))
        mMaxDegreeOfParallelism = nofTasks
        mWorkerMethod = method
        mWorkerList = New Task(MaxDegreeOfParallelism - 1) {}

        For i = 0 To MaxDegreeOfParallelism - 1
            mWorkerList(i) = Task.Factory.StartNew(AddressOf Worker, TaskCreationOptions.LongRunning)
        Next
    End Sub

#End Region


#Region " Utility "

    ''' <summary>
    ''' The main function of each worker.
    ''' </summary>
    <SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")>
    <SuppressMessage("Design", "CC0004:Catch block cannot be empty", Justification:="Swallow exceptions in a thread")>
    Private Sub Worker()
        Dim workItem As TItem

        Try
            While mWorkQueue.TryTake(workItem, Timeout.Infinite, mShutdownToken.Token)
                Try
                    mWorkerMethod(workItem, mShutdownToken.Token)
                    mWorkItemList.TryTake(workItem)

                Catch ex As Exception
                    ' Probably already logged
                    Throw
                End Try
            End While

        Catch ex As OperationCanceledException
            ' Shut down
        Catch ex As Exception
            ' Report and shut down
            InterfaceMapper.GetImplementation(Of IMessageLog)().LogLoadingError(
                "Error in background thread: {0}", ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' Ask for cancellation of all activities.
    ''' </summary>
    Private Sub Shutdown(immediate As Boolean)
        mWorkQueue.CompleteAdding()
        mShutdownToken.Cancel()

        If Not immediate Then
            Return
        End If

        ' Wait for all tasks to finish
        Task.WaitAll(mWorkerList)
        Dispose()
    End Sub

#End Region


#Region " IBackgroundWorker implementation "

    ''' <summary>
    ''' Executes an asynchronous background operation.
    ''' All exceptions are ignored.
    ''' Don't use async with QueueWorkItem.
    ''' If the work item is long running, it should periodically check the cancellation token to
    ''' check if cancellation is requested.
    ''' </summary>
    ''' <param name="workItem">The item to perform background operation on</param>
    ''' <param name="comparer">To identify and ignore duplicate requests</param>
    Public Sub QueueWorkItem(workItem As TItem, comparer As Func(Of TItem, Boolean)
    ) Implements IBackgroundWorkerPool(Of TItem).QueueWorkItem

        If mWorkItemList.Any(comparer) Then
            Return
        End If

        mWorkItemList.Add(workItem)
        mWorkQueue.Add(workItem)
    End Sub

#End Region


#Region " IDisposable implementation "

    Public Sub Dispose() Implements IDisposable.Dispose
        If mDisposed Then
            Return
        End If

        Shutdown(False)
        mShutdownToken.Dispose()
        mWorkQueue.Dispose()
        mDisposed = True
    End Sub

#End Region

End Class
