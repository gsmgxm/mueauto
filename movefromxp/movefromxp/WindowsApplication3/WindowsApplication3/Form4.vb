Option Strict Off
Option Explicit On
Imports VB = Microsoft.VisualBasic
Imports System.IO
Imports System.Net.Sockets
Imports System.Net
Imports System.Threading
Imports System.Text
Imports NTCPMSG.Server
Imports NTCPMSG.Event
Imports NTCPMSG.Serialize
Friend Class Form4
	Inherits System.Windows.Forms.Form

	
	Private Const NORMAL_PRIORITY_CLASS As Integer = &H20
	Private Const STARTF_USESTDHANDLES As Integer = &H100
	Private Const STARTF_USESHOWWINDOW As Integer = &H1
    Dim numftpsession As Short
    Dim scenariostep As Integer = 0
    Public listchange As Boolean = False
    Dim socketCount As Integer
    Dim threadFlag As Object
    Dim MAX_SOCKET_COUNT As Integer = 250
    Dim tcpstate As Boolean = False
    Dim OneWay As UInt32 = 1
    Dim Returns As UInt32 = 2
    Dim PushMessage As UInt32 = 3
    Dim Bin As UInt32 = 4
    Dim port As Integer = 9104
    Public TPmsgpool As New List(Of String)
    Public TPmmsgpool As New List(Of String)
    Public OPmsgpool As New List(Of String)
    Public OPmmsgpool As New List(Of String)
    Public appcloseflag As Boolean = False

    Dim tcpserversession As Thread = Nothing
    Public tcpstarted As Boolean = False
    Dim idleueliststring As String = ""
    Dim paginguelistchanged As Boolean = False
    Dim ueliststate As New Dictionary(Of String, DateTime)
    Structure uesingnal
        Dim cellinfo As String
        Dim RSSI As String
        Dim rsrp As String
        Dim sinr As String

    End Structure
    Dim UEsignalinfo As uesingnal
	Private Structure SECURITY_ATTRIBUTES
		Dim nLength As Integer
		Dim lpSecurityDescriptor As Integer
		Dim bInheritHandle As Integer
	End Structure
	
	Private Structure STARTUPINFO
		Dim cb As Integer
		Dim lpReserved As Integer
		Dim lpDesktop As Integer
		Dim lpTitle As Integer
		Dim dwX As Integer
		Dim dwY As Integer
		Dim dwXSize As Integer
		Dim dwYSize As Integer
		Dim dwXCountChars As Integer
		Dim dwYCountChars As Integer
		Dim dwFillAttribute As Integer
		Dim dwFlags As Integer
		Dim wShowWindow As Short
		Dim cbReserved2 As Short
		Dim lpReserved2 As Integer
		Dim hStdInput As Integer
		Dim hStdOutput As Integer
		Dim hStdError As Integer
	End Structure
	
	Private Structure PROCESS_INFORMATION
		Dim hProcess As Integer
		Dim hThread As Integer
		Dim dwProcessId As Integer
		Dim dwThreadID As Integer
	End Structure
	
	'UPGRADE_WARNING: Structure SECURITY_ATTRIBUTES may require marshalling attributes to be passed as an argument in this Declare statement. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
	Private Declare Function CreatePipe Lib "kernel32" (ByRef phReadPipe As Integer, ByRef phWritePipe As Integer, ByRef lpPipeAttributes As SECURITY_ATTRIBUTES, ByVal nSize As Integer) As Integer
	'UPGRADE_WARNING: Structure PROCESS_INFORMATION may require marshalling attributes to be passed as an argument in this Declare statement. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
	'UPGRADE_WARNING: Structure STARTUPINFO may require marshalling attributes to be passed as an argument in this Declare statement. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
	'UPGRADE_ISSUE: Declaring a parameter 'As Any' is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"'
	'UPGRADE_WARNING: Structure SECURITY_ATTRIBUTES may require marshalling attributes to be passed as an argument in this Declare statement. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
	'UPGRADE_WARNING: Structure SECURITY_ATTRIBUTES may require marshalling attributes to be passed as an argument in this Declare statement. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"'
    Private Declare Function CreateProcess Lib "kernel32" Alias "CreateProcessA" (ByVal lpApplicationName As String, ByVal lpCommandLine As String, ByRef lpProcessAttributes As SECURITY_ATTRIBUTES, ByRef lpThreadAttributes As SECURITY_ATTRIBUTES, ByVal bInheritHandles As Integer, ByVal dwCreationFlags As Integer, ByRef lpEnvironment As Integer, ByVal lpCurrentDriectory As String, ByRef lpStartupInfo As STARTUPINFO, ByRef lpProcessInformation As PROCESS_INFORMATION) As Integer
	Private Declare Function ReadFile Lib "kernel32" (ByVal hFile As Integer, ByVal lpBuffer As String, ByVal nNumberOfBytesToRead As Integer, ByRef lpNumberOfBytesRead As Integer, ByVal lpOverlapped As Integer) As Integer
	Private Declare Function CloseHandle Lib "kernel32" (ByVal hObject As Integer) As Integer
	Private Declare Function OpenProcess Lib "kernel32" (ByVal dwDesiredAccess As Integer, ByVal bInheritHandle As Integer, ByVal dwProcessId As Integer) As Integer
	Private Declare Function GetExitCodeProcess Lib "kernel32" (ByVal hProcess As Integer, ByRef lpExitCode As Integer) As Integer
	
	Const PROCESS_QUERY_INFORMATION As Integer = &H400
	Const STILL_ALIVE As Integer = &H103
	
	Public Sub runshellfile(ByRef commandstr1 As String, ByRef style As Short)
		On Error Resume Next
		Dim pid As Integer
		Dim hProcess As Integer
		Dim exitcode As Integer
		Dim s As String
		
		
		pid = Shell("cmd.exe /c " & commandstr1 & " > d:\temp\1.txt", style)
		
		hProcess = OpenProcess(PROCESS_QUERY_INFORMATION, 0, pid)
		Do 
			wait((1000))
			Call GetExitCodeProcess(hProcess, exitcode)
			System.Windows.Forms.Application.DoEvents()
			FileOpen(1, "d:\temp\1.txt", OpenMode.Binary)
			s = InputString(1, LOF(1))
            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & s & vbCrLf
            FileClose(1)
        Loop While exitcode = STILL_ALIVE
        Call CloseHandle(hProcess)

        'Kill "d:\temp\1.txt"
    End Sub



    Private Sub Command1_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs)

        Dim shellstr As String
        Command1.Enabled = False

        shellstr = "ping 127.0.0.1 -n 10"
        runshellfile(shellstr, 1)

        Command1.Enabled = True

    End Sub

    Private Sub Command10_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs)
        Dim pp As Object
        'Dim aa As Object
        Dim newdirname As String
        Dim UElogdir As String
        Dim runedcip As String
        Try
            setallbuttonstate(0, False)
            runedcip = ":"
            UElogdir = Module1.ReadKeyVal(My.Application.Info.DirectoryPath + "\ueconfig.ini", "dirs", "UElog")
            If scenariostep = 0 Then
                pp = MsgBox("Do you need delete logs in log dir after backup?", vbYesNoCancel)

            Else
                pp = vbYes
            End If


            If pp = vbYes Or pp = vbNo Then
                Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "++++++++++++Backup Logs start++++++++++++++++++++++" & vbCrLf

                setallcursor(0)
                newdirname = Replace(Now().ToString("yyyy-MM-dd-HH:mm:ss"), ":", "-")
                Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "create backup directory" & vbCrLf
                FileSystem.MkDir(UElogdir + "\" + newdirname)
                'runshellfile("mkdir " + UElogdir + "\" + newdirname, vbNormal)
                Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "backup UE log files" & vbCrLf
                CopyDir(UElogdir, UElogdir + "\" + newdirname)
                'runshellfile("copy " + UElogdir + "\*.* " + UElogdir + "\" + newdirname, vbNormal)
                Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "backup MUEcontrol log" & vbCrLf
                Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "copy " + My.Application.Info.DirectoryPath + "\rawdata.log " + UElogdir + "\" + newdirname & vbCrLf
                'runshellfile("copy " + My.Application.Info.DirectoryPath + "\rawdata.log " + UElogdir + "\" + newdirname, vbHide)
                Try
                    FileCopy(My.Application.Info.DirectoryPath + "\rawdata.log", UElogdir + "\" + newdirname + "\rawdata.log")
                    FileCopy(My.Application.Info.DirectoryPath + "\rawOP.log", UElogdir + "\" + newdirname + "\rawOP.log")

                Catch e As Exception
                    Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & e.Message & vbNewLine

                End Try

                If pp = vbYes Then
                    'Timer1.Enabled = False
                    If Not (monitorform.objTimer1 Is Nothing) Then monitorform.objTimer1.Change(-1, 10)
                    monitorform.Timer2.Enabled = False
                    monitorform.Timer1.Enabled = False
                    monitorform.logon = 0
                    For i = 0 To ListView1.Items.Count - 1
                        If ListView1.Items.Item(i).Checked = True And ListView1.Items(i).SubItems(3).Text <> "" And InStr(runedcip, ListView1.Items(i).SubItems(3).Text) < 1 Then
                            runedcip = runedcip & ";" & ListView1.Items(i).SubItems(3).Text
                            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "kill netmeter on board:" & ListView1.Items(i).SubItems(3).Text & vbCrLf
                            ' aa = runftpwagentnewtcp(listview1.Items(i).SubItems(3).Text, "", "", "taskkill -f -im HooNetMeter.exe") 'send stop netmeter command
                            'Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & aa & vbCrLf
                        End If
                    Next
                    If monitorform.loglock = True Then
                        wait(1000)
                    End If
                    Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "delete UE log files" & vbCrLf
                    DeleteDirfiles(UElogdir)
                    'runshellfile("del " + UElogdir + "\*.* /Q /F", vbHide)
                    'stop rawdata.log
                    Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "delete MUEcontrol log file" & vbCrLf
                    Try
                        File.Delete(My.Application.Info.DirectoryPath + "\rawdata.log")
                        File.Delete(My.Application.Info.DirectoryPath + "\rawOP.log")
                    Catch e As Exception
                        Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & e.Message & vbNewLine
                    End Try

                    'delete file
                    runedcip = ";"
                    For i = 1 To ListView1.Items.Count - 1
                        If ListView1.Items.Item(i).Checked = True And ListView1.Items(i).SubItems(3).Text <> "" And InStr(runedcip, ListView1.Items(i).SubItems(3).Text) < 1 Then
                            runedcip = runedcip & ";" & ListView1.Items(i).SubItems(3).Text
                            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "restart netmeter on board:" & ListView1.Items(i).SubItems(3).Text & vbCrLf
                            'aa = runftpwagent(listview1.Items(i).SubItems(3).Text, "", "", "C:\Program Files\HooTech\NetMeter\HooNetMeter.exe") 'send stop netmeter command 'netmeter will run when run ue call killmobilepartner.
                            'Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " &  aa & vbCrLf
                        End If
                    Next
                    If Not (monitorform.objTimer1 Is Nothing) Then monitorform.objTimer1.Change(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(monitorform.updateinterval))
                    'Timer1.Enabled = True
                    monitorform.Timer2.Enabled = True
                    monitorform.Timer1.Enabled = True
                    monitorform.logon = 1
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Arrow
                End If
                Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "++++++++++++Backup Logs end++++++++++++++++++++++" & vbCrLf
                setallcursor(1)
            End If
        Catch e As Exception
            Text1.Text = Text1.Text & "bakup fail: " + e.Message + vbCrLf
        End Try


        setallbuttonstate(1, False)
    End Sub

    Public Function writeueiplog(ByVal logstr As String)
        Dim outputstr, fn As String
        Dim writed As Boolean
        Dim i As Integer

        i = 0
        writed = False
        ' My.Application.DoEvents()

        fn = "D:\uelog\ipaddress"
        Do While writed = False And i < 3
            Try
                Dim fw As System.IO.StreamWriter = New System.IO.StreamWriter(fn, True, System.Text.Encoding.UTF8)


                outputstr = Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + logstr + vbCrLf
                fw.Write(outputstr)

                writed = True
                fw.Close()
                fw.Dispose()
                Return ""
            Catch e As Exception
                writed = False

            End Try
            i = i + 1
        Loop
        Return "OK"

    End Function







    Public Sub Command4_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command4.Click

        Dim tj As Integer
        Dim UElogdir As String
        Dim ueipstr As String
        Dim dropintervalstr As String
        Dim lintervalstr As String
        Dim comportstr As String
        Dim serveripstr, serveripstr2, serveripstr3, serveripstr4 As String
        Dim UEtypestr As String
        Dim logipstr As String
        Dim udpulstr, udpdlstr, tnumberstr As String
        Dim traffictype As String
        Dim ftpsession As String
        Dim sshexename As String
        Dim runmode As String
        Dim i As Integer
        Dim ij As Integer
        '  Any string automatically begins a fully-functional 30-day trial.
        '----------------------------------------------------------------------------------------------------------

        '----------------------------------------------------------------------------------------------------------------------------------------------------------------------

        '  Display the remote shell's command output:
        Dim uecipstr, tpinterval As String
        Dim results As New Collection
        Dim ueidlist As New Collection
        Dim totalcommandstr As String
        Dim uestartlog As String
        Dim totalcommandstrs As New Collection
        Dim ueipstrs As New Collection
        Dim uestartlogs As New Collection
        Dim haverun As String
        'Timer2.Enabled = True
        'Timer3.Enabled = True
        Timer1.Enabled = True
        runmode = ""
        UEtypestr = ""
        MDIForm1.state = "running UEs"
        Try
            'Timer1.Enabled = False
            haverun = ""
            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "++++++++++++start run UEs++++++++++++++++" & vbCrLf
            If MDIForm1.AutobackupToolStripMenuItem.Checked = True And MDIForm1.remoteon = False Then
                Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "backup logs" & vbCrLf
                Command10_Click(Nothing, Nothing)
            End If
            setallbuttonstate(0, False)
            setallcursor(0)
            For ij = 0 To ListView1.Items.Count - 1
                uestartlogs.Clear()
                ueidlist.Clear()
                totalcommandstrs.Clear()
                If ListView1.Items.Item(ij).Checked = True And Not (InStr(haverun, ListView1.Items.Item(ij).SubItems(4).Text) > 0) Then
                    uecipstr = ListView1.Items.Item(ij).SubItems(4).Text
                    If ListView1.Items.Item(ij).SubItems.Count > 1 Then
                        ListView1.Items.Item(ij).SubItems(1).Text = ""
                    Else
                        ' listview1.Items.Item(ij).SubItems.Insert(1, New System.Windows.Forms.ListViewItem.ListViewSubItem(Nothing, ""))
                    End If
                    For i = 0 To ListView1.Items.Count - 1
                        If ListView1.Items.Item(i).SubItems(4).Text = uecipstr And ListView1.Items.Item(i).Checked = True Then
                            '=================================================================================================
                            '--------------action-------------------------------
                            Select Case ListView1.Items.Item(i).SubItems(9).Text '8
                                Case Is = "long-run"
                                    runmode = "" 'default long time run
                                Case Is = "attach-detach"
                                    runmode = " -l" 'attach- detach loop mode
                                Case Is = "paging"
                                    runmode = " -pa"
                                    writeueiplog("new start")
                                Case Is = "attach-idle"
                                    runmode = " -I"
                                Case Is = "idle-active"
                                    runmode = " -A"
                                Case Is = "VOLTEMOC"
                                    runmode = " -V"

                                Case Is = "VOLTEvoiceMOC"
                                    runmode = " -VVC"
                                Case Is = "VOLTEvoiceMTC"
                                    runmode = " -VVT"
                                Case Else

                            End Select
                            '-------------exename----------------------------
                            'sshexename = "c:\python27\python.exe d:\Mueauto\uecontrol.py"
                            sshexename = "d:\mueauto\MUEclient.exe"
                            '-------------uetpype----------------------------
                            Select Case ListView1.Items.Item(i).SubItems(8).Tag
                                Case Is = "Qualcomm8996", "Qualcomm8998"
                                    UEtypestr = " -t Qualcomm9600"
                                Case Is = "SMG9350"
                                    UEtypestr = " -t SMG9350"
                                Case Is = "HE5776", "E5375"
                                    UEtypestr = " -t E5776" 'Huawei E5776
                                Case Is = "hisi"
                                    UEtypestr = " -t H" 'Hisi
                                Case Is = "Qualcomm9600", "SIM7000", "SIM7200", "MC7455", "BG96", "EM7565"
                                    UEtypestr = " -t Qualcomm9600" 'Qualcomm9600
                                Case Is = "YY9027"
                                    UEtypestr = " -t YY9027" 'Qualcomm9028
                                Case Is = "BandluxeC508"
                                    UEtypestr = " -t BandluxeC508"
                                Case Is = "ALT-C186"
                                    UEtypestr = " -t ALT-C186"
                                Case Is = "Dialcommon"
                                    UEtypestr = " -t Dialcommon"
                                Case Else
                            End Select
                            '-------------udppar--------------------
                            udpulstr = " -u " + ListView1.Items.Item(i).SubItems(18).Text
                            udpdlstr = " -y " + ListView1.Items.Item(i).SubItems(19).Text

                            '-------------voltepar------------------
                            tnumberstr = " -h " + ListView1.Items.Item(i).SubItems(20).Text
                            '-------------serverip----------------------------
                            serveripstr = " -s " & ListView1.Items.Item(i).SubItems(6).Tag
                            serveripstr2 = ""
                            serveripstr3 = ""
                            serveripstr4 = ""
                            If ListView1.Items.Item(i).SubItems(15).Text <> "" Then serveripstr2 = " -s2 " & ListView1.Items.Item(i).SubItems(15).Text
                            If ListView1.Items.Item(i).SubItems(16).Text <> "" Then serveripstr3 = " -s3 " & ListView1.Items.Item(i).SubItems(16).Text
                            If ListView1.Items.Item(i).SubItems(17).Text <> "" Then serveripstr4 = " -s4 " & ListView1.Items.Item(i).SubItems(17).Text
                            serveripstr = serveripstr & serveripstr2 & serveripstr3 & serveripstr4
                            '---------------comport-----------------------------
                            comportstr = " -p " & ListView1.Items.Item(i).SubItems(7).Text
                            '-------------Linterval----------------------------
                            lintervalstr = " -i " & ListView1.Items.Item(i).SubItems(11).Text
                            '---------------dropinterval-----------------------------
                            dropintervalstr = " -d " & ListView1.Items.Item(i).SubItems(10).Text

                            '---------------UEcip---------------------------------------
                            uecipstr = ListView1.Items.Item(i).SubItems(4).Text

                            '--------------logip---------------------------------------
                            logipstr = " -w " & ListView1.Items.Item(i).SubItems(14).Text

                            If haverun.IndexOf(Trim(uecipstr)) < 0 Then
                                haverun = haverun & "," & uecipstr '处理过的板卡的UEcip做记录
                                ' totalcommandstr = "cmd /c taskkill"

                                'totalcommandstrs.Add(totalcommandstr)
                            End If

                            traffictype = ListView1.Items.Item(i).SubItems(12).Text
                            Try
                                If Int(ListView1.Items.Item(i).SubItems(13).Text) < 20 Or (traffictype <> "httpdownload" Or traffictype <> "http" Or traffictype <> "ping") Then
                                    ftpsession = Trim(ListView1.Items.Item(i).SubItems(13).Text)
                                Else
                                    ftpsession = "2"
                                End If
                            Catch
                                ftpsession = "2"
                            End Try
                            '-------------------------------------------------clear state info
                            ListView1.Items.Item(i).SubItems(0).Tag = ""
                            ListView1.Items.Item(i).SubItems(1).Text = ""

                            'uecipstrs.Add uecipstr
                            ueipstr = ListView1.Items.Item(i).SubItems(3).Tag
                            ueipstrs.Add(ueipstr)
                            UElogdir = Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\ueconfig.ini", "dirs", "UElog")
                            uestartlog = UElogdir + "\" + ueipstr + ".log"
                            uestartlogs.Add(uestartlog)
                            ueidlist.Add(i + 1) '? shen dongdong
                            '---------------TP report interval-----------------------
                            tpinterval = MDIForm1.TPinterval.Text

                            '---------------total command-----------------------------
                            totalcommandstr = sshexename + runmode + UEtypestr + serveripstr + comportstr + lintervalstr + dropintervalstr + logipstr + udpdlstr + udpulstr + tnumberstr + " -g " + ueipstr + " -n " + ftpsession + " -T " + traffictype + " -a " + tpinterval

                            totalcommandstrs.Add(totalcommandstr)

                            '========================================================================================
                        End If
                    Next

                    Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "Now start run UE on PC:" & ListView1.Items.Item(ij).SubItems(4).Text & vbCrLf
                    results = runUEwagentnewtcp(uecipstr, "lte", "asb#1234", totalcommandstrs)


                    For tj = 1 To uestartlogs.Count()
                        ' Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " &  results.Item(tj) & vbCrLf
                        If ListView1.Items.Item(ueidlist.Item(tj) - 1).SubItems.Count > 1 And uestartlogs.Count = results.Count Then
                            ListView1.Items.Item(ueidlist.Item(tj) - 1).SubItems(1).Text = results.Item(tj)
                        Else
                            If uestartlogs.Count <> results.Count Then
                                Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " run log num <> ue num"
                            End If
                            'listview1.Items.Item(ueidlist.Item(tj) - 1).SubItems.Insert(1, New System.Windows.Forms.ListViewItem.ListViewSubItem(Nothing, results.Item(tj)))
                        End If

                    Next
                End If
            Next
            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "++++++++++++end run UEs++++++++++++++++" & vbCrLf
        Catch e As Exception
            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & e.Message & vbCrLf
            Text1.Text = Text1.Text & "error at run UE part"
            MDIForm1.state = "idle"
        End Try
        MDIForm1.state = "idle"
        setallbuttonstate(1, False)
        setallcursor(1)
        ' Timer1.Enabled = True
    End Sub
    Public Sub Command2_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command2.Click
        Dim i As Object
        For i = 1 To ListView1.Items.Count
            ListView1.Items.Item(i - 1).Checked = True
        Next
    End Sub

    Public Sub selectoneUE()
        Dim i As Object
        MDIForm1.singleUEselect = True
        For i = 1 To ListView1.Items.Count
            If ListView1.Items.Item(i - 1).SubItems(7).text = targetUE Then
                ListView1.Items.Item(i - 1).checked = True
            Else
                ListView1.Items.Item(i - 1).checked = False

            End If

        Next
    End Sub

    Private Sub Command3_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command3.Click
        Dim i As Object
        For i = 0 To ListView1.Items.Count - 1
            ListView1.Items.Item(i).Checked = False
        Next
    End Sub

    Private Sub Command5_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs)
        Dim i As Object
        For i = 0 To ListView1.Items.Count - 1
            'UPGRADE_WARNING: Couldn't resolve default property of object i. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
            'UPGRADE_WARNING: Lower bound of collection listview1.ListItems has changed from 1 to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A3B628A0-A810-4AE2-BFA2-9E7A29EB9AD0"'
            'UPGRADE_WARNING: Lower bound of collection listview1.ListItems() has changed from 1 to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A3B628A0-A810-4AE2-BFA2-9E7A29EB9AD0"'
            If ListView1.Items.Item(i).SubItems(1).Text = "Running" Then
                'UPGRADE_WARNING: Couldn't resolve default property of object i. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Lower bound of collection listview1.ListItems has changed from 1 to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A3B628A0-A810-4AE2-BFA2-9E7A29EB9AD0"'
                ListView1.Items.Item(i).Checked = False
            Else
                'UPGRADE_WARNING: Couldn't resolve default property of object i. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                'UPGRADE_WARNING: Lower bound of collection listview1.ListItems has changed from 1 to 0. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A3B628A0-A810-4AE2-BFA2-9E7A29EB9AD0"'
                ListView1.Items.Item(i).Checked = True
            End If
        Next

    End Sub

    Public Sub Command7_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command7.Click
        'Dim UElogdir As Object
        Dim ueipstr As Object
        Dim dropintervalstr As Object
        Dim lintervalstr As Object
        Dim comportstr As Object
        Dim serveripstr As Object
        Dim UEtypestr As Object
        Dim sshexename As Object
        Dim runmode As Object
        Dim i As Object
        Dim uecipstr As String
        Dim result As String
        Dim totalcommandstrs, uestartlogs, ueidlist As New Collection
        Dim results As New Collection
        Dim totalcommandstr As String
        ' Dim uestartlog As String
        Dim logipstr As String
        Dim ftpsession As String
        Dim traffictype As String
        Dim ueciplist As String
        Dim searchuecip As String
        Dim jj As Integer
        Dim shutdownuenamelist As New List(Of String)
        uecipstr = ""
        Timer2.Enabled = False
        Timer3.Enabled = False
        ' Timer1.Enabled = False
        setallbuttonstate(0, False)
        setallcursor(0)
        Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "++++++++++++stop run UEs++++++++++++++++" & vbCrLf
        ueciplist = ""
        MDIForm1.state = "stopping UEs"
        For i = 1 To ListView1.Items.Count
            uestartlogs.Clear()
            ueidlist.Clear()
            ' If listview1.Items.Item(i - 1).SubItems.Count > 1 Then '把ue状态设为空
            'listview1.Items.Item(i - 1).SubItems(1).Text = ""
            'Else
            'End If
            If ListView1.Items.Item(i - 1).Checked = True And ueciplist.IndexOf(ListView1.Items.Item(i - 1).SubItems(4).Text) < 0 Then '查找新的被checked uecip
                searchuecip = ListView1.Items.Item(i - 1).SubItems(4).Text
                ueciplist = ueciplist + "," + searchuecip
                '---------------search 所有uecipstr的行数
                jj = i
                If jj <= ListView1.Items.Count Then
                    For jj = i - 1 To ListView1.Items.Count - 1
                        If ListView1.Items.Item(jj).Checked = True And ListView1.Items.Item(jj).SubItems(4).Text = searchuecip Then  '查找新的被checked uecip当前再查的ue
                            '--------------action-------------------------------

                            runmode = " -c" 'shutdown

                            '-------------exename----------------------------
                            sshexename = "d:\mueauto\MUEclient.exe"
                            '-------------uetpype----------------------------
                            UEtypestr = "HE5776"


                            Select Case ListView1.Items.Item(jj).SubItems(8).Tag
                                Case Is = "Qualcomm8996", "Qualcomm8998"
                                    UEtypestr = " -t Qualcomm9600"
                                Case Is = "SMG9350"
                                    UEtypestr = " -t SMG9350"
                                Case Is = "E5776", "E5375"
                                    UEtypestr = " -t E5776" 'Huawei E5776
                                Case Is = "hisi"
                                    UEtypestr = " -t H" 'Hisi
                                Case Is = "Qualcomm9600", "SIM7000", "SIM7200", "MC7455", "BG96", "EM7565"
                                    UEtypestr = " -t Qualcomm9600" 'Qualcomm9600
                                Case Is = "YY9027"
                                    UEtypestr = " -t YY9027" 'Qualcomm9028
                                Case Is = "BandluxeC508"
                                    UEtypestr = " -t BandluxeC508"
                                Case Is = "ALT-C186"
                                    UEtypestr = " -t ALT-C186"
                                Case Is = "Dialcommon"
                                    UEtypestr = " -t Dialcommon"
                                Case Else
                            End Select
                            Try
                                If Int(ListView1.Items.Item(jj).SubItems(13).Text) < 20 Then
                                    ftpsession = Trim(ListView1.Items.Item(jj).SubItems(13).Text)
                                Else
                                    ftpsession = "2"
                                End If
                            Catch
                                ftpsession = "2"
                            End Try
                            traffictype = ListView1.Items.Item(jj).SubItems(12).Text
                            '-------------serverip----------------------------
                            serveripstr = " -s " & ListView1.Items.Item(jj).SubItems(6).Tag
                            '---------------comport-----------------------------
                            comportstr = " -p " & ListView1.Items.Item(jj).SubItems(7).Text
                            '-------------Linterval----------------------------
                            lintervalstr = " -i " & ListView1.Items.Item(jj).SubItems(11).Text
                            '---------------dropinterval-----------------------------
                            dropintervalstr = " -d " & ListView1.Items.Item(jj).SubItems(10).Text

                            '---------------UEcip---------------------------------------
                            uecipstr = ListView1.Items.Item(jj).SubItems(4).Text

                            '--------------logip---------------------------------------
                            logipstr = " -w " & ListView1.Items.Item(jj).SubItems(14).Text
                            '---------------UEip---------------------------------------

                            ueipstr = ListView1.Items.Item(jj).SubItems(3).Tag
                            shutdownuenamelist.Add(ueipstr)
                            '---------------------------------------------------------

                            ueidlist.Add(jj + 1)
                            '---------------total command-----------------------------
                            totalcommandstr = sshexename + runmode + UEtypestr + serveripstr + comportstr + lintervalstr + dropintervalstr + logipstr + " -g " + ueipstr + " -n " + ftpsession + " -T " + traffictype

                            totalcommandstrs.Add(totalcommandstr)

                            ' If listview1.Items.Item(jj).SubItems.Count > 1 Then
                            'listview1.Items.Item(jj).SubItems(1).Text = "Stoped"
                            'Else
                            'End If

                        End If


                    Next
                    If totalcommandstrs.Count > 0 Then
                        '  UElogdir = Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\ueconfig.ini", "dirs", "UElog")
                        ' uestartlog = UElogdir + "\" + ueipstr + ".log"
                        If MDIForm1.singleUEselect = False Then
                            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "Stop running UE on control board:" & uecipstr & vbCrLf
                            result = stoprunUEwagentnewtcp(uecipstr)
                            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & result & vbCrLf

                        Else
                            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "Stop running UE on control board:" & uecipstr & vbCrLf
                            newstopUEwagentnewtcp(uecipstr, "lte", "asb#1234", totalcommandstrs)
                            Application.DoEvents()
                        End If
                        Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "shutdown UE on control board:" & uecipstr & vbCrLf
                        results = runUEwagentnewtcp(uecipstr, "lte", "asb#1234", totalcommandstrs)


                        For tj = 1 To ueidlist.Count()
                            ' Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " &  results.Item(tj) & vbCrLf
                            If ListView1.Items.Item(ueidlist.Item(tj) - 1).SubItems.Count > 1 And ueidlist.Count = results.Count Then
                                ListView1.Items.Item(ueidlist.Item(tj) - 1).SubItems(1).Text = results.Item(tj)
                            Else
                                Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " ueid num <> results num" & vbCrLf

                                'listview1.Items.Item(ueidlist.Item(tj) - 1).SubItems.Insert(1, New System.Windows.Forms.ListViewItem.ListViewSubItem(Nothing, results.Item(tj)))
                            End If

                        Next
                        totalcommandstrs.Clear()
                    End If
                End If
                '----------------search 所有uecipstr的行数



            End If




        Next



        '----------------------------------------------------------------------------
        For Each uename As String In shutdownuenamelist
            clearTPinfo(uename)

        Next



        Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "++++++++++++end stop run UEs++++++++++++++++" & vbCrLf
        Command7.Enabled = True
        setallbuttonstate(1, True)
        Runscenario.Enabled = True
        setallcursor(1)
        'Timer2.Enabled = True
        'Timer3.Enabled = True
        Timer1.Enabled = True
        MDIForm1.state = "idle"
        MDIForm1.ToolStripStatusLabel1.Text = ""
    End Sub

    Private Sub Command8_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs)

        Dim i As Object
        Dim uecipstr As String
        Dim result As String
        Dim ueciplist As String
        ueciplist = ""
        Text1.Text = ""
        For i = 1 To ListView1.Items.Count
            If ListView1.Items.Item(i - 1).SubItems.Count > 1 Then
                ListView1.Items.Item(i - 1).SubItems(1).Text = ""
            Else
                ListView1.Items.Item(i - 1).SubItems.Insert(1, New System.Windows.Forms.ListViewItem.ListViewSubItem(Nothing, ""))
            End If
            If ListView1.Items.Item(i - 1).Checked = True Then
                '---------------UEcip---------------------------------------
                uecipstr = ListView1.Items.Item(i - 1).SubItems(4).Text
                result = ""
                If InStr(ueciplist, uecipstr) < 1 Then
                    ueciplist = ueciplist + ";" + uecipstr
                    ListView1.Items.Item(i - 1).SubItems(1).Text = "restart board"
                    Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "restart board:" & ListView1.Items.Item(i - 1).SubItems(3).Text & vbCrLf
                    result = runftpwagentnewtcp(uecipstr, "", "", "shutdown -r -f -t 10")

                    Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & result & vbCrLf
                    ListView1.Items.Item(i - 1).SubItems(1).Text = result
                End If

            End If
        Next

    End Sub

    Public Sub Command9_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Command9.Click
        Dim i As Object
        Dim uecipstr As String
        Dim result As String
        Dim ueciplist As String
        ueciplist = ""
        Text1.Text = ""
        MDIForm1.state = "shuting down boards"
        For i = 1 To ListView1.Items.Count
            If ListView1.Items.Item(i - 1).SubItems.Count > 1 Then
                ListView1.Items.Item(i - 1).SubItems(1).Text = ""
            Else
                ListView1.Items.Item(i - 1).SubItems.Insert(1, New System.Windows.Forms.ListViewItem.ListViewSubItem(Nothing, ""))
            End If
            If ListView1.Items.Item(i - 1).Checked = True Then
                '---------------UEcip---------------------------------------
                uecipstr = ListView1.Items.Item(i - 1).SubItems(4).Text
                result = ""
                If InStr(ueciplist, uecipstr) < 1 Then
                    ueciplist = ueciplist + ";" + uecipstr
                    ListView1.Items.Item(i - 1).SubItems(1).Text = "shutdown board"
                    Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "shutdown board:" & ListView1.Items.Item(i - 1).SubItems(4).Text & vbCrLf
                    result = runftpwagentnewtcp(uecipstr, "", "", "shutdown -s -f -t 10")

                    Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & result & vbCrLf
                    ListView1.Items.Item(i - 1).SubItems(1).Text = result
                End If

            End If
        Next
        MDIForm1.state = "idle"
    End Sub

    Private Sub Form4_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        If System.IO.File.Exists(My.Application.Info.DirectoryPath + "\scenarios.xml") Then
            DataSet1.ReadXml(My.Application.Info.DirectoryPath + "\scenarios.xml")
        End If

        initform(My.Application.Info.DirectoryPath + "\ueconfig.ini")

    End Sub


    Private Sub Form4_FormClosed(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        DataSet1.WriteXml(My.Application.Info.DirectoryPath + "\scenarios.xml")
        Dim closetcpclient As New TcpClient
        appcloseflag = True
        'closetcpclient.Connect("127.0.0.1", 9104)
        'Dim message As String = ""
        'Dim data As [Byte]() = System.Text.Encoding.ASCII.GetBytes(Message)

        '' Get a client stream for reading and writing.
        ''  Stream stream = client.GetStream();
        'Dim stream As NetworkStream = closetcpclient.GetStream()

        '' Send the message to the connected TcpServer. 
        'stream.Write(data, 0, data.Length)
        'Thread.Sleep(1000)
        'ConsoleHelper.FreeConsole()


    End Sub


    Private Sub runshell(ByRef commandstring As String)
        Dim Proc As PROCESS_INFORMATION '进程信息
        Dim Start As STARTUPINFO '启动信息
        Dim SecAttr As SECURITY_ATTRIBUTES '安全属性
        Dim hReadPipe As Integer '读取管道句柄
        Dim hWritePipe As Integer '写入管道句柄
        Dim lngBytesRead As Integer '读出数据的字节数
        Dim strBuffer As String '* 256000000 '读取管道的字符串buffer
        'UPGRADE_NOTE: command was upgraded to command_Renamed. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
        Dim command_Renamed As String 'DOS命令
        Dim ret As Integer 'API函数返回值
        Dim lpOutputs As String '读出的最终结果
        Dim h As Short '读取指针
        strBuffer = ""
        lpOutputs = ""
        '设置安全属性
        With SecAttr
            'UPGRADE_ISSUE: LenB function is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"'
            .nLength = Len(SecAttr)
            .bInheritHandle = True
            .lpSecurityDescriptor = 0
        End With

        '创建管道
        ret = CreatePipe(hReadPipe, hWritePipe, SecAttr, 0)
        If ret = 0 Then
            MsgBox("无法创建管道", MsgBoxStyle.Exclamation, "错误")
            Exit Sub
        End If

        '设置进程启动前的信息
        With Start
            'UPGRADE_ISSUE: LenB function is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"'
            .cb = Len(Start)
            .dwFlags = STARTF_USESHOWWINDOW Or STARTF_USESTDHANDLES
            .hStdOutput = hWritePipe '设置输出管道
            .hStdError = hWritePipe '设置错误管道
        End With

        '启动进程
        command_Renamed = commandstring 'DOS进程以ipconfig.exe为例
        ret = CreateProcess(vbNullString, command_Renamed, SecAttr, SecAttr, True, NORMAL_PRIORITY_CLASS, 0, vbNullString, Start, Proc)
        If ret = 0 Then
            MsgBox("无法启动新进程", MsgBoxStyle.Exclamation, "错误")
            ret = CloseHandle(hWritePipe)
            ret = CloseHandle(hReadPipe)
            Exit Sub
        End If

        '因为无需写入数据，所以先关闭写入管道。而且这里必须关闭此管道，否则将无法读取数据
        ret = CloseHandle(hWritePipe)

        '从输出管道读取数据，每次最多读取256字节
        h = 1
        Do
            wait(10)
            ret = ReadFile(hReadPipe, strBuffer, 256, lngBytesRead, 0)
            lpOutputs = lpOutputs & VB.Right(strBuffer, lngBytesRead)

            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & strBuffer & vbCrLf  'Right(strBuffer, lngBytesRead)
            System.Windows.Forms.Application.DoEvents()
            'h = h + 1
        Loop While (ret <> 0) And (lngBytesRead <> 0) '当ret=0时说明ReadFile执行失败，已经没有数据可读了

        '读取操作完成，关闭各句柄
        ret = CloseHandle(Proc.hProcess)
        ret = CloseHandle(Proc.hThread)
        ret = CloseHandle(hReadPipe)

        'MsgBox lpOutputs, vbInformation, "结果"

    End Sub

    Public Sub wait(ByRef ms As Short)

        Dim Start As Integer
        Start = VB.Timer()
        Do While VB.Timer() < Start + ms / 1000 '   3   秒的延时
            System.Windows.Forms.Application.DoEvents() '转让控制权
        Loop

    End Sub

    Private Sub listview1_ItemCheck(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.ItemCheckEventArgs)

    End Sub



    'UPGRADE_WARNING: Event Text1.TextChanged may fire when form is initialized. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"'
    Private Sub Text1_TextChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs)
        Text1.SelectionStart = Len(Text1.Text)
        Text1.ScrollToCaret()
    End Sub

    Private Function runUEwagent(ByRef UEcip As String, ByRef logname As String, ByRef pass As String, ByRef command_Renamed As Collection) As Collection
        Dim i As Integer
        Dim h As Integer
        Dim result As String
        Dim results As New Collection

        Dim receivedata As Object
        receivedata = Nothing
        Try
            'tcpclient.Close()
            'tcpclient.RemoteHost = UEcip
            'tcpclient.RemotePort = 33891
            'tcpclient.Connect()
            wait(2000)
            If 1 Then 'tcpclient.CtlState = MSWinsockLib.StateConstants.sckConnected Then
                result = "Connected"

            End If


            If 1 Then 'tcpclient.CtlState <> MSWinsockLib.StateConstants.sckConnected Then
                result = "Connected fail"
                Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & result & vbCrLf
                'tcpclient.Close()
                For h = 1 To command_Renamed.Count()
                    results.Add(result)
                Next
                runUEwagent = results
                Exit Function
            End If

            'tcpclient.SendData("run=" & "d:\mueauto\killall.bat")
            wait(1000)
            'tcpclient.SendData("run=" & "d:\mueauto\killmobilepartner.bat")

            wait(2000)

            If 1 Then 'tcpclient.CtlState <> MSWinsockLib.StateConstants.sckConnected Then
                result = "Connected fail"
                For h = 1 To command_Renamed.Count()
                    results.Add(result)
                Next
                runUEwagent = results
                Exit Function
            End If

            For i = 1 To command_Renamed.Count()
                If command_Renamed.Item(i).ToString.IndexOf(" -c") >= 0 Then
                    Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "shutdown UE:" & Mid(command_Renamed.Item(i).ToString, command_Renamed.Item(i).ToString.IndexOf("-g ") + 3, command_Renamed.Item(i).ToString.IndexOf("-n ") - command_Renamed.Item(i).ToString.IndexOf("-g ") - 3) & vbCrLf
                Else
                    If command_Renamed.Item(i).ToString.IndexOf("copy") >= 0 Then

                    Else
                        If command_Renamed.Item(i).ToString.IndexOf("-n ") >= 0 Then
                            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "run UE:" & Mid(command_Renamed.Item(i).ToString, command_Renamed.Item(i).ToString.IndexOf("-g ") + 3, command_Renamed.Item(i).ToString.IndexOf("-n ") - command_Renamed.Item(i).ToString.IndexOf("-g ") - 3) & vbCrLf
                        Else
                            'Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "run UE:" & Mid(command_Renamed.Item(i).ToString, command_Renamed.Item(i).ToString.IndexOf("-g ") + 3, command_Renamed.Item(i).ToString.Length - command_Renamed.Item(i).ToString.IndexOf("-g ") - 3) & vbCrLf
                        End If

                    End If
                End If
                'tcpclient.SendData("run=" & command_Renamed.Item(i))
                wait(1000)
                ''tcpclient.GetData(receivedata, vbString, 1500)
                'Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & receivedata & vbCrLf
                result = ""
                'If Trim(receivedata).IndexOf(">") >= 0 Then

                If command_Renamed.Item(i).ToString.IndexOf(" -c") >= 0 Then
                    result = "shutdown"
                Else
                    result = "Running"
                End If
                Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & result & " command success" & vbCrLf
                'End If
                results.Add(result)

                'tcpclient.SendData("run=" + "d:\mueauto\logcommand.bat " + command_Renamed.Item(i))
                wait(2000)
                ''tcpclient.GetData(receivedata, vbString, 1500)
                'Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & receivedata & vbCrLf
            Next
            'tcpclient.Close()
            runUEwagent = results
        Catch ex As Exception
            'tcpclient.Close()
            results.Add(ex.ToString)
            Return results
        End Try

    End Function

    Private Sub Winsock1_DataArrival(ByVal bytesTotal As Integer)
        Dim strData As String
        strData = ""
        'tcpclient.GetData(strData)
        Text1.Text = strData & vbCrLf
    End Sub

    Private Function checkrunningstate(ByRef FileName As String) As String
        Dim j As Object
        Dim i As Object
        Dim Buff As String
        Dim LineBuff() As String
        Dim START As Long

        Try
            checkrunningstate = ""
            If File.Exists(FileName) Then
                FileOpen(1, FileName, OpenMode.Binary, OpenAccess.Read, OpenShare.Shared)
                START = LOF(1)
                Buff = Space(100000)
                If START > 100000 Then
                    START = START - 100000
                Else
                    START = 1
                End If
                'UPGRADE_WARNING: Get was upgraded to FileGet and has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
                FileGet(1, Buff, START)
                FileClose(1)

                LineBuff = Split(Buff, vbCrLf) '这里的vbcrlf需要具体拿到你的文件的二进制格式才能确认是什么

                'Label1.Caption = LineBuff(104500)    '要哪一行,就直接取得

                i = UBound(LineBuff) 'i用来控制行数

                j = 1 'j用来控制是否找到


                While j = 1 And i > 0
                    If InStr(LineBuff(i), "UE shutdowned") > 0 And j = 1 Then '查找掉话字串 "start OK"表示没有串口问题
                        checkrunningstate = "Shutdown"
                        j = 0
                    End If
                    If InStr(LineBuff(i), "start OK") > 0 And j = 1 Then '查找掉话字串 "start OK"表示没有串口问题
                        checkrunningstate = "Running"
                        j = 0
                    End If
                    If InStr(LineBuff(i), "new start") > 0 And j = 1 Then '每次run开始第一行打印new start，找到最开头了。
                        checkrunningstate = "Not start"
                        j = 0
                    End If
                    If InStr(LineBuff(i), "port fail") > 0 And j = 1 Then 'com fail
                        checkrunningstate = "Com port fail"
                        j = 0
                    End If
                    If InStr(LineBuff(i), "ip not find") > 0 And j = 1 Then 'com fail
                        checkrunningstate = "UE no IP"
                        j = 0
                    End If
                    If InStr(LineBuff(i), "ping OK") > 0 And j = 1 Then 'com fail
                        checkrunningstate = "ping OK"
                        j = 0
                    End If
                    If InStr(LineBuff(i), "call drop") > 0 And j = 1 Then 'com fail
                        checkrunningstate = "call drop"
                        j = 0
                    End If
                    i = i - 1
                End While


                If j = 0 Then

                    Dim getstatetime, nowtime As DateTime
                    Dim isoldlog As Boolean = True

                    getstatetime = Convert.ToDateTime(Mid(LineBuff(i + 1), 1, LineBuff(i + 1).IndexOf(" ", LineBuff(i + 1).IndexOf(" ", 1) + 1)))

                    nowtime = Now
                    If DateDiff(DateInterval.Minute, getstatetime, nowtime) > 10 Then '与当前时间超过10分钟的log无效
                        checkrunningstate = "No state in log"
                    End If

                Else
                    checkrunningstate = "No state in log"

                End If
                Erase LineBuff
            Else
                checkrunningstate = "no log file"
            End If


        Catch e2 As Exception
            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & e2.Message & vbCrLf
            Text1.Text = Text1.Text & "error at read ue state log part" & vbCrLf

            Return "error get file"
        End Try

    End Function

    Private Function stoprunUEwagent(ByRef UEcip As String) As String
        Dim receivedata As String
        receivedata = ""
        Try
            'tcpclient.Protocol = MSWinsockLib.ProtocolConstants.sckTCPProtocol
            'tcpclient.RemoteHost = UEcip
            'tcpclient.RemotePort = 33891
            'tcpclient.Connect()
            wait(2000)
            'UPGRADE_NOTE: State was upgraded to CtlState. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
            If 1 Then 'tcpclient.CtlState = MSWinsockLib.StateConstants.sckConnected Then


                stoprunUEwagent = "Connected"

            End If

            'kill python---------------------------------------------------------------
            'UPGRADE_NOTE: State was upgraded to CtlState. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
            If 1 Then 'tcpclient.CtlState <> MSWinsockLib.StateConstants.sckConnected Then

                stoprunUEwagent = "Connected fail"
                'tcpclient.Close()
                Exit Function
            End If

            'tcpclient.SendData("run=" & "d:\mueauto\killall.bat") 'command
            wait(2000)
            'tcpclient.GetData(receivedata, vbString, 1500)
            ' Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " &  receivedata & vbCrLf
            If receivedata.IndexOf(">") >= 0 Then
                stoprunUEwagent = receivedata + "send command OK"
            Else
                stoprunUEwagent = "taskkill fail"
            End If


            'tcpclient.Close()
        Catch ex As Exception
            'tcpclient.Close()
            Return ex.ToString
        End Try



    End Function
    'UPGRADE_NOTE: command was upgraded to command_Renamed. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
    Public Function runftpwagent(ByRef UEcip As String, ByRef logname As String, ByRef pass As String, ByRef command_Renamed As String) As String
        Dim receivedata As Object
        receivedata = Nothing
        Try
            'tcpclient.Close()
            'tcpclient.Protocol = MSWinsockLib.ProtocolConstants.sckTCPProtocol
            'tcpclient.RemoteHost = UEcip
            'tcpclient.RemotePort = 33891
            'tcpclient.Connect()
            wait(2000)
            'UPGRADE_NOTE: State was upgraded to CtlState. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
            If 1 Then 'tcpclient.CtlState = MSWinsockLib.StateConstants.sckConnected Then


                runftpwagent = "Connected"

            End If

            'run ftp---------------------------
            'UPGRADE_NOTE: State was upgraded to CtlState. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"'
            If 1 Then 'tcpclient.CtlState <> MSWinsockLib.StateConstants.sckConnected Then

                runftpwagent = "Connected fail"
                Exit Function
            End If
            'tcpclient.SendData("run=" & command_Renamed)
            wait(2000)
            'tcpclient.GetData(receivedata, vbString, 1500)
            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & receivedata & vbCrLf
            If receivedata <> ">" Then
                runftpwagent = receivedata
            Else
                runftpwagent = "ok"
            End If


            'tcpclient.Close()

        Catch ex As Exception
            'tcpclient.Close()
            Return ex.ToString
        End Try


    End Function

    Public Shared Function CharByte(ByVal vstrWord As String) As Integer
        'aStr字串之第一個字的位元組長度
        If Len(vstrWord) = 0 Then
            Return 0
        Else
            Select Case Asc(vstrWord)
                Case 0 To 255
                    Return 1
                Case Else
                    Return 2
            End Select
        End If
    End Function
    Public Shared Function StrLenB(ByVal vstrValue As String) As Integer
        '如同VB 6.0的LenB函數，傳回字串aStr的位元組長度
        Dim i, k As Integer
        For i = 1 To Len(vstrValue)
            k += CharByte(Mid(vstrValue, i, 1))
        Next
        Return k
    End Function

    Private Sub listview1_ItemChecked(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckedEventArgs)
        Dim Item As System.Windows.Forms.ListViewItem
        Dim i As Integer
        Dim UEcip As String
        Dim mode As Boolean
        mode = MDIForm1.singleUEselect
        If mode = False Then
            Item = e.Item
            MDIForm1.ToolStripStatusLabel1.Text = "Checking same board UE check state"
            Try
                If Item.SubItems.Count > 3 And ListView1.Items.Count > 0 Then
                    UEcip = Item.SubItems(3).Text
                    For i = 0 To ListView1.Items.Count - 1

                        If ListView1.Items.Item(i).SubItems(3).Text = UEcip And i <> Item.Index Then
                            If Item.Checked = True And ListView1.Items.Item(i).Checked = False Then
                                ListView1.Items.Item(i).Checked = True
                            End If
                            If Item.Checked = False And ListView1.Items.Item(i).Checked = True Then
                                ListView1.Items.Item(i).Checked = False
                            End If
                        End If

                        System.Windows.Forms.Application.DoEvents()
                    Next
                End If
            Catch
            End Try
            MDIForm1.ToolStripStatusLabel1.Text = ""
        End If
    End Sub

    Private Sub listview1_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)

    End Sub


    Private Sub Command6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub
    Private Sub setallcursor(ByVal state As Integer)

        Dim t As Control
        If state = 0 Then
            Me.Cursor = Cursors.WaitCursor
        End If

        If state = 1 Then
            Me.Cursor = Cursors.Default
        End If

        For Each t In Me.Controls
            If state = 0 Then t.Cursor = Cursors.WaitCursor
            If state = 1 Then t.Cursor = Cursors.Default
        Next t

    End Sub
    Private Sub setallbuttonstate(ByVal state As Integer, ByVal scenario As Boolean)
        Dim t As Control
        For Each t In Me.Controls
            If t.Name <> "Text1" And t.Name <> "listview1" Then
                If t.Name <> "Command7" Then
                    If state = 0 Then t.Enabled = False
                    If state = 1 Then t.Enabled = True
                ElseIf scenario = False Then
                    If state = 0 Then t.Enabled = False
                    If state = 1 Then t.Enabled = True
                End If
            End If
        Next t
    End Sub

    'Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
    '    ' Dim result As String
    '    'Dim uelogdir As String
    '    'Try
    '    'uelogdir = Module1.ReadKeyVal(My.Application.Info.DirectoryPath + "\ueconfig.ini", "dirs", "UElog")
    '    'For tj = 1 To ListView1.Items.Count
    '    'result = checkrunningstate(uelogdir + "\" + ListView1.Items.Item(tj - 1).SubItems(2).Text + ".log")
    '    'ListView1.Items.Item(tj - 1).SubItems(1).Text = result
    '    'Next
    '    'Catch e1 As Exception
    '    'Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & e1.Message & vbCrLf
    '    'Text1.Text = Text1.Text & "error at period check part"
    '    'End Try
    'End Sub

    ' ======================================================

    ' 实现一个静态方法将指定文件夹下面的所有内容Detele

    ' 测试的时候要小心*作，删除之后无法恢复。

    ' ======================================================

    Public Shared Sub DeleteDirfiles(ByVal aimPath As String)





        ' 检查目标目录是否以目录分割字符结束如果不是则添加之

        If (aimPath(aimPath.Length - 1) <> Path.DirectorySeparatorChar) Then

            aimPath += Path.DirectorySeparatorChar

        End If



        '判断待删除的目录是否存在,不存在则退出.

        If (Not Directory.Exists(aimPath)) Then Exit Sub



        ' 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组

        ' 如果你指向Delete目标文件下面的文件而不包含目录请使用下面的方法

        ' string[] fileList = Directory.GetFiles(aimPath);

        Dim fileList() As String = Directory.GetFileSystemEntries(aimPath)



        ' 遍历所有的文件和目录

        For Each FileName As String In fileList

            If (Directory.Exists(FileName)) Then

                ' 先当作目录处理如果存在这个目录就递归Delete该目录下面的文件

                '    DeleteDir(aimPath + Path.GetFileName(FileName))

            Else

                ' 否则直接Delete文件
                Try
                    File.Delete(aimPath + Path.GetFileName(FileName))
                Catch ex As Exception

                    ' MessageBox.Show(ex.ToString())

                End Try
            End If

        Next

        '删除文件夹

        'System.IO.Directory.Delete(aimPath, True)


    End Sub


    ' ======================================================

    ' 实现一个静态方法将指定文件夹下面的所有内容copy到目标文件夹下面

    ' 如果目标文件夹为只读属性就会报错。

    ' ======================================================

    Public Shared Sub CopyDir(ByVal srcPath As String, ByVal aimPath As String)

        Try

            ' 检查目标目录是否以目录分割字符\结束,如果不是则添加之

            If aimPath(aimPath.Length - 1) <> Path.DirectorySeparatorChar Then

                aimPath += Path.DirectorySeparatorChar

            End If



            '判断源目录是否存在,不存在则退出.

            If (Not Directory.Exists(srcPath)) Then Exit Sub



            ' 判断目标目录是否存在如果不存在则新建之

            If (Not Directory.Exists(aimPath)) Then Directory.CreateDirectory(aimPath)



            ' 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组

            ' 如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法

            ' string[] fileList = Directory.GetFiles(srcPath);

            Dim fileList() As String = Directory.GetFileSystemEntries(srcPath)



            ' 遍历所有的文件和目录



            For Each FileName As String In fileList

                ' 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件

                If Directory.Exists(FileName) Then

                    'CopyDir(FileName, aimPath + Path.GetFileName(FileName))

                    ' 否则直接Copy文件

                Else

                    File.Copy(FileName, aimPath + Path.GetFileName(FileName), True)

                End If

            Next



        Catch ex As Exception

            ' MessageBox.Show(ex.ToString())

        End Try

    End Sub
    Public Sub initform(ByVal configfilename As String)
        Dim UElogdir As String
        Dim UEnumbers As String
        Dim i As Short
        Dim oneueconfig As String
        Dim objRWINI As New cRWINI
        ' ConsoleHelper.openconsole(Me.Handle, Me.Width, Me.Height)
        UEconfigurations = ""
        'UElogdir = Module1.ReadKeyVal(configfilename, "dirs", "UElog")
        UElogdir = objRWINI.ReadKeyVal(configfilename, "dirs", "UElog")
        'UEnumbers = CStr(Module1.TotalSections(configfilename))
        UEnumbers = CStr(objRWINI.TotalSections(configfilename))

        ListView1.Items.Clear() '清空列表
        ListView1.Columns.Clear() '清空列表头
        ListView1.View = System.Windows.Forms.View.Details '设置列表显示方式
        ListView1.GridLines = True '显示网络线
        ListView1.LabelEdit = False '禁止标签编辑
        ListView1.FullRowSelect = True '选择整行


        ListView1.Columns.Add("", "UE ID", 50) '给列表中添加列名
        ListView1.Columns.Add("", "UE state", 100)
        ListView1.Columns.Add("", "Cell info", 110)
        ListView1.Columns.Add("", "UE IP", 110)
        ListView1.Columns.Add("", "UE control IP", 110)
        ListView1.Columns.Add("", "Throughput", 250)
        ListView1.Columns.Add("", "server IP", 110)
        ListView1.Columns.Add("", "UE COM port", 50)
        ListView1.Columns.Add("", "UE type", 100)
        ListView1.Columns.Add("", "UE action", 100)
        ListView1.Columns.Add("", "drop detect interval", 50)
        ListView1.Columns.Add("", "attach-detach interval", 50)
        ListView1.Columns.Add("", "traffic type", 100)
        ListView1.Columns.Add("", "Ftp session", 50)
        ListView1.Columns.Add("", "Log ip", 110)
        ListView1.Columns.Add("", "server IP2", 110)
        ListView1.Columns.Add("", "server IP3", 110)
        ListView1.Columns.Add("", "server IP4", 110)
        ListView1.Columns.Add("", "udpul", 110)
        ListView1.Columns.Add("", "udpdl", 110)
        ListView1.Columns.Add("", "Volte target number", 110)
        MDIForm1.ToolStripStatusLabel1.Text = "Initializing run page UE list "



        If CDbl(UEnumbers) - 2 < 0 Then
            MDIForm1.ToolStripProgressBar1.Maximum = 1
        Else
            MDIForm1.ToolStripProgressBar1.Maximum = CDbl(UEnumbers) - 2
        End If



        MDIForm1.ToolStripProgressBar1.Value = 0
        If CDbl(UEnumbers) - 2 >= 0 Then
            For i = 0 To CDbl(UEnumbers) - 2
                'If Module1.GetSection(configfilename, i + 1) <> "dirs" Then
                If objRWINI.GetSection(configfilename, i + 1) <> "dirs" Then
                    'ListView1.Items.Add(Module1.GetSection(configfilename, i + 1))
                    ListView1.Items.Add(objRWINI.GetSection(configfilename, i + 1))
                    ListView1.Items.Item(i).SubItems.Add("")
                    ListView1.Items.Item(i).SubItems.Add("")
                    ListView1.Items.Item(i).SubItems.Add(Trim(objRWINI.ReadKeyVal(configfilename, objRWINI.GetSection(configfilename, i + 1), "ip")))
                    ListView1.Items.Item(i).SubItems(3).Tag = Trim(objRWINI.ReadKeyVal(configfilename, objRWINI.GetSection(configfilename, i + 1), "ip"))
                    ueliststate.Add(ListView1.Items.Item(i).SubItems(3).Tag, Now)
                    ListView1.Items.Item(i).SubItems.Add(Trim(objRWINI.ReadKeyVal(configfilename, objRWINI.GetSection(configfilename, i + 1), "cip")))
                    ListView1.Items.Item(i).SubItems.Add(Trim(objRWINI.ReadKeyVal(configfilename, objRWINI.GetSection(configfilename, i + 1), "throughput")))
                    ListView1.Items.Item(i).SubItems.Add(Trim(objRWINI.ReadKeyVal(configfilename, objRWINI.GetSection(configfilename, i + 1), "serverip")))
                    ListView1.Items.Item(i).SubItems(6).Tag = Trim(objRWINI.ReadKeyVal(configfilename, objRWINI.GetSection(configfilename, i + 1), "serverip"))

                    ListView1.Items.Item(i).SubItems.Add(Trim(objRWINI.ReadKeyVal(configfilename, objRWINI.GetSection(configfilename, i + 1), "com")))
                    ListView1.Items.Item(i).SubItems.Add(Trim(objRWINI.ReadKeyVal(configfilename, objRWINI.GetSection(configfilename, i + 1), "type")))
                    ListView1.Items.Item(i).SubItems(8).Tag = Trim(objRWINI.ReadKeyVal(configfilename, objRWINI.GetSection(configfilename, i + 1), "type"))

                    ListView1.Items.Item(i).SubItems.Add(Trim(objRWINI.ReadKeyVal(configfilename, objRWINI.GetSection(configfilename, i + 1), "action")))
                    ListView1.Items.Item(i).SubItems.Add(Trim(objRWINI.ReadKeyVal(configfilename, objRWINI.GetSection(configfilename, i + 1), "dinterval")))
                    ListView1.Items.Item(i).SubItems.Add(Trim(objRWINI.ReadKeyVal(configfilename, objRWINI.GetSection(configfilename, i + 1), "loopinterval")))
                    ListView1.Items.Item(i).SubItems.Add(Trim(objRWINI.ReadKeyVal(configfilename, objRWINI.GetSection(configfilename, i + 1), "traffic")))
                    ListView1.Items.Item(i).SubItems.Add(Trim(objRWINI.ReadKeyVal(configfilename, objRWINI.GetSection(configfilename, i + 1), "ftpsession")))
                    ListView1.Items.Item(i).SubItems.Add(Trim(objRWINI.ReadKeyVal(configfilename, objRWINI.GetSection(configfilename, i + 1), "logip")))
                    ListView1.Items.Item(i).SubItems.Add(Trim(objRWINI.ReadKeyVal(configfilename, objRWINI.GetSection(configfilename, i + 1), "serverip2")))
                    ListView1.Items.Item(i).SubItems.Add(Trim(objRWINI.ReadKeyVal(configfilename, objRWINI.GetSection(configfilename, i + 1), "serverip3")))
                    ListView1.Items.Item(i).SubItems.Add(Trim(objRWINI.ReadKeyVal(configfilename, objRWINI.GetSection(configfilename, i + 1), "serverip4")))
                    ListView1.Items.Item(i).SubItems.Add(Trim(objRWINI.ReadKeyVal(configfilename, objRWINI.GetSection(configfilename, i + 1), "udpul")))
                    ListView1.Items.Item(i).SubItems.Add(Trim(objRWINI.ReadKeyVal(configfilename, objRWINI.GetSection(configfilename, i + 1), "udpdl")))
                    ListView1.Items.Item(i).SubItems.Add(Trim(objRWINI.ReadKeyVal(configfilename, objRWINI.GetSection(configfilename, i + 1), "tnumber")))
                    'ListView1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfilename, Module1.GetSection(configfilename, i + 1), "ip")))
                    'ListView1.Items.Item(i).SubItems(3).Tag = Trim(Module1.ReadKeyVal(configfilename, Module1.GetSection(configfilename, i + 1), "ip"))
                    'ueliststate.Add(ListView1.Items.Item(i).SubItems(3).Tag, Now)
                    'ListView1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfilename, Module1.GetSection(configfilename, i + 1), "cip")))
                    'ListView1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfilename, Module1.GetSection(configfilename, i + 1), "throughput")))
                    'ListView1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfilename, Module1.GetSection(configfilename, i + 1), "serverip")))
                    'ListView1.Items.Item(i).SubItems(6).Tag = Trim(Module1.ReadKeyVal(configfilename, Module1.GetSection(configfilename, i + 1), "serverip"))

                    'ListView1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfilename, Module1.GetSection(configfilename, i + 1), "com")))
                    'ListView1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfilename, Module1.GetSection(configfilename, i + 1), "type")))
                    'ListView1.Items.Item(i).SubItems(8).Tag = Trim(Module1.ReadKeyVal(configfilename, Module1.GetSection(configfilename, i + 1), "type"))

                    'ListView1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfilename, Module1.GetSection(configfilename, i + 1), "action")))
                    'ListView1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfilename, Module1.GetSection(configfilename, i + 1), "dinterval")))
                    'ListView1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfilename, Module1.GetSection(configfilename, i + 1), "loopinterval")))
                    'ListView1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfilename, Module1.GetSection(configfilename, i + 1), "traffic")))
                    'ListView1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfilename, Module1.GetSection(configfilename, i + 1), "ftpsession")))
                    'ListView1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfilename, Module1.GetSection(configfilename, i + 1), "logip")))
                    'ListView1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfilename, Module1.GetSection(configfilename, i + 1), "serverip2")))
                    'ListView1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfilename, Module1.GetSection(configfilename, i + 1), "serverip3")))
                    'ListView1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfilename, Module1.GetSection(configfilename, i + 1), "serverip4")))
                    'ListView1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfilename, Module1.GetSection(configfilename, i + 1), "udpul")))
                    'ListView1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfilename, Module1.GetSection(configfilename, i + 1), "udpdl")))
                    'ListView1.Items.Item(i).SubItems.Add(Trim(Module1.ReadKeyVal(configfilename, Module1.GetSection(configfilename, i + 1), "tnumber")))
                    ' ListView1.Items.Item(i).Checked = True
                End If
                MDIForm1.ToolStripProgressBar1.Value = i
                System.Windows.Forms.Application.DoEvents()
            Next
            MDIForm1.ToolStripStatusLabel1.Text = ""

            If tcpstarted = False Then
                tcpserversession = New Thread(New ParameterizedThreadStart(AddressOf RunTCPserver))
                tcpserversession.Name = "TP Tcp server"
                tcpserversession.Start()
                tcpstarted = True
            End If

            initializegraphics()


            Timer1.Interval = 1000
        End If
        For i = 0 To ListView1.Items.Count - 1
            oneueconfig = ""
            For j = 3 To 19
                If j <> 5 Then
                    oneueconfig = oneueconfig + "," + ListView1.Items(i).SubItems(j).Text
                End If

            Next
            UEconfigurations = UEconfigurations + "|" + oneueconfig
        Next

    End Sub
    Private Sub Form4_VisibleChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.VisibleChanged
        If Visible = True And MDIForm1.configischanged = True Then
            If Not (monitorform.objTimer1 Is Nothing) Then monitorform.objTimer1.Change(-1, 10)
            initform(My.Application.Info.DirectoryPath + "\ueconfig.ini")
            MDIForm1.configischanged = False
            monitorform.monitorwindowshow(My.Application.Info.DirectoryPath + "\ueconfig.ini")
            If Not (monitorform.objTimer1 Is Nothing) Then monitorform.objTimer1.Change(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(monitorform.updateinterval))

        End If

    End Sub

    Private Sub listview1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub butaddscenario_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim s As String

        OpenFileDialog1.InitialDirectory = My.Application.Info.DirectoryPath
        If OpenFileDialog1.ShowDialog() <> Windows.Forms.DialogResult.Cancel Then
            For Each s In OpenFileDialog1.FileNames
                Dim newrow As DataRow = DataSet1.Tables("scenarios").NewRow()
                newrow.Item(0) = s
                newrow.Item(1) = 86400
                DataSet1.Tables("scenarios").Rows.Add(newrow)
            Next
        End If
    End Sub




    Private Sub DataGridView1_CellContentClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs)
        'Dim configfilename As String
        'If Not (monitorform.objTimer1 Is Nothing) Then monitorform.objTimer1.Change(-1, 10)
        'configfilename = DataGridView1.CurrentRow.Cells(0).Value.ToString
        'ListView1.Items.Clear()
        'initform(configfilename)
        'monitorform.monitorwindowshow(configfilename)
        'If Not (monitorform.objTimer1 Is Nothing) Then monitorform.objTimer1.Change(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(monitorform.updateinterval))
        'Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
    End Sub

    Private Sub butdeletscenario_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If DataGridView1.Rows.Count > 0 Then
            DataGridView1.Rows.Remove(DataGridView1.CurrentRow)
        End If



    End Sub

    Private Sub SplitContainer1_Panel1_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs)

    End Sub

    Public Sub Runscenario_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim configfilename As String
        Dim i As Integer
        If DataGridView1.Rows.Count > 0 Then
            ProgressBar1.Maximum = 0
            ProgressBar1.Value = 0
            For i = 0 To DataGridView1.Rows.Count - 1
                ProgressBar1.Maximum = ProgressBar1.Maximum + DataGridView1.Rows(i).Cells(1).Value * 1000
            Next

            scenariostep = 1
            Timer2.Interval = DataGridView1.Rows(0).Cells(1).Value * 1000
            configfilename = DataGridView1.Rows(0).Cells(0).Value.ToString
            initform(configfilename)
            Command4_Click(Nothing, Nothing)
            Timer3.Enabled = True
            Timer2.Enabled = True
            setallbuttonstate(0, False)
            Command7.Enabled = True
            setallcursor(0)
            Runscenario.Enabled = False
        End If

    End Sub

    Private Sub Timer2_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles Timer2.Tick
        Dim configfilename As String
        Timer2.Enabled = False
        scenariostep = scenariostep + 1
        ProgressBar1.Value = ProgressBar1.Value + 1000
        If scenariostep <= DataGridView1.Rows.Count Then

            Command7_Click(Nothing, Nothing)
            Timer2.Interval = DataGridView1.Rows(scenariostep - 1).Cells(1).Value * 1000
            configfilename = DataGridView1.Rows(scenariostep - 1).Cells(0).Value.ToString
            initform(configfilename)
            wait(3000)
            Command4_Click(Nothing, Nothing)
            Timer3.Enabled = True
            Timer2.Enabled = True
            setallbuttonstate(0, True)
            Command7.Enabled = True
            setallcursor(0)
        Else
            scenariostep = 0
            Runscenario.Enabled = True
            setallbuttonstate(1, False)
            setallcursor(1)
            Timer3.Enabled = True
        End If


    End Sub

    Private Sub Timer3_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer3.Tick
        If ProgressBar1.Value + 1000 > ProgressBar1.Maximum Then
            ProgressBar1.Value = ProgressBar1.Maximum
            Timer3.Enabled = False
        Else
            ProgressBar1.Value = ProgressBar1.Value + 1000
        End If

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        'Dim UElogdir As Object
        Dim configfilename As String

        Dim i As Object
        Dim uecipstr As String
        Dim totalcommandstrs, uestartlogs, ueidlist As New Collection
        Dim results As New Collection
        Dim totalcommandstr As String
        ' Dim uestartlog As String
        Dim logipstr As String
        Dim ueciplist As String
        Dim searchuecip As String
        Dim jj As Integer
        uecipstr = ""
        Timer2.Enabled = False
        Timer3.Enabled = False
        'Timer1.Enabled = False
        setallbuttonstate(0, False)
        setallcursor(0)
        logipstr = ""
        ueciplist = ""
        OpenFileDialog1.Filter = "ftp config file|*.bat|exe file|*.exe" '"config files|*.ini"
        OpenFileDialog1.DefaultExt = "bat" '"ini"
        If OpenFileDialog1.ShowDialog() <> Windows.Forms.DialogResult.Cancel Then
            configfilename = OpenFileDialog1.FileName
            configfilename = My.Computer.FileSystem.GetName(configfilename)
            IO.File.Copy(OpenFileDialog1.FileName, "d:\uelog\" + configfilename, True)
            'configfilename = My.Computer.FileSystem.GetName(configfilename)
            For i = 1 To ListView1.Items.Count
                uestartlogs.Clear()
                ueidlist.Clear()

                If ListView1.Items.Item(i - 1).Checked = True And ueciplist.IndexOf(ListView1.Items.Item(i - 1).SubItems(4).Text) < 0 Then '查找新的被checked uecip
                    searchuecip = ListView1.Items.Item(i - 1).SubItems(4).Text
                    ueciplist = ueciplist + "," + searchuecip
                    '---------------search 所有uecipstr的行数
                    jj = i
                    If jj <= ListView1.Items.Count Then
                        For jj = i - 1 To ListView1.Items.Count - 1
                            If ListView1.Items.Item(jj).Checked = True And ListView1.Items.Item(jj).SubItems(4).Text = searchuecip Then  '查找新的被checked uecip当前再查的ue
                                '---------------UEcip---------------------------------------
                                uecipstr = ListView1.Items.Item(jj).SubItems(4).Text


                                '--------------logip---------------------------------------
                                logipstr = ListView1.Items.Item(jj).SubItems(14).Text
                                ueidlist.Add(jj + 1)



                            End If


                        Next
                        '---------------total command-----------------------------
                        totalcommandstr = "xcopy.exe \\" + logipstr + "\uelog\" + configfilename + " d:\mueauto\  /y"
                        totalcommandstrs.Add(totalcommandstr)

                        If totalcommandstrs.Count > 0 Then

                            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "++++++++++++Copy config file to " + uecipstr + "+++++++++++++++" & vbCrLf
                            results = runUEwagentnewtcp(uecipstr, "lte", "asb#1234", totalcommandstrs)
                            totalcommandstrs.Clear()
                        End If
                    End If
                    '----------------search 所有uecipstr的行数



                End If




            Next

        End If

        '----------------------------------------------------------------------------




        OpenFileDialog1.Filter = "config files|*.ini"
        OpenFileDialog1.DefaultExt = "ini"
        Command7.Enabled = True
        setallbuttonstate(1, True)
        Runscenario.Enabled = True
        setallcursor(1)
        ' Timer2.Enabled = True
        ' Timer3.Enabled = True
        ' Timer1.Enabled = True
    End Sub

    Private Sub synctime_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles synctime.Click
        Dim tj As Integer
        Dim UElogdir As String
        Dim ueipstr As String

        Dim UEtypestr As String
        Dim logipstr As String

        Dim sshexename As String
        Dim runmode As String
        Dim i As Integer
        Dim ij As Integer
        '  Any string automatically begins a fully-functional 30-day trial.
        '----------------------------------------------------------------------------------------------------------

        '----------------------------------------------------------------------------------------------------------------------------------------------------------------------

        '  Display the remote shell's command output:
        Dim uecipstr, tpinterval As String
        Dim results As New Collection
        Dim ueidlist As New Collection
        Dim totalcommandstr As String
        Dim uestartlog As String
        Dim totalcommandstrs As New Collection
        Dim ueipstrs As New Collection
        Dim uestartlogs As New Collection
        Dim haverun As String
        Dim currenttime As Long
        MDIForm1.state = "syncing time"
        runmode = ""
        UEtypestr = ""
        Try
            Timer1.Enabled = False
            haverun = ""
            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "++++++++++++start run UEs++++++++++++++++" & vbCrLf
            If MDIForm1.AutobackupToolStripMenuItem.Checked = True Then
                Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "backup logs" & vbCrLf
                Command10_Click(Nothing, Nothing)
            End If
            setallbuttonstate(0, False)
            setallcursor(0)
            For ij = 0 To ListView1.Items.Count - 1
                uestartlogs.Clear()
                ueidlist.Clear()
                totalcommandstrs.Clear()
                If ListView1.Items.Item(ij).Checked = True And Not (InStr(haverun, ListView1.Items.Item(ij).SubItems(4).Text) > 0) Then
                    uecipstr = ListView1.Items.Item(ij).SubItems(4).Text
                    If ListView1.Items.Item(ij).SubItems.Count > 1 Then
                        ListView1.Items.Item(ij).SubItems(1).Text = ""
                    Else
                        ' listview1.Items.Item(ij).SubItems.Insert(1, New System.Windows.Forms.ListViewItem.ListViewSubItem(Nothing, ""))
                    End If
                    For i = 0 To ListView1.Items.Count - 1
                        If (ListView1.Items.Item(i).SubItems(3).Text = uecipstr) And Not (InStr(haverun, ListView1.Items.Item(ij).SubItems(3).Text) > 0) Then
                            '=================================================================================================
                            '-------------exename----------------------------
                            'sshexename = "c:\python27\python.exe d:\Mueauto\uecontrol.py"
                            sshexename = "d:\mueauto\MUEclient.exe"
                            '---------------UEcip---------------------------------------
                            uecipstr = ListView1.Items.Item(i).SubItems(4).Text

                            '--------------logip---------------------------------------
                            logipstr = " -w " & ListView1.Items.Item(i).SubItems(14).Text

                            haverun = haverun & "," & uecipstr '处理过的板卡的UEcip做记录

                            'uecipstrs.Add(uecipstr)
                            ueipstr = ListView1.Items.Item(i).SubItems(3).Tag
                            ueipstrs.Add(ueipstr)
                            UElogdir = Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\ueconfig.ini", "dirs", "UElog")
                            uestartlog = UElogdir + "\" + ueipstr + ".log"
                            uestartlogs.Add(uestartlog)
                            ueidlist.Add(i + 1)
                            '---------------TP report interval-----------------------
                            tpinterval = MDIForm1.TPinterval.Text
                            '---------------time sync --------------------
                            currenttime = DateDiff("s", "2000-01-01 00:00:00", Now)
                            '---------------total command-----------------------------
                            totalcommandstr = sshexename + " -S " + currenttime.ToString + logipstr + " -g " + uecipstr

                            totalcommandstrs.Add(totalcommandstr)

                            '========================================================================================
                        End If
                    Next

                    Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "Now start sync time on PC:" & ListView1.Items.Item(ij).SubItems(3).Text & vbCrLf
                    results = runUEwagentnewtcp(uecipstr, "lte", "asb#1234", totalcommandstrs)


                    For tj = 1 To uestartlogs.Count()
                        If ListView1.Items.Item(ueidlist.Item(tj) - 1).SubItems.Count > 1 And uestartlogs.Count = results.Count Then
                            ListView1.Items.Item(ueidlist.Item(tj) - 1).SubItems(1).Text = results.Item(tj)
                        Else
                            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "ue num <> result num" & vbCrLf
                        End If

                    Next
                End If
            Next
            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "++++++++++++end run UEs++++++++++++++++" & vbCrLf
        Catch e3 As Exception
            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & e3.Message & vbCrLf
            Text1.Text = Text1.Text & "error at run UE part"
        End Try
        setallbuttonstate(1, False)
        setallcursor(1)
        Timer1.Enabled = True
        MDIForm1.state = "idle"
    End Sub


    Public Function sendtcpcommand(ByVal command As String, ByVal ip As String) As String
        Try
            'Dim port As Int32 = 33891
            Dim client As New System.Net.Sockets.TcpClient(ip, 33891)
            ' Translate the passed message into ASCII and store it as a Byte array.
            Dim data As [Byte]() = System.Text.Encoding.ASCII.GetBytes(command)

            ' Get a client stream for reading and writing.
            '  Stream stream = client.GetStream();
            Dim stream As System.Net.Sockets.NetworkStream = client.GetStream()
            client.ReceiveTimeout = 1000
            If client.Connected = False Then
                Return "Connection fail"
            End If

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
        Catch e As ArgumentNullException
            Return ("Connection fail")
        Catch e As System.Net.Sockets.SocketException
            Return ("Connection fail")
        End Try
    End Function

    Private Function runUEwagentnewtcp(ByRef UEcip As String, ByRef logname As String, ByRef pass As String, ByRef command_Renamed As Collection) As Collection
        Dim i As Integer
        Dim h As Integer
        Dim result As String
        Dim results As New Collection

        Dim receivedata As Object
        receivedata = Nothing
        Try
            If MDIForm1.singleUEselect = False Then
                receivedata = sendtcpcommand("run=" & "d:\mueauto\killall.bat", UEcip)
                ' 'tcpclient.SendData("run=" & "d:\mueauto\killall.bat")
                If receivedata = "Connection fail" Then
                    Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & receivedata & vbCrLf
                    For h = 1 To command_Renamed.Count()
                        results.Add(receivedata)
                    Next

                    Return results
                End If
                wait(500)
            End If
            receivedata = sendtcpcommand("run=" & "d:\mueauto\killmobilepartner.bat", UEcip)
            If receivedata = "Connection fail" Then
                Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & receivedata & vbCrLf
                For h = 1 To command_Renamed.Count()
                    results.Add(receivedata)
                Next

                Return results
                wait(500)
            End If

            For i = 1 To command_Renamed.Count()
                If command_Renamed.Item(i).ToString.IndexOf(" -c") >= 0 Then
                    Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "shutdown UE:" & Mid(command_Renamed.Item(i).ToString, command_Renamed.Item(i).ToString.IndexOf("-g ") + 3, command_Renamed.Item(i).ToString.IndexOf("-n ") - command_Renamed.Item(i).ToString.IndexOf("-g ") - 3) & vbCrLf
                Else
                    If command_Renamed.Item(i).ToString.IndexOf("copy") >= 0 Then

                    Else
                        If command_Renamed.Item(i).ToString.IndexOf("-n ") >= 0 And command_Renamed.Item(i).ToString.IndexOf(" -e") < 0 Then
                            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "run UE:" & Mid(command_Renamed.Item(i).ToString, command_Renamed.Item(i).ToString.IndexOf("-g ") + 3, command_Renamed.Item(i).ToString.IndexOf("-n ") - command_Renamed.Item(i).ToString.IndexOf("-g ") - 3) & vbCrLf
                        Else
                            'Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "run UE:" & Mid(command_Renamed.Item(i).ToString, command_Renamed.Item(i).ToString.IndexOf("-g ") + 3, command_Renamed.Item(i).ToString.Length - command_Renamed.Item(i).ToString.IndexOf("-g ") - 3) & vbCrLf
                        End If

                    End If
                    If command_Renamed.Item(i).ToString.IndexOf(" -e") >= 0 Then
                        Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "reseting UE:" & Mid(command_Renamed.Item(i).ToString, command_Renamed.Item(i).ToString.IndexOf("-g ") + 3, command_Renamed.Item(i).ToString.IndexOf("-n ") - command_Renamed.Item(i).ToString.IndexOf("-g ") - 3) & vbCrLf

                    End If

                End If
                ' 'tcpclient.SendData("run=" & command_Renamed.Item(i))
                receivedata = sendtcpcommand("run=" & command_Renamed.Item(i), UEcip)
                wait(500)
                ''tcpclient.GetData(receivedata, vbString, 1500)
                Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & receivedata & vbCrLf
                result = "start run"
                'If Trim(receivedata).IndexOf(">") >= 0 Then

                If command_Renamed.Item(i).ToString.IndexOf(" -c") >= 0 Then
                    result = "shutdown"
                Else
                    If command_Renamed.Item(i).ToString.IndexOf(" -e") >= 0 Then result = "resetting" Else result = "Running"
                End If
                Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & result & " command success" & vbCrLf
                'End If
                results.Add(result)
                receivedata = sendtcpcommand("run=" + "d:\mueauto\logcommand.bat " + command_Renamed.Item(i), UEcip)
                ''tcpclient.SendData("run=" + "d:\mueauto\logcommand.bat " + command_Renamed.Item(i))
                wait(500)
                ''tcpclient.GetData(receivedata, vbString, 1500)
                Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & receivedata & vbCrLf
            Next
            'tcpclient.Close()
            runUEwagentnewtcp = results
        Catch ex As Exception
            'tcpclient.Close()
            results.Add(ex.ToString)
            Return results
        End Try

    End Function
    Private Function newstopUEwagentnewtcp(ByRef UEcip As String, ByRef logname As String, ByRef pass As String, ByRef command_Renamed As Collection) As Collection
        Dim i As Integer
        Dim h As Integer
        Dim result, command, comport As String
        Dim results As New Collection

        Dim receivedata As Object
        receivedata = Nothing
        Try

            For i = 1 To command_Renamed.Count()
                If command_Renamed.Item(i).ToString.IndexOf("-e ") Or command_Renamed.Item(i).ToString.IndexOf("-c ") >= 0 Then
                    Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "stop UE:" & Mid(command_Renamed.Item(i).ToString, command_Renamed.Item(i).ToString.IndexOf("-g ") + 3, command_Renamed.Item(i).ToString.IndexOf("-n ") - command_Renamed.Item(i).ToString.IndexOf("-g ") - 3) & vbCrLf
                Else
                    'Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "run UE:" & Mid(command_Renamed.Item(i).ToString, command_Renamed.Item(i).ToString.IndexOf("-g ") + 3, command_Renamed.Item(i).ToString.Length - command_Renamed.Item(i).ToString.IndexOf("-g ") - 3) & vbCrLf
                End If
                command = command_Renamed.Item(i)
                comport = Trim(Mid(command, command.IndexOf("-p") + 3, 8))
                If comport.IndexOf(" ") > 0 Then
                    comport = Trim(Mid(comport, 1, comport.IndexOf(" ")))
                End If
                command = "Closewindow=MUEclientform:" + comport + "."
                receivedata = sendtcpcommand(command, UEcip)
                If receivedata = "Connection fail" Then
                    Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & receivedata & vbCrLf
                    For h = 1 To command_Renamed.Count()
                        results.Add(receivedata)
                    Next

                    Return results
                End If
                Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & receivedata & vbCrLf
                result = receivedata
                command = "Closewindow=Volte sim:" + comport + "."
                receivedata = sendtcpcommand(command, UEcip)
                If receivedata = "Connection fail" Then
                    Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & receivedata & vbCrLf
                    For h = 1 To command_Renamed.Count()
                        results.Add(receivedata)
                    Next

                    Return results
                End If
                Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & receivedata & vbCrLf
                result = receivedata
                command = "Closewindow=Http " + comport + "."
                receivedata = sendtcpcommand(command, UEcip)
                If receivedata = "Connection fail" Then
                    Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & receivedata & vbCrLf
                    For h = 1 To command_Renamed.Count()
                        results.Add(receivedata)
                    Next

                    Return results
                End If
                Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & receivedata & vbCrLf
                result = receivedata
                command = "Closewindow=Http download " + comport + "."
                receivedata = sendtcpcommand(command, UEcip)
                If receivedata = "Connection fail" Then
                    Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & receivedata & vbCrLf
                    For h = 1 To command_Renamed.Count()
                        results.Add(receivedata)
                    Next

                    Return results
                End If
                ' wait(1000)
                Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & receivedata & vbCrLf
                result = receivedata

                command = "Closewindow=FTP:" + comport + "."
                receivedata = sendtcpcommand(command, UEcip)
                If receivedata = "Connection fail" Then
                    Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & receivedata & vbCrLf
                    For h = 1 To command_Renamed.Count()
                        results.Add(receivedata)
                    Next

                    Return results
                End If
                ' wait(1000)
                Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & receivedata & vbCrLf
                result = receivedata

                command = "Closewindow=Ping:" + comport + "."
                receivedata = sendtcpcommand(command, UEcip)
                If receivedata = "Connection fail" Then
                    Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & receivedata & vbCrLf
                    For h = 1 To command_Renamed.Count()
                        results.Add(receivedata)
                    Next

                    Return results
                End If
                ' wait(1000)
                Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & receivedata & vbCrLf
                result = receivedata


            Next

            newstopUEwagentnewtcp = results
        Catch ex As Exception
            'tcpclient.Close()
            results.Add(ex.ToString)
            Return results
        End Try

    End Function

    Public Function runftpwagentnewtcp(ByRef UEcip As String, ByRef logname As String, ByRef pass As String, ByRef command_Renamed As String) As String
        Dim receivedata As Object
        receivedata = Nothing
        Try
            receivedata = sendtcpcommand("run=" & command_Renamed, UEcip)
            ' 'tcpclient.SendData("run=" & command_Renamed)
            wait(2000)
            ''tcpclient.GetData(receivedata, vbString, 1500)
            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & receivedata & vbCrLf
            If receivedata <> ">" Then
                runftpwagentnewtcp = receivedata
            Else
                runftpwagentnewtcp = "ok"
            End If


            'tcpclient.Close()

        Catch ex As Exception
            'tcpclient.Close()
            Return ex.ToString
        End Try


    End Function
    Private Function stoprunUEwagentnewtcp(ByRef UEcip As String) As String
        Dim receivedata As String
        receivedata = ""
        Try
            receivedata = sendtcpcommand("run=" & "d:\mueauto\killall.bat", UEcip)
            'tcpclient.SendData("run=" & "d:\mueauto\killall.bat") 'command
            wait(2000)
            'tcpclient.GetData(receivedata, vbString, 1500)
            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & receivedata & vbCrLf
            If receivedata.IndexOf(">") >= 0 Then
                stoprunUEwagentnewtcp = receivedata + "send command OK"
            Else
                stoprunUEwagentnewtcp = "taskkill fail"
            End If


            'tcpclient.Close()
        Catch ex As Exception
            'tcpclient.Close()
            Return ex.ToString
        End Try



    End Function

    Public Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        'Dim UElogdir As Object
        Dim ueipstr As Object
        Dim dropintervalstr As Object
        Dim lintervalstr As Object
        Dim comportstr As Object
        Dim serveripstr As Object
        Dim UEtypestr As Object
        Dim sshexename As Object
        Dim runmode As Object
        Dim i As Object
        Dim uecipstr As String
        Dim result As String = ""
        Dim totalcommandstrs, uestartlogs, ueidlist As New Collection
        Dim results As New Collection
        Dim totalcommandstr As String
        ' Dim uestartlog As String
        Dim logipstr As String
        Dim ftpsession As String
        Dim traffictype As String
        Dim ueciplist As String
        Dim searchuecip As String
        Dim jj As Integer
        uecipstr = ""
        MDIForm1.state = "reseting UEs"
        'Timer2.Enabled = False
        Timer3.Enabled = False
        Timer1.Enabled = False
        setallbuttonstate(0, False)
        setallcursor(0)
        Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "++++++++++++stop run UEs++++++++++++++++" & vbCrLf
        ueciplist = ""
        For i = 1 To ListView1.Items.Count
            uestartlogs.Clear()
            ueidlist.Clear()
            ' If listview1.Items.Item(i - 1).SubItems.Count > 1 Then '把ue状态设为空
            'listview1.Items.Item(i - 1).SubItems(1).Text = ""
            'Else
            'End If
            If ListView1.Items.Item(i - 1).Checked = True And ueciplist.IndexOf(ListView1.Items.Item(i - 1).SubItems(4).Text) < 0 Then '查找新的被checked uecip
                searchuecip = ListView1.Items.Item(i - 1).SubItems(4).Text
                ueciplist = ueciplist + "," + searchuecip
                '---------------search 所有uecipstr的行数
                jj = i
                If jj <= ListView1.Items.Count Then
                    For jj = i - 1 To ListView1.Items.Count - 1
                        If ListView1.Items.Item(jj).Checked = True And ListView1.Items.Item(jj).SubItems(4).Text = searchuecip Then  '查找新的被checked uecip当前再查的ue
                            '--------------action-------------------------------

                            runmode = " -e" 'shutdown

                            '-------------exename----------------------------
                            sshexename = "d:\mueauto\MUEclient.exe"
                            '-------------uetpype----------------------------
                            UEtypestr = "HE5776"
                            Select Case ListView1.Items.Item(jj).SubItems(8).Tag
                                Case Is = "Qualcomm8996", "Qualcomm8998"
                                    UEtypestr = " -t Qualcomm9600"
                                Case Is = "SMG9350"
                                    UEtypestr = " -t SMG9350"
                                Case Is = "E5776", "E5375"
                                    UEtypestr = " -t E5776" 'Huawei E5776
                                Case Is = "hisi"
                                    UEtypestr = " -t H" 'Hisi
                                Case Is = "Qualcomm9600", "SIM7000", "SIM7200", "MC7455", "BG96", "EM7565"
                                    UEtypestr = " -t Qualcomm9600" 'Qualcomm9600
                                Case Is = "YY9027"
                                    UEtypestr = " -t YY9027" 'Qualcomm9028
                                Case Is = "BandluxeC508"
                                    UEtypestr = " -t BandluxeC508"
                                Case Is = "ALT-C186"
                                    UEtypestr = " -t ALT-C186"
                                Case Is = "Dialcommon"
                                    UEtypestr = " -t Dialcommon"
                                Case Else
                            End Select
                            Try
                                If Int(ListView1.Items.Item(jj).SubItems(13).Text) < 20 Then
                                    ftpsession = Trim(ListView1.Items.Item(jj).SubItems(13).Text)
                                Else
                                    ftpsession = "2"
                                End If
                            Catch
                                ftpsession = "2"
                            End Try
                            traffictype = ListView1.Items.Item(jj).SubItems(12).Text
                            '-------------serverip----------------------------
                            serveripstr = " -s " & ListView1.Items.Item(jj).SubItems(6).Tag
                            '---------------comport-----------------------------
                            comportstr = " -p " & ListView1.Items.Item(jj).SubItems(7).Text
                            '-------------Linterval----------------------------
                            lintervalstr = " -i " & ListView1.Items.Item(jj).SubItems(11).Text
                            '---------------dropinterval-----------------------------
                            dropintervalstr = " -d " & ListView1.Items.Item(jj).SubItems(10).Text

                            '---------------UEcip---------------------------------------
                            uecipstr = ListView1.Items.Item(jj).SubItems(4).Text

                            '--------------logip---------------------------------------
                            logipstr = " -w " & ListView1.Items.Item(jj).SubItems(14).Text
                            '---------------UEip---------------------------------------

                            ueipstr = ListView1.Items.Item(jj).SubItems(3).Tag
                            '---------------------------------------------------------

                            ueidlist.Add(jj + 1)
                            '---------------total command-----------------------------
                            totalcommandstr = sshexename + runmode + UEtypestr + serveripstr + comportstr + lintervalstr + dropintervalstr + logipstr + " -g " + ueipstr + " -n " + ftpsession + " -T " + traffictype

                            totalcommandstrs.Add(totalcommandstr)

                            ' If listview1.Items.Item(jj).SubItems.Count > 1 Then
                            'listview1.Items.Item(jj).SubItems(1).Text = "Stoped"
                            'Else
                            'End If

                        End If


                    Next
                    If totalcommandstrs.Count > 0 Then
                        '  UElogdir = Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\ueconfig.ini", "dirs", "UElog")
                        ' uestartlog = UElogdir + "\" + ueipstr + ".log"
                        ' Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "reset UE on control board:" & uecipstr & vbCrLf
                        'result = stoprunUEwagentnewtcp(uecipstr)
                        If MDIForm1.singleUEselect = False Then
                            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "Stop running UE on control board:" & uecipstr & vbCrLf
                            result = stoprunUEwagentnewtcp(uecipstr)
                            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & result & vbCrLf

                        Else
                            Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "Stop running UE on control board:" & uecipstr & vbCrLf
                            newstopUEwagentnewtcp(uecipstr, "lte", "asb#1234", totalcommandstrs)
                            Application.DoEvents()
                        End If

                        Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & result & vbCrLf
                        Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "reset UE on control board:" & uecipstr & vbCrLf

                        results = runUEwagentnewtcp(uecipstr, "lte", "asb#1234", totalcommandstrs)


                        For tj = 1 To ueidlist.Count()
                            ' Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " &  results.Item(tj) & vbCrLf
                            If ListView1.Items.Item(ueidlist.Item(tj) - 1).SubItems.Count > 1 And ueidlist.Count = results.Count Then
                                ListView1.Items.Item(ueidlist.Item(tj) - 1).SubItems(1).Text = results.Item(tj)
                            Else
                                Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " ueid num <> results num" & vbCrLf

                                'listview1.Items.Item(ueidlist.Item(tj) - 1).SubItems.Insert(1, New System.Windows.Forms.ListViewItem.ListViewSubItem(Nothing, results.Item(tj)))
                            End If

                        Next
                        totalcommandstrs.Clear()
                    End If
                End If
                '----------------search 所有uecipstr的行数



            End If




        Next



        '----------------------------------------------------------------------------




        Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "++++++++++++end stop run UEs++++++++++++++++" & vbCrLf
        Command7.Enabled = True
        setallbuttonstate(1, True)
        Runscenario.Enabled = True
        setallcursor(1)
        ' Timer2.Enabled = True
        ' Timer3.Enabled = True
        Timer1.Enabled = True
        MDIForm1.state = "idle"
    End Sub


    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim i As Object
        Dim uecipstr As String
        Dim result As String
        Dim ueciplist As String
        ueciplist = ""
        Text1.Text = ""
        MDIForm1.state = "restarting borads"
        For i = 1 To ListView1.Items.Count
            If ListView1.Items.Item(i - 1).SubItems.Count > 1 Then
                ListView1.Items.Item(i - 1).SubItems(1).Text = ""
            Else
                ListView1.Items.Item(i - 1).SubItems.Insert(1, New System.Windows.Forms.ListViewItem.ListViewSubItem(Nothing, ""))
            End If
            If ListView1.Items.Item(i - 1).Checked = True Then
                '---------------UEcip---------------------------------------
                uecipstr = ListView1.Items.Item(i - 1).SubItems(4).Text
                result = ""
                If InStr(ueciplist, uecipstr) < 1 Then
                    ueciplist = ueciplist + ";" + uecipstr
                    ListView1.Items.Item(i - 1).SubItems(1).Text = "restart board"
                    Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & "restart board:" & ListView1.Items.Item(i - 1).SubItems(4).Text & vbCrLf
                    result = runftpwagentnewtcp(uecipstr, "", "", "shutdown -r -f -t 10")

                    Text1.Text = Text1.Text & Now.ToString("HH:mm:ss") & " " & result & vbCrLf
                    ListView1.Items.Item(i - 1).SubItems(1).Text = result
                End If

            End If
        Next
        MDIForm1.state = "idle"
    End Sub

    Sub initializegraphics()
        Dim rows As Integer = ListView1.Items.Count
        Dim graphic As NationalInstruments.UI.WindowsForms.ScatterGraph
        Dim plot1, plot2 As NationalInstruments.UI.ScatterPlot
        Dim i As Integer
        Dim xaxis1 As NationalInstruments.UI.XAxis
        Dim yaxis1, yaxis2 As NationalInstruments.UI.YAxis
        Dim cursor1, cursor2 As NationalInstruments.UI.XYCursor
        For i = 1 To rows
            graphic = New NationalInstruments.UI.WindowsForms.ScatterGraph
            graphic.BackColor = Color.Black
            graphic.CaptionVisible = False
            graphic.Border = Border.SunkenLite
            graphic.PlotAreaBorder = Border.None
            graphic.PlotAreaColor = Color.Black
            cursor1 = New NationalInstruments.UI.XYCursor
            cursor2 = New NationalInstruments.UI.XYCursor
            cursor1.LabelAlignment = NationalInstruments.UI.PointAlignment.Auto
            cursor2.LabelAlignment = NationalInstruments.UI.PointAlignment.Auto
            cursor1.LabelDisplay = XYCursorLabelDisplay.ShowY
            cursor2.LabelDisplay = XYCursorLabelDisplay.ShowY
            cursor1.LabelYFormat = New NationalInstruments.UI.FormatString(NationalInstruments.UI.FormatStringMode.Engineering, "S")
            cursor2.LabelYFormat = New NationalInstruments.UI.FormatString(NationalInstruments.UI.FormatStringMode.Engineering, "S")
            cursor1.LabelForeColor = Color.Pink
            cursor2.LabelForeColor = Color.Lime
            cursor1.LineStyle = LineStyle.None
            cursor2.LineStyle = LineStyle.None
            cursor1.PointSize = New Point(0, 0)
            cursor2.PointSize = New Point(0, 0)
            cursor1.LabelVisible = True
            cursor2.LabelVisible = True
            graphic.Cursors.Add(cursor1)
            graphic.Cursors.Add(cursor2)
            xaxis1 = New NationalInstruments.UI.XAxis
            xaxis1.Visible = False
            xaxis1.Inverted = True
            xaxis1.Mode = AxisMode.StripChart
            xaxis1.Range = New Range(0, 60)
            yaxis1 = New NationalInstruments.UI.YAxis
            yaxis2 = New NationalInstruments.UI.YAxis
            yaxis1.Visible = False
            yaxis2.Visible = False
            yaxis1.Position = YAxisPosition.Left
            yaxis2.Position = YAxisPosition.Right
            graphic.XAxes.Add(xaxis1)
            graphic.YAxes.Add(yaxis1)
            graphic.YAxes.Add(yaxis2)

            plot1 = New NationalInstruments.UI.ScatterPlot
            plot1.LineStyle = LineStyle.None
            plot1.XAxis = graphic.XAxes(0)
            plot1.YAxis = yaxis1
            plot1.HistoryCapacity = 60
            plot1.FillBase = XYPlotFillBase.YValue
            plot1.FillMode = PlotFillMode.Fill
            plot1.FillToBaseColor = Color.Red
            plot1.FillToBaseStyle = NationalInstruments.UI.FillStyle.Solid
            plot1.SmoothUpdates = True

            cursor1.Plot = plot1
            For h = 1 To 61
                plot1.PlotXYAppend(h, 0)
            Next
            graphic.Plots.Add(plot1)


            plot2 = New NationalInstruments.UI.ScatterPlot
            plot2.LineStyle = LineStyle.None
            plot2.XAxis = graphic.XAxes(0)
            plot2.YAxis = yaxis2
            plot2.HistoryCapacity = 60
            plot2.FillBase = XYPlotFillBase.YValue
            plot2.FillMode = PlotFillMode.Lines
            plot2.LineToBaseColor = Color.Lime
            plot2.LineToBaseStyle = LineStyle.Solid
            plot2.LineToBaseWidth = 3
            plot2.SmoothUpdates = True
            plot2.FillToBaseStyle = NationalInstruments.UI.FillStyle.None
            For h = 1 To 61
                plot2.PlotXYAppend(h, 0)
            Next
            graphic.Plots.Add(plot2)
            cursor2.Plot = plot2
            graphic.Cursors(0).XPosition = 60
            graphic.Cursors(1).XPosition = 60



            ListView1.AddEmbeddedControl(graphic, 5, i - 1)

        Next


    End Sub

    Private Sub Timer1_Tick1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Try
            'Dim rows As Integer = ListView1.Items.Count
            'Dim graphic As NationalInstruments.UI.WindowsForms.ScatterGraph
            'Dim r As System.Random
            'r = New Random(System.DateTime.Now.Millisecond)
            'Dim cnt1 As Integer = r.Next(100)
            'Dim cnt2 = r.Next(100)
            Dim msg As Object
            Dim i As Integer


            i = 1

            'graphic = ListView1.GetEmbeddedControl(3, i - 1)
            ' refreshgrah(graphic, cnt1, cnt2)

            While TPmsgpool.Count <> 0
                If Not (TPmsgpool.Item(0) Is Nothing) Then
                    msg = Split(TPmsgpool.Item(0), "|")
                    Dim uename As String = msg(0)
                    Dim type As String = msg(1)

                    Dim TP1 As String = msg(2)
                    Dim TP2 As String = msg(3)
                    If type = "s" Then
                        updateinfo(uename, TP1, TP2)

                        TPmsgpool.RemoveAt(0)
                    End If
                    If type = "i" Then
                        TPmsgpool.RemoveAt(0)
                    End If
                Else
                    TPmsgpool.RemoveAt(0)
                End If

            End While

            '**************stub
            'OPmsgpool.Add(" | ")
            '*******************

            While OPmsgpool.Count <> 0
                If Not (OPmsgpool.Item(0) Is Nothing) Then
                    msg = Split(OPmsgpool.Item(0), "|")
                    Dim uename As String = msg(0)
                    Dim message As String = msg(1)
                    '*******stub
                    'message = "$QCSQ :-67,,0,0"
                    ''message = "+CGREG: 2,1,FFFE,10400"
                    ' message = "+CGREG: 2,1,""FFFE"",""1B18821"",7"
                    'message = "+CGREG:5,""0001"",""00010700"""

                    ''message = "^HCSQ:""""LTE"""",55,45,151,26"
                    'uename = "1.1.1.1"
                    ' message = "+CMGSI: Main_Info,4,1,40,38950" + vbLf + "+CMGSI: RX_Power,0x00000003,RX_Chain0,1,-872,-30,-1097,0,RX_Chain1,1,-830,-36,-1065,0" + vbCrLf + "+CMGSI: TX_Power,0,-32768,0" + vbCrLf + "+CMGSI: Phy_Cellid,1,480" + vbCrLf + "+CMGSI: Log_Sinr10xdb,1,155" + vbCrLf + "OK" + vbCrLf + "AT$ QCSQ" + vbCrLf + "$QCSQ :-74,,0,0," + vbCrLf + "OK" + vbCrLf + "                    '****************stub"
                    ' message = "at!lteinfo" + vbCrLf + " !LTEINFO:" + vbCrLf + "Serving:   EARFCN MCC MNC   TAC      CID Bd D U SNR PCI  RSRQ   RSRP   RSSI RXLV" + vbCrLf + "             2120 460  02     1 00000A03  4 3 3  10  31 -11.7 -110.7  -81.2 --" + vbCrLf + "IntraFreq:                                          PCI  RSRQ   RSRP   RSSI RXLV" + vbCrLf + "                                                    389  -8.4  -99.4  -71.1  24"

                    updateOPinfo(uename, message)
                    OPmsgpool.RemoveAt(0)
                Else
                    OPmsgpool.RemoveAt(0)
                End If
            End While

        Catch ex As Exception
            Dim a = 1

        End Try

        'checkoutofcontrol()

        updatecellinfobar()


    End Sub
    Sub checkoutofcontrol()
        Dim graphic As NationalInstruments.UI.WindowsForms.ScatterGraph
        For Each ue As String In ueliststate.Keys
            If DateDiff(DateInterval.Minute, ueliststate(ue), Now) > 3 Then
                For Each item As ListViewItem In ListView1.Items
                    If item.SubItems(3).Tag = ue Then
                        If item.SubItems(1).Text <> "Out of control" Then
                            item.SubItems(1).Text = "Out of control"
                            'clear TP
                            For h = 1 To 61
                                graphic = ListView1.GetEmbeddedControl(5, item.Index)
                                refreshgrah(graphic, 0, 0)
                            Next
                        End If
                    End If


                Next
            End If
        Next



    End Sub
    Function getrealuetype(ByVal message As String) As String
        If message.IndexOf("MC7455") >= 0 Then Return "MC7455"
        If message.IndexOf("EM7565") >= 0 Then Return "EM7565"
        If message.IndexOf("BG96") >= 0 Then Return "BG96"
        If message.IndexOf("SIM7200") >= 0 Then Return "SIM7200"
        If message.IndexOf("4093") >= 0 Then Return "Roma 9x25"
        If message.IndexOf("4115") >= 0 Then Return "Qualcomm 8998"
        If message.IndexOf("4112") >= 0 Then Return "Qualcomm 8996"

    End Function

    Sub updateuelist()
        Dim isidle As Boolean = False
        Dim totalips As String = ""
        Dim ueip As String = ""
        Dim ips As String = "idlelist"
        For i = 0 To ListView1.Items.Count - 1
            If ListView1.Items(i).SubItems(9).Text = "attach-idle" And ListView1.Items(i).SubItems(0).Tag <> "" Then
                totalips = ListView1.Items(i).SubItems(6).Text
                If totalips.IndexOf("UE") >= 0 Then
                    ueip = Replace(Replace(Trim(Split(totalips, "UE:")(1)), vbCrLf, ""), vbCr, "")
                    ips = ips + "," + ueip
                End If
            End If

        Next
        idleueliststring = ips
        paginguelistchanged = True
    End Sub

    Sub updateOPinfo(ByVal uename As String, ByVal message As String)
        'message = "CellD:01801RSRPD:-114.8 SINR:8"
        'uename = "1.1.201.1"
        Dim realuetyp As String = ""
        Dim row As Integer = 0
        Dim match As System.Text.RegularExpressions.Match = System.Text.RegularExpressions.Regex.Match(message, "\d{15}")
        realuetyp = getrealuetype(message)
        For i = 0 To ListView1.Items.Count - 1
            Try
                If ListView1.Items(i).SubItems(3).Tag = uename Then
                    ueliststate(uename) = Now
                    If realuetyp <> "" Then

                        ListView1.Items(i).SubItems(8).Text = realuetyp

                    End If
                    If message.IndexOf("AT+CFUN=0") >= 0 Or message.IndexOf("AT+CGATT=0") >= 0 Then
                        ListView1.Items(i).SubItems(2).Text = ""
                        ListView1.Items(i).SubItems(2).Tag = ""
                        ListView1.Items(i).Tag = ""
                        ListView1.Items(i).SubItems(6).Text = ListView1.Items(i).SubItems(6).Tag
                    End If
                    If message.IndexOf("port opened") >= 0 Then

                        ListView1.Items(i).SubItems(0).Tag = "1"


                    End If

                    If message.IndexOf("port fail") >= 0 Then

                        ListView1.Items(i).SubItems(0).Tag = ""
                        ListView1.Items(i).SubItems(6).Text = ListView1.Items(i).SubItems(6).Tag
                        updateuelist()
                    End If

                    If message.IndexOf("RSRPD") < 0 And message.IndexOf("RSSID") < 0 And message.IndexOf("CellD") < 0 And message.IndexOf("UED") < 0 And message.IndexOf("QCSQ") < 0 And message.IndexOf("CGREG") < 0 And message.IndexOf("CREG") < 0 And message.IndexOf("HCSQ") < 0 And message.IndexOf("+CMGSI") < 0 And message.IndexOf("^") < 0 And message.IndexOf("!LTEINFO:") < 0 And message.IndexOf("AT!lteinfo") < 0 And message.IndexOf("+CMTI:") < 0 Then
                        message = message.Replace("ERROR", "")
                        If Trim(message) <> "" And Trim((message.Replace(vbCrLf, "")).Replace(vbCr, "")) <> "" Then
                            ListView1.Items(i).SubItems(1).Text = message
                        End If



                    End If

                    If match.Success Then
                        If match.Groups(0).Value.IndexOf("460") = 0 Then
                            ListView1.Items(i).SubItems(3).Text = ListView1.Items(i).SubItems(3).Tag + vbCr + match.Groups(0).Value

                            ListView1.Items(i).SubItems(0).Tag = "1"
                        End If
                    End If

                    If message.IndexOf("UED") >= 0 Then

                        ListView1.Items(i).SubItems(6).Text = ListView1.Items(i).SubItems(6).Tag + vbCr + Replace(message, "UED", "UE")
                        updateuelist()

                    End If
                    Dim tempcell, temprsrp As String
                    If message.IndexOf("CellD") >= 0 And message.IndexOf("RSRPD") < 0 Then
                        ListView1.Items(i).SubItems(2).Text = Replace(message, "CellD", "Cell")
                        If ListView1.Items(i).Tag.ToString.IndexOf(",") < 0 Then
                            tempcell = Replace(message, "CellD:", "")
                            tempcell = Replace(tempcell, Chr(10), "")
                            tempcell = Replace(tempcell, Chr(13), "")
                            tempcell = Trim(tempcell)
                            'writeueiplog(tempcell + ":" + (Len(tempcell).ToString))
                            If Len(Trim(tempcell)) = 4 Then tempcell = "0" + tempcell
                            If Len(Trim(tempcell)) = 3 Then tempcell = "00" + tempcell
                            If Len(Trim(tempcell)) = 2 Then tempcell = "000" + tempcell
                            If Len(Trim(tempcell)) = 1 Then tempcell = "0000" + tempcell
                            tempcell = "Cell:" + tempcell
                            ListView1.Items(i).Tag = tempcell
                        End If

                        ListView1.Items(i).SubItems(2).Tag = ListView1.Items(i).SubItems(2).Text
                    End If
                    If message.IndexOf("CellD") >= 0 And message.IndexOf("RSRPD") > 0 Then

                        tempcell = Replace(VB.Left(message, message.IndexOf("RSRPD")), "CellD", "Cell")
                        temprsrp = Replace(Mid(message, message.IndexOf("RSRPD") + 1), "RSRPD", "RSRP")
                        ListView1.Items(i).SubItems(2).Text = tempcell + vbCr + temprsrp
                        ListView1.Items(i).SubItems(2).Tag = tempcell
                        If ListView1.Items(i).Tag Is Nothing Then
                            ListView1.Items(i).Tag = tempcell
                        Else
                            If ListView1.Items(i).Tag.ToString.IndexOf(",") < 0 Then ListView1.Items(i).Tag = tempcell

                        End If



                    End If


                    'If message.IndexOf("CGREG") >= 0 Then
                    '    If (message.IndexOf("CGREG: 5," + """") >= 0 Or message.IndexOf("CGREG: 1," + """") >= 0) Then
                    '        If findcell5786(message) <> "" Then
                    '            ListView1.Items(i).SubItems(2).Text = " Cell:" + findcell5786(message)
                    '            If ListView1.Items(i).Tag.ToString.IndexOf(",") < 0 Then ListView1.Items(i).Tag = findcell5786(message)
                    '            ListView1.Items(i).SubItems(2).Tag = ListView1.Items(i).SubItems(2).Text
                    '        End If

                    '    End If
                    'End If


                    'If message.IndexOf("+CGREG: 2") >= 0 And UBound(Split(message, ",")) >= 3 Then
                    '    If findcell(message) <> "" Then
                    '        ListView1.Items(i).SubItems(2).Text = " Cell:" + findcell(message)
                    '        If Not (ListView1.Items(i).Tag) Is Nothing Then
                    '            If ListView1.Items(i).Tag.ToString.IndexOf(",") < 0 Then ListView1.Items(i).Tag = findcell(message)
                    '        Else
                    '            ListView1.Items(i).Tag = findcell(message)
                    '        End If

                    '        ListView1.Items(i).SubItems(2).Tag = ListView1.Items(i).SubItems(2).Text
                    '    End If

                    'End If

                    'If message.IndexOf("+CMGSI: Phy_Cellid") >= 0 And UBound(Split(message, ",")) >= 2 Then
                    '    ListView1.Items(i).SubItems(2).Text = " Cell:" + findcell(message)
                    '    ListView1.Items(i).Tag = findcell(message)
                    '    ListView1.Items(i).SubItems(2).Tag = ListView1.Items(i).SubItems(2).Text

                    'End If

                    If message.IndexOf("RSSID") >= 0 Then
                        ListView1.Items(i).SubItems(2).Text = ListView1.Items(i).SubItems(2).Tag + vbCr + Replace(message, "RSSID", " RSSI:")
                        ListView1.Items(i).SubItems(0).Tag = "1"
                    End If

                    If message.IndexOf("RSRPD") >= 0 And message.IndexOf("CellD") < 0 Then
                        ListView1.Items(i).SubItems(2).Text = ListView1.Items(i).SubItems(2).Tag + vbCr + Replace(message, "RSRPD", " RSRP")
                        ListView1.Items(i).SubItems(0).Tag = "1"

                    End If

                    'If message.IndexOf("$QCSQ :") >= 0 And UBound(Split(message, ",")) = 4 Then
                    '    ListView1.Items(i).SubItems(2).Text = ListView1.Items(i).SubItems(2).Tag + vbCr + " RSSI:" + findpower(message)
                    '    ListView1.Items(i).SubItems(0).Tag = "1"
                    'End If
                    'If message.IndexOf("+CMGSI: RX_Power") >= 0 And UBound(Split(message, ",")) >= 2 Then

                    '    ListView1.Items(i).SubItems(2).Text = ListView1.Items(i).SubItems(2).Tag + vbCr + findpower(message)

                    'End If

                    'If message.IndexOf("^HCSQ") >= 0 And UBound(Split(message, ",")) = 4 Then
                    '    ListView1.Items(i).SubItems(2).Text = ListView1.Items(i).SubItems(2).Tag + vbCr + " " + findpowerhs(message)
                    '    ListView1.Items(i).SubItems(0).Tag = "1"
                    'End If

                    'If message.IndexOf("!LTEINFO:") >= 0 And message.IndexOf("IntraFreq:") >= 0 Then
                    '    If findcellsierra(message) <> "" Then
                    '        ListView1.Items(i).SubItems(2).Text = " Cell:" + findcellsierra(message)
                    '        ListView1.Items(i).Tag = findcellsierra(message)
                    '        ListView1.Items(i).SubItems(2).Tag = ListView1.Items(i).SubItems(2).Text
                    '        ListView1.Items(i).SubItems(2).Text = ListView1.Items(i).SubItems(2).Tag + vbCr + " " + findpowersierra(message)
                    '    End If
                    'End If

                    Exit For
                End If

            Catch ex As Exception
                Dim a = 1
            End Try

        Next




    End Sub
    Sub updatecellinfobar()
        Dim dictionry As New Dictionary(Of String, Integer)
        Dim cellid As String = ""
        Dim signalinfo As String = ""
        Dim ipinfo As String = ""
        Dim portinfo As String = ""
        Dim imsi As String = ""
        Dim tempUEinformations As String = ""
        Dim outofcntrol As Integer = 0
        Dim uedlthroughput As Long
        Dim ueulthroughput As Long
        Dim graphicUE As NationalInstruments.UI.WindowsForms.ScatterGraph
        'Try
        For i = 0 To ListView1.Items.Count - 1
            cellid = ""
            signalinfo = ""
            ipinfo = ""
            portinfo = ""
            imsi = ""
            uedlthroughput = 0
            ueulthroughput = 0
            If Not (ListView1.Items(i).Tag Is Nothing) And ListView1.Items(i).Tag <> "" Then
                cellid = ListView1.Items(i).Tag
                If dictionry.ContainsKey(cellid) Then

                    dictionry(cellid) = dictionry(cellid) + 1

                Else
                    dictionry.Add(cellid, 1)
                End If
            End If
            If ListView1.Items(i).SubItems(1).Text = "Out of control" Then outofcntrol = outofcntrol + 1
            signalinfo = ListView1.Items(i).SubItems(2).Text
            portinfo = ListView1.Items(i).SubItems(7).Text
            ipinfo = ListView1.Items(i).SubItems(6).Text.Replace(vbCr, "*")
            'Text1.Text = Text1.Text + vbCrLf + ipinfo
            If ipinfo.IndexOf("*") > 0 Then
                'Text1.Text = Text1.Text + Split(ipinfo, "*")(0)
                ipinfo = Split(ipinfo, "*")(1)
            Else
                ipinfo = ""
            End If
            'Text1.Text = Now.ToLongTimeString + ":" + "ip ok" + vbCrLf + Text1.Text
            imsi = ListView1.Items(i).SubItems(3).Text.Replace(vbCr, "*")
            'Text1.Text = Now.ToLongTimeString + ":" + imsi + vbCrLf + Text1.Text
            If imsi.IndexOf("*") > 0 Then
                'Text1.Text = Now.ToLongTimeString + ":" + "split * and get size " + UBound(Split(ipinfo, "*")).ToString + vbCrLf + Text1.Text
                'UBound(Split(ipinfo, "*"))
                imsi = Split(imsi, "*")(1)
            Else
                imsi = ""
            End If

            graphicUE = ListView1.GetEmbeddedControl(5, i)
            ueulthroughput = graphicUE.Cursors(1).YPosition
            uedlthroughput = graphicUE.Cursors(0).YPosition
            tempUEinformations = tempUEinformations + "|" + portinfo + "," + ipinfo + "," + imsi + "," + uedlthroughput.ToString + "," + ueulthroughput.ToString + "," + signalinfo
        Next
        UEinformations = tempUEinformations
        Dim outputstr As String = ""
        For Each key As String In dictionry.Keys
            outputstr = outputstr + ";" + key + "-" + dictionry(key).ToString
        Next
        If outputstr <> "" Then
            outputstr = VB.Mid(outputstr, 2)
        End If

        Dim ul, dl, nothroughput As String
        updatethroughput(ul, dl, nothroughput)
        outputstr = Replace(outputstr, vbCrLf, "")
        outputstr = Replace(outputstr, vbCr, "")
        outputstr = Replace(outputstr, Chr(10), "")
        outputstr = Replace(outputstr, "Cell:", "")
        outputstr = Replace(outputstr, " ", "")

        MDIForm1.ToolStripStatusLabel1.Text = "Total DL:" + dl + ";Total UL:" + ul + "; Total UE number:" + ListView1.Items.Count.ToString + ";No traffic number:" + nothroughput + ";Out of control number:" + outofcntrol.ToString + " |" + Replace(Replace(outputstr, vbCr, ""), vbCrLf, "") + "|"
        totalstatus = "Total DL:" + dl + ";Total UL:" + ul + "; Total UE number:" + ListView1.Items.Count.ToString + ";No traffic number:" + nothroughput + ";Out of control number:" + outofcntrol.ToString + " |" + Replace(Replace(outputstr, vbCr, ""), vbCrLf, "") + "|"
        'Catch ex As Exception
        '    Dim a = 1
        '    MessageBox.Show(ex.Message.ToString)
        'End Try

    End Sub
    Sub updatethroughput(ByRef UL As String, ByRef DL As String, ByRef nothroughput As String)
        Dim row As Integer = 0
        Dim utp, dtp As Long
        utp = 0
        dtp = 0
        Dim notp As Integer = 0
        Dim graphic As NationalInstruments.UI.WindowsForms.ScatterGraph

        For i = 0 To ListView1.Items.Count - 1


            graphic = ListView1.GetEmbeddedControl(5, i)
            utp = utp + graphic.Cursors(1).YPosition
            dtp = dtp + graphic.Cursors(0).YPosition
            If graphic.Cursors(1).YPosition = 0 And graphic.Cursors(0).YPosition = 0 Then notp = notp + 1
        Next
        If utp < 1024 Then UL = utp.ToString
        If utp >= 1024 And utp < 1024 * 1024 Then UL = (utp / 1024).ToString("F2") + "K"
        If utp >= 1024 * 1024 And utp < 1024 * 1024 * 1024 Then UL = (utp / 1024 / 1024).ToString("F2") + "M"
        If dtp < 1024 Then DL = dtp.ToString
        If dtp >= 1024 And dtp < 1024 * 1024 Then DL = (dtp / 1024).ToString("F2") + "K"
        If dtp >= 1024 * 1024 And dtp < 1024 * 1024 * 1024 Then DL = (dtp / 1024 / 1024).ToString("F2") + "M"

        nothroughput = notp
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
        Return "RSRP:" + rsrp + " SNR:" + snr

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
        Return "rsrp:" + real1.ToString + ",sinr:" + real2.ToString

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
                Return "RSRP:" + rsrp
            Else
                Return "RSRP:" + rsrp + "SINR:" + sinr
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
    Function findueip(ByVal inputstr As String) As String
        Dim regexstr As String
        regexstr = "ip=((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))"
        Dim regex As System.Text.RegularExpressions.Regex = New System.Text.RegularExpressions.Regex(regexstr)
        If regex.IsMatch(inputstr) Then
            Return Replace(System.Text.RegularExpressions.Regex.Match(inputstr, regexstr).Groups(0).Value, "ip=", "UE:")
        Else
            Return "0"
        End If


    End Function

    Sub refreshgrah(ByVal WaveformGraph1 As NationalInstruments.UI.WindowsForms.ScatterGraph, ByVal DL As Integer, ByVal UL As Integer)
        If DL > 1024 Then DL = DL / 1.024
        If DL > 1024 * 1024 Then DL = DL / 1.024 / 1.024

        If UL > 1024 Then UL = UL / 1.024
        If UL > 1024 * 1024 Then UL = UL / 1.024 / 1.024
        WaveformGraph1.Plots(0).PlotXYAppend(WaveformGraph1.Plots(0).GetXData()(WaveformGraph1.Plots(0).GetXData().Length - 1) + 1, DL)
        WaveformGraph1.Plots(1).PlotXYAppend(WaveformGraph1.Plots(0).GetXData()(WaveformGraph1.Plots(0).GetXData().Length - 1) + 1, UL)


        If WaveformGraph1.Plots(0).HistoryCount = 60 Then
            WaveformGraph1.Cursors(0).XPosition = WaveformGraph1.Plots(0).GetXData(WaveformGraph1.Plots(0).HistoryCount - 1) - 30
            WaveformGraph1.Cursors(1).XPosition = WaveformGraph1.Plots(1).GetXData(WaveformGraph1.Plots(1).HistoryCount - 1)
        Else
            WaveformGraph1.Cursors(0).XPosition = WaveformGraph1.Plots(0).GetXData(WaveformGraph1.Plots(0).HistoryCount - 1)
            WaveformGraph1.Cursors(1).XPosition = WaveformGraph1.Plots(1).GetXData(WaveformGraph1.Plots(1).HistoryCount - 1)
        End If
    End Sub
    Sub cleargraphic(ByVal WaveformGraph1 As NationalInstruments.UI.WindowsForms.ScatterGraph)
        WaveformGraph1.Plots(0).ClearData()
        WaveformGraph1.Plots(1).ClearData()
        For h = 1 To 61
            WaveformGraph1.Plots(0).PlotXYAppend(h, 0)
            WaveformGraph1.Plots(1).PlotXYAppend(h, 0)
        Next

    End Sub

    'Sub runTCPserver()
    '    threadFlag = New Object
    '    socketCount = 0
    '    Dim serverScoket As Socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
    '    Dim serverPoint As IPEndPoint = New IPEndPoint(IPAddress.Any, 9104) 'Parse(ip), 9104)
    '    serverScoket.Bind(serverPoint)
    '    serverScoket.Listen(250)
    '    Dim t As Thread = Nothing
    '    Dim clientSocket As Socket = Nothing
    '    Try

    '        While True
    '            If appcloseflag = True Then
    '                clientSocket.Close()
    '                Exit While
    '            End If

    '            While (socketCount >= MAX_SOCKET_COUNT) And appcloseflag = False
    '                Thread.Sleep(1000)

    '            End While
    '            If appcloseflag = True Then
    '                clientSocket.Close()
    '                Exit While
    '            End If
    '            clientSocket = serverScoket.Accept
    '            socketCount = (socketCount + 1)
    '            Dim clientPoint As IPEndPoint = CType(clientSocket.RemoteEndPoint, IPEndPoint)
    '            If consoleon = True Then ConsoleHelper.AllocConsole()
    '            If consoleon = True Then
    '                Console.WriteLine("client {0}:{1} connect", clientPoint.Address, clientPoint.Port)
    '            End If

    '            t = New Thread(New ParameterizedThreadStart(AddressOf ThreadConnect))
    '            t.Name = "TCP connect"
    '            t.Start(clientSocket)

    '        End While
    '    Catch ex As Exception
    '        Dim a = 1
    '    Finally
    '        serverScoket.Close()

    '    End Try

    'End Sub

    'Private Sub ThreadConnect(ByVal clientObj As Object)
    '    Try
    '        Dim clientSocket As Socket = CType(clientObj, Socket)
    '        Dim clientPoint As IPEndPoint = CType(clientSocket.RemoteEndPoint, IPEndPoint)
    '        If (clientSocket Is Nothing) Then
    '            SyncLock threadFlag
    '                socketCount = (socketCount - 1)
    '                Return
    '            End SyncLock
    '        End If

    '        clientSocket.Send(Encoding.ASCII.GetBytes("Hello world" + ",scocknum:" + Str(socketCount)), SocketFlags.None)
    '        Dim datas() As Byte
    '        Dim rec As Integer

    '        While True
    '            datas = New Byte((1024) - 1) {}
    '            rec = clientSocket.Receive(datas)
    '            If appcloseflag = True Then Exit Sub
    '            If (rec = 0) Then
    '                Exit While
    '            End If
    '            dealmsg(Microsoft.VisualBasic.Strings.Left(Encoding.ASCII.GetString(datas), rec))
    '            Dim msg As String = ("Msg has been receive length is " + Str(rec) + vbCrLf + "Msg received:" + Microsoft.VisualBasic.Strings.Left(Encoding.ASCII.GetString(datas), rec) + vbCrLf)
    '            clientSocket.Send(Encoding.ASCII.GetBytes(msg), SocketFlags.None)
    '            If consoleon = True Then ConsoleHelper.AllocConsole()
    '            If consoleon = True Then Console.WriteLine("Msg received:" + Microsoft.VisualBasic.Strings.Left(Encoding.ASCII.GetString(datas), rec))

    '        End While


    '        If consoleon = True Then Console.WriteLine("client {0}:{1} disconnect", clientPoint.Address, clientPoint.Port)
    '        SyncLock threadFlag
    '            socketCount = (socketCount - 1)
    '            clientSocket.Close()
    '        End SyncLock
    '    Catch ex As Exception
    '        Dim a = 1

    '    End Try
    'End Sub

    Public Sub dealmsg(ByVal dealmsg As String)
        Dim msgs As Object
        Dim tempstr As Object
        Dim midstr, outputstr As String
        Dim uename As String
        Dim TP1, TP2 As String

        Try

            msgs = Split(dealmsg, "~")
            For i = 1 To UBound(msgs)
                If Trim(msgs(i)) <> "" Then
                    tempstr = Split(msgs(i), "|")
                    If UBound(tempstr) >= 2 Then

                        If tempstr(0) = "TP" And tempstr(2) = "s" Then
                            If UBound(tempstr) >= 4 Then
                                If Trim(tempstr(4)) <> "" Then
                                    uename = tempstr(1)
                                    midstr = Trim(tempstr(4))
                                    TP1 = (Mid(midstr, 1, midstr.IndexOf(" ")))
                                    TP2 = (Trim(Mid(midstr, midstr.IndexOf(" ") + 1, midstr.Length)))

                                    outputstr = uename + "|" + tempstr(2) + "|" + TP1 + "|" + TP2
                                    'updateinfo(uename, TP1, TP2)
                                    If TPmsgpool.Count >= 1000 Then TPmsgpool.RemoveAt(0)
                                    TPmsgpool.Add(outputstr)
                                    'If mdiform1.consoleon=true = True Then Console.WriteLine("TPmsgpool size:" + TPmsgpool.Count.ToString)
                                End If
                            End If
                        ElseIf tempstr(0) = "OP" Then
                            'writelog(tempstr)

                            If OPmsgpool.Count >= 1000 Then OPmsgpool.RemoveAt(0)
                            If OPmmsgpool.Count >= 1000 Then OPmmsgpool.RemoveAt(0)
                            If UBound(tempstr) >= 3 Then
                                outputstr = tempstr(1) + "|" + tempstr(3)
                                OPmsgpool.Add(outputstr)
                                outputstr = tempstr(1) + "|" + tempstr(2) + "|" + tempstr(3)
                                OPmmsgpool.Add(outputstr)
                                If consoleon = True Then Console.WriteLine("OPmsgpool size:" + OPmsgpool.Count.ToString)
                                If consoleon = True Then Console.WriteLine("OPmmsgpool size:" + OPmmsgpool.Count.ToString)
                            End If
                        ElseIf tempstr(0) = "TP" And tempstr(2) = "i" Then
                            If UBound(tempstr) >= 4 Then
                                uename = tempstr(1)
                                outputstr = uename + "|" + tempstr(3) + "|" + tempstr(4)
                                'updateinfo(uename, TP1, TP2)
                                If TPmmsgpool.Count >= 1000 Then TPmmsgpool.RemoveAt(0)
                                TPmmsgpool.Add(outputstr)
                                If consoleon = True Then Console.WriteLine("TPmmsgpool size:" + TPmmsgpool.Count.ToString)
                            End If
                        End If
                    End If
                End If

            Next
        Catch ex As Exception
            If consoleon = True Then Console.WriteLine(ex.Message.ToString)
        End Try

    End Sub

    Sub updateinfo(ByVal uename, ByVal TP1, ByVal TP2)
        Dim row As Integer = 0
        Dim graphic As NationalInstruments.UI.WindowsForms.ScatterGraph
        For i = 0 To ListView1.Items.Count - 1
            If ListView1.Items(i).SubItems(3).Tag = uename Then
                ueliststate(uename) = Now
                graphic = ListView1.GetEmbeddedControl(5, i)
                refreshgrah(graphic, TP1, TP2)

                Exit Sub
            End If


        Next


    End Sub
    Sub clearTPinfo(ByVal uename)
        Dim row As Integer = 0
        Dim graphic As NationalInstruments.UI.WindowsForms.ScatterGraph
        For i = 0 To ListView1.Items.Count - 1
            If ListView1.Items(i).SubItems(3).Tag = uename Then
                graphic = ListView1.GetEmbeddedControl(5, i)
                cleargraphic(graphic)
                Exit Sub
            End If


        Next
    End Sub






    Private Sub ListViewEx1_ItemChecked(ByVal sender As Object, ByVal e As System.Windows.Forms.ItemCheckedEventArgs) Handles ListView1.ItemChecked
        Dim Item As System.Windows.Forms.ListViewItem
        Dim i As Integer
        Dim UEcip As String
        Dim mode As Boolean
        mode = MDIForm1.singleUEselect
        If mode = False Then
            Item = e.Item
            MDIForm1.ToolStripStatusLabel1.Text = "Checking same board UE check state"
            Try
                If Item.SubItems.Count > 3 And ListView1.Items.Count > 0 Then
                    UEcip = Item.SubItems(4).Text
                    For i = 0 To ListView1.Items.Count - 1

                        If ListView1.Items.Item(i).SubItems(4).Text = UEcip And i <> Item.Index Then
                            If Item.Checked = True And ListView1.Items.Item(i).Checked = False Then
                                ListView1.Items.Item(i).Checked = True
                            End If
                            If Item.Checked = False And ListView1.Items.Item(i).Checked = True Then
                                ListView1.Items.Item(i).Checked = False
                            End If
                        End If

                        System.Windows.Forms.Application.DoEvents()
                    Next
                End If
            Catch ex As Exception
                Dim a = 1
            End Try
            MDIForm1.ToolStripStatusLabel1.Text = ""
        End If
    End Sub

    Enum Event1

        OneWay = 1

        _Return = 2

        PushMessage = 3

        Bin = 4
    End Enum
    Class BinMessageParse
        Inherits MessageParse

        Public Sub New()
            MyBase.New(New BinSerializer, New BinSerializer)

        End Sub
        Overrides Function ProcessMessage(ByVal SCBID As Integer, ByVal RemoteIPEndPoint As System.Net.EndPoint, ByVal Flag As NTCPMSG.MessageFlag, ByVal CableId As UShort, ByVal Channel As UInteger, ByVal Event1 As UInteger, ByVal obj As Object) As Object
            Console.WriteLine(obj)
            Return Nothing
        End Function
    End Class
    Function getueiplist()
        Return idleueliststring
    End Function

    Private Shared _sBinMessageParse As BinMessageParse = New BinMessageParse

    ''' <summary>
    ''' DataReceived event will be called back when server get message from client which connect to.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    Private Sub ReceiveEventHandler(ByVal sender As Object, ByVal args As ReceiveEventArgs)
        Select Case (CType(args.Event, Event1))
            Case Event1.OneWay
                'Get OneWay message from client
                If (Not (args.Data) Is Nothing) Then

                    Try
                        'CType(sender, NTcpListener).AsyncSend(args.RemoteIPEndPoint, 1, Encoding.ASCII.GetBytes("!"))
                        'If paginguelistchanged = True Then
                        CType(sender, NTcpListener).AsyncSend(args.RemoteIPEndPoint, 1, Encoding.ASCII.GetBytes("!" + getueiplist()))
                        'paginguelistchanged = False
                        'End If
                        If (args.CableId <> 0) Then
                            Console.WriteLine("Get one way message from cable {0}", args.CableId)
                        Else
                            Console.WriteLine("Get one way message from {0}", args.RemoteIPEndPoint)
                        End If

                        Console.WriteLine(Encoding.UTF8.GetString(args.Data))
                    Catch e As Exception
                        Console.WriteLine(e)
                    End Try
                    ' send.AsyncSend(clientIpEndPoint, CType(Event1.PushMessage, UInteger), Encoding.UTF8.GetBytes("I am from server!"))
                End If

            Case Event1._Return
                'Get return message from client
                If (Not (args.Data) Is Nothing) Then
                    Try
                        ' If consoleon = True Then ConsoleHelper.AttachConsole(ConsoleHelper.ATTACH_PARENT_PROCESS)
                        If consoleon = True Then Console.WriteLine(Encoding.UTF8.GetString(args.Data))
                        dealmsg(Encoding.ASCII.GetString(args.Data))
                        Dim fromClient As Integer = BitConverter.ToInt32(args.Data, 0)
                        args.ReturnData = BitConverter.GetBytes(++fromClient)
                    Catch e As Exception
                        Console.WriteLine(e)
                    End Try

                End If

            Case Event1.Bin
                _sBinMessageParse.ReceiveEventHandler(sender, args)
        End Select

    End Sub

    ''' <summary>
    ''' RemoteDisconnected event will be called back when specified client disconnected.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    Private Shared Sub DisconnectEventHandler(ByVal sender As Object, ByVal args As DisconnectEventArgs)
        If consoleon = True Then Console.WriteLine("Remote socket:{0} disconnected.", args.RemoteIPEndPoint)
    End Sub

    ''' <summary>
    ''' Accepted event will be called back when specified client connected
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="args"></param>
    Private Shared Sub AcceptedEventHandler(ByVal sender As Object, ByVal args As AcceptEventArgs)
        If consoleon = True Then Console.WriteLine("Remote socket:{0} connected.", args.RemoteIPEndPoint)
    End Sub



    Public Sub RunTCPserver(ByVal args() As String)
        Dim listener As NTCPMSG.Server.NTcpListener
        'Create a tcp listener that listen 2500 TCP port.
        listener = New NTcpListener(New IPEndPoint(IPAddress.Any, 2500))
        'DataReceived event will be called back when server get message from client which connect to.
        AddHandler listener.DataReceived, AddressOf ReceiveEventHandler
        'RemoteDisconnected event will be called back when specified client disconnected.
        AddHandler listener.RemoteDisconnected, AddressOf DisconnectEventHandler
        'Accepted event will be called back when specified client connected
        AddHandler listener.Accepted, AddressOf AcceptedEventHandler
        'Start listening.
        'This function will not block current thread.
        listener.Listen()
        If consoleon = True Then Console.WriteLine("Listening...")

        'While True
        '    System.Threading.Thread.Sleep((5 * 1000))
        '    'Push message to client example.
        '    For Each clientIpEndPoint As IPEndPoint In listener.GetRemoteEndPoints
        '        Dim successful As Boolean = listener.AsyncSend(clientIpEndPoint, CType(Event1.PushMessage, UInteger), Encoding.UTF8.GetBytes("I am from server!"))
        '        If successful Then
        '            Console.WriteLine(String.Format("Push message to {0} successful!", clientIpEndPoint))
        '        Else
        '            Console.WriteLine(String.Format("Push message to {0} fail!", clientIpEndPoint))
        '        End If

        '    Next
        '    For Each cableId As UInt16 In listener.GetCableIds
        '        Dim successful As Boolean = listener.AsyncSend(cableId, CType(Event1.PushMessage, UInteger), Encoding.UTF8.GetBytes(String.Format("Hi cable {0}!", cableId)))
        '        If successful Then
        '            Console.WriteLine(String.Format("Push message to cable {0} successful!", cableId))
        '        Else
        '            Console.WriteLine(String.Format("Push message to cable {0} fail!", cableId))
        '        End If

        '    Next

        'End While
        'System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite)
    End Sub

    Private Sub SplitContainer2_SplitterMoved(ByVal sender As System.Object, ByVal e As System.Windows.Forms.SplitterEventArgs) Handles SplitContainer2.SplitterMoved

    End Sub


    Private Sub SplitContainer2_Panel1_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles SplitContainer2.Panel1.Paint

    End Sub
End Class