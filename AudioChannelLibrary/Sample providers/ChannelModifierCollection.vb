


''' <summary>
''' Describes the sound modifications, when it is played
''' from the given physical channel as though it would've been
''' played from a point in space.
''' Contains a collection of modifiers with different parameters
''' (in particular, different delays), which shall be mixed together.
''' </summary>
Public Class ChannelModifierCollection
	Inherits List(Of ChannelModifier)

#Region " Init and clean-up "

	Public Sub New()
		' Do nothing
	End Sub


	Public Sub New(capacity As Integer)
		MyBase.New(capacity)
	End Sub

#End Region


#Region " API "

	''' <summary>
	''' Provide a smooth transition from the old coefficient list to Me.
	''' </summary>
	''' <param name="progress">0 - full "from", 1 - full "to" (Me)</param>
	Public Function Transition(fromModif As ChannelModifierCollection, progress As Single) As ChannelModifierCollection
		If fromModif Is Nothing Then Return Me
		If progress < RelativeVolumePrecision Then Return fromModif
		If progress > 1 - RelativeVolumePrecision Then Return Me

		Dim maxLen = Math.Max(fromModif.Count, Count)
		Dim res As New ChannelModifierCollection(maxLen)
		For idx = 0 To maxLen - 1
			Dim fromMod = If(idx < fromModif.Count, fromModif(idx), Nothing)
			Dim toMod = If(idx < Count, Me(idx), Nothing)
			res.Add(Transition(fromMod, toMod, progress))
		Next

		Return res
	End Function

#End Region


#Region " Utility "

	''' <summary>
	''' Create a transitional modifier.
	''' </summary>
	''' <param name="fromMod">From or Nothing</param>
	''' <param name="toMod">To or Nothing</param>
	''' <param name="progress">0 - full "from", 1 - full "to" (Me)</param>
	Private Function Transition(fromMod As ChannelModifier, toMod As ChannelModifier, progress As Single) As ChannelModifier
		Dim srcCnt = If(fromMod, toMod).SourceChannelCount
		Dim coef = New Single(srcCnt - 1) {}

		For srcIdx = 0 To srcCnt - 1
			Dim coefFrom = If(fromMod Is Nothing, 0, fromMod.Volume(srcIdx))
			Dim coefTo = If(toMod Is Nothing, 0, toMod.Volume(srcIdx))
			coef(srcIdx) = coefTo * progress + coefFrom * (1 - progress)
		Next

		Dim delayFrom = If(fromMod Is Nothing, 0, fromMod.Delay)
		Dim delayTo = If(toMod Is Nothing, 0, toMod.Delay)
		Dim delay = delayTo * progress + delayFrom * (1 - progress)

		Return New ChannelModifier(delay, coef)
	End Function

#End Region

End Class
