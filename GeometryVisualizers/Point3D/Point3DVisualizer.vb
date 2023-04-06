Imports Common
Imports Microsoft.VisualStudio.DebuggerVisualizers


<Assembly: DebuggerVisualizer(GetType(Point3DVisualizer), GetType(VisualizerObjectSource), Target:=GetType(Point3D(Of)), Description:="Point3D visualizer")>
Public Class Point3DVisualizer
    Inherits DialogDebuggerVisualizer

    Protected Overrides Sub Show(windowService As IDialogVisualizerService, objectProvider As IVisualizerObjectProvider)
        Dim pt = TryCast(objectProvider.GetObject(), IPoint3D)
        Dim pointType = pt.GetType()

        Dim wnd = New Point3DVisualizerWindow()
        wnd.VisualizerControl.Point = pt
        wnd.VisualizerControl.PointType = pointType
        wnd.ShowDialog()
    End Sub

End Class
