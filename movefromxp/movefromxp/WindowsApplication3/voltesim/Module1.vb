Imports System.Net
Imports System.IO
Imports System.Threading
Imports System.Management
Imports System.Text.RegularExpressions


Module Module1
    Dim voiceserverip As String
    Dim ftpuserid As String
    Dim ftppassword As String
    Dim alldelaysa As New List(Of Single)
    Dim allsenta As New List(Of Integer)
    Dim alllosta As New List(Of Integer)
    Dim alldelaysb As New List(Of Single)
    Dim allsentb As New List(Of Integer)
    Dim alllostb As New List(Of Integer)
    Sub Main()

        Dim myArg() As String
        Dim ftpmode As String = "DL"
        'Dim sessionno As Integer
        Dim filename As String = ""
        Dim ulfilename As String = ""
        Dim ulremotename As String = "1"
        Dim com As String = ""

        Try
            myArg = System.Environment.GetCommandLineArgs
            If UBound(myArg) >= 2 Then

                voiceserverip = myArg(1).ToString

                com = myArg(2).ToString
                Console.Title = "Volte sim:" + com
                Console.WriteLine("serverip:" + voiceserverip)
                senttowlenping(voiceserverip)
            Else
                Console.WriteLine("No enough input parameter")
                Console.WriteLine("Voltesim.exe IP comportname ")
                Console.WriteLine("Press key return to exit ")
                Console.ReadLine()
            End If

        Catch ex As Exception
        End Try





    End Sub

    Sub senttowlenping(ByVal serverip As String)
        Dim returnstring As String
        While 1

            Try
                'sentvoice 5s

                returnstring = rundoscommand("hrping " + serverip + " -L 500 -s 20 -n 250")
                If returnstring.IndexOf("RTTs") >= 0 Then
                    While allsenta.Sum > 1000
                        allsenta.RemoveAt(0)
                        alllosta.RemoveAt(0)

                    End While
                    While alldelaysa.Count > 1000
                        alldelaysa.RemoveAt(0)
                    End While
                    getresult(returnstring, "a")
                    Console.SetCursorPosition(0, 2)
                    Console.WriteLine("")
                    Console.SetCursorPosition(0, 0)
                    Console.WriteLine("Talk sent=" + allsenta.Sum.ToString + ", lost=" + alllosta.Sum.ToString + "(" + (Int(10000 * alllosta.Sum / allsenta.Sum) / 100).ToString + "%), RTTs in min/avg/max(ms):" + alldelaysa.Min.ToString + "/" + alldelaysa.Average.ToString + "/" + alldelaysa.Max.ToString)
                Else
                    Console.SetCursorPosition(0, 2)
                    Console.WriteLine("Can not access server:" + serverip)
                End If

                'send mute 5s

                returnstring = rundoscommand("hrping " + serverip + " -L 92 -s 160 -n 32")
                If returnstring.IndexOf("RTTs") >= 0 Then
                    While allsentb.Sum > 1000
                        allsentb.RemoveAt(0)
                        alllostb.RemoveAt(0)
                    End While
                    While alldelaysb.Count > 1000
                        alldelaysb.RemoveAt(0)
                    End While
                    getresult(returnstring, "b")
                    Console.SetCursorPosition(0, 2)
                    Console.WriteLine("")
                    Console.SetCursorPosition(0, 1)
                    Console.WriteLine("Mute sent=" + allsentb.Sum.ToString + ", lost=" + alllostb.Sum.ToString + "(" + (Int(10000 * alllostb.Sum / allsentb.Sum) / 100).ToString + "%), RTTs in min/avg/max(ms):" + alldelaysb.Min.ToString + "/" + alldelaysb.Average.ToString + "/" + alldelaysb.Max.ToString)
                Else
                    Console.SetCursorPosition(0, 2)
                    Console.WriteLine("Can not access server:" + serverip)
                End If
            Catch ex As Exception

            End Try



        End While




    End Sub



    Function getresult(ByVal inputstring As String, ByVal type As String) As Boolean

        Dim tempstr As String = ""
        Dim values As String
        Try
            If type = "a" Then
                values = ""
                Dim expression As New Regex("time=[-+]?[0-9]*\.?[0-9]+ms")

                Dim mc As MatchCollection = expression.Matches(inputstring)
                If mc.Count >= 1 Then
                    For i As Integer = 0 To mc.Count - 1
                        tempstr = Replace(mc(i).ToString, "ms", "")
                        tempstr = Replace(tempstr, "time=", "")
                        alldelaysa.Add(tempstr)

                    Next
                End If
                Dim expression2 As New Regex("lost=[-+]?[0-9]*\.?[0-9]")

                Dim mc2 As MatchCollection = expression2.Matches(inputstring)
                If mc2.Count >= 1 Then
                    For i As Integer = 0 To mc2.Count - 1

                        tempstr = Replace(mc2(i).ToString, "lost=", "")
                        alllosta.Add(Int(tempstr))

                    Next
                End If
                Dim expression3 As New Regex("Packets: sent=[-+]?[0-9]*\.?[0-9]")

                Dim mc3 As MatchCollection = expression3.Matches(inputstring)
                If mc3.Count >= 1 Then
                    For i As Integer = 0 To mc3.Count - 1

                        tempstr = Replace(mc3(i).ToString, "Packets: sent=", "")
                        allsenta.Add(Int(tempstr))

                    Next
                End If
            End If

            If type = "b" Then
                values = ""
                Dim expression As New Regex("time=[-+]?[0-9]*\.?[0-9]+ms")

                Dim mc As MatchCollection = expression.Matches(inputstring)
                If mc.Count >= 1 Then
                    For i As Integer = 0 To mc.Count - 1
                        tempstr = Replace(mc(i).ToString, "ms", "")
                        tempstr = Replace(tempstr, "time=", "")
                        alldelaysb.Add(tempstr)

                    Next
                End If
                Dim expression2 As New Regex("lost=[-+]?[0-9]*\.?[0-9]")

                Dim mc2 As MatchCollection = expression2.Matches(inputstring)
                If mc2.Count >= 1 Then
                    For i As Integer = 0 To mc2.Count - 1

                        tempstr = Replace(mc2(i).ToString, "lost=", "")
                        alllostb.Add(Int(tempstr))

                    Next
                End If
                Dim expression3 As New Regex("Packets: sent=[-+]?[0-9]*\.?[0-9]")

                Dim mc3 As MatchCollection = expression3.Matches(inputstring)
                If mc3.Count >= 1 Then
                    For i As Integer = 0 To mc3.Count - 1

                        tempstr = Replace(mc3(i).ToString, "Packets: sent=", "")
                        allsentb.Add(Int(tempstr))

                    Next
                End If
            End If

        Catch ex As Exception
        End Try

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
            sIn.Close()
            sOut.Close()
            sErr.Close()
            myProcess.Close()

            ' Console.WriteLine(s)
            Return s
        Catch ex As Exception
            Return ""
        End Try

    End Function


End Module
