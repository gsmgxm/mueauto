Imports System.Windows.Forms

Public Class Dialog1
    Dim customerfilterlist As New Dictionary(Of String, String)

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        DataSet1.Tables("customerfilter").Rows.Clear()
        For i = 0 To customerfilterlist.Count - 1
            Dim newrow As DataRow = DataSet1.Tables("customerfilter").NewRow()
            newrow.Item(0) = customerfilterlist.Keys(i)
            newrow.Item(1) = customerfilterlist.Values(i)
            DataSet1.Tables("customerfilter").Rows.Add(newrow)
        Next
        DataSet1.WriteXml(My.Application.Info.DirectoryPath + "\customerfilter.xml")
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub Dialog1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        customerfilterlist.Clear()
        DataSet1.Tables("customerfilter").Rows.Clear()
        If System.IO.File.Exists(My.Application.Info.DirectoryPath + "\customerfilter.xml") Then
            DataSet1.ReadXml(My.Application.Info.DirectoryPath + "\customerfilter.xml")
            If DataSet1.Tables("customerfilter").Rows.Count > 0 Then
                For i = 0 To DataSet1.Tables("customerfilter").Rows.Count - 1
                    customerfilterlist.Add(DataSet1.Tables("customerfilter").Rows(i).Item(0), DataSet1.Tables("customerfilter").Rows(i).Item(1))
                Next
            End If
        End If

        '-------------------------------------------------------------------------
        ComboBoxcustom.Items.Clear()
        If customerfilterlist.Count > 0 Then
            For i = 0 To customerfilterlist.Count - 1
                ComboBoxcustom.Items.Add(customerfilterlist.Keys(i).ToString)
                ComboBoxcustom.Text = ComboBoxcustom.Items(0).ToString
            Next
        End If
        Listtobeselected.Items.Clear()
        For i = 0 To 200

            If monitorform.data(i, 1) <> "" Then
                Listtobeselected.Items.Add(monitorform.data(i, 1))
            End If
        Next
        Bumodify.Enabled = False
    End Sub

    Private Sub Buadd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Buadd.Click
        Dim tempselected As String
        For i = 1 To Listtobeselected.SelectedItems.Count
            tempselected = Listtobeselected.SelectedItems(i - 1).ToString

            If ListBoxselected.FindStringExact(tempselected) < 0 Then

                ListBoxselected.Items.Add(tempselected)
            End If


        Next
        Bumodify.Enabled = True
    End Sub

    Private Sub Buremove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Buremove.Click
        Dim i As Integer = 0
        i = ListBoxselected.SelectedItems.Count - 1
        If i >= 0 Then
            Do
                ListBoxselected.Items.Remove(ListBoxselected.SelectedItems(i))
                i = i - 1
            Loop Until i = -1
        End If
        Bumodify.Enabled = True
    End Sub

    Private Sub BuNewOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BuNewOK.Click
        Dim groupstring As String = ""
        If BuNewOK.Text = "Add" Then

            Dim newfiltername As String = ""
            newfiltername = ComboBoxcustom.Text
            If newfiltername <> "" Then
                If ComboBoxcustom.FindStringExact(newfiltername) < 0 Then
                    ComboBoxcustom.Items.Add(newfiltername)
                    ComboBoxcustom.Text = newfiltername
                    ListBoxselected.Items.Clear()
                End If
            End If
        Else






        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Bumodify.Click
        Dim groupstring As String = ""
        Dim selected As Integer = -1
        For h = 0 To ListBoxselected.Items.Count - 1
            groupstring = groupstring + ListBoxselected.Items(h).ToString + ","

        Next

        If customerfilterlist.ContainsKey(ComboBoxcustom.Text) = False And ComboBoxcustom.Text <> "" Then
            customerfilterlist.Add(ComboBoxcustom.Text, groupstring)
        ElseIf ComboBoxcustom.Text <> "" Then
            customerfilterlist.Remove(ComboBoxcustom.Text)
            customerfilterlist.Add(ComboBoxcustom.Text, groupstring)
        End If



        Bumodify.Enabled = False
    End Sub

    Private Sub ComboBoxcustom_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBoxcustom.SelectedIndexChanged
        Dim tempgroupstring As String
        Dim tempselect() As String
        ListBoxselected.Items.Clear()
        If customerfilterlist.ContainsKey(ComboBoxcustom.Text) Then
            tempgroupstring = customerfilterlist.Item(ComboBoxcustom.Text).ToString

            tempselect = Split(tempgroupstring, ",")
            For i = 0 To UBound(tempselect) - 1
                ListBoxselected.Items.Add(tempselect(i))

            Next
        End If

    End Sub

    Private Sub Budelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Budelete.Click
        If ComboBoxcustom.Text <> "" Then

            If customerfilterlist.ContainsKey(ComboBoxcustom.Text) And (ComboBoxcustom.FindStringExact(ComboBoxcustom.Text)) > -1 Then
                customerfilterlist.Remove(ComboBoxcustom.Text)
                ComboBoxcustom.Items.RemoveAt(ComboBoxcustom.FindStringExact(ComboBoxcustom.Text))
                ListBoxselected.Items.Clear()
                Bumodify.Enabled = False
                ComboBoxcustom.Text = ""
            End If

        End If
    End Sub
End Class
