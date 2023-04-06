Imports NAudio.CoreAudioApi
Imports NAudio.CoreAudioApi.Interfaces


Public NotInheritable Class AudioChangedListener
    Implements IMMNotificationClient, IDisposable

#Region " Events "

    <CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")>
    Public Event DevicesUpdated(sender As Object, args As EventArgs)


    Private Sub RaiseDevicesUpdated()
        RaiseEvent DevicesUpdated(Me, EventArgs.Empty)
    End Sub

#End Region


#Region " Listening "

    Private mDeviceEnum As New MMDeviceEnumerator()

#End Region


#Region " Singleton "

    Public Shared ReadOnly Property Instance As AudioChangedListener = New AudioChangedListener()


    Private Sub New()
        If Environment.OSVersion.Version.Major < 6 Then
            Throw New NotSupportedException("This functionality is only supported on Windows Vista or newer.")
        End If

        mDeviceEnum.RegisterEndpointNotificationCallback(Me)
    End Sub

#End Region


#Region " IMMNotificationClient implementation "

    Public Sub OnDeviceAdded(pwstrDeviceId As String) Implements IMMNotificationClient.OnDeviceAdded
        RaiseDevicesUpdated()
    End Sub


    Public Sub OnDeviceRemoved(deviceId As String) Implements IMMNotificationClient.OnDeviceRemoved
        RaiseDevicesUpdated()
    End Sub


    Public Sub OnDefaultDeviceChanged(flow As DataFlow, role As Role, defaultDeviceId As String) Implements IMMNotificationClient.OnDefaultDeviceChanged
        RaiseDevicesUpdated()
    End Sub


    Public Sub OnDeviceStateChanged(deviceId As String, newState As DeviceState) Implements IMMNotificationClient.OnDeviceStateChanged
        ' Ignore
    End Sub


    Public Sub OnPropertyValueChanged(pwstrDeviceId As String, key As PropertyKey) Implements IMMNotificationClient.OnPropertyValueChanged
        ' Ignore
    End Sub

#End Region


#Region " IDisposable implementation "

    Private mDisposed As Boolean ' To detect redundant calls


    Public Sub Dispose() Implements IDisposable.Dispose
        If mDisposed Then
            Return
        End If

        mDeviceEnum.Dispose()
        Instance.Dispose()
        mDisposed = True
    End Sub

#End Region

End Class
