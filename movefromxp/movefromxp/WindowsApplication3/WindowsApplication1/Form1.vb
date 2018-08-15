Imports System.Management
Imports System.IO.Ports
Imports System.Net.NetworkInformation
Imports Microsoft.Win32  '用途 ： 注册表操作 
Imports System.Security.AccessControl '用途 ： 访问权限控制  
Imports VB = Microsoft.VisualBasic
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Runtime.InteropServices
Imports System.Net
Imports System
Imports NTCPMSG

Imports NTCPMSG.Client
Imports NTCPMSG.Event
Imports System.Text

Public Class Form1

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub

    Dim ip As String = ""
    Dim port As Integer = 2501
    Dim syncflag As Boolean = True
    Dim Mytcpclient As NTCPMSG.Client.SingleConnection
    Dim messagebuffer As New List(Of String)
    Dim _continue As Boolean = True
    Dim messagepool As New List(Of String)
    Sub changeconnectionname(ByVal soldname As String, ByVal snewname As String)

        Const NETWORK_CONNECTIONS = &H31&






        Dim objShell, objFolder, colItems, objItem
        Try
            objShell = CreateObject("Shell.Application")
            objFolder = objShell.Namespace(NETWORK_CONNECTIONS)

            colItems = objFolder.Items

            For Each objItem In colItems
                'Console.WriteLine(objItem.Name)
                If objItem.Name.ToString.ToLower = soldname.ToLower Then
                    objItem.Name = snewname
                End If
                'Console.WriteLine(objItem.Name)
            Next
        Catch
            'MsgBox("The name is already used with other adaptor")
        End Try
    End Sub

    Sub getallinformation(ByVal freshall As Boolean)
        Dim s, tmps As String
        ' Dim obj2 As String
        Dim avalibleports As New Collection
        Dim find As Boolean


        Dim wmiobjectset As Object

        If freshall Then
            ListBox1.Items.Clear()
            For Each s In SerialPort.GetPortNames
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
                Dim searcher As New ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity ")
                For Each mgt As ManagementObject In searcher.[Get]()
                    Try
                        If mgt("Name").ToString().IndexOf("(" + s.ToString + ")") > 0 Then
                            'Label1.Text = "311"
                            tmps = Trim(mgt("Name").ToString())
                            If tmps.IndexOf("BandLuxe Wideband AT CMD Interface") >= 0 Or tmps.IndexOf("Altair LTE Application Interface") >= 0 Or tmps.IndexOf("HUAWEI Mobile Connect - PC UI Interface") >= 0 Or tmps.IndexOf("SimTech HS-USB AT Port") >= 0 Then ' add more type dongles
                                ' avalibleports.Add(mgt("Name"), s.ToString)
                                If ListBox1.FindString(mgt("Name")) = -1 Then
                                    ListBox1.Items.Add(mgt("Name"))
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
                                    If ListBox1.FindString(modemname & "(" & s.ToUpper & ")") = -1 Then
                                        ListBox1.Items.Add(modemname & "(" & s.ToUpper & ")")

                                    End If


                                End If
                            End If
                        End If



                    End If

                Next






            Next


        End If

        'Label1.Text = "33"
        ListBox2.Items.Clear()
        Dim adapters As System.Net.NetworkInformation.NetworkInterface() = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
        Dim adapter As System.Net.NetworkInformation.NetworkInterface
        For Each adapter In adapters
            Dim properties As System.Net.NetworkInformation.IPInterfaceProperties = adapter.GetIPProperties()
            If adapter.Name.IndexOf("Loopback") < 0 And adapter.Name.IndexOf("isatap") < 0 And adapter.Name.IndexOf("Bluetooth") < 0 Then
                ListBox2.Items.Add(adapter.Name + "--" + adapter.Description)
            End If

        Next adapter

        Dim realname As String
        Dim path As String = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        path = path + "\Microsoft\Network\Connections\Pbk\rasphone.pbk"

        If IO.File.Exists(path) Then
            For i = 1 To Module1.TotalSections(path)
                realname = Module1.GetSection(path, i - 1)
                ListBox2.Items.Add(realname + "--" + Module1.ReadKeyVal(path, realname, "Device"))
            Next
        End If

        path = IO.Path.GetPathRoot(path)
        path = path + "\Users\All Users\Microsoft\Network\Connections\PBK\rasphone.pbk"

        If IO.File.Exists(path) Then
            For i = 1 To Module1.TotalSections(path)
                realname = Module1.GetSection(path, i - 1)
                ListBox2.Items.Add(realname + "--" + Module1.ReadKeyVal(path, realname, "Device"))
            Next
        End If




    End Sub






    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
 
    End Sub

    Public Function GetRndString(ByVal lngNum As Long) As String

        If lngNum <= 0 Then
            GetRndString = ""
            Exit Function
        End If

        Dim i As Long
        Dim intLength As Integer
        Const STRINGSOURCE = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
        GetRndString = ""
        intLength = Len(STRINGSOURCE) - 1
        Randomize()
        For i = 1 To lngNum
            GetRndString = GetRndString & Mid(STRINGSOURCE, Int(Rnd() * intLength + 1), 1)
        Next
    End Function

    Private Sub ListBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub Button2_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim oldname, newname As String
        Randomize()
        If ListBox2.SelectedItem <> Nothing Then
            oldname = Microsoft.VisualBasic.Left(ListBox2.SelectedItem.ToString, ListBox2.SelectedItem.ToString.IndexOf("--"))
            newname = "Local Area Connection " & GetRndString(4)
            changeconnectionname(oldname, newname)
            getallinformation(False)
        End If
    End Sub

    Private Sub releaseCOM(ByVal searchedcom As String)
        Dim Key1 As Microsoft.Win32.RegistryKey
        Key1 = My.Computer.Registry.LocalMachine  '返回当前用户键 

        Key1 = Key1.OpenSubKey("SYSTEM\ControlSet001\Control\Network\{4D36E972-E325-11CE-BFC1-08002BE10318}", True)

        If Key1.SubKeyCount > 0 Then
            Dim keyName() As String
            Dim keyTemp As Microsoft.Win32.RegistryKey
            keyName = Key1.GetSubKeyNames
            Dim i As Integer
            For i = 1 To keyName.GetLength(0) - 1
                Try
                    'sb.AppendLine(keyName(i))
                    keyTemp = Key1.OpenSubKey(keyName(i), True)
                    keyTemp = keyTemp.OpenSubKey("Connection", True)

                    If searchedcom.ToLower = keyTemp.GetValue("Name").ToString.ToLower Then
                        ' keyTemp.SetAccessControl()
                        'Dim RegkeyAcl As RegistrySecurity = keyTemp.GetAccessControl(AccessControlSections.All)
                        'Dim AccessRule As RegistryAccessRule = New RegistryAccessRule("Everyone", RegistryRights.FullControl, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow)
                        'RegkeyAcl.AddAccessRule(AccessRule)
                        'keyTemp.SetAccessControl(RegkeyAcl)
                        keyTemp.SetValue("Name", "Local Area Connection " & GetRndString(4))


                    End If
                Catch ex As Exception
                    MessageBox.Show("error")
                End Try
            Next
        End If
    End Sub



    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim oldname, newname As String
        If ListBox1.SelectedItem <> Nothing And ListBox2.SelectedItem <> Nothing Then
            newname = Microsoft.VisualBasic.Mid(ListBox1.SelectedItem.ToString, ListBox1.SelectedItem.ToString.IndexOf("COM") + 1, Len(ListBox1.SelectedItem.ToString) - 1 - ListBox1.SelectedItem.ToString.IndexOf("COM"))
            oldname = Microsoft.VisualBasic.Left(ListBox2.SelectedItem.ToString, ListBox2.SelectedItem.ToString.IndexOf("--"))
            If newname <> oldname Then
                changeconnectionname(oldname, newname)
                getallinformation(False)
            End If
        End If
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Button3.Enabled = False
        Me.Cursor = Cursors.WaitCursor
        getallinformation(True)
        Me.Cursor = Cursors.Default
        Button3.Enabled = True
    End Sub

    Private Sub ListBox2_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListBox2.SelectedIndexChanged

    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Dim oldname As String
        Randomize()
        If ListBox1.SelectedItem <> Nothing Then
            oldname = Microsoft.VisualBasic.Mid(ListBox1.SelectedItem.ToString, ListBox1.SelectedItem.ToString.IndexOf("COM") + 1, Len(ListBox1.SelectedItem.ToString) - 1 - ListBox1.SelectedItem.ToString.IndexOf("COM"))
            'newname = "Local Area Connection " & GetRndString(4)
            releaseCOM(oldname)
            'getallinformation()
        End If
    End Sub

    Private Function checkipconflict(ByVal iplists As Collection) As Boolean
        Dim cardinformation(4) As String '0.cardname,1.ip,2.gateway,3,carddiscription,4.imsicode
        Dim cardinformation2(4) As String '0.cardname,1.ip,2.gateway,3,carddiscription,4.imsicode
        checkipconflict = 0

        For Each cardinformation In iplists

            For Each cardinformation2 In iplists
                If cardinformation(2) = cardinformation2(2) And cardinformation(3) <> cardinformation2(3) Then

                    MsgBox("same gateway ip " & cardinformation(2) & "was used by UE ip" & cardinformation(1) & " and " & cardinformation2(1))
                    checkipconflict = 1
                    Exit Function
                End If


            Next




        Next


    End Function
    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        Dim cardinformation(4) As String '0.cardname,1.ip,2.gateway,3,carddiscription,4.imsicode
        Dim cardinformations As New Collection
        Dim imislists As New Collection
        Dim HISICOMlists As New Collection
        Dim hisicom(2) As String '1.comname,2.imsicode
        getallHISImifiactiveports(cardinformations)
        'getimislist(imislists, cardinformations)
        gethisicomport(cardinformations)
        If checkipconflict(cardinformations) = False And cardinformations.Count > 0 Then

            'getHISICOMlist(HISICOMlists)






            For Each cardinformation In cardinformations
                If cardinformation(0).ToUpper <> cardinformation(4).ToUpper Then
                    Console.WriteLine("Try change name from " & cardinformation(0) & " " & cardinformation(4))
                    changeconnectionname(cardinformation(0), cardinformation(4))
                End If

            Next





            getallinformation(False)
        End If
    End Sub
    Private Sub getHISICOMlist(ByVal hisicomlists As Collection)
        Dim wmiobjectset As Object
        Dim s, tmps As String
        Dim avalibleports As New Collection
        Dim find As Boolean
        Dim comportname As String
        Dim imiscode As String

        For Each s In SerialPort.GetPortNames

            wmiobjectset = Nothing


            find = False
            Dim searcher As New ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity ")
            'Console.WriteLine("1")
            Try
                For Each mgt As ManagementObject In searcher.[Get]()
                    'Console.WriteLine("2")

                    If mgt("Name").ToString().IndexOf(s.ToString) > 0 Then
                        'Console.WriteLine("3")
                        tmps = Trim(mgt("Name").ToString())
                        If tmps.IndexOf("HUAWEI Mobile Connect - PC UI Interface") >= 0 Then ' add more type dongles
                            Console.WriteLine("4")
                            comportname = Microsoft.VisualBasic.Mid(tmps, tmps.IndexOf("COM") + 1, Len(tmps) - 1 - tmps.IndexOf("COM"))
                            imiscode = serialportreadimis(comportname)
                            Console.WriteLine(comportname & ":" & imiscode)
                            If imiscode <> "" Then
                                Dim hisicom(2) As String
                                hisicom(1) = comportname
                                hisicom(2) = imiscode
                                hisicomlists.Add(hisicom)
                            End If


                        End If

                        Exit For
                    End If
                Next
            Catch ex As Exception
            End Try
        Next


    End Sub
    Private Sub getqualcommlist(ByVal hisicomlists As Collection)
        Dim wmiobjectset As Object
        Dim s, tmps As String
        Dim avalibleports As New Collection
        Dim find As Boolean
        Dim comportname As String
        ' Dim imiscode As String
        Dim rootkey As Microsoft.Win32.RegistryKey
        Dim regdirlist As Object
        Dim modemname As String
        Dim comname As String
        Dim tempstring As String
        Dim tempint As Integer = 0
        rootkey = My.Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\ControlSet001\Control\Class\{4D36E96D-E325-11CE-BFC1-08002BE10318}")
        regdirlist = rootkey.GetSubKeyNames
        For Each s In SerialPort.GetPortNames

            wmiobjectset = Nothing

            'Label1.Text = "1"
            find = False
            Dim searcher As New ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity ")

            For Each mgt As ManagementObject In searcher.[Get]()
                Try
                    tempstring = mgt("Name").ToString()
                    'Label1.Text = "11"
                    tempint = tempint + 1
                    'Label1.Text = "11:" + tempint.ToString

                    If Not (mgt("name") Is Nothing) Then
                        ' MessageBox.Show(s.ToString + "|" + mgt("Name").ToString)
                        If mgt("Name").ToString().IndexOf(s.ToString) > 0 Then
                            tmps = Trim(mgt("Name").ToString())
                            'Label2.Text = "12"
                            If tmps.IndexOf("Qualcomm HS-USB Modem") >= 0 Or tmps.IndexOf("Qualcomm HS-USB Android Modem 9025") >= 0 Or tmps.IndexOf("SimTech HS-USB AT Port") >= 0 Or tmps.IndexOf("SAMSUNG Mobile USB Modem") >= 0 or tmps.IndexOf("Quectel USB AT Port") >= 0 Then ' add more type dongles
                                'Label2.Text = "13"
                                comportname = Microsoft.VisualBasic.Mid(tmps, tmps.IndexOf("COM") + 1, Len(tmps) - 1 - tmps.IndexOf("COM"))
                                ' imiscode = serialportreadimis(comportname)
                                'Console.WriteLine(comportname & ":" & imiscode)
                                'If imiscode <> "" Then
                                Dim hisicom(2) As String
                                hisicom(0) = tmps
                                hisicom(1) = comportname
                                regsearch(0) = Nothing
                                regsearch(1) = Nothing
                                regsearch(2) = Nothing
                                regsearch(3) = Nothing
                                SearchSubKeys(My.Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\ControlSet001\Enum\USB"), hisicom(1))
                                If regsearch(3) <> Nothing Then
                                    regsearch(1) = Mid(regsearch(1), 1, regsearch(1).LastIndexOf("\"))
                                    hisicom(2) = Split(Mid(regsearch(1), regsearch(1).LastIndexOf("\") + 2, regsearch(1).Length - regsearch(1).LastIndexOf("\")), "&")(0) + "&" + Split(Mid(regsearch(1), regsearch(1).LastIndexOf("\") + 2, regsearch(1).Length - regsearch(1).LastIndexOf("\")), "&")(1) + "&" + Split(Mid(regsearch(1), regsearch(1).LastIndexOf("\") + 2, regsearch(1).Length - regsearch(1).LastIndexOf("\")), "&")(2)

                                End If
                                'hisicom(2) = imiscode
                                hisicomlists.Add(hisicom)
                                'End If
                                Console.WriteLine(hisicom(0) + "," + hisicom(1) + "," + hisicom(2))

                            End If

                            Exit For
                        End If
                    End If
                Catch aa As Exception

                    'MessageBox.Show(aa.ToString)
                End Try

            Next
            'Label1.Text = "2"
        For Each regdir As Object In regdirlist
            Dim a = 1
                'Label1.Text = "21"
            If regdir.ToString <> "Properties" Then
                comname = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\Class\{4D36E96D-E325-11CE-BFC1-08002BE10318}\" & regdir, "AttachedTo", String.Empty)
                If comname.ToLower = s.ToString.ToLower Then
                    modemname = My.Computer.Registry.GetValue("HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\Class\{4D36E96D-E325-11CE-BFC1-08002BE10318}\" & regdir, "DriverDesc", String.Empty)
                        'Label1.Text = "22"
                    If modemname <> "" And modemname.IndexOf("SimTech") Then
                        'imiscode = serialportreadimis(s.ToUpper)
                        'Console.WriteLine(s.ToUpper & ":" & imiscode)
                        'If imiscode <> "" Then
                        Dim hisicom(2) As String
                        hisicom(1) = s.ToUpper
                        hisicom(0) = modemname
                        regsearch(0) = Nothing
                        regsearch(1) = Nothing
                        regsearch(2) = Nothing
                        regsearch(3) = Nothing
                        SearchSubKeys(My.Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\ControlSet001\Enum\USB"), hisicom(1))
                        If regsearch(3) <> Nothing Then
                            regsearch(1) = Mid(regsearch(1), 1, regsearch(1).LastIndexOf("\"))
                            hisicom(2) = Split(Mid(regsearch(1), regsearch(1).LastIndexOf("\") + 2, regsearch(1).Length - regsearch(1).LastIndexOf("\")), "&")(0) + "&" + Split(Mid(regsearch(1), regsearch(1).LastIndexOf("\") + 2, regsearch(1).Length - regsearch(1).LastIndexOf("\")), "&")(1) + "&" + Split(Mid(regsearch(1), regsearch(1).LastIndexOf("\") + 2, regsearch(1).Length - regsearch(1).LastIndexOf("\")), "&")(2)

                        End If
                        Console.WriteLine(hisicom(0) + "," + hisicom(1) + "," + hisicom(2))
                        '   hisicom(2) = imiscode
                        hisicomlists.Add(hisicom)
                        'End If



                    End If

                End If



            End If

        Next
        Next

       
       





    End Sub
    Sub gethisicomport(ByVal cardinformations As Collection)
        For Each cardinformation In cardinformations
            Dim cominfo As String
            cominfo = geteachcominfhisi(cardinformation(0))
            If cominfo Is Nothing Then
            Else
                cardinformation(4) = Mid(cominfo, cominfo.IndexOf("(") + 2, 6).Replace(")", "")
            End If
        Next cardinformation
    End Sub

    Function geteachcominfhisi(ByVal connectionanme As String)
        Dim netcardsubkey As String = ""
        Dim comsubkey As String = ""
        Dim comkey As String = ""
        Dim comname As String = ""
        'Dim rootkey As Microsoft.Win32.RegistryKey
        'Dim checkkey As Microsoft.Win32.RegistryKey
        'Dim regdirlist As Object

        '------------search in 001------5786
        ' rootkey = My.Computer.Registry.LocalMachine.OpenSubKey("HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\Network")
        'regdirlist = rootkey.GetSubKeyNames
        regsearch(0) = Nothing
        regsearch(1) = Nothing
        regsearch(2) = Nothing
        regsearch(3) = Nothing

        SearchSubKeys(My.Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\ControlSet001\Control\Network"), connectionanme)
        If regsearch(3) <> Nothing Then
            regsearch(1) = Mid(regsearch(1), 1, regsearch(1).LastIndexOf("\"))
            netcardsubkey = My.Computer.Registry.GetValue(regsearch(1) + "\Connection", "PnpInstanceID", String.Empty)
            comsubkey = Microsoft.VisualBasic.Strings.Right(netcardsubkey, netcardsubkey.Length - netcardsubkey.LastIndexOf("\") - 1)
            comkey = "HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Enum\USB\Vid_12D1&Subclass_03&Prot_12\" + Microsoft.VisualBasic.Strings.Left(comsubkey, comsubkey.Length - 1).ToLower + "0"
            comname = My.Computer.Registry.GetValue(comkey, "FriendlyName", String.Empty)
            If comname <> "" Then

                Return comname

            End If

        End If
        'go on search in002 -------5786
        regsearch(0) = Nothing
        regsearch(1) = Nothing
        regsearch(2) = Nothing
        regsearch(3) = Nothing

        SearchSubKeys(My.Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\ControlSet002\Control\Network"), connectionanme)
        If regsearch(3) <> Nothing Then
            regsearch(1) = Mid(regsearch(1), 1, regsearch(1).LastIndexOf("\"))
            netcardsubkey = My.Computer.Registry.GetValue(regsearch(1) + "\Connection", "PnpInstanceID", String.Empty)
            comsubkey = Microsoft.VisualBasic.Strings.Right(netcardsubkey, netcardsubkey.Length - netcardsubkey.LastIndexOf("\") - 1)
            comkey = "HKEY_LOCAL_MACHINE\SYSTEM\ControlSet002\Enum\USB\Vid_12D1&Subclass_03&Prot_12\" + Microsoft.VisualBasic.Strings.Left(comsubkey, comsubkey.Length - 1).ToLower + "0"
            comname = My.Computer.Registry.GetValue(comkey, "FriendlyName", String.Empty)
            If comname <> "" Then

                Return comname

            End If
        End If
        'go on search in 001 ------5375/5776
        regsearch(0) = Nothing
        regsearch(1) = Nothing
        regsearch(2) = Nothing
        regsearch(3) = Nothing

        SearchSubKeys(My.Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\ControlSet001\Control\Network"), connectionanme)
        If regsearch(3) <> Nothing Then
            regsearch(1) = Mid(regsearch(1), 1, regsearch(1).LastIndexOf("\"))
            netcardsubkey = My.Computer.Registry.GetValue(regsearch(1) + "\Connection", "PnpInstanceID", String.Empty)
            comsubkey = Microsoft.VisualBasic.Strings.Right(netcardsubkey, netcardsubkey.Length - netcardsubkey.LastIndexOf("\") - 1)
            comsubkey = comsubkey.Replace("0001_00", "0000_00")
            comkey = "HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Enum\usbcdcacm\Vid_12D1&Subclass_02&Prot_12\" + comsubkey
            comname = My.Computer.Registry.GetValue(comkey, "FriendlyName", String.Empty)
            If comname <> "" Then

                Return comname

            End If
        End If
        'go on search in 002 ------5375/5776
        regsearch(0) = Nothing
        regsearch(1) = Nothing
        regsearch(2) = Nothing
        regsearch(3) = Nothing

        SearchSubKeys(My.Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\ControlSet002\Control\Network"), connectionanme)
        If regsearch(3) <> Nothing Then
            regsearch(1) = Mid(regsearch(1), 1, regsearch(1).LastIndexOf("\"))
            netcardsubkey = My.Computer.Registry.GetValue(regsearch(1) + "\Connection", "PnpInstanceID", String.Empty)
            comsubkey = Microsoft.VisualBasic.Strings.Right(netcardsubkey, netcardsubkey.Length - netcardsubkey.LastIndexOf("\") - 1)
            comsubkey = comsubkey.Replace("0001_00", "0000_00")
            comkey = "HKEY_LOCAL_MACHINE\SYSTEM\ControlSet002\Enum\usbcdcacm\Vid_12D1&Subclass_02&Prot_12\" + comsubkey
            comname = My.Computer.Registry.GetValue(comkey, "FriendlyName", String.Empty)
            If comname <> "" Then

                Return comname

            End If
        End If








    End Function

    Private Sub getimislist(ByVal outputimislists As Collection, ByVal cardinformations As Collection)
        'Dim cardinfomation(4) As String
        For Each cardinformation In cardinformations
            Dim imisinf As String
            imisinf = getimisinf(cardinformation(2))
            cardinformation(4) = imisinf
            outputimislists.Add(imisinf)
        Next cardinformation


    End Sub

    Private Function getimisinf(ByRef gateway As String) As String
        Dim sbTemp As System.Text.StringBuilder = New System.Text.StringBuilder()
        sbTemp.Append("<request><Username>admin</Username><Password>YWRtaW4=</Password></request>")
        Dim bTemp() As Byte = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(sbTemp.ToString())
        Dim postReturn As String = doPostRequest("http://" + gateway + "/api/user/login", bTemp)
        Console.WriteLine("Post response is: " + postReturn)
        Dim sbTemp2 As System.Text.StringBuilder = New System.Text.StringBuilder()
        sbTemp2.Append("") '("<request><Control>1</Control></request>")
        Dim bTemp2() As Byte = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(sbTemp2.ToString())
        postReturn = doGetRequest("http://" + gateway + "/api/device/information")
        Console.WriteLine("Post response is: " + postReturn)
        Dim rightposition As Integer
        Dim leftposition As Integer
        leftposition = postReturn.IndexOf("<Imsi>") + 7
        rightposition = postReturn.IndexOf("</Imsi>") + 1
        getimisinf = Mid(postReturn, leftposition, rightposition - leftposition)
        Console.WriteLine("IMSI:" & getimisinf)
    End Function

    '发送HTTP GET请求得结果
    Private Function doGetRequest(ByVal url As String) As String
        Dim webClient As New System.Net.WebClient
        Dim result As String = webClient.DownloadString(url)
        Return result
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

    Private Function getallHISImifiactiveports(ByVal outputinfo As Collection)
        Dim cardinfomation(4) As String '0.cardname,1.ip,2.gateway,3.carddiscription,4.imsi
        Dim adapters As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
        Dim adapter As NetworkInterface
        For Each adapter In adapters
            Dim properties As IPInterfaceProperties = adapter.GetIPProperties()
            If (adapter.Description.IndexOf("HUAWEI Mobile") >= 0) And adapter.OperationalStatus.ToString = "Up" Then '(adapter.Description.IndexOf("HUAWEI Mobile") >= 0) And 
                cardinfomation(0) = adapter.Name.ToString
                cardinfomation(3) = adapter.Description.ToString
                Dim unicastipaddressinformationcollection As UnicastIPAddressInformationCollection = properties.UnicastAddresses
                Dim unicastip As UnicastIPAddressInformation
                For Each unicastip In unicastipaddressinformationcollection
                    'displaylog("  ip address............:{0}" & unicastip.Address.ToString, "g")
                    'displaylog("  " & unicastip.Address.AddressFamily.ToString, "g")
                    ' If isIPV4 And unicastip.Address.AddressFamily.ToString = "InterNetwork" Then 'InterNetworkV6 means ipV6 address
                    'cardinfomation(1) = unicastip.Address.ToString 'only one ip ge
                    'Exit For
                    'ElseIf isIPV4 = False And unicastip.Address.AddressFamily.ToString = "InterNetworkV6" Then
                    'cardinfomation(1) = unicastip.Address.ToString 'only one ip get
                    'Exit For
                    'End If
                    If Trim(unicastip.Address.ToString) <> "" And unicastip.Address.AddressFamily.ToString = "InterNetwork" Then 'ipv4
                        cardinfomation(1) = unicastip.Address.ToString 'only one ip ge
                        Exit For
                    End If
                Next unicastip



                Dim gatewaycollection As GatewayIPAddressInformationCollection = properties.GatewayAddresses
                Dim gatewayip As GatewayIPAddressInformation
                For Each gatewayip In gatewaycollection
                    'displaylog("  gateway address.................:" & gatewayip.Address.ToString, "g")
                    ' If isIPV4 And gatewayip.Address.AddressFamily.ToString = "InterNetwork" Then 'InterNetworkV6 means ipV6 address
                    'cardinfomation(2) = gatewayip.Address.ToString 'only one ip ge
                    'Exit For
                    'ElseIf isIPV4 = False And gatewayip.Address.AddressFamily.ToString = "InterNetworkV6" Then
                    'cardinfomation(2) = gatewayip.Address.ToString 'only one ip get
                    'Exit For
                    'End If
                    If Trim(gatewayip.Address.ToString) <> "" Then
                        cardinfomation(2) = gatewayip.Address.ToString 'only one ip ge
                        Exit For
                    End If

                Next gatewayip
                Dim newcardinfomation(4) As String
                newcardinfomation(0) = cardinfomation(0)
                newcardinfomation(1) = cardinfomation(1)
                newcardinfomation(2) = cardinfomation(2)
                newcardinfomation(3) = cardinfomation(3)
                outputinfo.Add(newcardinfomation)
                Console.WriteLine(cardinfomation(0))
                Console.WriteLine(cardinfomation(1))
                Console.WriteLine(cardinfomation(2))
                Console.WriteLine(cardinfomation(3))
                Console.WriteLine()

            End If



            '---------------------------------------------------------------------------------------------------------------------------------------
        Next adapter

        Return 0
    End Function
    Sub serialsenddata(ByVal command As String, ByVal SerialPort1 As SerialPort, ByVal linemode As Boolean)
        Dim times As Integer = 0
        While serialSendDatabasic(command, SerialPort1, linemode) <> "OK" And times < 10
            times = times + 1
        End While
        If times = 10 Then Console.WriteLine("Write AT command:" + command + " failed " + times.ToString + " times.")
    End Sub
    Private Function serialSendDatabasic(ByVal command As String, ByVal SerialPort1 As SerialPort, ByVal linemode As Boolean) As String
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

    Public Sub Read(ByVal port As SerialPort)
        'While port.ReadByte > 3 '_continue
        Try
            'Threading.Thread.Sleep(100)
            Dim message As String = port.ReadExisting()
            If Trim(message) <> "" Then
                ' Console.WriteLine(message)
                messagepool.Add(message)
            End If

        Catch ex As Exception
            Dim a As Integer = 1
        End Try
        'End While
    End Sub
    Private Function serialportreadimis(ByVal portname) As String
        'Dim name As String
        Dim opentime As Integer = 0
        Dim sComparer As StringComparer = StringComparer.OrdinalIgnoreCase
        'Dim _continue As Boolean
        Dim _serialPort As SerialPort
        ' Dim readThread As New System.Threading.Thread(AddressOf Read)
        'readThread.Name = "readimis"
        serialportreadimis = ""
        Console.WriteLine("serial port name:" & portname)
        ' Create a new SerialPort object with default settings.
        _serialPort = New SerialPort()
        _serialPort.PortName = portname
        Console.WriteLine(portname)
        _serialPort.BaudRate = 115200
        _serialPort.StopBits = IO.Ports.StopBits.One
        _serialPort.Parity = IO.Ports.Parity.None
        _serialPort.DataBits = 8
        ' Set the read/write timeouts
        _serialPort.ReadTimeout = 5000
        _serialPort.WriteTimeout = 500
        _serialPort.DtrEnable = True
        _serialPort.RtsEnable = True

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
        Loop Until _serialPort.IsOpen = True Or opentime > 4

        'If _serialPort.IsOpen = True Then
        '    'serialSendData("AT" + vbCr, _serialPort)
        '    Threading.Thread.Sleep(300)
        '    serialSendData("AT+CIMI" + vbCr, _serialPort, False)
        '    While _continue And repeatetime < 5
        '        Dim message As String = _serialPort.ReadExisting
        '        If message.IndexOf("460") >= 0 Then
        '            serialportreadimis = Mid(message, message.IndexOf("460") + 1, 15)
        '            _continue = False
        '            Console.WriteLine(serialportreadimis)
        '            Return serialportreadimis
        '        End If
        '        serialsenddata("AT+CIMI" + vbCr, _serialPort, False)
        '        repeatetime = repeatetime + 1
        '    End While
        '    serialportreadimis = ""
        '    _serialPort.Close()
        '    _continue = True
        'Else

        '    serialportreadimis = ""

        'End If
        messagepool.Clear()
        If _serialPort.IsOpen = True Then
            serialsenddata("AT+CIMI" + vbCr, _serialPort, False)
            _continue = True

            repeatetime = 0
            While _continue And repeatetime < 5
                Threading.Thread.Sleep(100)
                Read(_serialPort)
                While messagepool.Count <> 0
                    If messagepool(0).IndexOf("460") >= 0 Then
                        Console.WriteLine(messagepool(0))
                        serialportreadimis = Mid(messagepool(0), messagepool(0).IndexOf("460") + 1, 15)
                        _continue = False
                        Console.WriteLine(serialportreadimis)
                        Try
                            'readThread.Abort()
                            Threading.Thread.Sleep(100)
                            '_serialPort.Close()
                        Catch
                            Dim a = 1
                        End Try
                        Return serialportreadimis
                    End If
                    messagepool.RemoveAt(0)
                End While
                serialsenddata("AT+CIMI" + vbCr, _serialPort, False)
                repeatetime = repeatetime + 1
            End While
            Try
                ' readThread.Abort()
                '_serialPort.Close()
            Catch

            End Try
            serialportreadimis = ""
            _serialPort.Close()
            
        Else

            serialportreadimis = ""

        End If


    End Function

    Private Function getallqualcommactiveports(ByVal outputinfo As Collection)
        Dim cardinfomation(6) As String '0.cardname,1.ip,2.gateway,3.carddiscription,4.imsi,5.usbcode,6 regpath
        Dim adapters As NetworkInterface() = NetworkInterface.GetAllNetworkInterfaces()
        Dim adapter As NetworkInterface
        Dim regpath As String = ""
        For Each adapter In adapters
            Dim properties As IPInterfaceProperties = adapter.GetIPProperties()
            If (adapter.Description.IndexOf("HS-USB") >= 0 Or adapter.Description.IndexOf("Sierra Wireless") >= 0) Or (adapter.Description.IndexOf("SAMSUNG Mobile USB Remote NDIS Network Device") >= 0) Or adapter.Description.IndexOf("HTC Remote NDIS based Device") >= 0 or adapter.Description.IndexOf("Quectel Wireless Ethernet Adapter") >= 0 Then

                regsearch(0) = Nothing
                regsearch(1) = Nothing
                regsearch(2) = Nothing
                regsearch(3) = Nothing
                cardinfomation(0) = Nothing
                cardinfomation(1) = Nothing
                cardinfomation(2) = Nothing
                cardinfomation(3) = Nothing
                cardinfomation(4) = Nothing
                cardinfomation(5) = Nothing
                cardinfomation(6) = Nothing
                cardinfomation(0) = adapter.Name.ToLower.ToString
                SearchSubKeys(My.Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\ControlSet001\Enum\USB"), adapter.Description)
                If regsearch(3) <> Nothing Then

                    cardinfomation(5) = Split(Mid(regsearch(1), regsearch(1).LastIndexOf("\") + 2, regsearch(1).Length - regsearch(1).LastIndexOf("\")), "&")(0) + "&" + Split(Mid(regsearch(1), regsearch(1).LastIndexOf("\") + 2, regsearch(1).Length - regsearch(1).LastIndexOf("\")), "&")(1) + "&" + Split(Mid(regsearch(1), regsearch(1).LastIndexOf("\") + 2, regsearch(1).Length - regsearch(1).LastIndexOf("\")), "&")(2)
                    cardinfomation(6) = Mid(regsearch(1), 1, regsearch(1).LastIndexOf("\"))
                Else
                    SearchSubKeys(My.Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\ControlSet001\Control\Network"), adapter.Name)
                    If regsearch(3) <> Nothing Then
                        Dim tempstr As String
                        tempstr = My.Computer.Registry.GetValue(regsearch(1), "PnpInstanceID", String.Empty)
                        cardinfomation(5) = Mid(Mid(tempstr, tempstr.LastIndexOf("\") + 2), 1, Mid(tempstr, tempstr.LastIndexOf("\") + 1).Length - 6)
                        cardinfomation(6) = "HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Enum\" + Mid(tempstr, 1, tempstr.LastIndexOf("\"))
                    End If

                End If

                cardinfomation(3) = adapter.Description.ToString
                Dim unicastipaddressinformationcollection As UnicastIPAddressInformationCollection = properties.UnicastAddresses
                Dim unicastip As UnicastIPAddressInformation
                For Each unicastip In unicastipaddressinformationcollection
                    'displaylog("  ip address............:{0}" & unicastip.Address.ToString, "g")
                    'displaylog("  " & unicastip.Address.AddressFamily.ToString, "g")
                    ' If isIPV4 And unicastip.Address.AddressFamily.ToString = "InterNetwork" Then 'InterNetworkV6 means ipV6 address
                    'cardinfomation(1) = unicastip.Address.ToString 'only one ip ge
                    'Exit For
                    'ElseIf isIPV4 = False And unicastip.Address.AddressFamily.ToString = "InterNetworkV6" Then
                    'cardinfomation(1) = unicastip.Address.ToString 'only one ip get
                    'Exit For
                    'End If
                    If Trim(unicastip.Address.ToString) <> "" And unicastip.Address.AddressFamily.ToString = "InterNetwork" Then 'ipv4
                        cardinfomation(1) = unicastip.Address.ToString 'only one ip ge
                        Exit For
                    End If
                Next unicastip



                Dim newcardinfomation(6) As String
                newcardinfomation(0) = cardinfomation(0)
                newcardinfomation(1) = cardinfomation(1)
                newcardinfomation(2) = cardinfomation(2)
                newcardinfomation(3) = cardinfomation(3)
                newcardinfomation(4) = cardinfomation(4)
                newcardinfomation(5) = cardinfomation(5)
                newcardinfomation(6) = cardinfomation(6)
                outputinfo.Add(newcardinfomation)
                Console.WriteLine(cardinfomation(0))
                Console.WriteLine(cardinfomation(1))
                Console.WriteLine(cardinfomation(2))
                Console.WriteLine(cardinfomation(3))
                Console.WriteLine(cardinfomation(4))
                Console.WriteLine(cardinfomation(5))
                Console.WriteLine()

            End If



            '---------------------------------------------------------------------------------------------------------------------------------------
        Next adapter

        Return 0
    End Function
    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        Dim cardinformation(6) As String '0.cardname,1.ip,2.gateway,3,carddiscription,4.imsicode
        Dim cardinformations As New Collection
        Dim imislists As New Collection
        Dim HISICOMlists As New Collection
        Dim hisicom(3) As String '1.comname,2.imsicode
        Dim tempregpath As String = ""
        Console.WriteLine("****list all net port")
        getallqualcommactiveports(cardinformations)
        ' getimislist(imislists, cardinformations)


        Console.WriteLine("****list all com ports at and modem")
        getqualcommlist(HISICOMlists)



        Dim found As Boolean = False

        For Each hisicom In HISICOMlists
            found = False
            For Each cardinformation In cardinformations
                If Not (cardinformation(5) Is Nothing) Then

                    If hisicom(2).ToLower = cardinformation(5).ToLower Then
                        If cardinformation(0).ToLower <> hisicom(1).ToLower Then 'already rightly binded
                            Console.WriteLine("Try change name from " & cardinformation(0) & " " & hisicom(1))
                            changeconnectionname(cardinformation(0), hisicom(1))
                            found = True
                            Exit For
                        Else
                            found = True  'already rightly binded
                        End If

                    End If





                End If

            Next
            Dim cardname As String = ""
            If found = False Then
                For i = 1 To cardinformations.Count
                    If Not (cardinformations(i)(6) Is Nothing) Then
                        tempregpath = cardinformations(i)(6)
                        Exit For
                    End If
                Next
                If tempregpath <> "" Then
                    cardname = getmissedcardinformation(hisicom, tempregpath)
                    If cardname <> "" Then
                        For i = 1 To cardinformations.Count
                            If cardinformations(i)(3) = cardname Then
                                changeconnectionname(cardinformations(i)(0), hisicom(1))
                                Exit For
                            End If

                        Next

                    End If

                End If

            End If


        Next
        Console.WriteLine("binding success")
        Console.WriteLine("*************************************")

        getallinformation(False)
    End Sub

    Function getmissedcardinformation(ByVal hisicom As Object, ByVal tempregpath As String) As String
        tempregpath = Mid(tempregpath, tempregpath.IndexOf("\") + 2, tempregpath.Length)
        regsearch(0) = Nothing
        regsearch(1) = Nothing
        regsearch(2) = Nothing
        regsearch(3) = Nothing
        SearchSubKeys(My.Computer.Registry.LocalMachine.OpenSubKey(tempregpath + "\" + hisicom(2) + "&00" + Split(tempregpath, "_")(UBound(Split(tempregpath, "_")))), "DeviceDesc")
        If Not (regsearch(3) Is Nothing) Then
            Return Mid(regsearch(3), regsearch(3).IndexOf(";") + 2, regsearch(3).Length)
        Else
            Return ""
        End If

    End Function



    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        SearchSubKeys(My.Computer.Registry.LocalMachine.OpenSubKey("SYSTEM\ControlSet001\Enum\USB"), "Qualcomm HS-USB WWAN Adapter 9025 #3")
    End Sub





    Dim totalcount, count As Integer
    Dim regsearch(5) As String

    Sub SearchSubKeys(ByVal root As RegistryKey, ByVal searchKey As String)
        Dim matchtype As String = Nothing

        ' Dim itm As ListViewItem

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





    Private Sub Form1_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Dim cominfo As String = ""
        Try
            getallinformation(True)
            ConsoleHelper.openconsole(Me.Handle, Me.Width, Me.Height)
            Console.SetWindowSize(50, 20)
            If checkauto() = True Then
                For i = 1 To ListBox1.Items.Count

                    cominfo = cominfo + Getcomport(ListBox1.Items(i - 1).ToString) + "-" + Getuetype(ListBox1.Items(i - 1).ToString) + "*"


                Next
                If cominfo <> "" Then cominfo = Mid(cominfo, 1, cominfo.Length - 1)
                cominfo = "$Check|" + GetLocalIP() + "|" + Now.ToString("MM/dd/yyyy   HH:mm:ss") + "|" + cominfo + vbCrLf
                Console.WriteLine(cominfo)

                While TCPwrite(cominfo) = False
                    wait(1000)

                End While


                Button6_Click(Nothing, Nothing)
                Button5_Click(Nothing, Nothing)
                Application.Exit()

            End If


        Catch ex As Exception
            Application.Exit()
        End Try

    End Sub
    Public Sub wait(ByRef ms As Short)

        Dim Start As Integer
        Start = VB.Timer()
        Do While VB.Timer() < Start + ms / 1000 '   3   秒的延时
            System.Windows.Forms.Application.DoEvents() '转让控制权
        Loop

    End Sub
    Sub tcpsend(ByVal info As String)

    End Sub

    Function checkauto() As Boolean
        Dim myArg() As String, iCount As Integer
        Dim isauto As Boolean
        myArg = System.Environment.GetCommandLineArgs

        If UBound(myArg) < 1 Then
            Return False
        End If
        For iCount = 1 To UBound(myArg)

            'TextBox1.Text = TextBox1.Text & "|" & myArg(iCount).ToString
            Select Case myArg(iCount).ToString
                Case "-a"
                    isauto = True
                Case "-i"
                    ip = myArg(iCount + 1)

            End Select

        Next
        If isauto Then Return True
    End Function

    Private Sub Button7_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        Dim saveitem As Object
        Dim i As Integer
        Dim configfile As String
        If SaveFileDialog1.ShowDialog <> Windows.Forms.DialogResult.Cancel Then
            configfile = SaveFileDialog1.FileName
            FileSystem.Kill(configfile)
            'configfile = My.Application.Info.DirectoryPath & "\ueconfig.ini"
            'UElogdir = Module1.ReadKeyVal(My.Application.Info.DirectoryPath & "\ueconfig.ini", "dirs", "UElog")
            'UEnumbers = listview1.Items.Count
            'sections = Module1.TotalSections(configfile)
            Try
                For i = 1 To ListBox1.Items.Count
                    saveitem = Module1.WriteKeyVal(configfile, Str(i), "ip", "")
                    saveitem = Module1.WriteKeyVal(configfile, Str(i), "cip", GetLocalIP())
                    saveitem = Module1.WriteKeyVal(configfile, Str(i), "serverip", "")
                    saveitem = Module1.WriteKeyVal(configfile, Str(i), "com", Getcomport(ListBox1.Items(i - 1).ToString))
                    saveitem = Module1.WriteKeyVal(configfile, Str(i), "type", Getuetype(ListBox1.Items(i - 1).ToString))
                    saveitem = Module1.WriteKeyVal(configfile, Str(i), "action", "")
                    saveitem = Module1.WriteKeyVal(configfile, Str(i), "dinterval", "")
                    saveitem = Module1.WriteKeyVal(configfile, Str(i), "loopinterval", "")
                    saveitem = Module1.WriteKeyVal(configfile, Str(i), "traffic", "")
                    saveitem = Module1.WriteKeyVal(configfile, Str(i), "ftpsession", "")
                    saveitem = Module1.WriteKeyVal(configfile, Str(i), "logip", "")
                    saveitem = Module1.WriteKeyVal(configfile, Str(i), "serverip2", "")
                    saveitem = Module1.WriteKeyVal(configfile, Str(i), "serverip3", "")
                    saveitem = Module1.WriteKeyVal(configfile, Str(i), "serverip4", "")

                Next

                Dim iresult = MsgBox("Export success !", MsgBoxStyle.OkOnly)
            Catch ex As Exception

                Dim iresult = MsgBox("Export failed !", MsgBoxStyle.OkOnly)

            End Try

        End If
    End Sub

    Function Getuetype(ByVal i) As String
        Dim returnstr As String
        'serialportreadimis(Getcomport(i))
        Dim uetypeinfo As String
        Dim imsi As String = ""
        uetypeinfo = atuetype(Getcomport(i))
        ' returnstr = "Qualcomm9600"
        'If i.ToString.IndexOf("9028") >= 0 Then returnstr = "Qualcomm9028"
        'If i.ToString.IndexOf("Qualcomm") >= 0 Then returnstr = "Qualcomm9600"
        'If i.ToString.IndexOf("HUAWEI") >= 0 Then returnstr = "HE5776"
        'If i.ToString.IndexOf("9028") >= 0 Then returnstr = "Qualcomm9028"
        If uetypeinfo = "4108" And i.ToString.IndexOf("9025") >= 0 Then uetypeinfo = "YY9027" Else If uetypeinfo = "4108" Then uetypeinfo = "Qualcomm9206"
        If i.ToString.IndexOf("908B") >= 0 Then uetypeinfo = "YY9206"
        returnstr = uetypeinfo
        imsi = serialportreadimis(Getcomport(i))
        Return returnstr + "#" + imsi
    End Function

    Private Function atuetype(ByVal portname) As String
        'Dim name As String

        Dim sComparer As StringComparer = StringComparer.OrdinalIgnoreCase
        Dim _continue As Boolean
        Dim _serialPort As SerialPort
        Dim opentime As Integer = 0
        ' Dim readThread As New System.Threading.Thread(AddressOf Read)
        ' readThread.Name = "atuetype"
        atuetype = "Qualcomm9600"
        Console.WriteLine("serial port name:" & portname)
        ' Create a new SerialPort object with default settings.
        _serialPort = New SerialPort()

        ' Allow the user to set the appropriate properties.
        '_serialPort.PortName = portname
        '_serialPort.BaudRate = 9600
        '_serialPort.Parity = Parity.None
        '_serialPort.DataBits = 8
        '_serialPort.StopBits = StopBits.One
        '_serialPort.Handshake = Handshake.None

        _serialPort.PortName = portname
        Console.WriteLine(portname)
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
        Loop Until _serialPort.IsOpen = True Or opentime > 4
        If _serialPort.IsOpen = True Then
            ' Threading.Thread.Sleep(300)
            messagepool.Clear()
            serialsenddata("AT+GMM" + vbCr, _serialPort, True)

            Dim h As Integer = 0
            While _continue And h < 5
                Threading.Thread.Sleep(100)
                Read(_serialPort)
                While messagepool.Count <> 0
                    Dim message As String = messagepool(0)
                    If Trim(message) <> "" Then
                        If message.IndexOf("SM-G9350") >= 0 Then
                            atuetype = "SMG9350"
                            _continue = False
                            Console.WriteLine(atuetype)
                        End If
                        If message.IndexOf("MC7455") >= 0 Then
                            atuetype = "MC7455"
                            _continue = False
                            Console.WriteLine(atuetype)
                        End If
                        If message.IndexOf("EM7565") >= 0 Then
                            atuetype = "EM7565"
                            _continue = False
                            Console.WriteLine(atuetype)
                        End If
                        If message.IndexOf("SIM7200") >= 0 Then
                            atuetype = "SIM7200"
                            _continue = False
                            Console.WriteLine(atuetype)
                        End If
                        If message.IndexOf("BG96") >= 0 Then
                            atuetype = "BG96"
                            _continue = False
                            Console.WriteLine(atuetype)
                        End If
                        If message.IndexOf("SIMCOM_SIM7000") >= 0 Then
                            atuetype = "SIM7000"
                            _continue = False
                            Console.WriteLine(atuetype)
                        End If
                        If message.IndexOf("4108") >= 0 Then
                            atuetype = "4108"
                            _continue = False
                            Console.WriteLine(atuetype)
                        End If
                        If message.IndexOf("4112") >= 0 Then
                            atuetype = "Qualcomm8996"
                            _continue = False
                            Console.WriteLine(atuetype)
                        End If
                        If message.IndexOf("4115") >= 0 Then
                            atuetype = "Qualcomm8998"
                            _continue = False
                            Console.WriteLine(atuetype)
                        End If
                        If message.IndexOf("E5375") >= 0 Then
                            atuetype = "E5375"
                            _continue = False
                            Console.WriteLine(atuetype)
                        End If
                        If message.IndexOf("E5776") >= 0 Then
                            atuetype = "E5776"
                            _continue = False
                            Console.WriteLine(atuetype)
                        End If
                        If _continue = False Then Exit While
                    Else


                    End If
                    messagepool.RemoveAt(0)
                End While
                h = h + 1
                If h = 4 Then
                    atuetype = "Qualcomm9600"
                    _continue = False
                End If
                serialsenddata("AT+GMM" + vbCr, _serialPort, True)

            End While

            Try
                'readThread.Abort()
                _serialPort.Close()
            Catch

            End Try


            '_continue = True
        Else
            Console.WriteLine("Open " + portname + " fail " + opentime.ToString)
            _serialPort.Close()
            '_continue = True
            Return ""
        End If


    End Function



    Function Getcomport(ByVal i) As String
        Dim tempstr = Split(i, "(")(1)
        tempstr = Split(tempstr, ")")(0)
        Return tempstr
    End Function

    Protected Function GetLocalIP() As String
        'Dim addr As System.Net.IPAddress
        Dim subnet As String()
        Dim subnetstring1, subnetstring2 As String
        subnet = getsubnettoserver(ip)

        subnetstring1 = subnet(0).ToString + "." + subnet(1).ToString + "." + subnet(2).ToString + "."
        subnetstring2 = subnet(0).ToString + "." + subnet(1).ToString + "."

        If subnetstring1 = "127.0.0." Then Return "127.0.0.1"

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
        dosstr = Split(rundoscommand("tracert " + ip), vbCrLf)
        For i = 0 To dosstr.Count - 1
            If Trim(dosstr(i)) <> "" Then
                If Trim(dosstr(i)).Chars(0) = "1" Then
                    tempstr = Split(Trim(dosstr(i)), " ")(Split(Trim(dosstr(i)), " ").Length - 1)
                    tempstr = Replace(Replace(tempstr, "[", ""), "]", "")
                    If checkipaddress(tempstr) = True Then
                        output = Split(tempstr, ".")
                        Return output
                    End If
                    found = True
                    Exit For
                End If
            End If
        Next
        If found = False Then
            Return Split("192.168.10.1 ", ".")
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

        Console.WriteLine(s)

        sIn.Close()
        sOut.Close()
        sErr.Close()
        myProcess.Close()
        Return s
    End Function


    Function TCPwrite(ByVal message As String) As Boolean
        ' Dim receivestr As String
        Dim a As Integer = 0
        'Console.WriteLine(message)
        Try
            Dim client As New System.Net.Sockets.TcpClient()
            Dim result = client.BeginConnect(ip, 2501, Nothing, Nothing)
            Dim success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2))
            If Not success Then
                Throw New Exception("Failed to connect.")
            End If
            ' Translate the passed message into ASCII and store it as a Byte array.
            Dim data As [Byte]() = System.Text.Encoding.ASCII.GetBytes(message)

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
            Console.WriteLine(Trim(responseData))
            stream.Close()
            client.Close()
            Return True







            'If Mytcpclient Is Nothing Then
            '    TCPconnect()
            'End If

            'If syncflag = False Or Mytcpclient.Connected = False Then
            '    Mytcpclient.Close()
            '    Mytcpclient.Connect(100)
            'End If
            'If Mytcpclient.Connected = True Then
            '    While messagebuffer.Count > 0
            '        Dim retData1() As Byte = Mytcpclient.SyncSend(2, Encoding.ASCII.GetBytes(messagebuffer.Item(0)), 100)
            '        receivestr = Encoding.ASCII.GetString(retData1)
            '        If Trim(receivestr).IndexOf("$") >= 0 Then
            '            syncflag = True


            '            a = Console.CursorLeft
            '            Console.Write("!") '("receive things:" + Trim(Encoding.ASCII.GetString(value)))
            '            Console.CursorLeft = a
            '            messagebuffer.RemoveAt(0)
            '        Else
            '            syncflag = False

            '            Exit Function
            '        End If

            '    End While
            '    Dim retData() As Byte = Mytcpclient.SyncSend(2, Encoding.ASCII.GetBytes(message), 100)
            '    receivestr = Encoding.ASCII.GetString(retData)
            '    If Trim(receivestr).IndexOf("$") >= 0 Then
            '        syncflag = True

            '        a = Console.CursorLeft
            '        Console.Write("!") '("receive things:" + Trim(Encoding.ASCII.GetString(value)))
            '        Console.CursorLeft = a

            '    Else
            '        syncflag = False
            '    End If

            '    Return True
            'End If
        Catch ex As Exception
            a = Console.CursorLeft
            Console.Write("?")
            Console.CursorLeft = a
            'While messagebuffer.Count > 1000
            '    messagebuffer.RemoveAt(0)
            'End While
            'messagebuffer.Add(message)
            Return False

        End Try


    End Function
    Sub TCPconnect()
        Try
            Mytcpclient = New SingleConnection(ip, port)
            AddHandler Mytcpclient.ReceiveEventHandler, AddressOf ReceiveEventHandler1
            AddHandler Mytcpclient.ErrorEventHandler, AddressOf ErrorEventHandler1
            AddHandler Mytcpclient.RemoteDisconnected, AddressOf RemoteDisconnected1
            Mytcpclient.Connect(100)
        Catch ex As Exception
            Dim a As Integer = 1



        End Try

    End Sub
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



    Public Sub New()

    End Sub
End Class