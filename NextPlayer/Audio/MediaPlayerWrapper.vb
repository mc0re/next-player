Imports System.IO
Imports AudioChannelLibrary
Imports AudioPlayerLibrary
Imports Common


''' <summary>
''' Trivial implementation on top of MediaPlayer class.
''' </summary>
Public Class MediaPlayerWrapper
	Implements IAudioPlayer

#Region " Events "

	Public Event MediaOpened(ByVal sender As Object, ByVal args As EventArgs) Implements IAudioPlayer.MediaOpened

	Public Event MediaFailed(ByVal sender As Object, ByVal args As MediaFailedEventArgs) Implements IAudioPlayer.MediaFailed

	Public Event MediaEnded(ByVal sender As Object, ByVal args As MediaEndedEventArgs) Implements IAudioPlayer.MediaEnded

#End Region


#Region " Fields "

	Private WithEvents mPlayer As New MediaPlayer()

#End Region


#Region " IAudioPlayer properties "

	Public ReadOnly Property NaturalDuration As Duration Implements IAudioPlayer.NaturalDuration
		Get
			Return mPlayer.NaturalDuration
		End Get
	End Property


	Public Property Position As TimeSpan Implements IAudioPlayer.Position
		Get
			Return mPlayer.Position
		End Get
		Set(value As TimeSpan)
			mPlayer.Position = value
		End Set
	End Property


	''' <summary>
	''' Does absolutely nothing, as MediaPlayer cannot redirect output.
	''' </summary>
	Public ReadOnly Property Channel As Integer = 0 Implements IAudioPlayer.Channel

#End Region


#Region " PlaybackInfo property "

	Private WithEvents mPlaybackInfo As New AudioPlaybackInfo()


	Public Property PlaybackInfo As AudioPlaybackInfo Implements IAudioPlayer.PlaybackInfo
		Get
			Return mPlaybackInfo
		End Get
		Set(value As AudioPlaybackInfo)
			mPlaybackInfo = value
			SetPlayerParams()
		End Set
	End Property


	Private Sub SetPlayerParams() Handles mPlaybackInfo.PropertyChanged
		mPlayer.IsMuted = mPlaybackInfo.IsMuted
		mPlayer.Volume = mPlaybackInfo.Volume
		mPlayer.Balance = If(
					mPlaybackInfo.PanningModel <> PanningModels.Fixed, mPlaybackInfo.Panning, 0)
	End Sub

#End Region


#Region " API "
	''' <summary>
	''' MediaPlayer cannot play to a non-default output.
	''' </summary>
	''' <param name="channelNo">Ignored</param>
	Public Sub Open(fileName As String, channelNo As Integer) Implements IAudioPlayer.Open
		mPlayer.Open(New Uri(fileName))
	End Sub


	Public Sub Close() Implements IAudioPlayer.Close
		mPlayer.Close()
	End Sub


	Public Sub PlayAndForget(strm As Stream, channel As Integer) Implements IVoicePlayer.PlayAndForget
		' Do nothing, Media Player can only play from URL
	End Sub


	Public Sub Execute(ByVal action As Action) Implements IAudioPlayer.Execute
		mPlayer.Dispatcher.Invoke(action)
	End Sub


	Public Sub Play() Implements IAudioPlayer.Play
		mPlayer.Play()
	End Sub
	Public Sub Pause() Implements IAudioPlayer.Pause
		mPlayer.Pause()
	End Sub
	Public Sub [Stop]() Implements IAudioPlayer.[Stop]
		mPlayer.Stop()
	End Sub




#End Region
#Region " Event listeners "

	Private Sub MediaOpenedHandler(sender As Object, args As EventArgs) Handles mPlayer.MediaOpened
		RaiseEvent MediaOpened(sender, args)
	End Sub


	Private Sub MediaFailedHandler(sender As Object, args As EventArgs) Handles mPlayer.MediaFailed
		RaiseEvent MediaFailed(sender, New MediaFailedEventArgs With {.FileName = mPlayer.Source.LocalPath})
	End Sub


	Private Sub MediaEndedHandler(sender As Object, args As EventArgs) Handles mPlayer.MediaEnded
		RaiseEvent MediaEnded(sender, New MediaEndedEventArgs With {.FileName = mPlayer.Source.LocalPath})
	End Sub




#End Region
#Region " IDisposable implementation "

	Public Sub Dispose() Implements IDisposable.Dispose
		mPlayer.Close()
		mPlayer = Nothing
	End Sub


#End Region

End Class
