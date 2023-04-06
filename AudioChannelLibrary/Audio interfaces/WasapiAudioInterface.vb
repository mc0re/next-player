Imports System.ComponentModel
Imports System.Windows
Imports NAudio.CoreAudioApi
Imports NAudio.CoreAudioApi.Interfaces
Imports NAudio.Wave


<Description("WASAPI")>
<Serializable>
Public Class WasapiAudioInterface
    Inherits AudioOutputInterfaceBase
    Implements IMMNotificationClient

#Region " Fields "

    <NonSerialized>
    Private mDeviceEnumerator As MMDeviceEnumerator

#End Region


#Region " Latency property "

    ''' <summary>
    ''' Desired latency [milliseconds].
    ''' </summary>
    Public Property Latency As Integer = 100

#End Region


#Region " DeviceId notifying property "

    Private mDeviceId As String


    ''' <summary>
    ''' Device ID
    ''' </summary>
    Public Property DeviceId As String
        Get
            Return mDeviceId
        End Get
        Set(value As String)
            SetField(mDeviceId, value, Function() DeviceId)
        End Set
    End Property

#End Region


#Region " DeviceList read-only notifying property "

    ''' <summary>
    ''' List of WASAPI devices.
    ''' </summary>
    Private Shared sDeviceList As BindingList(Of WasapiInterfaceInfo)


    ''' <summary>
    ''' Maximum device number
    ''' </summary>
    Public ReadOnly Property DeviceList As IEnumerable(Of WasapiInterfaceInfo)
        Get
            Return sDeviceList
        End Get
    End Property

#End Region


#Region " Init and clean-up "

    ''' <summary>
    ''' Create as a default audio interface.
    ''' </summary>
    Public Sub New()
        mDeviceEnumerator = New MMDeviceEnumerator()
        mDeviceEnumerator.RegisterEndpointNotificationCallback(Me)
        UpdateDeviceInfo()
    End Sub

#End Region


#Region " IAudioOutputInterface methods "

    Public Overrides Function CreatePlayer() As IWavePlayer
        Dim dev = (
            From d In mDeviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)
            Where d.ID = DeviceId
        ).FirstOrDefault()

        If dev Is Nothing Then Return Nothing

        Return New WasapiOut(dev, AudioClientShareMode.Shared, True, Latency)
    End Function

#End Region


#Region " Utility "

    Private Sub UpdateDeviceInfo()
        Try
            Dim devList = mDeviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active)
            Dim defDevice = mDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia)

            If sDeviceList IsNot Nothing AndAlso devList.Count = sDeviceList.Count Then
                Return
            End If

            Dim wasapiList = (
                From d In devList
                Select New WasapiInterfaceInfo With {
                    .Id = d.ID,
                    .Name = d.FriendlyName,
                    .Icon = d.IconPath,
                    .IsDefault = (d.ID = defDevice.ID)
                }
            ).ToList()

            Application.Current.Dispatcher.BeginInvoke(
                Sub()
                    UpdateDeviceInfoUnsafe(wasapiList)
                    RaisePropertyChanged(Function() DeviceList)
                End Sub)

        Catch ex As Exception
            Return
        End Try
    End Sub


    ''' <summary>
    ''' Creation and updating of the list shall happen on the same thread.
    ''' </summary>
    Private Shared Sub UpdateDeviceInfoUnsafe(devList As IList(Of WasapiInterfaceInfo))
        If sDeviceList Is Nothing Then
            sDeviceList = New BindingList(Of WasapiInterfaceInfo)()
        End If

        ' Just empty the list
        If devList.Count = 0 Then
            sDeviceList.Clear()
            Return
        End If

        ' Add or update devices
        For Each dev In devList
            Dim existing = sDeviceList.SingleOrDefault(Function(d) d.Id = dev.Id)

            If existing Is Nothing Then
                sDeviceList.Add(dev)
            Else
                existing.Name = dev.Name
                existing.Icon = dev.Icon
            End If
        Next

        ' Delete superfluous devices
        Dim toDel = sDeviceList.Where(Function(d) Not devList.Any(Function(s) s.Id = d.Id)).ToList()
        For Each delDev In toDel
            sDeviceList.Remove(delDev)
        Next
    End Sub

#End Region


#Region " IMMNotificationClient implementation "

    Public Sub OnDeviceStateChanged(deviceIdentifier As String, newState As DeviceState) Implements IMMNotificationClient.OnDeviceStateChanged
        UpdateDeviceInfo()
    End Sub


    Public Sub OnDeviceAdded(deviceIdentifier As String) Implements IMMNotificationClient.OnDeviceAdded
        UpdateDeviceInfo()
    End Sub


    Public Sub OnDeviceRemoved(deviceIdentifier As String) Implements IMMNotificationClient.OnDeviceRemoved
        UpdateDeviceInfo()
    End Sub


    Public Sub OnDefaultDeviceChanged(flow As DataFlow, role As Role, defaultDeviceId As String) Implements IMMNotificationClient.OnDefaultDeviceChanged
        UpdateDeviceInfo()
    End Sub


    Public Sub OnPropertyValueChanged(pwstrDeviceId As String, key As PropertyKey) Implements IMMNotificationClient.OnPropertyValueChanged
        UpdateDeviceInfo()
    End Sub

#End Region

End Class
