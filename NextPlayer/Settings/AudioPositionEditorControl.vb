Imports System.ComponentModel
Imports System.Windows.Media.Media3D
Imports AudioChannelLibrary
Imports Common
Imports DrawingLibrary
Imports HelixToolkit.Wpf
Imports RoomDivisionLibrary
Imports WpfResources


<TemplatePart(Name:="PartView3D", Type:=GetType(HelixViewport3D))>
Public Class AudioPositionEditorControl
    Inherits Control

#Region " Fields "

    Private WithEvents mView As HelixViewport3D


    Private ReadOnly mModelList As New ModelReferenceList()


    Private WithEvents mLayouter As I3DLayouter(Of AudioPhysicalChannel)

#End Region


#Region " Room dependency property "

    Private WithEvents mRoom As Room3D


    Public Shared ReadOnly RoomProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(Room), GetType(Room3D), GetType(AudioPositionEditorControl),
        New PropertyMetadata(New PropertyChangedCallback(AddressOf RoomPropertyChanged)))


    <Description("Defines the room, in which the channels are placed")>
    Public Property Room As Room3D
        Get
            Return CType(GetValue(RoomProperty), Room3D)
        End Get
        Set(value As Room3D)
            SetValue(RoomProperty, value)
        End Set
    End Property


    Private Shared Sub RoomPropertyChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
        Dim this = CType(obj, AudioPositionEditorControl)
        this.mRoom = CType(args.NewValue, Room3D)
        this.RoomChanged(obj, Nothing)
    End Sub

#End Region


#Region " ShowDivision dependency property "

    Public Shared ReadOnly ShowDivisionProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(ShowDivision), GetType(Integer), GetType(AudioPositionEditorControl),
        New PropertyMetadata(New PropertyChangedCallback(AddressOf ShowDivisionPropertyChanged)))


    <Description("For which channel to show the room subdivision, 0 if none")>
    Public Property ShowDivision As Integer
        Get
            Return CInt(GetValue(ShowDivisionProperty))
        End Get
        Set(value As Integer)
            SetValue(ShowDivisionProperty, value)
        End Set
    End Property


    Private Shared Sub ShowDivisionPropertyChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
        Dim this = CType(obj, AudioPositionEditorControl)
        this.GenerateRoomAndSpeakers()
    End Sub


#End Region


#Region " Channels dependency property "

    Private WithEvents mChannels As ChannelCollection(Of AudioPhysicalChannel)


    Public Shared ReadOnly ChannelsProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(Channels), GetType(ChannelCollection(Of AudioPhysicalChannel)), GetType(AudioPositionEditorControl),
        New PropertyMetadata(New PropertyChangedCallback(AddressOf ChannelsPropertyChanged)))


    <Description("A collection of channels to edit")>
    Public Property Channels As ChannelCollection(Of AudioPhysicalChannel)
        Get
            Return CType(GetValue(ChannelsProperty), ChannelCollection(Of AudioPhysicalChannel))
        End Get
        Set(value As ChannelCollection(Of AudioPhysicalChannel))
            SetValue(ChannelsProperty, value)
        End Set
    End Property


    Private Shared Sub ChannelsPropertyChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
        Dim this = CType(obj, AudioPositionEditorControl)
        this.mChannels = CType(args.NewValue, ChannelCollection(Of AudioPhysicalChannel))
        this.ChannelChangedHandler()
    End Sub

#End Region


#Region " Projection dependency property "

    Public Shared ReadOnly ProjectionProperty As DependencyProperty = DependencyProperty.Register(NameOf(Projection), GetType(Projections), GetType(AudioPositionEditorControl), New PropertyMetadata(New PropertyChangedCallback(AddressOf ProjectionPropertyChanged)))


    <Description("Which projection to show")>
    Public Property Projection As Projections
        Get
            Return CType(GetValue(ProjectionProperty), Projections)
        End Get
        Set(value As Projections)
            SetValue(ProjectionProperty, value)
        End Set
    End Property


    Private Shared Sub ProjectionPropertyChanged(obj As DependencyObject, args As DependencyPropertyChangedEventArgs)
        Dim this = CType(obj, AudioPositionEditorControl)

        If this.mView IsNot Nothing Then
            this.mView.Camera = HelixDrawer.GenerateCamera(this.mView, this.Room, this.Projection)
        End If
    End Sub

#End Region


#Region " Init and clean-up "

    Private Sub LoadedHandler() Handles Me.Loaded
        ' Design mode
        If Template Is Nothing Then Return

        mView = CType(Template.FindName("PartView3D", Me), HelixViewport3D)
        mView.Camera = HelixDrawer.GenerateCamera(mView, Room, Projection)
        GenerateRoomAndSpeakers()
    End Sub


#End Region


#Region " Event handlers "

    Private Sub RoomChanged(sender As Object, args As PropertyChangedEventArgs) Handles mRoom.PropertyChanged
        GenerateRoomAndSpeakers()
    End Sub


    Private Sub ChannelChangedHandler() Handles mChannels.ListChanged
        GenerateRoomAndSpeakers()
    End Sub


    Private Sub LayoutChangedHandler() Handles mLayouter.LayoutChanged
        GenerateRoomAndSpeakers()
    End Sub

#End Region


#Region " Generate models "

    ''' <summary>
    ''' Draw room and the audience.
    ''' </summary>
    Private Sub GenerateRoomAndSpeakers()
        ' Ready?
        If mView Is Nothing Then Return
        If Room Is Nothing Then Return

        Dim conf = CType(InterfaceMapper.GetImplementation(Of IConfiguration)().Skin, SkinConfiguration)
        Dim ae = InterfaceMapper.GetImplementation(Of IAudioEnvironmentStorage)

        mView.Children.Clear()
        mView.Children.Add(ModelBuilder.CreateLight())
        mView.Children.Add(GetRoomGeometry(conf))
        mView.Children.Add(GetAudienceGeometry(conf))
        mView.Children.Add(GenerateChannelSpeakers(ShowDivision, ae, conf))


        ' Transparent objects shall be rendered last
        If ShowDivision > 0 Then
            mView.Children.Add(GetDivisionGeometry(ShowDivision, conf))

            Dim newLayouter = ae.GetLayouter(ShowDivision)
            If mLayouter IsNot newLayouter Then
                mLayouter = newLayouter
            End If
        Else
            mLayouter = Nothing
        End If
    End Sub


    ''' <summary>
    ''' Draw the room boundaries.
    ''' </summary>
    Private Function GetRoomGeometry(conf As SkinConfiguration) As Visual3D
        Dim res = BuildParallelepipedMeshVisual(Room, conf.FrameColor)

        mModelList.SetModel(ModelledObjectTypes.Room, res, Room)

        Return res
    End Function


    ''' <summary>
    ''' Draw audience placement.
    ''' </summary>
    Private Function GetAudienceGeometry(conf As SkinConfiguration) As Visual3D
        Dim res = BuildAudienceVisual(Room, conf.MainTextColor)

        mModelList.SetModel(ModelledObjectTypes.Audience, res, Room)

        Return res
    End Function


    ''' <summary>
    ''' Draw the room subdivision.
    ''' </summary>
    Private Function GetDivisionGeometry(
        logChNr As Integer, conf As SkinConfiguration
    ) As Visual3D
        Dim audio = InterfaceMapper.GetImplementation(Of IAudioEnvironmentStorage)()
        Dim res = BuildPolyhedronListMeshVisual(
            audio.GetLayouter(logChNr).GetPolyhedrons(), conf.MainTextColor)

        mModelList.SetModel(ModelledObjectTypes.DividingPolyhedrons, res, audio.GetLayouter(logChNr))

        Return res
    End Function


    ''' <summary>
    ''' Draw ordinary speakers.
    ''' </summary>
    Private Function GenerateChannelSpeakers(
        logCh As Integer, env As IAudioEnvironmentStorage,
        conf As SkinConfiguration
    ) As Visual3D
        Dim res As New ModelVisual3D()
        Dim phList As IList(Of Integer) = Nothing

        If logCh > 0 Then
            phList = (
                From lnk In env.GetLinks(logCh) Select lnk.Physical.Channel
            ).ToList()
        End If

        For Each ch In Channels
            Dim isEnabled = If(logCh = 0, True,
                phList.Contains(ch.Channel) AndAlso ch.IsEnabled)
            Dim color =
                If(ch.IsActive, conf.ActiveTextColor,
                If(isEnabled, conf.MainTextColor,
                conf.DisabledTextColor))
            Dim spkModel = BuildPointVisual(ch, color)

            res.Children.Add(spkModel)
        Next

        mModelList.SetModel(ModelledObjectTypes.AllSpeakers, res, Channels)

        Return res
    End Function


    ''' <summary>
    ''' Draw selected speakers.
    ''' </summary>
    Private Function GenerateSelectedSpeakers(
        spkList As IEnumerable(Of AudioPhysicalChannel),
        conf As SkinConfiguration
    ) As Visual3D
        Dim res As New ModelVisual3D()

        For Each ch In spkList
            Dim color =
                If(ch.IsActive, conf.ActiveTextColor,
                If(ch.IsEnabled, conf.MainTextColor,
                conf.DisabledTextColor))
            Dim spkModel = BuildPointVisual(ch, color, 1.5)

            res.Children.Add(spkModel)
        Next

        Return res
    End Function

#End Region


#Region " Mouse events "

    Private Sub MouseMoveHander(sender As Object, args As MouseEventArgs) Handles mView.MouseMove
        If args.LeftButton = MouseButtonState.Released AndAlso args.RightButton = MouseButtonState.Released Then
            Dim objList = mView.Viewport.FindHits(args.GetPosition(mView))
            If objList.Count = 0 Then Return

            If objList.Any(Function(h) TypeOf h.Visual Is SphereVisual3D) Then
                ' Mouse over the speaker
            End If

            ' Emphasize the polyhedron we're inside of
            Dim pos = objList.First().Position
            Dim pt = Point3DHelper.Create(pos.X, pos.Y, pos.Z)

            Dim divModel = mModelList.GetModel(ModelledObjectTypes.DividingPolyhedrons)
            If divModel Is Nothing Then Return

            ' Clean the visual tree
            Dim selPolyModel = mModelList.GetModel(ModelledObjectTypes.SelectedPolyhedron)
            If selPolyModel IsNot Nothing Then
                mView.Children.Remove(selPolyModel.Model)
            End If

            Dim selSpkModel = mModelList.GetModel(ModelledObjectTypes.SelectedSpeakers)
            If selSpkModel IsNot Nothing Then
                mView.Children.Remove(selSpkModel.Model)
            End If

            ' Get the layout
            Dim layouter = CType(divModel.Reference, I3DLayouter(Of AudioPhysicalChannel))
            Dim conf = CType(InterfaceMapper.GetImplementation(Of IConfiguration)().Skin, SkinConfiguration)

            ' Add new objects
            Dim spkList = layouter.GetReferences(pt)
            Dim selSpk = GenerateSelectedSpeakers(spkList, conf)
            mView.Children.Add(selSpk)
            mModelList.SetModel(ModelledObjectTypes.SelectedSpeakers, selSpk, Nothing)

            Dim poly = layouter.GetContainingPolyhedron(pt)
            If poly Is Nothing Then Return

            Dim selPoly = BuildPolyhedronFaceVisual(poly, conf.PlayIndicatorColor)
            mView.Children.Add(selPoly)
            mModelList.SetModel(ModelledObjectTypes.SelectedPolyhedron, selPoly, Nothing)
        End If
    End Sub

#End Region

End Class
