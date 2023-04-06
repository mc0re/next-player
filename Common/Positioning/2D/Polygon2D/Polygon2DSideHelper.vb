Public Class Polygon2DSideHelper

#Region " Factory API "

    ''' <summary>
    ''' Create an infinite side.
    ''' </summary>
    ''' <param name="line"></param>
    ''' <param name="inside"></param>
    Public Shared Function CreateInfinite(Of TRef)(line As ILine2D, inside As Line2DDirections) As Polygon2DSide(Of TRef)
        Return New Polygon2DSide(Of TRef)(line, inside, Enumerable.Empty(Of TRef)())
    End Function


    ''' <summary>
    ''' Create a polygon side limited by lines coordinates.
    ''' </summary>
    Public Shared Function Create(Of TRef)(
        line As ILine2D, a As Double, b As Double,
        inside As Line2DDirections
    ) As Polygon2DSide(Of TRef)

        Return New Polygon2DSide(Of TRef)(line, a, b, inside, Enumerable.Empty(Of TRef)())
    End Function


    ''' <summary>
    ''' Create an infinite side.
    ''' </summary>
    ''' <param name="line"></param>
    ''' <param name="inside"></param>
    Public Shared Function CreateInfinite(Of TRef)(
        line As ILine2D, inside As Line2DDirections, ref As IEnumerable(Of TRef)
    ) As Polygon2DSide(Of TRef)

        Return New Polygon2DSide(Of TRef)(line, inside, ref)
    End Function


    ''' <summary>
    ''' Create a side limited by two points.
    ''' </summary>
    ''' <param name="a"></param>
    ''' <param name="b"></param>
    ''' <param name="inside"></param>
    ''' <returns></returns>
    Public Shared Function CreatePoints(Of TRef)(
        a As Point2D(Of TRef), b As Point2D(Of TRef), inside As Line2DDirections
    ) As Polygon2DSide(Of TRef)

        Return New Polygon2DSide(Of TRef)(a, b, inside)
    End Function


    ''' <summary>
    ''' Create a side limited by two points.
    ''' </summary>
    Public Shared Function CreatePoints(
        a As Point2D(Of NoRef), b As Point2D(Of NoRef), inside As Line2DDirections
    ) As IPolygon2DSide

        Return New Polygon2DSide(Of NoRef)(a, b, inside)
    End Function


    ''' <summary>
    ''' Create a polygon side limited by lines coordinates.
    ''' </summary>
    Public Shared Function Create(Of TRef)(
        line As ILine2D, a As Double, b As Double,
        inside As Line2DDirections,
        references As IReadOnlyCollection(Of TRef)
    ) As Polygon2DSide(Of TRef)

        Return New Polygon2DSide(Of TRef)(line, a, b, inside, references)
    End Function

#End Region


#Region " API "

    ''' <summary>
    ''' Check whether the two sides are essentially the same (including the direction).
    ''' </summary>
    Public Shared Function IsSame(side1 As IPolygon2DSide, side2 As IPolygon2DSide) As Boolean
        Dim res = LineSegment2DHelper.IsSame(side1, side2)

        If side1.Inside = side2.Inside Then
            Return res = 1
        Else
            Return res = -1
        End If
    End Function

#End Region

End Class
