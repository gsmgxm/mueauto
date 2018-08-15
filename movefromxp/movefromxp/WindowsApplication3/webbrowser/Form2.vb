Public Class Form2
    Dim webbrowser1 As System.Windows.Forms.WebBrowser
    Dim httpip As String = ""
    Dim httpips() As String
    Dim times As Integer = 0
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        If Not (webbrowser1 Is Nothing) Then
            Me.Controls.Remove(webbrowser1)
            webbrowser1.Dispose()

            Createdwebbrowser(New Uri(httpip))
            Me.Refresh()
        End If
    End Sub

    Private Sub Form2_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim myarg() As String
        myArg = System.Environment.GetCommandLineArgs
        If UBound(myarg) >= 1 Then

            httpip = "http://" + myarg(1).ToString
        End If
        If UBound(myarg) >= 2 Then

            Timer1.Interval = Int(myarg(2).ToString) * 1000
        End If
        If UBound(myarg) >= 3 Then

            Me.Text = "Http " + myarg(3).ToString
        End If
        Dim tempstring As String = Module1.ReadKeyVal("D:\mueauto\ftp.ini", "http", "addresslist")

        If tempstring <> "" Then
            httpips = Split(tempstring, "|")
            For i = 0 To httpips.Count - 1
                httpips(i) = httpip + httpips(i)
            Next
        End If

        If tempstring = "" Then
            Createdwebbrowser(New Uri(httpip))
        Else
            Createdwebbrowser(New Uri(httpips(times)))
            ' times = times + 1
        End If


    End Sub

    Sub Createdwebbrowser(ByVal url As Uri)

        webbrowser1 = New System.Windows.Forms.WebBrowser
        Me.Controls.Add(webbrowser1)
        Me.webbrowser1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.webbrowser1.Location = New System.Drawing.Point(0, 0)
        Me.webbrowser1.MinimumSize = New System.Drawing.Size(20, 18)
        Me.webbrowser1.Name = "WebBrowser1"
        Me.webbrowser1.Size = New System.Drawing.Size(292, 246)
        Me.webbrowser1.TabIndex = 0

        If httpips Is Nothing Then
            webbrowser1.Url = url
        Else

            If times >= UBound(httpips) + 1 Then times = 0
            webbrowser1.Url = New Uri(httpips(times))
            times = times + 1
        End If

        webbrowser1.Visible = True

    End Sub
End Class