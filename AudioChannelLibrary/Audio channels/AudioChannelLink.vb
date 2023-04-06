Imports System.ComponentModel
Imports System.Diagnostics.CodeAnalysis
Imports System.Xml.Serialization
Imports Common


<Serializable>
<CLSCompliant(True)>
Public Class AudioChannelLink
    Inherits ChannelLink
    Implements IVolumeController

#Region " Fields "

    ''' <summary>
    ''' Link index (0..), the index of this link in the logical channel.
    ''' </summary>
    Private ReadOnly mLinkIndex As Integer


    ''' <summary>
    ''' The number of links assigned to the logical channel.
    ''' </summary>
    ''' <remarks>The default value is needed to make sure all links are created</remarks>
    Private ReadOnly mLinkCount As Integer

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


#Region " MaxInputs notifying property "

    Private mCollectionIsBeingModified As Boolean

    Private mMaxInputs As Integer


    ''' <summary>
    ''' The maximum number of inputs defined for this link.
    ''' </summary>
    <XmlIgnore>
    Public Property MaxInputs As Integer
        Get
            Return mMaxInputs
        End Get
        Set(value As Integer)
            If mCollectionIsBeingModified OrElse value = mMaxInputs Then Return
            mCollectionIsBeingModified = True

            SetField(mMaxInputs, value, NameOf(MaxInputs))

            ' Add missing mappings
            For nch = 1 To value
                GetOrCreateMapping(nch)
            Next

            ' Remove surplus mappings
            For Each m In mMappingBacking.ToList()
                If m.NofChannels > value Then
                    mMappingBacking.Remove(m)
                End If
            Next

            MappingCollection.ResetBindings()
            mCollectionIsBeingModified = False
            RaisePropertyChanged(NameOf(MappingCollection))
        End Set
    End Property

#End Region


#Region " MappingCollection notifying property "

    Private ReadOnly mMappingBacking As New List(Of AudioChannelLinkMapping)()


    Private WithEvents mMappingCollection As New BindingList(Of AudioChannelLinkMapping)(mMappingBacking)


    Public Property MappingCollection As BindingList(Of AudioChannelLinkMapping)
        Get
            Return mMappingCollection
        End Get
        Set(value As BindingList(Of AudioChannelLinkMapping))
            SetField(mMappingCollection, value, NameOf(MappingCollection))
        End Set
    End Property


    Private Sub ConnectionCollectionChangedHandler(sender As Object, e As ListChangedEventArgs) Handles mMappingCollection.ListChanged
        If e.ListChangedType = ListChangedType.Reset Then Return

        MappingCollection.RaiseListChangedEvents = False

        mMappingBacking.Sort(Function(ml, mr) mr.NofChannels - ml.NofChannels)

        MappingCollection.RaiseListChangedEvents = True
        MappingCollection.ResetBindings()

        RaisePropertyChanged(NameOf(MappingCollection))
    End Sub

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Serialization constructor.
    ''' </summary>
    Public Sub New()
        ' Do nothing
    End Sub


    ''' <summary>
    ''' Initializing constructor.
    ''' Make sure mono and stereo mappings are created.
    ''' </summary>
    ''' <param name="linkIdx">
    ''' Link index (0..), the index of the current link in the logical channel
    ''' </param>
    ''' <param name="linkCount">The number of links to initialize</param>
    Public Sub New(linkIdx As Integer, linkCount As Integer)
        mLinkIndex = linkIdx
        mLinkCount = linkCount
    End Sub

#End Region


#Region " ChannelLink overrides "

    Public Overrides Sub AfterLoad()
        If mCollectionIsBeingModified Then Return

        ' Create default mappings for mono and stereo sources
        MaxInputs = If(MappingCollection.FirstOrDefault()?.NofChannels, 2)
    End Sub

#End Region


#Region " Own functionality "

    ''' <summary>
    ''' Get a mapping configuration for the given amount of source channels.
    ''' If does not exist, create a mapping with all channels set.
    ''' </summary>
    Public Function GetOrCreateMapping(chCount As Integer) As AudioChannelLinkMapping
        Dim res = MappingCollection.FirstOrDefault(Function(m) m.NofChannels = chCount)

        If res Is Nothing Then
            ' Set all mappings to True
            res = New AudioChannelLinkMapping(chCount, mLinkIndex, mLinkCount)
            MappingCollection.Add(res)
        End If

        Return res
    End Function

#End Region


#Region " ToString "

    <ExcludeFromCodeCoverage>
    Public Overrides Function ToString() As String
        Return String.Format("{0}->{1}: {2:p}", Logical, Physical, Volume)
    End Function

#End Region

End Class
