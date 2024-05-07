Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.IO
Imports System.Xml.Serialization
Imports AudioChannelLibrary
Imports AudioPlayerLibrary
Imports Common


''' <summary>
''' An action to play audio from a file.
''' </summary>
<Serializable()>
Public Class PlayerActionFile
    Inherits PlayerAction
    Implements IInputFile, ISoundProducer, IPositionRelative

#Region " Fields "

    Private Const ResetPosition As Double = -1

    Private mCurrentPlayPosition As Double = ResetPosition

#End Region


#Region " ISoundProducer.PositionChanged implementation "

    <NonSerialized()>
    Public Event PositionChanged(sender As ISoundProducer) Implements ISoundProducer.PositionChanged

#End Region


#Region " ISoundProducer.EndReached implementation "

    <NonSerialized()>
    Public Event EndReached(sender As ISoundProducer) Implements ISoundProducer.EndReached


    ''' <summary>
    ''' Simulate reaching the end of file.
    ''' </summary>
    Public Sub SimulateEndReached() Implements ISoundProducer.SimulateEndReached
        mPlayer?.Execute(Sub() MediaEndedHandler(Me, Nothing))
    End Sub

#End Region


#Region " Persistent properties "

#Region " FileToPlay notifying property "

    Private mFileToPlay As String


    ''' <summary>
    ''' Path to file to be played, relative.
    ''' </summary>
    <IgnoreForReport()>
    Public Property FileToPlay As String
        Get
            Return mFileToPlay
        End Get
        Set(value As String)
            If String.IsNullOrEmpty(value) OrElse mFileToPlay = value Then Return

            mFileToPlay = value
            RaisePropertyChanged(Function() FileToPlay)
            ShortFileName = Path.GetFileName(mFileToPlay)
        End Set
    End Property


#End Region


#Region " FileTimestamp notifying property "

    Private mFileTimestamp As DateTime


    ''' <summary>
    ''' File modified timestamp, to check for updates.
    ''' </summary>
    <IgnoreForReport()>
    Public Property FileTimestamp As DateTime Implements IInputFile.FileTimestamp
        Get
            Return mFileTimestamp
        End Get
        Set(value As DateTime)
            If mFileTimestamp = value Then Return

            mFileTimestamp = value
            RaisePropertyChanged(Function() FileTimestamp)
        End Set
    End Property


#End Region


#Region " Channel notifying property "

    Private mChannel As Integer = 1


    <XmlIgnore>
    <IgnoreForReport()>
    Public Property Channel As Integer
        Get
            Return mChannel
        End Get
        Set(value As Integer)
            If mChannel = value Then Return

            mChannel = value
            SetEffectivePlayerParameters()
            RaisePropertyChanged(Function() Channel)
        End Set
    End Property

#End Region


#Region " IsMuted notifying property "

    Private mIsMuted As Boolean = False


    ''' <summary>
    ''' Whether the sound is muted.
    ''' </summary>
    Public Property IsMuted As Boolean Implements ISoundProducer.IsMuted
        Get
            Return mIsMuted
        End Get
        Set(value As Boolean)
            If mIsMuted = value Then Return

            mIsMuted = value
            SetEffectivePlayerParameters()
            RaisePropertyChanged(Function() IsMuted)
        End Set
    End Property

#End Region


#Region " Volume notifying property "

    Private mVolume As Single = 1


    ''' <summary>
    ''' Volume (0-1).
    ''' </summary>
    Public Property Volume As Single Implements IVolumeController.Volume
        Get
            Return mVolume
        End Get
        Set(value As Single)
            Dim config = InterfaceMapper.GetImplementation(Of IVolumeConfiguration)()
            If Math.Abs(mVolume - value) < config.VolumePrecision Then Return

            mVolume = value
            RaisePropertyChanged(Function() Volume)

            SetEffectiveVolume(Nothing, value)
        End Set
    End Property

#End Region


#Region " SoundPositionMode notifying property "

    Private mSoundPositionModes As SoundPositionModes = SoundPositionModes.FixedToChannels


    Public Property SoundPositionMode As SoundPositionModes
        Get
            Return mSoundPositionModes
        End Get
        Set(value As SoundPositionModes)
            If mSoundPositionModes = value Then Return

            mSoundPositionModes = value
            RaisePropertyChanged(Function() SoundPositionMode)

            SetEffectivePlayerParameters()
        End Set
    End Property

#End Region


#Region " Balance notifying property "

    Private mBalance As Double = 0


    ''' <summary>
    ''' Stereo balance (-1 .. 1).
    ''' </summary>
    ''' <remarks>Only has effect when <see cref="SoundPositionMode"/> is False.</remarks>
    Public Property Balance As Double Implements ISoundProducer.Balance
        Get
            Return mBalance
        End Get
        Set(value As Double)
            Dim config = InterfaceMapper.GetImplementation(Of IVolumeConfiguration)()
            If Math.Abs(mBalance - value) < config.BalancePrecision Then Return

            mBalance = value
            RaisePropertyChanged(Function() Balance)

            SetEffectivePlayerParameters()
        End Set
    End Property

#End Region


#Region " X notifying property "

    Private mX As Single


    ''' <summary>
    ''' X-location of the simulated playback
    ''' (in percent, 0 is center, -1 is left wall, 1 is right wall).
    ''' </summary>
    Public Property X As Single Implements IPositionRelative.X
        Get
            Return mX
        End Get
        Set(value As Single)
            If IsEqual(mX, value) Then Return

            mX = value
            RaisePropertyChanged(Function() X)

            SetEffectivePlayerParameters()
        End Set
    End Property

#End Region


#Region " Y notifying property "

    Private mY As Single


    ''' <summary>
    ''' Y-location of the simulated playback
    ''' (in percent, 0 is center, -1 is back wall, +1 is front wall).
    ''' </summary>
    Public Property Y As Single Implements IPositionRelative.Y
        Get
            Return mY
        End Get
        Set(value As Single)
            If IsEqual(mY, value) Then Return

            mY = value
            RaisePropertyChanged(Function() Y)

            SetEffectivePlayerParameters()
        End Set
    End Property

#End Region


#Region " Z notifying property "

    Private mZ As Single


    ''' <summary>
    ''' Z-location of the simulated playback
    ''' (in percent, 0 is center, -1 is floor, +1 is ceiling).
    ''' </summary>
    Public Property Z As Single Implements IPositionRelative.Z
        Get
            Return mZ
        End Get
        Set(value As Single)
            If IsEqual(mZ, value) Then Return

            mZ = value
            RaisePropertyChanged(Function() Z)

            SetEffectivePlayerParameters()
        End Set
    End Property

#End Region


#Region " StartPosition notifying property "

    Private mStartPosition As TimeSpan


    ''' <summary>
    ''' From which position the playback should start.
    ''' </summary>
    <XmlIgnore()>
    Public Property StartPosition As TimeSpan
        Get
            Return mStartPosition
        End Get
        Set(value As TimeSpan)
            If mStartPosition = value Then Return
            mStartPosition = value
            RaisePropertyChanged(Function() StartPosition)
            RaisePropertyChanged(Function() StartPositionMilliseconds)
        End Set
    End Property


    ''' <summary>
    ''' From which position the playback should start.
    ''' </summary>
    <XmlIgnore()>
    <IgnoreForReport()>
    Public Overrides ReadOnly Property StartPositionReadOnly As TimeSpan
        Get
            Return StartPosition
        End Get
    End Property


    ''' <summary>
    ''' Serializable StartPosition.
    ''' </summary>
    ''' <remarks>To work around inability to serialize TimeSpan.</remarks>
    <XmlElement(NameOf(StartPosition))>
    <IgnoreForReport()>
    Public Property StartPositionMilliseconds As Double
        Get
            Return StartPosition.TotalMilliseconds()
        End Get
        Set(value As Double)
            StartPosition = TimeSpan.FromMilliseconds(value)
        End Set
    End Property

#End Region


#Region " StopPosition notifying property "

    Private mStopPosition As TimeSpan


    ''' <summary>
    ''' At which position the playback should stop, counting from the end of the song towards the beginning.
    ''' </summary>
    <XmlIgnore()>
    Public Property StopPosition As TimeSpan
        Get
            Return mStopPosition
        End Get
        Set(value As TimeSpan)
            If mStopPosition = value Then Return
            mStopPosition = value
            RaisePropertyChanged(Function() StopPosition)
            RaisePropertyChanged(Function() StopPositionMilliseconds)
        End Set
    End Property


    ''' <summary>
    ''' At which position the playback should stop, counting from the end of the song towards the beginning.
    ''' </summary>
    <XmlIgnore()>
    <IgnoreForReport()>
    Public Overrides ReadOnly Property StopPositionReadOnly As TimeSpan
        Get
            Return StopPosition
        End Get
    End Property


    ''' <summary>
    ''' Serializable StopPosition.
    ''' </summary>
    ''' <remarks>To work around inability to serialize TimeSpan.</remarks>
    <XmlElement(NameOf(StopPosition))>
    <IgnoreForReport()>
    Public Property StopPositionMilliseconds As Double
        Get
            Return StopPosition.TotalMilliseconds()
        End Get
        Set(value As Double)
            StopPosition = TimeSpan.FromMilliseconds(value)
        End Set
    End Property

#End Region

#End Region


#Region " Non-persistent properties "

#Region " EffectiveVolume notifying property "

    Private mEffectiveVolume As Double = 1


    ''' <summary>
    ''' Effective volume (0-1), used for volume animation.
    ''' </summary>
    <XmlIgnore()>
    <IgnoreForReport()>
    Public ReadOnly Property EffectiveVolume As Double Implements ISoundProducer.EffectiveVolume
        Get
            ' Would show effect from effects in the list,
            ' but not autogenerated ones like during crossfade.
            Return If(Effects.Any() OrElse GeneratedEffects.Any(), mEffectiveVolume, mVolume)
        End Get
    End Property


    ''' <inheritdoc/>
    Public Function GetEffectiveVolume() As Double Implements ISoundProducer.GetEffectiveVolume
        If mCurrentPlayPosition <> PlayPosition.TotalMilliseconds Then
            ' First request for the new position
            mCurrentPlayPosition = PlayPosition.TotalMilliseconds
            Return mVolume
        Else
            Return mEffectiveVolume
        End If
    End Function


    ''' <inheritdoc/>
    Public Sub SetEffectiveVolume(sender As ISoundAutomation, newVolume As Double) Implements ISoundProducer.SetEffectiveVolume
        mEffectiveVolume = newVolume

        Dim lastEffect = (From ef In Effects Where ef.IsPlaying).LastOrDefault()
        If lastEffect IsNot Nothing AndAlso sender IsNot Nothing AndAlso sender IsNot lastEffect Then
            Return
        End If

        SetEffectivePlayerParameters()
        ' To avoid applying the same effects several times
        mCurrentPlayPosition = ResetPosition
        RaisePropertyChanged(Function() EffectiveVolume)
    End Sub

#End Region


#Region " Effects notifying property "

    Private WithEvents mEffects As New ObservableCollection(Of ISoundAutomation)()


    <XmlIgnore()>
    Public Property Effects As IList(Of ISoundAutomation) Implements ISoundProducer.Effects
        Get
            Return mEffects
        End Get
        Set(value As IList(Of ISoundAutomation))
            mEffects.Clear()

            For Each item In value
                mEffects.Add(item)
            Next

            RaisePropertyChanged(Function() Effects)
        End Set
    End Property


    Private Sub EffectsChangedHandler() Handles mEffects.CollectionChanged
        RaisePropertyChanged(Function() Effects)
    End Sub

#End Region


    Public ReadOnly Property GeneratedEffects As IList(Of ISoundAutomation) =
        New List(Of ISoundAutomation)() Implements ISoundProducer.GeneratedEffects


#Region " LastRootPath property "

    <XmlIgnore>
    Public Property LastRootPath As String Implements IInputFile.LastRootPath

#End Region


#Region " ShortFileName read-only notifying non-persistent property  "

    Private mShortFileName As String


    ''' <summary>
    ''' Get user-friendly name of the file.
    ''' </summary>
    <XmlIgnore()>
    Public Property ShortFileName As String
        Get
            Return mShortFileName
        End Get
        Set(value As String)
            If mShortFileName = value Then Return

            mShortFileName = value
            RaisePropertyChanged(Function() ShortFileName)
        End Set
    End Property

#End Region


#Region " AbsFileToPlay notifying non-persistent property "

    Private mAbsFileToPlay As String


    ''' <summary>
    ''' Path to file to be played, absolute.
    ''' Not serialized.
    ''' Notifying for XAML sake.
    ''' </summary>
    <XmlIgnore()>
    Public Property AbsFileToPlay As String
        Get
            Return mAbsFileToPlay
        End Get
        Set(value As String)
            If value = mAbsFileToPlay Then Return

            mAbsFileToPlay = value
            RaisePropertyChanged(Function() AbsFileToPlay)
        End Set
    End Property


    <XmlIgnore()>
    <IgnoreForReport()>
    Public ReadOnly Property AbsFileToPlayReadOnly As String Implements IInputFile.FileName
        Get
            Return AbsFileToPlay
        End Get
    End Property

#End Region


#Region " IsLoadingFailed non-persistent notifying property "

    Private mIsLoadingFailed As Boolean


    <XmlIgnore()>
    <Description("Whether there was a problem loading the file")>
    Public Property IsLoadingFailed As Boolean Implements IInputFile.IsLoadingFailed
        Get
            Return mIsLoadingFailed
        End Get
        Set(value As Boolean)
            If mIsLoadingFailed = value Then Return
            mIsLoadingFailed = value
            RaisePropertyChanged(Function() IsLoadingFailed)
        End Set
    End Property

#End Region


#Region " MessageLog property "

    Private mMessageLog As IMessageLog


    Private ReadOnly Property MessageLog As IMessageLog
        Get
            If mMessageLog Is Nothing Then
                mMessageLog = InterfaceMapper.GetImplementation(Of IMessageLog)()
            End If

            Return mMessageLog
        End Get
    End Property

#End Region

#End Region


#Region " Player field and PlayPosition methods "

    Private mPlayer As IAudioPlayer


    Private mPlaylistAudioConfig As IAudioChannelStorage



    ''' <summary>
    ''' Override it to update the playback position from inside the action.
    ''' Use SetPlayPosition(...) to set the value and raise the PropertyChanged.
    ''' </summary>
    Public Overrides Sub UpdatePlayPosition()
        If mPlayer Is Nothing Then Return

        mPlayer.Execute(
            Sub()
                If mPlayer Is Nothing Then Return
                SetPlayPosition(mPlayer.Position)
                If StopPositionMilliseconds > 0 AndAlso mPlayer.Position >= Duration - StopPosition Then StopPlayer(False)
            End Sub)
    End Sub


    Protected Overrides Sub OnPlayPositionChanged()
        If mPlayer Is Nothing Then Return

        mPlayer.Execute(Sub() If mPlayer IsNot Nothing Then mPlayer.Position = PlayPosition)
        RaiseEvent PositionChanged(Me)
    End Sub

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Empty constructor for serialization.
    ''' </summary>
    Public Sub New()
        ' Do nothing
    End Sub


    ''' <summary>
    ''' Set all defaults.
    ''' </summary>
    Public Sub New(fileName As String)
        FileToPlay = fileName
        ' ShortFileName set by FileToPlay setter
        Name = Path.GetFileNameWithoutExtension(ShortFileName)
        DelayType = DelayTypes.Manual
    End Sub

#End Region


#Region " Load / save operations preparations "

    ''' <summary>
    ''' Set AbsFileToPlay to absolute, according to the load path.
    ''' Check modification date.
    ''' </summary>
    Public Sub AfterLoad(rootPath As String) Implements IInputFile.AfterLoad
        ' Get the timestamp for non-existing file (not Date.MinValue)
        Static noDate As Date = File.GetLastWriteTime(Path.GetRandomFileName())

        Dim lastDate As Date
        AbsFileToPlay = FileUtility.RelPathToAbsolute(FileToPlay, rootPath, lastDate)
        LastRootPath = rootPath

        If lastDate <> FileTimestamp OrElse lastDate = noDate OrElse Duration = TimeSpan.Zero OrElse IsLoadingFailed Then
            DurationLibrary.GetDuration(Me)
            FileTimestamp = lastDate
        End If
    End Sub


    ''' <summary>
    ''' Set FileToplay to relative, according to the new save path.
    ''' </summary>
    Public Sub BeforeSave(rootPath As String) Implements IInputFile.BeforeSave
        FileToPlay = FileUtility.AbsPathToRelative(AbsFileToPlay, rootPath)
        LastRootPath = rootPath
    End Sub

#End Region


#Region " PlayerAction overrides "

    ''' <inheritdoc/>
    Public Overrides Sub PrepareStart()
        MyBase.PrepareStart()

        If mPlayer Is Nothing Then
            mPlayer = CreatePlayer()
            If mPlayer Is Nothing Then Return
        Else
            MediaOpenedHandler(Me, Nothing)
        End If

        mPlayer.Execute(Sub()
                            mPlayer.PlaybackInfo = New AudioPlaybackInfo With {
                                .ActionName = Name
                            }
                            SetPlaybackInfo(Volume)
                            SetPlayPosition(StartPosition)
                        End Sub)

        mPlayer.Open(AbsFileToPlay, mPlaylistAudioConfig.Logical.Channel(Channel).Channel)
    End Sub


    ''' <summary>
    ''' Start producing the sound.
    ''' </summary>
    Public Overrides Sub Start()
        MyBase.Start()

        ' Can happen here that the player is stopped due to a loading error
        mPlayer?.Execute(Sub()
                             mCurrentPlayPosition = ResetPosition
                             mPlayer.Position = PlayPosition
                         End Sub)

        ' Can also happen here that the player is stopped due to a loading error
        mPlayer?.Play()
    End Sub


    ''' <summary>
    ''' Stop producing the sound.
    ''' </summary>
    Public Overrides Sub [Stop](intendedResume As Boolean)
        If intendedResume And mPlayer IsNot Nothing Then
            mPlayer.Pause()
        Else
            StopPlayer(True)
            SetPlayPosition(StartPosition)
        End If

        MyBase.Stop(intendedResume)
    End Sub


    ''' <summary>
    ''' Modify volume by the given delta (%).
    ''' </summary>
    Public Overrides Sub ModifyVolume(delta As Single)
        Volume = CalculateVolume(Volume, delta)
    End Sub

#End Region


#Region " Utility "

    ''' <summary>
    ''' Create a player and start the playback.
    ''' </summary>
    Private Function CreatePlayer() As IAudioPlayer
        If Not File.Exists(AbsFileToPlay) Then
            IsLoadingFailed = True
            Return Nothing
        End If

        mPlayer = InterfaceMapper.GetImplementation(Of IAudioPlayer)()
        mPlaylistAudioConfig = InterfaceMapper.GetImplementation(Of IAudioChannelStorage)()

        AddHandler mPlayer.MediaOpened, AddressOf MediaOpenedHandler
        AddHandler mPlayer.MediaEnded, AddressOf MediaEndedHandler
        AddHandler mPlayer.MediaFailed, AddressOf MediaFailedHandler

        IsLoadingFailed = False

        Return mPlayer
    End Function


    ''' <summary>
    ''' Stop the playback, unregister event handlers.
    ''' </summary>
    ''' <param name="asCommand">If not UI command (False), send the EndReached event.</param>
    Private Sub StopPlayer(asCommand As Boolean)
        If mPlayer Is Nothing Then Return

        RemoveHandler mPlayer.MediaOpened, AddressOf MediaOpenedHandler
        RemoveHandler mPlayer.MediaEnded, AddressOf MediaEndedHandler
        RemoveHandler mPlayer.MediaFailed, AddressOf MediaFailedHandler

        mPlayer.Stop()
        mPlayer.Dispose()
        mPlayer = Nothing

        If Not asCommand Then
            MyBase.Stop(False)
            RaiseEvent EndReached(Me)
        End If
    End Sub


    ''' <summary>
    ''' Set volume and other flags during playback.
    ''' </summary>
    Private Sub SetEffectivePlayerParameters()
        If mPlayer Is Nothing Then Return

        mPlayer.Execute(Sub() SetPlaybackInfo(CSng(EffectiveVolume)))
    End Sub


    ''' <summary>
    ''' Set volume, mute state and panning into <see cref="IAudioPlayer.PlaybackInfo"/>.
    ''' </summary>
    Private Sub SetPlaybackInfo(actualVolume As Single)
        If mPlayer Is Nothing Then Return

        With mPlayer.PlaybackInfo
            .IsMuted = IsMuted
            .Volume = actualVolume

            Select Case SoundPositionMode
                Case SoundPositionModes.FixedToChannels
                    .PanningModel = PanningModels.Fixed

                Case SoundPositionModes.Panning
                    .Panning = CSng(Balance)
                    .PanningModel = PanningModels.ConstantPower

                Case SoundPositionModes.Coordinates
                    .X = X
                    .Y = Y
                    .Z = Z
                    .PanningModel = PanningModels.Coordinates
            End Select
        End With
    End Sub


#End Region


#Region " Event listeners "

    ''' <summary>
    ''' File opened - go along and say it.
    ''' </summary>
    Private Sub MediaOpenedHandler(sender As Object, args As EventArgs)
        HasDuration = mPlayer.NaturalDuration.HasTimeSpan
        If HasDuration Then
            Duration = mPlayer.NaturalDuration.TimeSpan
        End If
    End Sub


    ''' <summary>
    ''' Playback ended.
    ''' </summary>
    Private Sub MediaEndedHandler(sender As Object, args As MediaEndedEventArgs)
        StopPlayer(False)

        If args Is Nothing OrElse String.IsNullOrEmpty(args.ErrorMessage) Then Return

        MessageLog.LogAudioError("{0}: {1}", Path.GetFileName(args.FileName), args.ErrorMessage)
    End Sub


    ''' <summary>
    ''' There was an error.
    ''' </summary>
    Private Sub MediaFailedHandler(sender As Object, args As MediaFailedEventArgs)
        MessageLog.LogFileError("{0}: {1}", Path.GetFileName(args.FileName), args.Reason)
        IsLoadingFailed = True
        StopPlayer(False)
    End Sub

#End Region


#Region " ToString "

    ''' <summary>
    ''' For debugging.
    ''' </summary>
    Public Overrides Function ToString() As String
        Dim prefix = If(ExecutionType = ExecutionTypes.Parallel, String.Format("({0}) ", ParallelIndex), String.Empty)
        Return String.Format("{0}{1}{2}", prefix, Name, ShortFileName)
    End Function

#End Region

End Class
