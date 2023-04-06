
''' <summary>
''' Stores the room and audience dimensions.
''' </summary>
''' <remarks>
''' The <see cref="IPoint3D"/> is bogus, only to ensure the types are
''' for absolute positioning.
''' 
''' Room and audience sizes are always rounded to
''' <see cref="AbsoluteCoordPrecisionDigits"/> decimals.
''' </remarks>
<Serializable>
<CLSCompliant(True)>
Public Class Room3D
    Inherits PropertyChangedHelper
    Implements IPoint3D

#Region " XLeft notifying property "

    Private mXLeft As Double = 1


    ''' <summary>
    ''' From the center to the left room border (in negative X direction).
    ''' </summary>
    Public Property XLeft As Double
        Get
            Return mXLeft
        End Get
        Set(value As Double)
            If value < 0 Then value = 0
            value = Math.Round(value, AbsoluteCoordPrecisionDigits)
            SetField(mXLeft, value, NameOf(XLeft))
            mXLeftPlane = Nothing
            UpdateReadOnlyProperties()
        End Set
    End Property

#End Region


#Region " XRight notifying property "

    Private mXRight As Double = 1


    ''' <summary>
    ''' From the center to the right room border (positive X).
    ''' </summary>
    Public Property XRight As Double Implements IPoint3D.X
        Get
            Return mXRight
        End Get
        Set(value As Double)
            If value < 0 Then value = 0
            value = Math.Round(value, AbsoluteCoordPrecisionDigits)
            SetField(mXRight, value, NameOf(XRight))
            mXRightPlane = Nothing
            UpdateReadOnlyProperties()
        End Set
    End Property

#End Region


#Region " YFront notifying property "

    Private mYFront As Double = 1


    ''' <summary>
    ''' From the center to the room border in front of the audience (positive Y).
    ''' </summary>
    Public Property YFront As Double Implements IPoint3D.Y
        Get
            Return mYFront
        End Get
        Set(value As Double)
            If value < 0 Then value = 0
            value = Math.Round(value, AbsoluteCoordPrecisionDigits)
            SetField(mYFront, value, NameOf(YFront))
            mYFrontPlane = Nothing
            UpdateReadOnlyProperties()
        End Set
    End Property

#End Region


#Region " YBack notifying property "

    Private mYBack As Double = 1


    ''' <summary>
    ''' From the center to the room border behind the audience (negative Y).
    ''' </summary>
    Public Property YBack As Double
        Get
            Return mYBack
        End Get
        Set(value As Double)
            If value < 0 Then value = 0
            value = Math.Round(value, AbsoluteCoordPrecisionDigits)
            SetField(mYBack, value, NameOf(YBack))
            mYBackPlane = Nothing
            UpdateReadOnlyProperties()
        End Set
    End Property

#End Region


#Region " ZAbove notifying property "

    Private mZAbove As Double = 1


    ''' <summary>
    ''' From the center to the room's ceiling (positive Z).
    ''' </summary>
    Public Property ZAbove As Double Implements IPoint3D.Z
        Get
            Return mZAbove
        End Get
        Set(value As Double)
            If value < 0 Then value = 0
            value = Math.Round(value, AbsoluteCoordPrecisionDigits)
            SetField(mZAbove, value, NameOf(ZAbove))
            mZAbovePlane = Nothing
            UpdateReadOnlyProperties()
        End Set
    End Property

#End Region


#Region " ZBelow notifying property "

    Private mZBelow As Double = 0


    ''' <summary>
    ''' From the center to the room's floor (negative Z).
    ''' </summary>
    Public Property ZBelow As Double
        Get
            Return mZBelow
        End Get
        Set(value As Double)
            If value < 0 Then value = 0
            value = Math.Round(value, AbsoluteCoordPrecisionDigits)
            SetField(mZBelow, value, NameOf(ZBelow))
            mZBelowPlane = Nothing
            UpdateReadOnlyProperties()
        End Set
    End Property

#End Region


#Region " AudienceLeft notifying property "

    Private mAudienceLeft As Double = 0


    ''' <summary>
    ''' From the center to the left audience border (negative X).
    ''' </summary>
    Public Property AudienceLeft As Double
        Get
            Return mAudienceLeft
        End Get
        Set(value As Double)
            If value < 0 Then value = 0
            value = Math.Round(value, AbsoluteCoordPrecisionDigits)
            SetField(mAudienceLeft, value, NameOf(AudienceLeft))
        End Set
    End Property

#End Region


#Region " AudienceRight notifying property "

    Private mAudienceRight As Double = 0


    ''' <summary>
    ''' From the center to the right audience border (positive X).
    ''' </summary>
    Public Property AudienceRight As Double
        Get
            Return mAudienceRight
        End Get
        Set(value As Double)
            If value < 0 Then value = 0
            value = Math.Round(value, AbsoluteCoordPrecisionDigits)
            SetField(mAudienceRight, value, NameOf(AudienceRight))
        End Set
    End Property

#End Region


#Region " AudienceFront notifying property "

    Private mAudienceFront As Double = 0


    ''' <summary>
    ''' From the center to the front audience border (positive Y).
    ''' </summary>
    Public Property AudienceFront As Double
        Get
            Return mAudienceFront
        End Get
        Set(value As Double)
            If value < 0 Then value = 0
            value = Math.Round(value, AbsoluteCoordPrecisionDigits)
            SetField(mAudienceFront, value, NameOf(AudienceFront))
        End Set
    End Property

#End Region


#Region " AudienceBack notifying property "

    Private mAudienceBack As Double = 0


    ''' <summary>
    ''' From the center to the back audience border (negative Y).
    ''' </summary>
    Public Property AudienceBack As Double
        Get
            Return mAudienceBack
        End Get
        Set(value As Double)
            If value < 0 Then value = 0
            value = Math.Round(value, AbsoluteCoordPrecisionDigits)
            SetField(mAudienceBack, value, NameOf(AudienceBack))
        End Set
    End Property

#End Region


#Region " RoomSize read-only property "

    <NonSerialized>
    Private mRoomSize As Double?


    ''' <summary>
    ''' Get the size, which is no smaller (but may be larger)
    ''' than the largest distance from origin to the farhest room's corner.
    ''' </summary>
    Public ReadOnly Property RoomSize As Double
        Get
            If Not mRoomSize.HasValue Then
                Dim xmax = Math.Max(XLeft, XRight)
                Dim ymax = Math.Max(YFront, YBack)
                Dim zmax = Math.Max(ZAbove, ZBelow)
                mRoomSize = Math.Sqrt(xmax * xmax + ymax * ymax + zmax * zmax)
            End If

            Return mRoomSize.Value
        End Get
    End Property

#End Region


#Region " AudiencePoints read-only property "

    <NonSerialized>
    Private mAudiencePoints As List(Of IPoint3D)


    ''' <summary>
    ''' Get vertices of the audio plane as a list.
    ''' </summary>
    Public ReadOnly Property AudiencePoints As IReadOnlyList(Of IPoint3D)
        Get
            If mAudiencePoints Is Nothing Then
                mAudiencePoints = New List(Of IPoint3D)()

                If IsEqual(-AudienceLeft, AudienceRight) AndAlso IsEqual(-AudienceBack, AudienceFront) Then
                    ' Only one point, the origin
                    mAudiencePoints.Add(Point3DHelper.Origin)

                ElseIf IsEqual(-AudienceLeft, AudienceRight) Then
                    ' X's are 0
                    mAudiencePoints.Add(Point3DHelper.Create(0, AudienceFront, 0))
                    mAudiencePoints.Add(Point3DHelper.Create(0, -AudienceBack, 0))

                ElseIf IsEqual(-AudienceBack, AudienceFront) Then
                    ' Y's are 0
                    mAudiencePoints.Add(Point3DHelper.Create(-AudienceLeft, 0, 0))
                    mAudiencePoints.Add(Point3DHelper.Create(AudienceRight, 0, 0))

                Else
                    ' All 4 points are present
                    mAudiencePoints.Add(Point3DHelper.Create(-AudienceLeft, AudienceFront, 0))
                    mAudiencePoints.Add(Point3DHelper.Create(AudienceRight, AudienceFront, 0))
                    mAudiencePoints.Add(Point3DHelper.Create(AudienceRight, -AudienceBack, 0))
                    mAudiencePoints.Add(Point3DHelper.Create(-AudienceLeft, -AudienceBack, 0))
                End If
            End If

            Return mAudiencePoints
        End Get
    End Property

#End Region


#Region " XLeftPlane read-only property "

    <NonSerialized>
    Private mXLeftPlane As IPlane3D


    ''' <summary>
    ''' Return a plane, which goes along the room's left wall,
    ''' and is positive inside the room.
    ''' </summary>
    Public ReadOnly Property XLeftPlane As IPlane3D
        Get
            If mXLeftPlane Is Nothing Then
                mXLeftPlane = Plane3DHelper.CreatePointNormal(
                    Point3DHelper.Create(-XLeft, 0, 0), Vector3D.AlongX, NoRef.Empty)
            End If

            Return mXLeftPlane
        End Get
    End Property

#End Region


#Region " XRightPlane read-only property "

    <NonSerialized>
    Private mXRightPlane As IPlane3D


    ''' <summary>
    ''' Return a plane, which goes along the room's right wall,
    ''' and is positive inside the room.
    ''' </summary>
    Public ReadOnly Property XRightPlane As IPlane3D
        Get
            If mXRightPlane Is Nothing Then
                mXRightPlane = Plane3DHelper.CreatePointNormal(
                    Point3DHelper.Create(XRight, 0, 0), New Vector3D(-1, 0, 0), NoRef.Empty)
            End If

            Return mXRightPlane
        End Get
    End Property

#End Region


#Region " YFrontPlane read-only property "

    <NonSerialized>
    Private mYFrontPlane As IPlane3D


    ''' <summary>
    ''' Return a plane, which goes along the room's front wall,
    ''' and is positive inside the room.
    ''' </summary>
    Public ReadOnly Property YFrontPlane As IPlane3D
        Get
            If mYFrontPlane Is Nothing Then
                mYFrontPlane = Plane3DHelper.CreatePointNormal(
                    Point3DHelper.Create(0, YFront, 0), New Vector3D(0, -1, 0), NoRef.Empty)
            End If

            Return mYFrontPlane
        End Get
    End Property

#End Region


#Region " YBackPlane read-only property "

    <NonSerialized>
    Private mYBackPlane As IPlane3D


    ''' <summary>
    ''' Return a plane, which goes along the room's back wall,
    ''' and is positive inside the room.
    ''' </summary>
    Public ReadOnly Property YBackPlane As IPlane3D
        Get
            If mYBackPlane Is Nothing Then
                mYBackPlane = Plane3DHelper.CreatePointNormal(
                    Point3DHelper.Create(0, -YBack, 0), Vector3D.AlongY, NoRef.Empty)
            End If

            Return mYBackPlane
        End Get
    End Property

#End Region


#Region " ZAbovePlane read-only property "

    <NonSerialized>
    Private mZAbovePlane As IPlane3D


    ''' <summary>
    ''' Return a plane, which goes along the room's ceiling,
    ''' and is positive inside the room.
    ''' </summary>
    Public ReadOnly Property ZAbovePlane As IPlane3D
        Get
            If mZAbovePlane Is Nothing Then
                mZAbovePlane = Plane3DHelper.CreatePointNormal(
                    Point3DHelper.Create(0, 0, ZAbove), New Vector3D(0, 0, -1), NoRef.Empty)
            End If

            Return mZAbovePlane
        End Get
    End Property

#End Region


#Region " ZBelowPlane read-only property "

    <NonSerialized>
    Private mZBelowPlane As IPlane3D


    ''' <summary>
    ''' Return a plane, which goes along the room's floor,
    ''' and is positive inside the room.
    ''' </summary>
    Public ReadOnly Property ZBelowPlane As IPlane3D
        Get
            If mZBelowPlane Is Nothing Then
                mZBelowPlane = Plane3DHelper.CreatePointNormal(
                    Point3DHelper.Create(0, 0, -ZBelow), Vector3D.AlongZ, NoRef.Empty)
            End If

            Return mZBelowPlane
        End Get
    End Property

#End Region


#Region " AllSides read-only property "

    Private ReadOnly mAllSides As IDictionary(Of Type, IReadOnlyCollection(Of IBorder3D)) =
        New Dictionary(Of Type, IReadOnlyCollection(Of IBorder3D))


    ''' <summary>
    ''' Get all room sides as 3D borders.
    ''' </summary>
    Public Function AllSideBorders(Of TRef)() As IReadOnlyCollection(Of Border3D(Of TRef))
        Dim sides As IReadOnlyCollection(Of IBorder3D) = Nothing

        If mAllSides.TryGetValue(GetType(TRef), sides) Then
            Return sides.Cast(Of Border3D(Of TRef))().ToList()
        Else
            Dim sidesAsList = New List(Of Border3D(Of TRef)) From {
                Plane3DHelper.CreateBorderNoRef(Of TRef)(XLeftPlane, 1),
                Plane3DHelper.CreateBorderNoRef(Of TRef)(XRightPlane, 1),
                Plane3DHelper.CreateBorderNoRef(Of TRef)(YFrontPlane, 1),
                Plane3DHelper.CreateBorderNoRef(Of TRef)(YBackPlane, 1),
                Plane3DHelper.CreateBorderNoRef(Of TRef)(ZAbovePlane, 1),
                Plane3DHelper.CreateBorderNoRef(Of TRef)(ZBelowPlane, 1)
            }
            mAllSides.Add(GetType(TRef), sidesAsList)

            Return sidesAsList
        End If
    End Function

#End Region


#Region " Property updates "

    Private Sub UpdateReadOnlyProperties()
        mRoomSize = Nothing
        mAllSides?.Clear()
        mAudiencePoints = Nothing
    End Sub

#End Region


#Region " API "

    ''' <summary>
    ''' Return a plane, on which audience resides.
    ''' "Up" is towards positive Z (up).
    ''' </summary>
    Public Function GetAudiencePlane() As IPlane3D
        Dim res = Plane3DHelper.CreatePointNormal(
            Point3DHelper.Origin, Vector3D.AlongZ, NoRef.Empty)

        Return res
    End Function


    ''' <summary>
    ''' Convert relative coordinates (of a sound source) to physical coordinates.
    ''' </summary>
    Public Function ConvertRelative(c As IPositionRelative) As IPoint3D
        Dim x = If(c.X < 0, c.X * XLeft, c.X * XRight)
        Dim y = If(c.Y < 0, c.Y * YBack, c.Y * YFront)
        Dim z = If(c.Z < 0, c.Z * ZBelow, c.Z * ZAbove)

        Return Point3DHelper.Create(x, y, z)
    End Function


    ''' <summary>
    ''' Project the given point to the audience (at plane Z=0).
    ''' </summary>
    Public Function ProjectToAudience(c As IPoint3D) As IPoint3D
        Dim projX = c.X, projY = c.Y

        If Sign(projX - -AudienceLeft) < 0 Then
            projX = -AudienceLeft
        ElseIf Sign(projX - AudienceRight) > 0 Then
            projX = AudienceRight
        End If

        If Sign(projY - -AudienceBack) < 0 Then
            projY = -AudienceBack
        ElseIf Sign(projY - AudienceFront) > 0 Then
            projY = AudienceFront
        End If

        Return Point3DHelper.Create(projX, projY, 0)
    End Function


    ''' <summary>
    ''' Project the given point to the room's walls.
    ''' </summary>
    ''' <remarks>
    ''' The projection is done by first projecting to the audience plane,
    ''' then building a line between <paramref name="c"/> and its projection
    ''' and finding an intersection with the walls.
    ''' </remarks>
    ''' <returns>
    ''' A line segment, where point A is on the audience plane,
    ''' and point B is going towards the room borders.
    ''' </returns>
    Public Function ProjectPoint(
        c As IPoint3D, mode As ProjectionModes
    ) As ILineSegment3D
        Dim aud = ProjectToAudience(c)
        Dim ln =
            If(Point3DHelper.IsSame(c, aud),
            Line3DHelper.Create(aud, Vector3D.AlongZ),
            Line3DHelper.Create(aud, c))
        Dim coef As Double

        Select Case mode
            Case ProjectionModes.Cube
                coef = {
                        ln.CutByPlane(XLeftPlane),
                        ln.CutByPlane(XRightPlane),
                        ln.CutByPlane(YFrontPlane),
                        ln.CutByPlane(YBackPlane),
                        ln.CutByPlane(ZAbovePlane),
                        ln.CutByPlane(ZBelowPlane)
                    }.
                    Where(Function(p) p IsNot Nothing AndAlso p.Coordinate > 0).
                    Min(Function(p) p.Coordinate)

            Case ProjectionModes.Sphere
                Dim sph As New Sphere3D(Point3DHelper.Origin, RoomSize)
                coef = ln.CutBySphere(sph).Coordinate

            Case Else
                Throw New ArgumentException("Unsupported projection mode " & mode)
        End Select

        Dim ep = ln.Vector.Multiply(coef).From(ln.Point)

        Return LineSegment3DHelper.Create(aud, ep)
    End Function

#End Region


#Region " Test API "

    ''' <summary>
    ''' Create a squared audience with the same side size in all directions.
    ''' </summary>
    Public Sub SetAudienceSides(stdSize As Double)
        AudienceLeft = stdSize
        AudienceRight = stdSize
        AudienceFront = stdSize
        AudienceBack = stdSize
    End Sub


    ''' <summary>
    ''' Create a squared room with the same side size in all directions.
    ''' </summary>
    Public Sub SetAllSides(stdSize As Double)
        XLeft = stdSize
        XRight = stdSize
        YFront = stdSize
        YBack = stdSize
        ZAbove = stdSize
        ZBelow = stdSize
    End Sub

#End Region

End Class
