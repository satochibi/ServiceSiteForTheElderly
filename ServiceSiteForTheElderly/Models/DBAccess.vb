
Public Class DBAccess

    ' http://sairoutine.hatenablog.com/entry/2014/02/17/121504

    Private _connection As IDbConnection

    Public Sub New()

        Dim connectionString = ConfigurationManager.ConnectionStrings("LocalDB").ConnectionString
        Connection = New SqlClient.SqlConnection(connectionString)
    End Sub

    Public Property Connection() As IDbConnection
        Get
            Return _connection
        End Get
        Set(ByVal value As IDbConnection)
            _connection = value
        End Set
    End Property

    Public Property Transaction() As IDbTransaction

    Protected Sub OpenConnection()

        If (Not _connection Is Nothing) AndAlso (Not _connection.State = ConnectionState.Open) Then
            _connection.Open()
        End If

    End Sub

    Protected Sub CloseConnection()

        If (Not _connection Is Nothing) Then
            _connection.Close()
        End If

    End Sub



    Public Function Execute(sql As String) As Integer

        Dim rtn As Integer = 0
        'Dim transaction As IDbTransaction = Nothing


        OpenConnection()
        Try
            Dim cmd = _connection.CreateCommand()

            cmd.CommandText = sql

            rtn = cmd.ExecuteNonQuery()

        Catch ex As Exception
            rtn = -1
        Finally

        End Try

        Return rtn
    End Function

    Public Sub Query(sql As String, ByRef dt As DataTable)
        'If sql = "" Then sql = "select * from M_BUKA"

        Dim ds As DataSet = New DataSet()
        'Dim dt As DataTable = New DataTable

        Dim dbFactory As Common.DbProviderFactory = Common.DbProviderFactories.GetFactory(CType(_connection, Common.DbConnection))
        Dim dbAdapter As IDbDataAdapter = dbFactory.CreateDataAdapter()

        Dim dbCommand As IDbCommand = _connection.CreateCommand()
        dbCommand.CommandText = sql
        dbCommand.CommandType = CommandType.Text
        dbAdapter.SelectCommand = dbCommand
        dbAdapter.Fill(ds)

        dt = ds.Tables(0)

        ' var connection = new SqlConnection( dbContext.Database.Connection.ConnectionString);
        ' var bulk_copy = new SqlBulkCopy( (SqlConnection)dbContext.Database.Connection );
    End Sub

End Class
