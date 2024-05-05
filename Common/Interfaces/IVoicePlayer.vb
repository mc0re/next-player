Imports System.IO


''' <summary>
''' Used by voice synthesizer in VoiceControl library to
''' actually pronounce things by AudioPlayer.
''' </summary>
Public Interface IVoicePlayer

    ''' <summary>
    ''' Play-and-forget the given audio stream on the given logical channel.
    ''' </summary>
    Sub PlayAndForget(strm As Stream, channelNo As Integer)

End Interface
