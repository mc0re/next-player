<CLSCompliant(True)>
Public Interface ILineSegment3D

#Region " Properties "

    ReadOnly Property Line As ILine3D


    ''' <summary>
    ''' Current start position of the segment.
    ''' </summary>
    ReadOnly Property PointA As IPoint3D


    ''' <summary>
    ''' Current end position of the segment.
    ''' </summary>
    ReadOnly Property PointB As IPoint3D


    ReadOnly Property CoordinateA As Double


    ReadOnly Property CoordinateB As Double


    ReadOnly Property Length As Double

#End Region


#Region " API "

    Function Intersect(plane As IPlane3D) As SegmentIntersectInfo

    Function GetCutInfo(plane As IPlane3D, inside As Integer) As SegmentCutInfo

    Function ConvertToCutInfo(
        plane As IPlane3D, inside As Integer, intersectInfo As SegmentIntersectInfo
    ) As SegmentCutInfo

#End Region

End Interface
