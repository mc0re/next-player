Imports System.Configuration
Imports Common


<ConfigurationCollection(GetType(VoiceCommandConfigurationElement), AddItemName:="VoiceCommand")>
Public Class VoiceCommandConfigurationCollection
	Inherits ConfigurationElementCollection

#Region " Overrides "

	Protected Overrides Function CreateNewElement() As ConfigurationElement
		Return New VoiceCommandConfigurationElement()
	End Function


	Protected Overrides Function GetElementKey(element As ConfigurationElement) As Object
		Return CType(element, VoiceCommandConfigurationElement).Name
	End Function

#End Region


#Region " Collection accessors "

	Default Public Shadows Property Item(index As Integer) As VoiceCommandConfigurationElement
		Get
			Return CType(BaseGet(index), VoiceCommandConfigurationElement)
		End Get
		Set(value As VoiceCommandConfigurationElement)
			If BaseGet(index) IsNot Nothing Then
				BaseRemoveAt(index)
			End If
			BaseAdd(index, value)
		End Set
	End Property


	Public Sub Add(elem As VoiceCommandConfigurationElement)
		BaseAdd(elem)
	End Sub


	Public Sub Clear()
		BaseClear()
	End Sub


	Public Sub Remove(elem As VoiceCommandConfigurationElement)
		BaseRemove(elem)
	End Sub


	Public Sub RemoveAt(index As Integer)
		BaseRemoveAt(index)
	End Sub


	Public Sub Remove(name As String)
		BaseRemove(name)
	End Sub

#End Region


#Region " Enumerator "

	Public Shadows Iterator Function GetEnumerator() As IEnumerator(Of VoiceCommandConfigurationElement)
		Dim cnt = Count

		For i = 0 To cnt - 1
			Yield CType(BaseGet(i), VoiceCommandConfigurationElement)
		Next
	End Function

#End Region


#Region " Model conversion "

	Public Shared Sub ToModel(model As VoiceCommandConfigItemCollection, settings As VoiceCommandConfigurationCollection)
		model.Clear()

		If settings Is Nothing Then Return

		For Each elem In settings
			model.Add(elem.ToModel())
		Next
	End Sub


	Public Shared Function FromModel(model As VoiceCommandConfigItemCollection) As VoiceCommandConfigurationCollection
		Dim sett As New VoiceCommandConfigurationCollection()

		For Each modelItem In model
			sett.Add(VoiceCommandConfigurationElement.FromModel(modelItem))
		Next

		Return sett
	End Function

#End Region

End Class
