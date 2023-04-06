

<CLSCompliant(True)>
Public Class LineSegment2DHelper

#Region " Equality comparison "

    Private NotInheritable Class Polygon2DSideEqualityComparer
        Implements IEqualityComparer(Of ILineSegment2D)

        Public Shadows Function Equals(x As ILineSegment2D, y As ILineSegment2D) As Boolean Implements IEqualityComparer(Of ILineSegment2D).Equals
            Return IsSame(x, y) <> 0
        End Function


        ''' <summary>
        ''' Get object's hash code.
        ''' </summary>
        ''' <remarks>
        ''' Different hash codes mean different objects,
        ''' so the actual equality method would not be even called.
        ''' </remarks>
        Public Shadows Function GetHashCode(obj As ILineSegment2D) As Integer Implements IEqualityComparer(Of ILineSegment2D).GetHashCode
            Return obj.Line.GetHashCode() Xor
                obj.CoordinateA.GetHashCode() Xor obj.CoordinateB.GetHashCode()
        End Function

    End Class


    Public Shared Property EqualityComparer As IEqualityComparer(Of ILineSegment2D) =
        New Polygon2DSideEqualityComparer()

#End Region


#Region " Factory API "

    ''' <summary>
    ''' Create a segment between two points.
    ''' </summary>
    ''' <param name="a">Point A</param>
    ''' <param name="b">Point B</param>
    Public Shared Function Create(Of TRef)(
        a As Point2D(Of TRef), b As Point2D(Of TRef)
    ) As LineSegment2D(Of TRef)

        Return New LineSegment2D(Of TRef)(a, b)
    End Function


    ''' <summary>
    ''' Create an segment going to infinity along the given line in both directions.
    ''' Take line's references.
    ''' </summary>
    ''' <param name="line">Parent line</param>
    Public Shared Function Create(Of TRef)(
        line As ILine2D, ref As IEnumerable(Of TRef)
    ) As LineSegment2D(Of TRef)

        Return New LineSegment2D(Of TRef)(line, ref)
    End Function


    ''' <summary>
    ''' Create a semiopen segment from the given point along the given vector.
    ''' </summary>
    ''' <typeparam name="TRef"></typeparam>
    Public Shared Function Create(Of TRef)(
        a As IPoint2D, v As Vector2D, ref As IEnumerable(Of TRef)
    ) As LineSegment2D(Of TRef)

        Dim line = Line2DHelper.Create(a, v)

        Return New LineSegment2D(Of TRef)(line, 0, Double.PositiveInfinity, ref)
    End Function


    ''' <summary>
    ''' Create a semiopen segment from the given point along the given vector.
    ''' References are given separately.
    ''' </summary>
    ''' <typeparam name="TRef"></typeparam>
    Public Shared Function Create(Of TRef)(
        line As ILine2D, a As Double, b As Double,
        referenceList As IEnumerable(Of TRef)
    ) As LineSegment2D(Of TRef)

        Return New LineSegment2D(Of TRef)(line, a, b, referenceList)
    End Function

#End Region


#Region " API "

    ''' <summary>
    ''' Check whether the two segments are identical (or reversed); 0 = different.
    ''' </summary>
    ''' <param name="segment1">Line segment 1</param>
    ''' <param name="segment2">Line segment 2</param>
    ''' <returns>
    ''' 0 if they are different
    ''' 1 if the segments are the same
    ''' -1 if the segments are reversed
    ''' </returns>
    Public Shared Function IsSame(segment1 As ILineSegment2D, segment2 As ILineSegment2D) As Integer
        Dim segment1A = segment1.Line.GetPoint(segment1.CoordinateA)
        Dim segment1B = segment1.Line.GetPoint(segment1.CoordinateB)
        Dim segment2A = segment2.Line.GetPoint(segment2.CoordinateA)
        Dim segment2B = segment2.Line.GetPoint(segment2.CoordinateB)

        If Point2DHelper.IsSame(segment1A, segment2A) AndAlso Point2DHelper.IsSame(segment1B, segment2B) Then
            Return 1
        ElseIf Point2DHelper.IsSame(segment1A, segment2B) AndAlso Point2DHelper.IsSame(segment1B, segment2A) Then
            Return -1
        Else
            Return 0
        End If
    End Function

#End Region

End Class
