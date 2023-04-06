Imports Microsoft.Win32
Imports Common
Imports WpfResources
Imports AudioChannelLibrary


''' <summary>
''' Represents an editor frame for audio channel manipulations.
''' </summary>
Public Class AudioOutputEditorControl
    Inherits ChannelEditorControlBase

#Region " Constants "

    Private Const LayoutFilter = "XML Files|*.xml|All files|*.*"

    Private Const LayoutDefaultExtension = ".xml"

#End Region


#Region " PhysicalLayout command "

    ''' <summary>
    ''' Opens a window, where the user can visually edit the room and channel layout.
    ''' </summary>
    Public Property PhysicalLayoutCommand As New DelegateCommand(
        AddressOf PhysicalLayoutCommandExecuted, AddressOf PhysicalLayoutCommandCanExecute)


    Private Function PhysicalLayoutCommandCanExecute(param As Object) As Boolean
        Dim chList = CType(param, ChannelCollection(Of AudioPhysicalChannel))
        Return chList IsNot Nothing AndAlso chList.Any()
    End Function


    Private Sub PhysicalLayoutCommandExecuted(param As Object)
        Dim chList = CType(param, ChannelCollection(Of AudioPhysicalChannel))

        ' A hack of a sort, but only the current configuration can be edited
        Dim wnd As New AudioPositionEditorWindow() With {
            .Room = AppConfiguration.Instance.CurrentActionCollectionTyped.CurrentEnvironment.Room,
            .Channels = chList
        }
        wnd.Show()
    End Sub

#End Region


#Region " ExportLayout command "

    ''' <summary>
    ''' Export the physical layout to a file.
    ''' </summary>
    Public Property ExportLayoutCommand As New DelegateCommand(AddressOf ExportLayoutCommandExecuted)


    Private Sub ExportLayoutCommandExecuted(param As Object)
        Dim fd As New SaveFileDialog With {
            .Title = "Export room configuration",
            .Filter = LayoutFilter,
            .DefaultExt = LayoutDefaultExtension
        }

        If Not fd.ShowDialog() Then Return

        ' A hack of a sort, but only the current configuration can be exported
        Dim layout As New LayoutConfiguration With {
            .Channels = CType(param, ChannelCollection(Of AudioPhysicalChannel)),
            .Room = AppConfiguration.Instance.CurrentActionCollectionTyped.CurrentEnvironment.Room
        }

        Try
            layout.Export(fd.FileName)

        Catch ex As Exception
            MessageBox.Show(String.Format("Error when exporting to file '{1}'.{0}{2}",
                                          vbCrLf, fd.FileName, ex.Message),
                            AppConfiguration.AppName,
                            MessageBoxButton.OK, MessageBoxImage.Error)
        End Try
    End Sub

#End Region


#Region " ImportLayout command "

    ''' <summary>
    ''' Import the physical layout from a file, overwriting the existing one.
    ''' </summary>
    Public Property ImportLayoutCommand As New DelegateCommand(AddressOf ImportLayoutCommandExecuted)


    Private Sub ImportLayoutCommandExecuted(param As Object)
        Dim fd As New OpenFileDialog With {
            .Title = "Import room configuration",
            .Filter = LayoutFilter,
            .DefaultExt = LayoutDefaultExtension
        }

        If Not fd.ShowDialog() Then Return

        Dim chList = CType(param, ChannelCollection(Of AudioPhysicalChannel))

        ' A hack of a sort, but only the current configuration can be imported
        Dim room = AppConfiguration.Instance.CurrentActionCollectionTyped.CurrentEnvironment.Room
    End Sub

#End Region


#Region " SetupPhysical command "

    Public Property SetupPhysicalCommand As New DelegateCommand(AddressOf SetupPhysicalCommandExecuted)


    Private Sub SetupPhysicalCommandExecuted(param As Object)
        Dim physicalChannel = CType(param, AudioPhysicalChannel)
        Dim wnd As New AudioChannelEditorWindow() With {
            .Channel = physicalChannel
        }
        wnd.Show()
    End Sub

#End Region


#Region " ChannelEditorControlGenericBase overrides: logical "

    ''' <summary>
    ''' Add a new channel definition.
    ''' </summary>
    Protected Overrides Sub AddLogicalCommandOverride()
        Dim audioConfig = InterfaceMapper.GetImplementation(Of IAudioChannelStorage)()
        audioConfig.Logical.CreateNewChannel()
    End Sub


    ''' <summary>
    ''' Delete the channel definition, defined by command parameter.
    ''' </summary>
    Protected Overrides Sub DeleteLogicalCommandOverride(logChannel As ILogicalChannel)
        Dim audioConfig = InterfaceMapper.GetImplementation(Of IAudioChannelStorage)()
        audioConfig.Logical.RemoveChannel(logChannel.Channel)
    End Sub


    ''' <summary>
    ''' Beep on a channel, defined by command parameter.
    ''' </summary>
    Protected Overrides Sub TestLogicalCommandOverride(logChannel As ILogicalChannel)
        Dim ch = CType(logChannel, AudioLogicalChannel)

        If ch.IsActive Then
            ch.StopTestSound()
        Else
            ch.PlayTestSound()
        End If
    End Sub

#End Region


#Region " ChannelEditorControlGenericBase overrides: physical "

    Protected Overrides Sub AddPhysicalCommandOverride()
        Dim audioConfig = InterfaceMapper.GetImplementation(Of IAudioEnvironmentStorage)()
        audioConfig.Physical.CreateNewChannel()
    End Sub


    Protected Overrides Sub DeletePhysicalCommandOverride(physChannel As IPhysicalChannel)
        Dim audioConfig = InterfaceMapper.GetImplementation(Of IAudioEnvironmentStorage)()
        audioConfig.Physical.RemoveChannel(physChannel.Channel)
    End Sub


    Protected Overrides Sub TestPhysicalCommandOverride(physChannel As IPhysicalChannel)
        Dim ph = CType(physChannel, AudioPhysicalChannel)

        If ph.IsActive Then
            ph.StopTestSound()
        Else
            ph.PlayTestSound()
        End If
    End Sub

#End Region

End Class
