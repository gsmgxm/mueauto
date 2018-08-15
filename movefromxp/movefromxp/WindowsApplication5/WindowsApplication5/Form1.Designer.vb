<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Me.SerialPort = New System.IO.Ports.SerialPort(Me.components)
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.Timer3 = New System.Windows.Forms.Timer(Me.components)
        Me.PerformanceCounter1 = New System.Diagnostics.PerformanceCounter
        Me.Timer2 = New System.Timers.Timer
        Me.Timer4 = New System.Windows.Forms.Timer(Me.components)
        CType(Me.PerformanceCounter1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.Timer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'SerialPort
        '
        Me.SerialPort.DtrEnable = True
        Me.SerialPort.RtsEnable = True
        '
        'Timer1
        '
        Me.Timer1.Interval = 3000
        '
        'Timer3
        '
        Me.Timer3.Interval = 1000
        '
        'PerformanceCounter1
        '
        Me.PerformanceCounter1.CategoryName = "RAS Port"
        Me.PerformanceCounter1.CounterName = "Bytes Transmitted/Sec"
        Me.PerformanceCounter1.InstanceName = "COM8"
        '
        'Timer2
        '
        Me.Timer2.Interval = 2000
        Me.Timer2.SynchronizingObject = Nothing
        '
        'Timer4
        '
        Me.Timer4.Interval = 20000
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ControlText
        Me.ClientSize = New System.Drawing.Size(852, 304)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Form1"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "MUEclient:xcom"
        CType(Me.PerformanceCounter1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.Timer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents SerialPort As System.IO.Ports.SerialPort
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents Timer3 As System.Windows.Forms.Timer
    Friend WithEvents PerformanceCounter1 As System.Diagnostics.PerformanceCounter
    Friend WithEvents Timer2 As System.Timers.Timer
    Friend WithEvents Timer4 As System.Windows.Forms.Timer

End Class
