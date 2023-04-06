Imports System.Windows.Media
Imports System.Windows.Media.Media3D
Imports Common
Imports HelixToolkit.Wpf


Public Module ModelBuilder

#Region " Constants "

    ''' <summary>
    ''' Radius of a sphere representing an output channel.
    ''' </summary>
    Private Const ChannelSphereRadius = 0.05


    ''' <summary>
    ''' Audience box size where the actual size is 0.
    ''' </summary>
    Private Const ZeroSize = 0.01


    ''' <summary>
    ''' Thickness of the mesh lines (in pixels).
    ''' </summary>
    Private Const LineThickness = 2


    ''' <summary>
    ''' Transparency of the selected polygon's face.
    ''' </summary>
    Private Const PolygonFaceAlpha = 0.3

#End Region


#Region " Visuals API "

    ''' <summary>
    ''' Create a set of lights.
    ''' </summary>
    ''' <remarks>
    ''' For now simply uses <see cref="DefaultLights"/>.
    ''' </remarks>
    Public Function CreateLight() As Visual3D
        Return New DefaultLights()
    End Function


    ''' <summary>
    ''' Create a 3D box.
    ''' </summary>
    Public Function BuildParallelepipedMeshVisual(room As Room3D, edgeColor As Color) As Visual3D
        Dim c As New RoomDimensions(room)
        Dim p000 As New Media3D.Point3D(c.RoomXNeg, c.RoomYNeg, c.RoomZNeg)
        Dim p001 As New Media3D.Point3D(c.RoomXNeg, c.RoomYNeg, c.RoomZPos)
        Dim p010 As New Media3D.Point3D(c.RoomXNeg, c.RoomYPos, c.RoomZNeg)
        Dim p011 As New Media3D.Point3D(c.RoomXNeg, c.RoomYPos, c.RoomZPos)
        Dim p100 As New Media3D.Point3D(c.RoomXPos, c.RoomYNeg, c.RoomZNeg)
        Dim p101 As New Media3D.Point3D(c.RoomXPos, c.RoomYNeg, c.RoomZPos)
        Dim p110 As New Media3D.Point3D(c.RoomXPos, c.RoomYPos, c.RoomZNeg)
        Dim p111 As New Media3D.Point3D(c.RoomXPos, c.RoomYPos, c.RoomZPos)

        Dim roomPts As New Point3DCollection()
        AddLine(roomPts, p000, p001, p011, p010, p000, p100, p101, p111, p110, p100)
        AddLine(roomPts, p001, p101)
        AddLine(roomPts, p010, p110)
        AddLine(roomPts, p011, p111)

        Return New LinesVisual3D With {
            .Points = roomPts,
            .Color = edgeColor,
            .Thickness = LineThickness
            }
    End Function


    ''' <summary>
    ''' Create the audience plane as a rectangle.
    ''' </summary>
    ''' <remarks>
    ''' If one or both dimensions are 0, the rectanle's size in this dimensions is set to <see cref="ZeroSize"/>.
    ''' </remarks>
    Public Function BuildAudienceVisual(room As Room3D, faceColor As Color) As Visual3D
        Dim c As New RoomDimensions(room)
        Dim faceMaterial As New DiffuseMaterial(New SolidColorBrush(faceColor))

        Dim xs = Math.Max(c.AudXSize, ZeroSize)
        Dim ys = Math.Max(c.AudYSize, ZeroSize)

        Return New BoxVisual3D With {
            .Width = ys,
            .Length = xs,
            .Height = ZeroSize,
            .Center = New Media3D.Point3D(c.AudXNeg + xs / 2, c.AudYNeg + ys / 2, 0),
            .Material = faceMaterial
            }
    End Function


    ''' <summary>
    ''' Create a 3D model for a list of polyhedrons.
    ''' </summary>
    ''' <param name="polys">A list of polyhedrons to show</param>
    ''' <param name="edgeColor">Color of the polyhedron edges</param>
    Public Function BuildPolyhedronListMeshVisual(
        polys As IEnumerable(Of IPolyhedron), edgeColor As Color
    ) As Visual3D
        Dim models As New ModelVisual3D()

        For Each poly In polys
            models.Children.Add(BuildPolyhedronMeshVisual(poly, edgeColor))
        Next

        Return models
    End Function


    ''' <summary>
    ''' Build a mesh model for a single polyhedron.
    ''' </summary>
    Public Function BuildPolyhedronMeshVisual(
        poly As IPolyhedron, edgeColor As Color
    ) As Visual3D
        Dim edgePts As New Point3DCollection()

        For Each polygon In poly.Sides
            For Each side In polygon.Polygon.Sides
                edgePts.Add(MakePoint(side.PointA))
                edgePts.Add(MakePoint(side.PointB))
            Next
        Next

        Return New LinesVisual3D With {
            .Points = edgePts, .Color = edgeColor
        }
    End Function


    ''' <summary>
    ''' Build a face model for a single polyhedron.
    ''' </summary>
    Public Function BuildPolyhedronFaceVisual(
        poly As IPolyhedron, faceColor As Color
    ) As Visual3D
        Dim mat As New DiffuseMaterial(
            New SolidColorBrush(faceColor) With {.Opacity = PolygonFaceAlpha})

        Dim trPts As New List(Of Media3D.Point3D)()

        For Each polygon In poly.Sides
            Dim p0 = MakePoint(polygon.Polygon.Sides.First().PointA)

            For Each side In polygon.Polygon.Sides
                trPts.Add(p0)
                trPts.Add(MakePoint(side.PointA))
                trPts.Add(MakePoint(side.PointB))
            Next
        Next

        Dim res = New MeshVisual3D With {
            .Mesh = New Mesh3D(trPts, Enumerable.Range(0, trPts.Count)),
            .SharedVertices = True,
            .FaceMaterial = mat,
            .FaceBackMaterial = mat,
            .EdgeDiameter = 0,
            .VertexRadius = 0
        }

        ' Force mesh update to propagate materials
        res.SharedVertices = False

        Return res
    End Function


    ''' <summary>
    ''' Create a point as a 3D sphere.
    ''' </summary>
    Public Function BuildPointVisual(
        c As IPoint3D, color As Color, Optional scale As Double = 1.0
    ) As Visual3D
        Dim toolTip As New ToolTipHelper With {.ToolTipContent = c.ToString()}

        Dim model As New SphereVisual3D With {
            .Center = MakePoint(c),
            .Radius = ChannelSphereRadius * scale,
            .Material = New DiffuseMaterial(New SolidColorBrush(color))
        }

        Dim ui As New ModelUIElement3D With {.Model = model.Model}

        AddHandler ui.MouseEnter, AddressOf toolTip.OnMouseEnter
        AddHandler ui.MouseLeave, AddressOf toolTip.OnMouseLeave

        Return ui
    End Function


    ''' <summary>
    ''' Create a point as a 3D sphere.
    ''' </summary>
    Public Function BuildSegmentVisual(
        p1 As IPoint3D, p2 As IPoint3D, color As Color
    ) As Visual3D
        Dim toolTip As New ToolTipHelper With {
            .ToolTipContent = p1.ToString() & " - " & p2.ToString()
        }

        Dim ui As New ModelUIElement3D With {
            .Model = BuildSegmentModel(p1, p2, color)
        }

        AddHandler ui.MouseEnter, AddressOf toolTip.OnMouseEnter
        AddHandler ui.MouseLeave, AddressOf toolTip.OnMouseLeave

        Return ui
    End Function


    ''' <summary>
    ''' Create a point as a 3D sphere.
    ''' </summary>
    Public Function BuildVectorVisual(
        start As IPoint3D, v As Common.Vector3D, color As Color,
        toolTipText As String
    ) As Visual3D
        Dim toolTip As New ToolTipHelper With {
            .ToolTipContent = toolTipText
        }

        Dim ptMaterial As New DiffuseMaterial(New SolidColorBrush(color))

        Dim startModel As New SphereVisual3D With {
                        .Center = MakePoint(start),
                        .Radius = ChannelSphereRadius,
                        .Material = ptMaterial
                    }

        Dim vecModel As New ArrowVisual3D With {
            .Point1 = MakePoint(start),
            .Point2 = MakePoint(v.From(start)),
            .Diameter = ChannelSphereRadius,
            .HeadLength = 7,
            .Material = ptMaterial
        }

        Dim model As New Model3DGroup()
        model.Children.Add(startModel.Model)
        model.Children.Add(vecModel.Model)

        Dim ui As New ModelUIElement3D With {.Model = model}

        AddHandler ui.MouseEnter, AddressOf toolTip.OnMouseEnter
        AddHandler ui.MouseLeave, AddressOf toolTip.OnMouseLeave

        Return ui
    End Function


    ''' <summary>
    ''' Create a plane face for a rectangle.
    ''' </summary>
    Public Function BuildRectangleFaceVisual(
        p1 As IPoint3D, p2 As IPoint3D, p3 As IPoint3D, p4 As IPoint3D,
        faceColor As Color, toolTipText As String
    ) As Visual3D
        Dim toolTip As New ToolTipHelper With {
            .ToolTipContent = toolTipText
        }

        Dim faceMaterial As New DiffuseMaterial(New SolidColorBrush(faceColor))

        Dim model As New QuadVisual3D With {
            .Point1 = MakePoint(p1),
            .Point2 = MakePoint(p2),
            .Point3 = MakePoint(p3),
            .Point4 = MakePoint(p4),
            .Material = faceMaterial
        }

        Dim ui As New ModelUIElement3D With {.Model = model.Content}

        AddHandler ui.MouseEnter, AddressOf toolTip.OnMouseEnter
        AddHandler ui.MouseLeave, AddressOf toolTip.OnMouseLeave

        Return ui
    End Function


    ''' <summary>
    ''' Create a face for a polygon and border.
    ''' </summary>
    Public Function Build2DFaceVisual(faceColor As Color, ParamArray points() As IPoint3D) As Visual3D
        Dim toolTip As New ToolTipHelper With {
            .ToolTipContent = String.Join(" - ", From p In points Let s = p.ToString() Select s)
        }

        Dim model As New Model3DGroup()
        Dim faceMaterial As New DiffuseMaterial(New SolidColorBrush(faceColor))
        Dim helixPoints = (From p In points Select MakePoint(p)).ToList()

        If points.Length > 2 Then
            Dim triangles = Enumerable.Range(1, points.Length - 2).
                SelectMany(Function(idx, l) {0, idx, idx + 1}).
                ToList()

            Dim face As New MeshVisual3D With {
                .Mesh = New Mesh3D(helixPoints, triangles),
                .FaceMaterial = faceMaterial,
                .FaceBackMaterial = faceMaterial,
                .EdgeDiameter = 0,
                .VertexRadius = 0
            }

            model.Children.Add(face.Content)
        End If

        For i = 0 To points.Length - 1
            Dim iNext = If(i = points.Length - 1, 0, i)
            model.Children.Add(
                BuildSegmentModel(points(i), points(iNext), faceColor))
        Next

        Dim ui As New ModelUIElement3D With {.Model = model}

        AddHandler ui.MouseEnter, AddressOf toolTip.OnMouseEnter
        AddHandler ui.MouseLeave, AddressOf toolTip.OnMouseLeave

        Return ui
    End Function

#End Region


#Region " Utility "

    ''' <summary>
    ''' Add line segments (start-end).
    ''' </summary>
    Private Sub AddLine(coll As Point3DCollection, ParamArray points As Media3D.Point3D())
        For i = 0 To points.Count - 2
            coll.Add(points(i))
            coll.Add(points(i + 1))
        Next
    End Sub


    Public Function MakePoint(c As IPoint3D) As Media3D.Point3D
        Return New Media3D.Point3D(c.X, c.Y, c.Z)
    End Function


    Private Function BuildSegmentModel(p1 As IPoint3D, p2 As IPoint3D, color As Color) As Model3D
        Dim ptMaterial As New DiffuseMaterial(New SolidColorBrush(color))

        Dim model As New PipeVisual3D With {
            .Point1 = MakePoint(p1),
            .Point2 = MakePoint(p2),
            .Diameter = ChannelSphereRadius,
            .Material = ptMaterial
        }

        Return model.Content
    End Function

#End Region

End Module
