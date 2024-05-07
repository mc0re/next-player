Imports PlayerActions


Public Class ConfigurationManager

	Public Shared Event ConfigurationChanged()


	''' <summary>
	''' Delete all configurations with the given name.
	''' </summary>
	''' <returns>True if anything was deleted</returns>
	Public Shared Function DeleteAll(configName As String) As Boolean
		Dim deleted = False

		Dim localList = AppConfiguration.Instance.EnvironmentSettingsList
		For Each localEnv In localList.ToList()
			If localEnv.Name = configName Then
				localList.Remove(localEnv)
				deleted = True
			End If
		Next

		Dim playlistList = AppConfiguration.Instance.CurrentActionCollectionTyped.EnvironmentList
		For Each plEnv In playlistList.ToList()
			If plEnv.Name = configName Then
				playlistList.Remove(plEnv)
				deleted = True
			End If
		Next

		If deleted Then
			RaiseEvent ConfigurationChanged()
		End If

		Return deleted
	End Function


	''' <summary>
	''' Rename all configurations.
	''' </summary>
	Public Shared Sub Rename(oldName As String, newName As String)
		If newName = oldName Then Return

		For Each localEnv In AppConfiguration.Instance.EnvironmentSettingsList
			If localEnv.Name = oldName Then
				localEnv.Name = newName
			End If
		Next

		For Each plEnv In AppConfiguration.Instance.CurrentActionCollectionTyped.EnvironmentList
			If plEnv.Name = oldName Then
				plEnv.Name = newName
			End If
		Next

		RaiseEvent ConfigurationChanged()

		If oldName = AppConfiguration.Instance.EnvironmentName Then
			AppConfiguration.Instance.EnvironmentName = newName
		End If
	End Sub


	''' <summary>
	''' Get playlist configuration with the given name for the given computer.
	''' </summary>
	Public Shared Function GetPlaylistEnv(envName As String, machineName As String) As PlaylistEnvironmentConfiguration
		Dim env = AppConfiguration.Instance.CurrentActionCollectionTyped.EnvironmentList.
			FirstOrDefault(Function(c) c.MachineName = machineName AndAlso c.Name = envName)

		Return env
	End Function


	''' <summary>
	''' Add a configuration with the given name as a copy of the current one.
	''' If the named configuration already exists, nothing happens.
	''' </summary>
	Public Shared Sub CreateLocal(envName As String)
		If AppConfiguration.Instance.EnvironmentSettingsList.Any(Function(e) e.Name = envName) Then Return

		AppConfiguration.Instance.CloneCurrentEnvironment(envName)
		RaiseEvent ConfigurationChanged()
	End Sub


	''' <summary>
	''' Delete a local configuration with the given name.
	''' </summary>
	Public Shared Sub DeleteLocal(envName As String)
		Dim list = AppConfiguration.Instance.EnvironmentSettingsList
		Dim toDelete = list.Where(Function(c) c.Name = envName).ToList()

		If toDelete.Any() Then
			For Each d In toDelete
				list.Remove(d)
			Next

			RaiseEvent ConfigurationChanged()
		End If
	End Sub


	''' <summary>
	''' Add a playlist configuration with the given name for the given computer
	''' as a copy of an existing one.
	''' </summary>
	Public Shared Sub CreatePlaylist(envName As String, machineName As String)
		Dim coll = AppConfiguration.Instance.CurrentActionCollectionTyped.EnvironmentList

		Dim existingForEnv = coll.FirstOrDefault(Function(c) c.Name = envName)
		Dim existingForMachine = If(coll.FirstOrDefault(Function(c) c.MachineName = machineName), New PlaylistEnvironmentConfiguration())

		Dim newEnv = If(existingForEnv IsNot Nothing,
			existingForEnv.Clone(),
			New PlaylistEnvironmentConfiguration() With {
				.MachineId = existingForMachine.MachineId,
				.MachineName = existingForMachine.MachineName
			})

		coll.Add(newEnv)
		RaiseEvent ConfigurationChanged()
	End Sub


	''' <summary>
	''' Delete a playlist configuration with the given name for the given computer.
	''' </summary>
	Public Shared Sub DeletePlaylist(envName As String, machineName As String)
		Dim coll = AppConfiguration.Instance.CurrentActionCollectionTyped.EnvironmentList

		Dim existing = coll.
			FirstOrDefault(Function(c) c.Name = envName AndAlso c.MachineName = machineName)

		If existing IsNot Nothing Then
			coll.Remove(existing)
			RaiseEvent ConfigurationChanged()
		End If
	End Sub

End Class
