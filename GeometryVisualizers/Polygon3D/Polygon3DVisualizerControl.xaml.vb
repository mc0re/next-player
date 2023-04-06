Imports System.Windows
Imports System.Windows.Controls
Imports Common
Imports DrawingLibrary
Imports HelixToolkit.Wpf


Public Class Polygon3DVisualizerControl
    Inherits UserControl

#Region " Polygon dependency property "

    Public Shared ReadOnly PolygonProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(Polygon), GetType(IPolygon3D), GetType(Polygon3DVisualizerControl))


    Public Property Polygon As IPolygon3D
        Get
            Return CType(GetValue(PolygonProperty), IPolygon3D)
        End Get
        Set(value As IPolygon3D)
            SetValue(PolygonProperty, value)
        End Set
    End Property

#End Region


#Region " Event listeners "

    Private Sub LoadHandler() Handles Me.Loaded
        ShowPolygon()
    End Sub

#End Region


#Region " Generate the models "

    Private Sub ShowPolygon()
        Dim vp = Utility.CreateViewPort(Me)

        Dim points = Polygon.Vertices.ToArray()

        ' ReSharper disable once ImpureMethodCallOnReadonlyValueField
        ' ChangeAlpha is a pure method
        vp.Children.Add(Build2DFaceVisual(
            Constants.DrawingColor.ChangeAlpha(Constants.FaceOpacity),
            points))

        ' To allow tooltips on the vertices
        For Each p In points
            vp.Children.Add(BuildPointVisual(p, Constants.DrawingColor))
        Next
    End Sub

#End Region

End Class
