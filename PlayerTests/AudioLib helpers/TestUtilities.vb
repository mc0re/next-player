Imports System.Reflection
Imports AudioChannelLibrary
Imports Common


Public Module TestUtilities

    Public Function GetTestFilePath(name As String) As String
        Dim exeDir = IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        Dim resPath = IO.Path.Combine(exeDir, "Resources")
        Return IO.Path.Combine(resPath, name)
    End Function


    ''' <remarks>
    ''' The definition format is:
    '''   mapping ::= { "+" | "-" } [ "|" panning ]
    '''   panning ::= number -1..+1, default 0
    ''' </remarks>
    Public Function ParseLinkMapping(def As String) As AudioChannelLinkMapping
        Dim split = def.Split("|"c)
        Dim res As New AudioChannelLinkMapping With {.NofChannels = split(0).Length}

        For Each ch In split(0)
            Dim isSet As Boolean

            Select Case ch
                Case "+"c
                    isSet = True
                Case "-"c
                    isSet = False
                Case Else
                    Throw New ArgumentException()
            End Select

            res.MappingList.Add(New AudioChannelMappingItem With {.IsSet = isSet})
        Next

        If split.Length > 1 Then
            res.Panning = Single.Parse(split(1))
        End If

        Return res
    End Function


    ''' <summary>
    ''' To avoid problems with Dispatcher, a test audio interface is needed.
    ''' </summary>
    Public Sub SetupTestIo()
        AudioPhysicalChannel.AudioInterfaceTypeList.Clear()

        AudioPhysicalChannel.AudioInterfaceTypeList.Add(New TypeImplementationInfo With {
            .Name = "Test mono", .ImplementingType = GetType(TestMonoOutputInterface)})
        AudioPhysicalChannel.AudioInterfaceTypeList.Add(New TypeImplementationInfo With {
            .Name = "Test stereo", .ImplementingType = GetType(TestStereoOutputInterface)})
        AudioPhysicalChannel.AudioInterfaceTypeList.Add(New TypeImplementationInfo With {
            .Name = "Test triple", .ImplementingType = GetType(TestTripleOutputInterface)})

        ' Default - mono output for each channel
        InterfaceMapper.SetType(Of IAudioOutputInterface, TestMonoOutputInterface)()
    End Sub

End Module
