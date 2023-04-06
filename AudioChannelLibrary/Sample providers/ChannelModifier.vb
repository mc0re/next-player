''' <summary>
''' Describes the sound modification, when it is played
''' from the given physical channel as though it would've been
''' played from a point in space.
''' </summary>
<CLSCompliant(True)>
Public Class ChannelModifier

#Region " Delay read-only property "

    Private ReadOnly mDelay As Single


    ''' <summary>
    ''' Delay (0 or greater) required for the simulation.
    ''' </summary>
    Public ReadOnly Property Delay As Single
        Get
            Return mDelay
        End Get
    End Property

#End Region


#Region " Volume read-only property "

    Private ReadOnly mVolume As Single()


    ''' <summary>
    ''' Coefficient (0 to 1) required for the simulation.
    ''' </summary>
    ''' <remarks>
    ''' If the index is beyond the array size, return 0 just in case.
    ''' </remarks>
    Public ReadOnly Property Volume(srcIdx As Integer) As Single
        Get
            Return If(srcIdx < mVolume.Length, mVolume(srcIdx), 0)
        End Get
    End Property

#End Region


#Region " SourceChannelCount read-only property "

    ''' <summary>
    ''' The number of source channels.
    ''' </summary>
    Public ReadOnly Property SourceChannelCount As Integer
        Get
            Return mVolume.Length
        End Get
    End Property

#End Region


#Region " Init and clean-up "

    Public Sub New(delay As Single, coef As Single())
        mDelay = delay
        mVolume = coef
    End Sub

#End Region

End Class
