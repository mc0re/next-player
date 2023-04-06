Imports System.Xml.Serialization
Imports Common
Imports NAudio.Wave


''' <summary>
''' This class is to overcome problems when serializing interfaces.
''' </summary>
<XmlInclude(GetType(WaveOutAudioInterface))>
<XmlInclude(GetType(DirectSoundAudioInterface))>
<XmlInclude(GetType(AsioAudioInterface))>
<XmlInclude(GetType(WasapiAudioInterface))>
<CLSCompliant(True)>
<Serializable>
Public MustInherit Class AudioOutputInterfaceBase
    Inherits PropertyChangedHelper
    Implements IAudioOutputInterface

#Region " Channels notifying property "

    Private mChannels As Integer = 2


    ''' <summary>
    ''' Supported number of channels
    ''' </summary>
    Public Property Channels As Integer
        Get
            Return mChannels
        End Get
        Set(value As Integer)
            SetField(mChannels, value, Function() Channels)
        End Set
    End Property

#End Region


#Region " Must overrides "

#Disable Warning BC40027 ' Return type of function is not CLS-compliant
    Public MustOverride Function CreatePlayer() As IWavePlayer Implements IAudioOutputInterface.CreatePlayer
#Enable Warning BC40027 ' Return type of function is not CLS-compliant

#End Region

End Class
