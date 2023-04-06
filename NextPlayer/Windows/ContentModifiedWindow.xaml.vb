Imports System.ComponentModel


Public Class ContentModifiedWindow

#Region " ShallSave property "

    Public Property ShallSave As Boolean = False

#End Region


#Region " ContentName dependency proeprty "

	Public Shared ReadOnly ContentNameProperty As DependencyProperty = DependencyProperty.Register(
	 NameOf(ContentName), GetType(String), GetType(ContentModifiedWindow),
	 New FrameworkPropertyMetadata("Content"))


	<Category("Common Properties"), Description("What kind of content was modified; used in user message")>
    Public Property ContentName As String
        Get
            Return CStr(GetValue(ContentNameProperty))
        End Get
        Set(value As String)
            SetValue(ContentNameProperty, value)
        End Set
    End Property

#End Region


#Region " Init and clean-up "

    Public Sub New(owner As Window)
        ' This call is required by the designer.
        InitializeComponent()

        Me.Owner = owner
        Me.WindowStartupLocation = Windows.WindowStartupLocation.CenterOwner
    End Sub

#End Region


#Region " Button handling "

    Private Sub SaveButton_Click(sender As Object, args As RoutedEventArgs)
        Me.DialogResult = True
        ShallSave = True
        Me.Close()
    End Sub


    Private Sub DiscardButton_Click(sender As Object, args As RoutedEventArgs)
        Me.DialogResult = True
        Me.Close()
    End Sub

#End Region

End Class
