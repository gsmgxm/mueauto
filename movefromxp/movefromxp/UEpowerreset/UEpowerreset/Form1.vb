Public Class Form1

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        NotifyIcon1.Dispose()
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        ConsoleHelper.openconsole(Me.Handle, Me.Width, Me.Height)
        Console.Title = "UE power reset"
        ConsoleHelper.setconsolehide()

        Me.Hide()

        Me.ShowInTaskbar = False
        NotifyIcon1.Visible = True
        Dim mainthread As New Threading.Thread(AddressOf Main)
        mainthread.Start()

    End Sub


    Dim apppath As String
    Dim winshow As Boolean = False
    Sub Main()
        apppath = Mid(System.Reflection.Assembly.GetEntryAssembly.Location, 1, System.Reflection.Assembly.GetEntryAssembly.Location.LastIndexOf("\") + 1)
        Dim controlip As String = ""
        Dim port As String = ""
        controlip = Module2.ReadKeyVal(apppath + "setting.ini", "ip", "controlip")
        port = Module2.ReadKeyVal(apppath + "setting.ini", "ip", "controlport")
        Dim timelist As List(Of String)
        timelist = gettimelist()

        If controlip <> "" And port <> "" Then
            Console.Write("Target time is:")
            For Each timestr As String In timelist
                Console.Write(timestr + "|")
            Next
            Console.WriteLine()
            Console.WriteLine("Now is:")
            Console.CursorTop = 2
            Console.CursorLeft = 1
            Console.WriteLine(Now.ToString("HH:mm"))
            Do
                If checktime(timelist) = True Then
                    Console.WriteLine("power off and on at:" + Now.ToString("yyyy-MM-dd HH:mm:ss"))
                    poweroffon(getiplist, controlip, port)

                End If
                Threading.Thread.Sleep(60000)
                Console.CursorTop = 2
                Console.CursorLeft = 1
                Console.WriteLine(Now.ToString("HH:mm"))

            Loop



        End If


    End Sub


    Function checktime(ByVal timelist) As Boolean
        Dim nowtime As String
        nowtime = (Now.ToString("HH:mm"))
        checktime = False
        For Each inputtime As String In timelist
            If inputtime = nowtime Then
                Return True
            End If


        Next


    End Function


    Public Function sendtcpcommand(ByVal command As String, ByVal ip As String, ByVal port As String) As String
        Try
            'Dim port As Int32 = 33891
            'Dim client As New System.Net.Sockets.TcpClient(ip, 33891, )
            Dim client As New System.Net.Sockets.TcpClient()
            Dim result
            Dim success As Boolean

            Dim i As Integer = 1
            Do While i < 4
                client = New System.Net.Sockets.TcpClient
                result = client.BeginConnect(ip, port, Nothing, Nothing)
                success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(3))
                If client.Connected = False Then
                    Console.WriteLine("Failed to connect." + ip + "|" + port + "|" + command)

                    'client.EndConnect(result)
                    client.Close()
                Else
                    Exit Do
                End If
                i = i + 1
                If i = 4 Then Return "fail"
            Loop
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
            'client.EndConnect(result)
            stream.Close()
            client.Close()
            Threading.Thread.Sleep(300)
        Catch e As Exception
            Return ("Connection fail:" + e.Message)
            'Catch e As System.Net.Sockets.SocketException
            '    Return ("Connection fail")
        End Try
    End Function


    Function poweroffon(ByVal iplist As List(Of String), ByVal controlip As String, ByVal port As String)
        Dim commands As List(Of String)
        commands = iplistcommand(iplist)
        For Each command As String In commands
            Console.WriteLine("send command:" + command)
            Console.WriteLine("reply:" + sendtcpcommand(command, controlip, port))


        Next

    End Function

    Function iplistcommand(ByVal iplist As List(Of String)) As List(Of String)
        Dim returncommand As New List(Of String)
        Dim portnumber As Integer = 1
        Dim portnumberstr As String = ""

        For Each ip As String In iplist

            If 1 = 1 Then
                portnumber = Int(ip)
                Select Case portnumber
                    Case 10
                        portnumberstr = "A"
                    Case 11
                        portnumberstr = "B"
                    Case 12
                        portnumberstr = "C"
                    Case 13
                        portnumberstr = "D"
                    Case 14
                        portnumberstr = "E"
                    Case 17
                        portnumberstr = "F"
                    Case 16
                        portnumberstr = "G"
                    Case 1, 2, 3, 4, 5, 6, 7, 8, 9
                        portnumberstr = portnumber.ToString
                End Select
                returncommand.Add("1" + portnumberstr + ":10")

            End If



        Next

        iplistcommand = returncommand

    End Function

    Function getiplist() As List(Of String)

        Dim iplist As New List(Of String)
        Dim ips As Object
        ips = Split(Module2.ReadKeyVal(apppath + "setting.ini", "ip", "portlist"), ",")

        For Each ip As String In ips
            iplist.Add(ip)

        Next

        Return iplist



    End Function

    Function gettimelist() As List(Of String)
        Dim timelist As New List(Of String)
        Dim times As Object
        times = Split(Module2.ReadKeyVal(apppath + "setting.ini", "time", "timeclock"), ",")

        For Each time1 As String In times
            timelist.Add(time1)

        Next

        Return timelist



    End Function

    Private Sub NotifyIcon1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles NotifyIcon1.Click
        If winshow = False Then
            ConsoleHelper.setconsolemax()
            winshow = True
        Else
            ConsoleHelper.setconsolehide()
            winshow = False
        End If

    End Sub

End Class
