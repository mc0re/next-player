Imports AudioChannelLibrary


Public Interface IAudioPlayer
	Inherits IDurationPlayer

#Region " Events "

    ''' <summary>
    ''' The media playback has naturally ended.
    ''' </summary>
    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly",
                                  Justification:="I want 'args', CA wants 'e'.")>
    Event MediaEnded(sender As Object, args As MediaEndedEventArgs)

#End Region


#Region " Properties "

	''' <summary>
	''' Logical channel number for playback (1..)
	''' </summary>
	ReadOnly Property Channel As Integer


	''' <summary>
	''' Position from the start of the input.
	''' </summary>
	Property Position As TimeSpan


	Property PlaybackInfo As AudioPlaybackInfo

#End Region


#Region " Methods "

	''' <summary>
	''' Execute the given action in such a context (thread), where
	''' IAudioPlayer properties are accessible.
	''' </summary>
	Sub Execute(action As Action)


	''' <summary>
	''' Start / resume playback.
	''' </summary>
	Sub Play()


	''' <summary>
	''' Pause playback.
	''' </summary>
	Sub Pause()


	''' <summary>
	''' Stop (and never restart) playback.
	''' </summary>
	''' <remarks>Called just before Close().</remarks>
	Sub [Stop]()

#End Region

End Interface
