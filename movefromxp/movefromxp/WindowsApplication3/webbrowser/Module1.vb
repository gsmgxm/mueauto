﻿

Option Strict Off
Option Explicit On
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading
Imports System.Runtime.InteropServices
Imports System

Imports System.Text
Module Module1
    'Attribute VB_Name = "Module1"
    '**********************************************
    'ini AllOperation .bas
    '
    '**********************************************
    '-------------------------------------------------'-------------------------------------------------'
    '                     .ini file all operation bas
    '-------------------------------------------------'-------------------------------------------------'
    'Format of .ini file
    '----------------------------------------------------
    '[Section 1]
    'Key 1=Key1Value
    '[Section 2]
    'Key 1=Key1Value
    'Key 2=Key2Value
    'Key 3=Key3Value
    'Key 4=Key4Value
    'Key 5=Key5Value
    '[Section 3]
    'Key 1=Key1Value
    'Key 2=Key2Value
    'Key 3=Key3Value
    '----------------------------------------------------
    'Function list
    '------------------------------------------------------------------------------------------------
    '    Name            Parameter
    '    ReadKeyVal        FileName,    Section,    Key
    '    WriteKeyVal        FileName,    Section,    Key,        KeyValue
    '    DeleteSection        FileName,    Section
    '    DeleteKey        FileName,    Section,    Key
    '    DeleteKeyValue        FileName,    Section,    Key
    '    TotalSections        FileName
    '    TotalKeys        FileName
    '    NumKeys            FileName,    Section
    '    RenameSection        FileName,    Section,    NewSectionName
    '    RenameKey        FileName,    Section,    KeyName,    NewKeyName
    '    GetKey            FileName,    Section,    KeyIndexNum,
    '    GetKey2            FileName,    SectionIndexNum,KeyIndexNum
    '    GetSection        FileName,    SectionIndexNum
    '    IsKey            TextLine
    '    IsSection        TextLine
    '    KeyExists        FileName,    Section,    Key
    '    KeyExists2        FileName,    SectionIndexNum,Key
    '    SectionExists        FileName,    Section
    '    GetSectionIndex        FileName,    Section
    '    GetKeyIndex        FileName,    Section,    Key
    '    GetKeyIndex2        FileName,    SectionIndexNum,Key
    '------------------------------------------------------------------------------------------------

    'APIs to access INI files and retrieve data
    'UPGRADE_ISSUE: Declaring a parameter 'As Any' is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"'
    Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As String, ByVal nSize As Integer, ByVal lpFileName As String) As Integer
    'UPGRADE_ISSUE: Declaring a parameter 'As Any' is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"'
    'UPGRADE_ISSUE: Declaring a parameter 'As Any' is not supported. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"'
    Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Integer
    '
    '------ Base part for operations ( use api access method ) ------
    '------ Read or Write ------
    Function ReadKeyVal(ByVal FileName As String, ByVal Section As String, ByVal Key As String) As String
        'Returns info from an INI file
        Dim RetVal As String
        Dim Worked As Short
        Dim GetKeyVal As String
        ReadKeyVal = ""
        GetKeyVal = ""
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If Dir(FileName) = "" Then MsgBox(FileName & " not found.", MsgBoxStyle.Critical, "File Not Found") : Exit Function
        RetVal = New String(Chr(0), 255)
        Worked = GetPrivateProfileString(Section, Key, "", RetVal, Len(RetVal), FileName)
        ReadKeyVal = IIf(Worked = 0, "", Left(RetVal, InStr(RetVal, Chr(0)) - 1))
    End Function
    '------
    Function WriteKeyVal(ByVal FileName As String, ByVal Section As String, ByVal Key As String, ByVal KeyValue As String) As Integer
        'Add info to an INI file
        'Function returns non 0 if successful and 0 if unsuccessful
        WriteKeyVal = 0
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If Dir(FileName) = "" Then MsgBox(FileName & " not found.", MsgBoxStyle.Critical, "File Not Found") : Exit Function
        WriteKeyVal = WritePrivateProfileString(Section, Key, KeyValue, FileName)
    End Function
    '---------------------------
    '------ Delete Section or Key or KeyValue ------
    Function DeleteSection(ByVal FileName As String, ByVal Section As String) As Integer
        'Delete an entire section and all it's keys from a given INI file
        'Function returns non 0 if successful and 0 if unsuccessful
        DeleteSection = 0
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If Dir(FileName) = "" Then MsgBox(FileName & " not found.", MsgBoxStyle.Critical, "File Not Found") : Exit Function
        If Not SectionExists(FileName, Section) Then MsgBox("Section, " & Section & ", Not Found. ~(DeleteSection)" & vbCrLf & "Verify spelling and capitilization is correct.  Case-sensative.", MsgBoxStyle.Information, "Section Not Found.") : Exit Function
        DeleteSection = WritePrivateProfileString(Section, vbNullString, vbNullString, FileName)
    End Function
    '------
    Function DeleteKey(ByVal FileName As String, ByVal Section As String, ByVal Key As String) As Integer
        'Delete a key from an INI file
        'Function returns non 0 if successful and 0 if unsuccessful
        DeleteKey = 0
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If Dir(FileName) = "" Then MsgBox(FileName & " not found.", MsgBoxStyle.Critical, "File Not Found") : Exit Function
        If Not SectionExists(FileName, Section) Then MsgBox("Section, " & Section & ", Not Found. ~(DeleteKey)" & vbCrLf & "Verify spelling and capitilization is correct.  Case-sensative.", MsgBoxStyle.Information, "Section Not Found.") : Exit Function
        If Not KeyExists(FileName, Section, Key) Then MsgBox("Key, " & Key & ", Not Found. ~(DeleteKey)" & vbCrLf & "Verify spelling and capitilization is correct.  Case-sensative.", MsgBoxStyle.Information, "Key Not Found.") : Exit Function
        DeleteKey = WritePrivateProfileString(Section, Key, vbNullString, FileName)
    End Function
    '------
    Function DeleteKeyValue(ByVal FileName As String, ByVal Section As String, ByVal Key As String) As Short
        'Delete a key's value from an INI file
        'Function returns no 0 if successful and 0 if unsuccessful
        DeleteKeyValue = 0
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If Dir(FileName) = "" Then MsgBox(FileName & " not found.", MsgBoxStyle.Critical, "File Not Found") : Exit Function
        If Not SectionExists(FileName, Section) Then MsgBox("Section, " & Section & ", Not Found. ~(DeleteKeyValue)" & vbCrLf & "Verify spelling and capitilization is correct.  Case-sensative.", MsgBoxStyle.Information, "Section Not Found.") : Exit Function
        If Not KeyExists(FileName, Section, Key) Then MsgBox("Key, " & Key & ", Not Found. ~(DeleteKeyValue)" & vbCrLf & "Verify spelling and capitilization is correct.  Case-sensative.", MsgBoxStyle.Information, "Key Not Found.") : Exit Function
        DeleteKeyValue = WritePrivateProfileString(Section, Key, "", FileName)
    End Function
    '---------------------------------------------------------------------------------------------------------------------------
    '------ Extend part for other operations ( Only use sequence file access method ) ------
    '------ Total Sections or Keys ------
    Public Function TotalSections(ByVal FileName As String) As Integer
        'Returns the total number of sections in a given INI file
        Dim Counter As Integer
        Dim InputData As String
        Dim fNum As Byte
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If Dir(FileName) = "" Then MsgBox(FileName & " not found.", MsgBoxStyle.Critical, "File Not Found") : Exit Function
        Counter = 0
        fNum = FreeFile
        FileOpen(fNum, FileName, OpenMode.Input)
        Do While Not EOF(fNum)
            InputData = LineInput(fNum)
            If IsSection(InputData) Then Counter = Counter + 1
        Loop
        FileClose(fNum)
        TotalSections = Counter
    End Function
    '------
    Public Function TotalKeys(ByVal FileName As String) As Integer
        'Returns the total number of keys in a given INI file
        Dim Counter As Integer
        Dim InputData As String

        Dim fNum As Byte
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If Dir(FileName) = "" Then MsgBox(FileName & " not found.", MsgBoxStyle.Critical, "File Not Found") : Exit Function
        Counter = 0
        fNum = FreeFile
        FileOpen(fNum, FileName, OpenMode.Input)
        Do While Not EOF(fNum)
            InputData = LineInput(fNum)
            If IsKey(InputData) Then Counter = Counter + 1
        Loop
        FileClose(fNum)
        TotalKeys = Counter
    End Function
    '---------------------------------------------------------------------------------------------------------------------------
    '------ NumKeys in Section ------
    Public Function NumKeys(ByVal FileName As String, ByVal Section As String) As Integer
        'Returns the total number of keys in 1 given section.
        Dim Counter As Integer
        Dim InputData As String
        Dim InZone As Boolean
        Dim fNum As Byte
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If Dir(FileName) = "" Then MsgBox(FileName & " not found.", MsgBoxStyle.Critical, "File Not Found") : Exit Function
        If Not SectionExists(FileName, Section) Then MsgBox("Section, " & Section & ", Not Found. ~(NumKeys)" & vbCrLf & "Verify spelling and capitilization is correct.  Case-sensative.", MsgBoxStyle.Information, "Section Not Found.") : Exit Function
        InZone = False
        Counter = 0
        fNum = FreeFile
        FileOpen(fNum, FileName, OpenMode.Input)
        Do While Not EOF(fNum)
            InputData = LineInput(fNum)
            If InZone Then
                If IsSection(InputData) Or EOF(fNum) Then
                    If EOF(fNum) Then
                        NumKeys = Counter + 1
                    Else
                        NumKeys = Counter
                    End If
                    Exit Do
                Else
                    If IsKey(InputData) Then Counter = Counter + 1
                End If
            Else
                If InputData = "[" & Section & "]" Then
                    InZone = True
                End If
            End If
        Loop
        FileClose(fNum)
    End Function
    '---------------------------------------------------------------------------------------------------------------------------
    '------ Rename Section or Key
    Public Function RenameSection(ByVal FileName As String, ByVal SectionName As String, ByVal NewSectionName As String) As Boolean
        'Renames a section in a given INI file.
        'Function returns true if successful and false if unsuccessful
        Dim TopKeys As String
        Dim BotKeys As String

        Dim InputData As String
        Dim InZone As Boolean

        Dim fNum1, fNum2 As Byte
        BotKeys = ""
        TopKeys = ""

        RenameSection = False 'unsuccessful
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If Dir(FileName) = "" Then MsgBox(FileName & " not found.", MsgBoxStyle.Critical, "File Not Found") : Exit Function
        If Not SectionExists(FileName, SectionName) Then MsgBox("Section, " & SectionName & ", Not Found. ~(RenameSection)" & vbCrLf & "Verify spelling and capitilization is correct.  Case-sensative.", MsgBoxStyle.Information, "Section Not Found.") : RenameSection = 0 : Exit Function
        'UPGRADE_WARNING: Couldn't resolve default property of object SectionExists(FileName, NewSectionName). Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        If SectionExists(FileName, NewSectionName) Then MsgBox(NewSectionName & " allready exists.  ~(RenameSection)", MsgBoxStyle.Information, "Duplicate Section") : RenameSection = 0 : Exit Function
        InZone = False
        fNum1 = FreeFile
        FileOpen(fNum1, FileName, OpenMode.Input)
        Do While Not EOF(fNum1)
            InputData = LineInput(fNum1)
            If InZone Then
                If Not EOF(fNum1) Then
                    'If BotKeys = "" Then BotKeys = InputData Else BotKeys = BotKeys & vbCrLf & InputData
                    BotKeys = IIf(BotKeys = "", InputData, BotKeys & vbCrLf & InputData) 'keep bot data
                Else 'file old eof then process
                    FileClose(fNum1)
                    Kill(FileName)
                    'Recreate ini file whicn is the section has renamed
                    fNum2 = FreeFile
                    FileOpen(fNum2, FileName, OpenMode.Append)
                    If TopKeys <> "" Then PrintLine(fNum2, TopKeys) 'Write top data
                    PrintLine(fNum2, "[" & NewSectionName & "]" & vbCrLf & BotKeys) 'Write name and bot data
                    FileClose(fNum2)
                    RenameSection = True 'successful
                    Exit Function
                End If
            Else
                If InputData = "[" & SectionName & "]" Then 'compare sectionname
                    InZone = True 'sectionname correct flag = true
                Else
                    'If TopKeys = "" Then TopKeys = InputData Else TopKeys = TopKeys & vbCrLf & InputData
                    TopKeys = IIf(TopKeys = "", InputData, TopKeys & vbCrLf & InputData) 'keep top data
                End If
            End If
        Loop
        FileClose(fNum1)
    End Function
    '------
    Public Function RenameKey(ByVal FileName As String, ByVal Section As String, ByVal KeyName As String, ByVal NewKeyName As String) As Boolean
        'Renames a key in a given INI file
        'Function returns 1 if successful and 0 if unsuccessful
        Dim KeyVal As String
        RenameKey = False
        'err process
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If Dir(FileName) = "" Then MsgBox(FileName & " not found.", MsgBoxStyle.Critical, "File Not Found") : RenameKey = 0 : Exit Function
        If Not SectionExists(FileName, Section) Then MsgBox("Section, " & Section & ", Not Found. ~(RenameKey)" & vbCrLf & "Verify spelling and capitilization is correct.  Case-sensative.", MsgBoxStyle.Information, "Section Not Found.") : RenameKey = 0 : Exit Function
        If Not KeyExists(FileName, Section, KeyName) Then MsgBox("Key, " & KeyName & ", Not Found. ~(RenameKey)" & vbCrLf & "Verify spelling and capitilization is correct.  Case-sensative.", MsgBoxStyle.Information, "Key Not Found.") : RenameKey = 0 : Exit Function
        If KeyExists(FileName, Section, NewKeyName) Then MsgBox(NewKeyName & " allready exists in the section, " & Section & ".  ~(RenameKey)", MsgBoxStyle.Information, "Duplicate Key.") : RenameKey = 0 : Exit Function
        KeyVal = ReadKeyVal(FileName, Section, KeyName)
        If DeleteKey(FileName, Section, KeyName) = 0 Then Exit Function
        If WriteKeyVal(FileName, Section, NewKeyName, KeyVal) = 0 Then Exit Function
        RenameKey = True
    End Function
    '---------------------------------------------------------------------------------------------------------------------------
    '------ Get Key or Section
    Public Function GetKey(ByVal FileName As String, ByVal Section As String, ByVal KeyIndexNum As Short) As String
        'This function returns the name of a key which is identified by it's IndexNumber.
        'The Section is identified as Text - GetKey2 identifies Section by it's IndexNumber
        'IndexNumbers begin at 0 and increment up
        Dim Counter As Short
        Dim InputData, KeyName As String
        Dim InZone As Boolean
        Dim Looper As Short
        Dim fNum As Byte
        KeyName = ""
        GetKey = ""
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If Dir(FileName) = "" Then MsgBox(FileName & " not found.", MsgBoxStyle.Critical, "File Not Found") : Exit Function
        If Not SectionExists(FileName, Section) Then MsgBox("Section, " & Section & ", Not Found. ~(GetKey)" & vbCrLf & "Verify spelling and capitilization is correct.  Case-sensative.", MsgBoxStyle.Information, "Section Not Found.") : Exit Function
        If NumKeys(FileName, Section) - 1 < KeyIndexNum Then MsgBox(KeyIndexNum & ", not a valid Key Index Number. ~(GetKey)", MsgBoxStyle.Information, "Invalid Index Number.") : Exit Function
        'init
        Counter = -1
        fNum = FreeFile
        FileOpen(fNum, FileName, OpenMode.Input)
        Do While Not EOF(fNum)
            InputData = LineInput(fNum)
            If InZone Then
                If IsKey(InputData) Then 'the current data is the key
                    Counter = Counter + 1
                    If Counter = KeyIndexNum Then 'find the correct keyindexnum
                        For Looper = 1 To Len(InputData)
                            If Mid(InputData, Looper, 1) = "=" Then
                                GetKey = KeyName 'return keyname
                                Exit Do
                            Else
                                KeyName = KeyName & Mid(InputData, Looper, 1) 'keep "=" left data
                            End If
                        Next Looper
                    End If
                End If
            Else
                If InputData = "[" & Section & "]" Then InZone = True 'find the correct section
            End If
        Loop
        FileClose(fNum)
    End Function
    '------
    Public Function GetKey2(ByVal FileName As String, ByVal SectionIndexNum As Short, ByVal KeyIndexNum As Short) As String
        'This function returns the name of a key which is identified by it's IndexNumber.
        'The Section is identified by it's IndexNumber
        'IndexNumbers begin at 0 and increment up
        Dim Counter As Short
        Dim Counter2 As Short
        Dim InputData, KeyName As String
        Dim InZone As Boolean
        Dim Looper As Short
        Dim fNum As Byte
        KeyName = ""
        GetKey2 = ""
        'error process
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If Dir(FileName) = "" Then MsgBox(FileName & " not found.", MsgBoxStyle.Critical, "File Not Found") : Exit Function
        If TotalSections(FileName) - 1 < SectionIndexNum Then MsgBox(SectionIndexNum & ", not a valid Section Index Number. ~(GetKey2)", MsgBoxStyle.Information, "Invalid Index Number.") : Exit Function
        If NumKeys(FileName, GetSection(FileName, SectionIndexNum)) - 1 < KeyIndexNum Then MsgBox(KeyIndexNum & ", not a valid Key Index Number. ~(GetKey2)", MsgBoxStyle.Information, "Invalid Index Number.") : Exit Function
        'init
        Counter = -1
        Counter2 = -1
        fNum = FreeFile
        FileOpen(fNum, FileName, OpenMode.Input)
        Do While Not EOF(fNum)
            InputData = LineInput(fNum)
            If InZone Then 'the secrtionindexnum is correct
                If IsKey(InputData) Then 'the inputdata is the key
                    Counter = Counter + 1
                    If Counter = KeyIndexNum Then 'the index is the kenindexnum
                        For Looper = 1 To Len(InputData)
                            If Mid(InputData, Looper, 1) = "=" Then
                                GetKey2 = KeyName 'return keyname
                                Exit Do
                            Else
                                KeyName = KeyName & Mid(InputData, Looper, 1) 'keep "=" left data
                            End If
                        Next Looper
                    End If
                End If
            Else
                If IsSection(InputData) Then 'current inputdata is the section
                    Counter2 = Counter2 + 1
                    If Counter2 = SectionIndexNum Then InZone = True 'find the correct sectionindexnum
                End If
            End If
        Loop
        FileClose(fNum)
    End Function
    '------
    Public Function GetSection(ByVal FileName As String, ByVal SectionIndexNum As Short) As String
        'Returns a section name which is identified by it's indexnumber
        'IndexNumbers begin at 0 and increment up
        Dim InputData As String
        Dim Counter As Short
        Dim fNum As Byte
        GetSection = ""
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If Dir(FileName) = "" Then MsgBox(FileName & " not found.", MsgBoxStyle.Critical, "File Not Found") : Exit Function
        If TotalSections(FileName) - 1 < SectionIndexNum Then MsgBox(SectionIndexNum & ", not a valid Section Index Number. ~(GetSection)", MsgBoxStyle.Information, "Invalid Index Number.") : Exit Function
        Counter = -1
        fNum = FreeFile
        FileOpen(fNum, FileName, OpenMode.Input)
        Do While Not EOF(fNum)
            InputData = LineInput(fNum)
            If IsSection(InputData) Then 'the inputdata is  section
                Counter = Counter + 1
                'InputData = Right(InputData, Len(InputData) - 1)
                'InputData = Left(InputData, Len(InputData) - 1)
                InputData = Mid(InputData, 2, Len(InputData) - 2)
                If Counter = SectionIndexNum Then GetSection = InputData : Exit Do
            End If
        Loop
        FileClose(fNum)
    End Function
    '---------------------------------------------------------------------------------------------------------------------------
    '------ Is Key or Section------
    Private Function IsKey(ByVal TextLine As String) As Boolean
        'This function determines whether or not a line of text is a valid Key (ex. "This=key")
        'This returns True or False

        IsKey = False
        If TextLine = "" Then Exit Function
        'For Looper = 1 To Len(TextLine)
        '    If Mid(TextLine, Looper, 1) = "=" Then IsKey = True: Exit For
        'Next Looper
        If InStr(TextLine, "=") <> 0 Then IsKey = True
    End Function
    '------
    Private Function IsSection(ByVal TextLine As String) As Boolean
        'This function determines whether or not a line of text is a
        'valid section (ex. "[section]")
        'This return's True or False
        Dim FirstChar, LastChar As String
        IsSection = False
        If TextLine = "" Then Exit Function
        FirstChar = Mid(TextLine, 1, 1)
        LastChar = Mid(TextLine, Len(TextLine), 1)
        If FirstChar = "[" And LastChar = "]" Then IsSection = True
    End Function
    '---------------------------------------------------------------------------------------------------------------------------
    '------ Key or Section Exist -----
    Public Function KeyExists(ByVal FileName As String, ByVal Section As String, ByVal Key As String) As Boolean
        'This function determines if a key exists in a given section
        'The Section is identified as Text - KeyExists2 identifies Section by its IndexNumber
        'This returns True or False
        Dim InZone As Boolean
        Dim InputData As String

        Dim fNum As Byte
        KeyExists = False
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If Dir(FileName) = "" Then MsgBox(FileName & " not found.", MsgBoxStyle.Critical, "File Not Found") : Exit Function
        If Not SectionExists(FileName, Section) Then MsgBox("Section, " & Section & ", Not Found. ~(KeyExists)" & vbCrLf & "Verify spelling and capitilization is correct.  Case-sensative.", MsgBoxStyle.Information, "Section Not Found.") : Exit Function
        fNum = FreeFile
        InZone = False
        FileOpen(fNum, FileName, OpenMode.Input)
        Do While Not EOF(fNum)
            InputData = LineInput(fNum)
            'find the correct section ?
            If InZone Then
                If IsKey(InputData) Then 'the inputdata is the key
                    If Left(InputData, Len(Key)) = Key Then 'the inputdata is the key
                        KeyExists = True 'return true
                        Exit Do
                    End If
                ElseIf IsSection(InputData) Then  'the inputdata is the section
                    Exit Do
                End If
            Else
                If InputData = "[" & Section & "]" Then InZone = True 'find
            End If
        Loop
        FileClose(fNum)
    End Function
    '------
    Public Function KeyExists2(ByVal FileName As String, ByVal SectionIndexNum As Short, ByVal Key As String) As Boolean
        'This function determines if a key exists in a given section
        'The Section is identified by its IndexNumber
        'IndexNumbers begin at 0 and increment up
        'This returns True or False
        Dim InZone As Boolean
        Dim InputData As String
        Dim Counter As Short
        Dim fNum As Byte
        KeyExists2 = False
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If Dir(FileName) = "" Then MsgBox(FileName & " not found.", MsgBoxStyle.Critical, "File Not Found") : Exit Function
        If TotalSections(FileName) - 1 < SectionIndexNum Then MsgBox(SectionIndexNum & ", not a valid Section Index Number. ~(KeyExists2)", MsgBoxStyle.Information, "Invalid Index Number.") : Exit Function
        fNum = FreeFile
        Counter = -1
        FileOpen(fNum, FileName, OpenMode.Input)
        Do While Not EOF(fNum)
            InputData = LineInput(fNum)
            If InZone Then
                If IsKey(InputData) Then
                    If Left(InputData, Len(Key)) = Key Then
                        KeyExists2 = True
                        Exit Do
                    End If
                ElseIf IsSection(InputData) Then
                    Exit Do
                End If
            Else
                If IsSection(InputData) Then
                    Counter = Counter + 1
                    If Counter = SectionIndexNum Then InZone = True
                End If
            End If
        Loop
        FileClose(fNum)
    End Function
    '------
    Public Function SectionExists(ByVal FileName As String, ByVal Section As String) As Object
        'This determines if a section exists in a given INI file
        'This returns True or False
        Dim InputData As String
        Dim fNum As Byte
        'UPGRADE_WARNING: Couldn't resolve default property of object SectionExists. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
        SectionExists = False
        fNum = FreeFile
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If Dir(FileName) = "" Then MsgBox(FileName & " not found.", MsgBoxStyle.Critical, "File Not Found") : Exit Function
        FileOpen(fNum, FileName, OpenMode.Input)
        Do While Not EOF(fNum)
            InputData = LineInput(fNum)
            If "[" & Section & "]" = InputData Then
                'UPGRADE_WARNING: Couldn't resolve default property of object SectionExists. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"'
                SectionExists = True : Exit Do
            End If
        Loop
        FileClose(fNum)
    End Function
    '---------------------------------------------------------------------------------------------------------------------------
    '------ Get Section or Key index ------
    Public Function GetSectionIndex(ByVal FileName As String, ByVal Section As String) As Short
        'This function is used to get the IndexNumber for a given Section
        Dim InputData As String
        Dim Counter As Short
        Dim fNum As Byte
        GetSectionIndex = -1
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If Dir(FileName) = "" Then MsgBox(FileName & " not found.", MsgBoxStyle.Critical, "File Not Found") : Exit Function
        If Not SectionExists(FileName, Section) Then MsgBox("Section, " & Section & ", Not Found. ~(GetSectionIndex)" & vbCrLf & "Verify spelling and capitilization is correct.  Case-sensative.", MsgBoxStyle.Information, "Section Not Found.") : Exit Function
        Counter = -1
        fNum = FreeFile
        FileOpen(fNum, FileName, OpenMode.Input)
        Do While Not EOF(fNum)
            InputData = LineInput(fNum)
            If IsSection(InputData) Then
                Counter = Counter + 1
                If "[" & Section & "]" = InputData Then GetSectionIndex = Counter
            End If
        Loop
        FileClose(fNum)
    End Function
    '------
    Public Function GetKeyIndex(ByVal FileName As String, ByVal Section As String, ByVal Key As String) As Short
        'This function returns the IndexNumber of a key in a given Section
        'The Section is identified as Text - GetKeyIndex2, Section is
        'identified by it's IndexNumber
        'IndexNumbers start at 0 and increment up
        Dim InputData As String
        Dim InZone As Boolean
        Dim Counter As Short
        Dim fNum As Byte
        GetKeyIndex = -1
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If Dir(FileName) = "" Then MsgBox(FileName & " not found.", MsgBoxStyle.Critical, "File Not Found") : Exit Function
        If Not SectionExists(FileName, Section) Then MsgBox("Section, " & Section & ", Not Found. ~(GetKeyIndex)" & vbCrLf & "Verify spelling and capitilization is correct.  Case-sensative.", MsgBoxStyle.Information, "Section Not Found.") : Exit Function
        If Not KeyExists(FileName, Section, Key) Then MsgBox("Key, " & Key & ", Not Found. ~(GetKetIndex)" & vbCrLf & "Verify spelling and capitilization is correct.  Case-sensative.", MsgBoxStyle.Information, "Key Not Found.") : Exit Function
        Counter = -1
        fNum = FreeFile
        FileOpen(fNum, FileName, OpenMode.Input)
        Do While Not EOF(fNum)
            InputData = LineInput(fNum)
            If InZone Then
                If IsKey(InputData) Then
                    Counter = Counter + 1
                    If Left(InputData, Len(Key)) = Key Then
                        GetKeyIndex = Counter
                        Exit Do
                    End If
                ElseIf IsSection(InputData) Then
                    Exit Do
                End If
            Else
                If "[" & Section & "]" = InputData Then InZone = True
            End If
        Loop
        FileClose(fNum)
    End Function
    '------
    Public Function GetKeyIndex2(ByVal FileName As String, ByVal SectionIndexNum As Short, ByVal Key As String) As Short
        'This function returns the IndexNumber of a key in a given Section
        'The Section is identified by it's IndexNumber
        'IndexNumbers start at 0 and increment up
        Dim InputData As String
        Dim Counter As Short
        Dim Counter2 As Short
        Dim InZone As Boolean
        Dim fNum As Byte
        GetKeyIndex2 = -1
        'UPGRADE_WARNING: Dir has a new behavior. Click for more: 'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"'
        If Dir(FileName) = "" Then MsgBox(FileName & " not found.", MsgBoxStyle.Critical, "File Not Found") : Exit Function
        If TotalSections(FileName) - 1 < SectionIndexNum Then MsgBox(SectionIndexNum & ", not a valid Section Index Number. ~(GetKeyIndex2)", MsgBoxStyle.Information, "Invalid Index Number.") : Exit Function
        If Not KeyExists(FileName, GetSection(FileName, SectionIndexNum), Key) Then MsgBox("Key, " & Key & ", Not Found. ~(GetKetIndex2)" & vbCrLf & "Verify spelling and capitilization is correct.  Case-sensative.", MsgBoxStyle.Information, "Key Not Found.") : Exit Function
        Counter = -1
        Counter2 = -1
        fNum = FreeFile
        FileOpen(fNum, FileName, OpenMode.Input)
        Do While Not EOF(fNum)
            InputData = LineInput(fNum)
            If InZone Then
                If IsKey(InputData) Then
                    Counter = Counter + 1
                    If Left(InputData, Len(Key)) = Key Then
                        GetKeyIndex2 = Counter
                        Exit Do
                    End If
                ElseIf IsSection(InputData) Then
                    Exit Do
                End If
            Else
                If IsSection(InputData) Then
                    Counter2 = Counter2 + 1
                    If Counter2 = SectionIndexNum Then InZone = True
                End If
            End If
        Loop
        FileClose(fNum)
    End Function
    '**********************************************************************************************************  
    '''''' 类名：TCPServer    
    '''''' 说明：监听主线程，用于监听客户端联接，并记录客户端联接，接收和发送数据  
    '''''' 与客户端的联接采用TCP联接  
    '**********************************************************************************************************  


    ''' <summary>  
    ''' 侦听客户端联接  
    ''' </summary>  
    Public Class TCPServer

#Region "私有成员"
        Private _LocationListenSocket As Socket '本地侦听服务  
        Private _ListenPort As String '服务器侦听客户端联接的端口  
        Private _MaxClient As Integer '最大客户端连接数  
        Private _Clients As New SortedList '客户端队列  
        Private _ListenThread As Thread = Nothing '侦听线程  
        Private _ServerStart As Boolean = False '服务器是否已经启动  
        Private _RecvMax As Integer '接收缓冲区大小   
#End Region

#Region "事件"
        ''' <summary>  
        ''' 客户端联接事件  
        ''' </summary>  
        ''' <param name="IP">客户端联接IP</param>  
        ''' <param name="Port">客户端联接端口号</param>  
        ''' <remarks></remarks>  
        Public Event ClientConnected(ByVal IP As String, ByVal Port As String)
        ''' <summary>  
        ''' 客户端断开事件  
        ''' </summary>  
        ''' <param name="IP">客户端联接IP</param>  
        ''' <param name="Port">客户端联接端口号</param>  
        ''' <remarks></remarks>  
        Public Event ClientClose(ByVal IP As String, ByVal Port As String)
        ''' <summary>  
        ''' 接收到客户端的数据  
        ''' </summary>  
        ''' <param name="value">数据</param>  
        ''' <param name="IPAddress">数据来源IP</param>  
        ''' <param name="Port">数据来源端口</param>  
        ''' <remarks></remarks>  
        Public Event DataArrived(ByVal value As Byte(), ByVal Len As Integer, ByVal IPAddress As String, ByVal Port As String)
        ''' <summary>  
        ''' 异常数据  
        ''' </summary>  
        ''' <param name="ex"></param>  
        ''' <remarks></remarks>  
        Public Event Exception(ByVal ex As Exception)

#End Region

#Region "属性"
        ''' <summary>  
        ''' 侦听服务是否已经启动  
        ''' </summary>  
        ''' <value></value>  
        ''' <returns></returns>  
        ''' <remarks></remarks>  
        Public ReadOnly Property IsServerStart() As Boolean
            Get
                Return _ServerStart
            End Get
        End Property
#End Region

#Region "方法"
        ''' <summary>  
        ''' 实例　TCPServer  
        ''' </summary>  
        ''' <param name="Port">侦听客户端联接的端口号</param>  
        ''' <param name="MaxClient">最大可以联接的客户端数量</param>  
        ''' <param name="RecvMax">接收缓冲区大小</param>  
        ''' <param name="RecvSleep">接收线程睡眠时间</param>  
        ''' <remarks></remarks>  
        Sub New(ByVal Port As String, ByVal MaxClient As Integer, ByVal RecvMax As Integer, ByVal RecvSleep As Integer)
            Try
                Dim strHostName As String = Dns.GetHostName()
                _ListenPort = Port
                _MaxClient = MaxClient
                _RecvMax = RecvMax
                Dim strServerHost As New IPEndPoint(IPAddress.Any, Int32.Parse(_ListenPort))
                '建立TCP侦听  
                _LocationListenSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                _LocationListenSocket.Bind(strServerHost)
                _LocationListenSocket.Listen(_MaxClient)
                _LocationListenSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.AcceptConnection, 1)
            Catch ex As Exception
                RaiseEvent Exception(ex)
            End Try
        End Sub

        ''' <summary>  
        ''' 开始侦听服务  
        ''' </summary>  
        ''' <remarks></remarks>  
        Public Sub StartServer()
            _ServerStart = True
            Try
                _ListenThread = New Thread(New ThreadStart(AddressOf ListenClient))
                _ListenThread.Name = "监听客户端主线程"
                _ListenThread.Start()
            Catch ex As Exception
                If (Not _LocationListenSocket Is Nothing) Then
                    If _LocationListenSocket.Connected Then
                        _LocationListenSocket.Close()
                    End If
                End If
                RaiseEvent Exception(ex)
            End Try
        End Sub

        ''' <summary>  
        ''' 关闭侦听  
        ''' </summary>  
        ''' <remarks></remarks>  
        Public Sub Close()
            Try
                _ServerStart = False
                'CloseAllClient()  
                Thread.Sleep(5)
                _ListenThread.Abort()
                _LocationListenSocket.Close()
                _ListenThread = Nothing
            Catch ex As Exception
                RaiseEvent Exception(ex)
            End Try
        End Sub

        ''' <summary>  
        ''' 客户端侦听线程  
        ''' </summary>  
        ''' <remarks></remarks>  
        Private Sub ListenClient()
            Dim sKey As String
            While (_ServerStart)
                Try
                    If Not _LocationListenSocket Is Nothing Then
                        Dim clientSocket As System.Net.Sockets.Socket
                        clientSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                        clientSocket = _LocationListenSocket.Accept()
                        If Not clientSocket Is Nothing Then
                            Dim clientInfoT As IPEndPoint = CType(clientSocket.RemoteEndPoint, IPEndPoint)
                            sKey = clientInfoT.Address.ToString & "\&" & clientInfoT.Port.ToString
                            _Clients.Add(sKey, clientSocket)
                            RaiseEvent ClientConnected(clientInfoT.Address.ToString, clientInfoT.Port.ToString) '举起有客户端联接的事件  
                            '启动客户端接收主线程，开始侦听并接收客户端上传的数据  
                            Dim lb As New ClientCommunication(_LocationListenSocket, clientSocket, Me)
                            AddHandler lb.Exception, AddressOf WriteErrorEvent_ClientCommunication
                            Dim thrClient As New Thread(New ThreadStart(AddressOf lb.serverThreadProc))
                            thrClient.Name = "客户端接收线程,客户端" & clientInfoT.Address.ToString & ":" & clientInfoT.Port.ToString
                            thrClient.Start()
                        End If
                    End If
                Catch ex As Exception
                    RaiseEvent Exception(ex)
                End Try
            End While
        End Sub

        Private Sub WriteErrorEvent_ClientCommunication(ByVal ex As Exception)
            RaiseEvent Exception(ex)
        End Sub

        Public Sub CloseClient(ByVal IP As String, ByVal Port As String)
            GetClientSocket(IP, Port).Close()
            GetClientClose(IP, Port)
        End Sub
        'Public Sub AlertNoticeClientAll(ByVal DepartmentName As String, ByVal LineName As String, ByVal ErrorCode As Integer)  
        '    '#DepartmentName,LineName,AlertCodeValue.  
        '    ' ''Dim mStr As String  
        '    ' ''mStr = "#" & DepartmentName & "," & LineName & "," & ErrorCode  
        '    ' ''Dim SendByte() As Byte = System.Text.UTF8Encoding.Default.GetBytes(mStr)  
        '    ' ''For Each sc As System.Net.Sockets.Socket In _ClientComputers.Values  
        '    ' ''    sc.Send(SendByte, SendByte.Length(), SocketFlags.None)  
        '    ' ''Next  
        'End Sub  
        Public Sub CloseAllClient()
            For Each sc As System.Net.Sockets.Socket In _Clients.Values
                '断开所有工作站的Socket连接。  
                Dim clientInfoT As IPEndPoint = CType(sc.RemoteEndPoint, IPEndPoint)
                CloseClient(clientInfoT.Address.ToString, clientInfoT.Port.ToString)
            Next
        End Sub
#Region "接收客户端的数据"

        ''' <summary>  
        ''' 接收到客户端的数据-字节数组  
        ''' </summary>  
        ''' <param name="value">数据内容</param>  
        ''' <param name="Len">字节长度</param>  
        ''' <param name="IPAddress">发送该数据的IP地址</param>  
        ''' <param name="Port">发送该数据的端口号</param>  
        ''' <remarks></remarks>  
        Private Sub GetData_Byte(ByVal value As Byte(), ByVal Len As Integer, ByVal IPAddress As String, ByVal Port As String)
            Try
                RaiseEvent DataArrived(value, Len, IPAddress, Port)
                'Catch exx As Sockets.SocketException  
                '    CloseClient(IPAddress, Port)  
            Catch ex As Exception
                RaiseEvent Exception(ex)
            End Try
        End Sub

        ''' <summary>  
        ''' 得到客户端断开或失去客户端联连事件  
        ''' </summary>  
        ''' <param name="IP">客户端联接IP</param>  
        ''' <param name="Port">客户端联接端口号</param>  
        ''' <remarks></remarks>  
        Private Sub GetClientClose(ByVal IP As String, ByVal Port As String)
            Try
                If _Clients.ContainsKey(IP & "\&" & Port) Then
                    SyncLock _Clients.SyncRoot
                        '_Clients.Item(IP & "\&" & Port)  
                        _Clients.Remove(IP & "\&" & Port)
                    End SyncLock
                End If
                RaiseEvent ClientClose(IP, Port)
            Catch ex As Exception
                RaiseEvent Exception(ex)
            End Try
        End Sub
#End Region


#Region "向客户端发送数据"

        ''' <summary>  
        ''' 向客户端发送信息  
        ''' </summary>  
        ''' <param name="value">发送的内容</param>  
        ''' <param name="IPAddress">IP地址</param>  
        ''' <param name="Port">端口号</param>  
        ''' <returns> Boolean</returns>  
        ''' <remarks></remarks>  
        Public Function SendData(ByVal value As Byte(), ByVal IPAddress As String, ByVal Port As String) As Boolean
            Try
                Dim clientSocket As System.Net.Sockets.Socket
                clientSocket = _Clients.Item(IPAddress & "\&" & Port)
                clientSocket.Send(value, value.Length, SocketFlags.None)
                Return True
            Catch ex As Exception
                RaiseEvent Exception(ex)
                Return False
            End Try
        End Function
        Public Function SendFile(ByVal value As String, ByVal IPAddress As String, ByVal Port As String) As Boolean
            Try
                Dim clientSocket As System.Net.Sockets.Socket
                clientSocket = _Clients.Item(IPAddress & "\&" & Port)
                clientSocket.SendFile(value)
                Return True
            Catch ex As Exception
                RaiseEvent Exception(ex)
                Return False
            End Try
        End Function
        Public Function SendDataToAllClient(ByVal value As Byte()) As Boolean
            Try
                For Each clientSocket As System.Net.Sockets.Socket In _Clients.Values
                    clientSocket.Send(value, value.Length, SocketFlags.None)
                Next
                Return True
            Catch ex As Exception
                RaiseEvent Exception(ex)
                Return False
            End Try
        End Function

#End Region

        ''' <summary>  
        ''' 得到客户端的Socket联接  
        ''' </summary>  
        ''' <param name="IPAddress">客户端的IP</param>  
        ''' <param name="Port">客户端的端口号</param>  
        ''' <returns>Socket联接</returns>  
        ''' <remarks></remarks>  
        Private Function GetClientSocket(ByVal IPAddress As String, ByVal Port As String) As Socket
            Try
                Dim ClientSocket As Socket
                ClientSocket = _Clients.Item(IPAddress & "\&" & Port)
                Return ClientSocket
            Catch ex As Exception
                RaiseEvent Exception(ex)
                Return Nothing
            End Try
        End Function
#End Region

        Private Class ClientCommunication
            Public Event Exception(ByVal ex As Exception)

            Private ServerSocket As New System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            Private myClientSocket As New System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            Private myParentObject As TCPServer
            Private oldbytes() As Byte
            Private _IPAddress, _Port As String
            Private NclientInfoT As IPEndPoint = Nothing
            Private iLen As Integer
            Private allDone As New ManualResetEvent(False)
            ''' <summary>  
            ''' 实例ClientCommunication类  
            ''' </summary>  
            ''' <param name="ServerSocket"></param>  
            ''' <param name="ClientSocket"></param>  
            ''' <param name="ParentObject"></param>  
            ''' <remarks></remarks>  
            Public Sub New(ByVal ServerSocket As Socket, ByVal ClientSocket As Socket, ByVal ParentObject As TCPServer)
                Me.ServerSocket = ServerSocket
                myClientSocket = ClientSocket
                myParentObject = ParentObject
                NclientInfoT = CType(myClientSocket.RemoteEndPoint, IPEndPoint)
                _IPAddress = NclientInfoT.Address.ToString
                _Port = NclientInfoT.Port.ToString
            End Sub

            ''' <summary>  
            ''' 客户端通讯主线程  
            ''' </summary>  
            ''' <remarks></remarks>  
            Public Sub serverThreadProc()
                Try
                    Dim sb As New SocketAndBuffer
                    sb.Socket = myClientSocket
                    sb.Socket.BeginReceive(sb.Buffer, 0, sb.Buffer.Length, SocketFlags.None, AddressOf ReceiveCallBack, sb)
                    'allDone.WaitOne()  
                Catch ex As Exception
                    RaiseEvent Exception(ex)
                End Try
            End Sub

            ''' <summary>  
            ''' socket异步接收回调函数  
            ''' </summary>  
            ''' <param name="ar"></param>  
            ''' <remarks></remarks>  
            Private Sub ReceiveCallBack(ByVal ar As IAsyncResult)
                Dim sb As SocketAndBuffer
                allDone.Set()
                sb = CType(ar.AsyncState, SocketAndBuffer)
                Try
                    If sb.Socket.Connected Then
                        iLen = sb.Socket.EndReceive(ar)
                        If iLen > 0 Then
                            ReDim oldbytes(iLen - 1)
                            Array.Copy(sb.Buffer, 0, oldbytes, 0, iLen)
                            myParentObject.GetData_Byte(oldbytes, oldbytes.Length, _IPAddress, _Port)
                            sb.Socket.BeginReceive(sb.Buffer, 0, sb.Buffer.Length, SocketFlags.None, AddressOf ReceiveCallBack, sb)
                        Else
                            If (Not myClientSocket Is Nothing) Then
                                If myClientSocket.Connected Then
                                    myClientSocket.Close()
                                Else
                                    myClientSocket.Close()
                                End If
                                myClientSocket = Nothing
                                If Not NclientInfoT Is Nothing Then
                                    myParentObject._Clients.Remove(_IPAddress & "\&" & _Port)
                                    myParentObject.GetClientClose(_IPAddress, _Port)
                                End If
                            End If
                        End If
                    End If
                Catch ex As Exception
                    If (Not myClientSocket Is Nothing) Then
                        If myClientSocket.Connected Then
                            myClientSocket.Close()
                        Else
                            myClientSocket.Close()
                        End If
                        myClientSocket = Nothing
                        If Not NclientInfoT Is Nothing Then
                            myParentObject._Clients.Remove(_IPAddress & "\&" & _Port)
                            myParentObject.GetClientClose(_IPAddress, _Port)
                        End If
                    End If
                    RaiseEvent Exception(ex)
                End Try
            End Sub

            ''' <summary>  
            ''' 异步操作socket缓冲类  
            ''' </summary>  
            ''' <remarks></remarks>  
            Private Class SocketAndBuffer
                Public Socket As System.Net.Sockets.Socket
                Public Buffer(8192) As Byte
            End Class
        End Class


    End Class




    Public Class TCPClient
#Region "私有成员"
        Private _LocationClientSocket As Socket '本地侦听服务  
        Private _LocalPort As String '本地端口  
        Private _LocalHostName As String
        Private _LocalIP As String
        Private autoEvent As AutoResetEvent
        Private _RemoteHostName As String '遠程端計算機名  
        Private _RemoteIP As String     '遠程端計算機IP  
        Private _RemotePort As String   '遠程端計算機Port  
        Private _RemoteIPOrHostName As String
        'Private _MaxClient As Integer '最大客户端连接数  
        'Private _Clients As New SortedList '客户端队列  
        'Private _ListenThread As Thread = Nothing '侦听线程  
        'Private _ServerStart As Boolean = False '服务器是否已经启动  
        Private _RecvMax As Integer '接收缓冲区大小   

        Private ClientThread As Thread
        'Private ClitenStream As NetworkStream  
        Private IsStop As Boolean = False

#End Region

#Region "事件"
        ''' <summary>  
        ''' 客户端联接事件  
        ''' </summary>   
        ''' <remarks></remarks>  
        Public Event ClientConnected()
        ''' <summary>  
        ''' 客户端断开事件  
        ''' </summary>  
        ''' <remarks></remarks>  
        Public Event ClientClosed()
        ''' <summary>  
        ''' 接收到客户端的数据  
        ''' </summary>  
        ''' <param name="value">数据</param>  
        ''' <remarks></remarks>  
        Public Event DataArrived(ByVal value As Byte(), ByVal Len As Integer)
        ''' <summary>  
        ''' 异常数据  
        ''' </summary>  
        ''' <param name="ex"></param>  
        ''' <remarks></remarks>  
        Public Event Exception(ByVal ex As Exception)
#End Region

#Region "属性"
        ''' <summary>  
        ''' 是否已經連接  
        ''' </summary>  
        ''' <value></value>  
        ''' <returns></returns>  
        ''' <remarks></remarks>  
        Public ReadOnly Property Connected() As Boolean
            Get
                If _LocationClientSocket Is Nothing Then
                    Return False
                Else

                    Return _LocationClientSocket.Connected
                End If

            End Get
        End Property

        ''' <summary>  
        ''' 本地計算機名稱  
        ''' </summary>  
        ''' <value></value>  
        ''' <returns></returns>  
        ''' <remarks></remarks>  
        Public ReadOnly Property LocalHostName() As String
            Get
                Return _LocalHostName
            End Get
        End Property

        ''' <summary>  
        ''' 本地計算IP  
        ''' </summary>  
        ''' <value></value>  
        ''' <returns></returns>  
        ''' <remarks></remarks>  
        Public ReadOnly Property LocalIP() As String
            Get
                Return _LocalIP
            End Get
        End Property

        ''' <summary>  
        ''' 本地計算機端口  
        ''' </summary>  
        ''' <value></value>  
        ''' <returns></returns>  
        ''' <remarks></remarks>  
        Public ReadOnly Property LocalPort() As String
            Get
                Return _LocalPort
            End Get
        End Property

        ''' <summary>  
        ''' 遠程計算機IP  
        ''' </summary>  
        ''' <value></value>  
        ''' <returns></returns>  
        ''' <remarks></remarks>  
        Public ReadOnly Property RemoteIP() As String
            Get
                Return _RemoteIP
            End Get

        End Property

        ''' <summary>  
        ''' 遠程計算機端口  
        ''' </summary>  
        ''' <value></value>  
        ''' <returns></returns>  
        ''' <remarks></remarks>  
        Public ReadOnly Property RemotePort() As String
            Get
                Return _RemotePort
            End Get

        End Property

        ''' <summary>  
        ''' 遠程計算機名稱  
        ''' </summary>  
        ''' <value></value>  
        ''' <returns></returns>  
        ''' <remarks></remarks>  
        Public ReadOnly Property RemoteHostName() As String
            Get
                Return _RemoteHostName
            End Get

        End Property
#End Region

#Region "方法"
        ''' <summary>  
        ''' 实例　TCPServer  
        ''' </summary>  
        ''' <param name="RemoteIPOrHostName">需要連接服務的IP地址或計算機名稱</param>  
        ''' <param name="Port">侦听客户端联接的端口号</param>  
        ''' <param name="RecvMax">接收缓冲区大小</param>  
        ''' <param name="RecvSleep">接收线程睡眠时间</param>  
        ''' <remarks></remarks>  
        Sub New(ByVal RemoteIPOrHostName As String, ByVal Port As String, ByVal RecvMax As Integer, ByVal RecvSleep As Integer)
            Try
                _LocalHostName = Dns.GetHostName()

                '_RemoteIP = Dns.GetHostAddresses(RemoteIPOrHostName)(0).ToString  
                _RemotePort = Port
                _RecvMax = RecvMax
                _RemoteIPOrHostName = RemoteIPOrHostName
                _RemotePort = Port
                'Dim strServerHost As New IPEndPoint(IPAddress.Any, Int32.Parse(_ListenPort))  

                '建立TCP侦听  
                _LocationClientSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                autoEvent = New AutoResetEvent(False)
                Dim cThread As New Thread(New ThreadStart(AddressOf ConnectHost))
                cThread.Start()
                autoEvent.WaitOne(1000, False)
                cThread.Abort()
            Catch ex As Exception
                RaiseEvent Exception(ex)
            End Try
        End Sub
        Public Sub ConnectHost()
            Try
                Dim remoteIP As Net.IPAddress = Nothing
                If IPAddress.TryParse(_RemoteIPOrHostName, remoteIP) Then
                    '_LocationClientSocket.BeginConnect()  
                    _LocationClientSocket.Connect(remoteIP, _RemotePort)
                    '_RemoteIP = RemoteHostName  

                Else
                    _LocationClientSocket.Connect(_RemoteIPOrHostName, _RemotePort)
                    _RemoteHostName = RemoteHostName
                End If

                If _LocationClientSocket.Connected Then
                    _LocationClientSocket.SendBufferSize = _RecvMax
                    _LocationClientSocket.ReceiveBufferSize = _RecvMax

                    Dim clientInfoT As IPEndPoint

                    clientInfoT = CType(_LocationClientSocket.RemoteEndPoint, IPEndPoint)
                    _RemoteIP = clientInfoT.Address.ToString
                    'Dim remoteHost As Net.IPHostEntry  

                    _RemoteHostName = Dns.GetHostEntry(_RemoteIP).HostName

                    clientInfoT = CType(_LocationClientSocket.LocalEndPoint, IPEndPoint)

                    _LocalIP = clientInfoT.Address.ToString
                    _LocalPort = clientInfoT.Port.ToString

                    IsStop = False

                    RaiseEvent ClientConnected()

                    ClientThread = New Thread(New ThreadStart(AddressOf ClientListen))
                    ClientThread.Start()
                    autoEvent.Set()
                End If
            Catch ex As Exception

            End Try


        End Sub

        ''' <summary>  
        ''' 關閉客戶端連接  
        ''' </summary>  
        ''' <remarks></remarks>  
        Public Sub Close()
            Try
                If _LocationClientSocket Is Nothing Then Exit Sub
                IsStop = True
                If Not ClientThread Is Nothing Then
                    Thread.Sleep(5)
                    ClientThread.Abort()
                End If
                _LocationClientSocket.Close()
                _LocationClientSocket = Nothing
                ClientThread = Nothing
                RaiseEvent ClientClosed()
            Catch ex As Exception
                RaiseEvent Exception(ex)
            End Try

        End Sub

        ''' <summary>  
        ''' 实例　TCPServer  
        ''' </summary>  
        ''' <param name="value">發送的資料,二進制數組</param>  
        ''' <remarks></remarks>  
        Public Function SendData(ByVal value As Byte()) As Boolean
            Try
                _LocationClientSocket.Send(value)
            Catch ex As Exception
                RaiseEvent Exception(ex)
            End Try
        End Function

        Private Sub ClientListen()
            Dim tmpByt(8192) As Byte
            Dim recData() As Byte
            Dim R As Integer
            While Not IsStop
                Try
                    If _LocationClientSocket.Poll(50, SelectMode.SelectWrite) Then
                        R = _LocationClientSocket.Receive(tmpByt)
                        If R > 0 Then

                            ReDim recData(R - 1)
                            Array.Copy(tmpByt, recData, R)
                            RaiseEvent DataArrived(recData, recData.Length)
                        Else
                            If (Not _LocationClientSocket Is Nothing) Then
                                _LocationClientSocket.Close()
                                _LocationClientSocket = Nothing
                                IsStop = True
                                RaiseEvent ClientClosed()
                            End If
                        End If
                    End If
                Catch sex As SocketException
                    If sex.ErrorCode = 10054 Then
                        If (Not _LocationClientSocket Is Nothing) Then
                            _LocationClientSocket.Close()
                            _LocationClientSocket = Nothing
                            IsStop = True
                            RaiseEvent ClientClosed()
                        End If
                    Else
                        RaiseEvent Exception(sex)
                    End If
                Catch ex As Exception
                    RaiseEvent Exception(ex)
                End Try
            End While
        End Sub
#End Region

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
        <DllImport("User32.dll", EntryPoint:="ShowWindow")> _
        Friend Shared Function ShowWindow(ByVal hWnd As Integer, ByVal nCmdShow As Integer) As Integer
        End Function

        Private Const GWL_STYLE = (-16)
        Private Const WS_CAPTION = &HC00000
        Private Const WS_THICKFRAME = &H40000
        Private Const GWL_EXSTYLE = -20
        Private Const WS_MINIMIZE = &H20000000
        Private Const SW_SHOWMINIMIZED = 2

        Public Shared Sub setconsoleminize()
            Dim conHandler As Integer = ConsoleHelper.FindWindowEx(0, 0, "ConsoleWindowClass", Console.Title)
            ' Dim y As Long
            ShowWindow(conHandler, SW_SHOWMINIMIZED)
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

    Public Class myprocess
        Public Shared Sub killprocessbyimportfile(ByVal filename As String)
            Dim itemAdd As String()
            Dim proc As Process
            Dim processes() As Process
            itemAdd = Nothing
            readfile(itemAdd, filename)
            Try
                For i As Integer = 0 To UBound(itemAdd)
                    If itemAdd(i) <> "" Then
                        processes = Process.GetProcessesByName(itemAdd(i))
                        For Each proc In processes
                            Console.WriteLine("kill process:" + proc.ProcessName + " process id:" + proc.Id.ToString)
                            proc.Kill()
                            proc.WaitForExit()
                        Next
                    End If
                Next
            Catch
            End Try
        End Sub

        Public Shared Sub killprocess(ByVal processname As String)
            Dim processes() As Process
            Try
                processes = Process.GetProcessesByName(processname)
                For Each proc In processes
                    Console.WriteLine("kill process:" + proc.ProcessName + " process id:" + proc.Id.ToString)
                    proc.Kill()
                    proc.WaitForExit()
                Next
            Catch
            End Try
        End Sub
        Public Shared Sub readfile(ByRef processname As String(), ByVal filename As String)
            Try
                Dim file As New System.IO.StreamReader(filename)
                Dim words As String = file.ReadToEnd()
                processname = Split(words, vbNewLine)
                file.Close()
            Catch
            End Try
        End Sub


    End Class





    ' State object for receiving data from remote device.

   





End Module

