Imports System.Timers
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input


Public Class ToolTipHelper

#Region " Fields "

    Private ReadOnly mToolTip As ToolTip

    Private WithEvents mTimer As Timer

#End Region


#Region " ToolTipContent property "

    ''' <summary>
    ''' Gets or sets the text for the tooltip.
    ''' </summary>
    Public Property ToolTipContent As Object
        Get
            Return mToolTip.Content
        End Get
        Set
            mToolTip.Content = Value
        End Set
    End Property

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Creates an instance
    ''' </summary>
    Public Sub New()
        mToolTip = New ToolTip()
        mTimer = New Timer With {.AutoReset = False}
    End Sub

#End Region


#Region " API "

    ''' <summary>
    ''' To be called when the mouse enters the ui area.
    ''' </summary>
    Public Sub OnMouseEnter(sender As Object, e As MouseEventArgs)
        mTimer.Interval =
            If(Application.Current IsNot Nothing,
            ToolTipService.GetInitialShowDelay(Application.Current.MainWindow),
            100)
        mTimer.Start()
    End Sub


    ''' <summary>
    ''' To be called when the mouse leaves the ui area.
    ''' </summary>
    Public Sub OnMouseLeave(sender As Object, e As MouseEventArgs)
        mTimer.Stop()
        If mToolTip IsNot Nothing Then
            mToolTip.IsOpen = False
        End If
    End Sub

#End Region


#Region " Utility "

    Private Sub ShowToolTip(sender As Object, e As ElapsedEventArgs) Handles mTimer.Elapsed
        mTimer.Stop()
        If mToolTip IsNot Nothing Then
            mToolTip.Dispatcher.Invoke(New Action(Sub() mToolTip.IsOpen = True))
        End If
    End Sub

#End Region

End Class
