Imports System.Windows
Imports System.Windows.Controls
Imports Common
Imports DrawingLibrary


Public Class PolyhedronSideVisualizerControl
    Inherits UserControl

#Region " Side dependency property "

    Public Shared ReadOnly SideProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(Side), GetType(IPolyhedronSide), GetType(PolyhedronSideVisualizerControl))


    Public Property Side As IPolyhedronSide
        Get
            Return CType(GetValue(SideProperty), IPolyhedronSide)
        End Get
        Set(value As IPolyhedronSide)
            SetValue(SideProperty, value)
        End Set
    End Property

#End Region


#Region " Event listeners "

    Private Sub LoadHandler() Handles Me.Loaded
        ShowPolyhedronSide()
    End Sub

#End Region


#Region " Generate the models "

    Private Sub ShowPolyhedronSide()
        Dim vp = Utility.CreateViewPort(Me)

        Utility.CreatePlane(vp, Side.Polygon.Plane)

        ' Show inside direction
        Dim inside = If(Side.Inside > 0, Side.Polygon.Plane.Normal, Side.Polygon.Plane.Normal.Negate)
        vp.Children.Add(BuildVectorVisual(
            Side.Polygon.Plane.Point, inside, Constants.DrawingColor,
            Side.Polygon.Plane.Normal.ToString() & If(Side.Inside > 0, "", " reverted")))
    End Sub

#End Region

End Class
