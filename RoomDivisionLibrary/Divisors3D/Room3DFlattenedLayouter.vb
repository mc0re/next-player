Imports Common


''' <summary>
''' Layout the room by ignoring the Z-coordinate
''' and flattening all points onto a plane.
''' Then use the 2D layouter.
''' </summary>
''' <typeparam name="TRef">Payload type</typeparam>
<CLSCompliant(True)>
Public Class Room3DFlattenedLayouter(Of TRef)
    Implements I3DLayouter(Of TRef)

#Region " Events "

    Public Event LayoutChanged As I3DLayouter(Of TRef).LayoutChangedEventHandler Implements I3DLayouter(Of TRef).LayoutChanged

#End Region


#Region " Fields "

    Private ReadOnly mLayouter As New Room2DLayouter(Of TRef)()


    Private ReadOnly mPolygonMapper As New Dictionary(Of Polygon2D(Of TRef), Polyhedron(Of TRef))()

#End Region


#Region " Properties "

    Private mRoom As Room3D


    Public ReadOnly Property Room As Room3D Implements I3DLayouter(Of TRef).Room
        Get
            Return mRoom
        End Get
    End Property

#End Region


#Region " ILayouter API "

    Public Sub PrepareLayout(room As Room3D, vertices As IEnumerable(Of Point3D(Of TRef))) Implements I3DLayouter(Of TRef).PrepareLayout
        mPolygonMapper.Clear()
        mRoom = room

        Dim room2d = New Room2D(
                        room.XLeft, room.XRight, room.YBack, room.YFront,
                        room.AudienceLeft, room.AudienceRight, room.AudienceBack, room.AudienceFront)
        Dim flattened = From v In vertices Select Point2DHelper.Create(v.X, v.Y, v.References)
        mLayouter.PrepareLayout(room2d, flattened)

        RaiseEvent LayoutChanged()
    End Sub


    Public Function GetReferences(location As IPoint3D) As IReadOnlyCollection(Of TRef) Implements I3DLayouter(Of TRef).GetReferences
        Dim c = Point2DHelper.Create(location.X, location.Y)
        Return mLayouter.GetReferences(c)
    End Function

#End Region


#Region " Visualization API "

    Public Function GetPolyhedrons() As IEnumerable(Of Polyhedron(Of TRef)) Implements I3DLayouter(Of TRef).GetPolyhedrons
        If Not mPolygonMapper.Any() Then
            For Each p In mLayouter.GetPolygons()
                mPolygonMapper.Add(p, ToPolyhedron(p, -Room.ZBelow, Room.ZAbove))
            Next
        End If

        Return mPolygonMapper.Values
    End Function


    Public Function GetContainingPolyhedron(pt As IPoint3D) As Polyhedron(Of TRef) Implements I3DLayouter(Of TRef).GetContainingPolyhedron
        Dim polygon = mLayouter.GetContainingPolygon(Point2DHelper.Create(pt.X, pt.Y))

        If polygon Is Nothing Then
            Return Nothing
        End If

        Return mPolygonMapper(polygon)
    End Function

#End Region


#Region " Visualization utility "

    ''' <summary>
    ''' Convert the given polygon into a polyhedron by extending it in Z dimension.
    ''' </summary>
    ''' <param name="polygon"></param>
    ''' <param name="zBelow"></param>
    ''' <param name="zAbove"></param>
    Private Function ToPolyhedron(
        polygon As Polygon2D(Of TRef), zBelow As Double, zAbove As Double
    ) As Polyhedron(Of TRef)

        Dim below = Plane3DHelper.CreatePointNormal(
            Point3DHelper.Create(0, 0, zBelow), Vector3D.AlongZ, polygon.References)
        Dim above = Plane3DHelper.CreatePointNormal(
            Point3DHelper.Create(0, 0, zAbove), Vector3D.AlongZ, Enumerable.Empty(Of TRef)())

        Return PolyhedronHelper.CreateFromBorders(
            (From s In polygon.Sides Select ToSide(s)
            ).Concat({
                Plane3DHelper.CreateBorder(Of TRef)(below, 1, {}),
                Plane3DHelper.CreateBorder(Of TRef)(above, -1, {})
            }).ToList()
        )
    End Function


    ''' <summary>
    ''' Convert a polygon side into a polyhedron side by extending it in Z dimension.
    ''' </summary>
    ''' <param name="segment"></param>
    ''' <returns></returns>
    Private Function ToSide(segment As Polygon2DSide(Of TRef)) As Border3D(Of TRef)
        Dim pt2d = segment.Line.GetPoint(0)
        Dim vec2d = segment.Line.Vector.Perpendicular
        Dim plane = Plane3DHelper.CreatePointNormal(
            Point3DHelper.Create(pt2d.X, pt2d.Y, 0),
            New Vector3D(vec2d.X, vec2d.Y, 0),
            segment.References)

        Return Plane3DHelper.CreateBorder(Of TRef)(
            plane, If(segment.InsideDirection = Line2DDirections.Left, -1, 1), {})
    End Function

#End Region

End Class
