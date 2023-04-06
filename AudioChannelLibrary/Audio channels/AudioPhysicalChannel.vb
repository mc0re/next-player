Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Common
Imports NAudio.Wave
Imports NAudio.Wave.SampleProviders


<Serializable>
<CLSCompliant(True)>
Public Class AudioPhysicalChannel
    Inherits ChannelBase
    Implements IPhysicalChannel, IVolumeController, IPoint3D

#Region " Constants "

    Private Const DefaultDeviceName As String = "Audio device"

#End Region


#Region " AudioInterfaceType notifying property "

    <NonSerialized>
    Private mAudioInterfaceType As TypeImplementationInfo


    ''' <summary>
    ''' Used to change the interface.
    ''' </summary>
    <XmlIgnore>
    Public Property AudioInterfaceType As TypeImplementationInfo
        Get
            Return mAudioInterfaceType
        End Get
        Set(value As TypeImplementationInfo)
            If mAudioInterfaceType = value Then Return

            SetField(mAudioInterfaceType, value, Function() AudioInterfaceType)

            AudioInterface = CType(
                Activator.CreateInstance(value.ImplementingType),
                AudioOutputInterfaceBase)
        End Set
    End Property

#End Region


#Region " AudioInterfaceTypeList shared notifying property "

    ''' <summary>
    ''' Is a Friend to allow unit test inject own player implementation.
    ''' </summary>
    Private Shared ReadOnly sAudioInterfaceTypeList As New BindingList(Of TypeImplementationInfo)()


    ''' <summary>
    ''' A collection of available audio interface types.
    ''' </summary>
    <XmlIgnore>
    Public Shared Property AudioInterfaceTypeList As IList(Of TypeImplementationInfo)
        Get
            Return sAudioInterfaceTypeList
        End Get
        Set(value As IList(Of TypeImplementationInfo))
            sAudioInterfaceTypeList.Clear()

            For Each item In value
                sAudioInterfaceTypeList.Add(item)
            Next
        End Set
    End Property

#End Region


#Region " AudioInterface notifying property "

    Private WithEvents mAudioInterface As AudioOutputInterfaceBase


    ''' <summary>
    ''' The actual audio interface behind the channel.
    ''' </summary>
    Public Property AudioInterface As AudioOutputInterfaceBase
        Get
            Return mAudioInterface
        End Get
        Set(value As AudioOutputInterfaceBase)
            ' SetField is not sufficient here, as the property is not updated upon its RaisePropertyChanged
            If value.Equals(mAudioInterface) Then Return
            mAudioInterface = value
            RaisePropertyChanged(NameOf(AudioInterface))

            Dim implInfo = AudioInterfaceTypeList.FirstOrDefault(
                Function(m) m.ImplementingType = value.GetType())

            If implInfo Is Nothing Then
                implInfo = New TypeImplementationInfo With {
                    .Name = "Undefined " & value.GetType().Name,
                    .ImplementingType = value.GetType()
                    }
            End If

            AudioInterfaceType = implInfo
        End Set
    End Property


    Private Sub AudioInterfacePropertyChangedHandler(sender As Object, args As PropertyChangedEventArgs) Handles mAudioInterface.PropertyChanged
        Dim p = mAudioInterface.GetType().GetProperty(args.PropertyName)
        ' Ignore read-only properties
        If Not p.CanWrite Then Return

        RaisePropertyChanged(NameOf(AudioInterface))
    End Sub

#End Region


#Region " DeviceChannel notifying property "

    Private mDeviceChannel As Integer = 1


    ''' <summary>
    ''' The channel on the actual output device (1-based).
    ''' Max is defined by <see cref="AudioOutputInterfaceBase.Channels"/>.
    ''' </summary>
    Public Property DeviceChannel As Integer
        Get
            Return mDeviceChannel
        End Get
        Set(value As Integer)
            SetField(mDeviceChannel, value, Function() DeviceChannel)
        End Set
    End Property

#End Region


#Region " Volume notifying property "

    Private mVolume As Single = 1


    ''' <summary>
    ''' Volume multiplier, 0-1.
    ''' </summary>
    Public Property Volume As Single Implements IVolumeController.Volume
        Get
            Return mVolume
        End Get
        Set(value As Single)
            SetField(mVolume, value, Function() Volume)
        End Set
    End Property

#End Region


#Region " Delay notifying property "

    Private mDelay As Single


    ''' <summary>
    ''' Delay in milliseconds.
    ''' </summary>
    Public Property Delay As Single
        Get
            Return mDelay
        End Get
        Set(value As Single)
            SetField(mDelay, value, NameOf(Delay))
        End Set
    End Property

#End Region


#Region " ReversedPhase notifying property "

    Private mReversedPhase As Boolean


    ''' <summary>
    ''' Whether the phase should be reversed.
    ''' </summary>
    Public Property ReversedPhase As Boolean
        Get
            Return mReversedPhase
        End Get
        Set(value As Boolean)
            SetField(mReversedPhase, value, NameOf(ReversedPhase))
        End Set
    End Property

#End Region


#Region " X notifying property "

    Private mX As Double


    ''' <summary>
    ''' X-location of the actual output.
    ''' </summary>
    Public Property X As Double Implements IPoint3D.X
        Get
            Return mX
        End Get
        Set(value As Double)
            SetField(mX, value, Function() X)
        End Set
    End Property

#End Region


#Region " Y notifying property "

    Private mY As Double


    ''' <summary>
    ''' Y-location of the actual output.
    ''' </summary>
    Public Property Y As Double Implements IPoint3D.Y
        Get
            Return mY
        End Get
        Set(value As Double)
            SetField(mY, value, Function() Y)
        End Set
    End Property

#End Region


#Region " Z notifying property "

    Private mZ As Double


    ''' <summary>
    ''' Z-location of the actual output.
    ''' </summary>
    Public Property Z As Double Implements IPoint3D.Z
        Get
            Return mZ
        End Get
        Set(value As Double)
            SetField(mZ, value, Function() Z)
        End Set
    End Property

#End Region


#Region " Init and clean-up "

    Shared Sub New()
        CollectImplementations(GetType(IAudioOutputInterface), sAudioInterfaceTypeList)
    End Sub


    ''' <summary>
    ''' Create a default output.
    ''' </summary>
    Public Sub New()
        Description = DefaultDeviceName

        Dim defAudioType = InterfaceMapper.GetImplementingType(Of IAudioOutputInterface)()

        Dim ifType = AudioInterfaceTypeList.SingleOrDefault(
            Function(info) info.ImplementingType = defAudioType)

        If ifType Is Nothing Then
            Throw New ArgumentException($"Interface type '{defAudioType.Name}' is not found in the list of available implementations.")
        End If

        AudioInterfaceType = ifType
    End Sub

#End Region


#Region " API "

    Private mTestPlayer As IWavePlayer


    Public Sub PlayTestSound(Optional link As AudioChannelLink = Nothing)
        StopTestSound()

        mTestPlayer = AudioInterface.CreatePlayer()
        If mTestPlayer Is Nothing Then Return

        If link Is Nothing Then
            link = New AudioChannelLink() With {.Physical = Channel}
        End If

        Dim signal = CreateTestSoundProvider(link)
        mTestPlayer.Init(signal)
        mTestPlayer.Play()

        IsActive = True
    End Sub


    Public Sub StopTestSound()
        If mTestPlayer IsNot Nothing Then
            mTestPlayer.Stop()
            mTestPlayer.Dispose()
            mTestPlayer = Nothing
        End If

        IsActive = False
    End Sub

#End Region


#Region " Utility "

    Private Class TestSoundPlaybackInfo
        Inherits AudioPlaybackInfo

        Public Sub New()
            PanningModel = PanningModels.ConstantPower
            Panning = 0
            IsMuted = False
            Volume = 1
        End Sub

    End Class


    Private Function CreateTestSoundProvider(link As AudioChannelLink) As VolumeProvider
        ' 1 source to 1 destination
        Dim pan = CreatePanningInfo(
            1,
            New LinkResult(Of AudioChannelLink, AudioPhysicalChannel) With {.Link = link, .Physical = Me},
            0F)
        Dim panList As New List(Of SourcePanningInfo(Of Single)) From {pan}

        Dim info As New TestSoundPlaybackInfo()
        info.CoefficientGenerator = New PanningCoefficientGenerator(info, info, panList)

        ' Mono sinus generator, default frequency 440 Hz
        Dim signal As New VolumeProvider(New SignalGenerator(44100, 1), info, Me, link)
        Return signal
    End Function

#End Region

End Class
