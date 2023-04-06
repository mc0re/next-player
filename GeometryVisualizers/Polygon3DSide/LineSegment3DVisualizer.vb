Imports System.Windows.Forms
Imports Common
Imports Microsoft.VisualStudio.DebuggerVisualizers


<Assembly: DebuggerVisualizer(GetType(LineSegment3DVisualizer), GetType(VisualizerObjectSource), Target:=GetType(LineSegment3D(Of)), Description:="LineSegment3D visualizer")>
Public Class LineSegment3DVisualizer
    Inherits DialogDebuggerVisualizer

    Protected Overrides Sub Show(windowService As IDialogVisualizerService, objectProvider As IVisualizerObjectProvider)
        Dim side = TryCast(objectProvider.GetObject(), ILineSegment3D)
        If side Is Nothing Then
            MessageBox.Show($"Expected {NameOf(ILineSegment3D)}, got {objectProvider.GetObject().GetType().Name}")
            Return
        End If

        Dim wnd As New LineSegment3DVisualizerWindow()
        wnd.VisualizerControl.Side = side
        wnd.ShowDialog()
    End Sub

End Class
