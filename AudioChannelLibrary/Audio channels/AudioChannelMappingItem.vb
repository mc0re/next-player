Imports Common


''' <summary>
''' Stores the information for mapping a single wave channel to
''' a single physical channel.
''' </summary>
<Serializable>
<CLSCompliant(True)>
Public Class AudioChannelMappingItem
	Inherits PropertyChangedHelper

#Region " IsSet notifying property "

	Private mIsSet As Boolean


	Public Property IsSet As Boolean
		Get
			Return mIsSet
		End Get
		Set(value As Boolean)
			SetField(mIsSet, value, NameOf(IsSet))
		End Set
	End Property

#End Region


#Region " ToString "

	Public Overrides Function ToString() As String
		Return If(IsSet, "X", "-")
	End Function

#End Region

End Class
