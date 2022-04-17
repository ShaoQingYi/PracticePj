Public Class Utils
    ''' <summary>
    ''' 清空msg
    ''' </summary>
    Public Shared Sub clearMsg(msgLabel As Label)
        msgLabel.Text = ""
    End Sub

    ''' <summary>
    ''' 一览是否选中一条
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function isSelectOne(DataGridView1 As DataGridView) As Boolean
        Dim isOk As Boolean = False

        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            If DataGridView1.Rows(i).Cells(0).Value Then
                isOk = True
            End If
        Next

        Return isOk
    End Function

    ''' <summary>
    ''' 判断目录是否存在
    ''' </summary>
    ''' <param name="Str_Path"></param>
    ''' <returns></returns>
    Public Shared Function isDirExist(ByVal Str_Path As String) As Boolean
        isDirExist = System.IO.Directory.Exists(Str_Path)
    End Function

    ''' <summary>
    ''' 判断文件是否存在
    ''' </summary>
    ''' <param name="Str_File"></param>
    ''' <returns></returns>
    Public Shared Function isFileExist(ByVal Str_File As String) As Boolean
        isFileExist = System.IO.File.Exists(Str_File)
    End Function
End Class
