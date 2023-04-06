Imports System.Diagnostics.CodeAnalysis


''' <summary>
''' As this class is mainly used for cutting,
''' where the original points cannot be preserved,
''' it is not a generic class.
''' </summary>
<CLSCompliant(True)>
<Serializable>
Public Class LineSegment3D(Of TRef)
    Implements ILineSegment3D, IRefKeeper(Of TRef)

#Region " ILineSegment3D properties "

    Public ReadOnly Property Line As ILine3D Implements ILineSegment3D.Line


    Public ReadOnly Property PointA As IPoint3D Implements ILineSegment3D.PointA


    Public ReadOnly Property PointB As IPoint3D Implements ILineSegment3D.PointB


    Public ReadOnly Property CoordinateA As Double Implements ILineSegment3D.CoordinateA


    Public ReadOnly Property CoordinateB As Double Implements ILineSegment3D.CoordinateB

#End Region


#Region " ILineSegment3D.Length calculated property "

    Public ReadOnly Property Length As Double Implements ILineSegment3D.Length
        Get
            Return Line.Vector.Length * (CoordinateB - CoordinateA)
        End Get
    End Property

#End Region


#Region " IRefKeeper properties "

    Private ReadOnly mReferences As New List(Of TRef)()


    Public ReadOnly Property References As IReadOnlyCollection(Of TRef) Implements IRefKeeper(Of TRef).References
        Get
            Return mReferences
        End Get
    End Property

#End Region


#Region " Init and clean-up "

    Friend Sub New(a As IPoint3D, b As IPoint3D, ref As IEnumerable(Of TRef))
        Line = Line3DHelper.Create(a, b)
        PointA = Point3DHelper.Create(a)
        PointB = Point3DHelper.Create(b)
        CoordinateA = 0
        CoordinateB = 1

        mReferences.AddRange(ref)
    End Sub

#End Region


#Region " API "

    ''' <summary>
    ''' Check how this segment intersects with the given plane.
    ''' </summary>
    ''' <param name="plane">Plane to check against</param>
    ''' <returns>Intersection type and possibly its position</returns>
    Public Function Intersect(plane As IPlane3D) As SegmentIntersectInfo Implements ILineSegment3D.Intersect
        Dim res As New SegmentIntersectInfo()
        Dim cutPosNullable = Line.CutByPlane(plane)

        ' Line and plane are parallel, check containment
        If cutPosNullable Is Nothing Then
            res.Action = If(
                plane.IsPointOnPlane(Line.Point),
                SegmentIntersectResults.Contains,
                SegmentIntersectResults.Beyond)
            Return res
        End If

        Dim cutPos = cutPosNullable.Coordinate

        ' Check ends
        If Sign(cutPos - CoordinateA) < 0 OrElse Sign(cutPos - CoordinateB) > 0 Then
            res.Action = SegmentIntersectResults.Beyond

        ElseIf IsEqual(cutPos, CoordinateA) OrElse IsEqual(cutPos, CoordinateB) Then
            res.Action = SegmentIntersectResults.Touches
            res.Position = cutPos

        Else
            res.Action = SegmentIntersectResults.Intersects
            res.Position = cutPos
        End If

        Return res
    End Function


    ''' <summary>
    ''' Try cutting this segment by a plane
    ''' and keep the part inside the plane.
    ''' </summary>
    Public Function GetCutInfo(plane As IPlane3D, inside As Integer) As SegmentCutInfo Implements ILineSegment3D.GetCutInfo
        Debug.Assert(inside <> 0)
        Dim intersectInfo = Intersect(plane)

        Return ConvertToCutInfo(plane, inside, intersectInfo)
    End Function

#End Region


#Region " Utility "

    ''' <summary>
    ''' Separated primarily for the sake of unit-testing.
    ''' </summary>
    Public Function ConvertToCutInfo(
        plane As IPlane3D, inside As Integer, intersectInfo As SegmentIntersectInfo
    ) As SegmentCutInfo Implements ILineSegment3D.ConvertToCutInfo

        Dim cutRes As New SegmentCutInfo()

        Select Case intersectInfo.Action
            Case SegmentIntersectResults.Intersects, SegmentIntersectResults.Touches
                CreateCutInfoIntersect(plane, inside, cutRes, intersectInfo.Position)

            Case SegmentIntersectResults.Contains
                cutRes.Action = SegmentCutResults.Contains

            Case SegmentIntersectResults.Beyond
                ' Both points are on the same side, pick any
                Dim dir = Sign(plane.GetDistanceToPoint(PointA))
                cutRes.Action = If(dir = inside, SegmentCutResults.Beyond, SegmentCutResults.EliminateFromA)

            Case Else
                Throw New ArgumentException(String.Format(
                    "Unknown intersection result '{0}'", intersectInfo.Action.ToString()))
        End Select

        Return cutRes
    End Function


    ''' <summary>
    ''' We know that the intersection occurs, at a vertice or in between.
    ''' </summary>
    Private Sub CreateCutInfoIntersect(
        plane As IPlane3D, inside As Integer,
        cutRes As SegmentCutInfo, cutPos As Double
    )
        Dim abIsInside = Sign(Line.Vector.DotProduct(plane.Normal.Multiply(inside))) = 1
        cutRes.Action = SegmentCutResults.Beyond

        ' If the plane "inside" direction (which is Normal * inside)
        ' goes along the line direction, point A is cut off.
        ' If it is the opposite, point B is cut off.
        If abIsInside Then
            ' Leave intact from cut point to B
            If Sign(cutPos - CoordinateB) >= 0 Then
                cutRes.Action = SegmentCutResults.EliminateFromB
            ElseIf Sign(cutPos - CoordinateA) > 0 Then
                cutRes.Action = SegmentCutResults.KeepB
            End If

        Else
            ' Leave intact from cut point to A
            If Sign(cutPos - CoordinateA) <= 0 Then
                cutRes.Action = SegmentCutResults.EliminateFromA
            ElseIf Sign(cutPos - CoordinateB) < 0 Then
                cutRes.Action = SegmentCutResults.KeepA
            End If
        End If

        cutRes.Position = cutPos
    End Sub

#End Region


#Region " ToString "

    <ExcludeFromCodeCoverage>
    Public Overrides Function ToString() As String
        Return $"{PointA} - {PointB} / {ReferencesHelper.AsString(References)}"
    End Function

#End Region

End Class
