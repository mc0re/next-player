Imports Common


''' <summary>
''' Combine the needed interfaces.
''' </summary>
Public MustInherit Class DurationLibraryItemBase

	Public MustOverride ReadOnly Property AsInputFile As IInputFile

	Public MustOverride ReadOnly Property AsDuration As IDurationElement

End Class
