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
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.D1 = New System.Windows.Forms.Label
        Me.D2 = New System.Windows.Forms.Label
        Me.D3 = New System.Windows.Forms.Label
        Me.d4 = New System.Windows.Forms.Label
        Me.a4 = New System.Windows.Forms.Label
        Me.a3 = New System.Windows.Forms.Label
        Me.a2 = New System.Windows.Forms.Label
        Me.a1 = New System.Windows.Forms.Label
        Me.dtime = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'Timer1
        '
        Me.Timer1.Interval = 1000
        '
        'D1
        '
        Me.D1.AutoSize = True
        Me.D1.Location = New System.Drawing.Point(23, 9)
        Me.D1.Name = "D1"
        Me.D1.Size = New System.Drawing.Size(131, 12)
        Me.D1.TabIndex = 0
        Me.D1.Text = "Download file 1 name:"
        '
        'D2
        '
        Me.D2.AutoSize = True
        Me.D2.Location = New System.Drawing.Point(23, 43)
        Me.D2.Name = "D2"
        Me.D2.Size = New System.Drawing.Size(131, 12)
        Me.D2.TabIndex = 1
        Me.D2.Text = "Download file 2 name:"
        '
        'D3
        '
        Me.D3.AutoSize = True
        Me.D3.Location = New System.Drawing.Point(23, 77)
        Me.D3.Name = "D3"
        Me.D3.Size = New System.Drawing.Size(131, 12)
        Me.D3.TabIndex = 2
        Me.D3.Text = "Download file 3 name:"
        '
        'd4
        '
        Me.d4.AutoSize = True
        Me.d4.Location = New System.Drawing.Point(23, 111)
        Me.d4.Name = "d4"
        Me.d4.Size = New System.Drawing.Size(131, 12)
        Me.d4.TabIndex = 3
        Me.d4.Text = "Download file 4 name:"
        '
        'a4
        '
        Me.a4.AutoSize = True
        Me.a4.Location = New System.Drawing.Point(23, 128)
        Me.a4.Name = "a4"
        Me.a4.Size = New System.Drawing.Size(137, 12)
        Me.a4.TabIndex = 5
        Me.a4.Text = "Average download time:"
        '
        'a3
        '
        Me.a3.AutoSize = True
        Me.a3.Location = New System.Drawing.Point(23, 94)
        Me.a3.Name = "a3"
        Me.a3.Size = New System.Drawing.Size(137, 12)
        Me.a3.TabIndex = 6
        Me.a3.Text = "Average download time:"
        '
        'a2
        '
        Me.a2.AutoSize = True
        Me.a2.Location = New System.Drawing.Point(23, 60)
        Me.a2.Name = "a2"
        Me.a2.Size = New System.Drawing.Size(137, 12)
        Me.a2.TabIndex = 7
        Me.a2.Text = "Average download time:"
        '
        'a1
        '
        Me.a1.AutoSize = True
        Me.a1.Location = New System.Drawing.Point(23, 26)
        Me.a1.Name = "a1"
        Me.a1.Size = New System.Drawing.Size(137, 12)
        Me.a1.TabIndex = 8
        Me.a1.Text = "Average download time:"
        '
        'dtime
        '
        Me.dtime.AutoSize = True
        Me.dtime.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.dtime.Location = New System.Drawing.Point(0, 164)
        Me.dtime.Name = "dtime"
        Me.dtime.Size = New System.Drawing.Size(95, 12)
        Me.dtime.TabIndex = 9
        Me.dtime.Text = "Download times:"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(687, 176)
        Me.Controls.Add(Me.dtime)
        Me.Controls.Add(Me.a1)
        Me.Controls.Add(Me.a2)
        Me.Controls.Add(Me.a3)
        Me.Controls.Add(Me.a4)
        Me.Controls.Add(Me.d4)
        Me.Controls.Add(Me.D3)
        Me.Controls.Add(Me.D2)
        Me.Controls.Add(Me.D1)
        Me.Name = "Form1"
        Me.Text = "Http download"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents D1 As System.Windows.Forms.Label
    Friend WithEvents D2 As System.Windows.Forms.Label
    Friend WithEvents D3 As System.Windows.Forms.Label
    Friend WithEvents d4 As System.Windows.Forms.Label
    Friend WithEvents a4 As System.Windows.Forms.Label
    Friend WithEvents a3 As System.Windows.Forms.Label
    Friend WithEvents a2 As System.Windows.Forms.Label
    Friend WithEvents a1 As System.Windows.Forms.Label
    Friend WithEvents dtime As System.Windows.Forms.Label

End Class
