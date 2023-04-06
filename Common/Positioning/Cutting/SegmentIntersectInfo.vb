Imports System.Diagnostics.CodeAnalysis


<CLSCompliant(True)>
Public Class SegmentIntersectInfo

	Public Action As SegmentIntersectResults

	''' <summary>
	''' Only makes sense when <see cref="Action"/> is
	''' <see cref="SegmentIntersectResults.Intersects"/>.
	''' </summary>
	Public Position As Double


	<ExcludeFromCodeCoverage>
	Public Overrides Function ToString() As String
		Return String.Format("{0} at {1}", Action.ToString(), Position)
	End Function

End Class
