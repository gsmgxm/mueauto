<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Dialog1
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel
        Me.OK_Button = New System.Windows.Forms.Button
        Me.Cancel_Button = New System.Windows.Forms.Button
        Me.BuNewOK = New System.Windows.Forms.Button
        Me.Budelete = New System.Windows.Forms.Button
        Me.Listtobeselected = New System.Windows.Forms.ListBox
        Me.ListBoxselected = New System.Windows.Forms.ListBox
        Me.Buadd = New System.Windows.Forms.Button
        Me.ComboBoxcustom = New System.Windows.Forms.ComboBox
        Me.Buremove = New System.Windows.Forms.Button
        Me.Bumodify = New System.Windows.Forms.Button
        Me.DataSet1 = New System.Data.DataSet
        Me.DataTable1 = New System.Data.DataTable
        Me.DataColumn1 = New System.Data.DataColumn
        Me.DataColumn2 = New System.Data.DataColumn
        Me.Label1 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.TableLayoutPanel1.SuspendLayout()
        CType(Me.DataSet1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataTable1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(282, 274)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(146, 29)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(3, 3)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(67, 23)
        Me.OK_Button.TabIndex = 0
        Me.OK_Button.Text = "OK"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(76, 3)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
        Me.Cancel_Button.TabIndex = 1
        Me.Cancel_Button.Text = "Cancel"
        '
        'BuNewOK
        '
        Me.BuNewOK.Location = New System.Drawing.Point(198, 22)
        Me.BuNewOK.Name = "BuNewOK"
        Me.BuNewOK.Size = New System.Drawing.Size(75, 23)
        Me.BuNewOK.TabIndex = 1
        Me.BuNewOK.Text = "Add"
        Me.BuNewOK.UseVisualStyleBackColor = True
        '
        'Budelete
        '
        Me.Budelete.Location = New System.Drawing.Point(358, 22)
        Me.Budelete.Name = "Budelete"
        Me.Budelete.Size = New System.Drawing.Size(75, 23)
        Me.Budelete.TabIndex = 2
        Me.Budelete.Text = "Delete"
        Me.Budelete.UseVisualStyleBackColor = True
        '
        'Listtobeselected
        '
        Me.Listtobeselected.FormattingEnabled = True
        Me.Listtobeselected.Location = New System.Drawing.Point(23, 80)
        Me.Listtobeselected.Name = "Listtobeselected"
        Me.Listtobeselected.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.Listtobeselected.Size = New System.Drawing.Size(152, 173)
        Me.Listtobeselected.TabIndex = 3
        '
        'ListBoxselected
        '
        Me.ListBoxselected.FormattingEnabled = True
        Me.ListBoxselected.Location = New System.Drawing.Point(270, 80)
        Me.ListBoxselected.Name = "ListBoxselected"
        Me.ListBoxselected.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended
        Me.ListBoxselected.Size = New System.Drawing.Size(150, 173)
        Me.ListBoxselected.TabIndex = 4
        '
        'Buadd
        '
        Me.Buadd.Location = New System.Drawing.Point(198, 106)
        Me.Buadd.Name = "Buadd"
        Me.Buadd.Size = New System.Drawing.Size(46, 23)
        Me.Buadd.TabIndex = 5
        Me.Buadd.Text = "-->"
        Me.Buadd.UseVisualStyleBackColor = True
        '
        'ComboBoxcustom
        '
        Me.ComboBoxcustom.FormattingEnabled = True
        Me.ComboBoxcustom.Location = New System.Drawing.Point(23, 24)
        Me.ComboBoxcustom.Name = "ComboBoxcustom"
        Me.ComboBoxcustom.Size = New System.Drawing.Size(152, 21)
        Me.ComboBoxcustom.TabIndex = 6
        '
        'Buremove
        '
        Me.Buremove.Location = New System.Drawing.Point(198, 163)
        Me.Buremove.Name = "Buremove"
        Me.Buremove.Size = New System.Drawing.Size(46, 23)
        Me.Buremove.TabIndex = 7
        Me.Buremove.Text = "<--"
        Me.Buremove.UseVisualStyleBackColor = True
        '
        'Bumodify
        '
        Me.Bumodify.Location = New System.Drawing.Point(277, 22)
        Me.Bumodify.Name = "Bumodify"
        Me.Bumodify.Size = New System.Drawing.Size(75, 23)
        Me.Bumodify.TabIndex = 8
        Me.Bumodify.Text = "Modify"
        Me.Bumodify.UseVisualStyleBackColor = True
        '
        'DataSet1
        '
        Me.DataSet1.DataSetName = "NewDataSet"
        Me.DataSet1.Tables.AddRange(New System.Data.DataTable() {Me.DataTable1})
        '
        'DataTable1
        '
        Me.DataTable1.Columns.AddRange(New System.Data.DataColumn() {Me.DataColumn1, Me.DataColumn2})
        Me.DataTable1.TableName = "customerfilter"
        '
        'DataColumn1
        '
        Me.DataColumn1.ColumnName = "filtername"
        '
        'DataColumn2
        '
        Me.DataColumn2.ColumnName = "filterlist"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(267, 64)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(70, 13)
        Me.Label1.TabIndex = 9
        Me.Label1.Text = "Selected UE:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(20, 64)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(40, 13)
        Me.Label2.TabIndex = 10
        Me.Label2.Text = "UE list:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(20, 8)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(135, 13)
        Me.Label3.TabIndex = 11
        Me.Label3.Text = "Customer filter group name:"
        '
        'Dialog1
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(440, 315)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Bumodify)
        Me.Controls.Add(Me.Buremove)
        Me.Controls.Add(Me.ComboBoxcustom)
        Me.Controls.Add(Me.Buadd)
        Me.Controls.Add(Me.ListBoxselected)
        Me.Controls.Add(Me.Listtobeselected)
        Me.Controls.Add(Me.Budelete)
        Me.Controls.Add(Me.BuNewOK)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Dialog1"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Customer filter"
        Me.TableLayoutPanel1.ResumeLayout(False)
        CType(Me.DataSet1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataTable1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents OK_Button As System.Windows.Forms.Button
    Friend WithEvents Cancel_Button As System.Windows.Forms.Button
    Friend WithEvents BuNewOK As System.Windows.Forms.Button
    Friend WithEvents Budelete As System.Windows.Forms.Button
    Friend WithEvents Listtobeselected As System.Windows.Forms.ListBox
    Friend WithEvents ListBoxselected As System.Windows.Forms.ListBox
    Friend WithEvents Buadd As System.Windows.Forms.Button
    Friend WithEvents ComboBoxcustom As System.Windows.Forms.ComboBox
    Friend WithEvents Buremove As System.Windows.Forms.Button
    Friend WithEvents Bumodify As System.Windows.Forms.Button
    Friend WithEvents DataSet1 As System.Data.DataSet
    Friend WithEvents DataTable1 As System.Data.DataTable
    Friend WithEvents DataColumn1 As System.Data.DataColumn
    Friend WithEvents DataColumn2 As System.Data.DataColumn
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label

End Class
