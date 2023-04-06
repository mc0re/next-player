Imports System.Configuration


Public Class EnvironmentConfigurationSection
	Inherits ConfigurationSection

#Region " Constants "

	Private Const EnvSectionName = "EnvironmentSection"

#End Region


#Region " Section property "

	Public Shared ReadOnly Property EnvSection As EnvironmentConfigurationSection
		Get
			Return EnsureSection(Of EnvironmentConfigurationSection)(OpenConfig(), EnvSectionName)
		End Get
	End Property

#End Region


#Region " Environments property "

	<ConfigurationProperty(NameOf(Environments), IsDefaultCollection:=False)>
	Public ReadOnly Property Environments As EnvironmentConfigurationCollection
		Get
			Dim coll = TryCast(Item(NameOf(Environments)), EnvironmentConfigurationCollection)

			If coll.Count = 0 Then
				coll.Add(New EnvironmentConfigurationElement())
			End If

			Return coll
		End Get
	End Property

#End Region


#Region " IsReadOnly override "

	Public Overrides Function IsReadOnly() As Boolean
		Return False
	End Function

#End Region


#Region " Reading "

	Private Shared Function OpenConfig() As Configuration
		Return System.Configuration.ConfigurationManager.OpenExeConfiguration(
			ConfigurationUserLevel.PerUserRoamingAndLocal)
	End Function


	Private Shared Function EnsureSection(Of T As ConfigurationSection)(
		config As Configuration, secName As String
	) As T
		Dim section = TryCast(config.GetSection(secName), T)

		config.Save(ConfigurationSaveMode.Modified)

		Return section
	End Function

#End Region


#Region " Saving "

	Public Sub Save()
		' To write to
		Dim config = OpenConfig()
        Dim section = EnsureSection(Of EnvironmentConfigurationSection)(config, EnvSectionName)

        For Each prop In section.Properties.Cast(Of ConfigurationProperty)()
            Dim name = prop.Name
            section.SetPropertyValue(section.Properties(name), Item(name), False)
        Next

		config.Save(ConfigurationSaveMode.Modified)
		System.Configuration.ConfigurationManager.RefreshSection(EnvSectionName)
	End Sub

#End Region

End Class
