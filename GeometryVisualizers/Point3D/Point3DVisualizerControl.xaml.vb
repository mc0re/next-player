Imports System.Windows
Imports System.Windows.Controls
Imports Common
Imports DrawingLibrary
Imports HelixToolkit.Wpf
Imports WpfResources


Public Class Point3DVisualizerControl
    Inherits UserControl

#Region " Line dependency property "

    Public Shared ReadOnly PointProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(Point), GetType(IPoint3D), GetType(Point3DVisualizerControl))


    Public Property Point As IPoint3D
        Get
            Return CType(GetValue(PointProperty), IPoint3D)
        End Get
        Set(value As IPoint3D)
            SetValue(PointProperty, value)
        End Set
    End Property

#End Region


#Region " PointType dependency property "

    Public Shared ReadOnly PointTypeProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(PointType), GetType(Type), GetType(Point3DVisualizerControl))


    Public Property PointType As Type
        Get
            Return CType(GetValue(PointTypeProperty), Type)
        End Get
        Set(value As Type)
            SetValue(PointTypeProperty, value)
        End Set
    End Property

#End Region


#Region " Event listeners "

    Private Sub LoadHandler() Handles Me.Loaded
        ShowPoint()
    End Sub

#End Region


#Region " Generate the models "

    Private Sub ShowPoint()
        Dim vp = CType(FindName("PartView3D"), HelixViewport3D)
        Dim room As New Room3D()
        room.SetAllSides(1)
        vp.Camera = HelixDrawer.GenerateCamera(vp, room, Projections.ThreeD)
        vp.Children.Clear()
        vp.Children.Add(CreateLight())
        vp.Children.Add(BuildPointVisual(Point3DHelper.Origin, Constants.OriginColor))
        vp.Children.Add(BuildPointVisual(Point, Constants.DrawingColor))
    End Sub

#End Region

End Class
