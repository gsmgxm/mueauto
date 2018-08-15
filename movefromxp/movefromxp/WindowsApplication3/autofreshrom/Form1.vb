Public Class Form1

    Dim devicessn As New List(Of String)
    Dim result As String = ""
    Dim t As Threading.Thread
    Dim backprogress As Integer
    Dim listbox As New List(Of String)

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        GetAlladbdevices()
        runflashroms()
    End Sub

    Sub runflashroms()
        ProgressBar1.Value = 0
        ProgressBar1.Maximum = devicessn.Count
        For Each sn As String In devicessn
            Process1.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(TextBox3.Text)
            Process1.StartInfo.FileName = System.IO.Path.GetFileName(TextBox3.Text)
            Process1.StartInfo.Arguments = sn
            TextBox1.Text = TextBox1.Text + "Start to flash device " + sn + vbCrLf
            Process1.Start()
            Process1.WaitForExit()

            ProgressBar1.Value = ProgressBar1.Value + 1
        Next
    End Sub

    Sub getalladbdevices()
        Dim result As String
        Dim lines As String()
        Dim devicssn As String
        Dim displayinfo As String
        devicessn.Clear()
        result = rundoscomandt("adb devices")
        If result <> "" Then
            lines = Split(result, vbCrLf)
            For Each line As String In lines
                If line.IndexOf(Chr(9) + "device") > 0 And line.IndexOf("devices") < 0 Then

                    devicssn = Split(line, Chr(9))(0)
                    devicessn.Add(devicssn)

                End If
            Next
        End If
        For Each S As String In devicessn
            displayinfo = displayinfo + "|" + S

        Next
        TextBox1.Text = TextBox1.Text + displayinfo + vbCrLf
    End Sub
    Private Sub rundoscommand(ByVal commands As String)
        Dim command() As String
        command = Split(commands, "|")
        Dim myProcess As Process = New Process()
        'Dim s As String
        myProcess.StartInfo.FileName = "cmd.exe"
        myProcess.StartInfo.WorkingDirectory = System.Windows.Forms.Application.StartupPath
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
            Threading.Thread.Sleep("3000")
            Try
                Process.GetProcessById(backprogress).Kill()
            Catch

            End Try

            t.Abort()
        End If




    End Function


    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim devicetype As String = TextBox3.Text
        getallcomport(devicetype)

        writenewcommand(TextBox2.Text)


    End Sub
    Function Getcomport(ByVal i) As String
        Dim tempstr = Split(i, "(")(1)
        tempstr = Split(tempstr, ")")(0)
        Return tempstr
    End Function
    Sub writenewcommand(ByVal command)
        Dim _serialport As New IO.Ports.SerialPort
        Dim opentime As Integer = 0

        _serialPort.BaudRate = 115200
        _serialPort.StopBits = IO.Ports.StopBits.One
        _serialPort.Parity = IO.Ports.Parity.None
        _serialPort.DataBits = 8
        ' Set the read/write timeouts
        _serialPort.ReadTimeout = 5000
        _serialPort.WriteTimeout = 500
        _serialPort.DtrEnable = True
        _serialport.RtsEnable = True

        Dim commandlist As String()
        commandlist = Split(command, "|")
        ProgressBar1.Value = 0
        ProgressBar1.Maximum = listbox.Count
        For Each com As String In listbox
            _serialport.PortName = Getcomport(com)
            TextBox1.Text = TextBox1.Text + "Start write AT command to " + com
            Do
                Try
                    _serialport.Open()
                Catch ex As TimeoutException
                    Console.WriteLine("Timeout")
                    Console.WriteLine(ex.ToString)
                Catch ex As Exception
                    Console.WriteLine("open port fail")
                End Try
                opentime = opentime + 1
            Loop Until _serialport.IsOpen = True Or opentime > 3
            sendcommand(commandlist, _serialport)
            _serialport.Close()
            ProgressBar1.Value = ProgressBar1.Value + 1
        Next
    End Sub

    Function sendcommand(ByVal commandlist As String(), ByVal serialport As IO.Ports.SerialPort) As Boolean
        Try
            For Each Command As String In commandlist

                serialsenddata2(Command + vbCr, serialport, False)
                Threading.Thread.Sleep(1000)
                Dim message As String = serialport.ReadExisting
                TextBox1.Text = TextBox1.Text + (message) + vbCrLf

                'Threading.Thread.Sleep(1000)
            Next
        Catch ex As Exception
            TextBox1.Text = TextBox1.Text + "serialport error:" + ex.ToString + vbCrLf
            Return False
        End Try
        Return True

    End Function
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



    Function getallcomport(ByVal device)
        Dim cominfo As String
        getallinformation(True, device)
        For Each com As String In listbox
            cominfo = cominfo + "|" + com
        Next
        TextBox1.Text = TextBox1.Text + cominfo + vbCrLf
    End Function

    Sub getallinformation(ByVal freshall As Boolean, ByVal device As String)
        Dim s, tmps As String
        ' Dim obj2 As String
        Dim avalibleports As New Collection
        Dim find As Boolean


        Dim wmiobjectset As Object

        If freshall Then
            listbox.Clear()
            For Each s In IO.Ports.SerialPort.GetPortNames
                'Label1.Text = "31"
                '  wmiobjectset = GetObject("winmgmts:\\.\root\CIMV2").ExecQuery("SELECT * FROM Win32_POTSModem")
                ' For Each wmiobject In wmiobjectset
                ''MsgBox(wmiobject.Name & " on " & wmiobject.attachedto)
                'If wmiobject.attachedto.indexof(s.ToString) >= 0 Then
                'If ListBox1.FindString(wmiobject.Name & "(" & wmiobject.attachedto & ")") = -1 Then
                'ListBox1.Items.Add(wmiobject.Name & "(" & wmiobject.attachedto & ")")
                'End If
                'End If
                'Next
                wmiobjectset = Nothing

                find = False
                Dim searcher As New System.Management.ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity ")
                For Each mgt As System.Management.ManagementObject In searcher.[Get]()
                    Try
                        If mgt("Name").ToString().IndexOf("(" + s.ToString + ")") > 0 Then
                            'Label1.Text = "311"
                            tmps = Trim(mgt("Name").ToString())
                            If tmps.IndexOf("BandLuxe Wideband AT CMD Interface") >= 0 Or tmps.IndexOf("Altair LTE Application Interface") >= 0 Or tmps.IndexOf("HUAWEI Mobile Connect - PC UI Interface") >= 0 Or tmps.IndexOf("SimTech HS-USB AT Port") >= 0 Then ' add more type dongles
                                ' avalibleports.Add(mgt("Name"), s.ToString)
                                If listbox.Contains(mgt("Name")) = -1 And mgt("Name").ToString.IndexOf(device) > 0 Then
                                    listbox.Add(mgt("Name"))
                                End If
                                'tmps.IndexOf("Application") >= 0 Or
                            End If

                            Exit For
                        End If
                    Catch ex1 As Exception

                    End Try
                Next
                Dim rootkey As Microsoft.Win32.RegistryKey
                Dim regdirlist As Object
                Dim modemname As String
                Dim comname As String
                rootkey = My.Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\ControlSet001\Control\Class\{4D36E96D-E325-11CE-BFC1-08002BE10318}") 'modem
                'HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\Class\{4D36E972-E325-11CE-BFC1-08002BE10318} 'netcard
                regdirlist = rootkey.GetSubKeyNames
                'Label1.Text = "32"
                For Each regdir As Object In regdirlist
                    Dim a = 1
                    If regdir.ToString <> "Properties" Then
                        comname = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\Class\{4D36E96D-E325-11CE-BFC1-08002BE10318}\" & regdir, "AttachedTo", String.Empty)
                        If comname.ToLower = s.ToString.ToLower Then
                            modemname = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\Class\{4D36E96D-E325-11CE-BFC1-08002BE10318}\" & regdir, "DriverDesc", String.Empty)
                            If modemname.IndexOf("SimTech") < 0 Then
                                If modemname <> "" Then
                                    If listbox.Contains(modemname) = False Then
                                        listbox.Add(modemname & "(" & s.ToUpper & ")")

                                    End If


                                End If
                            End If
                        End If



                    End If

                Next






            Next


        End If







    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        OpenFileDialog1.Filter = "flashexe|*.bat;*.exe"
        If (OpenFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK) Then
            TextBox3.Text = OpenFileDialog1.FileName
        End If
    End Sub
End Class
