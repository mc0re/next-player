Imports System.IO
Imports AudioChannelLibrary
Imports NAudio.Wave


Public Class TestInputStreamProvider
	Implements IInputStreamProvider

#Region " Fields "

	Private ReadOnly mChannels As Integer


	Private ReadOnly mSampleDef As String

#End Region


#Region " Init and clean-up "

	''' <summary>
	''' Throw an expcetion when a stream is requested.
	''' </summary>
	Public Sub New()
		' Do nothing
	End Sub


	''' <summary>
	''' Return a new stream with the given characteristics.
	''' </summary>
	Public Sub New(channels As Integer, sampleDef As String)
		mChannels = channels
		mSampleDef = sampleDef
	End Sub

#End Region


#Region " API "

	Public Function CreateStream(fileName As String) As WaveStream Implements IInputStreamProvider.CreateStream
		If mChannels = 0 Then
			Throw New FileNotFoundException()
		End If

		Return New TestInputStream(mChannels, mSampleDef)
	End Function

#End Region

End Class
