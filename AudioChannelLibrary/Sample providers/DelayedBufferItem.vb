Imports System.Text


''' <summary>
''' A single buffer storage.
''' </summary>
Public Class DelayedBufferItem

	''' <summary>
	''' Whether the buffer is ready for reuse.
	''' </summary>
	Public IsFree As Boolean


	''' <summary>
	''' The number of samples in the buffer (if not free).
	''' </summary>
	Public DataLength As Integer


	''' <summary>
	''' Actual buffer.
	''' </summary>
	Public Buffer() As Single


	Public Overrides Function ToString() As String
		Dim str As New StringBuilder()

		If IsFree Then str.Append("Free ")
		If Buffer.Length > 0 Then str.AppendFormat("{0}...", Buffer(0))

		Return str.ToString()
	End Function

End Class
