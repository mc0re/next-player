Imports System.Diagnostics.CodeAnalysis


''' <summary>
''' Basic line with all the needed calculations.
''' </summary>
<CLSCompliant(True)>
<Serializable>
Public Class Line2D
    Implements ILine2D

#Region " Fields "

    ' Along-the-line distance equation
    Private ReadOnly mDistX As Double

    Private ReadOnly mDistY As Double

    Private ReadOnly mDist As Double

#End Region


#Region " ILine2D properties "

    ''' <inheritdoc/>
    Public ReadOnly Property FactorX As Double Implements ILine2D.FactorX


    ''' <inheritdoc/>
    Public ReadOnly Property FactorY As Double Implements ILine2D.FactorY


    ''' <inheritdoc/>
    Public ReadOnly Property Offset As Double Implements ILine2D.Offset

#End Region


#Region " Own properties "

    ''' <summary>
    ''' Origin point in line's coordinate system.
    ''' </summary>
    Public ReadOnly Property Point As IPoint2D


    ''' <inheritdoc/>
    Public ReadOnly Property Vector As Vector2D Implements ILine2D.Vector

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Create a line through two points.
    ''' </summary>
    Friend Sub New(a As IPoint2D, b As IPoint2D)
        Point = a

        Dim dx = b.X - a.X
        Dim dy = b.Y - a.Y
        Vector = Vector2DHelper.Create(dx, dy)

        Dim div = Math.Sqrt(dx * dx + dy * dy)

        If IsEqual(div, 0) Then
            Throw New ArgumentException("Cannot create a line from colliding points")
        End If

        FactorX = dy / div
        FactorY = -dx / div
        Offset = (b.X * a.Y - a.X * b.Y) / div

        If Not IsEqual(FactorY, 0) Then
            mDistX = 1 / (b.X - a.X)
            mDistY = 0
            mDist = -a.X / (b.X - a.X)
        Else
            mDistX = 0
            mDistY = 1 / (b.Y - a.Y)
            mDist = -a.Y / (b.Y - a.Y)
        End If
    End Sub


    ''' <summary>
    ''' Create a line through two points.
    ''' </summary>
    ''' <remarks>
    ''' The distance from point to line is |AC - b * AB|, where b = AC * AB / AB^2.
    ''' </remarks>
    Friend Sub New(a As IPoint2D, v As Vector2D)
        Me.New(a, v.FromPoint(a))
    End Sub

#End Region


#Region " ILine2D implementation "

    ''' <inheritdoc/>
    Public Function Contains(p As IPoint2D) As Boolean Implements IObject2D.Contains
        Return IsEqual(GetDistanceToLine(p), 0)
    End Function


    ''' <inheritdoc/>
    Public Function GetDistanceToLine(c As IPoint2D) As Double Implements ILine2D.GetDistanceToLine
        Return FactorX * c.X + FactorY * c.Y + Offset
    End Function


    ''' <inheritdoc/>
    Public Function GetCoordinate(c As IPoint2D) As Double Implements ILine2D.GetCoordinate
        Return mDistX * c.X + mDistY * c.Y + mDist
    End Function


    ''' <inheritdoc/>
    Public Function GetPoint(u As Double) As IPoint2D Implements ILine2D.GetPoint
        Dim toInfinity = Function(coef As Double, def As Double) As Double
                             Select Case Sign(coef)
                                 Case -1 : Return Double.NegativeInfinity
                                 Case 1 : Return Double.PositiveInfinity
                                 Case Else : Return def
                             End Select
                         End Function

        If Double.IsNegativeInfinity(u) Then
            Return Point2DHelper.Create(toInfinity(-Vector.X, Point.X), toInfinity(-Vector.Y, Point.Y))
        ElseIf Double.IsPositiveInfinity(u) Then
            Return Point2DHelper.Create(toInfinity(Vector.X, Point.X), toInfinity(Vector.Y, Point.Y))
        End If

        Return Point2DHelper.Create(Point.X + u * Vector.X, Point.Y + u * Vector.Y)
    End Function


    ''' <inheritdoc/>
    Public Function CutByLine(cutLine As ILine2D) As LineCutResult Implements ILine2D.CutByLine
        Dim d = FactorX * cutLine.FactorY - FactorY * cutLine.FactorX

        If IsEqual(d, 0) Then
            Return Nothing
        End If

        Dim dx = cutLine.Offset * FactorY - Offset * cutLine.FactorY
        Dim dy = -cutLine.Offset * FactorX + Offset * cutLine.FactorX
        Dim cutX = dx / d
        Dim cutY = dy / d

        Return New LineCutResult With {
            .Coordinate = mDistX * cutX + mDistY * cutY + mDist,
            .VectorSign = Sign(d)
        }
    End Function


    ''' <inheritdoc/>
    Public Function Perpendicular(pt As IPoint2D) As ILine2D Implements ILine2D.Perpendicular
        Return Line2DHelper.Create(pt, Vector.Perpendicular)
    End Function


    ''' <inheritdoc/>
    Public Function Revert() As ILine2D Implements ILine2D.Revert
        Return Line2DHelper.Create(Point, Vector.Multiply(-1))
    End Function

#End Region


#Region " ToString "

    ''' <summary>
    ''' For debugging.
    ''' </summary>
    <ExcludeFromCodeCoverage>
    Public Overrides Function ToString() As String
        Return $"{Point} {Vector}"
    End Function

#End Region

End Class
