Imports System.Windows
Imports System.Windows.Controls
Imports Common
Imports DrawingLibrary


Public Class Plane3DVisualizerControl
    Inherits UserControl

#Region " Plane dependency property "

    Public Shared ReadOnly PlaneProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(Plane), GetType(IPlane3D), GetType(Plane3DVisualizerControl))


    Public Property Plane As IPlane3D
        Get
            Return CType(GetValue(PlaneProperty), IPlane3D)
        End Get
        Set(value As IPlane3D)
            SetValue(PlaneProperty, value)
        End Set
    End Property

#End Region


#Region " PointType dependency property "

    Public Shared ReadOnly PointTypeProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(PointType), GetType(Type), GetType(Plane3DVisualizerControl))


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
        ShowLine()
    End Sub

#End Region


#Region " Generate the models "

    Private Sub ShowLine()
        Dim vp = Utility.CreateViewPort(Me)

        ' Get points on the plane
        Utility.CreatePlane(vp, Plane)

        ' Show the normal
        vp.Children.Add(BuildVectorVisual(
            Plane.Point, Plane.Normal, Constants.DrawingColor, Plane.Normal.ToString()))

        ' Show the point
        vp.Children.Add(BuildPointVisual(Plane.Point, Constants.DrawingColor))
    End Sub

#End Region

End Class
