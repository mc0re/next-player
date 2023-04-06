Imports System.Collections.Specialized
Imports System.ComponentModel
Imports AudioPlayerLibrary


''' <summary>
''' Derived class to add property name.
''' </summary>
Public Class PlayerNotifyCollectionChangedEventArgs
	Inherits NotifyCollectionChangedEventArgs

	Public ReadOnly ChangedProperty As PropertyDescriptor


	Public Sub New(action As NotifyCollectionChangedAction)
		MyBase.New(action)
	End Sub


	Public Sub New(action As NotifyCollectionChangedAction, newItem As IPlayerAction)
		MyBase.New(action, newItem)
	End Sub


	Public Sub New(action As NotifyCollectionChangedAction, changedItem As IPlayerAction, prop As PropertyDescriptor)
		MyBase.New(action, changedItem, changedItem)
		ChangedProperty = prop
	End Sub


	Public Sub New(action As NotifyCollectionChangedAction, movedItem As IPlayerAction, oldIndex As Integer, newIndex As Integer)
		MyBase.New(action, movedItem, newIndex, oldIndex)
	End Sub

End Class
