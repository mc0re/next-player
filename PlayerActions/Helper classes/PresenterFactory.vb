Imports Common


''' <summary>
''' Factory class for creating presenter instances.
''' </summary>
Public Class PresenterFactory

#Region " Events "

	Public Shared Event PresenterChanged(presenterIndex As Integer)

#End Region


#Region " Factory "

	Private Shared mPptDict As New Dictionary(Of Integer, PowerPointReference)


	''' <summary>
	''' Factory method, ensuring one reference per presentation file.
	''' </summary>
	''' <returns>Nothing if the presenter is not configured, an object otherwise</returns>
	Public Shared Function GetReference(presIndex As Integer) As IPresenterReference
		Dim res As PowerPointReference = Nothing

		If Not mPptDict.TryGetValue(presIndex, res) Then
			res = New PowerPointReference(presIndex)
			mPptDict.Add(presIndex, res)
			AddHandler res.PresenterChanged, Sub(index) RaiseEvent PresenterChanged(index)
		End If

		If res.IsInitialized Then Return res

		Dim presConfFact = InterfaceMapper.GetImplementation(Of IPresenterConfiguration)()
		If presConfFact Is Nothing Then Return res
		Dim presConf = presConfFact.GetPresenter(presIndex)
		If presConf Is Nothing Then Return res

		res.PresentationFileName = presConf.FilePath

		If Not res.GetReferences() Then Return res
		Return res
	End Function

#End Region

End Class
