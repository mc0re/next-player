Imports System.ComponentModel


<Serializable>
Public Class PlaylistEnvironmentConfigurationCollection
    Inherits BindingList(Of PlaylistEnvironmentConfiguration)

    Public Function Clone() As PlaylistEnvironmentConfigurationCollection
        Dim res As New PlaylistEnvironmentConfigurationCollection

        For Each env In Me
            res.Add(env.Clone())
        Next

        Return res
    End Function

End Class
