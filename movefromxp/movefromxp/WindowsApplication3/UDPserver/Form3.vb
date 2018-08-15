Imports System
Imports System.Net.Sockets
Imports System.Net
Imports UDPserver


Public Class Form3
    Private listener As System.Net.Sockets.TcpListener
    Private listenThread As System.Threading.Thread
    Dim udptrhread As System.Threading.Thread
    Dim closesserver As Boolean = False
    Dim Hwmsgpool As New List(Of String)
    Dim sessionlist As New Dictionary(Of String, String())
    Dim paginglist As New Dictionary(Of String, String())
    Dim threadlist As New Dictionary(Of String, Threading.Thread)

    '--------------------------------------------------------------------
    Public Delegate Sub printinf(ByVal input As String)
    Public Sub writelog(ByVal inputstr As String)
        TextBox1.Text = TextBox1.Text & vbCrLf & Now.ToString("HH:mm:ss") + ">" + inputstr
    End Sub



    Private Sub Form3_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        udptrhread.Abort()
        closesserver = True
        Button1.Text = "Connect"
        closesserver = True
        TextBox1.Text = TextBox1.Text + "Server stoped" + vbCrLf
        Closeallsessions()

    End Sub

    '--------------------------------------------------------------------
    Private Sub Form3_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Button1_Click(Nothing, Nothing)

    End Sub
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        If Button1.Text = "Connect" Then

            runTCPserver()
            Button1.Text = "Disconnect"
            Dim thr As New Threading.Thread(AddressOf clearuselessprocessthread)
            thr.Name = "Moniter useless traffic"
            thr.IsBackground = True
            thr.Start()
            Dim thr2 As New Threading.Thread(AddressOf processmsg)
            thr2.Name = "Msg process"
            thr2.IsBackground = True
            thr2.Start()
            startUDPserverthread()
        Else

            Button1.Text = "Connect"
            listenThread.Abort()
            listener.Stop()
            TextBox1.Text = TextBox1.Text + "Server stoped" + vbCrLf
        End If



    End Sub






    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged
        TextBox1.SelectionStart = Len(TextBox1.Text)
        TextBox1.ScrollToCaret()
    End Sub



    Private Sub TextBox2_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBox2.KeyPress
        Dim KeyAscii As Short = Asc(e.KeyChar)
        Try
            If TextBox2.Tag = "" Then TextBox2.Tag = TextBox2.Text
            If KeyAscii = 27 Then TextBox2.Text = TextBox2.Tag
            If InStr("1234567890", Chr(KeyAscii)) <= 0 And KeyAscii <> 8 Then KeyAscii = 0
            If KeyAscii = 13 Then TextBox2.Tag = TextBox2.Text
            e.KeyChar = Chr(KeyAscii)
            If KeyAscii = 0 Then
                e.Handled = True
            End If
        Catch
            TextBox2.Text = TextBox2.Tag

        End Try
    End Sub

    Sub runTCPserver()
        Invoke(printlog, "start TCP server listening on port " + TextBox2.Text)
        listener = New System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, Val(TextBox2.Text)) 'The TcpListener will listen for incoming connections at port 43001
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
                incomingClient = listener.AcceptTcpClient  'Accept the incoming connection. This is a blocking method so execution will halt here until someone tries to connect.
                'TextBox1.Text = TextBox1.Text & "Connected!" & vbCrLf
                'Invoke(printlog, "Connected")
                If closesserver = True Then
                    Console.WriteLine("tcpserver now exit")
                    Exit Sub

                End If

                Dim thr = New System.Threading.Thread(AddressOf recmsg)
                thr.IsBackground = True
                thr.Start(incomingClient)

            Catch e1 As System.IO.IOException
                'Invoke(printlog, "Client disconnected")
                ''Finally
                ''   listener.Stop()
            Catch ex As Exception
                Exit Sub
            End Try
        Loop

    End Sub

    Sub reordermsg(ByRef hwmsgpool As List(Of String))
        Dim iplist As New List(Of String)
        Dim output As New List(Of String)
        For i = 0 To hwmsgpool.Count - 1
            iplist.Add(Split(hwmsgpool(i), "|")(1))
        Next
        iplist.Sort()
        For i = 0 To iplist.Count - 1
            For j = 0 To hwmsgpool.Count - 1
                If hwmsgpool(j).IndexOf(iplist(i)) >= 0 Then
                    output.Add(hwmsgpool(j))
                End If
            Next
        Next
        hwmsgpool = output
    End Sub
    Dim printlog As New printinf(AddressOf writelog) '定义数据显示委托实例
    Sub recmsg(ByVal sokconnectionarn As Net.Sockets.TcpClient)
        Dim bytes(1024) As Byte
        Dim data As String = Nothing
        Dim stream As System.Net.Sockets.NetworkStream = sokconnectionarn.GetStream()

        Dim i As Int32
        i = 0
        ' Loop to receive all the data sent by the client.
        Try
            If sokconnectionarn.Connected = True Then
                Invoke(printlog, sokconnectionarn.Client.RemoteEndPoint.ToString + " Connected")
                updateconnecttime(sokconnectionarn.Client.RemoteEndPoint.ToString)
                data = vbCrLf + ">"
                Dim msg0 As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
                stream.Write(msg0, 0, msg0.Length)
                'Invoke(printlog, "Send:" + data)
                i = stream.Read(bytes, 0, bytes.Length)
            End If

            While (i <> 0)

                If closesserver = True Then
                    Invoke(printlog, "tcp server now closed")
                    Exit Sub
                End If

                ' Translate data bytes to a ASCII string.
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i)
                If data.IndexOf(Chr(13) & Chr(10)) > 0 Then data = Microsoft.VisualBasic.Left(data, data.Length - 2)
                If Replace(Trim(data), vbCrLf, "") <> "" Then
                    Hwmsgpool.Add(data)
                    Invoke(printlog, data)
                    ' Process the data sent by the client.
                    data = vbCrLf + ">"
                    Dim msg9 As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
                    stream.Write(msg9, 0, msg9.Length)
                End If
                If sokconnectionarn.Connected = True Then
                    i = stream.Read(bytes, 0, bytes.Length)
                Else
                    i = 0
                End If
            End While
        Catch ex As Exception
            Invoke(printlog, sokconnectionarn.Client.RemoteEndPoint.ToString + " disconnected:" + ex.ToString)
            Exit Sub
            'Catch ex2 As Exception
            '    Invoke(printlog, sokconnectionarn.Client.RemoteEndPoint.ToString + " error:" + ex2.ToString)
            '    Exit Sub
        End Try
        Invoke(printlog, sokconnectionarn.Client.RemoteEndPoint.ToString + " disconnected")
        sokconnectionarn.Close()

    End Sub

    Sub processmsg()
        Dim command As String = ""
        Invoke(printlog, "msg process started")
        Do While 1
            Do While Hwmsgpool.Count <> 0
                command = Hwmsgpool.Item(0)
                Invoke(printlog, "process msg:" + command)
                runnewsession(command)
                Hwmsgpool.RemoveAt(0)
            Loop
            Threading.Thread.Sleep(1000)
        Loop


    End Sub
    Function getip(ByVal command As String) As String
        Dim splitedstr As String()
        splitedstr = Split(command, "|")
        Return splitedstr(0)
    End Function
    Sub runnewsession(ByVal command As String)
        'command format   192.168.1.1:3456|h
        'command format   192.168.1.1:3456|p|2000
        'command format   192.168.1.1:2345|c|[200,1000,99999][7000,1000,50][speed,packetsize,duration]
        'session listformat key=ip:port value=|thread id|datetime|process id
        'check command format
        Select Case checkformat(command)
            Case "h"
                updateconnecttime(getip(command))

            Case "c"
                killoldthread(getip(command))
                Dim tr As New Threading.Thread(AddressOf runnew)
                tr.Name = getip(command)
                tr.Start(command)
                updateconnectthreadid(getip(command), tr, command, "c")
            Case "p"
                killoldthread(getip(command))
                Dim tr As New Threading.Thread(AddressOf runpagingsession)
                tr.Name = getip(command)
                tr.Start(command)
                updateconnectthreadid(getip(command), tr, command, "p")
        End Select

    End Sub
    Sub runpagingsession(ByVal Command)
        Dim ipstr As String = ""
        Dim commandlist As String()
        Dim commandsnum As Integer = 0
        Dim interval As Integer = 0
        ipstr = getip(Command)
        killoldprocess(ipstr)

        interval = Val(Split(Command, "|")(2)) * 1000
        Do While 1
            
            killoldprocess(ipstr)
            If runpaging(ipstr) = True Then
                If paginglist.ContainsKey(ipstr) Then
                    paginglist(ipstr)(0) = (Int(Val(paginglist(ipstr)(0))) + 1).ToString
                Else
                    Dim pvalue(2) As String
                    pvalue(0) = "0"
                    pvalue(1) = "0"
                    paginglist.Add(ipstr, pvalue)
                    paginglist(ipstr)(0) = (Int(Val(paginglist(ipstr)(0))) + 1).ToString
                End If

            End If
                paginglist(ipstr)(1) = (Int(Val(paginglist(ipstr)(1))) + 1).ToString
                Invoke(printlog, "Paging " + ipstr + " times:" + paginglist(ipstr)(1) + "|success times:" + paginglist(ipstr)(0) + " successful rate:" + Int(Val(paginglist(ipstr)(0)) * 100 / Val(paginglist(ipstr)(1))).ToString)
                Threading.Thread.Sleep(interval)

        Loop


    End Sub
    Dim result As String = ""
    Dim t As Threading.Thread
    Function rundoscomandt(ByVal commands As String, Optional ByVal timeouts As Integer = 0, Optional ByVal normal As Boolean = True) As String
        Dim bkprocess As Process
        result = ""
        'dosaction = commands
        If Not t Is Nothing Then
            If t.IsAlive = True Then t.Abort()
        End If
        If normal = True Then
            t = New Threading.Thread(AddressOf rundoscommand) '创建线程，使它指向test过程，注意该过程不能带有参数
        Else
            t = New Threading.Thread(AddressOf rundoscommandmoniter)
        End If

        t.IsBackground = True
        t.Start(commands) '启动线程
        If timeouts = 0 Then
            While result = ""
                Application.DoEvents()
                Threading.Thread.Sleep(100)
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
            While DateDiff(DateInterval.Second, starttime, endtime) < s 'And exitwindow = False
                Threading.Thread.Sleep(30)
                Windows.Forms.Application.DoEvents()
                If DateDiff(DateInterval.Second, temptime, endtime) Then
                    ' If exitwindow <> True Then Windows.Forms.Application.DoEvents()
                    temptime = endtime
                    Console.ForegroundColor = ConsoleColor.Green
                    a = Console.CursorLeft
                    Console.Write((s - DateDiff(DateInterval.Second, starttime, endtime)).ToString + "  ")
                    Console.CursorLeft = a
                End If
                endtime = DateTime.Now


            End While

            Console.WriteLine(" ")
        Catch
        End Try

        ' If exitwindow = True Then End
    End Sub

    Dim backprogress As Integer
    Private Sub rundoscommand(ByVal commands As String)
        Dim command() As String
        command = Split(commands, "|")
        Dim myProcess As Process = New Process()
        'Dim s As String
        myProcess.StartInfo.FileName = "cmd.exe"
        'myProcess.StartInfo.WorkingDirectory = "d:\mueauto\autocall"
        myProcess.StartInfo.UseShellExecute = False
        myProcess.StartInfo.CreateNoWindow = True
        myProcess.StartInfo.RedirectStandardInput = True
        myProcess.StartInfo.RedirectStandardOutput = True
        myProcess.StartInfo.RedirectStandardError = True
        myProcess.Start()
        backprogress = myProcess.Id
        Dim sIn As IO.StreamWriter = myProcess.StandardInput
        sIn.AutoFlush = True
        Dim sOut As IO.StreamReader = myProcess.StandardOutput
        Dim sErr As IO.StreamReader = myProcess.StandardError

        For Each cmd As String In command
            Try
                sIn.Write(cmd & System.Environment.NewLine)
            Catch ex As Exception
                Dim a = 1
            End Try

            Threading.Thread.Sleep(100)
        Next

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
    End Sub
    Private Sub rundoscommandmoniter(ByVal commands As String)
        Dim command() As String
        command = Split(commands, "|")
        Dim myProcess As Process = New Process()
        'Dim s As String
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
        Dim sIn As IO.StreamWriter = myProcess.StandardInput
        sIn.AutoFlush = True
        Dim sOut As IO.StreamReader = myProcess.StandardOutput
        Dim sErr As IO.StreamReader = myProcess.StandardError

        For Each cmd As String In command
            If cmd <> command(0) And cmd <> command(1) Then
                Try
                    sIn.Write(cmd & System.Environment.NewLine)
                Catch ex As Exception
                    Dim a = 1
                End Try

                Threading.Thread.Sleep(100)
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
    End Sub

    Function runpaging(ByVal paramtere As String) As Boolean
        Dim serverip As String = ""
        Dim returnstr As String = ""
        Dim values As String = ""

        serverip = Split(paramtere, ":")(0)
        returnstr = rundoscomandt("ping -w 10000 -n " + "5" + " " + serverip)

        If InStr(returnstr, "unreachable") > 0 Then
            values = "100" '= ping fail
        Else
            Dim expression As New System.Text.RegularExpressions.Regex("(\d+)%")
            'TextBox1.Text = TextBox1.Text & returnstr & vbNewLine
            Dim mc As System.Text.RegularExpressions.MatchCollection = expression.Matches(returnstr)
            For i As Integer = 0 To mc.Count - 1
                values = mc(i).ToString.Split("%")(0)
                'Invoke("ping " & mc(i).ToString, "r")

            Next
        End If

        If values = "" Then
            Return False


        ElseIf Int(values) = 100 Then
            Return False
        Else
            Return True

        End If
    End Function



    Sub killoldthread(ByVal ipstr As String)
        Dim killedthread As Threading.Thread
        If threadlist.ContainsKey(ipstr) Then

            killedthread = threadlist(ipstr)
            killedthread.Abort()
            threadlist.Remove(ipstr)

        End If
    End Sub

    Sub killoldprocess(ByVal ipstr As String)
        Dim killid As Integer = 0
        Try
            If sessionlist.ContainsKey(ipstr) And Not (sessionlist(ipstr)(2) Is Nothing) Then
                killid = Int(Val(sessionlist(ipstr)(2)))
                Dim pProcessTemp As System.Diagnostics.Process = Process.GetProcessById(killid)
                Invoke(printlog, "kill process id:" & killid & "  process name:" & pProcessTemp.ProcessName)
                pProcessTemp.Kill()
                pProcessTemp.Close()

            Else


            End If
        Catch
        End Try


    End Sub
    Sub runnew(ByVal command As String)
        Dim ipstr As String = ""
        Dim commandlist As String()
        Dim commandsnum As Integer = 0
        ipstr = getip(command)
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
        Try
            path = Application.StartupPath
            speed = Split(paramtere, ",")(0)
            If speed <> "0" Then
                dueration = Split(paramtere, ",")(2)
                packetsize = Split(paramtere, ",")(1)
                ip = Split(ipstr, ":")(0)
                'port = Split(ipstr, ":")(1)
                Dim myprocess As Process = New Process()
                myprocess.StartInfo.WorkingDirectory = path
                myprocess.StartInfo.FileName = "miperf.exe"
                Invoke(printlog, "miperf parameter:" + "-c" + ip + " -t" + dueration + " -u -b" + speed + ".0k -l" + packetsize + " -i 10 -p 33894")
                myprocess.StartInfo.Arguments = "-c" + ip + " -t" + dueration + "-u -b" + speed + ".0k -l" + packetsize + " -i 10 -p 33894"
                myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized

                myprocess.Start()
                updateconnectprocessid(ipstr, myprocess.Id)
                Invoke(printlog, "udp session id opened:" & myprocess.Id.ToString)

            Else
                updateconnectprocessid(ipstr, "0")
                Invoke(printlog, "speed 0, no traffic,go sleep")
            End If
        Catch
        End Try
    End Sub
    Function getcommandlist(ByVal command As String) As String()
        Dim commandstr As String = ""
        commandstr = Split(command, "|")(2)
        commandstr = Mid(commandstr, 1, commandstr.LastIndexOf("]"))
        Return Split("]" + commandstr, "][")

    End Function


    Function checkformat(ByVal command As String) As String
        Dim splitedstr As String()
        splitedstr = Split(command, "|")
        If command.IndexOf("|h") >= 0 Then
            Return "h"
        Else
            If command.IndexOf("|c") >= 0 Then
                Return "c"
            End If
            If command.IndexOf("|p") >= 0 Then
                Return "p"
            End If
        End If
    End Function
    Function checkprocessisworking(ByVal ip As String) As Boolean
        Dim killid As Integer
        killid = Int(Val(sessionlist(ip)(2)))
        Try
            Dim pProcessTemp As System.Diagnostics.Process = Process.GetProcessById(killid)
            If pProcessTemp.HasExited Then Return False Else Return True
        Catch
            Return False
        End Try

    End Function
    Sub keeprunning(ByVal ip As String)
        Dim command As String
        If checkprocessisworking(ip) = False And sessionlist(ip)(3) <> "" And sessionlist(ip)(2) <> "0" And sessionlist(ip)(0) = "c" Then
            Invoke(printlog, ip + " iperf process terminaled,try recover the traffic")
            killoldthread(ip)
            command = sessionlist(ip)(3)
            Dim tr As New Threading.Thread(AddressOf runnew)
            tr.Name = ip
            tr.Start(command)
            'updateconnectthreadid(ip, tr)

        End If
    End Sub
    Sub updateconnecttime(ByVal ip As String)
        Dim value(3) As String

        If sessionlist.ContainsKey(ip) Then
            sessionlist(ip)(1) = Now.ToString("MM/dd/yyyy HH:mm:ss")
            Invoke(printlog, "Connection session " + ip + " time updated")

        Else
            value(0) = ""
            value(1) = Now.ToString("MM/dd/yyyy HH:mm:ss")
            value(2) = ""
            value(3) = ""
            sessionlist.Add(ip, value)
            Invoke(printlog, "New connection session " + ip + " added")
        End If

        keeprunning(ip)

    End Sub

    Sub updateconnectthreadid(ByVal ip As String, ByVal newthread As Threading.Thread, ByVal command As String, ByVal mode As String)
        Dim value(3) As String
        Dim pvalue(2) As String
        If threadlist.ContainsKey(ip) Then
            threadlist(ip) = newthread

        Else
            threadlist.Add(ip, newthread)
            Invoke(printlog, "New " + ip + " traffic thread: " + newthread.ManagedThreadId.ToString + " added")
        End If
        If sessionlist.ContainsKey(ip) Then
            sessionlist(ip)(3) = command
            sessionlist(ip)(0) = mode
            If paginglist.ContainsKey(ip) Then
                paginglist(ip)(0) = "0"
                paginglist(ip)(1) = "0"
            End If
            'sessionlist(ip)(1) = Now.ToString("MM/dd/yyyy HH:mm:ss")
            'Invoke(printlog, "Connection session " + ip + " process id updated :" + processid)
        Else
            value(3) = command
            value(2) = ""
            value(1) = Now.ToString("MM/dd/yyyy HH:mm:ss")
            value(0) = mode
            pvalue(0) = "0"
            pvalue(1) = "0"
            sessionlist.Add(ip, value)
            paginglist.Add(ip, pvalue)
            Invoke(printlog, "New connection session " + ip + " added")
        End If


    End Sub

    Sub updateconnectprocessid(ByVal ip As String, ByVal processid As String)
        Dim value(4) As String

        If sessionlist.ContainsKey(ip) Then
            sessionlist(ip)(2) = processid
            'sessionlist(ip)(1) = Now.ToString("MM/dd/yyyy HH:mm:ss")
            Invoke(printlog, "Connection session " + ip + " process id updated :" + processid)
        Else
            value(2) = processid
            value(1) = Now.ToString("MM/dd/yyyy HH:mm:ss")
            value(3) = ""
            sessionlist.Add(ip, value)
            Invoke(printlog, "New connection session " + ip + " added")
        End If


    End Sub
    Sub clearuselessprocessthread()
        Dim lasttime As New Date
        Dim timediffer As Integer
        Dim connection As String
        Dim point As Integer = 0
        Invoke(printlog, "Traffic process monitor start")
        Do While 1
            point = 0
            Threading.Thread.Sleep(1000)
            Do Until point = sessionlist.Keys.Count
                If closesserver = True Then Exit Sub
                connection = sessionlist.Keys(point)
                lasttime = DateTime.ParseExact(sessionlist(connection)(1), "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture)
                timediffer = DateDiff(DateInterval.Minute, lasttime, Now)
                If timediffer > 2 And sessionlist(connection)(0) = "c" Then
                    Invoke(printlog, "Close useless traffic to " + connection)
                    killoldprocess(connection)
                    killoldthread(connection)
                    sessionlist.Remove(connection)
                    point = point - 1
                ElseIf timediffer > 2 And sessionlist(connection)(0) = "p" Then
                    Invoke(printlog, "Close useless paging to " + connection)
                    killoldprocess(connection)
                    killoldthread(connection)
                    sessionlist.Remove(connection)
                    paginglist.Remove(connection)
                    point = point - 1

                End If
                point = point + 1
            Loop
        Loop
    End Sub

    Sub Closeallsessions()
        Dim lasttime As New Date
        Dim timediffer As Integer
        Dim connection As String
        Dim point As Integer = 0
        Invoke(printlog, "Closing all traffic")

        Do Until point = sessionlist.Keys.Count
            connection = sessionlist.Keys(point)
            'lasttime = DateTime.ParseExact(sessionlist(connection)(1), "MM/dd/yyyy HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture)
            'timediffer = DateDiff(DateInterval.Minute, lasttime, Now)
            'Invoke(printlog, "Close useless traffic to " + connection)
            killoldprocess(connection)
            killoldthread(connection)
            'sessionlist.Remove(connection)
            point = point + 1
        Loop
    End Sub
    Sub startUDPserver()
        Dim hostEntry As IPHostEntry = Dns.GetHostEntry(Dns.GetHostName)
        Dim endPoint As IPEndPoint = New IPEndPoint(System.Net.IPAddress.Any, 33892)
        Dim s As New Net.Sockets.Socket(endPoint.Address.AddressFamily, SocketType.Dgram, ProtocolType.Udp)
        Dim sender As IPEndPoint = New IPEndPoint(IPAddress.Any, 0)
        Dim senderRemote As EndPoint = CType(sender, EndPoint)
        s.Bind(endPoint)
        'Dim data() As Byte = New Byte((256) - 1) {}
        Console.WriteLine("Waiting...")
       
        While True
            Dim data() As Byte = New Byte((256) - 1) {}
            s.ReceiveFrom(data, senderRemote)
            Dim deviceID As String = Convert.ToString(data(5), 16)
            Dim message As String = System.Text.Encoding.ASCII.GetString(data)
            Dim iPstr As String = senderRemote.ToString
            If message.IndexOf(Chr(13) & Chr(10)) > 0 Then message = Microsoft.VisualBasic.Left(message, message.Length - 2)
            If Replace(Trim(message), vbCrLf, "") <> "" Then
                Hwmsgpool.Add(message) 'iPstr + "|" + message)
                Invoke(printlog, message)
                ' Process the data sent by the client.
                ' message = vbCrLf + ">"
                'Dim msg9 As Byte() = System.Text.Encoding.ASCII.GetBytes(message)
                'stream.Write(msg9, 0, msg9.Length)
            End If

        End While
    End Sub
    Sub startUDPserverthread()
        Dim thr As New Threading.Thread(AddressOf startUDPserver)
        thr.Name = "UDP server"
        thr.IsBackground = True
        thr.Start()
        udptrhread = thr
        Invoke(printlog, "start UDP server listening on port 33892")
    End Sub
End Class