Imports System.IO
Imports Common


Public Class SkinEditorControl
    Inherits Control

#Region " Constants "

    Private Const SkinExtensionFilter = "NexTPlayer skins|*.tps|All files|*.*"

    Private Const SkinDefaultExtension = ".tps"

#End Region


#Region " Command definitions "

    Public Property LoadSkinCommand As New DelegateCommand(AddressOf LoadCommandExecute)

    Public Property SaveSkinCommand As New DelegateCommand(AddressOf SaveCommandExecute)

    Public Property SetDefaultSkinCommand As New DelegateCommand(AddressOf SetDefaultCommandExecute, AddressOf SetDefaultCommandCanExecute)

#End Region


#Region " Skin dependency property "

    ''' <summary>
    ''' Initializer should also be okay in design mode.
    ''' </summary>
    Public Shared ReadOnly SkinProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(Skin), GetType(SkinConfiguration), GetType(SkinEditorControl),
        New PropertyMetadata(InterfaceMapper.GetImplementation(Of IConfiguration)(True)?.Skin))


    Public Property Skin As SkinConfiguration
        Get
            Return CType(GetValue(SkinProperty), SkinConfiguration)
        End Get
        Set(value As SkinConfiguration)
            SetValue(SkinProperty, value)
        End Set
    End Property

#End Region


#Region " Save and load commands "

    ''' <summary>
    ''' Save to a file.
    ''' </summary>
    Private Sub SaveCommandExecute(param As Object)
        Dim dlg As New Microsoft.Win32.SaveFileDialog() With {
            .DefaultExt = SkinDefaultExtension,
            .Filter = SkinExtensionFilter,
            .CheckPathExists = True,
            .CheckFileExists = False,
            .OverwritePrompt = True
        }

        ' Show open file dialog box 
        Dim result? As Boolean = dlg.ShowDialog()

        ' Process open file dialog box results 
        If result <> True Then Return

        Try
            Using fstr = File.Create(dlg.FileName)
                Skin.Save(fstr)
            End Using

        Catch ex As Exception
            InterfaceMapper.GetImplementation(Of IMessageLog).LogFileError(ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' Load from a file.
    ''' </summary>
    Private Sub LoadCommandExecute(param As Object)
        Dim dlg As New Microsoft.Win32.OpenFileDialog() With {
            .DefaultExt = SkinDefaultExtension,
            .Filter = SkinExtensionFilter,
            .CheckPathExists = True,
            .CheckFileExists = True
        }

        ' Show open file dialog box 
        Dim result? As Boolean = dlg.ShowDialog()

        ' Process open file dialog box results 
        If result <> True Then Return

        Try
            InterfaceMapper.GetImplementation(Of IConfiguration).CurrentSkinPath = dlg.FileName
            SetDefaultSkinCommand.RaiseCanExecuteChanged()

        Catch ex As Exception
            InterfaceMapper.GetImplementation(Of IMessageLog).LogFileError(ex.Message)
        End Try
    End Sub


    ''' <summary>
    ''' Check whether there is a skin to delete.
    ''' </summary>
    Private Function SetDefaultCommandCanExecute(param As Object) As Boolean
        ' Protection for design mode
        Dim conf = InterfaceMapper.GetImplementation(Of IConfiguration)(True)
        Return If(conf Is Nothing, False, Not String.IsNullOrEmpty(conf.CurrentSkinPath))
    End Function


    ''' <summary>
    ''' Clear skin setting, thereby setting default skin.
    ''' </summary>
    Private Sub SetDefaultCommandExecute(param As Object)
        InterfaceMapper.GetImplementation(Of IConfiguration).CurrentSkinPath = String.Empty
        SetDefaultSkinCommand.RaiseCanExecuteChanged()
    End Sub

#End Region

End Class
