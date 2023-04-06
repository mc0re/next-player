Imports Common
Imports WpfResources
Imports TextChannelLibrary
Imports TextWindowLibrary


Public Class TextOutputEditorControl
    Inherits ChannelEditorControlBase

#Region " ChannelEditorControlGenericBase overrides: logical "

    ''' <summary>
    ''' Add a new text window definition.
    ''' </summary>
    Protected Overrides Sub AddLogicalCommandOverride()
        Dim txtConfig = InterfaceMapper.GetImplementation(Of ITextChannelStorage)()
        txtConfig.Logical.CreateNewChannel()
    End Sub


    ''' <summary>
    ''' Delete the window definition, defined by command parameter.
    ''' </summary>
    Protected Overrides Sub DeleteLogicalCommandOverride(logChannel As ILogicalChannel)
        Dim txtConfig = InterfaceMapper.GetImplementation(Of ITextChannelStorage)()
        txtConfig.Logical.RemoveChannel(logChannel.Channel)
    End Sub


    ''' <summary>
    ''' Show or hide the window definition, defined by command parameter.
    ''' </summary>
    Protected Overrides Sub TestLogicalCommandOverride(logChannel As ILogicalChannel)
        Dim txtLogical = CType(logChannel, TextLogicalChannel)

        If txtLogical Is Nothing Then
            Return
        ElseIf txtLogical.IsActive Then
            txtLogical.HideText()
        Else
            txtLogical.ShowText(String.Format(
                "Logical {0}: '{1}'", txtLogical.Channel, txtLogical.Description))
        End If
    End Sub

#End Region


#Region " ChannelEditorControlGenericBase overrides: physical "

    ''' <summary>
    ''' Only TextWindow is currently available.
    ''' </summary>
    Protected Overrides Sub AddPhysicalCommandOverride()
        Dim txtConfig = InterfaceMapper.GetImplementation(Of ITextEnvironmentStorage)()
        txtConfig.Physical.CreateNewChannel(Of TextWindowPhysicalChannel)()
    End Sub


    Protected Overrides Sub DeletePhysicalCommandOverride(physChannel As IPhysicalChannel)
        Dim txtConfig = InterfaceMapper.GetImplementation(Of ITextEnvironmentStorage)()
        txtConfig.Physical.RemoveChannel(physChannel.Channel)
    End Sub


    Protected Overrides Sub TestPhysicalCommandOverride(physChannel As IPhysicalChannel)
        Dim txtConfig = InterfaceMapper.GetImplementation(Of ITextEnvironmentStorage)()
        Dim phys = txtConfig.Physical.Channel(physChannel.Channel)

        If phys Is Nothing Then
            Return
        ElseIf phys.IsActive Then
            phys.HideText()
        Else
            phys.ShowText(String.Format(
                "Physical {0}: '{1}'", phys.Channel, phys.Description))
        End If
    End Sub

#End Region

End Class
