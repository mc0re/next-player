''' <summary>
''' A buffer storage, which allows delaying samples.
''' </summary>
Public Class DelayedBuffer

#Region " Fields "

	''' <summary>
	''' A list of buffers.
	''' </summary>
	''' <remarks>
	''' A new buffer is inserted in the beginning of the list.
	''' This simplifies length checking.
	''' </remarks>
	Private ReadOnly mBufferList As New List(Of DelayedBufferItem)()


	''' <summary>
	''' Last set delay.
	''' </summary>
	Private mDelay As Integer


	''' <summary>
	''' Cache - last returned buffer.
	''' </summary>
	Private mLastBuffer As DelayedBufferItem

#End Region


#Region " Properties "

	''' <summary>
	''' The maximum number of samples to store in the buffer.
	''' If zero, the same buffer is always reused.
	''' </summary>
	Public Property MaxDelay As Integer

#End Region


#Region " Utility "

	''' <summary>
	''' Check buffers and mark free one(s).
	''' </summary>
	Private Sub CheckFreeBuffers()
		Dim sumLength = 0

		For Each buf In mBufferList
			If buf.DataLength = 0 OrElse sumLength >= MaxDelay Then
				buf.IsFree = True
			Else
				sumLength += buf.DataLength
			End If
		Next
	End Sub


	''' <summary>
	''' Find a position from the current buffer.
	''' </summary>
	''' <param name="pos">Desired position, can be positive and negative</param>
	''' <param name="resBuf">Output: buffer to read from</param>
	''' <param name="resIdx">Output: Index in that buffer</param>
	Private Sub FindReadPosition(pos As Integer, ByRef resBuf As DelayedBufferItem, ByRef resIdx As Integer)
		If pos >= 0 Then
			resBuf = mLastBuffer
			resIdx = pos
		Else
			Dim startPos = 0

			For Each buf In mBufferList.Skip(1)
				startPos -= buf.DataLength
				If startPos <= pos Then
					resBuf = buf
					resIdx = pos - startPos
					Exit For
				End If
			Next
		End If
	End Sub

#End Region


#Region " API "

	''' <summary>
	''' Provide a continuous buffer for reading into.
	''' </summary>
	''' <remarks>
	''' This buffer is marked as the latest data, and consequent set and
	''' reading operations have its start as a pivot point.
	''' 
	''' If too many buffers (more than MaxDelay) are collected,
	''' they begin to be reused.
	''' </remarks>
	''' <param name="length">Needed buffer length (in elements)</param>
	Public Function ProvideBuffer(length As Integer) As Single()
		' Check existing buffers
		CheckFreeBuffers()
		Dim buf = (From b In mBufferList Where b.IsFree).FirstOrDefault()

		If buf Is Nothing Then
			' No buffer - create a new one
			buf = New DelayedBufferItem()
		Else
			' Found a free buffer - reuse
			mBufferList.Remove(buf)
			buf.IsFree = False
		End If

		mBufferList.Insert(0, buf)

		If buf.Buffer Is Nothing OrElse buf.Buffer.Length < length Then
			buf.Buffer = New Single(length - 1) {}
		End If

		mLastBuffer = buf

		Return buf.Buffer
	End Function


	''' <summary>
	''' Set the number of samples written in the last buffer.
	''' </summary>
	Public Sub SetSamplesWritten(samples As Integer)
		Debug.Assert(samples <= mLastBuffer.Buffer.Length)

		mLastBuffer.DataLength = samples
	End Sub


	''' <summary>
	''' Set delay for consequent Read operations.
	''' </summary>
	''' <param name="samples">The number of samples to delay reading by</param>
	Public Sub SetReadDelay(samples As Integer)
		mDelay = samples
	End Sub


	''' <summary>
	''' Read a sample (considering delay).
	''' </summary>
	''' <remarks>
	''' When a last sample in a buffer has been read,
	''' the buffer may be is marked as free for reuse.
	''' </remarks>
	Public Function Read(offset As Integer) As Single
		Dim buf As DelayedBufferItem = Nothing
		Dim idx As Integer

		FindReadPosition(-mDelay + offset, buf, idx)

		Return If(buf Is Nothing OrElse idx >= buf.DataLength, 0, buf.Buffer(idx))
	End Function

#End Region

End Class
