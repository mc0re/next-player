Public Class Border2D
    Implements IBorder2D

#Region " IBorder2D implementation "

    Public ReadOnly Property Line As ILine2D Implements IBorder2D.Line

    Public ReadOnly Property Inside As Line2DDirections Implements IBorder2D.Inside

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' COnvenience constructor.
    ''' </summary>
    Public Sub New(line As ILine2D, inside As Line2DDirections)
        Me.Line = line
        Me.Inside = inside
    End Sub

#End Region

End Class
