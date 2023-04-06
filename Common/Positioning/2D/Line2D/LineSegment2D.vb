Imports System.Diagnostics.CodeAnalysis


''' <summary>
''' A line segment (2 points) in a 2D coordinate system.
''' </summary>
<CLSCompliant(True)>
<Serializable>
Public Class LineSegment2D(Of TRef)
    Implements ILineSegment2D, IRefKeeper(Of TRef)

#Region " IRefKeeper property "

    Public ReadOnly Property References As IReadOnlyCollection(Of TRef) Implements IRefKeeper(Of TRef).References

#End Region


#Region " ILine2DSegment properties "

    ''' <summary>
    ''' Relative coordinate of end point A on <see cref="Line"/> or Nothing.
    ''' </summary>
    Public ReadOnly Property CoordinateA As Double Implements ILineSegment2D.CoordinateA


    ''' <summary>
    ''' Relative coordinate of end point B on <see cref="Line"/> or Nothing.
    ''' </summary>
    Public ReadOnly Property CoordinateB As Double Implements ILineSegment2D.CoordinateB


    ''' <summary>
    ''' Line along which the segment is aligned.
    ''' </summary>
    Public ReadOnly Property Line As ILine2D Implements ILineSegment2D.Line

#End Region


#Region " Own properties "

    Public ReadOnly Property PointA As Point2D(Of TRef)

    Public ReadOnly Property PointB As Point2D(Of TRef)

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Create a segment between two points.
    ''' </summary>
    ''' <param name="a">Point A</param>
    ''' <param name="b">Point B</param>
    Friend Sub New(a As Point2D(Of TRef), b As Point2D(Of TRef))
        Line = Line2DHelper.Create(a, b)
        PointA = a
        PointB = b
        CoordinateA = Line.GetCoordinate(a)
        CoordinateB = Line.GetCoordinate(b)
        References = ReferencesHelper.MergeReferences({a, b})
    End Sub


    ''' <summary>
    ''' Create a segment going to infinity along the given line in both directions.
    ''' </summary>
    ''' <param name="line">Parent line</param>
    Friend Sub New(line As ILine2D, referenceList As IEnumerable(Of TRef))
        CoordinateA = Double.NegativeInfinity
        CoordinateB = Double.PositiveInfinity
        Me.Line = line
        References = referenceList.ToList()
    End Sub


    ''' <summary>
    ''' Create a segment between two points along the line.
    ''' </summary>
    ''' <param name="line">Parent line</param>
    ''' <param name="a">Point A's relative coordinate on the line</param>
    ''' <param name="b">Point B's relative coordinate on the line</param>
    Friend Sub New(line As ILine2D, a As Double, b As Double, referenceList As IEnumerable(Of TRef))
        If Sign(a - b) > 0 Then Throw New ArgumentException("Start coordinate must be less then end.")

        Me.Line = line
        CoordinateA = a
        CoordinateB = b
        References = referenceList.ToList()
    End Sub

#End Region


#Region " API "

    ''' <inheritdoc/>
    Public Function Contains(location As IPoint2D) As Boolean Implements IObject2D.Contains
        If Not Line.Contains(location) Then Return False

        Dim coord = Line.GetCoordinate(location)

        Return Sign(CoordinateA - coord) < 0 AndAlso Sign(coord - CoordinateB) < 0
    End Function

#End Region


#Region " ToString "

    <ExcludeFromCodeCoverage>
    Public Overrides Function ToString() As String
        Dim a = If(Double.IsNegativeInfinity(CoordinateA), "-Inf", Line.GetPoint(CoordinateA).ToString())
        Dim b = If(Double.IsPositiveInfinity(CoordinateB), "+Inf", Line.GetPoint(CoordinateB).ToString())
        Return String.Format("{0} - {1} / {2}", a, b, ReferencesHelper.AsString(References))
    End Function

#End Region

End Class
