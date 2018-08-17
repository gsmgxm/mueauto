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
    Dim UEtype, serverip, a_interval, interval, action, ftpsessionnum, UEip, UErealip, UEname, serialportname, traffictype, logip, serverip2, serverip3, serverip4, androiddevid As String
    Dim port As Integer = 2500
    Dim exitwindow As Boolean
    Dim ftphandle(8), pinghandle(8), videohandle(8), voltehandle(4), httphandle, volteadbahhandle, udpdlhandle, udpulhandle As Integer
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
    Dim connectedtime As DateTime = Now
    Dim disconnecttimes As Integer = 0
    Dim appstarttime As DateTime = Now
    Dim appstate As String = "run"
    Dim uerealtype As String = "Qualcomm"
    Dim sierraresettime As Integer = 2
    Dim DLtpcounter As New List(Of Integer)
    Dim adbdvicesn As String = ""
    Dim tcperror As String = "0"
    Dim t As Thread
    Dim result As String = ""
    Dim ueresetflag As String
    Dim paginglist As New List(Of String)
    Dim updatepaginglistflag = False
    Dim oldpagingstring As String = ""
    Dim pagingsessionlist As New List(Of Int64)
    Dim backprogress As Integer
    Dim ueinternalip As String
    Dim udpulstr As String = ""
    Dim udpdlstr As String = ""
    Dim targetphonenumber As String = ""
    Dim sessionlist As New Dictionary(Of String, String)
    Dim currenttime As Long
    Dim isring As Boolean = False
    Dim isoff As Boolean = False
    Dim callfail As Boolean = False
    Dim waitadbsn As Boolean = False
    Dim isanswering As Boolean = False
    Dim restartingflag As Boolean = False
    Dim disablenetcard As String = "0"
    Dim mCallState As String = "0"
    Dim mCallIncomingNumber As String = ""
    Dim mDataConnectionState As String = "-1"
    Dim mRingCallState As String = "0"
    Dim mlasttime As String = "0"
    Dim showdebuginfo As Boolean = False
    Dim IOlock As Boolean = False
    Dim adbinputmessagepool As New List(Of String)
    Dim adbresultmessagepool As New List(Of String)
    Dim adbinputpoolsize As Integer
    Dim returnstrdic As New Dictionary(Of String, String)
    Dim flagtime4 As Integer = 1
    Sub updatepaginglist(ByVal source As String)
        Dim ips As Object
        paginglist.Clear()
        Dim ipsstring As String
        Dim localip As String
        ipsstring = Split(source, "idlelist,")(1)
        ips = Split(ipsstring, ",")
        localip = GetLocalIPe()
        For Each ip As String In ips
            If localip.IndexOf(Trim(ip)) < 0 Then
                paginglist.Add(Trim(ip))
            End If

        Next
    End Sub
    Sub ReceiveEventHandler1(ByVal sender As Object, ByVal e As NTCPMSG.Event.ReceiveEventArgs)
        Dim a = 1
        If Encoding.ASCII.GetString(e.Data).IndexOf("!") >= 0 Then
            syncflag = True
            connectedtime = Now
            If action = "pa" And Encoding.ASCII.GetString(e.Data).IndexOf("idlelist,") >= 0 And Trim(Encoding.ASCII.GetString(e.Data)) <> oldpagingstring Then
                updatepaginglist(Encoding.ASCII.GetString(e.Data))
                oldpagingstring = Trim(Encoding.ASCII.GetString(e.Data))
                updatepaginglistflag = True
            End If
        End If

        If tcperror = "1" Then
            a = Console.CursorLeft
            Console.Write("!") '("receive things:" + Trim(Encoding.ASCII.GetString(value)))
            Console.CursorLeft = a

        End If
        ' 
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
    Sub connected(ByVal sender As Object, ByVal e As NTCPMSG.Event.DisconnectEventArgs)
        syncflag = True
    End Sub
    Sub TCPwrite(ByVal message As String)
        'Dim receivestr As String
        Dim a As Integer = 0
        'Console.WriteLine(message)
        Try
            'If Mytcpclient Is Nothing Then
            '    ' TCPconnect()
            'Else
            '    If Mytcpclient.Connected = True And syncflag = True Then
            '        connectedtime = Now
            '    End If
            'End If
            If DateDiff(DateInterval.Second, connectedtime, Now) > 30 And syncflag = True Then
                syncflag = False
                If tcperror = "1" Then
                    Console.WriteLine("disconnection times" + disconnecttimes.ToString + "| sync out of 10s")
                End If
            End If

            If DateDiff(DateInterval.Second, connectedtime, Now) > disconnecttimes * 10 Then
                disconnecttimes = disconnecttimes + 1
                If Mytcpclient Is Nothing Then
                    TCPconnect()
                    Console.WriteLine("@" + disconnecttimes.ToString)
                    If Mytcpclient.Connected = True Then syncflag = True
                    disconnecttimes = 0
                End If

                If syncflag = False Or Mytcpclient.Connected = False Then
                    Mytcpclient.Close()
                    Mytcpclient.Connect(100)
                    Console.WriteLine("#" + disconnecttimes.ToString)
                    If Mytcpclient.Connected = True Then syncflag = True
                End If
                'While messagebuffer.Count > 3
                '    messagebuffer.RemoveAt(0)
                'End While
            End If
            If Mytcpclient.Connected = True And syncflag = True Then
                If disconnecttimes > 6 Then disconnecttimes = 0
                ''While messagebuffer.Count > 0
                ''    Dim retData1() As Byte = Mytcpclient.SyncSend(2, Encoding.ASCII.GetBytes(messagebuffer.Item(0)), 100)
                ''    receivestr = Encoding.ASCII.GetString(retData1)
                ''    If Trim(receivestr).IndexOf("~") >= 0 Then
                ''        syncflag = True


                ''        a = Console.CursorLeft
                ''        Console.Write("!") '("receive things:" + Trim(Encoding.ASCII.GetString(value)))
                ''        Console.CursorLeft = a
                ''        messagebuffer.RemoveAt(0)
                ''    Else
                ''        syncflag = False

                ''        Exit Sub
                ''    End If

                ''End While
                'Dim retData() As Byte = Mytcpclient.SyncSend(2, Encoding.ASCII.GetBytes(message), 10)
                'receivestr = Encoding.ASCII.GetString(retData)
                'If Trim(receivestr).IndexOf("~") >= 0 Then
                '    syncflag = True

                '    a = Console.CursorLeft
                '    Console.Write("!") '("receive things:" + Trim(Encoding.ASCII.GetString(value)))
                '    Console.CursorLeft = a

                'Else
                '    syncflag = False
                'End If
                Mytcpclient.AsyncSend(2, Encoding.ASCII.GetBytes(message))
                Mytcpclient.AsyncSend(1, Encoding.ASCII.GetBytes(" "))
            Else
                a = Console.CursorLeft
                Console.Write("?") '("receive things:" + Trim(Encoding.ASCII.GetString(value)))
                Console.CursorLeft = a
            End If
        Catch ex As Exception
            a = Console.CursorLeft
            If tcperror = "1" Then

                Console.WriteLine("disconnection times" + disconnecttimes.ToString + "|" + ex.Message.ToString)
            End If
            Console.Write("?")
            Console.CursorLeft = a
            syncflag = False
            'While messagebuffer.Count > 3
            '    messagebuffer.RemoveAt(0)
            'End While
            'messagebuffer.Add(message)


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
        Dim count As Integer
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
        myprocess.killwindowbytitle(serialportname.ToUpper + ".")
        If traffictype = "ftpdl" Or traffictype = "ftpul" Or traffictype = "ftpdlul" Then killoldftp()
        If traffictype = "ping" Then killoldping()
        If traffictype = "video" Then killvideo()
        If traffictype = "volte" Then killoldvolte()
        If action = "V" Then killoldvolte()
        If traffictype = "http" Then killoldhttp()
        If traffictype = "httpdownload" Then killoldhttpdownload()
        If traffictype = "MOC" Or traffictype = "MTC" Then killoldhttpdownload()
        If action = "pa" Then killpagingping()
        displaylog("Done", "r")
        exitwindow = True
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' killexistingprocessHandle("MUEcon")
        '------------------------------------------------------------------------------------------------------
        'getportinterface("com11")
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
        'runcommerialadroidcall()
        'testrunmain()
       

    End Sub
    Sub testrunmain()
        Dim enabledebuginfostr As String
        'Dim teststr As String = "1.1.506/29/2017   07:01:25|AT+CGCONTRDP ERROR"
        'teststr = teststr + vbCr + "+CGCONTRDP: 1,5,tddltenoip.com,172.18.254.233,253.150.186.23.147.140.254.217.0.0.0.0.0.0.0.2, 254.128.0.0.0.0.0.0.0.0.0.0.0.0.0.1,,"
        'Console.WriteLine(findueinternalip(teststr))
        'Console.ReadLine()
        enabledebuginfostr = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "info", "enabledebuginfo")
        If enabledebuginfostr = "1" Then showdebuginfo = True

        Dim returnstr = andriodcommand(serialportname, "RSRP")
        displaylog(returnstr, "r")
        'returnstr = andriodcommand("9887fc434152443455", "IMSI")
        'displaylog(returnstr, "g")
        While 1
            returnstr = andriodcommand(serialportname, "TP")
            displaylog(returnstr, "g")
            wait(1)
        End While

        Console.ReadLine()



    End Sub
    Sub stopcommerialadroidcall()
        Dim sn As String = "A710e"
        Console.WriteLine(rundoscomandt("adb -s " + sn + " push stop /data/local/tmp/stop.txt"))
    End Sub
    Function checkoldsh(ByVal sn As String, ByVal killtitle As String, ByVal killmark As String) As String
        Dim psinfo() As String
        Dim pid As String = ""
        'Dim killtitle As String = "sh"
        'Dim killmark As String = "shell"
        psinfo = Split(rundoscomandt("adb -s " + sn + " shell ps sh"), vbCrLf)
        'Console.WriteLine(psinfo.Length.ToString)
        'psinfo(0) = "adb  shell ps sh "
        'psinfo(1) = "USER     PID   PPID  VSIZE  RSS     WCHAN    PC         NAME"
        'psinfo(2) = "shell     5149  95    776    384   c003f458 40047f94 S sh"
        'psinfo(3) = "shell     5150  95    776    384   c003f458 40047f94 S /system/bin/sh"
        For Each Str As String In psinfo

            If Str.IndexOf(killmark) >= 0 Then
                Str = Replace(Replace(Replace(Replace(Str, "    ", " "), "   ", " "), "  ", " "), vbCr, "")
                'Console.WriteLine(Str)
                If Split(Str, " ").Length > 7 Then
                    'Console.WriteLine(Trim(Split(Str, " ")(8)).Length)
                    If Trim(Split(Str, " ")(8)) = killtitle Then
                        'For Each substr As String In Split(Str, " ")
                        '    Console.WriteLine(Trim((substr)))

                        'Next

                        pid = Trim(Split(Str, " ")(1))
                        'Console.WriteLine("kill old process pid:" + pid)
                        'rundoscomandt("adb -s " + sn + " shell kill -s 9 " + pid)
                    End If
                End If

            End If


        Next
        Return pid
    End Function
    Sub killoldsh(ByVal sn As String, ByVal killtitle As String, ByVal killmark As String)
        Dim psinfo() As String
        Dim pid As String
        'Dim killtitle As String = "sh"
        'Dim killmark As String = "shell"
        psinfo = Split(rundoscomandt("adb -s " + sn + " shell ps sh"), vbCrLf)
        'Console.WriteLine(psinfo.Length.ToString)
        'psinfo(0) = "adb  shell ps sh "
        'psinfo(1) = "USER     PID   PPID  VSIZE  RSS     WCHAN    PC         NAME"
        'psinfo(2) = "shell     5149  95    776    384   c003f458 40047f94 S sh"
        'psinfo(3) = "shell     5150  95    776    384   c003f458 40047f94 S /system/bin/sh"
        For Each Str As String In psinfo

            If Str.IndexOf(killmark) >= 0 Then
                Str = Replace(Replace(Replace(Replace(Str, "    ", " "), "   ", " "), "  ", " "), vbCr, "")
                'Console.WriteLine(Str)
                If Split(Str, " ").Length > 7 Then
                    'Console.WriteLine(Trim(Split(Str, " ")(8)).Length)
                    If Trim(Split(Str, " ")(8)) = killtitle Then
                        'For Each substr As String In Split(Str, " ")
                        '    Console.WriteLine(Trim((substr)))

                        'Next

                        pid = Trim(Split(Str, " ")(1))
                        Console.WriteLine("kill old process pid:" + pid)
                        rundoscomandt("adb -s " + sn + " shell kill -s 9 " + pid)
                    End If
                End If

            End If


        Next

    End Sub

    Sub runcommerialadroidcall(ByVal sn As String, ByVal phonenum As String, ByVal cduration As String, ByVal interval As String)
        'Dim phonenum As String = "2222"
        'Dim cduration As String = "10"
        'Dim sn As String = "A710e"
        Dim waittime As String = ""
        waittime = (Val(interval) - Val(cduration)).ToString
        killoldsh(sn, "sh", "shell")
        Console.WriteLine(rundoscomandt("echo " + phonenum + ">phonenumber"))
        'Console.ReadLine()
        Console.WriteLine(rundoscomandt("echo " + cduration + ">calllong"))
        Console.WriteLine(rundoscomandt("echo " + cduration + ">callwait"))
        'Console.ReadLine()
        Console.WriteLine(rundoscomandt("adb -s " + sn + " push run /data/local/tmp/stop.txt"))
        'Console.ReadLine()
        Console.WriteLine(rundoscomandt("adb -s " + sn + " push Call.sh /data/local/tmp/Call.sh"))
        'Console.ReadLine()
        Console.WriteLine(rundoscomandt("adb -s " + sn + " push calllong /data/local/tmp/calllong.txt"))
        Console.WriteLine(rundoscomandt("adb -s " + sn + " push callwait /data/local/tmp/callwait.txt"))
        'Console.ReadLine()
        Console.WriteLine(rundoscomandt("adb -s " + sn + " push phonenumber /data/local/tmp/phonenumber.txt"))
        'Console.ReadLine()
        Console.WriteLine(rundoscomandt("adb -s " + sn + " push start.sh /data/local/tmp/start.sh"))
        Console.WriteLine(rundoscomandt("d:\mueauto\autocall\adb.exe| -s " + sn + " shell" + "|" + "sh /data/local/tmp/Call.sh &|" + Chr(3), 4, False))

        Console.ReadLine()
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
            'If firstime = True Then End
            myprocess.killprocessbyimportfile("d:\mueauto\killuesoft.cfg")
            displaylog("wait 180s, auto restart program ", "g")
            'Timer4.Enabled = False
            'Mytcpclient.Close()
            'wait(60)
            'Timer2.Enabled = False
            'Try
            '    Me.Form1_FormClosing(Nothing, Nothing)
            '    Me.Form1_FormClosed(Nothing, Nothing)
            '    Dim myprocess As Process = New Process()

            '    myprocess.StartInfo.FileName = "d:\mueauto\mueclient.exe"
            '    Dim commandline As String()
            '    commandline = System.Environment.GetCommandLineArgs()
            '    Dim commandlinestr As String = ""
            '    For i = 1 To UBound(commandline)
            '        commandlinestr = commandlinestr + commandline(i) + " "
            '    Next
            '    myprocess.StartInfo.Arguments = commandlinestr

            '    myprocess.Start()
            '    Process.GetCurrentProcess().Kill()

            'Catch ex As Exception

            'End Try
            Timer4.Enabled = False
            Timer2.Enabled = False
            appstate = "resetting"
            wait(180)
            myprocess.killwindowbytitle(serialportname.ToUpper + ".")
            If traffictype = "ftpdl" Or traffictype = "ftpul" Or traffictype = "ftpdlul" Then killoldftp()
            If traffictype = "ping" Then killoldping()
            If traffictype = "video" Then killvideo()
            If traffictype = "volte" Then killoldvolte()
            If action = "V" Then killoldvolte()
            If traffictype = "http" Then killoldhttp()
            If traffictype = "httpdownload" Then killoldhttpdownload()
            Application.Restart()
        End Try



    End Function

    Private Sub serialSendData(ByVal command As String, ByVal logfilename As String)
        Dim hexsendFlag As Boolean
        If appstate <> "resetting" Then
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
                        If UEtype = "SMG9350" Then
                            For Each STR As Char In outDataBuf
                                SerialPort.Write(STR)
                                ' Module1.writelog(outDataBuf, 0, logfilename)


                                'BarCountTx.Text = Val(BarCountTx.Text) + outDataBuf.Length '发送字节计数
                            Next
                        Else
                            'SerialPort.WriteTimeout
                            SerialPort.WriteLine(outDataBuf)

                        End If

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
                appstate = "resetting"
                displaylog("wait 60s, auto restart program ", "g")
                wait(60)
                myprocess.killprocessbyimportfile("d:\mueauto\killuesoft.cfg")
                If action <> "c" Then
                    Application.Restart()
                End If


            End Try
        End If
    End Sub

    Private Sub SerialPort_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort.DataReceived
        Dim HexRecieveFlag As Boolean
        HexRecieveFlag = False
        Try
            If appstate <> "resetting" Then
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

                    If SerialPort.IsOpen = True Then

                        str = SerialPort.ReadExisting '读取全部可用字符串

                        If exitwindow = False And (Not (str.IndexOf("DSFLOWRPT") >= 0)) Then

                            str = findip(str)


                            'Invoke(RecieveRefresh, str)
                            showRecievedata(str)
                        End If

                        ' BarCountRx.Text = (Val(BarCountRx.Text) + str.Length).ToString '接收字节计数

                    End If
                End If
            End If
        Catch ex As Exception
            Console.WriteLine(ex.Message.ToString)
        End Try

    End Sub
    Function findip(ByVal inputstr As String) As String
        Dim regexstr As String
        regexstr = "((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))"
        Dim regex As Regex = New Regex(regexstr)
        If regex.IsMatch(inputstr) Then
            ueinternalip = regex.Match(inputstr, regexstr).Groups(0).Value
            Return inputstr + vbCrLf + "UE IP:" + regex.Match(inputstr, regexstr).Groups(0).Value
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
        If Trim(str) <> "" Then
            displaylog2(str, "g")
        End If


    End Sub


    Sub attachdetach(ByVal uetype As String, ByVal attachtime As Integer)
        Dim continoustime As Integer = 0

        displaylog("attach detach run", "r")
        'Shell("start d:\mueauto\killuesoft.bat")
        '-------------- UE detach+attach-----------------
        'Select Case uetype
        '    Case "H"
        '        attachdetach_HISI()
        '    Case "E5776"
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

                Case "E5776"
                    If attachdetach_HISI() <> "OK" Then
                        attachstate = False
                    End If

                Case "Qualcomm9600", "YY9027", "Qualcomm9206", "Qualcomm9027", "YY9206"
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
            Dim h = 0

            Do While (h < 3) And ueinternalip = ""
                serialSendData("AT+CGCONTRDP" & vbCrLf, logname)
                displaylog("AT+CGCONTRDP", "g")
                wait(10)
                displaylog("UErealip:=" + ueinternalip, "r")

                h = h + 1
            Loop
            trytimes = trytimes + 1
            continoustime = continoustime + 1
            displaylog("attach try times:" + trytimes.ToString, "r")
            If addroute() = "ip not find" Then
                If ueinternalip <> "" And (uerealtype = "Sierra") Then
                    displaylog("UE attach success, but it can not assign IP to PC. For Sierra chip, try disable/enable netcard", "r")
                    disenablenetcard()

                End If
                attachstate = attachstate * False
            End If
            If (uetype = "H" Or uetype = "E5776") And attachstate = True Then
                If monitoring(True) <> "OK" Then
                    attachstate = False
                End If
            End If
            attachstate = attachstate * Not (exitwindow)
            If attachstate = False Then
                displaylog("attach fail", "r")
            End If
            If attachstate = False Then
                Randomize()
                wait(30 + Int(Rnd() * (30)))

            End If

            If continoustime > 10 Then
                displaylog("attach try over 10 times, all failed, stop attach, if Qualcomm chip will  reset 10 minutes later,and  will go on attach", "r")
                wait(600)
                reset_qualcomm()
                wait(180)
                killtraffic()
                'myprocess.killwindowbytitle(serialportname.ToUpper + ".")
                'If traffictype = "ftpdl" Or traffictype = "ftpul" Or traffictype = "ftpdlul" Then killoldftp()
                'If traffictype = "ping" Then killoldping()
                'If traffictype = "video" Then killvideo()
                'If traffictype = "volte" Then killoldvolte()
                'If traffictype = "http" Then killoldhttp()
                'If traffictype = "httpdownload" Then killoldhttpdownload()
                'exitwindow = True
                'wait(60)
                Application.Restart()
            End If

        Loop Until attachstate = True

        continoustime = 0
        runtraffic()

        'If traffictype = "ftpdl" Or traffictype = "ftpul" Or traffictype = "ftpdlul" Then
        '    '-----------run ftp
        '    wait(5)
        '    If rundlftp() = "no ip no traffic" Then
        '        wait(10)
        '        If rundlftp() = "no ip no traffic" Then
        '            ' wait(attachtime)
        '            realtimes = realtimes + 1
        '            trytimes = trytimes + 1
        '            displaylog(" realtimes=" + realtimes.ToString + " trytimes=" + trytimes.ToString + " successtimes=" + successtimes.ToString + " successrate=" + (Int(successtimes / trytimes * 100)).ToString + "%", "r")

        '            Return
        '        End If
        '    End If

        'End If
        'If traffictype = "http" Then
        '    '-----------run http
        '    wait(5)
        '    If rundlhttp() = "no ip no traffic" Then
        '        wait(10)
        '        If rundlhttp() = "no ip no traffic" Then
        '            'wait(attachtime)
        '            realtimes = realtimes + 1
        '            'trytimes = trytimes + 1
        '            displaylog(" realtimes=" + realtimes.ToString + " trytimes=" + trytimes.ToString + " successtimes=" + successtimes.ToString + " successrate=" + (Int(successtimes / trytimes * 100)).ToString + "%", "r")

        '            Return
        '        End If
        '    End If
        'End If
        'If traffictype = "httpdownload" Then
        '    '-----------run httpdownload
        '    wait(5)
        '    If rundlhttpdownload() = "no ip no traffic" Then
        '        wait(10)
        '        If rundlhttpdownload() = "no ip no traffic" Then
        '            'wait(attachtime)
        '            realtimes = realtimes + 1
        '            'trytimes = trytimes + 1
        '            displaylog(" realtimes=" + realtimes.ToString + " trytimes=" + trytimes.ToString + " successtimes=" + successtimes.ToString + " successrate=" + (Int(successtimes / trytimes * 100)).ToString + "%", "r")

        '            Return
        '        End If
        '    End If

        'End If
        'If traffictype = "ping" Then
        '    '-----------run ping
        '    wait(5)
        '    If runping() = "no ip no traffic" Then
        '        wait(10)
        '        If runping() = "no ip no traffic" Then
        '            wait(attachtime)
        '            realtimes = realtimes + 1
        '            ' trytimes = trytimes + 1
        '            displaylog(" realtimes=" + realtimes.ToString + " trytimes=" + trytimes.ToString + " successtimes=" + successtimes.ToString + " successrate=" + (Int(successtimes / trytimes * 100)).ToString + "%", "r")

        '            Return
        '        End If
        '    End If
        'End If
        'If traffictype = "video" Then
        '    '-----------run video
        '    wait(5)
        '    If runvideo() = "no ip no traffic" Then
        '        wait(10)
        '        If runvideo() = "no ip no traffic" Then
        '            wait(attachtime)
        '            realtimes = realtimes + 1
        '            'trytimes = trytimes + 1
        '            displaylog(" realtimes=" + realtimes.ToString + " trytimes=" + trytimes.ToString + " successtimes=" + successtimes.ToString + " successrate=" + (Int(successtimes / trytimes * 100)).ToString + "%", "r")

        '            Return
        '        End If
        '    End If

        'End If

        'If traffictype = "volte" Then
        '    '-----------run volte
        '    wait(5)
        '    If runvolte() = "no ip no traffic" Then
        '        wait(10)

        '        If runvolte() = "no ip no traffic" Then
        '            wait(attachtime)
        '            realtimes = realtimes + 1
        '            'trytimes = trytimes + 1
        '            displaylog(" realtimes=" + realtimes.ToString + " trytimes=" + trytimes.ToString + " successtimes=" + successtimes.ToString + " successrate=" + (Int(successtimes / trytimes * 100)).ToString + "%", "r")

        '            Return
        '        End If
        '    End If

        'End If
        '-----------wait
        displaylog("start wait:" + attachtime.ToString + "s", "g")

        wait(attachtime)
        ''-----------monitoring call drop
        'calldroped = False
        'monitoring()
        ''-----------caculate the counter
        realtimes = realtimes + 1
        'If calldroped = False Then
        '    'trytimes = trytimes + 1
        successtimes = successtimes + 1

        'End If
        'If calldroped = True And trytimes <> 0 Then
        '    'trytimes = trytimes + 1
        'End If
        If trytimes > 0 Then
            displaylog(" realtimes=" + realtimes.ToString + " trytimes=" + trytimes.ToString + " successtimes=" + successtimes.ToString + " successrate=" + (Int(successtimes / trytimes * 100)).ToString + "%", "r")
            wait(2)
        End If
        killtraffic()
        'myprocess.killwindowbytitle(serialportname.ToUpper + ".")
        'If traffictype = "ftpdl" Or traffictype = "ftpul" Or traffictype = "ftpdlul" Then killoldftp()
        'If traffictype = "ping" Then killoldping()
        'If traffictype = "video" Then killvideo()
        'If traffictype = "volte" Then killoldvolte()
        'If traffictype = "http" Then killoldhttp()
        'If traffictype = "httpdownload" Then killoldhttpdownload()
    End Sub
    Sub disenablenetcard()
        Dim str As String
        str = "netsh.exe interface set interface """ + serialportname.ToUpper + """ disabled"
        rundoscommand(Str)
        displaylog(Str, "g", False)
        wait(8)
        str = "netsh.exe interface set interface """ + serialportname.ToUpper + """ enabled "
        rundoscommand(str)
        displaylog(str, "g", False)

    End Sub
    Sub attachtoin()
        Dim attachstate As Boolean = True
        Dim continoustime As Integer
        UErealip = ""
        Do 'UE attach until get add route ok
            attachstate = True
            '-------------- UE detach+attach-----------------
            Select Case UEtype
                Case "H"
                    If attachdetach_HISI() <> "OK" Then
                        attachstate = False
                    End If

                Case "E5776"
                    If attachdetach_HISI() <> "OK" Then
                        attachstate = False
                    End If

                Case "Qualcomm9600", "YY9027", "Qualcomm9206", "Qualcomm9027", "YY9206"
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
            continoustime = continoustime + 1
            displaylog("attach try times:" + trytimes.ToString, "r")
            Dim h = 0
            Do While (h < 3) And ueinternalip = ""
                serialSendData("AT+CGCONTRDP" & vbCrLf, logname)
                displaylog("AT+CGCONTRDP", "g")
                wait(10)
                displaylog("UErealip:=" + ueinternalip, "r")

                h = h + 1
            Loop
            If addroute() = "ip not find" Then
                If ueinternalip <> "" And (uerealtype = "Sierra") Then
                    displaylog("UE attach success, but it can not assign IP to PC. For Sierra chip, try disable/enable netcard", "r")
                    disenablenetcard()

                End If
                attachstate = attachstate * False
            End If
            If (UEtype = "H" Or UEtype = "E5776") And attachstate = True Then
                If monitoring(True) <> "OK" Then
                    attachstate = False
                End If
            End If
            attachstate = attachstate * Not (exitwindow)
            If attachstate = False Then
                displaylog("attach fail", "r")
            End If
            If attachstate = False Then
                Randomize()
                wait(30 + Int(Rnd() * (30)))

            End If

            If continoustime > 10 Then
                displaylog("attach try over 10 times, all failed, stop attach, if Qualcomm chip will  reset 10 minutes later,and  will go on attach", "r")
                wait(600)
                reset_qualcomm()
                wait(180)
                killtraffic()
                Application.Restart()
            End If

        Loop Until attachstate = True
        realtimes = realtimes + 1
    End Sub
    Sub attachideltoactive(ByVal uetype As String, ByVal attachtime As Integer, ByVal ideltime As Integer)
        Dim continoustime As Integer = 0
        displaylog("attach idel to active run", "r")
        attachtoin()
        continoustime = 0
        Do While True = True
            goactive(uetype)
            runtraffic()
            '-----------wait
            displaylog("start wait:" + attachtime.ToString + "s", "g")
            wait(attachtime)
            ''-----------monitoring call drop
            'calldroped = False
            'monitoring()
            If trytimes > 0 Then
                displaylog(" realtimes=" + realtimes.ToString + " trytimes=" + trytimes.ToString + " successtimes=" + realtimes.ToString + " successrate=" + (Int(realtimes / trytimes * 100)).ToString + "%", "r")
                wait(2)
            End If
            myprocess.killwindowbytitle(serialportname.ToUpper + ".")
            If traffictype = "ftpdl" Or traffictype = "ftpul" Or traffictype = "ftpdlul" Then killoldftp()
            If traffictype = "ping" Then killoldping()
            If traffictype = "video" Then killvideo()
            If traffictype = "volte" Then killoldvolte()
            If traffictype = "http" Then killoldhttp()
            If traffictype = "httpdownload" Then killoldhttpdownload()
            goidel(uetype)
            displaylog("start idle:" + ideltime.ToString + "s", "g")
            wait(ideltime)
        Loop

    End Sub

    Sub goidel(ByVal uetype As String)
        displaylog("Go idle", "g")
        Select Case uetype
            Case "H"

            Case "E5776"


            Case "Qualcomm9600"
                'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
                idel_Qualcomm("9600")

            Case "Qualcomm9028"
                'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
                idel_Qualcomm("9028")
            Case "BandluxeC508"
                ' idel_Bandrich()

            Case "ALT-C186"


            Case "dialcomm"
                'idel_comm()
        End Select
    End Sub

    Sub goactive(ByVal uetype As String)
        displaylog("Go active", "g")
        Select Case uetype
            Case "H"

            Case "E5776"


            Case "Qualcomm9600"
                'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
                active_Qualcomm("9600")

            Case "Qualcomm9028"
                'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
                active_Qualcomm("9028")
            Case "BandluxeC508"
                ' idel_Bandrich()

            Case "ALT-C186"


            Case "dialcomm"
                'idel_comm()
        End Select
        While addroute() = "ip not find"
            attachtoin()
        End While

    End Sub
    Function idel_Qualcomm(ByVal type As String)
        If type = "9028" Then


        Else
            If traffictype <> "MOC" And traffictype <> "MTC" Then
                wwanundial()
            End If

        End If
        'If type = "Qualcomm9206" Then
        ' wwanundial()
        'Else
        serialSendData("AT$QCRMCALL=0,1,1,2,1" & vbCrLf, logname)
        displaylog(" AT^NDISCONN=0,1", "g")
        'End If

        Timer4.Enabled = False
        Return "OK"
    End Function
    Function active_Qualcomm(ByVal type As String)
        If type = "9028" Then


        Else
            If traffictype <> "MOC" And traffictype <> "MTC" Then
                wwandial()
            End If

        End If
        'If type = "Qualcomm9206" Then
        ' wwandial()
        'Else
        serialSendData("AT$QCRMCALL=1,1,1,2,1" & vbCrLf, logname)
        displaylog(" AT^NDISCONN=1,1", "g")
        'End If


        Timer4.Enabled = True
        Return "OK"
    End Function
    Function killpagingping()
        Dim count As Integer
        Dim killid As Integer
        displaylog("kill old pagings", "g")
        'Shell("taskkill /F /IM ftp" & UEip & ".exe")
        'Shell("taskkill /F /IM shortping.exe")
        'myprocess.killprocess("ftp" & UEip)
        myprocess.killprocess("shortping")
        count = 1
        For Each killid In pagingsessionlist
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
        Next
        Return 1
    End Function
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
        Try
            serialSendData("AT+CGATT=0" & vbCrLf, logname)
            displaylog("AT+CGATT=0", "g")
            wait(5)
            serialSendData("AT+CFUN=1,1" & vbCrLf, logname)
            displaylog("AT+CFUN=1,1", "g")
            wait(1)
            reset_qualcomm = "OK"
            appstate = "resetting"
            ' SerialPort.Close()
        Catch ex As Exception
            Dim a = 1
        End Try


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
            If UEtype = "E5776" Or "E5375" Then
                serialSendData("AT+CGATT=0" & vbCrLf, logname)
                displaylog("AT+CGATT=0", "g")
                wait(5)

                serialSendData("AT+CFUN=0" & vbCrLf, logname)
                displaylog("AT+CFUN=0", "g")
                wait(5)

                serialSendData("AT+CFUN=0" & vbCrLf, logname)
                displaylog("AT^RESET", "g")
                wait(5)

              

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
            If UEtype = "E5776" Or UEtype = "5375" Then
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
        serialSendData("AT+CHUP" & vbCrLf, logname)
        displaylog("AT+CHUP", "g")

        wait(5)

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
        Timer2.Enabled = False
        Timer4.Enabled = False
        'Shell("start d:\mueauto\killuesoft.bat")
        '-------------- UE detach+attach-----------------
        Select Case uetype
            Case "H"
                shutdown_HISI()
            Case "E5776"
                reset_HISI5776()

            Case "Qualcomm9600", "YY9027", "Qualcomm9206", "Qualcomm9027", "YY9206"
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
        'displaylog("reset UE done", "r")
        Application.Exit()

    End Sub

    Sub shutdown(ByVal uetype As String)
        Dim sn As String = ""
        displaylog("shutdown UE", "r")
        Timer2.Enabled = False
        Timer4.Enabled = False
        'Shell("start d:\mueauto\killuesoft.bat")
        '-------------- UE detach+attach-----------------
        If uetype = "SMG9350" Then
            sn = getadbdevice()
            If sn <> "" Then
                displaylog("stop old UE(" + sn + ") internal script session", "r")
                killoldsh(sn, "sh", "shell")
            End If

        End If
        Select Case uetype
            Case "H"
                shutdown_HISI()
            Case "E5776"
                shutdown_HISI()

            Case "Qualcomm9600", "YY9027", "Qualcomm9206", "Qualcomm9027", "SMG9350", "YY9206"
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
        'displaylog("shutdown UE successfully", "r")
        Application.Exit()

    End Sub
    Function getportinterface(ByVal portname) As String
        Dim returnstr As String
        Dim returnlines As Object
        returnstr = rundoscomandt("netsh int ipv4 show interfaces")
        returnlines = returnstr.Split(vbCrLf)
        Dim startline As Integer = 0
        Dim stopline As Integer = 0
        Dim tempportname As String = ""
        For i = 0 To UBound(returnlines) - 1
            If returnlines(i).indexof("---------------------------") >= 0 Then startline = i + 1
            If returnlines(i).indexof(" ") <= 0 And returnlines(i + 1).indexof(" ") <= 0 Then
                stopline = i - 1
            End If
        Next
        Dim portid As String = ""
        Dim templine As String = ""
        For s = startline To stopline
            templine = System.Text.RegularExpressions.Regex.Replace(returnlines(s), "\b\s+\b", "|")
            templine = templine.Replace(Chr(10), "")
            tempportname = Trim(templine.Split("|")(templine.Split("|").Count - 1))

            If tempportname = portname Then
                portid = Trim(templine.Split("|")(0))
                Return portid

            End If
        Next
        Return ""
    End Function

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
        If Split(ipaddress, ":")(0) = "169" Then
            displaylog("This is a private address,please check UE:" + ipaddress, "r")
            Return "ip not find"
            Exit Function
        End If
        displaylog("Clear old route to " & serverip, "r")
        returnstr = rundoscomandt("route delete " + serverip)
        wait(1)
        displaylog("UE IP:" & ipaddress & "  Gateway:" & gateway, "r")
        UErealip = ipaddress
        Dim interfaceid As String = ""
        interfaceid = getportinterface(SerialPort.PortName)
        displaylog("net port id:" + interfaceid, "r")
        displaylog("Route Add :" + serverip + " " + gateway + " IF " + interfaceid, "g")
        If interfaceid <> "" Then
            returnstr = rundoscomandt("route add " + serverip + " " + gateway + " IF " + interfaceid)
        Else
            returnstr = rundoscomandt("route add " + serverip + " " + gateway)
        End If

        'displaylog(returnstr, "g")
        If traffictype = "volte" Then
            If serverip2 <> "" Then returnstr = rundoscomandt("route add " + serverip2 + " " + gateway)
            If serverip3 <> "" Then returnstr = rundoscomandt("route add " + serverip3 + " " + gateway)
            If serverip3 <> "" Then returnstr = rundoscomandt("route add " + serverip4 + " " + gateway)
        End If
        Return "OK"

    End Function
    Function tlistsum(ByVal intlist As List(Of Integer)) As Int64
        tlistsum = 0
        For Each Inter As Int64 In intlist
            tlistsum = tlistsum + Inter
        Next

    End Function
    Function monitoring(Optional ByVal forattach As Boolean = False) As String
        Dim returnstr As String
        Dim values As String

        If tlistsum(DLtpcounter) = 0 Then

            values = ""
            displaylog("Checking traffic", "r")
            returnstr = rundoscomandt("ping -w 10000 -n " + "5" + " " + serverip)

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
        Else
            calldroped = False
            displaylog("Traffic OK", "g")

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
        returnstr = rundoscomandt("route print " + serverip)

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
            If UErealip <> "" Then


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
            If UErealip <> "" Then


                count = 1
                displaylog("start new ftp session", "g")

                Dim myprocess As Process = New Process()
                If traffictype = "ftpdl" Then
                    myprocess.StartInfo.FileName = "d:\mueauto\ftpclient.exe"
                    myprocess.StartInfo.Arguments = serverip + " " + username + " " + pass + " " + "DL" + " " + DLsessionno + " " + DLfilename + " " + serialportname + "."
                    displaylog("ftp parameter:" & serverip + " " + username + " " + pass + " " + "DL" + " " + DLsessionno + " " + DLfilename, "g")
                Else
                    If traffictype = "ftpul" Then
                        myprocess.StartInfo.FileName = "d:\mueauto\ftpclient.exe"
                        myprocess.StartInfo.Arguments = serverip + " " + username + " " + pass + " " + "UL" + " " + ULsessionno + " " + DLfilename + " " + serialportname + ". " + ULremotename + " " + ULfilename
                        displaylog("ftp parameter:" & serverip + " " + username + " " + pass + " " + "UL" + " " + ULsessionno + " " + DLfilename + " " + ULremotename + " " + ULfilename, "g")
                    End If
                    If traffictype = "ftpdlul" Then
                        myprocess.StartInfo.FileName = "d:\mueauto\ftpclient.exe"
                        myprocess.StartInfo.Arguments = serverip + " " + username + " " + pass + " " + "DL" + " " + DLsessionno + " " + DLfilename + " " + serialportname + "."
                        displaylog("ftp parameter:" & serverip + " " + username + " " + pass + " " + "DL" + " " + DLsessionno + " " + DLfilename, "g")
                    End If

                End If

                myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized
                myprocess.Start()
                ftphandle(0) = myprocess.Id
                displaylog("ftp session id opened:" & myprocess.Id.ToString, "g")

                If traffictype = "ftpdlul" Then
                    myprocess.StartInfo.FileName = "d:\mueauto\ftpclient.exe"
                    myprocess.StartInfo.Arguments = serverip + " " + username + " " + pass + " " + "UL" + " " + ULsessionno + " " + DLfilename + " " + serialportname + ". " + ULremotename + " " + ULfilename
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
            If UErealip <> "" Then


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

        If UErealip <> "" Then


            count = 1
            displaylog("start new http session", "g")


            Dim myprocess As Process = New Process()

            myprocess.StartInfo.FileName = "d:\mueauto\webdownload.exe"

            myprocess.StartInfo.Arguments = serverip + " " + ftpsessionnum + " " + serialportname + "."
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
                If UErealip <> "" Then


                    count = 1
                    displaylog("start new ping session", "g")
                    Dim intervalstring As String = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "ping", "interval")
                    Dim lenstring As String = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "ping", "len")

                    Do While count <= Int(1)
                        Dim myprocess As Process = New Process()
                        If traffictype = "ping" Or traffictype = "MOC" Or traffictype = "MTC" Then
                            ' myprocess.StartInfo.FileName = "d:\mueauto\ping.exe"
                            'myprocess.StartInfo.FileName = "d:\mueauto\hrping.exe"
                            myprocess.StartInfo.FileName = "d:\mueauto\myping.exe"
                            myprocess.StartInfo.WorkingDirectory = "d:\mueauto"
                            myprocess.StartInfo.Verb = "runas"

                            If intervalstring <> "" And lenstring <> "" Then
                                ' myprocess.StartInfo.Arguments = serverip + " -t -L " + lenstring + " -y 60 -s " + intervalstring  'wait change size,interval 10K/s 
                                myprocess.StartInfo.Arguments = "Ping:" + serialportname.ToUpper + ". " + serverip + " " + lenstring + " " + intervalstring
                            Else
                                myprocess.StartInfo.Arguments = "Ping:" + serialportname.ToUpper + ". " + serverip + " 32 1000"
                            End If


                            'myprocess.StartInfo.Arguments = serverip + " -t"
                            myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized

                            myprocess.Start()
                            pinghandle(count - 1) = myprocess.Id
                            displaylog("ping session id opened:" & myprocess.Id.ToString, "g")
                            wait(3)
                            'SetWindowTextA(myprocess.MainWindowHandle, "Ping:" + serialportname.ToUpper)
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
    Sub udpdl(ByVal command)
        Dim hartbeatinterval As String = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "udp", "hartbeat")
        If hartbeatinterval = "" Then hartbeatinterval = "60"
        Dim localIpep As New IPEndPoint(IPAddress.Parse(UErealip), 33893) ' // 本机IP，指定的端口号  
        Dim udpcSend = New Net.Sockets.UdpClient(localIpep)
        Dim message As String = UErealip + ":33893" + "|c|" + command
        Dim sendbytes As Byte() = Encoding.ASCII.GetBytes(message)
        Dim oldip As String = UErealip
        Dim remoteIpep As New IPEndPoint(IPAddress.Parse(serverip), 33892) ' // 发送到的IP地址和端口号  

        udpcSend.Send(sendbytes, sendbytes.Length, remoteIpep)
        Do While exitwindow = False
            Try
                Thread.Sleep(Val(hartbeatinterval) * 1000)
                findueip()
                message = UErealip + ":33893" + "|h"
                If oldip <> UErealip Then
                    message = UErealip + ":33893" + "|c|" + command
                    displaylog("ip address changed from " & oldip & " to " & UErealip & ",send new udp command", "r")
                    oldip = UErealip
                    localIpep = New IPEndPoint(IPAddress.Parse(UErealip), 33893)
                    udpcSend = New Net.Sockets.UdpClient(localIpep)
                    remoteIpep = New IPEndPoint(IPAddress.Parse(serverip), 33892)
                End If
                sendbytes = Encoding.ASCII.GetBytes(message)
                udpcSend.Send(sendbytes, sendbytes.Length, remoteIpep)
            Catch ex As Exception
                displaylog(ex.ToString, "r")
            End Try

        Loop



    End Sub
    Function runudpdl()
        'hartbeat and sent command thread
        Dim thr As New Thread(AddressOf udpdl)
        thr.Name = "udpdl"
        thr.IsBackground = True
        thr.Start(udpdlstr)
        'run local miperf
        'Dim myprocess As Process = New Process()
        'myprocess.StartInfo.FileName = "d:\mueauto\myiperfr.exe"
        'myprocess.StartInfo.WorkingDirectory = "d:\mueauto"
        'myprocess.StartInfo.Verb = "runas"

        'myprocess.StartInfo.Arguments = serialportname  '"-s -fk -u -i10 -p33892"
        'myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized
        'myprocess.Start()
        'udpdlhandle = myprocess.Id
        'displaylog("iperf receive session id opened:" & myprocess.Id.ToString, "g")



    End Function
    Function runvideo() As String
        Dim count As Integer
        runvideo = "OK"
        If Int(ftpsessionnum) > 0 Then
            If UErealip <> "" Then


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
        'Dim servers, tempstr As String
        'getcallnum = ""
        'If imsi <> "" Then
        '    getcallnum = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "volterealtarget", imsi)
        'Else
        '    displaylog("Not find imsi number", "r")
        'End If
        If targetphonenumber <> "" Then
            Return targetphonenumber
        Else
            displaylog("targetphonenumber is empty", "r")
        End If
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

            If UErealip <> "" Then

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
                myprocess.StartInfo.FileName = "d:\mueauto\myping.exe"
                myprocess.StartInfo.WorkingDirectory = "d:\mueauto"
                myprocess.StartInfo.Arguments = "Ping:" + serialportname.ToUpper + ". " + serverip2 + " 500 500" 'wait change size,interval 10K/s
                myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized
                myprocess.Start()
                voltehandle(count - 1) = myprocess.Id
                displaylog("sip hrping sim session id opened:" & myprocess.Id.ToString, "g")
                ' SetWindowTextA(myprocess.MainWindowHandle, "Ping:" + serialportname.ToUpper)
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
                myprocess.StartInfo.WorkingDirectory = "d:\mueauto"
                myprocess.StartInfo.Arguments = "Ping:" + serialportname.ToUpper + ". " + serverip4 + " 500 5" 'wait chage size, interval
                myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized
                myprocess.Start()
                voltehandle(count - 1) = myprocess.Id
                displaylog("video traffic ftp sim session id opened:" & myprocess.Id.ToString, "g")
                'SetWindowTextA(myprocess.MainWindowHandle, "Ping:" + serialportname.ToUpper)
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
    Function killoldudpdl() As Integer
        ' Dim count As Integer
        Dim killid As Integer
        'Dim ftpsessionnumreal As Integer
        killid = udpdlhandle
        Dim pProcessTemp As System.Diagnostics.Process = Process.GetProcessById(killid)
        pProcessTemp = Process.GetProcessById(killid)
        displaylog("kill process id:" & killid & "  process name:" & pProcessTemp.ProcessName, "g")
        pProcessTemp.Kill()
        pProcessTemp.Close()
    End Function

    Function killoldudpul() As Integer
        Dim killid As Integer
        'killid = udpulhandle
        Try
            Dim pProcessTemp As System.Diagnostics.Process = Process.GetProcessById(killid)
            pProcessTemp = Process.GetProcessById(killid)
            displaylog("kill process id:" & killid & "  process name:" & pProcessTemp.ProcessName, "g")
            pProcessTemp.Kill()
            pProcessTemp.Close()
        Catch






        End Try
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

        If UErealip <> "" Then


            count = 1
            displaylog("start new http session", "g")


            Dim myprocess As Process = New Process()
            'If traffictype = "http" Then
            myprocess.StartInfo.FileName = "d:\mueauto\webbrowser.exe"
            'End If
            myprocess.StartInfo.Arguments = serverip + " " + ftpsessionnum + " " + serialportname + "."
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

        If UErealip <> "" Then
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

    Function runrealvolteadbhungup()
        displaylog("Huangup the call ", "g")
        If adbdvicesn <> "" Then
            rundoscomandt("d:\mueauto\adb.exe" + " -s " + adbdvicesn + " shell input keyevent 6") '
        Else
            rundoscomandt("d:\mueauto\adb.exe" + " -s 123456 shell input keyevent 6")
        End If
    End Function

    Function runrealvolteadb(ByVal phonenum) As String

        Dim count As Integer
        runrealvolteadb = "OK"


        If imsi <> "" Then
            displaylog("imsi:" + imsi, "g")

            displaylog("andriod sn:" + adbdvicesn, "g")
            If adbdvicesn <> "" Then
                count = 1
                displaylog("start volte call " + phonenum, "g")
                rundoscomandt("d:\mueauto\adb.exe" + " -s " + adbdvicesn + " shell am start -a android.intent.action.CALL tel:" + phonenum)

                'Dim myprocess As Process = New Process()
                'If traffictype = "http" Then
                '    myprocess.StartInfo.FileName = "d:\mueauto\adb.exe"
                'End If
                'myprocess.StartInfo.Arguments = " -s " + androiddevid + " shell am start -a Android.intent.action.CALL tel:" + phonenum
                'myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized

                'myprocess.Start()
                'volteadbahhandle = myprocess.Id
                'displaylog("volte adb id opened:" & myprocess.Id.ToString, "g")
            Else
                displaylog("not find ADB device sn", "r")
                Return "no ip no traffic"
            End If

        End If



    End Function
    Function getadbdevice()
        Dim devicesroot As String = ""
        Dim devicecontainid As String = ""
        Dim deviceserial As String = ""
        Dim FriendlyName As String = ""
        regsearch(0) = Nothing
        regsearch(1) = Nothing
        regsearch(2) = Nothing
        regsearch(3) = Nothing
        SearchSubKeys(My.Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\ControlSet001\Enum\USB"), serialportname.ToUpper)
        If regsearch(3) <> Nothing Then
            devicesroot = Mid(regsearch(1), 1, regsearch(1).LastIndexOf("\"))
            devicecontainid = My.Computer.Registry.GetValue(devicesroot, "ContainerID", String.Empty)
            FriendlyName = My.Computer.Registry.GetValue(devicesroot, "FriendlyName", String.Empty)
            If FriendlyName = "SAMSUNG Mobile USB Modem" Then
                Dim newroot As String = VB.Left(devicesroot, devicesroot.LastIndexOf("\"))
                newroot = VB.Left(newroot, newroot.LastIndexOf("&"))
                newroot = VB.Mid(newroot, newroot.IndexOf("SYSTEM") + 1)
                If devicecontainid <> "" Then
                    regsearch(0) = Nothing
                    regsearch(1) = Nothing
                    regsearch(2) = Nothing
                    regsearch(3) = Nothing
                    SearchSubKeys(My.Computer.Registry.LocalMachine.OpenSubKey(newroot), devicecontainid)
                    If regsearch(1) <> Nothing Then

                        deviceserial = VB.Mid(regsearch(1), regsearch(1).LastIndexOf("\") + 2)
                        Return Trim(deviceserial)
                    Else
                        Return ""
                    End If

                Else
                    Return ""
                End If

            End If
            If devicecontainid <> "" Then
                regsearch(0) = Nothing
                regsearch(1) = Nothing
                regsearch(2) = Nothing
                regsearch(3) = Nothing
                SearchSubKeys(My.Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\ControlSet001\Enum\USBSTOR"), devicecontainid)
                If regsearch(3) <> Nothing Then
                    devicesroot = Mid(regsearch(1), regsearch(1).LastIndexOf("\") + 1, regsearch(1).Length)
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

        'Dim itm As ListViewItem

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



    End Function
    Function attachdetach_HISI() As String
        If SerialPortOpen(logname, False) Then
            UErealip = ""

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
            If UEtype = "E5776" Then
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

    Function attachdetach_Qualcomm_volte(ByVal type As Integer) As String
        UErealip = ""
        serialSendData("at+ci", logname)
        wait(1)
        serialSendData("mi" & Chr(13), logname)
        wait(1)
        serialSendData("AT+CFUN=1" & vbCrLf, logname)
        displaylog("AT+CFUN=1", "g")
        wait(15)
        If type = "9028" Then
            serialSendData("AT^ENLOG=1" & vbCrLf, logname)
            displaylog("AT^ENLOG=1", "g")
            wait(1)
            serialSendData("AT+CGATT=1" & vbCrLf, logname)
            displaylog("AT+CGATT=1", "g")
            wait(1)

        Else
            If traffictype <> "MOC" And traffictype <> "MTC" Then
                wwandial()
            End If

        End If
        serialSendData("AT$QCRMCALL=1,1,1,2,1" & vbCrLf, logname)
        displaylog(" AT^NDISCONN=1,1", "g")
        wait(3)
        serialSendData("AT+CGCONTRDP" & vbCrLf, logname)
        displaylog("AT+CGCONTRDP", "g")
        wait(1)
        serialSendData("at+CIMI", logname)
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


    Function attachdetach_Qualcomm(ByVal type As Integer) As String
        UErealip = ""
        ueinternalip = ""
        serialSendData("AT+CHUP" & vbCrLf, logname)
        displaylog("AT+CHUP", "g")

        wait(2)
        serialSendData("AT+CGATT=0" & vbCrLf, logname)
        displaylog("AT+CGATT=0", "g")

        wait(5)

        serialSendData("AT+CFUN=0" & vbCrLf, logname)
        displaylog("AT+CFUN=0", "g")
        wait(15)
        If uerealtype = "Sierra" Then
            serialSendData("AT!GSTATUS?" & vbCrLf, logname)
            serialSendData("AT+CIMI" & vbCrLf, logname)
            wait(2)
        End If
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
            If traffictype <> "MOC" And traffictype <> "MTC" Then
                ' wwandial()
            End If

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
        'Dim i, pp, pp2, pp3, pp4, j, totallines As Integer
        Dim Buff As String ', yvalue, pointtime
        ' Dim searchedstr As String
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
    Function wwandial(Optional ByVal force As Boolean = False)
        Dim str As String
        Dim temstringarr As String()
        Dim profilename As String
        Dim i As Integer
        'str = rundoscomandt("netsh mbn show profiles")
        ''str = readfile("E:\test.txt")
        'displaylog(str, "g", False)
        'temstringarr = Split(str, vbCrLf)
        'For i = 0 To temstringarr.Length - 1
        '    If temstringarr(i).IndexOf(serialportname.ToUpper) >= 0 Then Exit For


        'Next
        'If i < temstringarr.Length Then
        '    profilename = Trim(temstringarr(i + 2))
        '    If profilename.IndexOf("<") < 0 Then
        '        ' str = "netsh mbn connect """ + serialportname.ToUpper + """ name """ + profilename + """"
        If disablenetcard = "1" Or force = True Then

            str = "netsh.exe interface set interface """ + serialportname.ToUpper + """ enabled "
            rundoscommand(str)
            displaylog(str, "g", False)

            wait(8)

        End If

        '    Else
        'displaylog("wwan " + serialportname.ToUpper + "port profile not find", "r")
        '    End If

        'Else
        'displaylog("Not find wwanport", "g")
        'End If


    End Function

    Function wwanundial()
        Dim str As String
        Dim temstringarr As String()
        Dim profilename As String
        Dim i As Integer
        str = rundoscomandt("netsh mbn show profiles")
        'str = readfile("E:\test.txt")
        displaylog(str, "g", False)
        temstringarr = Split(str, vbCrLf)
        For i = 0 To temstringarr.Length - 1
            If temstringarr(i).IndexOf(serialportname.ToUpper) >= 0 Then Exit For


        Next
        If i < temstringarr.Length Then
            profilename = Trim(temstringarr(i + 2))
            If profilename.IndexOf("<") < 0 Then
                'str = "netsh mbn disconnect """ + serialportname.ToUpper + """ name """ + profilename + """"
                If disablenetcard = "1" Then
                    str = "netsh interface set interface """ + serialportname.ToUpper + """ disabled"
                    rundoscommand(str)
                End If
                displaylog(str, "g", False)

                ' wait(8)
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
            Application.DoEvents()
            Dim properties As IPInterfaceProperties = adapter.GetIPProperties()
            If (adapter.Name.ToLower.ToString.Contains(comname.ToLower)) And (Trim(adapter.Name.ToLower.ToString) = Trim(comname.ToLower)) Then
                displaylog(adapter.Description, "g", False)
                displaylog(adapter.Name, "g", False)
                displaylog("  operate state................................. :" & adapter.OperationalStatus.ToString, "g", False)
                If adapter.OperationalStatus.ToString <> "Up" Then
                    ipaddress = "Down"
                    Return
                End If
                Dim unicastipaddressinformationcollection As UnicastIPAddressInformationCollection = properties.UnicastAddresses
                Dim unicastip As UnicastIPAddressInformation
                For Each unicastip In unicastipaddressinformationcollection
                    Application.DoEvents()
                    displaylog("  ip address............:{0}" & unicastip.Address.ToString, "g", False)
                    displaylog("  " & unicastip.Address.AddressFamily.ToString, "g", False)
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
                    Application.DoEvents()
                    displaylog("  gateway address.................:" & gatewayip.Address.ToString, "g", False)
                    If isIPV4 And gatewayip.Address.AddressFamily.ToString = "InterNetwork" Then 'InterNetworkV6 means ipV6 address
                        gatewayipaddress = gatewayip.Address.ToString 'only one ip ge
                        If gatewayipaddress <> "" And ipaddress <> "" And action = "pa" Then
                            'i = 0
                            'While (s = "Connection fail") And i < 3
                            '    s = sendtcpcommand("|" + ipaddress + "|" + UEname + "-" + serialportname, serverip)
                            '    i = i + 1
                            'End While

                        End If
                        Return
                    ElseIf isIPV4 = False And gatewayip.Address.AddressFamily.ToString = "InterNetworkV6" Then
                        gatewayipaddress = gatewayip.Address.ToString 'only one ip get
                        If gatewayipaddress <> "" And ipaddress <> "" And action = "pa" Then
                            i = 0
                            'While (s = "Connection fail") And i < 3
                            '    s = sendtcpcommand("|" + ipaddress + "|" + UEname + "-" + serialportname, serverip)
                            '    i = i + 1
                            'End While
                        End If
                        Return
                    End If






                Next gatewayip
            End If
        Next adapter

    End Sub
    Protected Function GetLocalIPe() As String
        ' Dim addr As System.Net.IPAddress
        Dim ips As String = ""
        For i = 1 To System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()).Length
            ips = ips + "," + System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())(i - 1).ToString
        Next
        Return ips
    End Function

    Protected Function GetLocalIP() As String
        'Dim addr As System.Net.IPAddress
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
        dosstr = Split(rundoscomandt("tracert -w 100 -h 2 " + logip), vbCrLf)
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

            UErealip = ""

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
            UErealip = ""
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
    Public Sub wait(ByRef s As Short, Optional ByVal needreturn As Boolean = True)
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
                    Console.Write((s - DateDiff(DateInterval.Second, starttime, endtime)).ToString + "  ")
                    Console.CursorLeft = a
                End If
                endtime = DateTime.Now


            End While
            If needreturn = True Then
                Console.WriteLine(" ")
            End If

        Catch
        End Try

        If exitwindow = True Then End
    End Sub

    Private Sub runmain()
        Dim myArg() As String, iCount As Integer
        Dim netcardname As String
        Dim enabledebuginfostr As String
        Dim resettime As String
        'Timer1.Enabled = False

        tcperror = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "info", "tcperror")

        ueresetflag = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "reset", "reset")
        resettime = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "reset", "sierraresettime")
        disablenetcard = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "idelmode", "disablenetcard")
        enabledebuginfostr = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "info", "enabledebuginfo")
        If enabledebuginfostr = "1" Then showdebuginfo = True
        If tcperror <> "1" Then tcperror = "0"
        If ueresetflag <> "0" Then ueresetflag = "1"
        If resettime <> "" Then sierraresettime = Int(resettime)
        If disablenetcard <> "1" Then disablenetcard = "0"

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
                Case "-u"
                    udpulstr = myArg(iCount + 1)

                Case "-y"
                    udpdlstr = myArg(iCount + 1)

                Case "-h"
                    targetphonenumber = myArg(iCount + 1)

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
                        Case "script-internal"
                            traffictype = "script-internal"
                        Case "None"
                            traffictype = "None"
                        Case "udpdl"
                            traffictype = "udpdl"
                        Case "udpul"
                            traffictype = "udpul"
                    End Select
                Case "-t"
                    Select Case myArg(iCount + 1).ToString
                        Case "H", "E5776", "E5375"
                            UEtype = "E5776"
                        Case "Qualcomm9600"
                            UEtype = "Qualcomm9600"
                        Case "Qualcomm8998"
                            UEtype = "Qualcomm9600"
                        Case "YY9206"
                            UEtype = "YY9206"
                        Case "YY9027"
                            UEtype = "YY9027"
                        Case "Qualcomm8996"
                            UEtype = "Qualcomm9600"
                        Case "Qualcomm9206"
                            UEtype = "Qualcomm9206"
                        Case "Qualcomm9027"
                            UEtype = "Qualcomm9027"
                        Case "BandluxeC508"
                            UEtype = "BandluxeC508"
                        Case "ALT-C186"
                            UEtype = "ALT-C186"
                        Case "Androidinter"
                            UEtype = "Androidinter"
                        Case "Dialcommon"
                            UEtype = "dialcomm"
                        Case "SMG9350"
                            UEtype = "SMG9350"
                        Case "Andriod"
                            UEtype = "Andriod"

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
                Case "-A"
                    action = "A"
                Case "-c"
                    action = "c"
                Case "-e"
                    action = "e"
                Case "-pa"
                    action = "pa"
                Case "-V"
                    action = "V"
                Case "-VVC"
                    action = "VOLTEvoiceMOC"
                Case "-VVT"
                    action = "VOLTEvoiceMTC"
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





        displaylog("new start", "r")
        displaylog("UEtype=" + UEtype, "r")
        displaylog("serverip=" + serverip + "," + serverip2 + "," + serverip3 + "," + serverip4, "r")
        displaylog("serial port=" + serialportname, "r")
        myprocess.killwindowbytitle(serialportname.ToUpper + ".")
        myprocess.killwindowbytitle("MUEclient:" + serialportname.ToUpper + ".")
        Console.Title = "MUEclient:" + serialportname.ToUpper + "."
        accesstype = "net"
        netcardname = ""
        If UEtype <> "Andriod" Then
            If action <> "S" And UEtype <> "Androidinter" And UEtype <> "Andriod" Then
                wwandial(True)
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
            SerialPort.DataBits = 8
            SerialPort.WriteTimeout = 1000
            SerialPort.ReadTimeout = 1000 '
            logname = "\\" + logip + "\uelog\" + logname
            'TPlogname = "\\" + logip + "\uelog\" + TPlogname
            TPlogname = "d:\" + TPlogname
            displaylog("logseverip=" + logip, "r")
            'displaylog("logname=" + logname, "r")
            'displaylog("TPlogname=" + TPlogname, "r")lo 
            displaylog("TP report interval=" + TPinterval.ToString, "r")



            'If traffictype = "MTC" Or traffictype = "MOC" Then
            'adbdvicesn = getadbdevice()
            If action = "V" Or action = "VOLTEvoiceMOC" Then 'Or action = "VOLTEvoiceMTC" Then
                displaylog("Volte Call duration =" + a_interval + "S, hang up time is=" + (Val(interval) - Val(a_interval)).ToString + "S", "r")
                displaylog("Target phone number=" + targetphonenumber, "r")
                'displaylog("Http download interval=" + ftpsessionnum + "S", "r")
            End If
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
            If action = "A" Then
                displaylog("mode:Idle to Active loop", "r")
            End If


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
                    If traffictype = "script-internal" Then 'volte script internal traffic no netcard used
                    Else
                        startandroidnetlog()
                    End If

                End If


                Dim ran = New Random(DateTime.Now.Millisecond)
                If action <> "c" And action <> "e" Then
                    wait(ran.Next(0, 120))
                End If

                Try
                    SerialPortOpen(logname, True)
                    serialSendData("AT" & vbCrLf, logname)
                    displaylog("start OK", "g")
                    Console.Title = "MUEclient:" + serialportname.ToUpper + "." '+ logname + " COM:" + serialportname
                    ConsoleHelper.setconsoleminize()
                    Me.Text = "MUEclientform:" + serialportname
                    writeTPlog("", True, TPlogname, False)

                    '----------------------------main 
                Catch ex As Exception
                    displaylog(ex.ToString, "r")
                End Try

            End If


            runsingleaction()


            SerialPort.Close()
        Else
            runAndriod()
        End If
    End Sub

    Sub runAndriod()
        displaylog("UE type:" + UEtype + ", traffic is fixed in Android UE internal app tasker setting！", "r")
        Timer2.Enabled = True
        Timer3.Interval = 1033
        Timer3.Enabled = True
        Timer4.Interval = 10131
        Timer4.Enabled = True
        adbinputpoolsize = 10
        Dim returnstr As String
        returnstr = andriodcommand(serialportname, "IMSI")
        Select Case action

            Case "P"
                displaylog("mode: run UE", "r")
                andriodrunUE()
            Case "c"
                displaylog("mode: Stop UE", "r")
                andriodcommand(serialportname, "stop")
            Case "e"
                displaylog("mode: Reboot UE", "r")
                andriodcommand(serialportname, "reboot")


        End Select







        Application.Exit()
    End Sub
    Sub andriodrunUE()
        andriodcommand(serialportname, "run")
        While 1
            Try

                Application.DoEvents()
                Threading.Thread.Sleep(100)
                For Each keystr As String In returnstrdic.Keys
                    If returnstrdic(keystr).IndexOf("@") >= 0 Then
                        displaylog(Decoderandrodmessage(Split(returnstrdic(keystr), "@", 2)(0), Split(returnstrdic(keystr), "^", 2)(1)), "g")
                        returnstrdic.Remove(keystr)
                        displaylog(returnstrdic.Count, "g")
                    End If

                Next

            Catch ex As Exception
            End Try

        End While
    End Sub
    Sub runsingleaction()
        Select Case action
            Case "S"
                caliberatetime(currenttime)
                wait(10)
                Application.Exit()
            Case "P"
                longrun(UEtype, interval)
            Case "L"
                Do While exitwindow = False
                    displaylog(getPrivateMemory, "r")
 
                    attachdetach(UEtype, a_interval)
                    displaylog("detach UE " + (Val(interval) - Val(a_interval)).ToString + "S", "g")
                    wait(Val(interval) - Val(a_interval))
                Loop
            Case "c"
                wwandial()
                shutdown(UEtype)
                displaylog("UE shutdowned", "r")
            Case "e"
                wwandial()
                resetUE(UEtype)
                displaylog("reset UE", "r")
            Case "pa"
                displaylog("paging mode, interval:" + a_interval.ToString + "S", "r")

                '                paging(UEtype, a_interval)
                udpserverpaging(UEtype, a_interval)
            Case "I"
                attachtoidle(UEtype)
                Do While exitwindow = False
                    displaylog("go to sleep", "r")
                    wait(600)
                    wwandial()
                    calldroped = False
                    monitoring()
                    wwanundial()
                    If calldroped = True Then
                        attachtoidle(UEtype)
                    End If
                Loop
            Case "A"
                attachideltoactive(UEtype, a_interval, interval)
            Case "V"
                Dim callsession As Integer
                Do While True

 

                    'ueinternalip = "1.1.1.2"
                    Dim itmp As Integer = 0
                    While attachcheckinternalip() = "" And itmp < 4
                        displaylog("Can not find UE IP address, try attach " + itmp.ToString + " time", "r")
                        itmp = itmp + 1
                    End While
                    If ueinternalip <> "" Then
                        displaylog("UE IP:" + ueinternalip, "r")
                        Timer4.Enabled = True
                        runvoltenewpar()

                        If traffictype <> "None" Then
                            If addroute() = "OK" Then
                                runtraffic()
                            Else
                                displaylog("Add route fail", "r")
                            End If

                        End If
                        Do
                            wait(interval)
                            Console.CursorTop = Console.CursorTop - 1
                            If traffictype <> "None" Then
                                monitoring()
                            End If
                            callsession = checkoldsh(adbdvicesn, "sh", "shell")
                            If callsession <> "" Then
                                displaylog("Call running:" + callsession, "g")
                            End If
                        Loop While calldroped = False
                        myprocess.killwindowbytitle(serialportname.ToUpper + ".")
                    Else
                        displaylog("Can not find UE IP address, check ue status", "r")
                    End If
                    Timer4.Enabled = True


                Loop
            Case "VOLTEvoiceMOC"

                wait(30)
                Timer4.Enabled = True
                Dim itmp As Integer = 0
                Do While True

                    ' ueinternalip = "1.1.1.2"
                    While attachcheckinternalip() = "" And itmp < 4
                        displaylog("Can not find UE IP address, try attach " + itmp.ToString + " time", "r")
                        itmp = itmp + 1
                    End While

                    If ueinternalip <> "" Then
                        displaylog("UE IP:" + ueinternalip, "r")
                        SerialPort.Close()
                        If traffictype <> "None" Then
                            If addroute() = "OK" Then
                                runtraffic()
                            Else
                                displaylog("Add route fail", "r")
                            End If
                        End If
                        internalthreadMOClocalyy(SerialPort.PortName, a_interval, (Val(interval) - Val(a_interval)).ToString)

                        'loop check traffic and adb
                    Else
                        displaylog("Can not find UE IP address, check ue status", "r")
                    End If
                Loop
                'Case "VOLTEvoiceMOC"

                '    wait(30)
                '    Dim itmp As Integer = 0
                '    Do While True
                '        If getPrivateMemory() > 100000 Then
                '            Application.Restart()
                '        End If
                '        ' ueinternalip = "1.1.1.2"
                '        While attachcheckinternalip() = "" And itmp < 4
                '            displaylog("Can not find UE IP address, try attach " + itmp.ToString + " time", "r")
                '            itmp = itmp + 1
                '        End While

                '        If ueinternalip <> "" Then
                '            displaylog("UE IP:" + ueinternalip, "r")
                '            SerialPort.Close()
                '            runvolteinternalvoicecall(SerialPort.PortName, a_interval, (Val(interval) - Val(a_interval)).ToString)

                '            If traffictype <> "None" Then
                '                If addroute() = "OK" Then
                '                    runtraffic()
                '                Else
                '                    displaylog("Add route fail", "r")
                '                End If
                '            End If
                '            Do

                '                wait(Val(interval))
                '                Console.CursorTop = Console.CursorTop - 1
                '                If traffictype <> "None" Then
                '                    monitoring()
                '                End If

                '            Loop While calldroped = False
                '            myprocess.killwindowbytitle(serialportname.ToUpper + ".")
                '            wait(1)
                '            SerialPort.Open()
                '            'loop check traffic and adb
                '        Else
                '            displaylog("Can not find UE IP address, check ue status", "r")
                '        End If
                '    Loop
                'Case "VOLTEvoiceMTC"

                '    Dim itmp As Integer = 0
                '    Do While True
                '        If getPrivateMemory() > 100000 Then
                '            Application.Restart()
                '        End If

                '        While attachcheckinternalip() = "" And itmp < 4
                '            displaylog("Can not find UE IP address, try attach " + itmp.ToString + " time", "r")
                '            itmp = itmp + 1
                '        End While

                '        If ueinternalip <> "" Then
                '            displaylog("UE IP:" + ueinternalip, "r")
                '            SerialPort.Close()
                '            runvolteinternalvoicewait(SerialPort.PortName)

                '            If traffictype <> "None" Then
                '                If addroute() = "OK" Then
                '                    runtraffic()
                '                Else
                '                    displaylog("Add route fail", "r")
                '                End If

                '            End If
                '            Do
                '                wait(Val(interval))
                '                Console.CursorTop = Console.CursorTop - 1
                '                If traffictype <> "None" Then
                '                    monitoring()
                '                End If

                '            Loop While calldroped = False
                '            myprocess.killwindowbytitle(serialportname.ToUpper + ".")
                '            wait(1)
                '            SerialPort.Open()
                '        Else
                '            displaylog("Can not find UE IP address, check ue status", "r")
                '        End If
                '    Loop
            Case "VOLTEvoiceMTC"

                Dim itmp As Integer = 0
                Timer4.Enabled = True
                Do While True


                    While attachcheckinternalip() = "" And itmp < 4
                        displaylog("Can not find UE IP address, try attach " + itmp.ToString + " time", "r")
                        itmp = itmp + 1
                    End While

                    If ueinternalip <> "" Then
                        displaylog("UE IP:" + ueinternalip, "r")
                        If traffictype <> "None" Then
                            If addroute() = "OK" Then
                                runtraffic()
                            Else
                                displaylog("Add route fail", "r")
                            End If

                        End If
                        internalthreadwaitlocalyy(SerialPort.PortName)

                    Else
                        displaylog("Can not find UE IP address, check ue status", "r")
                    End If
                Loop
        End Select
    End Sub





    Function isdrop() As Boolean
        isdrop = False


    End Function


    Sub runtraffic()
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
        If traffictype = "udpdl" Then
            runudpdl()
        End If
        If traffictype = "udpul" Then
            runudpul()
        End If
    End Sub
    Sub runudpul()
        'hartbeat and sent command thread
        Dim thr As New Thread(AddressOf runnew)
        thr.Name = "udpul"
        thr.IsBackground = True
        thr.Start(udpulstr)
        'run local miperf
        'Dim myprocess As Process = New Process()
        'myprocess.StartInfo.FileName = "d:\mueauto\myiperfs.exe"
        'myprocess.StartInfo.WorkingDirectory = "d:\mueauto"
        'myprocess.StartInfo.Verb = "runas"

        'myprocess.StartInfo.Arguments = serialportname  '"-s -fk -u -i10 -p33892"
        'myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized
        'myprocess.Start()
        'udpdlhandle = myprocess.Id
        'displaylog("iperf receive session id opened:" & myprocess.Id.ToString, "g")

    End Sub
    Sub killoldthread(ByVal ipstr As String)
        'Dim killedthread As Threading.Thread
        'If threadlist.ContainsKey(ipstr) Then

        '    killedthread = threadlist(ipstr)
        '    killedthread.Abort()
        '    threadlist.Remove(ipstr)

        'End If
    End Sub

    Sub killoldprocess(ByVal ipstr As String)
        Dim killid As Integer = 0
        Try
            myprocess.killwindowbytitle(serialportname.ToUpper + ".")
            killid = udpulhandle
            Dim pProcessTemp As System.Diagnostics.Process = Process.GetProcessById(killid)
            displaylog("kill process id:" & killid & "  process name:" & pProcessTemp.ProcessName, "g")
            pProcessTemp.Kill()
            pProcessTemp.Close()

        Catch
        End Try


    End Sub
    Function getcommandlist(ByVal command As String) As String()
        Dim commandstr As String = ""
        commandstr = Split(command, "|")(0)
        Return Split("]" + commandstr + "[", "][")

    End Function
    Sub runnew(ByVal command As String)
        Dim ipstr As String = ""
        Dim commandlist As String()
        Dim commandsnum As Integer = 0
        ipstr = serverip
        killoldprocess(ipstr)

        commandlist = getcommandlist(command)
        Do While 1
            For Each paramtere As String In commandlist
                If paramtere <> "" Then
                    commandsnum = commandsnum + 1
                    killoldprocess(ipstr)
                    runiperf(paramtere, ipstr)
                    Threading.Thread.Sleep(Val(Split(paramtere, ",")(2)) * 1000)
                End If
            Next
        Loop

    End Sub
    Sub runiperf(ByVal paramtere As String, ByVal ipstr As String)
        Dim path As String = ""
        Dim speed As String = "100"
        Dim packetsize As String = "1000"
        Dim ip, port, dueration As String
        path = "D:\mueauto"
        speed = Split(paramtere, ",")(0)
        If speed <> "0" Then
            dueration = Split(paramtere, ",")(2)
            packetsize = Split(paramtere, ",")(1)
            ip = ipstr
            'port = Split(ipstr, ":")(1)
            Dim myprocess As Process = New Process()

            myprocess.StartInfo.FileName = path + "\myiperf.exe"
            displaylog("miperf parameter:" + "-c" + ip + " -t" + dueration + " -u -b" + speed + ".0k -l" + packetsize + " -i 10 -p 33892", "g")
            myprocess.StartInfo.Arguments = serialportname.ToUpper + ". " + ip + " " + dueration + " " + speed + " " + packetsize 'serialportname + " -c" + ip + " -t" + dueration + "-u -b" + speed + ".0k -l" + packetsize + " -i 10 -p 33892"
            myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized

            myprocess.Start()
            udpulhandle = myprocess.Id
            displaylog("udp session id opened:" & myprocess.Id.ToString, "g")

        Else
            displaylog("speed 0, no traffic,go sleep", "g")
        End If

    End Sub


    Sub killtraffic()
        myprocess.killwindowbytitle(serialportname.ToUpper + ".")
        If traffictype = "ftpdl" Or traffictype = "ftpul" Or traffictype = "ftpdlul" Then
            killoldftp()
        End If
        If traffictype = "http" Then
            killoldhttp()
        End If
        If traffictype = "httpdownload" Then
            killoldhttpdownload()
        End If
        If traffictype = "ping" Then
            killoldping()
        End If
        If traffictype = "video" Then
            killoldvolte()
        End If
        If traffictype = "udpdl" Then
            killoldudpdl()
        End If
        If traffictype = "udpul" Then
            killoldudpul()
        End If


    End Sub
    Sub runvolteinternalvoicewaitinternal(ByVal serialportname As String)
        Dim thr As New Thread(AddressOf internalthreadwaityy)
        thr.Name = "MOC"
        thr.Start(serialportname)
        displaylog("Start YYMTC", "g")
    End Sub
    Sub runvolteinternalvoicecallinternal(ByVal serialportname As String, ByVal interval As String, ByVal stopinterval As String)
        Dim thr As New Thread(AddressOf internalthreadcallyy)
        thr.Name = "MOC"
        thr.Start(serialportname + "|" + interval + "|" + stopinterval)
        displaylog("Start YYMOC to " + targetphonenumber, "g")
    End Sub
    Function sendcommand(ByVal commandlist As List(Of String)) As Boolean
        Try
            For Each Command As String In commandlist

                serialSendData(Command & vbCrLf, logname)

                displaylog(Command, "g")

                wait(1)
            Next
        Catch ex As Exception
            displaylog("serialport error:" + ex.ToString, "r")
            Return False
        End Try
        Return True

    End Function
    Function ishavering(ByVal serialport As IO.Ports.SerialPort) As Boolean

        Dim message As String = serialport.ReadExisting

        If message.IndexOf("RING") = True Then

            displaylog(message, "g")
            Return True
        End If

        Return False

    End Function


    Sub internalthreadwaitlocalyy(ByVal serialportname As String)
        Dim sername As String = ""
        Dim nocalltime As Integer = 0
        sername = serialportname
        Console.Title = "MUEclient:" + serialportname.ToUpper + "." + "MTC imsi" + imsi
        Dim initcallcommandlist1 As New List(Of String)
        Dim initcallcommandlist As New List(Of String)
        Dim ringcommandlist As New List(Of String)
        Dim offcommandlist As New List(Of String)
        initcallcommandlist1.Add("ate")
        initcallcommandlist1.Add("at+remoat=5,get_serial")
        initcallcommandlist.Add("AT+CFUN = 1")
        initcallcommandlist.Add("AT+WAVMODE = 4, 0")
        initcallcommandlist.Add("AT+WAVMODE = 3, 0")

        ringcommandlist.Add("ATA")
        ringcommandlist.Add("AT+WAVMODE = 4, 3")
        ringcommandlist.Add("AT+WAVMODE = 4, 2")
        ringcommandlist.Add("AT+WAVMODE = 3, 2")

        offcommandlist.Add("ATH")
        offcommandlist.Add("AT+CHUP")
        offcommandlist.Add("AT+WAVMODE=4,0")
        offcommandlist.Add("AT+WAVMODE = 3, 0")

        displaylog("serial port name:" & sername, "g")
        ' Create a new SerialPort object with default settings.

        Try
            If SerialPortOpen(logname, False) Then
                'serialSendData("AT" + vbCr, _serialPort)
                waitadbsn = True
                wait(1, False)
                sendcommand(initcallcommandlist1)
                displaylog("Checking ADB sn...", "g")
                Do While adbdvicesn = ""
                    wait(1, False)
                Loop
                waitadbsn = False
                displaylog("Got adb sn:" + adbdvicesn, "g")
                Console.WriteLine(rundoscomandt("adb -s " + adbdvicesn + " push test.wav /data/play.wav"))

                sendcommand(initcallcommandlist)
                Do While 1
                    nocalltime = nocalltime + 1
                    If nocalltime > 300 And isanswering = False Then
                        displaylog("No call 5 mintues, restart UE for no IMS regist issue!", "r")
                        Application.Restart()

                    End If
 
                    If isring = True Then
                        isanswering = True
                        sendcommand(ringcommandlist)
                        isring = False
                        nocalltime = 0
                        displaylog("Answering...", "g")
                    End If
                    If isoff = True Then
                        isanswering = False
                        sendcommand(offcommandlist)
                        isoff = False
                        displaylog("Hung up", "g")
                    End If
                    wait(1, False)


                Loop
            Else

                displaylog("Open " + serialportname + " fail", "r")

            End If
        Catch ex As Exception

            displaylog(ex.ToString, "r")
        End Try



    End Sub
    Sub internalthreadMOClocalyy(ByVal serialportname As String, ByVal stopinterval As String, ByVal callinginterval As String)
        Dim sername As String = ""

        sername = serialportname
        Console.Title = "MUEclient:" + serialportname.ToUpper + "." + " MOC target:" + targetphonenumber
        Dim initcallcommandlist As New List(Of String)
        Dim initcallcommandlist1 As New List(Of String)
        Dim stopcommandlist As New List(Of String)
        Dim callcommandlist As New List(Of String)
        initcallcommandlist1.Add("ate")
        initcallcommandlist1.Add("at+remoat=5,get_serial")

        initcallcommandlist.Add("AT+CFUN = 1")
        initcallcommandlist.Add("AT+WAVMODE = 4, 0")
        initcallcommandlist.Add("AT+WAVMODE = 3, 0")
        initcallcommandlist.Add("ATD" + targetphonenumber + ";")
        initcallcommandlist.Add("AT+WAVMODE=4,3")
        initcallcommandlist.Add("AT+WAVMODE = 4, 2")
        initcallcommandlist.Add("AT+WAVMODE = 3, 2")

        stopcommandlist.Add("ATH")
        stopcommandlist.Add("AT+CHUP")
        stopcommandlist.Add("AT+WAVMODE=4,0")
        stopcommandlist.Add("AT+WAVMODE = 3, 0")

        callcommandlist.Add("ATD" + targetphonenumber + ";")
        callcommandlist.Add("AT+WAVMODE = 4, 3")
        callcommandlist.Add("AT+WAVMODE = 4, 2")
        callcommandlist.Add("AT+WAVMODE = 3, 2")

        displaylog("serial port name:" & sername, "g")
        ' Create a new SerialPort object with default settings.
        Dim callclock As Integer = 0
        Try
            If SerialPortOpen(logname, False) Then
                'serialSendData("AT" + vbCr, _serialPort)
                callfail = False
                waitadbsn = True
                wait(1, False)
                sendcommand(initcallcommandlist1)
                displaylog("Checking ADB sn...", "g")
                Do While adbdvicesn = ""
                    wait(1, False)
                Loop

                displaylog("Got adb sn:" + adbdvicesn, "g")
                Console.WriteLine(rundoscomandt("adb -s " + adbdvicesn + " push test.wav /data/play.wav"))
                sendcommand(initcallcommandlist)

                Do While 1
 
                    Do While callclock < Val(callinginterval)
                        If callfail = True Then

                            displaylog("Call fail", "g")
                            Exit Do
                        End If
                        wait(1, False)
                        callclock = callclock + 1
                    Loop
                    callclock = 0
                    sendcommand(stopcommandlist)
                    displaylog("Hung up", "g")
                    wait(Val(stopinterval))
                    displaylog("Calling...", "g")
                    callfail = False
                    sendcommand(callcommandlist)
                Loop


            Else

                displaylog("Open " + serialportname + " fail", "r")

            End If
        Catch ex As Exception

            displaylog(ex.ToString, "r")
        End Try



    End Sub

    Sub internalthreadwaityy(ByVal command)
        Dim sername As String = ""
        Dim interval As String = ""
        Dim stopinterval As String = ""
        sername = Split(command, "|")(0)
        interval = Split(command, "|")(1)
        stopinterval = Split(command, "|")(2)
        Dim initcallcommandlist As New List(Of String)
        Dim ringcommandlist As New List(Of String)

        initcallcommandlist.Add("ate")
        initcallcommandlist.Add("at+remoat=5,get_serial")
        initcallcommandlist.Add("AT+CFUN = 1")
        initcallcommandlist.Add("AT+WAVMODE = 4, 0")
        initcallcommandlist.Add("AT+WAVMODE = 3, 0")

        ringcommandlist.Add("AT+WAVMODE = 4, 3")
        ringcommandlist.Add("AT+WAVMODE = 4, 2")
        ringcommandlist.Add("AT+WAVMODE = 3, 2")
        ringcommandlist.Add("ATA")
        Dim opentime As Integer = 0
        Dim sComparer As StringComparer = StringComparer.OrdinalIgnoreCase
        Dim _continue As Boolean
        Dim _serialPort As New IO.Ports.SerialPort
        displaylog("serial port name:" & sername, "g")
        ' Create a new SerialPort object with default settings.

        _serialPort.PortName = sername

        _serialPort.BaudRate = 115200
        _serialPort.StopBits = IO.Ports.StopBits.One
        _serialPort.Parity = IO.Ports.Parity.None
        _serialPort.DataBits = 8
        ' Set the read/write timeouts
        _serialPort.ReadTimeout = 5000
        _serialPort.WriteTimeout = 500
        _serialPort.DtrEnable = True
        _serialPort.RtsEnable = True
        _continue = True
        Dim repeatetime As Integer = 0
        Do
            Try
                _serialPort.Open()
            Catch ex As TimeoutException
                Console.WriteLine("Timeout")
                Console.WriteLine(ex.ToString)
            Catch ex As Exception
                Console.WriteLine("open port fail")
            End Try
            opentime = opentime + 1
        Loop Until _serialPort.IsOpen = True Or opentime > 3


        If _serialPort.IsOpen = True Then
            'serialSendData("AT" + vbCr, _serialPort)
            'sendcommand(initcallcommandlist, _serialPort)
            'Do While 1
            '    If isring(_serialPort) = True Then Exit Sub
            '    wait(1)


            'Loop
            _serialPort.Close()
            _continue = True
        Else

            displaylog("Open " + _serialPort.PortName + " fail", "r")

        End If








    End Sub
    Sub internalthreadcallyy(ByVal command)

        Dim sername As String = ""
        Dim interval As String = ""
        Dim stopinterval As String = ""
        sername = Split(command, "|")(0)
        interval = Split(command, "|")(1)
        stopinterval = Split(command, "|")(2)
        Dim initcallcommandlist As New List(Of String)
        Dim stopcommandlist As New List(Of String)
        Dim callcommandlist As New List(Of String)
        initcallcommandlist.Add("ate")
        initcallcommandlist.Add("at+remoat=5,get_serial")
        initcallcommandlist.Add("AT+CFUN = 1")
        initcallcommandlist.Add("AT+WAVMODE = 4, 0")
        initcallcommandlist.Add("AT+WAVMODE = 3, 0")
        initcallcommandlist.Add("ATD" + targetphonenumber + ";")
        initcallcommandlist.Add("AT+WAVMODE=4,3")
        initcallcommandlist.Add("AT+WAVMODE = 4, 2")
        initcallcommandlist.Add("AT+WAVMODE = 3, 2")
        stopcommandlist.Add("ATH")
        stopcommandlist.Add("AT+CHUP")
        stopcommandlist.Add("AT+WAVMODE=4,0")
        stopcommandlist.Add("AT+WAVMODE = 3, 0")
        callcommandlist.Add("ATD" + targetphonenumber + ";")
        callcommandlist.Add("AT+WAVMODE = 4, 3")
        callcommandlist.Add("AT+WAVMODE = 4, 2")
        callcommandlist.Add("AT+WAVMODE = 3, 2")
        Dim opentime As Integer = 0
        Dim sComparer As StringComparer = StringComparer.OrdinalIgnoreCase
        Dim _continue As Boolean
        Dim _serialPort As New IO.Ports.SerialPort
        displaylog("serial port name:" & sername, "g")
        ' Create a new SerialPort object with default settings.

        _serialPort.PortName = sername

        _serialPort.BaudRate = 115200
        _serialPort.StopBits = IO.Ports.StopBits.One
        _serialPort.Parity = IO.Ports.Parity.None
        _serialPort.DataBits = 8
        ' Set the read/write timeouts
        _serialPort.ReadTimeout = 5000
        _serialPort.WriteTimeout = 500
        _serialPort.DtrEnable = True
        _serialPort.RtsEnable = True
        _continue = True
        Dim repeatetime As Integer = 0
        Do
            Try
                _serialPort.Open()
            Catch ex As TimeoutException
                Console.WriteLine("Timeout")
                Console.WriteLine(ex.ToString)
            Catch ex As Exception
                Console.WriteLine("open port fail")
            End Try
            opentime = opentime + 1
        Loop Until _serialPort.IsOpen = True Or opentime > 3


        If _serialPort.IsOpen = True Then
            'serialSendData("AT" + vbCr, _serialPort)
            'sendcommand(initcallcommandlist, _serialPort)
            'Do While 1
            '    If sendcommand(callcommandlist, _serialPort) = False Then Exit Sub
            '    wait(Int(Val(interval)))
            '    If sendcommand(stopcommandlist, _serialPort) = False Then Exit Sub
            '    wait(Int(Val(stopinterval)))

            'Loop
            _serialPort.Close()
            _continue = True
        Else

            displaylog("Open " + _serialPort.PortName + " fail", "r")

        End If







    End Sub
    Sub serialsenddata2(ByVal command As String, ByVal SerialPort1 As IO.Ports.SerialPort, ByVal linemode As Boolean)
        Dim times As Integer = 0
        While serialSendDatabasic(command, SerialPort1, linemode) <> "OK" And times < 10
            times = times + 1
        End While
        If times = 10 Then Console.WriteLine("Write AT command:" + command + " failed " + times.ToString + " times.")
    End Sub
    Private Function serialSendDatabasic(ByVal command As String, ByVal SerialPort1 As IO.Ports.SerialPort, ByVal linemode As Boolean) As String
        Dim hexsendFlag As Boolean

        Try



            Dim outDataBuf As String = command

            hexsendFlag = False

            If outDataBuf = "" Then Return "OK" '如果输入文本框中没有数据则不发送

            If SerialPort1.IsOpen = True Then '判断串口是否打开

                If hexsendFlag = True Then

                    '-----------十六进制发送------------

                    outDataBuf = outDataBuf.Replace(" ", "") '清除空格与回车

                    outDataBuf = outDataBuf.Replace(vbNewLine, "")

                    '十六进制数据位数为偶数，例如：FF 00 15 AC 0D

                    If outDataBuf.Length Mod 2 <> 0 Then

                        Console.WriteLine("请输入正确的十六进制数，用空格和回车隔开。", "r")
                        Return "OK"

                    End If

                    Dim outBytes(outDataBuf.Length / 2 - 1) As Byte

                    For I As Integer = 1 To outDataBuf.Length - 1 Step 2

                        outBytes((I - 1) / 2) = Val("&H" + Mid(outDataBuf, I, 2)) 'VB的十六进制表示方法，例如0x1D表示为&H1D

                    Next

                    SerialPort1.Write(outBytes, 0, outDataBuf.Length / 2)

                    'BarCountTx.Text = Val(BarCountTx.Text) + outDataBuf.Length / 2

                Else

                    '-------------文本发送--------------
                    If linemode = True Then

                        SerialPort1.WriteLine(outDataBuf)
                    Else
                        For Each STR As Char In outDataBuf
                            SerialPort1.Write(STR)
                            ' Module1.writelog(outDataBuf, 0, logfilename)


                            'BarCountTx.Text = Val(BarCountTx.Text) + outDataBuf.Length '发送字节计数
                        Next
                    End If
                    For Each STR As Char In outDataBuf
                        SerialPort1.Write(STR)
                        ' Module1.writelog(outDataBuf, 0, logfilename)


                        'BarCountTx.Text = Val(BarCountTx.Text) + outDataBuf.Length '发送字节计数
                    Next
                End If
                Return "OK"
            Else
                Console.WriteLine("串口未打开，请先打开串口。", "r")
                SerialPort1.Open()
                Return "fail"
            End If

            Return "fail"
        Catch ex As Exception
            Console.WriteLine("数据输入或发送错误！" + vbNewLine + ErrorToString(), "r")

            If SerialPort1.IsOpen Then
                Try
                    SerialPort1.Close()
                Catch
                End Try
            End If
            Return "fail"

        End Try

    End Function


    Function checktraceon() As Boolean
        Dim servers, tempstr As String
        Dim logonstr As String = ""
        If imsi <> "" Then
            logonstr = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "volterealtarget", "voicelogon")
            If logonstr.IndexOf(imsi) >= 0 Then
                displaylog("voice log on", "r")
                Return True
            Else
                displaylog("Not find imsi in ftp.ini, voice log off", "g")
            End If
        Else
            displaylog("Not find imsi , voice log off", "g")
        End If

    End Function
    Sub runvolteinternalvoicecall(ByVal serialportname As String, ByVal interval As String, ByVal stopinterval As String)
        Dim phonenum As String
        phonenum = getcallnum()
        If phonenum <> "" Then

            Dim myprocess As Process = New Process()
            If checktraceon() = False Then
                myprocess.StartInfo.FileName = "d:\mueauto\dist\voicemoc.exe"
            Else
                myprocess.StartInfo.FileName = "d:\mueauto\dist\voicemoco.exe"
            End If
            'myprocess.StartInfo.FileName = "d:\mueauto\dist\voicemoc.exe"
            myprocess.StartInfo.WorkingDirectory = "d:\mueauto\dist"
            myprocess.StartInfo.Arguments = serialportname + " " + phonenum + " " + interval + " " + stopinterval + " target" + phonenum
            displaylog("volte parameter:" & serialportname + " " + phonenum + " " + stopinterval + " " + interval, "g")



            myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized
            myprocess.Start()
            voltehandle(0) = myprocess.Id
            displaylog("volte call MOC process started:" & myprocess.Id.ToString, "g")



        Else

            displaylog("Not find phone number in Ftp.ini, imsi:" + imsi, "r")
        End If

    End Sub

    Sub runvolteinternalvoicewait(ByVal serialportname As String)
        ' Dim phonenum As String
        'phonenum = getcallnum()
        'If phonenum <> "" Then

        Dim myprocess As Process = New Process()
        If checktraceon() = False Then
            myprocess.StartInfo.FileName = "d:\mueauto\dist\voicemtc.exe"
        Else
            myprocess.StartInfo.FileName = "d:\mueauto\dist\voicemtco.exe"
        End If

        myprocess.StartInfo.WorkingDirectory = "d:\mueauto\dist"
        myprocess.StartInfo.Arguments = serialportname + " imsi" + imsi
        displaylog("volte MTC parameter:" & serialportname, "g")



        myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized
        myprocess.Start()
        voltehandle(0) = myprocess.Id
        displaylog("volte call MTC process started:" & myprocess.Id.ToString, "g")



        'Else

        'displaylog("Not find phone number in Ftp.ini, imsi:" + imsi, "r")
        'End If

    End Sub

    Function attachcheckinternalip() As String
        UErealip = ""
        serialSendData("at+chup" & vbCr, logname)
        displaylog("at+chup", "g")

        wait(1)
        serialSendData("AT+CGATT=0" & vbCr, logname)
        displaylog("AT+CGATT=0", "g")

        wait(1)

        serialSendData("AT+CFUN=0" & vbCr, logname)
        displaylog("AT+CFUN=0", "g")
        wait(2)

        serialSendData("AT+CFUN=1" & vbCr, logname)
        displaylog("AT+CFUN=1", "g")
        wait(20)

        serialSendData("AT+CIMI" & vbCr, logname)
        displaylog("AT+CIMI", "g")
        wait(1)
        serialSendData("AT+CGCONTRDP" & vbCr, logname)
        displaylog("AT+CGCONTRDP", "g")
        wait(1)
        Dim i As Integer = 0
        While ueinternalip = "" And i = 3
            serialSendData("AT+CGCONTRDP" & vbCr, logname)
            'wait(1)
            'serialSendData("AT+CGCONTRDP" & vbCrLf, logname)
            wait(1)
            i = i + 1
        End While
        If UEtype <> "SMG9350" Then
            serialSendData("AT$QCRMCALL=1,1,1,2,1" & vbCrLf, logname)
            wait(1)
            serialSendData("AT$QCRMCALL=1,1" & vbCrLf, logname)
            wait(3)
            If addroute() = "OK" Then
                If traffictype = "None" Then
                    serialSendData("AT$QCRMCALL=0,1" & vbCrLf, logname)
                    wait(1)
                    serialSendData("AT$QCRMCALL=0,1,1,2,1" & vbCrLf, logname)
                End If

            Else
                ueinternalip = ""
            End If

        End If
        Return ueinternalip
    End Function
    Sub attachtoidle(ByVal uetype As String)

        ''----------------attach ue in -------------------------
        'Do
        '    Select Case uetype
        '        Case "H"
        '            attachdetach_HISI()
        '        Case "E5776"
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

                Case "E5776"
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
            Dim h = 0
            Do While (h < 3) And ueinternalip = ""
                serialSendData("AT+CGCONTRDP" & vbCrLf, logname)
                displaylog("AT+CGCONTRDP", "g")
                wait(10)
                displaylog("UErealip:=" + ueinternalip, "r")

                h = h + 1
            Loop

            If addroute() = "ip not find" Then
                If ueinternalip <> "" And (uerealtype = "Sierra") Then
                    displaylog("UE attach success, but it can not assign IP to PC. For Sierra chip, try disable/enable netcard", "r")
                    disenablenetcard()

                End If
                attachstate = attachstate * False
            End If
            If (uetype = "H" Or uetype = "E5776") And attachstate = True Then
                If monitoring(True) <> "OK" Then
                    attachstate = False
                End If
            End If
            attachstate = attachstate * Not (exitwindow)
            If attachstate = False Then
                displaylog("attach fail", "r")
            End If
            If attachstate = False Then
                Randomize()
                wait(30 + Int(Rnd(30)))
            End If
        Loop Until attachstate = True
        wwanundial()

    End Sub
    Sub killoldpagingsessions(ByVal sessions As List(Of Int64))
        Dim pProcessTemp As System.Diagnostics.Process
        For Each session As Int64 In sessions
            pProcessTemp = Process.GetProcessById(session)
            displaylog("kill process id:" & session & "  process name:" & pProcessTemp.ProcessName, "g")
            pProcessTemp.Kill()
            pProcessTemp.Close()
            sessions.Clear()
        Next
    End Sub
    Function pagingoneip(ByRef sessions As List(Of Int64), ByVal ip As String, ByVal interval As Integer)
        'Dim command As String
        If pagingaddroute(ip) = "OK" Then

            Try


                If ip <> "" Then


                    Dim intervalstring As String = (interval * 1000).ToString
                    Dim lenstring As String = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "ping", "len")
                    displaylog("start new ping session, interval:" + intervalstring + ",length:" + lenstring, "g")
                    'Command = "d:\mueauto\hrping.exe " + ip + " -t -L " + lenstring + " -y 60 -s " + intervalstring
                    'Dim myProcess As Process = New Process()
                    ''Dim s As String
                    'myProcess.StartInfo.FileName = "cmd.exe"
                    ''myProcess.StartInfo.UseShellExecute = False
                    ''myProcess.StartInfo.CreateNoWindow = True
                    'myProcess.StartInfo.RedirectStandardInput = True
                    ''myProcess.StartInfo.RedirectStandardOutput = True
                    ''myProcess.StartInfo.RedirectStandardError = True
                    'myProcess.Start()
                    'sessions.Add(myProcess.Id)
                    'SetWindowText(myProcess.MainWindowHandle, "Ping:" + serialportname.ToUpper)
                    'Dim sIn As StreamWriter = myProcess.StandardInput
                    'sIn.AutoFlush = True
                    ''Dim sOut As StreamReader = myProcess.StandardOutput
                    ''Dim sErr As StreamReader = myProcess.StandardError
                    'sIn.Write(command & _
                    'System.Environment.NewLine)
                    ''sIn.Write("exit" & System.Environment.NewLine)
                    ''result = sOut.ReadToEnd()
                    'If Not myProcess.HasExited Then
                    '    myProcess.Kill()
                    'End If
                    ''displaylog(s, "g", False)
                    'sIn.Close()
                    ''sOut.Close()
                    ''sErr.Close()
                    Dim myprocess As Process = New Process()
                    If 1 = 1 Then
                        myprocess.StartInfo.FileName = "d:\mueauto\myping.exe"
                        myprocess.StartInfo.WorkingDirectory = "d:\mueauto"
                        myprocess.StartInfo.Verb = "runas"
                        myprocess.StartInfo.Arguments = "Ping:" + serialportname.ToUpper + ". " + ip + " " + lenstring + " " + intervalstring
                        ' myprocess.StartInfo.Arguments = "127.0.0.1 -t"
                        'myprocess.StartInfo.UseShellExecute = True
                        'myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized

                        myprocess.Start()
                        sessions.Add(myprocess.Id)
                        displaylog("ping session id opened:" & myprocess.Id.ToString, "g")
                        wait(1)
                        'SetWindowTextA(myprocess.MainWindowHandle, "2222")
                        'SetWindowTextA(myprocess.MainWindowHandle, "Ping:" + serialportname.ToUpper)

                    End If

                    Return "OK"
                Else
                    displaylog("no ip no traffic", "r")
                    Return "no ip no traffic"
                End If

            Catch ex As Exception
                displaylog("Ping is not excuted.", "g")
                Return "Ping is not excuted."
            End Try




        Else
            displaylog("Check UE:" + ip + " state, add route fail", "r")

        End If

    End Function
    Sub udpserverpaging(ByVal uetype As String, ByVal paginginterval As String)
        Dim itmp As Integer = 0
 
        'ueinternalip = "1.1.1.2"
        While attachcheckinternalip() = "" And itmp < 4
            displaylog("Can not find UE IP address, try attach " + itmp.ToString + " time", "r")
            itmp = itmp + 1
        End While

        If ueinternalip <> "" Then
            displaylog("UE IP:" + ueinternalip, "r")
            If addroute() = "OK" Then
                udppaging(paginginterval)
            Else
                displaylog("Add route fail", "r")
            End If


        Else
            displaylog("Can not find UE IP address, check ue status", "r")
        End If
    End Sub

    Sub udppaging(ByVal paginginterval As String)
        Dim hartbeatinterval As String = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "udp", "hartbeat")
        If hartbeatinterval = "" Then hartbeatinterval = "600"
        Dim localIpep As New IPEndPoint(IPAddress.Parse(UErealip), 33893) ' // 本机IP，指定的端口号  
        Dim udpcSend = New Net.Sockets.UdpClient(localIpep)
        Dim message As String = UErealip + ":33893" + "|p|" + paginginterval
        Dim sendbytes As Byte() = Encoding.ASCII.GetBytes(message)
        Dim oldip As String = UErealip

        Dim remoteIpep As New IPEndPoint(IPAddress.Parse(serverip), 33892) ' // 发送到的IP地址和端口号  

        udpcSend.Send(sendbytes, sendbytes.Length, remoteIpep)
        Do While exitwindow = False
            ' goidel(UEtype)
            Try

                wait(Val(hartbeatinterval))
                ' goactive(UEtype)
                findueip()
                message = UErealip + ":33893" + "|h"
                If oldip <> UErealip Then

                    message = UErealip + ":33893" + "|p|" + paginginterval

                    displaylog("ip address changed from " & oldip & " to " & UErealip & ",send new paging command", "r")
                    oldip = UErealip
                    localIpep = New IPEndPoint(IPAddress.Parse(UErealip), 33893)
                    udpcSend = New Net.Sockets.UdpClient(localIpep)
                    remoteIpep = New IPEndPoint(IPAddress.Parse(serverip), 33892)
                End If

                sendbytes = Encoding.ASCII.GetBytes(message)
                udpcSend.Send(sendbytes, sendbytes.Length, remoteIpep)
            Catch ex As Exception
                displaylog(ex.ToString, "r")
            End Try


        Loop


    End Sub

    Sub paging(ByVal uetype As String, ByVal interval As Integer)
        Dim total, fail As Integer

        total = 0
        fail = 0

        '----------------attach ue in -------------------------
        'Do
        '    Select Case uetype
        '        Case "H"
        '            attachdetach_HISI()
        '        Case "E5776"
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
        Dim attachstate As Boolean = True
        Do 'UE attach until get add route ok
            attachstate = True
            '-------------- UE detach+attach-----------------
            Select Case uetype
                Case "H"
                    If attachdetach_HISI() <> "OK" Then
                        attachstate = False
                    End If

                Case "E5776"
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
            Dim h = 0
            Do While (h < 3) And ueinternalip = ""
                serialSendData("AT+CGCONTRDP" & vbCrLf, logname)
                displaylog("AT+CGCONTRDP", "g")
                wait(10)
                displaylog("UErealip:=" + ueinternalip, "r")

                h = h + 1
            Loop
            If addroute() = "ip not find" Then
                If ueinternalip <> "" And (uerealtype = "Sierra") Then
                    displaylog("UE attach success, but it can not assign IP to PC. For Sierra chip, try disable/enable netcard", "r")
                    disenablenetcard()

                End If
                attachstate = attachstate * False
            End If
            If (uetype = "H" Or uetype = "E5776") And attachstate = True Then
                If monitoring(True) <> "OK" Then
                    attachstate = False
                End If
            End If
            attachstate = attachstate * Not (exitwindow)
            If attachstate = False Then
                displaylog("attach fail", "r")
            End If
        Loop Until attachstate = True
        displaylog("attach success,attach try times:" + trytimes.ToString, "g")
        Do While exitwindow = False
            '--------read paging target
            wait(2)
            If updatepaginglistflag = True Then
                updatepaginglistflag = False
                displaylog("Paging list is changed, the newest list is :" + oldpagingstring, "r")
                killoldpagingsessions(pagingsessionlist)

                If paginglist.Count <> 0 Then
                    For Each ip As String In paginglist
                        pagingoneip(pagingsessionlist, ip, interval)
                    Next

                Else
                    displaylog("No target ue IPs find in system after remove local IPs", "r")
                End If
            Else
                displaylog("Paging list is not changed, the old list is :" + oldpagingstring, "r")
            End If
            wait(180)
            monitoring()
            If calldroped = True Then
                killoldpagingsessions(pagingsessionlist)
                updatepaginglistflag = True
                Do 'UE attach until get add route ok
                    attachstate = True
                    '-------------- UE detach+attach-----------------
                    Select Case uetype
                        Case "H"
                            If attachdetach_HISI() <> "OK" Then
                                attachstate = False
                            End If

                        Case "E5776"
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
                    Dim h = 0
                    Do While (h < 3) And ueinternalip = ""
                        serialSendData("AT+CGCONTRDP" & vbCrLf, logname)
                        displaylog("AT+CGCONTRDP", "g")
                        wait(10)
                        displaylog("UErealip:=" + ueinternalip, "r")

                        h = h + 1
                    Loop
                    If addroute() = "ip not find" Then
                        If ueinternalip <> "" And (uerealtype = "Sierra") Then
                            displaylog("UE attach success, but it can not assign IP to PC. For Sierra chip, try disable/enable netcard", "r")
                            disenablenetcard()

                        End If
                        attachstate = attachstate * False
                    End If
                    If (uetype = "H" Or uetype = "E5776") And attachstate = True Then
                        If monitoring(True) <> "OK" Then
                            attachstate = False
                        End If
                    End If
                    attachstate = attachstate * Not (exitwindow)
                    If attachstate = False Then
                        displaylog("attach fail", "r")
                    End If
                Loop Until attachstate = True
            Else
                displaylog("paging UE list not change, go on the old paging processes", "g")
            End If
        Loop

    End Sub

    Function pingoneue(ByVal ip As String) As String
        Dim returnstr As String
        Dim values As String
        values = ""
        displaylog("paging ping ue:" + ip, "r")

        returnstr = rundoscomandt("ping -w 10000 -n " + "5" + " " + ip)

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
        displaylog("Clear old route to " & serverip, "r")
        returnstr = rundoscomandt("route delete " + serverip)
        wait(1)
        displaylog("UE IP:" & ipaddress & "  Gateway:" & gateway, "r")
        UErealip = ipaddress
        Dim interfaceid As String = ""
        interfaceid = getportinterface(SerialPort.PortName)
        displaylog("net port id:" + interfaceid, "r")
        displaylog("Route Add :" + serverip + " " + gateway + " IF " + interfaceid, "g")
        If interfaceid <> "" Then
            returnstr = rundoscomandt("route add " + serverip + " " + gateway + " IF " + interfaceid)
        Else
            returnstr = rundoscomandt("route add " + serverip + " " + gateway)
        End If
        Return "OK"

    End Function

    Function getallip(ByVal isIPV4 As Boolean) As String
        Dim adapters As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
        Dim adapter As NetworkInterface
        Dim ips As String = ""
        Dim ipaddress, gatewayipaddress As String

        For Each adapter In adapters
            Application.DoEvents()
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
    'Function rundoscommand(ByVal command) As String
    '    Dim myProcess As Process = New Process()
    '    Dim s As String
    '    myProcess.StartInfo.FileName = "cmd.exe"
    '    myProcess.StartInfo.UseShellExecute = False
    '    myProcess.StartInfo.CreateNoWindow = True
    '    myProcess.StartInfo.RedirectStandardInput = True
    '    myProcess.StartInfo.RedirectStandardOutput = True
    '    myProcess.StartInfo.RedirectStandardError = True
    '    myProcess.Start()
    '    Dim sIn As StreamWriter = myProcess.StandardInput
    '    sIn.AutoFlush = True

    '    Dim sOut As StreamReader = myProcess.StandardOutput
    '    Dim sErr As StreamReader = myProcess.StandardError
    '    sIn.Write(command & _
    '    System.Environment.NewLine)
    '    sIn.Write("exit" & System.Environment.NewLine)
    '    s = sOut.ReadToEnd()
    '    If Not myProcess.HasExited Then
    '        myProcess.Kill()
    '    End If

    '    displaylog(s, "g", False)

    '    sIn.Close()
    '    sOut.Close()
    '    sErr.Close()
    '    myProcess.Close()
    '    Return s
    'End Function
    Private Sub rundoscommand(ByVal commands As String, Optional ByVal workdir As String = "d:\mueauto\autocall")
        Dim command() As String
        Try
            command = Split(commands, "|")
            Dim myProcess As Process = New Process()
            'Dim s As String
            myProcess.StartInfo.FileName = "cmd.exe"
            myProcess.StartInfo.WorkingDirectory = workdir
            myProcess.StartInfo.UseShellExecute = False
            myProcess.StartInfo.CreateNoWindow = True
            myProcess.StartInfo.RedirectStandardInput = True
            myProcess.StartInfo.RedirectStandardOutput = True
            myProcess.StartInfo.RedirectStandardError = True
            myProcess.Start()
            backprogress = myProcess.Id
            Dim sIn As StreamWriter = myProcess.StandardInput
            sIn.AutoFlush = True
            Dim sOut As StreamReader = myProcess.StandardOutput
            Dim sErr As StreamReader = myProcess.StandardError

            For Each cmd As String In command
                Try
                    sIn.Write(cmd & System.Environment.NewLine)
                Catch ex As Exception
                    Dim a = 1
                    Console.WriteLine("send dos command fail," + ex.ToString)
                End Try

                Thread.Sleep(100)
            Next

            sIn.Write("exit" & System.Environment.NewLine)
            If myProcess.WaitForExit(5000) Then
                Console.WriteLine("reading result")
                result = sOut.ReadToEnd()
            Else
                result = "fail"
                myProcess.Kill()

            End If

            If Not myProcess.HasExited Then
                myProcess.Kill()
            End If
            'displaylog(s, "g", False)
            sIn.Close()
            sOut.Close()
            sErr.Close()
            myProcess.Close()

            't.Abort()
        Catch ex1 As Exception
            Console.WriteLine("waiting command result fail," + ex1.ToString)
            't.Abort()
        End Try
    End Sub

    Private Sub rundoscommandmoniter2(ByVal command As String, Optional ByVal workdir As String = "d:\mueauto\autocall")

        Dim s As String
        Dim realcommand As String = ""
        Dim resultsign As String = ""
        realcommand = Split(command, "@", 2)(0)
        resultsign = Split(command, "@", 2)(1)


        Dim myProcess As Process = New Process()
        'Dim s As String
        myProcess.StartInfo.FileName = "cmd.exe"
        myProcess.StartInfo.WorkingDirectory = workdir
        myProcess.StartInfo.UseShellExecute = False
        myProcess.StartInfo.CreateNoWindow = True
        myProcess.StartInfo.RedirectStandardInput = True
        myProcess.StartInfo.RedirectStandardOutput = True
        myProcess.StartInfo.RedirectStandardError = True
        Try
            myProcess.Start()
            backprogress = myProcess.Id
            'Thread.Sleep(1000)
            Dim sIn As StreamWriter = myProcess.StandardInput
            sIn.AutoFlush = True
            Dim sOut As StreamReader = myProcess.StandardOutput
            Dim sErr As StreamReader = myProcess.StandardError
            'Thread.Sleep(1000)
            Try
                sIn.Write(realcommand & System.Environment.NewLine)
            Catch ex As Exception
                Dim a = 1

            End Try

            Thread.Sleep(500)

            sIn.Write("exit" & System.Environment.NewLine)
            If myProcess.WaitForExit(1000) = True Then
                s = sOut.ReadToEnd()
                returnstrdic(resultsign) = s + "@" + resultsign
            Else
                returnstrdic(resultsign) = "nothing"
            End If
            If Not myProcess.HasExited Then
                myProcess.Kill()
            End If
            'displaylog(s, "g", False)
            sIn.Close()
            sOut.Close()
            sErr.Close()

            't.Abort()
        Catch ex1 As Exception
            ' t.Abort()
            myProcess.Close()
            returnstrdic(resultsign) = "nothing"
        End Try
    End Sub


    Private Sub rundoscommandmoniter(ByVal commands As String)

        Dim command() As String
        command = Split(commands, "|")
        Dim myProcess As Process = New Process()
        'Dim s As String
        Try
            myProcess.StartInfo.FileName = command(0)
            myProcess.StartInfo.Arguments = command(1)
            myProcess.StartInfo.WorkingDirectory = "d:\mueauto\autocall"
            myProcess.StartInfo.UseShellExecute = False
            myProcess.StartInfo.CreateNoWindow = True
            myProcess.StartInfo.RedirectStandardInput = True
            myProcess.StartInfo.RedirectStandardOutput = True
            myProcess.StartInfo.RedirectStandardError = True
            myProcess.Start()
            backprogress = myProcess.Id
            Dim sIn As StreamWriter = myProcess.StandardInput
            sIn.AutoFlush = True
            Dim sOut As StreamReader = myProcess.StandardOutput
            Dim sErr As StreamReader = myProcess.StandardError

            For Each cmd As String In command
                If cmd <> command(0) And cmd <> command(1) Then
                    Try
                        sIn.Write(cmd & System.Environment.NewLine)
                    Catch ex As Exception
                        Dim a = 1
                    End Try

                    Thread.Sleep(100)
                End If
            Next

            sIn.Write(Chr(3))
            sIn.Write(System.Environment.NewLine)

            sIn.Write(System.Environment.NewLine)
            sIn.Write("exit" & System.Environment.NewLine)
            result = sOut.ReadToEnd()
            If Not myProcess.HasExited Then
                myProcess.Kill()
            End If
            'displaylog(s, "g", False)
            sIn.Close()
            sOut.Close()
            sErr.Close()
            myProcess.Close()
            t.Abort()
        Catch ex As Exception
            t.Abort()
        End Try

    End Sub
    Function rundoscomandt2moniter(ByVal commands As String, ByVal argu As String, ByVal workdir As String) As String
        result = ""
        'dosaction = commands
        If Not t Is Nothing Then
            If t.IsAlive = True Then t.Abort()
        End If
        t = New Thread(AddressOf rundoscommandmoniter) '创建线程，使它指向test过程，注意该过程不能带有参数
        t.IsBackground = True
        t.Start(commands + "|" + argu + "|" + workdir) '启动线程
        Return "OK"
    End Function
    Function rundoscomandt(ByVal commands As String, Optional ByVal timeouts As Integer = 0, Optional ByVal normal As Boolean = True) As String
        result = ""
        'dosaction = commands
        If Not t Is Nothing Then
            If t.IsAlive = True Then t.Abort()
        End If
        If normal = True Then
            t = New Thread(AddressOf rundoscommand) '创建线程，使它指向test过程，注意该过程不能带有参数
        Else
            t = New Thread(AddressOf rundoscommandmoniter)
        End If

        t.IsBackground = True
        t.Start(commands) '启动线程
        If timeouts = 0 Then
            While result = ""
                Application.DoEvents()
                Thread.Sleep(100)
            End While
            Return result
        Else
            wait(timeouts)
            Try
                Process.GetProcessById(backprogress).Kill()
            Catch

            End Try

            t.Abort()
        End If




    End Function

    Function rundoscomandt2(ByVal commands As String, ByVal type As String, Optional ByVal timeouts As Integer = 0, Optional ByVal normal As Boolean = True) As String
        result = ""
        'dosaction = commands

        Dim resultsign As String = Now.ToString + "^" + type
        commands = commands + "@" + resultsign
        While returnstrdic.Count > adbinputpoolsize
            returnstrdic.Remove(returnstrdic.Keys(0))
        End While

        returnstrdic.Add(resultsign, "")
        If Not t Is Nothing Then
            'If t.IsAlive = True Then
            '    displaylog("t is alive, kill it", "r")
            '    t.Abort()
            'End If

        End If
        If normal = True Then
            t = New Thread(AddressOf rundoscommand) '创建线程，使它指向test过程，注意该过程不能带有参数
        Else
            t = New Thread(AddressOf rundoscommandmoniter2)
        End If

        t.IsBackground = True
        t.Start(commands) '启动线程
        If timeouts = 0 Then
            'While returnstrdic(resultsign) = ""
            '    'Application.DoEvents()
            '    Thread.Sleep(10)
            'End While
            Return ""
        Else
            wait(timeouts)
            Try
                Process.GetProcessById(backprogress).Kill()
            Catch

            End Try

            t.Abort()
            Return result
        End If




    End Function




    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
    Sub runvoltenewpar()
        Dim devicename As String = ""
        Dim voltemode As String = "AT"
        devicename = getandrioddevicename()
        Dim phonenum As String = ""
        If devicename = "Not andriod" Then
            displaylog("Not find this ue type in supported Andriod devices list", "r")
            Exit Sub
        End If

        If devicename = "others datacard" Then
            displaylog("Not find this ue type in supported Andriod devices list", "r")
            Exit Sub
        End If
        If devicename = "realUE" Then
            voltemode = "ADB"
        End If

        If voltemode = "ADB" And imsi <> "" Then
            displaylog("imsi:" + imsi, "g")
            phonenum = getcallnum()
            longrunvolterealuepar(adbdvicesn, phonenum, a_interval, interval, "MOC")

        End If

        If voltemode = "ADB" And traffictype = "MTC" And imsi <> "" Then

            'longrunvolterealuepar(adbdvicesn, phonenum, a_interval, "MTC")

        End If
        If voltemode = "AT" And traffictype = "MTC" And imsi <> "" Then

            'longrunvolterealue(adbdvicesn, phonenum, a_interval, "MTC")

        End If
        If voltemode = "AT" And traffictype = "MTC" And imsi <> "" Then

            'longrunvolterealue(adbdvicesn, phonenum, a_interval, "MTC")

        End If
    End Sub
    Sub runvoltenew()
        Dim devicename As String = ""
        Dim voltemode As String = "AT"
        devicename = getandrioddevicename()
        Dim phonenum As String = ""
        If devicename = "Not andriod" Then
            displaylog("Not find this ue type in supported Andriod devices list", "r")
            Exit Sub
        End If

        If devicename = "others datacard" Then
            displaylog("Not find this ue type in supported Andriod devices list", "r")
            Exit Sub
        End If
        If devicename = "realUE" Then
            voltemode = "ADB"
        End If

        If voltemode = "ADB" And traffictype = "script-internal" And imsi <> "" Then
            displaylog("imsi:" + imsi, "g")
            phonenum = getcallnum()
            longrunvolterealue(adbdvicesn, phonenum, a_interval, "MOC")

        End If

        If voltemode = "ADB" And traffictype = "MTC" And imsi <> "" Then

            longrunvolterealue(adbdvicesn, phonenum, a_interval, "MTC")

        End If
        If voltemode = "AT" And traffictype = "MTC" And imsi <> "" Then

            'longrunvolterealue(adbdvicesn, phonenum, a_interval, "MTC")

        End If
        If voltemode = "AT" And traffictype = "MTC" And imsi <> "" Then

            'longrunvolterealue(adbdvicesn, phonenum, a_interval, "MTC")

        End If
    End Sub
    Sub longrunvolterealuepar(ByVal sn As String, ByVal phonenum As String, ByVal duration As String, ByVal interval As String, ByVal mode As String)
        Dim attachstate As Boolean = True
        Dim devicename As String
        Dim continoustime As Integer = 0
        Dim runpid As String = ""
        displaylog("long run volte commercial UE " + mode, "r")
        If mode = "MOC" Then
            runcommerialadroidcall(sn, phonenum, duration, interval)
        End If
        If mode = "MTC" Then

        End If

    End Sub
    Sub longrunvolterealue(ByVal sn As String, ByVal phonenum As String, ByVal duration As String, ByVal mode As String)
        Dim attachstate As Boolean = True
        Dim devicename As String
        Dim continoustime As Integer = 0
        Dim runpid As String = ""
        displaylog("long run volte commercial UE " + mode, "r")
        If mode = "MOC" Then
            runcommerialadroidcall(sn, phonenum, duration, interval)
            While 1
                displaylog("start wait " + interval + "s", "g")
                wait(30)
                runpid = checkoldsh(sn, "sh", "shell")
                If runpid <> "" Then
                    displaylog("Call process running:" + runpid, "g")
                End If
            End While
        End If
        If mode = "MTC" Then
            While Not exitwindow
                wait(30)
                displaylog("MTC", "g")
            End While

        End If

    End Sub
    Sub longrun(ByVal uetype As String, ByVal attachtime As Integer)

        Dim attachstate As Boolean = True
        Dim droptimes As Integer
        Dim voltemode As String = "AT"
        Dim devicename As String
        Dim continoustime As Integer = 0
        droptimes = 0
        trytimes = 0
        displaylog("long run start", "r")
        'Shell("d:\mueauto\killuesoft.bat")
        myprocess.killprocessbyimportfile("d:\mueauto\killuesoft.cfg")
        Do 'UE attach until get add route ok
            attachstate = True
            If traffictype = "MOC" Or traffictype = "MTC" Then
                uetype = "volte"
                Timer4.Enabled = True
                devicename = getandrioddevicename()
                If devicename = "realUE" Then
                    voltemode = "ADB"
                Else
                    'If devicename = "Not an andriod device" Then
                    '    displaylog("Not an androd device can not make call", "r")

                    '    Exit Sub
                    'End If
                End If
            End If
            '-------------- UE detach+attach-----------------
            Select Case uetype
                Case "H"
                    If attachdetach_HISI() <> "OK" Then
                        attachstate = False
                    End If

                Case "E5776"
                    If attachdetach_HISI() <> "OK" Then
                        attachstate = False
                    End If
                Case "volte"
                    attachdetach_Qualcomm_volte("9600")
                Case "Qualcomm9600", "YY9027", "Qualcomm9206", "Qualcomm9027", "YY9206"
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
            continoustime = continoustime + 1
            displaylog("attach try times:" + trytimes.ToString, "r")
            Dim h = 0
            Do While (h < 3) And ueinternalip = ""
                serialSendData("AT+CGCONTRDP" & vbCrLf, logname)
                displaylog("AT+CGCONTRDP", "g")
                wait(10)
                displaylog("UErealip:=" + ueinternalip, "r")

                h = h + 1
            Loop
            If addroute() = "ip not find" Then
                If ueinternalip <> "" And (uerealtype = "Sierra") Then
                    displaylog("UE attach success, but it can not assign IP to PC. For Sierra chip, try disable/enable netcard", "r")
                    disenablenetcard()

                End If
                attachstate = attachstate * False
            End If
            If (uetype = "H" Or uetype = "E5776") And attachstate = True Then
                If monitoring(True) <> "OK" Then
                    attachstate = False
                End If
            End If
            attachstate = attachstate * Not (exitwindow)
            If attachstate = False Then
                displaylog("attach fail", "r")
            End If
            If attachstate = False Then
                Randomize()
                wait(30 + Int(Rnd() * (30)))

            End If

            If continoustime > 10 Then
                displaylog("attach try over 10 times, all failed, stop attach, if Qualcomm chip will  reset 10 minutes later,and  will go on attach", "r")
                wait(600)
                reset_qualcomm()
                wait(180)
                killtraffic()
                'myprocess.killwindowbytitle(serialportname.ToUpper + ".")
                'If traffictype = "ftpdl" Or traffictype = "ftpul" Or traffictype = "ftpdlul" Then killoldftp()
                'If traffictype = "ping" Then killoldping()
                'If traffictype = "video" Then killvideo()
                'If traffictype = "volte" Then killoldvolte()
                'If traffictype = "http" Then killoldhttp()
                'If traffictype = "httpdownload" Then killoldhttpdownload()
                'exitwindow = True
                'wait(60)
                Application.Restart()
            End If
        Loop Until attachstate = True
        continoustime = 0
        runtraffic()

        '-----------run ftp or http
        'If traffictype = "ftpdl" Or traffictype = "ftpul" Or traffictype = "ftpdlul" Then
        '    rundlftp()
        'End If
        'If traffictype = "http" Then
        '    rundlhttp()
        'End If
        'If traffictype = "httpdownload" Then
        '    rundlhttpdownload()
        'End If
        'If traffictype = "ping" Then
        '    runping()
        'End If
        'If traffictype = "video" Then
        '    runvideo()
        'End If

        ''If traffictype = "volte" Then
        ''    runvolte()
        ''End If

        ''If traffictype = "MTC" Then

        ''rundlhttpdownload()
        ''End If
        ''If traffictype = "MOC" Then
        ''rundlhttpdownload()

        ''End If

        Do While exitwindow = False
            If traffictype = "MOC" Then
                runMOC(voltemode)
                GoTo flagmoc
            End If
            '-----------wait
            If traffictype <> "MOC" Then
                wait(2)
                'killoldping()
                displaylog("start wait " + interval + "s", "g")
                wait(interval)

            End If

            '-----------monitoring call drop
flagmoc:    calldroped = False
            monitoring()

            displaylog(getPrivateMemory, "r")
 
            '-----------caculate the counter
            ' realtimes = realtimes + 1
            continoustime = 0
            If calldroped = True Then
                droptimes = droptimes + 1
                displaylog("call droped times:" + droptimes.ToString, "r")
                wait(2)
                killtraffic()
                'myprocess.killwindowbytitle(serialportname.ToUpper + ".")
                'If traffictype = "ftpdl" Or traffictype = "ftpul" Or traffictype = "ftpdlul" Then killoldftp()
                'If traffictype = "ping" Then killoldping()
                'If traffictype = "video" Then killvideo()
                'If traffictype = "volte" Then killoldvolte()
                'If traffictype = "MOC" Then
                '    ' killoldping()
                'End If
                'If traffictype = "MTC" Then
                '    'killoldping()
                'End If
                attachstate = True
                Do 'UE attach until get add route ok
                    attachstate = True
                    '-------------- UE detach+attach-----------------
                    Select Case uetype
                        Case "H"
                            If attachdetach_HISI() <> "OK" Then
                                attachstate = False
                            End If

                        Case "E5776"
                            If attachdetach_HISI() <> "OK" Then
                                attachstate = False
                            End If

                        Case "Qualcomm9600", "YY9027", "Qualcomm9206", "Qualcomm9027", "YY9206"
                            'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
                            attachdetach_Qualcomm("9600")
                        Case "volte"
                            attachdetach_Qualcomm_volte("9600")
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
                    continoustime = continoustime + 1
                    displaylog("attach try times:" + trytimes.ToString, "r")
                    Dim h = 0
                    Do While (h < 3) And ueinternalip = ""
                        serialSendData("AT+CGCONTRDP" & vbCrLf, logname)
                        displaylog("AT+CGCONTRDP", "g")
                        wait(10)
                        displaylog("UErealip:=" + ueinternalip, "r")

                        h = h + 1
                    Loop
                    If addroute() = "ip not find" Then
                        If ueinternalip <> "" And (uerealtype = "Sierra") Then
                            displaylog("UE attach success, but it can not assign IP to PC. For Sierra chip, try disable/enable netcard", "r")
                            disenablenetcard()

                        End If
                        attachstate = attachstate * False
                    End If
                    If (uetype = "H" Or uetype = "E5776") And attachstate = True Then
                        If monitoring(True) <> "OK" Then
                            attachstate = False
                        End If
                    End If
                    attachstate = attachstate * Not (exitwindow)
                    If attachstate = False Then
                        displaylog("attach fail", "r")
                    End If
                    If attachstate = False Then
                        Randomize()
                        wait(30 + Int(Rnd() * (30)))

                    End If
                    If continoustime > 10 Then
                        displaylog("attach try over 10 times, all failed, stop attach, if Qualcomm chip will  reset 10 minutes later,and  will go on attach", "r")
                        wait(600)
                        reset_qualcomm()
                        wait(180)
                        killtraffic()
                        'myprocess.killwindowbytitle(serialportname.ToUpper + ".")
                        'If traffictype = "ftpdl" Or traffictype = "ftpul" Or traffictype = "ftpdlul" Then killoldftp()
                        'If traffictype = "ping" Then killoldping()
                        'If traffictype = "video" Then killvideo()
                        'If traffictype = "volte" Then killoldvolte()
                        'If traffictype = "http" Then killoldhttp()
                        'If traffictype = "httpdownload" Then killoldhttpdownload()
                        ''exitwindow = True
                        ''wait(60)
                        Application.Restart()
                    End If
                Loop Until attachstate = True
                runtraffic()
                'If traffictype = "ftpdl" Or traffictype = "ftpul" Or traffictype = "ftpdlul" Then
                '    rundlftp()
                'End If
                'If traffictype = "http" Then
                '    rundlhttp()
                'End If
                'If traffictype = "httpdownload" Then
                '    rundlhttpdownload()
                'End If
                'If traffictype = "ping" Then
                '    runping()
                'End If
                'If traffictype = "video" Then
                '    runvideo()
                'End If
                ''If traffictype = "volte" Then
                ''    runvolte()
                ''End If
                ''If traffictype = "MOC" Then
                ''    runping()
                ''    runMOC()
                ''End If
                ''If traffictype = "MTC" Then
                ''    runping()
                ''    runMTC()
                ''End If
            Else

            End If
            displaylog("drop times:" + droptimes.ToString, "r")

        Loop
    End Sub
    Sub runMOC(ByVal voltemode As String)
        'Dim count As Integer
        Dim phonenum As String = "10086"
        Dim adbdvicesn As String = ""
        'Dim times As Integer
        Dim callinteval As Integer
        If imsi <> "" Then
            displaylog("imsi:" + imsi, "g")
            phonenum = getcallnum()
            If phonenum <> "" Then
                callinteval = a_interval
                If voltemode = "AT" Then
                    serialSendData("ATD" + phonenum + ";", logname)
                    displaylog("ATD" + phonenum + ";", "g")

                    wait(callinteval)
                    serialSendData("ATH", logname)
                    displaylog("ATH", "g")
                    wait(interval - callinteval)
                Else
                    runrealvolteadb(phonenum)
                    wait(callinteval)
                    runrealvolteadbhungup()
                    wait(interval - callinteval)
                End If

            Else
                displaylog("Check target phone number in ftp.ini", "r")
                wait(interval)
            End If

        Else
            displaylog("Not find IMSI number.", "r")
            wait(interval)
        End If


    End Sub

    Sub runMTC(ByVal voltemode)

    End Sub

    Sub displaylog(ByVal addstr As String, ByVal strcolor As String, Optional ByVal logout As Boolean = True, Optional ByVal debuginfo As Boolean = False)
        locked = True
        If debuginfo = True And showdebuginfo = False Then
            Exit Sub
        End If
        If Trim(addstr) <> " " Then
            Try
                Select Case strcolor
                    Case "g"
                        '  WebBrowser1.DocumentText = WebBrowser1.DocumentText & "<br><span lang=EN-US style='font-size:9.0pt;font-family:""Microsoft Sans Serif"";color:#00CC00'>" & addstr & vbNewLine
                        ConsoleHelper.displaylog(Trim(addstr))
                        If logout = True Then writelog(addstr, 0, logname)
                        ' locked = False
                    Case "r"
                        ' WebBrowser1.DocumentText = WebBrowser1.DocumentText & "<br><span ltang=EN-US style='font-size:9.0pt;font-family:""Microsoft Sans Serif"";color:red'>" & addstr & vbNewLine
                        ConsoleHelper.displaylog(Trim(addstr), "r")
                        If logout = True Then writelog(addstr, 0, logname)
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

                'writeueiplog(Trim(addstr1), logname)
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
                'Dim a As String = ex.Message
                'Dim b As String
            End Try
        End If
    End Sub

    Function findueip() As String
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
        displaylog("Clear old route to " & serverip, "r")
        returnstr = rundoscomandt("route delete " + serverip)
        wait(1)
        displaylog("UE IP:" & ipaddress & "  Gateway:" & gateway, "r")
        UErealip = ipaddress
        Dim interfaceid As String = ""
        interfaceid = getportinterface(SerialPort.PortName)
        displaylog("net port id:" + interfaceid, "r")
        displaylog("Route Add :" + serverip + " " + gateway + " IF " + interfaceid, "g")
        If interfaceid <> "" Then
            returnstr = rundoscomandt("route add " + serverip + " " + gateway + " IF " + interfaceid)
        Else
            returnstr = rundoscomandt("route add " + serverip + " " + gateway)
        End If
    End Function

    Public Function writeueiplog(ByVal logstr As String, ByVal logname As String)
        Dim outputstr, ip As String ', fn
        'Dim writed As Boolean
        'Dim i As Integer
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


    Function findcellsierra(ByVal message As String) As String
        Dim tempstring As String
        Try
            message = Mid(message, message.IndexOf("!LTEINFO:"))
            message = VB.Left(message, message.ToString.IndexOf("IntraFreq:"))
            tempstring = Split(message, vbCrLf)(2)
            tempstring = Trim(tempstring)
            tempstring = tempstring.Replace("    ", " ")
            tempstring = tempstring.Replace("    ", " ")
            tempstring = tempstring.Replace("    ", " ")
            tempstring = tempstring.Replace("   ", " ")
            tempstring = tempstring.Replace("   ", " ")
            tempstring = tempstring.Replace("  ", " ")
            tempstring = tempstring.Replace("  ", " ")
            tempstring = tempstring.Replace("  ", " ")
            tempstring = tempstring.Replace("  ", " ")
            tempstring = Split(tempstring, " ")(4)
            Return VB.Right(tempstring, 5)
        Catch ex As Exception
            Return ""
        End Try


    End Function
    Function findcellsierra2(ByVal message As String) As String
        Dim tempstring As String
        Try
            message = Mid(message, message.IndexOf("!LTEINFO:"))
            message = VB.Left(message, message.ToString.IndexOf("IntraFreq:"))
            tempstring = Split(message, vbCrLf)(2)
            tempstring = Trim(tempstring)
            tempstring = tempstring.Replace("    ", " ")
            tempstring = tempstring.Replace("    ", " ")
            tempstring = tempstring.Replace("    ", " ")
            tempstring = tempstring.Replace("   ", " ")
            tempstring = tempstring.Replace("   ", " ")
            tempstring = tempstring.Replace("  ", " ")
            tempstring = tempstring.Replace("  ", " ")
            tempstring = tempstring.Replace("  ", " ")
            tempstring = tempstring.Replace("  ", " ")
            tempstring = Split(tempstring, " ")(4)
            Return VB.Right(tempstring, 5)
        Catch ex As Exception
            Return ""
        End Try


    End Function
    Function findpowersierra(ByVal message As String) As String
        Dim tempstring As String
        Dim rsrp As String
        Dim snr As String
        message = Mid(message, message.IndexOf("!LTEINFO:"))
        message = VB.Left(message, message.ToString.IndexOf("IntraFreq:"))
        tempstring = Split(message, vbCrLf)(2)
        tempstring = Trim(tempstring)
        tempstring = tempstring.Replace("    ", " ")
        tempstring = tempstring.Replace("    ", " ")
        tempstring = tempstring.Replace("    ", " ")
        tempstring = tempstring.Replace("   ", " ")
        tempstring = tempstring.Replace("   ", " ")
        tempstring = tempstring.Replace("  ", " ")
        tempstring = tempstring.Replace("  ", " ")
        tempstring = tempstring.Replace("  ", " ")
        tempstring = tempstring.Replace("  ", " ")
        rsrp = Split(tempstring, " ")(11)
        snr = Split(tempstring, " ")(8)
        Return "RSRPD:" + rsrp + " SINR:" + snr

    End Function
    Function findpowersierra2(ByVal message As String) As String
        Dim tempstring() As String
        Dim rsrp As String
        Dim snr As String
        Dim rsrp1, rsrp2, cellid As String
        tempstring = Split(message, vbCrLf)
        For Each line As String In tempstring
            If line.IndexOf("PCC RxM RSSI") >= 0 And line.IndexOf("RSRP (dBm)") >= 0 Then
                rsrp1 = Trim(Replace(Replace(Split(line, "RSRP (dBm):")(1), vbCrLf, ""), vbCr, ""))
                Continue For
            End If
            If line.IndexOf("PCC RxD RSSI") >= 0 And line.IndexOf("RSRP (dBm)") >= 0 Then
                rsrp2 = Trim(Replace(Replace(Split(line, "RSRP (dBm):")(1), vbCrLf, ""), vbCr, ""))
                Continue For
            End If
            If line.IndexOf("PCC RxM RSSI") >= 0 And line.IndexOf("PCC RxM RSRP:") >= 0 Then
                rsrp1 = Trim(Replace(Replace(Split(line, "PCC RxM RSRP:")(1), vbCrLf, ""), vbCr, ""))
                Continue For
            End If
            If line.IndexOf("PCC RxD RSSI") >= 0 And line.IndexOf("PCC RxD RSRP:") >= 0 Then
                rsrp2 = Trim(Replace(Replace(Split(line, "PCC RxD RSRP:")(1), vbCrLf, ""), vbCr, ""))
                Continue For
            End If

            If line.IndexOf("SINR (dB)") >= 0 Then
                snr = Trim(Replace(Replace(Split(line, "SINR (dB):")(1), vbCrLf, ""), vbCr, ""))
                Continue For
            End If
            If line.IndexOf("RSRQ (dB):") >= 0 And line.IndexOf("Cell ID:") >= 0 Then
                cellid = Trim(Replace(Replace(Split(line, "Cell ID:")(1), vbCrLf, ""), vbCr, ""))
                Continue For
            End If
        Next
        rsrp = rsrp1 + "," + rsrp2
        Return "CellD:" + cellid + "RSRPD:" + rsrp + " SINR:" + snr

    End Function
    Function findpowerhs(ByVal inputstr As String) As String

        inputstr = Mid(inputstr, inputstr.IndexOf("^HCSQ:") + 1, 24)
        Dim rsrp, sinr, real1, real2 As Integer
        rsrp = Val(Split(inputstr, ",")(2))
        sinr = Val(Split(inputstr, ",")(3))
        If rsrp = 0 Then real1 = -140

        If rsrp >= 1 And rsrp <= 97 Then real1 = rsrp - 141
        If rsrp > 97 Then real1 = -140

        If sinr = 0 Then real1 = -20
        If sinr >= 1 And sinr <= 251 Then real2 = -20 + (sinr - 1) * 0.2
        Return "RSRPD:" + real1.ToString + " SINR:" + real2.ToString

    End Function

    Function findcell5786(ByVal inputstr As String) As String
        Dim tempstr As String
        ' 1,"FFFF","1B18821",7
        Try
            tempstr = Mid(inputstr, inputstr.IndexOf("CGREG"))
            tempstr = Split(tempstr, ",")(2)
            tempstr = Split(tempstr, " ")(0)
            tempstr = Split(tempstr, vbCr)(0)
            tempstr = Replace(Replace(Replace(tempstr, """", ""), " ", ""), vbCr, "")
            tempstr = VB.Right(Trim(tempstr), 5)
            Return tempstr

        Catch ex As Exception


            Return ""
        End Try

    End Function

    Function findpower(ByVal inputstr As String) As String
        Dim tempstr As String = ""
        Dim rsrp As String = ""
        Dim sinr As String = ""

        If inputstr.IndexOf("+CMGSI: RX_Power") >= 0 Then
            tempstr = Mid(inputstr, inputstr.IndexOf("+CMGSI: RX_Power"))
            tempstr = Split(tempstr, "+")(1)
            tempstr = tempstr.Replace(vbCrLf, "")
            rsrp = (Val(Trim(Split(tempstr, ",")(6))) / 10).ToString + "," + (Val(Trim(Split(tempstr, ",")(12))) / 10).ToString
            If inputstr.IndexOf("+CMGSI: Log_Sinr10xdb") >= 0 Then
                tempstr = Mid(inputstr, inputstr.IndexOf("+CMGSI: Log_Sinr10xdb"))
                tempstr = Split(tempstr, vbCrLf)(0)
                tempstr = Split(tempstr, ",")(2)
                sinr = (Val(tempstr) / 10).ToString
            End If
            If sinr = "" Then
                Return "RSRPD:" + rsrp
            Else
                Return "RSRPD:" + rsrp + " SINR:" + sinr
            End If

        End If

        If inputstr.IndexOf("$QCSQ :") >= 0 Then
            inputstr = Mid(inputstr, inputstr.IndexOf("$QCSQ :"), 13)
            Return Mid(inputstr, 8, 5).Replace(",", "").Replace(":", "")
        End If
        Return ""
    End Function
    Function findcell(ByVal inputstr As String) As String
        Dim tempstr As String
        Dim strings As Object
        Dim i As Integer = 0
        ' 2,1,"FFFF","1B18821",7
        Try
            If inputstr.IndexOf("+CGREG: 2") >= 0 Then
                strings = Split(inputstr, vbLf)
                Do
                    i = i + 1
                Loop Until strings(i).ToString.IndexOf("+CGREG: 2") >= 0
                tempstr = strings(i)
                If UBound(Split(tempstr, ",")) >= 3 Then
                    tempstr = Mid(tempstr, tempstr.IndexOf("+CGREG: 2") + 1)
                    tempstr = Split(tempstr, ",")(3)
                    tempstr = Split(tempstr, " ")(0)
                    tempstr = Split(tempstr, vbLf)(0)
                    tempstr = Replace(Replace(tempstr, """", ""), vbCr, "")
                    tempstr = VB.Right(tempstr, 5)
                    Return tempstr
                End If

            End If
            '+CMGSI: Phy_Cellid,1,480
            If inputstr.IndexOf("+CMGSI: Phy_Cellid") >= 0 Then
                tempstr = Mid(inputstr, inputstr.IndexOf("+CMGSI: Phy_Cellid"))
                tempstr = Split(tempstr, "+")(1)
                tempstr = tempstr.Replace(vbCrLf, "")
                Return Trim(Split(tempstr, ",")(2)) + "," + Trim(Split(tempstr, ",")(1))
            End If
        Catch ex As Exception
            Return ""
        End Try

    End Function



    Function dealcellinfo(ByVal message As String) As String
        Dim cellinfo As String = ""

        If message.IndexOf("UE IP:") >= 0 And message.IndexOf("  Gateway:") >= 0 Then



            Return "UED:" + Mid(message, 7, message.IndexOf("  Gateway:") - 6)


        End If
        If message.IndexOf("UE IP:") >= 0 And message.IndexOf("  Gateway:") < 0 Then


            Return "UED:" + Mid(message, 7)


        End If

        If message.IndexOf("CGREG") >= 0 Then
            If (message.IndexOf("CGREG: 5," + """") >= 0 Or message.IndexOf("CGREG: 1," + """") >= 0) Then
                If findcell5786(message) <> "" Then
                    Return "CellD:" + findcell5786(message)

                End If

            End If
        End If


        If message.IndexOf("+CGREG: 2") >= 0 And UBound(Split(message, ",")) >= 3 Then
            If findcell(message) <> "" Then
                Return "CellD:" + findcell(message)

            End If

        End If

        If message.IndexOf("+CMGSI: Phy_Cellid") >= 0 And UBound(Split(message, ",")) >= 2 Then
            Return "CellD:" + findcell(message) + (findpower(message))


        End If


        If message.IndexOf("$QCSQ :") >= 0 And UBound(Split(message, ",")) = 4 Then
            Return "RSSID:" + findpower(message)

        End If


        If message.IndexOf("^HCSQ") >= 0 And UBound(Split(message, ",")) = 4 Then
            Return findpowerhs(message)

        End If

        If message.IndexOf("!LTEINFO:") >= 0 And message.IndexOf("IntraFreq:") >= 0 Then
            If findcellsierra(message) <> "" Then
                Return "CellD:" + findcellsierra(message) + findpowersierra(message)

            End If
        End If
        If message.IndexOf("PCC RxM RSSI") >= 0 And message.IndexOf("SINR (dB):") >= 0 Then
            cellinfo = findpowersierra2(message)
            If cellinfo <> "" Then
                Return cellinfo
            End If
        End If

        Return message
    End Function
    Function findueinternalip(ByVal logstr As String) As String
        If logstr.IndexOf("CGCONTRDP") >= 0 Then
            logstr = Replace(Replace(logstr, vbCrLf, ""), vbCr, "")
            Dim myRegex As New Regex("(\d{1,3}\.){3}\d{1,3}") '指定其正则验证式
            Dim a As Match = myRegex.Match(logstr) '从指定内容中匹配字符串 
            Return a.Value

        End If


    End Function

    Public Function writelog(ByVal logstr As String, ByVal Start As Boolean, ByVal logname As String)
        Dim outputstr, ip As String ', fn, 
        Dim writed As Boolean
        Dim i As Integer
        Dim messagelist As Object
        ip = UEname
        If getPrivateMemory() > 200000 And restartingflag = False Then
            ConsoleHelper.displaylog("Memory occupy is over 100M, restart program", "r")
            restartingflag = True
            outputstr = "~OP|" + ip + "|" + Now.ToString("MM/dd/yyyy   HH:mm:ss") + "|" + "Memory occupy is over 100M, restart program" + vbCrLf
            TCPwrite(outputstr)
            Application.Restart()

        End If

        'If logstr.IndexOf("460") > 0 Then
        '    Dim a = 1
        'End If
        If imsi = "" Then
            findimsi(logstr)
        End If
        ' ueinternalip = findueinternalip(logstr)
        If Start = True Then
            outputstr = "~OP|" + ip + "|new " + vbCrLf
            TCPwrite(outputstr)
            'If MyClient Is Nothing Then
            'Else
            '    If MyClient.Connected = True Then MyClient.SendData(Encoding.ASCII.GetBytes(outputstr))
            'End If
        Else

            logstr = dealcellinfo(logstr)
            messagelist = Split(logstr, vbCrLf)
            For Each Message As String In messagelist
                If Trim(Replace(Message, vbCr, "")) <> "" Then
                    outputstr = "~OP|" + ip + "|" + Now.ToString("MM/dd/yyyy   HH:mm:ss") + "|" + Message + vbCrLf
                    TCPwrite(outputstr)
                    ' writetploglocal(outputstr, False, "d:\locallog.txt")
                End If
            Next



            'If MyClient Is Nothing Then
            'Else
            '    If MyClient.Connected = True Then MyClient.SendData(Encoding.ASCII.GetBytes(outputstr))
            'End If
        End If

        If logstr.IndexOf("RING") >= 0 And action <> "VOLTEvoiceMTC" Then
            serialSendData("ATA" & vbCrLf, logname)
        Else
            If logstr.IndexOf("RING") >= 0 And action = "VOLTEvoiceMTC" Then
                isring = True

            End If

        End If

        If logstr.IndexOf("NO CARRIER") >= 0 And action = "VOLTEvoiceMTC" Then
            isoff = True
        Else

            If logstr.IndexOf("NO CARRIER") >= 0 And action = "VOLTEvoiceMOC" Then
                callfail = True
            End If

        End If

        If logstr.IndexOf("Sierra") >= 0 Then
            uerealtype = "Sierra"
        End If

        If waitadbsn = True Then
            adbdvicesn = checkadbsn(logstr)
            If adbdvicesn <> "" Then
                waitadbsn = False
            End If
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

    Function checkadbsn(ByVal input As String) As String
        Dim snfirst As New Regex("(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z]{4,14}")
        Dim commands As New Regex("[a-zA-Z]{4,14}")
        Dim imsi As New Regex("[0-9]{15}")
        'Console.WriteLine("check sn info input:" + input)
        'Console.WriteLine("sn first resulte:" + snfirst.IsMatch(input).ToString)
        'Console.WriteLine("commands resulte:" + commands.IsMatch(input).ToString)
        'Console.WriteLine("imsi resulte:" + imsi.IsMatch(input).ToString)
        If snfirst.IsMatch(input) And Not (commands.IsMatch(input)) And Not (imsi.IsMatch(input)) Then
            Return snfirst.Matches(input)(0).ToString
        Else
            Return ""
        End If

    End Function


    Public Function writeTPlog(ByVal logstr As String, ByVal Start As Boolean, ByVal logname As String, ByVal second As Boolean)
        Dim outputstr, ip As String 'fn
        'Dim writed As Boolean
        'Dim i As Integer
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
            'writetploglocal(outputstr, Start, logname)
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
                If tcperror = "1" Then
                    Console.WriteLine("Try connect to log server in reconnect mode fail:" + errors.Message.ToString)
                End If
            End Try

        End If
        If initial = True Then
            While MyClient.Connected = False
                Try
                    If i = 0 Then
                        a = Console.CursorLeft
                        Console.Write("?") '("Try connect to log server:" + logip)
                        If tcperror = "1" Then
                            Console.WriteLine("Try connect to log server:" + logip)
                        End If
                        Console.CursorLeft = a
                    Else
                        a = Console.CursorLeft
                        Console.Write("?") '("Try reconnect to log server:" + logip)
                        If tcperror = "1" Then
                            Console.WriteLine("Try connect to log server:" + logip)
                        End If
                        Console.CursorLeft = a
                    End If
                    Thread.Sleep(1000)
                    'MyClient.Close()
                    Application.DoEvents()
                    MyClient = New TCPClient(logip, Str(port), 0, 500)

                    'MyClient.ConnectHost()

                Catch ex As Exception
                    If tcperror = "1" Then
                        Console.WriteLine("Try connect to log server:" + logip)

                        Console.WriteLine("Try connect to log server in reconnect mode fail:" + ex.Message.ToString)

                    End If
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
                If tcperror = "1" Then
                    Console.WriteLine("Try connect to log server:" + logip)

                    Console.WriteLine("Try connect to log server in reconnect mode fail:" + ex2.Message.ToString)

                End If
            End Try
        End If
        If MyClient.Connected = True Then
            Console.Write("o") '("Conneced!")
            ' Timer1.Enabled = True
        End If
    End Sub




    'Private Sub Timer1_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles Timer1.Tick
    '    'Dim output As String
    '    'If MyClient Is Nothing Then
    '    '    MyClientreconnect(False)
    '    '    Exit Sub
    '    'End If
    '    'If MyClient.Connected = False Then
    '    '    MyClientreconnect(False)
    '    '    Exit Sub
    '    'End If

    'End Sub
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
        'writetploglocal("check0", True, TPlogname)

        '****************debug*******************
        'If 1 = 0 Then
        '***************************************
        If UEtype <> "Andriod" Then
            Try
                DLthroughputlast = Val(DLthroughput.NextValue.ToString)
                ULthroughputlast = Val(ULthroughput.NextValue.ToString)
                'writetploglocal(DLthroughput.NextValue.ToString, True, TPlogname)
                'writetploglocal(ULthroughput.NextValue.ToString, True, TPlogname)
            Catch ex As Exception
                displaylog("getTP error!", "r", False)
            End Try
            'writetploglocal("check1", True, TPlogname)
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
                    DLtpcounter.Add(DLthroughputhis)
                    If DLtpcounter.Count > interval / 2 Then
                        DLtpcounter.RemoveAt(0)
                    End If
                    displayTPlog(output, "r", False)
                End If
                TPintervalcounter = TPintervalcounter + 1

            End If
            'If TPintervalcountersecond > (TPintervalsecond) Then TPintervalcountersecond = 1

            'If TPintervalcountersecond = 1 Then
            '    DLthroughputsecond = 0
            '    ULthroughputsecond = 0
            'End If

            ''If TPintervalcountersecond <= TPintervalsecond Then
            'DLthroughputsecond = DLthroughputsecond + DLthroughputlast
            'ULthroughputsecond = ULthroughputsecond + ULthroughputlast
            'If TPintervalcountersecond = (TPintervalsecond) Then
            'displaylog("netcard device name :" + netcardname, "r")
            output = formateTP((DLthroughputlast * 8).ToString, (ULthroughputlast * 8).ToString)
            displayTPlog(output, "r", True)
            'End If
            'TPintervalcountersecond = TPintervalcountersecond + 1

            'End If
            'End If
        Else
            Dim returnstr As String

            returnstr = andriodcommand(serialportname, "TP")
            'displaylog(returnstr, "g")


        End If


    End Sub

    Private Sub Timer3_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer3.Tick
        If UEtype = "Andriod" Then
            Dim returnstr As String

            returnstr = Trim(andriodcommand(serialportname, "statue"))
            If returnstr <> "" Then
                displaylog(returnstr, "g")
            End If

        End If

    End Sub

    Private Sub Timer4_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer4.Tick
        Dim nowtime As DateTime
        Select Case UEtype

            Case "E5776"
                serialSendData("AT+GMM" & vbCrLf, logname)
                serialSendData("AT^HCSQ" & vbCrLf, logname)
                wait(1)
                serialSendData("AT+CGREG?" & vbCrLf, logname)
                wait(1)

            Case "Qualcomm9600"
                If uerealtype = "Sierra" Then
                    serialSendData("AT!GSTATUS?" & vbCrLf, logname)
                    serialSendData("AT+CIMI" & vbCrLf, logname)
                    'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)

                    serialSendData("AT+GMM" & vbCrLf, logname)
                    'wait(1)




                Else
                    serialSendData("AT+CIMI" & vbCrLf, logname)
                    'subprocess.Popen("C:\Program Files\Qualcomm\QMICM\ConnectionManager.exe", Shell() = True)
                    serialSendData("AT+CGREG?" & vbCrLf, logname)
                    'wait(1)

                    'wait(1)

                    'wait(1)
                    serialSendData("AT+GMM" & vbCrLf, logname)
                    'wait(1)
                    serialSendData("AT+CMGSI=4" & vbCrLf, logname)
                    serialSendData("AT$QCSQ" & vbCrLf, logname)

                    serialSendData("AT!lteinfo" & vbCrLf, logname)
                End If

            Case "YY9027"
                serialSendData("AT+GMM" & vbCrLf, logname)
                serialSendData("AT+CIMI" & vbCrLf, logname)
                wait(1)
                serialSendData("AT+CGREG=2" & vbCrLf, logname)
                serialSendData("AT+CGREG?" & vbCrLf, logname)
                wait(1)
                serialSendData("AT$QCSQ" & vbCrLf, logname)
            Case "Qualcomm9028"
                serialSendData("AT+GMM" & vbCrLf, logname)
                serialSendData("AT+CGREG?" & vbCrLf, logname)
                wait(1)
                serialSendData("AT$QCSQ" & vbCrLf, logname)
                wait(1)
                serialSendData("AT+CMGSI=4" & vbCrLf, logname)
            Case "Andriod"
                If flagtime4 = 1 Then
                    andriodcommand(serialportname, "IP")

                    flagtime4 = 2
                Else
                    andriodcommand(serialportname, "RSRP")
                    flagtime4 = 1
                End If
                'displaylog(returnstr, "g")

                'displaylog(returnstr, "g")
                'IOlock = False
                'cell info


        End Select
        Select Case uerealtype

            Case "Sierra"

                If DateDiff(DateInterval.Hour, appstarttime, Now) < 0 Then
                    nowtime = DateAdd(DateInterval.Hour, 24, Now)
                Else
                    nowtime = Now
                End If
                If DateDiff(DateInterval.Hour, appstarttime, nowtime) >= sierraresettime And ueresetflag = "1" Then
                    Timer4.Enabled = False
                    Timer2.Enabled = False
                    displaylog("Resetting UE,will restart late", "r")
                    reset_qualcomm()
                    wait(180)
                    myprocess.killwindowbytitle(serialportname.ToUpper + ".")
                    If traffictype = "ftpdl" Or traffictype = "ftpul" Or traffictype = "ftpdlul" Then killoldftp()
                    If traffictype = "ping" Then killoldping()
                    If traffictype = "video" Then killvideo()
                    If traffictype = "volte" Then killoldvolte()
                    If traffictype = "http" Then killoldhttp()
                    If traffictype = "httpdownload" Then killoldhttpdownload()
                    'exitwindow = True
                    'wait(60)
                    Application.Restart()
                End If
        End Select

    End Sub

    Function getandrioddevicename() As String
        Dim devicename As String
        adbdvicesn = getadbdevice()
        If adbdvicesn <> "" Then
            displaylog("andriod sn:" + adbdvicesn, "g")
            devicename = rundoscomandt("d:\mueauto\adb -s " + adbdvicesn + " shell getprop ro.product.device")
            If devicename.IndexOf("msm") >= 0 Or devicename.IndexOf("hero2qltechn") >= 0 Then
                getadbimsi()
                Return "realUE"
            Else
                If devicename.IndexOf("device not found") >= 0 Then
                    Return "Not andriod"
                Else
                    getadbimsi()
                    Return "others datacard"
                End If

            End If

        Else
            Return "Not andriod"
        End If

    End Function
    Sub getadbimsi()
        Dim tempimsi As String
        tempimsi = rundoscomandt("d:\mueauto\adb -s " + adbdvicesn + " shell service call iphonesubinfo 8")
        If UBound(Split(tempimsi, "'")) > 5 Then
            imsi = Split(tempimsi, "'")(1) + Split(tempimsi, "'")(3) + Split(tempimsi, "'")(5)
            imsi = Trim(Replace(imsi, ".", ""))
            displaylog("IMSI:" + imsi, "g")
        Else
            displaylog("IMSI not find", "r")
            imsi = "12345678901"
        End If
    End Sub
    Public Function GetInstanceName(ByVal categoryName As String, ByVal counterName As String, ByVal p As Process) As String

        Try

            Dim processcounter As New PerformanceCounterCategory(categoryName)
            Dim instances As String() = processcounter.GetInstanceNames()
            For Each instance As String In instances

                Dim counter As PerformanceCounter = New PerformanceCounter(categoryName, counterName, instance)
                'Logger.Info("对比in mothod GetInstanceName，" + counter.NextValue() + "：" + p.Id);
                If counter.NextValue() = p.Id Then

                    Return instance
                End If

            Next

        Catch ex As Exception

            Return Nothing
        End Try

    End Function

    Public Function getPrivateMemory() As String
        Dim result As Long = 0

        result = Int(Process.GetCurrentProcess().PrivateMemorySize64 / 1024)
        'Console.WriteLine(result)
        'Console.WriteLine(Int(Process.GetCurrentProcess().WorkingSet64 / 1024))
        'Console.WriteLine(Int(Process.GetCurrentProcess().VirtualMemorySize64 / 1024))
        Return result.ToString

    End Function

    Function andriodcommand(ByVal sn As String, ByVal command As String) As String
        Dim sendcommand As String = ""
        Dim returnstr As String = ""
        ' rundoscomandt("d:\mueauto\autocall\adb.exe| -s " + sn + " shell" + "|" + "sh /data/local/tmp/Call.sh &|" + Chr(3), 4, False))

        Select Case command

            Case "IMSI"
                sendcommand = "adb -s " + sn + " shell ""service call iphonesubinfo 9 |tail -n 3|cut -d ""\'"" -f 2"""
            Case "RSRP"
                sendcommand = "adb -s " + sn + " shell ""dumpsys telephony.registry|grep -i signalstrength"""
            Case "statue"
                sendcommand = "adb -s " + sn + " shell ""dumpsys telephony.registry|grep -E 'mRing|mCallState|mCallIn|mDataConnectionS'"""
            Case "TP"
                sendcommand = "adb -s " + sn + " shell tail /sdcard/tmp/tp.txt"
            Case "stop"
                sendcommand = "adb -s " + sn + " shell ""echo 'stop'\\c > /sdcard/tmp/statues.txt"""
            Case "run"
                sendcommand = "adb -s " + sn + " shell ""echo 'run'\\c > /sdcard/tmp/statues.txt"""
            Case "reboot"
                sendcommand = "adb -s " + sn + " shell reboot"
            Case "IP"
                sendcommand = "adb -s " + sn + " shell ""ifconfig|grep 'inet addr'|cut -d ':' -f 2"""

            Case Else




        End Select


        If sendcommand <> "" Then
            'If adbinputmessagepool.Count = adbinputpoolsize Then adbinputmessagepool.RemoveAt(0)

            'adbinputmessagepool.Add(sendcommand)
            displaylog(sendcommand, "g", True, True)
            returnstr = rundoscomandt2(sendcommand, command, 0, False)
            'returnstr = Decoderandrodmessage(returnstr, command)
        End If
        Return " "
    End Function


    Function Decoderandrodmessage(ByVal message As String, ByVal type As String) As String

        Dim messagerealinfo As String
        Dim returnstr As String = ""
        Dim statues As Object
        Dim tempstr As String
        Dim messages As Object
        If message.IndexOf("d:\mueauto\autocall>") > 0 Then
            messagerealinfo = Split(message, "d:\mueauto\autocall>")(1)
            If messagerealinfo.IndexOf(vbCrLf) >= 0 Then

                If Trim(Replace(Mid(messagerealinfo, messagerealinfo.IndexOf(vbCrLf) + 1), vbCrLf, "")) <> "" Then
                    Select Case type

                        Case "IMSI"
                            messagerealinfo = Replace(Mid(messagerealinfo, messagerealinfo.IndexOf(vbCrLf) + 1), vbCrLf, "")
                            returnstr = "IMSI:" + Replace(messagerealinfo, ".", "")
                        Case "RSRP"
                            messagerealinfo = Replace(Mid(messagerealinfo, messagerealinfo.IndexOf(vbCrLf) + 1), vbCrLf, "")
                            messagerealinfo = Trim(messagerealinfo)

                            returnstr = "RSRP:" + messagerealinfo.Split(" ")(9)

                        Case "statue"
                            statues = Split(Trim(messagerealinfo), vbCrLf)
                            For Each statue As String In statues
                                statue = Trim(statue)
                                If statue.IndexOf("mCallState=") >= 0 Then
                                    tempstr = statue.Split("=")(1)
                                    tempstr = Trim(tempstr)
                                    If tempstr <> mCallState Then
                                        If tempstr = "0" Then mCallState = tempstr
                                        'If tempstr = "1" Then
                                        '    mCallState = tempstr
                                        '    returnstr = returnstr + "Paginged" + vbCrLf
                                        'End If
                                        If tempstr = 2 Then
                                            mCallState = tempstr
                                            returnstr = returnstr + "Calling" + vbCrLf
                                        End If
                                    End If

                                End If
                                If statue.IndexOf("mCallIncomingNumber=") >= 0 Then
                                    tempstr = statue.Split("=")(1)
                                    tempstr = Trim(tempstr)
                                    If tempstr <> "" Then
                                        returnstr = returnstr + "IncomingNum:" + tempstr + vbCrLf
                                    End If

                                End If
                                If statue.IndexOf("mDataConnectionState=") >= 0 Then
                                    tempstr = statue.Split("=")(1)
                                    tempstr = Trim(tempstr)
                                    If tempstr <> mDataConnectionState Then
                                        mDataConnectionState = tempstr
                                        If tempstr = "-1" Or tempstr = "0" Then
                                            returnstr = returnstr + "Data deactived" + vbCrLf
                                        End If
                                        If tempstr = "2" Then
                                            returnstr = returnstr + "Data actived" + vbCrLf
                                        End If
                                    End If

                                End If
                                If statue.IndexOf("mRingCallState=") >= 0 Then
                                    tempstr = statue.Split("=")(1)
                                    tempstr = Trim(tempstr)
                                    If tempstr <> mRingCallState Then
                                        mRingCallState = tempstr
                                        If tempstr = "5" Then
                                            returnstr = returnstr + "InComingCall" + vbCrLf
                                        End If

                                    End If

                                End If
                            Next

                            'mDataConnectionState=-1 没有接入，0 有attached 但用wifi，2 使用中
                            'mCallState=0 没电话，2通话中，1 呼叫中
                            'mCallIncomingNumber =, 呼叫进来的电话
                            'mRingingCallState =5 被叫振铃

                        Case "TP"
                            messagerealinfo = Replace(Mid(messagerealinfo, messagerealinfo.IndexOf(vbCrLf) + 1), vbCrLf, "")
                            If messagerealinfo <> "" Then
                                messages = Split(messagerealinfo, " ", 2)
                                If mlasttime <> messages(0) Then
                                    mlasttime = messages(0)
                                    returnstr = "Throughput:" + messages(1)
                                Else
                                    returnstr = "Throughput: 0 B/s 0 B/s"
                                End If
                                Dim dlcounter, ulcounter As String
                                Dim dtp, utp As Long
                                Dim dlbase, ulbase As String
                                ulcounter = Split(returnstr, "/s")(0)
                                ulcounter = Trim(Split(ulcounter, ":")(1))
                                ulbase = ulcounter.Split(" ")(1)
                                ulcounter = ulcounter.Split(" ")(0)
                                Select Case ulbase
                                    Case "B"
                                        utp = Int(Val(ulcounter)) * 8
                                    Case "KB"
                                        utp = Int(Val(ulcounter)) * 8 * 1024
                                    Case "MB"
                                        utp = Int(Val(ulcounter)) * 8 * 1024 * 1024
                                End Select

                                dlcounter = Split(returnstr, "/s")(1)
                                dlcounter = Trim(dlcounter)
                                dlbase = dlcounter.Split(" ")(1)
                                dlcounter = dlcounter.Split(" ")(0)
                                Select Case dlbase
                                    Case "B"
                                        dtp = Int(Val(dlcounter)) * 8
                                    Case "KB"
                                        dtp = Int(Val(dlcounter)) * 8 * 1024
                                    Case "MB"
                                        dtp = Int(Val(dlcounter)) * 8 * 1024 * 1024
                                End Select
                                Dim output As String
                                output = formateTP(dtp.ToString, utp.ToString)
                                displayTPlog(output, "r", True)
                                displaylog(output, "g")

                                'displayTPlog(, "r", True)


                                '
                                'CHECK TIME TO COMFIRM THROUGHPUT
                            End If

                        Case "stop"

                        Case "run"

                        Case "reboot"
                        Case "IP"
                            'returnstr = "IP:" + Trim(Replace(Replace(Replace(messagerealinfo, "127.0.0.1  Mask", ""), "Mask", ""), vbCrLf, ""))
                            statues = Split(Trim(messagerealinfo), vbCrLf)
                            For Each IP As String In statues
                                IP = Trim(IP)
                                If IP.IndexOf("Mask") >= 0 And IP.IndexOf("127.0.0.1") < 0 Then
                                    IP = Trim(IP.Replace("Mask", ""))
                                    returnstr = "IP:" + IP
                                    Exit Select
                                End If
                                    Next
                            returnstr = "IP:"
                        Case Else




                    End Select
                End If

            End If

        End If
        Return returnstr

    End Function

End Class

