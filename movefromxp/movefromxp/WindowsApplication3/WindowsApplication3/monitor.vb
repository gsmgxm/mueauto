Imports System.IO
Imports VB = Microsoft.VisualBasic
Imports System.Threading
Public Class monitorform

    Public customerfilterlist As New Dictionary(Of String, String)
    Public data(200, 8) As Object
    'data(x,1)=ip
    'data(x,2)=时间
    'data(x,3)=drop times
    'data(x,4)=drop rate
    'data(x,5)=attach times
    'data(x,6)=attach success rate
    'data(x,7)=ue state
    Dim TP(200, 3) As Object
    'TP(0,x)-->all throughput
    'TP(x,0)-->is filter group
    'TP(x,1)=时间
    'TP(x,2) = DLPT
    'TP(x,3)=ULPT
    Dim UElogdir As String
    Dim UEsize As Integer
    Dim FrmW As Single     '存放改变前的窗体的宽度
    Dim frmH As Single     '存放改变前的窗体的高度
    Dim currentAnnotation As String
    Dim p
    Public updateinterval As UInteger
    Public logon As Integer
    Public loglock As Boolean
    Dim measureon As Integer
    Dim filterallcounter As Integer
    Dim filtergroup() As String
    Dim graphfreeze As Boolean
    Dim plotnametoid As New Collection
    ' Dim tcb As New TimerCallback(AddressOf Me.Timersthread)
    ' Dim objTimer1 = New Timer(tcb, Nothing, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10))
    Public objTimer1 As Timer
    Dim netmeterlogformate As String
    Dim customerplotstart, customerplotend, customerfilterstart, customerfilterend As Integer
    Private Function writelog(ByVal logstr As String, ByVal Start As Boolean)
        Dim outputstr, fn As String
        loglock = True
        If logon = 1 Then
            My.Application.DoEvents()
            fn = My.Application.Info.DirectoryPath + "\rawdata.log"
            Dim fw As System.IO.StreamWriter = New System.IO.StreamWriter(fn, True, System.Text.Encoding.UTF8)

            If Start = True Then
                outputstr = Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + "start--------------------------------" + vbCrLf
                fw.Write(outputstr)
                outputstr = Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + "UE-ip time drop-times drop-rate attach-times attach-success TP-time DLPT ULPT" + vbCrLf
                fw.Write(outputstr)
            Else
                outputstr = Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + logstr + vbCrLf
                fw.Write(outputstr)
            End If


            fw.Close()
            fw.Dispose()
        End If
        loglock = False
        Return "OK"
    End Function
    Private Function writeOPlog(ByVal logstr As String, ByVal Start As Boolean)
        Dim outputstr, fn As String
        loglock = True
        If logon = 1 Then
            My.Application.DoEvents()
            fn = My.Application.Info.DirectoryPath + "\rawOP.log"
            Dim fw As System.IO.StreamWriter = New System.IO.StreamWriter(fn, True, System.Text.Encoding.UTF8)

            If Start = True Then
                outputstr = Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + "start--------------------------------" + vbCrLf
                fw.Write(outputstr)
                outputstr = Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + "OP" + vbCrLf
                fw.Write(outputstr)
            Else
                outputstr = logstr + vbCrLf
                fw.Write(outputstr)
            End If


            fw.Close()
            fw.Dispose()
        End If
        loglock = False
        Return "OK"
    End Function
    Private Sub _Toolbar1_Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _Toolbar1_Button9.Click
        Dim windowsize As Integer
        Dim temp1 As String
        temp1 = InputBox("Chart window size, total points showed in window." + vbCrLf + "interval=10s  windowsszie=360  1 hour" + vbCrLf + "interval=10s  windowsszie=8640  1 day" + vbCrLf + "interval=10s  windowsszie=25920  3 day", vbOKOnly)

        If Val(temp1) <> 0 Then

            windowsize = Val(temp1)
            cwGraph4.Plots(0).HistoryCapacity = windowsize
            cwGraph5.Plots(0).HistoryCapacity = windowsize
            For i = 1 To 200
                If data(i, 1) <> Nothing Then
                    If i < cwGraph2.Plots.Count Then cwGraph2.Plots(i).HistoryCapacity = windowsize
                    If i < cwGraph3.Plots.Count Then cwGraph3.Plots(i).HistoryCapacity = windowsize
                    cwGraph4.Plots(i).HistoryCapacity = windowsize
                    cwGraph5.Plots(i).HistoryCapacity = windowsize

                End If
            Next
            _Toolbar1_Button9.Text = "Size" + vbCrLf + temp1
        End If
    End Sub

    Private Sub _SSTab1_TabPage0_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub monitorform_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed

    End Sub

    Private Sub monitorform_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If Not (objTimer1 Is Nothing) Then objTimer1.Change(-1, 10)
        Timer2.Enabled = False
        'objTimer1.Dispose()
    End Sub

    Private Sub monitorform_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Leave

    End Sub

    Private Sub monitor_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        monitorwindowshow(My.Application.Info.DirectoryPath + "\ueconfig.ini")
    End Sub
    Public Sub monitorwindowshow(ByVal configfile As String)
        Dim plot As ScatterPlot
        Dim a As Object
        Me.MdiParent = MDIForm1
        todaydate = Today()
        updateinterval = Val(MDIForm1.TPinterval.Text)
        'Timer1.Interval = updateinterval * 1000
        Timer2.Interval = updateinterval * 1000
        'objTimer1 = New Timer(tcb, Nothing, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(updateinterval))
        Timer1.Enabled = False
        Timer2.Enabled = True
        ' _Toolbar1_Button8.Text = "interval" + vbCrLf + updateinterval.ToString
        logon = 1
        measureon = 0
        a = writelog("start--------------------------------", True)
        UEsize = 0
        Randomize()
        netmeterlogformate = ""


        cwGraph2.InteractionMode = GraphInteractionModes.DragAnnotationCaption
        cwGraph3.InteractionMode = GraphInteractionModes.DragAnnotationCaption
        cwGraph4.InteractionMode = GraphInteractionModes.DragAnnotationCaption
        cwGraph5.InteractionMode = GraphInteractionModes.DragAnnotationCaption


        '初始化参数
        ReDim data(200, 8)
        ReDim TP(200, 3)
        customerplotstart = 0
        customerplotend = 0
        customerfilterstart = 0
        customerfilterend = 0
        UElogdir = Module1.ReadKeyVal(configfile, "dirs", "UElog")

        '获取UE ip,初始化chart线条
        cwGraph2.Plots.Clear()
        cwGraph3.Plots.Clear()
        cwGraph4.Plots.Clear()
        cwGraph5.Plots.Clear()
        plotnametoid.Clear()

        data(0, 1) = "Total"
        plotnametoid.Add(0, data(0, 1))

        plot = New ScatterPlot
        plot.Tag = data(0, 1)
        plot.PointStyle = PointStyle.SolidSquare
        plot.PointSize = New System.Drawing.Size(2, 2)
        cwGraph2.UseColorGenerator = True
        cwGraph2.Plots.Add(plot)

        plot = New ScatterPlot
        plot.Tag = data(0, 1)
        plot.PointStyle = PointStyle.SolidSquare
        plot.PointSize = New System.Drawing.Size(2, 2)
        cwGraph3.UseColorGenerator = True
        cwGraph3.Plots.Add(plot)

        plot = New ScatterPlot
        plot.Tag = data(0, 1)
        plot.PointStyle = PointStyle.SolidSquare
        plot.PointSize = New System.Drawing.Size(2, 2)
        cwGraph4.UseColorGenerator = True
        cwGraph4.Plots.Add(plot)

        plot = New ScatterPlot
        plot.Tag = data(0, 1)
        plot.PointStyle = PointStyle.SolidSquare
        plot.PointSize = New System.Drawing.Size(2, 2)
        cwGraph5.UseColorGenerator = True
        cwGraph5.Plots.Add(plot)

        MDIForm1.ToolStripStatusLabel1.Text = "Initializing the Monitor page plots"
        MDIForm1.ToolStripProgressBar1.Maximum = 200
        MDIForm1.ToolStripProgressBar1.Value = 0
        For i = 1 To 200
            data(i, 1) = Module1.ReadKeyVal(configfile, Str(i), "ip")

            If data(i, 1) <> "" And Not (plotnametoid.Contains(data(i, 1))) Then 'ip address <>""表示UE已经被配置
                UEsize = UEsize + 1
                plotnametoid.Add(i, data(i, 1))
                plot = New ScatterPlot
                plot.Tag = data(i, 1)
                plot.PointStyle = PointStyle.SolidSquare
                plot.PointSize = New System.Drawing.Size(2, 2)
                cwGraph2.UseColorGenerator = True
                cwGraph2.Plots.Add(plot)

                plot = New ScatterPlot
                plot.Tag = data(i, 1)
                plot.PointStyle = PointStyle.SolidSquare
                plot.PointSize = New System.Drawing.Size(2, 2)
                cwGraph3.UseColorGenerator = True
                cwGraph3.Plots.Add(plot)

                plot = New ScatterPlot
                plot.Tag = data(i, 1)
                plot.PointStyle = PointStyle.SolidSquare
                plot.PointSize = New System.Drawing.Size(2, 2)
                cwGraph4.UseColorGenerator = True
                cwGraph4.Plots.Add(plot)

                plot = New ScatterPlot
                plot.Tag = data(i, 1)
                plot.PointStyle = PointStyle.SolidSquare
                plot.PointSize = New System.Drawing.Size(2, 2)
                cwGraph5.UseColorGenerator = True
                cwGraph5.Plots.Add(plot)

                customerplotstart = i + 1
            Else
                If data(i, 1) <> "" Then
                    MessageBox.Show("UE ip must be only " & data(i, 1))
                    Application.Exit()
                End If

            End If


            MDIForm1.ToolStripProgressBar1.Value = i
        Next

        MDIForm1.ToolStripStatusLabel1.Text = ""

        Form2.Show()
        Form4.Show()



        a = updatefilterlist()

    End Sub
    Private Function updatefilterlist()
        Dim plot1 As ScatterPlot
        filterallcounter = 0
        List1.Items.Clear()
        List1.Items.Add(("All"))
        For i = 1 To My.Forms.Form2.Combo2.Items.Count() 'add actions
            List1.Items.Add(My.Forms.Form2.Combo2.Items(i - 1))

        Next
        For i = 1 To My.Forms.Form2.Combo3.Items.Count  'add traffic type
            List1.Items.Add(My.Forms.Form2.Combo3.Items(i - 1))
        Next

        If System.IO.File.Exists(My.Application.Info.DirectoryPath + "\customerfilter.xml") Then
            Dim ds As New DataSet
            ds.ReadXml(My.Application.Info.DirectoryPath + "\customerfilter.xml")
            If ds.Tables.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 Then
                    filterallcounter = ds.Tables(0).Rows.Count  'filterallcounter 添加customer filter group的个数
                End If
            End If

        End If


        filterallcounter = filterallcounter + List1.Items.Count '获取业务类型生成group的个数，包含“All”

        customerfilterstart = List1.Items.Count
        customerfilterend = customerfilterstart
        '------------------------------------根据类型生成group
        ReDim filtergroup(filterallcounter) '去掉一个“All”
        For i = 1 To List1.Items.Count - 1  '最后一个留给后来手动选择生成的filter
            filtergroup(i) = List1.Items(i) & ":"
        Next

        For j = 0 To My.Forms.Form2.listview1.Items.Count - 1
            For i = 1 To List1.Items.Count - 1
                If filtergroup(i).Substring(0, filtergroup(i).IndexOf(":")) = My.Forms.Form2.listview1.Items(j).SubItems(10).Text Then
                    filtergroup(i) = filtergroup(i) + My.Forms.Form2.listview1.Items(j).SubItems(2).Text + ","
                End If

                If filtergroup(i).Substring(0, filtergroup(i).IndexOf(":")) = My.Forms.Form2.listview1.Items(j).SubItems(7).Text Then
                    filtergroup(i) = filtergroup(i) + My.Forms.Form2.listview1.Items(j).SubItems(2).Text + ","
                End If
            Next
        Next

        '--------------------------------------

        '-------------------------------------add customerfilter
        customerplotend = customerplotstart

        If System.IO.File.Exists(My.Application.Info.DirectoryPath + "\customerfilter.xml") Then
            Dim ds As New DataSet
            Dim list1counter As Integer
            '----清空原来的customer曲线
            Dim ii As Integer
            ii = customerplotstart
            Do Until data(ii, 1) = ""
                cwGraph4.Plots.RemoveAt(customerplotstart)
                cwGraph5.Plots.RemoveAt(customerplotstart)
                '清理plotnametoid
                plotnametoid.Remove(data(ii, 1))
                data(ii, 1) = ""
                TP(ii, 0) = Nothing

                ii = ii + 1
            Loop


            list1counter = List1.Items.Count

            ds.ReadXml(My.Application.Info.DirectoryPath + "\customerfilter.xml")
            If ds.Tables.Count > 0 And cwGraph4.Plots.Count > 0 And cwGraph5.Plots.Count > 0 Then
                If ds.Tables(0).Rows.Count > 0 Then

                    ii = customerplotstart
                    For i = 0 To ds.Tables(0).Rows.Count - 1
                        List1.Items.Add(ds.Tables(0).Rows(i).Item(0))
                        filtergroup(i + list1counter) = ds.Tables(0).Rows(i).Item(0) + ":" + ds.Tables(0).Rows(i).Item(1)


                        TP(ii + i, 0) = ds.Tables(0).Rows(i).Item(0)  '------添加customer filter UE和TP---------------
                        data(ii + i, 1) = "Group:" + ds.Tables(0).Rows(i).Item(0)
                        plotnametoid.Add(ii + i, data(ii + i, 1))
                        plot1 = New ScatterPlot
                        plot1.Tag = data(ii + i, 1)
                        plot1.PointStyle = PointStyle.SolidSquare
                        plot1.PointSize = New System.Drawing.Size(2, 2)
                        plot1.HistoryCapacity = cwGraph4.Plots(1).HistoryCapacity
                        cwGraph4.UseColorGenerator = True
                        cwGraph4.Plots.Add(plot1)

                        plot1 = New ScatterPlot
                        plot1.Tag = data(i + ii, 1)
                        plot1.PointStyle = PointStyle.SolidSquare
                        plot1.PointSize = New System.Drawing.Size(2, 2)
                        plot1.HistoryCapacity = cwGraph5.Plots(1).HistoryCapacity
                        cwGraph5.UseColorGenerator = True
                        cwGraph5.Plots.Add(plot1)
                        customerplotend = ii + i
                    Next

                End If


            End If

        End If

        customerfilterend = UBound(filtergroup) - 1


        '-----------------------------------------------
        For i = 0 To 200
            If data(i, 1) <> "" Then
                List1.Items.Add(data(i, 1))
            End If
        Next
        Return "OK"
    End Function

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        'Dim UElogname As String
        'Dim aa As Integer
        'aa = 1
        'For i = 1 To 200
        '    If data(i, 1) <> "" Then
        '        UElogname = UElogdir + "\" + data(i, 1) + ".log"
        '        ' getdata(UElogname, aa)
        '        aa = aa + 1
        '    End If
        '    My.Application.DoEvents()

        'Next

        'updatachart()
    End Sub

    Public Function getdata(ByVal FileName As String, ByVal UEnumber As Integer)
        Dim i, pp, pp2, pp3, pp4, j As Integer
        Dim Buff, yvalue, pointtime As String
        Dim searchedstr As String
        Dim LineBuff, Yvalues, trytimes, successtimes, tempstr As Object
        Dim start As Long
        ' writelog("getdata", False)
        ' writelog(Now.ToString("HH:mm:ss:ff"), False)
        If File.Exists(FileName) Then
            FileOpen(1, FileName, OpenMode.Binary, OpenAccess.Read, OpenShare.Shared)
            start = LOF(1)
            Buff = Space(100000)
            If start > 100000 Then
                start = start - 100000
            Else
                start = 1
            End If
            'UPGRADE_WARNING: Get was upgraded to FileGet and has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
            FileGet(1, Buff, start)
            FileClose(1)

            LineBuff = Split(Buff, vbCrLf) '这里的vbcrlf需要具体拿到你的文件的二进制格式才能确认是什么

            'Label1.Caption = LineBuff(104500)    '要哪一行,就直接取得

            i = UBound(LineBuff)
            j = 1
            pp = 1
            pp2 = 1
            pp3 = 1
            pp4 = 1
            While (j = 1 Or pp = 1 Or pp2 = 1 Or pp3 = 1 Or pp4 = 1) And i > 0
                My.Application.DoEvents()
                searchedstr = LineBuff(i)
                If InStr(searchedstr, "new start") > 0 Then
                    j = 0
                    pp = 0
                    pp2 = 0
                    pp3 = 0
                    pp4 = 0
                End If
                If InStr(searchedstr, "drop times:") > 0 And j = 1 Then '查找掉话字串

                    yvalue = Trim(Microsoft.VisualBasic.Right(searchedstr, Len(searchedstr) - InStr(searchedstr, "times:") - 5))
                    'pointtime = Trim(Microsoft.VisualBasic.Left(searchedstr, 19))
                    tempstr = Split(Trim(searchedstr), " ")
                    pointtime = tempstr(0) + " " + tempstr(1)
                    'Label1.Caption = pointtime
                    'timediff = DateDiff("s", "1899-12-30 00:00:00", pointtime) / 86400
                    data(UEnumber, 2) = pointtime
                    data(UEnumber, 3) = Int(yvalue)
                    j = 0
                End If
                My.Application.DoEvents()
                If InStr(searchedstr, "realtimes") > 0 And pp = 1 Then 'attach情况记录
                    'realtimes=9 trytimes=6 successtimes=5 success rate =83%
                    Yvalues = Split(searchedstr, " ")

                    yvalue = Trim(Microsoft.VisualBasic.Right(searchedstr, Len(searchedstr) - InStr(searchedstr, "realtimes=") - 5))
                    trytimes = Int(Split(Yvalues(5), "=")(1))
                    successtimes = Int(Split(Yvalues(6), "=")(1))
                    tempstr = Split(Trim(searchedstr), " ")
                    pointtime = tempstr(0) + " " + tempstr(1)
                    ' pointtime = Trim(Microsoft.VisualBasic.Left(searchedstr, 19))
                    'Label1.Caption = pointtime
                    'timediff = DateDiff("s", "1899-12-30 00:00:00", pointtime) / 86400
                    data(UEnumber, 4) = pointtime
                    data(UEnumber, 5) = trytimes
                    data(UEnumber, 6) = successtimes
                    pp = 0
                End If
                My.Application.DoEvents()
                If InStr(searchedstr, "lost packet rate") > 0 And pp2 = 1 Then 'ping 情况
                    'realtimes=9 trytimes=6 successtimes=5 success rate =83%
                    Yvalues = Split(searchedstr, "=")
                    If Trim(Yvalues(1)) = "100" Then
                        My.Forms.Form4.ListView1.Items(UEnumber - 1).SubItems(1).Text = "dropped"
                    Else
                        My.Forms.Form4.ListView1.Items(UEnumber - 1).SubItems(1).Text = "running"
                    End If
                    pp2 = 0
                End If
                My.Application.DoEvents()
                If InStr(searchedstr, "server unreachable") > 0 And pp3 = 1 And pp2 = 1 Then 'ping 情况
                    'realtimes=9 trytimes=6 successtimes=5 success rate =83%

                    My.Forms.Form4.ListView1.Items(UEnumber - 1).SubItems(1).Text = "server unreachable"
                    pp3 = 0
                End If
                My.Application.DoEvents()
                If InStr(searchedstr, "open") > 0 And InStr(searchedstr, "Fail") > 0 And pp3 = 1 And pp2 = 1 And pp4 = 1 Then 'ping 情况
                    'realtimes=9 trytimes=6 successtimes=5 success rate =83%

                    My.Forms.Form4.ListView1.Items(UEnumber - 1).SubItems(1).Text = "open COM Fail"
                    pp4 = 0
                End If
                My.Application.DoEvents()
                i = i - 1
            End While
            Erase LineBuff
            getdata = "done"
        Else
            getdata = "file not exist"
        End If
        ' writelog("getdata-end", False)
        'writelog(Now.ToString("HH:mm:ss:ff"), False)
    End Function

    Dim todaydate As Date
    Function updatachart()
        Dim logstr, output2, output3, output4, output5, output6 As String
        Dim y As Integer
        Dim datax(200) As Double
        Dim datay(200), timepointvalue As Double
        Dim a As Object
        ' writelog("updatachart", False)
        ' writelog(Now.ToString("HH:mm:ss:ff"), False)
        y = 1

        For i = 1 To customerplotstart - 1
            My.Application.DoEvents()
            If data(i, 1) <> "" Then
                'x
                datax(y - 1) = y
                'y
                datay(y - 1) = data(i, 3)

                My.Application.DoEvents()
                If data(i, 2) <> Nothing And graphfreeze <> True Then
                    timepointvalue = DateDiff("s", "0001-01-01 00:00:00", data(i, 2))
                    cwGraph2.Plots(i).PlotXYAppend(timepointvalue, data(i, 3))
                End If

                My.Application.DoEvents()
                If data(i, 4) <> Nothing And graphfreeze <> True Then
                    timepointvalue = DateDiff("s", "0001-01-01 00:00:00", data(i, 4))
                    cwGraph3.Plots(i).PlotXYAppend(timepointvalue, data(i, 6) / data(i, 5) * 100)
                End If

                If data(i, 2) = "" Then output2 = "null" Else output2 = Replace(data(i, 2), " ", "-")
                If data(i, 4) = "" Then output4 = "null" Else output4 = Replace(data(i, 4), " ", "-")
                If data(i, 3) = Nothing Then output3 = "null" Else output3 = Str(data(i, 3))
                If data(i, 5) = Nothing Then output5 = "null" Else output5 = Str(data(i, 5))
                If data(i, 6) = Nothing Then output6 = "null" Else output6 = Str(data(i, 6))
                logstr = data(i, 1) + " " + output2 + " " + output3 + " " + output4 + " " + output5 + " " + output6 + " null" + " null" + " null"
                a = writelog(logstr, False)
                '"UE-ip time drop-times drop-rate attach-times attach-success-rate TP-time DLPT ULPT"

                y = y + 1
            End If

        Next
        My.Application.DoEvents()
        'cwGraph1.PlotXY(datax, datay)
        ' writelog("updatachart-end", False)
        'writelog(Now.ToString("HH:mm:ss:ff"), False)
        Return "OK"
    End Function

    Private Sub cwGraph2_AfterDragAnnotationCaption(ByVal sender As Object, ByVal e As NationalInstruments.UI.AfterDragXYAnnotationCaptionEventArgs) Handles cwGraph2.AfterDragAnnotationCaption

    End Sub

    Private Sub cwGraph2_AfterDrawPointAnnotation(ByVal sender As Object, ByVal e As NationalInstruments.UI.AfterDrawXYPointAnnotationEventArgs) Handles cwGraph2.AfterDrawPointAnnotation

    End Sub

    Private Sub cwGraph2_AfterMoveCursor(ByVal sender As Object, ByVal e As NationalInstruments.UI.AfterMoveXYCursorEventArgs) Handles cwGraph2.AfterMoveCursor

    End Sub

    Private Sub cwGraph2_AnnotationsChanged(ByVal sender As Object, ByVal e As System.ComponentModel.CollectionChangeEventArgs) Handles cwGraph2.AnnotationsChanged

    End Sub

    Private Sub cwGraph2_BeforeDragAnnotationCaption(ByVal sender As Object, ByVal e As NationalInstruments.UI.BeforeDragXYAnnotationCaptionEventArgs) Handles cwGraph2.BeforeDragAnnotationCaption

    End Sub

    Private Sub cwGraph2_CaptionPositionChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cwGraph2.CaptionPositionChanged

    End Sub

    Private Sub cwGraph2_GiveFeedback(ByVal sender As Object, ByVal e As System.Windows.Forms.GiveFeedbackEventArgs) Handles cwGraph2.GiveFeedback

    End Sub

    Private Sub cwGraph2_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cwGraph2.MouseClick
        Dim xy As New XYPointAnnotation
        Dim annotationcapx, annotationcapy As Object
        If _Toolbar1_Button2.Checked = True Then
            xy = cwGraph2.Annotations(0)
            annotationcapx = (cwGraph2.XAxes(0).Range.Maximum - cwGraph2.XAxes(0).Range.Minimum) / 2 + cwGraph2.XAxes(0).Range.Minimum
            annotationcapy = (cwGraph2.YAxes(0).Range.Maximum - cwGraph2.YAxes(0).Range.Minimum) / 2 + cwGraph2.YAxes(0).Range.Minimum
            xy.SetCaptionPosition(annotationcapx, annotationcapy)
        End If
    End Sub

    Private Sub cwGraph2_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cwGraph2.MouseDown

    End Sub

    Private Sub cwGraph2_PlotAreaMouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cwGraph2.PlotAreaMouseDown
        Dim annotationcapx, annotationcapy As Object
        If (cwGraph2.HitTest(e.X, e.Y) = NationalInstruments.UI.XYGraphHitTestInfo.Plot) And _Toolbar1_Button2.Checked = True Then
            cwGraph2.Annotations(0).Caption = cwGraph2.GetPlotAt(e.X, e.Y).Tag
            Dim xy As New XYPointAnnotation
            xy = cwGraph2.Annotations(0)
            xy.XPosition = cwGraph2.PointToVirtual(e.Location).X * (cwGraph2.XAxes(0).Range.Maximum - cwGraph2.XAxes(0).Range.Minimum) + cwGraph2.XAxes(0).Range.Minimum
            xy.YPosition = cwGraph2.PointToVirtual(e.Location).Y * (cwGraph2.YAxes(0).Range.Maximum - cwGraph2.YAxes(0).Range.Minimum) + cwGraph2.YAxes(0).Range.Minimum
            annotationcapx = (cwGraph2.XAxes(0).Range.Maximum - cwGraph2.XAxes(0).Range.Minimum) / 2 + cwGraph2.XAxes(0).Range.Minimum
            annotationcapy = (cwGraph2.YAxes(0).Range.Maximum - cwGraph2.YAxes(0).Range.Minimum) / 2 + cwGraph2.YAxes(0).Range.Minimum
            xy.SetCaptionPosition(annotationcapx, annotationcapy)

            If xy.XPosition > 63082281600.0 Then
                TextBox1.Text = DateAdd("s", xy.XPosition - 63082281600.0, "2000-01-01 00:00:00")
            Else
                TextBox1.Text = DateAdd("s", xy.XPosition, "0001-01-01 00:00:00")
            End If
            TextBox2.Text = xy.YPosition

            cwGraph2.Annotations(0).Visible = True
            If e.Button = Windows.Forms.MouseButtons.Right Then
                cwGraph2.GetPlotAt(e.X, e.Y).Visible = False
            End If
        End If

        'cwGraph2.Annotations(0). = cwGraph2.PointToVirtual(e.Location).X * (cwGraph2.XAxes(0).Range.Maximum - cwGraph2.XAxes(0).Range.Minimum) + cwGraph2.XAxes(0).Range.Minimum
    End Sub

    Private Sub cwGraph2_PlotAreaMouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cwGraph2.PlotAreaMouseMove
        Dim from2000
        If cwGraph2.Cursors.Item(0).Visible = True Then
            cwGraph2.Cursors.Item(0).XPosition = cwGraph2.PointToVirtual(e.Location).X * (cwGraph2.XAxes(0).Range.Maximum - cwGraph2.XAxes(0).Range.Minimum) + cwGraph2.XAxes(0).Range.Minimum
            cwGraph2.Cursors.Item(0).YPosition = cwGraph2.PointToVirtual(e.Location).Y * (cwGraph2.YAxes(0).Range.Maximum - cwGraph2.YAxes(0).Range.Minimum) + cwGraph2.YAxes(0).Range.Minimum
            If cwGraph2.Cursors.Item(0).XPosition > 63082281600.0 Then
                from2000 = cwGraph2.Cursors.Item(0).XPosition - 63082281600.0
                TextBox1.Text = DateAdd("s", cwGraph2.Cursors.Item(0).XPosition - 63082281600.0, "2000-01-01 00:00:00")
            Else
                TextBox1.Text = DateAdd("s", cwGraph2.Cursors.Item(0).XPosition, "0001-01-01 00:00:00")
            End If
            'TextBox1.Text = Str(cwGraph5.Cursors.Item(0).XPosition)
            TextBox2.Text = Str(cwGraph2.Cursors.Item(0).YPosition)
        End If
    End Sub



    Private Sub cwGraph1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cwGraph1.MouseDown
        Dim xy As New XYPointAnnotation
        Dim annotationcapx, annotationcapy As Object
        If _Toolbar1_Button2.Checked = True Then
            xy = cwGraph1.Annotations(0)
            annotationcapx = (cwGraph1.XAxes(0).Range.Maximum - cwGraph1.XAxes(0).Range.Minimum) / 2 + cwGraph1.XAxes(0).Range.Minimum
            annotationcapy = (cwGraph1.YAxes(0).Range.Maximum - cwGraph1.YAxes(0).Range.Minimum) / 2 + cwGraph1.YAxes(0).Range.Minimum
            xy.SetCaptionPosition(annotationcapx, annotationcapy)
        End If
    End Sub

    Private Sub cwGraph1_PlotAreaMouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cwGraph1.PlotAreaMouseDown
        Dim annotationcapx, annotationcapy As Object
        If (cwGraph1.HitTest(e.X, e.Y) = NationalInstruments.UI.XYGraphHitTestInfo.Plot) And _Toolbar1_Button2.Checked = True Then
            'cwGraph1.Annotations(0).Caption = cwGraph1.GetPlotAt(e.X, e.Y).Tag
            Dim xy As New XYPointAnnotation
            xy = cwGraph1.Annotations(0)
            xy.XPosition = cwGraph1.PointToVirtual(e.Location).X * (cwGraph1.XAxes(0).Range.Maximum - cwGraph1.XAxes(0).Range.Minimum) + cwGraph1.XAxes(0).Range.Minimum
            xy.YPosition = cwGraph1.PointToVirtual(e.Location).Y * (cwGraph1.YAxes(0).Range.Maximum - cwGraph1.YAxes(0).Range.Minimum) + cwGraph1.YAxes(0).Range.Minimum
            annotationcapx = (cwGraph1.XAxes(0).Range.Maximum - cwGraph1.XAxes(0).Range.Minimum) / 2 + cwGraph1.XAxes(0).Range.Minimum
            annotationcapy = (cwGraph1.YAxes(0).Range.Maximum - cwGraph1.YAxes(0).Range.Minimum) / 2 + cwGraph1.YAxes(0).Range.Minimum
            xy.SetCaptionPosition(annotationcapx, annotationcapy)
            cwGraph1.Annotations(0).Caption = data(Math.Round(xy.XPosition), 1)
            TextBox1.Text = Math.Round(xy.XPosition)
            TextBox2.Text = Math.Round(xy.YPosition)

            cwGraph1.Annotations(0).Visible = True
            If e.Button = Windows.Forms.MouseButtons.Right Then
                cwGraph1.GetPlotAt(e.X, e.Y).Visible = False
            End If
        End If
    End Sub

    Private Sub cwGraph1_PlotAreaMouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cwGraph1.PlotAreaMouseMove
        If cwGraph1.Cursors.Item(0).Visible = True Then
            cwGraph1.Cursors.Item(0).XPosition = cwGraph1.PointToVirtual(e.Location).X * (cwGraph1.XAxes(0).Range.Maximum - cwGraph1.XAxes(0).Range.Minimum) + cwGraph1.XAxes(0).Range.Minimum
            cwGraph1.Cursors.Item(0).YPosition = cwGraph1.PointToVirtual(e.Location).Y * (cwGraph1.YAxes(0).Range.Maximum - cwGraph1.YAxes(0).Range.Minimum) + cwGraph1.YAxes(0).Range.Minimum
            'TextBox1.Text = DateAdd("s", cwGraph5.Cursors.Item(0).XPosition, "0001-01-01 00:00:00")
            TextBox1.Text = Str(cwGraph1.Cursors.Item(0).XPosition)
            TextBox2.Text = Str(cwGraph1.Cursors.Item(0).YPosition)
        End If
    End Sub

    Private Sub cwGraph1_PlotDataChanged(ByVal sender As System.Object, ByVal e As NationalInstruments.UI.XYPlotDataChangedEventArgs) Handles cwGraph1.PlotDataChanged

    End Sub

    Private Sub Timer2_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer2.Tick
        'Dim UETPlogname As String
        'Dim aa As Integer
        ''TextBox3.Text = TextBox3.Text + vbNewLine + "timer2 active"
        'aa = 1
        'For i = 1 To 200
        '    If data(i, 1) <> "" Then
        '        UETPlogname = UElogdir + "\" + data(i, 1) + ".TP.txt"
        '        'TextBox3.Text = TextBox3.Text + vbNewLine + UETPlogname
        '        getTPdata(UETPlogname, aa)
        '        aa = aa + 1
        '    End If
        '    My.Application.DoEvents()
        'Next
        'gettotalTP()
        'updataTP()
        Timer2.Interval = Val(MDIForm1.TPinterval.Text) * 1000

        Dim timestr As DateTime
        Dim uename As String
        Dim DLPT As Integer
        Dim ULPT As Integer
        Dim tmp As Object

        While Form4.TPmmsgpool.Count > 0
            tmp = Split(Form4.TPmmsgpool.Item(0), "|")
            uename = tmp(0)
            '************stub**********
            'tmp(1) = "asdcd"
            '************************
            Try
                timestr = Date.ParseExact(tmp(1), "MM/dd/yyyy   HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)

                'timestr = CDate(tmp(1))
                DLPT = Int(Mid(Trim(tmp(2).ToString), 1, Trim(tmp(2).ToString).IndexOf(" ")))
                ULPT = Int(Mid(Trim(tmp(2).ToString), Trim(tmp(2).ToString).IndexOf(" ") + 2, Trim(tmp(2).ToString).Length))
                If DLPT > 1024 Then DLPT = DLPT / 1.024
                If DLPT > 1024 * 1024 Then DLPT = DLPT / 1.024 / 1.024

                If ULPT > 1024 Then ULPT = ULPT / 1.024
                If ULPT > 1024 * 1024 Then ULPT = ULPT / 1.024 / 1.024

                For i = 1 To 200
                    If data(i, 1) Is Nothing Then
                        Exit For
                    ElseIf data(i, 1) = uename Then
                        TP(i, 1) = timestr.ToString("yyyy-MM-dd") + "   " + timestr.ToString("HH:mm:ss")
                        TP(i, 2) = DLPT
                        TP(i, 3) = ULPT
                        Exit For
                    End If

                Next

            Catch ex As Exception
                Dim array() As Byte
                Dim outputstr As String = ""
                array = System.Text.Encoding.ASCII.GetBytes(tmp(1))
                For jj = 0 To UBound(array)
                    outputstr = outputstr + "|" + array(jj).ToString
                Next
                MsgBox("Error,please copy following code or catch the image." + vbCrLf + outputstr)
            End Try


            Form4.TPmmsgpool.RemoveAt(0)
            Console.WriteLine("TPmmsgpool size:" + Form4.TPmmsgpool.Count.ToString)
        End While

        gettotalTP()
        updataTP()

        updateindicator()

    End Sub
    Public Sub gettotalTP()
        Dim aa As Integer
        Dim currenttimestring As String
        Dim Filterstring As String
        Try
            aa = 1
            '------------------清空total throughput--------------------------
            currenttimestring = Now().ToString("yyyy-MM-dd") + "   " + Now().ToString("HH:mm:ss")
            TP(0, 1) = currenttimestring
            TP(0, 2) = 0
            TP(0, 3) = 0
            '------------------清空customer total throughput---------------
            For i = customerplotstart To customerplotend
                If data(i, 1) <> "" Then
                    TP(i, 1) = currenttimestring
                    TP(i, 2) = 0
                    TP(i, 3) = 0
                End If
            Next


            For i = 1 To 200
                If data(i, 1) <> "" And TP(i, 1) <> "" Then
                    If DateDiff("s", currenttimestring, TP(i, 1)) > -(60 * 2) And TP(i, 0) = Nothing Then '后面tp（i，0）《》nothing 表示 filter的曲线不能累加
                        '累加非filter的throughput
                        TP(0, 2) = TP(0, 2) + TP(i, 2)
                        TP(0, 3) = TP(0, 3) + TP(i, 3)

                        ' writelog("customerfileterend=" + customerfilterend.ToString + ",customerfilterstart=" + customerfilterstart.ToString, False)
                        If customerfilterend >= customerfilterstart Then '累加filter的throughput
                            Dim kk = customerplotstart
                            For hh = customerfilterstart To customerfilterend
                                Filterstring = filtergroup(hh)
                                If InStr(Filterstring, data(i, 1)) > 0 Then
                                    TP(kk, 2) = TP(kk, 2) + TP(i, 2)
                                    TP(kk, 3) = TP(kk, 3) + TP(i, 3)

                                End If
                                kk = kk + 1
                            Next
                        End If
                    End If


                    aa = aa + 1
                End If


            Next

            '----------------计算group throughput---------------------
        Catch ex As Exception
        End Try


    End Sub


    Public Function getTPdata(ByVal FileName As String, ByVal UEnumber As Integer)

        Dim Buff As String, LineBuff() As Object
        Dim currenttime, currenttime2, currenttime3, realtime, pointtimereal, pointtime, pointtime1, pointtime2, pointtime3, pointtime4 As String
        Dim DLPT As Long
        Dim ULPT As Long
        DLPT = 0
        ULPT = 0
        Dim whattofind, finnalpoint As String
        Dim i, h, findreal, firstj, firstfind, j, totallines, zz As Integer
        Dim nowtime As Date
        Dim start As Long
        currenttime = ""
        currenttime2 = ""
        currenttime3 = ""
        realtime = ""
        pointtimereal = ""
        pointtime = ""
        pointtime1 = ""
        pointtime2 = ""
        pointtime3 = ""
        pointtime4 = ""
        whattofind = ""
        finnalpoint = ""
        'writelog("getTPdata", False)
        'writelog(Now.ToString("HH:mm:ss:ff"), False)
        'TextBox3.Text = TextBox3.Text + vbNewLine + "start check filename"
        If File.Exists(FileName) Then
            'Dim TxtReader As IO.StreamReader = New IO.StreamReader(FileName, System.Text.Encoding.Default, OpenAccess.Read,OpenMode .Binary)
            FileOpen(1, FileName, OpenMode.Binary, OpenAccess.Read, OpenShare.Shared)

            start = LOF(1)
            Buff = Space(20000)
            If start > 20000 Then
                start = start - 20000
            Else
                start = 1
            End If
            'UPGRADE_WARNING: Get was upgraded to FileGet and has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
            FileGet(1, Buff, start)

            FileClose(1)

            LineBuff = Split(Buff, vbCrLf) '这里的vbcrlf需要具体拿到你的文件的二进制格式才能确认是什么
            '   TextBox3.Text = TextBox3.Text + vbNewLine + Buff
            nowtime = Now()
            i = UBound(LineBuff)
            totallines = i
            '-----------------------check netmeterformate
            If netmeterlogformate = "" Then
                zz = i
                While (zz > 0) And netmeterlogformate = ""
                    If (InStr(LineBuff(zz), (nowtime.ToString("yyyy") + "-")) > 0) Then
                        netmeterlogformate = "yyyy-MM-dd"
                    End If
                    If (InStr(LineBuff(zz), ("/" + nowtime.ToString("yyyy"))) > 0) Then
                        netmeterlogformate = "MM/dd/yyyy"
                    End If

                    zz = zz - 1

                End While



            End If
            '----------------------------------------------
            If netmeterlogformate = "yyyy-MM-dd" Then
                realtime = nowtime.ToString("yyyy/MM/dd") + "   " + nowtime.ToString("HH:mm")
                realtime = Replace(realtime, "/", "-") '当前1分钟
                pointtimereal = nowtime.ToString("yyyy-MM-dd") + "   " + nowtime.ToString("HH:mm:ss")  '当前1分钟
                currenttime = DateAdd("n", -1, nowtime).ToString("yyyy/MM/dd   HH:mm")
                pointtime = DateAdd("n", -1, nowtime).ToString("yyyy-MM-dd HH:mm") '前1分钟
                pointtime1 = DateAdd("n", -1, nowtime).ToString("yyyy-MM-dd HH:mm:ss") '
                currenttime = Replace(currenttime, "/", "-")
                pointtime2 = DateAdd("n", -2, nowtime).ToString("yyyy/MM/dd   HH:mm") '前2分钟
                pointtime3 = DateAdd("n", -2, nowtime).ToString("yyyy-MM-dd HH:mm:ss")
                currenttime2 = Replace(pointtime2, "/", "-")
                pointtime4 = DateAdd("n", -3, nowtime).ToString("yyyy/MM/dd   HH:mm") '前3分钟
                'pointtime5 = DateAdd("n", -3, nowtime).ToString("yyyy-MM-dd HH:mm:ss") '
                currenttime3 = Replace(pointtime4, "/", "-")
            End If
            If netmeterlogformate = "MM/dd/yyyy" Then
                realtime = nowtime.ToString("MM/dd/yyyy") + "   " + nowtime.ToString("HH:mm")
                realtime = Replace(realtime, "-", "/") '当前1分钟
                pointtimereal = nowtime.ToString("yyyy-MM-dd") + "   " + nowtime.ToString("HH:mm:ss")  '当前1分钟
                currenttime = DateAdd("n", -1, nowtime).ToString("MM/dd/yyyy   HH:mm")
                pointtime = DateAdd("n", -1, nowtime).ToString("MM/dd/yyyy HH:mm") '前1分钟
                pointtime1 = DateAdd("n", -1, nowtime).ToString("MM/dd/yyyy HH:mm:ss") '
                currenttime = Replace(currenttime, "-", "/")
                pointtime2 = DateAdd("n", -2, nowtime).ToString("MM/dd/yyyy   HH:mm") '前2分钟
                pointtime3 = DateAdd("n", -2, nowtime).ToString("MM/dd/yyyy HH:mm:ss")
                currenttime2 = Replace(pointtime2, "-", "/")
                pointtime4 = DateAdd("n", -3, nowtime).ToString("MM/dd/yyyy   HH:mm") '前3分钟
                'pointtime5 = DateAdd("n", -3, nowtime).ToString("yyyy-MM-dd HH:mm:ss") '
                currenttime3 = Replace(pointtime4, "-", "/")
            End If
            ' LineBuff = Split(Buff, vbCrLf)  '这里的vbcrlf需要具体拿到你的文件的二进制格式才能确认是什么

            'Label1.Caption = LineBuff(104500)    '要哪一行,就直接取得


            h = 0
            findreal = 0 '发现的是当前一分钟
            firstj = 0 '第一次找到
            j = 1 ' j=1 表示找到
            firstfind = 0
            While j = 1 And i > 0
                My.Application.DoEvents()
                If InStr(LineBuff(i), "Traffic Rates Log") > 0 Then
                    j = 0
                Else
                    If InStr(LineBuff(i), realtime) > 0 And whattofind = "" Then
                        whattofind = realtime
                        finnalpoint = pointtimereal
                    End If

                    If InStr(LineBuff(i), currenttime) > 0 And whattofind = "" Then
                        whattofind = currenttime
                        finnalpoint = pointtimereal
                    End If


                    If InStr(LineBuff(i), currenttime2) > 0 And whattofind = "" Then
                        whattofind = currenttime2
                        finnalpoint = pointtimereal
                    End If

                    ' If InStr(LineBuff(i), currenttime3) > 0 And whattofind = "" Then
                    'whattofind = currenttime3
                    'finnalpoint = pointtimereal
                    'End If

                    If whattofind <> "" And InStr(LineBuff(i), whattofind) > 0 Then

                        DLPT = DLPT + Int(Trim(Mid(LineBuff(i), 28, 15)))
                        ULPT = ULPT + Int(Trim(Mid(LineBuff(i), 42, 15)))
                        h = h + 1 '找到多少行
                        j = 0 '只读取1行
                    Else
                        If whattofind <> "" Then
                            j = 0
                        End If
                    End If



                    i = i - 1
                    If totallines - 10 = i Then j = 0 '最只找10多行
                End If


            End While
            Erase LineBuff
            If h > 0 Then
                'timediff = DateDiff("s", "1899-12-30 00:00:00", finnalpoint) / 86400
                TP(UEnumber, 1) = finnalpoint
                If DLPT * 8 > 1024 And DLPT * 8 < 1048576 Then
                    DLPT = DLPT \ 1.024
                ElseIf DLPT > 1048576 Then
                    DLPT = DLPT \ 1.04875
                End If
                If ULPT * 8 > 1024 And ULPT * 8 < 1048576 Then
                    ULPT = ULPT \ 1.024
                ElseIf ULPT > 1048576 Then
                    ULPT = ULPT \ 1.04875
                End If
                TP(UEnumber, 2) = (DLPT * 8)
                TP(UEnumber, 3) = (ULPT * 8)

            End If
            getTPdata = "done"
        Else
            getTPdata = "file not exist"
            'TextBox3.Text = TextBox3.Text + vbNewLine + "file " + FileName + " is not exist"
        End If
        ' writelog("getTPdata-end", False)
        ' writelog(Now.ToString("HH:mm:ss:ff"), False)
    End Function
    Function updataTP()
        Dim logstr As String
        Dim xy(1, 200) As Object
        Dim timepointvalue As Double
        Dim y As Integer
        Dim output7, output8, output9 As String
        Dim a As Object
        ' writelog("updataTP", False)
        ' writelog(Now.ToString("HH:mm:ss:ff"), False)
        Try
            y = 1
            For i = 200 To 1 Step -1
                My.Application.DoEvents()
                If data(i, 1) <> "" Then '表示这个UE存在
                    My.Application.DoEvents()
                    If TP(i, 1) <> Nothing And graphfreeze <> True Then
                        timepointvalue = DateDiff("s", "00001-01-01 00:00:00", TP(i, 1))
                        cwGraph4.Plots(i).PlotXYAppend(timepointvalue, TP(i, 2))
                        My.Application.DoEvents()
                        cwGraph5.Plots(i).PlotXYAppend(timepointvalue, TP(i, 3))
                    End If

                    My.Application.DoEvents()

                    If TP(i, 1) = "" Then output7 = "null" Else output7 = Replace(TP(i, 1), " ", "-")
                    If TP(i, 2) = Nothing Then output8 = "null" Else output8 = TP(i, 2)
                    If TP(i, 3) = Nothing Then output9 = "null" Else output9 = TP(i, 3)
                    logstr = data(i, 1) + " null" + " null" + " null" + " null" + " null" + " " + output7 + " " + output8 + " " + output9
                    a = writelog(logstr, False)
                    '"UE-ip time drop-times drop-rate attach-times attach-success-rate TP-time DLPT ULPT"

                End If


            Next

            If TP(0, 1) <> Nothing And graphfreeze <> True Then
                timepointvalue = DateDiff("s", "00001-01-01 00:00:00", TP(0, 1))
                If cwGraph4.Plots.Count > 0 Then
                    cwGraph4.Plots(0).PlotXYAppend(timepointvalue, TP(0, 2))
                    cwGraph5.Plots(0).PlotXYAppend(timepointvalue, TP(0, 3))
                End If
            End If

            ' writelog("updataTP-end", False)
            ' writelog(Now.ToString("HH:mm:ss:ff"), False)
            Return "OK"
        Catch ex As Exception
        End Try

    End Function

    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub _Toolbar1_Button13_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _Toolbar1_Button13.Click
        If _Toolbar1_Button13.Checked = True Then
            logon = 1
        Else
            logon = 0
        End If
    End Sub

    Private Sub _Toolbar1_Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _Toolbar1_Button2.Click
        If _Toolbar1_Button2.Checked = True Then
            _Toolbar1_Button3.Checked = False
            _Toolbar1_Button4.Checked = False
            cwGraph1.Cursors(0).Visible = False
            cwGraph2.Cursors(0).Visible = False
            cwGraph3.Cursors(0).Visible = False
            cwGraph4.Cursors(0).Visible = False
            cwGraph5.Cursors(0).Visible = False
            CWGraph6.Cursors(0).Visible = False
            cwGraph1.InteractionMode = GraphInteractionModes.DragAnnotationCaption
            cwGraph2.InteractionMode = GraphInteractionModes.DragAnnotationCaption
            cwGraph3.InteractionMode = GraphInteractionModes.DragAnnotationCaption
            cwGraph4.InteractionMode = GraphInteractionModes.DragAnnotationCaption
            cwGraph5.InteractionMode = GraphInteractionModes.DragAnnotationCaption
            CWGraph6.InteractionMode = GraphInteractionModes.DragAnnotationCaption
            cwGraph1.InteractionModeDefault = GraphDefaultInteractionMode.None
            cwGraph2.InteractionModeDefault = GraphDefaultInteractionMode.None
            cwGraph3.InteractionModeDefault = GraphDefaultInteractionMode.None
            cwGraph4.InteractionModeDefault = GraphDefaultInteractionMode.None
            cwGraph5.InteractionModeDefault = GraphDefaultInteractionMode.None
            CWGraph6.InteractionModeDefault = GraphDefaultInteractionMode.None
            'cwGraph1.Annotations(0).Visible = True
            'cwGraph2.Annotations(0).Visible = True
            'cwGraph3.Annotations(0).Visible = True
            'cwGraph4.Annotations(0).Visible = True
            'cwGraph5.Annotations(0).Visible = True
        Else
            _Toolbar1_Button2.Checked = True
        End If
    End Sub

    Private Sub _Toolbar1_Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim temp1 As String
        temp1 = InputBox("Moniter update interval(s), this must be align with Netmeter record interval", vbOKOnly)

        If Val(temp1) <> 0 Then

            updateinterval = Int(temp1)
            'Timer1.Enabled = False
            Timer2.Enabled = False
            objTimer1.Change(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(updateinterval))
            'Timer1.Interval = updateinterval * 1000
            Timer2.Interval = updateinterval * 1000
            'Timer1.Enabled = True
            Timer2.Enabled = True
            ' _Toolbar1_Button8.Text = "interval" + vbCrLf + temp1
        End If
    End Sub

    Private Sub _Toolbar1_Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _Toolbar1_Button11.Click
        Dim pName As String
        If _Toolbar1_Button11.Text = "filter" Then
            _Toolbar1_Button11.Text = "OK"
            List1.Visible = True
        Else
            _Toolbar1_Button11.Text = "filter"
            List1.Visible = False
            List1.CheckedIndices.Contains(0)
            '------------------------------------------------------------------------------
            If List1.CheckedIndices.Contains(0) Then 'all
                For i = filterallcounter To List1.Items.Count - 1
                    pName = List1.Items(i)

                    'cwGraph1.Plots(pName).Visible = True
                    If plotnametoid(pName) <= cwGraph2.Plots.Count - 1 Then cwGraph2.Plots(plotnametoid(pName)).Visible = True
                    If plotnametoid(pName) <= cwGraph3.Plots.Count - 1 Then cwGraph3.Plots(plotnametoid(pName)).Visible = True
                    cwGraph4.Plots(plotnametoid(pName)).Visible = True
                    cwGraph5.Plots(plotnametoid(pName)).Visible = True
                Next
                Exit Sub
            End If
            '--------------------------------------------------------
            filtergroup(filterallcounter) = "selected:"
            For i = 1 To List1.Items.Count  'selected list
                If List1.CheckedIndices.Contains(i) Then
                    filtergroup(filterallcounter) = filtergroup(filterallcounter) + List1.Items(i) + ","
                End If
            Next


            '----
            For i = 0 To customerplotend
                '---
                If data(i, 1) <> "" Then
                    pName = data(i, 1)
                    '--
                    If i <= cwGraph2.Plots.Count - 1 Then cwGraph2.Plots(i).Visible = False
                    If i <= cwGraph3.Plots.Count - 1 Then cwGraph3.Plots(i).Visible = False
                    cwGraph4.Plots(i).Visible = False
                    cwGraph5.Plots(i).Visible = False

                    For j = 1 To filterallcounter

                        If InStr(filtergroup(j), pName) > 0 And j = filterallcounter Then 'filtergroup最后一个是UE 在手动选择UE名称形成的group
                            If i <= cwGraph2.Plots.Count - 1 Then cwGraph2.Plots(i).Visible = True
                            If i <= cwGraph3.Plots.Count - 1 Then cwGraph3.Plots(i).Visible = True
                            cwGraph4.Plots(i).Visible = True
                            cwGraph5.Plots(i).Visible = True
                        Else

                        End If
                        If InStr(filtergroup(j), pName) > 0 And j < filterallcounter And InStr(filtergroup(filterallcounter), Split(filtergroup(j), ":")(0)) > 0 Then 'UE 在被选择的预定义的filter
                            If i <= cwGraph2.Plots.Count - 1 Then cwGraph2.Plots(i).Visible = True
                            If i <= cwGraph3.Plots.Count - 1 Then cwGraph3.Plots(i).Visible = True
                            cwGraph4.Plots(i).Visible = True
                            cwGraph5.Plots(i).Visible = True
                        End If
                    Next
                    '--
                End If
                '---
            Next
            '----
        End If
    End Sub

    Private Sub cwGraph5_AfterMoveCursor(ByVal sender As Object, ByVal e As NationalInstruments.UI.AfterMoveXYCursorEventArgs) Handles cwGraph5.AfterMoveCursor


    End Sub

    Private Sub cwGraph5_CursorChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cwGraph5.CursorChanged

    End Sub

    Private Sub cwGraph5_CursorsChanged(ByVal sender As Object, ByVal e As System.ComponentModel.CollectionChangeEventArgs) Handles cwGraph5.CursorsChanged

    End Sub

    Private Sub cwGraph5_InteractionModeChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cwGraph5.InteractionModeChanged

    End Sub

    Private Sub cwGraph5_MouseCaptureChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cwGraph5.MouseCaptureChanged

    End Sub

    Private Sub cwGraph5_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cwGraph5.MouseClick
        Dim xy As New XYPointAnnotation
        Dim annotationcapx, annotationcapy As Object
        If _Toolbar1_Button2.Checked = True Then
            xy = cwGraph5.Annotations(0)
            annotationcapx = (cwGraph5.XAxes(0).Range.Maximum - cwGraph5.XAxes(0).Range.Minimum) / 2 + cwGraph5.XAxes(0).Range.Minimum
            annotationcapy = (cwGraph5.YAxes(0).Range.Maximum - cwGraph5.YAxes(0).Range.Minimum) / 2 + cwGraph5.YAxes(0).Range.Minimum
            xy.SetCaptionPosition(annotationcapx, annotationcapy)
        End If
    End Sub

    Private Sub cwGraph5_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cwGraph5.MouseMove



    End Sub

    Private Sub cwGraph5_PlotAreaMouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cwGraph5.PlotAreaMouseDown
        Dim annotationcapx, annotationcapy As Object
        If (cwGraph5.HitTest(e.X, e.Y) = NationalInstruments.UI.XYGraphHitTestInfo.Plot) And _Toolbar1_Button2.Checked = True Then
            cwGraph5.Annotations(0).Caption = cwGraph5.GetPlotAt(e.X, e.Y).Tag
            Dim xy As New XYPointAnnotation
            xy = cwGraph5.Annotations(0)
            xy.XPosition = cwGraph5.PointToVirtual(e.Location).X * (cwGraph5.XAxes(0).Range.Maximum - cwGraph5.XAxes(0).Range.Minimum) + cwGraph5.XAxes(0).Range.Minimum
            xy.YPosition = cwGraph5.PointToVirtual(e.Location).Y * (cwGraph5.YAxes(0).Range.Maximum - cwGraph5.YAxes(0).Range.Minimum) + cwGraph5.YAxes(0).Range.Minimum
            annotationcapx = (cwGraph5.XAxes(0).Range.Maximum - cwGraph5.XAxes(0).Range.Minimum) / 2 + cwGraph5.XAxes(0).Range.Minimum
            annotationcapy = (cwGraph5.YAxes(0).Range.Maximum - cwGraph5.YAxes(0).Range.Minimum) / 2 + cwGraph5.YAxes(0).Range.Minimum
            xy.SetCaptionPosition(annotationcapx, annotationcapy)

            If xy.XPosition > 63082281600.0 Then
                TextBox1.Text = DateAdd("s", xy.XPosition - 63082281600.0, "2000-01-01 00:00:00")
            Else
                TextBox1.Text = DateAdd("s", xy.XPosition, "0001-01-01 00:00:00")
            End If
            TextBox2.Text = xy.YPosition

            cwGraph5.Annotations(0).Visible = True
            If e.Button = Windows.Forms.MouseButtons.Right Then
                cwGraph5.GetPlotAt(e.X, e.Y).Visible = False
            End If
        End If
    End Sub

    Private Sub cwGraph5_PlotAreaMouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cwGraph5.PlotAreaMouseMove
        If cwGraph5.Cursors.Item(0).Visible = True Then
            cwGraph5.Cursors.Item(0).XPosition = cwGraph5.PointToVirtual(e.Location).X * (cwGraph5.XAxes(0).Range.Maximum - cwGraph5.XAxes(0).Range.Minimum) + cwGraph5.XAxes(0).Range.Minimum
            cwGraph5.Cursors.Item(0).YPosition = cwGraph5.PointToVirtual(e.Location).Y * (cwGraph5.YAxes(0).Range.Maximum - cwGraph5.YAxes(0).Range.Minimum) + cwGraph5.YAxes(0).Range.Minimum
            If cwGraph5.Cursors.Item(0).XPosition > 63082281600.0 Then
                TextBox1.Text = DateAdd("s", cwGraph5.Cursors.Item(0).XPosition - 63082281600.0, "2000-01-01 00:00:00")
            Else
                TextBox1.Text = DateAdd("s", cwGraph5.Cursors.Item(0).XPosition, "0001-01-01 00:00:00")
            End If
            'TextBox1.Text = Str(cwGraph5.Cursors.Item(0).XPosition)
            TextBox2.Text = Str(cwGraph5.Cursors.Item(0).YPosition)

        End If
    End Sub

    Private Sub cwGraph5_PlotDataChanged(ByVal sender As System.Object, ByVal e As NationalInstruments.UI.XYPlotDataChangedEventArgs) Handles cwGraph5.PlotDataChanged

    End Sub

    Private Sub _Toolbar1_Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _Toolbar1_Button6.Click
        If _Toolbar1_Button6.Checked = True Then
            graphfreeze = True
        Else
            graphfreeze = False
        End If
    End Sub

    Private Sub _Toolbar1_Button14_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub _Toolbar1_Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _Toolbar1_Button3.Click
        If _Toolbar1_Button3.Checked = True Then
            _Toolbar1_Button2.Checked = False
            _Toolbar1_Button4.Checked = False
            cwGraph1.Cursors(0).Visible = False
            cwGraph2.Cursors(0).Visible = False
            cwGraph3.Cursors(0).Visible = False
            cwGraph4.Cursors(0).Visible = False
            cwGraph5.Cursors(0).Visible = False
            CWGraph6.Cursors(0).Visible = False
            cwGraph1.InteractionMode = GraphInteractionModes.ZoomX
            cwGraph2.InteractionMode = GraphInteractionModes.ZoomX
            cwGraph3.InteractionMode = GraphInteractionModes.ZoomX
            cwGraph4.InteractionMode = GraphInteractionModes.ZoomX
            cwGraph5.InteractionMode = GraphInteractionModes.ZoomX
            CWGraph6.InteractionMode = GraphInteractionModes.ZoomX
            cwGraph1.InteractionModeDefault = GraphDefaultInteractionMode.ZoomX
            cwGraph2.InteractionModeDefault = GraphDefaultInteractionMode.ZoomX
            cwGraph3.InteractionModeDefault = GraphDefaultInteractionMode.ZoomX
            cwGraph4.InteractionModeDefault = GraphDefaultInteractionMode.ZoomX
            cwGraph5.InteractionModeDefault = GraphDefaultInteractionMode.ZoomX
            CWGraph6.InteractionModeDefault = GraphDefaultInteractionMode.ZoomX
            cwGraph1.Annotations(0).Visible = False
            cwGraph2.Annotations(0).Visible = False
            cwGraph3.Annotations(0).Visible = False
            cwGraph4.Annotations(0).Visible = False
            cwGraph5.Annotations(0).Visible = False
            CWGraph6.Annotations(0).Visible = False
        Else
            _Toolbar1_Button3.Checked = True
        End If
    End Sub

    Private Sub _Toolbar1_Button4_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles _Toolbar1_Button4.CheckedChanged

    End Sub

    Private Sub _Toolbar1_Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _Toolbar1_Button4.Click
        If _Toolbar1_Button4.Checked = True Then
            _Toolbar1_Button2.Checked = False
            _Toolbar1_Button3.Checked = False
            cwGraph1.Cursors(0).Visible = True
            cwGraph2.Cursors(0).Visible = True
            cwGraph3.Cursors(0).Visible = True
            cwGraph4.Cursors(0).Visible = True
            cwGraph5.Cursors(0).Visible = True
            CWGraph6.Cursors(0).Visible = True
            cwGraph1.InteractionMode = GraphInteractionModes.DragCursor
            cwGraph2.InteractionMode = GraphInteractionModes.DragCursor
            cwGraph3.InteractionMode = GraphInteractionModes.DragCursor
            cwGraph4.InteractionMode = GraphInteractionModes.DragCursor
            cwGraph5.InteractionMode = GraphInteractionModes.DragCursor
            CWGraph6.InteractionMode = GraphInteractionModes.DragCursor
            cwGraph1.InteractionModeDefault = GraphDefaultInteractionMode.None
            cwGraph2.InteractionModeDefault = GraphDefaultInteractionMode.None
            cwGraph3.InteractionModeDefault = GraphDefaultInteractionMode.None
            cwGraph4.InteractionModeDefault = GraphDefaultInteractionMode.None
            cwGraph5.InteractionModeDefault = GraphDefaultInteractionMode.None
            CWGraph6.InteractionModeDefault = GraphDefaultInteractionMode.None
            cwGraph1.Annotations(0).Visible = False
            cwGraph2.Annotations(0).Visible = False
            cwGraph3.Annotations(0).Visible = False
            cwGraph4.Annotations(0).Visible = False
            cwGraph5.Annotations(0).Visible = False
            CWGraph6.Annotations(0).Visible = False
        Else
            _Toolbar1_Button4.Checked = True
        End If
    End Sub

    Private Sub Button2_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
    End Sub

    Private Sub cwGraph4_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cwGraph4.MouseClick
        Dim xy As New XYPointAnnotation
        Dim annotationcapx, annotationcapy As Object
        If _Toolbar1_Button2.Checked = True Then
            xy = cwGraph4.Annotations(0)
            annotationcapx = (cwGraph4.XAxes(0).Range.Maximum - cwGraph4.XAxes(0).Range.Minimum) / 2 + cwGraph4.XAxes(0).Range.Minimum
            annotationcapy = (cwGraph4.YAxes(0).Range.Maximum - cwGraph4.YAxes(0).Range.Minimum) / 2 + cwGraph4.YAxes(0).Range.Minimum
            xy.SetCaptionPosition(annotationcapx, annotationcapy)
        End If
    End Sub

    Private Sub cwGraph4_PlotAreaMouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cwGraph4.PlotAreaMouseDown
        Dim annotationcapx, annotationcapy As Object
        If (cwGraph4.HitTest(e.X, e.Y) = NationalInstruments.UI.XYGraphHitTestInfo.Plot) And _Toolbar1_Button2.Checked = True Then
            cwGraph4.Annotations(0).Caption = cwGraph4.GetPlotAt(e.X, e.Y).Tag
            Dim xy As New XYPointAnnotation
            xy = cwGraph4.Annotations(0)
            xy.XPosition = cwGraph4.PointToVirtual(e.Location).X * (cwGraph4.XAxes(0).Range.Maximum - cwGraph4.XAxes(0).Range.Minimum) + cwGraph4.XAxes(0).Range.Minimum
            xy.YPosition = cwGraph4.PointToVirtual(e.Location).Y * (cwGraph4.YAxes(0).Range.Maximum - cwGraph4.YAxes(0).Range.Minimum) + cwGraph4.YAxes(0).Range.Minimum
            annotationcapx = (cwGraph4.XAxes(0).Range.Maximum - cwGraph4.XAxes(0).Range.Minimum) / 2 + cwGraph4.XAxes(0).Range.Minimum
            annotationcapy = (cwGraph4.YAxes(0).Range.Maximum - cwGraph4.YAxes(0).Range.Minimum) / 2 + cwGraph4.YAxes(0).Range.Minimum
            xy.SetCaptionPosition(annotationcapx, annotationcapy)

            If xy.XPosition > 63082281600.0 Then
                TextBox1.Text = DateAdd("s", xy.XPosition - 63082281600.0, "2000-01-01 00:00:00")
            Else
                TextBox1.Text = DateAdd("s", xy.XPosition, "0001-01-01 00:00:00")
            End If
            TextBox2.Text = xy.YPosition

            cwGraph4.Annotations(0).Visible = True
            If e.Button = Windows.Forms.MouseButtons.Right Then
                cwGraph4.GetPlotAt(e.X, e.Y).Visible = False
            End If
        End If
    End Sub

    Private Sub cwGraph4_PlotAreaMouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cwGraph4.PlotAreaMouseMove
        If cwGraph4.Cursors.Item(0).Visible = True Then
            cwGraph4.Cursors.Item(0).XPosition = cwGraph4.PointToVirtual(e.Location).X * (cwGraph4.XAxes(0).Range.Maximum - cwGraph4.XAxes(0).Range.Minimum) + cwGraph4.XAxes(0).Range.Minimum
            cwGraph4.Cursors.Item(0).YPosition = cwGraph4.PointToVirtual(e.Location).Y * (cwGraph4.YAxes(0).Range.Maximum - cwGraph4.YAxes(0).Range.Minimum) + cwGraph4.YAxes(0).Range.Minimum
            If cwGraph4.Cursors.Item(0).XPosition > 63082281600.0 Then
                TextBox1.Text = DateAdd("s", cwGraph4.Cursors.Item(0).XPosition - 63082281600.0, "2000-01-01 00:00:00")
            Else
                TextBox1.Text = DateAdd("s", cwGraph4.Cursors.Item(0).XPosition, "0001-01-01 00:00:00")
            End If
            'TextBox1.Text = Str(cwGraph5.Cursors.Item(0).XPosition)
            TextBox2.Text = Str(cwGraph4.Cursors.Item(0).YPosition)
        End If
    End Sub

    Private Sub cwGraph3_PlotAreaMouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        If cwGraph3.Cursors.Item(0).Visible = True Then
            cwGraph3.Cursors.Item(0).XPosition = cwGraph3.PointToVirtual(e.Location).X * (cwGraph3.XAxes(0).Range.Maximum - cwGraph3.XAxes(0).Range.Minimum) + cwGraph3.XAxes(0).Range.Minimum
            cwGraph3.Cursors.Item(0).YPosition = cwGraph3.PointToVirtual(e.Location).Y * (cwGraph3.YAxes(0).Range.Maximum - cwGraph3.YAxes(0).Range.Minimum) + cwGraph3.YAxes(0).Range.Minimum

            'TextBox1.Text = DateAdd("s", cwGraph3.Cursors.Item(0).XPosition, "0001-01-01 00:00:00")'overload
            If cwGraph3.Cursors.Item(0).XPosition > 63082281600.0 Then
                TextBox1.Text = DateAdd("s", cwGraph3.Cursors.Item(0).XPosition - 63082281600.0, "2000-01-01 00:00:00")
            Else
                TextBox1.Text = DateAdd("s", cwGraph3.Cursors.Item(0).XPosition, "0001-01-01 00:00:00")
            End If

            'TextBox1.Text = Str(cwGraph5.Cursors.Item(0).XPosition)
            TextBox2.Text = Str(cwGraph3.Cursors.Item(0).YPosition)
        End If
    End Sub

    Private Sub cwGraph3_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cwGraph3.MouseClick
        Dim xy As New XYPointAnnotation
        Dim annotationcapx, annotationcapy As Object
        If _Toolbar1_Button2.Checked = True Then
            xy = cwGraph3.Annotations(0)
            annotationcapx = (cwGraph3.XAxes(0).Range.Maximum - cwGraph3.XAxes(0).Range.Minimum) / 2 + cwGraph3.XAxes(0).Range.Minimum
            annotationcapy = (cwGraph3.YAxes(0).Range.Maximum - cwGraph3.YAxes(0).Range.Minimum) / 2 + cwGraph3.YAxes(0).Range.Minimum
            xy.SetCaptionPosition(annotationcapx, annotationcapy)
        End If
    End Sub

    Private Sub cwGraph3_PlotAreaMouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cwGraph3.PlotAreaMouseDown
        Dim annotationcapx, annotationcapy As Object
        If (cwGraph3.HitTest(e.X, e.Y) = NationalInstruments.UI.XYGraphHitTestInfo.Plot) And _Toolbar1_Button2.Checked = True Then
            cwGraph3.Annotations(0).Caption = cwGraph3.GetPlotAt(e.X, e.Y).Tag
            Dim xy As New XYPointAnnotation
            xy = cwGraph3.Annotations(0)
            xy.XPosition = cwGraph3.PointToVirtual(e.Location).X * (cwGraph3.XAxes(0).Range.Maximum - cwGraph3.XAxes(0).Range.Minimum) + cwGraph3.XAxes(0).Range.Minimum
            xy.YPosition = cwGraph3.PointToVirtual(e.Location).Y * (cwGraph3.YAxes(0).Range.Maximum - cwGraph3.YAxes(0).Range.Minimum) + cwGraph3.YAxes(0).Range.Minimum
            annotationcapx = (cwGraph3.XAxes(0).Range.Maximum - cwGraph3.XAxes(0).Range.Minimum) / 2 + cwGraph3.XAxes(0).Range.Minimum
            annotationcapy = (cwGraph3.YAxes(0).Range.Maximum - cwGraph3.YAxes(0).Range.Minimum) / 2 + cwGraph3.YAxes(0).Range.Minimum
            xy.SetCaptionPosition(annotationcapx, annotationcapy)

            If xy.XPosition > 63082281600.0 Then
                TextBox1.Text = DateAdd("s", xy.XPosition - 63082281600.0, "2000-01-01 00:00:00")
            Else
                TextBox1.Text = DateAdd("s", xy.XPosition, "0001-01-01 00:00:00")
            End If
            TextBox2.Text = xy.YPosition

            cwGraph3.Annotations(0).Visible = True
            If e.Button = Windows.Forms.MouseButtons.Right Then
                cwGraph3.GetPlotAt(e.X, e.Y).Visible = False
            End If
        End If
    End Sub

    Private Sub cwGraph3_PlotAreaMouseMove1(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles cwGraph3.PlotAreaMouseMove
        If cwGraph3.Cursors.Item(0).Visible = True Then
            cwGraph3.Cursors.Item(0).XPosition = cwGraph3.PointToVirtual(e.Location).X * (cwGraph3.XAxes(0).Range.Maximum - cwGraph3.XAxes(0).Range.Minimum) + cwGraph3.XAxes(0).Range.Minimum
            cwGraph3.Cursors.Item(0).YPosition = cwGraph3.PointToVirtual(e.Location).Y * (cwGraph3.YAxes(0).Range.Maximum - cwGraph3.YAxes(0).Range.Minimum) + cwGraph3.YAxes(0).Range.Minimum
            If cwGraph3.Cursors.Item(0).XPosition > 63082281600.0 Then
                TextBox1.Text = DateAdd("s", cwGraph3.Cursors.Item(0).XPosition - 63082281600.0, "2000-01-01 00:00:00")
            Else
                TextBox1.Text = DateAdd("s", cwGraph3.Cursors.Item(0).XPosition, "0001-01-01 00:00:00")
            End If
            'TextBox1.Text = Str(cwGraph5.Cursors.Item(0).XPosition)
            TextBox2.Text = Str(cwGraph3.Cursors.Item(0).YPosition)
        End If
    End Sub

    Private Sub cwGraph2_PlotsChanged(ByVal sender As Object, ByVal e As System.ComponentModel.CollectionChangeEventArgs) Handles cwGraph2.PlotsChanged
    End Sub

    Private Sub updateindicator()
        'Dim UElogname As String
        'Dim aa As Integer
        'aa = 1
        'For i = 1 To 200
        '    If data(i, 1) <> "" Then
        '        UElogname = UElogdir + "\" + data(i, 1) + ".log"
        '        getdata1(UElogname, aa)
        '        aa = aa + 1
        '    End If

        'Next
        'Invoke(New DelegateSub(AddressOf updatachart1), "")

        Dim timestr As DateTime
        Dim uename, message, yvalue As String
        Dim tmp, Yvalues As Object
        Dim trytimes, successtimes As Integer
        Try
            While Form4.OPmmsgpool.Count > 0
                tmp = Split(Form4.OPmmsgpool.Item(0), "|")
                If UBound(tmp) >= 2 Then
                    uename = tmp(0)
                    timestr = CDate(tmp(1))
                    message = tmp(2)
                    For i = 1 To 200
                        If data(i, 1) Is Nothing Then
                            Exit For
                        ElseIf data(i, 1) = uename Then

                            If InStr(message, "drop times:") > 0 Then '查找掉话字串

                                yvalue = Trim(Microsoft.VisualBasic.Right(message, Len(message) - InStr(message, "times:") - 5))
                                data(i, 2) = timestr.ToString("yyyy-MM-dd") + "   " + timestr.ToString("HH:mm:ss")
                                data(i, 3) = Int(yvalue)
                            End If

                            If InStr(message, "realtimes") >= 0 Then 'attach情况记录
                                'realtimes=9 trytimes=6 successtimes=5 success rate =83%
                                Yvalues = Split(message, " ")
                                For stri = 2 To UBound(Yvalues)

                                    If Yvalues(stri).ToString.IndexOf("trytimes=") >= 0 Then
                                        trytimes = Int(Split(Yvalues(stri), "=")(1))
                                    End If
                                    If Yvalues(stri).ToString.IndexOf("successtimes=") >= 0 Then
                                        successtimes = Int(Split(Yvalues(stri), "=")(1))
                                    End If
                                Next

                                data(i, 4) = timestr.ToString("yyyy-MM-dd") + "   " + timestr.ToString("HH:mm:ss")
                                data(i, 5) = trytimes
                                data(i, 6) = successtimes

                            End If



                            Exit For
                        End If

                    Next


                End If
                If Form4.OPmmsgpool.Count >= 1 Then
                    writeOPlog(Form4.OPmmsgpool(0), False)
                    Form4.OPmmsgpool.RemoveAt(0)

                    If consoleon = True Then Console.WriteLine("OPmmsgpool size:" + Form4.OPmmsgpool.Count.ToString)
                End If


            End While
            'While Form4.OPmmsgpool.Count > 0
            '    Form4.OPmmsgpool.RemoveAt(0)
            '    Console.WriteLine("OPmmsgpool size:" + Form4.OPmmsgpool.Count.ToString)
            'End While
            updatachart1()

        Catch ex As Exception
            writelog("error find in update indicator:" + Form4.OPmmsgpool.Item(0).ToString, False)

        End Try



    End Sub

    Private Function getdata1(ByVal FileName As String, ByVal UEnumber As Integer)
        Dim i, pp, pp2, pp3, pp4, j, totallines As Integer
        Dim Buff, yvalue, pointtime As String
        Dim searchedstr As String
        Dim LineBuff, Yvalues, trytimes, successtimes, tempstr As Object
        Dim start As Long
        LineBuff = Nothing
        Yvalues = Nothing
        trytimes = Nothing
        successtimes = Nothing
        tempstr = Nothing

        If File.Exists(FileName) Then
            FileOpen(1, FileName, OpenMode.Binary, OpenAccess.Read, OpenShare.Shared)

            start = LOF(1)
            Buff = Space(100000)
            If start > 100000 Then
                start = start - 100000
            Else
                start = 1
            End If
            'UPGRADE_WARNING: Get was upgraded to FileGet and has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
            FileGet(1, Buff, start)

            FileClose(1)

            LineBuff = Split(Buff, vbCrLf) '这里的vbcrlf需要具体拿到你的文件的二进制格式才能确认是什么

            i = UBound(LineBuff)
            totallines = i
            j = 1
            pp = 1
            pp2 = 1
            pp3 = 1
            pp4 = 1
            While (j = 1 Or pp = 1 Or pp2 = 1 Or pp3 = 1 Or pp4 = 1) And i > 0
                searchedstr = LineBuff(i)
                If InStr(searchedstr, "new start") > 0 Then
                    j = 0
                    pp = 0
                    pp2 = 0
                    pp3 = 0
                    pp4 = 0
                End If
                If InStr(searchedstr, "drop times:") > 0 And j = 1 Then '查找掉话字串

                    yvalue = Trim(Microsoft.VisualBasic.Right(searchedstr, Len(searchedstr) - InStr(searchedstr, "times:") - 5))
                    tempstr = Split(Trim(searchedstr), " ")
                    pointtime = tempstr(0) + " " + tempstr(1)
                    'pointtime = Trim(Microsoft.VisualBasic.Left(searchedstr, 19))
                    'Label1.Caption = pointtime
                    'timediff = DateDiff("s", "1899-12-30 00:00:00", pointtime) / 86400
                    data(UEnumber, 2) = pointtime
                    data(UEnumber, 3) = Int(yvalue)
                    j = 0
                End If

                If InStr(searchedstr, "realtimes") > 0 And pp = 1 Then 'attach情况记录
                    'realtimes=9 trytimes=6 successtimes=5 success rate =83%
                    Yvalues = Split(searchedstr, " ")
                    For stri = 2 To UBound(Yvalues)
                        'If Yvalues(stri).ToString.IndexOf("realtimes=") >= 0 Then
                        'realtimes = Int(Split(Yvalues(stri), "=")(1))
                        'End If
                        If Yvalues(stri).ToString.IndexOf("trytimes=") >= 0 Then
                            trytimes = Int(Split(Yvalues(stri), "=")(1))
                        End If
                        If Yvalues(stri).ToString.IndexOf("successtimes=") >= 0 Then
                            successtimes = Int(Split(Yvalues(stri), "=")(1))
                        End If
                    Next
                    'yvalue = Trim(Microsoft.VisualBasic.Right(searchedstr, Len(searchedstr) - InStr(searchedstr, "realtimes=") - 5))

                    'trytimes = Int(Split(Yvalues(5), "=")(1))
                    'successtimes = Int(Split(Yvalues(6), "=")(1))
                    tempstr = Split(Trim(searchedstr), " ")
                    pointtime = tempstr(0) + " " + tempstr(1)
                    'pointtime = Trim(Microsoft.VisualBasic.Left(searchedstr, 19))
                    'Label1.Caption = pointtime
                    'timediff = DateDiff("s", "1899-12-30 00:00:00", pointtime) / 86400
                    data(UEnumber, 4) = pointtime
                    data(UEnumber, 5) = trytimes
                    data(UEnumber, 6) = successtimes
                    pp = 0
                End If

                If InStr(searchedstr, "lost packet rate") > 0 And pp2 = 1 Then 'ping 情况
                    'realtimes=9 trytimes=6 successtimes=5 success rate =83%
                    Yvalues = Split(searchedstr, "=")
                    If Trim(Yvalues(1)) = "100" Then
                        ' My.Forms.Form4.listview1.Items(UEnumber - 1).SubItems(1).Text = "dropped"
                        data(UEnumber, 7) = "dropped"
                    Else
                        'My.Forms.Form4.listview1.Items(UEnumber - 1).SubItems(1).Text = "running"
                        data(UEnumber, 7) = "running"
                    End If
                    pp2 = 0
                End If

                If InStr(searchedstr, "server unreachable") > 0 And pp3 = 1 And pp2 = 1 Then 'ping 情况
                    'realtimes=9 trytimes=6 successtimes=5 success rate =83%

                    'My.Forms.Form4.listview1.Items(UEnumber - 1).SubItems(1).Text = "server unreachable"
                    data(UEnumber, 7) = "server unreachable"
                    pp3 = 0
                End If

                If InStr(searchedstr, "open") > 0 And InStr(searchedstr, "Fail") > 0 And pp3 = 1 And pp2 = 1 And pp4 = 1 Then 'ping 情况
                    'realtimes=9 trytimes=6 successtimes=5 success rate =83%

                    'My.Forms.Form4.listview1.Items(UEnumber - 1).SubItems(1).Text = "open COM Fail"
                    data(UEnumber, 7) = "open COM Fail"
                    pp4 = 0
                End If

                i = i - 1
                If totallines - 100 = i Then '最大只找100行
                    j = 0
                    pp = 0
                    pp2 = 0
                    pp3 = 0
                    pp4 = 0
                End If
            End While
            Erase LineBuff
            getdata1 = "done"
        Else
            getdata1 = "file not exist"
        End If
    End Function
    Public Delegate Sub DelegateSub(ByVal text As String) '
    Function updatachart1()
        Dim logstr, output2, output3, output4, output5, output6 As String
        Dim y As Integer
        Dim datax(200) As Double
        Dim datay(200), datayd(200), timepointvalue As Double
        Dim a As Object
        ' writelog("updatachart", False)
        ' writelog(Now.ToString("HH:mm:ss:ff"), False)
        y = 1

        For i = 1 To customerplotstart - 1
            If data(i, 1) <> "" Then
                'x
                datax(y - 1) = y
                'y
                datay(y - 1) = data(i, 3)
                datayd(y - 1) = data(i, 5) - data(i, 6)
                'My.Forms.Form4.ListView1.Items(i - 1).SubItems(1).Text = data(i, 7)
                My.Application.DoEvents()
                If data(i, 2) <> Nothing And graphfreeze <> True Then
                    timepointvalue = DateDiff("s", "0001-01-01 00:00:00", data(i, 2))
                    cwGraph2.Plots(i).PlotXYAppend(timepointvalue, data(i, 3))
                End If

                My.Application.DoEvents()
                If data(i, 4) <> Nothing And graphfreeze <> True Then
                    timepointvalue = DateDiff("s", "0001-01-01 00:00:00", data(i, 4))
                    cwGraph3.Plots(i).PlotXYAppend(timepointvalue, data(i, 6) / data(i, 5) * 100)
                    datayd(y - 1) = data(i, 5) - data(i, 6)
                End If

                If data(i, 2) = "" Then output2 = "null" Else output2 = Replace(data(i, 2), " ", "-")
                If data(i, 4) = "" Then output4 = "null" Else output4 = Replace(data(i, 4), " ", "-")
                If data(i, 3) = Nothing Then output3 = "null" Else output3 = Str(data(i, 3))
                If data(i, 5) = Nothing Then output5 = "null" Else output5 = Str(data(i, 5))
                If data(i, 6) = Nothing Then output6 = "null" Else output6 = Str(data(i, 6))
                logstr = data(i, 1) + " " + output2 + " " + output3 + " " + output4 + " " + output5 + " " + output6 + " null" + " null" + " null"
                a = writelog(logstr, False)
                '"UE-ip time drop-times drop-rate attach-times attach-success-rate TP-time DLPT ULPT"

                y = y + 1
            End If

        Next
        cwGraph1.PlotXY(datax, datay)
        CWGraph6.PlotXY(datax, datayd)
        Return "OK"
    End Function

    Private Sub _SSTab1_TabPage0_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _SSTab1_TabPage0.Click

    End Sub

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Private Sub CWGraph6_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles CWGraph6.MouseClick
        Dim xy As New XYPointAnnotation
        Dim annotationcapx, annotationcapy As Object
        If _Toolbar1_Button2.Checked = True Then
            xy = CWGraph6.Annotations(0)
            annotationcapx = (CWGraph6.XAxes(0).Range.Maximum - CWGraph6.XAxes(0).Range.Minimum) / 2 + CWGraph6.XAxes(0).Range.Minimum
            annotationcapy = (CWGraph6.YAxes(0).Range.Maximum - CWGraph6.YAxes(0).Range.Minimum) / 2 + CWGraph6.YAxes(0).Range.Minimum
            xy.SetCaptionPosition(annotationcapx, annotationcapy)
        End If
    End Sub

    Private Sub CWGraph6_PlotAreaMouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles CWGraph6.PlotAreaMouseDown
        Dim annotationcapx, annotationcapy As Object
        If (CWGraph6.HitTest(e.X, e.Y) = NationalInstruments.UI.XYGraphHitTestInfo.Plot) And _Toolbar1_Button2.Checked = True Then
            'cwGraph6.Annotations(0).Caption = cwGraph6.GetPlotAt(e.X, e.Y).Tag
            Dim xy As New XYPointAnnotation
            xy = CWGraph6.Annotations(0)
            xy.XPosition = CWGraph6.PointToVirtual(e.Location).X * (CWGraph6.XAxes(0).Range.Maximum - CWGraph6.XAxes(0).Range.Minimum) + CWGraph6.XAxes(0).Range.Minimum
            xy.YPosition = CWGraph6.PointToVirtual(e.Location).Y * (CWGraph6.YAxes(0).Range.Maximum - CWGraph6.YAxes(0).Range.Minimum) + CWGraph6.YAxes(0).Range.Minimum
            annotationcapx = (CWGraph6.XAxes(0).Range.Maximum - CWGraph6.XAxes(0).Range.Minimum) / 2 + CWGraph6.XAxes(0).Range.Minimum
            annotationcapy = (CWGraph6.YAxes(0).Range.Maximum - CWGraph6.YAxes(0).Range.Minimum) / 2 + CWGraph6.YAxes(0).Range.Minimum
            xy.SetCaptionPosition(annotationcapx, annotationcapy)
            CWGraph6.Annotations(0).Caption = data(Math.Round(xy.XPosition), 1)
            TextBox1.Text = Math.Round(xy.XPosition)
            TextBox2.Text = Math.Round(xy.YPosition)

            CWGraph6.Annotations(0).Visible = True
            If e.Button = Windows.Forms.MouseButtons.Right Then
                CWGraph6.GetPlotAt(e.X, e.Y).Visible = False
            End If
        End If
    End Sub

    Private Sub CWGraph6_PlotAreaMouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles CWGraph6.PlotAreaMouseMove
        If CWGraph6.Cursors.Item(0).Visible = True Then
            CWGraph6.Cursors.Item(0).XPosition = CWGraph6.PointToVirtual(e.Location).X * (CWGraph6.XAxes(0).Range.Maximum - CWGraph6.XAxes(0).Range.Minimum) + CWGraph6.XAxes(0).Range.Minimum
            CWGraph6.Cursors.Item(0).YPosition = CWGraph6.PointToVirtual(e.Location).Y * (CWGraph6.YAxes(0).Range.Maximum - CWGraph6.YAxes(0).Range.Minimum) + CWGraph6.YAxes(0).Range.Minimum
            'TextBox1.Text = DateAdd("s", cwGraph5.Cursors.Item(0).XPosition, "0001-01-01 00:00:00")
            TextBox1.Text = Str(CWGraph6.Cursors.Item(0).XPosition)
            TextBox2.Text = Str(CWGraph6.Cursors.Item(0).YPosition)
        End If
    End Sub


    Private Sub cwGraph2_PlotDataChanged(ByVal sender As System.Object, ByVal e As NationalInstruments.UI.XYPlotDataChangedEventArgs) Handles cwGraph2.PlotDataChanged

    End Sub

    Private Sub ToolStripButton1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripButton1.Click
        If Dialog1.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then

            updatefilterlist()

        End If
    End Sub
End Class
