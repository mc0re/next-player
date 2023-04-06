Imports AudioPlayerLibrary


Friend Class TestActionProducer
    Inherits TestAction
    Implements ISoundProducer

#Region " Events "

    Public Event EndReached(sender As ISoundProducer) Implements ISoundProducer.EndReached

    Public Event PositionChanged As ISoundProducer.PositionChangedEventHandler Implements ISoundProducer.PositionChanged

#End Region


#Region " Properties "

    Public ReadOnly Property EffectiveVolume As Double Implements ISoundProducer.EffectiveVolume
        Get
            Return 1
        End Get
    End Property


    Public Property Effects As IList(Of ISoundAutomation) = New List(Of ISoundAutomation)() Implements ISoundProducer.Effects


    Public ReadOnly Property GeneratedEffects As IList(Of ISoundAutomation) = New List(Of ISoundAutomation)() Implements ISoundProducer.GeneratedEffects


    Public Property IsMuted As Boolean Implements ISoundProducer.IsMuted


    Public Property Volume As Single = 1.0F Implements ISoundProducer.Volume


    Public Property Balance As Double Implements ISoundProducer.Balance

#End Region


#Region " Init and clean-up "

    Public Sub New()
        MyBase.New()
    End Sub


    Public Sub New(startAction As Action, stopAction As Action)
        MyBase.New(startAction, stopAction)
    End Sub

#End Region


#Region " API "


    ''' <inheritdoc/>
    Public Function GetEffectiveVolume() As Double Implements ISoundProducer.GetEffectiveVolume
        Return Volume
    End Function


    Public Sub SetEffectiveVolume(sender As ISoundAutomation, newVolume As Double) Implements ISoundProducer.SetEffectiveVolume

    End Sub


    Public Sub SimulateEndReached() Implements ISoundProducer.SimulateEndReached

    End Sub

#End Region

End Class
