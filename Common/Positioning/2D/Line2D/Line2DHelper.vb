''' <summary>
''' Represents a line in 2D space.
''' </summary>
<CLSCompliant(True)>
Public Class Line2DHelper

#Region " Factory API "

    ''' <summary>
    ''' Create a line through two points.
    ''' </summary>
    Public Shared Function Create(a As IPoint2D, b As IPoint2D) As Line2D
        Return New Line2D(a, b)
    End Function


    ''' <summary>
    ''' Create a line through a point towards the given vector.
    ''' </summary>
    Public Shared Function Create(a As IPoint2D, v As Vector2D) As Line2D
        Return New Line2D(a, v)
    End Function


    ''' <summary>
    ''' Create a border.
    ''' </summary>
    Public Shared Function CreateBorder(line As ILine2D, inside As Line2DDirections) As IBorder2D
        Return New Border2D(line, inside)
    End Function

#End Region


#Region " Geometry API "

    ''' <summary>
    ''' Check whether the two lines are parallel.
    ''' </summary>
    Public Shared Function AreParallel(line1 As ILine2D, line2 As ILine2D) As Boolean
        Dim d = line1.FactorX * line2.FactorY - line1.FactorY * line2.FactorX
        Return IsEqual(d, 0)
    End Function

#End Region

End Class
