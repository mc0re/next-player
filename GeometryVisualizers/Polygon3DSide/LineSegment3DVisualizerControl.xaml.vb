Imports System.Windows
Imports System.Windows.Controls
Imports Common
Imports DrawingLibrary


Public Class Polygon3DSideVisualizerControl
    Inherits UserControl

#Region " Side dependency property "

    Public Shared ReadOnly SideProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(Side), GetType(ILineSegment3D), GetType(Polygon3DSideVisualizerControl))


    Public Property Side As ILineSegment3D
        Get
            Return CType(GetValue(SideProperty), ILineSegment3D)
        End Get
        Set(value As ILineSegment3D)
            SetValue(SideProperty, value)
        End Set
    End Property

#End Region


#Region " Event listeners "

    Private Sub LoadHandler() Handles Me.Loaded
        ShowSide()
    End Sub

#End Region


#Region " Generate the models "

    Private Sub ShowSide()
        Dim vp = Utility.CreateViewPort(Me)

        vp.Children.Add(BuildSegmentVisual(Side.PointA,
                                          Side.PointB,
                                          Constants.DrawingColor))

        ' To allow tooltips on the vertices
        For Each p In {Side.PointA, Side.PointB}
            vp.Children.Add(BuildPointVisual(p, Constants.DrawingColor))
        Next
    End Sub

#End Region

End Class
