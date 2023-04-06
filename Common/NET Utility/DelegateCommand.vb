Imports System.Windows.Input


''' <summary>
''' Helper for MVVM commands.
''' </summary>
Public Class DelegateCommand
    Implements ICommand

#Region " Events "

    Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

#End Region


#Region " Fields "

    Private ReadOnly mCanExecute As Predicate(Of Object)

    Private ReadOnly mExecute As Action(Of Object)

#End Region


#Region " Init and clean-up "

    Public Sub New(execute As Action(Of Object))
        Me.New(execute, Nothing)
    End Sub


    Public Sub New(execute As Action(Of Object), canExecute As Predicate(Of Object))
        mExecute = execute
        mCanExecute = canExecute
    End Sub

#End Region


#Region " ICommand implementation "

    Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
        If mCanExecute Is Nothing Then
            Return True
        End If

        Return mCanExecute(parameter)
    End Function


    Public Sub Execute(parameter As Object) Implements ICommand.Execute
        mExecute(parameter)
    End Sub


    Public Sub RaiseCanExecuteChanged()
        RaiseEvent CanExecuteChanged(Me, EventArgs.Empty)
    End Sub

#End Region

End Class
