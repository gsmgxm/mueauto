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
        Me.Button1 = New System.Windows.Forms.Button
        Me.ueip = New System.Windows.Forms.TextBox
        Me.hrpingdir = New System.Windows.Forms.TextBox
        Me.QCI5ip = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.QCI1ip = New System.Windows.Forms.TextBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.QCI5size = New System.Windows.Forms.TextBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.QCI5interval = New System.Windows.Forms.TextBox
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label10 = New System.Windows.Forms.Label
        Me.QCI1size = New System.Windows.Forms.TextBox
        Me.Label11 = New System.Windows.Forms.Label
        Me.Label12 = New System.Windows.Forms.Label
        Me.QCI1interval = New System.Windows.Forms.TextBox
        Me.QCI5speed = New System.Windows.Forms.Label
        Me.QCI1speed = New System.Windows.Forms.Label
        Me.Button2 = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(264, 6)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "Start"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'ueip
        '
        Me.ueip.Location = New System.Drawing.Point(123, 13)
        Me.ueip.Name = "ueip"
        Me.ueip.Size = New System.Drawing.Size(100, 20)
        Me.ueip.TabIndex = 1
        Me.ueip.Text = "172.24.139.44"
        '
        'hrpingdir
        '
        Me.hrpingdir.Location = New System.Drawing.Point(123, 149)
        Me.hrpingdir.Name = "hrpingdir"
        Me.hrpingdir.Size = New System.Drawing.Size(100, 20)
        Me.hrpingdir.TabIndex = 3
        Me.hrpingdir.Text = "d:\mueauto"
        '
        'QCI5ip
        '
        Me.QCI5ip.Location = New System.Drawing.Point(123, 39)
        Me.QCI5ip.Name = "QCI5ip"
        Me.QCI5ip.Size = New System.Drawing.Size(100, 20)
        Me.QCI5ip.TabIndex = 4
        Me.QCI5ip.Text = "172.24.186.199"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(4, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(36, 13)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = "UE ip:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 156)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(60, 13)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "HRping dir:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(4, 42)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(108, 13)
        Me.Label4.TabIndex = 8
        Me.Label4.Text = "Appserverip for QCI5:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(4, 94)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(108, 13)
        Me.Label2.TabIndex = 10
        Me.Label2.Text = "Appserverip for QCI1:"
        '
        'QCI1ip
        '
        Me.QCI1ip.Location = New System.Drawing.Point(123, 94)
        Me.QCI1ip.Name = "QCI1ip"
        Me.QCI1ip.Size = New System.Drawing.Size(100, 20)
        Me.QCI1ip.TabIndex = 9
        Me.QCI1ip.Text = "172.24.186.199"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(261, 45)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(100, 13)
        Me.Label5.TabIndex = 12
        Me.Label5.Text = "QCI5 package size:"
        '
        'QCI5size
        '
        Me.QCI5size.Location = New System.Drawing.Point(372, 42)
        Me.QCI5size.Name = "QCI5size"
        Me.QCI5size.Size = New System.Drawing.Size(30, 20)
        Me.QCI5size.TabIndex = 11
        Me.QCI5size.Text = "500"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(4, 67)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(107, 13)
        Me.Label7.TabIndex = 16
        Me.Label7.Text = "QCI5 packet interval:"
        '
        'QCI5interval
        '
        Me.QCI5interval.Location = New System.Drawing.Point(123, 64)
        Me.QCI5interval.Name = "QCI5interval"
        Me.QCI5interval.Size = New System.Drawing.Size(48, 20)
        Me.QCI5interval.TabIndex = 15
        Me.QCI5interval.Text = "200"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(408, 46)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(27, 13)
        Me.Label8.TabIndex = 17
        Me.Label8.Text = "byte"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(177, 67)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(20, 13)
        Me.Label9.TabIndex = 18
        Me.Label9.Text = "ms"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(408, 101)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(27, 13)
        Me.Label6.TabIndex = 21
        Me.Label6.Text = "byte"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(261, 100)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(100, 13)
        Me.Label10.TabIndex = 20
        Me.Label10.Text = "QCI1 package size:"
        '
        'QCI1size
        '
        Me.QCI1size.Location = New System.Drawing.Point(372, 97)
        Me.QCI1size.Name = "QCI1size"
        Me.QCI1size.Size = New System.Drawing.Size(30, 20)
        Me.QCI1size.TabIndex = 19
        Me.QCI1size.Text = "100"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(177, 117)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(20, 13)
        Me.Label11.TabIndex = 24
        Me.Label11.Text = "ms"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(4, 117)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(107, 13)
        Me.Label12.TabIndex = 23
        Me.Label12.Text = "QCI1 packet interval:"
        '
        'QCI1interval
        '
        Me.QCI1interval.Location = New System.Drawing.Point(123, 114)
        Me.QCI1interval.Name = "QCI1interval"
        Me.QCI1interval.Size = New System.Drawing.Size(48, 20)
        Me.QCI1interval.TabIndex = 22
        Me.QCI1interval.Text = "20"
        '
        'QCI5speed
        '
        Me.QCI5speed.AutoSize = True
        Me.QCI5speed.Location = New System.Drawing.Point(261, 71)
        Me.QCI5speed.Name = "QCI5speed"
        Me.QCI5speed.Size = New System.Drawing.Size(63, 13)
        Me.QCI5speed.TabIndex = 25
        Me.QCI5speed.Text = "QCI5 speed"
        '
        'QCI1speed
        '
        Me.QCI1speed.AutoSize = True
        Me.QCI1speed.Location = New System.Drawing.Point(261, 121)
        Me.QCI1speed.Name = "QCI1speed"
        Me.QCI1speed.Size = New System.Drawing.Size(63, 13)
        Me.QCI1speed.TabIndex = 26
        Me.QCI1speed.Text = "QCI1 speed"
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(372, 6)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(83, 23)
        Me.Button2.TabIndex = 27
        Me.Button2.Text = "Show console"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(497, 187)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.QCI1speed)
        Me.Controls.Add(Me.QCI5speed)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.QCI1interval)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.QCI1size)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.QCI5interval)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.QCI5size)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.QCI1ip)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.QCI5ip)
        Me.Controls.Add(Me.hrpingdir)
        Me.Controls.Add(Me.ueip)
        Me.Controls.Add(Me.Button1)
        Me.Name = "Form1"
        Me.Text = "Volte packet loss test"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents ueip As System.Windows.Forms.TextBox
    Friend WithEvents hrpingdir As System.Windows.Forms.TextBox
    Friend WithEvents QCI5ip As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents QCI1ip As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents QCI5size As System.Windows.Forms.TextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents QCI5interval As System.Windows.Forms.TextBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents QCI1size As System.Windows.Forms.TextBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents QCI1interval As System.Windows.Forms.TextBox
    Friend WithEvents QCI5speed As System.Windows.Forms.Label
    Friend WithEvents QCI1speed As System.Windows.Forms.Label
    Friend WithEvents Button2 As System.Windows.Forms.Button

End Class
