<CLSCompliant(True)>
Public Class PlaneIntersectionResult

#Region " Properties "

    ''' <summary>
    ''' Whether there is an intersection.
    ''' </summary>
    Public State As PlaneIntersectionResults


    ''' <summary>
    ''' In case there is an intersection, the actual line where it happens for it.
    ''' </summary>
    Public Line As ILine2D


    ''' <summary>
    ''' In case there is an intersection, which direction is "inside".
    ''' </summary>
    Public Inside As Line2DDirections

#End Region


#Region " Constants "

    Public Shared ReadOnly Eliminated As New PlaneIntersectionResult(PlaneIntersectionResults.Eliminated)

    Public Shared ReadOnly Kept As New PlaneIntersectionResult(PlaneIntersectionResults.Kept)

#End Region


#Region " Init and clean-up "

    Public Sub New(state As PlaneIntersectionResults)
        Me.State = state
    End Sub


    Public Sub New(line As Line2D, inside As Line2DDirections)
        State = PlaneIntersectionResults.Line
        Me.Line = line
        Me.Inside = inside
    End Sub

#End Region

End Class
