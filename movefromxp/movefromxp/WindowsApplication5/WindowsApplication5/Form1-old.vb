Imports VB = Microsoft.VisualBasic
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Management
Imports System.Net.NetworkInformation
Imports System.Runtime.InteropServices
Imports System.Net
Imports System.Text
Imports System.Threading
Imports NTCPMSG
Imports NTCPMSG.Client
Imports NTCPMSG.Event
Imports Microsoft.Win32  '用途 ： 注册表操作 
Imports System.Security.AccessControl
'-l -t H -s 192.168.18.1 -s2 192.168.18.2 -s3 192.168.18.3 -s4 192.168.18.4 -p com6 -i 60 -d 100 -w 127.0.0.1 -g 193.168.154.100 -n 2 -T ftpdl -a 10

Public Class Form1
    Private WithEvents MyClient As TCPClient
    Dim Mytcpclient As NTCPMSG.Client.SingleConnection
    Dim logname, TPlogname, accesstype As String
    Dim UEtype, serverip, a_interval, interval, action, ftpsessionnum, UEip, UEname, serialportname, traffictype, logip, serverip2, serverip3, serverip4, androiddevid As String
    Dim port As Integer = 2500
    Dim exitwindow As Boolean
    Dim ftphandle(8), pinghandle(8), videohandle(8), voltehandle(4), httphandle, volteadbahhandle As Integer
    Dim calldroped As Boolean
    Dim realtimes, trytimes, successtimes As Integer
    Dim locked As Boolean
    Dim syncflag As Boolean = True
    Dim DLthroughput, ULthroughput As PerformanceCounter
    Dim DLthroughputhis, ULthroughputhis As Int64
    Dim dlcounter(2) As Int64
    Dim dlcounterfilter(2) As Int64
    Dim ulcounter(2) As Int64
    Dim ulcounterfilter(2) As Int64
    Dim TPinterval, TPintervalcounter As Integer
    Dim TPintervalsecond, TPintervalcountersecond As Integer
    Dim DLthroughputsecond, ULthroughputsecond As Int64
    Dim messagebuffer As New List(Of String)
    Dim imsi As String = ""
    Dim totalcount, count As Integer
    Dim regsearch(5) As String
    Private Declare Function SetWindowTextA Lib "user32" (ByVal hwnd As Long, ByVal lpString As String) As Long
    Sub ReceiveEventHandler1(ByVal sender As Object, ByVal e As NTCPMSG.Event.ReceiveEventArgs)
        Dim a = 1
        Console.WriteLine(Encoding.ASCII.GetString(e.Data))
    End Sub
    Sub ErrorEventHandler1(ByVal sender As Object, ByVal e As NTCPMSG.Event.ErrorEventArgs)
        Dim a = 0
        a = Console.CursorLeft
        Console.Write("?")
        Console.CursorLeft = a
        syncflag = False

    End Sub
    Sub RemoteDisconnected1(ByVal sender As Object, ByVal e As NTCPMSG.Event.DisconnectEventArgs)
        Dim a = 0
        a = Console.CursorLeft
        Console.Write("?")
        Console.CursorLeft = a
        syncflag = False
    End Sub
    Sub TCPconnect()
        Try
            Mytcpclient = New SingleConnection(logip, port)
            AddHandler Mytcpclient.ReceiveEventHandler, AddressOf ReceiveEventHandler1
            AddHandler Mytcpclient.ErrorEventHandler, AddressOf ErrorEventHandler1
            AddHandler Mytcpclient.RemoteDisconnected, AddressOf RemoteDisconnected1
            Mytcpclient.Connect(100)
        Catch ex As Exception
            Dim a As Integer = 1



        End Try

    End Sub
    Sub TCPwrite(ByVal message As String)
        Dim receivestr As String
        Dim a As Integer = 0
        'Console.WriteLine(message)
        Try
            If Mytcpclient Is Nothing Then
                TCPconnect()
            End If

            If syncflag = False Or Mytcpclient.Connected = False Then
                Mytcpclient.Close()
                Mytcpclient.Connect(100)
            End If
            If Mytcpclient.Connected = True Then
                While messagebuffer.Count > 0
                    Dim retData1() As Byte = Mytcpclient.SyncSend(2, Encoding.ASCII.GetBytes(messagebuffer.Item(0)), 100)
                    receivestr = Encoding.ASCII.GetString(retData1)
                    If Trim(receivestr).IndexOf("~") >= 0 Then
                        syncflag = True


                        a = Console.CursorLeft
                        Console.Write("!") '("receive things:" + Trim(Encoding.ASCII.GetString(value)))
                        Console.CursorLeft = a
                        messagebuffer.RemoveAt(0)
                    Else
                        syncflag = False

                        Exit Sub
                    End If

                End While
                Dim retData() As Byte = Mytcpclient.SyncSend(2, Encoding.ASCII.GetBytes(message), 100)
                receivestr = Encoding.ASCII.GetString(retData)
                If Trim(receivestr).IndexOf("~") >= 0 Then
                    syncflag = True

                    a = Console.CursorLeft
                    Console.Write("!") '("receive things:" + Trim(Encoding.ASCII.GetString(value)))
                    Console.CursorLeft = a

                Else
                    syncflag = False
                End If


            End If
        Catch ex As Exception
            a = Console.CursorLeft
            Console.Write("?")
            Console.CursorLeft = a
            While messagebuffer.Count > 1000
                messagebuffer.RemoveAt(0)
            End While
            messagebuffer.Add(message)


        End Try


    End Sub

    Private Sub Form1_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        exitwindow = True
    End Sub

    Private Sub Form1_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        exitwindow = True
        ConsoleHelper.FreeConsole()



    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Dim count, killid As Integer
        count = 1
        exitwindow = True
        'If ftphandle(0) <> Nothing Then
        '    Do While count <= Int(ftpsessionnum)
        '        killid = ftphandle(count - 1)
        '        Try
        '            Dim pProcessTemp As System.Diagnostics.Process = Process.GetProcessById(killid)
        '            'displaylog("kill process id:" & killid & "  process name:" & pProcessTemp.ProcessName, "g")
        '            pProcessTemp.Kill()
        '            pProcessTemp.Close()
        '            count = count + 1
        '            'Shell("taskkill /F /IM ftp" & UEip & ".exe")
        '            'Shell("taskkill /F /IM shortping.exe")
        '            myprocess.killprocess("ftp" & UEip)
        '            myprocess.killprocess("shortping")
        '            myprocess.killprocessbyimportfile("d:\mueauto\killuesoft.cfg")
        '        Catch e1 As Exception
        '            'displaylog(e1.Message, "r")
        '            count = count + 1
        '        End Try
        '    Loop
        'End If
        If traffictype = "ftpdl" Or traffictype = "ftpul" Or traffictype = "ftpdlul" Then killoldftp()
        If traffictype = "ping" Then killoldping()
        If traffictype = "video" Then killvideo()
        If traffictype = "volte" Then killoldvolte()
        If traffictype = "http" Then killoldhttp()
        If traffictype = "httpdownload" Then killoldhttpdownload()
        exitwindow = True
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        '------------------------------------------------------------------------------------------------------
        exitwindow = False
        locked = False
        '----init com default configuration
        'SerialPort.Encoding = System.Text.Encoding.Default
        SerialPort.PortName = "COM8"
        SerialPort.BaudRate = 115200
        SerialPort.StopBits = IO.Ports.StopBits.One
        SerialPort.Parity = IO.Ports.Parity.None
        SerialPort.DataBits = 8
        '----

        UEtype = "H"
        UEip = "192.168.79.11"
        serverip = "10.60.10.1"
        serverip2 = ""
        serverip3 = ""
        serverip4 = ""
        logip = "192.168.0.110"
        interval = "100"
        action = "P" ' P- longtime run, L--attach and dettach loop
        a_interval = "60"
        logname = "192.168.79.11.log"
        TPlogname = "192.168.79.11.TP.txt"
        ftpsessionnum = "2"
        traffictype = "ftpdl"
        TPinterval = 30
        TPintervalsecond = 1
        TPintervalcounter = 1
        TPintervalcountersecond = 1
        serialportname = "COM8"
        androiddevid = "A710e"
        '------------------------
        realtimes = 0
        trytimes = 0
        successtimes = 0

        '        WebBrowser1.DocumentText = WebBrowser1.DocumentText & "<html> <body bgcolor=""#000000""> </body> </html>" + vbNewLine
        'ConsoleHelper.openconsole(Panel1.Handle, Panel1.Width, Panel1.Height)
        ConsoleHelper.openconsole(Me.Handle, Me.Width, Me.Height)
        Console.Title = "MUEclient:"
        myprocess.killprocessbyimportfile("d:\mueauto\killuesoft.cfg")
        'addroute()
        Me.Visible = False
        checkhrpingreg()
        runmain()

    End Sub
    Sub checkhrpingreg()

        'VB.NET读写注册表3, 创建或读取注册表项
        Dim Key1 As Microsoft.Win32.RegistryKey
        Key1 = My.Computer.Registry.CurrentUser '返回当前用户键  

        Dim Key2 As Microsoft.Win32.RegistryKey
        Key2 = Key1.OpenSubKey("Software\cFos", True) '返回当前用户键下的northsnow键,如果想创建项，必须指定第二个参数为true  
        If Key2 Is Nothing Then
            Key2 = Key1.CreateSubKey("Software\cFos") '如果键不存在就创建它  
        End If
        Key2 = Key1.OpenSubKey("Software\cFos\hrPING", True) '返回当前用户键下的northsnow键,如果想创建项，必须指定第二个参数为true  
        If Key2 Is Nothing Then
            Key2 = Key1.CreateSubKey("Software\cFos\hrPING") '如果键不存在就创建它  
        End If
        Key2 = Key1.OpenSubKey("Software\cFos\hrPING\5.04", True) '返回当前用户键下的northsnow键,如果想创建项，必须指定第二个参数为true  
        If Key2 Is Nothing Then
            Key2 = Key1.CreateSubKey("Software\cFos\hrPING\5.04") '如果键不存在就创建它  
        End If

        '创建项，如果不存在就创建，如果存在则覆盖  
        Key2.SetValue("LicenseAccepted", "1", Microsoft.Win32.RegistryValueKind.DWord)



    End Sub

    ' Private Shared Sub OnTimedEvent(ByVal source As Object, ByVal e As System.EventArgs)
    '    Console.WriteLine("The Elapsed event was raised at {0}")
    'End Sub


    Private Function SerialPortOpen(ByVal logfilename As String, ByVal firstime As Boolean) As Boolean
        'displaylog(SerialPort.PortName, "r")

        Try
            ' SerialPort.Close()
            If SerialPort.IsOpen = False Then SerialPort.Open() '避免重复打开端口

            'SerialPort.Open()
            displaylog("port opened", "g")
            Return True
            Exit Function

        Catch exp As Exception
            displaylog(SerialPort.PortName + " port fail" + vbNewLine + ErrorToString(), "r")
            If firstime = True Then End
            myprocess.killprocessbyimportfile("d:\mueauto\killuesoft.cfg")

            ' SerialPort.Dispose()
            'SerialPort = New System.IO.Ports.SerialPort
            displaylog("wait 60s, auto restart program ", "g")
            Timer4.Enabled = False
            Mytcpclient.Close()
            wait(60)
            Timer2.Enabled = False
            Try
                Me.Form1_FormClosing(Nothing, Nothing)
                Me.Form1_FormClosed(Nothing, Nothing)
                Dim myprocess As Process = New Process()

                myprocess.StartInfo.FileName = "d:\mueauto\mueclient.exe"
                Dim commandline As String()
                commandline = System.Environment.GetCommandLineArgs()
                Dim commandlinestr As String = ""
                For i = 1 To UBound(commandline)
                    commandlinestr = commandlinestr + commandline(i) + " "
                Next
                myprocess.StartInfo.Arguments = commandlinestr
                'myprocess.StartInfo.Arguments = serverip + " -t"
                myprocess.Start()
                Process.GetCurrentProcess().Kill()

            Catch ex As Exception

            End Try

        End Try



    End Function

    Private Sub serialSendData(ByVal command As String, ByVal logfilename As String)
        Dim hexsendFlag As Boolean

        Try



            Dim outDataBuf As String = command

            hexsendFlag = False

            If outDataBuf = "" Then Exit Sub '如果输入文本框中没有数据则不发送

            If SerialPort.IsOpen = True Then '判断串口是否打开

                If hexsendFlag = True Then

                    '-----------十六进制发送------------

                    outDataBuf = outDataBuf.Replace(" ", "") '清除空格与回车

                    outDataBuf = outDataBuf.Replace(vbNewLine, "")

                    '十六进制数据位数为偶数，例如：FF 00 15 AC 0D

                    If outDataBuf.Length Mod 2 <> 0 Then

                        displaylog("请输入正确的十六进制数，用空格和回车隔开。", "r")
                        Exit Sub

                    End If

                    Dim outBytes(outDataBuf.Length / 2 - 1) As Byte

                    For I As Integer = 1 To outDataBuf.Length - 1 Step 2

                        outBytes((I - 1) / 2) = Val("&H" + Mid(outDataBuf, I, 2)) 'VB的十六进制表示方法，例如0x1D表示为&H1D

                    Next

                    SerialPort.Write(outBytes, 0, outDataBuf.Length / 2)

                    'BarCountTx.Text = Val(BarCountTx.Text) + outDataBuf.Length / 2

                Else

                    '-------------文本发送--------------

                    SerialPort.Write(outDataBuf)
                    ' Module1.writelog(outDataBuf, 0, logfilename)


                    'BarCountTx.Text = Val(BarCountTx.Text) + outDataBuf.Length '发送字节计数

                End If

            Else
                displaylog("串口未打开，请先打开串口。", "r")
                SerialPortOpen(logname, False)

            End If

            Exit Sub
        Catch ex As Exception
            displaylog("数据输入或发送错误！" + vbNewLine + ErrorToString(), "r")
            myprocess.killprocessbyimportfile("d:\mueauto\killuesoft.cfg")
            If SerialPort.IsOpen Then
                Try
                    SerialPort.Close()
                Catch
                End Try
            End If

            displaylog("wait 60s, auto restart program ", "g")
            wait(60)
            myprocess.killprocessbyimportfile("d:\mueauto\killuesoft.cfg")
            Application.Restart()

        End Try

    End Sub

    Private Sub SerialPort_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort.DataReceived
        Dim HexRecieveFlag As Boolean
        HexRecieveFlag = False
        If HexRecieveFlag Then

            '-----------十六进制显示------------

            Dim inDataLen As Integer = SerialPort.BytesToRead() '获取可读取的字节数

            If inDataLen > 0 Then

                Dim inBytes(inDataLen - 1) As Byte, bytes As Byte

                Dim strHex As String = ""

                SerialPort.Read(inBytes, 0, inDataLen) '读取数据

                For Each bytes In inBytes

                    strHex = strHex + [String].Format("{0:X2} ", bytes) '格式化成十六进制（不含&H）

                Next

                Invoke(RecieveRefresh, strHex) '调用委托，显示接收的数据

                ' BarCountRx.Text = (Val(BarCountRx.Text) + inDataLen).ToString '接收字节计数

            End If

        Else

            '-------------文本显示--------------

            Dim str As String

            str = SerialPort.ReadExisting '读取全部可用字符串

            If exitwindow = False And (Not (str.IndexOf("DSFLOWRPT") >= 0)) Then

                str = findip(str)


                Invoke(RecieveRefresh, str)
            End If

            ' BarCountRx.Text = (Val(BarCountRx.Text) + str.Length).ToString '接收字节计数

        End If

    End Sub
    Function findip(ByVal inputstr As String) As String
        Dim regexstr As String
        regexstr = "((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))"
        Dim regex As Regex = New Regex(regexstr)
        If regex.IsMatch(inputstr) Then
            Return inputstr + vbCrLf + "ip=" + regex.Match(inputstr, regexstr).Groups(0).Value
        Else
            Return inputstr
        End If


    End Function

    Private Function checkvalueOK(ByVal str As String, ByRef strtype As String) As Boolean

    End Function



    Private Function getCheckNum(ByVal str As String, ByVal regexstr As String) As Boolean


    End Function



    Delegate Sub RecieveRefreshMethodDelegate(ByVal [text] As String) '声明委托

    Dim RecieveRefresh As New RecieveRefreshMethodDelegate(AddressOf RecieveRefreshMethod) '定义数据显示委托实例

    Sub RecieveRefreshMethod(ByVal str As String) '定义一个数据显示委托实例的方法
        If Trim(str) <> "" Then showRecievedata(str)

    End Sub

    Sub showRecievedata(ByVal str As String)

        displaylog2(str, "g")

    End Sub


    Sub attachdetach(ByVal uetype As String, ByVal attachtime As Integer)


        displaylog("attatchdetach run", "r")
        'Shell("start d:\mueauto\killuesoft.bat")
        '-------------- UE detach+attach-----------------
        'Select Case uetype
        '    Case "H"
        '        attachdetach_HISI()
        '    Case "HE5776"
        '        attachdetach_HISI()

        '    Case "Qualcomm9600"
        '        'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
        '        attachdetach_Qualcomm(0)
        '    Case "Qualcomm9028"
        '        attachdetach_Qualcomm(9028)

        '    Case "BandluxeC508"
        '        attachdetach_Bandrich()

        '    Case "ALT-C186"
        '        attachdetach_ALT()
        '    Case "dialcomm"
        '        attachdetach_comm()
        'End Select

        Dim attachstate As Boolean = True
        Do 'UE attach until get add route ok
            attachstate = True
            '-------------- UE detach+attach-----------------
            Select Case uetype
                Case "H"
                    If attachdetach_HISI() <> "OK" Then
                        attachstate = False
                    End If

                Case "HE5776"
                    If attachdetach_HISI() <> "OK" Then
                        attachstate = False
                    End If

                Case "Qualcomm9600"
                    'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
                    attachdetach_Qualcomm("9600")

                Case "Qualcomm9028"
                    'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
                    attachdetach_Qualcomm("9028")
                Case "BandluxeC508"
                    attachdetach_Bandrich()

                Case "ALT-C186"
                    attachdetach_ALT()

                Case "dialcomm"
                    attachdetach_comm()
            End Select
            trytimes = trytimes + 1
            displaylog("attach try times:" + trytimes.ToString, "r")
            If addroute() = "ip not find" Then
                attachstate = attachstate * False
            End If
            If (uetype = "H" Or uetype = "HE5776") And attachstate = True Then
                If monitoring(True) <> "OK" Then
                    attachstate = False
                End If
            End If
            attachstate = attachstate * Not (exitwindow)
            If attachstate = False Then
                displaylog("attach fail", "r")
            End If
        Loop Until attachstate = True




        If traffictype = "ftpdl" Or traffictype = "ftpul" Or traffictype = "ftpdlul" Then
            '-----------run ftp
            wait(5)
            If rundlftp() = "no ip no traffic" Then
                wait(10)
                If rundlftp() = "no ip no traffic" Then
                    ' wait(attachtime)
                    realtimes = realtimes + 1
                    trytimes = trytimes + 1
                    displaylog(" realtimes=" + realtimes.ToString + " trytimes=" + trytimes.ToString + " successtimes=" + successtimes.ToString + " successrate=" + (Int(successtimes / trytimes * 100)).ToString + "%", "r")

                    Return
                End If
            End If

        End If
        If traffictype = "http" Then
            '-----------run http
            wait(5)
            If rundlhttp() = "no ip no traffic" Then
                wait(10)
                If rundlhttp() = "no ip no traffic" Then
                    'wait(attachtime)
                    realtimes = realtimes + 1
                    trytimes = trytimes + 1
                    displaylog(" realtimes=" + realtimes.ToString + " trytimes=" + trytimes.ToString + " successtimes=" + successtimes.ToString + " successrate=" + (Int(successtimes / trytimes * 100)).ToString + "%", "r")

                    Return
                End If
            End If
        End If
        If traffictype = "httpdownload" Then
            '-----------run httpdownload
            wait(5)
            If rundlhttpdownload() = "no ip no traffic" Then
                wait(10)
                If rundlhttpdownload() = "no ip no traffic" Then
                    'wait(attachtime)
                    realtimes = realtimes + 1
                    trytimes = trytimes + 1
                    displaylog(" realtimes=" + realtimes.ToString + " trytimes=" + trytimes.ToString + " successtimes=" + successtimes.ToString + " successrate=" + (Int(successtimes / trytimes * 100)).ToString + "%", "r")

                    Return
                End If
            End If

        End If
        If traffictype = "ping" Then
            '-----------run ping
            wait(5)
            If runping() = "no ip no traffic" Then
                wait(10)
                If runping() = "no ip no traffic" Then
                    wait(attachtime)
                    realtimes = realtimes + 1
                    trytimes = trytimes + 1
                    displaylog(" realtimes=" + realtimes.ToString + " trytimes=" + trytimes.ToString + " successtimes=" + successtimes.ToString + " successrate=" + (Int(successtimes / trytimes * 100)).ToString + "%", "r")

                    Return
                End If
            End If
        End If
        If traffictype = "video" Then
            '-----------run video
            wait(5)
            If runvideo() = "no ip no traffic" Then
                wait(10)
                If runvideo() = "no ip no traffic" Then
                    wait(attachtime)
                    realtimes = realtimes + 1
                    trytimes = trytimes + 1
                    displaylog(" realtimes=" + realtimes.ToString + " trytimes=" + trytimes.ToString + " successtimes=" + successtimes.ToString + " successrate=" + (Int(successtimes / trytimes * 100)).ToString + "%", "r")

                    Return
                End If
            End If

        End If

        If traffictype = "volte" Then
            '-----------run volte
            wait(5)
            If runvolte() = "no ip no traffic" Then
                wait(10)

                If runvolte() = "no ip no traffic" Then
                    wait(attachtime)
                    realtimes = realtimes + 1
                    trytimes = trytimes + 1
                    displaylog(" realtimes=" + realtimes.ToString + " trytimes=" + trytimes.ToString + " successtimes=" + successtimes.ToString + " successrate=" + (Int(successtimes / trytimes * 100)).ToString + "%", "r")

                    Return
                End If
            End If

        End If
        '-----------wait
        displaylog("start wait:" + attachtime.ToString + "s", "g")

        wait(attachtime)
        '-----------monitoring call drop
        monitoring()
        '-----------caculate the counter
        realtimes = realtimes + 1
        If calldroped = False Then
            trytimes = trytimes + 1
            successtimes = successtimes + 1

        End If
        If calldroped = True And trytimes <> 0 Then
            trytimes = trytimes + 1
        End If
        If trytimes > 0 Then
            displaylog(" realtimes=" + realtimes.ToString + " trytimes=" + trytimes.ToString + " successtimes=" + successtimes.ToString + " successrate=" + (Int(successtimes / trytimes * 100)).ToString + "%", "r")
            wait(2)
        End If
        If traffictype = "ftpdl" Or traffictype = "ftpul" Or traffictype = "ftpdlul" Then killoldftp()
        If traffictype = "ping" Then killoldping()
        If traffictype = "video" Then killvideo()
        If traffictype = "volte" Then killoldvolte()
        If traffictype = "http" Then killoldhttp()
        If traffictype = "httpdownload" Then killoldhttpdownload()
    End Sub
    Function killoldhttpdownload()
        Dim count As Integer
        Dim killid As Integer
        displaylog("kill old http", "g")
        'Shell("taskkill /F /IM ftp" & UEip & ".exe")
        'Shell("taskkill /F /IM shortping.exe")
        'myprocess.killprocess("ftp" & UEip)
        myprocess.killprocess("shortping")
        count = 1



        killid = httphandle
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

        Return 1
    End Function

    Function reset_qualcomm() As String
        serialSendData("AT+CGATT=0" & vbCrLf, logname)
        displaylog("AT+CGATT=0", "g")
        wait(5)

        serialSendData("AT+CFUN=6" & vbCrLf, logname)
        displaylog("AT+CFUN=6", "g")
        wait(15)
        reset_qualcomm = "OK"
    End Function



    Function reset_HISI5776() As String
        If SerialPortOpen(logname, False) Then

            If UEtype = "H" Then
                serialSendData("AT+CGATT=0" & vbCrLf, logname)
                displaylog("AT+CGATT=0", "g")
                wait(5)

                serialSendData("AT^PSPOWER=0" & vbCrLf, logname)
                displaylog("AT^PSPOWER=0", "g")
                wait(15)


            End If
            If UEtype = "HE5776" Then
                serialSendData("AT+CGATT=0" & vbCrLf, logname)
                displaylog("AT+CGATT=0", "g")
                wait(5)

                serialSendData("AT+CFUN=0" & vbCrLf, logname)
                displaylog("AT+CFUN=0", "g")
                wait(5)

                serialSendData("AT+CFUN=0" & vbCrLf, logname)
                displaylog("AT^RESET", "g")
                wait(5)

                'Dim IPAddress As String = ""
                'Dim gateway As String = ""
                'Dim netportname As String = SerialPort.PortName
                'getcomip(netportname, True, IPAddress, gateway)
                'If gateway <> "" Then
                '    Console.WriteLine("send login name and password")
                '    Dim sbTemp As System.Text.StringBuilder = New System.Text.StringBuilder()
                '    sbTemp.Append("<request><Username>admin</Username><Password>YWRtaW4=</Password></request>")
                '    Dim bTemp() As Byte = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(sbTemp.ToString())
                '    Dim postReturn As String = doPostRequest("http://" + gateway + "/api/user/login", bTemp)
                '    Console.WriteLine("Post response is: " + postReturn)
                '    Console.WriteLine("send reset command")
                '    Dim sbTemp2 As System.Text.StringBuilder = New System.Text.StringBuilder()
                '    sbTemp2.Append("<request><Control>1</Control></request>")
                '    Dim bTemp2() As Byte = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(sbTemp2.ToString())
                '    postReturn = doPostRequest("http://" + gateway + "/api/device/control", bTemp2)
                '    Console.WriteLine("Post response is: " + postReturn)

                '    wait(5)
                'End If

            End If
            reset_HISI5776 = "OK"
        Else
            reset_HISI5776 = "KO"
        End If
    End Function
    Function shutdown_HISI() As String
        If SerialPortOpen(logname, False) Then

            If UEtype = "H" Then
                serialSendData("AT+CGATT=0" & vbCrLf, logname)
                displaylog("AT+CGATT=0", "g")
                wait(5)

                serialSendData("AT^PSPOWER=0" & vbCrLf, logname)
                displaylog("AT^PSPOWER=0", "g")
                wait(15)


            End If
            If UEtype = "HE5776" Then
                serialSendData("AT+CGATT=0" & vbCrLf, logname)
                displaylog("AT+CGATT=0", "g")
                wait(5)

                serialSendData("AT+CFUN=0" & vbCrLf, logname)
                displaylog("AT+CFUN=0", "g")
                wait(5)

            End If
            shutdown_HISI = "OK"
        Else
            shutdown_HISI = "KO"
        End If
    End Function
    Function shutdown_Qualcomm() As String
        'Shell("D:\mueauto\Qclient.exe device=1", , True)
        'Shell("D:\mueauto\Qclient.exe command=Disconnect", , True)
        serialSendData("AT+CGATT=0" & vbCrLf, logname)
        displaylog("AT+CGATT=0", "g")

        wait(5)

        serialSendData("AT+CFUN=0" & vbCrLf, logname)
        displaylog("AT+CFUN=0", "g")
        wait(15)
        Return "OK"

    End Function
    Function shutdown_Bandrich() As String
        If SerialPortOpen(logname, False) Then

            serialSendData("at$wancall=0" & vbCrLf, logname)
            displaylog("at$wancall=0", "g")
            wait(5)

            serialSendData("AT+CGATT=0" & vbCrLf, logname)
            displaylog("AT+CGATT=0", "g")
            wait(5)

            serialSendData("AT+CFUN=0" & vbCrLf, logname)
            displaylog("AT+CFUN=0", "g")
            wait(15)
            Return "OK"
        Else
            Return "KO"
        End If

    End Function
    Function shutdown_ALT() As String
        If SerialPortOpen(logname, False) Then



            'serialSendData("AT+CGATT=0" & vbCrLf, logname)
            serialSendData("AT+CGATT=0", logname)
            displaylog("AT+CGATT=0", "g")
            wait(5)

            'serialSendData("AT+CFUN=0" & vbCrLf, logname)
            serialSendData("AT+CFUN=0", logname)
            displaylog("AT+CFUN=0", "g")
            wait(15)

            Return "OK"
        Else
            Return "KO"
        End If
    End Function

    Function shutdown_comm() As String
        If SerialPortOpen(logname, False) Then

            'serialSendData("AT+CGATT=0" & vbCrLf, logname)
            serialSendData("AT+CGATT=0", logname)
            displaylog("AT+CGATT=0", "g")
            wait(5)

            'serialSendData("AT+CFUN=0" & vbCrLf, logname)
            serialSendData("AT+CFUN=0", logname)
            displaylog("AT+CFUN=0", "g")
            wait(15)

            Return "OK"
        Else
            Return "KO"
        End If
    End Function
    Sub resetUE(ByVal uetype As String)

        displaylog("reset UE", "r")
        'Shell("start d:\mueauto\killuesoft.bat")
        '-------------- UE detach+attach-----------------
        Select Case uetype
            Case "H"
                shutdown_HISI()
            Case "HE5776"
                reset_HISI5776()

            Case "Qualcomm9600"
                'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
                'shutdown_Qualcomm()
                reset_qualcomm()
            Case "Qualcomm9028"
                reset_qualcomm()
            Case "BandluxeC508"
                'shutdown_Bandrich()
                reset_qualcomm()
            Case "ALT-C186"
                'shutdown_ALT()

        End Select
        Application.Exit()

    End Sub

    Sub shutdown(ByVal uetype As String)

        displaylog("shutdown UE", "r")
        'Shell("start d:\mueauto\killuesoft.bat")
        '-------------- UE detach+attach-----------------
        Select Case uetype
            Case "H"
                shutdown_HISI()
            Case "HE5776"
                shutdown_HISI()

            Case "Qualcomm9600"
                'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
                shutdown_Qualcomm()
            Case "Qualcomm9028"
                'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
                shutdown_Qualcomm()
            Case "BandluxeC508"
                shutdown_Bandrich()

            Case "ALT-C186"
                shutdown_ALT()

        End Select
        displaylog("shutdown UE successfully", "r")
        Application.Exit()

    End Sub
    Function addroute() As String
        Dim subipstring, ipaddress, gateway, returnstr As String
        Dim netportname As String

        ipaddress = ""
        gateway = ""
        subipstring = ""
        netportname = SerialPort.PortName
        getcomip(netportname, True, ipaddress, gateway)
        displaylog("gateway is " + gateway, "r")
        If ipaddress = "" Or gateway = "" Then
            displaylog("ip not find", "g")
            Return "ip not find"
            Exit Function
        End If
        displaylog("Clear old route to " & ipaddress, "r")
        returnstr = rundoscommand("route delete " + serverip)
        wait(1)
        displaylog("UE IP:" & ipaddress & "  Gateway:" & gateway, "r")
        displaylog(" " + serverip + " " + gateway + " Add :" + serverip + " " + gateway, "g")
        returnstr = rundoscommand("route add " + serverip + " " + gateway)
        'displaylog(returnstr, "g")
        If traffictype = "volte" Then
            If serverip2 <> "" Then returnstr = rundoscommand("route add " + serverip2 + " " + gateway)
            If serverip3 <> "" Then returnstr = rundoscommand("route add " + serverip3 + " " + gateway)
            If serverip3 <> "" Then returnstr = rundoscommand("route add " + serverip4 + " " + gateway)
        End If
        Return "OK"

    End Function
    Function monitoring(Optional ByVal forattach As Boolean = False) As String
        Dim returnstr As String
        Dim values As String
        values = ""
        displaylog("Checking traffic", "r")
        returnstr = rundoscommand("ping -w 10000 -n " + "5" + " " + serverip)

        If InStr(returnstr, "unreachable") > 0 Then
            values = "100" '= ping fail
        Else
            Dim expression As New Regex("(\d+)%")
            'TextBox1.Text = TextBox1.Text & returnstr & vbNewLine
            Dim mc As MatchCollection = expression.Matches(returnstr)
            For i As Integer = 0 To mc.Count - 1
                values = mc(i).ToString.Split("%")(0)
                displaylog("ping " & mc(i).ToString, "r")

            Next
        End If

        If values = "" Or Int(values) = 100 Then
            calldroped = True
            If forattach = True Then
                displaylog("Can not access app-server:" + serverip + ",reattach.", "r")
            Else
                displaylog("call drop ping fail", "r")
            End If

            Return "fail"
        Else
            calldroped = False
            displaylog("ping OK", "g")
        End If

        If checkroute() = False Then
            calldroped = True
            displaylog("call drop not find route to server", "r")
            Return "fail"
        End If

        Return "OK"

    End Function

    Function checkroute() As Boolean
        Dim returnstr As String
        Dim values As String
        values = ""
        displaylog("Checking route", "r")
        returnstr = rundoscommand("route print " + serverip)

        Dim expression As New Regex(serverip)
        'TextBox1.Text = TextBox1.Text & returnstr & vbNewLine
        Dim mc As MatchCollection = expression.Matches(returnstr)
        If mc.Count = 1 Then
            Return False
        Else
            Return True
        End If

    End Function

    Function rundlftpandroid() As String
        Dim count As Integer
        Dim ftpsessionnumreal As Integer
        rundlftpandroid = "ok"
        If traffictype = "ftpdlulandroid" Then ftpsessionnumreal = ftpsessionnum * 2 Else ftpsessionnumreal = ftpsessionnum

        If Int(ftpsessionnumreal) > 0 Then
            If addroute() <> "ip not find" Then


                count = 1
                displaylog("start new ftp session", "g")

                Do While count <= Int(ftpsessionnumreal)
                    'Dim myProcess As Process = System.Diagnostics.Process.Start("d:\mueauto\ftpdl.bat " + UEip + " " + Str(count) + " " + serverip)
                    Dim myprocess As Process = New Process()
                    If traffictype = "ftpdlandroid" Then
                        myprocess.StartInfo.FileName = "d:\mueauto\ftpdl.bat"
                    Else
                        If traffictype = "ftpulandroid" Then
                            myprocess.StartInfo.FileName = "d:\mueauto\ftpul.bat"
                        End If
                        If traffictype = "ftpdlulandroid" And (count Mod 2 = 1) Then
                            myprocess.StartInfo.FileName = "d:\mueauto\ftpdl.bat"
                        End If
                        If traffictype = "ftpdlulandroid" And (count Mod 2 = 0) Then
                            myprocess.StartInfo.FileName = "d:\mueauto\ftpul.bat"
                        End If
                    End If
                    myprocess.StartInfo.Arguments = UEip + " " + Str(count) + " " + serverip
                    myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized
                    myprocess.Start()
                    ftphandle(count - 1) = myprocess.Id
                    displaylog("ftp session id opened:" & myprocess.Id.ToString, "g")

                    count = count + 1
                Loop
            Else
                displaylog("no ip no traffic", "r")
                Return "no ip no traffic"
            End If

        Else
            displaylog("No ftp session needed", "g")
            Return "No ftp session needed"
        End If

    End Function
    Function rundlftp() As String
        Dim count As Integer
        Dim ftpsessionnumreal As Integer
        Dim username, pass, DLsessionno, ULsessionno, DLfilename, ULfilename, ULremotename As String
        rundlftp = "ok"
        If traffictype = "ftpdlul" Then ftpsessionnumreal = ftpsessionnum * 2 Else ftpsessionnumreal = ftpsessionnum
        '-----------------------------------get ftp configs

        username = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "server", "username")

        pass = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "server", "password")
        DLsessionno = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "DL", "sessionno")
        ULsessionno = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "UL", "sessionno")
        DLfilename = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "DL", "filename")
        ULfilename = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "UL", "filename")
        ULremotename = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "UL", "remotename")

        '---------------------------------------
        If Int(ftpsessionnumreal) > 0 Then
            If addroute() <> "ip not find" Then


                count = 1
                displaylog("start new ftp session", "g")

                Dim myprocess As Process = New Process()
                If traffictype = "ftpdl" Then
                    myprocess.StartInfo.FileName = "d:\mueauto\ftpclient.exe"
                    myprocess.StartInfo.Arguments = serverip + " " + username + " " + pass + " " + "DL" + " " + DLsessionno + " " + DLfilename + " " + serialportname
                    displaylog("ftp parameter:" & serverip + " " + username + " " + pass + " " + "DL" + " " + DLsessionno + " " + DLfilename, "g")
                Else
                    If traffictype = "ftpul" Then
                        myprocess.StartInfo.FileName = "d:\mueauto\ftpclient.exe"
                        myprocess.StartInfo.Arguments = serverip + " " + username + " " + pass + " " + "UL" + " " + ULsessionno + " " + DLfilename + " " + serialportname + " " + ULremotename + " " + ULfilename
                        displaylog("ftp parameter:" & serverip + " " + username + " " + pass + " " + "UL" + " " + ULsessionno + " " + DLfilename + " " + ULremotename + " " + ULfilename, "g")
                    End If
                    If traffictype = "ftpdlul" Then
                        myprocess.StartInfo.FileName = "d:\mueauto\ftpclient.exe"
                        myprocess.StartInfo.Arguments = serverip + " " + username + " " + pass + " " + "DL" + " " + DLsessionno + " " + DLfilename + " " + serialportname
                        displaylog("ftp parameter:" & serverip + " " + username + " " + pass + " " + "DL" + " " + DLsessionno + " " + DLfilename, "g")
                    End If

                End If

                myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized
                myprocess.Start()
                ftphandle(0) = myprocess.Id
                displaylog("ftp session id opened:" & myprocess.Id.ToString, "g")

                If traffictype = "ftpdlul" Then
                    myprocess.StartInfo.FileName = "d:\mueauto\ftpclient.exe"
                    myprocess.StartInfo.Arguments = serverip + " " + username + " " + pass + " " + "UL" + " " + ULsessionno + " " + DLfilename + " " + serialportname + " " + ULremotename + " " + ULfilename
                    displaylog("ftp parameter:" & serverip + " " + username + " " + pass + " " + "UL" + " " + ULsessionno + " " + DLfilename + " " + ULremotename + " " + ULfilename, "g")
                    myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized
                    myprocess.Start()
                    ftphandle(1) = myprocess.Id
                    displaylog("ftp session id opened:" & myprocess.Id.ToString, "g")

                End If

            Else
                displaylog("no ip no traffic", "r")
                Return "no ip no traffic"
            End If

        Else
            displaylog("No ftp session needed", "g")
            Return "No ftp session needed"
        End If

    End Function
    Function rundlftpo() As String
        Dim count As Integer
        Dim ftpsessionnumreal As Integer
        rundlftpo = "ok"
        If traffictype = "ftpdlul" Then ftpsessionnumreal = ftpsessionnum * 2 Else ftpsessionnumreal = ftpsessionnum

        If Int(ftpsessionnumreal) > 0 Then
            If addroute() <> "ip not find" Then


                count = 1
                displaylog("start new ftp session", "g")

                Do While count <= Int(ftpsessionnumreal)
                    'Dim myProcess As Process = System.Diagnostics.Process.Start("d:\mueauto\ftpdl.bat " + UEip + " " + Str(count) + " " + serverip)
                    Dim myprocess As Process = New Process()
                    If traffictype = "ftpdl" Then
                        myprocess.StartInfo.FileName = "d:\mueauto\ftpdl.bat"
                    Else
                        If traffictype = "ftpul" Then
                            myprocess.StartInfo.FileName = "d:\mueauto\ftpul.bat"
                        End If
                        If traffictype = "ftpdlul" And (count Mod 2 = 1) Then
                            myprocess.StartInfo.FileName = "d:\mueauto\ftpdl.bat"
                        End If
                        If traffictype = "ftpdlul" And (count Mod 2 = 0) Then
                            myprocess.StartInfo.FileName = "d:\mueauto\ftpul.bat"
                        End If
                    End If
                    myprocess.StartInfo.Arguments = UEip + " " + Str(count) + " " + serverip
                    myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized
                    myprocess.Start()
                    ftphandle(count - 1) = myprocess.Id
                    displaylog("ftp session id opened:" & myprocess.Id.ToString, "g")

                    count = count + 1
                Loop
            Else
                displaylog("no ip no traffic", "r")
                Return "no ip no traffic"
            End If

        Else
            displaylog("No ftp session needed", "g")
            Return "No ftp session needed"
        End If

    End Function
    Function rundlhttpdownload() As String

        'If addroute() <> "ip not find" Then
        '    displaylog("reopen the webbrowser", "g")
        '    Form2.Hide()
        '    Form2.WebBrowser1.Url = New Uri("http:\\" + serverip)
        '    Form2.Timer1.Enabled = False
        '    Form2.Timer1.Interval = ftpsessionnum * 1000
        '    Form2.Timer1.Enabled = True
        '    Form2.Show()
        '    Return "OK"
        'Else
        '    displaylog("no ip no traffic", "r")
        '    Return "no ip no traffic"
        'End If
        Dim count As Integer
        rundlhttpdownload = "OK"

        If addroute() <> "ip not find" Then


            count = 1
            displaylog("start new http session", "g")


            Dim myprocess As Process = New Process()
            If traffictype = "httpdownload" Then
                myprocess.StartInfo.FileName = "d:\mueauto\webdownload.exe"
            End If
            myprocess.StartInfo.Arguments = serverip + " " + ftpsessionnum + " " + serialportname
            myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized

            myprocess.Start()
            httphandle = myprocess.Id
            displaylog("http session id opened:" & myprocess.Id.ToString, "g")


        Else
            displaylog("no ip no traffic", "r")
            Return "no ip no traffic"
        End If


    End Function

    Function runping() As String
        Dim count As Integer
        Try
            runping = "OK"
            If Int(ftpsessionnum) > 0 Then
                If addroute() <> "ip not find" Then


                    count = 1
                    displaylog("start new ping session", "g")
                    Dim intervalstring As String = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "ping", "interval")
                    Dim lenstring As String = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "ping", "len")

                    Do While count <= Int(1)
                        Dim myprocess As Process = New Process()
                        If traffictype = "ping" Or traffictype = "MOC" Or traffictype = "MTC" Then
                            ' myprocess.StartInfo.FileName = "d:\mueauto\ping.exe"
                            myprocess.StartInfo.FileName = "d:\mueauto\hrping.exe"
                            If intervalstring <> "" And lenstring <> "" Then
                                myprocess.StartInfo.Arguments = serverip + " -t -L " + lenstring + " -y 60 -s " + intervalstring  'wait change size,interval 10K/s
                            Else
                                myprocess.StartInfo.Arguments = serverip + " -t  -y 60"
                            End If


                            'myprocess.StartInfo.Arguments = serverip + " -t"
                            myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized

                            myprocess.Start()
                            pinghandle(count - 1) = myprocess.Id
                            displaylog("ping session id opened:" & myprocess.Id.ToString, "g")
                            wait(3)
                            SetWindowTextA(myprocess.MainWindowHandle, "Ping:" + serialportname.ToUpper)
                            count = count + 1
                        End If
                    Loop
                Else
                    displaylog("no ip no traffic", "r")
                    Return "no ip no traffic"
                End If

            Else
                displaylog("No ping session needed", "g")
                Return "No ping session needed"
            End If
        Catch
            displaylog("Ping is not excuted.", "g")
            Return "Ping is not excuted."
        End Try

    End Function

    Function runvideo() As String
        Dim count As Integer
        runvideo = "OK"
        If Int(ftpsessionnum) > 0 Then
            If addroute() <> "ip not find" Then


                count = 1
                displaylog("start new video session", "g")

                Do While count <= 1
                    Dim myprocess As Process = New Process()
                    If traffictype = "video" Then
                        myprocess.StartInfo.FileName = "C:\Program Files\VideoLAN\VLC\vlc.exe"
                    End If
                    myprocess.StartInfo.Arguments = "rtsp://" + serverip + ":55555/ --scale=0.2 --qt-minimal-view --qt-start-minimized  "
                    myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized
                    myprocess.Start()
                    videohandle(count - 1) = myprocess.Id
                    displaylog("video session id opened:" & myprocess.Id.ToString, "g")

                    count = count + 1
                Loop
            Else
                displaylog("no ip no traffic", "r")
                Return "no ip no traffic"
            End If

        Else
            displaylog("No video session needed", "g")
            Return "No video session needed"
        End If
    End Function
    Function getcallnum() As String
        Dim servers, tempstr As String
        getcallnum = ""
        getcallnum = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "volterealtarget", imsi)
       
    End Function

    Sub getappserverip()
        Dim servers, tempstr As String
        servers = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "voltesimserver", imsi)
        If servers <> "" And UBound(Split(servers, ",")) = 2 Then
            tempstr = Trim(Split(servers, ",")(0))
            If checkipaddress(tempstr) Then
                serverip2 = tempstr
            Else
                displaylog("please check voltesimserver setting in ftp.ini " + tempstr + " is not a right ip address", "r")
            End If
            tempstr = Trim(Split(servers, ",")(1))
            If checkipaddress(tempstr) Then
                serverip3 = tempstr
            Else
                displaylog("please check voltesimserver setting ftp.ini " + tempstr + " is not a right ip address", "r")
            End If
            tempstr = Trim(Split(servers, ",")(2))
            If checkipaddress(tempstr) Then
                serverip4 = tempstr
            Else
                displaylog("please check voltesimserver setting ftp.ini " + tempstr + " is not a right ip address", "r")
            End If

        Else
            displaylog("please check voltesimserver setting ftp.ini " + servers + " is not a right ip addresss", "r")
        End If
    End Sub

    Function runvolte() As String
        Dim count As Integer
        Dim username, pass, DLsessionno, ULsessionno, DLfilename, ULfilename, ULremotename As String

        '-----------------------------------get ftp configs

        username = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "server", "username")

        pass = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "server", "password")
        DLsessionno = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "DL", "sessionno")
        ULsessionno = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "UL", "sessionno")
        DLfilename = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "DL", "filename")
        ULfilename = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "UL", "filename")
        ULremotename = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "UL", "remotename")
        '--------------------------------------------------------------------------------

        If imsi <> "" Then
            displaylog("imsi:" + imsi, "g")
            getappserverip()

            If addroute() <> "ip not find" Then

                count = 1
                displaylog("start new volte default bearer ftp session", "g")
                Dim myprocess As Process = New Process()
                myprocess.StartInfo.FileName = "d:\mueauto\ftpclient.exe"
                myprocess.StartInfo.Arguments = serverip + " " + username + " " + pass + " " + "DL" + " " + DLsessionno + " " + DLfilename + " " + serialportname
                displaylog("ftp parameter:" & serverip + " " + username + " " + pass + " " + "DL" + " " + DLsessionno + " " + DLfilename, "g")


                myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized
                myprocess.Start()
                voltehandle(count - 1) = myprocess.Id
                displaylog("ftp session id opened:" & myprocess.Id.ToString, "g")
                count = count + 1

                displaylog("start new volte sip bearer ping session", "g")
                myprocess = New Process()
                myprocess.StartInfo.FileName = "d:\mueauto\hrping.exe"
                myprocess.StartInfo.Arguments = serverip2 + " -t -l 500 -y -s 500" 'wait change size,interval 10K/s
                myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized
                myprocess.Start()
                voltehandle(count - 1) = myprocess.Id
                displaylog("sip hrping sim session id opened:" & myprocess.Id.ToString, "g")
                SetWindowTextA(myprocess.MainWindowHandle, "Ping:" + serialportname.ToUpper)
                count = count + 1

                displaylog("start new volte voice bearer ping session", "g")
                myprocess = New Process()
                myprocess.StartInfo.FileName = "d:\mueauto\voltesim.exe"
                myprocess.StartInfo.Arguments = serverip3 + " " + serialportname 'wait chage size, interval 50K/s， should gbr as 150K with sps
                myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized
                myprocess.Start()
                voltehandle(count - 1) = myprocess.Id
                displaylog("voice hrping sim session id opened:" & myprocess.Id.ToString, "g")
                count = count + 1

                displaylog("start new volte video bearer ping session", "g")
                myprocess = New Process()
                ' myprocess.StartInfo.FileName = "d:\mueauto\ftpdl.bat"
                'myprocess.StartInfo.Arguments = UEip + " " + Str(count) + " " + serverip4

                myprocess.StartInfo.FileName = "d:\mueauto\hrping.exe"
                myprocess.StartInfo.Arguments = serverip4 + " -t -l 500 -y -s 5" 'wait chage size, interval
                myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized
                myprocess.Start()
                voltehandle(count - 1) = myprocess.Id
                displaylog("video traffic ftp sim session id opened:" & myprocess.Id.ToString, "g")
                SetWindowTextA(myprocess.MainWindowHandle, "Ping:" + serialportname.ToUpper)
                count = count + 1
                Return "OK"
            Else
                displaylog("no ip no traffic", "r")
                Return "no ip no traffic"
            End If
        Else
            displaylog("not find imis code", "r")
            Return "no ip no traffic"
        End If


    End Function

    Function killoldvolte() As Integer
        Dim count As Integer
        Dim killid As Integer
        displaylog("kill old volte sessions", "g")
        'Shell("taskkill /F /IM ftp" & UEip & ".exe")
        'Shell("taskkill /F /IM shortping.exe")
        myprocess.killprocess("ftp" & UEip)
        myprocess.killprocess("shortping")
        count = 1

        If voltehandle(0) <> Nothing Then
            Do While count <= 4
                killid = voltehandle(count - 1)
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
        End If
        Return 1
    End Function
    Function killoldftp() As Integer
        Dim count As Integer
        Dim killid As Integer
        Dim ftpsessionnumreal As Integer

        displaylog("kill old ftp", "g")
        'Shell("taskkill /F /IM ftp" & UEip & ".exe")
        'Shell("taskkill /F /IM shortping.exe")
        'myprocess.killprocess("ftp" & UEip)
        'myprocess.killprocess("shortping")
        count = 1
        If traffictype = "ftpdlul" Then ftpsessionnumreal = ftpsessionnum * 2 Else ftpsessionnumreal = ftpsessionnum
        If ftphandle(0) <> Nothing Then

            killid = ftphandle(0)
            Try
                Dim pProcessTemp As System.Diagnostics.Process = Process.GetProcessById(killid)
                displaylog("kill process id:" & killid & "  process name:" & pProcessTemp.ProcessName, "g")
                pProcessTemp.Kill()
                pProcessTemp.Close()
                count = count + 1
                If traffictype = "ftpdlul" Then
                    killid = ftphandle(1)
                    pProcessTemp = Process.GetProcessById(killid)
                    displaylog("kill process id:" & killid & "  process name:" & pProcessTemp.ProcessName, "g")
                    pProcessTemp.Kill()
                    pProcessTemp.Close()
                    count = count + 1
                End If
                'Shell("taskkill /F /IM ftp" & UEip & ".exe")
                'Shell("taskkill /F /IM shortping.exe")
                ' Shell("start d:\mueauto\killuesoft.bat")
            Catch e As Exception
                displaylog(e.Message, "r")
                count = count + 1
                Return -1
            End Try
        End If
        Return 1
    End Function
    Function killoldftpo() As Integer
        Dim count As Integer
        Dim killid As Integer
        Dim ftpsessionnumreal As Integer

        displaylog("kill old ftp", "g")
        'Shell("taskkill /F /IM ftp" & UEip & ".exe")
        'Shell("taskkill /F /IM shortping.exe")
        myprocess.killprocess("ftp" & UEip)
        myprocess.killprocess("shortping")
        count = 1
        If traffictype = "ftpdlul" Then ftpsessionnumreal = ftpsessionnum * 2 Else ftpsessionnumreal = ftpsessionnum
        If ftphandle(0) <> Nothing Then
            Do While count <= Int(ftpsessionnumreal)
                killid = ftphandle(count - 1)
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
        End If
        Return 1
    End Function
    Function killoldping() As Integer
        Dim count As Integer
        Dim killid As Integer
        displaylog("kill old ping", "g")
        'Shell("taskkill /F /IM ftp" & UEip & ".exe")
        'Shell("taskkill /F /IM shortping.exe")
        'myprocess.killprocess("ftp" & UEip)
        myprocess.killprocess("shortping")
        count = 1

        If pinghandle(0) <> Nothing Then
            Do While count <= Int(1)
                killid = pinghandle(count - 1)
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
        End If
        Return 1
    End Function
    Function killoldvolteadbah() As Integer
        Dim count As Integer
        Dim killid As Integer
        displaylog("kill old volte", "g")
        'Shell("taskkill /F /IM ftp" & UEip & ".exe")
        'Shell("taskkill /F /IM shortping.exe")
        'myprocess.killprocess("ftp" & UEip)
        myprocess.killprocess("shortping")
        count = 1



        killid = volteadbahhandle
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

        Return 1
    End Function
    Function killoldhttp() As Integer
        Dim count As Integer
        Dim killid As Integer
        displaylog("kill old http", "g")
        'Shell("taskkill /F /IM ftp" & UEip & ".exe")
        'Shell("taskkill /F /IM shortping.exe")
        'myprocess.killprocess("ftp" & UEip)
        myprocess.killprocess("shortping")
        count = 1



        killid = httphandle
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

        Return 1
    End Function

    Function killvideo() As Integer
        Dim count As Integer
        Dim killid As Integer
        displaylog("kill old video", "g")
        'Shell("taskkill /F /IM ftp" & UEip & ".exe")
        'Shell("taskkill /F /IM shortping.exe")
        'myprocess.killprocess("ftp" & UEip)
        myprocess.killprocess("shortping")
        count = 1

        If videohandle(0) <> Nothing Then
            Do While count <= 1
                killid = videohandle(count - 1)
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
        End If
        Return 1
    End Function

    Function rundlhttp() As String

        'If addroute() <> "ip not find" Then
        '    displaylog("reopen the webbrowser", "g")
        '    Form2.Hide()
        '    Form2.WebBrowser1.Url = New Uri("http:\\" + serverip)
        '    Form2.Timer1.Enabled = False
        '    Form2.Timer1.Interval = ftpsessionnum * 1000
        '    Form2.Timer1.Enabled = True
        '    Form2.Show()
        '    Return "OK"
        'Else
        '    displaylog("no ip no traffic", "r")
        '    Return "no ip no traffic"
        'End If
        Dim count As Integer
        rundlhttp = "OK"

        If addroute() <> "ip not find" Then


            count = 1
            displaylog("start new http session", "g")


            Dim myprocess As Process = New Process()
            If traffictype = "http" Then
                myprocess.StartInfo.FileName = "d:\mueauto\webbrowser.exe"
            End If
            myprocess.StartInfo.Arguments = serverip + " " + ftpsessionnum + " " + serialportname
            myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized

            myprocess.Start()
            httphandle = myprocess.Id
            displaylog("http session id opened:" & myprocess.Id.ToString, "g")


        Else
            displaylog("no ip no traffic", "r")
            Return "no ip no traffic"
        End If


    End Function
    Function runrealvolteadbahmt() As String

        Dim count As Integer
        Dim phonenum As String = "10086"
        runrealvolteadbahmt = "OK"

        If addroute() <> "ip not find" Then
            If imsi <> "" Then
                displaylog("imsi:" + imsi, "g")
                phonenum = getcallnum()

                count = 1
                displaylog("start new http session", "g")


                Dim myprocess As Process = New Process()
                If traffictype = "http" Then
                    myprocess.StartInfo.FileName = "d:\mueauto\dist\modemtool.exe"
                End If
                myprocess.StartInfo.Arguments = serialportname + " -f d:\mueauto\dist\test.wav -o "
                myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized

                myprocess.Start()
                volteadbahhandle = myprocess.Id
                displaylog("volte adb id opened:" & myprocess.Id.ToString, "g")

            End If
        Else
            displaylog("no ip no traffic", "r")
            Return "no ip no traffic"
        End If


    End Function
    Function runrealvolteadb() As String

        Dim count As Integer
        Dim phonenum As String = "10086"
        Dim adbdvicesn As String = ""
        runrealvolteadb = "OK"

        If addroute() <> "ip not find" Then
            If imsi <> "" Then
                displaylog("imsi:" + imsi, "g")
                phonenum = getcallnum()
                adbdvicesn = getadbdevice()
                If adbdvicesn <> "" Then
                    count = 1
                    displaylog("start new http session", "g")


                    Dim myprocess As Process = New Process()
                    If traffictype = "http" Then
                        myprocess.StartInfo.FileName = "d:\mueauto\adb.exe"
                    End If
                    myprocess.StartInfo.Arguments = " -s " + androiddevid + " shell am start -a Android.intent.action.CALL tel:" + phonenum
                    myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized

                    myprocess.Start()
                    volteadbahhandle = myprocess.Id
                    displaylog("volte adb id opened:" & myprocess.Id.ToString, "g")
                Else
                    displaylog("not find ADB device sn", "r")
                    Return "no ip no traffic"
                End If

            End If
        Else
            displaylog("no ip no traffic", "r")
            Return "no ip no traffic"
        End If


    End Function
    Function getadbdevice()
        Dim devicesroot As String = ""
        Dim devicecontainid As String = ""
        Dim deviceserial As String = ""
        regsearch(0) = Nothing
        regsearch(1) = Nothing
        regsearch(2) = Nothing
        regsearch(3) = Nothing
        SearchSubKeys(My.Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\ControlSet001\Enum\USB"), serialportname.ToUpper)
        If regsearch(3) <> Nothing Then
            Devicesroot = Mid(regsearch(1), 1, regsearch(1).LastIndexOf("\"))
            devicecontainid = My.Computer.Registry.GetValue(devicesroot, "ContainerID", String.Empty)
            If devicecontainid <> "" Then
                regsearch(0) = Nothing
                regsearch(1) = Nothing
                regsearch(2) = Nothing
                regsearch(3) = Nothing
                SearchSubKeys(My.Computer.Registry.LocalMachine.OpenSubKey("HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Enum\USBSTOR"), devicecontainid)
                If regsearch(3) <> Nothing Then
                    devicesroot = VB.Right(regsearch(1), regsearch(1).LastIndexOf("\") + 1)
                    deviceserial = Split(devicesroot, "&")(3)
                    Return Trim(deviceserial)
                Else
                    Return ""
                End If

            Else
                Return ""
            End If
        Else
            Return ""
        End If
    End Function
    Sub SearchSubKeys(ByVal root As RegistryKey, ByVal searchKey As String)
        Dim matchtype As String = Nothing

        Dim itm As ListViewItem

        totalcount = totalcount + 1

        'Console.WriteLine("No. of keys searched : " + totalcount.ToString)
        'Console.WriteLine("Matching items : " + count.ToString)
        'Console.WriteLine("Last key scanned : " + root.ToString)



        'search key name

        If root.Name = (searchKey) Then

            matchtype = "Key"
            regsearch(0) = matchtype
            regsearch(1) = root.Name
            regsearch(2) = Nothing
            regsearch(3) = Nothing

            count = count + 1

        End If



        'search value names
        For Each valueName As String In root.GetValueNames

            If valueName = (searchKey) Then
                matchtype = "Value Name"
                regsearch(0) = matchtype
                regsearch(1) = root.Name
                regsearch(2) = valueName
                regsearch(3) = root.GetValue(valueName)
                'Console.WriteLine(str(0) + vbCrLf + str(1) + vbCrLf + str(2) + vbCrLf + str(3))

                count = count + 1
            End If


            'search values
            Select Case root.GetValueKind(valueName)
                Case RegistryValueKind.String, RegistryValueKind.ExpandString
                    Dim value As String = CStr(root.GetValue(valueName))
                    If value = (searchKey) Then
                        matchtype = "Value"
                        regsearch(0) = matchtype
                        regsearch(1) = root.Name
                        regsearch(2) = valueName
                        regsearch(3) = value
                        'Console.WriteLine(str(0) + vbCrLf + str(1) + vbCrLf + str(2) + vbCrLf + str(3))

                        count = count + 1
                    End If
                Case RegistryValueKind.MultiString
                    Dim value As String = String.Join(vbNewLine, CType(root.GetValue(valueName), String()))
                    If value = (searchKey) Then
                        matchtype = "Value"
                        regsearch(0) = matchtype
                        regsearch(1) = root.Name
                        regsearch(2) = valueName
                        regsearch(3) = value
                        'Console.WriteLine(str(0) + vbCrLf + str(1) + vbCrLf + str(2) + vbCrLf + str(3))

                        count = count + 1
                    End If
            End Select

        Next



        For Each subkeyName As String In root.GetSubKeyNames
            Try
                Dim subkey As RegistryKey = root.OpenSubKey(subkeyName, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey)
                SearchSubKeys(subkey, searchKey)
                subkey.Close()
            Catch ex As Exception

            End Try
        Next
    End Sub




    Function runrealvolteadbahmo() As String

        Dim count As Integer
        Dim phonenum As String = "10086"
        runrealvolteadbahmo = "OK"

        If addroute() <> "ip not find" Then
            If imsi <> "" Then
                displaylog("imsi:" + imsi, "g")
                phonenum = getcallnum()

                count = 1
                displaylog("start new http session", "g")


                Dim myprocess As Process = New Process()
                If traffictype = "http" Then
                    myprocess.StartInfo.FileName = "d:\mueauto\dist\modemtool.exe"
                End If
                myprocess.StartInfo.Arguments = serialportname + " -f d:\mueauto\dist\test.wav -o " + phonenum + " -t 3 -n 99999"
                myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized

                myprocess.Start()
                volteadbahhandle = myprocess.Id
                displaylog("volte adb id opened:" & myprocess.Id.ToString, "g")

            End If
        Else
            displaylog("no ip no traffic", "r")
            Return "no ip no traffic"
        End If


    End Function
    Function attachdetach_HISI() As String
        If SerialPortOpen(logname, False) Then

            If UEtype = "H" Then
                serialSendData("AT+CGATT=0" & vbCrLf, logname)
                displaylog("AT+CGATT=0", "g")
                wait(5)

                serialSendData("AT^PSPOWER=0" & vbCrLf, logname)
                displaylog("AT^PSPOWER=0", "g")
                wait(15)

                serialSendData("AT^PSPOWER=1" & vbCrLf, logname)
                displaylog("AT^PSPOWER=1", "g")
                wait(15)

                serialSendData("AT+CGATT=1" & vbCrLf, logname)
                displaylog("AT+CGATT=1", "g")
                wait(30)
                serialSendData("AT+CGCONTRDP" & vbCrLf, logname)
                displaylog("AT+CGCONTRDP", "g")
                wait(5)
                serialSendData("AT^NDISCONN=1,1" & vbCrLf, logname)
                displaylog("AT^NDISCONN=1,1", "g")
            End If
            If UEtype = "HE5776" Then
                serialSendData("AT+CGATT=0" & vbCrLf, logname)
                displaylog("AT+CGATT=0", "g")
                wait(5)

                serialSendData("AT+CFUN=0" & vbCrLf, logname)
                displaylog("AT+CFUN=0", "g")
                wait(15)

                serialSendData("AT+CFUN=1" & vbCrLf, logname)
                displaylog("AT+CFUN=1", "g")
                wait(30)
                serialSendData("AT+CGCONTRDP" & vbCrLf, logname)
                displaylog("AT+CGCONTRDP", "g")
                wait(5)
                serialSendData("AT+CGREG?" & vbCrLf, logname)
                displaylog("AT+CGREG?", "g")
                wait(5)
            End If
            serialSendData("AT+CGCONTRDP" & vbCrLf, logname)
            displaylog("AT+CGCONTRDP", "g")
            wait(1)
            serialSendData("AT+CIMI" & vbCrLf, logname)
            displaylog("AT+CIMI", "g")
            wait(1)
            serialSendData("AT+CGREG=2" & vbCrLf, logname)
            displaylog("AT+CGREG=2", "g")
            wait(1)
            serialSendData("AT+CGREG?" & vbCrLf, logname)
            displaylog("AT+CGREG?", "g")
            wait(1)
            serialSendData("AT+cgsn" & vbCrLf, logname)
            displaylog("AT+cgsn", "g")
            wait(1)
            Timer4.Enabled = True
            Return "OK"

        Else
            Return "KO"
        End If
    End Function
    Function attachdetach_Qualcomm(ByVal type As Integer) As String


        serialSendData("AT+CGATT=0" & vbCrLf, logname)
        displaylog("AT+CGATT=0", "g")

        wait(5)

        serialSendData("AT+CFUN=0" & vbCrLf, logname)
        displaylog("AT+CFUN=0", "g")
        wait(15)

        serialSendData("AT+CFUN=1" & vbCrLf, logname)
        displaylog("AT+CFUN=1", "g")
        wait(15)
        'If System.IO.File.Exists("C:\Program Files\Alcatel-lucent\QCT_Auto\pro\bin\ConnectionManager.exe") Then
        'Shell("C:\Program Files\Alcatel-lucent\QCT_Auto\pro\bin\ConnectionManager.exe", AppWinStyle.MinimizedNoFocus, False)
        'Else
        'MsgBox("Can not find C:\Program Files\Alcatel-lucent\QCT_Auto\pro\bin\ConnectionManager.exe")
        'Application.Exit()
        'End If

        'serialSendData("AT+CGATT=1" & vbCrLf, logname)
        'displaylog("AT+CGATT=1", "g")
        'wait(30)
        'Qualcommactive()
        'serialSendData("at$wancall=1" & vbCrLf, logname)
        'displaylog("at$wancall=1", "g")
        'wait(15)
        'Shell("D:\mueauto\Qclient.exe device=1", , True)
        'Shell("D:\mueauto\Qclient.exe command=Connect", , True)
        If type = "9028" Then
            serialSendData("AT^ENLOG=1" & vbCrLf, logname)
            displaylog("AT^ENLOG=1", "g")
            wait(1)
            serialSendData("AT+CGATT=1" & vbCrLf, logname)
            displaylog("AT+CGATT=1", "g")
            wait(1)

        Else
            wwandial()

        End If
        serialSendData("AT$QCRMCALL=1,1,1,2,1" & vbCrLf, logname)
        displaylog(" AT^NDISCONN=1,1", "g")
        wait(3)
        serialSendData("AT+CGCONTRDP" & vbCrLf, logname)
        displaylog("AT+CGCONTRDP", "g")
        wait(1)
        serialSendData("AT+CIMI" & vbCrLf, logname)
        displaylog("AT+CIMI", "g")
        wait(1)
        serialSendData("AT+CGREG=2" & vbCrLf, logname)
        displaylog("AT+CGREG=2", "g")
        wait(1)
        serialSendData("AT+CGREG?" & vbCrLf, logname)
        displaylog("AT+CGREG?", "g")
        wait(1)
        serialSendData("AT+cgsn" & vbCrLf, logname)
        displaylog("AT+cgsn", "g")
        Timer4.Enabled = True
        Return "OK"
    End Function
    Function readfile(ByVal filename) As String
        Dim i, pp, pp2, pp3, pp4, j, totallines As Integer
        Dim Buff, yvalue, pointtime As String
        Dim searchedstr As String
        Dim LineBuff, Yvalues, trytimes, successtimes, tempstr As Object
        Dim start As Long
        LineBuff = Nothing
        Yvalues = Nothing
        trytimes = Nothing
        successtimes = Nothing
        tempstr = Nothing

        If File.Exists(filename) Then
            FileOpen(1, filename, OpenMode.Binary, OpenAccess.Read, OpenShare.Shared)

            start = LOF(1)
            Buff = Space(100000)
            If start > 100000 Then
                start = start - 100000
            Else
                start = 1
            End If
            'UPGRADE_WARNING: Get was upgraded to FileGet and has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
            FileGet(1, Buff, start)

            FileClose(1)
            Return Buff
        End If
        Return ""
    End Function
    Function wwandial()
        Dim str As String
        Dim temstringarr As String()
        Dim profilename As String
        Dim i As Integer
        str = rundoscommand("netsh mbn show profiles")
        'str = readfile("E:\test.txt")
        displaylog(str, "g")
        temstringarr = Split(str, vbCrLf)
        For i = 0 To temstringarr.Length - 1
            If temstringarr(i).IndexOf(serialportname.ToUpper) >= 0 Then Exit For


        Next
        If i < temstringarr.Length Then
            profilename = Trim(temstringarr(i + 2))
            If profilename.IndexOf("<") < 0 Then
                str = "netsh mbn connect """ + serialportname.ToUpper + """ name """ + profilename + """"

                str = rundoscommand(str)
                displaylog(str, "g")

                wait(15)
            Else
                displaylog("wwan " + serialportname.ToUpper + "port profile not find", "r")
            End If

        Else
            displaylog("Not find wwanport", "g")
        End If


    End Function
    Public Function sendtcpcommand(ByVal command As String, ByVal ip As String) As String
        Try
            'Dim port As Int32 = 33891
            'Dim client As New System.Net.Sockets.TcpClient(ip, 33891, )
            Dim client As New System.Net.Sockets.TcpClient()
            Dim result = client.BeginConnect(ip, 33893, Nothing, Nothing)
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
    Function Qualcommactive() As String
        Try
            ' Create a TcpClient.
            ' Note, for this client to work you need to have a TcpServer 
            ' connected to the same address as specified by the server, port
            ' combination.
            Dim port As Int32 = 13000
            Dim client As New System.Net.Sockets.TcpClient("127.0.0.1", 9999)
            ' Translate the passed message into ASCII and store it as a Byte array.
            Dim data As [Byte]() = System.Text.Encoding.ASCII.GetBytes("connect")

            ' Get a client stream for reading and writing.
            '  Stream stream = client.GetStream();
            Dim stream As System.Net.Sockets.NetworkStream = client.GetStream()

            ' Send the message to the connected TcpServer. 
            stream.Write(data, 0, data.Length)

            Console.WriteLine("Sent: {0}", "connect")
            displaylog("sent:connect", "g")

            ' Receive the TcpServer.response.
            ' Buffer to store the response bytes.
            data = New [Byte](256) {}

            ' String to store the response ASCII representation.
            Dim responseData As [String] = [String].Empty

            ' Read the first batch of the TcpServer response bytes.
            Dim bytes As Int32 = stream.Read(data, 0, data.Length)
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes)
            Console.WriteLine("Received: {0}", responseData)
            displaylog("Received:" + responseData, "g")
            ' Close everything.
            stream.Close()
            client.Close()
        Catch e As ArgumentNullException
            Console.WriteLine("ArgumentNullException: {0}", e)
        Catch e As System.Net.Sockets.SocketException
            Console.WriteLine("SocketException: {0}", e)
        End Try
        Return "OK"
    End Function
    Function commdeactivate() As String
        Try
            displaylog("Disconnect dial-up", "g")
            'rundoscommand("rasdial " + serialportname + " /disconnect")
            ' rundoscommand("rasdial " + serialportname + " administrator asb /phone:192.168.116.128:11119")
            Shell("rasdial " + serialportname + " /disconnect", AppWinStyle.NormalNoFocus)
        Catch ex As Exception

        End Try
        Return "OK"
    End Function

    Function commactive() As String
        Try
            'rundoscommand("rasdial " + serialportname + " /disconnect")
            displaylog("dial up " + serialportname, "r")
            Shell("rasdial " + serialportname + " administrator asb /phone:192.168.116.128:11119", AppWinStyle.NormalNoFocus)
        Catch
        End Try
        Return "OK"
    End Function
    Function attachdetach_Bandrich() As String
        If SerialPortOpen(logname, False) Then

            serialSendData("at$wancall=0" & vbCrLf, logname)
            displaylog("at$wancall=0", "g")
            wait(5)

            serialSendData("AT+CGATT=0" & vbCrLf, logname)
            displaylog("AT+CGATT=0", "g")
            wait(5)

            serialSendData("AT+CFUN=0" & vbCrLf, logname)
            displaylog("AT+CFUN=0", "g")
            wait(15)

            serialSendData("AT+CFUN=1,0" & vbCrLf, logname)
            displaylog("AT+CFUN=1,0", "g")
            wait(15)

            serialSendData("AT+CGATT=1" & vbCrLf, logname)
            displaylog("AT+CGATT=1", "g")
            wait(30)

            serialSendData("AT+CGCONTRDP" & vbCrLf, logname)
            displaylog("AT+CGCONTRDP", "g")
            wait(5)

            serialSendData("at$wancall=1" & vbCrLf, logname)
            displaylog("at$wancall=1", "g")
            wait(15)
            Return "OK"
        Else
            Return "KO"
        End If

    End Function
    Private Function getcomnetname(ByVal comname As String) As String
        Dim realname As String



        Dim adapters As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
        Dim adapter As NetworkInterface
        getcomnetname = ""
        'root\CIMV2:Win32_PerfFormattedData_RemoteAccess_RASPort
        For Each adapter In adapters
            Dim properties As IPInterfaceProperties = adapter.GetIPProperties()
            If (adapter.Name.ToLower.ToString.Contains(comname.ToLower)) And (Trim(adapter.Name.ToLower.ToString) = Trim(comname.ToLower)) Then
                accesstype = "net"
                Return adapter.Description
            End If
        Next adapter

        Dim path As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        path = path + "\Microsoft\Network\Connections\Pbk\rasphone.pbk"
        If IO.File.Exists(path) Then
            For i = 1 To Module1.TotalSections(path)
                realname = Module1.GetSection(path, i - 1)
                If Trim(realname.ToLower) = Trim(comname.ToLower) Then
                    accesstype = "dial"
                    Return Module1.ReadKeyVal(path, realname, "port")
                End If
            Next

        End If
        path = IO.Path.GetPathRoot(path)
        path = path + "\Users\All Users\Microsoft\Network\Connections\PBK\rasphone.pbk"

        If IO.File.Exists(path) Then

            For i = 1 To Module1.TotalSections(path)
                realname = Module1.GetSection(path, i - 1)
                If Trim(realname.ToLower) = Trim(comname.ToLower) Then
                    accesstype = "dial"
                    Return Module1.ReadKeyVal(path, realname, "port")
                End If
            Next


        End If
    End Function
    Private Sub getcomip(ByVal comname As String, ByVal isIPV4 As Boolean, ByRef ipaddress As String, ByRef gatewayipaddress As String)
        Dim adapters As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
        Dim adapter As NetworkInterface
        Dim s As String = "Connection fail"
        Dim i As Integer = 0
        For Each adapter In adapters
            Dim properties As IPInterfaceProperties = adapter.GetIPProperties()
            If (adapter.Name.ToLower.ToString.Contains(comname.ToLower)) And (Trim(adapter.Name.ToLower.ToString) = Trim(comname.ToLower)) Then
                displaylog(adapter.Description, "g")
                displaylog(adapter.Name, "g")
                displaylog("  operate state................................. :" & adapter.OperationalStatus.ToString, "g")
                If adapter.OperationalStatus.ToString <> "Up" Then
                    ipaddress = "Down"
                    Return
                End If
                Dim unicastipaddressinformationcollection As UnicastIPAddressInformationCollection = properties.UnicastAddresses
                Dim unicastip As UnicastIPAddressInformation
                For Each unicastip In unicastipaddressinformationcollection
                    displaylog("  ip address............:{0}" & unicastip.Address.ToString, "g")
                    displaylog("  " & unicastip.Address.AddressFamily.ToString, "g")
                    If isIPV4 And unicastip.Address.AddressFamily.ToString = "InterNetwork" Then 'InterNetworkV6 means ipV6 address
                        ipaddress = unicastip.Address.ToString 'only one ip ge
                        Exit For
                    ElseIf isIPV4 = False And unicastip.Address.AddressFamily.ToString = "InterNetworkV6" Then
                        ipaddress = unicastip.Address.ToString 'only one ip get
                        Exit For
                    End If
                Next unicastip


                Dim gatewaycollection As GatewayIPAddressInformationCollection = properties.GatewayAddresses
                Dim gatewayip As GatewayIPAddressInformation
                For Each gatewayip In gatewaycollection
                    displaylog("  gateway address.................:" & gatewayip.Address.ToString, "g")
                    If isIPV4 And gatewayip.Address.AddressFamily.ToString = "InterNetwork" Then 'InterNetworkV6 means ipV6 address
                        gatewayipaddress = gatewayip.Address.ToString 'only one ip ge
                        If gatewayipaddress <> "" And ipaddress <> "" And action = "pa" Then
                            i = 0
                            While (s = "Connection fail") And i < 3
                                s = sendtcpcommand("|" + ipaddress + "|" + UEname + "-" + serialportname, serverip)
                                i = i + 1
                            End While

                        End If
                        Return
                    ElseIf isIPV4 = False And gatewayip.Address.AddressFamily.ToString = "InterNetworkV6" Then
                        gatewayipaddress = gatewayip.Address.ToString 'only one ip get
                        If gatewayipaddress <> "" And ipaddress <> "" And action = "pa" Then
                            i = 0
                            While (s = "Connection fail") And i < 3
                                s = sendtcpcommand("|" + ipaddress + "|" + UEname + "-" + serialportname, serverip)
                                i = i + 1
                            End While
                        End If
                        Return
                    End If






                Next gatewayip
            End If
        Next adapter

    End Sub


    Protected Function GetLocalIP() As String
        Dim addr As System.Net.IPAddress
        Dim subnet As String()
        Dim subnetstring1, subnetstring2 As String
        subnet = getsubnettoserver(logip)

        subnetstring1 = subnet(0).ToString + "." + subnet(1).ToString + "." + subnet(2).ToString + "."
        subnetstring2 = subnet(0).ToString + "." + subnet(1).ToString + "."


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
        dosstr = Split(rundoscommand("tracert -w 100 -h 2 " + logip), vbCrLf)
        For i = 0 To dosstr.Count - 1
            If Trim(dosstr(i)) <> "" Then
                If Trim(dosstr(i)).Chars(0) = "1" Then
                    tempstr = Split(Trim(dosstr(i)), " ")(Split(Trim(dosstr(i)), " ").Length - 1)
                    tempstr = Replace(Replace(tempstr, "[", ""), "]", "")
                    If checkipaddress(tempstr) = True Then
                        output = Split(tempstr, ".")
                        found = True
                    Else
                        output = Split(logip, ".")
                    End If

                    Exit For
                End If
            End If
        Next
        If found = False Then
            Return Split("192.168.10.1 ", ".")
        Else
            Return output
        End If
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







    Function attachdetach_ALT() As String
        If SerialPortOpen(logname, False) Then



            'serialSendData("AT+CGATT=0" & vbCrLf, logname)
            serialSendData("AT+CGATT=0", logname)
            displaylog("AT+CGATT=0", "g")
            wait(5)

            'serialSendData("AT+CFUN=0" & vbCrLf, logname)
            serialSendData("AT+CFUN=0", logname)
            displaylog("AT+CFUN=0", "g")
            wait(15)

            'serialSendData("AT+CFUN=1" & vbCrLf, logname)
            serialSendData("AT+CFUN=1", logname)
            displaylog("AT+CFUN=1", "g")
            wait(15)

            'serialSendData("AT+CGATT=1" & vbCrLf, logname)
            serialSendData("AT+CGATT=1", logname)
            displaylog("AT+CGATT=1", "g")
            wait(60)

            Return "OK"
        Else
            Return "KO"
        End If
    End Function

    Function attachdetach_comm() As String


        If SerialPortOpen(logname, False) Then

            commdeactivate()
            wait(10)
            'serialSendData("AT+CGATT=0" & vbCrLf, logname)
            serialSendData("AT+CGATT=0", logname)
            displaylog("AT+CGATT=0", "g")
            wait(5)

            'serialSendData("AT+CFUN=0" & vbCrLf, logname)
            serialSendData("AT+CFUN=0", logname)
            displaylog("AT+CFUN=0", "g")
            wait(15)

            'serialSendData("AT+CFUN=1" & vbCrLf, logname)
            serialSendData("AT+CFUN=1", logname)
            displaylog("AT+CFUN=1", "g")
            wait(15)

            'serialSendData("AT+CGATT=1" & vbCrLf, logname)
            serialSendData("AT+CGATT=1", logname)
            displaylog("AT+CGATT=1", "g")
            'serialSendData("AT", logname)
            'serialSendData("AT", logname)
            'wait(3)

            ' SerialPort.Close()

            wait(10)
            serialSendData("AT+CGCONTRDP" & vbCrLf, logname)
            displaylog("AT+CGCONTRDP", "g")
            wait(5)
            'displaylog(SerialPort.IsOpen.ToString, "g")
            'Shell("rasdial", AppWinStyle.Hide)
            'Shell("rasdial", AppWinStyle.Hide)
            'commdeactivate()
            'wait(10)
            commactive()

            wait(10)
            Return "OK"
        Else
            Return "KO"
        End If
    End Function



    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        'serialSendData(TextBox2.Text + vbCrLf, "c:\temp.txt")
        ' rundoscommand("ping 127.0.0.1 -n 20")
        'addroute()
        'rundlftp()
        monitoring()

    End Sub
    Public Sub wait(ByRef s As Short)
        Dim starttime, endtime, temptime As DateTime
        Dim a As Integer = 0
        ' Dim __time As DateTime = DateTime.Now
        ' Dim __Span As Int64 = s * 1000000   '因为时间是以100纳秒为单位。
        ' Timer3.Enabled = True
        'While (DateTime.Now.Ticks - __time.Ticks < __Span)
        'Application.DoEvents()


        'End While
        'Timer3.Enabled = False
        Try
            starttime = DateTime.Now
            endtime = DateTime.Now
            temptime = starttime
            While DateDiff(DateInterval.Second, starttime, endtime) < s And exitwindow = False
                Threading.Thread.Sleep(30)
                Windows.Forms.Application.DoEvents()
                If DateDiff(DateInterval.Second, temptime, endtime) Then
                    If exitwindow <> True Then Windows.Forms.Application.DoEvents()
                    temptime = endtime
                    Console.ForegroundColor = ConsoleColor.Green
                    a = Console.CursorLeft
                    Console.Write("*")
                    Console.CursorLeft = a
                End If
                endtime = DateTime.Now


            End While

            Console.WriteLine(" ")
        Catch
        End Try

        If exitwindow = True Then End
    End Sub

    Private Sub runmain()
        Dim myArg() As String, iCount As Integer
        Dim netcardname As String
        Dim currenttime As Long
        'Timer1.Enabled = False


        myArg = System.Environment.GetCommandLineArgs

        If UBound(myArg) < 1 Then
            displaylog("-t UE type, UEtype[H -> HISI,Qualcomm9600,BandluxeC508,ALT-C186,Androidinter....]", "r")
            displaylog("-s server ip", "r")
            displaylog("-p COM port name", "r")
            displaylog("-d drop check interval s", "r")
            displaylog("-i loop interval s", "r")
            displaylog("-l loop attach - detach", "r")
            displaylog("-c shutdown", "r")
            displaylog("-g UEip", "r")
            displaylog("-n ftpsessionnum, 0:no traffic", "r")
            displaylog("-T traffic type, ftpdl/ftpul/http/httpdownload/ping/volte", "r")
            displaylog("-a throughput report interval, 1/10/60", "r")
            displaylog("-S sync time value", "r")
            displaylog("-e reset UE", "r")
            displaylog("-D android device id", "r")
            displaylog("Auto close after 10s", "g")
            wait(10)
            Application.Exit()
        End If
        For iCount = 1 To UBound(myArg)

            'TextBox1.Text = TextBox1.Text & "|" & myArg(iCount).ToString
            Select Case myArg(iCount).ToString
                Case "-T"
                    Select Case myArg(iCount + 1).ToString
                        Case "ftpdl"
                            traffictype = "ftpdl"
                        Case "ftpul"
                            traffictype = "ftpul"
                        Case "ftpdlul"
                            traffictype = "ftpdlul"
                        Case "http"
                            traffictype = "http"
                        Case "httpdownload"
                            traffictype = "httpdownload"
                        Case "ping"
                            traffictype = "ping"
                        Case "video"
                            traffictype = "video"
                        Case "volte(4)"
                            traffictype = "volte"
                        Case "realvolteADB"
                            traffictype = "realvolteADB"
                        Case "realvolteMOC"
                            traffictype = "MOC"
                        Case "realvolteMTC"
                            traffictype = "MTC"

                    End Select
                Case "-t"
                    Select Case myArg(iCount + 1).ToString
                        Case "H"
                            UEtype = "H"
                        Case "HE5776"
                            UEtype = "HE5776"
                        Case "Qualcomm9600"
                            UEtype = "Qualcomm9600"
                        Case "Qualcomm9028"
                            UEtype = "Qualcomm9028"
                        Case "BandluxeC508"
                            UEtype = "BandluxeC508"
                        Case "ALT-C186"
                            UEtype = "ALT-C186"
                        Case "Androidinter"
                            UEtype = "Androidinter"
                        Case "Dialcommon"
                            UEtype = "dialcomm"
                    End Select
                Case "-s"
                    serverip = myArg(iCount + 1)
                Case "-s2"
                    serverip2 = myArg(iCount + 1)
                Case "-s3"
                    serverip3 = myArg(iCount + 1)
                Case "-s4"
                    serverip4 = myArg(iCount + 1)
                Case "-p"
                    serialportname = myArg(iCount + 1)
                Case "-d"
                    interval = myArg(iCount + 1)
                Case "-i"
                    a_interval = myArg(iCount + 1)
                Case "-I"
                    action = "I"
                Case "-l"
                    action = "L"
                Case "-c"
                    action = "c"
                Case "-e"
                    action = "e"
                Case "-pa"
                    action = "pa"
                Case "-w"
                    logip = myArg(iCount + 1)
                Case "-g"
                    logname = myArg(iCount + 1) + ".log"
                    TPlogname = myArg(iCount + 1) + ".tp.txt"
                    UEip = myArg(iCount + 1)
                    UEname = UEip
                Case "-n"
                    ftpsessionnum = myArg(iCount + 1)

                Case "-a"
                    TPinterval = Val(myArg(iCount + 1)) / 2
                Case "-S"
                    action = "S"
                    currenttime = Val(myArg(iCount + 1))
                Case "-D"
                    androiddevid = myArg(iCount + 1)
            End Select




        Next
        If traffictype = "MTC" Or traffictype = "MOC" Then action = "P"



        displaylog("new start", "r")
        displaylog("UEtype=" + UEtype, "r")
        displaylog("serverip=" + serverip + "," + serverip2 + "," + serverip3 + "," + serverip4, "r")
        displaylog("serial port=" + serialportname, "r")
        accesstype = "net"
        netcardname = ""
        If action <> "S" And UEtype <> "Androidinter" Then
            netcardname = getcomnetname(serialportname)
            netcardname = netcardname.Replace("#", "_")
            netcardname = netcardname.Replace("(", "[")
            netcardname = netcardname.Replace(")", "]")
            netcardname = netcardname.Replace("/", "_")
            If netcardname <> "" Then
                If accesstype = "net" Then
                    displaylog("netcard device name :" + netcardname, "r")
                    ' Timer2.Enabled = True
                Else
                    displaylog("dial up modem com port :" + netcardname, "r")
                    Shell("rasdial /disconnect")
                End If
            Else
                displaylog("can not find netcard or dial up modem port name is " + serialportname, "r")
                wait(10)
                Application.Exit()
            End If
        End If

        displaylog("traffic type=" + traffictype, "r")
        'SerialPortTester.SerialPortFixer.Execute(serialportname)
        SerialPort = New System.IO.Ports.SerialPort()
        SerialPort.DtrEnable = True
        SerialPort.RtsEnable = True
        SerialPort.PortName = serialportname
        SerialPort.BaudRate = 115200
        SerialPort.StopBits = IO.Ports.StopBits.One
        SerialPort.Parity = IO.Ports.Parity.None
        SerialPort.DataBits = 8 '
        logname = "\\" + logip + "\uelog\" + logname
        'TPlogname = "\\" + logip + "\uelog\" + TPlogname
        TPlogname = "d:\" + TPlogname
        displaylog("logseverip=" + logip, "r")
        'displaylog("logname=" + logname, "r")
        'displaylog("TPlogname=" + TPlogname, "r")lo
        displaylog("TP report interval=" + TPinterval.ToString, "r")


        If action = "L" Then
            displaylog("mode: attach - dettach Loop", "r")
            displaylog("Loop interval=" + a_interval, "r")

        End If
        If action = "pa" Then
            displaylog("mode:paging Loop", "r")
        End If

        If action = "I" Then
            displaylog("mode:attach then idle", "r")

        End If
        If action = "P" Then
            displaylog("mode: long time run", "r")
            displaylog("drop check interval=" + interval, "r")

        End If

        If action = "c" Then
            displaylog("mode: shutdown UE", "r")
        End If

        If action = "e" Then
            displaylog("mode: reset UE", "r")
        End If

        If action = "S" Then
            displaylog("mode:time sync", "r")
        End If
        '*******************init tcp connect**************
        ' MyClientreconnect(True)
        'myclient1connect()
        '*************************************************

        If action <> "S" Then
            If UEtype <> "Androidinter" Then
                If accesstype = "net" Then
                    DLthroughput = New PerformanceCounter("Network Interface", "Bytes Received/sec", netcardname)
                    ULthroughput = New PerformanceCounter("Network Interface", "Bytes sent/sec", netcardname)
                ElseIf accesstype = "dial" Then
                    DLthroughput = New PerformanceCounter("RAS Port", "Bytes Received/sec", netcardname)
                    ULthroughput = New PerformanceCounter("RAS Port", "Bytes Transmitted/Sec", netcardname)

                End If

                Timer2.Enabled = True
                'Timer1.Enabled = True
            Else
                startandroidnetlog()
            End If



            wait(5)
            SerialPortOpen(logname, True)
            serialSendData("AT" & vbCrLf, logname)
            displaylog("start OK", "g")
            Console.Title = "MUEclient:" + serialportname '+ logname + " COM:" + serialportname
            ConsoleHelper.setconsoleminize()
            Me.Text = "MUEclientform:" + serialportname
            writeTPlog("", True, TPlogname, False)

            '----------------------------main 
            If UEtype = "Qualcomm9600" Then
                ' Shell("C:\Program Files\Alcatel-Lucent\QCT_Auto\pro\bin\ConnectionManager.exe", AppWinStyle.MinimizedNoFocus, False)
            End If
        End If
        Select Case action
            Case "S"
                caliberatetime(currenttime)
                wait(10)
                Application.Exit()
            Case "P"
                longrun(UEtype, interval)
            Case "L"
                Do While exitwindow = False
                    attachdetach(UEtype, a_interval)
                Loop
            Case "c"
                shutdown(UEtype)
                displaylog("UE shutdowned", "r")
            Case "e"
                resetUE(UEtype)
                displaylog("reset UE", "r")
            Case "pa"
                displaylog("paging mode", "r")
                Do While exitwindow = False
                    paging(UEtype, a_interval)
                Loop
            Case "I"
                attachtoidle(UEtype)
                Do While exitwindow = False
                    displaylog("go to sleep", "r")
                    wait(600)
                    monitoring()
                    If calldroped = True Then
                        attachtoidle(UEtype)
                    End If
                Loop
        End Select
        SerialPort.Close()
    End Sub

    Sub attachtoidle(ByVal uetype As String)

        ''----------------attach ue in -------------------------
        'Do
        '    Select Case uetype
        '        Case "H"
        '            attachdetach_HISI()
        '        Case "HE5776"
        '            attachdetach_HISI()

        '        Case "Qualcomm9600"
        '            'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
        '            attachdetach_Qualcomm("9600")
        '        Case "Qualcomm9028"
        '            'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
        '            attachdetach_Qualcomm("9028")

        '        Case "BandluxeC508"
        '            attachdetach_Bandrich()

        '        Case "ALT-C186"
        '            attachdetach_ALT()

        '        Case "dialcomm"
        '            attachdetach_comm()
        '    End Select
        'Loop Until (addroute() <> "ip not find") Or (exitwindow = True)
        Dim attachstate As Boolean = True
        Do 'UE attach until get add route ok
            attachstate = True
            '-------------- UE detach+attach-----------------
            Select Case uetype
                Case "H"
                    If attachdetach_HISI() <> "OK" Then
                        attachstate = False
                    End If

                Case "HE5776"
                    If attachdetach_HISI() <> "OK" Then
                        attachstate = False
                    End If

                Case "Qualcomm9600"
                    'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
                    attachdetach_Qualcomm("9600")

                Case "Qualcomm9028"
                    'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
                    attachdetach_Qualcomm("9028")
                Case "BandluxeC508"
                    attachdetach_Bandrich()

                Case "ALT-C186"
                    attachdetach_ALT()

                Case "dialcomm"
                    attachdetach_comm()
            End Select
            trytimes = trytimes + 1
            displaylog("attach try times:" + trytimes.ToString, "r")
            If addroute() = "ip not find" Then
                attachstate = attachstate * False
            End If
            If (uetype = "H" Or uetype = "HE5776") And attachstate = True Then
                If monitoring(True) <> "OK" Then
                    attachstate = False
                End If
            End If
            attachstate = attachstate * Not (exitwindow)
            If attachstate = False Then
                displaylog("attach fail", "r")
            End If
        Loop Until attachstate = True
    End Sub

    Sub paging(ByVal uetype As String, ByVal attachtime As Integer)
        Dim total, fail As Integer
        Dim paginglist As New Collection
        total = 0
        fail = 0

        '----------------attach ue in -------------------------
        'Do
        '    Select Case uetype
        '        Case "H"
        '            attachdetach_HISI()
        '        Case "HE5776"
        '            attachdetach_HISI()

        '        Case "Qualcomm9600"
        '            'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
        '            attachdetach_Qualcomm()

        '        Case "BandluxeC508"
        '            attachdetach_Bandrich()

        '        Case "ALT-C186"
        '            attachdetach_ALT()

        '        Case "dialcomm"
        '            attachdetach_comm()
        '    End Select
        'Loop Until (addroute() <> "ip not find") Or (exitwindow = True)
        'paginglist = readpagingtarget()

        'Do While exitwindow = False
        '    '--------read paging target
        '    paginglist = readpagingtarget()

        '    If paginglist.Count <> 0 Then
        '        For i = 1 To paginglist.Count

        '            If paginglist.Contains(UEip) Then
        '                If paginglist.Item(UEip) = paginglist.Item(i) Then
        '                Else

        '                    If pagingaddroute(paginglist.Item(i)) <> "ip not find" Then

        '                        total = total + 1
        '                        If pingoneue(paginglist.Item(i)) = "fail" Then
        '                            fail = fail + 1
        '                            displaylog("paging " + paginglist.Item(i) + " fail", "r")
        '                        Else
        '                            displaylog("paging " + paginglist.Item(i) + " success", "r")
        '                        End If
        '                    End If

        '                End If
        '            End If
        '        Next
        '        displaylog("paging fail rate:" + (fail * 100 / total).ToString("f2") + "%, " + fail.ToString + ", " + total.ToString, "r")
        '    Else
        '        displaylog("No target ue ips find in system", "r")
        '    End If

        '    wait(180)
        '    monitoring()
        '    If calldroped = True Then
        '        Do
        '            Select Case uetype
        '                Case "H"
        '                    attachdetach_HISI()
        '                Case "HE5776"
        '                    attachdetach_HISI()

        '                Case "Qualcomm9600"
        '                    'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
        '                    attachdetach_Qualcomm()

        '                Case "BandluxeC508"
        '                    attachdetach_Bandrich()

        '                Case "ALT-C186"
        '                    attachdetach_ALT()

        '                Case "dialcomm"
        '                    attachdetach_comm()
        '            End Select
        '        Loop Until (addroute() <> "ip not find") Or (exitwindow = True)
        '    End If
        'Loop

        Dim attachstate As Boolean = True
        Do 'UE attach until get add route ok
            attachstate = True
            '-------------- UE detach+attach-----------------
            Select Case uetype
                Case "H"
                    If attachdetach_HISI() <> "OK" Then
                        attachstate = False
                    End If

                Case "HE5776"
                    If attachdetach_HISI() <> "OK" Then
                        attachstate = False
                    End If

                Case "Qualcomm9600"
                    'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
                    attachdetach_Qualcomm("9600")

                Case "Qualcomm9028"
                    'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
                    attachdetach_Qualcomm("9028")
                Case "BandluxeC508"
                    attachdetach_Bandrich()

                Case "ALT-C186"
                    attachdetach_ALT()

                Case "dialcomm"
                    attachdetach_comm()
            End Select
            trytimes = trytimes + 1
            displaylog("attach try times:" + trytimes.ToString, "r")
            If addroute() = "ip not find" Then
                attachstate = attachstate * False
            End If
            If (uetype = "H" Or uetype = "HE5776") And attachstate = True Then
                If monitoring(True) <> "OK" Then
                    attachstate = False
                End If
            End If
            attachstate = attachstate * Not (exitwindow)
            If attachstate = False Then
                displaylog("attach fail", "r")
            End If
        Loop Until attachstate = True

        '-----------wait
        displaylog("start wait:" + attachtime.ToString + "s", "g")

        wait(attachtime)
        '-----------monitoring call drop
        'monitoring()
        '-----------caculate the counter
        realtimes = realtimes + 1
        If calldroped = False Then
            trytimes = trytimes + 1
            successtimes = successtimes + 1

        End If
        If calldroped = True And trytimes <> 0 Then
            trytimes = trytimes + 1
        End If
        If trytimes > 0 Then
            displaylog(" realtimes=" + realtimes.ToString + " trytimes=" + trytimes.ToString + " successtimes=" + successtimes.ToString + " successrate=" + (Int(successtimes / trytimes * 100)).ToString + "%", "r")
            wait(2)
        End If
    End Sub

    Function pingoneue(ByVal ip As String) As String
        Dim returnstr As String
        Dim values As String
        values = ""
        displaylog("paging ping ue:" + ip, "r")

        returnstr = rundoscommand("ping -w 10000 -n " + "5" + " " + ip)

        If InStr(returnstr, "unreachable") > 0 Then
            values = "100" '= ping fail
        Else
            Dim expression As New Regex("(\d+)% loss")
            'TextBox1.Text = TextBox1.Text & returnstr & vbNewLine
            Dim mc As MatchCollection = expression.Matches(returnstr)
            For i As Integer = 0 To mc.Count - 1
                values = mc(i).ToString.Split("%")(0)
                displaylog("ping " & mc(i).ToString, "r")

            Next
        End If

        If values = "" Or Int(values) = 100 Then

            ' displaylog("paging ping fail", "r")
            Return "fail"
        Else

            'displaylog("paging ping OK", "g")
            Return "OK"
        End If


    End Function
    Function pagingaddroute(ByVal ip) As String
        Dim subipstring, ipaddress, gateway, returnstr As String
        Dim netportname As String

        ipaddress = ""
        gateway = ""
        subipstring = ""
        netportname = SerialPort.PortName
        getcomip(netportname, True, ipaddress, gateway)
        If ipaddress = "" Or gateway = "" Then
            displaylog("ip not find", "g")
            Return "ip not find"
            Exit Function
        End If
        displaylog("Clear old route to" & ipaddress, "r")
        returnstr = rundoscommand("route delete " + ip)
        wait(1)
        displaylog("UE IP:" & UEip & "  Gateway:" & gateway, "r")
        returnstr = rundoscommand("route add " + ip + " " + gateway)

        Return "OK"

    End Function

    Function getallip(ByVal isIPV4 As Boolean) As String
        Dim adapters As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
        Dim adapter As NetworkInterface
        Dim ips As String = ""
        Dim ipaddress, gatewayipaddress As String

        For Each adapter In adapters
            Dim properties As IPInterfaceProperties = adapter.GetIPProperties()
            Dim unicastipaddressinformationcollection As UnicastIPAddressInformationCollection = properties.UnicastAddresses
            Dim unicastip As UnicastIPAddressInformation
            For Each unicastip In unicastipaddressinformationcollection

                If isIPV4 And unicastip.Address.AddressFamily.ToString = "InterNetwork" Then 'InterNetworkV6 means ipV6 address
                    ipaddress = unicastip.Address.ToString 'only one ip ge
                    ips = ips + ipaddress + ","
                ElseIf isIPV4 = False And unicastip.Address.AddressFamily.ToString = "InterNetworkV6" Then
                    ipaddress = unicastip.Address.ToString 'only one ip get
                    ips = ips + ipaddress + ","
                End If
            Next unicastip

            Dim gatewaycollection As GatewayIPAddressInformationCollection = properties.GatewayAddresses
            Dim gatewayip As GatewayIPAddressInformation
            For Each gatewayip In gatewaycollection
                If isIPV4 And gatewayip.Address.AddressFamily.ToString = "InterNetwork" Then 'InterNetworkV6 means ipV6 address
                    gatewayipaddress = gatewayip.Address.ToString 'only one ip ge
                    ips = ips + gatewayipaddress + ","
                ElseIf isIPV4 = False And gatewayip.Address.AddressFamily.ToString = "InterNetworkV6" Then
                    gatewayipaddress = gatewayip.Address.ToString 'only one ip get
                    ips = ips + gatewayipaddress + ","
                End If

            Next gatewayip

        Next adapter
        Return ips
    End Function



    Function readpagingtarget() As Collection
        Dim result As New Collection  'System.Collections.Specialized.StringCollection
        Dim i, j As Integer
        Dim Buff As String
        Dim searchedstr As String
        Dim LineBuff As Object
        Dim start As Long
        Dim filename As String
        Dim regexstr As String
        Dim innerip As New Collection
        Dim innerips As String
        Dim finalip As New Collection
        result.Clear()
        filename = Microsoft.VisualBasic.Left(logname, logname.LastIndexOf("\") + 1) + "ipaddress"
        regexstr = "((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))"
        Dim regex As Regex = New Regex(regexstr)
        If File.Exists(filename) Then
            innerips = getallip(True)
            FileOpen(1, filename, OpenMode.Binary, OpenAccess.Read, OpenShare.Shared)
            start = LOF(1)
            Buff = Space(100000)
            If start > 100000 Then
                start = start - 100000
            Else
                start = 1
            End If
            FileGet(1, Buff, start)
            FileClose(1)

            LineBuff = Split(Buff, vbCrLf) '这里的vbcrlf需要具体拿到你的文件的二进制格式才能确认是什么


            i = UBound(LineBuff)
            j = 1

            While (j = 1) And i > 0
                My.Application.DoEvents()
                searchedstr = LineBuff(i)
                If InStr(searchedstr, "new start") > 0 Then
                    j = 0

                End If



                If regex.IsMatch(searchedstr) Then

                    If result.Contains(regex.Match(searchedstr, regexstr).Value) = False Then

                        If innerips.IndexOf(Trim(regex.Match(searchedstr, regexstr).NextMatch.Value)) < 0 Then '本机的ip必须扣除

                            result.Add(regex.Match(searchedstr, regexstr).NextMatch.Value, regex.Match(searchedstr, regexstr).Value)
                        End If

                    Else

                    End If
                End If


                i = i - 1
            End While
        Else
            Return result
        End If
        '------------------------扣除系统所有的ip


        Return result
    End Function


    Sub startandroidnetlog()
        If File.Exists("d:\mueauto\adb\adb.exe") Then
            Dim myprocess As Process = New Process()
            myprocess.StartInfo.FileName = "cmd.exe" 'd:\\mueauto\\adb\\adb.exe" + " -s " + androiddevid + " shell ping 127.0.0.1 > d:\uelog.tp"
            myprocess.StartInfo.Arguments = " /C d:\mueauto\adb\adb.exe" + " -s " + androiddevid + " shell sh /system/bin/netspeed.sh wlan0 1 > " + TPlogname
            'myprocess.StartInfo.UseShellExecute = True
            myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
            myprocess.Start()
            displaylog("android net monitor session id opened:" & myprocess.Id.ToString & " do not close it", "g")
        Else
            displaylog("Can not find d:\mueauto\adb\adb.exe, it is key to control andriod phone", "g")

        End If

    End Sub

    Function caliberatetime(ByVal nowDT As Long) As String
        Dim realnow As DateTime

        realnow = DateAdd("s", nowDT, "2000-01-01 00:00:00")

        displaylog("set date time:" + realnow.ToString, "g")
        TimeOfDay = realnow
        Today = realnow
        Return "OK"
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

        displaylog(s, "g", False)

        sIn.Close()
        sOut.Close()
        sErr.Close()
        myProcess.Close()
        Return s
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    Sub longrun(ByVal uetype As String, ByVal attachtime As Integer)

        Dim attachstate As Boolean = True
        Dim droptimes As Integer
        droptimes = 0
        trytimes = 0
        displaylog("long run start", "r")
        'Shell("d:\mueauto\killuesoft.bat")
        myprocess.killprocessbyimportfile("d:\mueauto\killuesoft.cfg")
        Do 'UE attach until get add route ok
            attachstate = True
            '-------------- UE detach+attach-----------------
            Select Case uetype
                Case "H"
                    If attachdetach_HISI() <> "OK" Then
                        attachstate = False
                    End If

                Case "HE5776"
                    If attachdetach_HISI() <> "OK" Then
                        attachstate = False
                    End If

                Case "Qualcomm9600"
                    'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
                    attachdetach_Qualcomm("9600")

                Case "Qualcomm9028"
                    'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
                    attachdetach_Qualcomm("9028")
                Case "BandluxeC508"
                    attachdetach_Bandrich()

                Case "ALT-C186"
                    attachdetach_ALT()

                Case "dialcomm"
                    attachdetach_comm()
            End Select
            trytimes = trytimes + 1
            displaylog("attach try times:" + trytimes.ToString, "r")
            If addroute() = "ip not find" Then
                attachstate = attachstate * False
            End If
            If (uetype = "H" Or uetype = "HE5776") And attachstate = True Then
                If monitoring(True) <> "OK" Then
                    attachstate = False
                End If
            End If
            attachstate = attachstate * Not (exitwindow)
            If attachstate = False Then
                displaylog("attach fail", "r")
            End If
        Loop Until attachstate = True

        '-----------run ftp or http
        If traffictype = "ftpdl" Or traffictype = "ftpul" Or traffictype = "ftpdlul" Then
            rundlftp()
        End If
        If traffictype = "http" Then
            rundlhttp()
        End If
        If traffictype = "httpdownload" Then
            rundlhttpdownload()
        End If
        If traffictype = "ping" Then
            runping()
        End If
        If traffictype = "video" Then
            runvideo()
        End If

        If traffictype = "volte" Then
            runvolte()
        End If
MOC:    If traffictype = "MOC" Then
            runping()
            runMOC()
        End If
        If traffictype = "MTC" Then
            runping()
            runMTC()

        End If
        Do While exitwindow = False
            '-----------wait
            If traffictype <> "MOC" Then
                wait(2)
                displaylog("start wait " + interval + "s", "g")
                wait(Int(interval))

            End If

            '-----------monitoring call drop
            monitoring()
            '-----------caculate the counter
            ' realtimes = realtimes + 1

            If calldroped = True Then
                droptimes = droptimes + 1
                displaylog("call droped times:" + droptimes.ToString, "r")
                wait(2)
                If traffictype = "ftpdl" Or traffictype = "ftpul" Or traffictype = "ftpdlul" Then killoldftp()
                If traffictype = "ping" Then killoldping()
                If traffictype = "video" Then killvideo()
                If traffictype = "volte" Then killoldvolte()
                If traffictype = "MOC" Then
                    killoldping()
                End If
                If traffictype = "MTC" Then
                    killoldping()
                End If
                attachstate = True
                Do 'UE attach until get add route ok
                    attachstate = True
                    '-------------- UE detach+attach-----------------
                    Select Case uetype
                        Case "H"
                            If attachdetach_HISI() <> "OK" Then
                                attachstate = False
                            End If

                        Case "HE5776"
                            If attachdetach_HISI() <> "OK" Then
                                attachstate = False
                            End If

                        Case "Qualcomm9600"
                            'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
                            attachdetach_Qualcomm("9600")

                        Case "Qualcomm9028"
                            'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
                            attachdetach_Qualcomm("9028")
                        Case "BandluxeC508"
                            attachdetach_Bandrich()

                        Case "ALT-C186"
                            attachdetach_ALT()

                        Case "dialcomm"
                            attachdetach_comm()
                    End Select
                    trytimes = trytimes + 1
                    displaylog("attach try times:" + trytimes.ToString, "r")
                    If addroute() = "ip not find" Then
                        attachstate = attachstate * False
                    End If
                    If (uetype = "H" Or uetype = "HE5776") And attachstate = True Then
                        If monitoring(True) <> "OK" Then
                            attachstate = False
                        End If
                    End If
                    attachstate = attachstate * Not (exitwindow)
                    If attachstate = False Then
                        displaylog("attach fail", "r")
                    End If
                Loop Until attachstate = True
                If traffictype = "ftpdl" Or traffictype = "ftpul" Or traffictype = "ftpdlul" Then
                    rundlftp()
                End If
                If traffictype = "http" Then
                    rundlhttp()
                End If
                If traffictype = "httpdownload" Then
                    rundlhttpdownload()
                End If
                If traffictype = "ping" Then
                    runping()
                End If
                If traffictype = "video" Then
                    runvideo()
                End If
                If traffictype = "volte" Then
                    runvolte()
                End If
                If traffictype = "MOC" Then
                    runping()
                    runMOC()
                End If
                If traffictype = "MTC" Then
                    runping()
                    runMTC()
                End If
            Else
                If traffictype = "MOC" Then
                    GoTo MOC
                End If

            End If
            displaylog("drop times:" + droptimes.ToString, "r")
           
        Loop
    End Sub
    Sub runMOC()
        Dim count As Integer
        Dim phonenum As String = "10086"
        Dim adbdvicesn As String = ""
        Dim times As Integer
        Dim callinteval As Integer
        If imsi <> "" Then
            displaylog("imsi:" + imsi, "g")
            phonenum = getcallnum()
            If phonenum <> "" Then
                callinteval = a_interval
                times = Int((interval + 10) / callinteval + 1)
                For i = 1 To times
                    serialSendData("ATD" + phonenum + ";", logname)
                    displaylog("ATD" + phonenum + ";", "g")

                    wait(callinteval)
                    serialSendData("ATH", logname)
                    displaylog("ATH", "g")
                    wait(10)
                Next
            Else
                displaylog("Check target phone number in ftp.ini", "r")
            End If

        Else
            displaylog("Not find IMSI number.", "r")
        End If


    End Sub

    Sub runMTC()

    End Sub

    Sub displaylog(ByVal addstr As String, ByVal strcolor As String, Optional ByVal logout As Boolean = True)
        locked = True
        If Trim(addstr) <> " " Then
            Try
                Select Case strcolor
                    Case "g"
                        '  WebBrowser1.DocumentText = WebBrowser1.DocumentText & "<br><span lang=EN-US style='font-size:9.0pt;font-family:""Microsoft Sans Serif"";color:#00CC00'>" & addstr & vbNewLine
                        ConsoleHelper.displaylog(Trim(addstr))
                        If logout = True Then writelog(addstr, 0, logname) Else writelog("ipconfig", 0, logname)
                        ' locked = False
                    Case "r"
                        ' WebBrowser1.DocumentText = WebBrowser1.DocumentText & "<br><span ltang=EN-US style='font-size:9.0pt;font-family:""Microsoft Sans Serif"";color:red'>" & addstr & vbNewLine
                        ConsoleHelper.displaylog(Trim(addstr), "r")
                        If logout = True Then writelog(addstr, 0, logname) Else writelog("ipconfig", 0, logname)

                End Select

            Catch
            End Try
            locked = False
        End If
    End Sub
    Sub displaylog2(ByVal addstr1 As String, ByVal strcolor1 As String)
        If Trim(addstr1) <> " " Then
            Try
                'System.Windows.Forms.Application.DoEvents() '转让控制权
                ConsoleHelper.displaylog(Trim(addstr1))

                writeueiplog(Trim(addstr1), logname)
                writelog(addstr1, 0, logname)

            Catch
            End Try
        End If
    End Sub
    Sub displayTPlog(ByVal addstr1 As String, ByVal strcolor1 As String, ByVal second As Boolean)
        If Trim(addstr1) <> " " Then
            Try
                'System.Windows.Forms.Application.DoEvents() '转让控制权
                ' ConsoleHelper.displaylog((addstr1))

                writeTPlog(addstr1, 0, TPlogname, second)

            Catch ex As Exception
                Dim a As String = ex.Message
                Dim b As String
            End Try
        End If
    End Sub

    Function findueip(ByVal inputstr As String) As String
        Dim regexstr As String
        regexstr = "ip=((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))"
        Dim regex As Regex = New Regex(regexstr)
        If regex.IsMatch(inputstr) Then
            Return UEip + ":" + Replace(regex.Match(inputstr, regexstr).Groups(0).Value, "ip=", "")
        Else
            Return "0"
        End If


    End Function

    Public Function writeueiplog(ByVal logstr As String, ByVal logname As String)
        Dim outputstr, fn, ip As String
        Dim writed As Boolean
        Dim i As Integer
        ip = UEname
        outputstr = "~OP|" + ip + "|" + Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + logstr + vbCrLf
        TCPwrite(outputstr)
        'If MyClient Is Nothing Then
        'Else
        '    outputstr = "$OP|" + ip + "|" + Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + logstr + vbCrLf
        '    If MyClient.Connected = True Then MyClient.SendData(Encoding.ASCII.GetBytes(outputstr))
        'End If

        writeueiplog = ""
        'logstr = findueip(logstr)
        'If logstr <> "0" Then

        '    i = 0
        '    writed = False
        '    ' My.Application.DoEvents()

        '    fn = Microsoft.VisualBasic.Left(logname, logname.LastIndexOf("\") + 1) + "ipaddress"
        '    Do While writed = False And i < 3
        '        Try
        '            Dim fw As System.IO.StreamWriter = New System.IO.StreamWriter(fn, True, System.Text.Encoding.UTF8)


        '            outputstr = Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + logstr + vbCrLf
        '            fw.Write(outputstr)

        '            writed = True
        '            fw.Close()
        '            fw.Dispose()
        '            Return ""
        '        Catch e As Exception
        '            writed = False

        '        End Try
        '        i = i + 1
        '    Loop
        '    Return "OK"

        'End If
        Return "OK"
    End Function
    Sub findimsi(ByVal inputstr As String)
        Dim match As System.Text.RegularExpressions.Match = System.Text.RegularExpressions.Regex.Match(inputstr, "\d{15}")
        If match.Success Then
            If match.Groups(0).Value.IndexOf("460") = 0 Then
                imsi = match.Groups(0).Value

            End If
        End If

    End Sub



    Public Function writelog(ByVal logstr As String, ByVal Start As Boolean, ByVal logname As String)
        Dim outputstr, fn, ip As String
        Dim writed As Boolean
        Dim i As Integer
        ip = UEname
        If imsi = "" Then
            findimsi(logstr)
        End If

        If Start = True Then
            outputstr = "~OP|" + ip + "|new " + vbCrLf
            TCPwrite(outputstr)
            'If MyClient Is Nothing Then
            'Else
            '    If MyClient.Connected = True Then MyClient.SendData(Encoding.ASCII.GetBytes(outputstr))
            'End If
        Else
            outputstr = "~OP|" + ip + "|" + Now.ToString("MM/dd/yyyy   HH:mm:ss") + "|" + logstr + vbCrLf
            TCPwrite(outputstr)
            'If MyClient Is Nothing Then
            'Else
            '    If MyClient.Connected = True Then MyClient.SendData(Encoding.ASCII.GetBytes(outputstr))
            'End If
        End If

        If logstr.IndexOf("RING") >= 0 Then
            serialSendData("ATA" & vbCrLf, logname)
        End If


        i = 0
        writed = False
        ' My.Application.DoEvents()
        'fn = logname
        'Do While writed = False And i < 3
        '    Try
        '        Dim fw As System.IO.StreamWriter = New System.IO.StreamWriter(fn, True, System.Text.Encoding.UTF8)

        '        If Start = True Then
        '            outputstr = Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + "start--------------------------------" + vbCrLf
        '            fw.Write(outputstr)
        '            'outputstr = Now + " " + "UE-ip time drop-times drop-rate attach-times attach-success TP-time DLPT ULPT" + vbCrLf
        '            'fw.Write(outputstr)
        '        Else
        '            outputstr = Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + logstr + vbCrLf
        '            fw.Write(outputstr)
        '        End If

        '        writed = True
        '        fw.Close()
        '        fw.Dispose()
        '        writelog = ""
        '    Catch e As Exception
        '        writed = False
        '        writelog = e.Message
        '    End Try
        '    i = i + 1
        'Loop
        Return "OK"
    End Function

    Public Function writeTPlog(ByVal logstr As String, ByVal Start As Boolean, ByVal logname As String, ByVal second As Boolean)
        Dim outputstr, fn, ip As String
        Dim writed As Boolean
        Dim i As Integer
        ip = UEname
        If Start = True Then
            outputstr = "~TP|" + ip + "|new " + vbCrLf
            TCPwrite(outputstr)
            'writetploglocal(outputstr, Start, logname)
            'If MyClient Is Nothing Then
            'Else
            '    If MyClient.Connected = True Then MyClient.SendData(Encoding.ASCII.GetBytes(outputstr))
            'End If
        Else
            If second = True Then
                outputstr = "~TP|" + ip + "|s|" + Now.ToString("MM/dd/yyyy   HH:mm:ss") + "|" + logstr + vbCrLf
            Else
                outputstr = "~TP|" + ip + "|i|" + Now.ToString("MM/dd/yyyy   HH:mm:ss") + "|" + logstr + vbCrLf
            End If
            TCPwrite(outputstr)
            writetploglocal(outputstr, Start, logname)
            'If MyClient Is Nothing Then
            'Else
            '    If MyClient.Connected = True Then MyClient.SendData(Encoding.ASCII.GetBytes(outputstr))
            'End If
        End If




        'i = 0
        'writed = False
        'My.Application.DoEvents()
        'fn = logname
        'Do While writed = False And i < 3
        '    Try
        '        Dim fw As System.IO.StreamWriter = New System.IO.StreamWriter(fn, True, System.Text.Encoding.UTF8)

        '        If Start = True Then
        '            outputstr = " new start log " + "File:      Traffic Rates Log" + vbCrLf
        '            fw.WriteLine("")
        '            fw.Write(outputstr)
        '            'outputstr = Now + " " + "UE-ip time drop-times drop-rate attach-times attach-success TP-time DLPT ULPT" + vbCrLf
        '            'fw.Write(outputstr)

        '        Else
        '            outputstr = Now.ToString("MM/dd/yyyy   HH:mm:ss") + "|" + logstr + vbCrLf
        '            fw.Write(outputstr)

        '        End If

        '        writed = True
        '        fw.Close()
        '        fw.Dispose()
        '        writeTPlog = ""
        '    Catch e As Exception
        '        writed = False
        '        writeTPlog = e.Message
        '    End Try
        '    i = i + 1
        'Loop
        Return "OK"
    End Function

    Sub writetploglocal(ByVal logstr As String, ByVal Start As Boolean, ByVal logname As String)
        Dim outputstr, fn, ip As String
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


    Private Sub SerialPort_ErrorReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialErrorReceivedEventArgs) Handles SerialPort.ErrorReceived
        Dim aa As Integer
        aa = 1
    End Sub





    Private Function formateTP(ByVal DLTP As String, ByVal ULTP As String) As String
        While DLTP.Length < 17
            DLTP = " " + DLTP
        End While
        While ULTP.Length < 15
            ULTP = " " + ULTP
        End While
        formateTP = DLTP + ULTP
    End Function

    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Function doGetRequest(ByVal url As String) As String
        Dim strReturn As String = ""
        Dim hwRequest As System.Net.HttpWebRequest
        Dim hwResponse As System.Net.HttpWebResponse
        Try
            hwRequest = System.Net.HttpWebRequest.Create(url)
            hwRequest.Timeout = 5000
            hwRequest.Method = "GET"
            hwRequest.ContentType = "application/x-www-form-urlencoded"

            hwResponse = hwRequest.GetResponse()
            Dim srReader As System.IO.StreamReader = New System.IO.StreamReader(hwResponse.GetResponseStream(), System.Text.Encoding.ASCII)
            strReturn = srReader.ReadToEnd()
            srReader.Close()
            hwResponse.Close()

        Catch
        End Try
        Return strReturn
    End Function

    '发送HTTP POST请求得结果
    Private Function doPostRequest(ByVal url As String, ByVal bData() As Byte) As String
        Dim strReturn As String = ""
        Dim hwRequest As System.Net.HttpWebRequest
        Dim hwResponse As System.Net.HttpWebResponse
        Try
            hwRequest = System.Net.HttpWebRequest.Create(url)
            hwRequest.Timeout = 5000
            hwRequest.Method = "POST"
            hwRequest.Headers.Add("x-requested-with", "XMLHttpRequest")
            hwRequest.ProtocolVersion = HttpVersion.Version11
            hwRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8"
            hwRequest.ContentLength = bData.Length

            Dim smWrite As System.IO.Stream = hwRequest.GetRequestStream()
            smWrite.Write(bData, 0, bData.Length)
            smWrite.Close()

            hwResponse = hwRequest.GetResponse()
            Dim srReader As System.IO.StreamReader = New System.IO.StreamReader(hwResponse.GetResponseStream(), System.Text.Encoding.ASCII)
            strReturn = srReader.ReadToEnd()
            srReader.Close()
            hwResponse.Close()
        Catch

        End Try
        Return strReturn
    End Function


    '*************************************************************************'
    'TCP client
    '*************************************************************************


    Sub MyClient_DataArrived(ByVal value() As Byte, ByVal Len As Integer) Handles MyClient.DataArrived
        Dim a As Integer = 0
        a = 1
        a = Console.CursorLeft
        Console.Write("!")
        '("receive things:" + Trim(Encoding.ASCII.GetString(value)))

        Console.CursorLeft = a
    End Sub


    Sub MyClient_Exception(ByVal ex As System.Exception) Handles MyClient.Exception
        Dim a As Integer = 0
        Console.WriteLine("Client error:" + ex.Message)
        ' MyClientreconnect()

    End Sub
    Sub MyClient_ClientClosed() Handles MyClient.ClientClosed
        Console.Write("x") '("Log server disconnected")
        ' MyClientreconnect()

    End Sub
    Sub MyClientreconnect(ByVal initial As Boolean)
        Dim i = 0
        Dim a As Integer = 0
        If MyClient Is Nothing Then
            Try
                MyClient = New TCPClient(logip, Str(port), 0, 500)

                ' MyClient.ConnectHost()
            Catch errors As Exception

            End Try

        End If
        If initial = True Then
            While MyClient.Connected = False
                Try
                    If i = 0 Then
                        a = Console.CursorLeft
                        Console.Write("?") '("Try connect to log server:" + logip)
                        Console.CursorLeft = a
                    Else
                        a = Console.CursorLeft
                        Console.Write("?") '("Try reconnect to log server:" + logip)
                        Console.CursorLeft = a
                    End If
                    Thread.Sleep(1000)
                    'MyClient.Close()
                    Application.DoEvents()
                    MyClient = New TCPClient(logip, Str(port), 0, 500)

                    'MyClient.ConnectHost()

                Catch ex As Exception

                End Try
                i = i + 1
            End While
        Else
            Try
                If i = 0 Then
                    a = Console.CursorLeft
                    Console.Write("?") '("Try connect to log server:" + logip)
                    Console.CursorLeft = a
                Else
                    a = Console.CursorLeft
                    Console.Write("?") '("Try reconnect to log server:" + logip)
                    Console.CursorLeft = a
                End If
                Thread.Sleep(1000)
                'MyClient.Close()
                Application.DoEvents()
                MyClient = New TCPClient(logip, Str(port), 0, 500)

                'MyClient.ConnectHost()

            Catch ex2 As Exception

            End Try
        End If
        If MyClient.Connected = True Then
            Console.Write("o") '("Conneced!")
            ' Timer1.Enabled = True
        End If
    End Sub




    Private Sub Timer1_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        'Dim output As String
        'If MyClient Is Nothing Then
        '    MyClientreconnect(False)
        '    Exit Sub
        'End If
        'If MyClient.Connected = False Then
        '    MyClientreconnect(False)
        '    Exit Sub
        'End If

    End Sub
    Sub myclient1connect()
        'MyClient1 = New SingleConnection(New IPEndPoint(IPAddress.Parse(logip), port))
        '' AddHandler MyClient1.ReceiveEventHandler, AddressOf ReceiveEventHandler
        'MyClient1.Connect()
    End Sub

    'Private Shared Sub ReceiveEventHandler(ByVal sender As Object, ByVal args As NTCPMSG.Event.ReceiveEventArgs) Handles MyClient1.ReceiveEventHandler
    '    Select Case args.Event
    '        Case "PushMessage"
    '            'Get OneWay message from server
    '            If (Not (args.Data) Is Nothing) Then
    '                Try
    '                    Console.WriteLine(Encoding.UTF8.GetString(args.Data))
    '                Catch e As Exception
    '                    Console.WriteLine(e)
    '                End Try

    '            End If

    '    End Select

    'End Sub

    'Private Shared Sub errorEventHandler(ByVal sender As Object, ByVal e As NTCPMSG.Event.ErrorEventArgs) Handles MyClient1.ErrorEventHandler
    '    Select Case e.ErrorException.Message
    '        Case "PushMessage"
    '            'Get OneWay message from server
    '            If (Not ("") Is Nothing) Then
    '                Try
    '                    'Console.WriteLine(Encoding.UTF8.GetString(""))
    '                Catch ex As Exception
    '                    Console.WriteLine(e)
    '                End Try

    '            End If

    '    End Select
    'End Sub
    'Private Sub RemoteDisconnected(ByVal sender As Object, ByVal e As NTCPMSG.Event.DisconnectEventArgs) Handles MyClient1.RemoteDisconnected
    '    Select Case e.CableId
    '        Case "PushMessage"
    '            'Get OneWay message from server
    '            If (Not ("") Is Nothing) Then
    '                Try
    '                    'Console.WriteLine(Encoding.UTF8.GetString(""))
    '                Catch ex As Exception
    '                    Console.WriteLine(e)
    '                End Try

    '            End If

    '    End Select
    'End Sub
    Function checkthroughput(ByVal dlorul As String, ByVal throughput As Int64) As Int64
        If dlorul = "DL" Then
            If System.Math.Abs((throughput - dlcounter(2)) / dlcounter(2)) < 0.5 Then
                dlcounter(0) = dlcounter(1)
                dlcounter(1) = dlcounter(2)
                dlcounter(2) = throughput
                dlcounterfilter(0) = dlcounterfilter(1)
                dlcounterfilter(1) = dlcounterfilter(2)
                dlcounterfilter(2) = throughput
                Return throughput
            End If
            If (throughput = 0 And dlcounter.Sum = 0) Then
                dlcounter(0) = dlcounter(1)
                dlcounter(1) = dlcounter(2)
                dlcounter(2) = throughput
                dlcounterfilter(0) = dlcounterfilter(1)
                dlcounterfilter(1) = dlcounterfilter(2)
                dlcounterfilter(2) = throughput
                Return 0
            Else

                dlcounter(0) = dlcounter(1)
                dlcounter(1) = dlcounter(2)
                dlcounter(2) = throughput
                dlcounterfilter(0) = dlcounterfilter(1)
                dlcounterfilter(1) = dlcounterfilter(2)
                dlcounterfilter(2) = throughput
                If throughput = 0 Then
                    Return dlcounter(1)
                End If

                If throughput > 2 * dlcounterfilter(1) Or 2 * throughput < dlcounterfilter(1) Then
                    dlcounterfilter(2) = (throughput - dlcounterfilter(1)) / 10 + dlcounterfilter(1)
                End If
                Return dlcounter.Average


            End If


        Else
            If System.Math.Abs((throughput - ulcounter(2)) / ulcounter(2)) < 0.5 Then
                ulcounter(0) = ulcounter(1)
                ulcounter(1) = ulcounter(2)
                ulcounter(2) = throughput
                ulcounterfilter(0) = ulcounterfilter(1)
                ulcounterfilter(1) = ulcounterfilter(2)
                ulcounterfilter(2) = throughput
                Return throughput
            End If
            If (throughput = 0 And ulcounter.Sum = 0) Then
                ulcounter(0) = ulcounter(1)
                ulcounter(1) = ulcounter(2)
                ulcounter(2) = throughput
                ulcounterfilter(0) = ulcounterfilter(1)
                ulcounterfilter(1) = ulcounterfilter(2)
                ulcounterfilter(2) = throughput
                Return 0
            Else

                ulcounter(0) = ulcounter(1)
                ulcounter(1) = ulcounter(2)
                ulcounter(2) = throughput
                ulcounterfilter(0) = ulcounterfilter(1)
                ulcounterfilter(1) = ulcounterfilter(2)
                ulcounterfilter(2) = throughput
                If throughput = 0 Then
                    Return ulcounter(1)
                End If

                If throughput > 2 * ulcounterfilter(1) Or 2 * throughput < ulcounterfilter(1) Then
                    ulcounterfilter(2) = (throughput - ulcounterfilter(1)) / 10 + ulcounterfilter(1)
                End If
                Return ulcounter.Average


            End If


        End If


    End Function
    Private Sub Timer4_Elapsed(ByVal sender As System.Object, ByVal e As System.Timers.ElapsedEventArgs) Handles Timer2.Elapsed
        Dim output As String
        Dim DLthroughputlast, ULthroughputlast As Int64
        writetploglocal("check0", True, TPlogname)
        Try
            DLthroughputlast = Val(DLthroughput.NextValue.ToString)
            ULthroughputlast = Val(ULthroughput.NextValue.ToString)
            writetploglocal(DLthroughput.NextValue.ToString, True, TPlogname)
            writetploglocal(ULthroughput.NextValue.ToString, True, TPlogname)
        Catch ex As Exception
            Dim aa = 1
        End Try
        writetploglocal("check1", True, TPlogname)
        DLthroughputlast = checkthroughput("DL", DLthroughputlast)
        ULthroughputlast = checkthroughput("UL", ULthroughputlast)
        If TPintervalcounter > (TPinterval) Then


            TPintervalcounter = 1

            If TPintervalcounter = 1 Then
                DLthroughputhis = DLthroughputlast
                ULthroughputhis = ULthroughputlast
                TPintervalcounter = TPintervalcounter + 1
            End If
            output = formateTP((DLthroughputlast * 8).ToString, (ULthroughputlast * 8).ToString)
            displayTPlog(output, "r", True)

            Exit Sub
        End If

        If TPintervalcounter <= TPinterval Then
            DLthroughputhis = DLthroughputhis + DLthroughputlast
            ULthroughputhis = ULthroughputhis + ULthroughputlast
            If TPintervalcounter = (TPinterval) Then
                'displaylog("netcard device name :" + netcardname, "r")
                output = formateTP((DLthroughputhis * 8 \ TPintervalcounter).ToString, (ULthroughputhis * 8 \ TPintervalcounter).ToString)
                displayTPlog(output, "r", False)
            End If
            TPintervalcounter = TPintervalcounter + 1

        End If
        'writetploglocal("check2", True, TPlogname)
        'output = formateTP((DLthroughputlast * 8).ToString, (ULthroughputlast * 8).ToString)
        'displayTPlog(output, "r", True)



        'End If
    End Sub

    Private Sub Timer3_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer3.Tick

    End Sub

    Private Sub Timer4_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer4.Tick

        Select Case UEtype

            Case "HE5776"
                serialSendData("AT^HCSQ" & vbCrLf, logname)
                wait(1)
                serialSendData("AT+CGREG?" & vbCrLf, logname)
            Case "Qualcomm9600"
                'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
                serialSendData("AT+CGREG?" & vbCrLf, logname)
                wait(1)
                serialSendData("AT$QCSQ" & vbCrLf, logname)
                wait(1)
                serialSendData("AT!lteinfo" & vbCrLf, logname)
                wait(1)
                serialSendData("AT+CMGSI=4" & vbCrLf, logname)
            Case "Qualcomm9028"
                serialSendData("AT+CGREG?" & vbCrLf, logname)
                wait(1)
                serialSendData("AT$QCSQ" & vbCrLf, logname)
                wait(1)
                serialSendData("AT+CMGSI=4" & vbCrLf, logname)
        End Select


    End Sub
End Class

