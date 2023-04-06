
''' <summary>
''' Represents a flat polygon.
''' </summary>
<CLSCompliant(True)>
<Serializable>
Public Class Polygon2D(Of TRef)
    Implements IPolygon2D, IRefKeeper(Of TRef)

#Region " IRefKeeper.References read-only property "

    Private ReadOnly mReferences As New List(Of TRef)()


    Public ReadOnly Property References As IReadOnlyCollection(Of TRef) Implements IRefKeeper(Of TRef).References
        Get
            Return mReferences
        End Get
    End Property

#End Region


#Region " IPolygon2D properties "

    Private ReadOnly mVertices As New List(Of IPoint2D)()


    ''' <inheritdoc/>
    Public ReadOnly Property Vertices As IReadOnlyCollection(Of IPoint2D) Implements IPolygon2D.Vertices
        Get
            If Not mVertices.Any() Then
                For Each s In mSides
                    For Each c In {s.CoordinateA, s.CoordinateB}
                        Dim p = s.Line.GetPoint(c)
                        If Not mVertices.Any(Function(v) Point2DHelper.IsSame(v, p)) Then
                            mVertices.Add(p)
                        End If
                    Next
                Next
            End If

            Return mVertices
        End Get
    End Property

#End Region


#Region " Sides read-only property "

    Private ReadOnly mSides As New List(Of Polygon2DSide(Of TRef))()


    Public ReadOnly Property Sides As IReadOnlyList(Of Polygon2DSide(Of TRef))
        Get
            Return mSides
        End Get
    End Property

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Create an infinite polygon - i.e. it has no sides yet.
    ''' </summary>
    Friend Sub New(referenceList As IEnumerable(Of TRef))
        mReferences.AddRange(referenceList)
    End Sub


    ''' <summary>
    ''' Create an polygon with the given sides.
    ''' </summary>
    Friend Sub New(sides As IEnumerable(Of Polygon2DSide(Of TRef)), referenceList As IEnumerable(Of TRef))
        mSides.AddRange(sides)
        mReferences.AddRange(referenceList)
    End Sub

#End Region


#Region " API "

    ''' <summary>
    ''' Check whether the polygon fully contains the given point
    ''' (or it is on the line, when the polygon only has 2 points).
    ''' </summary>
    ''' <returns>True if contains</returns>
    Public Function Contains(p As IPoint2D) As Boolean Implements IObject2D.Contains
        Dim outSide = ContainsInternal(p)
        Return outSide Is Nothing
    End Function


    ''' <summary>
    ''' Cut this polygon in place by a line.
    ''' The original references are preserved.
    ''' </summary>
    ''' <returns>True if the resulting polygon is non-degenerate</returns>
    Public Function CutByLine(line As ILine2D, inside As Line2DDirections) As Boolean Implements IPolygon2D.CutByLine
        Return CutByLine(Line2DHelper.CreateBorder(line, inside))
    End Function


    ''' <summary>
    ''' Cut this polygon in place by a line.
    ''' The original references are preserved.
    ''' </summary>
    ''' <param name="border"></param>
    ''' <returns>True if the resulting polygon is non-degenerate</returns>
    Public Function CutByLine(border As IBorder2D) As Boolean Implements IPolygon2D.CutByLine
        ' No sides yet? The cutting line is the new side
        If Not mSides.Any() Then
            mSides.Add(Polygon2DSideHelper.CreateInfinite(Of TRef)(border.Line, border.Inside))
            Return True
        End If

        ' Create a new list of kept (possibly cut) sides
        Dim keptSides As New List(Of Polygon2DSide(Of TRef))()
        Dim cuttingSide As Polygon2DSide(Of TRef) = Nothing
        Dim isCuttingEliminated = False

        For Each candidate In mSides
            ' The newSide always has the same direction as the original polySide
            Dim newSide = candidate.CutByBorder(border)

            If newSide Is Nothing Then
                Continue For
            End If

            keptSides.Add(newSide)
            If isCuttingEliminated Then
                Continue For
            End If

            ' If cutLine crosses a side, the side cuts the line as well.
            Dim origSide = If(cuttingSide, Polygon2DSideHelper.CreateInfinite(Of TRef)(border.Line, border.Inside, {}))
            cuttingSide = origSide.CutByBorder(candidate)
            If cuttingSide Is Nothing Then
                isCuttingEliminated = True
                Continue For
            End If

            ' There is a special case when cutLine and polySide collide,
            ' then the cutSide becomes infinite. It should be shrinked to newSide's dimensions.
            If ReferenceEquals(origSide, cuttingSide) AndAlso
               cuttingSide.Line.Contains(newSide.Line.GetPoint(0)) Then

                cuttingSide = ShrinkSide(cuttingSide, newSide)
            End If
        Next

        ' Add the cutting side as well
        If cuttingSide IsNot Nothing AndAlso Not keptSides.Any(Function(s) Polygon2DSideHelper.IsSame(s, cuttingSide)) Then
            keptSides.Add(cuttingSide)
        End If

        mSides.Clear()

        If Not keptSides.Any() Then
            ' Degenerate polygon
            Return False
        End If

        mSides.AddRange(keptSides)

        Return True
    End Function

#End Region


#Region " Utility "

    ''' <summary>
    ''' Shrink the given side by the size of the limiting side,
    ''' assuming they are colinear.
    ''' The original references are kept.
    ''' </summary>
    Private Shared Function ShrinkSide(
        sideToShrink As Polygon2DSide(Of TRef), limit As IPolygon2DSide
    ) As Polygon2DSide(Of TRef)

        Dim limitLine = CType(limit, ILineSegment2D).Line

        ' A and B limits for sideToShrink, A is always less than B
        Dim coords = {
                limitLine.GetPoint(limit.CoordinateA),
                limitLine.GetPoint(limit.CoordinateB)
            }.
            Select(Function(c) sideToShrink.Line.GetCoordinate(c)).
            OrderBy(Function(c) c).
            ToList()

        Dim newA = Math.Max(sideToShrink.CoordinateA, coords(0))
        Dim newB = Math.Min(sideToShrink.CoordinateB, coords(1))

        Return Polygon2DSideHelper.Create(
            sideToShrink.Line,
            newA, newB,
            sideToShrink.InsideDirection,
            sideToShrink.References)
    End Function


    ''' <summary>
    ''' Check whether the polygon fully contains the given point
    ''' (or it is on the line, when the polygon only has 2 points).
    ''' </summary>
    ''' <returns>
    ''' If the point is not contained, returns the segment closest to the point <paramref name="p"/>.
    ''' If it is, Nothing is returned.
    ''' </returns>
    Private Function ContainsInternal(p As IPoint2D) As Polygon2DSide(Of TRef)
        Dim outSide As Polygon2DSide(Of TRef) = Nothing
        Dim outSideDist = Double.PositiveInfinity

        For Each segm In mSides
            Dim dist = segm.Line.GetDistanceToLine(p)
            Dim expectedSign = 0

            Select Case segm.InsideDirection
                Case Line2DDirections.Left
                    expectedSign = -1
                Case Line2DDirections.Right
                    expectedSign = 1
            End Select

            Dim direction = Sign(dist)

            If direction = 0 Then
                ' On the boundary - return it as the closest side.
                Return segm
            End If

            If direction = expectedSign Then
                ' Completely inside
                Continue For
            End If

            ' Find the outside side closest to the origin
            If Math.Abs(dist) < outSideDist Then
                outSide = segm
                outSideDist = Math.Abs(dist)
            End If
        Next

        Return outSide
    End Function

#End Region

End Class
