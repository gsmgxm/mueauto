Imports System.Net
Imports System.IO
Imports System.Threading
Imports System.Management


Module Module1
    Dim ftpserverip As String
    Dim ftpuserid As String
    Dim ftppassword As String
    Sub Main()

        Dim myArg() As String
        Dim ftpmode As String = "DL"
        Dim sessionno As Integer
        Dim filename As String = ""
        Dim ulfilename As String = ""
        Dim ulremotename As String = "1"
        Dim com As String = ""
        Try
            myArg = System.Environment.GetCommandLineArgs

            If UBound(myArg) >= 6 Then

                ftpserverip = myArg(1).ToString
                Console.WriteLine("ftpserver ip:" + ftpserverip)
                ftpuserid = myArg(2).ToString
                Console.WriteLine("ftpuser id:" + ftpuserid)
                ftppassword = myArg(3).ToString
                Console.WriteLine("ftp password:" + ftppassword)
                ftpmode = myArg(4).ToString
                Console.WriteLine("ftpmode:" + ftpmode)
                sessionno = Int(myArg(5).ToString)
                Console.WriteLine("session num:" + sessionno.ToString)
                filename = myArg(6).ToString
                Console.WriteLine("DL filename:" + filename)
                com = myArg(7).ToString
                Console.Title = "FTP:" + com
                If UBound(myArg) = 9 Then
                    ulremotename = myArg(8).ToString
                    Console.WriteLine("UL remotename:" + ulremotename)
                    ulfilename = myArg(9).ToString
                    Console.WriteLine("UL filename:" + ulfilename)
                Else
                    ulremotename = "1"
                    ulfilename = "1"
                End If

            Else
                Console.WriteLine("No enough input parameter")
                Console.WriteLine("ftpclient IP counter password UL|DL sessionno filename ulremotename ulfilename ")
                Console.ReadLine()
            End If
            If ftpmode = "DL" Then
                multisession(sessionno, filename)
            ElseIf ftpmode = "UL" Then
                multisessionul(sessionno, ulfilename, ulremotename)
                'uploadloop(filename, ulremotename)
            ElseIf ftpmode = "DLUL" Then
                multisession(sessionno, filename)
                'multisessionul(sessionno, ulfilename, ulremotename)






            End If
        Catch ex As Exception
            Dim sw As New IO.StreamWriter("d:\ftpclienterror.txt")
            Try
                sw.WriteLine(Now.ToString + ex.ToString)

            Finally
                sw.Close()
            End Try
        End Try
    End Sub
    Sub fillbuffer(ByVal buff As Object)

        For i = 0 To UBound(buff) - 1

            buff(i) = CInt(Int((255 - 0 + 1) * Rnd() + 0))

        Next
    End Sub

    Private Function Upload(ByVal filename As String, ByVal remotename As String) As String
        ' Dim FileInfo As FileInfo = New FileInfo(filename)
        ' Dim uri As String = "ftp://" + ftpserverip + remotename + ftpserverip
        Dim uri As String = "ftp://" + ftpserverip + "/" + remotename
        Dim reqFTP As FtpWebRequest
        Dim strm As Stream
        Upload = ""
        Randomize()
        ' // 根据uri创建FtpWebRequest对象 
        reqFTP = FtpWebRequest.Create(New Uri("ftp://" + ftpserverip + "/" + remotename))

        '// ftp用户名和密码
        reqFTP.Credentials = New NetworkCredential(ftpuserid, ftppassword)

        '// 默认为true，连接不会被关闭
        '// 在一个命令之后被执行
        reqFTP.KeepAlive = True

        reqFTP.UsePassive = False
        '// 指定执行什么命令
        reqFTP.Method = WebRequestMethods.Ftp.UploadFile

        '// 指定数据传输类型.
        reqFTP.UseBinary = True

        '// 上传文件时通知服务器文件的大小
        reqFTP.ContentLength = 2096 * 20 * 2000 ' FileInfo.Length
        reqFTP.ReadWriteTimeout = 300000
        reqFTP.Timeout = 3000
        reqFTP.Proxy = Nothing

        '// 缓冲大小设置为2kb
        Dim buffLength As Integer = 2096 * 20

        Dim buff(buffLength) As Byte
        Dim contentLen As Integer
        Dim jj As Integer
        '// 打开一个文件流 (System.IO.FileStream) 去读上传的文件
        'Dim fs As FileStream
        ' fs = FileInfo.OpenRead()
        Try

            '// 把上传的文件写入流
            strm = reqFTP.GetRequestStream()

            '// 每次读文件流的2kb
            '    contentLen = fs.Read(buff, 0, buffLength)
            Console.WriteLine("start upload " & filename & " to remotefile:" & "ftp://" + ftpserverip + "/" + remotename)
            contentLen = 2096 * 20
            jj = 0
            fillbuffer(buff)
            System.Threading.Thread.Sleep(30)
            '// 流内容没有结束
            While (jj < 2000) And (contentLen <> 0) And strm.CanWrite = True

                ' // 把内容从file stream 写入 upload stream
                strm.Write(buff, 0, contentLen)

                jj = jj + 1
                'contentLen = fs.Read(buff, 0, buffLength)
                'Console.Write("*")
            End While

            '// 关闭两个流
            strm.Close()
            ' fs.Close()
            Console.WriteLine("")
            Console.WriteLine("upload success")
        Catch ex As Exception
            If Not (strm Is Nothing) Then
                ' strm.Dispose()

            End If
            'If Not (fs Is Nothing) Then
            'fs.Close()
            'End If
            If Not (reqFTP Is Nothing) Then
                reqFTP.Abort()
            End If

            Console.WriteLine(ex.Message.ToString & ";Upload Error " + remotename + ftpserverip)
            If ex.Message.ToString.IndexOf("The remote server returned an error: (550) File unavailable (e.g., file not found, no access).") >= 0 Then
                Dim rando1 As New Random(DateTime.Now.Millisecond)
                Return rando1.Next(5).ToString

            End If






            System.Threading.Thread.Sleep(3000)
        End Try

    End Function

    Private Sub multisessionul(ByVal sessionno As Integer, ByVal filename As String, ByVal ulremotename As String)
        Dim thlist(sessionno) As Thread
        Dim harddriver As String
        Dim filename2 As String
        harddriver = getip()
        ulremotename = ulremotename + harddriver
        For i = 1 To sessionno
            filename2 = filename + "|" + ulremotename + i.ToString
            thlist(i) = New Thread(AddressOf uploadloop)
            thlist(i).Start(filename2)
        Next

    End Sub

    Private Function getip() As String
        Dim returnstr As String
        Dim values As String
        Dim startindex, yy As Integer
        Dim tempstr As String
        values = ""
        getip = ""
        returnstr = rundoscommand("route print " + ftpserverip)
        startindex = returnstr.LastIndexOf(ftpserverip)
        If startindex >= 0 Then
            tempstr = Mid(returnstr, startindex, 100)
            yy = 0
            For i = 0 To UBound(Split(tempstr, " "))
                If Trim(Split(tempstr, " ")(i)) <> "" Then
                    yy = yy + 1

                End If
                If yy = 4 Then
                    getip = Trim(Split(tempstr, " ")(i))
                    Exit Function
                End If

            Next
        Else
            Return "NOip"
        End If


    End Function
    Function rundoscommand(ByVal command) As String
        Dim myProcess As Process = New Process()
        Dim s As String
        Try
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
        Catch ex As Exception
            Console.WriteLine("doscommand error:" + ex.ToString)
            Return ""
        End Try

    End Function
    Private Function sn() As String
        Randomize()
        sn = CInt(Int((25500 - 0 + 1) * Rnd() + 0))
    End Function

    'Private Function macsn() As String

    '    Dim netAddress As String = ""
    '    Dim netName As String = ""
    '    Dim searcher As New Management.ManagementObjectSearcher("select * from win32_NetworkAdapterConfiguration")
    '    Dim moc2 As Management.ManagementObjectCollection = searcher.Get()
    '    For Each mo As Management.ManagementObject In moc2
    '        If CBool(mo("IPEnabled")) Then '判断是否是网卡
    '            netName = mo.Properties("caption").Value.ToString '网卡名称
    '            netAddress = mo.Properties("MACAddress").Value.ToString 'mac地址 
    '        End If
    '    Next
    '    macsn = (Replace(netAddress, ":", "."))
    'End Function



    'Private Function serial() As String


    '    Try




    '        Dim myInfo As Microsoft.Win32.RegistryKey = My.Computer.Registry.LocalMachine.OpenSubKey("HARDWARE\DEVICEMAP\Scsi\Scsi Port 0\Scsi Bus 1\Target Id 0\Logical Unit Id 0")
    '        serial = Trim(myInfo.GetValue("SerialNumber"))
    '    Catch
    '        Try

    '            Dim myInfo As Microsoft.Win32.RegistryKey = My.Computer.Registry.LocalMachine.OpenSubKey("HARDWARE\DEVICEMAP\Scsi\Scsi Port 1\Scsi Bus 1\Target Id 0\Logical Unit Id 0")
    '            serial = Trim(myInfo.GetValue("SerialNumber"))
    '        Catch
    '            serial = ""
    '        End Try
    '    End Try
    'End Function

    'Public Function getCpu() As String
    '    Dim strCpu As String = Nothing
    '    Dim myCpu As New ManagementClass("win32_Processor")
    '    Dim myCpuConnection As ManagementObjectCollection = myCpu.GetInstances()
    '    For Each myObject As ManagementObject In myCpuConnection
    '        strCpu = myObject.Properties("Processorid").Value.ToString()
    '        Exit For
    '    Next
    '    Return strCpu
    'End Function

    'Private Function hardsn() As String
    '    Try
    '        Dim myInfo As Microsoft.Win32.RegistryKey = My.Computer.Registry.LocalMachine.OpenSubKey("HARDWARE\DEVICEMAP\Scsi\Scsi Port 0\Scsi Bus 1\Target Id 0\Logical Unit Id 0")
    '        hardsn = Trim(myInfo.GetValue("SerialNumber"))
    '    Catch
    '        Try
    '            Dim myInfo As Microsoft.Win32.RegistryKey = My.Computer.Registry.LocalMachine.OpenSubKey("HARDWARE\DEVICEMAP\Scsi\Scsi Port 1\Scsi Bus 1\Target Id 0\Logical Unit Id 0")
    '            hardsn = Trim(myInfo.GetValue("SerialNumber"))
    '        Catch
    '            hardsn = ""
    '        End Try
    '    End Try
    'End Function


    Public Sub uploadloop(ByVal filename1 As Object)
        Dim remotefile, filename As String
        Dim temp() As String
        Dim returen As String = ""
        temp = Split(filename1, "|")
        remotefile = temp(1)
        filename = temp(0)
        While 1

            returen = Upload(filename, remotefile + Trim(returen))


        End While
    End Sub


    Private Sub multisession(ByVal sessionno As Integer, ByVal filename As String)
        Dim thlist(sessionno) As Thread

        For i = 1 To sessionno
            thlist(i) = New Thread(AddressOf downloadloop)
            thlist(i).Start(filename)


        Next




    End Sub

    Public Sub downloadloop(ByVal fileName As Object)
        While 1 = 1
            Downloadsingle(fileName)
            ' Downloadsinglespeedlimit(fileName, 800000)
        End While
    End Sub

    Private Sub Downloadsinglespeedlimit(ByVal fileName As String, ByVal speedbits As Integer)

        Dim reqFTP As FtpWebRequest
        Dim response As FtpWebResponse
        Dim ftpStream As Stream
        Dim cl As Long
        Dim buffersize As Integer = 100 * 1
        Dim readcount As Integer
        Dim buffer(buffersize) As Byte
        Dim bitscount As Integer
        Dim stopWatch As New Stopwatch()
        '100ms check 一次
        Try

            'Dim outputStream As FileStream = New FileStream(filePath + "\\" + fileName, FileMode.Create)

            reqFTP = FtpWebRequest.Create(New Uri("ftp://" + ftpserverip + "/" + fileName))

            reqFTP.Method = WebRequestMethods.Ftp.DownloadFile

            reqFTP.UseBinary = True

            reqFTP.Proxy = Nothing

            reqFTP.Credentials = New NetworkCredential(ftpuserid, ftppassword)

            reqFTP.UsePassive = False

            reqFTP.KeepAlive = False
            reqFTP.ReadWriteTimeout = 3000
            reqFTP.Timeout = 3000

            response = reqFTP.GetResponse()

            ftpStream = response.GetResponseStream()


            cl = response.ContentLength

            Console.WriteLine("ftpdownload start :" & "ftp://" + ftpserverip + "/" + fileName)

            If ftpStream.CanRead = True Then

                stopWatch.Start()
                readcount = ftpStream.Read(buffer, 0, buffersize)
                bitscount = buffersize
                While (readcount > 0) And ftpStream.CanRead = True

                    'outputStream.Write(buffer, 0, readCount)
                    'stopWatch.Stop()
                    If (bitscount * 8 / (stopWatch.ElapsedMilliseconds / 1000)) < speedbits Then

                        readcount = ftpStream.Read(buffer, 0, buffersize)
                        bitscount = bitscount + buffersize
                    Else
                        Thread.Sleep(10)
                    End If

                    'If stopWatch.ElapsedMilliseconds > 10000 Then
                    'Console.WriteLine("bitcount:" + bitscount.ToString + ", time ms:" + Val(stopWatch.ElapsedMilliseconds).ToString)
                    'stopWatch.StartNew()
                    'bitscount = 0
                    'End If

                End While

                ftpStream.Close()

                'outputStream.Close()
                response.Close()
            End If

        Catch ex As Exception

            Console.WriteLine(ex.Message.ToString & ";DLerror")
            If Not (ftpStream Is Nothing) Then
                ' ftpStream.Close()
                'ftpStream.Dispose()
            End If

            If Not (reqFTP Is Nothing) Then

                reqFTP.Abort()
            End If

            If Not (response Is Nothing) Then

                response.Close()

            End If

            System.Threading.Thread.Sleep(3000)
        End Try
    End Sub

    Private Sub Downloadsingle(ByVal fileName As String)

        Dim reqFTP As FtpWebRequest
        Dim response As FtpWebResponse
        Dim ftpStream As Stream
        Dim cl As Long
        Dim buffersize As Integer = 2048 * 20
        Dim readcount As Integer
        Dim buffer(buffersize) As Byte
        Try

            'Dim outputStream As FileStream = New FileStream(filePath + "\\" + fileName, FileMode.Create)

            reqFTP = FtpWebRequest.Create(New Uri("ftp://" + ftpserverip + "/" + fileName))

            reqFTP.Method = WebRequestMethods.Ftp.DownloadFile

            reqFTP.UseBinary = True

            reqFTP.Proxy = Nothing

            reqFTP.Credentials = New NetworkCredential(ftpuserid, ftppassword)

            reqFTP.UsePassive = False

            reqFTP.KeepAlive = False
            reqFTP.ReadWriteTimeout = 3000
            reqFTP.Timeout = 3000

            response = reqFTP.GetResponse()

            ftpStream = response.GetResponseStream()


            cl = response.ContentLength

            Console.WriteLine("ftpdownload start :" & "ftp://" + ftpserverip + "/" + fileName)

            If ftpStream.CanRead = True Then
                readcount = ftpStream.Read(buffer, 0, buffersize)



                While (readcount > 0) And ftpStream.CanRead = True

                    'outputStream.Write(buffer, 0, readCount)

                    readcount = ftpStream.Read(buffer, 0, buffersize)
                End While

                ftpStream.Close()

                'outputStream.Close()
                response.Close()
            End If

        Catch ex As Exception

            Console.WriteLine(ex.Message.ToString & ";DLerror")
            If Not (ftpStream Is Nothing) Then
                ' ftpStream.Close()
                'ftpStream.Dispose()
            End If

            If Not (reqFTP Is Nothing) Then

                reqFTP.Abort()
            End If

            If Not (response Is Nothing) Then

                response.Close()

            End If

            System.Threading.Thread.Sleep(3000)
        End Try
    End Sub



End Module
