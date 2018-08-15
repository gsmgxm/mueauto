Imports System.Threading
Module Module1
    Dim listener As System.Net.Sockets.TcpListener
    Dim listenThread As System.Threading.Thread
    Dim closesserver As Boolean = False
    Dim msgspool As New List(Of String)
    Dim statespool As New Dictionary(Of String, String)
    Dim ratespool As New Dictionary(Of String, String)

    Sub Main()
        Dim input As String
        Console.Title = "mueauto paging server"
        Console.WriteLine("Do you want exit? Press Y and return if yes")
        Console.WriteLine("start tcp server listen ...")
        runTCPserver()
        Dim timer1 As New System.Threading.Timer(New TimerCallback(AddressOf doing), Nothing, 0, 30000)

        While 1
            input = Console.ReadLine
            If input = "y" Or input = "Y" Then
                Exit While
            End If
        End While
        timer1.Dispose()
        stopTCPserver()
    End Sub

    Sub doing()
        Dim pingthread As System.Threading.Thread
        Dim key, value As String
        Dim successtime, totaltime As Integer
        Dim i = 0
        While msgspool.Count > 0
            key = Split(msgspool(i), "|")(2)
            value = Split(msgspool(i), "|")(1)

            If statespool.ContainsKey(key) = False Then
                statespool.Add(key, value)
                ratespool.Add(key, "0|0")
            Else
                statespool.Item(key) = value
            End If
            msgspool.RemoveAt(0)
        End While


        Try
            For Each key In statespool.Keys
                pingthread = New System.Threading.Thread(AddressOf ping) 'This thread will run the doListen method

                pingthread.IsBackground = True 'Since we dont want this thread to keep on running after the application closes, we set isBackground to true.

                'pingthread.Start("ping " + statespool(key) + " -l 1000 -n 3", key) 'Start listening.
                pingthread.Start(key)
                pingthread.Name = "ping" + statespool(key)


                'Dim myprocess As Process = New Process()

                'myprocess.StartInfo.FileName = "ping"

                'myprocess.StartInfo.Arguments = statespool(key) + " -l 1000  -n 3"

                'myprocess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized
                'myprocess.Start()



                If Console.CursorTop = 16 Then
                    Console.CursorTop = 5
                    Console.CursorLeft = 0
                    Console.WriteLine("")
                    Console.CursorTop = 5
                    Console.CursorLeft = 0
                End If
                successtime = CInt(ratespool(key).Split("|")(0))
                totaltime = CInt(ratespool(key).Split("|")(1))
                If totaltime = 0 Then
                    Console.WriteLine(Now.ToString + ":" + "ping " + statespool(key) + "|" + key + "|" + "0%")

                Else
                    Console.WriteLine(Now.ToString + ":" + "ping " + statespool(key) + "|" + key + "|" + successtime.ToString + "," + totaltime.ToString + "|" + (successtime * 100 / totaltime).ToString("0.00") + "%")

                End If


            Next
        Catch ex As Exception
            Console.WriteLine(ex.Message.ToString)
        End Try


    End Sub
    Sub ping(ByVal key As String)
        Dim s As String
        Dim values = 100
        Dim target As String = "ping " + statespool(key) + " -l 1000 -w 5000"
        s = rundoscommand(target)
        ' Console.WriteLine(s)

        If InStr(s, "unreachable") > 0 Then
            ratespool(key) = ratespool(key).Split("|")(0) + "|" + (CInt(ratespool(key).Split("|")(1)) + 1).ToString  '= ping fail
        Else
            ratespool(key) = (CInt(ratespool(key).Split("|")(0)) + 1).ToString + "|" + (CInt(ratespool(key).Split("|")(1)) + 1).ToString
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
        Dim sIn As System.IO.StreamWriter = myProcess.StandardInput
        sIn.AutoFlush = True

        Dim sOut As System.IO.StreamReader = myProcess.StandardOutput
        Dim sErr As System.IO.StreamReader = myProcess.StandardError
        sIn.Write(command & _
        System.Environment.NewLine)
        sIn.Write("exit" & System.Environment.NewLine)
        s = sOut.ReadToEnd()

        If Not myProcess.HasExited Then
            myProcess.Kill()
        End If



        sIn.Close()
        sOut.Close()
        sErr.Close()
        myProcess.Close()
        Return s
    End Function

    Sub stopTCPserver()
        closesserver = True
        Try
            Dim closeclient = New System.Net.Sockets.TcpClient("127.0.0.1", 33893)
            If closeclient.Connected = True Then
                closeclient.Close()
                Console.WriteLine("Close tcp server....")
            Else

            End If



            listener.Stop()
            listenThread.Abort()
        Catch e3 As Exception
        End Try

    End Sub
    Sub runTCPserver()
        listener = New System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, 33893) 'The TcpListener will listen for incoming connections at port 43001
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

                    msgspool.Add(data)

                    Console.CursorLeft = 0
                    Console.CursorTop = 3
                    Console.WriteLine()
                    Console.CursorLeft = 0
                    Console.CursorTop = 3


                    Console.WriteLine(Now.ToString + "|new msg:" + data + "|msg count:" + msgspool.Count.ToString)

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






End Module
