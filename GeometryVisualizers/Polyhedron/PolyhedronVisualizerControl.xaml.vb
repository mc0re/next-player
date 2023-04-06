Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports Common
Imports DrawingLibrary


Public Class PolyhedronVisualizerControl
    Inherits UserControl

#Region " Polyhedron dependency property "

    Public Shared ReadOnly PolyhedronProperty As DependencyProperty = DependencyProperty.Register(
        NameOf(Polyhedron), GetType(IPolyhedron), GetType(PolyhedronVisualizerControl))


    Public Property Polyhedron As IPolyhedron
        Get
            Return CType(GetValue(PolyhedronProperty), IPolyhedron)
        End Get
        Set(value As Common.IPolyhedron)
            SetValue(PolyhedronProperty, value)
        End Set
    End Property

#End Region


#Region " Event listeners "

    Private Sub LoadHandler() Handles Me.Loaded
        ShowPolyhedron()
    End Sub

#End Region


#Region " Generate the models "

    Private Sub ShowPolyhedron()
        Dim vp = Utility.CreateViewPort(Me)
        Dim rnd As New Random()
        Dim points As IEnumerable(Of IPoint3D) = New List(Of IPoint3D)()

        For Each p In Polyhedron.Sides
            Dim pLocal = p
            Dim sidePoints = (From s In pLocal.Polygon.Sides Select s.PointA).ToArray()

            points = points.Union(sidePoints)

            Dim clr = Color.FromArgb(Constants.FaceOpacity,
                                     CByte(rnd.Next(192)),
                                     CByte(rnd.Next(192)),
                                     CByte(rnd.Next(192)))

            vp.Children.Add(Build2DFaceVisual(clr, sidePoints))
        Next

        ' To allow tooltips on the vertices
        For Each p In points
            vp.Children.Add(BuildPointVisual(p, Constants.DrawingColor))
        Next
    End Sub

#End Region

End Class
