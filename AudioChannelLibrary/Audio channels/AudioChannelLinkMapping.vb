Imports System.ComponentModel
Imports System.Xml.Serialization
Imports Common


''' <summary>
''' Define a set of connections for the given number of channels in the played data.
''' </summary>
<Serializable>
<CLSCompliant(True)>
Public Class AudioChannelLinkMapping
    Inherits PropertyChangedHelper

#Region " NofChannels notifying property "

    Private mNofChannels As Integer


    ''' <summary>
    ''' The needed number of channels in the input data.
    ''' </summary>
    Public Property NofChannels As Integer
        Get
            Return mNofChannels
        End Get
        Set(value As Integer)
            SetField(mNofChannels, value, NameOf(NofChannels))
            RaisePropertyChanged(NameOf(Title))
        End Set
    End Property

#End Region


#Region " Title read-only property "

    ''' <summary>
    ''' Automatically generated title for the given amount of channels.
    ''' </summary>
    Public ReadOnly Property Title As String
        Get
            Select Case mNofChannels
                Case 1
                    Return "Mono"
                Case 2
                    Return "Stereo"
                Case Else
                    Return $"{mNofChannels} channels"
            End Select
        End Get
    End Property

#End Region


#Region " PanningModel notifying property "

    Private mPanningModel As PanningModels = PanningModels.ConstantPower


    ''' <summary>
    ''' Which model is used for source panning.
    ''' For now, not written into the file, to be able to change it globally.
    ''' </summary>
    <XmlIgnore>
    Public Property PanningModel As PanningModels
        Get
            Return mPanningModel
        End Get
        Set(value As PanningModels)
            SetField(mPanningModel, value, NameOf(PanningModel))
        End Set
    End Property

#End Region


#Region " Panning notifying property "

    Private mPanning As Single


    ''' <summary>
    ''' The panning of the mono output relative to source channels.
    ''' </summary>
    Public Property Panning As Single
        Get
            Return mPanning
        End Get
        Set(value As Single)
            SetField(mPanning, value, NameOf(Panning))
        End Set
    End Property

#End Region


#Region " MappingList notifying property "

    Private WithEvents mMappingList As New BindingList(Of AudioChannelMappingItem)()


    ''' <summary>
    ''' A list of flags of size <see cref="NofChannels"/>.
    ''' A flag set to True means, a channel in the played data outputs its data into this link.
    ''' </summary>
    Public Property MappingList As BindingList(Of AudioChannelMappingItem)
        Get
            Return mMappingList
        End Get
        Set(value As BindingList(Of AudioChannelMappingItem))
            SetField(mMappingList, value, NameOf(MappingList))
        End Set
    End Property


    Private Sub ConnectionListChangedHandler() Handles mMappingList.ListChanged
        RaisePropertyChanged(NameOf(MappingList))
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
    ''' A constructor, automatically creating a mapping.
    ''' </summary>
    ''' <param name="srcChannels">The number of channels to creating mapping for</param>
    ''' <param name="linkIdx">
    ''' Link index (0..) for the current link in the logical channel
    ''' </param>
    ''' <param name="linkCount">The number of links to be initialized (for panning)</param>
    Public Sub New(srcChannels As Integer, linkIdx As Integer, linkCount As Integer)
        mNofChannels = srcChannels

        For idx = 0 To srcChannels - 1
            MappingList.Add(New AudioChannelMappingItem With {.IsSet = True})
        Next

        mPanning = If(linkCount <= 1, 0, CSng(linkIdx / (linkCount - 1) * 2 - 1))
    End Sub

#End Region


#Region " ToString "

    Public Overrides Function ToString() As String
        Return $"{NofChannels}: " &
            String.Join(" ", From item In MappingList Let a = item.ToString() Select a)
    End Function

#End Region

End Class
