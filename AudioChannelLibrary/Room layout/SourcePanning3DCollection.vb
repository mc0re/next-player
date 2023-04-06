Imports Common


Public Class SourcePanning3DCollection

    Private mCollection As New Dictionary(Of Integer, SourcePanningInfo(Of IPoint3D))()


    ''' <summary>
    ''' Add information for the given channel (in <paramref name="info"/>.
    ''' </summary>
    Public Sub Add(info As SourcePanningInfo(Of IPoint3D))
        mCollection.Add(info.PhysicalNr, info)
    End Sub


    ''' <summary>
    ''' Get panning info for the given physical channel.
    ''' </summary>
    ''' <param name="channelNr"></param>
    ''' <returns></returns>
    Public Function GetInfo(channelNr As Integer) As SourcePanningInfo(Of IPoint3D)
        Dim res As SourcePanningInfo(Of IPoint3D) = Nothing
        If mCollection.TryGetValue(channelNr, res) Then
            Return res
        End If

        Return New SourcePanningInfo(Of IPoint3D)()
    End Function

End Class
