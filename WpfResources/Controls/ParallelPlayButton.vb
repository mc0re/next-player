Imports System.ComponentModel


Public Class ParallelPlayButton
	Inherits Button

#Region " IsPlaying dependency property "

	Public Shared ReadOnly IsPlayingProperty As DependencyProperty = DependencyProperty.Register(
	 NameOf(IsPlaying), GetType(Boolean), GetType(ParallelPlayButton))


	<Category("Common Properties"), Description("Whether the action is being played")>
	Public Property IsPlaying As Boolean
		Get
			Return CBool(GetValue(IsPlayingProperty))
		End Get
		Set(value As Boolean)
			SetValue(IsPlayingProperty, value)
		End Set
	End Property

#End Region

End Class
