Imports System.ComponentModel


''' <summary>
''' This is a sorted by X list of AutomationPoint objects.
''' </summary>
<Serializable>
Public Class AutomationPointCollection
	Inherits BindingList(Of AutomationPoint)

	Public Shadows Sub Add(pt As AutomationPoint)
		Dim mSortedList = Me.ToList()

		If pt.X < 0 Then pt.X = 0
		If pt.Y < 0 Then pt.Y = 0
		If pt.Y > 1 Then pt.Y = 1

		mSortedList.Add(pt)
		mSortedList = mSortedList.OrderBy(Function(p) p.X).ToList()

		RaiseListChangedEvents = False
		MyBase.Clear()

		For Each pt In mSortedList
			MyBase.Add(pt)
		Next

		RaiseListChangedEvents = True
		ResetBindings()
	End Sub

End Class
