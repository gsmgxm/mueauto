Option Strict Off
Option Explicit On
Imports System.Net.Sockets
Imports System.Net
Imports System.Threading
Friend Class MDIForm1

    Inherits System.Windows.Forms.Form
    Public remoteon As Boolean = False
    Public configischanged As Boolean = False
    Public singleUEselect As Boolean = False
    Public state As String = "idle"
    Public remotecommand As Boolean = False
    Public remotecommandreturn As String = ""

    'Dim Cform2 As New Form2
    'Dim Cform3 As New Form3
    'Dim Cform4 As New Form4
    'Dim Cmonitorform As New monitorform
    Public Sub config_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles config.Click
        'monitorform.MdiParent = Me
        Form2.WindowState = System.Windows.Forms.FormWindowState.Maximized
        monitorform.Visible = False
        Form2.Visible = True
        Form4.Visible = False
        Form4.Timer1.Enabled = False

    End Sub

    Public Sub monitor_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles monitor1.Click
        ' monitorform.MdiParent = Me
        monitorform.WindowState = System.Windows.Forms.FormWindowState.Maximized
        monitorform.Visible = True
        Form2.Visible = False
        Form4.Visible = False
        Form4.Timer1.Enabled = False
        monitorform.WindowState = System.Windows.Forms.FormWindowState.Maximized
    End Sub

    Public Sub Run_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Run.Click
        Form4.WindowState = System.Windows.Forms.FormWindowState.Maximized
        monitorform.Visible = False
        Form2.Visible = False
        If Form4.Visible = False Then
            If configischanged = True Then
                Form4.ListView1.Items.Clear()
            End If

            Form4.Visible = True
        End If

        Form4.Timer1.Enabled = True


    End Sub

    Private Sub MDIForm1_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        Dim closetcpclient As New TcpClient
        If Form4.tcpstarted = True Then
            Try
                Form4.appcloseflag = True
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
                '  MessageBox.Show(ex.Message)
            End Try
        End If
    End Sub

    Private Sub MDIForm1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If AutobackupToolStripMenuItem.Checked = True Then
            Module1.WriteKeyVal(My.Application.Info.DirectoryPath & "\ueconfig.ini", "dirs", "autobackup", "1")
        Else
            Module1.WriteKeyVal(My.Application.Info.DirectoryPath & "\ueconfig.ini", "dirs", "autobackup", "0")

        End If

        Module1.WriteKeyVal(My.Application.Info.DirectoryPath & "\ueconfig.ini", "dirs", "TPinterval", TPinterval.Text)

        If groupmode.Checked = True Then
            Module1.WriteKeyVal(My.Application.Info.DirectoryPath & "\ueconfig.ini", "dirs", "groupmode", "1")
        Else
            Module1.WriteKeyVal(My.Application.Info.DirectoryPath & "\ueconfig.ini", "dirs", "groupmode", "0")
        End If

        If Remotestting.Checked = True Then
            Module1.WriteKeyVal(My.Application.Info.DirectoryPath & "\ueconfig.ini", "dirs", "remoteon", "1")
        Else
            Module1.WriteKeyVal(My.Application.Info.DirectoryPath & "\ueconfig.ini", "dirs", "remoteon", "0")
        End If


    End Sub

    Private Sub MDIForm1_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Leave

    End Sub
    Function killexistingprocessHandle(ByVal Title As String) As IntPtr
        Dim max As Integer = 0
        ' Dim newpro As Integer
        Try
            For Each p As Process In Process.GetProcesses()
                If p.ProcessName.IndexOf(Title, 0) = 0 Then
                    If p.Id <> System.Diagnostics.Process.GetCurrentProcess().Id Then
                        p.Kill()

                    End If

                    ' Return 1  'Found Window
                End If
            Next
            Return IntPtr.Zero 'No Match Found
        Catch ex As Exception
        End Try

    End Function

    Private Sub MDIForm1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        killexistingprocessHandle("MUEcon")


        singleUEselect = False
        If Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\ueconfig.ini", "dirs", "groupmode") = "1" Then
            groupmode.Checked = True
        Else
            groupmode.Checked = False
            singleUEselect = True
        End If


        If Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\ueconfig.ini", "dirs", "remoteon") = "1" Then
            Remotestting.Checked = True
            remoteon = True
            Form3.Show()
        Else
            Remotestting.Checked = False
            remoteon = False
        End If


        If Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\ueconfig.ini", "dirs", "autobackup") = "1" Then
            AutobackupToolStripMenuItem.Checked = True
        Else
            AutobackupToolStripMenuItem.Checked = False
        End If
        If Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\ueconfig.ini", "dirs", "TPinterval") <> "" Then
            TPinterval.Text = Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\ueconfig.ini", "dirs", "TPinterval")
        Else
            TPinterval.Text = 60
        End If
        '----------------------------------------------------------------
        Dim myArg() As String, iCount As Integer
        Dim configfilename As String

        myArg = System.Environment.GetCommandLineArgs

        If UBound(myArg) >= 1 Then
            For iCount = 1 To UBound(myArg)

                Select Case myArg(iCount).ToString

                    Case "-c"
                        configfilename = myArg(iCount + 1)

                        If (configfilename <> My.Application.Info.DirectoryPath & "\ueconfig.ini") And (System.IO.File.Exists(My.Application.Info.DirectoryPath & "\" & configfilename)) Then
                            System.IO.File.Copy(configfilename, My.Application.Info.DirectoryPath & "\ueconfig.ini", True)

                        End If
                    Case "-a"
                        Remotestting.Checked = True
                        remoteon = True
                        Form3.Show()

                End Select

            Next
        End If


        '---------------------------------------------------------------
        monitorform.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Form4.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Form2.WindowState = System.Windows.Forms.FormWindowState.Maximized
        monitorform.Visible = True
        Form2.Visible = True
        Form4.Visible = True

    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Sub MainMenu1_ItemClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ToolStripItemClickedEventArgs) Handles MainMenu1.ItemClicked

    End Sub

    Private Sub AutobackupToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AutobackupToolStripMenuItem.Click
        AutobackupToolStripMenuItem.Checked = Not AutobackupToolStripMenuItem.Checked

    End Sub




    Private Sub TPinterval_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TPinterval.TextChanged
        If TPinterval.Text <> "1" And TPinterval.Text <> "10" And TPinterval.Text <> "60" Then
            TPinterval.Text = "60"
        End If
    End Sub

    Private Sub Remote_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Remote.Click
        Form3.Show()
    End Sub

    Private Sub groupmode_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles groupmode.Click
        groupmode.Checked = Not groupmode.Checked
        If groupmode.Checked = True Then singleUEselect = False Else singleUEselect = True
    End Sub

    Private Sub Remotestting_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Remotestting.Click
        Remotestting.Checked = Not Remotestting.Checked
        If Remotestting.Checked = True Then remoteon = True Else remoteon = False

    End Sub

    Private Sub TPinterval_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TPinterval.Click

    End Sub

    Private Sub ConsoleToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ConsoleToolStripMenuItem.Click
        ConsoleToolStripMenuItem.Checked = Not ConsoleToolStripMenuItem.Checked
        If ConsoleToolStripMenuItem.Checked = True Then
            ConsoleHelper.FreeConsole()
            ConsoleHelper.AllocConsole()
            consoleon = True
        Else
            consoleon = False
            ConsoleHelper.FreeConsole()
        End If

    End Sub

    Private Sub UeconfiginiToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UeconfiginiToolStripMenuItem.Click

        
        Dim myprocess As Process = New Process()
        
        myprocess.StartInfo.FileName = "notepad.exe"

         
        myprocess.StartInfo.Arguments = System.Windows.Forms.Application.StartupPath + "\ueconfig.ini"
 
        myprocess.Start()
       

    End Sub

    Private Sub FtpiniToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FtpiniToolStripMenuItem.Click

        Dim myprocess As Process = New Process()

        myprocess.StartInfo.FileName = "notepad.exe"


        myprocess.StartInfo.Arguments = "D:\mueauto\ftp.ini"

        myprocess.Start()

    End Sub
End Class