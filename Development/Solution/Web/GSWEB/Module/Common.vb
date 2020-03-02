Imports System.IO
Imports System.Web
Imports System.Web.Http
Imports System.Data.SqlClient
Imports System.Linq
Imports Newtonsoft.Json
Imports System.Text
Imports System.Net.Http
Imports GSWEB.Utility
Imports System.Web.Optimization
Imports GSWEB.Common.Validation
Imports System.Text.RegularExpressions
Imports Yahoo.Yui.Compressor
Imports System.Net.Mail

Namespace Common

    Public Module Common

        Public connServer As String
        Public connDatabase As String
        Public connUserName As String
        Public connPassword As String
        Public ConnectionString As String
        Public connPort As String
        Public dtSetting As New DataTable
        Public mCollection As New MenuCollection.MenuClass
        Public TabsCollection As New MenuCollection.MenuObjects
        Public columnsCollection As New MenuCollection.MenuObjects
        Public buttonsCollection As New MenuCollection.MenuObjects
        Public buttonsValCollection As New MenuCollection.MenuObjects
        Public buttonsParamCollection As New MenuCollection.MenuObjects
        Public columnsValCollection As New MenuCollection.MenuObjects
        Public menuSubDataSourceCollection As New MenuCollection.MenuObjects

        Public bases As String = "ZYXWVUTSRQPONMLKJIHGFEDCBAzyxwvutsrqponmlkjihgfedcba9876543210"
        Public secretKey As Integer = 95242 'IDYNAMIC BA PER CLIENT?
        'Public smtpServer As New MailSender

        Public Sub Register(ByVal config As HttpConfiguration)
            config.Routes.MapHttpRoute( _
                name:="DefaultApi", _
                routeTemplate:="api/{controller}/{action}/{id}", _
                defaults:=New With {.id = RouteParameter.Optional} _
            )
            'route.RouteHandler = New MyHttpControllerRouteHandler()
            ''routeTemplate:="api/{controller}/{id}"  '' ADD ACTION FOR MULTIPLE POST
        End Sub

        Public Sub LoadSettings()
            Try
                dtSetting = getTable("SELECT Name,Value FROM tSetting WHERE Active = 1")
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

#Region "Menu Set"

        Public Sub LoadMenuSet()
            Try
                Dim dt As DataTable = getTable("SELECT * FROM vWebMenus WHERE IsActive = 1")
                For Each drow As DataRow In dt.Rows
                    mCollection.Add(New MenuCollection.Menu(drow))
                Next
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Public Sub LoadTabs(ByVal menuID As Integer, Optional ByVal cnt As Integer = 0)
            Try
                If cnt = 0 Then
                    Dim dtColumns As DataTable = getTable("SELECT * FROM vWebMenuTabs WHERE IsActive = 1 AND ID_WebMenus = " + menuID.ToString)
                    For Each crow As DataRow In dtColumns.Rows
                        TabsCollection.Add(crow)
                    Next
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Public Sub LoadColumns(ByVal menuID As Integer, Optional ByVal cnt As Integer = 0)
            Try
                If cnt = 0 Then
                    Dim dtColumns As DataTable = getTable("SELECT * FROM vWebMenuColumns WHERE IsActive = 1 AND ID_WebMenus = " + menuID.ToString)
                    For Each crow As DataRow In dtColumns.Rows
                        columnsCollection.Add(crow)
                    Next
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Public Sub LoadButtons(ByVal menuID As Integer, Optional ByVal cnt As Integer = 0)
            Try
                If cnt = 0 Then
                    Dim dtButtons As DataTable = getTable("SELECT * FROM vWebMenuButtons WHERE IsActive = 1 AND ID_WebMenus = " + menuID.ToString)
                    For Each brow As DataRow In dtButtons.Rows
                        buttonsCollection.Add(brow)
                        LoadButtonValidations(brow("ID").ToString)
                    Next
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Public Sub LoadButtonValidations(ByVal buttonID As Integer, Optional ByVal cnt As Integer = 0)
            Try
                If cnt = 0 Then
                    Dim dtButtonValidations As DataTable = getTable("SELECT * FROM dbo.vWebMenuButton_Validation WHERE IsActive = 1 AND ID_WebMenuButtons = " + buttonID.ToString)
                    For Each browv As DataRow In dtButtonValidations.Rows
                        buttonsValCollection.Add(browv)
                    Next
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Public Sub LoadButtonParameters(ByVal buttonID As Integer, Optional ByVal cnt As Integer = 0)
            Try
                If cnt = 0 Then
                    Dim dtButtonValidations As DataTable = getTable("SELECT * FROM dbo.vWebMenuButtonParameters WHERE IsActive = 1 AND ID_WebMenuButtons = " + buttonID.ToString)
                    For Each browv As DataRow In dtButtonValidations.Rows
                        buttonsParamCollection.Add(browv)
                    Next
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Public Sub LoadColumnValidation(ByVal columnID As Integer, Optional ByVal cnt As Integer = 0)
            Try
                If cnt = 0 Then
                    Dim dtColumnValidations As DataTable = getTable("SELECT * FROM dbo.vWebMenuColumn_Validation WHERE IsActive = 1 AND ID_WebMenuColumns = " + columnID.ToString)
                    For Each col As DataRow In dtColumnValidations.Rows
                        columnsValCollection.Add(col)
                    Next
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Public Sub ClearCollection()
            mCollection.Clear()
            columnsCollection.Clear()
            buttonsCollection.Clear()
            buttonsParamCollection.Clear()
            buttonsValCollection.Clear()
            columnsValCollection.Clear()
            'mWidgets.Clear()
            'mLinks.Clear()
            menuSubDataSourceCollection.Clear()
        End Sub

        Public Function getMenuColParameters(menuID As Integer, Optional ByVal IsSelectCommand As Boolean = False, Optional ByVal IsEditableGrid As Boolean = False) As String
            Dim result As New Text.StringBuilder
            Dim m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(menuID)
            If m.dtColumns Is Nothing Then Return result.ToString
            Dim cols As DataRow() = m.dtColumns.Select("Name <> 'ID' " & IIf(IsEditableGrid = True, "AND ShowInList = 1", "AND IsVisible = 1") & IIf(IsSelectCommand = True, "", " AND IsReadOnly = 0") & " AND (ID_WebMenuControlTypes NOT IN(6,25,27,30) OR ID_WebMenuControlTypes IS NULL)", "SeqNo")
            For x = 0 To cols.Length - 1
                result.Append("@" + cols(x)("Name") & ",")
            Next x
            Return result.ToString.Trim(",")
        End Function

        Public Function getColumns(m As GSWEB.MenuCollection.Menu) As String
            Dim result As New Text.StringBuilder
            If m.dtColumns Is Nothing Then Return result.ToString
            Dim cols As DataRow() = m.dtColumns.Select("Name <> 'ID' AND ShowInList = 1 AND IsNull(isTitle,0) = 0 AND ID_WebMenuControlTypes NOT IN(6, 27)")
            For x = 0 To cols.Length - 1
                result.Append(cols(x)("Name") & ",")
                If cols(x)("ID_WebMenuControlTypes") = 23 Then
                    result.Append(cols(x)("Name").ToString.Substring(3) & ",")
                End If
            Next x
            Return result.ToString.Trim(",")
        End Function

        Public Sub InitMailSender()
            'Try
            '    Using ttCon As New SqlClient.SqlConnection(ConnectionString)
            '        ttCon.Open()
            '        Using ttCmd As New SqlCommand("SELECT Paramname AS Name, ParamValue AS	Value FROM dbo.tWebParameters WHERE ID_WebParameterGroup = 2", ttCon)
            '            Dim Reader As SqlDataReader
            '            Reader = ttCmd.ExecuteReader()

            '            'Initialize SMTP
            '            While Reader.Read
            '                Select Case Reader.Item(0).ToString
            '                    Case "SMTP Server"
            '                        smtpServer.smtpServer = Reader.Item(1)
            '                    Case "SMTP Username"
            '                        smtpServer.fromEmail = Reader.Item(1)
            '                    Case "SMTP Password"
            '                        smtpServer.fromEmailPassword = Reader.Item(1)
            '                    Case "SMTP Port"
            '                        smtpServer.smtpPort = CInt(Reader.Item(1))
            '                    Case "SSL Enabled"
            '                        smtpServer.SSLEnabled = CBool(Reader.Item(1))
            '                    Case "SMTP Delivery Method"
            '                    Case "AnonymousAuthentication"
            '                        smtpServer.AnonymousAuthentication = CBool(Reader.Item(1))
            '                    Case "DisplayName"
            '                        smtpServer.fromName = IsNull(Reader.Item(1), "")
            '                    Case "AttachmentPath"
            '                    Case "DisplayFromAddress"
            '                        smtpServer.displayFromAddress = IsNull(Reader.Item(1), "")
            '                End Select
            '            End While
            '            smtpServer.connString = ConnectionString
            '            smtpServer.Init()

            '            Reader.Close()
            '        End Using
            '        ttCon.Close()
            '    End Using
            'Catch ex As Exception
            '    '
            'End Try
        End Sub
#End Region

        Public Sub ds()

        End Sub

        Public Function getTable(cmdtxt As String) As DataTable
            Dim dt As New DataTable
            Try
                Using ttCon As New SqlClient.SqlConnection(ConnectionString)
                    Using ttCmd As New SqlCommand(cmdtxt, ttCon)
                        Using da As New SqlDataAdapter(ttCmd)
                            da.Fill(dt)
                        End Using
                    End Using
                    ttCon.Close()
                End Using
            Catch ex As Exception
                Throw ex
            End Try
            Return dt
        End Function

        Public Function getTables(cmdtxt As String) As DataSet
            Dim ds As New DataSet
            Try
                Using ttCon As New SqlClient.SqlConnection(ConnectionString)
                    Using ttCmd As New SqlCommand(cmdtxt, ttCon)
                        Using da As New SqlDataAdapter(ttCmd)
                            da.Fill(ds)
                        End Using
                    End Using
                    ttCon.Close()
                End Using
            Catch ex As Exception
                Throw ex
            End Try
            Return ds
        End Function

        Public Function getJSONTable(cmdtxt As String, Optional EncryptID As Boolean = False, Optional Session As System.Web.SessionState.HttpSessionState = Nothing) As String
            Dim ret As String = String.Empty
            Try
                Using ttCon As New SqlClient.SqlConnection(ConnectionString)
                    ttCon.Open()
                    Using ttCmd As New SqlCommand(cmdtxt, ttCon)
                        ret = serialize(ttCmd.ExecuteReader, EncryptID, Session)
                    End Using
                    ttCon.Close()
                End Using
            Catch ex As Exception
                Throw ex
            End Try
            Return ret
        End Function

        Public Function getJSONTableRow(cmdtxt As String, Optional EncryptID As Boolean = False, Optional Session As System.Web.SessionState.HttpSessionState = Nothing, Optional AdditionalFields As List(Of String) = Nothing) As String
            Dim sb As New Text.StringBuilder
            Try
                Using ttCon As New SqlClient.SqlConnection(ConnectionString)
                    ttCon.Open()
                    Using sqlCommand As SqlCommand = New SqlCommand(cmdtxt, ttCon)

                        Dim sw As New StringWriter(sb)
                        Using jsonWriter As JsonWriter = New JsonTextWriter(sw)
                            Dim dataReader As SqlDataReader = sqlCommand.ExecuteReader
                            jsonWriter.WriteStartObject()
                            While dataReader.Read

                                For i As Integer = 0 To dataReader.FieldCount - 1
                                    jsonWriter.WritePropertyName(dataReader.GetName(i))
                                    jsonWriter.WriteValue(dataReader(i))
                                Next

                                If Not AdditionalFields Is Nothing Then
                                    For Each field As String In AdditionalFields
                                        jsonWriter.WritePropertyName("$$" & field)
                                        'jsonWriter.WriteValue(GetIndirectReference(dataReader.Item(field), Session))
                                        jsonWriter.WriteValue(toAnyBase(dataReader.Item(field), 62))
                                    Next
                                End If
                                Exit While
                            End While


                            jsonWriter.WriteEndObject()
                            dataReader.Close()
                        End Using
                        sqlCommand.Dispose()

                    End Using
                    ttCon.Close()
                End Using
            Catch ex As Exception
                Throw ex
            End Try
            Return sb.ToString
        End Function

        Public Function getJSONTableRowNoClose(ByRef dataReader As SqlDataReader, Optional EncryptID As Boolean = False, Optional Session As System.Web.SessionState.HttpSessionState = Nothing, Optional AdditionalFields As List(Of String) = Nothing) As String
            Dim sb As New Text.StringBuilder
            Try
                Using sw As New StringWriter(sb)
                    Using jsonWriter As JsonWriter = New JsonTextWriter(sw)
                        jsonWriter.WriteStartObject()
                        While dataReader.Read
                            For i As Integer = 0 To dataReader.FieldCount - 1
                                jsonWriter.WritePropertyName(dataReader.GetName(i))
                                jsonWriter.WriteValue(dataReader(i))
                            Next
                            If EncryptID Then
                                jsonWriter.WritePropertyName("$$rID")
                                'jsonWriter.WriteValue(GetIndirectReference(dataReader.Item("ID"), Session))
                                jsonWriter.WriteValue(toAnyBase(dataReader.Item("ID"), 62))
                            End If
                            If Not AdditionalFields Is Nothing Then
                                For Each field As String In AdditionalFields
                                    jsonWriter.WritePropertyName("$$" & field)
                                    'jsonWriter.WriteValue(GetIndirectReference(dataReader.Item(field), Session))
                                    jsonWriter.WriteValue(toAnyBase(dataReader.Item(field), 62))
                                Next
                            End If
                            Exit While
                        End While
                        jsonWriter.WriteEndObject()
                        jsonWriter.Close()
                    End Using
                    sw.Close()
                End Using
            Catch ex As Exception
                Throw ex
            End Try
            Return sb.ToString
        End Function

        Public Function getJSONArray(cmdtxt As String) As String
            Dim ret As String = String.Empty
            Try
                Using ttCon As New SqlClient.SqlConnection(ConnectionString)
                    ttCon.Open()
                    Using ttCmd As New SqlCommand(cmdtxt, ttCon)
                        ret = JavascriptArray(ttCmd.ExecuteReader)
                    End Using
                    ttCon.Close()
                End Using
            Catch ex As Exception
                Throw ex
            End Try
            Return ret
        End Function

        Public Function IsNull(ByVal Input As Object, ByVal NullOutput As Object) As Object
            Return IIf(IsDBNull(Input), NullOutput, Input)
        End Function

        Public Function generateRand(len As Integer) As String
            Dim sb As New System.Text.StringBuilder

            Dim random As Random = New Random()
            Randomize()
            Dim ch As New Char
            For i = 0 To len
                If Math.Floor(Rnd() * 10000) Mod 3 = 0 Then
                    ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * Rnd() + 65))) ''capital letters
                ElseIf random.Next Mod 3 = 1 Then
                    ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * Rnd() + 97))) ''small letters
                Else
                    ch = Convert.ToChar(Convert.ToInt32(Math.Floor(10 * Rnd() + 48))) ''numbers
                End If

                sb.Append(ch)
            Next
            Return sb.ToString
        End Function

        Public Function ExecScalarNoParams(cmdtxt As String) As Object
            Dim result As New Object
            Try
                Using ttCon As New SqlClient.SqlConnection(ConnectionString)
                    ttCon.Open()
                    Using ttCmd As New SqlCommand(cmdtxt, ttCon)
                        result = ttCmd.ExecuteScalar()
                    End Using
                    ttCon.Close()
                End Using
            Catch ex As Exception
                Throw ex
            End Try
            Return result
        End Function

        Public Function dictionaryToJSON(dict As Dictionary(Of String, Object), Optional pID As Integer = 0, Optional Session As System.Web.SessionState.HttpSessionState = Nothing, Optional parentTable As String = "", Optional master As Dictionary(Of String, Object) = Nothing) As String
            Dim e = dict.[Select](Function(d) String.Format("{0}: {1}", d.Key, defaultValues(d.Value, pID, Session, parentTable, d.Key, master)))
            Return "{" & String.Join(",", e) + "}"
        End Function

        Public Function JObjectToDictionary(obj As Newtonsoft.Json.Linq.JObject) As Dictionary(Of String, Object)
            Dim ret As New Dictionary(Of String, Object)

            For Each prop In obj.Properties
                ret.Add(prop.Name, obj(prop.Name))
            Next

            Return ret
        End Function

        Public Function defaultValues(Value As Object, Optional ByVal parentID As Integer = 0, Optional Session As System.Web.SessionState.HttpSessionState = Nothing, Optional parentTable As String = "", Optional Key As String = "", Optional master As Dictionary(Of String, Object) = Nothing) As Object
            Dim result As New Object
            If parentID > 0 AndAlso Key.Contains("ID_") AndAlso "t" & Key.Substring(3) = parentTable Then
                result = parentID
            ElseIf IsDBNull(Value) Then
                result = "null"
            ElseIf Value.ToString.StartsWith("@@") Then
                Select Case Value
                    Case "@@CurrentYear"
                        result = Year(Date.Now).ToString
                    Case "@@CurrentMonth"
                        result = Month(Date.Now).ToString
                    Case "@@CurrentMonthName"
                        result = MonthName(Month(Date.Now)).ToString
                    Case "@@CurrentDate"
                        result = "'" & Date.Now.ToString("MM/dd/yyyy") & "'"
                    Case "@@CurrentTime"
                        result = "'" & Date.Now.ToShortTimeString & "'"
                End Select
            ElseIf Value.ToString.StartsWith("@") Then
                result = Session(Value.ToString.Replace("@", ""))
            ElseIf Value.ToString.StartsWith("#") And parentID > 0 AndAlso parentTable.ToString <> "" Then
                If Value.ToString.ToLower.Replace("#", "").StartsWith("date") Or Value.ToString.ToLower.Replace("#", "").EndsWith("date") Then
                    Dim v As Date = CDate(master(Value.ToString.Replace("#", "")))
                    result = ExecScalarNoParams("SELECT CONVERT(VARCHAR(10)," + Value.Replace("#", "") + ",101) as " + Value.Replace("#", "") + " FROM v" + parentTable.Substring(1, parentTable.Length - 1) + " WHERE ID = " + parentID.ToString).ToString
                Else
                    result = ExecScalarNoParams("SELECT " + Value.Replace("#", "") + " FROM v" + parentTable.Substring(1, parentTable.Length - 1) + " WHERE ID = " + parentID.ToString).ToString
                End If
            ElseIf Value.ToString.StartsWith(":") And Value.ToString.EndsWith(":") Then
                result = StringReplace(Value.Replace(":", ""), , Session)
                result = ExecScalarNoParams(result)
            ElseIf Value.ToString.StartsWith("#") And parentID = 0 Then
                If Value.ToString.ToLower.Replace("#", "").StartsWith("date") Or Value.ToString.ToLower.Replace("#", "").EndsWith("date") Then
                    Dim v As Date = CDate(master(Value.ToString.Replace("#", "")))
                    result = "'" & v.ToString("MM/dd/yyyy") & "'"
                Else
                    result = "'" & master(Value.ToString.Replace("#", "")) & "'"
                End If
            ElseIf Value.ToString.StartsWith("d+") Then
                result = ExecScalarNoParams("selelct convert(VARCHAR(10)," + Value.ToString.Replace("d+", "DATEADD") + Value.ToString.Replace("d+", "") + ",101)")
            ElseIf Value.ToString.StartsWith("t+") Then
                result = ExecScalarNoParams("selelct convert(VARCHAR(8)," + Value.ToString.Replace("t+", "DATEADD") + Value.ToString.Replace("t+", "") + ",108)")
            Else
                result = Value.ToString
            End If
            Return result
        End Function

        Public Sub Writer(path As String, body As String)
            GC.Collect()
            GC.WaitForPendingFinalizers()
            If File.Exists(path) Then
                File.Delete(path)
            End If
            Using scriptWriter As StreamWriter = File.CreateText(path)
                Try
                    scriptWriter.Write(body)
                    scriptWriter.Flush()
                Catch ex As Exception
                    logError(ex)
                Finally
                    scriptWriter.Close()
                End Try
            End Using
        End Sub

        Public Sub buildApp()
            Dim appPath As String = HttpContext.Current.Server.MapPath("~/Scripts/route.js"),
                sb As New Text.StringBuilder
            sb.Append("'use strict';")
            sb.Append("define(['app'],function(A){")
            'sb.Append("var A=angular.module('app',['mGrid','ngGrid','sly','ui.bootstrap.dropdown','ui.bootstrap.typeahead','ui.bootstrap.modal','ui.bootstrap.tpls','wDirectives','ct.ui.router.extras','growlNotifications','ngSanitize','mgcrea.ngStrap','angularFileUpload','angular.filter','dialogs.main','chart.js','chieffancypants.loadingBar','angular-redactor','luegg.directives','ng.calendar','treeGrid','ngCookies','colorpicker.module','cfp.hotkeys','mFileExplorer']);")
            sb.Append("A.forDev=" & IsNull(ExecScalarNoParams("SELECT dbo.fGetWebParameter('ForDevelopment')"), 1) & ";")
            sb.Append("A.ClientCustomFolder='" & IsNull(ExecScalarNoParams("SELECT dbo.fGetWebParameter('ClientCustomFolder')"), "") & "';")
            sb.Append("A.SystemVersion='" & IsNull(ExecScalarNoParams("SELECT dbo.fGetWebParameter('SystemVersion')"), 1) & "';")
            sb.Append("A.config(['$stateProvider','$stickyStateProvider','$urlRouterProvider','$controllerProvider','$compileProvider','$filterProvider','$provide','$httpProvider',function($stateProvider,$stickyStateProvider,$urlRouterProvider,$controllerProvider,$compileProvider,$filterProvider,$provide,$httpProvider) {")
            sb.Append("var c=(A.forDev==1?'':'Build/'+A.SystemVersion+'/')+'Partials/Controller/',")
            sb.Append("v=(A.forDev==1?'':'Build/'+A.SystemVersion+'/')+'Partials/View/';")
            sb.Append("function rD(q,R,dependencies){")
            sb.Append("var d=q.defer();")
            sb.Append("require(dependencies,function(){")
            sb.Append("d.resolve();")
            sb.Append("R.$apply();")
            sb.Append("});")
            sb.Append("return d.promise;")
            sb.Append("};")

            sb.Append("A.register=")
            sb.Append("{")
            sb.Append("controller:$controllerProvider.register,")
            sb.Append("directive:$compileProvider.directive,")
            sb.Append("filter:$filterProvider.register,")
            sb.Append("factory:$provide.factory,")
            sb.Append("service:$provide.service")
            sb.Append("};")

            sb.Append("var S=[];")
            For Each m As GSWEB.MenuCollection.Menu In mCollection.GetRootMenu
                If (m.ColumnValue("ID_WebMenuTypes") <> 1 AndAlso m.ColumnValue("IsCustomPage") = False) OrElse (m.ColumnValue("ID_WebMenuTypes") = 1 AndAlso m.ColumnValue("IsCustomPage") = False AndAlso mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 1).Count = 0) Then
                    Select Case m.ColumnValue("ID_WebMenuTypes")
                        Case 1, 3
                            sb.Append("S.push({ ")
                            sb.Append("name:'" & m.MenuID & "',")
                            sb.Append("stateName:'" & m.Name & "',")
                            sb.Append("url:'/" & m.Name.Replace(" ", "-") & If(m.ColumnValue("ID_WebMenuTypes") = 1, "/{Filter_" & m.MenuID & "}", "/{ID_" & m.MenuID & "}") & "',")
                            sb.Append("controller:'c" & m.MenuID & "',")
                            sb.Append("templateUrl:v+'c" & m.MenuID & ".html',")
                            sb.Append("mID:" & m.MenuID & ",")
                            sb.Append("resolve:{")
                            sb.Append("load:['$q','$rootScope',function (q,R) {")
                            sb.Append("var d=[c+'c" & m.MenuID & ".js'];")
                            sb.Append("return rD(q,R,d);")
                            sb.Append(" }],")
                            sb.Append("resources:['dataService',function(d){")
                            sb.Append("return d.getResources(" & m.MenuID & ",0,0" & If(m.ColumnValue("ID_WebMenuTypes") = 1, ",S.Filter_" & m.MenuID, "") & ");")
                            sb.Append("}]")
                            sb.Append("}")
                            sb.Append("});")
                        Case 12, 14
                        Case Else
                            sb.Append("")
                    End Select
                End If
                buildChildApp(m, sb)
            Next

            sb.Append("angular.forEach(S,function(state){$stateProvider.state(state);});")
            sb.Append("}]);")

            'sb.Append("return A;")
            sb.Append("});")

            Writer(appPath, sb.ToString)

        End Sub

        Public Sub buildChildApp(m As GSWEB.MenuCollection.Menu, sb As Text.StringBuilder)
            For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID)
                If (m2.ColumnValue("ID_WebMenuTypes") <> 1 AndAlso m2.ColumnValue("IsCustomPage") = False) OrElse (m2.ColumnValue("ID_WebMenuTypes") = 1 AndAlso m2.ColumnValue("IsCustomPage") = False AndAlso mCollection.GetChild(m2.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 1).Count = 0) Then
                    Select Case m2.ColumnValue("ID_WebMenuTypes")
                        Case 1, 3
                            sb.Append("S.push({ ")
                            sb.Append("name:'" & m2.MenuID & "',")
                            sb.Append("stateName:'" & m2.Name & "',")
                            sb.Append("url:'/" & m2.Name.Replace(" ", "-") & If(m2.ColumnValue("ID_WebMenuTypes") = 1, "/{Filter_" & m2.MenuID & "}", "/{ID_" & m2.MenuID & "}") & "',") ':[0-9]+
                            sb.Append("controller:'c" & m2.MenuID & "',")
                            sb.Append("templateUrl:v+'c" & m2.MenuID & ".html',")
                            sb.Append("mID:" & m2.MenuID & ",")
                            sb.Append("resolve:{")
                            sb.Append("load:['$q','$rootScope',function (q,R) {")
                            sb.Append("return rD(q,R,[c+'c" & m2.MenuID & ".js']);")
                            sb.Append(" }],")
                            sb.Append("resources:['dataService','$stateParams',function(d,S){")
                            If m2.ColumnValue("ID_WebMenuTypes") = 1 Then
                                sb.Append("return d.getResources(" & m2.MenuID & ",0,0,S.Filter_" & m2.MenuID & ");")
                            Else
                                'sb.Append("return d.getResources(" & m2.MenuID & ",(S.ID_" & m2.MenuID & "?parseInt(S.ID_" & m2.MenuID & "):0),0);")
                                sb.Append("return d.getResources(" & m2.MenuID & ",S.ID_" & m2.MenuID & ",0);")
                            End If

                            sb.Append("}]")
                            sb.Append("}")
                            sb.Append("});")
                        Case 12, 14
                            If IsNull(m2.ColumnValue("HasOpen"), False) = True Then
                                sb.Append("S.push({")
                                sb.Append("name:'" & buildAppStateName(m2) & "',")
                                sb.Append("stateName:'" & m2.Name & "',")
                                'sb.append("parent:'" & m2.ParentID & "',")
                                sb.Append("mID:" & m2.MenuID & ",")
                                sb.Append("url:'/" & m2.Name.Replace(" ", "-") & "/{ID_" & m2.MenuID & "}',") ':[0-9]+
                                sb.Append("onEnter:function($modal,$state,$stateParams){")
                                sb.Append("$modal.open({")
                                sb.Append("templateUrl:v+'c" & m2.MenuID & ".html',")
                                sb.Append("controller:'c" & m2.MenuID & "',")
                                sb.Append("backdrop:'static',")
                                sb.Append("keyboard:false,")
                                sb.Append("size:'xl',")
                                sb.Append("resolve:{")
                                sb.Append("load:['$q','$rootScope',function (q,R) {")
                                sb.Append("return rD(q,R,[c+'c" & m2.MenuID & ".js']);")
                                sb.Append("}],")
                                sb.Append("resources:['dataService',function(d){")
                                'sb.Append("return d.getResources(" & m2.MenuID & ",($stateParams.ID_" & m2.MenuID & "?parseInt($stateParams.ID_" & m2.MenuID & "):0),($stateParams.ID_" & m2.ParentID & "?parseInt($stateParams.ID_" & m2.ParentID & "):0));")
                                sb.Append("return d.getResources(" & m2.MenuID & ",$stateParams.ID_" & m2.MenuID & ",$stateParams.ID_" & m2.ParentID & ");")
                                sb.Append("}]")
                                sb.Append("}")
                                sb.Append("}).result.then(function(result) {")
                                sb.Append("},function(){")
                                'sb.Append("$state.go('^')")
                                sb.Append("});")
                                sb.Append("}")
                                sb.Append("});")
                            End If
                        Case Else
                            sb.Append("")

                    End Select

                End If
                buildChildApp(m2, sb)
            Next
        End Sub

        Public Function buildAppStateName(m As GSWEB.MenuCollection.Menu) As String
            Dim sb As New Text.StringBuilder
            Dim tmp As GSWEB.MenuCollection.Menu = m
            Dim str As New List(Of String)
            While (tmp.ColumnValue("ID_WebMenuTypes") <> 1)
                str.Add(tmp.MenuID)
                tmp = mCollection.GetMenu(tmp.ParentID)

            End While
            str.Reverse()
            Return String.Join(".", str)
        End Function

        Public Function buildMenuRibbon(m As GSWEB.MenuCollection.Menu) As String
            Dim str As New List(Of String)
            Dim tmp As GSWEB.MenuCollection.Menu = m
            While (tmp.ColumnValue("ID_WebMenuTypes") <> 1)
                If tmp.Label = "" Then
                    str.Add("<li>" & tmp.Name & "</li>")
                Else
                    str.Add("<li>" & tmp.Label & "</li>")
                End If
                tmp = mCollection.GetMenu(tmp.ParentID)
            End While
            str.Reverse()
            Return String.Join("", str)
        End Function

        Public Sub buildScripts()
            For Each m As GSWEB.MenuCollection.Menu In mCollection.GetRootMenu
                If (m.ColumnValue("ID_WebMenuTypes") <> 1 AndAlso m.ColumnValue("IsCustomPage") = 0) OrElse (m.ColumnValue("ID_WebMenuTypes") = 1 AndAlso mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 1).Count = 0) Then
                    Dim controllerPath As String = HttpContext.Current.Server.MapPath("~/Partials/Controller/c" & m.MenuID & ".js"),
                         templatePath As String = HttpContext.Current.Server.MapPath("~/Partials/View/c" & m.MenuID & ".html")
                    Writer(controllerPath, m.buildController) ''CONTROLLER
                    Writer(templatePath, m.buildTemplate) ''TEMPLATE
                End If
                buildChildScripts(m)
            Next
        End Sub

        Public Sub buildChildScripts(m As GSWEB.MenuCollection.Menu)
            For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID)
                If (m2.ColumnValue("ID_WebMenuTypes") <> 1 AndAlso m2.ColumnValue("IsCustomPage") = 0) OrElse (m2.ColumnValue("ID_WebMenuTypes") = 1 AndAlso mCollection.GetChild(m2.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 1).Count = 0) Then
                    Dim controllerPath As String = HttpContext.Current.Server.MapPath("~/Partials/Controller/c" & m2.MenuID & ".js"),
                         templatePath As String = HttpContext.Current.Server.MapPath("~/Partials/View/c" & m2.MenuID & ".html")
                    Writer(controllerPath, m2.buildController)
                    Writer(templatePath, m2.buildTemplate) ''TEMPLATE
                End If
                buildChildScripts(m2)
            Next
        End Sub

        Public Function buildHTMLControl(dr As DataRow) As String
            Dim sb As New Text.StringBuilder

            Try
                Select Case dr("ID_WebMenuControlTypes")
                    Case 1 'TEXTBOX
                        sb.Append("<input type=""text"" name=""" & dr("Name") & """ " &
                                  "placeholder=""" & IsNull(dr("Label"), dr("Name")) & """ " &
                                  If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", "required ", " ng-required='" & dr("RequiredIf") & "' "), "") &
                                  If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", "ng-disabled='!" & dr("EnabledIf") & "' "), "disabled ") &
                                  If(IsDBNull(dr("Width")), "", "style='width:" & dr("Width") & "px;' ") &
                                  buildHTMLControlValidation(mCollection.GetMenu(dr("ID_WebMenus")), dr("ID")) &
                                  " class=""form-control"" ng-model=""Master." & dr("Name") & """/>")
                    Case 2 'DROPDOWN 
                        sb.Append("<select " & If(IsDBNull(dr("Width")), "", "style='width:" & dr("Width") & "px;' ") & " name=""" & dr("Name") & """ ng-options=""item.ID as item.Name for item in dropdown_source[" & dr("ID") & "]")
                        If Not IsDBNull(dr("JsEvents")) Then
                            Dim jsevents As String = dr("JsEvents")
                            If jsevents.Split("=")(0).ToLower = "filterdatasource" Then
                                sb.Append(" | filter: {" & jsevents.Split("=")(1).ToString & "}")
                            ElseIf jsevents.Split("=")(0).ToLower = "orderby" Then
                                sb.Append(" | orderBy: '" & jsevents.Split("=")(1).ToString & "'")
                            End If
                        End If
                        sb.Append("""")
                        sb.Append(If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required='" & dr("RequiredIf") & "' "), "") &
                                  If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled='!" & dr("EnabledIf") & "' "), " disabled ") &
                                  "class=""form-control"" ng-model=""Master." & dr("Name") & """ ng-init=""Master." & dr("Name") & """ = dropdown_source[" & dr("ID") & "][0]")
                        If Not IsDBNull(dr("JsEvents")) Then
                            Dim jsevents As String = dr("JsEvents")
                            If jsevents.Split("=")(0).ToLower = "cascade" Then
                                sb.Append(" ng-change=""CascadingDropdown(" & dr("ID_WebMenus") & "," & dr("ID") & ",Master." & dr("Name") & ",'" & jsevents.Split("=")(1).Split(";")(0) & "')"" ")
                            End If
                        End If
                        sb.Append("><option value="""">- Select -</option></select>") '<i></i></label>
                        ' " & If(IsDBNull(dr("Width")), "", "style='width:" & dr("Width") & "px;' ") & "
                        'sb.Append("<div " & If(IsDBNull(dr("Width")), "style='width:100%' ", "style='width:" & dr("Width") & "px;' ") &
                        '          If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled='" & dr("EnabledIf") & "' "), " ng-disabled='true' ") &
                        '          "dropdown-append-to-body='true' dropdown-on-select=""setID(Master,'" & dr("Name") & "',$item)"" " &
                        '          "dropdown='item.ID as item.Name for item in dropdown_source[" & dr("ID") & "]' ng-model='Master." & dr("Name").ToString.Substring(3) & "'></div>" &
                        '          "<input type='hidden' name='" & dr("Name") & "' class='form-control' ng-model='Master." & dr("Name") & "' " &
                        '          If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required='" & dr("RequiredIf") & "' "), "") & "/>")
                        'sb.Append("<button type=""button"" class='btn btn-select btn-default' " & If(IsDBNull(dr("Width")), "style='width:100%' ", "style='width:" & dr("Width") & "px;' ") &
                        '          If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required='" & dr("RequiredIf") & "' "), "") &
                        '          If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled='" & dr("EnabledIf") & "' "), " ng-disabled='true' ") &
                        '          "data-container='body' data-animation='am-flip-x' " &
                        '          "ng-options='item.ID as item.Name for item in dropdown_source[" & dr("ID") & "]' ng-model='Master." & dr("Name").ToString & "' bs-select>" &
                        '          "Action <span class='caret'></span>" &
                        '          "</button>")


                    Case 3 'CHECKBOX
                        sb.Append("<div class='smart-form noselect material-switch'>" &
                                  "<input id=""" & dr("Name") & "_" & dr("ID") & """ type=""checkbox"" name=""" & dr("Name") & """ " &
                                  If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled='!" & dr("EnabledIf") & "' "), " disabled ") &
                                  "class=\'form_checkbox\' ng-model='Master." & dr("Name") & "'/>" &
                                  "<label for=""" & dr("Name") & "_" & dr("ID") & """ class='" & If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "label-primary", "label-default"), "label-default") & "'></label>" &
                                  "</div>")
                    Case 4 'DATE
                        sb.Append("<div class='input-group'>" &
                                  "<input type=""text"" name=""" & dr("Name") & """ " &
                                  "placeholder=""" & IsNull(dr("Label"), dr("Name")) & """ " &
                                  If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required='" & dr("RequiredIf") & "' "), "") &
                                  If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled='!" & dr("EnabledIf") & "' "), " disabled ") &
                                  If(IsDBNull(dr("Width")), "", "style='width:" & dr("Width") & "px;' ") &
                                  "bs-datepicker data-container=""body"" date-to-iso " &
                                  buildHTMLControlValidation(mCollection.GetMenu(dr("ID_WebMenus")), dr("ID")) &
                                  " date-format=""MM/dd/yyyy"" " &
                                  "class=""form-control"" ng-model=""Master." & dr("Name") & """/>" &
                                  "<span class='input-group-addon'><i class='fa fa-calendar'></i></span></div>")
                    Case 5 'TIME
                        sb.Append("<div class='input-group'>" &
                                  "<input type=""text"" name=""" & dr("Name") & """ " &
                                  "placeholder=""" & IsNull(dr("Label"), dr("Name")) & """ " &
                                  If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required='" & dr("RequiredIf") & "' "), "") &
                                  If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled='!" & dr("EnabledIf") & "' "), " disabled ") &
                                  If(IsDBNull(dr("Width")), "", "style='width:" & dr("Width") & "px;' ") &
                                  "bs-timepicker time-to-iso data-container=""body"" " &
                                  buildHTMLControlValidation(mCollection.GetMenu(dr("ID_WebMenus")), dr("ID")) &
                                  " class=""form-control"" ng-model=""Master." & dr("Name") & """/>" &
                                  "<span class='input-group-addon'><i class='fa fa-clock-o'></i></span></div>")
                    Case 7 'IMAGE
                        sb.Append("<img ng-src='Contents/Photos/{{Master." & dr("Name") & "}}' alt='{{Master." & dr("Name") & "}}' height='" & If(IsDBNull(dr("Height")), "70", dr("Height")) & "' width='" & If(IsDBNull(dr("Width")), "70", dr("Width")) & "'/>")
                    Case 8 'TEXTAREA
                        sb.Append("<textarea name=""" & dr("Name") & """ " &
                                  "placeholder=""" & IsNull(dr("Label"), dr("Name")) & """ " &
                                  If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required='" & dr("RequiredIf") & "' "), "") &
                                  If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled='!" & dr("EnabledIf") & "' "), " disabled ") &
                                  If(IsDBNull(dr("Width")), "", "style='width:" & dr("Width") & "px;' ") &
                                  If(IsDBNull(dr("Height")), "style='height:70px;' ", "style='height:" & dr("Height") & "px;' ") &
                                  buildHTMLControlValidation(mCollection.GetMenu(dr("ID_WebMenus")), dr("ID")) &
                                  "class=""form-control"" ng-model=""Master." & dr("Name") & """></textarea>")
                    Case 10 'TEXT AUTOCOMPLETE
                        sb.Append("<input type=""text"" name=""" & dr("Name").ToString & """ " &
                                  "placeholder=""" & IsNull(dr("Label"), dr("Name")) & """ " &
                                  If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", "ng-disabled='!" & dr("EnabledIf") & "' "), "disabled ") &
                                  If(IsDBNull(dr("Width")), "", "style='width:" & dr("Width") & "px;' ") &
                                  "typeahead-append-to-body='true' " &
                                  "typeahead='item as item for item in text_autocomplete_source[" & dr("ID") & "] | filter:$viewValue | limitTo:10 ' " &
                                  "class=""form-control"" ng-model=""Master." & dr("Name").ToString & """/>")
                    Case 14  'LABEL
                        If dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("date") AndAlso dr("Name").ToString.ToLower.Contains("time") Then
                            sb.Append("<label class=""control-label"">{{Master." & dr("Name") & " | date : 'MM/dd/yyyy hh:mm:ss a'}}</label>")
                        ElseIf dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("date") Then
                            sb.Append("<label class=""control-label"">{{Master." & dr("Name") & " | date : 'MM/dd/yyyy'}}</label>")

                        ElseIf dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("time") Then
                            sb.Append("<label class=""control-label"">{{Master." & dr("Name") & " | date : 'hh:mm:ss a'}}</label>")
                        ElseIf dr("colDataType") = "varchar" OrElse dr("colDataType") = "nvarchar" Then
                            sb.Append("<span class='control-label' ng-bind-html='Master." & dr("Name") & " | trustedHTML'></span>")
                        Else
                            sb.Append("<span class=""control-label"">{{Master." & dr("Name") & "}}</span>")
                        End If
                        'Case 15
                        '    sb.Append("<rating ng-model='Master." & dr("Name") & "' max='10' state-on=""'fa fa-star'"" state-off=""'fa fa-star-o'""></rating>")
                    Case 17 ' LOOKUP
                        sb.Append("<div ng-class=""{'input-group':" & If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "true", dr("EnabledIf")), "false") & "}"">" &
                                  "<input target-name='Master." & dr("Name").ToString.Substring(3) & "' target-display=""'" & dr("DisplayMember") & "'"" target-value='Master." & dr("Name") & "' target-source='lookup_source[" & dr("ID") & "]' target-addon=""" & dr("Name") & "_r" & dr("ID") & "_c" & dr("ID") & """ type=""text"" cid=" & dr("ID") & " " &
                                  "placeholder=""" & IsNull(dr("Label"), dr("Name")) & """ " &
                                  If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled='!" & dr("EnabledIf") & "' "), " disabled ") &
                                  If(IsDBNull(dr("Width")), "", "style='width:" & dr("Width") & "px;' ") &
                                  " ng-form-lookup " &
                                  buildHTMLControlValidation(mCollection.GetMenu(dr("ID_WebMenus")), dr("ID")) &
                                  " class=""form-control"" ng-model=""Master." & dr("Name").ToString.Substring(3) & """/>" &
                                  "<input type=""hidden"" " &
                                  If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required='" & dr("RequiredIf") & "' "), "") &
                                  " name=""" & dr("Name") & """ ng-model=""Master." & dr("Name") & """ />" &
                                  "<span ID=""" & dr("Name") & "_r" & dr("ID") & "_c" & dr("ID") & """" &
                                  If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-show='" & dr("EnabledIf") & "' "), " ng-show='false'") &
                                  "class='input-group-addon'><i class='fa fa-list'></i></span></div>")
                    Case 19 ' CURRENCY BOX
                        sb.Append("<input type=""text"" name=""" & dr("Name") & """ " &
                                  "placeholder=""" & IsNull(dr("Label"), dr("Name")) & """ " &
                                  If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", "required ", " ng-required='" & dr("RequiredIf") & "' "), "") &
                                  If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", "ng-disabled='!" & dr("EnabledIf") & "' "), "disabled ") &
                                  If(IsDBNull(dr("Width")), "", "style='width:" & dr("Width") & "px;' ") &
                                  "ng-currency-only fixed-decimal='2' " &
                                  "class=""form-control"" ng-model=""Master." & dr("Name") & """/>")
                    Case 20 'HTML EDITOR
                        sb.Append("<textarea redactor name=""" & dr("Name") & """ " &
                                  If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required='" & dr("RequiredIf") & "' "), "") &
                                  If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled='!" & dr("EnabledIf") & "' "), " disabled ") &
                                  If(IsDBNull(dr("Width")), "", "style='width:" & dr("Width") & "px;' ") &
                                  If(IsDBNull(dr("Height")), "style='height:70px;' ", "style='height:" & dr("Height") & "px;' ") &
                                  buildHTMLControlValidation(mCollection.GetMenu(dr("ID_WebMenus")), dr("ID")) &
                                  " class=""form-control"" ng-model=""Master." & dr("Name") & """></textarea>")
                    Case 21 ' HIDDEN FIELD
                        sb.Append("<input type=""hidden"" name=""" & dr("Name") & """ " &
                                  "class=""form-control"" ng-model=""Master." & dr("Name") & """/>")
                    Case 23 ' AUTOCOMPLETE
                        'getAutoCompleteItems(" & dr("ID_WebMenus") & "," & dr("ID") & ",$viewValue) | filter:$viewValue | limitTo:10'
                        sb.Append("<input type=""text"" name=""" & dr("Name").ToString.Substring(3) & """ " &
                                  "placeholder=""" & IsNull(dr("Label"), dr("Name")) & """ " &
                                  If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", "ng-disabled='!" & dr("EnabledIf") & "' "), "disabled ") &
                                  If(IsDBNull(dr("Width")), "", "style='width:" & dr("Width") & "px;' ") &
                                  "ng-change=""clearAutoComplete(Master,'" & dr("Name") & "')"" " &
                                  "typeahead-append-to-body='true' typeahead-on-select=""setID(Master,'" & dr("Name") & "',$item)"" " &
                                  "typeahead='item as item.Name for item in autocomplete_source[" & dr("ID") & "] | filter:$viewValue | limitTo:10 ' " &
                                  "class=""form-control"" ng-model=""Master." & dr("Name").ToString.Substring(3) & """/>" &
                                  "<input type=""hidden"" name=""" & dr("Name") & """ " &
                                  If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", "required ", " ng-required='" & dr("RequiredIf") & "' "), "") &
                                  If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", "ng-disabled='!" & dr("EnabledIf") & "' "), "disabled ") &
                                  "class=""form-control"" ng-model=""Master." & dr("Name") & """/>")
                    Case 25 ' FILE UPLOAD
                        sb.Append("<div class='smart-form input-group' style='width:100%'>" &
                                  "<label for='file' class='input input-file'>" &
                                  "<div class='button'>" &
                                  "<input type='file' name='" & dr("Name") & "' file-input ng-file-select='onFileSelect($files," & dr("ID_WebMenus") & ",""" & dr("Name") & """, Master.ID) ' onclick='this.value = null' " &
                                  If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", "required ", " ng-required='" & dr("RequiredIf") & "' "), "") &
                                  If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", "ng-disabled='!" & dr("EnabledIf") & "' "), "disabled ") &
                                  "ng-model='Master." & dr("Name") & "'>Browse" &
                                  "</div>" &
                                  "<input type='text' ng-model='Master." & dr("Name") & "' placeholder='Select files...' readonly=''>" &
                                  "</label>" &
                                  "<span class='input-group-addon' ng-if='Master.ID > 0 && Master." & dr("Name") & " !== null' download-file='{{Master." & dr("Name") & "_GUID}}' filename='{{Master." & dr("Name") & "}}'><i class='fa fa-download'></i></span>" &
                                  "</div>")
                    Case 26 ' RADIO BUTTON
                        sb.Append("<label style='padding:10px' ng-repeat='rdb in rdb_source[" & dr("ID") & "]'>" &
                                  "<div class='col-md-6'>" &
                                  "<input style='height:20px;' type=""radio"" name=""" & dr("Name") & """ " &
                                  If(CBool(dr("HasValidation")), "ng-class=""{ 'has-error' : mainform." & dr("Name") & ".$invalid && mainform.$dirty }"" ", "") &
                                  If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required='" & dr("RequiredIf") & "' "), "") &
                                  If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled='!" & dr("EnabledIf") & "' "), " disabled ") &
                                  "class=""form-control"" ng-model=""Master." & dr("Name") & """ ng-value='rdb.ID'/>" &
                                  "</div>" &
                                  "<div style='top:5px;' class='col-md-5'>" &
                                  "{{rdb.Name}}</div></label>")
                    Case 28 ' NUMERIC
                        sb.Append("<input type=""text"" name=""" & dr("Name") & """ " &
                                  "placeholder=""" & IsNull(dr("Label"), dr("Name")) & """ " &
                                  If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", "required ", " ng-required='" & dr("RequiredIf") & "' "), "") &
                                  If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", "ng-disabled='!" & dr("EnabledIf") & "' "), "disabled ") &
                                  If(IsDBNull(dr("Width")), "", "style='width:" & dr("Width") & "px;' ") &
                                  "ng-numeric-only  " &
                                  "class=""form-control"" ng-model=""Master." & dr("Name") & """/>")
                    Case 30 ' UploadImage
                        sb.Append("<img id='currentImage' ng-src='{{Master." & dr("Name") & "_GUID ? ""Upload/"" + Master." & dr("Name") & "_GUID : ""Contents/Photos/avatar.png""}}' width='100px' height='100px' onclick='toggleUpload()' />")
                        sb.Append("<script type='text/javascript'>")
                        sb.Append("function toggleUpload(){" &
                                  "$('#myUpload').click();" &
                                  "}")
                        sb.Append("function changeImage(e){" &
                                  "if (e.target.files && e.target.files[0]) {" &
                                  "var reader = new FileReader();" &
                                  "reader.onload = function (e) {" &
                                  "$('#currentImage').attr('src', e.target.result);")
                        sb.AppendLine("}")
                        sb.Append("reader.readAsDataURL(e.target.files[0]);" &
                                  "}" &
                                  "}"
                                  )
                        sb.Append("</script>")
                        sb.Append("<div style='display:none;' class='smart-form input-group' style='width:100%'>" &
                                  "<label for='file' class='input input-file'>" &
                                  "<div class='button'>" &
                                  "<input id='myUpload' onchange='changeImage(event)' type='file' name='" & dr("Name") & "' file-input ng-file-select='onFileSelect($files," & dr("ID_WebMenus") & ",""" & dr("Name") & """, Master.ID) ' onclick='this.value = null' " &
                                  If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", "required ", " ng-required='" & dr("RequiredIf") & "' "), "") &
                                  If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", "ng-disabled='!" & dr("EnabledIf") & "' "), "disabled ") &
                                  "ng-model='Master." & dr("Name") & "'>Browse" &
                                  "</div>" &
                                  "<input type='text' ng-model='Master." & dr("Name") & "' placeholder='Select files...' readonly=''>" &
                                  "</label>" &
                                  "<span class='input-group-addon' ng-if='Master.ID > 0 && Master." & dr("Name") & " !== null' download-file='{{Master." & dr("Name") & "_GUID}}' filename='{{Master." & dr("Name") & "}}'><i class='fa fa-download'></i></span>" &
                                  "</div>")
                    Case 35 'DETAILED LOOKUP
                        sb.Append("<div class='input-group'>" &
                                  "<input target-value='Master." & dr("Name") & "' target-detail-source='detailedValuelookup_source[" & dr("ID") & "]' target-source='detailedlookup_source[" & dr("ID") & "]' type=""text"" cid=" & dr("ID") & " " &
                                  "placeholder=""" & IsNull(dr("Label"), dr("Name")) & """ " &
                                  If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled='!" & dr("EnabledIf") & "' "), " disabled ") &
                                  If(IsDBNull(dr("Width")), "", "style='width:" & dr("Width") & "px;' ") &
                                  " ng-form-detailed-lookup onkeyup=""return false"" onkeydown=""return false"" " &
                                  buildHTMLControlValidation(mCollection.GetMenu(dr("ID_WebMenus")), dr("ID")) &
                                  " class=""form-control"" /><br>" &
                                  "<input type=""hidden"" " &
                                  If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required='" & dr("RequiredIf") & "' "), "") &
                                  " name=""" & dr("Name") & """ ng-model=""Master." & dr("Name") & """ />" &
                                  "<span class='input-group-addon'><i class='fa fa-list'></i></span>" &
                                  "</div>" &
                                  "<div cid='" & dr("ID") & "' ng-detailed-lookup-grid></div>")
                    Case 36 'DATE TIME PICKER
                        sb.Append("<div class='input-group'>" &
                                  "<input onkeydown=""if(event.keyCode != 8) return false"" make-datetime-picker id=""" & dr("Name") & "_" & dr("ID") & """ type=""text"" name=""" & dr("Name") & """ " &
                                  "placeholder=""" & IsNull(dr("Label"), dr("Name")) & """ " &
                                  If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required='" & dr("RequiredIf") & "' "), "") &
                                  If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled='!" & dr("EnabledIf") & "' "), " disabled ") &
                                  If(IsDBNull(dr("Width")), "", "style='width:" & dr("Width") & "px;' ") &
                                  buildHTMLControlValidation(mCollection.GetMenu(dr("ID_WebMenus")), dr("ID")) &
                                  "class=""form-control"" ng-model=""Master." & dr("Name") & """/>" &
                                  "<span class='input-group-addon'><i class='fa fa-calendar'></i></span></div>")
                End Select
            Catch ex As Exception

            End Try
            Return sb.ToString
        End Function

        Public Function buildHTMLFilterControl(dr As DataRow, Optional isReportFilter As Boolean = True) As String
            Dim sb As New Text.StringBuilder

            Try
                Select Case dr("ID_FilterWebMenuControlTypes")
                    Case 1 'TEXTBOX
                        sb.Append("<input type=""text"" name=""" & dr("Name") & """ " &
                                  "placeholder='" & IsNull(dr("Label"), dr("Name")) & "'" &
                                  If(isReportFilter AndAlso CBool(IsNull(dr("IsRequired"), False)), "required ", "") &
                                  "class=""form-control"" ng-model=""filter[" & dr("ID_WebMenus") & "]." & dr("Name") & """ ng-enter=""Search(" & dr("ID_WebMenus") & ")""/>")
                    Case 2 'DROPDOWN 
                        sb.Append("<select name=""" & dr("Name") & """ ng-options=""item.ID as item.Name for item in filter_dropdown_source[" & dr("ID") & "]"" " &
                                  If(isReportFilter AndAlso CBool(IsNull(dr("IsRequired"), False)), "required ", "") &
                                  "class=""form-control"" ng-model=""filter[" & dr("ID_WebMenus") & "]." & dr("Name") & """><option value="""">- Select -</option></select>")
                    Case 3 'CHECKBOX
                        sb.Append("<input type=""checkbox"" name=""" & dr("Name") & """ " &
                                  "class=""form_checkbox"" ng-model=""filter[" & dr("ID_WebMenus") & "]." & dr("Name") & """/>")
                    Case 4 'DATE
                        If (dr("Name").ToString.ToLower.Contains("start") Or dr("Name").ToString.ToLower.Contains("end")) Or ((dr("Name").ToString.ToLower.StartsWith("sd") And dr("Name").ToString.ToLower.EndsWith("sd")) Or (dr("Name").ToString.ToLower.StartsWith("ed") And dr("Name").ToString.ToLower.EndsWith("ed"))) Then
                            sb.Append("<div class='input-group pad-bot-5' >" &
                                  "<input type=""text"" name='" & dr("Name") & "' placeholder='" & IsNull(dr("Label"), dr("Name")) & "' " &
                                  If(isReportFilter AndAlso CBool(IsNull(dr("IsRequired"), False)), "required ", "") &
                                  "bs-datepicker data-container=""body"" date-to-iso " &
                                  "date-format=""MM/dd/yyyy"" " &
                                  "class=""form-control"" ng-model=""filter[" & dr("ID_WebMenus") & "]." & dr("Name") & """/>" &
                                  "<span class='input-group-addon'><i class='fa fa-calendar'></i></span></div>")
                        Else
                            sb.Append("<div class='input-group pad-bot-5' >" &
                                  "<input type=""text"" name=""From_" & dr("Name") & """ placeholder=""From"" " &
                                  If(isReportFilter AndAlso CBool(IsNull(dr("IsRequired"), False)), "required ", "") &
                                  "bs-datepicker data-container=""body"" date-to-iso " &
                                  "date-format=""MM/dd/yyyy"" " &
                                  "class=""form-control"" ng-model=""filter[" & dr("ID_WebMenus") & "].From_" & dr("Name") & """/>" &
                                  "<span class='input-group-addon'><i class='fa fa-calendar'></i></span></div>" &
                                  "<div class='input-group' >" &
                                  "<input type=""text"" name=""To_" & dr("Name") & """ placeholder=""To"" " &
                                  "bs-datepicker data-container=""body"" date-to-iso " &
                                  "date-format=""MM/dd/yyyy"" " &
                                  "class=""form-control"" ng-model=""filter[" & dr("ID_WebMenus") & "].To_" & dr("Name") & """/>" &
                                  "<span class='input-group-addon'><i class='fa fa-calendar'></i></span></div>")
                        End If
                    Case 5 'TIME
                        sb.Append("<div class='input-group'>" &
                                  "<input type=""text"" name=""" & dr("Name") & """ " &
                                  If(isReportFilter AndAlso CBool(IsNull(dr("IsRequired"), False)), "required ", "") &
                                  "bs-timepicker data-container=""body"" " &
                                  "class=""form-control"" ng-model=""filter[" & dr("ID_WebMenus") & "]." & dr("Name") & """/>" &
                                  "<span class='input-group-addon'><i class='fa fa-clock-o'></i></span></div>")
                    Case 8 'TEXTAREA
                        sb.Append("<textarea name=""" & dr("Name") & """ " &
                                  If(isReportFilter AndAlso CBool(IsNull(dr("IsRequired"), False)), "required ", "") &
                                  If(IsDBNull(dr("Height")), "style='height:70px;' ", "style='height:" & dr("Height") & "px;' ") &
                                  "class=""form-control"" ng-model=""filter[" & dr("ID_WebMenus") & "]." & dr("Name") & """></textarea>")
                    Case 10 'TEXT AUTOCOMPLETE
                        sb.Append("<input type=""text"" name=""" & dr("Name").ToString & """ " &
                                  If(isReportFilter AndAlso CBool(IsNull(dr("IsRequired"), False)), "required ", "") &
                                  "placeholder=""" & IsNull(dr("Label"), dr("Name")) & """ " &
                                  "typeahead-append-to-body='true' " &
                                  "typeahead='item as item for item in filter_text_autocomplete_source[" & dr("ID") & "] | filter:$viewValue | limitTo:10 ' " &
                                  "class=""form-control"" ng-model=""filter[" & dr("ID_WebMenus") & "]." & dr("Name").ToString & """  ng-enter=""Search(" & dr("ID_WebMenus") & ")""/>")
                    Case 14  'LABEL
                        If dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("date") AndAlso dr("Name").ToString.ToLower.Contains("time") Then
                            sb.Append("<label class=""control-label"">{{Master." & dr("Name") & " | date : 'MM/dd/yyyy hh:mm:ss a'}}</label>")
                        ElseIf dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("date") Then
                            sb.Append("<label class=""control-label"">{{Master." & dr("Name") & " | date : 'MM/dd/yyyy'}}</label>")

                        ElseIf dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("time") Then
                            sb.Append("<label class=""control-label"">{{Master." & dr("Name") & " | date : 'hh:mm:ss a'}}</label>")
                        ElseIf dr("colDataType") = "varchar" OrElse dr("colDataType") = "nvarchar" Then
                            sb.Append("<span class='control-label' ng-bind-html='Master." & dr("Name") & " | trustedHTML'></span>")
                        Else
                            sb.Append("<span class=""control-label"">{{Master." & dr("Name") & "}}</span>")
                        End If
                    Case 17 ' LOOKUP
                        sb.Append("<div class='input-group'>" &
                                  "<input target-name=""filter[" & dr("ID_WebMenus") & "]." & dr("Name").ToString.Substring(3) & """ target-display=""'" & dr("DisplayMember") & "'"" target-value=""filter[" & dr("ID_WebMenus") & "]." & dr("Name") & """ target-source='filter_lookup_source[" & dr("ID") & "]' target-addon=""Filter_" & dr("Name") & "_r" & dr("ID") & "_c" & dr("ID") & """ type=""text"" cid=" & dr("ID") & " " &
                                  If(isReportFilter AndAlso CBool(IsNull(dr("IsRequired"), False)), "required ", "") &
                                  "placeholder=""" & IsNull(dr("Label"), dr("Name")) & """ " &
                                  If(IsDBNull(dr("Width")), "", "style='width:" & dr("Width") & "px;' ") &
                                  "ng-form-lookup " &
                                  "class=""form-control"" ng-model=""filter[" & dr("ID_WebMenus") & "]." & dr("Name").ToString.Substring(3) & """/><br>" &
                                  "<input type=""hidden"" " &
                                  "name=""" & dr("Name") & """ ng-model=""filter[" & dr("ID_WebMenus") & "]." & dr("Name") & """ />" &
                                  "<span ID=""Filter_" & dr("Name") & "_r" & dr("ID") & "_c" & dr("ID") & """ class='input-group-addon'><i class='fa fa-list'></i></span></div>")
                    Case 19 ' CURRENCY BOX
                        sb.Append("<input type=""text"" name=""" & dr("Name") & """ " &
                                  If(isReportFilter AndAlso CBool(IsNull(dr("IsRequired"), False)), "required ", "") &
                                  "placeholder='" & IsNull(dr("Label"), dr("Name")) & "'" &
                                  "ng-currency-only fixed-decimal='2' " &
                                  "class=""form-control"" ng-model=""filter[" & dr("ID_WebMenus") & "]." & dr("Name") & """  ng-enter=""Search(" & dr("ID_WebMenus") & ")""/>")
                    Case 21 ' HIDDEN FIELD
                        sb.Append("<input type=""hidden"" name=""" & dr("Name") & """ " &
                                  "class=""form-control"" ng-model=""filter[" & dr("ID_WebMenus") & "]." & dr("Name") & """/>")
                    Case 23 ' AUTOCOMPLETE 
                        ' getAutoCompleteItems(" & dr("ID_WebMenus") & "," & dr("ID") & ",$viewValue) | filter:$viewValue | limitTo:10'
                        sb.Append("<input type=""text"" name=""" & dr("Name").ToString.Substring(3) & """ " &
                                  If(isReportFilter AndAlso CBool(IsNull(dr("IsRequired"), False)), "required ", "") &
                                  "typeahead-append-to-body='true' typeahead-on-select=""setID(filter[" & dr("ID_WebMenus") & "],'" & dr("Name") & "',$item)"" " &
                                  "typeahead='item as item.Name for item in filter_autocomplete_source[" & dr("ID") & "] | filter:$viewValue | limitTo:10' " &
                                  "class=""form-control"" ng-model=""f[" & dr("ID_WebMenus") & "]." & dr("Name").ToString.Substring(3) & """  ng-enter=""Search(" & dr("ID_WebMenus") & ")""/>")
                        ' &
                        '"<input type=""hidden"" name=""" & dr("Name") & """ " &
                        ' "class=""form-control"" ng-model=""filter[" & dr("ID_WebMenus") & "]." & dr("Name") & """/>"
                    Case 26 ' RADIO BUTTON
                        sb.Append("<label ng-repeat='rdb in filter_rdb_source[" & dr("ID") & "]'>" &
                                  "<input type=""radio"" name=""" & dr("Name") & """ " &
                                  "class=""form-control"" ng-model=""filter[" & dr("ID_WebMenus") & "]." & dr("Name") & """ ng-value='rdb.ID'/>" &
                                  "{{rdb.Name}}</label>")
                    Case 28 ' NUMERIC
                        sb.Append("<input type=""text"" name=""" & dr("Name") & """ " &
                                  If(isReportFilter AndAlso CBool(IsNull(dr("IsRequired"), False)), "required ", "") &
                                  "placeholder='" & IsNull(dr("Label"), dr("Name")) & "'" &
                                  "ng-numeric-only  " &
                                  "class=""form-control"" ng-model=""filter[" & dr("ID_WebMenus") & "]." & dr("Name") & """ ng-enter=""Search(" & dr("ID_WebMenus") & ")""/>")
                End Select
            Catch ex As Exception

            End Try
            Return sb.ToString
        End Function

        Public Function Contains(arr As String(), cond As String) As Boolean
            For i As Integer = 0 To arr.Length - 1
                If arr(i) = cond Then
                    Return True
                End If
            Next
            Return False
        End Function

        Public Function addStripSlashes(s As String)
            Return s.Replace("'", "\'")
        End Function

        Public Function serialize(reader As SqlDataReader, Optional EncryptID As Boolean = False, Optional ByRef Session As System.Web.SessionState.HttpSessionState = Nothing, Optional AdditionalFields As List(Of String) = Nothing) As String
            '   Dim sb As New Text.StringBuilder
            Dim sw As New StringWriter()
            Try

                Using jsonWriter As JsonWriter = New JsonTextWriter(sw)
                    jsonWriter.WriteStartArray()
                    While reader.Read
                        jsonWriter.WriteStartObject()
                        For i As Integer = 0 To reader.FieldCount - 1
                            jsonWriter.WritePropertyName(reader.GetName(i))
                            jsonWriter.WriteValue(reader(i))
                        Next
                        If EncryptID Then
                            jsonWriter.WritePropertyName("$$rID")
                            'jsonWriter.WriteValue(GetIndirectReference(reader.Item("ID"), Session))
                            jsonWriter.WriteValue(toAnyBase(reader.Item("ID"), 62))
                        End If
                        If Not AdditionalFields Is Nothing Then
                            For Each field As String In AdditionalFields
                                jsonWriter.WritePropertyName("$$" & field)
                                'jsonWriter.WriteValue(GetIndirectReference(reader.Item(field), Session))
                                jsonWriter.WriteValue(toAnyBase(reader.Item(field), 62))
                            Next
                        End If
                        jsonWriter.WriteEndObject()
                    End While
                    jsonWriter.WriteEndArray()
                    jsonWriter.Close()
                End Using
            Catch ex As Exception
                logError(ex)
            Finally
                reader.Close()
            End Try
            Return sw.ToString
        End Function

        Public Function serializeNoClose(ByRef reader As SqlDataReader, Optional EncryptID As Boolean = False, Optional Session As System.Web.SessionState.HttpSessionState = Nothing, Optional AdditionalFields As List(Of String) = Nothing) As String
            Dim sb As New Text.StringBuilder
            Dim sw As New StringWriter(sb)
            Using jsonWriter As JsonWriter = New JsonTextWriter(sw)
                jsonWriter.WriteStartArray()
                While reader.Read
                    jsonWriter.WriteStartObject()
                    For i As Integer = 0 To reader.FieldCount - 1
                        jsonWriter.WritePropertyName(reader.GetName(i))
                        jsonWriter.WriteValue(reader(i))
                    Next
                    If EncryptID Then
                        jsonWriter.WritePropertyName("$$rID")
                        'jsonWriter.WriteValue(GetIndirectReference(reader.Item("ID"), Session))
                        jsonWriter.WriteValue(toAnyBase(reader.Item("ID"), 62))
                    End If
                    If Not AdditionalFields Is Nothing Then
                        For Each field As String In AdditionalFields
                            jsonWriter.WritePropertyName("$$" & field)
                            'jsonWriter.WriteValue(GetIndirectReference(reader.Item(field), Session))
                            jsonWriter.WriteValue(toAnyBase(reader.Item(field), 62))
                        Next
                    End If
                    jsonWriter.WriteEndObject()
                End While
                jsonWriter.WriteEndArray()
            End Using
            Return sb.ToString
        End Function

        Public Function JavascriptArray(reader As SqlDataReader) As String
            Dim sb As New Text.StringBuilder
            Dim sw As New StringWriter(sb)
            Using jsonWriter As JsonWriter = New JsonTextWriter(sw)
                jsonWriter.WriteStartArray()
                While reader.Read
                    For i As Integer = 0 To reader.FieldCount - 1
                        jsonWriter.WriteValue(reader(i))
                    Next
                End While
                jsonWriter.WriteEndArray()
            End Using
            reader.Close()
            Return sb.ToString
        End Function

        Public Function JavascriptArrayNoClose(ByRef reader As SqlDataReader) As String
            Dim sb As New Text.StringBuilder
            Dim sw As New StringWriter(sb)
            Using jsonWriter As JsonWriter = New JsonTextWriter(sw)
                jsonWriter.WriteStartArray()
                While reader.Read
                    For i As Integer = 0 To reader.FieldCount - 1
                        jsonWriter.WriteValue(reader(i))
                    Next
                End While
                jsonWriter.WriteEndArray()
            End Using
            Return sb.ToString
        End Function

        Public Function SetDataSourceFilter(ByVal m As GSWEB.MenuCollection.Menu, Optional ByVal buttonID As Integer = 0, Optional ByVal ParentID As Integer = 0, Optional ByVal SessionVariables As System.Web.SessionState.HttpSessionState = Nothing, Optional Level As Integer = 0, Optional ByVal menuFilter As String = "") As String

            If Trim(m.ColumnValue("DataSource").ToString) = "" AndAlso buttonID = 0 Then Return ""
            Dim dsFilter As String = If(menuFilter <> "", "(SELECT * FROM" & m.ColumnValue("DataSource").ToString & " WHERE " & menuFilter & ")a", m.ColumnValue("DataSource").ToString)

            ' Dim str As String = " WHERE " + viewFilter

            Dim parent As GSWEB.MenuCollection.Menu = mCollection.GetMenu(m.ParentID)
            While dsFilter.Contains("#")
                Dim tmpdsFilter As String = dsFilter.Substring(dsFilter.IndexOf("#"), (dsFilter.Length) - dsFilter.IndexOf("#"))
                If tmpdsFilter.Contains(" ") Then tmpdsFilter = tmpdsFilter.Substring(0, tmpdsFilter.IndexOf(" "))
                If tmpdsFilter.Contains(",") Then tmpdsFilter = tmpdsFilter.Substring(0, tmpdsFilter.IndexOf(","))
                If tmpdsFilter.Contains(")") Then tmpdsFilter = tmpdsFilter.Substring(0, tmpdsFilter.IndexOf(")"))
                If ParentID > 0 Then
                    'If CBool(IsNull(m.ColumnValue("isFilterFromLinkButtonParameter"), 0)) = True Then
                    '    dsFilter = dsFilter.Replace(tmpdsFilter, ParentID)

                    'Else
                    If Level = 1 Then
                        Return "v" & m.ColumnValue("TableName").ToString.Substring(1)
                    End If

                    If Not parent Is Nothing Then
                        'dsFilter = dsFilter.Replace(tmpdsFilter, ExecScalarNoParams("SELECT " & tmpdsFilter.Replace("#", "") & " FROM " & SetDataSourceFilter(parent, 0, ParentID, SessionVariables, 1) & " WHERE ID = " & ParentID).ToString)
                        dsFilter = Regex.Replace(dsFilter, "#\b" + tmpdsFilter.Replace("#", "").ToString + "\b", "'" + ExecScalarNoParams("SELECT " & tmpdsFilter.Replace("#", "") & " FROM " & SetDataSourceFilter(parent, 0, ParentID, SessionVariables, 1) & " WHERE ID = " & ParentID).ToString + "'")
                    Else
                        'dsFilter = dsFilter.Replace(tmpdsFilter, ExecScalarNoParams("SELECT " & tmpdsFilter.Replace("#", "") & " FROM " & SetDataSourceFilter(m, 0, ParentID, SessionVariables) & " WHERE ID = " & ParentID).ToString)
                        dsFilter = Regex.Replace(dsFilter, "#\b" + tmpdsFilter.Replace("#", "").ToString + "\b", "'" + ExecScalarNoParams("SELECT " & tmpdsFilter.Replace("#", "") & " FROM " & SetDataSourceFilter(m, 0, ParentID, SessionVariables) & " WHERE ID = " & ParentID).ToString + "'")
                    End If
                Else
                    dsFilter = dsFilter.Replace(tmpdsFilter, "''")
                End If
            End While

            While dsFilter.Contains("@")
                Dim tmpdsFilter As String = dsFilter.Substring(dsFilter.IndexOf("@"), (dsFilter.Length) - dsFilter.IndexOf("@"))
                If tmpdsFilter.Contains(" ") Then tmpdsFilter = tmpdsFilter.Substring(0, tmpdsFilter.IndexOf(" "))
                If tmpdsFilter.Contains(",") Then tmpdsFilter = tmpdsFilter.Substring(0, tmpdsFilter.IndexOf(","))
                If tmpdsFilter.Contains(")") Then tmpdsFilter = tmpdsFilter.Substring(0, tmpdsFilter.IndexOf(")"))
                'dsFilter = dsFilter.Replace(tmpdsFilter, SessionVariables.Item(tmpdsFilter.Replace("@", "")))
                dsFilter = Regex.Replace(dsFilter, "@\b" + tmpdsFilter.Replace("@", "").ToString + "\b", "'" + SessionVariables.Item(tmpdsFilter.Replace("@", "")).ToString + "'")
            End While

            'If buttonID > 0 Then
            '    If dsFilter.Contains("Where") Then
            '        dsFilter += " AND " + m.dtButtons.Select("ID = " + buttonID.ToString)(0)("CommandText").ToString
            '    Else
            '        dsFilter += " WHERE " + m.dtButtons.Select("ID = " + buttonID.ToString)(0)("CommandText").ToString
            '    End If
            'End If

            Return dsFilter
        End Function

        Public Function CheckFieldIf(ByVal m As GSWEB.MenuCollection.Menu, ByVal StringToReplace As String, ByVal refID As Integer, Optional ByVal buttonID As Integer = 0, Optional ByVal ParentID As Integer = 0, Optional ByVal SessionVariables As System.Web.SessionState.HttpSessionState = Nothing, Optional ByVal IsGridToolbarControl As Boolean = False) As Boolean
            If StringToReplace Is Nothing Then Return True
            If StringToReplace.ToString = "" Then Return True
            Dim rslt As String = StringToReplace

            If ParentID <> 0 Then
                Dim pm As GSWEB.MenuCollection.Menu = mCollection.GetMenu(m.ParentID)
                If rslt.Contains("#") Then
                    Dim dt As DataTable = getTable("SELECT * FROM " & SetDataSourceFilter(pm, 0, ParentID, SessionVariables, 1) & " WHERE ID =" & ParentID)
                    ' pm.dtDataSource(0, ParentID, SessionVariables, " WHERE ID = " & refID)
                    If dt.Rows.Count > 0 Then
                        rslt = StringReplace(rslt, dt.Rows(0))
                    Else
                        rslt = StringReplace(rslt, Nothing)
                    End If
                End If
            Else
                If rslt.Contains("#") Then
                Else
                    rslt = StringReplace(rslt, , SessionVariables)
                    Return ExecScalarNoParams("SELECT COUNT(ID) FROM " & SetDataSourceFilter(m, 0, ParentID, SessionVariables) & " WHERE " + rslt + IIf(rslt.Length > 0, " AND ", "") + "ID = " + refID.ToString)
                End If
            End If

            Return False
        End Function

        Public Function StringReplace(ByVal StringToReplace As String, Optional ByVal drParent As DataRow = Nothing, Optional ByVal SessionVariables As System.Web.SessionState.HttpSessionState = Nothing) As String
            Dim rslt As String = StringToReplace

            While rslt.Contains("#")
                Dim tmpStr As String = rslt.Substring(rslt.IndexOf("#"), (rslt.Length) - rslt.IndexOf("#"))
                If tmpStr.Contains(" ") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(" "))
                If tmpStr.Contains(",") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(","))
                If tmpStr.Contains(")") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(")"))
                tmpStr = tmpStr.Replace("#", "")
                If Not drParent Is Nothing Then
                    'rslt = rslt.Replace("#" + tmpStr, IsNull(drParent(tmpStr), "0").ToString.Replace("True", "1").Replace("False", "0"))
                    rslt = Regex.Replace(rslt, "#\b" + tmpStr + "\b", IsNull(drParent(tmpStr), "0").ToString.Replace("True", "1").Replace("False", "0").ToString)
                Else
                    'rslt = rslt.Replace("#" + tmpStr, "0")
                    rslt = Regex.Replace(rslt, "#\b" + tmpStr + "\b", "0")
                End If
            End While
            If Not SessionVariables Is Nothing Then
                While rslt.Contains("@")
                    Dim tmpStr As String = rslt.Substring(rslt.IndexOf("@"), (rslt.Length) - rslt.IndexOf("@"))
                    If tmpStr.Contains(" ") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(" "))
                    If tmpStr.Contains(",") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(","))
                    If tmpStr.Contains(")") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(")"))
                    'rslt = rslt.Replace(tmpStr, SessionVariables.Item(tmpStr.Replace("@", ""))).Replace("True", "1").Replace("False", "0")
                    rslt = Regex.Replace(rslt, "@\b" + tmpStr.Replace("@", "") + "\b", SessionVariables.Item(tmpStr.Replace("@", "")).ToString).Replace("True", "1").Replace("False", "0")
                End While
            End If
            Return rslt
        End Function

        Public Function SetFilter(m As GSWEB.MenuCollection.Menu, filter As String, Optional ParentID As Integer = 0, Optional RefID As Integer = 0, Optional ByVal SessionVariables As System.Web.SessionState.HttpSessionState = Nothing, Optional ByVal ID_WebMenuControlTypes As Integer = 0) As String
            Dim str As String = filter
            If str.Contains("#") Then
                Dim mParent As GSWEB.MenuCollection.Menu = mCollection.GetMenu(m.ParentID)
                If RefID = 0 Then
                    str = StringReplace(str, , SessionVariables)
                ElseIf ParentID > 0 Then
                    Dim dr As DataRow = getTable("SELECT * FROM v" & mParent.ColumnValue("TableName").ToString.Substring(1) & " WHERE ID = " & ParentID).Rows(0)
                    str = StringReplace(str, dr, SessionVariables)

                Else
                    Dim dr As DataRow = getTable("SELECT * FROM v" & mParent.ColumnValue("TableName").ToString.Substring(1) & " WHERE ID = " & RefID).Rows(0)
                    str = StringReplace(str, dr, SessionVariables)
                End If

                'For Each dr As DataRow In mParent.dtDataSource(, , SessionVariables, " WHERE ID = " + ParentID.ToString).Rows
                '    str = StringReplace(str, dr)
                '    Exit For
                'Next
            End If
            Dim drRow As DataTable = m.dtDataSource(, , SessionVariables, " WHERE ID = " & RefID.ToString)
            While str.Contains(":")
                Dim tmpStr As String = str.Substring(str.IndexOf(":"), (str.Length) - str.IndexOf(":"))
                If tmpStr.Contains(" ") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(" "))
                If tmpStr.Contains(",") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(","))
                If tmpStr.Contains(")") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(")"))
                tmpStr = tmpStr.Replace(":", "")
                If drRow.Rows.Count > 0 Then
                    'str = str.Replace(":" + tmpStr, IsNull(drRow(0)(tmpStr), "0").ToString.Replace("True", "1").Replace("False", "0"))
                    str = Regex.Replace(str, ":\b" + tmpStr + "\b", IsNull(drRow(0)(tmpStr), "0").ToString.Replace("True", "1").Replace("False", "0"))
                Else
                    If tmpStr = "ID" Then
                        ' str = str.Replace("= :" + tmpStr, "= " & RefID) 'for testing
                        'str = str.Replace(":" + tmpStr, RefID)
                        str = Regex.Replace(str, ":\b" + tmpStr + "\b", RefID.ToString)
                    Else
                        str = str.Replace("= :" + tmpStr, "<> 0")
                        str = str.Replace(":" & tmpStr, "0") 'TESTING EMIL
                    End If

                End If
            End While
            While str.Contains("@")
                Dim tmpStr As String = str.Substring(str.IndexOf("@"), (str.Length) - str.IndexOf("@"))
                If tmpStr.Contains(" ") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(" "))
                If tmpStr.Contains(",") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(","))
                If tmpStr.Contains(")") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(")"))
                If tmpStr.Contains(";") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(";"))
                'str = str.Replace(tmpStr, SessionVariables.Item(tmpStr.Replace("@", "")))
                str = Regex.Replace(str, "@\b" + tmpStr.Replace("@", "") + "\b", SessionVariables.Item(tmpStr.Replace("@", "")).ToString)
            End While
            Return str
        End Function

        Public Function GetDataType(Type As String) As System.Type
            Select Case Type
                Case "int", "bigint"
                    Return System.Type.GetType("System.Int32")
                Case "datetime", "date"
                    Return System.Type.GetType("System.DateTime")
                Case "varchar", "text"
                    Return System.Type.GetType("System.String")
                Case "decimal", "money", "numeric"
                    Return System.Type.GetType("System.Decimal")
                Case "bit"
                    Return System.Type.GetType("System.Boolean")
                Case "float"
                    Return System.Type.GetType("System.Double")
                Case Else
                    Return Nothing
            End Select
        End Function

        Public Function btnValidation(source As String, drParent As Dictionary(Of String, Object), refID As Integer, Optional ByVal SessionVariables As System.Web.SessionState.HttpSessionState = Nothing) As Boolean
            Dim result As Boolean = False

            Using sqlConn As New SqlConnection(ConnectionString)
                sqlConn.Open()
                Try
                    If source.Contains("#") Then
                        Dim tmpStr As String
                        While source.Contains("#")

                            tmpStr = source.Substring(source.IndexOf("#"), (source.Length) - source.IndexOf("#"))
                            If tmpStr.Contains(" ") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(" "))
                            If tmpStr.Contains(",") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(","))
                            If tmpStr.Contains(")") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(")"))
                            If tmpStr = "#ID" Then
                                'source = source.Replace(tmpStr, "''" & refID.ToString & "''")
                                source = Regex.Replace(source, "#\b" + tmpStr.Replace("#", "") + "\b", "''" & refID.ToString & "''")
                            Else
                                Dim tmp As String = tmpStr.Replace("#", "")
                                'source = source.Replace("#" + tmp, "''" & drParent(tmp) & "''")
                                source = Regex.Replace(source, "#\b" + tmpStr.Replace("#", "") + "\b", "''" & drParent(tmp).ToString & "''")
                            End If
                            'If Not source.Contains("#") Then Exit For
                            'Next
                        End While
                        'If source.Contains("#") Then
                        '    tmpStr = source.Substring(source.IndexOf("#"), (source.Length) - source.IndexOf("#"))
                        '    If tmpStr.Contains(" ") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(" "))
                        '    If tmpStr.Contains(",") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(","))
                        '    If tmpStr.Contains(")") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(")"))
                        '    If tmpStr = "#ID" Then
                        '        'source = source.Replace("#ID", "''" & refID.ToString & "''")
                        '        source = Regex.Replace(source, "@\b" + tmpStr.Replace("#", "") + "\b", "''" & refID.ToString & "''")
                        '    End If
                        'End If
                    End If
                    While source.Contains("@")
                        Dim tmpsource As String = source.Substring(source.IndexOf("@"), (source.Length) - source.IndexOf("@"))
                        If tmpsource.Contains(" ") Then tmpsource = tmpsource.Substring(0, tmpsource.IndexOf(" "))
                        If tmpsource.Contains(",") Then tmpsource = tmpsource.Substring(0, tmpsource.IndexOf(","))
                        If tmpsource.Contains(")") Then tmpsource = tmpsource.Substring(0, tmpsource.IndexOf(")"))
                        source = source.Replace(tmpsource, SessionVariables.Item(tmpsource.Replace("@", "")))
                        source = Regex.Replace(source, "@\b" + tmpsource.Replace("@", "") + "\b", SessionVariables.Item(tmpsource.Replace("@", "")).ToString)
                    End While
                    Using SqlCommand As New SqlCommand("EXEC ('" & source.Replace("false", "0").Replace("true", "1") & "')", sqlConn)
                        result = SqlCommand.ExecuteScalar
                    End Using
                Catch ex As Exception
                    Throw ex

                Finally
                    sqlConn.Close()
                    sqlConn.Dispose()
                End Try
            End Using



            Return result
        End Function

        Public Function formatCondition(s As String) As String
            Return s.Replace("Master.", "") _
                    .Replace("row.entity.", "") _
                    .Replace("==", "=") _
                    .Replace("!=", "<>") _
                    .Replace("&&", "AND") _
                    .Replace("||", "OR") _
                    .Replace("!", "NOT ") _
                    .Replace("true", "1") _
                    .Replace("false", "0")
        End Function

        Public Sub CreateController(pID As Integer)
            Dim sb As New StringBuilder

            sb.AppendLine("var appModule = angular.module('appModule', ['angular.filter', 'mgcrea.ngStrap', 'ngSanitize', 'ui.bootstrap.typeahead', 'ui.bootstrap.tpls']);")
            sb.AppendLine("var dateToIso = function () {")
            sb.AppendLine("return {")
            sb.AppendLine("restrict: 'A',")
            sb.AppendLine("require: 'ngModel',")
            sb.AppendLine("link: function (scope, element, attrs, ngModelCtrl) {")
            sb.AppendLine("ngModelCtrl.$parsers.push(function (datepickerValue) {")
            sb.AppendLine("return moment(datepickerValue).format('YYYY-MM-DD 00:00:00');")
            sb.AppendLine("});")
            sb.AppendLine("}")
            sb.AppendLine("};")
            sb.AppendLine("};")
            sb.AppendLine("appModule.directive('dateToIso', [dateToIso]);")

            sb.AppendLine("var ngMin = function () {")
            sb.AppendLine("return {")
            sb.AppendLine("restrict:   'A',")
            sb.AppendLine("require:    'ngModel',")
            sb.AppendLine("link: function (scope, elem, attr, ctrl) {")
            sb.AppendLine("function isEmpty(value) {")
            sb.AppendLine("return angular.isUndefined(value) || value === '' || value === null || value !== value;")
            sb.AppendLine("}")
            sb.AppendLine("var minValidator = function (value) {")
            sb.AppendLine("var min = scope.$eval(attr.ngMin) || 0;")
            sb.AppendLine("if (!isEmpty(value) && value < min) {")
            sb.AppendLine("ctrl.$setValidity('ngMin', false);")
            sb.AppendLine("return undefined;")
            sb.AppendLine("} else {")
            sb.AppendLine("ctrl.$setValidity('ngMin', true);")
            sb.AppendLine("return value;")
            sb.AppendLine("}")
            sb.AppendLine("};")
            sb.AppendLine("ctrl.$parsers.push(minValidator);")
            sb.AppendLine("ctrl.$formatters.push(minValidator);")
            sb.AppendLine("}")
            sb.AppendLine("};")
            sb.AppendLine("}")
            sb.AppendLine("appModule.directive('ngMin', ngMin);")

            sb.AppendLine("appModule.controller('appData', function($scope, $timeout){")

            sb.AppendLine("$(document).ready(function () {")
            sb.AppendLine("$('#bdate').bind('cut copy paste', function (e) {")
            sb.AppendLine("e.preventDefault();")
            sb.AppendLine("});")
            sb.AppendLine("});")

            sb.AppendLine("var dd = new Date();")
            sb.AppendLine("$scope.sDate = '1/1/' + parseInt(dd.getFullYear() - 10);")
            sb.AppendLine("$scope.minDate = '12/31/' + parseInt(dd.getFullYear() - 10);")

            sb.AppendLine("$scope.tabs = [];")
            sb.AppendLine("$scope.tabs.activeTab = 0;")

            sb.AppendLine("var findFormInvalid = function (ngForm) {")
            sb.AppendLine("var x = 0;")
            sb.AppendLine("for (var i in ngForm) {")
            sb.AppendLine("if (ngForm[i] && ngForm[i].hasOwnProperty && ngForm[i].hasOwnProperty('$invalid') && ngForm[i].$name != 'import-excel') {")
            sb.AppendLine("if (ngForm[i].$invalid) return x;")
            sb.AppendLine("x++;")
            sb.AppendLine("}} return 0 }")

            sb.AppendLine("$scope.addnewrow = function(mID){")
            sb.AppendLine("if(mID == 2068){")
            sb.AppendLine("$scope.Detail[mID].push({'ID': 0, 'SchoolName': null, 'DegreeMajorHonor': null, 'YearFrom': null, 'YearTo': null, 'ID_Persona': null,'IsRequired': true});")
            sb.AppendLine("}else{")
            sb.AppendLine("$scope.Detail[mID].push({'ID': 0, 'LicenseNo': null, 'LicenseName': null, 'LicenseDate': null});")
            sb.AppendLine("}}")

            sb.AppendLine("$scope.removeLiRow = function (mID, index, id) {")
            sb.AppendLine("$scope.Detail[mID].splice(index, 1);")
            sb.AppendLine("}")

            sb.AppendLine("$scope.Master = {")

            'parent
            Dim dt As MenuCollection.Menu = mCollection.GetMenu(pID)
            If dt.dtColumns.Rows.Count > 0 Then

                Dim masterRows As New List(Of String)

                For Each dr In dt.dtColumns.Rows
                    Dim row As String = dr("Name")
                    'masterRows.Add("'" & row & "':" & IIf(row.Contains("Date"), "'" & Date.Today & "'", IIf(dr("colDataType") = "int", 0, IIf(dr("colDataType") = "bit", "false", "null"))) & "")
                    If row.ToString.ToLower.Contains("date") Then
                        masterRows.Add("'" & row & "': '" & Date.Today & "' ")
                    ElseIf dr("colDataType") = "bit" Then
                        masterRows.Add("'" & row & "': false")
                    Else
                        masterRows.Add("'" & row & "': " & IsNull(dr("DefaultValue"), "null") & "")
                    End If
                Next

                sb.AppendLine(String.Join(",", masterRows))

            End If

            sb.AppendLine("};")

            'detail
            sb.AppendLine("$scope.Detail = {};")
            For Each m As MenuCollection.Menu In mCollection.GetChild(pID)

                If m.MenuID <> 2070 And m.MenuID <> 4235 Then

                    sb.AppendLine("$scope.Detail[" & m.MenuID & "] = [{")

                    Dim DetailRows As New List(Of String)
                    For Each mColumns As DataRow In m.dtColumns.Select
                        Dim row As String = mColumns("Name")
                        'DetailRows.Add("'" & row & "':" & IIf(row.Contains("Date"), "'" & Date.Today & "'", IIf(mColumns("colDataType") = "int", 0, IIf(mColumns("colDataType") = "bit", "false", "null"))) & "")
                        If row.ToString.ToLower.Contains("date") Then
                            DetailRows.Add("'" & row & "': '" & Date.Today & "' ")
                        ElseIf mColumns("colDataType") = "bit" Then
                            If m.MenuID = 2068 AndAlso row.ToString = "IsRequired" Then
                                DetailRows.Add("'" & row & "': true")
                            Else
                                DetailRows.Add("'" & row & "': false")
                            End If
                        Else
                            DetailRows.Add("'" & row & "': " & IsNull(mColumns("DefaultValue"), "null") & "")
                        End If
                    Next

                    sb.AppendLine(String.Join(",", DetailRows))

                    sb.AppendLine("}];")

                Else
                    Dim DetailRows As New List(Of String)
                    sb.AppendLine("$scope.Detail[" & m.MenuID & "] = [")

                    Dim dtsource As String = SetDataSourceFilter(m, 0, 0, Nothing)
                    Dim dt2070 As DataTable = getTable("select * from " & dtsource & "")

                    If dt2070.Rows.Count > 0 Then
                        For Each dr In dt2070.Rows
                            Dim dataRows As New List(Of String)

                            For Each dtcol In dt2070.Columns
                                Dim row As String = dtcol.ToString
                                If m.dtColumns.Select("Name = '" & row & "'").Count > 0 Then
                                    Dim coldt As DataTable = m.dtColumns.Select("Name = '" & row & "'").CopyToDataTable
                                    Dim colDataType As String = coldt.Rows(0)("colDataType").ToString
                                    'dataRows.Add("'" & row & "': " & IIf(colDataType = "int", dr(row), IIf(colDataType = "boolean", CBool(dr(row)), IIf(IsNull(dr(row), "") = "", "null", "'" & dr(row).ToString & "'"))) & "")
                                    If colDataType = "bit" Then
                                        dataRows.Add("'" & row & "': " & CBool(IsNull(dr(row), 0)).ToString.ToLower & " ")
                                    ElseIf colDataType = "int" Then
                                        dataRows.Add("'" & row & "': " & IIf(IsNull(dr(row).ToString, "") = "", "null", dr(row).ToString) & " ")
                                    Else
                                        dataRows.Add("'" & row & "': " & IIf(IsNull(dr(row).ToString, "") = "", "null", "'" & dr(row).ToString & "'") & " ")
                                    End If
                                Else
                                    dataRows.Add("'" & row & "': " & IIf(IsNull(dr(row).ToString, "") = "", "null", "'" & dr(row) & "'") & " ")
                                End If
                            Next

                            Dim drowString As String = "{" & String.Join(",", dataRows) & "}"
                            DetailRows.Add(drowString)
                        Next
                    End If

                    sb.AppendLine(String.Join(",", DetailRows))

                    sb.AppendLine("];")
                End If


            Next

            'dropdown source
            sb.AppendLine("$scope.dropdown_source = {};")
            sb.AppendLine("$scope.text_autocomplete_source = {};")
            'master dropdown source
            Dim parentMenuCollection As MenuCollection.Menu = mCollection.GetMenu(pID)
            For Each mColumns As DataRow In parentMenuCollection.dtColumns.Select("ID_WebMenuControlTypes = 2")

                Dim DetailRows As New List(Of String)

                sb.AppendLine("$scope.dropdown_source[" & mColumns("ID") & "] = [")

                Dim source As String = IsNull(mColumns("TableName"), "v" & mColumns("Name").ToString.Substring(3))
                Dim drpdwnSource As DataTable = getTable("select ID,Name from " & source & "")

                If drpdwnSource.Rows.Count > 0 Then
                    For Each drpdwnRows As DataRow In drpdwnSource.Rows
                        DetailRows.Add("{'ID': " & drpdwnRows("ID") & ", 'Name':'" & drpdwnRows("Name") & "'}")
                    Next
                End If

                sb.AppendLine(String.Join(",", DetailRows))
                sb.AppendLine("];")

            Next

            'detail dropdown source
            For Each m As MenuCollection.Menu In mCollection.GetChild(pID)

                Dim DetailRows As New List(Of String)

                For Each mColumns As DataRow In m.dtColumns.Select("ID_WebMenuControlTypes = 2")

                    sb.AppendLine("$scope.dropdown_source[" & mColumns("ID") & "] = [")

                    Dim source As String = IsNull(mColumns("TableName"), "v" & mColumns("Name").ToString.Substring(3))
                    Dim drpdwnSource As DataTable = getTable("select ID,Name from " & source & "")

                    If drpdwnSource.Rows.Count > 0 Then
                        For Each drpdwnRows As DataRow In drpdwnSource.Rows
                            DetailRows.Add("{'ID': " & drpdwnRows("ID") & ", 'Name':'" & drpdwnRows("Name") & "'}")
                        Next
                    End If

                    sb.AppendLine(String.Join(",", DetailRows))
                    sb.AppendLine("];")

                Next

            Next

            'text autocomplete source
            For Each m As MenuCollection.Menu In mCollection.GetChild(pID)

                Dim DetailRows As New List(Of String)

                For Each mColumns As DataRow In m.dtColumns.Select("ID_WebMenuControlTypes = 10")

                    sb.AppendLine("$scope.text_autocomplete_source[" & mColumns("ID") & "] = [")

                    Dim source As String = IsNull(mColumns("TableName"), "v" & mColumns("Name").ToString.Substring(3))
                    Dim drpdwnSource As DataTable = getTable("select Name from " & source & "")

                    If drpdwnSource.Rows.Count > 0 Then
                        For Each drpdwnRows As DataRow In drpdwnSource.Rows
                            DetailRows.Add("'" & drpdwnRows("Name").Replace("'", "\'") & "'")
                        Next
                    End If

                    sb.AppendLine(String.Join(",", DetailRows))
                    sb.AppendLine("];")

                Next

            Next

            sb.AppendLine("$scope.getAge = function (dateString) {")
            sb.AppendLine("var today = new Date();")
            sb.AppendLine("var birthDate = new Date(dateString);")
            sb.AppendLine("var age = today.getFullYear() - birthDate.getFullYear();")
            sb.AppendLine("var m = today.getMonth() - birthDate.getMonth();")
            sb.AppendLine("if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate())) {")
            sb.AppendLine("age--;")
            sb.AppendLine("}")
            sb.AppendLine("if (!isNaN(age) && ($scope.Master.BirthDate != '' && $scope.Master.BirthDate != undefined && $scope.Master.BirthDate != null)) {")
            sb.AppendLine("$scope.Master.Age = age;")
            sb.AppendLine("return age;")
            sb.AppendLine("}")
            sb.AppendLine("}")

            sb.AppendLine("$scope.Detail[2066].push({ 'ID': 0, 'Company': '', 'Designation': '', 'EmploymentStatus': '', 'YearsOfService': '', 'MonthsOfService': '', 'ReasonForLeaving': '', 'ImmediateSupervisor': '', 'ImmediateSupervisorDesignation': '', 'ContactNo': '', 'ID_Persona': 0 });")
            sb.AppendLine("$scope.Detail[2066].push({ 'ID': 0, 'Company': '', 'Designation': '', 'EmploymentStatus': '', 'YearsOfService': '', 'MonthsOfService': '', 'ReasonForLeaving': '', 'ImmediateSupervisor': '', 'ImmediateSupervisorDesignation': '', 'ContactNo': '', 'ID_Persona': 0 });")
            sb.AppendLine("$scope.Detail[2066].push({ 'ID': 0, 'Company': '', 'Designation': '', 'EmploymentStatus': '', 'YearsOfService': '', 'MonthsOfService': '', 'ReasonForLeaving': '', 'ImmediateSupervisor': '', 'ImmediateSupervisorDesignation': '', 'ContactNo': '', 'ID_Persona': 0 });")
            sb.AppendLine("$scope.Detail[2069].push({ 'ID': 0, 'Name': '', 'CompanyName': '', 'Designation': '', 'ContactNo': '', 'ID_Persona': 0, 'IsRequired': 1 });")
            sb.AppendLine("$scope.Detail[2069].push({ 'ID': 0, 'Name': '', 'CompanyName': '', 'Designation': '', 'ContactNo': '', 'ID_Persona': 0, 'IsRequired': 1 });")
            sb.AppendLine("$scope.Detail[2069][0].IsRequired = 1;")
            sb.AppendLine("$scope.Master.BirthDate = null;")
            sb.AppendLine("$scope.Master.DateIssued = null;")

            sb.AppendLine("$scope.Detail[4238] = [];")

            sb.AppendLine("var suffix = $scope.Master.Suffix;")
            sb.AppendLine("$scope.Master.Suffix = (suffix == null ? '' : suffix.toUpperCase());")

            sb.AppendLine("$scope.generateSaveData = function(){")
            sb.AppendLine("$scope.mainform.$submitted = true;")
            sb.AppendLine("if($scope.mainform.$valid){")

            sb.AppendLine("var textToWrite = '{ ""Master"":' + angular.toJson($scope.Master) + ', ""Detail"":' + angular.toJson($scope.Detail) + '}';")
            sb.AppendLine("var textFileAsBlob = new Blob([textToWrite], { type: 'text/plain' });")
            sb.AppendLine("var fileNameToSaveAs = 'ApplicationFormData.json';")
            sb.AppendLine("var downloadLink = document.createElement('a');")
            sb.AppendLine("downloadLink.download = fileNameToSaveAs;")
            sb.AppendLine("downloadLink.innerHTML = 'Download File';")
            sb.AppendLine("if (window.webkitURL != null) {")
            sb.AppendLine("downloadLink.href = window.webkitURL.createObjectURL(textFileAsBlob);")
            sb.AppendLine("}else{")
            sb.AppendLine("downloadLink.href = window.URL.createObjectURL(textFileAsBlob);")
            sb.AppendLine("downloadLink.onclick = destroyClickedElement;")
            sb.AppendLine("downloadLink.style.display = 'none';")
            sb.AppendLine("document.body.appendChild(downloadLink);")

            sb.AppendLine("}")
            sb.AppendLine("downloadLink.click();")
            sb.AppendLine("}else{")
            sb.AppendLine("var delay = undefined;")
            sb.AppendLine("$timeout(function () {")
            sb.AppendLine("if ($scope.mainform.$error.pattern || $scope.mainform.$error.minlength || $scope.mainform.$error.maxlength) {")
            'sb.AppendLine("alert('Invalid values on other fields.');")
            sb.AppendLine("} else {")
            sb.AppendLine("alert('Fill all the required fields.');")
            sb.AppendLine("}")
            sb.AppendLine("$scope.tabs.activeTab = (findFormInvalid($scope['mainform']) >= 1 ? 1 : findFormInvalid($scope['mainform'])) - (delay == undefined ? 0 : delay);")
            sb.AppendLine("});")
            sb.AppendLine("}")
            sb.AppendLine("}")

            sb.AppendLine("$scope.IgnoreNumbers = function (e) {")
            sb.AppendLine("if (e.keyCode >= 48 && e.keyCode <= 57) {")
            sb.AppendLine("e.preventDefault();")
            sb.AppendLine("return false;")
            sb.AppendLine("}}")

            sb.AppendLine("$scope.IgnoreCharacters = function (e) {")
            sb.AppendLine("if (e.keyCode < 48 || e.keyCode > 57) {")
            sb.AppendLine("e.preventDefault();")
            sb.AppendLine("return false;")
            sb.AppendLine("}}")

            sb.AppendLine("if ($scope.Master.BirthDate != undefined || $scope.Master.BirthDate != null) {")
            sb.AppendLine("$scope.Master.BirthDate = moment($scope.Master.BirthDate).format('MM/DD/YYYY');}")
            sb.AppendLine("$scope.formatDate = function () {")
            sb.AppendLine("var value = $scope.Master.BirthDate;")
            sb.AppendLine("var b;")
            sb.AppendLine("value = value.replace(/^([\d]{2})([\d]{2})([\d]{4})$/, '$1/$2/$3');")
            sb.AppendLine("b = value;")
            sb.AppendLine("var m = moment(b);")
            sb.AppendLine("if (m.isValid()) {")
            sb.AppendLine("$scope.Master.BirthDate = b;")
            sb.AppendLine("$scope.mainform.BirthDate.$error.pattern = false;")
            sb.AppendLine("$scope.mainform.BirthDate.$invalid = false;")
            sb.AppendLine("} else {")
            sb.AppendLine("$scope.mainform.BirthDate.$error.pattern = true;")
            sb.AppendLine("$scope.mainform.BirthDate.$invalid = true;")
            sb.AppendLine("}}")

            sb.AppendLine("});")

            CreateDocument(sb.ToString, "ApplicationForm", "Controller.js")


        End Sub

        Public Sub CreateDocument(fileContent As String, path As String, filename As String)
            Dim logFile As String = String.Empty
            Dim logWriter As StreamWriter = Nothing

            Try
                logFile = HttpContext.Current.Server.MapPath("~/" & path & "/" & filename)
                If (File.Exists(logFile)) Then
                    File.Delete(logFile)
                    logWriter = File.CreateText(logFile)
                Else
                    logWriter = File.CreateText(logFile)
                End If
                logWriter.WriteLine(fileContent)
                logWriter.Close()

            Catch ex As Exception
                'Throw
            Finally

            End Try

        End Sub

        Public Function SessionToDictionary(SessionVariables As System.Web.SessionState.HttpSessionState) As Dictionary(Of String, Object)
            Dim d As New Dictionary(Of String, Object)
            Dim keys As String() = {"ID_User", "ID_Persona", "ID_Employee", "ID_Company", "ID_Branch", "ID_Department", "ID_UserGroup", "ProfileImage", "ApplicantUserGroup", "isFirstLog", "IsPres", "isPasswordExpired", "ID_SelectedCompany", "TimeOutExpire", "IsSecretQuestionReady", "EnableCompanySelector"}
            For i = 0 To keys.Length - 1
                If (keys(i) = "ProfileImage") Then
                    d.Add(keys(i), """" & SessionVariables(keys(i)) & """")
                Else
                    If keys(i) = "isFirstLog" Or keys(i) = "isPasswordExpired" Then
                        d.Add(keys(i), SessionVariables(keys(i)).ToString.ToLower)
                    Else
                        d.Add(keys(i), SessionVariables(keys(i)))
                    End If
                End If

            Next
            Return d
        End Function

        Public Function SaveUploadedFile(mfData As MultipartFileData, UploadPath As String) As String
            Dim uploadedFileInfo = New FileInfo(mfData.LocalFileName),
                fileName As String = String.Empty,
                fileGUID As String = Guid.NewGuid.ToString
            If String.IsNullOrEmpty(mfData.Headers.ContentDisposition.FileName) Then
                fileName = fileGUID
            End If
            fileName = mfData.Headers.ContentDisposition.FileName
            If fileName.StartsWith("""") And fileName.EndsWith("""") Then
                fileName = fileName.Trim("""")
            End If
            If fileName.Contains("/") Or fileName.Contains("\") Then
                fileName = Path.GetFileName(fileName)
            End If

            fileName = fileGUID & Path.GetExtension(fileName)

            File.Move(mfData.LocalFileName, Path.Combine(HttpContext.Current.Server.MapPath("~/" & UploadPath), fileName))

            Return fileName
        End Function

        Public Function GenerateViewPageSource(m As GSWEB.MenuCollection.Menu, rID As Integer, pID As Integer, Session As System.Web.SessionState.HttpSessionState, Optional ByVal menuFilter As String = "") As String
            Dim ret As String = String.Empty

            Dim menuTemp As New List(Of String)
            Dim strQueryBuilder As New StringBuilder
            Dim columnDefs As New List(Of String),
                groups As New List(Of String)
            Try

                For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID)
                    Dim dataSource As String = SetDataSourceFilter(m2, 0, rID, Session, , menuFilter)
                    Dim query As String = String.Empty
                    If m2.ColumnValue("ID_WebMenuTypes") <> 15 Then
                        Dim columns As String = String.Empty,
                            pagination As String = String.Empty

                        Dim dt As DataTable = getTable("SELECT * FROM vWebMenuUserColumns WHERE ID_User =  " & Session.Item("ID_User") & " AND ID_WebMenus = " & m2.MenuID & " ORDER BY SeqNo")
                        If dt.Rows.Count = 0 Then
                            columns = m2.getListColumns
                            columnDefs.Add("[]")
                            groups.Add("")
                        Else
                            columns = "ID,"
                            Dim sb As New StringBuilder
                            sb.Append("[")
                            If CBool(IsNull(m2.ColumnValue("HasOpen"), False)) Then
                                sb.Append("{field:'$$'")
                                'If fixedColumns > 0 AndAlso IsNull(m.ColumnValue("FixedColumns"), 0) > 0 Then
                                '    sb.Append(",pinned:true")
                                '    fixedColumns -= 1
                                'End If
                                sb.Append(",width:30,sortable:false,resizable:false,displayName:' ',cellTemplate:'<div class=\'m-grid-cell-contents\' " & If(m2.ColumnValue("ID_WebMenuTypes") = 14, "", "ng-if=\'row.entity.ID > 0\'") & "><span><a ui-sref=\'" & buildAppStateName(m2) & "({ ID_" & m2.MenuID & ":row.entity.$$rID})\'><i class=\'fa fa-lg fa-fw fa-edit\'></i></a></span></div>'},")
                                'sb.Append(",width:30,displayName:'',cellTemplate:'<div class=\'ngCellText\' " & If(m.ColumnValue("ID_WebMenuTypes") = 14, "", "ng-if=\'row.entity.ID > 0\'") & "><span><a ui-sref=\'" & buildAppStateName(m) & "({ ID_" & m.MenuID & ":row.entity.$$rID})\'><i class=\'fa fa-lg fa-fw fa-edit\'></i></a></span></div>'},")
                            End If
                            Dim grps As New List(Of String)
                            For Each dr As DataRow In dt.Select
                                If dr("Name").ToString.ToLower <> "id" Then
                                    columns &= dr("Name").ToString & ","
                                    If Not IsDBNull(dr("ID_WebMenuControlTypes")) Then
                                        If dr("ID_WebMenuControlTypes") = 23 Or dr("ID_WebMenuControlTypes") = 2 Then
                                            columns &= dr("Name").ToString.Substring(3) & ","
                                        ElseIf dr("ID_WebMenuControlTypes") = 25 Then
                                            columns &= dr("Name").ToString & "_GUID,"
                                        End If
                                    End If
                                End If
                                sb.Append("{field:'" & If(dr("Name").ToString.ToLower.Contains("id_") AndAlso dr("colDataType") = "int", dr("Name").ToString.Substring(3), dr("Name")) & "'" &
                                                If(dr("ID_WebMenuControlTypes") = 7, ",width:50", If(IsNull(dr("Width"), "").ToString = "", ",width:'*'", ",width:'" & dr("Width").ToString & "'")) &
                                                ",displayName:'" & addStripSlashes(IsNull(dr("Label"), dr("Name"))) & "'")
                                'If fixedColumns > 0 AndAlso IsNull(m.ColumnValue("FixedColumns"), 0) > 0 Then
                                '    sb.Append(",pinned:true")
                                '    fixedColumns -= 1
                                'End If
                                If dr("ID_WebMenuControlTypes") = 7 Then
                                    sb.Append(",cellTemplate: '<div class=\'m-grid-cell-contents\'><img ng-src=\'Contents/Photos/{{row.entity." & dr("Name") & "}}\' alt=\'{{row.entity." & dr("Name") & "}}\' height=\'40\' width=\'40\'/></div>'")
                                ElseIf dr("ID_WebMenuControlTypes") = 6 Then
                                    sb.Append(",cellTemplate:'<div class=\'m-grid-cell-contents\'><span><a ui-sref=\'{{row.entity.ID_WebMenus}}({ ID_{{row.entity.ID_WebMenus}}:row.entity.RefID})\'><i class=\'fa fa-lg fa-fw fa-edit\'></i></a></span></div>'")
                                ElseIf dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("date") AndAlso dr("Name").ToString.ToLower.Contains("time") Then
                                    sb.Append(",cellFilter:'date:\'MM/dd/yyyy hh:mm:ss a\''")
                                ElseIf dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("date") Then
                                    sb.Append(",headerCellTemplateUrl:'mgrid/headerCellTemplateDate.html'")
                                    sb.Append(",cellFilter:'date:\'MM/dd/yyyy\''")
                                ElseIf dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("time") Then
                                    sb.Append(",headerCellTemplateUrl:'mgrid/headerCellTemplateTime.html'")
                                    sb.Append(",cellFilter:'date:\'hh:mm:ss a\''")
                                ElseIf dr("colDataType") = "bit" Then
                                    sb.Append(",cellTemplate:'<div class=\'m-grid-cell-contents\'><div class=\'smart-form noselect for_checkbox material-switch\'>" &
                                                            "<input disabled id=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' type=\'checkbox\' name=\'" & dr("Name") & "\' " &
                                                            "class=\'form-checkbox\' ng-model=\'row.entity." & dr("Name") & "\' disabled/>" &
                                                            "<label for=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' class=\'label-default\'></label>" &
                                                            "</div></div>'")
                                ElseIf dr("colDataType") = "varchar" OrElse dr("colDataType") = "nvarchar" OrElse dr("colDataType") = "text" Then
                                    sb.Append(",cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity." & dr("Name") & " | trustedHTML\'></span></div>'")
                                End If
                                If dr("GroupSeqNo") > 0 Then
                                    sb.Append(",visible: false,")
                                End If

                                sb.Append("},")
                            Next
                            sb.Append("]")
                            columns = columns.Trim(",")
                            columnDefs.Add(sb.ToString)

                            groups.Add(String.Join(",", dt.AsEnumerable() _
                                                          .Where(Function(x) x.Item("GroupSeqNo") > 0) _
                                                          .OrderBy(Function(x) x.Item("GroupSeqNo")).Select(Function(x) "'" & If(x.Item("Name").ToString.ToLower.Contains("id"), x.Item("Name").ToString.Substring(3), x.Item("Name").ToString) & "'").ToList))
                            '  columns = "ID," & String.Join(",", dt.AsEnumerable().Select(Function(x) x.Item("WebMenuColumns")).ToList)
                        End If

                        If m2.ColumnValue("WithPagination") Then
                            pagination = "  OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY"
                            'strQueryBuilder.Append("SELECT " & m2.getListColumns & " FROM " & dataSource & " ORDER BY " & IsNull(m2.ColumnValue("SortBy"), "ID DESC") & "  OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY" & ";")
                            ' Else
                            ' strQueryBuilder.Append("SELECT " & m2.getListColumns & " FROM " & dataSource & " ORDER BY " & IsNull(m2.ColumnValue("SortBy"), "ID DESC") & ";")
                        End If
                        strQueryBuilder.Append("SELECT " & columns & " FROM " & dataSource & " ORDER BY " & IsNull(m2.ColumnValue("SortBy"), "ID DESC") & pagination & ";")
                        strQueryBuilder.Append("SELECT COUNT(ID) FROM " & dataSource & ";")
                        menuTemp.Add(m2.MenuID.ToString)
                    End If
                Next

                'DROPDOWN SOURCES
                Dim ddTemp As New List(Of String)
                For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 3 Or x.ColumnValue("ID_WebMenuTypes") = 15)
                    generateFilterControlQueryBuilder(strQueryBuilder, ddTemp, m2, rID, pID, HttpContext.Current.Session, 2)
                Next
                For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 17)
                    For Each dr As DataRow In m2.dtColumns.Select("ID_WebMenuControlTypes = 2")
                        Dim filter As String = IsNull(dr("Filter"), ""),
                            dataSource = IsNull(dr("TableName"), "t" & dr("Name").ToString.Substring(3))
                        If filter = "" OrElse filter.Contains(":") Then
                            strQueryBuilder.Append("SELECT " & IsNull(dr("DisplayID"), "ID") & " AS ID," & IsNull(dr("DisplayMember"), "Name") & " AS Name " & IIf(IsNull(dr("AdditionalColumns"), "") <> "", ", " & dr("AdditionalColumns") & "", "") & " FROM " & dataSource & " ORDER BY " & IsNull(dr("DisplayMember"), "Name") & " ASC" & ";")
                        Else
                            strQueryBuilder.Append("SELECT " & IsNull(dr("DisplayID"), "ID") & " AS ID," & IsNull(dr("DisplayMember"), "Name") & " AS Name " & IIf(IsNull(dr("AdditionalColumns"), "") <> "", ", " & dr("AdditionalColumns") & "", "") & " FROM " & dataSource & " WHERE " & SetFilter(m, filter, pID, rID, HttpContext.Current.Session) & " ORDER BY " & IsNull(dr("DisplayMember"), "Name") & " ASC" & ";")
                        End If
                        ddTemp.Add(dr("ID"))
                    Next
                Next
                'AUTO COMPLETE
                Dim ddAutoCom As New List(Of String)
                For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 3)
                    generateFilterControlQueryBuilder(strQueryBuilder, ddAutoCom, m2, rID, pID, HttpContext.Current.Session, 23)
                Next
                For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 17)
                    For Each dr As DataRow In m2.dtColumns.Select("ID_WebMenuControlTypes = 23")
                        Dim filter As String = IsNull(dr("Filter"), ""),
                            dataSource = IsNull(dr("TableName"), "t" & dr("Name").ToString.Substring(3))
                        If filter = "" OrElse filter.Contains(":") Then
                            strQueryBuilder.Append("SELECT " & IsNull(dr("DisplayID"), "ID") & " AS ID," & IsNull(dr("DisplayMember"), "Name") & " AS Name FROM " & dataSource & " ORDER BY " & IsNull(dr("DisplayMember"), "Name") & " ASC" & ";")
                        Else
                            strQueryBuilder.Append("SELECT " & IsNull(dr("DisplayID"), "ID") & " AS ID," & IsNull(dr("DisplayMember"), "Name") & " AS Name FROM " & dataSource & " WHERE " & SetFilter(m, filter, pID, rID, HttpContext.Current.Session) & " ORDER BY " & IsNull(dr("DisplayMember"), "Name") & " ASC" & ";")
                        End If
                        ddAutoCom.Add(dr("ID"))
                    Next
                Next
                'TEXT AUTO COMPLETE
                Dim ddTAutoCom As New List(Of String)
                For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 3)
                    generateFilterControlQueryBuilder(strQueryBuilder, ddTAutoCom, m2, rID, pID, HttpContext.Current.Session, 10)
                Next
                For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 17)
                    For Each dr As DataRow In m2.dtColumns.Select("ID_WebMenuControlTypes = 10")
                        Dim filter As String = IsNull(dr("Filter"), ""),
                            dataSource = IsNull(dr("TableName"), "t" & dr("Name").ToString)
                        If filter = "" OrElse filter.Contains(":") Then
                            strQueryBuilder.Append("SELECT " & IsNull(dr("DisplayMember"), "Name") & " AS Name FROM " & dataSource & " ORDER BY " & IsNull(dr("DisplayMember"), "Name") & " ASC" & ";")
                        Else
                            strQueryBuilder.Append("SELECT " & IsNull(dr("DisplayMember"), "Name") & " AS Name FROM " & dataSource & " WHERE " & SetFilter(m, filter, pID, rID, HttpContext.Current.Session) & " ORDER BY " & IsNull(dr("DisplayMember"), "Name") & " ASC" & ";")
                        End If
                        ddTAutoCom.Add(dr("ID"))
                    Next
                Next
                'LOOKUP
                Dim ddLookup As New List(Of String)
                For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 3 Or x.ColumnValue("ID_WebMenuTypes") = 15)
                    For Each dr As DataRow In m2.dtColumns.Select("ID_FilterWebMenuControlTypes = 17")
                        Dim filter As String = IsNull(dr("Filter"), ""),
                        dataSource As String = String.Empty,
                        menu As GSWEB.MenuCollection.Menu = mCollection.GetMenu(dr("ID_TargetWebMenus")), lookupCols As New List(Of String)
                        If Not IsNothing(menu) Then
                            dataSource = IsNull(menu.ColumnValue("DataSource"), dr("TableName").ToString)

                            If IsNull(dataSource, "") <> "" Then
                                For Each lookuprows As DataRow In menu.dtColumns.Select("ID_WebMenuControlTypes IS NOT NULL")
                                    'If IsNull(lookuprows("Label"), "") = "" Then
                                    '    lookupCols.Add(lookuprows("Name"))
                                    'ElseIf IsNull(lookuprows("Label"), "") <> "" And lookuprows("Name").ToString.ToLower <> "id" Then
                                    '    lookupCols.Add(lookuprows("Name") & " as [" & lookuprows("Label") & "]")
                                    'Else
                                    '    lookupCols.Add(lookuprows("Name"))
                                    'End If

                                    If IsNull(lookuprows("Label"), "") = "" Then
                                        lookupCols.Add("[" & lookuprows("Name") & "]")
                                    ElseIf IsNull(lookuprows("Label"), "") <> "" And lookuprows("Name").ToString.ToLower <> "id" And lookuprows("Name").ToString.ToLower.StartsWith("id_") Then
                                        lookupCols.Add("[" & lookuprows("Name").ToString.Substring(3) & "]" & " as [" & lookuprows("Label") & "]")
                                    ElseIf IsNull(lookuprows("Label"), "") <> "" And lookuprows("Name").ToString.ToLower <> "id" And Not (lookuprows("Name").ToString.ToLower.StartsWith("id_")) Then
                                        lookupCols.Add("[" & lookuprows("Name") & "]" & " as [" & lookuprows("Label") & "]")
                                    Else
                                        lookupCols.Add("[" & lookuprows("Name") & "]")
                                    End If
                                Next
                                If filter = "" Then
                                    strQueryBuilder.Append("SELECT " & String.Join(",", lookupCols) & " FROM " & SetFilter(menu, dataSource, pID, rID, HttpContext.Current.Session) & IIf(IsNull(menu.ColumnValue("SortBy"), "") <> "", " ORDER BY ID ASC ", "") & ";")
                                Else
                                    strQueryBuilder.Append("SELECT " & String.Join(",", lookupCols) & " FROM " & SetFilter(menu, dataSource, pID, rID, HttpContext.Current.Session) & " WHERE " & SetFilter(menu, filter, pID, rID, HttpContext.Current.Session) & IIf(IsNull(menu.ColumnValue("SortBy"), "") <> "", " ORDER BY ID ASC ", "") & ";")
                                End If
                            End If

                        End If
                        ddLookup.Add(dr("ID"))
                    Next
                Next
                For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 17)
                    For Each dr As DataRow In m2.dtColumns.Select("ID_WebMenuControlTypes = 17")
                        Dim filter As String = IsNull(dr("Filter"), ""),
                        dataSource As String = String.Empty,
                        menu As GSWEB.MenuCollection.Menu = mCollection.GetMenu(dr("ID_TargetWebMenus")), lookupCols As New List(Of String)
                        If Not IsNothing(menu) Then
                            dataSource = IsNull(menu.ColumnValue("DataSource"), dr("TableName").ToString)

                            If IsNull(dataSource, "") <> "" Then
                                For Each lookuprows As DataRow In menu.dtColumns.Select()
                                    'If IsNull(lookuprows("Label"), "") = "" Then
                                    '    lookupCols.Add(lookuprows("Name"))
                                    'ElseIf IsNull(lookuprows("Label"), "") <> "" And lookuprows("Name").ToString.ToLower <> "id" Then
                                    '    lookupCols.Add(lookuprows("Name") & " as [" & lookuprows("Label") & "]")
                                    'Else
                                    '    lookupCols.Add(lookuprows("Name"))
                                    'End If
                                    If IsNull(lookuprows("Label"), "") = "" Then
                                        lookupCols.Add("[" & lookuprows("Name") & "]")
                                    ElseIf IsNull(lookuprows("Label"), "") <> "" And lookuprows("Name").ToString.ToLower <> "id" And lookuprows("Name").ToString.ToLower.StartsWith("id_") Then
                                        lookupCols.Add("[" & lookuprows("Name").ToString.Substring(3) & "]" & " as [" & lookuprows("Label") & "]")
                                    ElseIf IsNull(lookuprows("Label"), "") <> "" And lookuprows("Name").ToString.ToLower <> "id" And Not (lookuprows("Name").ToString.ToLower.StartsWith("id_")) Then
                                        lookupCols.Add("[" & lookuprows("Name") & "]" & " as [" & lookuprows("Label") & "]")
                                    Else
                                        lookupCols.Add("[" & lookuprows("Name") & "]")
                                    End If
                                Next
                                If filter = "" Then
                                    strQueryBuilder.Append("SELECT " & String.Join(",", lookupCols) & " FROM " & SetFilter(menu, dataSource, pID, rID, HttpContext.Current.Session) & IIf(IsNull(menu.ColumnValue("SortBy"), "") <> "", " ORDER BY " & menu.ColumnValue("SortBy"), "") & ";")
                                Else
                                    strQueryBuilder.Append("SELECT " & String.Join(",", lookupCols) & " FROM " & SetFilter(menu, dataSource, pID, rID, HttpContext.Current.Session) & " WHERE " & SetFilter(menu, filter, pID, rID, HttpContext.Current.Session) & IIf(IsNull(menu.ColumnValue("SortBy"), "") <> "", " ORDER BY " & menu.ColumnValue("SortBy"), "") & ";")
                                End If
                            End If

                        End If
                        ddLookup.Add(dr("ID"))
                    Next
                Next

                'SQL DATA FETCH

                ret = "{"
                Using ttCon As New SqlClient.SqlConnection(ConnectionString)
                    ttCon.Open()

                    Using ttCmd As New SqlCommand(strQueryBuilder.ToString, ttCon)
                        Dim Reader As SqlDataReader
                        Reader = ttCmd.ExecuteReader()

                        Dim totalServerItems As New StringBuilder,
                            columnDefinitions As New StringBuilder,
                            groupedColumns As New StringBuilder
                        ret &= "gridData:{"

                        For x As Integer = 0 To menuTemp.Count - 1
                            If x > 0 Then Reader.NextResult()
                            ret &= menuTemp(x) & ":" & serializeNoClose(Reader, True, HttpContext.Current.Session, mCollection.GetMenu(menuTemp(x)).dtColumns.AsEnumerable().Where(Function(c) IsNull(c.Item("ID_WebMenuControlTypes"), 0) = 27).Select(Function(c) CStr(c.Item("Name"))).ToList()) & ","
                            Reader.NextResult()
                            Reader.Read()
                            totalServerItems.Append(menuTemp(x) & " : " & Reader(0).ToString & ",")
                            columnDefinitions.Append(menuTemp(x) & " : " & columnDefs(x) & ",")
                            If groups(x).ToString.Length > 0 Then
                                groupedColumns.Append(menuTemp(x) & " : [" & groups(x) & "],")
                            End If

                        Next x

                        ret &= "},"
                        ret &= "totalServerItems:{ " & totalServerItems.ToString & " },"
                        ret &= "columnDefinitions:{ " & columnDefinitions.ToString & " },"
                        ret &= "groups: {" & groupedColumns.ToString & "},"
                        'DROPDOWN
                        ret &= "filter_dropdown_source:{"
                        For x As Integer = 0 To ddTemp.Count - 1
                            If menuTemp.Count > 0 Then
                                If x > 0 Then Reader.NextResult()
                                ret &= ddTemp(x) & ":" & serializeNoClose(Reader) & ","
                            Else
                                ret &= ddTemp(x) & ":" & serializeNoClose(Reader) & ","
                                Reader.NextResult()
                            End If
                        Next
                        ret &= "},"
                        'AUTO COMPLETE
                        ret &= "filter_autocomplete_source:{"
                        For x As Integer = 0 To ddAutoCom.Count - 1
                            If menuTemp.Count > 0 Then
                                If x > 0 Then Reader.NextResult()
                                ret &= ddAutoCom(x) & ":" & serializeNoClose(Reader) & ","
                            Else
                                ret &= ddAutoCom(x) & ":" & serializeNoClose(Reader) & ","
                                Reader.NextResult()
                            End If
                        Next
                        ret &= "},"
                        'TEXT AUTO COMPLETE
                        ret &= "filter_text_autocomplete_source:{"
                        For x As Integer = 0 To ddTAutoCom.Count - 1
                            If menuTemp.Count > 0 Then
                                If x > 0 Then Reader.NextResult()
                                ret &= ddTAutoCom(x) & ":" & JavascriptArrayNoClose(Reader) & ","
                            Else
                                ret &= ddTAutoCom(x) & ":" & JavascriptArrayNoClose(Reader) & ","
                                Reader.NextResult()
                            End If
                        Next
                        ret &= "},"
                        'LOOKUP
                        ret &= "filter_lookup_source:{"
                        For x As Integer = 0 To ddLookup.Count - 1
                            If menuTemp.Count > 0 Then
                                If x > 0 Then Reader.NextResult()
                                ret &= ddLookup(x) & ":" & serializeNoClose(Reader) & ","
                            Else
                                ret &= ddLookup(x) & ":" & serializeNoClose(Reader) & ","
                                Reader.NextResult()
                            End If
                        Next
                        ret &= "},"

                        Reader.Close()
                    End Using
                    ttCon.Close()
                End Using
                '------------------

            Catch ex As Exception
                logError(ex)
            End Try
            ret &= "}"

            Return ret
        End Function

        Public Async Function GenerateViewPageSourceAsync(m As GSWEB.MenuCollection.Menu, rID As Integer, pID As Integer, Session As System.Web.SessionState.HttpSessionState) As Task(Of String)
            Dim ret As String = String.Empty

            Dim menuTemp As New List(Of String)
            Dim strQueryBuilder As New StringBuilder
            Dim columnDefs As New List(Of String),
                groups As New List(Of String)
            Try
                Dim totalServerItems As New StringBuilder
                Dim gridTasks As Task(Of String)() = New Task(Of String)(mCollection.GetChild(m.MenuID).Count - 1) {}
                Dim act As Func(Of Object, String) = Function(menu As GSWEB.MenuCollection.Menu)
                                                         Dim r As String = String.Empty
                                                         Dim dataSource As String = SetDataSourceFilter(menu, 0, rID, Session)
                                                         Dim query As String = String.Empty
                                                         If menu.ColumnValue("WithPagination") Then
                                                             query = "SELECT " & menu.getListColumns & " FROM " & dataSource & " ORDER BY " & IsNull(menu.ColumnValue("SortBy"), "ID DESC") & "  OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY"
                                                         Else
                                                             query = "SELECT " & menu.getListColumns & " FROM " & dataSource & " ORDER BY " & IsNull(menu.ColumnValue("SortBy"), "ID DESC")
                                                         End If
                                                         totalServerItems.Append(menu.MenuID & " : " & ExecScalarNoParams("SELECT COUNT(ID) FROM " & dataSource) & ",")
                                                         Return menu.MenuID & ":" & getJSONTable(query, True, Session) & ","
                                                     End Function
                Dim ctr As Integer = 0
                For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID)
                    gridTasks(ctr) = Task(Of String).Factory.StartNew(act, m2)
                    ctr += 1
                Next
                Task.WaitAll(gridTasks)
                ret &= "{"
                ret &= "gridData:{"
                For i As Integer = 0 To mCollection.GetChild(m.MenuID).Count - 1
                    ret &= gridTasks(i).Result
                Next
                ret &= "},"
                ret &= "totalServerItems:{ " & totalServerItems.ToString & " },"
                ret &= "}"

                'For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID)
                '    Dim dataSource As String = SetDataSourceFilter(m2, 0, rID, Session)
                '    Dim query As String = String.Empty
                '    If m2.ColumnValue("ID_WebMenuTypes") <> 15 Then
                '        Dim columns As String = String.Empty,
                '            pagination As String = String.Empty

                '        Dim dt As DataTable = getTable("SELECT * FROM vWebMenuUserColumns WHERE ID_User =  " & Session.Item("ID_User") & " AND ID_WebMenus = " & m2.MenuID & " ORDER BY SeqNo")
                '        If dt.Rows.Count = 0 Then
                '            columns = m2.getListColumns
                '            columnDefs.Add("[]")
                '            groups.Add("")
                '        Else
                '            columns = "ID,"
                '            Dim sb As New StringBuilder
                '            sb.Append("[")
                '            If CBool(IsNull(m2.ColumnValue("HasOpen"), False)) Then
                '                sb.Append("{field:'$$'")
                '                'If fixedColumns > 0 AndAlso IsNull(m.ColumnValue("FixedColumns"), 0) > 0 Then
                '                '    sb.Append(",pinned:true")
                '                '    fixedColumns -= 1
                '                'End If
                '                sb.Append(",width:30,sortable:false,resizable:false,displayName:' ',cellTemplate:'<div class=\'m-grid-cell-contents\' " & If(m2.ColumnValue("ID_WebMenuTypes") = 14, "", "ng-if=\'row.entity.ID > 0\'") & "><span><a ui-sref=\'" & buildAppStateName(m2) & "({ ID_" & m2.MenuID & ":row.entity.$$rID})\'><i class=\'fa fa-lg fa-fw fa-edit\'></i></a></span></div>'},")
                '                'sb.Append(",width:30,displayName:'',cellTemplate:'<div class=\'ngCellText\' " & If(m.ColumnValue("ID_WebMenuTypes") = 14, "", "ng-if=\'row.entity.ID > 0\'") & "><span><a ui-sref=\'" & buildAppStateName(m) & "({ ID_" & m.MenuID & ":row.entity.$$rID})\'><i class=\'fa fa-lg fa-fw fa-edit\'></i></a></span></div>'},")
                '            End If
                '            Dim grps As New List(Of String)
                '            For Each dr As DataRow In dt.Select
                '                If dr("Name").ToString.ToLower <> "id" Then
                '                    columns &= dr("Name").ToString & ","
                '                    If Not IsDBNull(dr("ID_WebMenuControlTypes")) Then
                '                        If dr("ID_WebMenuControlTypes") = 23 Or dr("ID_WebMenuControlTypes") = 2 Then
                '                            columns &= dr("Name").ToString.Substring(3) & ","
                '                        ElseIf dr("ID_WebMenuControlTypes") = 25 Then
                '                            columns &= dr("Name").ToString & "_GUID,"
                '                        End If
                '                    End If
                '                End If
                '                sb.Append("{field:'" & If(dr("Name").ToString.ToLower.Contains("id_") AndAlso dr("colDataType") = "int", dr("Name").ToString.Substring(3), dr("Name")) & "'" &
                '                                If(dr("ID_WebMenuControlTypes") = 7, ",width:50", If(IsNull(dr("Width"), "").ToString = "", ",width:'*'", ",width:'" & dr("Width").ToString & "'")) &
                '                                ",displayName:'" & addStripSlashes(IsNull(dr("Label"), dr("Name"))) & "'")
                '                'If fixedColumns > 0 AndAlso IsNull(m.ColumnValue("FixedColumns"), 0) > 0 Then
                '                '    sb.Append(",pinned:true")
                '                '    fixedColumns -= 1
                '                'End If
                '                If dr("ID_WebMenuControlTypes") = 7 Then
                '                    sb.Append(",cellTemplate: '<div class=\'m-grid-cell-contents\'><img ng-src=\'Contents/Photos/{{row.entity." & dr("Name") & "}}\' alt=\'{{row.entity." & dr("Name") & "}}\' height=\'40\' width=\'40\'/></div>'")
                '                ElseIf dr("ID_WebMenuControlTypes") = 6 Then
                '                    sb.Append(",cellTemplate:'<div class=\'m-grid-cell-contents\'><span><a ui-sref=\'{{row.entity.ID_WebMenus}}({ ID_{{row.entity.ID_WebMenus}}:row.entity.RefID})\'><i class=\'fa fa-lg fa-fw fa-edit\'></i></a></span></div>'")
                '                ElseIf dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("date") AndAlso dr("Name").ToString.ToLower.Contains("time") Then
                '                    sb.Append(",cellFilter:'date:\'MM/dd/yyyy hh:mm:ss a\''")
                '                ElseIf dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("date") Then
                '                    sb.Append(",headerCellTemplateUrl:'mgrid/headerCellTemplateDate.html'")
                '                    sb.Append(",cellFilter:'date:\'MM/dd/yyyy\''")
                '                ElseIf dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("time") Then
                '                    sb.Append(",headerCellTemplateUrl:'mgrid/headerCellTemplateTime.html'")
                '                    sb.Append(",cellFilter:'date:\'hh:mm:ss a\''")
                '                ElseIf dr("colDataType") = "bit" Then
                '                    sb.Append(",cellTemplate:'<div class=\'m-grid-cell-contents\'><div class=\'smart-form noselect for_checkbox\'>" &
                '                                            "<label class=\'toggle state-disabled\'>" &
                '                                            "<input type=\'checkbox\' name=\'" & dr("Name") & "\' " &
                '                                            "class=\'form-checkbox\' ng-model=\'row.entity." & dr("Name") & "\' disabled/>" &
                '                                            "<i data-swchon-text=\'Yes\' data-swchoff-text=\'No\'></i>" &
                '                                            "</label>" &
                '                                            "</div></div>'")
                '                ElseIf dr("colDataType") = "varchar" OrElse dr("colDataType") = "nvarchar" OrElse dr("colDataType") = "text" Then
                '                    sb.Append(",cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity." & dr("Name") & " | trustedHTML\'></span></div>'")
                '                End If
                '                If dr("GroupSeqNo") > 0 Then
                '                    sb.Append(",visible: false,")
                '                End If

                '                sb.Append("},")
                '            Next
                '            sb.Append("]")
                '            columns = columns.Trim(",")
                '            columnDefs.Add(sb.ToString)

                '            groups.Add(String.Join(",", dt.AsEnumerable() _
                '                                          .Where(Function(x) x.Item("GroupSeqNo") > 0) _
                '                                          .OrderBy(Function(x) x.Item("GroupSeqNo")).Select(Function(x) "'" & If(x.Item("Name").ToString.ToLower.Contains("id"), x.Item("Name").ToString.Substring(3), x.Item("Name").ToString) & "'").ToList))
                '            '  columns = "ID," & String.Join(",", dt.AsEnumerable().Select(Function(x) x.Item("WebMenuColumns")).ToList)
                '        End If

                '        If m2.ColumnValue("WithPagination") Then
                '            pagination = "  OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY"
                '            'strQueryBuilder.Append("SELECT " & m2.getListColumns & " FROM " & dataSource & " ORDER BY " & IsNull(m2.ColumnValue("SortBy"), "ID DESC") & "  OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY" & ";")
                '            ' Else
                '            ' strQueryBuilder.Append("SELECT " & m2.getListColumns & " FROM " & dataSource & " ORDER BY " & IsNull(m2.ColumnValue("SortBy"), "ID DESC") & ";")
                '        End If
                '        strQueryBuilder.Append("SELECT " & columns & " FROM " & dataSource & " ORDER BY " & IsNull(m2.ColumnValue("SortBy"), "ID DESC") & pagination & ";")
                '        strQueryBuilder.Append("SELECT COUNT(ID) FROM " & dataSource & ";")
                '        menuTemp.Add(m2.MenuID.ToString)
                '    End If
                'Next

                ''DROPDOWN SOURCES
                'Dim ddTemp As New List(Of String)
                'For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 3 Or x.ColumnValue("ID_WebMenuTypes") = 15)
                '    generateFilterControlQueryBuilder(strQueryBuilder, ddTemp, m2, rID, pID, HttpContext.Current.Session, 2)
                'Next
                'For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 17)
                '    For Each dr As DataRow In m2.dtColumns.Select("ID_WebMenuControlTypes = 2")
                '        Dim filter As String = IsNull(dr("Filter"), ""),
                '            dataSource = IsNull(dr("TableName"), "t" & dr("Name").ToString.Substring(3))
                '        If filter = "" OrElse filter.Contains(":") Then
                '            strQueryBuilder.Append("SELECT " & IsNull(dr("DisplayID"), "ID") & " AS ID," & IsNull(dr("DisplayMember"), "Name") & " AS Name FROM " & dataSource & " ORDER BY " & IsNull(dr("DisplayMember"), "Name") & " ASC" & ";")
                '        Else
                '            strQueryBuilder.Append("SELECT " & IsNull(dr("DisplayID"), "ID") & " AS ID," & IsNull(dr("DisplayMember"), "Name") & " AS Name FROM " & dataSource & " WHERE " & SetFilter(m, filter, pID, rID, HttpContext.Current.Session) & " ORDER BY " & IsNull(dr("DisplayMember"), "Name") & " ASC" & ";")
                '        End If
                '        ddTemp.Add(dr("ID"))
                '    Next
                'Next
                ''AUTO COMPLETE
                'Dim ddAutoCom As New List(Of String)
                'For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 3)
                '    generateFilterControlQueryBuilder(strQueryBuilder, ddAutoCom, m2, rID, pID, HttpContext.Current.Session, 23)
                'Next
                'For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 17)
                '    For Each dr As DataRow In m2.dtColumns.Select("ID_WebMenuControlTypes = 23")
                '        Dim filter As String = IsNull(dr("Filter"), ""),
                '            dataSource = IsNull(dr("TableName"), "t" & dr("Name").ToString.Substring(3))
                '        If filter = "" OrElse filter.Contains(":") Then
                '            strQueryBuilder.Append("SELECT " & IsNull(dr("DisplayID"), "ID") & " AS ID," & IsNull(dr("DisplayMember"), "Name") & " AS Name FROM " & dataSource & " ORDER BY " & IsNull(dr("DisplayMember"), "Name") & " ASC" & ";")
                '        Else
                '            strQueryBuilder.Append("SELECT " & IsNull(dr("DisplayID"), "ID") & " AS ID," & IsNull(dr("DisplayMember"), "Name") & " AS Name FROM " & dataSource & " WHERE " & SetFilter(m, filter, pID, rID, HttpContext.Current.Session) & " ORDER BY " & IsNull(dr("DisplayMember"), "Name") & " ASC" & ";")
                '        End If
                '        ddAutoCom.Add(dr("ID"))
                '    Next
                'Next
                ''TEXT AUTO COMPLETE
                'Dim ddTAutoCom As New List(Of String)
                'For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 3)
                '    generateFilterControlQueryBuilder(strQueryBuilder, ddTAutoCom, m2, rID, pID, HttpContext.Current.Session, 10)
                'Next
                'For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 17)
                '    For Each dr As DataRow In m2.dtColumns.Select("ID_WebMenuControlTypes = 10")
                '        Dim filter As String = IsNull(dr("Filter"), ""),
                '            dataSource = IsNull(dr("TableName"), "t" & dr("Name").ToString)
                '        If filter = "" OrElse filter.Contains(":") Then
                '            strQueryBuilder.Append("SELECT " & IsNull(dr("DisplayMember"), "Name") & " AS Name FROM " & dataSource & " ORDER BY " & IsNull(dr("DisplayMember"), "Name") & " ASC" & ";")
                '        Else
                '            strQueryBuilder.Append("SELECT " & IsNull(dr("DisplayMember"), "Name") & " AS Name FROM " & dataSource & " WHERE " & SetFilter(m, filter, pID, rID, HttpContext.Current.Session) & " ORDER BY " & IsNull(dr("DisplayMember"), "Name") & " ASC" & ";")
                '        End If
                '        ddTAutoCom.Add(dr("ID"))
                '    Next
                'Next
                ''LOOKUP
                'Dim ddLookup As New List(Of String)
                'For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 3 Or x.ColumnValue("ID_WebMenuTypes") = 15)
                '    For Each dr As DataRow In m2.dtColumns.Select("ID_FilterWebMenuControlTypes = 17")
                '        Dim filter As String = IsNull(dr("Filter"), ""),
                '        dataSource As String = String.Empty,
                '        menu As GSWEB.MenuCollection.Menu = mCollection.GetMenu(dr("ID_TargetWebMenus")), lookupCols As New List(Of String)
                '        If Not IsNothing(menu) Then
                '            dataSource = IsNull(menu.ColumnValue("DataSource"), dr("TableName").ToString)

                '            If IsNull(dataSource, "") <> "" Or Not (dataSource.Contains(":")) Or Not (dataSource.Contains("@")) Then
                '                For Each lookuprows As DataRow In menu.dtColumns.Select()
                '                    If IsNull(lookuprows("Label"), "") = "" Then
                '                        lookupCols.Add(lookuprows("Name"))
                '                    Else
                '                        lookupCols.Add(lookuprows("Name") & " as " & lookuprows("Label"))
                '                    End If
                '                Next
                '                If filter = "" Then
                '                    strQueryBuilder.Append("SELECT " & String.Join(",", lookupCols) & " FROM " & dataSource & " ORDER BY " & menu.ColumnValue("SortBy") & ";")
                '                Else
                '                    strQueryBuilder.Append("SELECT " & String.Join(",", lookupCols) & " FROM " & dataSource & " WHERE " & SetFilter(menu, filter, pID, rID, HttpContext.Current.Session) & " ORDER BY " & menu.ColumnValue("SortBy") & ";")
                '                End If
                '            End If

                '        End If
                '        ddLookup.Add(dr("ID"))
                '    Next
                'Next
                'For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 17)
                '    For Each dr As DataRow In m2.dtColumns.Select("ID_WebMenuControlTypes = 17")
                '        Dim filter As String = IsNull(dr("Filter"), ""),
                '        dataSource As String = String.Empty,
                '        menu As GSWEB.MenuCollection.Menu = mCollection.GetMenu(dr("ID_TargetWebMenus")), lookupCols As New List(Of String)
                '        If Not IsNothing(menu) Then
                '            dataSource = IsNull(menu.ColumnValue("DataSource"), dr("TableName").ToString)

                '            If IsNull(dataSource, "") <> "" Or Not (dataSource.Contains(":")) Or Not (dataSource.Contains("@")) Then
                '                For Each lookuprows As DataRow In menu.dtColumns.Select()
                '                    If IsNull(lookuprows("Label"), "") = "" Then
                '                        lookupCols.Add(lookuprows("Name"))
                '                    Else
                '                        lookupCols.Add(lookuprows("Name") & " as " & lookuprows("Label"))
                '                    End If
                '                Next
                '                If filter = "" Then
                '                    strQueryBuilder.Append("SELECT " & String.Join(",", lookupCols) & " FROM " & dataSource & " ORDER BY " & menu.ColumnValue("SortBy") & ";")
                '                Else
                '                    strQueryBuilder.Append("SELECT " & String.Join(",", lookupCols) & " FROM " & dataSource & " WHERE " & SetFilter(menu, filter, pID, rID, HttpContext.Current.Session) & " ORDER BY " & menu.ColumnValue("SortBy") & ";")
                '                End If
                '            End If

                '        End If
                '        ddLookup.Add(dr("ID"))
                '    Next
                'Next

                ''SQL DATA FETCH

                'ret = "{"
                'Using ttCon As New SqlClient.SqlConnection(ConnectionString)
                '    ttCon.Open()

                '    Using ttCmd As New SqlCommand(strQueryBuilder.ToString, ttCon)
                '        Dim Reader As SqlDataReader
                '        Reader = ttCmd.ExecuteReader()

                '        Dim totalServerItems As New StringBuilder,
                '            columnDefinitions As New StringBuilder,
                '            groupedColumns As New StringBuilder
                '        ret &= "gridData:{"

                '        For x As Integer = 0 To menuTemp.Count - 1
                '            If x > 0 Then Reader.NextResult()
                '            ret &= menuTemp(x) & ":" & serializeNoClose(Reader, True, HttpContext.Current.Session, mCollection.GetMenu(menuTemp(x)).dtColumns.AsEnumerable().Where(Function(c) IsNull(c.Item("ID_WebMenuControlTypes"), 0) = 27).Select(Function(c) CStr(c.Item("Name"))).ToList()) & ","
                '            Reader.NextResult()
                '            Reader.Read()
                '            totalServerItems.Append(menuTemp(x) & " : " & Reader(0).ToString & ",")
                '            columnDefinitions.Append(menuTemp(x) & " : " & columnDefs(x) & ",")
                '            If groups(x).ToString.Length > 0 Then
                '                groupedColumns.Append(menuTemp(x) & " : [" & groups(x) & "],")
                '            End If

                '        Next x

                '        ret &= "},"
                '        ret &= "totalServerItems:{ " & totalServerItems.ToString & " },"
                '        ret &= "columnDefinitions:{ " & columnDefinitions.ToString & " },"
                '        ret &= "groups: {" & groupedColumns.ToString & "},"
                '        'DROPDOWN
                '        ret &= "filter_dropdown_source:{"
                '        For x As Integer = 0 To ddTemp.Count - 1
                '            Reader.NextResult()
                '            ret &= ddTemp(x) & ":" & serializeNoClose(Reader) & ","
                '        Next
                '        ret &= "},"
                '        'AUTO COMPLETE
                '        ret &= "filter_autocomplete_source:{"
                '        For x As Integer = 0 To ddAutoCom.Count - 1
                '            Reader.NextResult()
                '            ret &= ddAutoCom(x) & ":" & serializeNoClose(Reader) & ","
                '        Next
                '        ret &= "},"
                '        'TEXT AUTO COMPLETE
                '        ret &= "filter_text_autocomplete_source:{"
                '        For x As Integer = 0 To ddTAutoCom.Count - 1
                '            Reader.NextResult()
                '            ret &= ddTAutoCom(x) & ":" & JavascriptArrayNoClose(Reader) & ","
                '        Next
                '        ret &= "},"
                '        'LOOKUP
                '        ret &= "filter_lookup_source:{"
                '        For x As Integer = 0 To ddLookup.Count - 1
                '            Reader.NextResult()
                '            ret &= ddLookup(x) & ":" & serializeNoClose(Reader) & ","
                '        Next
                '        ret &= "},"

                '        Reader.Close()
                '    End Using
                '    ttCon.Close()
                'End Using
                '------------------

            Catch ex As Exception
                logError(ex)
            End Try
            ' ret &= "}"

            Return ret
        End Function

        Public Function generateGridDataSource(m As GSWEB.MenuCollection.Menu, rID As Integer, Session As System.Web.SessionState.HttpSessionState) As String
            Dim ret As String = String.Empty
            Dim tasks As Task(Of String)() = New Task(Of String)(mCollection.GetChild(m.MenuID).Count - 1) {}
            Dim totalServerItems As New StringBuilder
            Dim act As Func(Of Object, String) = Function(menu As GSWEB.MenuCollection.Menu)
                                                     Dim r As String = String.Empty
                                                     Dim dataSource As String = SetDataSourceFilter(menu, 0, rID, Session)
                                                     Dim query As String = String.Empty
                                                     If menu.ColumnValue("WithPagination") Then
                                                         query = "SELECT " & menu.getListColumns & " FROM " & dataSource & " ORDER BY " & IsNull(menu.ColumnValue("SortBy"), "ID DESC") & "  OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY"
                                                     Else
                                                         query = "SELECT " & menu.getListColumns & " FROM " & dataSource & " ORDER BY " & IsNull(menu.ColumnValue("SortBy"), "ID DESC")
                                                     End If
                                                     totalServerItems.Append(menu.MenuID & " : " & ExecScalarNoParams("SELECT COUNT(ID) FROM " & dataSource) & ",")
                                                     Return menu.MenuID & ":" & getJSONTable(query) & ","
                                                 End Function
            Dim ctr As Integer = 0
            For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID)
                tasks(ctr) = Task(Of String).Factory.StartNew(act, m2)
                ctr += 1
            Next
            Task.WaitAll(tasks)
            ret &= "gridData:{"
            For i As Integer = 0 To mCollection.GetChild(m.MenuID).Count - 1
                ret &= tasks(i).Result
            Next
            ret &= "},"
            ret &= "totalServerItems:{ " & totalServerItems.ToString & " },"
            Return ret
        End Function

        Public Function generateMasterDataSource(m As GSWEB.MenuCollection.Menu, rID As Integer, pID As Integer, Session As System.Web.SessionState.HttpSessionState) As String
            If rID = 0 Then
                Return dictionaryToJSON(m.getDefaultFormData(), pID, Session, m.TableName)
            Else
                Return getJSONTableRow("SELECT * FROM " + SetDataSourceFilter(m, 0, pID, Session) + " WHERE ID = " + rID.ToString)
            End If
        End Function

        Public Function generateDetailDataSource(m As GSWEB.MenuCollection.Menu, rID As Integer, Session As System.Web.SessionState.HttpSessionState) As String
            Dim ret As String = String.Empty
            'FOR TESTING
            'UNG MARAMING DETAILS AT RECORDS SA DETAIL
            Dim tasks As Task(Of String)() = New Task(Of String)(mCollection.GetChild(m.MenuID).Count - 1) {}
            Dim totalServerItems As New StringBuilder
            Dim act As Func(Of Object, String) = Function(menu As GSWEB.MenuCollection.Menu)
                                                     Return menu.MenuID & ":" & getJSONTable("SELECT " & menu.getListColumns & " FROM " + SetDataSourceFilter(menu, 0, rID, Session)) & ","
                                                 End Function
            Dim ctr As Integer = 0
            For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID)
                tasks(ctr) = Task(Of String).Factory.StartNew(act, m2)
                ctr += 1
            Next
            Task.WaitAll(tasks)
            For i As Integer = 0 To mCollection.GetChild(m.MenuID).Count - 1
                ret &= tasks(i).Result
            Next
            'For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID)
            '    ret &= m2.MenuID & ":"
            '    ret &= getJSONTable("SELECT " & m2.getListColumns & " FROM " + SetDataSourceFilter(m2, 0, rID, Session))
            '    ret &= ","
            'Next
            Return ret
        End Function

        Public Function generateHTMLControlDataSource(m As GSWEB.MenuCollection.Menu, rID As Integer, pID As Integer, Session As System.Web.SessionState.HttpSessionState) As String
            Dim ret As String = String.Empty,
                mDropDown As String = String.Empty,
                tmDropDown As New Task(Sub()
                                           mDropDown = generateControlDataSource(m, rID, pID, Session, 2, True)
                                       End Sub),
                dDropDown As String = String.Empty,
                tdDropDown As New Task(Sub()
                                           dDropDown = generateControlDetailDataSource(m, rID, pID, Session, 2)
                                       End Sub),
                mRDB As String = String.Empty,
                tRDB As New Task(Sub()
                                     mRDB = generateControlDataSource(m, rID, pID, Session, 26, True)
                                 End Sub),
                dRDB As String = String.Empty,
                tdRDB As New Task(Sub()
                                      dRDB = generateControlDetailDataSource(m, rID, pID, Session, 26)
                                  End Sub),
                mAutoComplete As String = String.Empty,
                tAutoComplete As New Task(Sub()
                                              mAutoComplete = generateControlDataSource(m, rID, pID, Session, 23, True)
                                          End Sub),
                dAutoComplete As String = String.Empty,
                tdAutoComplete As New Task(Sub()
                                               dAutoComplete = generateControlDetailDataSource(m, rID, pID, Session, 23)
                                           End Sub),
                mTextAutoComplete As String = String.Empty,
                tTextAutoComplete As New Task(Sub()
                                                  mTextAutoComplete = generateTextAutocompleteDataSource(m, rID, pID, Session, True)
                                              End Sub),
                mLookup As String = String.Empty,
                tmLookup As New Task(Sub()
                                         mLookup = generateControlDataSource(m, rID, pID, Session, 17, True)
                                     End Sub),
                dLookup As String = String.Empty,
                tdLookup As New Task(Sub()
                                         dLookup = generateControlDetailDataSource(m, rID, pID, Session, 17)
                                     End Sub)
            tmDropDown.Start()
            tdDropDown.Start()
            tRDB.Start()
            tdRDB.Start()
            tAutoComplete.Start()
            tdAutoComplete.Start()
            tTextAutoComplete.Start()
            tmLookup.Start()
            tdLookup.Start()
            Task.WaitAll(tmDropDown, tdDropDown, tRDB, tdRDB, tAutoComplete, tdAutoComplete, tTextAutoComplete, tmLookup, tdLookup)
            ret &= "dropdown_source:{" & mDropDown & dDropDown & "},"
            ret &= "rdb_source:{" & mRDB & dRDB & "},"
            ret &= "autocomplete_source:{" & mAutoComplete & dAutoComplete & "},"
            ret &= "text_autocomplete_source:{" & mTextAutoComplete & "},"
            ret &= "lookup_source:{" & mLookup & "},"
            Return ret
        End Function

        Public Function generateControlDataSource(m As GSWEB.MenuCollection.Menu, rID As Integer, pID As Integer, Session As System.Web.SessionState.HttpSessionState, ControlType As Integer, IsVisible As Boolean) As String
            Dim ret As String = String.Empty
            For Each dr As DataRow In m.dtColumns.Select(If(IsVisible, "IsVisible", "ShowInList") & " = 1 AND ID_WebMenuControlTypes = " & ControlType.ToString)
                ret &= dr("ID") & ":"
                Dim filter As String = IsNull(dr("Filter"), ""),
                    dataSource = SetFilter(m, IsNull(dr("TableName"), "t" & dr("Name").ToString.Substring(3)), pID, rID, Session)
                If filter = "" Then
                    Dim memCache As New MemoryCacher
                    If Not memCache.Contains("Control_" & dataSource) Then
                        memCache.Add("Control_" & dataSource, getJSONTable("SELECT " & IsNull(dr("DisplayID"), "ID") & "," & IsNull(dr("DisplayMember"), "Name") & " FROM " & dataSource & " ORDER BY " & IsNull(dr("DisplayMember"), "Name") & " ASC"), DateTime.Now.AddSeconds(15))
                    End If
                    ret &= memCache.GetValue("Control_" & dataSource)
                Else
                    ret &= getJSONTable("SELECT " & IsNull(dr("DisplayID"), "ID") & "," & IsNull(dr("DisplayMember"), "Name") & " FROM " & dataSource & If(filter = "", "", " WHERE " & SetFilter(m, filter, pID, rID, Session)) & " ORDER BY " & IsNull(dr("DisplayMember"), "Name") & " ASC")
                End If
                ret &= ","
            Next

            Return ret
        End Function

        Public Function generateTextAutocompleteDataSource(m As GSWEB.MenuCollection.Menu, rID As Integer, pID As Integer, Session As System.Web.SessionState.HttpSessionState, IsVisible As Boolean) As String
            Dim ret As String = String.Empty
            For Each dr As DataRow In m.dtColumns.Select(If(IsVisible, "IsVisible", "ShowInList") & " = 1 AND ID_WebMenuControlTypes = 10")
                ret &= dr("ID") & ":"
                Dim filter As String = IsNull(dr("Filter"), ""),
                    dataSource = IsNull(dr("TableName"), "t" & dr("Name").ToString)
                If filter = "" Then
                    Dim memCache As New MemoryCacher
                    If Not memCache.Contains("DD_" & dataSource) Then
                        memCache.Add("DD_" & dataSource, getJSONArray("SELECT " & IsNull(dr("DisplayMember"), "Name") & " AS Name FROM " & dataSource & " ORDER BY " & IsNull(dr("DisplayMember"), "Name") & " ASC"), DateTime.Now.AddSeconds(15))
                    End If
                    ret &= memCache.GetValue("DD_" & dataSource)
                Else
                    ret &= getJSONArray("SELECT " & IsNull(dr("DisplayMember"), "Name") & " AS Name FROM " & dataSource & IIf(filter = "", "", " WHERE " & SetFilter(m, filter, pID, rID, HttpContext.Current.Session)) & " ORDER BY " & IsNull(dr("DisplayMember"), "Name") & " ASC")
                End If
                ret &= ","
            Next

            Return ret
        End Function

        Public Function generateControlDetailDataSource(m As GSWEB.MenuCollection.Menu, rID As Integer, pID As Integer, Session As System.Web.SessionState.HttpSessionState, ControlType As Integer) As String
            Dim ret As String = String.Empty
            For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(m.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 14)
                ret &= generateControlDataSource(m2, rID, pID, Session, ControlType, False)
            Next
            Return ret
        End Function

        Public Function buildHTMLControlValidation(m As GSWEB.MenuCollection.Menu, columnID As Integer) As String
            Dim sb As New Text.StringBuilder
            sb.Append("")
            If Not m.dtColumnValidation(columnID) Is Nothing Then
                For Each dr As DataRow In m.dtColumnValidation(columnID).Rows
                    Select Case dr("ID_ColumnValidation")
                        Case 1 'minlength
                            sb.Append(" ng-minlength='" & CInt(dr("FieldCompare")).ToString & "' ")
                        Case 2 'maxlength
                            sb.Append(" ng-maxlength='" & CInt(dr("FieldCompare")).ToString & "' ")
                        Case 3 'min date
                            sb.Append(" data-min-date='" & defaultValues(dr("FieldCompare")).ToString & "' ")
                        Case 4 'max date
                            sb.Append(" data-max-date='" & defaultValues(dr("FieldCompare")).ToString & "' ")
                        Case 5 'min date
                            sb.Append(" min-time='" & defaultValues(dr("FieldCompare")).ToString & "' ")
                        Case 6 'max date
                            sb.Append(" max-time='" & defaultValues(dr("FieldCompare")).ToString & "' ")
                        Case 16 'max value
                            sb.Append(" ng-max='" & defaultValues(dr("FieldCompare")).ToString & "' ")
                        Case 17 'min value
                            sb.Append(" ng-min='" & defaultValues(dr("FieldCompare")).ToString & "' ")
                    End Select
                Next
            End If
            Return sb.ToString
        End Function

        Public Function buildHTMLDefinitionControlValidation(m As GSWEB.MenuCollection.Menu, columnID As Integer) As String
            Dim sb As New Text.StringBuilder
            sb.Append("")
            If Not m.dtColumnValidation(columnID) Is Nothing Then
                For Each dr As DataRow In m.dtColumnValidation(columnID).Rows
                    Select Case dr("ID_ColumnValidation")
                        Case 1 'minlength
                            sb.Append(" ng-minlength=\'" & CInt(dr("FieldCompare")).ToString & "\' ")
                        Case 2 'maxlength
                            sb.Append(" ng-maxlength=\'" & CInt(dr("FieldCompare")).ToString & "\' ")
                        Case 3 'min date
                            sb.Append(" data-min-date=\'" & defaultValues(dr("FieldCompare")).ToString & "\' ")
                        Case 4 'max date
                            sb.Append(" data-max-date=\'" & defaultValues(dr("FieldCompare")).ToString & "\' ")
                        Case 5 'min date
                            sb.Append(" min-time=\'" & defaultValues(dr("FieldCompare")).ToString & "\' ")
                        Case 6 'max date
                            sb.Append(" max-time=\'" & defaultValues(dr("FieldCompare")).ToString & "\' ")
                        Case 16 'max value
                            sb.Append(" ng-max=\'" & defaultValues(dr("FieldCompare")).ToString & "\' ")
                        Case 17 'min value 
                            sb.Append(" ng-min=\'" & defaultValues(dr("FieldComapre")).ToString & "\' ")
                    End Select
                Next
            End If
            Return sb.ToString
        End Function

        Public Sub logError(Exception As Exception, Optional menuID As Integer = 0, Optional rawUrl As String = "")
            Dim logFile As String = String.Empty
            Dim logWriter As StreamWriter

            Try
                logFile = HttpContext.Current.Server.MapPath("~/ErrorLog.txt")
                If (File.Exists(logFile)) Then
                    logWriter = File.AppendText(logFile)
                Else
                    logWriter = File.CreateText(logFile)
                End If
                logWriter.WriteLine("=>" + DateTime.Now + " " + " An Error occured : " +
                    Exception.StackTrace + " Message : " + Exception.Message)
                If menuID > 0 Then
                    logWriter.WriteLine("   MenuID : " & menuID & Environment.NewLine)
                Else
                    logWriter.WriteLine(Environment.NewLine)
                End If
                'logWriter.WriteLine("rawUrl : " & rawUrl)
                logWriter.Close()
                'Throw Exception

            Catch ex As Exception
                'Throw
            Finally

            End Try

        End Sub

        Public Sub LogEvent(message As String)
            Dim logFile As String = String.Empty
            Dim logWriter As StreamWriter

            Try
                logFile = HttpContext.Current.Server.MapPath("~/EventLog.txt")
                If (File.Exists(logFile)) Then
                    logWriter = File.AppendText(logFile)
                Else
                    logWriter = File.CreateText(logFile)
                End If
                logWriter.WriteLine("=> " + message)
                'logWriter.WriteLine("rawUrl : " & rawUrl)
                logWriter.Close()
                'Throw Exception

            Catch ex As Exception
                'Throw
            Finally

            End Try

        End Sub

        Public Function ShowMenuIf(source As String, Session As System.Web.SessionState.HttpSessionState) As Boolean
            Dim result As Boolean = False
            Using sqlConn As New SqlConnection(ConnectionString)
                Try
                    sqlConn.Open()
                    While source.Contains("@")
                        Dim tmpStr As String = source.Substring(source.IndexOf("@"), (source.Length) - source.IndexOf("@"))
                        If tmpStr.Contains(" ") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(" "))
                        If tmpStr.Contains(",") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(","))
                        If tmpStr.Contains(")") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(")"))
                        'source = source.Replace(tmpStr, Session.Item(tmpStr.Replace("@", "")))
                        source = Regex.Replace(source, "@\b" + tmpStr.Replace("@", "") + "\b", Session.Item(tmpStr.Replace("@", "")).ToString)
                    End While
                    Using SqlCommand As New SqlCommand("EXEC ('" & source & "')", sqlConn)
                        result = SqlCommand.ExecuteNonQuery()
                    End Using
                Catch ex As SqlException
                    'Throw
                Finally
                    sqlConn.Close()
                End Try
            End Using

            Return result
        End Function

        Public Sub generateControlQueryBuilder(ByRef strQueryBuilder As Text.StringBuilder, tmp As List(Of String), m As GSWEB.MenuCollection.Menu, rID As Integer, pID As Integer, Session As System.Web.SessionState.HttpSessionState, ControlType As Integer, IsVisible As Boolean)
            For Each dr As DataRow In m.dtColumns.Select(If(IsVisible, "IsVisible", "ShowInList") & " = 1 AND ID_WebMenuControlTypes = " & ControlType.ToString)
                Dim filter As String = IsNull(dr("Filter"), ""),
                    dataSource As String = String.Empty,
                    menu As GSWEB.MenuCollection.Menu = Nothing, lookupCols As New List(Of String)
                If ControlType = 17 Or ControlType = 35 Then
                    menu = mCollection.GetMenu(dr("ID_TargetWebMenus"))
                    If Not IsNothing(menu) Then
                        dataSource = IsNull(menu.ColumnValue("DataSource"), dr("TableName").ToString)

                        If IsNull(dataSource, "") <> "" Then
                            For Each lookuprows As DataRow In menu.dtColumns.Select("ID_WebMenuControlTypes IS NOT NULL")
                                'lookupCols.Add(lookuprows("Name"))
                                If IsNull(lookuprows("Label"), "") = "" Then
                                    lookupCols.Add("[" & lookuprows("Name") & "]")
                                ElseIf IsNull(lookuprows("Label"), "") <> "" And lookuprows("Name").ToString.ToLower <> "id" And lookuprows("Name").ToString.ToLower.StartsWith("id_") Then
                                    lookupCols.Add("[" & lookuprows("Name").ToString.Substring(3) & "]" & " as [" & lookuprows("Label") & "]")
                                ElseIf IsNull(lookuprows("Label"), "") <> "" And lookuprows("Name").ToString.ToLower <> "id" And Not (lookuprows("Name").ToString.ToLower.StartsWith("id_")) Then
                                    lookupCols.Add("[" & lookuprows("Name") & "]" & " as [" & lookuprows("Label") & "]")
                                Else
                                    lookupCols.Add("[" & lookuprows("Name") & "]")
                                End If
                            Next
                            If filter = "" Then
                                strQueryBuilder.Append("SELECT " & String.Join(",", lookupCols) & " FROM " & SetFilter(menu, dataSource, pID, rID, HttpContext.Current.Session) & IIf(IsNull(menu.ColumnValue("SortBy"), "") <> "", " ORDER BY ID ASC ", "") & ";")
                            Else
                                If ControlType = 35 Then
                                    If IsNull(dr("TableName"), "") <> "" Then
                                        strQueryBuilder.Append("SELECT " & String.Join(",", lookupCols) & " FROM " & SetFilter(menu, dataSource, pID, rID, HttpContext.Current.Session) & IIf(IsNull(menu.ColumnValue("SortBy"), "") <> "", " ORDER BY " & menu.ColumnValue("SortBy"), "") & ";")
                                    End If
                                Else
                                    strQueryBuilder.Append("SELECT " & String.Join(",", lookupCols) & " FROM " & SetFilter(menu, dataSource, pID, rID, HttpContext.Current.Session) & " WHERE " & SetFilter(menu, filter, pID, rID, HttpContext.Current.Session) & IIf(IsNull(menu.ColumnValue("SortBy"), "") <> "", " ORDER BY ID ASC ", "") & ";")
                                End If
                            End If
                        End If

                    End If
                Else
                    dataSource = IsNull(dr("TableName"), "t" & dr("Name").ToString.Substring(3))
                    If ControlType = 10 Then
                        If filter = "" Then
                            strQueryBuilder.Append("SELECT " & IsNull(dr("DisplayMember"), "Name") & " AS Name FROM " & dataSource & " ORDER BY " & IsNull(dr("DisplayMember"), "Name") & " ASC" & ";")
                        Else
                            strQueryBuilder.Append("SELECT " & IsNull(dr("DisplayMember"), "Name") & " AS Name FROM " & dataSource & " WHERE " & SetFilter(m, filter, pID, rID, HttpContext.Current.Session) & IIf(IsNull(dr("DisplayMember"), "Name") <> "", " ORDER BY " & IsNull(dr("DisplayMember"), "Name"), "") & ";")
                        End If
                    Else
                        If filter = "" Then
                            strQueryBuilder.Append("SELECT " & IsNull(dr("DisplayID"), "ID") & " AS ID," & IsNull(dr("DisplayMember"), "Name") & " AS Name " & IIf(IsNull(dr("AdditionalColumns"), "") <> "", ", " & dr("AdditionalColumns") & "", "") & " FROM " & SetFilter(m, dataSource, pID, rID, HttpContext.Current.Session) & IIf(IsNull(dr("DisplayMember"), "Name") <> "", " ORDER BY " & IsNull(dr("DisplayMember"), "Name"), "") & ";")
                        Else
                            strQueryBuilder.Append("SELECT " & IsNull(dr("DisplayID"), "ID") & " AS ID," & IsNull(dr("DisplayMember"), "Name") & " AS Name " & IIf(IsNull(dr("AdditionalColumns"), "") <> "", ", " & dr("AdditionalColumns") & "", "") & " FROM " & SetFilter(m, dataSource, pID, rID, HttpContext.Current.Session) & " WHERE " & SetFilter(m, filter, pID, rID, HttpContext.Current.Session) & IIf(IsNull(dr("DisplayMember"), "Name") <> "", " ORDER BY " & IsNull(dr("DisplayMember"), "Name"), "") & ";")
                        End If
                    End If

                End If

                tmp.Add(dr("ID"))
            Next
        End Sub

        Public Sub generateFilterControlQueryBuilder(ByRef strQueryBuilder As Text.StringBuilder, tmp As List(Of String), m As GSWEB.MenuCollection.Menu, rID As Integer, pID As Integer, Session As System.Web.SessionState.HttpSessionState, ControlType As Integer)
            For Each dr As DataRow In m.dtColumns.Select("ID_FilterWebMenuControlTypes = " & ControlType.ToString)
                Dim filter As String = IsNull(dr("Filter"), ""),
                    dataSource As String = IsNull(dr("TableName"), "t" & dr("Name").ToString.Substring(3))
                If ControlType = 10 Then
                    If filter = "" OrElse filter.Contains(":") Then
                        strQueryBuilder.Append("SELECT " & IsNull(dr("DisplayMember"), "Name") & " AS Name FROM " & SetFilter(m, dataSource, pID, rID, HttpContext.Current.Session) & " ORDER BY " & IsNull(dr("DisplayMember"), "Name") & " ASC" & ";")
                    Else
                        strQueryBuilder.Append("SELECT " & IsNull(dr("DisplayMember"), "Name") & " AS Name FROM " & SetFilter(m, dataSource, pID, rID, HttpContext.Current.Session) & " WHERE " & SetFilter(m, filter, pID, rID, HttpContext.Current.Session) & " ORDER BY " & IsNull(dr("DisplayMember"), "Name") & " ASC" & ";")
                    End If

                Else
                    If filter = "" OrElse filter.Contains(":") Then
                        strQueryBuilder.Append("SELECT " & IsNull(dr("DisplayID"), "ID") & " AS ID," & IsNull(dr("DisplayMember"), "Name") & " AS Name " & IIf(IsNull(dr("AdditionalColumns"), "") <> "", ", " & dr("AdditionalColumns") & "", "") & " FROM " & SetFilter(m, dataSource, pID, rID, HttpContext.Current.Session) & IIf(IsNull(dr("DisplayMember"), "Name") <> "", " ORDER BY " & IsNull(dr("DisplayMember"), "Name"), "") & ";")
                    Else
                        strQueryBuilder.Append("SELECT " & IsNull(dr("DisplayID"), "ID") & " AS ID," & IsNull(dr("DisplayMember"), "Name") & " AS Name " & IIf(IsNull(dr("AdditionalColumns"), "") <> "", ", " & dr("AdditionalColumns") & "", "") & " FROM " & SetFilter(m, dataSource, pID, rID, HttpContext.Current.Session) & " WHERE " & SetFilter(m, filter, pID, rID, HttpContext.Current.Session) & IIf(IsNull(dr("DisplayMember"), "Name") <> "", " ORDER BY " & IsNull(dr("DisplayMember"), "Name"), "") & ";")
                    End If

                End If

                tmp.Add(dr("ID"))
            Next
        End Sub

        Public Sub RegisterBundles(bundles As System.Web.Optimization.BundleCollection)

            bundles.Add(New ScriptBundle("~/bundles/core-scripts").Include(
                "~/Scripts/jquery-1.9.1.min.js",
                "~/Scripts/jquery-ui.min.js",
                "~/Scripts/underscore.js",
                "~/Scripts/angular.min.1.2.22.js",
                "~/Scripts/angular-sanitize.js",
                "~/Scripts/angular-ui-router.js",
                "~/Scripts/ct-ui-router-extras.js",
                "~/Scripts/moment.js",
                "~/Scripts/moment-locale.js",
                "~/Scripts/jquery.signalR-1.0.0.js",
                "~/Scripts/MultiLevelMenu.Codrops.js",
                "~/Scripts/classie.js",
                "~/Scripts/modernizr-custom.js"))
            '"~/Scripts/react.min.js",

            bundles.Add(New ScriptBundle("~/bundles/plugin-scripts").Include(
                "~/Scripts/ui-bootstrap-tpls.min.js",
                "~/Scripts/jquery.overscroll.min.js",
                "~/Scripts/dialogs.js",
                "~/Scripts/loading-bar.js",
                "~/Scripts/angular-file-upload-shim.js",
                "~/Scripts/angular-file-upload.min.js",
                "~/Scripts/angular-filter.min.js",
                "~/Scripts/angular-cookies.min.js",
                "~/Scripts/scalyr.js",
                "~/Scripts/chart.min.js",
                "~/Scripts/angular-chart.min.js",
                "~/Scripts/redactoboor.min.js",
                "~/Scripts/angular-redactor.min.js",
                "~/Scripts/growl-notifications.js",
                "~/Scripts/angular-strap.min.js",
                "~/Scripts/scrollglue.min.js",
                "~/Scripts/jquery.livequery.min.js",
                "~/Scripts/bootstrap-colorpicker.min.js",
                "~/Scripts/hotkeys.min.js",
                "~/Scripts/kc.fab.js",
                "~/Scripts/contextMenu.js",
                "~/Scripts/bootstrap-datetimepicker.js"))

            bundles.Add(New ScriptBundle("~/bundles/custom-scripts").Include(
                "~/Scripts/ng-tree-grid.min.js",
                "~/Scripts/angular-grid.js",
                "~/Scripts/angular-file-explorer.js",
                "~/Scripts/angular-calendar.js",
                "~/Scripts/angular-scrollbar.js",
                "~/Scripts/angular-treeview.js"))

            bundles.Add(New StyleBundle("~/Styles/System/system-css").Include(
                 "~/Styles/System/bootstrap.3.2.min.css",
                 "~/Styles/System/font-awesome.min.css",
                 "~/Styles/System/angular-chart.css",
                 "~/Styles/System/colorpicker.min.css",
                 "~/Styles/System/loading-bar.css",
                 "~/Styles/System/redactor.css",
                 "~/Styles/System/angular-grid.css",
                 "~/Styles/System/angular-bootstrap-calendar.min.css",
                 "~/Styles/System/angular-file-explorer.css",
                 "~/Styles/System/angular-scrollbar.css",
                 "~/Styles/System/angular-treeview.css",
                 "~/Styles/System/kc.fab.css",
                 "~/Styles/System/bootstrap-datetimepicker.css",
                 "~/Styles/System/component.css"))

            bundles.Add(New StyleBundle("~/Styles/System/additional-css").Include(
                "~/Styles/Default/default.css",
                "~/Styles/Default/additional.css"))

            ''LOGIN BUNDLES

            bundles.Add(New StyleBundle("~/Styles/System/login-css").Include(
                "~/Styles/System/bootstrap.3.2.min.css",
                "~/Styles/System/font-awesome.min.css",
                "~/Styles/Default/default.css",
                "~/Styles/Default/additional.css",
                "~/Styles/System/loading-bar.css"))

            bundles.Add(New ScriptBundle("~/bundles/login-script").Include(
                "~/Scripts/angular.min.1.2.22.js",
                "~/Scripts/angular-sanitize.js",
                "~/Scripts/ui-bootstrap-tpls.js",
                "~/Scripts/loading-bar.js",
                "~/Scripts/dialogs.js"))

            BundleTable.EnableOptimizations = True
        End Sub

        Public Function Encrypt(ByVal vData As String, ByVal vKey As Integer) As String
            Dim result As String = ""
            Dim i As Integer
            For Each c As Char In vData
                i = Asc(c)
                c = Chr((i Xor vKey))
                result &= c
            Next
            Return result
        End Function

        Public Function unEscape(s As String) As String
            Return HttpUtility.UrlDecode(s.Replace("+", "%2b"), System.Text.Encoding.Default)
        End Function

        Public Function SetMenuFilter(ByVal m As GSWEB.MenuCollection.Menu, Optional ByVal buttonID As Integer = 0, Optional ByVal ParentID As Integer = 0, Optional ByVal SessionVariables As System.Web.SessionState.HttpSessionState = Nothing) As String
            Dim viewFilter As String = ""
            If (m.ColumnValue("ViewFilter").ToString = "" Or m.ColumnValue("DataSource").ToString = "") AndAlso buttonID = 0 Then Return viewFilter
            viewFilter = m.ColumnValue("ViewFilter").ToString
            If buttonID > 0 Then
                viewFilter = m.dtButtons.Select("ID = " + buttonID.ToString)(0)("CommandText").ToString
            End If
            Dim str As String = " WHERE " + viewFilter
            Dim parent As GSWEB.MenuCollection.Menu = mCollection.GetMenu(m.ParentID)
            While str.Contains("#")
                Dim tmpStr As String = str.Substring(str.IndexOf("#"), (str.Length) - str.IndexOf("#"))
                If tmpStr.Contains(" ") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(" "))
                If tmpStr.Contains(",") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(","))
                If tmpStr.Contains(")") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(")"))
                If ParentID > 0 Then
                    'str = str.Replace(tmpStr, ExecScalarNoParams("SELECT " & tmpStr.Replace("#", "") & " FROM " & parent.ColumnValue("DataSource") & " WHERE ID = " & ParentID).ToString)
                    str = Regex.Replace(str, "#\b" + tmpStr.Replace("#", "") + "\b", ExecScalarNoParams("SELECT " & tmpStr.Replace("#", "") & " FROM " & parent.ColumnValue("DataSource") & " WHERE ID = " & ParentID).ToString)
                Else
                    'str = str.Replace(tmpStr, "''")
                    str = Regex.Replace(str, "#\b" + tmpStr.Replace("#", "") + "\b", "''")
                End If
            End While
            While str.Contains("@")
                Dim tmpStr As String = str.Substring(str.IndexOf("@"), (str.Length) - str.IndexOf("@"))
                If tmpStr.Contains(" ") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(" "))
                If tmpStr.Contains(",") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(","))
                If tmpStr.Contains(")") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(")"))
                'str = str.Replace(tmpStr, SessionVariables.Item(tmpStr.Replace("@", "")))
                str = Regex.Replace(str, "@\b" + tmpStr.Replace("@", "") + "\b", SessionVariables.Item(tmpStr.Replace("@", "")).ToString)
            End While
            Return str
        End Function

        Public Function replaceWithSession(query As String, SessionVariables As System.Web.SessionState.HttpSessionState) As String
            While query.Contains("@")
                For Each ss As Object In SessionVariables.Keys
                    'query = query.Replace("@" + ss.ToString, SessionVariables.Item(ss.ToString).ToString)
                    query = Regex.Replace(query, "@\b" + ss.ToString + "\b", SessionVariables.Item(ss.ToString).ToString)
                    If Not query.Contains("@") Then Exit For
                Next
            End While
            Return query
        End Function

        Public Sub LoadSubDataSource(ByVal menuID As Integer, Optional ByVal cnt As Integer = 0)
            Try
                If cnt = 0 Then
                    Dim dtSubDataSource As DataTable = getTable("SELECT ID,Name,DataSource,ID_WebMenus,SeqNo FROM vWebMenusSubDatasource WHERE IsActive = 1 AND ID_WebMenus = " + menuID.ToString)
                    For Each dr As DataRow In dtSubDataSource.Rows
                        menuSubDataSourceCollection.Add(dr)
                    Next
                End If
            Catch ex As Exception
                Throw ex
            End Try
        End Sub

        Public Function HTMLCompresss(s As String)
            Dim cssCompressor As New CssCompressor
            s = Regex.Replace(s, "^\s*", String.Empty, RegexOptions.Compiled Or RegexOptions.Multiline)
            s = Regex.Replace(s, "\r\n", String.Empty, RegexOptions.Compiled Or RegexOptions.Multiline)
            s = Regex.Replace(s, "<!--*.*?-->", String.Empty, RegexOptions.Compiled Or RegexOptions.Multiline)
            s = Regex.Replace(s, "/\**.*?\*/", String.Empty, RegexOptions.Compiled Or RegexOptions.Multiline)
            If s.IndexOf("<style>") > 0 Then
                Dim x As String = s.Substring(s.IndexOf("<style>") + 7, s.IndexOf("</style>") - (s.IndexOf("<style>") + 7))
                s = Regex.Replace(s, "<style>*.*?<\/style>", "<style>" & cssCompressor.Compress(x) & "</style>", RegexOptions.Compiled Or RegexOptions.Multiline)
            End If
            Return s
        End Function

        Public Sub CompressScript()
            Dim jCompressor As New JavaScriptCompressor,
                SystemVersion As String = ExecScalarNoParams("SELECT dbo.fGetWebParameter('SystemVersion')")
            Dim controllerFolders As String() = {"Partials/Controller", "CustomPartials/Controller"}

            For Each folder In controllerFolders
                Dim temp As String = HttpContext.Current.Server.MapPath("~/Build/" & SystemVersion & "/" & folder)
                If (Not System.IO.Directory.Exists(temp)) Then
                    System.IO.Directory.CreateDirectory(temp)
                End If

                Dim filePath As String = HttpContext.Current.Server.MapPath("~/" & folder)
                For Each file In Directory.GetFiles(filePath, "*.js")
                    Try
                        Using sr As New StreamReader(file)
                            Dim line As String = sr.ReadToEnd
                            Writer(HttpContext.Current.Server.MapPath("~/Build/" & SystemVersion & "/" & folder & "/" & Path.GetFileNameWithoutExtension(file) & ".js"), jCompressor.Compress(line))
                        End Using
                    Catch ex As Exception
                        logError(ex)
                    End Try

                Next
            Next
        End Sub

        Public Sub CompressHTMLPages()
            Dim viewFolders As String() = {"Partials/View", "CustomPartials/View"},
                SystemVersion As String = ExecScalarNoParams("SELECT dbo.fGetWebParameter('SystemVersion')")
            For Each folder In viewFolders
                Dim temp As String = HttpContext.Current.Server.MapPath("~/Build/" & SystemVersion & "/" & folder)
                If (Not System.IO.Directory.Exists(temp)) Then
                    System.IO.Directory.CreateDirectory(temp)
                End If
                Dim filePath As String = HttpContext.Current.Server.MapPath("~/" & folder)
                For Each file In Directory.GetFiles(filePath, "*.html")
                    Try
                        Using sr As New StreamReader(file)
                            Dim line As String = sr.ReadToEnd
                            Writer(HttpContext.Current.Server.MapPath("~/Build/" & SystemVersion & "/" & folder & "/" & Path.GetFileNameWithoutExtension(file) & ".html"), HTMLCompresss(line))
                        End Using
                    Catch ex As Exception
                        logError(ex)
                    End Try

                Next
            Next
        End Sub

        Public Sub CompressApplicationScripts()
            Dim jCompressor As New JavaScriptCompressor,
                SystemVersion As String = ExecScalarNoParams("SELECT dbo.fGetWebParameter('SystemVersion')")
            Dim jsFiles As String() = {"app", "customroute", "controllers", "directives", "dataservices", "utilservices", "filter", "template", "menu"}
            For Each jsFile In jsFiles
                Dim filePath As String = HttpContext.Current.Server.MapPath("~/Scripts/" & jsFile & ".js")
                Try
                    Using sr As New StreamReader(filePath)
                        Dim line As String = sr.ReadToEnd
                        Writer(HttpContext.Current.Server.MapPath("~/Build/" & SystemVersion & "/" & jsFile & ".js"), jCompressor.Compress(line))
                    End Using
                Catch ex As Exception
                    logError(ex)
                End Try
            Next
        End Sub

        Public Sub getDetailValueLookup(ByRef strQueryBuilder As Text.StringBuilder, tmp As List(Of String), m As GSWEB.MenuCollection.Menu, rID As Integer, pID As Integer, Session As System.Web.SessionState.HttpSessionState, ControlType As Integer, IsVisible As Boolean)
            For Each dr As DataRow In m.dtColumns.Select(If(IsVisible, "IsVisible", "ShowInList") & " = 1 AND ID_WebMenuControlTypes = " & ControlType.ToString)
                Dim filter As String = IsNull(dr("Filter"), ""),
                    dataSource As String = dr("TableName"),
                    menu As GSWEB.MenuCollection.Menu = mCollection.GetMenu(dr("ID_TargetWebMenus")), lookupCols As New List(Of String)

                If IsNull(dataSource, "") <> "" Then
                    strQueryBuilder.Append("SELECT " & dr("Name") & " as ID FROM " & dataSource & " " & IIf(IsNull(filter, "") <> "", "WHERE " & SetFilter(menu, filter, pID, rID, HttpContext.Current.Session), "") & " ")
                End If

                tmp.Add(dr("ID"))
            Next
        End Sub

        Public Sub CreateTheme(rID As Integer)
            Dim filePath As String = HttpContext.Current.Server.MapPath("~/Styles/Skins/default.scss")
            Using sr As New StreamReader(filePath)
                Dim line As String = sr.ReadToEnd
                line = Regex.Replace(line, "^\s*", String.Empty, RegexOptions.Compiled Or RegexOptions.Multiline)
                line = Regex.Replace(line, "\r\n", String.Empty, RegexOptions.Compiled Or RegexOptions.Multiline)
                line = Regex.Replace(line, "<!--*.*?-->", String.Empty, RegexOptions.Compiled Or RegexOptions.Multiline)
                line = Regex.Replace(line, "/\**.*?\*/", String.Empty, RegexOptions.Compiled Or RegexOptions.Multiline)
                Dim dr As DataRow = getTable("SELECT * FROM tSkins WHERE ID = " & rID)(0)
                ' Name,mainColor,borderBottomColor,buttonFontColor, tableRowEvenColor, tableAggregateColor, menuFontColor,FontFamily
                For Each column In dr.Table.Columns
                    If column.ToString = "Name" Then
                        line = Regex.Replace(line, "#{\$" & column.ToString & "}", dr.Item(column).ToString, RegexOptions.Compiled Or RegexOptions.Multiline)
                    Else
                        line = Regex.Replace(line, "\$" & column.ToString, dr.Item(column).ToString, RegexOptions.Compiled Or RegexOptions.Multiline)

                    End If
                Next

                'Dim color As New GSWEB.Utility.Color(dr("mainColor"))
                'line = Regex.Replace(line, "\$tableRowEvenColor", color.lighten(60), RegexOptions.Compiled Or RegexOptions.Multiline)
                'line = Regex.Replace(line, "\$tableAggregateColor", color.lighten(50), RegexOptions.Compiled Or RegexOptions.Multiline)
                'line = Regex.Replace(line, "\$menuFontColor", "#fff", RegexOptions.Compiled Or RegexOptions.Multiline)
                Dim cssCompressor As New CssCompressor
                Writer(HttpContext.Current.Server.MapPath("~/Styles/Skins/" & dr("Name").ToString.Replace(" ", "-") & ".css"), cssCompressor.Compress(line))

            End Using
        End Sub

        Public Function GetDirectReference(indirectReference As String, Session As System.Web.SessionState.HttpSessionState)
            Return DirectCast(Session("ReferenceMap"), Dictionary(Of String, String))(indirectReference)
        End Function

        Public Function GetIndirectReference(s As Integer, Session As System.Web.SessionState.HttpSessionState) As String
            Return CreateOrAddMapping(CStr(s), Session)
        End Function

        Public Function CreateOrAddMapping(directReference As String, Session As System.Web.SessionState.HttpSessionState)
            Dim map As Dictionary(Of String, String) = If(DirectCast(Session("ReferenceMap"), Dictionary(Of String, String)), New Dictionary(Of String, String)())
            If map.ContainsKey(directReference) Then Return map(directReference)

            Dim indirectReference As String = GetUrlSaveValue()
            map.Add(directReference, indirectReference)
            map.Add(indirectReference, directReference)
            Session.Item("ReferenceMap") = map

            Return indirectReference
        End Function

        Public Function GetUrlSaveValue() As String
            Dim csprng = New System.Security.Cryptography.RNGCryptoServiceProvider()
            Dim buffer = New [Byte](16) {}
            csprng.GetBytes(buffer)

            Return HttpServerUtility.UrlTokenEncode(buffer)
        End Function

        Public Sub ExecuteButtonValidation(ByVal m As GSWEB.MenuCollection.Menu, ByVal ValidationType As Integer, ByVal btnID As Integer, ByVal drMaster As CaseInsensitiveDictionary, ByVal rID As Integer, ByVal Session As System.Web.SessionState.HttpSessionState)
            If Not m.dtButtonValidations(btnID) Is Nothing Then
                For Each tdr As DataRow In m.dtButtonValidations(btnID).Select("ID_WebMenuButton_Validation_Type = " & ValidationType & " AND ID_ValidationON = 1")
                    If btnValidation(tdr("CommandText").ToString, drMaster, rID, Session) Then
                        Throw New Exception("<p>" & IsNull(tdr("ValidationMessage"), "Saving has been terminated") & "</p>")
                    End If
                Next
            End If
        End Sub

        Public Sub ResizeImage(path As String, output As String, targetWidth As Integer)
            Using image = System.Drawing.Image.FromFile(path)
                Dim nPercent As Decimal = 1.0
                If image.Width > targetWidth Then
                    nPercent = (image.Width / targetWidth)
                End If
                Dim width As Integer = image.Width / nPercent,
                    height As Integer = image.Height / nPercent
                Dim myCallback As System.Drawing.Image.GetThumbnailImageAbort = New System.Drawing.Image.GetThumbnailImageAbort(AddressOf ThumbnailCallback)
                Dim myThumbnail As System.Drawing.Image = image.GetThumbnailImage(width, height, myCallback, IntPtr.Zero)

                myThumbnail.Save(output)
            End Using
            'Using image = System.Drawing.Image.FromFile(path)
            '    Dim nPercent As Decimal = 1.0
            '    If image.Width > 192 Then
            '        nPercent = (image.Width / 192.0)
            '    End If
            '    Dim width As Integer = image.Width / nPercent,
            '        height As Integer = image.Height / nPercent
            '    Using thumbnail = New System.Drawing.Bitmap(width, height)
            '        Using graph = System.Drawing.Graphics.FromImage(thumbnail)
            '            graph.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic
            '            graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality
            '            graph.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality
            '            graph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality

            '            graph.DrawImage(image, 0, 0, width, height)

            '            Dim info As System.Drawing.Imaging.ImageCodecInfo() = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders()
            '            Dim encoderParameters As System.Drawing.Imaging.EncoderParameters = New System.Drawing.Imaging.EncoderParameters(1)
            '            encoderParameters.Param(0) = New System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L)

            '            Select Case System.IO.Path.GetExtension(path).Substring(1).ToLower
            '                Case "bmp"
            '                    thumbnail.Save(output, info(0), encoderParameters)
            '                Case "jpg", "jpeg"
            '                    thumbnail.Save(output, info(1), encoderParameters)
            '                Case "gif"
            '                    thumbnail.Save(output, info(2), encoderParameters)
            '                Case "tiff"
            '                    thumbnail.Save(output, info(3), encoderParameters)
            '                Case Else
            '                    thumbnail.Save(output, info(4), encoderParameters)
            '            End Select

            '        End Using
            '    End Using
            'End Using

        End Sub

        Public Function ThumbnailCallback() As Boolean
            Return False
        End Function

        Public Function getSystemMessage(message As String) As String
            Dim str As String = ""
            Dim tmpStr As String = ""
            If message.Contains("IX_") Then
                tmpStr = message.Substring(message.IndexOf("IX_"), (message.Length) - message.IndexOf("IX_"))
            ElseIf message.Contains("FK_") Then
                tmpStr = message.Substring(message.IndexOf("FK_"), (message.Length) - message.IndexOf("FK_"))
            ElseIf message.Contains("CK_") Then
                tmpStr = message.Substring(message.IndexOf("CK_"), (message.Length) - message.IndexOf("CK_"))
            End If

            If tmpStr.Contains("'") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf("'"))
            If tmpStr.Contains("""") Then tmpStr = tmpStr.Substring(0, tmpStr.IndexOf(""""))

            Try
                str = getTable("SELECT Description FROM tSystemMessage WHERE NAME  = '" & tmpStr & "'").Rows(0)(0).ToString
            Catch ex As Exception
                str = message
            End Try

            Return str
        End Function

        Public Function toAnyBase(num As Integer, Optional baseNum As Integer = 62) As String
            num += secretKey
            Dim ret As String = String.Empty
            Dim remainder As Integer = 0
            While num >= baseNum
                remainder = num Mod baseNum
                ret += bases(remainder).ToString()
                num = Math.Floor(num / baseNum)
            End While

            ret += bases(num).ToString
            Return StringReverse(ret)
        End Function

        Public Function toBase10(s As String, Optional baseNum As Integer = 62) As Integer
            Dim tmp As Integer = 0
            Dim tmpsamp As Integer = 0
            s = StringReverse(s)
            For i As Integer = 0 To s.Length - 1
                tmpsamp = bases.IndexOf(s(i))
                tmp += Math.Pow(baseNum, i) * bases.IndexOf(s(i))
            Next
            Return tmp - secretKey
        End Function

        Public Function StringReverse(r As String) As String
            Dim arr As Char() = r.ToCharArray()
            Array.Reverse(arr)
            Return New String(arr)
        End Function

        Public Async Function getJSONTableAsync(cmdtxt As String, Optional EncryptID As Boolean = False) As Task(Of String)
            Dim ret As String = String.Empty
            Try
                Using sqlConn As New SqlClient.SqlConnection(ConnectionString)
                    Await sqlConn.OpenAsync()
                    Using sqlCmd As New SqlCommand(cmdtxt, sqlConn)
                        Using reader As SqlDataReader = Await sqlCmd.ExecuteReaderAsync()
                            Dim sw As New StringWriter()
                            Try
                                Using jsonWriter As JsonWriter = New JsonTextWriter(sw)
                                    jsonWriter.WriteStartArray()
                                    While Await reader.ReadAsync()
                                        jsonWriter.WriteStartObject()
                                        For i As Integer = 0 To reader.FieldCount - 1
                                            jsonWriter.WritePropertyName(reader.GetName(i))
                                            jsonWriter.WriteValue(reader(i))
                                        Next
                                        If EncryptID Then
                                            jsonWriter.WritePropertyName("$$rID")
                                            jsonWriter.WriteValue(toAnyBase(reader.Item("ID")))
                                        End If
                                        jsonWriter.WriteEndObject()
                                    End While
                                    jsonWriter.WriteEndArray()
                                    jsonWriter.Close()
                                End Using

                            Catch ex As Exception
                                logError(ex)
                            Finally
                                reader.Close()
                                ret = sw.ToString()
                            End Try
                        End Using
                    End Using
                    sqlConn.Close()
                End Using
            Catch ex As Exception
                Throw ex
            End Try

            Return ret
        End Function

        Public Function encryptData(data As String) As String
            Dim ret As String
            Dim keys As String = "QAZ098***"

            data = data & keys
            Dim dataBytes As Byte() = System.Text.Encoding.Unicode.GetBytes(data)
            ret = Convert.ToBase64String(dataBytes)

            Return ret
        End Function

        Public Function decryptData(data As String) As String
            Dim ret As String
            Dim keys As String = "QAZ098***"

            Dim encryptedBytes() As Byte = Convert.FromBase64String(data)
            Dim origData As String = System.Text.Encoding.Unicode.GetString(encryptedBytes)
            origData = origData.Replace(keys, "")

            ret = origData

            Return ret
        End Function


    End Module


    Public Enum ngModel
        Master = 1
        Filter = 2
    End Enum

End Namespace