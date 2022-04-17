Imports System.Text
Imports System.IO
Imports PracticePj.Form1

''' <summary>
''' CSV出力
''' </summary>
''' <remarks></remarks>
Public Class Csv

    Public Shared Encoding As Encoding = Encoding.GetEncoding("UTF-8")

#Region "Write"
    ''' <summary>
    ''' 保存DataTable到CSV文件
    ''' </summary>
    ''' <param name="Path">CSV文件路径</param>
    ''' <param name="Table">要保存的DataTable</param>
    ''' <remarks></remarks>
    Public Shared Sub Save(ByVal Path As String, ByVal Table As DataTable)
        Log.log(System.Reflection.MethodBase.GetCurrentMethod.ReflectedType.Name &
                "." &
                System.Reflection.MethodBase.GetCurrentMethod.Name &
                logDivStart)

        Try
            Using sw As New StreamWriter(Path, False, Encoding)
                sw.Write(CsvString(Table)) : sw.Flush() : sw.Close()
            End Using
        Catch ex As Exception
            Log.log(ex.ToString)
        End Try

        Log.log(System.Reflection.MethodBase.GetCurrentMethod.ReflectedType.Name &
        "." &
        System.Reflection.MethodBase.GetCurrentMethod.Name &
        logDivEnd)
    End Sub
#End Region

#Region "Utils"

    Private Shared Function CsvString(ByVal Table As DataTable) As StringBuilder
        Dim str As New StringBuilder
        If Table.Columns.Count = 0 Then Return str
        For Each c As DataColumn In Table.Columns
            str.Append(CellString(c.ColumnName) & ",")
        Next
        str.Remove(str.Length - 1, 1).Append(vbCrLf)
        For Each r As DataRow In Table.Rows
            str.AppendLine(RowString(r))
        Next
        Return str
    End Function

    Private Shared Function RowString(ByVal Row As DataRow) As String
        Dim str As New StringBuilder
        For Each s In Row.ItemArray
            str.Append(CellString(s.ToString) & ",")
        Next
        Return str.Remove(str.Length - 1, 1).ToString
    End Function

    ''' <summary>
    ''' CSV字符串转换
    ''' </summary>
    ''' <param name="s">要转换的字符串</param>
    ''' <returns>返回转换后的字符串</returns>
    ''' <remarks></remarks>
    Public Shared Function CellString(ByVal s As String) As String
        If String.IsNullOrWhiteSpace(s) Then Return String.Empty
        Dim str As New StringBuilder(s)
        If IsAddChr34(s) Then
            str.Replace(Chr(34), Chr(34) & Chr(34))
            str.Insert(0, Chr(34))
            str.Append(Chr(34))
        End If
        Return str.ToString
    End Function

    Private Shared Function IsAddChr34(ByVal s As String) As Boolean
        Return s.IndexOf(Chr(10)) <> -1 OrElse s.IndexOf(Chr(13)) <> -1 _
        OrElse s.IndexOf(Chr(34)) <> -1 OrElse s.IndexOf(Chr(44)) <> -1
    End Function

#End Region

End Class

