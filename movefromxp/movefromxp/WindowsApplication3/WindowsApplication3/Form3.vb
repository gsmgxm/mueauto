Public Class Form3
    Private listener As System.Net.Sockets.TcpListener
    Private listenThread As System.Threading.Thread

    '--------------------------------------------------------------------
    Public Delegate Sub printinf(ByVal input As String)
    Public Sub writelog(ByVal inputstr As String)
        TextBox1.Text = TextBox1.Text & inputstr & vbCrLf
    End Sub

    Public Delegate Sub remoterun()
    Public Sub runUE()
        MDIForm1.Run_Click(Nothing, Nothing)
        Form4.Command2_Click(Nothing, Nothing)
        Form4.Command4_Click(Nothing, Nothing)
    End Sub

    Public Sub runoneUE()
        MDIForm1.Run_Click(Nothing, Nothing)

        Form4.selectoneUE()
        Form4.Command4_Click(Nothing, Nothing)
    End Sub
    Public Sub resetoneUE()
        MDIForm1.Run_Click(Nothing, Nothing)

        Form4.selectoneUE()
        Form4.Button2_Click(Nothing, Nothing)
    End Sub

    Public Delegate Sub remoteload(ByVal file As String)
    Public Sub loadfile(ByVal filename As String)
        MDIForm1.Run_Click(Nothing, Nothing)
        If filename <> My.Application.Info.DirectoryPath & "\ueconfig.ini" Then
            System.IO.File.Copy(filename, My.Application.Info.DirectoryPath & "\ueconfig.ini", True)

            Form2.restartapp()
        End If
    End Sub
    Public Delegate Sub remotescenario()
    Public Sub scenario()
        MDIForm1.Run_Click(Nothing, Nothing)
        Form4.Runscenario_Click(Nothing, Nothing)
    End Sub
    Public Delegate Sub remoteshutdown()
    Public Sub shutdownUE()
        MDIForm1.Run_Click(Nothing, Nothing)
        Form4.Command2_Click(Nothing, Nothing)
        Form4.Command7_Click(Nothing, Nothing)
    End Sub

    Public Sub shutdownoneUE()
        MDIForm1.Run_Click(Nothing, Nothing)
        Form4.selectoneUE()
        Form4.Command7_Click(Nothing, Nothing)
    End Sub



    Public Delegate Sub remotereconfigure()
    Public Sub reconfigure()
        MDIForm1.remotecommand = True
        MDIForm1.remotecommandreturn = ""
        MDIForm1.config_Click(Nothing, Nothing)
        Form2.Button5_Click(Nothing, Nothing)
        Form2.Command3_Click(Nothing, Nothing)
        MDIForm1.remotecommand = False
    End Sub
    Public Function readconfigfiles() As String
        Dim s As String
        readconfigfiles = ""
        s = Application.ExecutablePath
        s = System.IO.Path.GetDirectoryName(s)
        For Each ws In My.Computer.FileSystem.GetFiles(s)


            Dim kzm As String = ws.Substring(InStrRev(ws, "."), ws.Length - InStrRev(ws, ".")) '定义扩展名变量       
            If kzm = "ini" Then '这里一定要注意扩展名的大小写，如avi<>AVI的 
                ws = System.IO.Path.GetFileName(ws)
                readconfigfiles = readconfigfiles + ";" + ws
            End If
        Next

        s = s + "\auto"
        If System.IO.Directory.Exists(s) Then
            For Each ws In My.Computer.FileSystem.GetFiles(s)
                Dim kzm As String = ws.Substring(InStrRev(ws, "."), ws.Length - InStrRev(ws, ".")) '定义扩展名变量       
                If kzm = "ini" Then '这里一定要注意扩展名的大小写，如avi<>AVI的 
                    ws = System.IO.Path.GetFileName(ws)
                    readconfigfiles = readconfigfiles + ";" + "auto\" + ws
                End If
            Next
        End If

    End Function

    Private Sub Form3_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If Button1.Text = "Disconnect" Then
            MDIForm1.remoteon = False
            Button1.Text = "Connect"
            listenThread.Abort()
            listener.Stop()
            TextBox1.Text = TextBox1.Text + "Server stoped" + vbCrLf
        End If
    End Sub

    '--------------------------------------------------------------------
    Private Sub Form3_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If MDIForm1.remoteon = True Then
            Button1_Click(Nothing, Nothing)
        End If
    End Sub
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        If Button1.Text = "Connect" Then
            MDIForm1.remoteon = True
            listener = New System.Net.Sockets.TcpListener(System.Net.IPAddress.Any, Val(TextBox2.Text)) 'The TcpListener will listen for incoming connections at port 43001
            listenThread = New System.Threading.Thread(AddressOf doListen) 'This thread will run the doListen method

            listenThread.IsBackground = True 'Since we dont want this thread to keep on running after the application closes, we set isBackground to true.

            listener.Start() 'Start listening.

            listenThread.Start() 'Start executing doListen on the worker thread.
            Button1.Text = "Disconnect"
        Else
            MDIForm1.remoteon = False
            Button1.Text = "Connect"
            listenThread.Abort()
            listener.Stop()
            TextBox1.Text = TextBox1.Text + "Server stoped" + vbCrLf
        End If



    End Sub
    Private Sub doListen()
        Dim bytes(1024) As Byte
        Dim data As String = Nothing
        Dim incomingClient As System.Net.Sockets.TcpClient
        Dim printlog As New printinf(AddressOf writelog) '定义数据显示委托实例
        Dim run_ue As New remoterun(AddressOf runUE)
        Dim run_resetoneue As New remoterun(AddressOf resetoneUE)
        Dim run_oneue As New remoterun(AddressOf runoneUE)
        Dim run_load As New remoteload(AddressOf loadfile)
        Dim run_scenario As New remotescenario(AddressOf scenario)
        Dim run_shutdownUE As New remoteshutdown(AddressOf shutdownUE)
        Dim run_shutdownoneUE As New remoteshutdown(AddressOf shutdownoneUE)
        Dim auto_configure As New remotereconfigure(AddressOf reconfigure)
        Dim loadfilename As String = ""
        Do
            Invoke(printlog, "listening...")
            'TextBox1.Text = TextBox1.Text & "listening..." & vbCrLf
            incomingClient = listener.AcceptTcpClient 'Accept the incoming connection. This is a blocking method so execution will halt here until someone tries to connect.
            'TextBox1.Text = TextBox1.Text & "Connected!" & vbCrLf
            Invoke(printlog, "Connected")
            data = Nothing

            ' Get a stream object for reading and writing
            Dim stream As System.Net.Sockets.NetworkStream = incomingClient.GetStream()

            Dim i As Int32
            i = 0
            ' Loop to receive all the data sent by the client.
            Try
                If incomingClient.Connected = True Then
                    data = vbCrLf + ">"
                    Dim msg0 As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
                    stream.Write(msg0, 0, msg0.Length)
                    Invoke(printlog, "Send:" + data)
                    i = stream.Read(bytes, 0, bytes.Length)
                End If

                While (i <> 0)
                    ' Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i)
                    If data.IndexOf(Chr(13) & Chr(10)) > 0 Then data = Microsoft.VisualBasic.Left(data, data.Length - 2)

                    Invoke(printlog, "Received:" & data)

                    If Trim(data) = "run_UE" Then

                        data = "> OK" + vbCrLf + ">"
                        Dim msg As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
                        stream.Write(msg, 0, msg.Length)
                        Invoke(printlog, "Send:" + data)
                        Invoke(run_ue)
                    End If
                    If Trim(data).IndexOf("run_resetoneUE") >= 0 Then
                        targetUE = "COM9999"
                        If Trim(data).ToUpper.IndexOf("COM") >= 0 Then
                            targetUE = "C" + Trim(data.ToUpper).Split("COM")(1)
                            data = "> OK" + vbCrLf + ">"
                            Dim msg As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
                            stream.Write(msg, 0, msg.Length)
                            Invoke(printlog, "Send:" + data)
                            Invoke(run_resetoneue)
                        Else
                            data = "> Not OK, check UE COM port name,it should like COM123" + vbCrLf + ">"
                            Dim msg As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
                            stream.Write(msg, 0, msg.Length)
                            Invoke(printlog, "Send:" + data)

                        End If
                    End If

                    If Trim(data).IndexOf("run_oneUE") >= 0 Then
                        targetUE = "COM9999"
                        If Trim(data).ToUpper.IndexOf("COM") >= 0 Then
                            targetUE = "C" + Trim(data.ToUpper).Split("COM")(1)
                            data = "> OK" + vbCrLf + ">"
                            Dim msg As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
                            stream.Write(msg, 0, msg.Length)
                            Invoke(printlog, "Send:" + data)
                            Invoke(run_oneue)
                        Else
                            data = "> Not OK, check UE COM port name,it should like COM123" + vbCrLf + ">"
                            Dim msg As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
                            stream.Write(msg, 0, msg.Length)
                            Invoke(printlog, "Send:" + data)

                        End If
                    End If

                    If Trim(data).IndexOf("load") >= 0 Then
                        loadfilename = Trim(Mid(Trim(data), 5, Trim(data).Length - 4))
                        Invoke(printlog, "Send:" + loadfilename + ",path:" + IO.Path.GetDirectoryName(loadfilename) + ",apppath:" + My.Application.Info.DirectoryPath)

                        If Trim(IO.Path.GetDirectoryName(loadfilename)) = "" Then
                            If IO.File.Exists(My.Application.Info.DirectoryPath & "\" & loadfilename) Then

                                loadfilename = My.Application.Info.DirectoryPath & "\" & loadfilename
                                Invoke(printlog, "change path name to '" & loadfilename & "'")
                            End If
                        End If

                        If IO.File.Exists(loadfilename) Then

                            data = "> OK" + vbCrLf + ">"

                            Dim msg As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
                            stream.Write(msg, 0, msg.Length)
                            Invoke(printlog, "Send:" + data)
                            Invoke(run_load, loadfilename)
                        Else
                            data = "> Can not find file:" + loadfilename
                            Dim msg As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
                            stream.Write(msg, 0, msg.Length)
                            Invoke(printlog, "Send:" + data)
                        End If
                    End If

                    If Trim(data) = "run_scenario" Then

                        data = "> OK" + vbCrLf + ">"
                        Dim msg As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
                        stream.Write(msg, 0, msg.Length)
                        Invoke(printlog, "Send:" + data)
                        Invoke(run_scenario)
                    End If

                    If Trim(data) = "run_shutdownUE" Then
                        data = "> OK" + vbCrLf + ">"
                        Dim msg As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
                        stream.Write(msg, 0, msg.Length)
                        Invoke(printlog, "Send:" + data)
                        Invoke(run_shutdownUE)
                    End If

                    If Trim(data).IndexOf("run_shutdownoneUE") >= 0 Then
                        targetUE = "COM9999"
                        If Trim(data).ToUpper.IndexOf("COM") >= 0 Then
                            targetUE = "C" + Trim(data.ToUpper).Split("COM")(1)
                            data = "> OK" + vbCrLf + ">"
                            Dim msg As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
                            stream.Write(msg, 0, msg.Length)
                            Invoke(printlog, "Send:" + data)
                            Invoke(run_shutdownoneue)
                        Else
                            data = "> Not OK, check UE COM port name,it should like COM123" + vbCrLf + ">"
                            Dim msg As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
                            stream.Write(msg, 0, msg.Length)
                            Invoke(printlog, "Send:" + data)

                        End If
                    End If





                    If Trim(data) = "auto_configure" Then
                        data = "> OK" + vbCrLf + ">"
                        Dim msg As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
                        stream.Write(msg, 0, msg.Length)
                        Invoke(printlog, "Send:" + data)
                        Invoke(auto_configure)
                    End If

                    If Trim(data) = "read_config" Then

                        data = "> OK" + vbCrLf + "> files:" + readconfigfiles()
                        Dim msg As Byte() = System.Text.Encoding.ASCII.GetBytes(data + ";")
                        stream.Write(msg, 0, msg.Length)
                        Invoke(printlog, "Send:" + data + ";")

                    End If

                    If Trim(data) = "read_status" Then

                        data = "> OK" + vbCrLf + "> state:" + MDIForm1.state
                        Dim msg As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
                        stream.Write(msg, 0, msg.Length)
                        Invoke(printlog, "Send:" + data)

                    End If

                    If Trim(data) = "read_UEconfig" Then

                        data = "> OK" + vbCrLf + "> state:" + UEconfigurations
                        Dim msg As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
                        stream.Write(msg, 0, msg.Length)
                        Invoke(printlog, "Send:" + data)

                    End If

                    If Trim(data) = "read_UEstatus" Then

                        data = "> OK" + vbCrLf + "> state:" + UEinformations
                        Dim msg As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
                        stream.Write(msg, 0, msg.Length)
                        Invoke(printlog, "Send:" + data)

                    End If

                    If Trim(data) = "read_total" Then
                        data = "> OK" + vbCrLf + "> state:" + totalstatus
                        Dim msg As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
                        stream.Write(msg, 0, msg.Length)
                        Invoke(printlog, "Send:" + data)

                    End If

                    If Trim(data) = "read_returen" Then

                        data = "> OK" + vbCrLf + "> state:" + MDIForm1.remotecommandreturn
                        Dim msg As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
                        stream.Write(msg, 0, msg.Length)
                        Invoke(printlog, "Send:" + data)
                        MDIForm1.remotecommandreturn = ""

                    End If
                    ' Process the data sent by the client.
                    data = vbCrLf + ">"
                    Dim msg9 As Byte() = System.Text.Encoding.ASCII.GetBytes(data)
                    stream.Write(msg9, 0, msg9.Length)
                    Invoke(printlog, "Send:" + data)

                    If incomingClient.Connected = True Then
                        i = stream.Read(bytes, 0, bytes.Length)
                    Else
                        i = 0
                    End If
                End While

            Catch e1 As System.IO.IOException
                Invoke(printlog, "Client disconnected")
                'Finally
                '   listener.Stop()
            End Try
        Loop





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


End Class