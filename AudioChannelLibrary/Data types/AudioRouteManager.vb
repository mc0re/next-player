Imports NAudio.Wave


''' <summary>
''' For each channel, a list of outputs can be stored.
''' </summary>
Public NotInheritable Class AudioRouteManager

#Region " "

#End Region


#Region " Singleton "

	Private Shared mInstance As New AudioRouteManager()


	Public Shared ReadOnly Property Instance As AudioRouteManager
		Get
			Return mInstance
		End Get
	End Property


	Private Sub New()
		' Do nothing
	End Sub

#End Region


#Region " API "

	Public Function GetDeviceInfoList(channelNr As Integer) As DirectSoundDeviceInfo
		Return DirectSoundOut.Devices.Skip(channelNr).FirstOrDefault()
	End Function

#End Region

End Class
