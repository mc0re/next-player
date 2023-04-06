Imports System.Windows.Forms
Imports Common
Imports Microsoft.VisualStudio.DebuggerVisualizers


<Assembly: DebuggerVisualizer(GetType(Polygon3DVisualizer), GetType(VisualizerObjectSource), Target:=GetType(Polygon3D(Of)), Description:="Polygon3D visualizer")>
Public Class Polygon3DVisualizer
    Inherits DialogDebuggerVisualizer

    Protected Overrides Sub Show(windowService As IDialogVisualizerService, objectProvider As IVisualizerObjectProvider)
        Dim poly = TryCast(objectProvider.GetObject(), IPolygon3D)
        If poly Is Nothing Then
            MessageBox.Show($"Expected {NameOf(IPolygon3D)}, got {objectProvider.GetObject().GetType().Name}")
            Return
        End If

        Dim wnd As New Polygon3DVisualizerWindow()
        wnd.VisualizerControl.Polygon = poly
        wnd.ShowDialog()
    End Sub

End Class
