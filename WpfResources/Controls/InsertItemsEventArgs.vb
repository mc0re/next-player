Imports PlayerActions


''' <summary>
''' Definition of event arguments.
''' </summary>
Public Class InsertItemsEventArgs
    Inherits RoutedEventArgs

    Public Property InsertIndex As Integer

    Public Property ItemList As IEnumerable(Of PlayerAction)


    Public Sub New(evt As RoutedEvent)
        MyBase.New(evt)
    End Sub

End Class


''' <summary>
''' Delegate type for handling InsertItem event.
''' </summary>
Public Delegate Sub InsertItemsEventHandler(sender As Object, args As InsertItemsEventArgs)
