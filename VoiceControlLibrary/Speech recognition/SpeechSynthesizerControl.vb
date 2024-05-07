Imports System.Globalization
Imports System.IO
Imports System.Speech.AudioFormat
Imports System.Speech.Synthesis
Imports Common


Public NotInheritable Class SpeechSynthesizerControl
    Implements ISpeechSynthesizer

#Region " Fields "

    Private ReadOnly mSynth As New SpeechSynthesizer


    Private mPlayer As IVoicePlayer


    Private mVoiceConfig As IVoiceConfiguration

#End Region


#Region " SynthesizedVoices property "

    Private mSynthesizedVoices As IEnumerable(Of String)


    Public ReadOnly Property SynthesizedVoices As IEnumerable(Of String) Implements ISpeechSynthesizer.SynthesizedVoices
        Get
            If mSynthesizedVoices Is Nothing Then
                mSynthesizedVoices = mSynth.GetInstalledVoices().
                    Where(Function(v) v.Enabled And v.VoiceInfo.Culture.TwoLetterISOLanguageName = "en").
                    Select(Function(v) v.VoiceInfo.Name).
                    ToList()
            End If

            Return mSynthesizedVoices
        End Get
    End Property

#End Region


#Region " API "

    ''' <inheritdoc/>
    Public Sub Setup() Implements ISpeechSynthesizer.Setup
        mVoiceConfig = InterfaceMapper.GetImplementation(Of IVoiceConfiguration)()
        mPlayer = Nothing

        Try
            mSynth.SelectVoice(mVoiceConfig.VoiceControlFeedbackVoice)
        Catch
            ' Use default
        End Try
    End Sub


    ''' <inheritdoc/>
    Public Sub Speak(text As String) Implements ISpeechSynthesizer.Speak
        If Not mVoiceConfig.IsVoiceControlEnabled Then Return

        If mPlayer Is Nothing Then
            mPlayer = InterfaceMapper.GetImplementation(Of IVoicePlayer)()
        End If

        Dim audioStream As New MemoryStream
        Dim outputFormat = New SpeechAudioFormatInfo(44100, AudioBitsPerSample.Sixteen, AudioChannel.Mono)

        mSynth.SetOutputToAudioStream(audioStream, outputFormat)
        mSynth.Speak(SplitText(text))
        mSynth.SetOutputToNull()
        audioStream.Seek(0, SeekOrigin.Begin)
        mPlayer.PlayAndForget(audioStream, mVoiceConfig.VoiceControlFeedbackChannel)
    End Sub

#End Region


#Region " Utility "

    ''' <summary>
    ''' Assume Russian for non-English characters.
    ''' </summary>
    Private Function SplitText(text As String) As PromptBuilder
        Dim pb As New PromptBuilder()
        If String.IsNullOrEmpty(text) Then Return pb

        Dim startIdx = 0
        Dim startIsAscii = AscW(text(0)) <= 127
        Dim nextIdx = startIdx
        Dim nextIsAscii = startIsAscii

        While True
            If nextIdx >= text.Length Then
                ' Text ended, append the found block
                pb.AppendText(text.Substring(startIdx, text.Length - startIdx))
                pb.EndVoice()
                Exit While
            End If

            If startIsAscii <> nextIsAscii AndAlso nextIdx - startIdx > 0 Then
                ' Language has changed, append the found block
                pb.AppendText(text.Substring(startIdx, nextIdx - startIdx - 1))
                pb.EndVoice()
                startIdx = nextIdx - 1
            End If

            If startIsAscii <> nextIsAscii OrElse nextIdx = 0 Then
                ' Language is initialized or has changed, set the voice
                If nextIsAscii Then
                    pb.StartVoice(mVoiceConfig.VoiceControlFeedbackVoice)
                Else
                    pb.StartVoice(CultureInfo.GetCultureInfo("ru-RU"))
                End If

                startIsAscii = nextIsAscii
            End If

            Dim ch = text(nextIdx)
            nextIdx += 1

            If Not Char.IsLetter(ch) Then Continue While

            nextIsAscii = AscW(ch) <= 127
        End While

        Return pb
    End Function

#End Region

End Class
