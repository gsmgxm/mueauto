Public Class Form1
    Dim httpip As String = ""
    Dim httpips() As String
    Dim times As Integer = 0
    Dim dtimesint As Integer = 0
    Dim avtime(4) As Integer
    Dim errortime(4) As Integer
    Dim datasize As Integer
    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim myarg() As String
        myarg = System.Environment.GetCommandLineArgs
        If UBound(myarg) >= 1 Then

            httpip = "http://" + myarg(1).ToString
        End If
        If UBound(myarg) >= 2 Then

            Timer1.Interval = Int(myarg(2).ToString) * 1000
            Timer1.Enabled = True
        End If
        If UBound(myarg) >= 3 Then

            Me.Text = "Http download " + myarg(3).ToString
        End If

        Dim tempstring As String = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "httpdownload", "addresslist")

        If tempstring <> "" Then
            httpips = Split(tempstring, "|")
            For i = 0 To httpips.Count - 1
                httpips(i) = httpip + httpips(i)
            Next
        End If




    End Sub

    Function downloaddata(ByVal url As String) As Boolean

        Dim result As Byte()
        Dim newname As String = ""
        Try
            'documentURL = info
            'newname = username + DateTime.Now.ToString("yyyyMMddHHmmss") + ".csv"
            'dlFileName = Server.MapPath("uploadfiles") + "\" + newname
            Dim wc As New System.Net.WebClient

            result = wc.DownloadData(url)
            datasize = result.Length
            Return True
            'reportcontext = Encoding.UTF8.GetString(result)
            'wc.DownloadFile(documentURL, dlFileName)
        Catch ex As Exception
            Dim a = 1
            'MessageBox.Show("Error", ex.Message.ToString)
            Return False
        End Try


    End Function


    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        Dim time1, timer2 As Date
        Dim ts As TimeSpan
        Timer1.Enabled = False
        dtimesint = dtimesint + 1
        dtime.Text = "Download times:" + dtimesint.ToString
        time1 = Now
        If httpips Is Nothing Then
            If downloaddata(httpip) = True Then
                timer2 = Now
                ts = timer2 - time1
                D1.Text = "Download file 1 name:" + httpip + " Data size(Byte):" + datasize.ToString
                avtime(0) = avtime(0) + (ts.Milliseconds).ToString
                Dim tempint As String = Int(1000 * errortime(0) / dtimesint) / 100

                a1.Text = "Average download time(ms):" + (avtime(0) / (dtimesint - errortime(0))).ToString + " Fail rate:" + (Int(1000 * errortime(0) / dtimesint) / 100).ToString + "%"
            Else
                errortime(0) = errortime(0) + 1
                a1.Text = "Average download time(ms):" + (avtime(0) / (dtimesint - errortime(0))).ToString + " Fail rate:" + (Int(1000 * errortime(0) / dtimesint) / 100).ToString + "%"
            End If

        Else
            If times = UBound(httpips) + 1 Then times = 0

            If downloaddata(httpips(times)) = True Then
                timer2 = Now
                Select Case times
                    Case 0
                        ts = timer2 - time1
                        D1.Text = "Download file 1 name:" + httpips(times) + " Data size(Byte):" + datasize.ToString
                        avtime(0) = avtime(0) + (ts.Milliseconds).ToString
                        a1.Text = "Average download time(ms):" + (avtime(0) / ((Math.Ceiling(dtimesint / (UBound(httpips) + 1))) - errortime(0))).ToString + " Fail rate:" + (Int(10000 * errortime(0) / Math.Ceiling(dtimesint / (UBound(httpips) + 1))) / 100).ToString + "%"
                    Case 1

                        ts = timer2 - time1
                        D2.Text = "Download file 2 name:" + httpips(times) + " Data size(Byte):" + datasize.ToString
                        avtime(1) = avtime(1) + (ts.Milliseconds).ToString
                        a2.Text = "Average download time(ms):" + (avtime(1) / ((Math.Ceiling(dtimesint / (UBound(httpips) + 1))) - errortime(1))).ToString + " Fail rate:" + (Int(10000 * errortime(1) / Math.Ceiling(dtimesint / (UBound(httpips) + 1))) / 100).ToString + "%"
                    Case 2

                        ts = timer2 - time1
                        D3.Text = "Download file 3 name:" + httpips(times) + " Data size(Byte):" + datasize.ToString
                        avtime(2) = avtime(2) + (ts.Milliseconds).ToString
                        a3.Text = "Average download time(ms):" + (avtime(2) / ((Math.Ceiling(dtimesint / (UBound(httpips) + 1))) - errortime(2))).ToString + " Fail rate:" + (Int(10000 * errortime(2) / Math.Ceiling(dtimesint / (UBound(httpips) + 1))) / 100).ToString + "%"
                    Case 3

                        ts = timer2 - time1
                        d4.Text = "Download file 4 name:" + httpips(times) + " Data size(Byte):" + datasize.ToString
                        avtime(3) = avtime(3) + (ts.Milliseconds).ToString
                        a4.Text = "Average download time(ms):" + (avtime(3) / ((Math.Ceiling(dtimesint / (UBound(httpips) + 1))) - errortime(3))).ToString + " Fail rate:" + (Int(10000 * errortime(3) / Math.Ceiling(dtimesint / (UBound(httpips) + 1))) / 100).ToString + "%"
                End Select

            Else
                Select Case times
                    Case 0
                        D1.Text = "Download file 1 name:" + httpips(times) + " Data size(Byte):" + datasize.ToString
                        errortime(0) = errortime(0) + 1
                        a1.Text = "Average download time(ms):" + (avtime(0) / ((Math.Ceiling(dtimesint / (UBound(httpips) + 1))) - errortime(0))).ToString + " Fail rate:" + (Int(10000 * errortime(0) / Math.Ceiling(dtimesint / (UBound(httpips) + 1))) / 100).ToString + "%"

                    Case 1
                        D2.Text = "Download file 2 name:" + httpips(times) + " Data size(Byte):" + datasize.ToString
                        errortime(1) = errortime(1) + 1
                        a2.Text = "Average download time(ms):" + (avtime(1) / ((Math.Ceiling(dtimesint / (UBound(httpips) + 1))) - errortime(1))).ToString + " Fail rate:" + (Int(10000 * errortime(1) / Math.Ceiling(dtimesint / (UBound(httpips) + 1))) / 100).ToString + "%"

                    Case 2
                        D3.Text = "Download file 3 name:" + httpips(times) + " Data size(Byte):" + datasize.ToString
                        errortime(2) = errortime(2) + 1
                        a3.Text = "Average download time(ms):" + (avtime(2) / ((Math.Ceiling(dtimesint / (UBound(httpips) + 1))) - errortime(2))).ToString + " Fail rate:" + (Int(10000 * errortime(2) / Math.Ceiling(dtimesint / (UBound(httpips) + 1))) / 100).ToString + "%"

                    Case 3
                        d4.Text = "Download file 4 name:" + httpips(times) + " Data size(Byte):" + datasize.ToString
                        errortime(3) = errortime(3) + 1
                        a4.Text = "Average download time(ms):" + (avtime(3) / ((Math.Ceiling(dtimesint / (UBound(httpips) + 1))) - errortime(3))).ToString + " Fail rate:" + (Int(10000 * errortime(3) / Math.Ceiling(dtimesint / (UBound(httpips) + 1))) / 100).ToString + "%"

                End Select
            End If

            times = times + 1
            End If
            Timer1.Enabled = True
    End Sub


End Class
