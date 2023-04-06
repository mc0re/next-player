Imports System.ComponentModel
Imports AudioChannelLibrary
Imports PlayerActions
Imports VoiceControlLibrary
Imports Common
Imports TextChannelLibrary
Imports WpfResources


Partial Public Class SettingsWindow
    Inherits Window

#Region " OnAccept property "

    Public Property OnAccept As Action

#End Region


#Region " CopyEnvironment command "

    Public Property CopyEnvironmentCommand As New DelegateCommand(AddressOf CreateCopyOfCurrentExecuted)


    Private Sub CreateCopyOfCurrentExecuted(param As Object)
        Dim curConf = CType(AppConfiguration.Instance.CurrentEnvironment, AppEnvironmentConfiguration)
        Dim newName = curConf.Name & " Copy"
        AppConfiguration.Instance.CloneCurrentEnvironment(newName)

        AppConfiguration.Instance.EnvironmentName = newName
    End Sub

#End Region


#Region " ManageEnvironment command "

    Public Property ManageEnvironmentCommand As New DelegateCommand(AddressOf ManageEnvironmentCommandExecuted)


    Private Sub ManageEnvironmentCommandExecuted(param As Object)
        Dim wnd As New ManageEnvironmentsWindow()
        wnd.ShowDialog()
    End Sub

#End Region


#Region " VoiceCommands dependency property "

    Public Shared ReadOnly VoiceCommandsProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(VoiceCommands), GetType(VoiceCommandSettingItemCollection), GetType(SettingsWindow))


    <Category("Common Properties"), Description("A list of available voice commands and their settings")>
    Public Property VoiceCommands As VoiceCommandSettingItemCollection
        Get
            Return CType(GetValue(VoiceCommandsProperty), VoiceCommandSettingItemCollection)
        End Get
        Set(value As VoiceCommandSettingItemCollection)
            SetValue(VoiceCommandsProperty, value)
        End Set
    End Property

#End Region


#Region " Init and clean-up "

    Public Sub New()
        MyBase.New()

        InitializeComponent()
    End Sub


    Private Sub ClosedHandler() Handles Me.Closed
        Dim textPhysical = InterfaceMapper.GetImplementation(Of ITextEnvironmentStorage)()
        textPhysical.HideAll()
        Dim textLogical = InterfaceMapper.GetImplementation(Of ITextChannelStorage)()
        textLogical.HideAll()

        Dim audioPhysical = InterfaceMapper.GetImplementation(Of IAudioEnvironmentStorage)()
        audioPhysical.StopAllTests()
        Dim audioLogical = InterfaceMapper.GetImplementation(Of IAudioChannelStorage)()
        audioLogical.StopAllTests()

        ' Execute always so that voice command settings match the actual engine configuration
        OnAccept.Invoke()

        AppConfiguration.Instance.LoadSkinFromAppConfig()
    End Sub

#End Region


#Region " XAML-bound commands "

    ''' <summary>
    ''' Accept changes and close this window.
    ''' </summary>
    Private Sub CloseSettingsWindowCommandExecuted(sender As Object, e As ExecutedRoutedEventArgs)
        AppConfiguration.Instance.SaveSettings()
        DialogResult = True
    End Sub


    ''' <summary>
    ''' Reject changes and close this window.
    ''' </summary>
    Private Sub CancelSettingsWindowCommandExecuted(sender As Object, e As ExecutedRoutedEventArgs)
        DialogResult = False
    End Sub


    ''' <summary>
    ''' Choose a presentation file.
    ''' </summary>
    Private Sub ReplacePptFileCommandCanExecute(sender As Object, args As CanExecuteRoutedEventArgs)
        args.CanExecute = True
    End Sub


    ''' <summary>
    ''' Choose a presentation file.
    ''' </summary>
    Private Sub ReplaceFileCommandExecuted(sender As Object, e As ExecutedRoutedEventArgs)
        Dim dlg As New Microsoft.Win32.OpenFileDialog() With {
            .DefaultExt = PlayerWindow.PresentationDefaultExtension,
            .Filter = PlayerWindow.PresentationExtensionFilter
        }

        Dim presConf = InterfaceMapper.GetImplementation(Of IPresenterConfiguration)()
        If presConf Is Nothing Then
            InterfaceMapper.GetImplementation(Of IMessageLog)().LogPowerPointError(
                    "No presentation handler found.")
            Return
        End If

        Dim pres = presConf.PresentationFile

        If String.IsNullOrWhiteSpace(pres?.FileName) Then
            dlg.InitialDirectory = InterfaceMapper.GetImplementation(Of IPlaylist)().LastRootPath
        Else
            dlg.InitialDirectory = System.IO.Path.GetDirectoryName(pres.FileName)
            dlg.FileName = System.IO.Path.GetFileNameWithoutExtension(pres.FileName)
        End If

        ' Show open file dialog box 
        Dim result? As Boolean = dlg.ShowDialog()

        ' Process open file dialog box results 
        If result <> True Then Return

        Try
            Mouse.OverrideCursor = Cursors.Wait
            presConf.SetPresentation(dlg.FileName)
        Finally
            Mouse.OverrideCursor = Nothing
        End Try
    End Sub


    ''' <summary>
    ''' Check whether there is a file to update.
    ''' </summary>
    Private Sub HasPptFileCommandCanExecute(sender As Object, args As CanExecuteRoutedEventArgs)
        Dim pres = InterfaceMapper.GetImplementation(Of IPresenterConfiguration)()?.PresentationFile
        args.CanExecute = Not String.IsNullOrWhiteSpace(pres?.FileName)
    End Sub


    ''' <summary>
    ''' Open PowerPoint with the given file.
    ''' </summary>
    Private Sub OpenPptCommandExecuted(sender As Object, e As ExecutedRoutedEventArgs)
        Dim pres = PresenterFactory.GetReference(0)
        pres.OpenApplication()
    End Sub


    ''' <summary>
    ''' Refresh file info.
    ''' </summary>
    Private Sub UpdateFileCommandExecuted(sender As Object, e As ExecutedRoutedEventArgs)
        Dim pres = PresenterFactory.GetReference(0)
        pres.UpdateThumbnails(True)
    End Sub


    ''' <summary>
    ''' Show link editor for the given channel link.
    ''' </summary>
    Private Sub ShowLinkEditorCommandExecuted(sender As Object, e As ExecutedRoutedEventArgs)
        Dim args = CType(e.Parameter, ShowLinkEditorCommandArgs)
        Dim wnd As New ChannelLinkEditorWindow() With {
            .Link = args.Link,
            .LinkEditorTemplate = args.Template,
            .DeleteLinkCommand = args.DeleteCommand
        }
        wnd.Show()
    End Sub

#End Region

End Class
