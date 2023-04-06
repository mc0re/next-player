Imports System.ComponentModel
Imports System.Windows
Imports Common
Imports NAudio.Wave


<Description("Wave Out")>
<Serializable>
Public Class WaveOutAudioInterface
    Inherits AudioOutputInterfaceBase

#Region " Constants "

    Private Const DefaultName As String = "Default windows device"

#End Region


#Region " DeviceNumber notifying property "

    Private mDeviceNumber As Integer


    ''' <summary>
    ''' Device number; -1 = default
    ''' </summary>
    Public Property DeviceNumber As Integer
        Get
            Return mDeviceNumber
        End Get
        Set(value As Integer)
            If value = DeviceNumber Then Return

            If value > WaveOut.DeviceCount - 1 Then value = WaveOut.DeviceCount - 1
            SetField(mDeviceNumber, value, Function() DeviceNumber)
            Channels = WaveOut.GetCapabilities(value).Channels
        End Set
    End Property

#End Region


#Region " DeviceList read-only notifying property "

    ''' <summary>
    ''' List of WaveOut devices.
    ''' </summary>
    Private Shared sDeviceList As BindingList(Of WaveOutInterfaceInfo)


    ' ReSharper disable once MemberCanBeMadeStatic.Global
    ''' <summary>
    ''' Maximum device number
    ''' </summary>
    Public ReadOnly Property DeviceList As IEnumerable(Of WaveOutInterfaceInfo)
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
        DeviceNumber = -1
        WeakEventManager(Of AudioChangedListener, EventArgs).
            AddHandler(AudioChangedListener.Instance, "DevicesUpdated", AddressOf DevicesUpdatedHanlder)
        UpdateDeviceInfo()
    End Sub

#End Region


#Region " IAudioOutputInterface methods "

    Public Overrides Function CreatePlayer() As IWavePlayer
        Return New WaveOutEvent With {.DeviceNumber = DeviceNumber}
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
        Dim devCount = WaveOut.DeviceCount

        ' "Count - 1": Remember default device
        If sDeviceList IsNot Nothing AndAlso devCount = sDeviceList.Count - 1 Then
            Return
        End If

        Application.Current.Dispatcher.BeginInvoke(
            Sub()
                UpdateDeviceInfoUnsafe()
                RaisePropertyChanged(NameOf(DeviceList))
            End Sub)
    End Sub


    ''' <summary>
    ''' Creation and updating of the list shall happen on the same thread.
    ''' </summary>
    Private Shared Sub UpdateDeviceInfoUnsafe()
        If sDeviceList Is Nothing Then
            sDeviceList = New BindingList(Of WaveOutInterfaceInfo)()
        End If

        Dim devCount = WaveOut.DeviceCount

        ' Just empty the list
        If devCount = 0 Then
            sDeviceList.Clear()
            Return
        End If

        ' Add or update devices
        ' Default device = -1
        For devNum = -1 To devCount - 1
            Dim devNum1 = devNum
            Try
                ' If any changes occured on the way
                If devNum1 >= WaveOut.DeviceCount Then Exit For

                Dim cap = WaveOut.GetCapabilities(devNum1)
                Dim devName = If(devNum = -1, DefaultName, cap.ProductName)
                Dim existing = sDeviceList.SingleOrDefault(Function(dev) dev.DeviceNumber = devNum1)

                If existing Is Nothing Then
                    Dim info As New WaveOutInterfaceInfo With {
                        .DeviceNumber = devNum,
                        .Name = devName,
                        .Channels = cap.Channels
                    }
                    sDeviceList.Add(info)
                Else
                    existing.Name = devName
                    existing.Channels = cap.Channels
                End If

            Catch ex As Exception
                InterfaceMapper.GetImplementation(Of IMessageLog).LogAudioError(
                    "WaveOut error on channel {0}: {1}", devNum1, ex.Message)
            End Try
        Next

        ' Delete superfluous devices
        ' Take device "-1" into account
        For delNum = WaveOut.DeviceCount To sDeviceList.Count - 2
            sDeviceList.RemoveAt(delNum)
        Next
    End Sub

#End Region

End Class
