<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Polygon3DVisualizerWindow
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.Host = New System.Windows.Forms.Integration.ElementHost()
        Me.VisualizerControl = New GeometryVisualizers.Polygon3DVisualizerControl()
        Me.SuspendLayout()
        '
        'Host
        '
        Me.Host.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Host.Location = New System.Drawing.Point(0, 0)
        Me.Host.Margin = New System.Windows.Forms.Padding(0)
        Me.Host.Name = "Host"
        Me.Host.Size = New System.Drawing.Size(500, 500)
        Me.Host.TabIndex = 0
        Me.Host.Text = "ElementHost"
        Me.Host.Child = Me.VisualizerControl
        '
        'Polygon3DVisualizerWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(500, 500)
        Me.Controls.Add(Me.Host)
        Me.Name = "Polygon3DVisualizerWindow"
        Me.Text = "Polygon3D"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Host As Windows.Forms.Integration.ElementHost
    Friend VisualizerControl As Polygon3DVisualizerControl
End Class
