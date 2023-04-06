Imports AudioChannelLibrary
Imports Common


''' <summary>
''' Enhances AudioEnvironmentStorage with logical channels and simple set up for tests.
''' </summary>
Public Class TestAudioEnvironmentStorage
    Inherits AudioEnvironmentStorage

#Region " Fields "

    Private ReadOnly mLogicalList As New TestChannelCollection(Of AudioLogicalChannel)()

#End Region


#Region " Properties "

    Public ReadOnly LastLogicalChannel As Integer


    ReadOnly Property Logical As IChannelCollection(Of AudioLogicalChannel)
        Get
            Return mLogicalList
        End Get
    End Property

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Create a connection matrix for the given logical channel.
    ''' </summary>
    ''' <remarks>
    ''' The created logical channel bears number <see cref="LastLogicalChannel"/>.
    ''' The definition format is:
    '''   link-definition { ";" link-definition }
    '''   
    '''   link-definition ::= volume [ ":" delay [ ":" matrix { ":reversed" | ":disabled" } ] ]
    '''   matrix ::= mapping { "," mapping }
    '''   mapping ::= { "+" | "-" } [ "|" panning ]
    '''   panning ::= number -1..+1, default 0
    '''   
    ''' Example for a single link definition:
    '''   0.8:0:++|-1,+|-1:reversed
    ''' </remarks>
    Public Sub New(linkDef As String)
        LastLogicalChannel = mLogicalList.CreateNewChannel().Channel

        Dim defList = linkDef.Split(";"c)

        For Each def In defList
            Dim ph = Physical.CreateNewChannel()
            Links.Add(ParseLinkDefinition(def, ph))
        Next

        AfterLoad()
    End Sub

#End Region


#Region " Utility "

    ''' <remarks>
    ''' The definition format is:
    '''   volume [ ":" delay [ ":" matrix { ":reversed" | ":disabled" } ] ]
    '''   matrix ::= mapping { "," mapping }
    '''   mapping ::= { "+" | "-" } [ "|" panning ]
    '''   panning ::= number -1..+1, default 0
    ''' </remarks>
    Private Function ParseLinkDefinition(def As String, ph As IChannel) As AudioChannelLink
        Dim lnk As New AudioChannelLink With {
            .Logical = LastLogicalChannel, .Physical = ph.Channel
        }

        Dim parts = def.Split(":"c)
        Dim partIdx = 1
        lnk.Volume = Single.Parse(parts(0))

        If parts.Length > 1 Then
            lnk.Delay = Single.Parse(parts(1))
            partIdx += 1
        End If

        If parts.Length > 2 Then
            For Each mapDef In parts(2).Split(","c)
                lnk.MappingCollection.Add(ParseLinkMapping(mapDef.Trim()))
            Next

            partIdx += 1
        End If

        For Each rest In parts.Skip(partIdx)
            Select Case rest.Trim()
                Case "reversed"
                    lnk.ReversedPhase = True
                Case "disabled"
                    lnk.IsEnabled = False
            End Select
        Next

        Return lnk
    End Function

#End Region

End Class
