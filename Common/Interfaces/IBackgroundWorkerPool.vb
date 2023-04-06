Imports System.Threading


''' <summary>
''' Queue to execute work asynchronously in the background.
''' </summary>
''' <typeparam name="TItem">Type of item to work on</typeparam>
Public Interface IBackgroundWorkerPool(Of TItem)
	Inherits IDisposable

	''' <summary>
	''' The maximum amount of operations, executed in parallel.
	''' Defaults to the number of CPUs - 1.
	''' </summary>
	ReadOnly Property MaxDegreeOfParallelism As Integer


	''' <summary>
	''' A cancellation token that is set when ASP.NET is shutting down the app domain.
	''' </summary>
	ReadOnly Property ShutdownToken As CancellationToken


	''' <summary>
	''' Executes an asynchronous fire and forget background operation, registering it with ASP.NET.
	''' All exceptions are ignored. If you want to catch exceptions, you'll have to do it yourself.
	''' Dont use async with QueueWorkItem
	''' If the work item is long running, it should periodically check the cancellation token to
	''' check if cancellation is requested.
	''' </summary>
	''' <param name="workItem">The background operation</param>
	''' <param name="comparer">
	''' If <paramref name="comparer"/> returns True for any item being worked on, the addition is cancelled
	''' </param>
	''' 
	Sub QueueWorkItem(workItem As TItem, comparer As Func(Of TItem, Boolean))

End Interface
