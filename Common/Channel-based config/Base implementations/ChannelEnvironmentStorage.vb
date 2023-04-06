Imports System.Xml.Serialization


''' <summary>
''' This class holds a list of physical channels and channel links.
''' </summary>
<Serializable>
<CLSCompliant(True)>
Public Class ChannelEnvironmentStorage(Of TPhys As IPhysicalChannel, TLink As IChannelLink)
    Inherits PropertyChangedHelper
    Implements IChannelEnvironmentStorage(Of TPhys, TLink)

#Region " Physical read-only property "

    Private WithEvents mPhysical As IChannelCollection(Of TPhys)


    ''' <summary>
    ''' Has to be read-write for XML serialization.
    ''' </summary>
    <IgnoreForReport>
    Public Property Physical As IChannelCollection(Of TPhys) Implements IChannelEnvironmentStorage(Of TPhys, TLink).Physical
        Get
            If mPhysical Is Nothing Then
                mPhysical = New ChannelCollection(Of TPhys)()
            End If

            Return mPhysical
        End Get
        Set(value As IChannelCollection(Of TPhys))
            mPhysical = value
        End Set
    End Property


    Private Sub PhysicalCollectionChangedHandler() Handles mPhysical.CollectionChanged
        RaisePropertyChanged(NameOf(Physical))
    End Sub

#End Region


#Region " Links read-only property "

    Private WithEvents mLinks As New ChannelLinkCollection(Of TLink)()


    <XmlIgnore>
    <IgnoreForReport>
    Private ReadOnly Property LinksGeneric As IChannelLinkCollectionBase Implements ILinkStorageBase.Links
        Get
            Return mLinks
        End Get
    End Property


    ''' <summary>
    ''' Has to be read-write for XML serialization.
    ''' </summary>
    ''' <returns></returns>
    <XmlElement(NameOf(Links))>
    <IgnoreForReport>
    Public Property Links As ChannelLinkCollection(Of TLink) Implements IChannelEnvironmentStorage(Of TPhys, TLink).Links
        Get
            Return mLinks
        End Get
        Set(value As ChannelLinkCollection(Of TLink))
            SetField(mLinks, value, NameOf(Links))
        End Set
    End Property


    Private Sub LinksCollectionChanged() Handles mLinks.CollectionChanged
        RaisePropertyChanged(NameOf(Links))
    End Sub

#End Region


#Region " API "

    ''' <summary>
    ''' Make sure defaults are in place.
    ''' The descendants should override this method to get more feasible behaviour.
    ''' </summary>
    Public Overridable Sub AfterLoad() Implements IChannelEnvironmentStorage(Of TPhys, TLink).AfterLoad
        ' Create default channel
        Physical.AfterLoad()

        ' Link created physical channel
        If Links.Count = 0 Then
            Links.CreateNewLink(1, 1)
        End If
    End Sub


    ''' <summary>
    ''' Get a list of links and physical channels for the given logical channel.
    ''' </summary>
    Public Function GetLinks(logicalNr As Integer) As IReadOnlyCollection(Of LinkResult(Of TLink, TPhys)) Implements IChannelEnvironmentStorage(Of TPhys, TLink).GetLinks
        Dim res As New List(Of LinkResult(Of TLink, TPhys))
        Dim linkList = Links.GetForLogical(logicalNr)

        For Each linkObj In linkList
            Dim ph = Physical.Channel(linkObj.Physical)
            If ph Is Nothing Then Continue For
            res.Add(New LinkResult(Of TLink, TPhys) With {.Link = linkObj, .Physical = ph})
        Next

        Return res
    End Function


    Public Shadows Function Clone(Of T As {ChannelEnvironmentStorage(Of TPhys, TLink), New})() As T
        Dim st As New T()

        st.Physical.CopyFrom(Physical)
        st.Links.CopyFrom(Links)

        Return st
    End Function

#End Region

End Class
