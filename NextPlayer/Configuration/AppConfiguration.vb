Imports System.ComponentModel
Imports System.IO
Imports System.Reflection
Imports AudioChannelLibrary
Imports AudioPlayerLibrary
Imports Common
Imports PlayerActions
Imports WpfResources


''' <summary>
''' A collection of configuration settings, accessible from UI.
''' </summary>
''' <remarks>
''' This class contains:
''' * Mapped My.Settings properties (including EnvironmentName)
''' * A copy of EnvironmentConfigurationSection items
''' * CurrentEnvironment reference (refered to by EnvironmentName)
''' 
''' Use ConfigBindingExtension to get to both global properties as well as
''' current environment properties;
''' make sure their names are all different, otherwise the global will preceed.
''' 
''' Use SkinBindingExtension and SkinColorBindingExtension to retrieve
''' current skin properties.
''' </remarks>
Public Class AppConfiguration
    Inherits DependencyObject
    Implements IConfiguration, IPresenterStaticConfiguration, IVoiceConfiguration

#Region " Constants "

    Private Const DefaultSkinUri = "pack://application:,,,/Resources/Skins/DarkSkin.tps"


    Public Const AppName = "NexT Player"

#End Region


#Region " Common properties "

#Region " LastPlaylistFile dependency property "

    Public Shared ReadOnly LastPlaylistFileProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(LastPlaylistFile), GetType(String), GetType(AppConfiguration),
        New PropertyMetadata(New PropertyChangedCallback(AddressOf LastPlaylistFileChanged)))


    ' ReSharper disable once MemberCanBeInternal
    <SaveOnChange>
    <Category("Common Properties"), Description("Last opened playlist")>
    Public Property LastPlaylistFile As String
        Get
            Return CStr(GetValue(LastPlaylistFileProperty))
        End Get
        Set(value As String)
            SetValue(LastPlaylistFileProperty, value)
        End Set
    End Property


    Private Shared Sub LastPlaylistFileChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
        Dim this = CType(obj, AppConfiguration)
        this.SaveIfNeeded(Function() this.LastPlaylistFile)
    End Sub

#End Region


#Region " SaveTick dependency property "

    Public Shared ReadOnly SaveTickProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(SaveTick), GetType(Integer), GetType(AppConfiguration),
        New PropertyMetadata(1 * MillisecondsInSecond, AddressOf SaveTickChanged))


    ' ReSharper disable once MemberCanBeInternal
    <Category("Common Properties"), Description("Background save interval, if SavePlaylistOnChange is set (milliseconds)")>
    Public Property SaveTick As Integer
        Get
            Return CInt(GetValue(SaveTickProperty))
        End Get
        Set(value As Integer)
            SetValue(SaveTickProperty, value)
        End Set
    End Property


    Private Shared Sub SaveTickChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
        Dim this = CType(obj, AppConfiguration)
        this.SaveIfNeeded(Function() this.SaveTick)
    End Sub

#End Region


#Region " PlaybackTick dependency property "

    Public Shared ReadOnly PlaybackTickProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(PlaybackTick), GetType(Integer), GetType(AppConfiguration),
        New PropertyMetadata(CInt(0.1 * MillisecondsInSecond), AddressOf PlaybackTickChanged))


    ' ReSharper disable once MemberCanBeInternal
    <Category("Common Properties"), Description("Time and playback progress update interval (milliseconds)")>
    Public Property PlaybackTick As Integer
        Get
            Return CInt(GetValue(PlaybackTickProperty))
        End Get
        Set(value As Integer)
            SetValue(PlaybackTickProperty, value)
        End Set
    End Property


    Private Shared Sub PlaybackTickChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
        Dim this = CType(obj, AppConfiguration)
        this.SaveIfNeeded(Function() this.PlaybackTick)
    End Sub

#End Region


#Region " ReferenceIconOpacity dependency property "

    Public Shared ReadOnly ReferenceIconOpacityProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(ReferenceIconOpacity), GetType(Double), GetType(AppConfiguration),
        New PropertyMetadata(0.6, AddressOf ReferenceIconOpacityChanged))


    ' ReSharper disable once MemberCanBeInternal
    <Category("Common Properties"), Description("Opacity of reference icons in the playlist.")>
    Public Property ReferenceIconOpacity As Double
        Get
            Return CInt(GetValue(ReferenceIconOpacityProperty))
        End Get
        Set(value As Double)
            SetValue(ReferenceIconOpacityProperty, value)
        End Set
    End Property


    Private Shared Sub ReferenceIconOpacityChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
        Dim this = CType(obj, AppConfiguration)
        this.SaveIfNeeded(Function() this.ReferenceIconOpacity)
    End Sub

#End Region


#Region " DefaultFollowAction saved property "

    Private mDefaultFollowAction As FollowActionTypes = FollowActionTypes.ActiveProducer


    ' ReSharper disable once MemberCanBeInternal

    ''' <summary>
    ''' Default value for FollowAction.
    ''' </summary>
    Public Property DefaultFollowAction As FollowActionTypes
        Get
            Return mDefaultFollowAction
        End Get
        Set(value As FollowActionTypes)
            mDefaultFollowAction = value
            SaveIfNeeded(Function() DefaultFollowAction)
        End Set
    End Property

#End Region


#Region " Skin dependency property "

    ''' <summary>
    ''' The Settings are loaded in SkinConfiguration constructor.
    ''' </summary>
    Public Shared ReadOnly SkinProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(Skin), GetType(SkinConfiguration), GetType(AppConfiguration),
        New PropertyMetadata(New SkinConfiguration()))


    <Category("Common Properties"), Description("Skin information - colours and fonts")>
    Public Property Skin As SkinConfiguration
        Get
            Return CType(GetValue(SkinProperty), SkinConfiguration)
        End Get
        Set(value As SkinConfiguration)
            SetValue(SkinProperty, value)
        End Set
    End Property


    Public ReadOnly Property SkinGeneric As Object Implements IConfiguration.Skin
        Get
            Return Skin
        End Get
    End Property

#End Region


#Region " UseUpdateTimer dependency property "

    Public Shared ReadOnly UseUpdateTimerProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(UseUpdateTimer), GetType(Boolean), GetType(AppConfiguration))


    <Category("Common Properties"), Description("PowerPoint slide update flag")>
    Public Property UseUpdateTimer As Boolean Implements IPresenterStaticConfiguration.UseUpdateTimer
        Get
            Return CBool(GetValue(UseUpdateTimerProperty))
        End Get
        Set(value As Boolean)
            SetValue(UseUpdateTimerProperty, value)
        End Set
    End Property

#End Region


#Region " PowerPointUpdateInterval dependency property "

    Public Shared ReadOnly PowerPointUpdateIntervalProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(PowerPointUpdateInterval), GetType(Integer), GetType(AppConfiguration),
        New FrameworkPropertyMetadata(4))


    <Category("Common Properties"), Description("PowerPoint slide update interval [minutes]")>
    Public Property PowerPointUpdateInterval As Integer Implements IPresenterStaticConfiguration.PowerPointUpdateInterval
        Get
            Return CInt(GetValue(PowerPointUpdateIntervalProperty))
        End Get
        Set(value As Integer)
            SetValue(PowerPointUpdateIntervalProperty, value)
        End Set
    End Property

#End Region


#Region " IConfiguration.CurrentSkinPath property "

    ''' <summary>
    ''' Set new skin path and load the skin.
    ''' </summary>
    <SaveOnChange>
    Public Property CurrentSkinPath As String Implements IConfiguration.CurrentSkinPath
        Get
            Return My.Settings.CurrentSkin
        End Get
        Set(value As String)
            My.Settings.CurrentSkin = value
            SaveIfNeeded(Function() CurrentSkinPath)
            LoadSkinFromAppConfig()
        End Set
    End Property

#End Region


#Region " VoiceCommands read-only dependency property "

    Public Shared ReadOnly VoiceCommandsPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(VoiceCommands), GetType(VoiceCommandConfigItemCollection), GetType(AppConfiguration),
        New FrameworkPropertyMetadata(New VoiceCommandConfigItemCollection()))


    Public Shared ReadOnly VoiceCommandsProperty As DependencyProperty = VoiceCommandsPropertyKey.DependencyProperty


    <Category("Common Properties"), Description("A list of voice command definitions")>
    Public Property VoiceCommands As VoiceCommandConfigItemCollection Implements IVoiceConfiguration.VoiceCommands
        Get
            Return CType(GetValue(VoiceCommandsProperty), VoiceCommandConfigItemCollection)
        End Get
        Private Set(value As VoiceCommandConfigItemCollection)
            SetValue(VoiceCommandsPropertyKey, value)
        End Set
    End Property

#End Region


#Region " EnvironmentSettingsList dependency property "

    Private Shared ReadOnly EnvironmentSettingsListPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(EnvironmentSettingsList), GetType(BindingList(Of AppEnvironmentConfiguration)), GetType(AppConfiguration),
        New PropertyMetadata(New BindingList(Of AppEnvironmentConfiguration)()))


    Public Shared ReadOnly EnvironmentSettingsListProperty As DependencyProperty = EnvironmentSettingsListPropertyKey.DependencyProperty


    <SaveOnChange>
    <Category("Common Properties"), Description("A list of environment-dependant settings")>
    Public ReadOnly Property EnvironmentSettingsList As BindingList(Of AppEnvironmentConfiguration)
        Get
            Return CType(GetValue(EnvironmentSettingsListProperty), BindingList(Of AppEnvironmentConfiguration))
        End Get
    End Property

#End Region


#Region " EnvironmentName dependency property "

    Public Shared ReadOnly EnvironmentNameProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(EnvironmentName), GetType(String), GetType(AppConfiguration),
        New PropertyMetadata(New PropertyChangedCallback(AddressOf EnvironmentNameChanged)))


    <SaveOnChange>
    <Category("Common Properties"), Description("Name of the currently set environment")>
    Public Property EnvironmentName As String Implements IConfiguration.EnvironmentName
        Get
            Return CStr(GetValue(EnvironmentNameProperty))
        End Get
        Set(value As String)
            SetValue(EnvironmentNameProperty, value)
        End Set
    End Property


    Private Shared Sub EnvironmentNameChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
        Dim this = CType(obj, AppConfiguration)
        Dim newName = CStr(args.NewValue)
        this.SaveIfNeeded(Function() this.EnvironmentName)

        Dim env = this.EnvironmentSettingsList.FirstOrDefault(Function(e) e.Name = newName)
        If env Is Nothing Then env = this.EnvironmentSettingsList.First()
        If ReferenceEquals(this.CurrentEnvironment, env) Then Return

        this.CurrentEnvironment = env
        this.EnvironmentName = env.Name

        ' If already loaded, change. If not, do that when loaded.
        If this.CurrentActionCollection Is Nothing Then Return

        this.CurrentActionCollection.EnvironmentName = env.Name
    End Sub

#End Region

#End Region


#Region " Not saved properties "

#Region " XxxStep properties "

    Public Property VolumeStep As Single = 0.05


    Public Property PanningStep As Single = 0.05


    Public Property CoordinateStep As Single = 0.05

#End Region


#Region " IVolumeConfiguration properties "

    ''' <summary>
    ''' The difference in volume below this value is ignored.
    ''' </summary>
    Public Property VolumePrecision As Single = 0.01F Implements IVolumeConfiguration.VolumePrecision


    ''' <summary>
    ''' The difference in balance below this value is ignored.
    ''' </summary>
    Public Property BalancePrecision As Single = 0.01F Implements IVolumeConfiguration.BalancePrecision

#End Region


#Region " DefaultDuration property "

    ''' <summary>
    ''' When the duration for effect is not defined.
    ''' </summary>
    Public Property DefaultDuration As TimeSpan = TimeSpan.FromSeconds(1) Implements IEffectDurationConfiguration.DefaultDuration

#End Region


#Region " UpdatedCount dependency property "

    Public Shared ReadOnly UpdatedCountProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(UpdatedCount), GetType(Integer), GetType(AppConfiguration))


    <Category("Common Properties"), Description("Changed whenever a new waveform update is received")>
    Public Property UpdatedCount As Integer
        Get
            Return CInt(GetValue(UpdatedCountProperty))
        End Get
        Set(value As Integer)
            SetValue(UpdatedCountProperty, value)
        End Set
    End Property

#End Region


#Region " PresenterVersion dependency property "

    Public Shared ReadOnly PresenterVersionProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(PresenterVersion), GetType(Integer), GetType(AppConfiguration))


    <Category("Common Properties"), Description("Changed whenever a new presenter update is received")>
    Public Property PresenterVersion As Integer Implements IPresenterStaticConfiguration.PresenterVersion
        Get
            Return CInt(GetValue(PresenterVersionProperty))
        End Get
        Set(value As Integer)
            SetValue(PresenterVersionProperty, value)
        End Set
    End Property


    Private Sub UpdatePresenterVersion(presenterIndex As Integer)
        PresenterVersion = 1 - PresenterVersion
    End Sub

#End Region


#Region " IsVoiceControlEnabled dependency property "

    Public Shared ReadOnly IsVoiceControlEnabledProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(IsVoiceControlEnabled), GetType(Boolean), GetType(AppConfiguration),
        New FrameworkPropertyMetadata(New PropertyChangedCallback(AddressOf IsVoiceControlEnabledChangedHandler)))


    <Category("Common Properties"), Description("Whether voice control is enabled; mapped from current environment")>
    Public Property IsVoiceControlEnabled As Boolean Implements IVoiceConfiguration.IsVoiceControlEnabled
        Get
            Return CBool(GetValue(IsVoiceControlEnabledProperty))
        End Get
        Set(value As Boolean)
            SetValue(IsVoiceControlEnabledProperty, value)
        End Set
    End Property


    Private Shared Sub IsVoiceControlEnabledChangedHandler(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
        Dim this = DirectCast(obj, AppConfiguration)
        this.mCurrentEnvironment.IsVoiceControlEnabled = CBool(args.NewValue)
    End Sub

#End Region


#Region " CurrentActionCollection dependency property "

    Public Shared ReadOnly CurrentActionCollectionProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(CurrentActionCollection), GetType(IPlaylist), GetType(AppConfiguration),
        New FrameworkPropertyMetadata(New PropertyChangedCallback(AddressOf CurrentPlaylistChanged)))


    <Category("Common Properties"), Description("Reference to the currently opened playlist")>
    Public Property CurrentActionCollection As IPlaylist Implements IConfiguration.CurrentActionCollection
        Get
            Return CType(GetValue(CurrentActionCollectionProperty), IPlaylist)
        End Get
        Set(value As IPlaylist)
            SetValue(CurrentActionCollectionProperty, value)
        End Set
    End Property


    <Category("Common Properties"), Description("Reference to the currently opened playlist")>
    Public Property CurrentActionCollectionTyped As PlayerActionCollection
        Get
            Return CType(GetValue(CurrentActionCollectionProperty), PlayerActionCollection)
        End Get
        Set(value As PlayerActionCollection)
            SetValue(CurrentActionCollectionProperty, value)
        End Set
    End Property


    Private Shared Sub CurrentPlaylistChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
        Dim this = DirectCast(obj, AppConfiguration)
        Dim newList = CType(args.NewValue, IPlaylist)
        this.mCurrentPlaylist = newList
    End Sub


    Private WithEvents mCurrentPlaylist As IPlaylist


    Private Sub CurrentPlaylistPropertyChanged() Handles mCurrentPlaylist.PropertyChanged, mCurrentPlaylist.CollectionChanged
        Dispatcher.BeginInvoke(Sub() InvalidateProperty(CurrentActionCollectionProperty))
    End Sub

#End Region


#Region " CurrentEnvironment read-only dependency property "

    Private Shared ReadOnly CurrentEnvironmentPropertyKey As DependencyPropertyKey = DependencyProperty.RegisterReadOnly(
        NameOf(CurrentEnvironment), GetType(AppEnvironmentConfiguration), GetType(AppConfiguration),
        New FrameworkPropertyMetadata(New AppEnvironmentConfiguration(), New PropertyChangedCallback(AddressOf CurrentEnvironmentChanged)))


    Public Shared ReadOnly CurrentEnvironmentProperty As DependencyProperty = CurrentEnvironmentPropertyKey.DependencyProperty


    <Category("Common Properties"), Description("Current set of environment-dependant settings")>
    Public Property CurrentEnvironment As INotifyPropertyChanged Implements IConfiguration.CurrentEnvironment
        Get
            Return CType(GetValue(CurrentEnvironmentProperty), INotifyPropertyChanged)
        End Get
        Private Set(value As INotifyPropertyChanged)
            SetValue(CurrentEnvironmentPropertyKey, value)
        End Set
    End Property


    ''' <summary>
    ''' Property changed listener
    ''' </summary>
    Private WithEvents mCurrentEnvironment As AppEnvironmentConfiguration


    ''' <summary>
    ''' Update related dependencies.
    ''' </summary>
    Private Shared Sub CurrentEnvironmentChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
        Dim this = DirectCast(obj, AppConfiguration)
        Dim appSett = TryCast(args.NewValue, AppEnvironmentConfiguration)
        If appSett Is Nothing Then Return

        this.mCurrentEnvironment = appSett

        SetUpAudioLib()
        this.IsVoiceControlEnabled = appSett.IsVoiceControlEnabled
        InterfaceMapper.SetInstance(Of IVoiceConfiguration)(this)
    End Sub


    ''' <summary>
    ''' Map related properties, check saving.
    ''' </summary>
    Private Sub CurrentEnvironmentPropertyChanged(sender As Object, args As PropertyChangedEventArgs) Handles mCurrentEnvironment.PropertyChanged
        Select Case args.PropertyName
            Case NameOf(AppEnvironmentConfiguration.IsVoiceControlEnabled)
                IsVoiceControlEnabled = mCurrentEnvironment.IsVoiceControlEnabled
        End Select

        Dim ownedArgs = TryCast(args, OwnedPropertyChangedEventArgs)
        Dim owner = ownedArgs?.PropertyOwner
        SaveIfNeeded(args.PropertyName, owner)
    End Sub

#End Region

#End Region


#Region " Singleton "

    Public Shared ReadOnly Property Instance As AppConfiguration = New AppConfiguration()


    Public Shared ReadOnly Property CurrentEnvironmentName As String
        Get
            Return Instance.EnvironmentName
        End Get
    End Property

#End Region


#Region " Init and clean-up "

    Private Sub New()
        AddHandler PresenterFactory.PresenterChanged, AddressOf UpdatePresenterVersion
        LoadSettings()
        AddHandler ConfigurationManager.ConfigurationChanged, AddressOf SaveSettings
    End Sub

#End Region


#Region " Settings utility "

    Private mLoading As Boolean


    ''' <summary>
    ''' Load settings from app.config.
    ''' </summary>
    Private Sub LoadSettings()
        mLoading = True

        ' Load standard configuration section
        With My.Settings
            If Not .Upgraded Then
                .Upgrade()
                .Upgraded = True
            End If

            LastPlaylistFile = .LastPlaylistFile
            SaveTick = .SaveTick
            PlaybackTick = .PlaybackTick
            DefaultFollowAction = .DefaultFollowAction
            UseUpdateTimer = .UseUpdateTimer
            PowerPointUpdateInterval = .PowerPointUpdateInterval

            VoiceCommands.CopyFrom(.VoiceCommands)
            CheckVoiceCommands(VoiceCommands)

            LoadSkinFromAppConfig()
        End With

        ' Load environment settings list
        Dim envList = EnvironmentConfigurationSection.EnvSection.Environments
        For Each envConfig In envList
            Dim envSett As New AppEnvironmentConfiguration With {
                .Name = envConfig.Name,
                .IsEditEnabled = envConfig.IsEditEnabled,
                .IsPositionChangeEnabled = envConfig.IsPositionChangeEnabled,
                .IsVoiceControlEnabled = envConfig.IsVoiceControlEnabled,
                .UseNAudio = envConfig.UseNAudio,
                .PlayerWindowPosition = envConfig.PlayerWindowPosition,
                .MainWindowSplit = envConfig.MainWindowSplit,
                .StatusWindowSplit = envConfig.StatusWindowSplit
            }

            EnvironmentSettingsList.Add(envSett)
        Next

        ' None defined - create a default one
        If EnvironmentSettingsList.IsEmpty() Then
            EnvironmentSettingsList.Add(New AppEnvironmentConfiguration())
        End If

        ' Set the current configuration. Many things are updated in this setter.
        EnvironmentName = My.Settings.EnvironmentName

        mLoading = False
    End Sub


    ''' <summary>
    ''' Save all settings in app.config.
    ''' </summary>
    Public Sub SaveSettings()
        ' Do not save while loading
        If mLoading Then Return

        ' Plain settings
        With My.Settings
            .LastPlaylistFile = LastPlaylistFile
            .SaveTick = SaveTick
            .PlaybackTick = PlaybackTick
            .DefaultFollowAction = DefaultFollowAction
            .UseUpdateTimer = UseUpdateTimer
            .PowerPointUpdateInterval = PowerPointUpdateInterval
            .VoiceCommands = VoiceCommands
        End With

        ' Rebuild environment setting collection
        Dim envSection = EnvironmentConfigurationSection.EnvSection
        Dim envList = envSection.Environments
        envList.Clear()

        For Each envSett In EnvironmentSettingsList
            Dim envConfig As New EnvironmentConfigurationElement With {
                .Name = envSett.Name,
                .IsEditEnabled = envSett.IsEditEnabled,
                .IsPositionChangeEnabled = envSett.IsPositionChangeEnabled,
                .IsVoiceControlEnabled = envSett.IsVoiceControlEnabled,
                .UseNAudio = envSett.UseNAudio,
                .PlayerWindowPosition = envSett.PlayerWindowPosition,
                .MainWindowSplit = envSett.MainWindowSplit,
                .StatusWindowSplit = envSett.StatusWindowSplit
            }

            envList.Add(envConfig)
        Next

        envSection.Save()
        My.Settings.Save()
    End Sub


    ''' <summary>
    ''' Save settings immediatelly, if the property being changed is configured for this.
    ''' This is done by marking the property with SaveOnChangeAttribute.
    ''' </summary>
    Private Sub SaveIfNeeded(Of T)(prop As Expressions.Expression(Of Func(Of T)), Optional sender As Object = Nothing)
        Dim propName = PropertyChangedHelper.GetPropertyName(prop)
        SaveIfNeeded(propName, sender)
    End Sub


    ''' <summary>
    ''' Save settings immediatelly, if the property being changed is configured for this.
    ''' This is done by marking the property with SaveOnChangeAttribute.
    ''' </summary>
    Private Sub SaveIfNeeded(propName As String, Optional sender As Object = Nothing)
        If sender Is Nothing Then sender = Me
        Dim pi = sender.GetType().GetProperty(propName)

        ' If the property is not found, let's hope the changes are saved by other means
        ' (e.g. ConfigurationChanged event)
        If pi Is Nothing Then Return

        ' The property exists and not marked
        If pi.GetCustomAttribute(Of SaveOnChangeAttribute)() Is Nothing Then Return

        SaveSettings()
    End Sub


    ''' <summary>
    ''' Create a clone of the current environment configuration,
    ''' under a new name, and add it to the list.
    ''' </summary>
    Public Sub CloneCurrentEnvironment(newName As String)
        Dim newEnv = mCurrentEnvironment.Clone(Of AppEnvironmentConfiguration)()
        newEnv.Name = newName
        EnvironmentSettingsList.Add(newEnv)
    End Sub

#End Region


#Region " Audio utility "

    Public Shared Sub SetUpAudioLib()
        If Instance Is Nothing Then Return

        InterfaceMapper.SetType(Of IAudioOutputInterface, WaveOutAudioInterface)()

        If CType(Instance.CurrentEnvironment, AppEnvironmentConfiguration).UseNAudio Then
            ' NAudio
            InterfaceMapper.SetType(Of IAudioPlayer, NAudioPlayer)()
        Else
            ' Standard MediaPlayer
            InterfaceMapper.SetType(Of IAudioPlayer, MediaPlayerWrapper)()
        End If
    End Sub

#End Region


#Region " Event subscription "

    Private WithEvents mWaveformInstance As New WaveformStorage()


    Private Sub GeometryUpdatedHandler(fileName As String) Handles mWaveformInstance.GeometryUpdated
        UpdatedCount = 1 - UpdatedCount
    End Sub

#End Region


#Region " Skin utilities "

    ''' <summary>
    ''' Copy all skin settings from app.config.
    ''' </summary>
    Public Sub LoadSkinFromAppConfig()
        Dim path = My.Settings.CurrentSkin
        Dim skinStream As Stream

        If String.IsNullOrWhiteSpace(path) Or Not File.Exists(path) Then
            skinStream = Application.GetResourceStream(New Uri(DefaultSkinUri)).Stream
        Else
            skinStream = File.OpenRead(path)
        End If

        Skin.Load(skinStream)
        skinStream.Close()
    End Sub

#End Region


#Region " Voice command utility "

    ''' <summary>
    ''' Add missing commands, remove non-existing ones.
    ''' </summary>
    ''' <remarks>Uses <see cref="AppCommandList"/></remarks>
    Private Shared Sub CheckVoiceCommands(cmdList As VoiceCommandConfigItemCollection)
        Dim newList As New VoiceCommandConfigItemCollection

        For Each registeredCmd In AppCommandList
            Dim toAdd = cmdList.FirstOrDefault(Function(c) c.CommandName = registeredCmd.CommandName)

            If toAdd Is Nothing Then
                toAdd = New VoiceCommandConfigItem(registeredCmd.CommandName, registeredCmd.DefaultText) With {
                    .IsEnabled = False
                }
            End If

            newList.Add(toAdd)
        Next

        cmdList.Clear()

        For Each newItem In newList
            cmdList.Add(newItem)
        Next
    End Sub

#End Region

End Class
