Imports System.Collections.Concurrent
Imports System.IO
Imports Common


''' <summary>
''' Get file duration.
''' </summary>
Public NotInheritable Class DurationLibrary

#Region " Queue handling "

    Private Shared ReadOnly sInfoList As New ConcurrentQueue(Of DurationLibraryItemBase)


    ''' <summary>
    ''' A component for determining the duration; reused.
    ''' </summary>
    Private Shared WithEvents sPlayer As IDurationPlayer


    Private Shared sCurrentInfo As DurationLibraryItemBase


    ''' <summary>
    ''' Process next in queue request.
    ''' </summary>
    Private Shared Sub ProcessNext()
        Dim info As DurationLibraryItemBase = Nothing

        If sInfoList.TryDequeue(info) Then
            ProcessDuration(info)
        Else
            InterfaceMapper.GetImplementation(Of IMessageLog)().LogDurationWork(False)
        End If
    End Sub


    ''' <summary>
    ''' Process the given duration request.
    ''' </summary>
    Private Shared Sub ProcessDuration(infoItem As DurationLibraryItemBase)
        Debug.Assert(sCurrentInfo Is Nothing)
        sCurrentInfo = infoItem

        Try
            Dim fname = infoItem.AsInputFile.FileName

            If String.IsNullOrEmpty(fname) OrElse Not File.Exists(fname) Then
                GetDurationFailed()
            Else
                sPlayer.Open(fname, 0)
            End If

        Catch ex As Exception
            GetDurationFailed()
        End Try
    End Sub


#End Region


#Region " Utility "

    Private Shared Sub GetDurationFailed()
        sPlayer.Close()

        ' Paranoic check, might trigger in the debugging session
        If sCurrentInfo IsNot Nothing Then
            sCurrentInfo.AsInputFile.IsLoadingFailed = True
            sCurrentInfo.AsDuration.HasDuration = False

            InterfaceMapper.GetImplementation(Of IMessageLog)().LogFileError(
                "Cannot open as audio file '{0}'", sCurrentInfo.AsInputFile.FileName)
            sCurrentInfo = Nothing
        End If

        ProcessNext()
    End Sub

#End Region


#Region " API "

    ''' <summary>
    ''' Get media file duration.
    ''' </summary>
    Public Shared Sub GetDuration(Of T As {IInputFile, IDurationElement})(info As T)
        sPlayer = InterfaceMapper.GetImplementation(Of IDurationPlayer)()

        Dim item = New DurationLibraryItem(Of T)(info)
        sInfoList.Enqueue(item)

        If sCurrentInfo Is Nothing Then
            InterfaceMapper.GetImplementation(Of IMessageLog)().LogDurationWork(True)
            ProcessNext()
        End If
    End Sub


    ''' <summary>
    ''' The aplication is about to close.
    ''' </summary>
    Public Shared Sub Shutdown()
        If sPlayer Is Nothing Then Return

        sPlayer.Dispose()
    End Sub


    ''' <summary>
    ''' Success.
    ''' </summary>
    Private Shared Sub OnMediaOpened(sender As Object, args As EventArgs) Handles sPlayer.MediaOpened
        Dim res = sPlayer.NaturalDuration
        sPlayer.Close()

        ' Paranoic check, might trigger in the debugging session
        If sCurrentInfo IsNot Nothing Then
            sCurrentInfo.AsInputFile.IsLoadingFailed = False
            sCurrentInfo.AsDuration.HasDuration = res.HasTimeSpan
            sCurrentInfo.AsDuration.Duration = If(res.HasTimeSpan, res.TimeSpan, Nothing)

            sCurrentInfo = Nothing
        End If

        ProcessNext()
    End Sub


    ''' <summary>
    ''' Failure.
    ''' </summary>
    Private Shared Sub OnMediaFailed(sender As Object, args As EventArgs) Handles sPlayer.MediaFailed
        GetDurationFailed()
    End Sub

#End Region

End Class
