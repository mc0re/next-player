Imports Common


Public Class DurationLibraryItem(Of T As {IInputFile, IDurationElement})
	Inherits DurationLibraryItemBase

	Private ReadOnly mValue As T


	Public Overrides ReadOnly Property AsDuration As IDurationElement
		Get
			Return mValue
		End Get
	End Property


	Public Overrides ReadOnly Property AsInputFile As IInputFile
		Get
			Return mValue
		End Get
	End Property


	Public Sub New(value As T)
		mValue = value
	End Sub

End Class
