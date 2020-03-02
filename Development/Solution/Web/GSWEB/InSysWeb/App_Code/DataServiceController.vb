Imports System.Net
Imports System.Web.Http
Imports System.ServiceModel.Channels
Imports GSWEB
Imports GSWEB.Common
Imports System.Net.Http
Imports System.Data
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports System.Net.Http.Headers
Imports System.IO
Imports System.Threading.Tasks
Imports System.Net.Http.Formatting
Imports System.Linq
Imports Newtonsoft.Json.Schema
Imports System.Data.SqlClient
Imports Ionic
Imports Ionic.Zip
Imports System.Web.Http.Filters
Imports System.Runtime.Serialization.Formatters
Imports Newtonsoft.Json.Converters
Imports System.Runtime.Caching
Imports GSWEB.Utility
Imports System.IO.File
Imports System.Data.OleDb
Imports NPOI.HSSF.UserModel
Imports NPOI.SS.UserModel
Imports NPOI.SS.Util
Imports NPOI.HSSF.Util
Imports NPOI.POIFS.FileSystem
Imports NPOI.HPSF
Imports System.Web.Script.Serialization
Imports Yahoo.Yui.Compressor
Imports Yahoo.Yui
Public Class DataServiceController
    Inherits ApiController

    ' GET api/<controller>
    Public Function GetValues() As IEnumerable(Of String)
        Return New String() {"value1", "value2"}
    End Function

    ' GET api/<controller>/5
    Public Function GetValue(ByVal id As Integer) As String
        Return "value"
    End Function

    ' POST api/<controller>
    Public Sub PostValue(<FromBody()> ByVal value As String)

    End Sub

    ' PUT api/<controller>/5
    Public Sub PutValue(ByVal id As Integer, <FromBody()> ByVal value As String)

    End Sub

    ' DELETE api/<controller>/5
    Public Sub DeleteValue(ByVal id As Integer)

    End Sub

    Public UID As Integer = HttpContext.Current.Session("ID_User")

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Async Function Save() As Task(Of HttpResponseMessage) ' ByVal obj As Newtonsoft.Json.Linq.JObject
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        If Not Request.Content.IsMimeMultipartContent() Then
            Me.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType)
        End If

        Dim root = HttpContext.Current.Server.MapPath("~/Upload/")
        Directory.CreateDirectory(root)
        Dim provider = New MultipartFormDataStreamProvider(root),
            result = Await Request.Content.ReadAsMultipartAsync(provider)
        Dim mID As Integer = result.FormData.Item("mID"),
            rID As Integer = 0,
            btnID As Integer = result.FormData.Item("btnID"),
            drMaster As CaseInsensitiveDictionary = JsonConvert.DeserializeObject(Of CaseInsensitiveDictionary)(result.FormData.Item("Master")),
            ds As DataSet = JsonConvert.DeserializeObject(Of DataSet)(result.FormData.Item("Detail")),
            fileSummary As List(Of fileSummary) = JsonConvert.DeserializeObject(Of List(Of fileSummary))(result.FormData.Item("fileSummary"))
        If result.FormData.Item("rID") <> "0" Then
            Try
                rID = toBase10(result.FormData.Item("rID")) 'CInt(GetDirectReference(result.FormData.Item("rID"), HttpContext.Current.Session))
            Catch ex As Exception
                logError(ex, mID)
                Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
            End Try
        End If
        If Not UserHasAccessByUserGroup(mCollection.GetMenu(mID)) Then Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
        If Not UserHasAccessByData(mCollection.GetMenu(mID), rID, 0) Then Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
        Dim ret As String = String.Empty,
            message As String = String.Empty,
            messageType As MessageType = messageType.Success,
            id As Integer = 0,
            r As New JObject

        Try
            Dim m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(mID)
            Dim dr As DataRow = m.dtButtons.Select("ID = " & btnID)(0)

            ' BEFORE EXECUTE
            messageType = messageType.BeforeExecute
            ExecuteButtonValidation(m, 1, btnID, drMaster, rID, HttpContext.Current.Session)

            id = m.Save(ConnectionString, rID, drMaster, ds, fileSummary, result)

            message = dr("Message").ToString
            messageType = messageType.Success

            ''ISAMA BA SA TRANSACTION NG SAVE?
            ''COMMAND TEXT
            If dr("CommandText").ToString.Length > 0 Then
                Using sqlConn As New SqlConnection(ConnectionString)
                    sqlConn.Open()
                    Dim btn As DataRow = m.dtButtons.Select("ID = " & btnID)(0)
                    Using sqlCommand As New SqlCommand(btn("CommandText").ToString, sqlConn)
                        Try
                            If Not m.dtButtonParameters(btn("ID")) Is Nothing Then
                                For Each dr2 As DataRow In m.dtButtonParameters(btn("ID")).Rows
                                    Select Case dr2("Value").ToString.Substring(0, 1)
                                        Case "@"
                                            If dr2("Value").ToString = "@ID_WebMenus" Then
                                                sqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, mID.ToString)
                                            Else
                                                sqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, HttpContext.Current.Session(dr2("Value").ToString.Substring(1)).ToString)
                                            End If
                                        Case "#"
                                        Case "$"
                                        Case Else
                                            If dr2("Value").ToString = "ID" Then
                                                sqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, id)
                                            Else
                                                If Not IsNothing(drMaster(dr2("Value").ToString)) Then
                                                    sqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, CTypeDynamic(HttpUtility.HtmlEncode(drMaster(dr2("Value").ToString).ToString.Replace(Chr(160), Chr(32))), GetDataType(m.dtColumns.Select("Name = '" + dr2("Value").ToString + "'")(0).Item("colDataType").ToString)))
                                                Else
                                                    sqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, "")
                                                End If

                                            End If
                                    End Select
                                Next
                            End If
                            sqlCommand.ExecuteNonQuery()
                        Catch ex As Exception
                            messageType = messageType.BeforeExecute
                            Throw ex
                        Finally
                            sqlCommand.Dispose()
                        End Try
                    End Using
                    sqlConn.Close()
                End Using
            End If


            ' AFTER EXECUTE
            messageType = messageType.AfterExecute
            ExecuteButtonValidation(m, 2, btnID, drMaster, id, HttpContext.Current.Session)

        Catch ee As SqlException
            If ee.Message.Contains("IX_") Or ee.Message.Contains("CK_") Or ee.Message.Contains("FK_") Then
                message = getSystemMessage(ee.Message)
            Else
                message = ee.Message
            End If
        Catch ex As Exception
            'messageType = messageType.BeforeExecute
            message = ex.Message
            logError(ex, mID)
        End Try

        Try
            ret = "{"
            ret &= "message : '" & addStripSlashes(message) & "',"
            ret &= "messageType : '" & messageType & "',"
            ret &= "ID : '" & toAnyBase(id) & "',"  'GetIndirectReference(id, HttpContext.Current.Session)
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            logError(ex, mID)
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "'}")
        End Try


        Return New HttpResponseMessage() With { _
          .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function SpecialCommand(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)

        Dim mID As Integer = obj("mID").ToObject(Of Integer)(),
            rID As Integer = 0,
            btnID As Integer = obj("btnID").ToObject(Of Integer)(),
            drMaster As CaseInsensitiveDictionary = JsonConvert.DeserializeObject(Of CaseInsensitiveDictionary)(obj("Master"))
        If obj("rID").ToObject(Of String)() <> "0" Then
            Try
                rID = toBase10(obj("rID").ToObject(Of String)()) ' CInt(GetDirectReference(obj("rID").ToObject(Of String)(), HttpContext.Current.Session))
            Catch ex As Exception
                logError(ex, mID)
                Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
            End Try
        End If
        Dim ret As String = String.Empty,
            message As String = String.Empty,
            messageType As MessageType = messageType.Success,
            id As Integer = rID,
            r As New JObject

        Dim m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(mID)

        If Not UserHasAccessByUserGroup(m) Then Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
        If Not UserHasAccessByData(m, rID, 0) Then Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)

        Try
            Dim btn As DataRow = m.dtButtons.Select("ID = " & btnID)(0)
            ' BEFORE EXECUTE
            messageType = messageType.BeforeExecute
            ExecuteButtonValidation(m, 1, btnID, drMaster, rID, HttpContext.Current.Session)

            Using sqlConn As New SqlConnection(ConnectionString)
                sqlConn.Open()

                Using sqlCommand As New SqlCommand(btn("CommandText").ToString, sqlConn)
                    Try
                        If Not m.dtButtonParameters(btn("ID")) Is Nothing Then
                            For Each dr2 As DataRow In m.dtButtonParameters(btn("ID")).Rows
                                Select Case dr2("Value").ToString.Substring(0, 1)
                                    Case "@"
                                        If dr2("Value").ToString = "@ID_WebMenus" Then
                                            sqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, mID.ToString)
                                        Else
                                            sqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, HttpContext.Current.Session(dr2("Value").ToString.Substring(1)).ToString)
                                        End If
                                    Case "#"
                                    Case "$"
                                    Case Else
                                        If dr2("Value").ToString = "ID" Then
                                            sqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, rID.ToString)
                                        Else
                                            'sqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, IIf(IsNothing(CTypeDynamic(HttpUtility.HtmlEncode(drMaster(dr2("Value").ToString).ToString.Replace(Chr(160), Chr(32))), GetDataType(m.dtColumns.Select("Name = '" + dr2("Value").ToString + "'")(0).Item("colDataType").ToString))), "''", CTypeDynamic(HttpUtility.HtmlEncode(drMaster(dr2("Value").ToString).ToString.Replace(Chr(160), Chr(32))), GetDataType(m.dtColumns.Select("Name = '" + dr2("Value").ToString + "'")(0).Item("colDataType").ToString))))
                                            If IsNothing(drMaster(dr2("Value").ToString)) Then
                                                'If IsNothing(CTypeDynamic(HttpUtility.HtmlEncode(drMaster(dr2("Value").ToString).ToString.Replace(Chr(160), Chr(32))), GetDataType(m.dtColumns.Select("Name = '" + dr2("Value").ToString + "'")(0).Item("colDataType").ToString))) Then
                                                sqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, "")
                                            Else
                                                sqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, CTypeDynamic(HttpUtility.HtmlEncode(drMaster(dr2("Value").ToString).ToString.Replace(Chr(160), Chr(32))), GetDataType(m.dtColumns.Select("Name = '" + dr2("Value").ToString + "'")(0).Item("colDataType").ToString)))
                                            End If
                                        End If
                                End Select
                            Next
                        End If
                        sqlCommand.ExecuteNonQuery()
                        message = btn("Message").ToString

                    Catch ex As Exception
                        messageType = messageType.BeforeExecute
                        Throw ex
                    Finally
                        sqlCommand.Dispose()
                    End Try
                End Using
                sqlConn.Close()
            End Using


            ' AFTER EXECUTE
            messageType = messageType.AfterExecute
            ExecuteButtonValidation(m, 2, btnID, drMaster, rID, HttpContext.Current.Session)

        Catch ee As SqlException
            ' messageType = messageType.BeforeExecute
            message = ee.Message
        Catch ex As Exception
            ' messageType = messageType.BeforeExecute
            message = ex.Message
            logError(ex, mID)
        Finally
        End Try
        ''PAG DI NA EXISTING RECORD GO BACK ^
        Dim ctr2 As Integer = ExecScalarNoParams("SELECT COUNT(ID) FROM " & SetDataSourceFilter(m, 0, 0, HttpContext.Current.Session) & " WHERE ID = " & rID)
        Dim newmID As Integer = 0
        'Dim redirectMenuID As Integer = m.RedirectMenu
        If ctr2 = 0 Then
            newmID = CInt(IsNull(m.RedirectMenu, m.ParentID))
        End If

        Try
            ret = "{"
            ret &= "message : '" & addStripSlashes(message) & "',"
            ret &= "messageType : '" & messageType & "',"
            ret &= "ID : '" & If(ctr2 = 0, "0", toAnyBase(id)) & "'," ' GetIndirectReference(id, HttpContext.Current.Session)
            ret &= "mID : '" & newmID & "'"
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
            logError(ex, mID)
        End Try

        Return New HttpResponseMessage() With { _
         .Content = New JsonContent(r) _
       }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function Generate(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)

        Dim mID As Integer = obj("mID").ToObject(Of Integer)(),
            rID As Integer = 0,
            btnID As Integer = obj("btnID").ToObject(Of Integer)(),
            drMaster As CaseInsensitiveDictionary = JsonConvert.DeserializeObject(Of CaseInsensitiveDictionary)(obj("Master"))
        If obj("rID").ToObject(Of String)() <> "0" Then
            Try
                rID = toBase10(obj("rID").ToObject(Of String)()) 'CInt(GetDirectReference(obj("rID").ToObject(Of String)(), HttpContext.Current.Session))
            Catch ex As Exception
                logError(ex, mID)
                Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
            End Try
        End If
        Dim ret As String = String.Empty,
            message As String = String.Empty,
            messageType As MessageType = messageType.Success,
            targetmID As Integer = 0,
            Data As String = String.Empty,
            r As New JObject

        If Not UserHasAccessByUserGroup(mCollection.GetMenu(mID)) Then Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
        If Not UserHasAccessByData(mCollection.GetMenu(mID), rID, 0) Then Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)

        Try
            Dim m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(mID)
            Dim dr3 As DataRow = m.dtButtons.Select("ID = " & btnID)(0)
            ' BEFORE EXECUTE
            messageType = messageType.BeforeExecute
            ExecuteButtonValidation(m, 1, btnID, drMaster, rID, HttpContext.Current.Session)

            Using sqlConn As New SqlConnection(ConnectionString)
                sqlConn.Open()

                Dim cText As String = dr3("CommandText").ToString
                Using SqlCommand As New SqlCommand(dr3("CommandText").ToString, sqlConn)
                    If Not m.dtButtonParameters(dr3("ID").ToString) Is Nothing Then
                        For Each dr2 As DataRow In m.dtButtonParameters(dr3("ID").ToString).Rows
                            Select Case dr2("Value").ToString.Substring(0, 1)
                                Case "@"
                                    If dr2("Value").ToString = "@ID_WebMenus" Then
                                        SqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, mID.ToString)
                                    Else
                                        SqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, HttpContext.Current.Session(dr2("Value").ToString.Substring(1)).ToString)
                                    End If
                                Case "#"
                                Case "$"
                                Case Else
                                    If dr2("Value").ToString = "ID" Then
                                        SqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, rID)
                                    Else
                                        SqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, CTypeDynamic(drMaster(dr2("Value").ToString), GetDataType(m.dtColumns.Select("Name = '" + dr2("Value").ToString + "'")(0).Item("colDataType").ToString)))
                                    End If
                            End Select
                        Next
                    End If
                    Data = serialize(SqlCommand.ExecuteReader)
                End Using
                sqlConn.Close()
                message = dr3("Message").ToString
                targetmID = CInt(dr3("ID_TargetWebMenus").ToString)
            End Using


            ' AFTER EXECUTE
            messageType = messageType.AfterExecute
            ExecuteButtonValidation(m, 2, btnID, drMaster, rID, HttpContext.Current.Session)

        Catch ee As SqlException
            message = ee.Message
        Catch ex As Exception
            'messageType = messageType.BeforeExecute
            message = ex.Message
            logError(ex, mID)
        Finally

        End Try
        Try
            ret = "{"
            ret &= "message : '" & addStripSlashes(message) & "',"
            ret &= "messageType : " & messageType & ","
            ret &= "targetmID : " & targetmID & ","
            ret &= "data : " & Data & ""
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
            logError(ex, mID)
        End Try

        Return New HttpResponseMessage() With { _
          .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function ColumnSelection(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)

        Dim mID As Integer = obj("mID").ToObject(Of Integer)(),
            ret As String = String.Empty,
            r As JObject,
            err As String = String.Empty,
            data As String = String.Empty
        If Not UserHasAccessByUserGroup(mCollection.GetMenu(mID)) Then Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)

        Try
            data = getJSONTable("SELECT * FROM dbo.fGetWebMenuUserColumns(" & mID & ", " & HttpContext.Current.Session("ID_User") & ")")
        Catch ex As Exception
            err = ex.Message
            logError(ex, mID)
        Finally

        End Try
        Try
            ret = "{"
            ret &= "err : '" & addStripSlashes(err) & "',"
            ret &= "data : " & data & ""
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
            logError(ex, mID)
        End Try

        Return New HttpResponseMessage() With { _
          .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function SaveColumnSelection(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)

        Dim mID As Integer = obj("mID").ToObject(Of Integer)(),
            columns As DataTable = obj("columns").ToObject(Of DataTable)(),
            ret As String = String.Empty,
            r As JObject,
            err As String = String.Empty
        If Not UserHasAccessByUserGroup(mCollection.GetMenu(mID)) Then Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)

        Try
            Using sqlConn As New SqlConnection(ConnectionString)
                sqlConn.Open()
                Dim transName As String = mID & "_" & DateTime.Now.Ticks & "_"
                Dim sqlTrans As SqlClient.SqlTransaction = sqlConn.BeginTransaction(IsolationLevel.ReadCommitted, transName)
                Try
                    Using sqlCommand As New SqlCommand()
                        sqlCommand.CommandText = "dbo.pSaveWebMenuUserColumns @ID_WebMenuColumns, @ID_User, @SeqNo, @GroupSeqNo"
                        sqlCommand.Connection = sqlConn
                        sqlCommand.Transaction = sqlTrans

                        For Each dr As DataRow In columns.Select
                            sqlCommand.Parameters.Clear()

                            sqlCommand.Parameters.AddWithValue("@ID_WebMenuColumns", dr("ID_WebMenuColumns"))
                            sqlCommand.Parameters.AddWithValue("@ID_User", HttpContext.Current.Session("ID_User"))
                            sqlCommand.Parameters.AddWithValue("@SeqNo", dr("SeqNo"))
                            sqlCommand.Parameters.AddWithValue("@GroupSeqNo", dr("GroupSeqNo"))
                            sqlCommand.ExecuteNonQuery()

                        Next
                        sqlCommand.Parameters.Clear()
                        If columns.Rows.Count > 0 Then
                            sqlCommand.CommandText = "DELETE FROM tWebMenuUserColumns WHERE ID_User = @ID_User AND ID_WebMenuColumns NOT IN (" & String.Join(",", columns.AsEnumerable().Select(Function(x) x.Item("ID_WebMenuColumns")).ToList) & ")"
                            sqlCommand.Parameters.AddWithValue("@ID_User", HttpContext.Current.Session("ID_User"))
                        Else
                            sqlCommand.CommandText = "DELETE wmuc " +
                                                     "FROM dbo.tWebMenuUserColumns wmuc " +
                                                     "LEFT JOIN dbo.tWebMenuColumns wmc ON wmc.ID = wmuc.ID_WebMenuColumns " +
                                                     "LEFT JOIN dbo.tWebMenuTabs wmt ON wmt.ID = wmc.ID_WebMenuTabs " +
                                                     "WHERE wmuc.ID_User = " & HttpContext.Current.Session("ID_User") & " And wmt.ID_WebMenus = " & mID

                        End If

                        sqlCommand.ExecuteNonQuery()


                    End Using
                    sqlTrans.Commit()
                Catch ex As Exception
                    sqlTrans.Rollback()
                    err = ex.Message
                Finally
                    sqlTrans.Dispose()
                End Try

                sqlConn.Close()
                sqlConn.Dispose()
            End Using

        Catch ex As Exception
            err = ex.Message
            logError(ex, mID)
        Finally

        End Try
        Try
            ret = "{"
            ret &= "error : '" & addStripSlashes(err) & "'"
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
            logError(ex, mID)
        End Try

        Return New HttpResponseMessage() With { _
          .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function BatchGridDelete(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)

        Dim mID As Integer = obj("mID").ToObject(Of Integer)(),
            iDs As String = obj("iDs").ToObject(Of String)()

        If Not UserHasAccessByUserGroup(mCollection.GetMenu(mID)) Then Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)

        Dim ret As String = String.Empty,
            message As String = String.Empty,
            r As New JObject

        Try
            Dim m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(mID)

            'If CBool(IsNull(m.ColumnValue("HasDelete"), False)) Then
            '  Dim vfilter As String = SetViewFilter(m, 0, 0, Me.Session).Replace("WHERE", "")
            Dim oFilter As String = String.Empty

            If CBool(IsNull(m.ColumnValue("HasGridCheckBox"), False)) Then
                oFilter = formatCondition(IsNull(m.ColumnValue("HideGridCheckBoxIF"), ""))
            End If

            m.DeleteRow(iDs, oFilter)
            'End If



        Catch ex As Exception
            message = ex.Message
            logError(ex, mID)
        Finally

        End Try
        Try
            ret = "{"
            ret &= "message : '" & addStripSlashes(message) & "'"
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "'}")
            logError(ex, mID)
        End Try


        Return New HttpResponseMessage() With { _
          .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function GridDelete(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)

        Dim mID As Integer = obj("mID").ToObject(Of Integer)(),
            iD As Integer = obj("iD").ToObject(Of String)()

        Dim ret As String = String.Empty,
            message As String = String.Empty,
            r As New JObject
        ''TODO: ung mga ID nakareference map
        If Not UserHasAccessByUserGroup(mCollection.GetMenu(mID)) Then Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
        If Not UserHasAccessByData(mCollection.GetMenu(mID), iD, 0) Then Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)

        Try
            Dim m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(mID)

            If CBool(IsNull(m.ColumnValue("HasDelete"), False)) Then
                '  Dim vfilter As String = SetViewFilter(m, 0, 0, Me.Session).Replace("WHERE", "")
                Dim oFilter As String = String.Empty

                If CBool(IsNull(m.ColumnValue("HasGridCheckBox"), False)) Then
                    oFilter = formatCondition(IsNull(m.ColumnValue("HideGridCheckBoxIF"), ""))
                End If

                m.DeleteRow(iD.ToString, oFilter)
            End If

        Catch ex As Exception
            message = ex.Message
            logError(ex, mID)
        Finally

        End Try
        Try
            ret = "{"
            ret &= "message : '" & addStripSlashes(message) & "'"
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
            logError(ex, mID)
        End Try

        Return New HttpResponseMessage() With { _
          .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function BatchGridCommand(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)

        Dim mID As Integer = obj("mID").ToObject(Of Integer)(),
            btnID As Integer = obj("btnID").ToObject(Of Integer)(),
            iDs As String = obj("iDs").ToObject(Of String)()

        Dim ret As String = String.Empty,
            message As String = String.Empty,
            r As New JObject

        Try
            Dim m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(mID)
            Using sqlConn As New SqlConnection(ConnectionString)
                sqlConn.Open()
                Dim dr3 As DataRow = m.dtButtons.Select("ID = " & btnID)(0)
                Dim cText As String = dr3("CommandText").ToString
                For Each refID As String In iDs.Split(","c)
                    Using SqlCommand As New SqlCommand(dr3("CommandText").ToString, sqlConn)
                        If Not m.dtButtonParameters(dr3("ID").ToString) Is Nothing Then
                            For Each dr2 As DataRow In m.dtButtonParameters(dr3("ID").ToString).Rows
                                Select Case dr2("Value").ToString.Substring(0, 1)
                                    Case "@"
                                        If dr2("Value").ToString = "@ID_WebMenus" Then
                                            SqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, mID.ToString)
                                        Else
                                            SqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, HttpContext.Current.Session(dr2("Value").ToString.Substring(1)).ToString)
                                        End If
                                    Case "#"
                                    Case "$"
                                    Case Else
                                        If dr2("Value").ToString = "ID" Then
                                            SqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, refID)
                                        Else
                                            '  SqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, CTypeDynamic(drMaster(dr2("Value").ToString), GetDataType(m.dtColumns.Select("Name = '" + dr2("Value").ToString + "'")(0).Item("colDataType").ToString)))
                                        End If
                                End Select
                            Next
                        End If
                        SqlCommand.ExecuteNonQuery()
                    End Using
                Next
                sqlConn.Close()
                message = dr3("Message").ToString
            End Using



        Catch ex As Exception
            message = ex.Message
            logError(ex, mID)
        Finally

        End Try

        Try
            ret = "{"
            ret &= "message : '" & addStripSlashes(message) & "'"
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error: '" & addStripSlashes(ex.Message) & "' }")
            logError(ex, mID)
        End Try
        Return New HttpResponseMessage() With { _
          .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function BatchGridSave(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim mID As Integer = obj("mID").ToObject(Of Integer)(),
            btnID As Integer = obj("btnID").ToObject(Of Integer)(),
            gridData As DataTable = JsonConvert.DeserializeObject(Of DataTable)(obj("gridData"))
        Dim ret As String = String.Empty,
            message As String = String.Empty,
            messageType As MessageType = messageType.Success,
            id As Integer = 0,
            r As New JObject

        Try
            Dim m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(mID)

            Using sqlConn As SqlClient.SqlConnection = New SqlClient.SqlConnection(ConnectionString)
                sqlConn.Open()
                Dim dr3 As DataRow = m.dtButtons.Select("ID = " & btnID)(0)
                Dim transName As String = m.MenuID & "_" & DateTime.Now.Ticks
                Dim trans As SqlClient.SqlTransaction = sqlConn.BeginTransaction(IsolationLevel.ReadCommitted, transName)
                Using sqlCommand As SqlClient.SqlCommand = New SqlClient.SqlCommand
                    Try
                        sqlCommand.Connection = sqlConn
                        sqlCommand.Transaction = trans

                        Dim colsChild As List(Of String) = m.TableColumns(True).Split(",").ToList
                        For Each dr As DataRow In gridData.Select
                            sqlCommand.Parameters.Clear()
                            If dr("ID") = 0 Then
                                sqlCommand.CommandText = m.GETGRIDINSERTCommand()
                            Else
                                sqlCommand.CommandText = m.GETGRIDUPDATECommand()
                            End If

                            For Each col As String In colsChild
                                If dr(col).ToString = "" Then
                                    sqlCommand.Parameters.AddWithValue("@" & col, System.DBNull.Value)
                                Else
                                    Dim dataType As System.Type = GetDataType(m.dtColumns.Select("Name = '" + col + "'")(0).Item("colDataType").ToString)
                                    sqlCommand.Parameters.AddWithValue("@" + col, CTypeDynamic(dr(col).ToString.Replace(Chr(160), Chr(32)), dataType))
                                End If
                            Next
                            If dr("ID") > 0 Then
                                sqlCommand.Parameters.AddWithValue("@ID", dr("ID"))
                            End If
                            sqlCommand.ExecuteNonQuery()
                        Next
                        trans.Commit()
                    Catch ex As Exception
                        trans.Rollback()
                        Throw ex
                    Finally
                        sqlCommand.Dispose()
                        sqlConn.Close()
                        sqlConn.Dispose()
                    End Try
                End Using
                message = dr3("Message").ToString
            End Using

        Catch ee As SqlException
            messageType = messageType.BeforeExecute
            message = ee.Message
        Catch ex As Exception
            messageType = messageType.BeforeExecute
            message = ex.Message
            logError(ex, mID)
        End Try

        Try
            ret = "{"
            ret &= "message : '" & addStripSlashes(message) & "',"
            ret &= "messageType : '" & messageType & "',"
            ret &= "ID : " & id & ","
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
            logError(ex, mID)
        End Try
        Return New HttpResponseMessage() With { _
          .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function loadTab(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage '  <DeflateCompression>
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim ret As String = String.Empty,
            r As New JObject
        Try
            Dim mID As Integer = obj("mID").ToObject(Of Integer)(),
            rID As Integer = 0,
            m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(mID)
            If obj("rID").ToObject(Of String)() <> "0" Then
                rID = toBase10(obj("rID").ToObject(Of String)()) 'GetDirectReference(obj("rID").ToObject(Of String)(), HttpContext.Current.Session)
            End If
            If CBool(m.ColumnValue("HasOpen")) Then
                ret = "{ data :" & getJSONTable("SELECT " & m.getListColumns & " FROM " + SetDataSourceFilter(m, 0, rID, HttpContext.Current.Session) & " ORDER BY " & IsNull(m.ColumnValue("SortBy"), "ID DESC"), True, HttpContext.Current.Session) & ","
            Else
                ret = "{ data :" & getJSONTable("SELECT " & m.getListColumns & " FROM " + SetDataSourceFilter(m, 0, rID, HttpContext.Current.Session) & " ORDER BY " & IsNull(m.ColumnValue("SortBy"), "ID DESC")) & ","
            End If


            ret &= "}"

            r = JObject.Parse(ret)
            ret = Nothing
        Catch ex As Exception
            r = JObject.Parse("{ error:'" & addStripSlashes(ex.Message) & "' }")
            logError(ex)
        End Try

        Return New HttpResponseMessage() With { _
           .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function getResources(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim mID As Integer = obj("mID").ToObject(Of Integer)(),
            rID As Integer = 0,
            pID As Integer = 0,
            m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(mID),
            ret As String = String.Empty,
            r As New Newtonsoft.Json.Linq.JObject,
            Session As System.Web.SessionState.HttpSessionState = HttpContext.Current.Session,
            filter As String = If(obj("filter").ToObject(Of String)() <> "" And m.ColumnValue("ID_WebMenuTypes") = 1, decryptData(obj("filter").ToObject(Of String)()), "")

        If m.ColumnValue("ID_WebMenuTypes") <> 1 AndAlso obj("rID").ToObject(Of String)() <> "0" Then
            Try
                rID = toBase10(obj("rID").ToObject(Of String)()) ' GetDirectReference(obj("rID").ToObject(Of String)(), HttpContext.Current.Session)
            Catch ex As Exception
                Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
            End Try
        End If

        If obj("pID").ToObject(Of String)() <> "0" Then
            Try
                pID = toBase10(obj("pID").ToObject(Of String)()) ' GetDirectReference(obj("pID").ToObject(Of String)(), HttpContext.Current.Session)
            Catch ex As Exception
                Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
            End Try
        End If
        If Not UserHasAccessByUserGroup(mCollection.GetMenu(mID)) Then Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
        If Not UserHasAccessByData(m, rID, pID) Then Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
        'Dim stopWatch As New System.Diagnostics.Stopwatch
        'stopWatch.Start()

        Try
            Dim dateSettings As JsonSerializerSettings = New JsonSerializerSettings With {.DateFormatHandling = DateFormatHandling.IsoDateFormat}
            Select Case m.ColumnValue("ID_WebMenuTypes")
                Case 1
                    ret = GenerateViewPageSource(m, rID, pID, HttpContext.Current.Session, filter)
                Case 3, 12, 14

                    Dim strQueryBuilder As New StringBuilder
                    If rID > 0 Then
                        strQueryBuilder.Append("SELECT " & m.getFormColumns() & " FROM " & SetDataSourceFilter(m, 0, pID, Session) & " WHERE ID = " & rID.ToString + ";")
                    End If

                    If m.ColumnValue("ID_WebMenuTypes") = 12 Or m.ColumnValue("ID_WebMenuTypes") = 14 Then
                        strQueryBuilder.Append("SELECT " & m.getFormColumns() & " FROM " & SetDataSourceFilter(mCollection.GetMenu(m.MenuID), 0, 0, Session) & " WHERE ID = " & pID.ToString + ";")
                    End If
                    'DROPDOWN SOURCES
                    Dim ddTemp As New List(Of String)
                    generateControlQueryBuilder(strQueryBuilder, ddTemp, m, rID, pID, HttpContext.Current.Session, 2, True)
                    For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 14)
                        generateControlQueryBuilder(strQueryBuilder, ddTemp, m2, rID, pID, HttpContext.Current.Session, 2, False)
                    Next

                    'AUTO COMPLETE
                    Dim ddAutoCom As New List(Of String)
                    generateControlQueryBuilder(strQueryBuilder, ddAutoCom, m, rID, pID, HttpContext.Current.Session, 23, True)
                    For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 14)
                        generateControlQueryBuilder(strQueryBuilder, ddAutoCom, m2, rID, pID, HttpContext.Current.Session, 23, False)
                    Next


                    'RADIO BUTTON
                    Dim ddRdB As New List(Of String)
                    generateControlQueryBuilder(strQueryBuilder, ddRdB, m, rID, pID, HttpContext.Current.Session, 26, True)
                    For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 14)
                        generateControlQueryBuilder(strQueryBuilder, ddRdB, m2, rID, pID, HttpContext.Current.Session, 26, False)
                    Next

                    'Lookup
                    Dim ddLookUp As New List(Of String)
                    generateControlQueryBuilder(strQueryBuilder, ddLookUp, m, rID, pID, HttpContext.Current.Session, 17, True)
                    For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 14)
                        generateControlQueryBuilder(strQueryBuilder, ddLookUp, m2, rID, pID, HttpContext.Current.Session, 17, False)
                    Next

                    'Detailed Lookup
                    Dim ddDetailedLookUp As New List(Of String)
                    'Dim ddDetailedValueLookUp As New List(Of String)
                    generateControlQueryBuilder(strQueryBuilder, ddDetailedLookUp, m, rID, pID, HttpContext.Current.Session, 35, True)
                    'getDetailValueLookup(strQueryBuilder, ddDetailedValueLookUp, m, rID, pID, HttpContext.Current.Session, 35, True)
                    For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 14)
                        generateControlQueryBuilder(strQueryBuilder, ddDetailedLookUp, m2, rID, pID, HttpContext.Current.Session, 35, False)
                    Next


                    Using ttCon As New SqlClient.SqlConnection(ConnectionString)
                        ttCon.Open()
                        Using ttCmd As New SqlCommand(strQueryBuilder.ToString, ttCon)
                            Dim Reader As SqlDataReader = Nothing
                            ret = "{"
                            ret &= "Master:"
                            If strQueryBuilder.Length > 0 Then Reader = ttCmd.ExecuteReader
                            If rID = 0 Then
                                ret &= dictionaryToJSON(m.getDefaultFormData(), pID, Session, IsNull(mCollection.GetMenu(m.ParentID).TableName, ""))
                            Else
                                ret &= getJSONTableRowNoClose(Reader)
                                Reader.NextResult()
                            End If
                            ret &= ","

                            ret &= "Parent:"
                            If m.ColumnValue("ID_WebMenuTypes") = 12 Or m.ColumnValue("ID_WebMenuTypes") = 14 Then
                                ret &= getJSONTableRowNoClose(Reader)
                                Reader.NextResult()
                            Else
                                ret &= "{}"
                            End If
                            ret &= ","

                            ret &= "Detail:{"
                            For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID)
                                ret &= m2.MenuID & ":[],"
                            Next
                            ret &= "},"

                            'DROPDOWN
                            ret &= "dropdown_source:{"
                            For x As Integer = 0 To ddTemp.Count - 1
                                ret &= ddTemp(x) & ":" & serializeNoClose(Reader) & ","
                                Reader.NextResult()
                            Next
                            ret &= "},"
                            'AUTO COMPLETE
                            ret &= "autocomplete_source:{"
                            For x As Integer = 0 To ddAutoCom.Count - 1
                                ret &= ddAutoCom(x) & ":" & serializeNoClose(Reader) & ","
                                Reader.NextResult()
                            Next
                            ret &= "},"
                            'RADIO BUTTON
                            ret &= "rdb_source:{"
                            For x As Integer = 0 To ddRdB.Count - 1
                                ret &= ddRdB(x) & ":" & serializeNoClose(Reader) & ","
                                Reader.NextResult()
                            Next
                            ret &= "},"
                            'LOOKUP
                            ret &= "lookup_source:{"
                            For x As Integer = 0 To ddLookUp.Count - 1
                                ret &= ddLookUp(x) & ":" & serializeNoClose(Reader) & ","
                                Reader.NextResult()
                            Next
                            ret &= "},"
                            'DETAILED LOOKUP
                            ret &= "detailedlookup_source:{"
                            For x As Integer = 0 To ddDetailedLookUp.Count - 1
                                ret &= ddDetailedLookUp(x) & ":" & serializeNoClose(Reader) & ","
                                Reader.NextResult()
                            Next
                            ret &= "},"
                            'ret &= "detailedValuelookup_source:{"
                            'For x As Integer = 0 To ddDetailedValueLookUp.Count - 1
                            '    ret &= ddDetailedValueLookUp(x) & ":" & serializeNoClose(Reader) & ","
                            '    Reader.NextResult()
                            'Next
                            'ret &= "},"


                            ret &= "}"
                            If Not Reader Is Nothing Then Reader.Close()
                        End Using
                        ttCon.Close()
                    End Using
                    'Dim master As String = Await Task.FromResult(Of String)(generateMasterDataSource(m, rID, pID, Session))



                    'Dim master As String = String.Empty,
                    '    tMaster As New Task(Sub()
                    '                            master = generateMasterDataSource(m, rID, pID, Session)
                    '                        End Sub),
                    '    htmlControl As String = String.Empty,
                    '    thtmlControl As New Task(Sub()
                    '                                 htmlControl = generateHTMLControlDataSource(m, rID, pID, Session)
                    '                             End Sub)
                    'tMaster.Start()
                    'thtmlControl.Start()
                    'Task.WaitAll(tMaster, thtmlControl)

                    'ret = "{"
                    'ret &= "Master:" & master & ","
                    'ret &= "Detail:{"
                    'For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID)
                    '    ret &= m2.MenuID & ":[],"
                    'Next
                    'ret &= "},"
                    'ret &= htmlControl
                    'ret &= "}"
            End Select
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error:'" & addStripSlashes(ex.Message) & "' }")
            logError(ex, mID)
        End Try
        'stopWatch.Stop()
        'LogEvent("GET RESOURCES : " & System.DateTime.Now.ToString & " -> Menu ID : " & m.MenuID & ", rID : " & rID & ", Time Elapsed : " & stopWatch.ElapsedMilliseconds)
        Return New HttpResponseMessage() With { _
           .Content = New JsonContent(r) _
        }

    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function getAllResources(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim mID As Integer = obj("mID").ToObject(Of Integer)(),
            rID As Integer = 0,
            pID As Integer = 0,
            m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(mID),
            ret As String = String.Empty,
            r As New Newtonsoft.Json.Linq.JObject,
            Session As System.Web.SessionState.HttpSessionState = HttpContext.Current.Session
        If m.ColumnValue("ID_WebMenuTypes") <> 1 AndAlso obj("rID").ToObject(Of String)() <> "0" Then
            Try
                rID = toBase10(obj("rID").ToObject(Of String)()) 'GetDirectReference(obj("rID").ToObject(Of String)(), Session)
            Catch ex As Exception
                Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
            End Try
        End If

        If obj("pID").ToObject(Of String)() <> "0" Then
            Try
                pID = toBase10(obj("pID").ToObject(Of String)()) 'GetDirectReference(obj("pID").ToObject(Of String)(), Session)
            Catch ex As Exception
                Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
            End Try
        End If
        If Not UserHasAccessByUserGroup(mCollection.GetMenu(mID)) Then Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
        If Not UserHasAccessByData(m, rID, pID) Then Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
        'Dim stopWatch As New System.Diagnostics.Stopwatch
        'stopWatch.Start()

        Try
            Dim dateSettings As JsonSerializerSettings = New JsonSerializerSettings With {.DateFormatHandling = DateFormatHandling.IsoDateFormat}
            Select Case m.ColumnValue("ID_WebMenuTypes")
                Case 1
                    ret = GenerateViewPageSource(m, rID, pID, HttpContext.Current.Session)

                Case 3, 12, 14

                    Dim strQueryBuilder As New StringBuilder
                    If rID > 0 Then
                        strQueryBuilder.Append("SELECT " & m.getFormColumns() & " FROM " & SetDataSourceFilter(m, 0, pID, Session) & " WHERE ID = " & rID.ToString + ";")
                    End If

                    If m.ColumnValue("ID_WebMenuTypes") = 12 Or m.ColumnValue("ID_WebMenuTypes") = 14 Then
                        strQueryBuilder.Append("SELECT " & m.getFormColumns() & " FROM " & SetDataSourceFilter(mCollection.GetMenu(m.ParentID), 0, 0, Session) & " WHERE ID = " & pID.ToString + ";")
                    End If

                    'DETAIL
                    Dim menuTemp As New List(Of String)
                    For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID)
                        strQueryBuilder.Append("SELECT " & m2.getListColumns & " FROM " & SetDataSourceFilter(m2, 0, rID, Session) & ";")
                        menuTemp.Add(m2.MenuID.ToString)
                    Next
                    'DROPDOWN SOURCES
                    Dim ddTemp As New List(Of String)
                    generateControlQueryBuilder(strQueryBuilder, ddTemp, m, rID, pID, HttpContext.Current.Session, 2, True)
                    For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 14)
                        generateControlQueryBuilder(strQueryBuilder, ddTemp, m2, rID, pID, HttpContext.Current.Session, 2, False)
                    Next

                    'AUTO COMPLETE
                    Dim ddAutoCom As New List(Of String)
                    generateControlQueryBuilder(strQueryBuilder, ddAutoCom, m, rID, pID, HttpContext.Current.Session, 23, True)
                    For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 14)
                        generateControlQueryBuilder(strQueryBuilder, ddAutoCom, m2, rID, pID, HttpContext.Current.Session, 23, False)
                    Next

                    'TEXT AUTO COMPLETE
                    Dim ddTextAutoComplete As New List(Of String)
                    generateControlQueryBuilder(strQueryBuilder, ddTextAutoComplete, m, rID, pID, HttpContext.Current.Session, 10, True)
                    For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 14)
                        generateControlQueryBuilder(strQueryBuilder, ddTextAutoComplete, m2, rID, pID, HttpContext.Current.Session, 10, False)
                    Next

                    'RADIO BUTTON
                    Dim ddRdB As New List(Of String)
                    generateControlQueryBuilder(strQueryBuilder, ddRdB, m, rID, pID, HttpContext.Current.Session, 26, True)
                    For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 14)
                        generateControlQueryBuilder(strQueryBuilder, ddRdB, m2, rID, pID, HttpContext.Current.Session, 26, False)
                    Next

                    'Lookup
                    Dim ddLookUp As New List(Of String)
                    generateControlQueryBuilder(strQueryBuilder, ddLookUp, m, rID, pID, HttpContext.Current.Session, 17, True)
                    For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 14)
                        generateControlQueryBuilder(strQueryBuilder, ddLookUp, m2, rID, pID, HttpContext.Current.Session, 17, False)
                    Next


                    Using ttCon As New SqlClient.SqlConnection(ConnectionString)
                        ttCon.Open()
                        Using ttCmd As New SqlCommand(strQueryBuilder.ToString, ttCon)
                            Dim Reader As SqlDataReader = Nothing

                            ret = "{"
                            ret &= "Master:"
                            If strQueryBuilder.Length > 0 Then Reader = ttCmd.ExecuteReader
                            If rID = 0 Then
                                ret &= dictionaryToJSON(m.getDefaultFormData(), pID, Session, IsNull(mCollection.GetMenu(m.ParentID).TableName, ""))
                            Else
                                ret &= getJSONTableRowNoClose(Reader, True, HttpContext.Current.Session, mCollection.GetMenu(mID).dtColumns.AsEnumerable().Where(Function(c) IsNull(c.Item("ID_WebMenuControlTypes"), 0) = 27 And IsNull(c.Item("IsVisible"), False) = True).Select(Function(c) CStr(c.Item("Name"))).ToList())
                                Reader.NextResult()
                            End If
                            ret &= ","

                            ret &= "Parent:"
                            If m.ColumnValue("ID_WebMenuTypes") = 12 Or m.ColumnValue("ID_WebMenuTypes") = 14 Then
                                ret &= getJSONTableRowNoClose(Reader)
                                Reader.NextResult()
                            Else
                                ret &= "{}"
                            End If
                            ret &= ","

                            ret &= "Detail:{"
                            For x As Integer = 0 To menuTemp.Count - 1
                                ret &= menuTemp(x) & ":" & serializeNoClose(Reader) & ","
                                Reader.NextResult()
                            Next x
                            ret &= "},"

                            'DROPDOWN
                            ret &= "dropdown_source:{"
                            For x As Integer = 0 To ddTemp.Count - 1
                                ret &= ddTemp(x) & ":" & serializeNoClose(Reader) & ","
                                Reader.NextResult()
                            Next
                            ret &= "},"

                            'AUTO COMPLETE
                            ret &= "autocomplete_source:{"
                            For x As Integer = 0 To ddAutoCom.Count - 1
                                ret &= ddAutoCom(x) & ":" & serializeNoClose(Reader) & ","
                                Reader.NextResult()
                            Next
                            ret &= "},"

                            'TEXT AUTO COMPLETE
                            ret &= "text_autocomplete_source:{"
                            For x As Integer = 0 To ddTextAutoComplete.Count - 1
                                ret &= ddTextAutoComplete(x) & ":" & JavascriptArrayNoClose(Reader) & ","
                                Reader.NextResult()
                            Next
                            ret &= "},"

                            'RADIO BUTTON
                            ret &= "rdb_source:{"
                            For x As Integer = 0 To ddRdB.Count - 1
                                ret &= ddRdB(x) & ":" & serializeNoClose(Reader) & ","
                                Reader.NextResult()
                            Next
                            ret &= "},"
                            'LOOKUP
                            ret &= "lookup_source:{"
                            For x As Integer = 0 To ddLookUp.Count - 1
                                ret &= ddLookUp(x) & ":" & serializeNoClose(Reader) & ","
                                Reader.NextResult()
                            Next
                            ret &= "},"

                            ret &= "}"

                            If Not Reader Is Nothing Then Reader.Close()
                        End Using
                        ttCon.Close()
                    End Using
            End Select
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error:'" & addStripSlashes(ex.Message) & "' }")
            logError(ex, mID)
        End Try
        'stopWatch.Stop()
        'LogEvent("GET ALL RESOURCES : " & System.DateTime.Now.ToString & " -> Menu ID : " & m.MenuID & ", rID : " & rID & ", Time Elapsed : " & stopWatch.ElapsedMilliseconds)
        Return New HttpResponseMessage() With { _
           .Content = New JsonContent(r) _
        }

    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function getPagedData(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim ret As String = String.Empty,
            data As String = String.Empty,
            totalServerItems As String = String.Empty,
            r As New JObject

        Dim mID As Integer = obj("mID").ToObject(Of Integer)(),
            pageSize As String = obj("pageSize").ToObject(Of String)(),
            currentPage As Integer = If(obj("currentPage").ToObject(Of Integer)() = 0, 1, obj("currentPage").ToObject(Of Integer)()),
            field As String = obj("field").ToObject(Of String)(),
            direction As String = obj("direction").ToObject(Of String)(),
            IsSearchFilter As Boolean = CBool(obj("IsSearchAll").ToObject(Of Integer)()),
            filter As Dictionary(Of String, Object) = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(obj("filter")),
            m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(mID),
            rID As Integer = 0,
            dataSource As String = String.Empty 'SetDataSourceFilter(m, 0, rID, HttpContext.Current.Session)
        If obj("rID").ToObject(Of String)() <> "0" Then
            rID = toBase10(obj("rID").ToObject(Of String)()) ' GetDirectReference(obj("rID").ToObject(Of String)(), HttpContext.Current.Session)
        End If
        Dim filterString As New List(Of String)
        If Not IsSearchFilter Then
            For Each x In filter.Where(Function(i) i.Key <> "Detail_Menu" AndAlso i.Key <> "SearchAll" AndAlso Not i.Value Is Nothing AndAlso Not IsDBNull(i.Value) AndAlso Trim(i.Value.ToString).Length > 0)
                Dim dataType As String = String.Empty

                If x.Key.Contains("From_") OrElse x.Key.Contains("To_") Then
                    dataType = GetDataType(m.dtColumns.Select("Name = '" & x.Key.Replace("From_", "").Replace("To_", "") & "'")(0).Item("colDataType").ToString).FullName.ToString()
                Else
                    dataType = GetDataType(m.dtColumns.Select("Name = '" & x.Key & "'")(0).Item("colDataType").ToString).FullName.ToString()
                End If

                If dataType = "System.String" Then
                    filterString.Add(x.Key & " LIKE '%'+@" & x.Key & "+'%'")
                ElseIf dataType = "System.DateTime" OrElse dataType = "System.Int32" OrElse dataType = "System.Decimal" Then
                    If x.Key.Contains("From_") Then
                        filterString.Add(x.Key.Replace("From_", "") & " >= @" & x.Key)
                    ElseIf x.Key.Contains("To_") Then
                        filterString.Add(x.Key.Replace("To_", "") & " <= @" & x.Key)
                    Else
                        If x.Key.ToString.ToLower.Contains("start") Or x.Key.ToString.ToLower.Contains("end") Then
                            If x.Key.ToString.ToLower.Contains("start") Then
                                filterString.Add(x.Key & " >= @" & x.Key)
                            Else
                                filterString.Add(x.Key & " <= @" & x.Key)
                            End If
                        Else
                            filterString.Add(x.Key & " = @" & x.Key)
                        End If
                    End If
                Else
                    filterString.Add(x.Key & " = @" & x.Key)
                End If
            Next

            ' DETAIL FILTER
            If filter.Where(Function(i) i.Key = "Detail_Menu").Count > 0 Then
                Dim x = DirectCast(filter.Where(Function(i) i.Key = "Detail_Menu").First().Value, JObject)
                Dim subColDataType As String = String.Empty

                For Each item In x
                    Dim subFilterString As New List(Of String)
                    Dim m2 As GSWEB.MenuCollection.Menu = mCollection.GetMenu(CInt(item.Key.ToString))

                    For Each subCol As JProperty In item.Value
                        subColDataType = GetDataType(m2.dtColumns.Select("Name = '" & subCol.Name & "'")(0).Item("colDataType").ToString).FullName.ToString()

                        If subColDataType = "System.String" Then
                            subFilterString.Add(subCol.Name & " LIKE '%'+@" & subCol.Name & "+'%'")
                        ElseIf subColDataType = "System.DateTime" OrElse subColDataType = "System.Int32" OrElse subColDataType = "System.Decimal" Then
                            If subCol.Name.Contains("From_") Then
                                subFilterString.Add(subCol.Name.Replace("From_", "") & " >= @" & subCol.Name)
                            ElseIf subCol.Name.Contains("To_") Then
                                subFilterString.Add(subCol.Name.Replace("To_", "") & " <= @" & subCol.Name)
                            Else
                                If subCol.Name.ToString.ToLower.Contains("start") Or subCol.Name.ToString.ToLower.Contains("end") Then
                                    If subCol.Name.ToString.ToLower.Contains("start") Then
                                        subFilterString.Add(subCol.Name & " >= @" & subCol.Name)
                                    Else
                                        subFilterString.Add(subCol.Name & " <= @" & subCol.Name)
                                    End If
                                Else
                                    subFilterString.Add(subCol.Name & " = @" & subCol.Name)
                                End If
                            End If
                        Else
                            subFilterString.Add(subCol.Name & " = @" & subCol.Name)
                        End If
                    Next
                    filterString.Add(" ID IN (SELECT ID_" & m.TableName.Substring(1) & " FROM v" & m2.TableName.Substring(1) & " WHERE " & String.Join(" AND ", subFilterString) & ")")
                Next


            End If
        End If
        
        Using sqlConn As SqlClient.SqlConnection = New SqlClient.SqlConnection(ConnectionString)
            sqlConn.Open()
            Dim cmdtxt As String
            Dim dtSourceArr() As String = {}
            Dim viewName As String = ""
            Dim WhereString As String = ""
            Dim SearchllValue As String = ""
            Dim idx As Integer = 0
            If IsSearchFilter Then
                dtSourceArr = m.ColumnValue("DataSource").ToString.Split(" ")
                idx = dtSourceArr.ToList().FindIndex(Function(x) x.Contains("dbo."))
                If dtSourceArr.Length = 1 Then
                    viewName = dtSourceArr(0).ToString().Replace("dbo.", "")
                ElseIf dtSourceArr.Length > 1 Then
                    viewName = dtSourceArr(idx).ToString.Replace(")", "").Replace("dbo.", "")
                End If
                For Each z In filter.Where(Function(x) x.Key = "SearchAll")
                    SearchllValue = z.Value.ToString.Replace(Chr(160), Chr(32))
                Next
                WhereString = getTable("EXEC dbo.pSearchAllColumns '" & m.getListColumns & "', '" & SearchllValue & "', '" & viewName & "'").Rows(0)("StrQuery")
            End If

            If CBool(IsNull(m.ColumnValue("WithPagination"), "0")) AndAlso pageSize <> "All" Then
                If IsSearchFilter Then
                    cmdtxt = "SELECT " & m.getListColumns & " FROM " & SetDataSourceFilter(m, 0, 0, HttpContext.Current.Session) & " WHERE " & WhereString & " ORDER BY " & field & " " & direction & _
                   "  OFFSET " & (currentPage - 1) * pageSize & _
                   " ROWS FETCH NEXT " & pageSize & " ROWS ONLY"
                Else
                    cmdtxt = "SELECT " & m.getListColumns & " FROM " & SetDataSourceFilter(m, 0, 0, HttpContext.Current.Session) & If(filterString.Count = 0, "", " WHERE " & String.Join(" AND ", filterString)) & " ORDER BY " & field & " " & direction & _
                   "  OFFSET " & (currentPage - 1) * pageSize & _
                   " ROWS FETCH NEXT " & pageSize & " ROWS ONLY"
                End If
            Else
                If IsSearchFilter Then
                    cmdtxt = "SELECT " & m.getListColumns & " FROM " & SetDataSourceFilter(m, 0, rID, HttpContext.Current.Session) & " WHERE " & WhereString & " ORDER BY " & field & " " & direction
                Else
                    cmdtxt = "SELECT " & m.getListColumns & " FROM " & SetDataSourceFilter(m, 0, rID, HttpContext.Current.Session) & If(filterString.Count = 0, "", " WHERE " & String.Join(" AND ", filterString)) & " ORDER BY " & field & " " & direction
                End If
            End If
            Using sqlCommand As New SqlCommand(cmdtxt, sqlConn)
                For Each x In filter.Where(Function(i) i.Key <> "Detail_Menu" AndAlso i.Key <> "SearchAll" AndAlso Not i.Value Is Nothing AndAlso Not IsDBNull(i.Value) AndAlso Trim(i.Value.ToString).Length > 0)
                    Dim dataType As System.Type
                    If x.Key.Contains("From_") OrElse x.Key.Contains("To_") Then
                        dataType = GetDataType(m.dtColumns.Select("Name = '" & x.Key.Replace("From_", "").Replace("To_", "") & "'")(0).Item("colDataType").ToString)
                    Else
                        dataType = GetDataType(m.dtColumns.Select("Name = '" & x.Key & "'")(0).Item("colDataType").ToString)
                    End If

                    sqlCommand.Parameters.AddWithValue("@" & x.Key, CTypeDynamic(x.Value.ToString.Replace(Chr(160), Chr(32)), dataType))
                Next

                ' DETAIL FILTER
                If filter.Where(Function(i) i.Key = "Detail_Menu").Count > 0 Then
                    Dim x = DirectCast(filter.Where(Function(i) i.Key = "Detail_Menu").First().Value, JObject)
                    Dim subColDataType As System.Type

                    For Each item In x
                        Dim subFilterString As New List(Of String)
                        Dim m2 As GSWEB.MenuCollection.Menu = mCollection.GetMenu(CInt(item.Key.ToString))

                        For Each subCol As JProperty In item.Value
                            subColDataType = GetDataType(m2.dtColumns.Select("Name = '" & subCol.Name & "'")(0).Item("colDataType").ToString)
                            sqlCommand.Parameters.AddWithValue("@" & subCol.Name, CTypeDynamic(subCol.Value.ToString.Replace(Chr(160), Chr(32)), subColDataType))
                        Next
                    Next


                End If


                If CBool(m.ColumnValue("HasOpen")) Then
                    data = serialize(sqlCommand.ExecuteReader, True, HttpContext.Current.Session, m.dtColumns.AsEnumerable().Where(Function(c) IsNull(c.Item("ID_WebMenuControlTypes"), 0) = 27).Select(Function(c) CStr(c.Item("Name"))).ToList())
                Else
                    data = serialize(sqlCommand.ExecuteReader)
                End If

            End Using

            ' COUNT
            dataSource = "SELECT COUNT(ID) FROM " & SetDataSourceFilter(m, 0, rID, HttpContext.Current.Session) & If(filterString.Count = 0, "", " WHERE " & String.Join(" AND ", filterString))
            Using sqlCommand As New SqlCommand(dataSource, sqlConn)
                For Each x In filter.Where(Function(i) i.Key <> "Detail_Menu" AndAlso i.Key <> "SearchAll" AndAlso Not i.Value Is Nothing AndAlso Not IsDBNull(i.Value) AndAlso Trim(i.Value.ToString).Length > 0)
                    Dim dataType As System.Type
                    If x.Key.Contains("From_") OrElse x.Key.Contains("To_") Then
                        dataType = GetDataType(m.dtColumns.Select("Name = '" & x.Key.Replace("From_", "").Replace("To_", "") & "'")(0).Item("colDataType").ToString)
                    Else
                        dataType = GetDataType(m.dtColumns.Select("Name = '" & x.Key & "'")(0).Item("colDataType").ToString)
                    End If

                    sqlCommand.Parameters.AddWithValue("@" & x.Key, CTypeDynamic(x.Value.ToString.Replace(Chr(160), Chr(32)), dataType))
                Next

                ' DETAIL FILTER
                If filter.Where(Function(i) i.Key = "Detail_Menu").Count > 0 Then
                    Dim x = DirectCast(filter.Where(Function(i) i.Key = "Detail_Menu").First().Value, JObject)
                    Dim subColDataType As System.Type

                    For Each item In x
                        Dim subFilterString As New List(Of String)
                        Dim m2 As GSWEB.MenuCollection.Menu = mCollection.GetMenu(CInt(item.Key.ToString))

                        For Each subCol As JProperty In item.Value
                            subColDataType = GetDataType(m2.dtColumns.Select("Name = '" & subCol.Name & "'")(0).Item("colDataType").ToString)
                            sqlCommand.Parameters.AddWithValue("@" & subCol.Name, CTypeDynamic(subCol.Value.ToString.Replace(Chr(160), Chr(32)), subColDataType))
                        Next
                    Next

                End If

                totalServerItems = sqlCommand.ExecuteScalar
            End Using

            sqlConn.Close()

        End Using

        'data = getJSONTable("SELECT " & m.getListColumns & " FROM " & SetDataSourceFilter(m, 0, 0, HttpContext.Current.Session) & " ORDER BY " & field & " " & direction & _
        '                    "  OFFSET " & (currentPage - 1) * pageSize & _
        '                    " ROWS FETCH NEXT " & pageSize & " ROWS ONLY")

        'totalServerItems = ExecScalarNoParams(dataSource)

        Try
            ret = "{"
            ret &= "data:" & data & ","
            ret &= "totalServerItems:" & totalServerItems
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
            logError(ex, mID)
        End Try

        Return New HttpResponseMessage() With { _
           .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function AddNewRow(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim drMaster As CaseInsensitiveDictionary = JsonConvert.DeserializeObject(Of CaseInsensitiveDictionary)(obj("Master"))
        Dim ret As String = String.Empty,
            data As String = String.Empty,
            r As New JObject

        Dim mID As Integer = obj("mID").ToObject(Of Integer)(),
            m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(mID)

        Try
            ret = "{"
            ret &= "data:" & dictionaryToJSON(m.getDefaultRowData(), 0, HttpContext.Current.Session, mCollection.GetMenu(m.ParentID).TableName.ToString, drMaster) & ","
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
            logError(ex, mID)
        End Try

        Return New HttpResponseMessage() With { _
           .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function getAutoCompleteItems(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim ret As String = String.Empty,
            data As String = String.Empty,
            r As New JObject

        Dim mID As Integer = obj("mID").ToObject(Of Integer)(),
            colID As Integer = obj("colID").ToObject(Of Integer)(),
            value As String = obj("value").ToObject(Of String)(),
            m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(mID)
        Dim dr As DataRow = m.dtColumns.Select("ID = " & colID)(0)

        Dim filter As String = IsNull(dr("Filter"), "")
        Try
            ret = "{"
            Using ttCon As New SqlClient.SqlConnection(ConnectionString)
                ttCon.Open()
                Using ttCmd As New SqlCommand("SELECT " & IsNull(dr("DisplayID"), "ID") & " AS ID," & IsNull(dr("DisplayMember"), "Name") & " AS Name FROM " & IsNull(dr("TableName"), "v" & dr("Name").ToString.Substring(3)) & " WHERE " & IsNull(dr("DisplayMember"), "Name") & " LIKE @value + '%'" & IIf(filter = "", "", " AND " & SetFilter(m, filter, 0, 0, HttpContext.Current.Session)) & " ORDER BY " & IsNull(dr("DisplayMember"), "Name") & " ASC", ttCon)
                    ttCmd.Parameters.AddWithValue("value", value)
                    ret &= "items : " & serialize(ttCmd.ExecuteReader)
                End Using
                ttCon.Close()
            End Using

            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
            logError(ex, mID)
        End Try

        Return New HttpResponseMessage() With { _
           .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function PublishWebsite() As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim message As String = "Publishing website completed."
        Try '' TODO : SYSTEM LANG PDE MAGPUBLISH
            buildApp()
            buildScripts()
        Catch ex As Exception
            message = ex.Message
            logError(ex)
        End Try
        Return New HttpResponseMessage() With { _
            .Content = New JsonContent(JObject.Parse("{ message : '" & message & "' }")) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function Publish(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim message As String = "Publishing website completed."
        Try '' TODO : SYSTEM LANG PDE MAGPUBLISH
            ClearMenu()
            Dim drMaster As CaseInsensitiveDictionary = JsonConvert.DeserializeObject(Of CaseInsensitiveDictionary)(obj("Master"))
            Dim m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(CInt(drMaster("ID")))
            buildApp()
            Dim controllerPath As String = HttpContext.Current.Server.MapPath("~/Partials/Controller/c" & m.MenuID & ".js"),
                templatePath As String = HttpContext.Current.Server.MapPath("~/Partials/View/c" & m.MenuID & ".html")
            Writer(controllerPath, m.buildController)
            Writer(templatePath, m.buildTemplate)

            For Each c As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID)
                controllerPath = HttpContext.Current.Server.MapPath("~/Partials/Controller/c" & c.MenuID & ".js")
                templatePath = HttpContext.Current.Server.MapPath("~/Partials/View/c" & c.MenuID & ".html")
                Writer(controllerPath, c.buildController)
                Writer(templatePath, c.buildTemplate)
            Next

            If m.ColumnValue("ID_WebMenuTypes") = 3 Then
                controllerPath = HttpContext.Current.Server.MapPath("~/Partials/Controller/c" & m.ParentID & ".js")
                templatePath = HttpContext.Current.Server.MapPath("~/Partials/View/c" & m.ParentID & ".html")
                Writer(controllerPath, mCollection.GetMenu(m.ParentID).buildController)
                Writer(templatePath, mCollection.GetMenu(m.ParentID).buildTemplate)
            End If

        Catch ex As Exception
            message = ex.Message
            logError(ex)
        End Try
        Return New HttpResponseMessage() With { _
            .Content = New JsonContent(JObject.Parse("{ message : '" & message & "' }")) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function ClearMenu() As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim message As String = "Clearing menu completed.", memCache As New MemoryCacher
        Try '' TODO : SYSTEM LANG PDE MAGCLEAR
            ClearCollection()
            LoadMenuSet()
            memCache.Clear()
        Catch ex As Exception
            message = ex.Message
        End Try
        Return New HttpResponseMessage() With { _
            .Content = New JsonContent(JObject.Parse("{ message : '" & message & "' }")) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    Public Function HasAccess(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        Dim mID As Integer = obj("mID").ToObject(Of Integer)()
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)

        Return New HttpResponseMessage() With { _
            .Content = New StringContent("True", System.Text.Encoding.UTF8, "application/json") _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function getSession(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim ret As String = String.Empty
        Dim dictionary As Dictionary(Of String, Object) = SessionToDictionary(HttpContext.Current.Session)
        Dim e = dictionary.[Select](Function(d) String.Format("{0}:{1}", d.Key, IsNull(d.Value, 0)))

        ret = "{session:" & "{" & String.Join(",", e) + "}" & "}"
        Return New HttpResponseMessage() With { _
          .Content = New JsonContent(JObject.Parse(ret)) _
       }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function loadMenu() As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim memCache As New MemoryCacher, ret As String = String.Empty
        ' If Not memCache.Contains("MENU_" & HttpContext.Current.Session("ID_UserGroup")) Then
        Dim am As New List(Of AccessMenu)
        If Not Convert.ToBoolean(HttpContext.Current.Session("isFirstLog")) Then
            Dim WebMenus As New List(Of UserGroupWebMenus)
            Using sqlConn As New SqlConnection(ConnectionString)
                sqlConn.Open()
                Using SqlCommand As New SqlCommand("SELECT * FROM dbo.fGetWebMenus(" & HttpContext.Current.Session("ID_UserGroup") & ")", sqlConn)
                    Dim sqlDr As SqlDataReader = SqlCommand.ExecuteReader
                    While sqlDr.Read
                        WebMenus.Add(New UserGroupWebMenus(sqlDr("ID"), sqlDr("Name"), IsNull(sqlDr("Label"), ""), IsNull(sqlDr("ID_WebMenus"), 0), IsNull(sqlDr("ShowMenuIf"), ""), IsNull(sqlDr("SeqNo"), 0)))
                    End While
                    sqlDr.Close()
                    SqlCommand.Dispose()
                End Using
                sqlConn.Close()
                sqlConn.Dispose()
            End Using
            If System.Configuration.ConfigurationManager.AppSettings("UseMenu") = 0 Then
                For Each dr As UserGroupWebMenus In WebMenus.Where(Function(x) x.ID_WebMenus = 0).OrderBy(Function(x) x.SeqNo) ' dt.Select("ID_WebMenus IS NULL", "SeqNo ASC")
                    Dim a As New AccessMenu(dr.ID, dr.Name, IsNull(dr.Label, ""), IsNull(dr.ID_WebMenus, 0))
                    For Each dr2 As UserGroupWebMenus In WebMenus.Where(Function(x) x.ID_WebMenus = dr.ID).OrderBy(Function(x) x.SeqNo)
                        Dim ShowMenu As Boolean = True
                        If dr2.ShowMenuIf <> "" Then
                            ShowMenu = ShowMenuIf(dr2.ShowMenuIf, HttpContext.Current.Session)
                        End If
                        If ShowMenu Then buildChildMenu2(a, dr2, WebMenus)
                    Next

                    am.Add(a)

                Next
            Else
                For Each dr As UserGroupWebMenus In WebMenus.OrderBy(Function(x) x.SeqNo) ' dt.Select("ID_WebMenus IS NULL", "SeqNo ASC")
                    Dim a As New AccessMenu(dr.ID, dr.Name, IsNull(dr.Label, ""), IsNull(dr.ID_WebMenus, 0))
                    '!!!!!!!!!!! DISABLE FOR NEW MENU
                    For Each dr2 As UserGroupWebMenus In WebMenus.Where(Function(x) x.ID_WebMenus = dr.ID).OrderBy(Function(x) x.SeqNo)
                        Dim ShowMenu As Boolean = True
                        If dr2.ShowMenuIf <> "" Then
                            ShowMenu = ShowMenuIf(dr2.ShowMenuIf, HttpContext.Current.Session)
                        End If
                        If ShowMenu Then buildChildMenu2(a, dr2, WebMenus)
                    Next

                    am.Add(a)

                Next
            End If
            'For Each m As GSWEB.MenuCollection.Menu In mCollection.GetRootMenu.Where(Function(x) x.ColumnValue("IsVisible") = True)
            '    If m.HasAccess(HttpContext.Current.Session("ID_UserGroup")) Then
            '        If mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 1 And x.ColumnValue("IsVisible") = True).Count > 0 Then
            '            Dim a As New AccessMenu(m.MenuID, IsNull(m.ColumnValue("Label"), m.Name))
            '            For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 1 And x.ColumnValue("IsVisible") = True)
            '                If m2.HasAccess(HttpContext.Current.Session("ID_UserGroup")) Then
            '                    Dim ShowMenu As Boolean = True
            '                    If IsNull(m2.ColumnValue("ShowMenuIf"), "") <> "" Then
            '                        ShowMenu = ShowMenuIf(m2.ColumnValue("ShowMenuIf"), HttpContext.Current.Session)
            '                    End If
            '                    If ShowMenu Then buildChildMenu(a, m2)
            '                End If
            '            Next
            '            am.Add(a)
            '        Else
            '            Dim a As New AccessMenu(m.MenuID, IsNull(m.ColumnValue("Label"), m.Name))
            '            am.Add(a)
            '        End If
            '    End If
            'Next
        End If


        'memCache.Add("MENU_" & HttpContext.Current.Session("ID_UserGroup"), JsonConvert.SerializeObject(am), DateTime.Now.AddDays(1))
        '  End If
        'GetIndirectReference
        Dim IsNotification As String = System.Configuration.ConfigurationManager.AppSettings("EnableNotification").ToString.ToLower
        ret = "{"
        ret &= "menus:" & JsonConvert.SerializeObject(am) & ","
        ret &= "EnableNotification:" & IsNotification & ","
        ret &= "UseMenu:" & System.Configuration.ConfigurationManager.AppSettings("UseMenu") & ","
        If HttpContext.Current.Session("ID_UserGroup") = HttpContext.Current.Session("ApplicantUserGroup") Then

            ret &= "employee:" & getJSONTableRow("SELECT * FROM dbo.fGetApplicantBasicInfo(" & IsNull(HttpContext.Current.Session("ID_Persona"), 0) & ")") & ","
        Else
            Dim a As New List(Of String)
            a.Add("ID_User")
            ret &= "employee:" & getJSONTableRow("SELECT * FROM dbo.fWebGetUserInfo(" & IsNull(HttpContext.Current.Session("ID_User"), 0) & ")", False, HttpContext.Current.Session, a) & ","
        End If
        ret &= "}"
        '.Content = New StringContent(memCache.GetValue("MENU_" & HttpContext.Current.Session("ID_UserGroup")).ToString, System.Text.Encoding.UTF8, "application/json")
        Return New HttpResponseMessage() With { _
            .Content = New JsonContent(JObject.Parse(ret)) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    Public Function Banner() As HttpResponseMessage
        'If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim ret As String = String.Empty
        Dim banners() As String = Directory.GetFiles(HttpContext.Current.Server.MapPath("~/Resources/Banner/"))
        ret = "{images:["
        For x = 0 To banners.Length - 1
            ret &= "{image:'" & Path.GetFileName(banners(x)) & "'},"
        Next
        ret &= "]}"
        Return New HttpResponseMessage() With { _
            .Content = New JsonContent(JObject.Parse(ret)) _
        }
    End Function

    Private Enum MessageType
        Success = 1
        BeforeExecute = 2
        AfterExecute = 3
    End Enum

    'Private Sub buildChildMenu(a As AccessMenu, m As GSWEB.MenuCollection.Menu)
    '    If mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 1).Count > 0 Then

    '        Dim a2 As New AccessMenu(m.MenuID, IsNull(m.ColumnValue("Label"), m.Name))
    '        For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 1 And x.ColumnValue("IsVisible") = True)
    '            If m2.HasAccess(HttpContext.Current.Session("ID_UserGroup")) Then
    '                Dim ShowMenu As Boolean = True
    '                If IsNull(m2.ColumnValue("ShowMenuIf"), "") <> "" Then
    '                    ShowMenu = ShowMenuIf(m2.ColumnValue("ShowMenuIf"), HttpContext.Current.Session)
    '                End If
    '                If ShowMenu Then buildChildMenu(a2, m2)
    '            End If
    '        Next
    '        a.Add(a2)
    '    Else
    '        Dim a2 As New AccessMenu(m.MenuID, IsNull(m.ColumnValue("Label"), m.Name))
    '        a.Add(a2)
    '    End If
    'End Sub

    Private Sub buildChildMenu2(a As AccessMenu, dr As UserGroupWebMenus, WebMenus As List(Of UserGroupWebMenus))
        Dim a2 As New AccessMenu(dr.ID, dr.Name, IsNull(dr.Label, ""), IsNull(dr.ID_WebMenus, 0))
        For Each dr2 As UserGroupWebMenus In WebMenus.Where(Function(x) x.ID_WebMenus = dr.ID).OrderBy(Function(x) x.SeqNo)
            Dim ShowMenu As Boolean = True
            If dr2.ShowMenuIf <> "" Then
                ShowMenu = ShowMenuIf(dr2.ShowMenuIf, HttpContext.Current.Session)
            End If
            If ShowMenu Then buildChildMenu2(a2, dr2, WebMenus)
        Next
        a.Add(a2)

    End Sub

    Private Class AccessMenu
        Public Property ID As Integer
        Public Property Name As String
        Public Property Label As String
        Public Property Children As New List(Of AccessMenu)
        Public Property ID_WebMenus As Integer

        Sub New(ID As Integer, Name As String, Label As String, ID_WebMenus As Integer)
            Me.ID = ID
            Me.Name = Name
            Me.Label = Label
            Me.ID_WebMenus = ID_WebMenus

        End Sub

        Public Sub Add(a As AccessMenu)
            Me.Children.Add(a)
        End Sub
    End Class

    Public Class JsonContent
        Inherits HttpContent
        Private ReadOnly _value As JToken

        Public Sub New(value As JToken)
            _value = value
            Headers.ContentType = New MediaTypeHeaderValue("application/json")
        End Sub

        Protected Overrides Function SerializeToStreamAsync(stream As Stream, context As TransportContext) As Task
            Dim jw = New JsonTextWriter(New StreamWriter(stream)) With { _
                .Formatting = Formatting.None _
            }
            _value.WriteTo(jw)
            jw.Flush()
            Return Task.FromResult(Of Object)(Nothing)
        End Function

        Protected Overrides Function TryComputeLength(ByRef length As Long) As Boolean
            length = -1
            Return False
        End Function

    End Class

    Public Class CompressionHelper
        Public Shared Function DeflateByte(str As Byte()) As Byte()
            If str Is Nothing Then
                Return Nothing
            End If
            Using output = New MemoryStream()
                Using compressor = New Ionic.Zlib.DeflateStream(output, Ionic.Zlib.CompressionMode.Compress, Ionic.Zlib.CompressionLevel.BestSpeed)
                    compressor.Write(str, 0, str.Length)
                End Using
                Return output.ToArray()
            End Using
        End Function
    End Class

    Public Function UserHasAccessByUserGroup(m As GSWEB.MenuCollection.Menu) As Boolean
        If m.ColumnValue("ID_WebMenuTypes") = 1 AndAlso Not m.HasAccess(HttpContext.Current.Session("ID_UserGroup")) Then
            Return False
        End If
        Return True
    End Function

    Public Function UserHasAccessByData(m As GSWEB.MenuCollection.Menu, rID As Integer, pID As Integer) As Boolean
        ''DATA
        If rID = 0 AndAlso CBool(m.ColumnValue("HasNew")) Then Return True
        If m.ColumnValue("ID_WebMenuTypes") = 3 OrElse m.ColumnValue("ID_WebMenuTypes") = 5 OrElse m.ColumnValue("ID_WebMenuTypes") = 17 Then
            Using ttCon As New SqlClient.SqlConnection(ConnectionString)
                Dim dataSource As String = "SELECT  CASE WHEN EXISTS(SELECT ID FROM (SELECT ID FROM " &
                                           SetDataSourceFilter(m, pID, rID, HttpContext.Current.Session) & ") xxx WHERE xxx.ID = " & rID &
                                           ") THEN 1 ELSE 0 END",
                    ret As Boolean = True
                ttCon.Open()
                Using ttCmd As New SqlCommand(dataSource, ttCon)
                    ret = ttCmd.ExecuteScalar
                End Using
                ttCon.Close()
                Return ret
            End Using
        End If
        Return True
    End Function

    <HttpGet>
    Public Function downloadFile(dfile As String, filename As String) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        ''TODO: User lang na may access sa file ung pdeng magdownload.
        Dim result As New HttpResponseMessage
        Dim localFilePath = HttpContext.Current.Server.MapPath("~/" + dfile)

        If String.IsNullOrEmpty(dfile) Then
            result = Request.CreateResponse(HttpStatusCode.BadRequest)
        ElseIf Not File.Exists(localFilePath) Then
            result = Request.CreateResponse(HttpStatusCode.Gone)
        Else
            result = Request.CreateResponse(HttpStatusCode.OK)
            result.Content = New StreamContent(New FileStream(localFilePath, FileMode.Open, FileAccess.Read))
            result.Content.Headers.ContentType = New MediaTypeHeaderValue("application/octet-stream")
            result.Content.Headers.ContentDisposition = New System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
            result.Content.Headers.ContentDisposition.FileName = filename
        End If

        Return result
    End Function

    <HttpGet>
    Public Function GenerateExcelTemplate(mID As Integer, btnID As Integer, ID As String, Master As String) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        ''TODO: User lang na may access sa file ung pdeng magdownload.
        Dim strObj As String = unEscape(Master)
        Master = strObj
        Dim obj As JObject = JsonConvert.DeserializeObject(Master)
        Dim drMaster As CaseInsensitiveDictionary = JsonConvert.DeserializeObject(Of CaseInsensitiveDictionary)(Master)
        Dim m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(mID),
            rID As Integer = 0,
            btn As DataRow = m.dtButtons.Select("ID = " & btnID)(0),
            dfile As String = IsNull(btn("ImportFile"), ""),
            ExcelTemplatePath As String = dtSetting.AsEnumerable().Where(Function(x) x("Name") = "ExcelTemplatePath").FirstOrDefault()("Value")

        If ID <> "0" Then
            Try
                rID = toBase10(ID) 'CInt(GetDirectReference(result.FormData.Item("rID"), HttpContext.Current.Session))
            Catch ex As Exception
                logError(ex, mID)
                Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
            End Try
        End If

        Dim result As New HttpResponseMessage

        Try
            If Not String.IsNullOrEmpty(dfile) Then
                Dim localFilePath = Path.GetFullPath(ExcelTemplatePath + dfile)
                result = Request.CreateResponse(HttpStatusCode.OK)
                result.Content = New StreamContent(New FileStream(localFilePath, FileMode.Open, FileAccess.Read))
                result.Content.Headers.ContentType = New MediaTypeHeaderValue("application/octet-stream")
                result.Content.Headers.ContentDisposition = New System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                result.Content.Headers.ContentDisposition.FileName = dfile

            Else
                ''DYNAMIC TEMPLATE
                Dim workBook As HSSFWorkbook = New HSSFWorkbook(),
                    sheet As HSSFSheet,
                    cs2 As HSSFCellStyle = workBook.CreateCellStyle,
                    font As HSSFFont = workBook.CreateFont

                cs2.FillForegroundColor = HSSFColor.Grey50Percent.Index
                cs2.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin
                cs2.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin
                cs2.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin
                cs2.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin
                font.Boldweight = NPOI.SS.UserModel.FontBoldWeight.Bold

                Using sqlConn As New SqlConnection(ConnectionString)
                    If Not IsDBNull(btn("CommandText")) Then
                        sqlConn.Open()
                        Dim cmdtxt As String = btn("CommandText").ToString
                        Using sqlCommand As New SqlCommand(cmdtxt, sqlConn)
                            If Not m.dtButtonParameters(btn("ID").ToString) Is Nothing Then
                                For Each dr2 As DataRow In m.dtButtonParameters(btn("ID").ToString).Rows
                                    Select Case dr2("Value").ToString.Substring(0, 1)
                                        Case "@"
                                            If dr2("Value").ToString = "@ID_WebMenus" Then
                                                sqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, mID.ToString)
                                            Else
                                                sqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, HttpContext.Current.Session(dr2("Value").ToString.Substring(1)).ToString)
                                            End If
                                        Case "#"
                                        Case "$"
                                        Case Else
                                            'IPASA ANG MASTER
                                            If dr2("Value").ToString = "ID" Then
                                                sqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, rID)
                                            Else
                                                sqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, CTypeDynamic(HttpUtility.HtmlEncode(drMaster(dr2("Value").ToString).ToString.Replace(Chr(160), Chr(32))), GetDataType(m.dtColumns.Select("Name = '" + dr2("Value").ToString + "'")(0).Item("colDataType").ToString)))
                                                'sqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, CTypeDynamic(drMaster(dr2("Value").ToString), GetDataType(m.dtColumns.Select("Name = '" + dr2("Value").ToString + "'")(0).Item("colDataType").ToString)))
                                            End If
                                    End Select
                                Next
                            End If
                            Dim dt As New DataTable
                            Dim da As New SqlDataAdapter
                            da.SelectCommand = sqlCommand
                            da.Fill(dt)
                            sheet = workBook.CreateSheet("Sheet1")
                            FillExcelSheet(workBook, sheet, dt, btn("ID_TargetWebMenus"))
                        End Using
                    Else
                        If btn("ID_TargetWebMenus").ToString.Length > 0 Then
                            Dim targetMenu As GSWEB.MenuCollection.Menu = mCollection.GetMenu(btn("ID_TargetWebMenus"))
                            Dim f As HSSFFont = workBook.CreateFont
                            f.Boldweight = NPOI.SS.UserModel.FontBoldWeight.Bold
                            f.FontHeightInPoints = 9
                            f.FontName = "Arial"

                            sheet = workBook.CreateSheet("Sheet1")
                            Dim hrow As HSSFRow = sheet.CreateRow(0)
                            Dim csRequired As HSSFCellStyle = workBook.CreateCellStyle
                            csRequired.SetFont(font)
                            csRequired.FillForegroundColor = HSSFColor.Pink.Index
                            csRequired.FillPattern = FillPattern.SolidForeground
                            Dim csNotRequired As HSSFCellStyle = workBook.CreateCellStyle
                            csNotRequired.FillForegroundColor = HSSFColor.Grey25Percent.Index
                            csNotRequired.FillPattern = FillPattern.SolidForeground
                            csNotRequired.SetFont(font)
                            Dim ctr As Integer = 0
                            For Each dr As DataRow In targetMenu.dtColumns.Select("IsImported = 1", "SeqNo ASC")
                                hrow.CreateCell(ctr).SetCellValue(dr("Name").ToString)
                                Dim rowcell As HSSFCell = hrow.GetCell(ctr)
                                If IsNull(dr("HelperText"), "") <> "" Then
                                    Dim HelperText() As String = dr("HelperText").ToString.Split(";")
                                    Dim idr As IDrawing = sheet.CreateDrawingPatriarch()
                                    Dim cmt As IComment = idr.CreateCellComment(New HSSFClientAnchor(0, 0, 0, 0, 4, 2, 10, HelperText.Length + 5))
                                    Dim ht As String = ""
                                    For Each hStr As String In HelperText
                                        ht &= hStr + vbNewLine
                                    Next
                                    Dim richtext As HSSFRichTextString = New HSSFRichTextString(ht)
                                    cmt.String = richtext
                                    rowcell.CellComment = cmt
                                End If

                                If CBool(IsNull(dr("IsRequired"), False)) Then
                                    rowcell.CellStyle = csRequired
                                Else
                                    rowcell.CellStyle = csNotRequired
                                End If
                                sheet.AutoSizeColumn(ctr)
                                ctr += 1
                            Next
                        Else
                            Dim f As HSSFFont = workBook.CreateFont
                            f.Boldweight = NPOI.SS.UserModel.FontBoldWeight.Bold
                            f.FontHeightInPoints = 9
                            f.FontName = "Arial"
                            For Each targetMenu As GSWEB.MenuCollection.Menu In mCollection.GetChild(mID)
                                Dim dtCols() As DataRow = targetMenu.dtColumns.Select("IsImported = 1", "SeqNo ASC")
                                If dtCols.Count = 0 Then Continue For
                                sheet = workBook.CreateSheet(targetMenu.Name)
                                Dim hrow As HSSFRow = sheet.CreateRow(0)
                                Dim csRequired As HSSFCellStyle = workBook.CreateCellStyle
                                csRequired.SetFont(font)
                                csRequired.FillForegroundColor = HSSFColor.Pink.Index
                                csRequired.FillPattern = FillPattern.SolidForeground
                                Dim csNotRequired As HSSFCellStyle = workBook.CreateCellStyle
                                csNotRequired.FillForegroundColor = HSSFColor.Grey25Percent.Index
                                csNotRequired.FillPattern = FillPattern.SolidForeground
                                csNotRequired.SetFont(font)
                                Dim ctr As Integer = 0
                                For Each dr As DataRow In dtCols
                                    hrow.CreateCell(ctr).SetCellValue(dr("Name").ToString)
                                    Dim rowcell As HSSFCell = hrow.GetCell(ctr)

                                    If CBool(IsNull(dr("IsRequired"), False)) Then
                                        rowcell.CellStyle = csRequired
                                    Else
                                        rowcell.CellStyle = csNotRequired
                                    End If
                                    sheet.AutoSizeColumn(ctr)
                                    ctr += 1
                                Next
                            Next
                        End If
                    End If

                    If Not IsDBNull(btn("ExcelSubDataSource")) AndAlso Not IsDBNull(btn("FileReferenceSort")) Then

                        Using sqlCommand As New SqlCommand(SetFilter(m, btn("ExcelSubDataSource").ToString, , rID, HttpContext.Current.Session), sqlConn)
                            Dim fileReferenceSort As String() = btn("FileReferenceSort").ToString.Split(";")
                            Dim da As New SqlDataAdapter, ctr As Integer = 0
                            Dim ds As New DataSet
                            da.SelectCommand = sqlCommand
                            da.Fill(ds)
                            For Each dt As DataTable In ds.Tables
                                Dim sheet2 As HSSFSheet = workBook.CreateSheet(fileReferenceSort(ctr))
                                FillExcelSheet(workBook, sheet2, dt)
                                ctr += 1
                            Next

                            da.Dispose()
                        End Using

                    End If
                    Using exportData As MemoryStream = New MemoryStream()
                        workBook.Write(exportData)
                        result = Request.CreateResponse(HttpStatusCode.OK)
                        result.Content = New ByteArrayContent(exportData.ToArray())
                        result.Content.Headers.ContentType = New MediaTypeHeaderValue("application/vnd.ms-excel")
                        result.Content.Headers.ContentDisposition = New System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                        result.Content.Headers.ContentDisposition.FileName = m.Name.Replace(" ", "-") & ".xls"
                    End Using
                    sqlConn.Close()
                End Using
            End If
        Catch ex As Exception
            logError(ex, mID)
            Return New HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
        End Try

        Return result
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Async Function ImportExcelTemplate() As Task(Of HttpResponseMessage) ' ByVal obj As Newtonsoft.Json.Linq.JObject
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        If Not Request.Content.IsMimeMultipartContent() Then
            Me.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType)
        End If

        Dim root = HttpContext.Current.Server.MapPath("~/Upload/ExcelTemplateUpload/")
        Directory.CreateDirectory(root)
        Dim provider = New MultipartFormDataStreamProvider(root),
            result = Await Request.Content.ReadAsMultipartAsync(provider)
        Dim mID As Integer = result.FormData.Item("mID"),
            btnID As Integer = result.FormData.Item("btnID"),
            rID As Integer = 0,
            drMaster As CaseInsensitiveDictionary = JsonConvert.DeserializeObject(Of CaseInsensitiveDictionary)(result.FormData.Item("Master"))

        If result.FormData.Item("rID") <> "0" Then
            Try
                rID = toBase10(result.FormData.Item("rID")) 'CInt(GetDirectReference(result.FormData.Item("rID"), HttpContext.Current.Session))
            Catch ex As Exception
                logError(ex, mID)
                Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
            End Try
        End If

        Dim ret As String = String.Empty,
            message As String = String.Empty,
            messageType As MessageType = messageType.Success,
            r As New JObject,
            data As String = String.Empty,
             m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(mID)


        Try
            ''Before Execute
            messageType = messageType.BeforeExecute
            ExecuteButtonValidation(m, 1, btnID, drMaster, rID, HttpContext.Current.Session)

            'Dim query As String = "SELECT " & columns.ToString.Trim(",") & " FROM [Sheet1$]"
            Using conn As OleDbConnection = New OleDbConnection
                Dim btn As DataRow = m.dtButtons.Select("ID = " & btnID)(0),
                    targetMenu As GSWEB.MenuCollection.Menu = mCollection.GetMenu(IsNull(btn("ID_TargetWebMenus"), 0)),
                    templatePath As String = HttpContext.Current.Server.MapPath("~/Upload/ExcelTemplateUpload/"),
                    uploadedFile As String = SaveUploadedFile(result.FileData.Item(0), "Upload/ExcelTemplateUpload/"),
                    connString As String = String.Empty,
                    columns As New Text.StringBuilder,
                    cols As DataRow()

                Try
                    If Not targetMenu Is Nothing Then
                        cols = targetMenu.dtColumns.Select("IsImported = 1 AND ID_WebMenuControlTypes NOT IN(6, 27)")

                        For Each col As DataRow In cols
                            columns.Append(col("Name") & ",")
                        Next

                        If Path.GetExtension(uploadedFile).Trim() = ".xls" Then
                            connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & templatePath & "/" & uploadedFile & ";Extended Properties=""Excel 8.0;HDR=Yes;IMEX=2"""
                        ElseIf Path.GetExtension(uploadedFile).Trim() = ".xlsx" Then
                            connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & templatePath & "/" & uploadedFile & ";Extended Properties=""Excel 12.0;HDR=Yes;IMEX=2"""
                        End If
                        conn.ConnectionString = connString
                        conn.Open()
                        Using OleDbCommand As OleDbCommand = New OleDbCommand("SELECT " & columns.ToString.Trim(",") & " FROM [Sheet1$]", conn)
                            Using da As OleDbDataAdapter = New OleDbDataAdapter(OleDbCommand)
                                Try
                                    Dim ds As New DataSet,
                                        dt As New DataTable
                                    da.Fill(ds)
                                    dt = ds.Tables(0)

                                    Dim cols2 As DataRow() = targetMenu.dtColumns.Select("Name <> 'ID' AND IsImported = 0 AND IsVisible = 1 AND ID_WebMenuControlTypes NOT IN(6, 27)")
                                    dt.Columns.Add("ID", System.Type.GetType("System.Int32"))
                                    For Each col As DataRow In cols2
                                        Dim newColumn As New Data.DataColumn(col("Name").ToString, GetDataType(col("colDataType").ToString))
                                        If IsNull(col("DefaultValue").ToString, "") <> "" Then
                                            newColumn.DefaultValue = defaultValues(col("DefaultValue").ToString, rID, HttpContext.Current.Session, m.TableName)
                                        End If
                                        dt.Columns.Add(newColumn)
                                    Next
                                    data = JsonConvert.SerializeObject(dt)
                                    message = IsNull(btn("Message"), "")
                                Catch ex As Exception
                                    message = ex.Message
                                Finally
                                    da.Dispose()
                                End Try

                            End Using
                        End Using
                    Else
                        If Path.GetExtension(uploadedFile).Trim() = ".xls" Then
                            connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & templatePath & "/" & uploadedFile & ";Extended Properties=""Excel 8.0;HDR=Yes;IMEX=2"""
                        ElseIf Path.GetExtension(uploadedFile).Trim() = ".xlsx" Then
                            connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & templatePath & "/" & uploadedFile & ";Extended Properties=""Excel 12.0;HDR=Yes;IMEX=2"""
                        End If
                        conn.ConnectionString = connString
                        conn.Open()

                        data &= "["
                        For Each cMenu As GSWEB.MenuCollection.Menu In mCollection.GetChild(mID)
                            data &= "{ ID: " & cMenu.MenuID & ", Data: "
                            cols = cMenu.dtColumns.Select("IsImported = 1 AND ID_WebMenuControlTypes NOT IN(6, 27)")

                            Dim q = (From c In cols
                                    Select c("Name")).ToArray

                            Using OleDbCommand As OleDbCommand = New OleDbCommand("SELECT " & String.Join(",", q) & " FROM [" & cMenu.Name & "$]", conn)
                                Using da As OleDbDataAdapter = New OleDbDataAdapter(OleDbCommand)
                                    Try
                                        Dim ds As New DataSet,
                                            dt As New DataTable
                                        da.Fill(ds)
                                        dt = ds.Tables(0)

                                        Dim cols2 As DataRow() = cMenu.dtColumns.Select("Name <> 'ID' AND IsImported = 0 AND IsVisible = 1 AND ID_WebMenuControlTypes NOT IN(6, 27)")
                                        dt.Columns.Add("ID", System.Type.GetType("System.Int32"))
                                        For Each col As DataRow In cols2
                                            Dim newColumn As New Data.DataColumn(col("Name").ToString, GetDataType(col("colDataType").ToString))
                                            dt.Columns.Add(newColumn)
                                        Next
                                        data &= JsonConvert.SerializeObject(dt) & ", "
                                    Catch ex As Exception
                                        data &= "{}, "
                                    Finally
                                        da.Dispose()
                                    End Try

                                End Using
                            End Using
                            data &= "}, "
                        Next
                        data &= "]"

                        message = IsNull(btn("Message"), "")
                    End If


                Catch ex As Exception
                    messageType = messageType.BeforeExecute
                    message = ex.Message
                    logError(ex, mID)
                Finally
                    conn.Close()
                End Try
                File.Delete(templatePath & "/" & uploadedFile)
            End Using

            ''After Execute
            messageType = messageType.AfterExecute
            ExecuteButtonValidation(m, 2, btnID, drMaster, rID, HttpContext.Current.Session)

        Catch ee As SqlException
            'messageType = messageType.BeforeExecute
            message = ee.Message
        Catch ex As Exception
            'messageType = messageType.BeforeExecute
            message = ex.Message
            logError(ex, mID)
        End Try

        Try
            ret = "{"
            ret &= "data : " & data & ","
            ret &= "message : '" & addStripSlashes(message) & "',"
            ret &= "messageType : '" & messageType & "'"
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "'}")
            logError(ex, mID)
        End Try


        Return New HttpResponseMessage() With { _
         .Content = New JsonContent(r) _
       }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function CascadingDropdown(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)



        Dim ret As String = String.Empty,
            Data As String = String.Empty,
            r As New JObject

        'If Not UserHasAccessByUserGroup(mCollection.GetMenu(mID)) Then Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
        '  If Not UserHasAccessByData(mCollection.GetMenu(mID), rID, 0) Then Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)

        Try


            Using sqlConn As New SqlConnection(ConnectionString)
                sqlConn.Open()
                Dim mID As Integer = obj("mID").ToObject(Of Integer)(),
                    colID As Integer = obj("colID").ToObject(Of Integer)(),
                    value As Integer = obj("value").ToObject(Of Integer)(),
                    m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(mID),
                    btn As DataRow = m.dtColumns.Select("ID = " & colID)(0),
                    tmp As String = btn("JSEvents").ToString.Split("=")(1),
                    tmp2 As String() = tmp.Split(";")
                Data &= "{"
                For Each a In tmp2(0).Split(",")

                    Dim targetColumn As DataRow = m.dtColumns.Select("ID = " & a)(0)
                    Dim cmdTxt As String = "SELECT " & IsNull(targetColumn("DisplayID"), "ID") & " AS ID," & IsNull(targetColumn("DisplayMember"), "Name") & " AS Name FROM " & IsNull(targetColumn("TableName"), "t" & targetColumn("Name").ToString.Substring(3)) & " WHERE " & tmp2(1) & " = @value" & " ORDER BY " & IsNull(targetColumn("DisplayMember"), "Name") & " ASC"
                    Using sqlCommand As New SqlCommand(cmdTxt, sqlConn)
                        sqlCommand.Parameters.AddWithValue("@value", CInt(value))
                        Data &= a & ":" & serialize(sqlCommand.ExecuteReader) & ","
                        sqlCommand.Dispose()
                    End Using
                Next
                Data &= "}"
                sqlConn.Close()
                sqlConn.Dispose()
            End Using



        Catch ee As SqlException
            'messageType = messageType.BeforeExecute
            'message = ee.Message
        Catch ex As Exception
            'messageType = messageType.BeforeExecute
            'message = ex.Message
        Finally

        End Try
        Try
            ret = "{"
            ret &= "data : " & Data & " "
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
        End Try

        Return New HttpResponseMessage() With { _
          .Content = New JsonContent(r) _
        }
    End Function

    <HttpGet>
    Public Function ExportToExcel(mID As Integer, pageSize As String, currentPage As Integer, field As String, direction As String, filter As String, rID As Integer) As HttpResponseMessage
        Dim result As New HttpResponseMessage

        Dim filterD As Dictionary(Of String, Object) = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(System.Uri.UnescapeDataString(filter)),
            m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(mID),
            dataSource As String = SetDataSourceFilter(m, 0, rID, HttpContext.Current.Session)

        Dim filterString As New List(Of String)
        For Each x In filterD.Where(Function(i) Not i.Value Is Nothing AndAlso Not IsDBNull(i.Value) AndAlso Trim(i.Value.ToString).Length > 0)
            Dim dataType As String = String.Empty

            If x.Key.Contains("From_") OrElse x.Key.Contains("To_") Then
                dataType = GetDataType(m.dtColumns.Select("Name = '" & x.Key.Replace("From_", "").Replace("To_", "") & "'")(0).Item("colDataType").ToString).FullName.ToString()
            Else
                dataType = GetDataType(m.dtColumns.Select("Name = '" & x.Key & "'")(0).Item("colDataType").ToString).FullName.ToString()
            End If

            If dataType = "System.String" Then
                filterString.Add(x.Key & " LIKE '%'+@" & x.Key & "+'%'")
            ElseIf dataType = "System.DateTime" OrElse dataType = "System.Int32" OrElse dataType = "System.Decimal" Then
                If x.Key.Contains("From_") Then
                    filterString.Add(x.Key.Replace("From_", "") & " >= @" & x.Key)
                ElseIf x.Key.Contains("To_") Then
                    filterString.Add(x.Key.Replace("To_", "") & " <= @" & x.Key)
                Else
                    If x.Key.ToString.ToLower.Contains("start") Or x.Key.ToString.ToLower.Contains("end") Then
                        If x.Key.ToString.ToLower.Contains("start") Then
                            filterString.Add(x.Key & " >= @" & x.Key)
                        Else
                            filterString.Add(x.Key & " <= @" & x.Key)
                        End If
                    Else
                        filterString.Add(x.Key & " = @" & x.Key)
                    End If
                End If
            Else
                filterString.Add(x.Key & " = @" & x.Key)
            End If
        Next

        Using sqlConn As SqlClient.SqlConnection = New SqlClient.SqlConnection(ConnectionString)
            sqlConn.Open()
            Dim cmdtxt As String, ListColumns As String = String.Empty

            Dim cols As DataRow() = m.dtColumns.Select("Name <> 'ID' AND ShowInList = 1 AND IsNull(isTitle,0) = 0 AND ID_WebMenuControlTypes NOT IN(6, 27, 21)", "SeqNo ASC")
            For x = 0 To cols.Length - 1
                If cols(x)("Name").ToString.ToLower.Contains("id_") Then
                    ListColumns &= cols(x)("Name").ToString.Substring(3) & " AS [" & IsNull(cols(x)("Label"), cols(x)("Name").ToString.Substring(3)) & "],"
                Else
                    ListColumns &= cols(x)("Name") & " AS [" & IsNull(cols(x)("Label"), cols(x)("Name")) & "],"
                End If

            Next x

            If CBool(IsNull(m.ColumnValue("WithPagination"), "0")) And pageSize <> "All" Then
                cmdtxt = "SELECT " & ListColumns.ToString.Trim(",") & " FROM " & SetDataSourceFilter(m, 0, 0, HttpContext.Current.Session) & If(filterString.Count = 0, "", " WHERE " & String.Join(" AND ", filterString)) & " ORDER BY " & field & " " & direction & _
               "  OFFSET " & (currentPage - 1) * pageSize & _
               " ROWS FETCH NEXT " & pageSize & " ROWS ONLY"
            Else
                cmdtxt = "SELECT " & ListColumns.ToString.Trim(",") & " FROM " & SetDataSourceFilter(m, 0, rID, HttpContext.Current.Session) & If(filterString.Count = 0, "", " WHERE " & String.Join(" AND ", filterString)) & " ORDER BY " & field & " " & direction
            End If

            Using sqlCommand As New SqlCommand(cmdtxt, sqlConn)
                For Each x In filterD.Where(Function(i) Not i.Value Is Nothing AndAlso Not IsDBNull(i.Value) AndAlso Trim(i.Value.ToString).Length > 0)
                    Dim dataType As System.Type
                    If x.Key.Contains("From_") OrElse x.Key.Contains("To_") Then
                        dataType = GetDataType(m.dtColumns.Select("Name = '" & x.Key.Replace("From_", "").Replace("To_", "") & "'")(0).Item("colDataType").ToString)
                    Else
                        dataType = GetDataType(m.dtColumns.Select("Name = '" & x.Key & "'")(0).Item("colDataType").ToString)
                    End If

                    sqlCommand.Parameters.AddWithValue("@" & x.Key, CTypeDynamic(x.Value.ToString.Replace(Chr(160), Chr(32)), dataType))
                Next
                Dim da As New SqlDataAdapter
                da.SelectCommand = sqlCommand
                Dim dt As New DataTable
                da.Fill(dt)
                Dim workBook As HSSFWorkbook = New HSSFWorkbook(),
                    sheet As HSSFSheet = workBook.CreateSheet("Sheet1")
                FillExcelSheet(workBook, sheet, dt)

                Using exportData As MemoryStream = New MemoryStream()
                    workBook.Write(exportData)
                    result = Request.CreateResponse(HttpStatusCode.OK)
                    result.Content = New ByteArrayContent(exportData.ToArray())
                    result.Content.Headers.ContentType = New MediaTypeHeaderValue("application/vnd.ms-excel")
                    result.Content.Headers.ContentDisposition = New System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                    result.Content.Headers.ContentDisposition.FileName = m.Name.Replace(" ", "-") & ".xls"
                End Using
                'Data = serialize(sqlCommand.ExecuteReader)
            End Using
            sqlConn.Close()
        End Using
        Return result
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function loadOrgChart() As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim memCache As New MemoryCacher, ret As String = String.Empty
        ' If Not memCache.Contains("MENU_" & HttpContext.Current.Session("ID_UserGroup")) Then
        Dim am As New List(Of OrgChart)

        'PALIT QUERY
        Dim dtEmp As DataTable = getTable("Select * from vEmployeeDesignation ed ORDER BY ed.ID_Designation")
        Dim dtVacant As DataTable = getTable("Select * from vOrgChartDesignationVacancy")

        Dim drHead As DataRow = dtEmp.Select("ID_Supervisor =" & 0).FirstOrDefault
        Dim Position As String = String.Empty, Name As String = String.Empty

        If Not drHead Is Nothing Then
            Position = drHead("Position")
            Name = drHead("Name")
            For Each dr As DataRow In dtEmp.Select("ID_Supervisor =" & drHead("ID"))

                Dim a As New OrgChart(IsNull(dr("Position"), ""), dr("Name"))

                For Each dr2 As DataRow In dtEmp.Select("ID_Supervisor = " & dr("ID"))
                    buildChildOrgChart(a, dr2, dtEmp, dtVacant)

                    If hasVacantOrgChart(dtVacant, dr2("ID_Designation")) Then
                        buildVacantOrgChart(a, dr2("ID_Designation"), dr2("Position"), dtEmp, dtVacant)
                    End If
                Next

                am.Add(a)

                If hasVacantOrgChart(dtVacant, dr("ID_Designation")) Then
                    buildVacantOrgChart(am, dr("ID_Designation"), dr("Position"), dtVacant)
                End If
            Next
        End If
        ret = "{"
        ret &= "data: {"
        ret &= "Position:'" & Position & "',"
        ret &= "Name:'" & Name & "',"
        ret &= "Supervises:" & JsonConvert.SerializeObject(am)
        ret &= "}"
        ret &= "}"
        '.Content = New StringContent(memCache.GetValue("MENU_" & HttpContext.Current.Session("ID_UserGroup")).ToString, System.Text.Encoding.UTF8, "application/json")
        Return New HttpResponseMessage() With { _
            .Content = New JsonContent(JObject.Parse(ret)) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function MinifyScripts(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim message As String = "Minifying scripts completed."
        Try '' TODO : SYSTEM LANG PDE MAGMINIFY

            Dim newBuildFolderPath As String = HttpContext.Current.Server.MapPath("~/Build/" & ExecScalarNoParams("SELECT dbo.fGetWebParameter('SystemVersion')"))
            If (Not System.IO.Directory.Exists(newBuildFolderPath)) Then
                System.IO.Directory.CreateDirectory(newBuildFolderPath)
            End If

            CompressScript()
            CompressHTMLPages()
            CompressApplicationScripts()
        Catch ex As Exception
            message = ex.Message
            logError(ex)
        End Try
        Return New HttpResponseMessage() With { _
            .Content = New JsonContent(JObject.Parse("{ message : '" & message & "' }")) _
        }
    End Function

    Dim idD As Integer

    Private Sub buildChildOrgChart(orgChart As OrgChart, dr As DataRow, dtEmp As DataTable, dtVacancy As DataTable)
        Dim a2 As New OrgChart(IsNull(dr("Position"), ""), dr("Name"))

        For Each dr2 As DataRow In dtEmp.Select("ID_Supervisor = " & dr("ID"))

            idD = dr2("ID_Designation")

            buildChildOrgChart(a2, dr2, dtEmp, dtVacancy)

        Next

        orgChart.Add(a2)

        If hasVacantOrgChart(dtVacancy, idD) Then
            buildVacantOrgChart(orgChart, idD, IsNull(dr("Position"), ""), dtEmp, dtVacancy)
        End If

    End Sub

    Private Sub buildVacantOrgChart(orgChart As OrgChart, idDesignation As Integer, position As String, dtEmp As DataTable, dtVacant As DataTable)

        Dim x As DataRow = dtVacant.Select("ID_Designation = " & idDesignation).FirstOrDefault()

        Dim countVacant = x("Vacancy")

        Dim resultList = orgChart.Supervises.Where(Function(t) t.Position = position).Count()

        Dim resultEmp = dtEmp.Select("ID_Designation = " & idDesignation).Count()

        If resultList = resultEmp Then

            orgChart.Add(New OrgChart(IsNull(position, ""), "Vacant(" & countVacant & ")"))

            'for repeated vacancy chart
            'For o As Integer = 1 To countVacant
            '    orgChart.Add(New OrgChart(IsNull(position, ""), "Vacant(" & countVacant & ")"))
            'Next

        End If

    End Sub

    Private Sub buildVacantOrgChart(orgChart As List(Of OrgChart), idDesignation As Integer, position As String, dtVacant As DataTable)

        Dim x As DataRow = dtVacant.Select("ID_Designation = " & idDesignation).FirstOrDefault()

        Dim countVacant = x("Vacancy")

        orgChart.Add(New OrgChart(IsNull(position, ""), "Vacant(" & countVacant & ")"))
    End Sub

    Private Function hasVacantOrgChart(dtVacant As DataTable, idDesignation As Integer) As Boolean

        Dim countVacant = dtVacant.Select("ID_Designation = " & idDesignation).Count()

        If countVacant > 0 Then
            Return True
        End If
        Return False
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function GetCalendarSource(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Using sqlCon As New SqlConnection(ConnectionString)
            sqlCon.Open()
            Dim ret As String = String.Empty,
            StartDate As DateTime = obj("StartDate").ToObject(Of DateTime)(),
            EndDate As DateTime = obj("EndDate").ToObject(Of DateTime)(),
            ID_Employee As Integer = IsNull(obj("ID_Employee").ToObject(Of Integer)(), HttpContext.Current.Session("ID_Employee")),
            r As New JObject
            Using AccessNo As SqlDataReader = New SqlCommand("Select AccessNo From dbo.tEmployee where ID =" & HttpContext.Current.Session("ID_Employee"), sqlCon).ExecuteReader()
            StartDate = DateAdd(DateInterval.Day, 1, StartDate).ToShortDateString
                EndDate = EndDate.ToShortDateString
                While AccessNo.Read
                    Try
                ret = "{ data:"
                If ID_Employee = -1 Then
                    ret += getJSONTable("SELECT * FROM dbo.fzgetApproverUnder(" & HttpContext.Current.Session("ID_User") & ") a CROSS APPLY dbo.fgetCalendarSource2(a.ID, '" & StartDate & "','" & EndDate & "', '" & AccessNo("AccessNo").ToString & "')")
                Else
                    ret += getJSONTable("SELECT * FROM dbo.fgetCalendarSource(" & ID_Employee & ", '" & StartDate & "', '" & EndDate & "', '" & AccessNo("AccessNo").ToString & "')")
                End If
                ret += "}"
                r = JObject.Parse(ret)
            Catch ex As Exception
                r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
            End Try
                End While
                Return New HttpResponseMessage() With {
               .Content = New JsonContent(r)
            }
        End Using
        End Using
    End Function

    <HttpPost>
    <DeflateCompression>
    Public Function GetWebParameters() As HttpResponseMessage
        Dim ret As String = String.Empty
        Dim r As New Newtonsoft.Json.Linq.JObject
        Dim dtParameters As New DataTable

        Using sqlCon As New SqlConnection(ConnectionString)
            sqlCon.Open()

            Using sqlCmd As New SqlCommand("SELECT * FROM dbo.vWebParameters wp", sqlCon)
                Dim sqlAdapter As New SqlDataAdapter
                sqlAdapter.SelectCommand = sqlCmd
                sqlAdapter.Fill(dtParameters)
            End Using

            sqlCon.Close()
        End Using

        ret = "{"
        For Each drParam As DataRow In dtParameters.Rows

            ret &= drParam("ParamName").ToString().Trim() & ":'" & drParam("ParamValue").ToString().Trim() & "',"

        Next

        ret = ret.Substring(0, ret.Length - 1)

        ret &= "}"

        ret = ret.Replace(" ", "")

        r = JObject.Parse(ret)

        Return New HttpResponseMessage() With { _
           .Content = New JsonContent(r) _
        }

    End Function

    <HttpPost>
    <DeflateCompression>
    Public Function SavePasswordReset(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)

        Dim ret As String = String.Empty,
            r As New Newtonsoft.Json.Linq.JObject,
            message As String = String.Empty

        Try
            'GetDirectReference(obj("ID").ToObject(Of String)(), HttpContext.Current.Session),
            Dim ID As Integer = toBase10(obj("ID").ToObject(Of String)()),
            NewPassword As String = obj("NewPassword").ToObject(Of String)()

            Using sqlConn As New SqlConnection(ConnectionString)
                sqlConn.Open()
                Using sqlCmd As New SqlCommand("EXEC pResetPassword @ID_User, @NewPassword", sqlConn)
                    sqlCmd.Parameters.AddWithValue("ID_User", ID)
                    sqlCmd.Parameters.AddWithValue("NewPassword", IIf(NewPassword <> "", Common.Encrypt(NewPassword, 41) & "_BJTGLR", ""))

                    sqlCmd.ExecuteNonQuery()
                End Using
                sqlConn.Close()
            End Using


        Catch ee As SqlException
            message = ee.Message
        Catch ex As Exception
            message = ex.Message
        Finally
        End Try

        Try
            ret = "{"
            ret &= "message : '" & addStripSlashes(message) & "'"
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
            logError(ex)
        End Try

        Return New HttpResponseMessage() With {.Content = New JsonContent(r)}

    End Function

    <HttpPost>
    <DeflateCompression>
    Public Function SavePassword(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)

        Dim ret As String = String.Empty,
            r As New Newtonsoft.Json.Linq.JObject,
            message As String = String.Empty,
            currentPassword As String = String.Empty

        Try
            ' GetDirectReference(obj("ID").ToObject(Of String)(), HttpContext.Current.Session)
            Dim ID As Integer = toBase10(obj("ID").ToObject(Of String)()),
                Password As String = obj("Password").ToObject(Of String)(),
                NewPassword As String = obj("NewPassword").ToObject(Of String)(),
                 OldPassword As String = String.Empty 'IIf(IsNothing(obj("OldPassword")), String.Empty, obj("OldPassword").ToObject(Of String)()),


            OldPassword = IIf(IsNothing(obj("OldPassword")), String.Empty, obj("OldPassword").ToObject(Of String)())

            Dim IsFirstLog As Boolean = ExecScalarNoParams("SELECT IsFirstLog FROM tUser WHERE ID = " & ID)


            If OldPassword IsNot String.Empty And OldPassword <> Nothing Then
                If OldPassword <> ExecScalarNoParams(String.Format("SELECT dbo.fEncrypt(REPLACE(u.Password,'_BJTGLR',''),41) FROM dbo.tUser u WHERE u.ID ={0} ", ID)).ToString() Then
                    Throw New Exception("Invalid Old Password.")
                End If
            End If



            Using sqlConn As New SqlConnection(ConnectionString)
                sqlConn.Open()

                'validate password if it is not first log ----------------- 
                If Not IsFirstLog Then
                    Using sqlCommand As New SqlCommand("SELECT ISNULL(u.Password,'') 'Password' FROM dbo.tUser u WHERE u.ID = @ID", sqlConn)
                        Try
                            sqlCommand.Parameters.AddWithValue("@ID", ID)
                            currentPassword = sqlCommand.ExecuteScalar()
                        Catch ex As Exception
                            Throw ex
                        Finally
                            sqlCommand.Dispose()
                        End Try
                    End Using

                    If Not Password Is Nothing Then
                        If Not Common.Encrypt(Password, 41) & "_BJTGLR" = currentPassword Then
                            Throw New Exception("Invalid current password.")
                        End If
                    End If

                    If currentPassword = NewPassword Then
                        Throw New Exception("New password is the existing password.")
                    End If
                End If

                'validate password -----------------

                Using sqlCommand As New SqlCommand("UPDATE dbo.tUser SET Password = @Password, isFirstLog = 0, LastPasswordChangeDate = Getdate() WHERE ID = @ID;", sqlConn)
                    Try
                        sqlCommand.Parameters.AddWithValue("@ID", ID)
                        sqlCommand.Parameters.AddWithValue("@Password", IIf(NewPassword <> "", Common.Encrypt(NewPassword, 41) & "_BJTGLR", ""))
                        sqlCommand.ExecuteNonQuery()
                    Catch ex As Exception
                        Throw ex
                    Finally
                        sqlCommand.Dispose()
                    End Try
                End Using

            End Using

        Catch ee As SqlException
            message = ee.Message
        Catch ex As Exception
            message = ex.Message
        Finally
        End Try

        Try
            ret = "{"
            ret &= "message : '" & addStripSlashes(message) & "'"
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
            logError(ex)
        End Try

        Return New HttpResponseMessage() With { _
           .Content = New JsonContent(r) _
        }

    End Function

    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function CreateTheme(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim message As String = "Creating theme completed."

        Try
            Dim rID As Integer = toBase10(obj("rID").ToObject(Of String)()) ' GetDirectReference(obj("rID").ToObject(Of String)(), HttpContext.Current.Session)
            GSWEB.Common.CreateTheme(rID)
        Catch ex As Exception
            message = ex.Message
            logError(ex)
        End Try
        Return New HttpResponseMessage() With { _
            .Content = New JsonContent(JObject.Parse("{ message : '" & message & "' }")) _
        }
    End Function

    <DeflateCompression>
   <CheckCSRFHeader>
    Public Function ApplyTheme(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim message As String = "Applying theme completed."

        Try
            Dim rID As Integer = toBase10(obj("rID").ToObject(Of String)()) ' GetDirectReference(obj("rID").ToObject(Of String)(), HttpContext.Current.Session)
            Using sqlCon As New SqlConnection(ConnectionString)
                sqlCon.Open()
                Using sqlCmd As New SqlCommand("UPDATE dbo.tCompany SET ID_Skins = @ID", sqlCon)
                    Try
                        sqlCmd.Parameters.AddWithValue("@ID", rID)
                        sqlCmd.ExecuteNonQuery()
                    Catch ex As Exception
                        Throw ex
                    Finally
                        sqlCmd.Dispose()
                    End Try
                End Using
            End Using
        Catch ex As Exception
            message = ex.Message
            logError(ex)
        End Try
        Return New HttpResponseMessage() With { _
            .Content = New JsonContent(JObject.Parse("{ message : '" & message & "' }")) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function RunScript(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)

        Dim ret As String = String.Empty,
            r As New Newtonsoft.Json.Linq.JObject,
            message As String = String.Empty,
            data As String = "[]"

        Try
            Dim script As String = obj("script").ToObject(Of String)()

            Using sqlConn As New SqlConnection(ConnectionString)
                sqlConn.Open()
                Dim ds As New DataSet
                Using sqlCmd As New SqlCommand(script, sqlConn)
                    Using da As New SqlDataAdapter(sqlCmd)
                        da.Fill(ds)
                        If (ds.Tables.Count > 0) Then
                            data = JsonConvert.SerializeObject(ds.Tables(0))
                        End If


                    End Using
                End Using
                sqlConn.Close()
            End Using
            message = "ok"
        Catch ee As SqlException
            message = ee.Message
        Catch ex As Exception
            message = ex.Message
        Finally
        End Try

        Try
            ret = "{"
            ret &= "message : '" & addStripSlashes(message) & "',"
            ret &= "data : " & data
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
            logError(ex)
        End Try

        Return New HttpResponseMessage() With { _
           .Content = New JsonContent(r) _
        }

    End Function

    <HttpPost>
    <DeflateCompression>
    Public Function getAllPublicResources() As HttpResponseMessage
        'If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim mID As Integer = 1061,
            rID As Integer = 0,
            pID As Integer = 0,
            m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(mID),
            ret As String = String.Empty,
            r As New Newtonsoft.Json.Linq.JObject,
            Session As System.Web.SessionState.HttpSessionState = HttpContext.Current.Session

        Try
            Dim dateSettings As JsonSerializerSettings = New JsonSerializerSettings With {.DateFormatHandling = DateFormatHandling.IsoDateFormat}

            Dim strQueryBuilder As New StringBuilder

            'DETAIL
            Dim menuTemp As New List(Of String)
            For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID)
                strQueryBuilder.Append("SELECT " & m2.getListColumns & " FROM " & SetDataSourceFilter(m2, 0, rID, Session) & ";")
                menuTemp.Add(m2.MenuID.ToString)
            Next
            'DROPDOWN SOURCES
            Dim ddTemp As New List(Of String)
            generateControlQueryBuilder(strQueryBuilder, ddTemp, m, rID, pID, HttpContext.Current.Session, 2, True)
            For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 14)
                generateControlQueryBuilder(strQueryBuilder, ddTemp, m2, rID, pID, HttpContext.Current.Session, 2, False)
            Next

            'AUTO COMPLETE
            Dim ddAutoCom As New List(Of String)
            generateControlQueryBuilder(strQueryBuilder, ddAutoCom, m, rID, pID, HttpContext.Current.Session, 23, True)
            For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 14)
                generateControlQueryBuilder(strQueryBuilder, ddAutoCom, m2, rID, pID, HttpContext.Current.Session, 23, False)
            Next

            'TEXT AUTO COMPLETE
            Dim ddTextAutoComplete As New List(Of String)
            generateControlQueryBuilder(strQueryBuilder, ddTextAutoComplete, m, rID, pID, HttpContext.Current.Session, 10, True)
            For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 14)
                generateControlQueryBuilder(strQueryBuilder, ddTextAutoComplete, m2, rID, pID, HttpContext.Current.Session, 10, False)
            Next

            'RADIO BUTTON
            Dim ddRdB As New List(Of String)
            generateControlQueryBuilder(strQueryBuilder, ddRdB, m, rID, pID, HttpContext.Current.Session, 26, True)
            For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 14)
                generateControlQueryBuilder(strQueryBuilder, ddRdB, m2, rID, pID, HttpContext.Current.Session, 26, False)
            Next

            'Lookup
            Dim ddLookUp As New List(Of String)
            generateControlQueryBuilder(strQueryBuilder, ddLookUp, m, rID, pID, HttpContext.Current.Session, 17, True)
            For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 14)
                generateControlQueryBuilder(strQueryBuilder, ddLookUp, m2, rID, pID, HttpContext.Current.Session, 17, False)
            Next


            Using ttCon As New SqlClient.SqlConnection(ConnectionString)
                ttCon.Open()
                Using ttCmd As New SqlCommand(strQueryBuilder.ToString, ttCon)
                    Dim Reader As SqlDataReader = Nothing

                    ret = "{"
                    ret &= "Master:"
                    If strQueryBuilder.Length > 0 Then Reader = ttCmd.ExecuteReader
                    If rID = 0 Then
                        ret &= dictionaryToJSON(m.getDefaultFormData(), pID, Session, IsNull(mCollection.GetMenu(m.ParentID).TableName, ""))
                    Else
                        ret &= getJSONTableRowNoClose(Reader, True, HttpContext.Current.Session, mCollection.GetMenu(mID).dtColumns.AsEnumerable().Where(Function(c) IsNull(c.Item("ID_WebMenuControlTypes"), 0) = 27 And IsNull(c.Item("IsVisible"), False) = True).Select(Function(c) CStr(c.Item("Name"))).ToList())
                        Reader.NextResult()
                    End If
                    ret &= ","

                    ret &= "Parent:"
                    If m.ColumnValue("ID_WebMenuTypes") = 12 Or m.ColumnValue("ID_WebMenuTypes") = 14 Then
                        ret &= getJSONTableRowNoClose(Reader)
                        Reader.NextResult()
                    Else
                        ret &= "{}"
                    End If
                    ret &= ","

                    ret &= "Detail:{"
                    For x As Integer = 0 To menuTemp.Count - 1
                        ret &= menuTemp(x) & ":" & serializeNoClose(Reader) & ","
                        Reader.NextResult()
                    Next x
                    ret &= "},"

                    'DROPDOWN
                    ret &= "dropdown_source:{"
                    For x As Integer = 0 To ddTemp.Count - 1
                        ret &= ddTemp(x) & ":" & serializeNoClose(Reader) & ","
                        Reader.NextResult()
                    Next
                    ret &= "},"

                    'AUTO COMPLETE
                    ret &= "autocomplete_source:{"
                    For x As Integer = 0 To ddAutoCom.Count - 1
                        ret &= ddAutoCom(x) & ":" & serializeNoClose(Reader) & ","
                        Reader.NextResult()
                    Next
                    ret &= "},"

                    'TEXT AUTO COMPLETE
                    ret &= "text_autocomplete_source:{"
                    For x As Integer = 0 To ddTextAutoComplete.Count - 1
                        ret &= ddTextAutoComplete(x) & ":" & JavascriptArrayNoClose(Reader) & ","
                        Reader.NextResult()
                    Next
                    ret &= "},"

                    'RADIO BUTTON
                    ret &= "rdb_source:{"
                    For x As Integer = 0 To ddRdB.Count - 1
                        ret &= ddRdB(x) & ":" & serializeNoClose(Reader) & ","
                        Reader.NextResult()
                    Next
                    ret &= "},"
                    'LOOKUP
                    ret &= "lookup_source:{"
                    For x As Integer = 0 To ddLookUp.Count - 1
                        ret &= ddLookUp(x) & ":" & serializeNoClose(Reader) & ","
                        Reader.NextResult()
                    Next
                    ret &= "},"

                    ret &= "}"

                    If Not Reader Is Nothing Then Reader.Close()
                End Using
                ttCon.Close()
            End Using

            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error:'" & addStripSlashes(ex.Message) & "' }")
            logError(ex, mID)
        End Try

        Return New HttpResponseMessage() With { _
           .Content = New JsonContent(r) _
        }

    End Function

    <HttpPost>
    <DeflateCompression>
    Public Function AddPublicNewRow(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        Dim ret As String = String.Empty,
            data As String = String.Empty,
            r As New JObject

        Dim mID As Integer = obj("mID").ToObject(Of Integer)(),
            m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(mID)

        Try
            ret = "{"
            ret &= "data:" & dictionaryToJSON(m.getDefaultRowData(), 0, HttpContext.Current.Session, mCollection.GetMenu(m.ParentID).TableName.ToString) & ","
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
            logError(ex, mID)
        End Try

        Return New HttpResponseMessage() With { _
           .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    Public Async Function publicSave() As Task(Of HttpResponseMessage) ' ByVal obj As Newtonsoft.Json.Linq.JObject
        If Not Request.Content.IsMimeMultipartContent() Then
            Me.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType)
        End If

        Dim root = HttpContext.Current.Server.MapPath("~/Upload/")
        Directory.CreateDirectory(root)
        Dim provider = New MultipartFormDataStreamProvider(root),
            result = Await Request.Content.ReadAsMultipartAsync(provider)
        Dim mID As Integer = result.FormData.Item("mID"),
            rID As Integer = 0,
            btnID As Integer = result.FormData.Item("btnID"),
            drMaster As CaseInsensitiveDictionary = JsonConvert.DeserializeObject(Of CaseInsensitiveDictionary)(result.FormData.Item("Master")),
            ds As DataSet = JsonConvert.DeserializeObject(Of DataSet)(result.FormData.Item("Detail")),
            fileSummary As List(Of fileSummary) = JsonConvert.DeserializeObject(Of List(Of fileSummary))(result.FormData.Item("fileSummary"))

        Dim ret As String = String.Empty,
            message As String = String.Empty,
            messageType As MessageType = messageType.Success,
            id As Integer = 0,
            r As New JObject

        Try
            Dim m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(mID)

            id = m.Save(ConnectionString, rID, drMaster, ds, fileSummary, result)
            'Dim dr As DataRow = m.dtButtons.Select("ID = " & btnID)(0)

        Catch ee As SqlException
            messageType = messageType.BeforeExecute
            If ee.Message.Contains("IX_") Or ee.Message.Contains("CK_") Or ee.Message.Contains("FK_") Then
                message = getSystemMessage(ee.Message)
            Else
                message = ee.Message
            End If
        Catch ex As Exception
            messageType = messageType.BeforeExecute
            message = ex.Message
            logError(ex, mID)
        End Try

        Try
            ret = "{"
            ret &= "message : '" & addStripSlashes(message) & "',"
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            logError(ex, mID)
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "'}")
        End Try


        Return New HttpResponseMessage() With { _
          .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    Public Function getCurrentDate(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        Dim ret As String = String.Empty,
            data As String = String.Empty,
            r As New JObject
        Dim currentDate As String = Today.ToString("MM/dd/yyyy")
        Try
            ret = "{"
            ret &= "data: '" & currentDate & "'"
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
        End Try

        Return New HttpResponseMessage() With { _
           .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function getIonsFileSummary() As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim ID_User As Integer = HttpContext.Current.Session("ID_User"),
            ID_Employee As Integer = HttpContext.Current.Session("ID_Employee")
        Dim dt As DataTable = getTable("SELECT * FROM dbo.fFilingApprovalSummary(" & ID_Employee & ", " & ID_User & ")")
        Dim menuFilingID As New List(Of Integer),
            menuApprovalID As New List(Of Integer),
            dtMenuFilingID As DataTable = dt.Clone(),
            dtMenuApprovalID As DataTable = dt.Clone()
        'ID_webmenuApproval
        For Each dr As DataRow In dt.Rows
            menuFilingID.Add(dr("ID_WebMenuFiling"))
            menuApprovalID.Add(dr("ID_WebMenuApproval"))
        Next

        For Each m As GSWEB.MenuCollection.Menu In mCollection.Where(Function(x) menuFilingID.Contains(x.MenuID))
            If m.HasAccess(HttpContext.Current.Session("ID_UserGroup")) Then
                Dim tmp As DataTable = dt.Select("ID_WebMenuFiling = " & m.MenuID).CopyToDataTable
                dtMenuFilingID.ImportRow(tmp.Rows(0))
            End If
        Next

        For Each m As GSWEB.MenuCollection.Menu In mCollection.Where(Function(x) menuApprovalID.Contains(x.MenuID))
            If m.HasAccess(HttpContext.Current.Session("ID_UserGroup")) Then
                Dim tmp As DataTable = dt.Select("ID_WebMenuApproval = " & m.MenuID).CopyToDataTable
                dtMenuApprovalID.ImportRow(tmp.Rows(0))
            End If
        Next
        Dim mFilingString As String = JsonConvert.SerializeObject(dtMenuFilingID)
        Dim mApprovalString As String = JsonConvert.SerializeObject(dtMenuApprovalID)

        Dim ret As String = String.Empty

        ret = "{menuFiling:" & mFilingString & ", menuApprovals:" & mApprovalString & "}"
        Return New HttpResponseMessage() With { _
            .Content = New JsonContent(JObject.Parse(ret)) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function getNotification(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim ID_User As Integer = HttpContext.Current.Session("ID_User"),
            Msg As String = String.Empty,
            TotalCnt As String = obj("TotalCnt").ToObject(Of String)(),
            LastID As Integer = obj("refID").ToObject(Of Integer)(),
            dt As DataTable = Nothing,
            dt2 As DataTable = Nothing,
            dt3 As DataTable = Nothing,
            NotificationObject As String = ""

        If getTable("select TOP " & TotalCnt & " * from dbo.vWebUserNotifications where ID_User = " & ID_User & " AND ID_Employee IS NOT NULL order by DateTimeCreated DESC ").Rows.Count > 0 Then
            dt = getTable("select TOP " & TotalCnt & " * from dbo.vWebUserNotifications where ID_User = " & ID_User & " AND ID_Employee IS NOT NULL order by DateTimeCreated DESC ")
        End If

        Dim cntRecord As Integer = getTable("select count(ID) as cnt from dbo.vWebUserNotifications where ID_User = " & ID_User & " AND ID_Employee IS NOT NULL and IsView = 0 ").Rows(0)("cnt")

        If Not (IsNothing(dt)) Then

            For Each dr As DataRow In dt.Rows
                dr("ReferenceID") = If(IsNull(dr("ReferenceID"), "") = "", "", toAnyBase(dr("ReferenceID")))
                If Not Exists("Upload/" & dr("ImageFile")) Then
                    dr("ImageFile") = "avatar.png"
                End If
            Next

            If LastID < dt.Rows(0)("ID") Then

                If getTable("SELECT * FROM dbo.vWebUserNotifications WHERE ID_User = " & ID_User & " AND ID_Employee IS NOT NULL AND ID > " & LastID & "").Rows.Count > 0 Then
                    dt3 = getTable("SELECT * FROM dbo.vWebUserNotifications WHERE ID_User = " & ID_User & " AND ID_Employee IS NOT NULL AND ID > " & LastID & "")
                    Dim q = (From r In dt3.AsEnumerable() Group r By
                      ID_WebMenus = r("ID_WebMenus"),
                      WebMenus = r("WebMenus"),
                      ID_Parent = r("ID_Parent")
                      Into Group
                      Select New With {
                          .ID_WebMenus = ID_WebMenus,
                          .ID_Parent = ID_Parent,
                          .WebMenus = WebMenus
                      })
                    Dim dtGroupMenus As New DataTable,
                        dtEmployee As New DataTable,
                        NewMsg As New List(Of String)

                    dtGroupMenus.Columns.Add("ID_WebMenus", GetType(Integer))
                    dtGroupMenus.Columns.Add("ID_Parent", GetType(Integer))
                    dtGroupMenus.Columns.Add("WebMenus", GetType(String))

                    dtEmployee.Columns.Add("ID_Employee", GetType(Integer))
                    dtEmployee.Columns.Add("ID_WebMenus", GetType(Integer))
                    dtEmployee.Columns.Add("Sender", GetType(String))
                    dtEmployee.Columns.Add("CNT", GetType(Integer))
                    dtEmployee.Columns.Add("fData", GetType(String))

                    For Each grp In q
                        dtGroupMenus.Rows.Add(grp.ID_WebMenus, grp.ID_Parent, grp.WebMenus)
                    Next

                    Dim q2 = (From r In dt3.AsEnumerable() Group r By
                      ID_Employee = r("ID_Employee"),
                      ID_WebMenus = r("ID_WebMenus"),
                      Sender = r("Sender")
                    Into Group
                      Select New With {
                          .ID_Employee = ID_Employee,
                          .ID_WebMenus = ID_WebMenus,
                          .Sender = Sender,
                          .CNT = Group.Count()
                      })

                    Dim listSb As New List(Of String)
                    For Each dr As DataRow In dtGroupMenus.Rows
                        If IsNull(dr("ID_WebMenus"), 0) <> 0 Then
                            Dim sb As New StringBuilder
                            Dim qq = q2.Where(Function(x) x.ID_WebMenus = dr("ID_WebMenus"))
                            Dim ids As New List(Of String)
                            Dim filterData As String = ""
                            dtEmployee.Clear()
                            For Each grp In qq
                                Dim qe = dt3.AsEnumerable().Where(Function(x) x("ID_WebMenus") = dr("ID_WebMenus") And x("ID_Employee") = grp.ID_Employee)
                                For Each qemp In qe
                                    ids.Add(qemp("ReferenceID"))
                                Next
                                filterData = "ID_Employee = " & grp.ID_Employee & " AND ID IN (" & String.Join(",", ids) & ")"
                                dtEmployee.Rows.Add(grp.ID_Employee, grp.ID_WebMenus, grp.Sender, grp.CNT, encryptData(filterData))
                            Next

                            sb.Append("{")
                            sb.Append("""" & dr("WebMenus") & """:{")
                            sb.Append("""ID_Parent"":" & dr("ID_Parent") & ",")
                            sb.Append("""Employee"":" & JsonConvert.SerializeObject(dtEmployee) & "")

                            sb.Append("}")
                            sb.Append("}")

                            listSb.Add(sb.ToString)
                        End If
                    Next
                    If listSb.Count > 0 Then
                        NotificationObject = "[" & String.Join(",", listSb) & "]"
                    End If
                    dt2 = dt.Select("ID > " & LastID).CopyToDataTable

                End If

            End If

            Msg = JsonConvert.SerializeObject(dt)

        End If

        'Dim filterData As String = ""
        'If Not (IsNothing(dt2)) Then
        '    Dim list As New List(Of String)
        '    For Each dr As DataRow In dt2.Rows
        '        list.Add(dr("ID").ToString)
        '    Next
        '    filterData = "ID IN (" & String.Join(",", list) & ")"
        '    filterData = encryptData(filterData)
        'End If

        Dim ret As String = String.Empty

        ret = "{Msg:" & IIf(Msg = "" Or IsNull(Msg, "") = "", "''", Msg) & ", CntRecord: " & cntRecord & ", NewMsg: " & If(NotificationObject <> "", NotificationObject, "''") & "}"
        Return New HttpResponseMessage() With { _
            .Content = New JsonContent(JObject.Parse(ret)) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function updateNotification(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim ID_User As Integer = HttpContext.Current.Session("ID_User"),
            refID As Integer = obj("refID").ToObject(Of Integer)(),
            IsView As String = 0,
            message As String = String.Empty

        Try
            Using sqlCon As New SqlConnection(ConnectionString)
                sqlCon.Open()
                Using sqlCmd As New SqlCommand("Update dbo.tWebUserNotifications SET IsView = 1 where ID = @ID ", sqlCon)
                    Try
                        sqlCmd.Parameters.AddWithValue("@ID", refID)
                        sqlCmd.ExecuteNonQuery()
                        IsView = 1
                    Catch ex As Exception
                        Throw ex
                    Finally
                        sqlCmd.Dispose()
                    End Try
                End Using
            End Using
        Catch ee As SqlException
            message = ee.Message
        Catch ex As Exception
            message = ex.Message
        End Try

        Dim ret As String = String.Empty

        ret = "{IsView:'" & IsView & "', message: " & IIf(message = "", "''", message) & "}"
        Return New HttpResponseMessage() With { _
            .Content = New JsonContent(JObject.Parse(ret)) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Async Function saveUploadPhoto() As Task(Of HttpResponseMessage) ' ByVal obj As Newtonsoft.Json.Linq.JObject
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        If Not Request.Content.IsMimeMultipartContent() Then
            Me.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType)
        End If

        Dim root = HttpContext.Current.Server.MapPath("~/Upload/")
        Directory.CreateDirectory(root)

        Dim provider = New MultipartFormDataStreamProvider(root),
            result = Await Request.Content.ReadAsMultipartAsync(provider),
            ID_Persona As Integer = getTable("select ID_Persona from dbo.tEmployee where ID = " & result.FormData.Item("empid")).Rows(0)("ID_Persona"),
            OldImageFile As String = getTable("select ISNULL(ImageFile_GUID, '') as ImageFile_GUID from dbo.tPersona where ID = " & ID_Persona).Rows(0)("ImageFile_GUID")

        'upload
        For Each mfData As MultipartFileData In result.FileData
            Dim LocalFileName As String = mfData.LocalFileName
            Dim CDFileName As String = mfData.Headers.ContentDisposition.FileName

            Dim fileGUID As String = System.Guid.NewGuid.ToString,
                                   uploadedFileInfo = New FileInfo(LocalFileName),
                                   fileName As String = String.Empty
            If String.IsNullOrEmpty(CDFileName) Then
                fileName = fileGUID
            End If
            fileName = CDFileName
            If fileName.StartsWith("""") And fileName.EndsWith("""") Then
                fileName = fileName.Trim("""")
            End If
            If fileName.Contains("/") Or fileName.Contains("\") Then
                fileName = Path.GetFileName(fileName)
            End If

            fileName = fileGUID & Path.GetExtension(fileName)

            Using sqlConn As New SqlConnection(ConnectionString)
                sqlConn.Open()
                Dim transaction As SqlTransaction

                ' Start a local transaction
                transaction = sqlConn.BeginTransaction("Trans")

                Using sqlCommand As New SqlCommand("Update dbo.tPersona set ImageFile = @ImageFile, ImageFile_GUID = @ImageFile_GUID where ID = @ID ", sqlConn)
                    Try
                        With sqlCommand
                            .Transaction = transaction
                            .Parameters.AddWithValue("@ID", ID_Persona)
                            .Parameters.AddWithValue("@ImageFile", CDFileName.Replace("""", ""))
                            .Parameters.AddWithValue("@ImageFile_GUID", fileName)
                            .ExecuteNonQuery()

                            File.Move(LocalFileName, Path.Combine(HttpContext.Current.Server.MapPath("~/Upload/"), fileName))
                            If OldImageFile.ToLower <> "avatar.png" And IsNull(OldImageFile.ToString, "") <> "" Then
                                File.Delete(Path.Combine(HttpContext.Current.Server.MapPath("~/Upload/"), OldImageFile))
                            End If
                            transaction.Commit()

                        End With
                    Catch ex As Exception
                        transaction.Rollback()
                        Throw ex
                    Finally
                        sqlCommand.Dispose()
                    End Try
                End Using
                sqlConn.Close()
            End Using

        Next
        'end of upload

        Dim ret As String = String.Empty,
            message As String = String.Empty,
            messageType As MessageType = messageType.Success,
            id As Integer = 0,
            r As New JObject

        Try
            ret = "{"
            ret &= "message : '" & addStripSlashes(message) & "'"
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "'}")
        End Try

        Return New HttpResponseMessage() With { _
                 .Content = New JsonContent(r) _
               }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function getAttachments(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        Dim refID As Integer = obj("refID").ToObject(Of Integer)(),
            message As String = String.Empty, ret As String = String.Empty,
            r As New JObject, data As String = String.Empty

        Try
            data = getJSONTable("select * from dbo.tAnnouncements_Detail where ID_Announcements = " & refID & "")
        Catch ex As Exception
            message = ex.Message
        End Try

        Try
            ret = "{"
            ret &= "data : " & data & ""
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "'}")
        End Try
        Return New HttpResponseMessage() With { _
            .Content = New JsonContent(r) _
        }
    End Function

    '<HttpPost>
    '<DeflateCompression>
    '<CheckCSRFHeader>
    'Public Function loadList(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
    '    If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
    '    Dim rID As Integer = obj("rID").ToObject(Of Integer)(),
    '        mID As Integer = obj("mID").ToObject(Of Integer)()
    '    Try
    '        Using sqlCon As New SqlConnection(ConnectionString)
    '            sqlCon.Open()
    '            Dim m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(mID)
    '            Try
    '                Dim listKey As String = m.dtColumns().AsEnumerable().Where(Function(x) x.Item("ListKey") = True).First.Item("Name"),
    '                    parentKey As String = m.dtColumns().AsEnumerable().Where(Function(x) x.Item("ParentKey") = True).First.Item("Name")

    '                Using sqlCmd As New SqlCommand("SELECT NULL AS ID, " & listKey & "," & parentKey & "," & listKey.Substring(3) & _
    '                                               " FROM " & m.ColumnValue("ListSource") & " a ")


    '                End Using
    '            Catch ex As Exception
    '            Finally
    '                sqlCon.Close()
    '                sqlCon.Dispose()
    '            End Try


    '        End Using
    '    Catch ee As SqlException
    '        'message = ee.Message
    '    Catch ex As Exception
    '        ' message = ex.Message
    '    End Try

    '    Dim ret As String = String.Empty

    '    ret = "{data:" & IsView & "}"
    '    Return New HttpResponseMessage() With { _
    '        .Content = New JsonContent(JObject.Parse(ret)) _
    '    }
    'End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function GetApproverEmployee(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)

        Dim ret As String = String.Empty,
            ID_Employee As Integer = obj("eID").ToObject(Of Integer)(),
            r As New JObject
        Try
            ret = "{ data:"
            ret += getJSONTable("SELECT * FROM dbo.fzgetApproverUnder(" & HttpContext.Current.Session("ID_User") & ")")
            ret += "}"
            r = JObject.Parse(ret)

        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
        End Try

        Return New HttpResponseMessage() With { _
           .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    Public Function getSecurityQuestion(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        Dim ret As String = String.Empty,
            r As New JObject
        Try
            ret = "{ data:"
            ret += getJSONTable("SELECT ID,Name FROM dbo.vSecretQuestion")
            ret += "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
        End Try

        Return New HttpResponseMessage() With { _
           .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    Public Function SaveSecretQuestion(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        Dim ret As String = String.Empty,
            r As New JObject,
            ID_User As Integer = HttpContext.Current.Session("ID_User"),
            ID_SecretQuestion As Integer = obj("ID_SecurityQuestion").ToObject(Of Integer)(),
            SecretAnswer As String = obj("SecurityAnswer").ToObject(Of String)()

        Using sqlConn As New SqlConnection(ConnectionString)
            sqlConn.Open()
            Try
                Using sqlcmd As New SqlCommand("Update dbo.tUser set ID_SecretQuestion = @ID_SecurityQuestion, SecretAnswer = @SecretAnswer where ID = @ID_User", sqlConn)
                    sqlcmd.Parameters.AddWithValue("ID_SecurityQuestion", ID_SecretQuestion)
                    sqlcmd.Parameters.AddWithValue("SecretAnswer", SecretAnswer)
                    sqlcmd.Parameters.AddWithValue("ID_User", ID_User)
                    sqlcmd.ExecuteNonQuery()

                End Using

                ret = "{ data: {isAuthenticated : 'true'}}"
                r = JObject.Parse(ret)
                HttpContext.Current.Session("IsSecretQuestionReady") = 1
            Catch ex As Exception
                r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
            Finally
                sqlConn.Close()
                sqlConn.Dispose()
            End Try

        End Using

        Return New HttpResponseMessage() With { _
           .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    Public Function SaveSQ(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)

        Dim ret As String = String.Empty,
            r As New Newtonsoft.Json.Linq.JObject,
            message As String = String.Empty,
            ID_SecretQuestion As Integer = obj("ID_SecretQuestion").ToObject(Of Integer)(),
            ID_NewSecretQuestion As Integer = obj("NewID_SecretQuestion").ToObject(Of Integer)(),
            SecretAnswer As String = obj("SecretAnswer").ToObject(Of String)(),
            NewSecretAnswer As String = obj("NewSecretAnswer").ToObject(Of String)()

        Try
            ' GetDirectReference(obj("ID").ToObject(Of String)(), HttpContext.Current.Session)
            Dim ID As Integer = toBase10(obj("ID").ToObject(Of String)())
            Using sqlConn As New SqlConnection(ConnectionString)
                sqlConn.Open()

                'validate password if it is not first log ----------------- 
                Try
                    Using sqlcmd As New SqlCommand("EXEC dbo.pChangeSecretQuestion @ID_SecretQuestion, @SecretAnswer, @ID_NewSecretQuestion, @NewSecretAnswer, @ID", sqlConn)
                        sqlcmd.Parameters.AddWithValue("ID_SecretQuestion", ID_SecretQuestion)
                        sqlcmd.Parameters.AddWithValue("SecretAnswer", SecretAnswer)
                        sqlcmd.Parameters.AddWithValue("ID_NewSecretQuestion", ID_NewSecretQuestion)
                        sqlcmd.Parameters.AddWithValue("NewSecretAnswer", NewSecretAnswer)
                        sqlcmd.Parameters.AddWithValue("ID", ID)
                        Dim i As Integer = sqlcmd.ExecuteNonQuery()
                    End Using

                Catch ex As Exception
                    message = ex.Message
                End Try

            End Using

        Catch ee As SqlException
            message = ee.Message
        Catch ex As Exception
            message = ex.Message
        Finally
        End Try

        Try
            ret = "{"
            ret &= "message : '" & addStripSlashes(message) & "'"
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
            logError(ex)
        End Try

        Return New HttpResponseMessage() With { _
           .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Async Function AsyncCommand(ByVal obj As Newtonsoft.Json.Linq.JObject) As Task(Of HttpResponseMessage)
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
        UID = HttpContext.Current.Session("ID_User")
        Dim mID As Integer = obj("mID").ToObject(Of Integer)(),
            rID As Integer = 0,
            btnID As Integer = obj("btnID").ToObject(Of Integer)(),
            drMaster As CaseInsensitiveDictionary = JsonConvert.DeserializeObject(Of CaseInsensitiveDictionary)(obj("Master"))
        If obj("rID").ToObject(Of String)() <> "0" Then
            Try
                rID = toBase10(obj("rID").ToObject(Of String)()) ' CInt(GetDirectReference(obj("rID").ToObject(Of String)(), HttpContext.Current.Session))
            Catch ex As Exception
                logError(ex, mID)
                Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
            End Try
        End If
        Dim ret As String = String.Empty,
            message As String = String.Empty,
            messageType As MessageType = messageType.Success,
            id As Integer = rID,
            r As New JObject

        Dim m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(mID)

        If Not UserHasAccessByUserGroup(m) Then Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
        If Not UserHasAccessByData(m, rID, 0) Then Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)

        Try
            Dim btn As DataRow = m.dtButtons.Select("ID = " & btnID)(0)
            ' BEFORE EXECUTE
            messageType = messageType.BeforeExecute
            ExecuteButtonValidation(m, 1, btnID, drMaster, rID, HttpContext.Current.Session)

            Using sqlConn As New SqlConnection(ConnectionString)
                sqlConn.FireInfoMessageEventOnUserErrors = True
                AddHandler sqlConn.InfoMessage, New SqlInfoMessageEventHandler(AddressOf AsyncMessage)
                sqlConn.Open()

                Using sqlCommand As New SqlCommand(btn("CommandText").ToString, sqlConn)
                    Try
                        If Not m.dtButtonParameters(btn("ID")) Is Nothing Then
                            For Each dr2 As DataRow In m.dtButtonParameters(btn("ID")).Rows
                                Select Case dr2("Value").ToString.Substring(0, 1)
                                    Case "@"
                                        If dr2("Value").ToString = "@ID_WebMenus" Then
                                            sqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, mID.ToString)
                                        Else
                                            sqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, HttpContext.Current.Session(dr2("Value").ToString.Substring(1)).ToString)
                                        End If
                                    Case "#"
                                    Case "$"
                                    Case Else
                                        If dr2("Value").ToString = "ID" Then
                                            sqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, rID.ToString)
                                        Else
                                            'sqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, IIf(IsNothing(CTypeDynamic(HttpUtility.HtmlEncode(drMaster(dr2("Value").ToString).ToString.Replace(Chr(160), Chr(32))), GetDataType(m.dtColumns.Select("Name = '" + dr2("Value").ToString + "'")(0).Item("colDataType").ToString))), "''", CTypeDynamic(HttpUtility.HtmlEncode(drMaster(dr2("Value").ToString).ToString.Replace(Chr(160), Chr(32))), GetDataType(m.dtColumns.Select("Name = '" + dr2("Value").ToString + "'")(0).Item("colDataType").ToString))))
                                            If IsNothing(drMaster(dr2("Value").ToString)) Then
                                                'If IsNothing(CTypeDynamic(HttpUtility.HtmlEncode(drMaster(dr2("Value").ToString).ToString.Replace(Chr(160), Chr(32))), GetDataType(m.dtColumns.Select("Name = '" + dr2("Value").ToString + "'")(0).Item("colDataType").ToString))) Then
                                                sqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, "")
                                            Else
                                                sqlCommand.Parameters.AddWithValue("@" + dr2("Name").ToString, CTypeDynamic(HttpUtility.HtmlEncode(drMaster(dr2("Value").ToString).ToString.Replace(Chr(160), Chr(32))), GetDataType(m.dtColumns.Select("Name = '" + dr2("Value").ToString + "'")(0).Item("colDataType").ToString)))
                                            End If
                                        End If
                                End Select
                            Next
                        End If
                        sqlCommand.CommandTimeout = 0
                        Await sqlCommand.ExecuteNonQueryAsync()
                        message = btn("Message").ToString

                    Catch ex As Exception
                        messageType = messageType.BeforeExecute
                        Throw ex
                    Finally
                        sqlCommand.Dispose()
                    End Try
                End Using
                sqlConn.Close()
            End Using


            ' AFTER EXECUTE
            messageType = messageType.AfterExecute
            ExecuteButtonValidation(m, 2, btnID, drMaster, rID, HttpContext.Current.Session)

        Catch ee As SqlException
            ' messageType = messageType.BeforeExecute
            message = ee.Message
        Catch ex As Exception
            ' messageType = messageType.BeforeExecute
            message = ex.Message
            logError(ex, mID)
        Finally
        End Try
        ''PAG DI NA EXISTING RECORD GO BACK ^
        Dim ctr2 As Integer = ExecScalarNoParams("SELECT COUNT(ID) FROM " & SetDataSourceFilter(m, 0, 0, HttpContext.Current.Session) & " WHERE ID = " & rID)
        Dim newmID As Integer = 0
        'Dim redirectMenuID As Integer = m.RedirectMenu
        If ctr2 = 0 Then
            newmID = CInt(IsNull(m.RedirectMenu, m.ParentID))
        End If

        Try
            ret = "{"
            ret &= "message : '" & addStripSlashes(message) & "',"
            ret &= "messageType : '" & messageType & "',"
            ret &= "ID : '" & If(ctr2 = 0, "0", toAnyBase(id)) & "'," ' GetIndirectReference(id, HttpContext.Current.Session)
            ret &= "mID : '" & newmID & "'"
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
            logError(ex, mID)
        End Try

        Return New HttpResponseMessage() With { _
         .Content = New JsonContent(r) _
       }
    End Function

    Public Sub AsyncMessage(sender As Object, args As SqlInfoMessageEventArgs)
        Dim message As String = args.Errors(0).Message
        SignalRHub.SignalRHub.ShowNotifications(UID, message)
    End Sub

    <HttpPost>
    <DeflateCompression>
    Public Function getColumnValues(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        Dim value As Integer = 0
        If IsNull(obj("val").ToObject(Of String)(), "") <> "" Then
            value = obj("val").ToObject(Of Integer)()
        End If
        Dim ret As String = String.Empty,
            r As New JObject,
            ID_WebMenus As Integer = obj("ID_WebMenus").ToObject(Of Integer)(),
            colID As Integer = obj("colID").ToObject(Of Integer)()

        Dim m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(ID_WebMenus)
        Dim cols As DataRow() = m.dtColumns.Select("ISNULL(JSEvents,'') <> '' and ID_WebMenuControlTypes = 17")
        Dim collector As New List(Of String)
        For Each c As DataRow In cols
            Dim jsEvents As String = c("JSEvents").ToString.Split("=")(0).ToLower()
            Dim sb As New StringBuilder
            If jsEvents = "getcolumnvalues" Then
                Dim columns() As String = c("JSEvents").ToString.Split("=")(1).Split(";")
                Dim colName As New List(Of String)
                For Each c2 As String In columns
                    Dim item() As String = c2.Split(":")
                    colName.Add(item(1) & " as " & item(0))
                Next
                Dim m2 As GSWEB.MenuCollection.Menu = mCollection.GetMenu(c("ID_TargetWebMenus"))
                Dim ds As String = "SELECT " & String.Join(",", colName) & " FROM " & SetDataSourceFilter(m2, , , HttpContext.Current.Session, , ) & " where ID = " & value.ToString
                Dim jsonTable As String = getJSONTable(ds)
                sb.Append("{" & c("ID") & ":")
                sb.Append(jsonTable)
                sb.Append("}")
                collector.Add(sb.ToString)
            End If
        Next

        Try
            ret = "{ data:"
            ret += String.Join(",", collector)
            ret += "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
        End Try

        Return New HttpResponseMessage() With { _
           .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    Public Function addToFavourites(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        Dim ret As String = String.Empty,
            r As New JObject,
            ID_WebMenus As Integer = obj("mID").ToObject(Of Integer)(),
            msg As String = "", type As Integer = 1

        Using sqlCon As New SqlConnection(ConnectionString)
            Try
                sqlCon.Open()
                Using sqlcmd As New SqlCommand("INSERT INTO dbo.tUserFavouriteWebMenu(ID_User, ID_WebMenus) VALUES (@ID_User, @ID_WebMenus)", sqlCon)
                    sqlcmd.Parameters.AddWithValue("ID_User", HttpContext.Current.Session("ID_User"))
                    sqlcmd.Parameters.AddWithValue("ID_WebMenus", ID_WebMenus)
                    Dim i As Integer = sqlcmd.ExecuteNonQuery()
                End Using
                msg = "Successfully added to favourites"
                type = 1
            Catch ex As Exception
                msg = ex.Message
                type = 2
            Finally
                sqlCon.Close()
                sqlCon.Dispose()
            End Try
        End Using
        
        Try
            ret = "{ data:"
            ret += "{'msg': '" & msg & "', 'type': '" & type & "'}"
            ret += "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
        End Try

        Return New HttpResponseMessage() With { _
           .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    Public Function getAllFavourites(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        Dim ret As String = String.Empty,
            r As New JObject,
            msg As String = "", type As Integer = 1, menuList As New List(Of String)

        Using sqlCon As New SqlConnection(ConnectionString)
            Try
                sqlCon.Open()
                Dim dt As DataTable = getTable("select * from dbo.vUserFavouriteWebMenu where ID_User = " & HttpContext.Current.Session("ID_User"))
                For Each dr As DataRow In dt.Rows
                    Dim m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(dr("ID_WebMenus"))
                    If m.HasAccess(HttpContext.Current.Session("ID_UserGroup")) Then
                        menuList.Add("{'mID':" & dr("ID_WebMenus") & ", 'WebMenus':'" & dr("WebMenus") & "', 'url':'" & dr("url") & "', 'bgcolor':'" & dr("bgcolor") & "','color':'" & dr("color") & "','icon':'" & dr("icon") & "'}")
                    End If
                Next
                type = 1
            Catch ex As Exception
                msg = ex.Message
                type = 2
            Finally
                sqlCon.Close()
                sqlCon.Dispose()
            End Try
        End Using

        Try
            ret = "{ data:"
            ret += "{'msg': '" & msg & "', 'type': '" & type & "', 'menus': [" & String.Join(",", menuList) & "]}"
            ret += "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
        End Try

        Return New HttpResponseMessage() With { _
           .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    Public Function removeFromFavourites(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        Dim ret As String = String.Empty,
            r As New JObject,
            ID_WebMenus As Integer = obj("mID").ToObject(Of Integer)(),
            msg As String = "", type As Integer = 1

        Using sqlCon As New SqlConnection(ConnectionString)
            Try
                sqlCon.Open()
                Using sqlcmd As New SqlCommand("DELETE FROM dbo.tUserFavouriteWebMenu WHERE ID_WebMenus = @ID_WebMenus AND ID_User = @ID_User", sqlCon)
                    sqlcmd.Parameters.AddWithValue("ID_User", HttpContext.Current.Session("ID_User"))
                    sqlcmd.Parameters.AddWithValue("ID_WebMenus", ID_WebMenus)
                    Dim i As Integer = sqlcmd.ExecuteNonQuery()
                End Using
                msg = "Successfully removed favourites"
                type = 1
            Catch ex As Exception
                msg = ex.Message
                type = 2
            Finally
                sqlCon.Close()
                sqlCon.Dispose()
            End Try
        End Using

        Try
            ret = "{ data:"
            ret += "{'msg': '" & msg & "', 'type': '" & type & "'}"
            ret += "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
        End Try

        Return New HttpResponseMessage() With { _
           .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function CascadingDropdown2(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)



        Dim ret As String = String.Empty,
            Data As String = String.Empty,
            r As New JObject

        'If Not UserHasAccessByUserGroup(mCollection.GetMenu(mID)) Then Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
        '  If Not UserHasAccessByData(mCollection.GetMenu(mID), rID, 0) Then Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)

        Try


            Using sqlConn As New SqlConnection(ConnectionString)
                sqlConn.Open()
                Dim mID As Integer = obj("mID").ToObject(Of Integer)(),
                    colID As Integer = obj("colID").ToObject(Of Integer)(),
                    value As Integer = obj("value").ToObject(Of Integer)(),
                    m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(mID),
                    btn As DataRow = m.dtColumns.Select("ID = " & colID)(0),
                    tmp As String = btn("JSEvents").ToString.Split("=")(1),
                    tmp2 As String() = tmp.Split(";")
                Data &= "{"
                For Each a In tmp2(0).Split(",")
                    Dim targetColumn As DataRow = m.dtColumns.Select("ID = " & a)(0)
                    Dim valueToGet As String = getTable("SELECT cast(" & tmp2(1) & " as VARCHAR(8000)) FROM " & IsNull(btn("TableName"), "v" & btn("Name").ToString.Substring(3)) & " WHERE ID = " & value).Rows(0)(0)
                    Dim cmdTxt As String = "SELECT " & IsNull(targetColumn("DisplayID"), "ID") & " AS ID," & IsNull(targetColumn("DisplayMember"), "Name") & " AS Name FROM " & IsNull(targetColumn("TableName"), "v" & targetColumn("Name").ToString.Substring(3)) & " WHERE ID = @value"
                    Using sqlCommand As New SqlCommand(cmdTxt, sqlConn)
                        sqlCommand.Parameters.AddWithValue("@value", valueToGet)
                        Data &= a & ":" & serialize(sqlCommand.ExecuteReader) & ","
                        sqlCommand.Dispose()
                    End Using
                Next
                Data &= "}"
                sqlConn.Close()
                sqlConn.Dispose()
            End Using



        Catch ee As SqlException
            'messageType = messageType.BeforeExecute
            'message = ee.Message
        Catch ex As Exception
            'messageType = messageType.BeforeExecute
            'message = ex.Message
        Finally

        End Try
        Try
            ret = "{"
            ret &= "data : " & Data & " "
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
        End Try

        Return New HttpResponseMessage() With { _
          .Content = New JsonContent(r) _
        }
    End Function

    <HttpPost>
    <DeflateCompression>
    <CheckCSRFHeader>
    Public Function getMoreRecord(ByVal obj As Newtonsoft.Json.Linq.JObject) As HttpResponseMessage
        If IsNull(HttpContext.Current.Session("ID_User"), 0) = 0 Then Return New HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)



        Dim ret As String = String.Empty,
            Data As String = String.Empty,
            r As New JObject

        'If Not UserHasAccessByUserGroup(mCollection.GetMenu(mID)) Then Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)
        '  If Not UserHasAccessByData(mCollection.GetMenu(mID), rID, 0) Then Return New HttpResponseMessage(System.Net.HttpStatusCode.NotFound)

        Try


            Using sqlConn As New SqlConnection(ConnectionString)
                sqlConn.Open()
                Dim lastID As Integer = obj("lastID").ToObject(Of Integer)(),
                    colID As Integer = obj("colID").ToObject(Of Integer)(),
                    menuID As Integer = getTable("select ID_WebMenus from dbo.vWebMenuColumns where ID = " & colID).Rows(0)(0),
                    m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(menuID),
                    dr As DataRow = m.dtColumns.Select("ID = " & colID)(0),
                    m2 As GSWEB.MenuCollection.Menu = mCollection.GetMenu(dr("ID_TargetWebMenus")),
                    cols As New List(Of String)

                For Each rows As DataRow In m2.dtColumns.Select
                    If IsNull(rows("Label"), "") = "" Then
                        cols.Add("[" & rows("Name") & "]")
                    ElseIf IsNull(rows("Label"), "") <> "" And rows("Name").ToString.ToLower <> "id" And rows("Name").ToString.ToLower.StartsWith("id_") Then
                        cols.Add("[" & rows("Name").ToString.Substring(3) & "]" & " as [" & rows("Label") & "]")
                    ElseIf IsNull(rows("Label"), "") <> "" And rows("Name").ToString.ToLower <> "id" And Not (rows("Name").ToString.ToLower.StartsWith("id_")) Then
                        cols.Add("[" & rows("Name") & "]" & " as [" & rows("Label") & "]")
                    Else
                        cols.Add("[" & rows("Name") & "]")
                    End If
                Next

                Dim ds As String = "SELECT TOP 10 " & String.Join(",", cols) & " FROM " & SetFilter(m2, m2.ColumnValue("DataSource"), 0, 0, HttpContext.Current.Session) & " WHERE ID > " & lastID & " ORDER BY ID ASC"
                Data = getJSONTable(ds)
                sqlConn.Close()
                sqlConn.Dispose()
            End Using



        Catch ee As SqlException
            'messageType = messageType.BeforeExecute
            'message = ee.Message
        Catch ex As Exception
            'messageType = messageType.BeforeExecute
            'message = ex.Message
        Finally

        End Try
        Try
            ret = "{"
            ret &= "data : " & Data & " "
            ret &= "}"
            r = JObject.Parse(ret)
        Catch ex As Exception
            r = JObject.Parse("{ error : '" & addStripSlashes(ex.Message) & "' }")
        End Try

        Return New HttpResponseMessage() With { _
          .Content = New JsonContent(r) _
        }
    End Function

#Region "PRIVATE METHODS"

    Private Sub FillExcelSheet(wb As HSSFWorkbook, sheet As HSSFSheet, dt As DataTable, Optional targetMenu As Integer = 0)
        Dim font As HSSFFont = wb.CreateFont
        font.Boldweight = NPOI.SS.UserModel.FontBoldWeight.Bold
        font.FontHeightInPoints = 9
        font.FontName = "Arial"
        Dim hrow As HSSFRow = sheet.CreateRow(0)
        Dim ctr As Integer = 0


        If targetMenu <> 0 Then
            Dim m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(targetMenu)
            Dim csRequired As HSSFCellStyle = wb.CreateCellStyle
            csRequired.SetFont(font)
            csRequired.FillForegroundColor = HSSFColor.Pink.Index
            csRequired.FillPattern = FillPattern.SolidForeground
            Dim csNotRequired As HSSFCellStyle = wb.CreateCellStyle
            csNotRequired.FillForegroundColor = HSSFColor.Grey25Percent.Index
            csNotRequired.FillPattern = FillPattern.SolidForeground
            csNotRequired.SetFont(font)
            For Each dc As DataColumn In dt.Columns
                hrow.CreateCell(ctr).SetCellValue(dc.ColumnName)
                Dim rowcell As HSSFCell = hrow.GetCell(ctr)
                If IsNull(m.dtColumns.Select("Name = '" & dc.ColumnName & "'")(0)("HelperText"), "") <> "" Then
                    Dim HelperText() As String = m.dtColumns.Select("Name = '" & dc.ColumnName & "'")(0)("HelperText").ToString.Split(";")
                    Dim idr As IDrawing = sheet.CreateDrawingPatriarch()
                    Dim cmt As IComment = idr.CreateCellComment(New HSSFClientAnchor(0, 0, 0, 0, 4, 2, 10, HelperText.Length + 5))
                    Dim ht As String = ""
                    For Each hStr As String In HelperText
                        ht &= hStr + vbNewLine
                    Next
                    Dim richtext As HSSFRichTextString = New HSSFRichTextString(ht)
                    cmt.String = richtext
                    rowcell.CellComment = cmt
                End If
                If m.dtColumns.Select("IsRequired = 1 AND Name = '" & dc.ColumnName & "'").Count > 0 Then
                    rowcell.CellStyle = csRequired
                Else
                    rowcell.CellStyle = csNotRequired
                End If
                'rowcell.CellStyle.SetFont(font)
                ctr += 1
            Next
        Else
            For Each dc As DataColumn In dt.Columns
                hrow.CreateCell(ctr).SetCellValue(dc.ColumnName)
                Dim rowcell As HSSFCell = hrow.GetCell(ctr)
                rowcell.CellStyle.SetFont(font)

                ctr += 1
            Next
        End If

        Dim fontNormal As HSSFFont = wb.CreateFont
        fontNormal.Boldweight = NPOI.SS.UserModel.FontBoldWeight.Normal
        fontNormal.FontHeightInPoints = 8
        fontNormal.FontName = "Arial"
        Dim cellStyleNormal As HSSFCellStyle = wb.CreateCellStyle
        cellStyleNormal.SetFont(fontNormal)
        Dim row As Integer = 1, col As Integer = 0
        For Each dr As DataRow In dt.Select
            hrow = sheet.CreateRow(row)

            For Each dc As DataColumn In dt.Columns
                Dim cell = hrow.CreateCell(col)
                cell.CellStyle = cellStyleNormal
                Select Case dc.DataType
                    Case System.Type.GetType("System.Int32")
                        If Not IsDBNull(dr(dc.ColumnName)) Then
                            cell.SetCellValue(CInt(dr(dc.ColumnName)))
                        End If
                    Case System.Type.GetType("System.DateTime")
                        If Not IsDBNull(dr(dc.ColumnName)) Then
                            cell.SetCellValue(dr(dc.ColumnName).ToString)
                        End If
                    Case System.Type.GetType("System.Boolean")
                        If Not IsDBNull(dr(dc.ColumnName)) Then
                            cell.SetCellValue(CBool(dr(dc.ColumnName).ToString))
                        End If
                    Case System.Type.GetType("System.Decimal")
                        If Not IsDBNull(dr(dc.ColumnName)) Then
                            cell.SetCellValue(CDbl(dr(dc.ColumnName).ToString))
                        End If
                    Case Else
                        cell.SetCellValue(dr(dc.ColumnName).ToString)
                End Select
                '   cell.CellStyle.SetFont(fontNormal)
                col += 1
            Next
            col = 0
            row += 1
        Next
        For i = 0 To dt.Columns.Count - 1
            sheet.AutoSizeColumn(i)
        Next
    End Sub

#End Region

#Region "CUSTOM ATTRIBUTES"

    Public Class DeflateCompressionAttribute
        Inherits ActionFilterAttribute
        Public Overrides Sub OnActionExecuted(actContext As HttpActionExecutedContext)
            Dim content = actContext.Response.Content
            Dim bytes = If(content Is Nothing, Nothing, content.ReadAsByteArrayAsync().Result)
            Dim zlibbedContent = If(bytes Is Nothing, New Byte(-1) {}, CompressionHelper.DeflateByte(bytes))
            actContext.Response.Content = New ByteArrayContent(zlibbedContent)
            actContext.Response.Content.Headers.Remove("Content-Type")
            actContext.Response.Content.Headers.Add("Content-encoding", "deflate")
            actContext.Response.Content.Headers.Add("Content-Type", "application/json")
            GC.Collect()
            GC.SuppressFinalize(Me)
            MyBase.OnActionExecuted(actContext)
        End Sub
    End Class

    Public Class CheckCSRFHeaderAttribute
        Inherits AuthorizeAttribute

        Protected Overrides Function IsAuthorized(context As Controllers.HttpActionContext) As Boolean
            Dim ID_User As String = HttpContext.Current.Session("ID_User")
            If String.IsNullOrEmpty(ID_User) Then Return False
            Dim authToken As String = CStr(HttpContext.Current.Session("ID_User")) + "_" + CStr(HttpContext.Current.Session("CSRF-TOKEN"))
            Dim csrfToken As String = context.Request.Headers.GetValues("X-CSRF-Token").FirstOrDefault()
            If (String.IsNullOrEmpty(csrfToken)) Then Return False
            Return New CsrfTokenHelper().DoesCsrfTokenMatchAuthToken(csrfToken, authToken)
        End Function

    End Class



#End Region
End Class
