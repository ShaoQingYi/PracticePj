Imports System.Data.SQLite
Imports PracticePj.Utils
Imports PracticePj.Log
Imports System.IO

Public Class Form1
    Dim conn As New SQLiteConnection
    Dim sqlcmd As New SQLiteCommand
    Dim sqlreader As SQLiteDataReader

    Dim strCsvDirPath As String
    Dim strLogDirPath As String

    Dim testForGithub As String

    Public Shared logDivStart As String = "  ===Start==="
    Public Shared logDivEnd As String = "  ===End==="

    Private Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpDefault As String, ByVal lpReturnedString As String, ByVal nSize As Int32, ByVal lpFileName As String) As Int32
    Private Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" (ByVal lpApplicationName As String, ByVal lpKeyName As String, ByVal lpString As String, ByVal lpFileName As String) As Int32
    '定义读取配置文件函数
    Public Function GetINI(ByVal Section As String, ByVal AppName As String, ByVal lpDefault As String, ByVal FileName As String) As String
        Dim Str As String = LSet(Str, 256)
        GetPrivateProfileString(Section, AppName, lpDefault, Str, Len(Str), FileName)
        Return Microsoft.VisualBasic.Left(Str, InStr(Str, Chr(0)) - 1)
    End Function
    '定义写入配置文件函数
    Public Function WriteINI(ByVal Section As String, ByVal AppName As String, ByVal lpDefault As String, ByVal FileName As String) As Long
        WriteINI = WritePrivateProfileString(Section, AppName, lpDefault, FileName)
    End Function

    ''' <summary>
    ''' 初始化窗体
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' 加载配置文件
        Dim path As String
        path = Application.StartupPath + "\server.ini"
        strCsvDirPath = GetINI("Server", "csv_path", "", path)
        strLogDirPath = GetINI("Server", "log_path", "", path)

        Dim strDate As String = Format(Now(), "yyyy/MM/dd")
        strDate = strDate.Replace("/", "")

        ' logFilePath
        LogFile_path = strLogDirPath & strDate & ".log"

        Try
            ' logDir路径是否存在
            If Not isDirExist(strLogDirPath) Then
                Directory.CreateDirectory(strLogDirPath)
            End If

            ' logFile文件是否存在
            If Not isFileExist(LogFile_path) Then
                File.Create(LogFile_path).Dispose()
            End If

            ' csvDir路径是否存在
            If Not isDirExist(strCsvDirPath) Then
                Directory.CreateDirectory(strCsvDirPath)
            End If
        Catch ex As Exception
            MsgBox(ex.ToString)
            onErrorOccur()
            Return
        End Try

        Log.log(System.Reflection.MethodBase.GetCurrentMethod.ReflectedType.Name &
                "." &
                System.Reflection.MethodBase.GetCurrentMethod.Name &
                logDivStart)

        ' 初始化spread
        initSpread()

        Try
            Dim s As String = System.Windows.Forms.Application.StartupPath + "myFirstSQLiteDb.db"
            conn.ConnectionString = "Data Source=" & s
            conn.Open()
            sqlcmd.Connection = conn
        Catch ex As Exception
            Log.log(ex.ToString)
            msgLabel.Text = "Error occurs, please refer to the log file!"
            msgLabel.ForeColor = Color.Red
            onErrorOccur()
        End Try

        ' 检索所有数据
        getAllInfo()

        Log.log(System.Reflection.MethodBase.GetCurrentMethod.ReflectedType.Name &
                "." &
                System.Reflection.MethodBase.GetCurrentMethod.Name &
                logDivEnd)
    End Sub

    ''' <summary>
    ''' 初始化grid
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub initSpread()
        Log.log(System.Reflection.MethodBase.GetCurrentMethod.ReflectedType.Name &
                "." &
                System.Reflection.MethodBase.GetCurrentMethod.Name &
                logDivStart)

        Dim checkBoxCol As DataGridViewCheckBoxColumn
        Dim textBoxCol_fieldKey As DataGridViewTextBoxColumn
        Dim textBoxCol_field1 As DataGridViewTextBoxColumn
        Dim textBoxCol_field2 As DataGridViewTextBoxColumn
        Dim textBoxCol_field1Hiden As DataGridViewTextBoxColumn
        Dim textBoxCol_field2Hiden As DataGridViewTextBoxColumn

        checkBoxCol = New DataGridViewCheckBoxColumn()
        checkBoxCol.Name = ""
        checkBoxCol.HeaderText = ""
        checkBoxCol.Width = 25
        DataGridView1.Columns.Add(checkBoxCol)

        textBoxCol_fieldKey = New DataGridViewTextBoxColumn()
        textBoxCol_fieldKey.HeaderText = "key"
        DataGridView1.Columns.Add(textBoxCol_fieldKey)
        textBoxCol_fieldKey.ReadOnly = True

        textBoxCol_field1 = New DataGridViewTextBoxColumn()
        textBoxCol_field1.HeaderText = "field1"
        DataGridView1.Columns.Add(textBoxCol_field1)

        textBoxCol_field2 = New DataGridViewTextBoxColumn()
        textBoxCol_field2.HeaderText = "field2"
        DataGridView1.Columns.Add(textBoxCol_field2)

        textBoxCol_field1Hiden = New DataGridViewTextBoxColumn()
        textBoxCol_field1Hiden.HeaderText = "field1Hiden"
        DataGridView1.Columns.Add(textBoxCol_field1Hiden)
        textBoxCol_field1Hiden.Visible = False

        textBoxCol_field2Hiden = New DataGridViewTextBoxColumn()
        textBoxCol_field2Hiden.HeaderText = "field2Hiden"
        DataGridView1.Columns.Add(textBoxCol_field2Hiden)
        textBoxCol_field2Hiden.Visible = False

        Log.log(System.Reflection.MethodBase.GetCurrentMethod.ReflectedType.Name &
                "." &
                System.Reflection.MethodBase.GetCurrentMethod.Name &
                logDivEnd)
    End Sub

    ''' <summary>
    ''' 获取DB中所有数据，显示到一览中
    ''' </summary>
    Private Sub getAllInfo()
        Log.log(System.Reflection.MethodBase.GetCurrentMethod.ReflectedType.Name &
                "." &
                System.Reflection.MethodBase.GetCurrentMethod.Name &
                logDivStart)

        DataGridView1.Rows.Clear()

        Try
            ' csv出力用
            Dim dt As DataTable

            sqlcmd.CommandText = "SELECT * FROM table1 order by table1.fieldKey"
            sqlreader = sqlcmd.ExecuteReader

            ' csv出力用
            dt = CreateTableBySchemaTable(sqlreader.GetSchemaTable())
            ' csv出力用
            Dim values(dt.Columns.Count - 1) As Object

            Dim rowIndex As Int32 = 0

            While (sqlreader.Read())
                DataGridView1.Rows.Add()

                DataGridView1.Rows(rowIndex).Cells(1).Value = sqlreader.GetValue("fieldKey")
                DataGridView1.Rows(rowIndex).Cells(2).Value = sqlreader.GetValue("field1")
                DataGridView1.Rows(rowIndex).Cells(3).Value = sqlreader.GetValue("field2")

                ' hiden 更新用
                DataGridView1.Rows(rowIndex).Cells(4).Value = sqlreader.GetValue("field1")
                DataGridView1.Rows(rowIndex).Cells(5).Value = sqlreader.GetValue("field2")

                rowIndex = rowIndex + 1

                sqlreader.GetValues(values)
                dt.LoadDataRow(values, True)
            End While

            sqlreader.Close()

            'csv出力
            Dim strDate As String = Format(Now(), "yyyy/MM/dd H:mm:ss ffff")
            strDate = strDate.Replace("/", "")
            strDate = strDate.Replace(":", "")
            strDate = strDate.Replace(" ", "")

            ' logFilePath
            Dim strCsvFilePath As String
            strCsvFilePath = strCsvDirPath & strDate & ".csv"
            File.Create(strCsvFilePath).Dispose()

            Csv.Save(strCsvFilePath, dt)
        Catch ex As Exception
            Log.log(ex.ToString)
            msgLabel.Text = "Error occurs, please refer to the log file!"
            msgLabel.ForeColor = Color.Red
            onErrorOccur()
        End Try

        Log.log(System.Reflection.MethodBase.GetCurrentMethod.ReflectedType.Name &
        "." &
        System.Reflection.MethodBase.GetCurrentMethod.Name &
        logDivEnd)
    End Sub

    ''' <summary>
    ''' insert事件
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Insert_Click(sender As Object, e As EventArgs) Handles Insert.Click
        Log.log(System.Reflection.MethodBase.GetCurrentMethod.ReflectedType.Name &
                "." &
                System.Reflection.MethodBase.GetCurrentMethod.Name &
                logDivStart)

        clearMsg(msgLabel)

        If checkForInsert() Then
            Dim strSql As String

            Try
                strSql = String.Format("insert into table1 values ( {0}, '{1}', '{2}')", Key.Text, field2.Text, field3.Text)

                sqlcmd.CommandText = strSql
                sqlcmd.ExecuteNonQuery()
            Catch ex As Exception
                Log.log(ex.ToString)
                msgLabel.Text = "Error occurs, please refer to the log file!"
                msgLabel.ForeColor = Color.Red
                onErrorOccur()
            End Try

            postInsert()
        End If

        Log.log(System.Reflection.MethodBase.GetCurrentMethod.ReflectedType.Name &
        "." &
        System.Reflection.MethodBase.GetCurrentMethod.Name &
        logDivEnd)
    End Sub

    ''' <summary>
    ''' insert前check
    ''' </summary>
    ''' <returns></returns>
    Private Function checkForInsert() As Boolean
        Log.log(System.Reflection.MethodBase.GetCurrentMethod.ReflectedType.Name &
                "." &
                System.Reflection.MethodBase.GetCurrentMethod.Name &
                logDivStart)

        Dim isOk As Boolean = True

        ' key必须入力
        If Key.Text = "" Then
            msgLabel.Text = "Please enter key!"
            msgLabel.ForeColor = Color.Red
            isOk = False
            Return isOk
        End If

        Try
            ' 主key存在check
            sqlcmd.CommandText = String.Format("select * from table1 where table1.fieldKey = {0}", Key.Text)
            sqlreader = sqlcmd.ExecuteReader

            If sqlreader.HasRows Then
                msgLabel.Text = "Key already exists!"
                msgLabel.ForeColor = Color.Red
                isOk = False
            End If

            sqlreader.Close()
        Catch ex As Exception
            Log.log(ex.ToString)
            msgLabel.Text = "Error occurs, please refer to the log file!"
            msgLabel.ForeColor = Color.Red
            onErrorOccur()
        End Try

        Log.log(System.Reflection.MethodBase.GetCurrentMethod.ReflectedType.Name &
                "." &
                System.Reflection.MethodBase.GetCurrentMethod.Name &
                logDivEnd)

        Return isOk
    End Function

    ''' <summary>
    ''' insert后处理
    ''' </summary>
    ''' <returns></returns>
    Private Sub postInsert()
        ' 再检索
        getAllInfo()

        ' 更新msg
        msgLabel.Text = "Insert successful!"
        msgLabel.ForeColor = Color.Blue
    End Sub

    ''' <summary>
    ''' update事件
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Update_Click(sender As Object, e As EventArgs) Handles Update.Click
        Log.log(System.Reflection.MethodBase.GetCurrentMethod.ReflectedType.Name &
                "." &
                System.Reflection.MethodBase.GetCurrentMethod.Name &
                logDivStart)

        clearMsg(msgLabel)

        If checkForUpdate() Then
            Try
                For i As Integer = 0 To DataGridView1.Rows.Count - 1
                    ' 更新对象判断（一览中被选中，并且field2和field3变更）
                    If DataGridView1.Rows(i).Cells(0).Value And
                   (DataGridView1.Rows(i).Cells(2).Value <> DataGridView1.Rows(i).Cells(4).Value Or
                   DataGridView1.Rows(i).Cells(3).Value <> DataGridView1.Rows(i).Cells(5).Value) Then
                        Dim strSql As String

                        strSql = String.Format("update table1 set (field1, field2) = ('{1}', '{2}') where table1.fieldKey = {0}",
                                           DataGridView1.Rows(i).Cells(1).Value,
                                           DataGridView1.Rows(i).Cells(2).Value,
                                           DataGridView1.Rows(i).Cells(3).Value)

                        sqlcmd.CommandText = strSql
                        sqlcmd.ExecuteNonQuery()
                    End If
                Next
            Catch ex As Exception
                Log.log(ex.ToString)
                msgLabel.Text = "Error occurs, please refer to the log file!"
                msgLabel.ForeColor = Color.Red
                onErrorOccur()
            End Try

            postUpdate()
        End If

        Log.log(System.Reflection.MethodBase.GetCurrentMethod.ReflectedType.Name &
        "." &
        System.Reflection.MethodBase.GetCurrentMethod.Name &
        logDivEnd)
    End Sub

    ''' <summary>
    ''' update前check
    ''' </summary>
    ''' <returns></returns>
    Private Function checkForUpdate() As Boolean
        Dim isOk As Boolean = True

        ' 一览中必须选中一条
        If Not isSelectOne(DataGridView1) Then
            msgLabel.Text = "Please select at least one piece of data for Update!"
            msgLabel.ForeColor = Color.Red
            isOk = False
        End If

        Return isOk
    End Function

    ''' <summary>
    ''' update后处理
    ''' </summary>
    ''' <returns></returns>
    Private Sub postUpdate()
        ' 再检索
        getAllInfo()

        ' 更新msg
        msgLabel.Text = "Update successful!"
        msgLabel.ForeColor = Color.Blue
    End Sub

    ''' <summary>
    ''' delete事件
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Delete_Click(sender As Object, e As EventArgs) Handles Delete.Click
        Log.log(System.Reflection.MethodBase.GetCurrentMethod.ReflectedType.Name &
                "." &
                System.Reflection.MethodBase.GetCurrentMethod.Name &
                logDivStart)

        clearMsg(msgLabel)

        If checkForDelete() Then
            Try
                For i As Integer = 0 To DataGridView1.Rows.Count - 1
                    ' 删除对象判断（一览中被选中）
                    If DataGridView1.Rows(i).Cells(0).Value Then
                        Dim strSql As String

                        strSql = String.Format("delete from table1 where table1.fieldKey = {0}",
                                           DataGridView1.Rows(i).Cells(1).Value)

                        sqlcmd.CommandText = strSql
                        sqlcmd.ExecuteNonQuery()
                    End If
                Next
            Catch ex As Exception
                Log.log(ex.ToString)
                msgLabel.Text = "Error occurs, please refer to the log file!"
                msgLabel.ForeColor = Color.Red
                onErrorOccur()
            End Try

            postDelete()
        End If

        Log.log(System.Reflection.MethodBase.GetCurrentMethod.ReflectedType.Name &
        "." &
        System.Reflection.MethodBase.GetCurrentMethod.Name &
        logDivEnd)
    End Sub

    ''' <summary>
    ''' delete前check
    ''' </summary>
    ''' <returns></returns>
    Private Function checkForDelete() As Boolean
        Dim isOk As Boolean = True

        ' 一览中必须选中一条
        If Not isSelectOne(DataGridView1) Then
            msgLabel.Text = "Please select at least one piece of data for Delete!"
            msgLabel.ForeColor = Color.Red
            isOk = False
        End If

        Return isOk
    End Function

    ''' <summary>
    ''' delete后处理
    ''' </summary>
    ''' <returns></returns>
    Private Sub postDelete()
        ' 再检索
        getAllInfo()

        ' 更新msg
        msgLabel.Text = "Delete successful!"
        msgLabel.ForeColor = Color.Blue
    End Sub

    ''' <summary>
    ''' error时，禁用所有按钮
    ''' </summary>
    Private Sub onErrorOccur()
        Insert.Enabled = False
        Update.Enabled = False
        Delete.Enabled = False
    End Sub

    Protected Function CreateTableBySchemaTable(pSchemaTable As DataTable) As DataTable
        Dim dtReturn As DataTable
        Dim dc As DataColumn
        Dim dr As DataRow

        dtReturn = New DataTable()

        For i As Integer = 0 To pSchemaTable.Rows.Count - 1
            dr = pSchemaTable.Rows(i)
            dc = New DataColumn(dr("ColumnName").ToString(), dr("DataType"))
            dtReturn.Columns.Add(dc)
        Next

        Return dtReturn
    End Function

End Class
