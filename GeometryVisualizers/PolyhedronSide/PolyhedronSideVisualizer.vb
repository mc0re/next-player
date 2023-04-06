Imports System.Windows.Forms
Imports Common
Imports Microsoft.VisualStudio.DebuggerVisualizers


<Assembly: DebuggerVisualizer(GetType(PolyhedronSideVisualizer), GetType(VisualizerObjectSource), Target:=GetType(PolyhedronSide(Of)), Description:="Polyhedron side visualizer")>
Public Class PolyhedronSideVisualizer
    Inherits DialogDebuggerVisualizer

    Protected Overrides Sub Show(windowService As IDialogVisualizerService, objectProvider As IVisualizerObjectProvider)
        Dim polySide = TryCast(objectProvider.GetObject(), IPolyhedronSide)
        If polySide Is Nothing Then
            MessageBox.Show($"Expected {NameOf(IPolyhedronSide)}, got {objectProvider.GetObject().GetType().Name}")
            Return
        End If

        Dim wnd As New PolyhedronSideVisualizerWindow()
        wnd.VisualizerControl.Side = polySide
        wnd.ShowDialog()
    End Sub

End Class
