Imports System.ComponentModel
Imports System.Windows
Imports Common
Imports NAudio.Wave


<Description("ASIO")>
<Serializable>
Public Class AsioAudioInterface
    Inherits AudioOutputInterfaceBase

#Region " Commands "

    <NonSerialized>
    Private ReadOnly mOpenAsioControlCommand As DelegateCommand = New DelegateCommand(
        AddressOf OpenAsioControlCommandExecuted, AddressOf OpenAsioControlCommandCanExecute)


    Public ReadOnly Property OpenAsioControlCommand As DelegateCommand
        Get
            Return mOpenAsioControlCommand
        End Get
    End Property


    Private Function OpenAsioControlCommandCanExecute(obj As Object) As Boolean
        Return Not String.IsNullOrEmpty(DriverName)
    End Function


    Private Sub OpenAsioControlCommandExecuted(param As Object)
        Try
            Dim asioDrv As New AsioOut(DriverName)
            If asioDrv Is Nothing Then Return

            asioDrv.ShowControlPanel()

        Catch ex As Exception
            InterfaceMapper.GetImplementation(Of IMessageLog).LogAudioError(
                "Cannot open control panel for '{0}'", DriverName)
        End Try
    End Sub

#End Region


#Region " DriverName notifying property "

    Private mDriverName As String = String.Empty


    ''' <summary>
    ''' Driver name, from AsioOut.GetDrivers()
    ''' </summary>
    Public Property DriverName As String
        Get
            Return mDriverName
        End Get
        Set(value As String)
            SetField(mDriverName, value, Function() DriverName)
        End Set
    End Property

#End Region


#Region " DeviceList read-only notifying property "

    ''' <summary>
    ''' List of ASIO devices.
    ''' </summary>
    Private Shared sDeviceList As BindingList(Of AsioInterfaceInfo)


    ' ReSharper disable once MemberCanBeMadeStatic.Global
    ''' <summary>
    ''' List of devices
    ''' </summary>
    Public ReadOnly Property DeviceList As IEnumerable(Of AsioInterfaceInfo)
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
        WeakEventManager(Of AudioChangedListener, EventArgs).
            AddHandler(AudioChangedListener.Instance, "DevicesUpdated", AddressOf DevicesUpdatedHandler)
        UpdateDeviceInfo()
    End Sub

#End Region


#Region " IAudioOutputInterface methods "

    Public Overrides Function CreatePlayer() As IWavePlayer
        Try
            If String.IsNullOrEmpty(DriverName) Then Return Nothing

            Return New AsioOut(DriverName)

        Catch ex As Exception
            InterfaceMapper.GetImplementation(Of IMessageLog).LogAudioError(
                "Cannot open ASIO driver '{0}'", DriverName)
            Return Nothing
        End Try
    End Function

#End Region


#Region " Utility "

    ''' <summary>
    ''' React on audio device updates.
    ''' </summary>
    Private Sub DevicesUpdatedHandler(sender As Object, args As EventArgs)
        UpdateDeviceInfo()
    End Sub


    ''' <summary>
    ''' Re-read ASIO drivers.
    ''' </summary>
    Private Sub UpdateDeviceInfo()
        Dim devList() As String = {}

        If AsioOut.isSupported Then
            devList = AsioOut.GetDriverNames()
        End If

        Application.Current.Dispatcher.BeginInvoke(
            Sub()
                UpdateDeviceInfoUnsafe(devList)
                RaisePropertyChanged(Function() DeviceList)
                UpdateFirstDeviceUnsafe()
            End Sub)
    End Sub


    ''' <summary>
    ''' Creation and updating of the list shall happen on the same thread.
    ''' </summary>
    Private Shared Sub UpdateDeviceInfoUnsafe(devList As IEnumerable(Of String))
        ' Make sure it is created in the right thread
        If sDeviceList Is Nothing Then
            sDeviceList = New BindingList(Of AsioInterfaceInfo)()
        End If

        ' No devices - just empty the list
        If devList.Count = 0 Then
            sDeviceList.Clear()
            Return
        End If

        ' Add or update devices
        For Each drvName In devList
            Dim existing = sDeviceList.SingleOrDefault(Function(d) d.DriverName = drvName)
            Try
                Dim info As AsioInterfaceInfo
                Using asioDriver As New AsioOut(drvName)
                    info = New AsioInterfaceInfo With {
                        .DriverName = drvName,
                        .Channels = asioDriver.DriverOutputChannelCount
                }
                End Using

                If existing Is Nothing Then
                    sDeviceList.Add(info)
                Else
                    existing.DriverName = drvName ' info.DriverName is empty
                    existing.Channels = info.Channels
                End If

            Catch ex As Exception
                InterfaceMapper.GetImplementation(Of IMessageLog)().LogAudioError(
                    "Error opening ASIO driver '{0}': {1}", drvName, ex.Message)
            End Try
        Next

        ' Delete superfluous devices
        Dim toDel = sDeviceList.Where(Function(d) Not devList.Any(Function(s) s = d.DriverName)).ToList()
        For Each delDev In toDel
            sDeviceList.Remove(delDev)
        Next
    End Sub


    ''' <summary>
    ''' If the current device is not set, set to the first available.
    ''' </summary>
    Private Sub UpdateFirstDeviceUnsafe()
        If String.IsNullOrEmpty(DriverName) AndAlso DeviceList.Count > 0 Then
            Dim first = DeviceList.First()
            DriverName = first.DriverName
            Channels = first.Channels
        End If
    End Sub

#End Region

End Class
