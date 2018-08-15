Option Strict Off
Option Explicit On
Imports VB = Microsoft.VisualBasic
Imports System.IO
Imports System.Net.Sockets
Imports System.Net
Imports System.Threading
Imports System.Text
Imports NTCPMSG.Server
Imports NTCPMSG.Event
Imports NTCPMSG.Serialize
Public Class Form2
    Inherits System.Windows.Forms.Form
    Public Hwmsgpool As New List(Of String)
    Dim tcpserversession As Thread = Nothing
    Dim configfile, openfilename As String
    Dim liveclient As New List(Of String)
    Dim closesserver As Boolean = False
    Private Sub Combo1_KeyPress(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyPressEventArgs) Handles Combo1.KeyPress
        Dim KeyAscii As Short = Asc(eventArgs.KeyChar)
        If KeyAscii = 27 Then Combo1.Text = Combo1.Tag
        eventArgs.KeyChar = Chr(KeyAscii)
        If KeyAscii = 0 Then
            eventArgs.Handled = True
        End If
    End Sub

    Private Sub Combo2_KeyPress(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyPressEventArgs) Handles Combo2.KeyPress
        Dim KeyAscii As Short = Asc(eventArgs.KeyChar)
        If KeyAscii = 27 Then Combo2.Text = Combo2.Tag
        eventArgs.KeyChar = Chr(KeyAscii)
        If KeyAscii = 0 Then
            eventArgs.Handled = True
        End If
    End Sub

    Private Sub Command1_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles command1.Click
        Dim i As Integer
        Dim h As Integer
        Dim checktext3 As Boolean = True
        For h = 0 To listview1.Items.Count - 1
            If listview1.Items.Item(h).SubItems(2).Text = Trim(Text3.Text) Then
                checktext3 = False
            End If
        Next
        If modifycheck() = False Then Exit Sub
        If checktext3 = True Then
            If Text3.Text <> "" And Text4.Text <> "" And Text1.Text <> "" And Text6.Text <> "" And Combo2.Text <> "" And Combo1.Text <> "" And Text8.Text <> "" And Text9.Text <> "" Then
                'UPGRADE_WARNING: Couldn't resolve default property of object i. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                i = listview1.Items.Count
                'UPGRADE_WARNING: Couldn't resolve default property of object i. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                listview1.Items.Add(i + 1)
                listview1.Items.Item(i).SubItems.Add("")
                listview1.Items.Item(i).SubItems.Add(Text3.Text)
                listview1.Items.Item(i).SubItems.Add(Text4.Text)
                listview1.Items.Item(i).SubItems.Add(Text1.Text)
                listview1.Items.Item(i).SubItems.Add(Text6.Text)
                listview1.Items.Item(i).SubItems.Add(Combo1.Text)
                listview1.Items.Item(i).SubItems.Add(Combo2.Text)
                listview1.Items.Item(i).SubItems.Add(Text8.Text)
                listview1.Items.Item(i).SubItems.Add(Text9.Text)
                listview1.Items.Item(i).SubItems.Add(Combo3.Text)

                ' If Combo1.Text = "hisi" Then
                'Text5.Text = Split(Text3.Text, ".")(0) + "." + Split(Text3.Text, ".")(1) + "." + Split(Text3.Text, ".")(2) + "." + Trim(Str(Int(Split(Text3.Text, ".")(3)) + 1))
                'End If
                'If Combo1.Text = "Qualcomm9600" Then
                'Text5.Text = Text3.Text
                'End If
                'If Combo1.Text = "BandluxeC508" Then
                'Text5.Text = Split(Text3.Text, ".")(0) + "." + Split(Text3.Text, ".")(1) + "." + Split(Text3.Text, ".")(2) + "." + Trim(Str(Int(Split(Text3.Text, ".")(3)) - 1))
                'End If
                listview1.Items.Item(i).SubItems.Add(Text5.Text)
                listview1.Items.Item(i).SubItems.Add(Text10.Text)
                listview1.Items.Item(i).SubItems.Add(TextBox1.Text)
                listview1.Items.Item(i).SubItems.Add(TextBox2.Text)
                listview1.Items.Item(i).SubItems.Add(TextBox3.Text)
                listview1.Items.Item(i).SubItems.Add(udpulconfigbox.Text)
                listview1.Items.Item(i).SubItems.Add(udpdlconfigbox.Text)
                listview1.Items.Item(i).SubItems.Add(targetNumbox.Text)


                Text2.Tag = listview1.SelectedItems.Item(0).SubItems(0).Text
                Text3.Tag = listview1.SelectedItems.Item(0).SubItems(2).Text
                Text4.Tag = listview1.SelectedItems.Item(0).SubItems(3).Text
                Text1.Tag = listview1.SelectedItems.Item(0).SubItems(4).Text
                Text6.Tag = listview1.SelectedItems.Item(0).SubItems(5).Text
                Combo1.Tag = listview1.SelectedItems.Item(0).SubItems(6).Text
                Combo2.Tag = listview1.SelectedItems.Item(0).SubItems(7).Text
                Text8.Tag = listview1.SelectedItems.Item(0).SubItems(8).Text
                Text9.Tag = listview1.SelectedItems.Item(0).SubItems(9).Text
                Combo3.Tag = listview1.SelectedItems.Item(0).SubItems(10).Text
                Text5.Tag = listview1.SelectedItems.Item(0).SubItems(11).Text
                Text10.Tag = listview1.SelectedItems.Item(0).SubItems(12).Text
                TextBox1.Tag = listview1.SelectedItems.Item(0).SubItems(13).Text
                TextBox2.Tag = listview1.SelectedItems.Item(0).SubItems(14).Text
                TextBox3.Tag = listview1.SelectedItems.Item(0).SubItems(15).Text
                udpulconfigbox.Tag = listview1.SelectedItems.Item(0).SubItems(16).Text
                udpdlconfigbox.Tag = listview1.SelectedItems.Item(0).SubItems(17).Text
                targetNumbox.Tag = listview1.SelectedItems.Item(0).SubItems(18).Text






                Command3.Enabled = True

            Else
                MsgBox("请输入所有的值")
            End If
        Else
            MsgBox("UE ip must unique!")
        End If


    End Sub
    Function modifycheck() As Boolean
        If Combo3.Text = "udpdl" And Trim(udpdlconfigbox.Text = "") Then
            MsgBox("traffic = udpdl, please input udpdl config info")
            Return False
        End If
        If Combo3.Text = "udpul" And Trim(udpulconfigbox.Text = "") Then
            MsgBox("traffic= udpul, please input udpul config info")
            Return False
        End If
        If Combo3.Text = "VOLTEvoiceMOC" Or Combo3.Text = "VOLTEMOC" And Trim(targetNumbox.Text = "") Then
            MsgBox("traffic= VOLTEvoiceMOC|VOLTEMOC, please input target phone number")
            Return False
        End If

        Return True



    End Function
    Private Sub Command2_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command2.Click
        Dim i As Object
        Dim h As Integer
        Dim checktext3 As Boolean = True
        If listview1.SelectedItems.Count <> 0 Then
            For h = 0 To listview1.Items.Count - 1
                If listview1.Items.Item(h).SubItems(2).Text = Trim(Text3.Text) And h <> listview1.FocusedItem.Index Then
                    checktext3 = False
                End If
            Next
            If modifycheck() = False Then Exit Sub
            If checktext3 = True Then

                If Text3.Text <> "" And Text4.Text <> "" And Text1.Text <> "" And Text6.Text <> "" And Combo2.Text <> "" And Combo1.Text <> "" And Combo3.Text <> "" And Text8.Text <> "" And Text9.Text <> "" Then
                    'UPGRADE_WARNING: Couldn't resolve default property of object i. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                    i = listview1.FocusedItem.Index
                    listview1.SelectedItems.Item(0).SubItems(0).Text = Text2.Text
                    listview1.SelectedItems.Item(0).SubItems(2).Text = Text3.Text
                    listview1.SelectedItems.Item(0).SubItems(3).Text = Text4.Text
                    listview1.SelectedItems.Item(0).SubItems(4).Text = Text1.Text
                    listview1.SelectedItems.Item(0).SubItems(5).Text = Text6.Text
                    listview1.SelectedItems.Item(0).SubItems(6).Text = Combo1.Text
                    listview1.SelectedItems.Item(0).SubItems(7).Text = Combo2.Text
                    listview1.SelectedItems.Item(0).SubItems(8).Text = Text8.Text
                    listview1.SelectedItems.Item(0).SubItems(9).Text = Text9.Text
                    listview1.SelectedItems.Item(0).SubItems(10).Text = Combo3.Text
                    If Trim(Text5.Text) = "" Then
                        Text5.Text = "2"
                    End If
                    listview1.SelectedItems.Item(0).SubItems(11).Text = Text5.Text
                    listview1.SelectedItems.Item(0).SubItems(12).Text = Text10.Text
                    listview1.SelectedItems.Item(0).SubItems(13).Text = TextBox1.Text
                    listview1.SelectedItems.Item(0).SubItems(14).Text = TextBox2.Text
                    listview1.SelectedItems.Item(0).SubItems(15).Text = TextBox3.Text
                    listview1.SelectedItems.Item(0).SubItems(16).Text = udpulconfigbox.Text
                    listview1.SelectedItems.Item(0).SubItems(17).Text = udpdlconfigbox.Text
                    listview1.SelectedItems.Item(0).SubItems(18).Text = targetNumbox.Text
                    '--------------------------------------------------
                    Text2.Tag = listview1.SelectedItems.Item(0).SubItems(0).Text
                    Text3.Tag = listview1.SelectedItems.Item(0).SubItems(2).Text
                    Text4.Tag = listview1.SelectedItems.Item(0).SubItems(3).Text
                    Text1.Tag = listview1.SelectedItems.Item(0).SubItems(4).Text
                    Text6.Tag = listview1.SelectedItems.Item(0).SubItems(5).Text
                    Combo1.Tag = listview1.SelectedItems.Item(0).SubItems(6).Text
                    Combo2.Tag = listview1.SelectedItems.Item(0).SubItems(7).Text
                    Text8.Tag = listview1.SelectedItems.Item(0).SubItems(8).Text
                    Text9.Tag = listview1.SelectedItems.Item(0).SubItems(9).Text
                    Combo3.Tag = listview1.SelectedItems.Item(0).SubItems(10).Text
                    Text5.Tag = listview1.SelectedItems.Item(0).SubItems(11).Text
                    Text10.Tag = listview1.SelectedItems.Item(0).SubItems(12).Text
                    TextBox1.Tag = listview1.SelectedItems.Item(0).SubItems(13).Text
                    TextBox2.Tag = listview1.SelectedItems.Item(0).SubItems(14).Text
                    TextBox3.Tag = listview1.SelectedItems.Item(0).SubItems(15).Text
                    udpulconfigbox.Tag = listview1.SelectedItems.Item(0).SubItems(16).Text
                    udpdlconfigbox.Tag = listview1.SelectedItems.Item(0).SubItems(17).Text
                    targetNumbox.Tag = listview1.SelectedItems.Item(0).SubItems(18).Text
                    Command3.Enabled = True

                    MDIForm1.configischanged = True
                Else
                    MsgBox("请输入所有的值")
                End If
            Else
                MsgBox("UE ip must unique!")
            End If

        End If

    End Sub
    Function checklogip(ByVal j As Integer) As Boolean
        Dim ip As String = ""
        For i = 0 To Text10.Items.Count - 1
            ip = ip + Text10.Items(i)
        Next
        If ip.IndexOf(listview1.Items.Item(j - 1).SubItems(12).Text) < 0 Then
            Return False
        End If
        Return True
    End Function
    Public Sub Command3_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command3.Click
        Dim saveitem As Object
        Dim i As Integer
        Dim UEnumbers As Integer
        Dim UElogdir As String
        Dim sections As Integer
        Command3.Enabled = Not Command3.Enabled
        UElogdir = Module1.ReadKeyVal(configfile, "dirs", "UElog")
        UEnumbers = listview1.Items.Count
        sections = Module1.TotalSections(configfile)
        For i = 1 To sections - 1
            Module1.DeleteSection(configfile, i.ToString)
        Next
        For i = 1 To UEnumbers
            If checklogip(i) = False Then
                If MDIForm1.remotecommand = False Then
                    MsgBox("There are Log IP setting which not find on this PC! Please check again.")
                Else
                    MDIForm1.remotecommandreturn = "There are Log IP setting which not find on this PC! Please check again."
                End If
                Command3.Enabled = True
                Exit Sub
            End If
        Next
        For i = 1 To UEnumbers
            saveitem = Module1.WriteKeyVal(configfile, Str(i), "ip", listview1.Items.Item(i - 1).SubItems(2).Text)
            saveitem = Module1.WriteKeyVal(configfile, Str(i), "cip", listview1.Items.Item(i - 1).SubItems(3).Text)
            saveitem = Module1.WriteKeyVal(configfile, Str(i), "serverip", listview1.Items.Item(i - 1).SubItems(4).Text)
            saveitem = Module1.WriteKeyVal(configfile, Str(i), "com", listview1.Items.Item(i - 1).SubItems(5).Text)
            saveitem = Module1.WriteKeyVal(configfile, Str(i), "type", listview1.Items.Item(i - 1).SubItems(6).Text)
            saveitem = Module1.WriteKeyVal(configfile, Str(i), "action", listview1.Items.Item(i - 1).SubItems(7).Text)
            saveitem = Module1.WriteKeyVal(configfile, Str(i), "dinterval", listview1.Items.Item(i - 1).SubItems(8).Text)
            saveitem = Module1.WriteKeyVal(configfile, Str(i), "loopinterval", listview1.Items.Item(i - 1).SubItems(9).Text)
            saveitem = Module1.WriteKeyVal(configfile, Str(i), "traffic", listview1.Items.Item(i - 1).SubItems(10).Text)
            saveitem = Module1.WriteKeyVal(configfile, Str(i), "ftpsession", listview1.Items.Item(i - 1).SubItems(11).Text)
            saveitem = Module1.WriteKeyVal(configfile, Str(i), "logip", listview1.Items.Item(i - 1).SubItems(12).Text)
            saveitem = Module1.WriteKeyVal(configfile, Str(i), "serverip2", listview1.Items.Item(i - 1).SubItems(13).Text)
            saveitem = Module1.WriteKeyVal(configfile, Str(i), "serverip3", listview1.Items.Item(i - 1).SubItems(14).Text)
            saveitem = Module1.WriteKeyVal(configfile, Str(i), "serverip4", listview1.Items.Item(i - 1).SubItems(15).Text)
            saveitem = Module1.WriteKeyVal(configfile, Str(i), "udpul", listview1.Items.Item(i - 1).SubItems(16).Text)
            saveitem = Module1.WriteKeyVal(configfile, Str(i), "udpdl", listview1.Items.Item(i - 1).SubItems(17).Text)
            saveitem = Module1.WriteKeyVal(configfile, Str(i), "tnumber", listview1.Items.Item(i - 1).SubItems(18).Text)
        Next
        If configfile <> My.Application.Info.DirectoryPath & "\ueconfig.ini" Then
            System.IO.File.Copy(configfile, My.Application.Info.DirectoryPath & "\ueconfig.ini", True)
        End If
        If MDIForm1.remotecommand = False Then
            Dim iresult = MsgBox("Do you want restart program to make changes work", MsgBoxStyle.OkCancel)
            If iresult = Microsoft.VisualBasic.MsgBoxResult.Ok Then

                restartapp()

            End If
        Else
            restartapp()
        End If


    End Sub
    Sub restartapp()
        Dim closetcpclient As New TcpClient
        Form4.appcloseflag = True
        Try
            closetcpclient.Connect("127.0.0.1", 2500)
            Dim message As String = ""
            Dim data As [Byte]() = System.Text.Encoding.ASCII.GetBytes(message)

            ' Get a client stream for reading and writing.
            '  Stream stream = client.GetStream();
            Dim stream As NetworkStream = closetcpclient.GetStream()

            ' Send the message to the connected TcpServer. 
            stream.Write(data, 0, data.Length)
            Thread.Sleep(1000)
            ConsoleHelper.FreeConsole()
        Catch ex As Exception
        End Try

        stopTCPserver()


        Application.Restart()
    End Sub
    Private Sub Form2_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        stopTCPserver()
    End Sub

    Private Sub Form2_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        initform()

    End Sub

    Private Sub UEidlist_BeforeLabelEdit(ByRef Cancel As Short)

    End Sub


    Private Sub listview1_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs)


    End Sub

    Private Sub Text1_KeyPress(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyPressEventArgs) Handles Text1.KeyPress
        Dim KeyAscii As Short = Asc(eventArgs.KeyChar)
        Try
            If KeyAscii = 27 Then Text1.Text = Text1.Tag
            If InStr("1234567890.", Chr(KeyAscii)) <= 0 And KeyAscii <> 8 Then KeyAscii = 0

            eventArgs.KeyChar = Chr(KeyAscii)
            If KeyAscii = 0 Then
                eventArgs.Handled = True
            End If
        Catch
            Text1.Text = Text1.Tag

        End Try

    End Sub
    Private Sub Text10_KeyPress(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyPressEventArgs)
        Dim KeyAscii As Short = Asc(eventArgs.KeyChar)
        Try
            If KeyAscii = 27 Then Text4.Text = Text4.Tag
            If InStr("1234567890.", Chr(KeyAscii)) <= 0 And KeyAscii <> 8 Then KeyAscii = 0

            eventArgs.KeyChar = Chr(KeyAscii)
            If KeyAscii = 0 Then
                eventArgs.Handled = True
            End If
        Catch
            Text4.Text = Text4.Tag
        End Try

    End Sub

    Private Sub Text2_KeyPress(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyPressEventArgs) Handles Text2.KeyPress
        Dim KeyAscii As Short = Asc(eventArgs.KeyChar)
        Try
            If KeyAscii = 27 Then Text2.Text = Text2.Tag
            If InStr("1234567890", Chr(KeyAscii)) <= 0 And KeyAscii <> 8 Then KeyAscii = 0



            eventArgs.KeyChar = Chr(KeyAscii)
            If KeyAscii = 0 Then
                eventArgs.Handled = True
            End If
        Catch
            Text2.Text = Text2.Tag
        End Try

    End Sub

    Private Sub Text3_KeyPress(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyPressEventArgs) Handles Text3.KeyPress
        Dim KeyAscii As Short = Asc(eventArgs.KeyChar)
        Try
            If KeyAscii = 27 Then Text3.Text = Text3.Tag
            If InStr("1234567890.", Chr(KeyAscii)) <= 0 And KeyAscii <> 8 Then KeyAscii = 0

            eventArgs.KeyChar = Chr(KeyAscii)
            If KeyAscii = 0 Then
                eventArgs.Handled = True
            End If
        Catch
            Text3.Text = Text3.Tag
        End Try

    End Sub

    Private Sub Text4_KeyPress(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyPressEventArgs) Handles Text4.KeyPress
        Dim KeyAscii As Short = Asc(eventArgs.KeyChar)
        Try
            If KeyAscii = 27 Then Text4.Text = Text4.Tag
            If InStr("1234567890.", Chr(KeyAscii)) <= 0 And KeyAscii <> 8 Then KeyAscii = 0

            eventArgs.KeyChar = Chr(KeyAscii)
            If KeyAscii = 0 Then
                eventArgs.Handled = True
            End If
        Catch
            Text4.Text = Text4.Tag
        End Try

    End Sub

    Private Sub Text5_KeyPress(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyPressEventArgs) Handles Text5.KeyPress
        Dim KeyAscii As Short = Asc(eventArgs.KeyChar)
        Try
            If KeyAscii = 27 Then Text5.Text = Text5.Tag
            If InStr("1234567890", Chr(KeyAscii)) <= 0 And KeyAscii <> 8 Then KeyAscii = 0
            eventArgs.KeyChar = Chr(KeyAscii)
            If KeyAscii = 0 Then
                eventArgs.Handled = True
            End If
        Catch
            Text5.Text = Text5.Tag
        End Try

    End Sub

    Private Sub Text8_KeyPress(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyPressEventArgs) Handles Text8.KeyPress
        Dim KeyAscii As Short = Asc(eventArgs.KeyChar)
        Try
            If KeyAscii = 27 Then Text8.Text = Text8.Tag
            If InStr("1234567890", Chr(KeyAscii)) <= 0 And KeyAscii <> 8 Then KeyAscii = 0

            eventArgs.KeyChar = Chr(KeyAscii)
            If KeyAscii = 0 Then
                eventArgs.Handled = True
            End If
        Catch
            Text8.Text = Text8.Tag
        End Try

    End Sub

    Private Sub Text9_KeyPress(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyPressEventArgs) Handles Text9.KeyPress
        Dim KeyAscii As Short = Asc(eventArgs.KeyChar)
        Try
            If KeyAscii = 27 Then Text9.Text = Text9.Tag
            If InStr("1234567890", Chr(KeyAscii)) <= 0 And KeyAscii <> 8 Then KeyAscii = 0

            eventArgs.KeyChar = Chr(KeyAscii)
            If KeyAscii = 0 Then
                eventArgs.Handled = True
            End If
        Catch
            Text9.Text = Text9.Tag
        End Try
    End Sub

    Private Sub listview1_Click1(ByVal sender As Object, ByVal e As System.EventArgs) Handles listview1.Click

        Text2.Text = listview1.SelectedItems.Item(0).SubItems(0).Text
        Text3.Text = listview1.SelectedItems.Item(0).SubItems(2).Text
        Text4.Text = listview1.SelectedItems.Item(0).SubItems(3).Text
        Text1.Text = listview1.SelectedItems.Item(0).SubItems(4).Text
        Text6.Text = listview1.SelectedItems.Item(0).SubItems(5).Text
        Combo1.Text = listview1.SelectedItems.Item(0).SubItems(6).Text
        Combo2.Text = listview1.SelectedItems.Item(0).SubItems(7).Text
        Text8.Text = listview1.SelectedItems.Item(0).SubItems(8).Text
        Text9.Text = listview1.SelectedItems.Item(0).SubItems(9).Text
        Combo3.Text = listview1.SelectedItems.Item(0).SubItems(10).Text
        If Combo3.Text = "volte4" Then
            udpdlconfigbox.Visible = False
            udpulconfigbox.Visible = False
            targetNumbox.Visible = False
            udpdllabel.Visible = False
            udpullabel.Visible = False
            targetNumlabel.Visible = False
        Else
            udpdlconfigbox.Visible = True
            udpulconfigbox.Visible = True
            targetNumbox.Visible = True
            udpdllabel.Visible = True
            udpullabel.Visible = True
            targetNumlabel.Visible = True

        End If
        Text5.Text = listview1.SelectedItems.Item(0).SubItems(11).Text
        Text10.Text = listview1.SelectedItems.Item(0).SubItems(12).Text
        TextBox1.Text = listview1.SelectedItems.Item(0).SubItems(13).Text
        TextBox2.Text = listview1.SelectedItems.Item(0).SubItems(14).Text
        TextBox3.Text = listview1.SelectedItems.Item(0).SubItems(15).Text
        udpulconfigbox.Text = listview1.SelectedItems.Item(0).SubItems(16).Text
        udpdlconfigbox.Text = listview1.SelectedItems.Item(0).SubItems(17).Text
        targetNumbox.Text = listview1.SelectedItems.Item(0).SubItems(18).Text
        '--------------------------------------------------
        Text2.Tag = listview1.SelectedItems.Item(0).SubItems(0).Text
        Text3.Tag = listview1.SelectedItems.Item(0).SubItems(2).Text
        Text4.Tag = listview1.SelectedItems.Item(0).SubItems(3).Text
        Text1.Tag = listview1.SelectedItems.Item(0).SubItems(4).Text
        Text6.Tag = listview1.SelectedItems.Item(0).SubItems(5).Text
        Combo1.Tag = listview1.SelectedItems.Item(0).SubItems(6).Text
        Combo2.Tag = listview1.SelectedItems.Item(0).SubItems(7).Text
        Text8.Tag = listview1.SelectedItems.Item(0).SubItems(8).Text
        Text9.Tag = listview1.SelectedItems.Item(0).SubItems(9).Text
        Combo3.Tag = listview1.SelectedItems.Item(0).SubItems(10).Text
        Text5.Tag = listview1.SelectedItems.Item(0).SubItems(11).Text
        Text10.Tag = listview1.SelectedItems.Item(0).SubItems(12).Text
        TextBox1.Tag = listview1.SelectedItems.Item(0).SubItems(13).Text
        TextBox2.Tag = listview1.SelectedItems.Item(0).SubItems(14).Text
        TextBox3.Tag = listview1.SelectedItems.Item(0).SubItems(15).Text
        udpulconfigbox.Tag = listview1.SelectedItems.Item(0).SubItems(16).Text
        udpdlconfigbox.Tag = listview1.SelectedItems.Item(0).SubItems(17).Text
        targetNumbox.Tag = listview1.SelectedItems.Item(0).SubItems(18).Text
        '-----deal no volte support ue
        Select Case Combo1.Text
            Case "BG96", "SIM7000", "SIM7200", "MC7455", "E5776", "E5375", "EM7565"
                disablevoltincombo()
            Case "Qualcomm9600", "SMG9350", "Qualcomm8998", "Qualcomm8996"
                Enabledvolteincombo()
            Case "YY9027", "YY9206"
                enablevoicecombo3()


        End Select



    End Sub
    Sub enablevoicecombo3()
        Combo2.Items.Clear()
        Combo2.Items.Add("attach-detach")
        Combo2.Items.Add("attach-idle")
        Combo2.Items.Add("idle-active")
        Combo2.Items.Add("long-run")
        Combo2.Items.Add("paging")
        Combo2.Items.Add("VOLTEvoiceMOC")
        Combo2.Items.Add("VOLTEvoiceMTC")
        Combo3.Items.Clear()
        Combo3.Items.Add("ftpdl")
        Combo3.Items.Add("ftpdlul")
        Combo3.Items.Add("ftpul")
        Combo3.Items.Add("http")
        Combo3.Items.Add("httpdownload")
        Combo3.Items.Add("ping")
        Combo3.Items.Add("udpul")
        Combo3.Items.Add("udpdl")
        Combo3.Items.Add("video")
        Combo3.Items.Add("None")

    End Sub
    Sub disablevoltincombo()
        Combo2.Items.Clear()
        Combo2.Items.Add("attach-detach")
        Combo2.Items.Add("attach-idle")
        Combo2.Items.Add("idle-active")
        Combo2.Items.Add("long-run")
        Combo2.Items.Add("paging")
        'Combo2.Items.Add("VOLTEMOC")

        Combo3.Items.Clear()
        Combo3.Items.Add("ftpdl")
        Combo3.Items.Add("ftpdlul")
        Combo3.Items.Add("ftpul")
        Combo3.Items.Add("http")
        Combo3.Items.Add("httpdownload")
        Combo3.Items.Add("ping")
        Combo3.Items.Add("udpul")
        Combo3.Items.Add("udpdl")
        Combo3.Items.Add("video")
        Combo3.Items.Add("volte4")
        Combo3.Items.Add("waitcall")
    End Sub
    Sub Enabledvolteincombo()
        Combo2.Items.Clear()
        Combo2.Items.Add("attach-detach")
        Combo2.Items.Add("attach-idle")
        Combo2.Items.Add("idle-active")
        Combo2.Items.Add("long-run")
        Combo2.Items.Add("paging")
        Combo2.Items.Add("VOLTEMOC")
        Combo3.Items.Clear()
        Combo3.Items.Add("ftpdl")
        Combo3.Items.Add("ftpdlul")
        Combo3.Items.Add("ftpul")
        Combo3.Items.Add("http")
        Combo3.Items.Add("httpdownload")
        Combo3.Items.Add("ping")
        Combo3.Items.Add("udpul")
        Combo3.Items.Add("udpdl")
        Combo3.Items.Add("video")
        'Combo3.Items.Add("script-internal")
        'Combo3.Items.Add("waitcall")
        Combo3.Items.Add("None")
    End Sub

    Private Sub Text6_KeyPress(ByVal sender As Object, ByVal eventArgs As System.Windows.Forms.KeyPressEventArgs) Handles Text6.KeyPress
        Dim KeyAscii As Short = Asc(eventArgs.KeyChar)
        If KeyAscii = 27 Then Text6.Text = Text6.Tag
    End Sub

    Private Sub Text6_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Text6.TextChanged

    End Sub

    Private Sub Combo3_KeyPress(ByVal sender As Object, ByVal eventArgs As System.Windows.Forms.KeyPressEventArgs) Handles Combo3.KeyPress
        Dim KeyAscii As Short = Asc(eventArgs.KeyChar)
        If KeyAscii = 27 Then Combo3.Text = Combo3.Tag
    End Sub



    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub Text1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Text1.TextChanged

    End Sub

    Private Sub listview1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles listview1.SelectedIndexChanged

    End Sub

    Private Sub Text5_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Text5.TextChanged

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try

            listview1.Items.Remove(listview1.SelectedItems(0))
        Catch

        End Try

        Command3.Enabled = True
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

        If OpenFileDialog1.ShowDialog() <> Windows.Forms.DialogResult.Cancel Then
            configfile = OpenFileDialog1.FileName
            Command3.Enabled = True
            If configfile <> My.Application.Info.DirectoryPath & "\ueconfig.ini" Then
                System.IO.File.Copy(configfile, My.Application.Info.DirectoryPath & "\ueconfig.ini", True)
                initform()
                If Not (Form4 Is Nothing) Then
                    Form4.listchange = True
                End If
            End If
        End If

    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim saveitem As Object
        Dim i As Integer
        Dim UEnumbers As Integer
        Dim UElogdir As String
        Dim savefile As String
        Dim sections As Integer
        If SaveFileDialog1.ShowDialog <> Windows.Forms.DialogResult.Cancel Then
            savefile = SaveFileDialog1.FileName
            configfile = My.Application.Info.DirectoryPath & "\ueconfig.ini"
            UElogdir = Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\ueconfig.ini", "dirs", "UElog")
            UEnumbers = listview1.Items.Count
            sections = Module1.TotalSections(configfile)
            For i = 1 To sections - 1
                Module1.DeleteSection(configfile, i.ToString)
            Next

            For i = 1 To UEnumbers
                saveitem = Module1.WriteKeyVal(configfile, Str(i), "ip", listview1.Items.Item(i - 1).SubItems(2).Text)
                saveitem = Module1.WriteKeyVal(configfile, Str(i), "cip", listview1.Items.Item(i - 1).SubItems(3).Text)
                saveitem = Module1.WriteKeyVal(configfile, Str(i), "serverip", listview1.Items.Item(i - 1).SubItems(4).Text)
                saveitem = Module1.WriteKeyVal(configfile, Str(i), "com", listview1.Items.Item(i - 1).SubItems(5).Text)
                saveitem = Module1.WriteKeyVal(configfile, Str(i), "type", listview1.Items.Item(i - 1).SubItems(6).Text)
                saveitem = Module1.WriteKeyVal(configfile, Str(i), "action", listview1.Items.Item(i - 1).SubItems(7).Text)
                saveitem = Module1.WriteKeyVal(configfile, Str(i), "dinterval", listview1.Items.Item(i - 1).SubItems(8).Text)
                saveitem = Module1.WriteKeyVal(configfile, Str(i), "loopinterval", listview1.Items.Item(i - 1).SubItems(9).Text)
                saveitem = Module1.WriteKeyVal(configfile, Str(i), "traffic", listview1.Items.Item(i - 1).SubItems(10).Text)
                saveitem = Module1.WriteKeyVal(configfile, Str(i), "ftpsession", listview1.Items.Item(i - 1).SubItems(11).Text)
                saveitem = Module1.WriteKeyVal(configfile, Str(i), "logip", listview1.Items.Item(i - 1).SubItems(12).Text)
                saveitem = Module1.WriteKeyVal(configfile, Str(i), "serverip2", listview1.Items.Item(i - 1).SubItems(13).Text)
                saveitem = Module1.WriteKeyVal(configfile, Str(i), "serverip3", listview1.Items.Item(i - 1).SubItems(14).Text)
                saveitem = Module1.WriteKeyVal(configfile, Str(i), "serverip4", listview1.Items.Item(i - 1).SubItems(15).Text)

            Next
            If savefile <> My.Application.Info.DirectoryPath & "\ueconfig.ini" Then
                System.IO.File.Copy(My.Application.Info.DirectoryPath & "\ueconfig.ini", savefile, True)
            End If

            Dim iresult = MsgBox("Do you want restart program to make changes work", MsgBoxStyle.OkCancel)
            If iresult = Microsoft.VisualBasic.MsgBoxResult.Ok Then
                stopTCPserver()
                Application.Restart()
            End If
        End If
    End Sub

    Public Sub initform()
        Dim UElogdir As String
        Dim UEnumbers As String
        Dim i As Short
        configfile = My.Application.Info.DirectoryPath & "\ueconfig.ini"
        UElogdir = Module1.ReadKeyVal(configfile, "dirs", "UElog")
        UEnumbers = Module1.TotalSections(configfile)
        listview1.Items.Clear() '清空列表
        listview1.Columns.Clear() '清空列表头
        listview1.View = System.Windows.Forms.View.Details '设置列表显示方式
        listview1.GridLines = True '显示网络线
        listview1.LabelEdit = False '禁止标签编辑
        listview1.FullRowSelect = True '选择整行


        listview1.Columns.Add("", "UE ID", 50) '给列表中添加列名
        listview1.Columns.Add("", "UE IMSI", 100)
        listview1.Columns.Add("", "UE IP", 110)
        listview1.Columns.Add("", "UE control IP", 110)
        listview1.Columns.Add("", "server IP", 110)
        listview1.Columns.Add("", "UE COM port", 50)
        listview1.Columns.Add("", "UE type", 100)
        listview1.Columns.Add("", "UE action", 100)
        listview1.Columns.Add("", "drop detect interval", 50)
        listview1.Columns.Add("", "attach-detach interval", 50)
        listview1.Columns.Add("", "traffic type", 100)
        listview1.Columns.Add("", "Ftp session", 50)
        listview1.Columns.Add("", "Log IP", 110)
        listview1.Columns.Add("", "server IP2", 110)
        listview1.Columns.Add("", "server IP3", 110)
        listview1.Columns.Add("", "server IP4", 110)
        listview1.Columns.Add("", "udpul", 110)
        listview1.Columns.Add("", "udpdl", 110)
        listview1.Columns.Add("", "volte target phone number", 110)
        MDIForm1.ToolStripStatusLabel1.Text = "Initliazing UE config page UE list"


        If CDbl(UEnumbers) - 2 < 0 Then
            MDIForm1.ToolStripProgressBar1.Maximum = 1
        Else
            MDIForm1.ToolStripProgressBar1.Maximum = CDbl(UEnumbers) - 2
        End If
        MDIForm1.ToolStripProgressBar1.Value = 0
        If CDbl(UEnumbers) - 2 >= 0 Then
            For i = 0 To CDbl(UEnumbers) - 2
                If Module1.GetSection(configfile, i + 1) <> "dirs" Then

                    listview1.Items.Add(Module1.GetSection(configfile, i + 1))
                    listview1.Items.Item(i).SubItems.Add("")
                    listview1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i + 1), "ip")))
                    listview1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i + 1), "cip")))
                    listview1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i + 1), "serverip")))
                    listview1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i + 1), "com")))
                    listview1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i + 1), "type")))
                    listview1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i + 1), "action")))
                    listview1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i + 1), "dinterval")))
                    listview1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i + 1), "loopinterval")))
                    listview1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i + 1), "traffic")))
                    listview1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i + 1), "ftpsession")))
                    listview1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i + 1), "logip")))
                    listview1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i + 1), "serverip2")))
                    listview1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i + 1), "serverip3")))
                    listview1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i + 1), "serverip4")))
                    listview1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i + 1), "udpul")))
                    listview1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i + 1), "udpdl")))
                    listview1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i + 1), "tnumber")))

                End If
                MDIForm1.ToolStripProgressBar1.Value = i
                System.Windows.Forms.Application.DoEvents()
            Next
        End If

        MDIForm1.ToolStripStatusLabel1.Text = ""

        GetLocalIPe()

    End Sub

    Private Sub TextBox1_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox1.KeyPress
        Dim KeyAscii As Short = Asc(e.KeyChar)
        Try
            If KeyAscii = 27 Then TextBox1.Text = TextBox1.Tag
            If InStr("1234567890.", Chr(KeyAscii)) <= 0 And KeyAscii <> 8 Then KeyAscii = 0

            e.KeyChar = Chr(KeyAscii)
            If KeyAscii = 0 Then
                e.Handled = True
            End If
        Catch
            TextBox1.Text = TextBox1.Tag
        End Try
    End Sub

    Private Sub TextBox1_TextChanged_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged

    End Sub

    Private Sub TextBox2_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox2.KeyPress
        Dim KeyAscii As Short = Asc(e.KeyChar)
        Try
            If KeyAscii = 27 Then TextBox2.Text = TextBox2.Tag
            If InStr("1234567890.", Chr(KeyAscii)) <= 0 And KeyAscii <> 8 Then KeyAscii = 0

            e.KeyChar = Chr(KeyAscii)
            If KeyAscii = 0 Then
                e.Handled = True
            End If
        Catch
            TextBox2.Text = TextBox2.Tag
        End Try
    End Sub

    Private Sub TextBox3_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox3.KeyPress
        Dim KeyAscii As Short = Asc(e.KeyChar)
        Try
            If KeyAscii = 27 Then TextBox3.Text = TextBox3.Tag
            If InStr("1234567890.", Chr(KeyAscii)) <= 0 And KeyAscii <> 8 Then KeyAscii = 0

            e.KeyChar = Chr(KeyAscii)
            If KeyAscii = 0 Then
                e.Handled = True
            End If
        Catch
            TextBox3.Text = TextBox3.Tag
        End Try

    End Sub
    Protected Function GetLocalIPe() As IPAddress
        ' Dim addr As System.Net.IPAddress
        For i = 1 To System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()).Length
            Text10.Items.Add(System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())(i - 1).ToString)
        Next
        Text10.Items.Add("127.0.0.1")
    End Function

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        If OpenFileDialog1.ShowDialog() <> Windows.Forms.DialogResult.Cancel Then
            configfile = OpenFileDialog1.FileName

            Dim i = 0
            Dim j = listview1.Items.Count 'getcurrentlastline()
            While Not (Module1.GetSection(configfile, i) = -1)
                listview1.Items.Add(listview1.Items.Count + 1)
                listview1.Items.Item(j).SubItems.Add("")
                listview1.Items.Item(j).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i), "ip")))
                listview1.Items.Item(j).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i), "cip")))
                listview1.Items.Item(j).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i), "serverip")))
                listview1.Items.Item(j).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i), "com")))
                listview1.Items.Item(j).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i), "type")))
                listview1.Items.Item(j).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i), "action")))
                listview1.Items.Item(j).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i), "dinterval")))
                listview1.Items.Item(j).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i), "loopinterval")))
                listview1.Items.Item(j).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i), "traffic")))
                listview1.Items.Item(j).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i), "ftpsession")))
                listview1.Items.Item(j).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i), "logip")))
                listview1.Items.Item(j).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i), "serverip2")))
                listview1.Items.Item(j).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i), "serverip3")))
                listview1.Items.Item(j).SubItems.Add(Trim(Module1.ReadKeyVal(configfile, Module1.GetSection(configfile, i), "serverip4")))
                i = i + 1
                j = j + 1
            End While
        End If

    End Sub

    Public Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        Hwmsgpool.Clear()
        If listview1.Items.Count = 0 Then

            MsgBox("Please input at least one UE configuration as a sample for auto configutation!")
            Exit Sub

        End If
        If senderconfigcommand() = True Then
            MDIForm1.state = "reconfiguring"
            stopTCPserver()
            RunTCPserver()

            getnewconfig()
            MDIForm1.ToolStripProgressBar1.Value = 75
            stopTCPserver()

            ' debugstub()
            compareautomodifyconfig() '1.try have to delete 2.try have 0 to none 0 3. try change
            MDIForm1.ToolStripProgressBar1.Value = 100
            MDIForm1.ToolStripStatusLabel1.Text = " Update hardware info finished!"
            MDIForm1.state = "idle"
        End If

    End Sub
    Sub debugstub()
        Hwmsgpool.Clear()
        'Hwmsgpool.Add("$Check|1.1.1.1|06/17/2016   15:47:41|COM1-Qualcomm9600*COM2-Qualcomm9600*COM5-Qualcomm9600*COM6-Qualcomm9600*COM7-Qualcomm9600")
        'Hwmsgpool.Add("$Check|1.1.2.1|06/17/2016   15:47:41|COM5-Qualcomm9600*COM6-Qualcomm9600*COM9-Qualcomm9600*COM10-Qualcomm9600*COM11-Qualcomm9600")
        Hwmsgpool.Add("$Check|192.168.1.3|06/20/2016   10:32:24|COM1-Qualcomm9600*COM2-Qualcomm9600*COM24-Qualcomm9600*COM26-Qualcomm9600*COM28-Qualcomm9600*COM30-Qualcomm9600*COM46-Qualcomm9600*COM48-Qualcomm9600*COM50-Qualcomm9600*COM52-Qualcomm9600*COM54-Qualcomm9600*COM56-Qualcomm9600*COM58-Qualcomm9600*COM8-Qualcomm9600")
        Hwmsgpool.Add("$Check|192.168.1.5|06/20/2016   10:32:49|COM1-Qualcomm9600*COM20-Qualcomm9600*COM22-Qualcomm9600*COM24-Qualcomm9600*COM26-Qualcomm9600*COM28-Qualcomm9600*COM30-Qualcomm9600*COM46-Qualcomm9600*COM50-Qualcomm9600*COM52-Qualcomm9600*COM54-Qualcomm9600*COM56-Qualcomm9600*COM58-Qualcomm9600*COM8-Qualcomm9600")
        Hwmsgpool.Add("$Check|192.168.1.4|06/20/2016   10:32:49|COM1-Qualcomm9600*COM20-Qualcomm9600*COM24-Qualcomm9600*COM26-Qualcomm9600*COM28-Qualcomm9600*COM30-Qualcomm9600*COM46-Qualcomm9600*COM48-Qualcomm9600*COM50-Qualcomm9600*COM52-Qualcomm9600*COM54-Qualcomm9600*COM56-Qualcomm9600*COM58-Qualcomm9600*COM8-Qualcomm9600")
        Hwmsgpool.Add("$Check|192.168.1.2|06/20/2016   10:32:50|COM1-Qualcomm9600*COM20-Qualcomm9600*COM22-Qualcomm9600*COM24-Qualcomm9600*COM26-Qualcomm9600*COM28-Qualcomm9600*COM30-Qualcomm9600*COM46-Qualcomm9600*COM50-Qualcomm9600*COM52-Qualcomm9600*COM54-Qualcomm9600*COM56-Qualcomm9600*COM58-Qualcomm9600*COM8-Qualcomm9600")
        Hwmsgpool.Add("$Check|192.168.1.1|06/20/2016   10:32:27|COM1-Qualcomm9600*COM20-Qualcomm9600*COM24-Qualcomm9600*COM26-Qualcomm9600*COM28-Qualcomm9600*COM30-Qualcomm9600*COM46-Qualcomm9600*COM48-Qualcomm9600*COM50-Qualcomm9600*COM52-Qualcomm9600*COM54-Qualcomm9600*COM56-Qualcomm9600*COM58-Qualcomm9600*COM8-Qualcomm9600")
        reordermsg(Hwmsgpool)
    End Sub
    Sub compareautomodifyconfig()
        'ip|com|type
        Dim newconfiglist As List(Of String)
        Dim tempstr As String
        Dim binfo As New List(Of String)

        '--------------
        newconfiglist = getnewlist()
        Console.WriteLine("print newconfiglist")
        For i = 0 To newconfiglist.Count - 1
            Console.WriteLine(newconfiglist(i))
        Next

        Dim oldlist As List(Of String)
        oldlist = getoldlist()
        Console.WriteLine("print oldlist")
        For i = 0 To oldlist.Count - 1
            Console.WriteLine(oldlist(i))
        Next
        Console.WriteLine("remove same ue")
        '-----------remove same part----------
        removesamelist(newconfiglist, oldlist)
        Console.WriteLine("print newconfiglist after remove same ue")
        For i = 0 To newconfiglist.Count - 1
            Console.WriteLine(newconfiglist(i))
        Next
        Console.WriteLine("print oldlist after remove same ue")
        For i = 0 To oldlist.Count - 1
            Console.WriteLine(oldlist(i))
        Next

        '----------change old part, output 2 condition, 1.old more 2.new more------------
        changeoldpart(newconfiglist, oldlist)
        newconfiglist.Sort()
        '----------delete remaining old ue, add news ---------------
        Dim actionb As String = ""
        Dim dintervalb As String = ""
        Dim loopintervalb As String = ""
        Dim trafficb As String = ""
        Dim ftpsessionb As String = ""
        Dim logipb As String = ""

        If oldlist.Count > 0 Then
            tempstr = "Do you want delete these not find UEs:" + vbCrLf
            Dim tempi, tempib As Integer
            tempib = 0
            For i = 0 To oldlist.Count - 1
                tempi = CInt(i / 2)
                If tempi > tempib Then
                    tempstr = tempstr + oldlist(i) + vbCrLf
                    tempib = tempi
                Else
                    tempstr = tempstr + oldlist(i) + ","
                End If

            Next
            If MDIForm1.remotecommand = True Then
                Dim i = listview1.Items.Count - 1

                binfo.Add(listview1.Items(i).SubItems(7).Text)
                binfo.Add(listview1.Items(i).SubItems(8).Text)
                binfo.Add(listview1.Items(i).SubItems(9).Text)
                binfo.Add(listview1.Items(i).SubItems(10).Text)
                binfo.Add(listview1.Items(i).SubItems(11).Text)
                binfo.Add(listview1.Items(i).SubItems(12).Text)


                deleteoldue(oldlist)
            Else
                If MessageBox.Show(tempstr, "Confirm delete old UEs", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
                    '---------bakuplast info 
                    Dim i = listview1.Items.Count - 1

                    binfo.Add(listview1.Items(i).SubItems(7).Text)
                    binfo.Add(listview1.Items(i).SubItems(8).Text)
                    binfo.Add(listview1.Items(i).SubItems(9).Text)
                    binfo.Add(listview1.Items(i).SubItems(10).Text)
                    binfo.Add(listview1.Items(i).SubItems(11).Text)
                    binfo.Add(listview1.Items(i).SubItems(12).Text)


                    deleteoldue(oldlist)
                End If
            End If



        End If



        If newconfiglist.Count > 0 Then

            tempstr = "Do you want add these new find UEs:" + vbCrLf
            Dim tempi, tempib As Integer
            tempib = 0
            For i = 0 To newconfiglist.Count - 1
                tempi = CInt(i / 2)
                If tempi > tempib Then
                    tempstr = tempstr + newconfiglist(i) + vbCrLf
                    tempib = tempi
                Else
                    tempstr = tempstr + newconfiglist(i) + ","
                End If

            Next
            If MDIForm1.remotecommand <> True Then
                If MessageBox.Show(tempstr, "Confirm add new UEs", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then

                    addnewue(newconfiglist, binfo)

                End If
            Else
                addnewue(newconfiglist, binfo)
            End If



        End If


        Command3.Enabled = True

    End Sub
    Sub addnewue(ByVal newconfiglist As List(Of String), ByVal binfo As List(Of String))
        Dim cpip, lastcpip As String
        Dim uename As New List(Of String)
        Dim uenameb As New List(Of String)
        Dim serverip As New List(Of String)
        Dim serveripb As New List(Of String)
        Dim action As String = ""
        Dim dinterval As String = ""
        Dim loopinterval As String = ""
        Dim traffic As String = ""
        Dim ftpsession As String = ""
        Dim logip As String = ""
        Dim server2, server3, server4 As String
        server2 = ""
        server3 = ""
        server4 = ""
        Dim i, j As Integer
        Dim nameadd As Integer = 256
        Dim cominfo, uetype As String
        Dim timsi As String = ""
        Dim udpulstr As String = ""
        Dim udpdlstr As String = ""
        Dim targetnum As String = ""
        lastcpip = "init"
        For j = 0 To newconfiglist.Count - 1
            cominfo = Split(newconfiglist(j), "|")(1)
            uetype = Split(Split(newconfiglist(j), "|")(2), "#")(0)
            timsi = Split(Split(newconfiglist(j), "|")(2), "#")(1)
            cpip = Split(newconfiglist(j), "|")(0)
            If cpip <> lastcpip Then
                uename.Clear()
                getbasicinf(cpip, uename, serverip, action, dinterval, loopinterval, traffic, ftpsession, logip, binfo, udpulstr, udpdlstr, targetnum)
                lastcpip = cpip
            Else
            End If

            i = listview1.Items.Count
            listview1.Items.Add(i + 1)
            If timsi <> "" Then
                listview1.Items.Item(i).SubItems.Add(timsi)
            Else
                listview1.Items.Item(i).SubItems.Add("")
            End If

            If uename.Count > 0 Then
                listview1.Items.Item(i).SubItems.Add(uename(0))
                uename.RemoveAt(0)
            Else
                listview1.Items.Item(i).SubItems.Add("1.1.1." + nameadd.ToString)
                nameadd = nameadd + 1
            End If

            listview1.Items.Item(i).SubItems.Add(cpip)
            If serverip.Count > 0 Then
                listview1.Items.Item(i).SubItems.Add(serverip(0))
                serverip.RemoveAt(0)
            Else
                listview1.Items.Item(i).SubItems.Add("1.1.1.1")
            End If
            listview1.Items.Item(i).SubItems.Add(cominfo.ToUpper)
            listview1.Items.Item(i).SubItems.Add(uetype)
            listview1.Items.Item(i).SubItems.Add(action)
            listview1.Items.Item(i).SubItems.Add(dinterval)
            listview1.Items.Item(i).SubItems.Add(loopinterval)
            listview1.Items.Item(i).SubItems.Add(traffic)
            listview1.Items.Item(i).SubItems.Add(ftpsession)
            listview1.Items.Item(i).SubItems.Add(logip)
            listview1.Items.Item(i).SubItems.Add(server2)
            listview1.Items.Item(i).SubItems.Add(server3)
            listview1.Items.Item(i).SubItems.Add(server4)
            listview1.Items.Item(i).SubItems.Add(udpulstr)
            listview1.Items.Item(i).SubItems.Add(udpdlstr)
            listview1.Items.Item(i).SubItems.Add(targetnum)
        Next

    End Sub
    Sub getbasicinf(ByVal cpip As String, ByRef uename As List(Of String), ByRef serverip As List(Of String), ByRef action As String, ByRef dinterval As String, ByRef loopinterval As String, ByRef traffic As String, ByRef ftpsession As String, ByRef logip As String, ByVal binfo As List(Of String), ByVal udpulstr As String, ByVal udpdlstr As String, ByVal targetnum As String)
        Dim found As Boolean = False
        If listview1.Items.Count > 0 Then
            For i = listview1.Items.Count - 1 To 0 Step -1
                If listview1.Items(i).SubItems(3).Text = cpip Then
                    action = listview1.Items(i).SubItems(7).Text
                    dinterval = listview1.Items(i).SubItems(8).Text
                    loopinterval = listview1.Items(i).SubItems(9).Text
                    traffic = listview1.Items(i).SubItems(10).Text
                    ftpsession = listview1.Items(i).SubItems(11).Text
                    logip = listview1.Items(i).SubItems(12).Text
                    udpulstr = listview1.Items(i).SubItems(16).Text
                    udpdlstr = listview1.Items(i).SubItems(17).Text
                    targetnum = listview1.Items(i).SubItems(18).Text
                    found = False
                    Exit For
                End If
            Next
        End If

        If found = False And listview1.Items.Count > 0 Then
            action = listview1.Items(listview1.Items.Count - 1).SubItems(7).Text
            dinterval = listview1.Items(listview1.Items.Count - 1).SubItems(8).Text
            loopinterval = listview1.Items(listview1.Items.Count - 1).SubItems(9).Text
            traffic = listview1.Items(listview1.Items.Count - 1).SubItems(10).Text
            ftpsession = listview1.Items(listview1.Items.Count - 1).SubItems(11).Text
            logip = listview1.Items(listview1.Items.Count - 1).SubItems(12).Text
            udpulstr = listview1.Items(listview1.Items.Count - 1).SubItems(16).Text
            udpdlstr = listview1.Items(listview1.Items.Count - 1).SubItems(17).Text
            targetnum = listview1.Items(listview1.Items.Count - 1).SubItems(18).Text
        ElseIf listview1.Items.Count = 0 Then
            action = binfo(0)
            dinterval = binfo(1)
            loopinterval = binfo(2)
            traffic = binfo(3)
            ftpsession = binfo(4)
            logip = binfo(5)


        End If
        '------------getavalibleserveriplist-----------
        getappserverspool(serverip)

        '-------------getavalibleuename-----------------
        Dim tempuename As String
        tempuename = "1.1." + Split(cpip, ".")(3) + "."
        uename.Clear()
        For i = 1 To 400
            uename.Add(tempuename + i.ToString)
        Next
        '------------removeuseduenameand serverip----------
        For i = listview1.Items.Count - 1 To 0 Step -1
            If listview1.Items(i).SubItems(3).Text = cpip Then
                If serverip.IndexOf(listview1.Items(i).SubItems(4).Text) >= 0 Then
                    serverip.RemoveAt(serverip.IndexOf(listview1.Items(i).SubItems(4).Text))
                End If

            End If
            If uename.IndexOf(listview1.Items(i).SubItems(2).Text) >= 0 Then
                uename.RemoveAt(uename.IndexOf(listview1.Items(i).SubItems(2).Text))
            End If
        Next



    End Sub

    Sub getappserverspool(ByRef serverip As List(Of String))
        Dim startip, stopip As Integer
        Dim spoolstr As String
        Dim serverspoolstrlist As String()
        Dim servergroup As String
        Dim outputlist As New List(Of String)
        configfile = My.Application.Info.DirectoryPath & "\ueconfig.ini"
        spoolstr = Module1.ReadKeyVal(configfile, "dirs", "serverpool")
        If spoolstr = "" Then
            MessageBox.Show("Not find server spool info in ueconfig.ini file dir section, key serverpool, will use default spool 172.31.252.1-100", "Alarm", MessageBoxButtons.OK)
            spoolstr = "172.31.252.1,1,100"

        End If
        serverspoolstrlist = Split(spoolstr, "|")
        For i = 0 To serverspoolstrlist.Length - 1
            servergroup = Split(serverspoolstrlist(i), ",")(0)
            startip = Val(Split(serverspoolstrlist(i), ",")(1))
            stopip = Val(Split(serverspoolstrlist(i), ",")(2))
            For j = startip To stopip
                outputlist.Add(servergroup + j.ToString)
            Next

        Next
        serverip = outputlist
    End Sub


    Sub deleteoldue(ByVal oldlist As List(Of String))
        For i = oldlist.Count - 1 To 0 Step -1
            listview1.Items.Item(CInt(Split(oldlist(i), "|")(0)) - 1).Remove()
        Next
    End Sub

    Sub changeoldpart(ByRef newconfiglist As List(Of String), ByRef oldlist As List(Of String))
        Dim changelist As New List(Of String)
        Dim changeliststring As String
        Dim oldcpip, tempnewcpip As String
        Dim ueid As String
        Dim tempnewuetype As String
        Dim tempnewcominfo As String
        Dim i As Integer = 0
        Dim j As Integer = 0
        While i < oldlist.Count
            oldcpip = Split(oldlist(i), "|")(1)
            ueid = Split(oldlist(i), "|")(0)
            j = 0
            While j < newconfiglist.Count
                tempnewcpip = Split(newconfiglist(j), "|")(0)
                tempnewcominfo = Split(newconfiglist(j), "|")(1)
                tempnewuetype = Split(newconfiglist(j), "|")(2)
                If Trim(tempnewcpip) = Trim(oldcpip) Then
                    changeliststring = ueid + "|" + Trim(tempnewcpip) + "|" + tempnewcominfo.ToUpper + "|" + tempnewuetype
                    changelist.Add(changeliststring)
                    newconfiglist.RemoveAt(j)
                    oldlist.RemoveAt(i)
                    i = i - 1
                    j = j - 1
                    Exit While
                End If
                j = j + 1

            End While

            i = i + 1
        End While
        Dim changestr As String = "Have changed these UEs configuration:" + vbCrLf
        Dim templist As String()
        For i = 0 To changelist.Count - 1
            templist = Split(changelist(i), "|")
            'listview1.Items.Item(CInt(templist(0)) - 1).SubItems(3).Text = templist(1)
            listview1.Items.Item(CInt(templist(0)) - 1).SubItems(5).Text = templist(2)
            listview1.Items.Item(CInt(templist(0)) - 1).SubItems(6).Text = Split(templist(3), "#")(0)
            listview1.Items.Item(CInt(templist(0)) - 1).SubItems(1).Text = Split(templist(3), "#")(1)
            changestr = changestr + "Ue ip " + listview1.Items.Item(CInt(templist(0)) - 1).SubItems(2).Text + " to " + templist(2) + "|" + templist(3) + vbCrLf
        Next
        If changelist.Count > 0 Then
            MsgBox(changestr)
        End If



        Console.WriteLine("Change remain the old list to new")
        Console.WriteLine("Print need replace list")
        For i = 0 To changelist.Count - 1
            Console.WriteLine(changelist(i))
        Next

        Console.WriteLine("print remaining oldlist")
        For i = 0 To oldlist.Count - 1
            Console.WriteLine(oldlist(i))
        Next
        Console.WriteLine("print remaining newlist")
        For i = 0 To newconfiglist.Count - 1
            Console.WriteLine(newconfiglist(i))
        Next
    End Sub




    Sub removesamelist(ByRef newconfiglist As List(Of String), ByRef oldlist As List(Of String))
        Dim newn As Integer = 0
        Dim oldn As Integer = 0
        Dim tempnew As String = ""
        Dim tempold As String = ""
        While 1
            If newconfiglist.Count = 0 Or oldlist.Count = 0 Then Exit Sub
            newn = 0
            While newn < newconfiglist.Count And oldn >= 0
                tempnew = newconfiglist(newn).ToUpper
                tempold = Mid(oldlist(oldn).ToUpper, oldlist(oldn).IndexOf("|") + 2)
                ' If tempnew.IndexOf(tempold) >= 0 Then
                If tempold.IndexOf(tempnew) >= 0 Then
                    newconfiglist.RemoveAt(newn)
                    oldlist.RemoveAt(oldn)
                    newn = newn - 1
                    oldn = oldn - 1
                    Exit While
                End If
                newn = newn + 1
            End While

            oldn = oldn + 1
            If oldn > oldlist.Count - 1 Then Exit Sub
        End While

    End Sub
    Function getoldlist() As List(Of String)
        Dim uenumbers As Integer
        uenumbers = listview1.Items.Count
        Dim outputlist As New List(Of String)
        Dim tempstr As String = ""
        For i = 1 To uenumbers
            tempstr = i.ToString + "|"
            tempstr = tempstr + listview1.Items.Item(i - 1).SubItems(3).Text + "|" 'cip
            tempstr = tempstr + listview1.Items.Item(i - 1).SubItems(5).Text + "|" 'com
            tempstr = tempstr + listview1.Items.Item(i - 1).SubItems(6).Text + "#" 'type
            tempstr = tempstr + listview1.Items.Item(i - 1).SubItems(1).Text  'type

            outputlist.Add(tempstr)
        Next
        Return outputlist
    End Function
    Function getnewlist() As List(Of String)
        Dim outputlist As New List(Of String)
        Dim tempstr, tempstr2 As String()
        Dim tempmsg, tempmsg2 As String
        For i = 0 To Hwmsgpool.Count - 1
            tempstr = Split(Hwmsgpool(i), "|")
            If tempstr.Count = 4 And Trim(tempstr(3)) <> "" Then

                tempmsg = tempstr(1) + "|"

                tempstr2 = Split(tempstr(3), "*")
                For j = 0 To tempstr2.Count - 1
                    tempmsg2 = tempmsg + Replace(tempstr2(j), "-", "|")
                    outputlist.Add(tempmsg2)
                Next



            End If
        Next
        Return outputlist
    End Function
    Sub getnewconfig()
        Dim nomsclient As List(Of String)
        nomsclient = liveclient
        MDIForm1.ToolStripStatusLabel1.Text = "Waiting receive hw info *"
        While 1
            If nomsclient.Count = 0 Then Exit Sub
            For j = 0 To Hwmsgpool.Count - 1
                If nomsclient.Count = 0 Then Exit While
                For i = 0 To nomsclient.Count - 1

                    If Hwmsgpool(j).IndexOf(nomsclient(i)) >= 0 Then
                        nomsclient.RemoveAt(i)
                        Exit For
                    End If
                Next
            Next
            MDIForm1.ToolStripStatusLabel1.Text = "Waiting receive hw info x"
            wait(500)
            MDIForm1.ToolStripStatusLabel1.Text = "Waiting receive hw info +"
            wait(500)
        End While
        MDIForm1.ToolStripStatusLabel1.Text = "Have got hw info from " + Hwmsgpool.Count.ToString + " clients!"
        Console.WriteLine("hwmspool have msg:" + Hwmsgpool.Count.ToString)
        'For j = 0 To Hwmsgpool.Count - 1
        '    writetploglocal(Hwmsgpool(j), False, "d:\newconfig.txt")
        '    Console.WriteLine(Hwmsgpool(j))
        'Next
        reordermsg(Hwmsgpool)


    End Sub
    Sub reordermsg(ByRef hwmsgpool As List(Of String))
        Dim iplist As New List(Of String)
        Dim output As New List(Of String)
        For i = 0 To hwmsgpool.count - 1
            iplist.Add(Split(hwmsgpool(i), "|")(1))
        Next
        iplist.Sort()
        For i = 0 To iplist.Count - 1
            For j = 0 To hwmsgpool.count - 1
                If hwmsgpool(j).IndexOf(iplist(i)) >= 0 Then
                    output.Add(hwmsgpool(j))
                End If
            Next
        Next
        hwmsgpool = output
    End Sub
    Sub writetploglocal(ByVal logstr As String, ByVal Start As Boolean, ByVal logname As String)
        Dim outputstr, fn As String
        Dim writed As Boolean
        Dim i As Integer
        i = 0
        writed = False
        My.Application.DoEvents()
        fn = logname
        Do While writed = False And i < 3
            Try
                Dim fw As System.IO.StreamWriter = New System.IO.StreamWriter(fn, True, System.Text.Encoding.UTF8)
                outputstr = Now.ToString("MM/dd/yyyy   HH:mm:ss") + "|" + logstr + vbCrLf
                fw.Write(outputstr)

                writed = True
                fw.Close()
                fw.Dispose()

            Catch e As Exception
                writed = False

            End Try
            i = i + 1
        Loop
    End Sub
    Public Sub wait(ByRef ms As Short)

        Dim Start As Integer
        Start = VB.Timer()
        Do While VB.Timer() < Start + ms / 1000 '   3   秒的延时
            System.Windows.Forms.Application.DoEvents() '转让控制权
        Loop

    End Sub
    Public Function senderconfigcommand() As Boolean
        Dim iplist As String()
        Dim receivedata As String
        Dim localip As String
        MDIForm1.ToolStripStatusLabel1.Text = "Checking network state to Clients."
        Application.DoEvents()
        iplist = gettargetiplist()
        If iplist(0) = "" Then
            MDIForm1.ToolStripStatusLabel1.Text = "Not find client ip setting in ueconfig. "
            Return False
        End If
        ''For i = 0 To UBound(iplist)
        ''    localip = GetLocalIP(iplist(i))
        ''    If localip <> "1.1.1.1" Then Exit For
        ''Next

        'Console.WriteLine(localip)
        'If localip = "1.1.1.1" Then
        '    MDIForm1.ToolStripStatusLabel1.Text = "Not find route interface ip which is to clients " + iplist(0) + " (usconfig.ini setting)"
        '    Return False
        'End If

        MDIForm1.ToolStripProgressBar1.Maximum = 100
        For i = 0 To iplist.Count - 1

            MDIForm1.ToolStripStatusLabel1.Text = "Try get hw info from " + iplist(i)
            localip = GetLocalIP(iplist(i))
            If localip <> "1.1.1.1" Then
                receivedata = sendtcpcommand("run=" & "d:\mueauto\bindingtool.exe -a -i " + localip, iplist(i))
                Application.DoEvents()
                If receivedata <> "Connection fail" Then
                    liveclient.Add(iplist(i))

                Else
                    MDIForm1.ToolStripStatusLabel1.Text = "Get hw info from " + iplist(i) + " fail!"
                    Application.DoEvents()
                End If
            Else
                MDIForm1.ToolStripStatusLabel1.Text = "Not find route interface ip which is to clients " + iplist(0) + " (usconfig.ini setting)"
            End If

            MDIForm1.ToolStripProgressBar1.Value = CInt((i / iplist.Count / 2) * 100)
        Next

        Return True
    End Function
    Function getmyip(ByVal iplist As String()) As String
        ' Dim addr As System.Net.IPAddress

        For i = 1 To System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()).Length
            Try
                If Split(System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())(i - 1).ToString(), ".")(0) = Split(iplist(0), ".")(0) And Split(System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())(i - 1).ToString(), ".")(1) = Split(iplist(0), ".")(1) And Split(System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())(i - 1).ToString(), ".")(2) = Split(iplist(0), ".")(2) Then
                    Return System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())(i - 1).ToString()
                End If
            Catch ex As Exception
                Dim a = 1
            End Try
        Next
        Return "Not find ip which is in same subnet as clients(ftp.ini setting)"
    End Function
    Function gettargetiplist() As String()
        configfile = My.Application.Info.DirectoryPath & "\ueconfig.ini"
        Return Split(Module1.ReadKeyVal(configfile, "dirs", "clientip"), ",")
    End Function
    Public Function sendtcpcommand(ByVal command As String, ByVal ip As String) As String
        Try
            'Dim port As Int32 = 33891
            'Dim client As New System.Net.Sockets.TcpClient(ip, 33891, )
            Dim client As New System.Net.Sockets.TcpClient()
            Dim result = client.BeginConnect(ip, 33891, Nothing, Nothing)
            Dim success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2))
            If Not success Then
                Throw New Exception("Failed to connect.")
            End If
            ' Translate the passed message into ASCII and store it as a Byte array.
            Dim data As [Byte]() = System.Text.Encoding.ASCII.GetBytes(command)

            ' Get a client stream for reading and writing.
            '  Stream stream = client.GetStream();
            Dim stream As System.Net.Sockets.NetworkStream = client.GetStream()
            client.ReceiveTimeout = 1000
            'If client.Connected = False Then
            '    Return "Connection fail"
            'End If

            ' Send the message to the connected TcpServer. 
            stream.Write(data, 0, data.Length)

            ' Receive the TcpServer.response.
            ' Buffer to store the response bytes.
            data = New [Byte](256) {}

            ' String to store the response ASCII representation.
            Dim responseData As [String] = [String].Empty

            ' Read the first batch of the TcpServer response bytes.
            Dim bytes As Int32 = stream.Read(data, 0, data.Length)
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes)
            sendtcpcommand = Trim(responseData)
            stream.Close()
            client.Close()
        Catch e As Exception
            Return ("Connection fail")
            'Catch e As System.Net.Sockets.SocketException
            '    Return ("Connection fail")
        End Try
    End Function
    Public Sub RunTCPserver(ByVal args() As String)
        Dim listener As NTCPMSG.Server.NTcpListener
        'Create a tcp listener that listen 2500 TCP port.
        listener = New NTcpListener(New IPEndPoint(IPAddress.Any, 2501))
        'DataReceived event will be called back when server get message from client which connect to.
        AddHandler listener.DataReceived, AddressOf ReceiveEventHandler
        'RemoteDisconnected event will be called back when specified client disconnected.
        AddHandler listener.RemoteDisconnected, AddressOf DisconnectEventHandler
        'Accepted event will be called back when specified client connected
        AddHandler listener.Accepted, AddressOf AcceptedEventHandler
        'Start listening.
        'This function will not block current thread.
        listener.Listen()
        If consoleon = True Then Console.WriteLine("Listening...")

        'While True
        '    System.Threading.Thread.Sleep((5 * 1000))
        '    'Push message to client example.
        '    For Each clientIpEndPoint As IPEndPoint In listener.GetRemoteEndPoints
        '        Dim successful As Boolean = listener.AsyncSend(clientIpEndPoint, CType(Event1.PushMessage, UInteger), Encoding.UTF8.GetBytes("I am from server!"))
        '        If successful Then
        '            Console.WriteLine(String.Format("Push message to {0} successful!", clientIpEndPoint))
        '        Else
        '            Console.WriteLine(String.Format("Push message to {0} fail!", clientIpEndPoint))
        '        End If

        '    Next
        '    For Each cableId As UInt16 In listener.GetCableIds
        '        Dim successful As Boolean = listener.AsyncSend(cableId, CType(Event1.PushMessage, UInteger), Encoding.UTF8.GetBytes(String.Format("Hi cable {0}!", cableId)))
        '        If successful Then
        '            Console.WriteLine(String.Format("Push message to cable {0} successful!", cableId))
        '        Else
        '            Console.WriteLine(String.Format("Push message to cable {0} fail!", cableId))
        '        End If

        '    Next

        'End While
        'System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite)
    End Sub
    Enum Event1

        OneWay = 1

        _Return = 2

        PushMessage = 3

        Bin = 4
    End Enum
    Class BinMessageParse
        Inherits MessageParse

        Public Sub New()
            MyBase.New(New BinSerializer, New BinSerializer)

        End Sub
        Overrides Function ProcessMessage(ByVal SCBID As Integer, ByVal RemoteIPEndPoint As System.Net.EndPoint, ByVal Flag As NTCPMSG.MessageFlag, ByVal CableId As UShort, ByVal Channel As UInteger, ByVal Event1 As UInteger, ByVal obj As Object) As Object
            Console.WriteLine(obj)
            Return Nothing
        End Function
    End Class


    Private Shared _sBinMessageParse As BinMessageParse = New BinMessageParse

    ''' <summary>
    ''' DataReceived event will be called back when server get message from client which connect to.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    Private Sub ReceiveEventHandler(ByVal sender As Object, ByVal args As ReceiveEventArgs)
        Select Case (CType(args.Event, Event1))
            Case Event1.OneWay
                'Get OneWay message from client
                If (Not (args.Data) Is Nothing) Then

                    Try
                        CType(sender, NTcpListener).AsyncSend(args.RemoteIPEndPoint, 1, Encoding.ASCII.GetBytes("!"))
                        If (args.CableId <> 0) Then
                            Console.WriteLine("Get one way message from cable {0}", args.CableId)
                        Else
                            Console.WriteLine("Get one way message from {0}", args.RemoteIPEndPoint)
                        End If

                        Console.WriteLine(Encoding.UTF8.GetString(args.Data))
                    Catch e As Exception
                        Console.WriteLine(e)
                    End Try
                    ' send.AsyncSend(clientIpEndPoint, CType(Event1.PushMessage, UInteger), Encoding.UTF8.GetBytes("I am from server!"))
                End If

            Case Event1._Return
                'Get return message from client
                If (Not (args.Data) Is Nothing) Then
                    Try
                        ' If consoleon = True Then ConsoleHelper.AttachConsole(ConsoleHelper.ATTACH_PARENT_PROCESS)
                        If consoleon = True Then Console.WriteLine(Encoding.UTF8.GetString(args.Data))
                        dealmsg(Encoding.ASCII.GetString(args.Data))
                        Dim fromClient As Integer = BitConverter.ToInt32(args.Data, 0)
                        args.ReturnData = BitConverter.GetBytes(++fromClient)
                    Catch e As Exception
                        Console.WriteLine(e)
                    End Try

                End If

            Case Event1.Bin
                _sBinMessageParse.ReceiveEventHandler(sender, args)
        End Select

    End Sub

    ''' <summary>
    ''' RemoteDisconnected event will be called back when specified client disconnected.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    Private Shared Sub DisconnectEventHandler(ByVal sender As Object, ByVal args As DisconnectEventArgs)
        If consoleon = True Then Console.WriteLine("Remote socket:{0} disconnected.", args.RemoteIPEndPoint)
    End Sub

    ''' <summary>
    ''' Accepted event will be called back when specified client connected
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    Private Shared Sub AcceptedEventHandler(ByVal sender As Object, ByVal args As AcceptEventArgs)
        If consoleon = True Then Console.WriteLine("Remote socket:{0} connected.", args.RemoteIPEndPoint)
    End Sub
    Public Sub dealmsg(ByVal dealmsg As String)
        Dim msgs As Object
        Dim tempstr As Object
        Dim outputstr As String
        'Dim uename As String
        ' Dim TP1, TP2 As String

        Try

            msgs = Split(dealmsg, "$")
            For i = 1 To UBound(msgs)
                If Trim(msgs(i)) <> "" Then
                    tempstr = Split(msgs(i), "|")
                    If UBound(tempstr) >= 2 Then

                        If tempstr(0) = "Check" Then
                            'writelog(tempstr)

                            If UBound(tempstr) >= 3 Then
                                outputstr = tempstr(1) + "|" + tempstr(3)
                                Hwmsgpool.Add(outputstr)
                            End If

                        End If
                    End If
                End If

            Next
        Catch ex As Exception
            If consoleon = True Then Console.WriteLine(ex.Message.ToString)
        End Try

    End Sub
    Private listener As System.Net.Sockets.TcpListener
    Private listenThread As System.Threading.Thread
    Sub stopTCPserver()
        closesserver = True
        Try
            Dim closeclient = New TcpClient("127.0.0.1", 2501)
            If closeclient.Connected = True Then
                closeclient.Close()
                Console.WriteLine("Close tcp server....")
            Else

            End If


            If Not (listener Is Nothing) Then
                listener.Stop()
                listenThread.Abort()
            End If

        Catch e3 As Exception
        End Try

    End Sub
    Sub runTCPserver()
        listener = New System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, 2501) 'The TcpListener will listen for incoming connections at port 43001
        listenThread = New System.Threading.Thread(AddressOf doListen) 'This thread will run the doListen method

        listenThread.IsBackground = True 'Since we dont want this thread to keep on running after the application closes, we set isBackground to true.

        listener.Start() 'Start listening.
        listenThread.Name = "TCPserver"

        listenThread.Start()
        closesserver = False
    End Sub
    Private Sub doListen()
        Dim bytes(1024) As Byte
        Dim data As String = Nothing
        Dim incomingClient As System.Net.Sockets.TcpClient
        'Dim printlog As New printinf(AddressOf writelog) '定义数据显示委托实例
        'Dim run_ue As New remoterun(AddressOf runUE)
        'Dim run_load As New remoteload(AddressOf loadfile)
        'Dim run_scenario As New remotescenario(AddressOf scenario)
        'Dim run_shutdownUE As New remoteshutdown(AddressOf shutdownUE)
        Dim loadfilename As String = ""
        Do
            Try
                If closesserver = True Then
                    Console.WriteLine("tcpserver now exit")
                    Exit Sub

                End If
                'Invoke(printlog, "listening...")
                ''TextBox1.Text = TextBox1.Text & "listening..." & vbCrLf
                incomingClient = listener.AcceptTcpClient 'Accept the incoming connection. This is a blocking method so execution will halt here until someone tries to connect.
                'TextBox1.Text = TextBox1.Text & "Connected!" & vbCrLf
                'Invoke(printlog, "Connected")
                If closesserver = True Then
                    Console.WriteLine("tcpserver now exit")
                    Exit Sub

                End If

                data = Nothing

                ' Get a stream object for reading and writing
                Dim stream As System.Net.Sockets.NetworkStream = incomingClient.GetStream()

                Dim i As Int32
                i = 0
                ' Loop to receive all the data sent by the client.

                If incomingClient.Connected = True Then
                    data = vbCrLf + ">"
                    Dim msg0 As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
                    stream.Write(msg0, 0, msg0.Length)
                    'Invoke(printlog, "Send:" + data)
                    i = stream.Read(bytes, 0, bytes.Length)
                End If

                While (i <> 0)
                    If closesserver = True Then
                        Console.WriteLine("tcp server now closed")
                        Exit Sub
                    End If

                    ' Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i)
                    If data.IndexOf(Chr(13) & Chr(10)) > 0 Then data = Microsoft.VisualBasic.Left(data, data.Length - 2)
                    Hwmsgpool.Add(data)
                    Console.WriteLine("add new msg:" + data)

                    ' Process the data sent by the client.
                    data = vbCrLf + ">"
                    Dim msg9 As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
                    stream.Write(msg9, 0, msg9.Length)

                    If incomingClient.Connected = True Then
                        i = stream.Read(bytes, 0, bytes.Length)
                    Else
                        i = 0
                    End If
                End While

            Catch e1 As System.IO.IOException
                'Invoke(printlog, "Client disconnected")
                ''Finally
                ''   listener.Stop()
            Catch ex As Exception
                Exit Sub
            End Try
        Loop

    End Sub
    Protected Function GetLocalIP(ByVal serverip As String) As String
        'Dim addr As System.Net.IPAddress
        Dim subnet As String()
        Dim subnetstring1, subnetstring2 As String
        subnet = getsubnettoserver(serverip)

        subnetstring1 = subnet(0).ToString + "." + subnet(1).ToString + "." + subnet(2).ToString + "."
        subnetstring2 = subnet(0).ToString + "." + subnet(1).ToString + "."

        If subnetstring1 = "127.0.0." Then Return "127.0.0.1"

        For i = 1 To System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()).Length
            If System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())(i - 1).ToString.IndexOf(subnetstring1) = 0 Then
                Return System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())(i - 1).ToString
            End If
        Next
        For i = 1 To System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()).Length
            If System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())(i - 1).ToString.IndexOf(subnetstring2) = 0 Then
                Return System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())(i - 1).ToString
            End If
        Next

        Return "1.1.1.1"
    End Function

    Function getsubnettoserver(ByVal serverip As String) As String()
        Dim output As String()
        Dim dosstr As String()
        Dim found As Boolean = False
        Dim tempstr As String
        Try
            dosstr = Split(rundoscommand("tracert -h 6 " + serverip), vbCrLf)
            For i = 0 To dosstr.Count - 1
                If Trim(dosstr(i)) <> "" Then
                    If Trim(dosstr(i)).Chars(0) = "1" Then
                        tempstr = Split(Trim(dosstr(i)), " ")(Split(Trim(dosstr(i)), " ").Length - 1)
                        tempstr = Replace(Replace(tempstr, "[", ""), "]", "")
                        If checkipaddress(tempstr) = True Then
                            output = Split(tempstr, ".")
                            found = True
                            Exit For
                        End If

                    End If
                End If
            Next
            If found = False Then
                Return Split(serverip, ".")
            Else
                Return output
            End If
        Catch ex As Exception
            Return Split(serverip, ".")
        End Try

    End Function
    Function checkipaddress(ByVal inputstring As String) As Boolean
        Try
            Dim tempip As IPAddress
            tempip = System.Net.IPAddress.Parse(inputstring)
            Console.WriteLine(tempip.ToString)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Function rundoscommand(ByVal command) As String
        Dim myProcess As Process = New Process()
        Dim s As String
        myProcess.StartInfo.FileName = "cmd.exe"
        myProcess.StartInfo.UseShellExecute = False
        myProcess.StartInfo.CreateNoWindow = True
        myProcess.StartInfo.RedirectStandardInput = True
        myProcess.StartInfo.RedirectStandardOutput = True
        myProcess.StartInfo.RedirectStandardError = True
        myProcess.Start()
        Dim sIn As StreamWriter = myProcess.StandardInput
        sIn.AutoFlush = True

        Dim sOut As StreamReader = myProcess.StandardOutput
        Dim sErr As StreamReader = myProcess.StandardError
        sIn.Write(command & _
        System.Environment.NewLine)
        sIn.Write("exit" & System.Environment.NewLine)
        s = sOut.ReadToEnd()
        If Not myProcess.HasExited Then
            myProcess.Kill()
        End If

        Console.WriteLine(s)

        sIn.Close()
        sOut.Close()
        sErr.Close()
        myProcess.Close()
        Return s
    End Function

    Private Sub Combo2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Combo2.SelectedIndexChanged
        'If Combo2.Text = "VOLTEMOC" Then
        '    Select Case Combo1.Text
        '        Case "BG96", "SIM7000", "SIM7200", "MC7455", "E5776", "E5375", "MC7455"
        '            disablevoltincombo()
        '        Case "Qualcomm9600", "Qualcomm8998", "Qualcomm8996"
        '            Enabledvolteincombo()
        '        Case "SMG9350"
        '            'disableinternal
        '        Case "9027"
        '            'adbvolte
        '    End Select


        '    ' changetrafficlist()

        'Else
        '    backtrafficlist()
        'End If
    End Sub

    Sub changetrafficlist()
        Combo3.Text = "script-internal"
        Combo3.Enabled = False
    End Sub

    Sub backtrafficlist()
        Combo3.Text = "ftpdl"
        Combo3.Enabled = True
    End Sub



    Private Sub Combo3_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Combo3.SelectedIndexChanged
        If Combo3.Text = "volte4" Then
            udpdlconfigbox.Visible = False
            udpulconfigbox.Visible = False
            targetNumbox.Visible = False
            udpdllabel.Visible = False
            udpullabel.Visible = False
            targetNumlabel.Visible = False
        Else
            udpdlconfigbox.Visible = True
            udpulconfigbox.Visible = True
            targetNumbox.Visible = True
            udpdllabel.Visible = True
            udpullabel.Visible = True
            targetNumlabel.Visible = True

        End If

    End Sub

    Private Sub Combo1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Combo1.SelectedIndexChanged
          Select Combo1.Text
            Case "BG96", "SIM7000", "SIM7200", "MC7455", "E5776", "E5375", "EM7565"
                disablevoltincombo()
            Case "Qualcomm9600", "SMG9350", "Qualcomm8998", "Qualcomm8996"
                Enabledvolteincombo()
            Case "YY9027", "YY9206"
                enablevoicecombo3()
        End Select

    End Sub
End Class