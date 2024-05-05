Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.IO
Imports System.Reflection
Imports System.Xml.Serialization
Imports AudioChannelLibrary
Imports AudioPlayerLibrary
Imports Common
Imports LicenseLibrary
Imports TextChannelLibrary


''' <summary>
''' A list of player actions, along with all other settings
''' saved in the playlist.
''' </summary>
''' <remarks>
''' This class contains the actual list of PlayerAction items,
''' as well as a list of environments and a reference to the
''' current environment (defined by AppConfiguration.EnvironmentName).
''' 
''' Use PlaylistBindingExtension to bind to playlist and current environment properties.
''' 
''' Note that this class is serialized as is.
''' </remarks>
<Serializable()>
Public Class PlayerActionCollection
    Implements IPlaylist, IPresenterConfiguration, IAudioChannelStorage, ITextChannelStorage

#Region " INotifyPropertyChanged implementation "

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged


    ''' <summary>
    ''' Raise PropertyChanged event.
    ''' </summary>
    Private Sub RaisePropertyChanged(propName As String)
        If Not sIsUpdateEventEnabled Then Return

        Dim pi = GetType(PlayerActionCollection).GetProperty(propName)
        If pi.GetCustomAttribute(Of XmlIgnoreAttribute)() Is Nothing Then
            IsSaveNeeded = True
        End If

        Dim args As New PlayerNotifyPropertyChangedEventArgs(propName, pi)
        RaiseEvent PropertyChanged(Me, args)
    End Sub


    ''' <summary>
    ''' Helper function to raise the event.
    ''' NB. BindingList does not propagate properties of derived objects.
    ''' </summary>
    Private Sub RaisePropertyChanged(Of T)(prop As Expressions.Expression(Of Func(Of T)))
        RaisePropertyChanged(PropertyChangedHelper.GetPropertyName(prop))
    End Sub

#End Region


#Region " INotifyCollectionChanged implementation "

    Public Event CollectionChanged(sender As Object, args As NotifyCollectionChangedEventArgs) Implements INotifyCollectionChanged.CollectionChanged


    ''' <summary>
    ''' Raise PropertyChanged event with Reset event.
    ''' </summary>
    Private Sub RaiseCollectionChangedReset()
        IsSaveNeeded = IsSaveNeeded OrElse IsItemSaveNeeded()
        Dim args = New PlayerNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset)
        RaiseEvent CollectionChanged(Me, args)
    End Sub


    ''' <summary>
    ''' Raise PropertyChanged event with Add event.
    ''' </summary>
    Private Sub RaiseCollectionChangedAdd(newItem As IPlayerAction)
        IsSaveNeeded = True
        Dim args = New PlayerNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem)
        RaiseEvent CollectionChanged(Me, args)
    End Sub


    ''' <summary>
    ''' Raise PropertyChanged event with Replace event and changed property information.
    ''' </summary>
    Private Sub RaiseCollectionChangedProperty(changedItem As IPlayerAction, prop As PropertyDescriptor)
        IsSaveNeeded = IsSaveNeeded OrElse IsItemSaveNeeded()
        Dim args = New PlayerNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, changedItem, prop)
        RaiseEvent CollectionChanged(Me, args)
    End Sub


    ''' <summary>
    ''' Raise PropertyChanged event with Remove event.
    ''' </summary>
    Private Sub RaiseCollectionChangedRemove(removedItem As IPlayerAction)
        IsSaveNeeded = True
        Dim args = New PlayerNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem)
        RaiseEvent CollectionChanged(Me, args)
    End Sub


    ''' <summary>
    ''' Raise PropertyChanged event with Move event.
    ''' </summary>
    Private Sub RaiseCollectionChangedMove(movedItem As IPlayerAction, oldIndex As Integer, newIndex As Integer)
        IsSaveNeeded = True
        Dim args = New PlayerNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, movedItem, oldIndex, newIndex)
        RaiseEvent CollectionChanged(Me, args)
    End Sub


    Private Sub RaiseCollectionChanged(sender As Object, args As ListChangedEventArgs) Handles mItems.ListChanged
        If Not sIsUpdateEventEnabled Then Return

        Select Case args.ListChangedType
            Case ListChangedType.ItemAdded
                RaiseCollectionChangedAdd(Items(args.NewIndex))
            Case ListChangedType.ItemDeleted
                RaiseCollectionChangedRemove(Nothing)
            Case ListChangedType.ItemMoved
                RaiseCollectionChangedMove(Nothing, args.OldIndex, args.NewIndex)
            Case ListChangedType.ItemChanged
                RaiseCollectionChangedProperty(Items(args.NewIndex), args.PropertyDescriptor)
            Case Else
                RaiseCollectionChangedReset()
        End Select
    End Sub

#End Region


#Region " Constants "

    Private Const DefaultName = "New playlist"


    Private Const DefaultXmlNamespace = "http://nextplayer.nikitins.dk"


    Public Const PlaylistDefaultExtension = ".tpl"


    Private Shared Serializer As New XmlSerializer(GetType(PlayerActionCollection), DefaultXmlNamespace)

#End Region


#Region " Fields "

    <NonSerialized>
    Private Shared ReadOnly mAdditionalSerializedTypes As New HashSet(Of Type)()


    <NonSerialized>
    Private Shared sIsUpdateEventEnabled As Boolean = True


    <NonSerialized>
    Private ReadOnly mMachineId As MachineFingerPrint

#End Region


#Region " Constant metadata property "

    Private ReadOnly mMetadata As New PlaylistMetadata("Playlist")


    Public Property Metadata As PlaylistMetadata
        Get
            Return mMetadata
        End Get
        Set(value As PlaylistMetadata)
            ' Ignore setting from XML
        End Set
    End Property

#End Region


#Region " Saved properties "

#Region " Items notifying property "

    Private WithEvents mItems As New BindingList(Of PlayerAction)()


    ''' <summary>
    ''' Collection items.
    ''' </summary>
    Public Property Items As BindingList(Of PlayerAction)
        Get
            Return mItems
        End Get
        Set(value As BindingList(Of PlayerAction))
            mItems = value
            RaisePropertyChanged(Function() Items)
        End Set
    End Property


    Private Sub ItemsCollectionChanged(sender As Object, args As ListChangedEventArgs) Handles mItems.ListChanged
        RaiseCollectionChanged(sender, args)
    End Sub


    Private ReadOnly Property ActiveItems As IEnumerable(Of IPlayerAction)
        Get
            Return mItems.Where(Function(a) a.IsEnabled AndAlso a.CanExecute)
        End Get
    End Property

#End Region


#Region " UseSettingsForManual notifying property "

    Private mUseSettingsForManual As Boolean = True


    ''' <summary>
    ''' Whether the UI should use visual settings for manul actions.
    ''' </summary>
    Public Property UseSettingsForManual As Boolean
        Get
            Return mUseSettingsForManual
        End Get
        Set(value As Boolean)
            mUseSettingsForManual = value
            RaisePropertyChanged(Function() UseSettingsForManual)
        End Set
    End Property

#End Region


#Region " PresentationFile notifying property "

    Private WithEvents mPresentationFile As New PlaylistPresentationFile()


    ''' <summary>
    ''' Information about presentation (PPT) file.
    ''' </summary>
    Public Property PresentationFile As PlaylistPresentationFile
        Get
            Return mPresentationFile
        End Get
        Set(value As PlaylistPresentationFile)
            mPresentationFile = value
            RaisePropertyChanged(Function() PresentationFile)
        End Set
    End Property


    ''' <summary>
    ''' Information about presentation (PPT) file.
    ''' </summary>
    <XmlIgnore>
    Public ReadOnly Property PresentationFileInterface As IInputFile Implements IPresenterConfiguration.PresentationFile
        Get
            Return mPresentationFile
        End Get
    End Property


    ''' <summary>
    ''' Any change to presentation is propagated
    ''' </summary>
    Private Sub PresentationFileChangedHandler() Handles mPresentationFile.PropertyChanged
        RaisePropertyChanged(Function() PresentationFile)
    End Sub

#End Region


#Region " AudioLogicalChannels property "

    Private WithEvents mAudioLogicalChannels As IChannelCollection(Of AudioLogicalChannel)


    ''' <summary>
    ''' This stores logical audio channels in the playlist.
    ''' </summary>
    <IgnoreForReport>
    Public Property AudioLogicalChannels As IChannelCollection(Of AudioLogicalChannel) Implements IAudioChannelStorage.Logical
        Get
            If mAudioLogicalChannels Is Nothing Then
                mAudioLogicalChannels = New ChannelCollection(Of AudioLogicalChannel)()
            End If

            Return mAudioLogicalChannels
        End Get
        Set(value As IChannelCollection(Of AudioLogicalChannel))
            mAudioLogicalChannels = value
        End Set
    End Property


    Private Sub AudioLogicalChannelsChangedHandler() Handles mAudioLogicalChannels.CollectionChanged
        RaisePropertyChanged(NameOf(AudioLogicalChannels))
    End Sub

#End Region


#Region " TextLogicalChannels property "

    Private WithEvents mTextLogicalChannels As New ChannelCollection(Of TextLogicalChannel)()


    ''' <summary>
    ''' This stores logical text channels in the playlist.
    ''' </summary>
    ''' <remarks>For serialization only</remarks>
    <IgnoreForReport>
    Public Property TextLogicalChannels As ChannelCollection(Of TextLogicalChannel) Implements ITextChannelStorage.Logical
        Get
            Return mTextLogicalChannels
        End Get
        Set(value As ChannelCollection(Of TextLogicalChannel))
            mTextLogicalChannels = value
        End Set
    End Property


    Private Sub TextLogicalChannelsChangedHandler() Handles mTextLogicalChannels.CollectionChanged
        RaisePropertyChanged(NameOf(TextLogicalChannels))
    End Sub

#End Region


#Region " EnvironmentList property "

    Private WithEvents mEnvironmentList As New PlaylistEnvironmentConfigurationCollection()


    ''' <summary>
    ''' This stores in the playlist environment-dependent information.
    ''' </summary>
    <IgnoreForReport>
    Public Property EnvironmentList As PlaylistEnvironmentConfigurationCollection
        Get
            Return mEnvironmentList
        End Get
        Set(value As PlaylistEnvironmentConfigurationCollection)
            mEnvironmentList = value

            If mEnvironmentList.Count = 0 Then
                mEnvironmentList.Add(New PlaylistEnvironmentConfiguration())
            End If
        End Set
    End Property


    Private Sub EnvironmentListChangedHandler() Handles mEnvironmentList.ListChanged
        RaisePropertyChanged(NameOf(EnvironmentList))
    End Sub

#End Region

#End Region


#Region " Not saved properties "

#Region " EnvironmentName virtual property "

    ''' <summary>
    ''' Name of the currently set environment.
    ''' Should match the one in AppConfiguration.
    ''' </summary>
    <XmlIgnore>
    <IgnoreForReport>
    Public Property EnvironmentName As String Implements IPlaylist.EnvironmentName
        Get
            Return CurrentEnvironment?.Name
        End Get
        Private Set(value As String)
            Dim env = EnvironmentList.FirstOrDefault(
                Function(e) mMachineId.IsMatching(e.MachineId) AndAlso e.Name = value)

            If env Is Nothing Then
                env = New PlaylistEnvironmentConfiguration With {.Name = value}
                EnvironmentList.Add(env)
            Else
                ' In case it changed
                env.MachineId = mMachineId.FingerPrint
            End If

            env.AfterLoad()
            CurrentEnvironment = env
        End Set
    End Property

#End Region


#Region " CurrentEnvironment property "

    Private mCurrentEnvironment As PlaylistEnvironmentConfiguration


    ''' <summary>
    ''' This references the currently set environment.
    ''' </summary>
    <XmlIgnore>
    <IgnoreForReport>
    Public Property CurrentEnvironment As PlaylistEnvironmentConfiguration
        Get
            Return mCurrentEnvironment
        End Get
        Private Set(value As PlaylistEnvironmentConfiguration)
            mCurrentEnvironment = value
            InterfaceMapper.SetInstance(Of IAudioEnvironmentStorage)(value.AudioOutput)
            InterfaceMapper.SetInstance(Of ITextEnvironmentStorage)(value.TextOutput)
            RaisePropertyChanged(NameOf(CurrentEnvironment))
        End Set
    End Property


    ''' <summary>
    ''' This references the currently set environment.
    ''' </summary>
    <XmlIgnore>
    <IgnoreForReport>
    Public ReadOnly Property CurrentEnvironmentGeneric As Object Implements IPlaylist.CurrentEnvironment
        Get
            Return mCurrentEnvironment
        End Get
    End Property

#End Region


#Region " FilePath notifying property "

    Private mFilePath As String


    ''' <summary>
    ''' Playlist full file path.
    ''' </summary>
    <XmlIgnore()>
    Public Property FilePath As String Implements IInputFile.LastRootPath
        Get
            Return mFilePath
        End Get
        Set(value As String)
            mFilePath = value
            RaisePropertyChanged(Function() FilePath)
        End Set
    End Property

#End Region


#Region " FileTimestamp notifying property "

    Private mFileTimestamp As DateTime


    ''' <summary>
    ''' Playlist file's modification timestamp.
    ''' </summary>
    <XmlIgnore()>
    Public Property FileTimestamp As DateTime Implements IInputFile.FileTimestamp
        Get
            Return mFileTimestamp
        End Get
        Set(value As DateTime)
            mFileTimestamp = value
            RaisePropertyChanged(Function() FileTimestamp)
        End Set
    End Property

#End Region


#Region " IsLoadingFailed notifying property "

    Private mIsLoadingFailed As Boolean


    ''' <summary>
    ''' Whether there were problems when reading the playlist from a file.
    ''' </summary>
    <XmlIgnore()>
    Public Property IsLoadingFailed As Boolean Implements IInputFile.IsLoadingFailed
        Get
            Return mIsLoadingFailed
        End Get
        Set(value As Boolean)
            mIsLoadingFailed = value
            RaisePropertyChanged(Function() IsLoadingFailed)
        End Set
    End Property

#End Region


#Region " FileName notifying property "

    Private mFileName As String


    ''' <summary>
    ''' Playlist full file name.
    ''' </summary>
    <XmlIgnore()>
    Public Property FileName As String Implements IInputFile.FileName
        Get
            Return mFileName
        End Get
        Set(value As String)
            mFileName = value
            RaisePropertyChanged(Function() FileName)
        End Set
    End Property

#End Region


#Region " Name notifying property "

    Private mName As String = DefaultName


    ''' <summary>
    ''' Collection name, usually inherits from the file it's loaded from.
    ''' </summary>
    <XmlIgnore()>
    Public Property Name As String
        Get
            Return mName
        End Get
        Set(value As String)
            mName = value
            RaisePropertyChanged(Function() Name)
        End Set
    End Property

#End Region


#Region " DoesNameExist notifying property "

    Private mDoesNameExist As Boolean


    ''' <summary>
    ''' Whether the collection was previously saved.
    ''' </summary>
    <XmlIgnore()>
    Public Property DoesNameExist As Boolean
        Get
            Return mDoesNameExist
        End Get
        Private Set(value As Boolean)
            mDoesNameExist = value
            RaisePropertyChanged(Function() DoesNameExist)
        End Set
    End Property

#End Region


#Region " IsSaveNeeded notifying property and related method "

    Private mIsSaveNeeded As Boolean


    ''' <summary>
    ''' Whether the collection has been modified since last save.
    ''' </summary>
    <XmlIgnore()>
    Public Property IsSaveNeeded As Boolean
        Get
            Return mIsSaveNeeded
        End Get
        Private Set(value As Boolean)
            If mIsSaveNeeded = value Then Return

            mIsSaveNeeded = value
            RaisePropertyChanged(Function() IsSaveNeeded)
        End Set
    End Property


    ''' <summary>
    ''' Check whether any item needs saving.
    ''' </summary>
    Private Function IsItemSaveNeeded() As Boolean
        Return Items.Any(Function(item) item.IsSaveNeeded)
    End Function


    ''' <summary>
    ''' Collection is saved, clear IsSaveNeeded flags.
    ''' </summary>
    Private Sub Saved()
        For Each item In Items
            item.IsSaveNeeded = False
        Next

        IsSaveNeeded = IsItemSaveNeeded()
    End Sub

#End Region


#Region " GlobalParallelList read-only notifying property "

    Private mGlobalParallelList As IEnumerable(Of IPlayerAction)


    ''' <summary>
    ''' A list of globally available parallel actions.
    ''' </summary>
    <XmlIgnore()>
    <IgnoreForReport()>
    Public Property GlobalParallelList As IEnumerable(Of IPlayerAction)
        Get
            Return mGlobalParallelList
        End Get
        Set(value As IEnumerable(Of IPlayerAction))
            mGlobalParallelList = value
            RaisePropertyChanged(Function() GlobalParallelList)
        End Set
    End Property

#End Region


#Region " MaxParallels read-only notifying property "

    Private mMaxParallels As Integer


    ''' <summary>
    ''' The maximum number of parallel actions.
    ''' </summary>
    <XmlIgnore()>
    Public Property MaxParallels As Integer
        Get
            Return mMaxParallels
        End Get
        Private Set(value As Integer)
            If mMaxParallels = value Then Return

            mMaxParallels = value
            RaisePropertyChanged(Function() MaxParallels)
        End Set
    End Property

#End Region


#Region " MaxActions read-only notifying property "

    Private mMaxActions As Integer


    ''' <summary>
    ''' The number of actions in the list (including disabled ones).
    ''' </summary>
    <XmlIgnore()>
    Public Property MaxActions As Integer
        Get
            Return mMaxActions
        End Get
        Private Set(value As Integer)
            If mMaxActions = value Then Return

            mMaxActions = value
            RaisePropertyChanged(Function() MaxActions)
        End Set
    End Property

#End Region

#End Region


#Region " Events "

    ''' <summary>
    ''' Check and rearrange parallel indices.
    ''' </summary>
    Private Sub CollectionChangedHandler(sender As Object, args As NotifyCollectionChangedEventArgs) Handles Me.CollectionChanged
        If CanAffectStructure(CType(args, PlayerNotifyCollectionChangedEventArgs)) Then
            ArrangeStructure()
        End If
    End Sub

#End Region


#Region " Init and clean-up "

    Public Sub New()
        mMachineId = MachineFingerPrint.MachineInstance

        ' Assign interfaces
        ' IPresenterConfiguration is set in AfterLoad
        InterfaceMapper.SetInstance(Of IPlaylist)(Me)
        InterfaceMapper.SetInstance(Of IAudioChannelStorage)(Me)
        InterfaceMapper.SetInstance(Of ITextChannelStorage)(Me)
    End Sub


    ''' <summary>
    ''' Create a deep copy of the playlist.
    ''' </summary>
    Public Function Clone() As PlayerActionCollection
        Dim res As New PlayerActionCollection()

        With res
            .Name = Name
            .UseSettingsForManual = UseSettingsForManual
            .PresentationFile = PresentationFile.Clone()

            For Each origItem In Items
                .Items.Add(CType(origItem.Clone(), PlayerAction))
            Next

            .AudioLogicalChannels.CopyFrom(AudioLogicalChannels)
            .TextLogicalChannels.CopyFrom(TextLogicalChannels)

            .EnvironmentList = EnvironmentList.Clone()
            .EnvironmentName = EnvironmentName

            ' Other properties are irrelevant for the purpose of export
        End With

        Return res
    End Function

#End Region


#Region " Init API "

    Public Shared Sub AddSerializedType(actType As Type)
        If Not mAdditionalSerializedTypes.Add(actType) Then Return

        Serializer = New XmlSerializer(
            GetType(PlayerActionCollection), Nothing,
            mAdditionalSerializedTypes.ToArray(), Nothing,
            DefaultXmlNamespace)
    End Sub

#End Region


#Region " Settings utility "

    ''' <summary>
    ''' Create a clone of the current environment configuration,
    ''' under a new name, and add it to the list.
    ''' </summary>
    Public Sub CloneCurrentEnvironment(newName As String)
        Dim newEnv = mCurrentEnvironment.Clone()
        newEnv.Name = newName
        EnvironmentList.Add(newEnv)
    End Sub

#End Region


#Region " IPresenterConfiguration implementation "

    Public ReadOnly Property PresenterCount As Integer Implements IPresenterConfiguration.Count
        Get
            Return If(String.IsNullOrEmpty(PresentationFile.AbsFileName), 0, 1)
        End Get
    End Property


    Public Sub SetPresentation(fileName As String) Implements IPresenterConfiguration.SetPresentation
        Dim pres As New PlaylistPresentationFile With {
            .FileName = fileName,
            .FileTimestamp = DateTime.MinValue
        }
        pres.AfterLoad(FilePath)
        PresentationFile = pres
    End Sub


    Public Function GetPresenter(index As Integer) As PowerPointConfiguration Implements IPresenterConfiguration.GetPresenter
        If String.IsNullOrEmpty(PresentationFile.AbsFileName) Then Return Nothing

        Dim stConf = InterfaceMapper.GetImplementation(Of IPresenterStaticConfiguration)()

        Return New PowerPointConfiguration With {
            .FilePath = PresentationFile.AbsFileName,
            .Index = index,
            .PowerPointUpdateInterval = stConf.PowerPointUpdateInterval,
            .UseUpdateTimer = stConf.UseUpdateTimer
        }
    End Function

#End Region


#Region " ITextChannelStorage implementation "

    Private Sub HideAllTexts() Implements ITextChannelStorage.HideAll
        For Each logCh In TextLogicalChannels
            logCh.HideText()
        Next
    End Sub

#End Region


#Region " IAudioChannelStorage implementation "

    Private Sub StopAllTests() Implements IAudioChannelStorage.StopAllTests
        For Each logCh In AudioLogicalChannels
            logCh.StopTestSound()
        Next
    End Sub

#End Region


#Region " File operations "

    ''' <summary>
    ''' Adjust collection after loading.
    ''' </summary>
    ''' <param name="fileName">Name of the loaded playlist file</param>
    Private Sub AfterLoad(fileName As String) Implements IInputFile.AfterLoad
        Dim rootPath = String.Empty

        If Not String.IsNullOrWhiteSpace(fileName) Then
            rootPath = Path.GetDirectoryName(fileName)

            ' Set up playlist name
            FilePath = rootPath
            Name = Path.GetFileNameWithoutExtension(fileName)
            DoesNameExist = True
        Else
            Name = "Unsaved playlist"
            DoesNameExist = False
        End If

        ' Ensure default channels
        AudioLogicalChannels.AfterLoad()
        TextLogicalChannels.AfterLoad()

        ' Adjust environments
        Dim config = InterfaceMapper.GetImplementation(Of IConfiguration)()
        config.SetEnvironments(EnvironmentList)
        EnvironmentName = config.EnvironmentName

        ' Adjust audio file paths
        For Each act In Items.OfType(Of IInputFile)()
            act.AfterLoad(rootPath)
        Next

        ' Adjust presentation file path
        If PresentationFile IsNot Nothing Then
            InterfaceMapper.SetInstance(Of IPresenterConfiguration)(Me)
            PresentationFile.AfterLoad(rootPath)
        Else
            InterfaceMapper.SetInstance(Of IPresenterConfiguration)(Nothing)
            PresentationFile = New PlaylistPresentationFile()
        End If

        ' Structure collection elements
        ArrangeStructure()
    End Sub


    ''' <summary>
    ''' Adjust paths before saving.
    ''' </summary>
    ''' <param name="fileName">Full name of the new playlist location</param>
    Private Sub BeforeSave(fileName As String) Implements IInputFile.BeforeSave
        If String.IsNullOrEmpty(fileName) Then Return

        Dim rootPath = Path.GetDirectoryName(fileName)

        ' Adjust playlist name
        FilePath = rootPath
        Name = Path.GetFileNameWithoutExtension(fileName)

        ' Adjust audio file paths
        For Each act In Me.Items.OfType(Of IInputFile)()
            act.BeforeSave(rootPath)
        Next

        ' Adjust presentation file path
        PresentationFile.BeforeSave(rootPath)
    End Sub


    Public Shared Function CreateEmpty() As PlayerActionCollection
        Dim newList = New PlayerActionCollection()
        newList.AfterLoad(String.Empty)
        newList.Saved()
        newList.RaiseCollectionChangedReset()
        Return newList
    End Function


    ''' <summary>
    ''' Load data from XML, replacing the existing list.
    ''' </summary>
    ''' <param name="fileName">Full path to the file with XML data; empty string to create a new playlist</param>
    Public Shared Function LoadFromFile(fileName As String) As PlayerActionCollection
        Dim newList As PlayerActionCollection = Nothing
        Debug.Assert(Not String.IsNullOrWhiteSpace(fileName))

        Try
            sIsUpdateEventEnabled = False

            Using strm = File.OpenRead(fileName)
                newList = TryCast(Serializer.Deserialize(strm), PlayerActionCollection)

                If newList Is Nothing Then
                    Throw New ArgumentException("Unrecognized stream passed to Load; must be XML data of PlayerAction.")
                End If

                newList.AfterLoad(fileName)
                Return newList
            End Using

            newList.IsLoadingFailed = False

        Catch ex As Exception
            InterfaceMapper.GetImplementation(Of IMessageLog)().LogFileError(
                "Error loading playlist: " & ex.Message)

            If newList IsNot Nothing Then
                newList.IsLoadingFailed = True
            End If

        Finally
            sIsUpdateEventEnabled = True

            If newList IsNot Nothing Then
                newList.Saved()
                newList.RaiseCollectionChangedReset()
            End If
        End Try

        Return Nothing
    End Function


    ''' <summary>
    ''' Save this list to XML.
    ''' </summary>
    ''' <param name="strm">Stream to write to</param>
    Public Sub Save(strm As Stream, Optional fileToSave As String = Nothing)
        If Not String.IsNullOrEmpty(fileToSave) Then
            BeforeSave(fileToSave)
        End If

        Serializer.Serialize(strm, Me)

        If Not String.IsNullOrEmpty(fileToSave) Then
            DoesNameExist = True
            Saved()
        End If
    End Sub

#End Region


#Region " Structure API "

    ''' <summary>
    ''' Go through list items and adjust their structure-related properties.
    ''' </summary>
    Public Sub ArrangeStructure()
        Dim data = PlaylistStructureLibrary.ArrangeStructure(Items.Cast(Of IPlayerAction)().ToList())

        GlobalParallelList = data.GlobalParallelList
        MaxParallels = data.MaxParallels
        MaxActions = Items.Count
    End Sub

#End Region


#Region " Notifications checkers "

    ''' <summary>
    ''' Check whether list modification can affect data saving.
    ''' </summary>
    ''' <returns>True if it can</returns>
    ''' <remarks>
    ''' Checks XmlIgnore attribute.
    ''' </remarks>
    Public Shared Function CanAffectXml(args As PlayerNotifyCollectionChangedEventArgs) As Boolean
        ' BindingList does not propagate new derived properties.
        ' We don't know, which property has been changed, it could be a persistant one.
        If args.ChangedProperty Is Nothing Then
            Return True
        End If

        ' Marked properties are not saved.
        Return args.ChangedProperty.Attributes.OfType(Of XmlIgnoreAttribute)().IsEmpty()
    End Function


    ''' <summary>
    ''' Check whether list modification can affect parallel tree or effects structure.
    ''' </summary>
    ''' <returns>True if it can</returns>
    ''' <remarks>
    ''' Ignore item changes unless relevant.
    ''' Accept list changes.
    ''' </remarks>
    Public Shared Function CanAffectStructure(args As PlayerNotifyCollectionChangedEventArgs) As Boolean
        ' Adding and removing items might affect the parallel system.
        If args.Action <> NotifyCollectionChangedAction.Replace Then
            Return True
        End If

        ' BindingList does not propagate new derived properties.
        ' We don't know, which property has been changed, but we know, that no
        ' derived properties should affect the parallel system.
        If args.ChangedProperty Is Nothing Then
            Return False
        End If

        ' Only the marked properties affect the structure.
        Return args.ChangedProperty.Attributes.OfType(Of AffectsStructureAttribute)().Any()
    End Function


    ''' <summary>
    ''' Check whether list modification can affect automatic triggers.
    ''' </summary>
    ''' <returns>True if it can</returns>
    Public Shared Function CanAffectTriggers(args As PlayerNotifyCollectionChangedEventArgs) As Boolean
        ' BindingList does not propagate new derived properties.
        ' We don't know, which property has been changed, but we know, that
        ' no derived properties affect triggers.
        If args.ChangedProperty Is Nothing Then
            Return False
        End If

        ' Only the marked properties affect the triggers.
        Return args.ChangedProperty.Attributes.OfType(Of AffectsTriggersAttribute)().Any()
    End Function

#End Region

End Class
