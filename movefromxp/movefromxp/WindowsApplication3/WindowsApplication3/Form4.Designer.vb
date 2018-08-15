<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> Partial Class Form4
#Region "Windows Form Designer generated code "
	<System.Diagnostics.DebuggerNonUserCode()> Public Sub New()
		MyBase.New()
		'This call is required by the Windows Form Designer.
		InitializeComponent()
		'This form is an MDI child.
		'This code simulates the VB6 
		' functionality of automatically
		' loading and showing an MDI
		' child's parent.
        Me.MdiParent = MDIForm1
        MDIForm1.Show()
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
	Public ToolTip1 As System.Windows.Forms.ToolTip
    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim ListViewItem1 As System.Windows.Forms.ListViewItem = New System.Windows.Forms.ListViewItem("")
        Dim ListViewItem2 As System.Windows.Forms.ListViewItem = New System.Windows.Forms.ListViewItem("")
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.DataSet1 = New System.Data.DataSet
        Me.DataTable1 = New System.Data.DataTable
        Me.DataColumn1 = New System.Data.DataColumn
        Me.DataColumn2 = New System.Data.DataColumn
        Me.DataTable2 = New System.Data.DataTable
        Me.DataColumn3 = New System.Data.DataColumn
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog
        Me.Timer2 = New System.Windows.Forms.Timer(Me.components)
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar
        Me.Timer3 = New System.Windows.Forms.Timer(Me.components)
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.SplitContainer3 = New System.Windows.Forms.SplitContainer
        Me.SplitContainer4 = New System.Windows.Forms.SplitContainer
        Me.ListView1 = New ListViewEmbeddedControls.ListViewEx
        Me.ColumnHeader1 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader2 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader3 = New System.Windows.Forms.ColumnHeader
        Me.ColumnHeader4 = New System.Windows.Forms.ColumnHeader
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.Command4 = New System.Windows.Forms.Button
        Me.Button1 = New System.Windows.Forms.Button
        Me.Command9 = New System.Windows.Forms.Button
        Me.synctime = New System.Windows.Forms.Button
        Me.Command3 = New System.Windows.Forms.Button
        Me.Command2 = New System.Windows.Forms.Button
        Me.Command7 = New System.Windows.Forms.Button
        Me.Button3 = New System.Windows.Forms.Button
        Me.Button2 = New System.Windows.Forms.Button
        Me.Text1 = New System.Windows.Forms.TextBox
        Me.Command10 = New System.Windows.Forms.Button
        Me.Command6 = New System.Windows.Forms.Button
        Me.Command1 = New System.Windows.Forms.Button
        Me.Command8 = New System.Windows.Forms.Button
        Me.Command5 = New System.Windows.Forms.Button
        Me.DataGridView1 = New System.Windows.Forms.DataGridView
        Me.ConfigurationfileDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.RuntimeDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn
        Me.Runscenario = New System.Windows.Forms.Button
        Me.butdeletscenario = New System.Windows.Forms.Button
        Me.butaddscenario = New System.Windows.Forms.Button
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer
        CType(Me.DataSet1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataTable1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataTable2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer3.Panel1.SuspendLayout()
        Me.SplitContainer3.Panel2.SuspendLayout()
        Me.SplitContainer3.SuspendLayout()
        Me.SplitContainer4.Panel1.SuspendLayout()
        Me.SplitContainer4.Panel2.SuspendLayout()
        Me.SplitContainer4.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        Me.SuspendLayout()
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 60000
        '
        'DataSet1
        '
        Me.DataSet1.DataSetName = "NewDataSet"
        Me.DataSet1.Tables.AddRange(New System.Data.DataTable() {Me.DataTable1, Me.DataTable2})
        '
        'DataTable1
        '
        Me.DataTable1.Columns.AddRange(New System.Data.DataColumn() {Me.DataColumn1, Me.DataColumn2})
        Me.DataTable1.TableName = "scenarios"
        '
        'DataColumn1
        '
        Me.DataColumn1.ColumnName = "configuration_file"
        '
        'DataColumn2
        '
        Me.DataColumn2.ColumnName = "run_time"
        Me.DataColumn2.DataType = GetType(Long)
        '
        'DataTable2
        '
        Me.DataTable2.Columns.AddRange(New System.Data.DataColumn() {Me.DataColumn3})
        Me.DataTable2.TableName = "customerfilter"
        '
        'DataColumn3
        '
        Me.DataColumn3.ColumnName = "filterlist"
        Me.DataColumn3.DataType = GetType(Object)
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.DefaultExt = "ini"
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        Me.OpenFileDialog1.Filter = "config files|*.ini"
        Me.OpenFileDialog1.Multiselect = True
        '
        'Timer2
        '
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ProgressBar1.Location = New System.Drawing.Point(1, 1)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(962, 5)
        Me.ProgressBar1.TabIndex = 18
        '
        'Timer3
        '
        Me.Timer3.Interval = 1000
        '
        'ImageList1
        '
        Me.ImageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit
        Me.ImageList1.ImageSize = New System.Drawing.Size(35, 35)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        '
        'SplitContainer3
        '
        Me.SplitContainer3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer3.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer3.Name = "SplitContainer3"
        Me.SplitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer3.Panel1
        '
        Me.SplitContainer3.Panel1.Controls.Add(Me.SplitContainer4)
        '
        'SplitContainer3.Panel2
        '
        Me.SplitContainer3.Panel2.Controls.Add(Me.Text1)
        Me.SplitContainer3.Panel2.Controls.Add(Me.Command10)
        Me.SplitContainer3.Panel2.Controls.Add(Me.Command6)
        Me.SplitContainer3.Panel2.Controls.Add(Me.Command1)
        Me.SplitContainer3.Panel2.Controls.Add(Me.Command8)
        Me.SplitContainer3.Panel2.Controls.Add(Me.Command5)
        Me.SplitContainer3.Panel2.Controls.Add(Me.DataGridView1)
        Me.SplitContainer3.Panel2.Controls.Add(Me.Runscenario)
        Me.SplitContainer3.Panel2.Controls.Add(Me.butdeletscenario)
        Me.SplitContainer3.Panel2.Controls.Add(Me.butaddscenario)
        Me.SplitContainer3.Size = New System.Drawing.Size(968, 689)
        Me.SplitContainer3.SplitterDistance = 641
        Me.SplitContainer3.TabIndex = 0
        '
        'SplitContainer4
        '
        Me.SplitContainer4.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer4.FixedPanel = System.Windows.Forms.FixedPanel.Panel2
        Me.SplitContainer4.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer4.Name = "SplitContainer4"
        Me.SplitContainer4.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer4.Panel1
        '
        Me.SplitContainer4.Panel1.Controls.Add(Me.ListView1)
        '
        'SplitContainer4.Panel2
        '
        Me.SplitContainer4.Panel2.Controls.Add(Me.TableLayoutPanel1)
        Me.SplitContainer4.Size = New System.Drawing.Size(968, 641)
        Me.SplitContainer4.SplitterDistance = 595
        Me.SplitContainer4.TabIndex = 34
        '
        'ListView1
        '
        Me.ListView1.CheckBoxes = True
        Me.ListView1.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3, Me.ColumnHeader4})
        Me.ListView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ListView1.GridLines = True
        ListViewItem1.StateImageIndex = 0
        ListViewItem2.StateImageIndex = 0
        Me.ListView1.Items.AddRange(New System.Windows.Forms.ListViewItem() {ListViewItem1, ListViewItem2})
        Me.ListView1.Location = New System.Drawing.Point(0, 0)
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Size = New System.Drawing.Size(968, 595)
        Me.ListView1.SmallImageList = Me.ImageList1
        Me.ListView1.TabIndex = 34
        Me.ListView1.UseCompatibleStateImageBehavior = False
        Me.ListView1.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "UE id"
        Me.ColumnHeader1.Width = 75
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "UEidenty"
        Me.ColumnHeader2.Width = 101
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Width = 186
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Width = 190
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 9
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.11111!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.Command4, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Button1, 8, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Command9, 6, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.synctime, 7, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Command3, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Command2, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Command7, 3, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Button3, 5, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Button2, 4, 0)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(968, 42)
        Me.TableLayoutPanel1.TabIndex = 43
        '
        'Command4
        '
        Me.Command4.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Command4.BackColor = System.Drawing.SystemColors.Control
        Me.Command4.Cursor = System.Windows.Forms.Cursors.Default
        Me.Command4.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Command4.Location = New System.Drawing.Point(229, 5)
        Me.Command4.Name = "Command4"
        Me.Command4.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Command4.Size = New System.Drawing.Size(77, 34)
        Me.Command4.TabIndex = 36
        Me.Command4.Text = "Run UE"
        Me.Command4.UseVisualStyleBackColor = False
        '
        'Button1
        '
        Me.Button1.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Button1.BackColor = System.Drawing.SystemColors.Control
        Me.Button1.Cursor = System.Windows.Forms.Cursors.Default
        Me.Button1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Button1.Location = New System.Drawing.Point(873, 5)
        Me.Button1.Name = "Button1"
        Me.Button1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Button1.Size = New System.Drawing.Size(77, 34)
        Me.Button1.TabIndex = 41
        Me.Button1.Text = "download ftp config file"
        Me.Button1.UseVisualStyleBackColor = False
        '
        'Command9
        '
        Me.Command9.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Command9.BackColor = System.Drawing.SystemColors.Control
        Me.Command9.Cursor = System.Windows.Forms.Cursors.Default
        Me.Command9.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Command9.Location = New System.Drawing.Point(657, 5)
        Me.Command9.Name = "Command9"
        Me.Command9.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Command9.Size = New System.Drawing.Size(77, 34)
        Me.Command9.TabIndex = 42
        Me.Command9.Text = "shutdown board"
        Me.Command9.UseVisualStyleBackColor = False
        '
        'synctime
        '
        Me.synctime.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.synctime.BackColor = System.Drawing.SystemColors.Control
        Me.synctime.Cursor = System.Windows.Forms.Cursors.Default
        Me.synctime.ForeColor = System.Drawing.SystemColors.ControlText
        Me.synctime.Location = New System.Drawing.Point(764, 5)
        Me.synctime.Name = "synctime"
        Me.synctime.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.synctime.Size = New System.Drawing.Size(77, 34)
        Me.synctime.TabIndex = 30
        Me.synctime.Text = "Sync time"
        Me.synctime.UseVisualStyleBackColor = False
        '
        'Command3
        '
        Me.Command3.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Command3.BackColor = System.Drawing.SystemColors.Control
        Me.Command3.Cursor = System.Windows.Forms.Cursors.Default
        Me.Command3.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Command3.Location = New System.Drawing.Point(13, 5)
        Me.Command3.Name = "Command3"
        Me.Command3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Command3.Size = New System.Drawing.Size(80, 34)
        Me.Command3.TabIndex = 35
        Me.Command3.Text = "Select None"
        Me.Command3.UseVisualStyleBackColor = False
        '
        'Command2
        '
        Me.Command2.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Command2.BackColor = System.Drawing.SystemColors.Control
        Me.Command2.Cursor = System.Windows.Forms.Cursors.Default
        Me.Command2.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Command2.Location = New System.Drawing.Point(122, 5)
        Me.Command2.Name = "Command2"
        Me.Command2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Command2.Size = New System.Drawing.Size(77, 34)
        Me.Command2.TabIndex = 34
        Me.Command2.Text = "Select all"
        Me.Command2.UseVisualStyleBackColor = False
        '
        'Command7
        '
        Me.Command7.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Command7.BackColor = System.Drawing.SystemColors.Control
        Me.Command7.Cursor = System.Windows.Forms.Cursors.Default
        Me.Command7.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Command7.Location = New System.Drawing.Point(336, 5)
        Me.Command7.Name = "Command7"
        Me.Command7.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Command7.Size = New System.Drawing.Size(77, 34)
        Me.Command7.TabIndex = 37
        Me.Command7.Text = "Stop running"
        Me.Command7.UseVisualStyleBackColor = False
        '
        'Button3
        '
        Me.Button3.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Button3.BackColor = System.Drawing.SystemColors.Control
        Me.Button3.Cursor = System.Windows.Forms.Cursors.Default
        Me.Button3.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Button3.Location = New System.Drawing.Point(550, 5)
        Me.Button3.Name = "Button3"
        Me.Button3.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Button3.Size = New System.Drawing.Size(77, 34)
        Me.Button3.TabIndex = 40
        Me.Button3.Text = "restart board"
        Me.Button3.UseVisualStyleBackColor = False
        '
        'Button2
        '
        Me.Button2.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Button2.BackColor = System.Drawing.SystemColors.Control
        Me.Button2.Cursor = System.Windows.Forms.Cursors.Default
        Me.Button2.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Button2.Location = New System.Drawing.Point(443, 5)
        Me.Button2.Name = "Button2"
        Me.Button2.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Button2.Size = New System.Drawing.Size(77, 34)
        Me.Button2.TabIndex = 39
        Me.Button2.Text = "reset UE"
        Me.Button2.UseVisualStyleBackColor = False
        '
        'Text1
        '
        Me.Text1.AcceptsReturn = True
        Me.Text1.BackColor = System.Drawing.SystemColors.WindowFrame
        Me.Text1.Cursor = System.Windows.Forms.Cursors.IBeam
        Me.Text1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Text1.ForeColor = System.Drawing.SystemColors.Window
        Me.Text1.Location = New System.Drawing.Point(0, 0)
        Me.Text1.MaxLength = 0
        Me.Text1.Multiline = True
        Me.Text1.Name = "Text1"
        Me.Text1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Text1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.Text1.Size = New System.Drawing.Size(968, 44)
        Me.Text1.TabIndex = 17
        Me.Text1.WordWrap = False
        '
        'Command10
        '
        Me.Command10.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Command10.BackColor = System.Drawing.SystemColors.Control
        Me.Command10.Cursor = System.Windows.Forms.Cursors.Default
        Me.Command10.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Command10.Location = New System.Drawing.Point(443, 19)
        Me.Command10.Name = "Command10"
        Me.Command10.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Command10.Size = New System.Drawing.Size(89, 38)
        Me.Command10.TabIndex = 16
        Me.Command10.Text = "Backup Logs"
        Me.Command10.UseVisualStyleBackColor = False
        '
        'Command6
        '
        Me.Command6.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Command6.BackColor = System.Drawing.SystemColors.Control
        Me.Command6.Cursor = System.Windows.Forms.Cursors.Default
        Me.Command6.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Command6.Location = New System.Drawing.Point(572, 19)
        Me.Command6.Name = "Command6"
        Me.Command6.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Command6.Size = New System.Drawing.Size(89, 38)
        Me.Command6.TabIndex = 14
        Me.Command6.Text = "Save log"
        Me.Command6.UseVisualStyleBackColor = False
        '
        'Command1
        '
        Me.Command1.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Command1.BackColor = System.Drawing.SystemColors.Control
        Me.Command1.Cursor = System.Windows.Forms.Cursors.Default
        Me.Command1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Command1.Location = New System.Drawing.Point(318, 19)
        Me.Command1.Name = "Command1"
        Me.Command1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Command1.Size = New System.Drawing.Size(89, 38)
        Me.Command1.TabIndex = 12
        Me.Command1.Text = "Run dos shell"
        Me.Command1.UseVisualStyleBackColor = False
        '
        'Command8
        '
        Me.Command8.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Command8.BackColor = System.Drawing.SystemColors.Control
        Me.Command8.Cursor = System.Windows.Forms.Cursors.Default
        Me.Command8.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Command8.Location = New System.Drawing.Point(202, 19)
        Me.Command8.Name = "Command8"
        Me.Command8.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Command8.Size = New System.Drawing.Size(89, 38)
        Me.Command8.TabIndex = 15
        Me.Command8.Text = "restart board"
        Me.Command8.UseVisualStyleBackColor = False
        '
        'Command5
        '
        Me.Command5.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Command5.BackColor = System.Drawing.SystemColors.Control
        Me.Command5.Cursor = System.Windows.Forms.Cursors.Default
        Me.Command5.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Command5.Location = New System.Drawing.Point(677, 6)
        Me.Command5.Name = "Command5"
        Me.Command5.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Command5.Size = New System.Drawing.Size(89, 45)
        Me.Command5.TabIndex = 13
        Me.Command5.Text = "Select not Runing"
        Me.Command5.UseVisualStyleBackColor = False
        '
        'DataGridView1
        '
        Me.DataGridView1.AllowUserToAddRows = False
        Me.DataGridView1.AllowUserToDeleteRows = False
        Me.DataGridView1.AutoGenerateColumns = False
        Me.DataGridView1.BackgroundColor = System.Drawing.SystemColors.ControlLightLight
        Me.DataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DataGridView1.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ConfigurationfileDataGridViewTextBoxColumn, Me.RuntimeDataGridViewTextBoxColumn})
        Me.DataGridView1.DataMember = "scenarios"
        Me.DataGridView1.DataSource = Me.DataSet1
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        DataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DataGridView1.DefaultCellStyle = DataGridViewCellStyle3
        Me.DataGridView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataGridView1.Location = New System.Drawing.Point(0, 0)
        Me.DataGridView1.Name = "DataGridView1"
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
        DataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        DataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.DataGridView1.RowHeadersDefaultCellStyle = DataGridViewCellStyle4
        Me.DataGridView1.RowTemplate.Height = 23
        Me.DataGridView1.Size = New System.Drawing.Size(968, 44)
        Me.DataGridView1.TabIndex = 18
        Me.DataGridView1.Visible = False
        '
        'ConfigurationfileDataGridViewTextBoxColumn
        '
        Me.ConfigurationfileDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells
        Me.ConfigurationfileDataGridViewTextBoxColumn.DataPropertyName = "configuration_file"
        Me.ConfigurationfileDataGridViewTextBoxColumn.HeaderText = "configuration_file"
        Me.ConfigurationfileDataGridViewTextBoxColumn.Name = "ConfigurationfileDataGridViewTextBoxColumn"
        Me.ConfigurationfileDataGridViewTextBoxColumn.Width = 112
        '
        'RuntimeDataGridViewTextBoxColumn
        '
        Me.RuntimeDataGridViewTextBoxColumn.DataPropertyName = "run_time"
        DataGridViewCellStyle2.Format = "N0"
        DataGridViewCellStyle2.NullValue = "864000"
        Me.RuntimeDataGridViewTextBoxColumn.DefaultCellStyle = DataGridViewCellStyle2
        Me.RuntimeDataGridViewTextBoxColumn.HeaderText = "run_time(S)"
        Me.RuntimeDataGridViewTextBoxColumn.Name = "RuntimeDataGridViewTextBoxColumn"
        '
        'Runscenario
        '
        Me.Runscenario.AccessibleRole = System.Windows.Forms.AccessibleRole.None
        Me.Runscenario.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Runscenario.AutoSize = True
        Me.Runscenario.Location = New System.Drawing.Point(440, 42)
        Me.Runscenario.Name = "Runscenario"
        Me.Runscenario.Size = New System.Drawing.Size(112, 22)
        Me.Runscenario.TabIndex = 21
        Me.Runscenario.Text = "Run scenario"
        Me.Runscenario.UseVisualStyleBackColor = True
        Me.Runscenario.Visible = False
        '
        'butdeletscenario
        '
        Me.butdeletscenario.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butdeletscenario.AutoSize = True
        Me.butdeletscenario.Location = New System.Drawing.Point(417, 17)
        Me.butdeletscenario.Name = "butdeletscenario"
        Me.butdeletscenario.Size = New System.Drawing.Size(135, 22)
        Me.butdeletscenario.TabIndex = 20
        Me.butdeletscenario.Text = "delete from scenario"
        Me.butdeletscenario.UseVisualStyleBackColor = True
        Me.butdeletscenario.Visible = False
        '
        'butaddscenario
        '
        Me.butaddscenario.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.butaddscenario.AutoSize = True
        Me.butaddscenario.Location = New System.Drawing.Point(444, 1)
        Me.butaddscenario.Name = "butaddscenario"
        Me.butaddscenario.Size = New System.Drawing.Size(112, 22)
        Me.butaddscenario.TabIndex = 19
        Me.butaddscenario.Text = "add to scenario"
        Me.butaddscenario.UseVisualStyleBackColor = True
        Me.butaddscenario.Visible = False
        '
        'SplitContainer2
        '
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer2.Name = "SplitContainer2"
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1Collapsed = True
        Me.SplitContainer2.Panel1MinSize = 0
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.SplitContainer3)
        Me.SplitContainer2.Size = New System.Drawing.Size(968, 689)
        Me.SplitContainer2.SplitterDistance = 72
        Me.SplitContainer2.TabIndex = 24
        '
        'Form4
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(968, 689)
        Me.ControlBox = False
        Me.Controls.Add(Me.SplitContainer2)
        Me.Controls.Add(Me.ProgressBar1)
        Me.Cursor = System.Windows.Forms.Cursors.Default
        Me.Location = New System.Drawing.Point(4, 23)
        Me.MaximizeBox = False
        Me.Name = "Form4"
        Me.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Run UEs"
        CType(Me.DataSet1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataTable1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataTable2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer3.Panel1.ResumeLayout(False)
        Me.SplitContainer3.Panel2.ResumeLayout(False)
        Me.SplitContainer3.Panel2.PerformLayout()
        Me.SplitContainer3.ResumeLayout(False)
        Me.SplitContainer4.Panel1.ResumeLayout(False)
        Me.SplitContainer4.Panel2.ResumeLayout(False)
        Me.SplitContainer4.ResumeLayout(False)
        Me.TableLayoutPanel1.ResumeLayout(False)
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        Me.SplitContainer2.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents DataSet1 As System.Data.DataSet
    Friend WithEvents DataTable1 As System.Data.DataTable
    Friend WithEvents DataColumn1 As System.Data.DataColumn
    Friend WithEvents DataColumn2 As System.Data.DataColumn
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents Timer2 As System.Windows.Forms.Timer
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
    Friend WithEvents Timer3 As System.Windows.Forms.Timer
    Friend WithEvents DataTable2 As System.Data.DataTable
    Friend WithEvents DataColumn3 As System.Data.DataColumn
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents SplitContainer3 As System.Windows.Forms.SplitContainer
    Friend WithEvents SplitContainer4 As System.Windows.Forms.SplitContainer
    Friend WithEvents ListView1 As ListViewEmbeddedControls.ListViewEx
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader4 As System.Windows.Forms.ColumnHeader
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Public WithEvents Command4 As System.Windows.Forms.Button
    Public WithEvents Button1 As System.Windows.Forms.Button
    Public WithEvents Command9 As System.Windows.Forms.Button
    Public WithEvents synctime As System.Windows.Forms.Button
    Public WithEvents Command3 As System.Windows.Forms.Button
    Public WithEvents Command2 As System.Windows.Forms.Button
    Public WithEvents Command7 As System.Windows.Forms.Button
    Public WithEvents Button3 As System.Windows.Forms.Button
    Public WithEvents Button2 As System.Windows.Forms.Button
    Public WithEvents Text1 As System.Windows.Forms.TextBox
    Public WithEvents Command10 As System.Windows.Forms.Button
    Public WithEvents Command6 As System.Windows.Forms.Button
    Public WithEvents Command1 As System.Windows.Forms.Button
    Public WithEvents Command8 As System.Windows.Forms.Button
    Public WithEvents Command5 As System.Windows.Forms.Button
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents ConfigurationfileDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents RuntimeDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents Runscenario As System.Windows.Forms.Button
    Friend WithEvents butdeletscenario As System.Windows.Forms.Button
    Friend WithEvents butaddscenario As System.Windows.Forms.Button
    Friend WithEvents SplitContainer2 As System.Windows.Forms.SplitContainer
#End Region
End Class