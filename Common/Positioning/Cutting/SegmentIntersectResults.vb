''' <summary>
''' The order is important - modified first,
''' see <see cref="Polygon3D.Intersect"/>.
''' </summary>
<CLSCompliant(True)>
Public Enum SegmentIntersectResults

	''' <summary>
	''' Initial value.
	''' </summary>
	None

	''' <summary>
	''' The plane cuts the segment in two parts.
	''' </summary>
	Intersects

	''' <summary>
	''' The plane fully contains the line segment.
	''' </summary>
	Contains

	''' <summary>
	''' The plane contains one end of the line segment.
	''' </summary>
	Touches

	''' <summary>
	''' The plane does not touch the line segment, it shall stay intact.
	''' </summary>
	Beyond

End Enum
