Imports System.Text.RegularExpressions
Module Module1

    Sub Main()
        Console.WriteLine("c6cf8390:" + checkadbsn("c6cf8390"))
        Console.WriteLine("AT$QCRMCALL=1,1,1,2,1:" + checkadbsn("AT$QCRMCALL=1,1,1,2,1"))
        Console.WriteLine("4.60022E+14:" + checkadbsn("4.60022E+14"))
        Console.ReadLine()


    End Sub
    Function checkadbsn(ByVal input As String) As String
        Dim snfirst As New Regex("(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z]{4,14}")
        Dim commands As New Regex("[a-zA-Z]{4,14}")
        Dim imsi As New Regex("[0-9]{15}")

        If snfirst.IsMatch(input) And Not (commands.IsMatch(input)) And Not (imsi.IsMatch(input)) Then
            Return snfirst.Matches(input)(0).ToString
        Else
            Return ""
        End If

    End Function
End Module
