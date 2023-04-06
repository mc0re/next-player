Imports System.Collections.Concurrent
Imports AudioChannelLibrary
Imports Common


''' <summary>
''' Collects all written data for each channel.
''' </summary>
Public Class TestAudioWriter
    Implements ITestDataWriter

#Region " Fields "

    Private ReadOnly mBuffers As New ConcurrentDictionary(Of Integer, TestDataBuffer)()

#End Region


#Region " Properties "

    Public ReadOnly Property Channels As IReadOnlyCollection(Of Integer)
        Get
            Return mBuffers.Keys.OrderBy(Function(a) a).ToList()
        End Get
    End Property


    Public ReadOnly Property DataBuffer(channel As Integer) As TestDataBuffer
        Get
            Dim buf As TestDataBuffer = Nothing

            mBuffers.TryGetValue(channel, buf)

            Return buf
        End Get
    End Property

#End Region


#Region " ITestDataWriter implementation "

    Public Sub Write(ph As IChannel, buffer() As Single, offset As Integer, requestedCount As Integer, producedCount As Integer) Implements ITestDataWriter.Write
        Dim buf = mBuffers.GetOrAdd(ph.Channel, New TestDataBuffer())

        If producedCount = 0 Then Return

        buf.Write(buffer.Skip(offset).Take(producedCount))

        ' Zero out the actual data to not produce sounds during the test
        Array.Clear(buffer, offset, producedCount)
    End Sub

#End Region

End Class
