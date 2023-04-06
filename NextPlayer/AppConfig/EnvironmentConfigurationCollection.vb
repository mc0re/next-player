Imports System.Configuration


<ConfigurationCollection(GetType(EnvironmentConfigurationElement), AddItemName:="Environment")>
Public Class EnvironmentConfigurationCollection
	Inherits ConfigurationElementCollection

#Region " Overrides "

	Protected Overrides Function CreateNewElement() As ConfigurationElement
		Return New EnvironmentConfigurationElement()
	End Function


	Protected Overrides Function GetElementKey(element As ConfigurationElement) As Object
		Return CType(element, EnvironmentConfigurationElement).Name
	End Function

#End Region


#Region " Collection accessors "

	Default Public Shadows Property Item(index As Integer) As EnvironmentConfigurationElement
		Get
			Return CType(BaseGet(index), EnvironmentConfigurationElement)
		End Get
		Set(value As EnvironmentConfigurationElement)
			If BaseGet(index) IsNot Nothing Then
				BaseRemoveAt(index)
			End If
			BaseAdd(index, value)
		End Set
	End Property


	Public Sub Add(elem As EnvironmentConfigurationElement)
		BaseAdd(elem)
	End Sub


	Public Sub Clear()
		BaseClear()
	End Sub


	Public Sub Remove(elem As EnvironmentConfigurationElement)
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

	Public Shadows Iterator Function GetEnumerator() As IEnumerator(Of EnvironmentConfigurationElement)
        Dim cnt = Count

        For i = 0 To cnt - 1
			Yield CType(BaseGet(i), EnvironmentConfigurationElement)
		Next
	End Function

#End Region

End Class
