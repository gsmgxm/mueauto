Imports VB = Microsoft.VisualBasic
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Management
Imports System.Net.NetworkInformation
Imports System.Runtime.InteropServices
Imports System.Net
Public Class Form1

    Dim voltehandle(2) As Integer
    Function checkipexist() As Boolean
        Return True
    End Function
    Function calspeed(ByVal interval As String, ByVal size As String) As String
        Return (Val(size) * 8 / (Val(interval) / 1000) / 1024).ToString + "kb/s"
    End Function
    Sub getgateway(ByVal ipaddress As String, ByRef gateway As String)
        gateway = ""
        If ipaddress <> "" Then
            gateway = ipaddress.Split(".")(0) + "." + ipaddress.Split(".")(1) + "." + ipaddress.Split(".")(2) + ".1"

        End If
    End Sub
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

        displaylog(s, "g", False)

        sIn.Close()
        sOut.Close()
        sErr.Close()
        myProcess.Close()
        Return s
    End Function

    Function addroute() As String
        Dim subipstring, ipaddress, gateway, returnstr As String
        'Dim netportname As String

        ipaddress = ""
        gateway = ""
        subipstring = ""
        ipaddress = ueip.Text
        getgateway(ipaddress, gateway)
        If ipaddress = "" Or gateway = "" Then
            displaylog("ip not find", "g")
            Return "ip not find"
            Exit Function
        End If
        displaylog("Clear old route to" & ipaddress, "r")
        If QCI5ip.Text <> "" Then
            returnstr = rundoscommand("route delete " + QCI5ip.Text)
            wait(1)
            returnstr = rundoscommand("route add " + QCI5ip.Text + " " + gateway)
            wait(1)
        End If

        If QCI1ip.Text <> "" Then
            returnstr = rundoscommand("route delete " + QCI1ip.Text)
            wait(1)
            returnstr = rundoscommand("route add " + QCI1ip.Text + " " + gateway)
            wait(1)
        End If

        Return "OK"

    End Function
    Private Sub runvolte()
        Dim count As Integer
        'Dim username, pass, DLsessionno, ULsessionno, DLfilename, ULfilename, ULremotename As String

        '-----------------------------------get ftp configs
        If hrpingdir.Text(Len(hrpingdir.Text) - 1) = "\" Then
            hrpingdir.Text = Mid(hrpingdir.Text, 1, Len(hrpingdir.Text) - 1)
        End If

        If checkvalueOK(ueip.Text, "ipadress") = False Then

        End If



        If checkvalueOK(ueip.Text, "ipadress") = True And checkvalueOK(QCI1ip.Text, "ipadress") = True And checkvalueOK(QCI5ip.Text, "ipadress") = True And (File.Exists(hrpingdir.Text + "\hrping.exe")) Then

            If addroute() <> "ip not find" Then

                count = 1

                count = count + 1

                count = count + 1
                Dim myprocess As Process = New Process()
                displaylog("start new volte voice bearer ping session", "g")
                myprocess = New Process()
                myprocess.StartInfo.FileName = hrpingdir.Text + "\hrping.exe"
                myprocess.StartInfo.Arguments = QCI5ip.Text + " -t -L " + QCI5size.Text + " -y -s " + QCI5interval.Text  'wait chage size, interval 50K/s， should gbr as 150K with sps
                myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized
                myprocess.Start()
                voltehandle(1) = myprocess.Id
                displaylog("voice hrping sim session id opened:" & myprocess.Id.ToString, "g")
                count = count + 1

                displaylog("start new volte video bearer ping session", "g")
                myprocess = New Process()


                myprocess.StartInfo.FileName = hrpingdir.Text + "\hrping.exe"
                myprocess.StartInfo.Arguments = QCI1ip.Text + " -t -L " + QCI1size.Text + " -y -s " + QCI1interval.Text  'wait chage size, interval
                myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized
                myprocess.Start()
                voltehandle(2) = myprocess.Id
                displaylog("video traffic ftp sim session id opened:" & myprocess.Id.ToString, "g")
                count = count + 1
                '
            Else
                Button1.Text = "Start"
            End If
        Else
            Button1.Text = "Start"
            MsgBox("Check server ip address or check whether hrping.exe is in the directory")
        End If

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If Button1.Text = "Start" Then
            Button1.Text = "Stop"
            runvolte()
        Else
            Button1.Text = "Start"
            killoldvolte()
        End If
    End Sub
    Sub displaylog(ByVal addstr As String, ByVal strcolor As String, Optional ByVal logout As Boolean = True)

        If Trim(addstr) <> " " Then
            Try
                Select Case strcolor
                    Case "g"
                        '  WebBrowser1.DocumentText = WebBrowser1.DocumentText & "<br><span lang=EN-US style='font-size:9.0pt;font-family:""Microsoft Sans Serif"";color:#00CC00'>" & addstr & vbNewLine
                        ConsoleHelper.displaylog(Trim(addstr))
                        ' If logout = True Then writelog(addstr, 0, logname) Else writelog("ipconfig", 0, logname)
                        ' locked = False
                    Case "r"
                        ' WebBrowser1.DocumentText = WebBrowser1.DocumentText & "<br><span ltang=EN-US style='font-size:9.0pt;font-family:""Microsoft Sans Serif"";color:red'>" & addstr & vbNewLine
                        ConsoleHelper.displaylog(Trim(addstr), "r")
                        ' If logout = True Then writelog(addstr, 0, logname) Else writelog("ipconfig", 0, logname)

                End Select

            Catch
            End Try

        End If
    End Sub
    Public Sub wait(ByRef s As Short)
        Dim starttime, endtime, temptime As DateTime

        Try
            starttime = DateTime.Now
            endtime = DateTime.Now
            temptime = starttime
            While DateDiff(DateInterval.Second, starttime, endtime) < s
                Threading.Thread.Sleep(30)
                If DateDiff(DateInterval.Second, temptime, endtime) Then

                    temptime = endtime
                    Console.ForegroundColor = ConsoleColor.Green
                    Console.Write("*")
                End If
                endtime = DateTime.Now


            End While

            Console.WriteLine(" ")
        Catch
        End Try


    End Sub

    Function killoldvolte() As Integer
        Dim count As Integer
        Dim killid As Integer
        displaylog("kill old volte sessions", "g")

        count = 1


        Do While count <= 2
            killid = voltehandle(count)
            Try
                Dim pProcessTemp As System.Diagnostics.Process = Process.GetProcessById(killid)
                displaylog("kill process id:" & killid & "  process name:" & pProcessTemp.ProcessName, "g")
                pProcessTemp.Kill()
                pProcessTemp.Close()
                count = count + 1
                'Shell("taskkill /F /IM ftp" & UEip & ".exe")
                'Shell("taskkill /F /IM shortping.exe")
                ' Shell("start d:\mueauto\killuesoft.bat")
            Catch e As Exception
                displaylog(e.Message, "r")
                count = count + 1
                Return -1
            End Try
        Loop

        Return 1
    End Function

    Private Sub Form1_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        Dim a As Integer = 0
    End Sub

    Private Sub Form1_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        saveinput()
        ConsoleHelper.FreeConsole()
        killoldvolte()
    End Sub
    Private Sub saveinput()
        Module1.WriteKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "ueip", ueip.Text)
        Module1.WriteKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "QCI1server", QCI1ip.Text)
        Module1.WriteKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "QCI5server", QCI5ip.Text)
        Module1.WriteKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "QCI1size", QCI1size.Text)
        Module1.WriteKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "QCI5size", QCI5size.Text)
        Module1.WriteKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "QCI1int", QCI1interval.Text)
        Module1.WriteKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "QCI5int", QCI5interval.Text)
        Module1.WriteKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "HRpingdir", hrpingdir.Text)
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Dim a As Integer = 1
    End Sub
    Private Sub initinputs()
        If File.Exists(My.Application.Info.DirectoryPath & "\volte.ini") Then
            Try
                If Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "ueip") <> "" Then
                    ueip.Text = Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "ueip")
                Else
                    ueip.Text = "172.24.139.44"
                End If

                If Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "QCI1server") <> "" Then
                    QCI1ip.Text = Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "QCI1server")
                Else
                    QCI1ip.Text = "172.24.186.199"
                End If
                If Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "QCI5server") <> "" Then
                    QCI5ip.Text = Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "QCI5server")
                Else
                    QCI5ip.Text = "172.24.186.199"
                End If
                If Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "QCI1size") <> "" Then
                    QCI1size.Text = Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "QCI1size")
                Else
                    QCI1size.Text = "109"
                End If
                If Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "QCI5size") <> "" Then
                    QCI5size.Text = Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "QCI5size")
                Else
                    QCI5size.Text = "100"
                End If
                If Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "QCI5int") <> "" Then
                    QCI5interval.Text = Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "QCI5int")
                Else
                    QCI5interval.Text = "300"
                End If
                If Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "QCI1int") <> "" Then
                    QCI1interval.Text = Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "QCI1int")
                Else
                    QCI1interval.Text = "20"
                End If
                If Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "HRpingdir") <> "" Then
                    hrpingdir.Text = Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\volte.ini", "info", "HRpingdir")
                Else
                    hrpingdir.Text = "d:\mueauto"
                End If
            Catch ex As Exception
                Dim a As Integer = 1
            End Try
        End If
    End Sub
    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        initinputs()
        ConsoleHelper.openconsole(Me.Handle, Me.Width, Me.Height)
        ConsoleHelper.hideconsole()
        Me.Visible = False
        QCI5speed.Text = "QCI5 speed:" + calspeed(QCI5interval.Text, QCI5size.Text)
        QCI1speed.Text = "QCI1 speed:" + calspeed(QCI1interval.Text, QCI1size.Text)
    End Sub

    Private Sub QCI5size_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QCI5size.TextChanged
        QCI5speed.Text = "QCI5 speed:" + calspeed(QCI5interval.Text, QCI5size.Text)

    End Sub

    Private Sub QCI5interval_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QCI5interval.TextChanged
        QCI5speed.Text = "QCI5 speed:" + calspeed(QCI5interval.Text, QCI5size.Text)

    End Sub

    Private Sub QCI1size_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QCI1size.TextChanged

        QCI1speed.Text = "QCI1 speed:" + calspeed(QCI1interval.Text, QCI1size.Text)
    End Sub

    Private Sub QCI1interval_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QCI1interval.TextChanged

        QCI1speed.Text = "QCI1 speed:" + calspeed(QCI1interval.Text, QCI1size.Text)
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If Button2.Text = "Show console" Then
            Button2.Text = "Hide console"
            ConsoleHelper.showconsole()
        Else
            Button2.Text = "Show console"
            ConsoleHelper.hideconsole()
        End If

    End Sub
    Private Function getCheckNum(ByVal str As String, ByVal regexstr As String) As Boolean

        Dim regex As Regex = New Regex(regexstr)
        If regex.IsMatch(str) Then
            Return True
        Else
            Return False
        End If
    End Function
    Private Function checkvalueOK(ByVal str As String, ByRef strtype As String) As Boolean
        Dim regexstr As String

        If strtype = "datetime" Then

            regexstr = "^((((1[6-9]|[2-9]\d)\d{2})/(0?[13578]|1[02])/(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})/(0?[13456789]|1[012])/(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})/0?2/(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d$"

            Return getCheckNum(str, regexstr)
        End If

        If strtype = "ipadress" Then
            regexstr = "^((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))$"
            Return getCheckNum(str, regexstr)
        End If

        If strtype = "port" Then
            regexstr = "^(\d{5}|\d{6})$"
            Return getCheckNum(str, regexstr)
        End If

        If strtype = "speed" Then

            regexstr = "^[1-9]?$|^0.[1-9]?$"
            Return getCheckNum(str, regexstr)
        End If

        If strtype = "int" Then
            regexstr = "^[1-9]\d*$"
            Return getCheckNum(str, regexstr)
        End If
    End Function




    Private Sub ueip_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ueip.TextChanged

    End Sub
End Class
Public Class ConsoleHelper
    <Runtime.InteropServices.DllImport("kernel32.dll")> _
    Public Shared Function AllocConsole() As Int32
    End Function


    <Runtime.InteropServices.DllImport("kernel32.dll")> _
    Public Shared Function FreeConsole() As Int32
    End Function


    Public Delegate Function HandlerRoutine(ByVal dwCtrlType As Integer) As Boolean


    <Runtime.InteropServices.DllImport("kernel32.dll")> _
    Public Shared Function SetConsoleCtrlHandler(ByVal hr As HandlerRoutine, ByVal Add As Boolean) As Boolean
    End Function


    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Public Shared Function FindWindowEx(ByVal parentHandle As IntPtr, _
                      ByVal childAfter As IntPtr, _
                      ByVal lclassName As String, _
                      ByVal windowTitle As String) As IntPtr
    End Function


    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Public Shared Function DeleteMenu(ByVal hMenu As Integer, _
       ByVal uPosition As Integer, ByVal uFlags As Integer) As Boolean
    End Function


    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Public Shared Function GetSystemMenu(ByVal hWnd As Integer, _
       ByVal bRevert As Boolean) As Integer
    End Function
    <DllImport("User32.dll", EntryPoint:="GetWindowLong")> _
    Friend Shared Function GetWindowLong(ByVal HWND As IntPtr, ByVal Index As Integer) As Integer
    End Function

    Private Const GWL_STYLE = (-16)
    Private Const WS_CAPTION = &HC00000
    Private Const WS_THICKFRAME = &H40000
    Const SWP_SHOWWINDOW = &H40
    Const SWP_HIDEWINDOW = &H80


    Public Shared Sub setintoform(ByVal hwndnewparent As IntPtr, ByVal width As Integer, ByVal height As Integer)
        Dim conHandler As Integer = ConsoleHelper.FindWindowEx(0, 0, "ConsoleWindowClass", Console.Title)
        'Dim y As Long
        'SetWindowLong(conHandler, GWL_STYLE, GetWindowLong(conHandler, GWL_STYLE) And Not WS_CAPTION And Not WS_THICKFRAME)
        'SetParent(conHandler, hwndnewparent)
        'Console.WindowLeft = 0
        'Console.WindowTop = 0
        'SendMessage(conHandler, &H112, 61488, 0)
        'SetWindowPos(conHandler, -1, 0, 0, 525, 350, &H4)
    End Sub
    Private Declare Function SendMessage Lib "user32.dll" Alias "SendMessageA" (ByVal hwnd As Int32, ByVal wMsg As Int32, ByVal wParam As Int32, ByVal lParam As Int32) As Int32
    Declare Function SetParent Lib "user32" Alias "SetParent" (ByVal hWndChild As IntPtr, ByVal hWndNewParent As IntPtr) As Integer
    Declare Function SetWindowLong Lib "user32" Alias "SetWindowLongA" (ByVal hwnd As IntPtr, ByVal nIndex As Integer, ByVal dwNewLong As Integer) As Long
    ' Declare Function getwindowlong Lib "user32" Alias "GetWindowLongA" (ByRef hwnd As IntPtr, ByVal nindex As Integer) As Integer
    Private Declare Function SetWindowPos Lib "user32" (ByVal hwnd As Long, ByVal hWndInsertAfter As Long, ByVal x As Long, ByVal y As Long, ByVal cx As Long, ByVal cy As Long, ByVal wFlags As Long) As Long
    Private Declare Function MoveWindow Lib "user32" (ByVal hwnd As Long, ByVal x As Long, ByVal y As Long, ByVal nWidth As Long, ByVal nHeight As Long, ByVal bRepaint As Long) As Long
    Private Declare Function ShowWindow Lib "user32" (ByVal hwnd As Long, ByVal nCmdShow As Long) As Integer
    Const SW_HIDE = 0
    Const SW_SHOW = 5

    Public Shared Function changetitle(ByVal newtitle As String)
        Console.Title = newtitle
        Return newtitle
    End Function


    Public Shared Sub displaylog(ByVal inputstring As String, Optional ByVal color As String = "g")
        Select Case color
            Case "g"
                Console.ForegroundColor = ConsoleColor.Green
                Console.WriteLine(inputstring)
            Case "r"
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine(inputstring)
        End Select


    End Sub
    Public Shared Sub openconsole(ByVal formhandle As IntPtr, ByVal width As Integer, ByVal height As Integer)
        ConsoleHelper.AllocConsole()


        ' ConsoleHelper.SetConsoleCtrlHandler(New ConsoleHelper.HandlerRoutine(AddressOf HandleCtrlKey), True)


        Dim conHandler As IntPtr = ConsoleHelper.FindWindowEx(0, 0, "ConsoleWindowClass", Console.Title)
        'handlercon = conHandler
        'Dim sysMenuHandler As Integer = ConsoleHelper.GetSystemMenu(conHandler, False)
        ' ConsoleHelper.DeleteMenu(sysMenuHandler, 6, 102)
        'ConsoleHelper.setintoform(formhandle, width, height)
    End Sub

    Public Shared Sub hideconsole()

        ' ConsoleHelper.SetConsoleCtrlHandler(New ConsoleHelper.HandlerRoutine(AddressOf HandleCtrlKey), True)


        Dim conHandler As IntPtr = ConsoleHelper.FindWindowEx(0, 0, "ConsoleWindowClass", Console.Title)
        ShowWindow(conHandler, SW_HIDE)
        'handlercon = conHandler
        'Dim sysMenuHandler As Integer = ConsoleHelper.GetSystemMenu(conHandler, False)
        ' ConsoleHelper.DeleteMenu(sysMenuHandler, 6, 102)
        'ConsoleHelper.setintoform(formhandle, width, height)
    End Sub

    Public Shared Sub showconsole()

        ' ConsoleHelper.SetConsoleCtrlHandler(New ConsoleHelper.HandlerRoutine(AddressOf HandleCtrlKey), True)


        Dim conHandler As IntPtr = ConsoleHelper.FindWindowEx(0, 0, "ConsoleWindowClass", Console.Title)
        ShowWindow(conHandler, SW_SHOW)
        'handlercon = conHandler
        'Dim sysMenuHandler As Integer = ConsoleHelper.GetSystemMenu(conHandler, False)
        ' ConsoleHelper.DeleteMenu(sysMenuHandler, 6, 102)
        'ConsoleHelper.setintoform(formhandle, width, height)
    End Sub

    Public Sub New()

    End Sub
End Class