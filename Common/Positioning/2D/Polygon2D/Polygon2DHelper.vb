
''' <summary>
''' Represents a flat polygon.
''' </summary>
<CLSCompliant(True)>
Public Class Polygon2DHelper

#Region " API "

    Public Shared Function CreateInfinite(Of TRef)() As Polygon2D(Of TRef)
        Return CreateInfinite(Enumerable.Empty(Of TRef)())
    End Function


    Public Shared Function CreateInfinite(Of TRef)(references As IEnumerable(Of TRef)) As Polygon2D(Of TRef)
        Return New Polygon2D(Of TRef)(references)
    End Function


    ''' <summary>
    ''' Create a polygon, which has the given vertices.
    ''' Collect vertices' references - pair-wise for the sides, all for the polygon.
    ''' </summary>
    ''' <param name="vertices">
    ''' The vertices must be defined in hull-order,
    ''' though the direction is not important.
    ''' </param>
    Public Shared Function Create(Of TRef)(
        vertices As IEnumerable(Of Point2D(Of TRef))
    ) As Polygon2D(Of TRef)

        Dim verticeList = vertices.ToList()
        If verticeList.Count < 3 Then
            Throw New ArgumentException("Polygon must have 3 or more vertices.")
        End If

        Dim inside = GetInsideDirection(verticeList)
        Dim sides = CreateSides(verticeList, inside)
        Dim allRefs = verticeList.SelectMany(Function(v) v.References)

        Return New Polygon2D(Of TRef)(sides, allRefs)
    End Function

#End Region


#Region " Utility "

    ''' <summary>
    ''' Go through the list of vertices and make sure
    ''' the polygon bends in the same direction.
    ''' </summary>
    Private Shared Function GetInsideDirection(vertices As IEnumerable(Of IPoint2D)) As Line2DDirections
        Dim verticeList = vertices.ToList()
        Dim indexList = Enumerable.Range(0, verticeList.Count).Concat({0, 1}).ToList()

        ' Initialized in the loop when idxA = 0
        Dim inside = Line2DDirections.Left
        Dim insideKnown = False

        For idxA = 0 To verticeList.Count - 1
            Dim idxB = indexList(idxA + 1)
            Dim idxC = indexList(idxA + 2)
            Dim pA = verticeList(idxA)
            Dim pB = verticeList(idxB)
            Dim line = Line2DHelper.Create(pA, pB)
            Dim dist = line.GetDistanceToLine(verticeList(idxC))

            If IsEqual(dist, 0) Then Continue For
            Dim thisInside = If(dist > 0, Line2DDirections.Right, Line2DDirections.Left)

            If Not insideKnown Then
                inside = thisInside
                insideKnown = True
            ElseIf inside <> thisInside Then
                Throw New ArgumentException("The polygon must be concave.")
            End If
        Next

        Return inside
    End Function


    ''' <summary>
    ''' Create sides pair-wise, collect their references
    ''' </summary>
    Private Shared Function CreateSides(Of TRef)(
        verticeList As List(Of Point2D(Of TRef)), inside As Line2DDirections
    ) As List(Of Polygon2DSide(Of TRef))

        Dim sides As New List(Of Polygon2DSide(Of TRef))()

        Dim indexList = Enumerable.Range(0, verticeList.Count).Concat({0}).ToList()

        For idxA = 0 To verticeList.Count - 1
            Dim pA = verticeList(idxA)
            Dim pB = verticeList(indexList(idxA + 1))

            sides.Add(Polygon2DSideHelper.CreatePoints(pA, pB, inside))
        Next

        Return sides
    End Function

#End Region

End Class
