<CLSCompliant(True)>
Public Class Rectangle2D
    Implements IObject2D

#Region " Properties "

    ''' <summary>
    ''' Coordinate of the left border along X axis (the lesser of the pair).
    ''' </summary>
    Public ReadOnly Property Left As Double


    ''' <summary>
    ''' Coordinate of the right border along X axis (the greater of the pair).
    ''' </summary>
    Public ReadOnly Property Right As Double


    ''' <summary>
    ''' Coordinate of the back border along Y axis (the lesser of the pair).
    ''' </summary>
    Public ReadOnly Property Back As Double


    ''' <summary>
    ''' Coordinate of the front border along Y axis (the greater of the pair).
    ''' </summary>
    Public ReadOnly Property Front As Double


    Public ReadOnly Property Sides As IReadOnlyCollection(Of IPolygon2DSide)

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Create a symmetrical ove (0, 0) rectangle.
    ''' </summary>
    ''' <param name="width">Width of the rectangle (X axis)</param>
    ''' <param name="height">Height of the rectangle (Y axis)</param>
    Public Sub New(width As Double, height As Double)
        Me.New(-width / 2, width / 2, -height / 2, height / 2)
    End Sub


    ''' <summary>
    ''' Create a rectangle.
    ''' </summary>
    Public Sub New(left As Double, right As Double, back As Double, front As Double)
        Me.Left = left
        Me.Right = right
        Me.Back = back
        Me.Front = front

        Dim lb = Point2DHelper.Create(left, back)
        Dim backLine = Line2DHelper.Create(lb, Vector2D.AlongX)
        Dim leftLine = Line2DHelper.Create(lb, Vector2D.AlongY)
        Dim rf = Point2DHelper.Create(right, front)
        Dim frontLine = Line2DHelper.Create(rf, Vector2D.AlongX)
        Dim rightLine = Line2DHelper.Create(rf, Vector2D.AlongY)

        Sides = New List(Of IPolygon2DSide) From
        {
            Polygon2DSideHelper.Create(Of NoRef)(leftLine, back, front, Line2DDirections.Right),
            Polygon2DSideHelper.Create(Of NoRef)(frontLine, left, right, Line2DDirections.Right),
            Polygon2DSideHelper.Create(Of NoRef)(rightLine, back, front, Line2DDirections.Left),
            Polygon2DSideHelper.Create(Of NoRef)(backLine, left, right, Line2DDirections.Left)
        }
    End Sub

#End Region


#Region " API "

    Public Function Contains(location As IPoint2D) As Boolean Implements IObject2D.Contains
        Return Left <= location.X AndAlso location.X <= Right AndAlso
               Back <= location.Y AndAlso location.Y <= Front
    End Function

#End Region

End Class
