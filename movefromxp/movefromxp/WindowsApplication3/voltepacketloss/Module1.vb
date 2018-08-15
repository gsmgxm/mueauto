Option Strict Off
Option Explicit On
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
        'If Dir(FileName) = "" Then MsgBox(FileName & " not found.", MsgBoxStyle.Critical, "File Not Found") : Exit Function
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
End Module