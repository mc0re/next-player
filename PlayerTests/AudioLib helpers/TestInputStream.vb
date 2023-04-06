Imports NAudio.Wave


''' <summary>
''' Creates a sample stream with the givne samples
''' and provides this data to the audio player.
''' </summary>
Public Class TestInputStream
	Inherits WaveStream

#Region " Fields "

	Private ReadOnly mFormat As WaveFormat


	Private ReadOnly mSamples As IList(Of Byte)


	Private mPosition As Long

#End Region


#Region " Init and clean-up "

	''' <summary>
	''' Create a predefined sample set.
	''' </summary>
	''' <remarks>
	''' The format of the definition is:
	'''   { sample { "," sample } { "x" count } ";" }
	'''   
	''' Example:
	'''   0,1,1,0 x 100; 0.5 x 2 x 150; 0
	'''   
	''' Produces 100 quatruples of numbers (0.0, 1.0, 1.0, 0.0),
	''' then 300 times number 0.5,
	''' then 0.0.
	''' </remarks>
	Public Sub New(channels As Integer, sampleDef As String)
		Dim buf As New TestDataBuffer(sampleDef)
		mSamples = ToBytes(buf.Data)
		mFormat = New WaveFormat(44100, 32, channels)
	End Sub

#End Region


#Region " Parsing utility "

	Private Shared Function ToBytes(singleList As IEnumerable(Of Single)) As IList(Of Byte)
		Dim res As New List(Of Byte)()

		For Each s In singleList
			res.AddRange(ToByteArray(s))
		Next

		Return res
	End Function


	''' <summary>
	''' This works the same way as NAudio algorithm.
	''' </summary>
	''' <remarks>
	''' Probably, BitConverter.GetBytes() would be a more correct way,
	''' but this is not how NAudio works, at least, now.
	''' </remarks>
	Private Shared Function ToByteArray(s As Single) As IEnumerable(Of Byte)
		Dim bAsInt = CLng(CDbl(s) * 2147483647)
		Dim res(3) As Byte

		res(3) = CByte(bAsInt \ &H1000000 And &HFF)
		res(2) = CByte(bAsInt \ &H10000 And &HFF)
		res(1) = CByte(bAsInt \ &H100 And &HFF)
		res(0) = CByte(bAsInt And &HFF)

		Return res
	End Function

#End Region


#Region " WaveStream implementation "

	Public Overrides ReadOnly Property Length As Long
		Get
			Return mSamples.Count
		End Get
	End Property


	Public Overrides Property Position As Long
		Get
			Return mPosition
		End Get
		Set(value As Long)
			mPosition = value
		End Set
	End Property


	Public Overrides ReadOnly Property WaveFormat As WaveFormat
		Get
			Return mFormat
		End Get
	End Property


	Public Overrides Function Read(buffer() As Byte, offset As Integer, count As Integer) As Integer
		Dim cnt = Math.Min(count, CInt(mSamples.Count - mPosition))

		For idx = 0 To cnt - 1
			buffer(offset + idx) = mSamples(CInt(mPosition) + idx)
		Next

		mPosition += cnt
		Return cnt
	End Function

#End Region

End Class
