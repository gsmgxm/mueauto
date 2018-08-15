<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class MDIForm1
#Region "Windows Form Designer generated code "
	<System.Diagnostics.DebuggerNonUserCode()> Public Sub New()
		MyBase.New()
		'This call is required by the Windows Form Designer.
		InitializeComponent()
	End Sub
	'Form overrides dispose to clean up the component list.
	<System.Diagnostics.DebuggerNonUserCode()> Protected Overloads Overrides Sub Dispose(ByVal Disposing As Boolean)
		If Disposing Then
			If Not components Is Nothing Then
				components.Dispose()
			End If
		End If
		MyBase.Dispose(Disposing)
	End Sub
	'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer
    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.config = New System.Windows.Forms.ToolStripMenuItem
        Me.Run = New System.Windows.Forms.ToolStripMenuItem
        Me.monitor1 = New System.Windows.Forms.ToolStripMenuItem
        Me.OptionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.AutobackupToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.TPinterval = New System.Windows.Forms.ToolStripComboBox
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator
        Me.groupmode = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator
        Me.Remotestting = New System.Windows.Forms.ToolStripMenuItem
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator
        Me.ConsoleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MainMenu1 = New System.Windows.Forms.MenuStrip
        Me.Remote = New System.Windows.Forms.ToolStripMenuItem
        Me.EditConfigurationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.UeconfiginiToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.FtpiniToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip
        Me.ToolStripProgressBar1 = New System.Windows.Forms.ToolStripProgressBar
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel
        Me.MainMenu1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'config
        '
        Me.config.MergeAction = System.Windows.Forms.MergeAction.Remove
        Me.config.Name = "config"
        Me.config.Size = New System.Drawing.Size(95, 20)
        Me.config.Text = "Configuration"
        '
        'Run
        '
        Me.Run.MergeAction = System.Windows.Forms.MergeAction.Remove
        Me.Run.Name = "Run"
        Me.Run.Size = New System.Drawing.Size(35, 20)
        Me.Run.Text = "Run"
        '
        'monitor1
        '
        Me.monitor1.MergeAction = System.Windows.Forms.MergeAction.Remove
        Me.monitor1.Name = "monitor1"
        Me.monitor1.Size = New System.Drawing.Size(59, 20)
        Me.monitor1.Text = "Monitor"
        '
        'OptionToolStripMenuItem
        '
        Me.OptionToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AutobackupToolStripMenuItem, Me.ToolStripSeparator1, Me.ToolStripMenuItem1, Me.TPinterval, Me.ToolStripSeparator2, Me.groupmode, Me.ToolStripSeparator3, Me.Remotestting, Me.ToolStripSeparator4, Me.ConsoleToolStripMenuItem})
        Me.OptionToolStripMenuItem.Name = "OptionToolStripMenuItem"
        Me.OptionToolStripMenuItem.Size = New System.Drawing.Size(53, 20)
        Me.OptionToolStripMenuItem.Text = "Option"
        '
        'AutobackupToolStripMenuItem
        '
        Me.AutobackupToolStripMenuItem.Checked = True
        Me.AutobackupToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.AutobackupToolStripMenuItem.Name = "AutobackupToolStripMenuItem"
        Me.AutobackupToolStripMenuItem.Size = New System.Drawing.Size(184, 22)
        Me.AutobackupToolStripMenuItem.Text = "Auto-backup"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(181, 6)
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(184, 22)
        Me.ToolStripMenuItem1.Text = "TP report interval:"
        '
        'TPinterval
        '
        Me.TPinterval.AccessibleDescription = ""
        Me.TPinterval.AccessibleName = ""
        Me.TPinterval.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.TPinterval.AutoToolTip = True
        Me.TPinterval.Items.AddRange(New Object() {"1", "10", "60"})
        Me.TPinterval.Name = "TPinterval"
        Me.TPinterval.Size = New System.Drawing.Size(121, 20)
        Me.TPinterval.Text = "TP report interval s"
        Me.TPinterval.ToolTipText = "Throughput report interval s"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(181, 6)
        '
        'groupmode
        '
        Me.groupmode.Checked = True
        Me.groupmode.CheckState = System.Windows.Forms.CheckState.Checked
        Me.groupmode.Name = "groupmode"
        Me.groupmode.Size = New System.Drawing.Size(184, 22)
        Me.groupmode.Text = "group select mode"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(181, 6)
        '
        'Remotestting
        '
        Me.Remotestting.Checked = True
        Me.Remotestting.CheckState = System.Windows.Forms.CheckState.Checked
        Me.Remotestting.Name = "Remotestting"
        Me.Remotestting.Size = New System.Drawing.Size(184, 22)
        Me.Remotestting.Text = "remote auto on"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(181, 6)
        '
        'ConsoleToolStripMenuItem
        '
        Me.ConsoleToolStripMenuItem.Name = "ConsoleToolStripMenuItem"
        Me.ConsoleToolStripMenuItem.Size = New System.Drawing.Size(184, 22)
        Me.ConsoleToolStripMenuItem.Text = "Console"
        '
        'MainMenu1
        '
        Me.MainMenu1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.config, Me.Run, Me.monitor1, Me.OptionToolStripMenuItem, Me.Remote, Me.EditConfigurationToolStripMenuItem})
        Me.MainMenu1.Location = New System.Drawing.Point(0, 0)
        Me.MainMenu1.Name = "MainMenu1"
        Me.MainMenu1.Size = New System.Drawing.Size(821, 24)
        Me.MainMenu1.TabIndex = 1
        '
        'Remote
        '
        Me.Remote.Name = "Remote"
        Me.Remote.Size = New System.Drawing.Size(53, 20)
        Me.Remote.Text = "Remote"
        '
        'EditConfigurationToolStripMenuItem
        '
        Me.EditConfigurationToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.UeconfiginiToolStripMenuItem, Me.FtpiniToolStripMenuItem})
        Me.EditConfigurationToolStripMenuItem.Name = "EditConfigurationToolStripMenuItem"
        Me.EditConfigurationToolStripMenuItem.Size = New System.Drawing.Size(125, 20)
        Me.EditConfigurationToolStripMenuItem.Text = "Edit configuration"
        '
        'UeconfiginiToolStripMenuItem
        '
        Me.UeconfiginiToolStripMenuItem.Name = "UeconfiginiToolStripMenuItem"
        Me.UeconfiginiToolStripMenuItem.Size = New System.Drawing.Size(142, 22)
        Me.UeconfiginiToolStripMenuItem.Text = "ueconfig.ini"
        '
        'FtpiniToolStripMenuItem
        '
        Me.FtpiniToolStripMenuItem.Name = "FtpiniToolStripMenuItem"
        Me.FtpiniToolStripMenuItem.Size = New System.Drawing.Size(142, 22)
        Me.FtpiniToolStripMenuItem.Text = "ftp.ini"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripProgressBar1, Me.ToolStripStatusLabel1})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 579)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(821, 22)
        Me.StatusStrip1.TabIndex = 3
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripProgressBar1
        '
        Me.ToolStripProgressBar1.Name = "ToolStripProgressBar1"
        Me.ToolStripProgressBar1.Size = New System.Drawing.Size(100, 16)
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.AutoSize = False
        Me.ToolStripStatusLabel1.BackColor = System.Drawing.SystemColors.ScrollBar
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(704, 17)
        Me.ToolStripStatusLabel1.Spring = True
        Me.ToolStripStatusLabel1.Text = "2017/01/19 15:29:42 Total DL:6.63M;Total UL:78.51K; Total UE number:14;No traffic" & _
            " number:3;Out of control number:1 |Cell:01801-10;1801" & Global.Microsoft.VisualBasic.ChrW(10) & "-1;1800" & Global.Microsoft.VisualBasic.ChrW(10) & "-1|"
        Me.ToolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'MDIForm1
        '
        Me.BackColor = System.Drawing.SystemColors.AppWorkspace
        Me.ClientSize = New System.Drawing.Size(821, 601)
        Me.Controls.Add(Me.MainMenu1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.IsMdiContainer = True
        Me.Location = New System.Drawing.Point(11, 57)
        Me.MainMenuStrip = Me.MainMenu1
        Me.Name = "MDIForm1"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Text = "MUEauto"
        Me.MainMenu1.ResumeLayout(False)
        Me.MainMenu1.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Public WithEvents MainMenu1 As System.Windows.Forms.MenuStrip
    Public WithEvents config As System.Windows.Forms.ToolStripMenuItem
    Public WithEvents Run As System.Windows.Forms.ToolStripMenuItem
    Public WithEvents monitor1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents OptionToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AutobackupToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TPinterval As System.Windows.Forms.ToolStripComboBox
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents Remote As System.Windows.Forms.ToolStripMenuItem
#End Region

    Private Sub MDIForm1_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated

    End Sub
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents ToolStripProgressBar1 As System.Windows.Forms.ToolStripProgressBar
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents groupmode As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents Remotestting As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripSeparator4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ConsoleToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents EditConfigurationToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UeconfiginiToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FtpiniToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
End Class