Imports System.Data.SqlClient
Imports GSWEB.Common

Namespace SQL

    Public Class DataSource

#Region "Variables"
        Dim CommandTimeOut As Integer = 3000

        Dim conn As New SqlConnection
        Dim cmd As New SqlCommand
        Dim da As New SqlDataAdapter
        Dim trans As SqlTransaction
        Dim ds As New DataSet
        Dim dt As New DataTable
        Dim trans_name As String = ""

#End Region

#Region "Properties"

        Public ReadOnly Property CommandText As String
            Get
                Return Me.cmd.CommandText
            End Get
        End Property

        Public ReadOnly Property CanConnect As Boolean
            Get
                Disconnect()
                Dim result As Boolean = False
                Try
                    Me.conn.ConnectionString = ConnectionString
                    Me.conn.Open()
                    result = True
                Catch ex As Exception
                End Try
                Return result
            End Get
        End Property

#End Region

#Region "Constructors"

        Public Sub New()
            MyBase.New()
        End Sub

#End Region

#Region "Functions"

        Public Function Connect(Optional trans_name As String = "") As Boolean
            Disconnect()
            Dim result As Boolean = False
            Try
                Me.conn.ConnectionString = ConnectionString
                Me.conn.Open()

                If trans_name <> "" Then
                    Me.trans = Me.conn.BeginTransaction(trans_name)
                    Me.trans_name = trans_name
                End If

                Me.cmd.Connection = conn
                If trans_name <> "" Then
                    Me.cmd.Transaction = Me.trans
                End If
                Me.cmd.CommandTimeout = CommandTimeOut

                result = True
            Catch ex As Exception
                Throw ex
                '    Dim msg As String
                '    Select Case ex.Number
                '        Case 18456, 11001, 4060 ' 18456 Login Failed: 11001 Host Not Found: 4060 Cannot Open Database
                '            msg = "Cannot connect to database server."
                '        Case Else 'Other errors
                '            msg = getSystemMessage(ex.Message)
                '    End Select
                'Catch exAll As Exception
                '    Dim msg As String
                '    msg = getSystemMessage(exAll.Message)
            End Try
            Return result
        End Function

        Public Function ExecuteNonQuery() As Integer
            Dim result As Integer = 0
            Try
                result = Me.cmd.ExecuteNonQuery
                If Me.trans_name <> "" Then
                    Me.trans.Commit()
                End If
            Catch ex As Exception
                If Me.trans_name <> "" Then Me.trans.Rollback()
                Throw ex
            End Try
            Return result
        End Function

        Public Function ExecuteReader() As SqlDataReader
            Dim result As SqlDataReader = Nothing
            Try
                result = Me.cmd.ExecuteReader
                If Me.trans_name <> "" Then
                    Me.trans.Commit()
                End If
            Catch ex As Exception
                If Me.trans_name <> "" Then Me.trans.Rollback()
                Throw ex
            End Try
            Return result
        End Function

        Public Function ExecuteScalar() As Object
            Dim result As Object = Nothing
            Try
                result = Me.cmd.ExecuteScalar
                If Me.trans_name <> "" Then
                    Me.trans.Commit()
                End If
            Catch ex As Exception
                If Me.trans_name <> "" Then Me.trans.Rollback()
                Throw ex
            End Try
            Return result
        End Function

        Public Function FillDataset() As DataSet
            Me.da.SelectCommand = Me.cmd
            Me.da.Fill(Me.ds)
            Return Me.ds
        End Function

        Public Function FillDataTable() As DataTable
            Me.da.SelectCommand = Me.cmd
            Me.da.Fill(Me.dt)
            'If Me.dt.Columns.Contains("ID") Then
            '    Dim dc(1) As DataColumn
            '    dc(0) = Me.dt.Columns("ID")
            '    dc(0).ReadOnly = True
            '    dc(0).AutoIncrement = True
            '    dc(0).AutoIncrementSeed = 0
            '    Me.dt.PrimaryKey = dc
            'End If
            Return Me.dt
        End Function

#End Region

#Region "Subs"

        Public Sub Disconnect()
            Try
                If Me.conn.State = ConnectionState.Open Then
                    Me.conn.Close()
                End If
            Catch ex As Exception
            End Try
        End Sub

        Public Sub SetCommandText(cmdtxt As String)
            If Me.cmd.Parameters.Count > 0 Then
                Me.cmd.Parameters.Clear()
            End If
            Me.cmd.CommandText = Nothing
            Me.cmd.CommandText = cmdtxt
        End Sub

        Public Sub SetCommandType(type As CommandType)
            Me.cmd.CommandType = type
        End Sub

        Public Sub AddParameter(paramName As String, paramValue As Object)
            Me.cmd.Parameters.AddWithValue(paramName, paramValue)
        End Sub

        Private ID As Integer
        Public Property rID As Integer
            Get
                Return ID
            End Get
            Set(value As Integer)
                ID = value
            End Set
        End Property

#End Region

    End Class

End Namespace