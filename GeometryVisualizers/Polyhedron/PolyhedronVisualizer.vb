Imports System.Windows.Forms
Imports Common
Imports Microsoft.VisualStudio.DebuggerVisualizers


<Assembly: DebuggerVisualizer(GetType(PolyhedronVisualizer), GetType(VisualizerObjectSource), Target:=GetType(Polyhedron(Of)), Description:="Polyhedron visualizer")>
Public Class PolyhedronVisualizer
    Inherits DialogDebuggerVisualizer

    Protected Overrides Sub Show(windowService As IDialogVisualizerService, objectProvider As IVisualizerObjectProvider)
        Dim poly = TryCast(objectProvider.GetObject(), IPolyhedron)
        If poly Is Nothing Then
            MessageBox.Show($"Expected {NameOf(IPolyhedron)}, got {objectProvider.GetObject().GetType().Name}")
            Return
        End If

        Dim wnd As New PolyhedronVisualizerWindow()
        wnd.VisualizerControl.Polyhedron = poly
        wnd.ShowDialog()
    End Sub

End Class
