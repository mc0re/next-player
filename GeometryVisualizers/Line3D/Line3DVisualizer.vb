Imports System.Windows.Forms
Imports Common
Imports Microsoft.VisualStudio.DebuggerVisualizers


<Assembly: DebuggerVisualizer(GetType(Line3DVisualizer), GetType(VisualizerObjectSource), Target:=GetType(Line3D(Of)), Description:="Line3D visualizer")>
Public Class Line3DVisualizer
    Inherits DialogDebuggerVisualizer

    Protected Overrides Sub Show(windowService As IDialogVisualizerService, objectProvider As IVisualizerObjectProvider)
        Dim lineGeneric = TryCast(objectProvider.GetObject(), ILine3D)
        If lineGeneric Is Nothing Then
            MessageBox.Show($"Expected {NameOf(ILine3D)}, got {objectProvider.GetObject().GetType().Name}")
            Return
        End If

        Dim pointType = lineGeneric.GetType().GetGenericArguments().FirstOrDefault()

        Dim wnd As New Line3DVisualizerWindow()
        wnd.VisualizerControl.Line = lineGeneric
        wnd.VisualizerControl.PointType = pointType
        wnd.ShowDialog()
    End Sub

End Class
