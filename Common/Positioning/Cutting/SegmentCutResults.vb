<CLSCompliant(True)>
Public Enum SegmentCutResults

	''' <summary>
	''' Initial value.
	''' </summary>
	None

	''' <summary>
	''' Point A is cut off (and replaced), point B remains.
	''' </summary>
	KeepB

	''' <summary>
	''' Point B is cut off (and replaced), point A remains.
	''' </summary>
	KeepA

	''' <summary>
	''' The plane's inside includes, if any, only point B,
	''' so the segment shall be deleted.
	''' </summary>
	EliminateFromB

	''' <summary>
	''' The plane's inside includes, if any, only point A,
	''' so the segment shall be deleted.
	''' </summary>
	EliminateFromA

	''' <summary>
	''' The plane fully contains the line segment.
	''' </summary>
	Contains

	''' <summary>
	''' The plane does not touch the line segment, it shall stay intact.
	''' </summary>
	Beyond

End Enum
