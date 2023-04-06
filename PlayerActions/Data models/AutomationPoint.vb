Imports System.ComponentModel
Imports Common


''' <summary>
''' A single point on an automation curve.
''' X - time, Y - value.
''' </summary>
Public Class AutomationPoint
    Implements INotifyPropertyChanged

#Region " INotifyPropertyChanged implementation "

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged


    ''' <summary>
    ''' Helper function to raise the event.
    ''' </summary>
    Private Sub RaisePropertyChanged(propName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propName))
    End Sub


    ''' <summary>
    ''' Helper function to raise the event.
    ''' </summary>
    Protected Sub RaisePropertyChanged(Of T)(prop As Expressions.Expression(Of Func(Of T)))
        RaisePropertyChanged(PropertyChangedHelper.GetPropertyName(prop))
    End Sub

#End Region


#Region " X notifying property "

    Private mBackingX As Double


    Public Property X As Double
        Get
            Return mBackingX
        End Get
        Set(value As Double)
            mBackingX = value
            RaisePropertyChanged(Function() X)
        End Set
    End Property

#End Region


#Region " Y notifying property "

    Private mBackingY As Single


    Public Property Y As Single
        Get
            Return mBackingY
        End Get
        Set(value As Single)
            mBackingY = value
            RaisePropertyChanged(Function() Y)
        End Set
    End Property

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' For serialization.
    ''' </summary>
    Public Sub New()
        ' Do nothing
    End Sub


    ''' <summary>
    ''' For creation convenience.
    ''' </summary>
    ''' <param name="x">Time coordinate</param>
    ''' <param name="y">Value coordinate</param>
    Public Sub New(x As Double, y As Single)
        Me.X = x
        Me.Y = y
    End Sub

#End Region


#Region " ToString "

    Public Overrides Function ToString() As String
        Return String.Format("{0}, {1}", X, Y)
    End Function

#End Region

End Class
