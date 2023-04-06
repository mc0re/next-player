Imports Common


Friend Class ColinearCoordinate(Of TRef)

#Region " Properties "

    Public ReadOnly Property Coordinate As Double

    Public ReadOnly Property Perpendicular As ILine2D

    Public ReadOnly Property Point As Point2D(Of TRef)

    Public ReadOnly Property References As IReadOnlyCollection(Of TRef)

#End Region


#Region " Init and clean-up "

    Public Sub New(coordinate As Double, perpendicular As ILine2D, point As Point2D(Of TRef))
        Me.Point = point
        Me.Coordinate = coordinate
        Me.Perpendicular = perpendicular
        References = point.References
    End Sub

#End Region

End Class
