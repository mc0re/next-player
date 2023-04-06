Imports AudioPlayerLibrary
Imports Common
Imports TextChannelLibrary


''' <summary>
''' Show text in a prepared window.
''' </summary>
Public Class PlayerActionText
	Inherits PlayerAction

#Region " Text notifying property "

	Private mText As String


	''' <summary>
	''' Text to show.
	''' </summary>
	Public Property Text As String
		Get
			Return mText
		End Get
		Set(value As String)
			mText = value
			RaisePropertyChanged(Function() Text)
		End Set
	End Property

#End Region


#Region " Channel notifying property "

	Private mChannel As Integer = 1


	''' <summary>
	''' Channel to use, 1-based.
	''' </summary>
	Public Property Channel As Integer
		Get
			Return mChannel
		End Get
		Set(value As Integer)
			mChannel = value
			RaisePropertyChanged(Function() Channel)
		End Set
	End Property

#End Region


#Region " Init and clean-up "

	Public Sub New()
		Name = "Show text"
		HasDuration = True
		ExecutionType = ExecutionTypes.MainContinuePrev
	End Sub

#End Region


#Region " PlayerAction overrides "

	Public Overrides Sub Start()
		Dim storage = InterfaceMapper.GetImplementation(Of ITextChannelStorage)()
		Dim ch = storage.Logical.Channel(Channel)

		' Avoid occasional Null-reference exceptions
		If ch Is Nothing Then Return

		If String.IsNullOrEmpty(Text) Then
			ch.HideText()
		Else
			ch.ShowText(Text)
		End If
	End Sub

#End Region


#Region " Additional actions "

	Public Shared Sub Reset()
		Dim chList = InterfaceMapper.GetImplementation(Of ITextChannelStorage)()
		chList.HideAll()
		Dim storage = InterfaceMapper.GetImplementation(Of ITextEnvironmentStorage)()
		storage.HideAll()
	End Sub

#End Region


#Region " ToString "

	Public Overrides Function ToString() As String
		Return String.Format("Text '{0}'", Text)
	End Function

#End Region

End Class
