Imports System.Windows
Imports System.Windows.Controls
Imports Common
Imports DrawingLibrary


Public Class Line3DVisualizerControl
    Inherits UserControl

#Region " Line dependency property "


    Public Shared ReadOnly LineProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(Line), GetType(ILine3D), GetType(Line3DVisualizerControl))


    Public Property Line As ILine3D
        Get
            Return CType(GetValue(LineProperty), ILine3D)
        End Get
        Set(value As ILine3D)
            SetValue(LineProperty, value)
        End Set
    End Property


#End Region


#Region " PointType dependency property "


    Public Shared ReadOnly PointTypeProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(PointType), GetType(Type), GetType(Line3DVisualizerControl))


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

        vp.Children.Add(
            BuildVectorVisual(
                Line.Point, Line.Vector, Constants.DrawingColor, Line.ToString()))

        vp.Children.Add(BuildSegmentVisual(Line.Point,
                                          Line.GetPoint(-Constants.Infinity),
                                          Constants.DrawingColor))
        vp.Children.Add(BuildSegmentVisual(Line.GetPoint(1),
                                          Line.GetPoint(Constants.Infinity),
                                          Constants.DrawingColor))
    End Sub

#End Region

End Class
