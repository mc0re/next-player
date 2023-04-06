Imports System.Windows.Controls
Imports HelixToolkit.Wpf
Imports Common
Imports DrawingLibrary
Imports WpfResources


Public Class Utility

    ''' <summary>
    ''' Create a standard viewport.
    ''' Expects an element named "PartView3D".
    ''' </summary>
    Public Shared Function CreateViewPort(ctrl As Control) As HelixViewport3D
        Dim vp = CType(ctrl.FindName("PartView3D"), HelixViewport3D)

        Dim room As New Room3D()
        room.SetAllSides(1)
        vp.Camera = HelixDrawer.GenerateCamera(vp, room, Projections.ThreeD)

        vp.Children.Clear()
        vp.Children.Add(CreateLight())
        vp.Children.Add(BuildPointVisual(Point3DHelper.Origin, Constants.OriginColor))

        Return vp
    End Function


    ''' <summary>
    ''' Create a visualization of the given plane as a square.
    ''' </summary>
    ''' <param name="vp">Viewport to visualize in</param>
    ''' <param name="plane">Plane to visualize</param>
    Public Shared Sub CreatePlane(vp As HelixViewport3D, plane As IPlane3D)
        Dim p1 = plane.GetPoint(-1, -1)
        Dim p2 = plane.GetPoint(1, -1)
        Dim p3 = plane.GetPoint(1, 1)
        Dim p4 = plane.GetPoint(-1, 1)

        ' ReSharper disable once ImpureMethodCallOnReadonlyValueField
        ' ChangeAlpha is a pure method
        vp.Children.Add(
            BuildRectangleFaceVisual(
                p1, p2, p3, p4,
                Constants.DrawingColor.ChangeAlpha(Constants.FaceOpacity), plane.ToString()))

        vp.Children.Add(BuildSegmentVisual(plane.GetPoint(-1, -1),
                                          plane.GetPoint(-Constants.Infinity, -Constants.Infinity),
                                          Constants.DrawingColor))
        vp.Children.Add(BuildSegmentVisual(plane.GetPoint(1, -1),
                                          plane.GetPoint(Constants.Infinity, -Constants.Infinity),
                                          Constants.DrawingColor))
        vp.Children.Add(BuildSegmentVisual(plane.GetPoint(1, 1),
                                          plane.GetPoint(Constants.Infinity, Constants.Infinity),
                                          Constants.DrawingColor))
        vp.Children.Add(BuildSegmentVisual(plane.GetPoint(-1, 1),
                                          plane.GetPoint(-Constants.Infinity, Constants.Infinity),
                                          Constants.DrawingColor))
    End Sub

End Class
