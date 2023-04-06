Imports System.ComponentModel
Imports System.Windows
Imports NAudio.Wave


<Description("Direct Sound")>
<Serializable>
Public Class DirectSoundAudioInterface
    Inherits AudioOutputInterfaceBase

#Region " Latency property "

    ''' <summary>
    ''' Desired latency [milliseconds].
    ''' </summary>
    Public Property Latency As Integer = 100

#End Region


#Region " DeviceGuid notifying property "

    Private mDeviceGuid As Guid


    ''' <summary>
    ''' Device ID, comes from DirectSoundOut.GetDevices().
    ''' </summary>
    Public Property DeviceGuid As Guid
        Get
            Return mDeviceGuid
        End Get
        Set(value As Guid)
            SetField(mDeviceGuid, value, Function() DeviceGuid)
        End Set
    End Property

#End Region


#Region " DeviceList read-only notifying property "

    ''' <summary>
    ''' List of DirectSound devices.
    ''' </summary>
    Private Shared sDeviceList As BindingList(Of DirectSoundInterfaceInfo)


    ' ReSharper disable once MemberCanBeMadeStatic.Global
    ''' <summary>
    ''' Maximum device number
    ''' </summary>
    Public ReadOnly Property DeviceList As IEnumerable(Of DirectSoundInterfaceInfo)
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
        DeviceGuid = Guid.Empty
        WeakEventManager(Of AudioChangedListener, EventArgs).
            AddHandler(AudioChangedListener.Instance, "DevicesUpdated", AddressOf DevicesUpdatedHanlder)
        UpdateDeviceInfo()
    End Sub

#End Region


#Region " IAudioOutputInterface methods "

    Public Overrides Function CreatePlayer() As IWavePlayer
        Return New DirectSoundOut(DeviceGuid, Latency)
    End Function

#End Region


#Region " Utility "

    ''' <summary>
    ''' React on audio device updates.
    ''' </summary>
    Private Sub DevicesUpdatedHanlder(sender As Object, args As EventArgs)
        UpdateDeviceInfo()
    End Sub


    Private Sub UpdateDeviceInfo()
        Dim devList = DirectSoundOut.Devices.ToList()
        If sDeviceList IsNot Nothing AndAlso devList.Count = sDeviceList.Count Then
            Return
        End If

        Application.Current.Dispatcher.BeginInvoke(
            Sub()
                UpdateDeviceInfoUnsafe(devList)
                RaisePropertyChanged(Function() DeviceList)
            End Sub)
    End Sub


    ''' <summary>
    ''' Creation and updating of the list shall happen on the same thread.
    ''' </summary>
    Private Shared Sub UpdateDeviceInfoUnsafe(devList As IList(Of DirectSoundDeviceInfo))
        If sDeviceList Is Nothing Then
            sDeviceList = New BindingList(Of DirectSoundInterfaceInfo)()
        End If

        ' Just empty the list
        If devList.Count = 0 Then
            sDeviceList.Clear()
            Return
        End If

        ' Add or update devices
        For Each dev In devList
            Dim existing = sDeviceList.SingleOrDefault(Function(d) d.DeviceGuid = dev.Guid)

            If existing Is Nothing Then
                Dim info As New DirectSoundInterfaceInfo With {
                    .DeviceGuid = dev.Guid,
                    .Name = dev.Description
                }
                sDeviceList.Add(info)
            Else
                existing.Name = dev.Description
            End If
        Next

        ' Delete superfluous devices
        Dim toDel = sDeviceList.Where(Function(d) Not devList.Any(Function(s) s.Guid = d.DeviceGuid)).ToList()
        For Each delDev In toDel
            sDeviceList.Remove(delDev)
        Next
    End Sub

#End Region

End Class
