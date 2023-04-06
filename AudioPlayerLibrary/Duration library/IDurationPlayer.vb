Imports System.Windows


''' <summary>
''' Interface for a component producing duration information.
''' </summary>
Public Interface IDurationPlayer
	Inherits IDisposable

    ''' <summary>
    ''' Audio file opened successfully.
    ''' </summary>
    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly",
                                                     Justification:="I want 'args', CA wants 'e'.")>
    Event MediaOpened(sender As Object, args As EventArgs)


    ''' <summary>
    ''' There was an error during opening or analyzing / playing the file.
    ''' </summary>
    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly",
                                                     Justification:="I want 'args', CA wants 'e'.")>
    Event MediaFailed(sender As Object, args As MediaFailedEventArgs)


	''' <summary>
	''' Duration of the currently opened file.
	''' </summary>
	ReadOnly Property NaturalDuration As Duration


	''' <summary>
	''' Open a given file; the channel must be already set.
	''' </summary>
	''' <param name="channelNo">Logical channel number: 1.. for real playback, 0 for duration only</param>
	''' <remarks>
	''' It is allowed to open a file, close it, and then open another one in the same player instance.
	''' </remarks>
	Sub Open(fileName As String, channelNo As Integer)


	''' <summary>
	''' Close the currently opened file.
	''' </summary>
	''' <remarks>This does not dispose the resources. Can be called multiple times.</remarks>
	Sub Close()

End Interface
