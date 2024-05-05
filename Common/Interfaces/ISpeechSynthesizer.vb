''' <summary>
''' ABstract away communication to the speech synthesizer.
''' </summary>
Public Interface ISpeechSynthesizer

    ''' <summary>
    ''' Get a collection of names of the installed voices.
    ''' Set the chosen voice in <see cref="IVoiceConfiguration.VoiceControlFeedbackVoice"/>.
    ''' </summary>
    ReadOnly Property SynthesizedVoices As IEnumerable(Of String)


    ''' <summary>
    ''' (Re)read setup information from <see cref="IVoiceConfiguration"/>.
    ''' </summary>
    Sub Setup()


    ''' <summary>
    ''' Asynchronously speak the given text.
    ''' </summary>
    Sub Speak(text As String)

End Interface
