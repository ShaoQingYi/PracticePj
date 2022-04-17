Imports System.IO
Public Class Log
    Private Shared _log_path As String

    Public Shared Property LogFile_path As String
        Get
            Return _log_path
        End Get
        Set(value As String)
            _log_path = value
        End Set
    End Property

    Public Shared Sub log(strLog As String)
        Dim dt As Date = Now
        'Console.WriteLine("[" & dt.ToLocalTime & "]:" & strLog)
        Logic.Print("[" & Format(Now(), "yyyy/MM/dd H:mm:ss ffff") & "]:" & strLog)
    End Sub
    Class Logic
        Public Shared Sub Print(txt)
            Dim nowtime = ""
            Using logObject As StreamWriter = File.AppendText(_log_path)
                logObject.WriteLine(nowtime & txt)
                logObject.Close()
            End Using
        End Sub
    End Class
End Class