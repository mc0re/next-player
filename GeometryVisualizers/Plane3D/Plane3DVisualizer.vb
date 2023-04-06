Imports System.Windows.Forms
Imports Common
Imports Microsoft.VisualStudio.DebuggerVisualizers


<Assembly: DebuggerVisualizer(GetType(Plane3DVisualizer), GetType(VisualizerObjectSource), Target:=GetType(Plane3D(Of)), Description:="Plane3D visualizer")>
Public Class Plane3DVisualizer
    Inherits DialogDebuggerVisualizer

    Protected Overrides Sub Show(windowService As IDialogVisualizerService, objectProvider As IVisualizerObjectProvider)
        Dim planeGeneric = TryCast(objectProvider.GetObject(), IPlane3D)
        If planeGeneric Is Nothing Then
            MessageBox.Show($"Expected {NameOf(IPlane3D)}, got {objectProvider.GetObject().GetType().Name}")
            Return
        End If

        Dim pointType = planeGeneric.GetType().GetGenericArguments().FirstOrDefault()

        Dim wnd As New Plane3DVisualizerWindow()
        wnd.VisualizerControl.Plane = planeGeneric
        wnd.VisualizerControl.PointType = pointType
        wnd.ShowDialog()
    End Sub

End Class
