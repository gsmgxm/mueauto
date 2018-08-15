Imports System
Imports System.IO
Imports System.IO.Ports
Imports System.Runtime.InteropServices
Imports System.Text
Imports Microsoft.Win32.SafeHandles

Namespace SerialPortTester
    Public Class SerialPortFixer  ' IDisposable
        Implements IDisposable
        Public Shared Sub Execute(ByVal portName As String)
            Using New SerialPortFixer(portName)
            End Using
        End Sub
#Region "IDisposable Members"

        Public Sub Dispose() Implements IDisposable.Dispose
            If Not (m_Handle Is Nothing) Then
                m_Handle.Close()
                m_Handle = Nothing
            End If
        End Sub

        Protected Overrides Sub Finalize()

        End Sub

#End Region

#Region "Implementation"

        Private Const DcbFlagAbortOnError As Integer = 14
        Private Const CommStateRetries As Integer = 10
        Private m_Handle As SafeFileHandle


        Private Sub New(ByVal portName As String)
            Const dwFlagsAndAttributes As Integer = &H40000000
            Const dwAccess As Integer = CInt(&HC0000000)

            If (portName Is Nothing) OrElse Not (portName.StartsWith("COM", StringComparison.OrdinalIgnoreCase)) Then Throw New ArgumentException("Invalid Serial Port", "portName")

            Dim hFile As SafeFileHandle = CreateFile("\\.\" + portName, dwAccess, 0, IntPtr.Zero, 3, dwFlagsAndAttributes, _
                                              IntPtr.Zero)
            If (hFile.IsInvalid) Then WinIoError()

            Try
                Dim fileType As Integer = GetFileType(hFile)
                If ((fileType <> 2) And (fileType <> 0)) Then Throw New ArgumentException("Invalid Serial Port", "portName")
                m_Handle = hFile
                InitializeDcb()
            Catch
                hFile.Close()
                m_Handle = Nothing
                Throw
            End Try
        End Sub

        <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
        Private Shared Function FormatMessage(ByVal dwFlags As Integer, ByVal lpSource As HandleRef, ByVal dwMessageId As Integer, ByVal dwLanguageId As Integer, _
                                               ByVal lpBuffer As StringBuilder, ByVal nSize As Integer, ByVal arguments As IntPtr) As Integer
        End Function
        <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
         Private Shared Function GetCommState(ByVal hFile As SafeFileHandle, ByRef lpDcb As Dcb) As Boolean

        End Function
        <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
          Private Shared Function SetCommState(ByVal hFile As SafeFileHandle, ByRef lpDcb As Dcb) As Boolean

        End Function
        <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
          Private Shared Function ClearCommError(ByVal hFile As SafeFileHandle, ByRef lpErrors As Integer, ByRef lpStat As Comstat) As Boolean

        End Function
        <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
            Private Shared Function CreateFile(ByVal lpFileName As String, ByVal dwDesiredAccess As Integer, ByVal dwShareMode As Integer, _
                                                           ByVal securityAttrs As IntPtr, ByVal dwCreationDisposition As Integer, _
                                                           ByVal dwFlagsAndAttributes As Integer, ByVal hTemplateFile As IntPtr) As SafeFileHandle

        End Function
        <DllImport("kernel32.dll", SetLastError:=True)> _
         Private Shared Function GetFileType(ByVal hFile As SafeFileHandle) As Integer

        End Function

        Private Sub InitializeDcb()
            Dim Dcb As Dcb = New Dcb()
            GetCommStateNative(Dcb)
            Dcb.Flags = DcbFlagAbortOnError
            SetCommStateNative(Dcb)
        End Sub

        Private Function GetMessage(ByVal errorCode As Integer) As String
            Dim lpBuffer As StringBuilder = New StringBuilder(&H200)
            If (FormatMessage(&H3200, New HandleRef(Nothing, IntPtr.Zero), errorCode, 0, lpBuffer, lpBuffer.Capacity, _
                              IntPtr.Zero) <> 0) Then
                Return lpBuffer.ToString()
            End If
            Return "Unknown Error"
        End Function

        Private Function MakeHrFromErrorCode(ByVal errorCode As Integer) As Integer
            Return &H80070000 Or errorCode
        End Function

        Private Sub WinIoError()
            Dim errorCode As Integer = Marshal.GetLastWin32Error()
            Throw New IOException(GetMessage(errorCode), MakeHrFromErrorCode(errorCode))
        End Sub

        Private Sub GetCommStateNative(ByRef lpDcb As Dcb)
            Dim commErrors As Integer = 0
            Dim Comstat As Comstat = New Comstat()
            For i As Integer = 0 To CommStateRetries - 1
                If Not (ClearCommError(m_Handle, commErrors, Comstat)) Then WinIoError()
                If (GetCommState(m_Handle, lpDcb)) Then Exit Sub
                If (i = CommStateRetries - 1) Then WinIoError()
            Next
        End Sub

        Private Sub SetCommStateNative(ByRef lpDcb As Dcb)
            Dim commErrors As Integer = 0
            Dim Comstat As Comstat = New Comstat()
            For i As Integer = 0 To CommStateRetries - 1
                If Not (ClearCommError(m_Handle, commErrors, Comstat)) Then WinIoError()
                If (SetCommState(m_Handle, lpDcb)) Then Exit Sub
                If (i = CommStateRetries - 1) Then WinIoError()
            Next
        End Sub

#Region "Nested type: COMSTAT"

        Private Structure Comstat

            Public ReadOnly Flags As UInteger
            Public ReadOnly cbInQue As UInteger
            Public ReadOnly cbOutQue As UInteger

        End Structure
#End Region

#Region "Nested type: DCB"

        Private Structure Dcb

            Public ReadOnly DCBlength As UInteger
            Public ReadOnly BaudRate As UInteger
            Public Flags As UInteger
            Public ReadOnly wReserved As UShort
            Public ReadOnly XonLim As UShort
            Public ReadOnly XoffLim As UShort
            Public ReadOnly ByteSize As Byte
            Public ReadOnly Parity As Byte
            Public ReadOnly StopBits As Byte
            Public ReadOnly XonChar As Byte
            Public ReadOnly XoffChar As Byte
            Public ReadOnly ErrorChar As Byte
            Public ReadOnly EofChar As Byte
            Public ReadOnly EvtChar As Byte
            Public ReadOnly wReserved1 As UShort
        End Structure
#End Region

#End Region
    End Class
End Namespace
