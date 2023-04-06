Imports AudioChannelLibrary


<TestClass>
Public Class DelayedBufferTest

	<TestMethod, TestCategory("DelayedBuffer")>
	Public Sub DelayedBuffer_MaxZero_ReuseBuffers()
		Dim buf As New DelayedBuffer() With {
			.MaxDelay = 0
		}

		Dim b1 = buf.ProvideBuffer(10)
		buf.SetSamplesWritten(10)
		Dim b2 = buf.ProvideBuffer(10)

		Assert.AreSame(b2, b1)
	End Sub


	<TestMethod, TestCategory("DelayedBuffer")>
	Public Sub DelayedBuffer_NoData_ReuseBuffers()
		Dim buf As New DelayedBuffer() With {
			.MaxDelay = 10
		}

		Dim b1 = buf.ProvideBuffer(10)
		Dim b2 = buf.ProvideBuffer(10)

		Assert.AreSame(b2, b1)
	End Sub


	<TestMethod, TestCategory("DelayedBuffer")>
	Public Sub DelayedBuffer_Max10_ReuseBuffers()
		' The buffer needs to keep at least 10 samples.
		' Therefore b1 is still needed when b2 is provided.
		' But it is not needed any more as soon as b2 has 10 samples.
		Dim buf As New DelayedBuffer() With {
			.MaxDelay = 10
		}

		Dim b1 = buf.ProvideBuffer(10)
		buf.SetSamplesWritten(10)
		Dim b2 = buf.ProvideBuffer(10)
		buf.SetSamplesWritten(10)
		Dim b3 = buf.ProvideBuffer(10)

		Assert.AreNotSame(b2, b1)
		Assert.AreNotSame(b3, b2)
		Assert.AreSame(b3, b1)
	End Sub


	<TestMethod, TestCategory("DelayedBuffer")>
	Public Sub DelayedBuffer_Max11_ReuseBuffers()
		' The buffer needs to keep at least 10 samples.
		' Therefore b1 is still needed when b2 is provided,
		' and when b3 is provided (as b2 only has 10 samples, 1 sample
		' from b1 is still needed).
		Dim buf As New DelayedBuffer() With {
			.MaxDelay = 11
		}

		Dim b1 = buf.ProvideBuffer(10)
		buf.SetSamplesWritten(10)
		Dim b2 = buf.ProvideBuffer(10)
		buf.SetSamplesWritten(10)
		Dim b3 = buf.ProvideBuffer(10)

		Assert.AreNotSame(b2, b1)
		Assert.AreNotSame(b3, b2)
		Assert.AreNotSame(b3, b1)
	End Sub


	<TestMethod, TestCategory("DelayedBuffer")>
	Public Sub DelayedBuffer_NoDataRead_Ok()
		Dim buf As New DelayedBuffer()

		Dim s = buf.Read(0)
		Assert.AreEqual(0F, s)
	End Sub


	<TestMethod, TestCategory("DelayedBuffer")>
	Public Sub DelayedBuffer_NoDelayRead_Ok()
		Dim buf As New DelayedBuffer() With {
			.MaxDelay = 10
		}

		Dim b1 = buf.ProvideBuffer(10)
		For i = 0 To 9
			b1(i) = i + 1
		Next
		buf.SetSamplesWritten(10)

		For i = 0 To 9
			Dim s = buf.Read(i)
			Assert.AreEqual(CSng(i + 1), s)
		Next

		' Reading beyond the buffer
		Dim beyond = buf.Read(10)
		Assert.AreEqual(0F, beyond)
	End Sub


	<TestMethod, TestCategory("DelayedBuffer")>
	Public Sub DelayedBuffer_NoDelayReadLast_Ok()
		Dim buf As New DelayedBuffer() With {
			.MaxDelay = 10
		}

		Dim b1 = buf.ProvideBuffer(10)
		For i = 0 To 9
			b1(i) = i + 1
		Next
		buf.SetSamplesWritten(10)

		' b1 is now non-actual for reading
		Dim b2 = buf.ProvideBuffer(10)
		For i = 0 To 9
			b2(i) = i + 11
		Next
		buf.SetSamplesWritten(10)

		For i = 0 To 9
			Dim s = buf.Read(i)
			Assert.AreEqual(CSng(i + 11), s)
		Next

		' Reading beyond the buffer
		Dim beyond = buf.Read(10)
		Assert.AreEqual(0F, beyond)
	End Sub


	<TestMethod, TestCategory("DelayedBuffer")>
	Public Sub DelayedBuffer_Delay10Read_Ok()
		Dim buf As New DelayedBuffer() With {
			.MaxDelay = 10
		}
		buf.SetReadDelay(10)

		Dim b1 = buf.ProvideBuffer(10)
		For i = 0 To 9
			b1(i) = i + 1
		Next
		buf.SetSamplesWritten(10)

		Dim b2 = buf.ProvideBuffer(10)
		For i = 0 To 9
			b2(i) = i + 11
		Next
		buf.SetSamplesWritten(10)

		For i = 0 To 19
			Dim s = buf.Read(i)
			Assert.AreEqual(CSng(i + 1), s)
		Next

		' Reading beyond the buffer
		Dim beyond = buf.Read(20)
		Assert.AreEqual(0F, beyond)
	End Sub


	<TestMethod, TestCategory("DelayedBuffer")>
	Public Sub DelayedBuffer_Delay1000Read_Ok()
		Dim buf As New DelayedBuffer() With {
			.MaxDelay = 1000
		}

		' Create 100 of 10-sample buffers
		For bufIdx = 0 To 99
			Dim b1 = buf.ProvideBuffer(12)
			For i = 0 To 9
				b1(i) = i + bufIdx * 10 + 1
			Next
			buf.SetSamplesWritten(10)
		Next

		' Start reading from 800 samples before the last buffer
		Dim b = buf.ProvideBuffer(10)
		buf.SetReadDelay(800)

		For i = 0 To 799
			Dim s = buf.Read(i)
			Assert.AreEqual(CSng(i + 201), s)
		Next
	End Sub


	<TestMethod, TestCategory("DelayedBuffer")>
	Public Sub DelayedBuffer_DelayedPlayback_Ok()
		Const BufferSize = 10
		Const ActualDelay = 882

		Dim buf As New DelayedBuffer() With {
			.MaxDelay = 1000
		}
		buf.SetReadDelay(ActualDelay)

		Dim writeValue = 1.0F
		Dim readIndex = 0
		Dim readValue = 0F

		While readValue < 1000
			Dim b = buf.ProvideBuffer(BufferSize)

			For i = 0 To BufferSize - 1
				b(i) = writeValue
				writeValue += 1
			Next

			buf.SetSamplesWritten(BufferSize)

			For i = 0 To BufferSize - 1
				Dim s = buf.Read(i)
				Assert.AreEqual(readValue, s)
				readIndex += 1
				If readIndex >= ActualDelay Then
					readValue += 1
				End If
			Next
		End While
	End Sub

End Class
