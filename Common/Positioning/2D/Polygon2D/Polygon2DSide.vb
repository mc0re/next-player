Imports System.Diagnostics.CodeAnalysis


<CLSCompliant(True)>
<Serializable>
Public Class Polygon2DSide(Of TRef)
    Inherits LineSegment2D(Of TRef)
    Implements IPolygon2DSide, IRefKeeper(Of TRef)

#Region " IBorder2D properties "

    ''' <summary>
    ''' Border line.
    ''' </summary>
    Private ReadOnly Property IBorder2DLine As ILine2D Implements IBorder2D.Line
        Get
            Return Line
        End Get
    End Property


    ''' <summary>
    ''' Which direction is inside the polygon, when looking along the line's vector.
    ''' </summary>
    Public ReadOnly Property InsideDirection As Line2DDirections Implements IBorder2D.Inside

#End Region


#Region " Init and clean-up "

    Friend Sub New(line As ILine2D, inside As Line2DDirections, ref As IEnumerable(Of TRef))
        MyBase.New(line, ref)
        InsideDirection = inside
    End Sub


    Friend Sub New(a As Point2D(Of TRef), b As Point2D(Of TRef), inside As Line2DDirections)
        MyBase.New(a, b)
        InsideDirection = inside
    End Sub


    Friend Sub New(line As ILine2D, a As Double, b As Double, inside As Line2DDirections, ref As IEnumerable(Of TRef))
        MyBase.New(line, a, b, ref)
        InsideDirection = inside
    End Sub

#End Region


#Region " API "

    ''' <summary>
    ''' Intersect this side by another border.
    ''' </summary>
    ''' <returns>New side (possibly identical) or Nothing, if this side is eliminated by the cut</returns>
    Public Function CutByBorder(border As IBorder2D) As Polygon2DSide(Of TRef)
        If Line2DHelper.AreParallel(Line, border.Line) Then
            Return If(IsCutByParallel(border), Me, Nothing)
        End If

        ' Check if the cut is actually inside the side
        Dim cut = Line.CutByLine(border.Line)
        Dim isCuttingA = cut.VectorSign > 0 AndAlso border.Inside = Line2DDirections.Right OrElse
                         cut.VectorSign < 0 AndAlso border.Inside = Line2DDirections.Left

        If isCuttingA Then
            If Sign(cut.Coordinate - CoordinateB) >= 0 Then Return Nothing

            Dim newA = Math.Max(CoordinateA, cut.Coordinate)
            Return Polygon2DSideHelper.Create(Of TRef)(Line, newA, CoordinateB, InsideDirection)
        Else
            If Sign(cut.Coordinate - CoordinateA) <= 0 Then Return Nothing

            Dim newB = Math.Min(CoordinateB, cut.Coordinate)
            Return Polygon2DSideHelper.Create(Of TRef)(Line, CoordinateA, newB, InsideDirection)
        End If
    End Function

#End Region


#Region " Utility "

    ''' <summary>
    ''' Check if 'this' line is cut out by <paramref name="border"/>.
    ''' <paramref name="border"/> must be parallel to this line.
    ''' </summary>
    ''' <returns>True if the line is inside (not cut out)</returns>
    ''' <remarks>
    ''' If the borders collide, their 'inside' directions must be the same.
    ''' </remarks>
    Private Function IsCutByParallel(border As IBorder2D) As Boolean
        Dim toSideLine = Sign(border.Line.GetDistanceToLine(Line.GetPoint(0)))

        If toSideLine = 0 Then
            Dim sameDir = Vector2DHelper.CompareDirections(border.Line.Vector, Line.Vector)
            Dim sameInside = border.Inside = InsideDirection
            Dim eliminated = sameDir > 0 AndAlso Not sameInside OrElse
                             sameDir < 0 AndAlso sameInside

            Return Not eliminated
        Else
            Dim isSideOutsideCut = toSideLine > 0 AndAlso border.Inside = Line2DDirections.Right OrElse
            toSideLine < 0 AndAlso border.Inside = Line2DDirections.Left

            Return isSideOutsideCut
        End If
    End Function

#End Region


#Region " ToString "

    <ExcludeFromCodeCoverage>
    Public Overrides Function ToString() As String
        Return $"{MyBase.ToString()}, inside {InsideDirection}"
    End Function

#End Region

End Class
