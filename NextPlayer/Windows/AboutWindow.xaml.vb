
Imports System.Reflection


Public Class AboutWindow
    Inherits Window

#Region " Constants "

    Private Const AboutUri = "pack://application:,,,/Resources/About.rtf"

    Private Const HelpUri = "pack://application:,,,/Resources/LocalHelp.rtf"

#End Region


#Region " AppName read-only property "

    Public ReadOnly Property AppName As String
        Get
            Return Assembly.GetExecutingAssembly().GetCustomAttribute(Of AssemblyProductAttribute)().Product
        End Get
    End Property

#End Region


#Region " Version read-only property "

    Public ReadOnly Property Version As Version
        Get
            Return Assembly.GetExecutingAssembly().GetName().Version
        End Get
    End Property

#End Region


#Region " Init and clean-up "

    Private Sub AboutWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Dim aboutStrm = Application.GetResourceStream(New Uri(AboutUri))
        Dim range As New TextRange(AboutText.Document.ContentStart, AboutText.Document.ContentEnd)
        range.Load(aboutStrm.Stream, DataFormats.Rtf)
        aboutStrm.Stream.Close()

        Dim helpStrm = Application.GetResourceStream(New Uri(HelpUri))
        range = New TextRange(HelpText.Document.ContentStart, HelpText.Document.ContentEnd)
        range.Load(helpStrm.Stream, DataFormats.Rtf)
        helpStrm.Stream.Close()
    End Sub

#End Region


#Region " Command handlers "

    ''' <summary>
    ''' Close this window.
    ''' </summary>
    Private Sub CloseWindowCommandExecuted(sender As Object, e As ExecutedRoutedEventArgs)
        DialogResult = True
    End Sub

#End Region

End Class
