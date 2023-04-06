Imports System.IO
Imports System.Collections.Concurrent
Imports NAudio.Wave
Imports Common
Imports AudioChannelLibrary


''' <summary>
''' Keeps requested files in memory, so the access is faster
''' and does not involve hard disk operations.
''' </summary>
Public Class FileCache
	Implements IInputStreamProvider

#Region " Helper class DataCache "

    ''' <summary>
    ''' Actual storage and trivial API.
    ''' </summary>
    Private NotInheritable Class DataCache

        Private ReadOnly mDataCollection As New ConcurrentDictionary(Of String, Byte())


        Private mDataSize As Integer


        Private Function CreateEntry(filename As String) As Byte()
            Dim bytes = File.ReadAllBytes(filename)

            mDataSize = mDataCollection.Select(Function(m) m.Value.Length).Sum() + bytes.Length
            InterfaceMapper.GetImplementation(Of IMessageLog)().LogFileCacheInfo(mDataSize)

            Return bytes
        End Function


        Public Function GetStreamInternal(filename As String) As Stream
            Dim content = mDataCollection.GetOrAdd(filename, AddressOf CreateEntry)
            Return New MemoryStream(content, False)
        End Function


        Public Sub EraseEntry(filename As String)
            If String.IsNullOrWhiteSpace(filename) Then Return

            mDataCollection.TryRemove(filename, Nothing)

            mDataSize = mDataCollection.Select(Function(m) m.Value.Length).Sum()
            InterfaceMapper.GetImplementation(Of IMessageLog)().LogFileCacheInfo(mDataSize)
        End Sub


        Public Sub Clear()
            mDataCollection.Clear()

            mDataSize = 0
            InterfaceMapper.GetImplementation(Of IMessageLog)().LogFileCacheInfo(mDataSize)
        End Sub

    End Class

#End Region


#Region " Fields "

    Private ReadOnly mDataCache As New DataCache()

#End Region


#Region " Singleton "

    ''' <summary>
    ''' The one and only instance.
    ''' </summary>
    Public Shared ReadOnly Property Instance As FileCache = New FileCache()


    Private Sub New()
        ' Do nothing
    End Sub

#End Region


#Region " Utility methods "

    Private Function CreateReaderStream(fileName As String) As WaveStream
        Dim res As WaveStream
        Dim strm = mDataCache.GetStreamInternal(fileName)
		Dim ext = Path.GetExtension(fileName).ToLowerInvariant()

		Select Case ext
			Case ".wav"
				res = New WaveFileReader(strm)
				If res.WaveFormat.Encoding <> WaveFormatEncoding.Pcm AndAlso res.WaveFormat.Encoding <> WaveFormatEncoding.IeeeFloat Then
					res = WaveFormatConversionStream.CreatePcmStream(res)
					res = New BlockAlignReductionStream(res)
				End If

			Case ".mp3"
				res = New Mp3FileReader(strm)

			Case ".aiff"
				res = New AiffFileReader(strm)

				'Case ".wma"
				'	res = New NAudio.WindowsMediaFormat.WMAFileReader(strm)

			Case Else
				EraseEntry(fileName)
                Throw New ArgumentException($"Unknown file extension for '{fileName}'")
        End Select

		Return res
	End Function

#End Region


#Region " Public API "

	Public Function CreateStream(fileName As String) As WaveStream Implements IInputStreamProvider.CreateStream
		Return CreateReaderStream(fileName)
	End Function


	Public Sub EraseEntry(filename As String)
		mDataCache.EraseEntry(filename)
	End Sub


	Public Sub Clear()
		mDataCache.Clear()
	End Sub

#End Region

End Class
