Imports System.Web
Imports GSWEB.Common
Imports System.Data
Imports System.Data.DataTableExtensions
Imports System.IO
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports System.Linq
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Formatting
Imports System.Text.RegularExpressions

Namespace MenuCollection

    Public Class Menu
        Implements IEquatable(Of Menu)

#Region "Declarations"
        Private mRow As DataRow
        Private mConnection As New GSWEB.SQL.DataSource
        Private mParameters As DataTable
        Private dView As DataView
        Private sFilter As String = ""
        Private mButtons As New DataTable
        Private mButtonValidations As New DataTable
        Private SortBy As String = ""
        Private AccessRights As New List(Of Integer)
        Private defaultFormData As New Dictionary(Of String, Object)
        Private defaultRowData As New Dictionary(Of String, Object)
        Private FormInsertCommand As String = String.Empty
        Private FormUpdateCommand As String = String.Empty
        Private GridInsertCommand As String = String.Empty
        Private GridUpdateCommand As String = String.Empty
        Private ListColumns As String = String.Empty
        Private FormColumns As String = String.Empty
        'Private FormColumns As New List(Of String)
        'Private GridColumns As New List(Of String)
#End Region

#Region "Properties"

        Public ReadOnly Property MenuID As Integer
            Get
                Return CInt(mRow("ID"))
            End Get
        End Property

        Public ReadOnly Property ParentID As Integer
            Get
                Dim i As Integer = 0
                If mRow("ID_WebMenus").ToString <> "" Then
                    i = CInt(mRow("ID_WebMenus"))
                End If
                Return i
            End Get
        End Property

        Public ReadOnly Property Name() As String
            Get
                Return CStr(mRow("Name"))
            End Get
        End Property

        Public ReadOnly Property TableName() As String
            Get
                Return CStr(IsNull(mRow("TableName"), ""))
            End Get
        End Property

        Public ReadOnly Property ColumnValue(ByVal ColumnName As String) As Object
            Get
                Return CObj(mRow(ColumnName))
            End Get
        End Property

        Public ReadOnly Property HasChild() As Boolean
            Get
                If mCollection.GetChild(Me.MenuID).Count > 0 Then
                    Return True
                Else
                    Return False
                End If
            End Get
        End Property

        Public ReadOnly Property mConn() As GSWEB.SQL.DataSource
            Get
                Return mConnection
            End Get
        End Property

        Public ReadOnly Property ReportPath() As String
            Get
                Return CStr(mRow("ReportPath"))
            End Get
        End Property

        Public ReadOnly Property RedirectMenu() As Object
            Get
                Return mRow("ID_WebMenus_Redirect")
            End Get
        End Property

        Public ReadOnly Property TableColumns(Optional ByVal IsEditableGrid As Boolean = False) As String
            Get
                Return getMenuColParameters(Me.MenuID, , IsEditableGrid).Replace("@", "")
            End Get
        End Property

        Public ReadOnly Property HasAccess(ByVal ID_UserGroup As Integer) As Boolean
            Get
                'Gamitin PAG Access Rights Gustong nsa memory
                'Return AccessRights.Contains(ID_UserGroup)
                ' Return ExecScalarNoParams("SELECT EXISTS(ID_UserGroup) FROM dbo.tUserGroupWebMenus WHERE ID_WebMenus = " + MenuID.ToString + " AND ID_UserGroup = " + ID_UserGroup.ToString)

                Return CBool(ExecScalarNoParams("SELECT CASE WHEN EXISTS(SELECT ID_UserGroup FROM dbo.tUserGroupWebMenus WHERE ID_WebMenus = " & MenuID.ToString & " AND ID_UserGroup = " & ID_UserGroup & ") THEN 1 ELSE 0 END"))
            End Get
        End Property

        'Public ReadOnly Property ColumnsWithValidation(Optional ByVal IsEditableGrid As Boolean = False) As List(Of String)
        '    Get
        '        Return getColumnWithValidation(Me.MenuID, , IsEditableGrid)
        '    End Get
        'End Property

        Public ReadOnly Property Label() As String
            Get
                Return CStr(IsNull(mRow("Label"), ""))
            End Get
        End Property


#End Region

#Region "Constructors"

        Public Sub New(ByVal mData As DataRow)
            mRow = mData
            'UnComment PAG Access Rights Gustong nsa memory
            'If CInt(Me.ColumnValue("ID_WebMenuTypes")) = 1 Then
            '    LoadAccessRights()
            'End If
        End Sub

#End Region

#Region "Subs"

        Public Function dtDataSource(Optional ByVal ButtonID As Integer = 0, Optional ByVal ParentID As Integer = 0, Optional ByVal SessionVariables As System.Web.SessionState.HttpSessionState = Nothing, Optional filter As String = "") As DataTable
            If Trim(mRow("DataSource").ToString) = "" Then Return Nothing
            Dim da As New SqlClient.SqlDataAdapter
            Dim dataSource As String = mRow("DataSource").ToString
            Dim dSource As New GSWEB.SQL.DataSource
            Try
                '    If filter <> "" Then sFilter = filter
                If Me.ColumnValue("SortBy").ToString <> "" Then SortBy = " ORDER BY " + Me.ColumnValue("SortBy").ToString

                'dSource.SetCommandText("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;" + _
                '                       "BEGIN TRANSACTION " + _
                '                       "Select * FROM " + mRow("View").ToString + " WITH (NOLOCK) " + sFilter.ToString + SortBy.ToString + _
                '                       " COMMIT TRANSACTION ")

                dSource.SetCommandText("SELECT * FROM " + SetDataSourceFilter(Me, ButtonID, ParentID, SessionVariables) + filter + SortBy.ToString)
                Dim dtD As DataTable = New DataTable
                If dSource.Connect() Then
                    dtD.Load(dSource.ExecuteReader)
                End If

                'mConnection.rID = 0
                Return dtD
            Catch ex As Exception
                logError(ex, Me.MenuID)
                Return Nothing
            Finally
                dSource.Disconnect()
            End Try
        End Function

        'Public Function dtDataSourceCount(Optional ByVal ButtonID As Integer = 0, Optional ByVal ParentID As Integer = 0, Optional ByVal SessionVariables As System.Web.SessionState.HttpSessionState = Nothing, Optional filter As String = "") As Integer
        '    If Trim(mRow("DataSource").ToString) = "" Then Return Nothing
        '    Dim cnt As Integer = 0
        '    Try
        '        '    If filter <> "" Then sFilter = filter
        '        If Me.ColumnValue("SortBy").ToString <> "" Then SortBy = " ORDER BY " + Me.ColumnValue("SortBy").ToString

        '        'dSource.SetCommandText("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;" + _
        '        '                       "BEGIN TRANSACTION " + _
        '        '                       "Select * FROM " + mRow("View").ToString + " WITH (NOLOCK) " + sFilter.ToString + SortBy.ToString + _
        '        '                       " COMMIT TRANSACTION ")
        '        cnt = ExecScalarNoParams("SELECT COUNT(ID) FROM " + SetDataSourceFilter(Me, ButtonID, ParentID, SessionVariables) + filter + SortBy.ToString)

        '        'mConnection.rID = 0
        '        Return cnt
        '    Catch ex As Exception
        '        Return Nothing
        '    Finally

        '    End Try
        'End Function

        Public Sub DeleteRow(ByVal IDS As String, Optional Filter As String = "")
            If mRow("TableName").ToString = "" Or IDS = "" Then Exit Sub
            Using sqlConn As New SqlClient.SqlConnection(ConnectionString)
                sqlConn.Open()
                Try
                    Using sqlCommand As New SqlClient.SqlCommand("DELETE FROM " + mRow("TableName").ToString + " WHERE ID IN(" + IDS + ") " & IIf(Filter.Length = 0, "", " AND " & Filter), sqlConn)
                        sqlCommand.ExecuteNonQuery()
                    End Using
                Catch ex As Exception
                    logError(ex, Me.MenuID)
                Finally
                    sqlConn.Close()
                    sqlConn.Dispose()
                End Try

            End Using
            'mConnection.SetCommandText("DELETE FROM " + mRow("TableName").ToString + " WHERE ID IN(" + IDS + ") " & IIf(Filter.Length = 0, "", " AND " & Filter))
            'mConnection.SetCommandType(CommandType.Text)
            'Try
            '    If mConnection.Connect() Then
            '        mConnection.ExecuteNonQuery()
            '    End If
            'Catch ex As Exception
            '    logError(ex, Me.MenuID)
            '    Throw ex
            'Finally
            '    mConnection.Disconnect()
            'End Try
        End Sub

        Public Function buildController() As String
            Dim sb As New System.Text.StringBuilder
            Try
                sb.Append("'use strict';")
                sb.Append("define(['app'],function(app){")

                Select Case CInt(Me.ColumnValue("ID_WebMenuTypes"))
                    Case 1
                        sb.Append("var c" & Me.MenuID & "=function($c,s,r,d,u,S,g,SS){")

                        Dim gridOptions As New Text.StringBuilder,
                            pagingOptions As New Text.StringBuilder,
                            sortInfo As New Text.StringBuilder,
                            filter As New Text.StringBuilder,
                            mID As Integer = 0
                        For Each m As Menu In mCollection.GetChild(Me.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 3 Or x.ColumnValue("ID_WebMenuTypes") = 17)
                            'If m.ColumnValue("ID_WebMenuTypes") = 15 Then
                            '    mID = m.MenuID
                            'End If
                            gridOptions.Append(m.MenuID & ":{")
                            gridOptions.Append("data:'gridData[" & m.MenuID & "]',")
                            If CBool(IsNull(m.ColumnValue("HasGridCheckbox"), False)) Then
                                gridOptions.Append("multiSelect:true,")
                                Dim hideGridCheckBoxIf As String = IsNull(m.ColumnValue("HideGridCheckBoxIf"), "")
                                If hideGridCheckBoxIf.Length > 0 Then
                                    gridOptions.Append("multiSelectIf:'" & hideGridCheckBoxIf & "',")
                                End If
                            End If
                            gridOptions.Append(buildColumnDefinition(m))
                            gridOptions.Append("totalServerItems:r.totalServerItems[" & m.MenuID & "],")
                            If m.dtColumns.Select("ID_WebMenuControlTypes = 7 AND ShowInList = 1").Count > 0 Then
                                gridOptions.Append("rowHeight:50,")
                            End If
                            gridOptions.Append("enableColumnResizing: " & CBool(IsNull(m.ColumnValue("IsEnableColumnResize"), True)).ToString.ToLower & ",")

                            'SORTING
                            Dim sortBy As String = IsNull(m.ColumnValue("SortBy"), "ID DESC")
                            gridOptions.Append("currentSortColumn:'" & sortBy.Split(" ")(0) & "',")
                            gridOptions.Append("currentSortDirection:'" & sortBy.Split(" ")(1).ToUpper & "',")

                            If CBool(IsNull(m.ColumnValue("WithPagination"), False)) Then
                                gridOptions.Append("enableSorting:true,")
                                gridOptions.Append("useExternalSorting:true,")
                                gridOptions.Append("enablePagination:true,")
                                gridOptions.Append("useExternalPagination:true,")
                            End If

                            If m.dtColumns.Select("IsGroup = 1 AND ShowInList = 1").Count > 0 Then
                                gridOptions.Append("enableGrouping:true,")
                                gridOptions.Append("hideGroupedValues:true,")
                            End If

                            gridOptions.Append("registerApi : function(events){")
                            gridOptions.Append("s.gridEvents[" & m.MenuID & "] = events;")
                            gridOptions.Append("s.gridEvents[" & m.MenuID & "].on.sortChange(s,function(opts){")
                            gridOptions.Append("s.refreshGrid(" & m.MenuID & ", opts.currentPageSize, opts.currentPage, opts.currentSortColumn, opts.currentSortDirection, s.filter[" & m.MenuID & "]);")
                            gridOptions.Append("});")

                            gridOptions.Append("s.gridEvents[" & m.MenuID & "].on.pageChange(s, function(opts){")
                            gridOptions.Append("s.refreshGrid(" & m.MenuID & ", opts.currentPageSize, opts.currentPage, opts.currentSortColumn, opts.currentSortDirection, s.filter[" & m.MenuID & "]);")
                            gridOptions.Append("});")
                            gridOptions.Append("},")


                            gridOptions.Append("},")

                            filter.Append(m.MenuID & ":{},")
                        Next

                        For Each m As Menu In mCollection.GetChild(Me.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 15)
                            filter.Append(m.MenuID & ":{},")
                        Next

                        sb.Append("s.gridOptions={" & gridOptions.ToString & "};")

                        sb.Append("for(var i in r.columnDefinitions){")
                        sb.Append("if(r.columnDefinitions[i].length>0){")
                        sb.Append("s.gridOptions[i].columnDefs=r.columnDefinitions[i];")
                        sb.Append("}")
                        sb.Append("};")

                        sb.Append("for(var i in r.groups){")
                        sb.Append("if(r.groups[i].length>0){")
                        sb.Append("s.gridOptions[i].groups=r.groups[i];")
                        sb.Append("}")
                        sb.Append("};")

                        sb.Append("s.filter={" & filter.ToString & "};")

                        'sb.Append("s.gridData[4]=u.getTree(r.gridData[4],'ID','ID_WebMenus');")
                        Dim treeViewOptions As New Text.StringBuilder,
                            treeViewData As New Text.StringBuilder
                        For Each m As Menu In mCollection.GetChild(Me.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 18)
                            treeViewOptions.Append(m.MenuID & ":{")
                            treeViewOptions.Append("data:'gridData[" & m.MenuID & "]',")
                            treeViewOptions.Append("expandLevel:2,")

                            treeViewOptions.Append("expandOn:{")
                            If CBool(IsNull(m.ColumnValue("HasOpen"), False)) Then
                                treeViewOptions.Append("field:'ID',")
                                treeViewOptions.Append("displayName:'',")
                                treeViewOptions.Append("cellTemplate:'<a ui-sref=\'" & buildAppStateName(m) & "({ ID_" & m.MenuID & ":row.entity.$$rID})\'><i class=\'fa fa-lg fa-fw fa-edit\'></i></a>'")
                            Else
                                Dim firstCol As DataRow = m.dtColumns.Select("ShowInList = 1", "SeqNo ASC").FirstOrDefault
                                treeViewOptions.Append("field:'" & firstCol("Name") & "',")
                                treeViewOptions.Append("displayName:'" & IsNull(firstCol("Label"), firstCol("Name")) & "',")
                            End If
                            treeViewOptions.Append("},")

                            treeViewOptions.Append(buildTreeViewColumnDefinition(m))
                            treeViewOptions.Append("},")
                            treeViewData.Append("s.gridData[" & m.MenuID & "]=u.getTree(r.gridData[" & m.MenuID & "],'" & m.dtColumns.Select("ListKey = 1")(0)("Name") & "','" & m.dtColumns.Select("ParentKey = 1")(0)("Name") & "');")
                        Next
                        sb.Append(treeViewData.ToString)
                        sb.Append("s.treeViewOptions={" & treeViewOptions.ToString & "};")


                        sb.Append("$c('BaseListController',{$scope:s,resources:r});")
                        For Each m As Menu In mCollection.GetChild(Me.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 15)
                            sb.Append("var iframe = $('<iframe />');")
                            sb.Append("iframe.attr('style', 'width:100%;height:850px;');")
                            sb.Append("iframe.attr('src', '../ModulePage/Report.aspx?menuID=' + " & m.MenuID & " + '&refID=' + 0 + '&params=' + JSON.stringify(s.rawData));")
                            sb.Append("iframe.attr('id', 'frame_" & m.MenuID & "');")
                            sb.Append("$('.grid').append(iframe);")
                        Next
                        'If mID > 0 Then
                        '    sb.Append("s.Search=function(mID){")
                        '    sb.Append("s.RefreshReport();")
                        '    sb.Append("};")

                        '    sb.Append("s.RefreshReport = function(){")
                        '    sb.Append("s.rawData = [];")
                        '    sb.Append("$.each(s.filter[" & mID & "], function(d, v){")
                        '    sb.Append("var item = {")
                        '    sb.Append("'id' : 'obj_' + '" & mID & "_' + '0_' + d,")
                        '    sb.Append("'name' : 'obj_' + '" & mID & "_' + '0_' + d,")
                        '    sb.Append("'value' : (v = null ? '' : v),")
                        '    sb.Append("'menuID' : " & mID & ",")
                        '    sb.Append("'refID' : 0,")
                        '    sb.Append("'seq_no' : 0,")
                        '    sb.Append("'columnName' : d")
                        '    sb.Append("};")
                        '    sb.Append("s.rawData.push(item);")
                        '    sb.Append("});")
                        '    sb.Append("d.ValidateFilterReport(" & mID & ", JSON.stringify(s.rawData)).then(function(data){")
                        '    sb.Append("var warning = '', MessageTitle = '';")
                        '    sb.Append("warning = data.warning;")
                        '    sb.Append("MessageTitle = data.MessageTitle;")
                        '    sb.Append("if (warning == 'none') {")
                        '    sb.Append("$('#frame_" & mID & "').attr('src', '../ModulePage/Report.aspx?menuID=' + " & mID & " + '&refID=' + 0 + '&params=' + JSON.stringify(s.rawData));")
                        '    sb.Append("} else {")
                        '    sb.Append("console.log(MessageTitle);")
                        '    sb.Append("}")
                        '    sb.Append("});")
                        '    sb.Append("};")
                        '    sb.Append("var iframe = $('<iframe />');")
                        '    sb.Append("iframe.attr('style', 'width:100%;height:850px;');")
                        '    sb.Append("iframe.attr('src', '../ModulePage/Report.aspx?menuID=' + " & mID & " + '&refID=' + 0 + '&params=' + JSON.stringify(s.rawData));")
                        '    sb.Append("iframe.attr('id', 'frame_" & mID & "');")
                        '    sb.Append("$('.grid').append(iframe);")

                        'End If


                        sb.Append("};")
                        sb.Append("app.register.controller('c" & Me.MenuID & "',['$controller','$scope','resources','dataService','utilService','$state','growlNotifications','Session',c" & Me.MenuID & "]);")

                    Case 3, 12, 14
                        sb.Append("var c" & Me.MenuID & "=function($c,s,r,d,u,S" & If(Me.ColumnValue("ID_WebMenuTypes") <> 3, ",mI", "") & ",g,SS,ck){")
                        sb.Append("$c('BaseFormController',{$scope:s,resources:r});")


                        sb.Append("s.mID=" & Me.MenuID & ";")
                        sb.Append("s.rID=S.params.ID_" & Me.MenuID & ";")
                        If Me.ColumnValue("ID_WebMenuTypes") <> 3 Then
                            sb.Append("s.close=function(){")
                            sb.Append("mI.dismiss('close');")
                            sb.Append("S.go('^',{reload:true});")
                            sb.Append("};")
                        End If

                        sb.Append("s.goPrevious=function(){")
                        If Me.ColumnValue("ID_WebMenuTypes") <> 3 Then
                            sb.Append("S.go('^',{reload:true});")
                        Else
                            sb.Append("S.go('" & Me.ParentID & "',{},{reload:true,inherit:false,notify:true});")
                        End If
                        sb.Append("};")


                        Dim gridOptions As New Text.StringBuilder


                        For Each m In mCollection.GetChild(Me.MenuID).Where(Function(x) x.ColumnValue("IsVisible") = True And {12, 14}.Contains(x.ColumnValue("ID_WebMenuTypes")))
                            gridOptions.Append(m.MenuID & ":{")
                            gridOptions.Append("data:'Detail[" & m.MenuID & "]',")
                            gridOptions.Append("enableSorting:true,")
                            If IsNull(m.ColumnValue("FixedColumns"), 0) > 0 Then
                                gridOptions.Append("enablePinning:true,")
                                gridOptions.Append("enableColumnResizing:true,")
                            End If

                            gridOptions.Append(buildColumnDefinition(m))

                            If m.dtColumns.Select("ID_WebMenuControlTypes = 7 AND ShowInList = 1").Count > 0 Then
                                gridOptions.Append("rowHeight:50,")
                            End If
                            If m.dtColumns.Select("IsGroup = 1 AND IsVisible = 1").Count > 0 Then
                                gridOptions.Append("enableGrouping:true,")
                            End If
                            gridOptions.Append("},")

                        Next

                        Dim m2 As GSWEB.MenuCollection.Menu = mCollection.GetMenu(Me.MenuID)

                        For Each cols As DataRow In m2.dtColumns.Select("ID_WebMenuControlTypes = 17 AND IsVisible = 1 AND IsActive = 1")
                            Dim DisplayMember As String = IsNull(cols("DisplayMember"), "Name")
                            sb.Append("s.$watch('Master." & cols("Name") & "', function(nv,ov){")
                            sb.Append("if(nv !== ov){")
                            sb.Append("if (nv == undefined || nv == null) {")
                            sb.Append("s.Master." & cols("Name").ToString.Substring(3) & " = '';")
                            sb.Append("} else {")

                            If IsNull(cols("JSEvents"), "") <> "" Then
                                Dim jsEvents As String = cols("JSEvents").ToString.Split("=")(0).ToLower()
                                Dim ev As String = cols("JSEvents").ToString.Split("=")(1)
                                Dim c As String = ev.Split(";")(1)
                                Dim m As GSWEB.MenuCollection.Menu = mCollection.GetMenu(Me.MenuID),
                                    targetColumn As DataRow = m.dtColumns.Select("ID = " & ev.Split(";")(0))(0)
                                If jsEvents = "getcolumnvalues" Then
                                    sb.Append("d.getColumnValues(" & Me.MenuID & ", " & cols("ID") & ", nv).then(function(res){")
                                    sb.Append("angular.forEach(res.data, function (obj) {")
                                    sb.Append("angular.forEach(obj[0], function (val, col) {")
                                    sb.Append("s.Master[col] = val;")
                                    sb.Append("});")
                                    sb.Append("});")
                                    sb.Append("});")
                                ElseIf jsEvents = "cascade2" Then
                                    sb.Append("s.ccd = function(mID, colID, value, dropdown){")
                                    sb.Append("d.CascadingDropdown2(mID, colID, value).then(function (results) {")
                                    sb.Append("var a = dropdown.split(',');")
                                    sb.Append("for (var i = 0; i < a.length; i++) {")
                                    sb.Append("s.dropdown_source[a[i]] = results.data[a[i]];")
                                    sb.Append("}if (s.Master." & cols("Name").ToString.Substring(3) & " == '') {")
                                    sb.Append("s.Master." & c & " = '';")
                                    sb.Append("} else {")
                                    sb.Append("s.Master." & c & " = s.dropdown_source[" & targetColumn("ID") & "][0].ID;")
                                    sb.Append("}});};")
                                    sb.Append("s.ccd(" & Me.MenuID & ", " & cols("ID") & ", s.Master." & cols("Name") & ", '" & ev.Split(";")(0) & "');")
                                End If
                            End If

                            sb.Append("var obj = s.lookup_source[" & cols("ID") & "].filter(function (x) { return x.ID == nv });")
                            sb.Append("s.Master." & cols("Name").ToString.Substring(3) & " = obj[0]['" & DisplayMember & "'];")
                            sb.Append("}")
                            sb.Append("}")
                            sb.Append("});")
                        Next

                        sb.Append("s.gridOptions={" & gridOptions.ToString & "};")

                        If mCollection.GetChild(Me.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 18).Count > 0 Then
                            Dim treeViewOptions As New Text.StringBuilder
                            For Each m In mCollection.GetChild(Me.MenuID).Where(Function(x) x.ColumnValue("IsVisible") = True And x.ColumnValue("ID_WebMenuTypes") = 18)
                                treeViewOptions.Append(m.MenuID & ":{")
                                treeViewOptions.Append("data:'Detail[" & m.MenuID & "]',")
                                treeViewOptions.Append("listKey:'" & m.dtColumns().AsEnumerable().Where(Function(x) x.Item("ListKey") = True).First.Item("Name") & "',")
                                treeViewOptions.Append("parentKey:'" & m.dtColumns().AsEnumerable().Where(Function(x) x.Item("ParentKey") = True).First.Item("Name") & "',")
                                treeViewOptions.Append("displayName:'" & m.dtColumns().AsEnumerable().Where(Function(x) x.Item("ListKey") = True).First.Item("Name").ToString.Substring(3) & "'")


                                treeViewOptions.Append("},")

                            Next
                            sb.Append("s.treeOptions={" & treeViewOptions.ToString & "};")
                        End If

                        ''EXCEL UPLOAD
                        'If Not Me.dtButtons Is Nothing Then
                        '    Me.dtButtons.CaseSensitive = False
                        '    If Me.dtButtons.Select("Name = 'ImportExcelTemplate'").Count > 0 Then
                        '        sb.Append("s.onExcelSelect=function($files,mID,btnID,targetmID,targetTab){")
                        '        sb.Append("if($files.length>0){")
                        '        sb.Append("d.ImportExcelTemplate(mID,btnID,$files[0]).then(function(results){")
                        '        sb.Append("if(results.messageType==2){")
                        '        sb.Append("g.add(results.message,'anger',5000);")
                        '        sb.Append("}else{")
                        '        sb.Append("if(targetmID!=0) {")
                        '        sb.Append("s.Detail[targetmID]=results.data;")
                        '        sb.Append("s.tabs.activeTab=targetTab;")
                        '        sb.Append("if (s.loadedTab.indexOf(targetmID) == -1) {")
                        '        sb.Append("s.loadedTab.push(targetmID);")
                        '        sb.Append("}")
                        '        sb.Append("} else {")
                        '        sb.Append("for(var x = 0; x < results.data.length; x++) {")
                        '        sb.Append("s.Detail[results.data[x].ID] = results.data[x].Data;")
                        '        sb.Append("s.tabs.activeTab = x + 1;")
                        '        sb.Append("if (s.loadedTab.indexOf(results.data[x].ID) == -1) {")
                        '        sb.Append("s.loadedTab.push(results.data[x].ID);")
                        '        sb.Append("}")
                        '        sb.Append("}")
                        '        sb.Append("s.tabs.activeTab = 0;")
                        '        sb.Append("}")
                        '        sb.Append("g.add(results.message,'info',5000);")
                        '        sb.Append("}")
                        '        sb.Append("});")
                        '        sb.Append("}")
                        '        sb.Append("};")
                        '    End If
                        'End If

                        '' JSEVENTS
                        '' CASCADING DROPDOWN
                        'sb.Append("s.CascadingDropdown=function(mID,colID,value,dropdown){")
                        'sb.Append("d.CascadingDropdown(mID,colID,value).then(function(results){")
                        'sb.Append("var a=dropdown.split("","");")
                        'sb.Append("for(var i=0;i<a.length;i++){")
                        'sb.Append("s.dropdown_source[a[i]]=results.data[a[i]];")
                        'sb.Append("}")
                        'sb.Append("});")
                        'sb.Append("};")

                        'sb.Append("ck.check(s);")
                        sb.Append("};")
                        sb.Append("app.register.controller('c" & Me.MenuID & "',['$controller','$scope','resources','dataService','utilService','$state'" & If(Me.ColumnValue("ID_WebMenuTypes") <> 3, ",'$modalInstance'", "") & ",'growlNotifications','Session','ckFormPristine',c" & Me.MenuID & "]);")



                End Select

                sb.Append("});")
            Catch ex As Exception
                logError(ex, Me.MenuID)
            End Try
            Return sb.ToString
        End Function

        Public Function buildTemplate() As String
            Dim sb As New System.Text.StringBuilder
            Try
                Select Case Me.ColumnValue("ID_WebMenuTypes")
                    Case 1

                        sb.Append("<div class='ribbon'>")
                        sb.Append("<span class='ribbon-button-alignment'>")
                        'sb.Append("<span class='btn btn-ribbon'><i class='fa fa-home'></i></span>")
                        sb.Append("</span>")
                        sb.Append("<ol class='breadcrumb'><li>" & IIf(IsNull(Me.ColumnValue("Label"), "") = "", Me.Name, Me.ColumnValue("Label")) & "</li></ol>")
                        sb.Append("</div>")
                        sb.Append("<div class='content'>")
                        Dim query = mCollection.GetChild(Me.MenuID).Where(Function(x) x.ColumnValue("ID_WebMenuTypes") = 3 Or x.ColumnValue("ID_WebMenuTypes") = 17 Or x.ColumnValue("ID_WebMenuTypes") = 18 Or x.ColumnValue("ID_WebMenuTypes") = 15)
                        sb.Append("<div class='tab_container' " & IIf(query.Count > 1, "bs-tabs", "") & ">")
                        For Each m As GSWEB.MenuCollection.Menu In query
                            sb.Append("<div title='" & m.Name & "' " & IIf(query.Count > 1, "bs-pane", "") & ">")
                            Dim defaultFilterCount As Integer = m.dtColumns.Select("InFilter = 1 AND ((ISNULL(ID_FilterWebMenuControlTypes,14) <> 14) AND (ISNULL(ID_FilterWebMenuControlTypes,0) <> 0))").Count

                            'sb.Append("<div class='button-holder'>")
                            'sb.Append("<ol class='breadcrumb'>")
                            'If m.dtColumns.Select("ISNULL(InFilter,0) = 0 AND ISNULL(ID_FilterWebMenuControlTypes,14) <> 14").Count > 0 Then
                            '    sb.Append("<li>")
                            '    sb.Append("<span class='advance-filter' data-toggle-filter='" & m.MenuID & "' >Advance Filter</span>")
                            '    sb.Append("</li>")
                            'End If
                            'If defaultFilterCount > 0 Then
                            '    sb.Append("<li>")
                            '    Dim dr As DataRow = m.dtColumns.Select("InFilter = 1 AND ISNULL(ID_FilterWebMenuControlTypes,14) <> 14").First
                            '    sb.Append(buildHTMLFilterControl(dr))
                            '    sb.Append("</li>")
                            '    sb.Append("<li>")
                            '    sb.Append("<button type='button' class='btn btn-primary btn-search' mtype='3' ng-click='Search(" & m.MenuID & ")' ><i class='fa fa-search'></i></button>")
                            '    sb.Append("</li>")
                            'End If


                            'If CBool(m.ColumnValue("HasNew")) Then
                            '    sb.Append("<li>")
                            '    sb.Append("<button type='button' class='btn btn-primary' ng-form-new mid='" & m.MenuID & "' mtype='3'><i class='fa fa-file-o'></i>New</button>")
                            '    sb.Append("</li>")
                            'End If
                            'sb.Append("<li>")
                            'sb.Append("<button type='button' class='btn btn-primary' ng-click='Refresh()'><i class='fa fa-refresh'></i>Refresh</button>")
                            'sb.Append("</li>")
                            'If Not m.dtButtons Is Nothing Then
                            '    For Each btn In m.dtButtons.Select("IsVisible = 1 AND ID_WebMenuButtonTypes = 4", "SeqNo ASC")
                            '        sb.Append("<li>")
                            '        Select Case btn("Name").ToString.ToLower
                            '            Case "delete"
                            '                sb.Append("<button type='button' class='btn btn-primary' ng-batch-grid-delete  mid='" & m.MenuID & "'>Delete</button>")
                            '            Case "save"
                            '                sb.Append("<button type='button' class='btn btn-primary' ng-batch-grid-save  mid='" & m.MenuID & "' btnid='" & btn("ID") & "'>Save</button>")
                            '            Case "generateexceltemplate"
                            '                sb.Append("<button " & If(IsDBNull(btn("VisibleIf")), "", "ng-show='" & btn("VisibleIf").ToString & "'") & " type='button' class='btn btn-primary' ng-generate-excel-template " & If(IsNull(btn("ConfirmationDialog"), "") = "", "", "confirm=""" & HttpUtility.HtmlEncode(btn("ConfirmationDialog")) & """") & " btnid='" & btn("ID") & "' mid='" & btn("ID_WebMenus") & "' >" & IsNull(btn("Label"), btn("Name")) & "</button>")
                            '            Case "printgridreport"
                            '                sb.Append("<button " & If(IsDBNull(btn("VisibleIf")), "", "ng-show='" & btn("VisibleIf").ToString & "'") & " type='button' class='btn btn-primary' ng-form-print-report btnid='" & btn("ID") & "' mid='" & btn("ID_WebMenus") & "' >" & IsNull(btn("Label"), btn("Name")) & "</button>")
                            '            Case Else
                            '                sb.Append("<button type='button' class='btn btn-primary' ng-batch-grid-command mid='" & m.MenuID & "' btnid='" & btn("ID") & "'>" & IsNull(btn("Label"), btn("Name")) & "</button>")
                            '        End Select
                            '        sb.Append("</li>")
                            '    Next
                            'End If

                            ''EXPORT TO EXCEL
                            'If CBool(m.ColumnValue("WithExportToExcel")) Then
                            '    sb.Append("<li>")
                            '    sb.Append("<button type='button' class='btn btn-primary' ng-export-to-excel mid='" & m.MenuID & "'><i class='fa fa-file-excel-o'></i>Export To Excel</button>")
                            '    sb.Append("</li>")
                            'End If

                            ''COLUMN SELECTION -TODO: LAGYAN BA NG SETTINGS?
                            'sb.Append("<li>")
                            'sb.Append("<button type='button' class='btn btn-primary' ng-column-selection='" & m.MenuID & "'><i class='fa fa-columns'></i>Columns</button>")
                            'sb.Append("</li>")

                            'sb.Append("</ol>")

                            'sb.Append("</div>")

                            sb.Append("<nav class='navbar navbar-right navbar-button-holder col-sm-12'>")
                            sb.Append("<div>")
                            sb.Append("<form autocomplete='off' class='navbar-form navbar-right'>")
                            'If m.dtColumns.Select("ISNULL(InFilter,0) = 0 AND ((ISNULL(ID_FilterWebMenuControlTypes,14) <> 14) AND (ISNULL(ID_FilterWebMenuControlTypes,0) <> 0))").Count > 0 Then
                            '    sb.Append("<a class='btn navbar-btn advance-filter' data-toggle-filter='" & m.MenuID & "' >" & If(m.ColumnValue("ID_WebMenuTypes") <> 15, "Advance ", "") & "Filter</a>")
                            'End If
                            If m.dtColumns.Select("((ISNULL(ID_FilterWebMenuControlTypes,14) <> 14) AND (ISNULL(ID_FilterWebMenuControlTypes,0) <> 0))").Count > 0 Then
                                sb.Append("<a class='btn navbar-btn advance-filter' data-toggle-filter='" & m.MenuID & "' >" & If(m.ColumnValue("ID_WebMenuTypes") <> 15, "Advance ", "") & "Filter</a>")
                            End If
                            'If defaultFilterCount > 0 AndAlso m.ColumnValue("ID_WebMenuTypes") <> 15 Then
                            '    sb.Append("<div class='input-group navbar-btn'>")
                            '    Dim dr As DataRow = m.dtColumns.Select("InFilter = 1 AND ((ISNULL(ID_FilterWebMenuControlTypes,14) <> 14) AND (ISNULL(ID_FilterWebMenuControlTypes,0) <> 0))").First
                            '    sb.Append(buildHTMLFilterControl(dr))
                            '    sb.Append("<span class='input-group-addon' mtype='3' ng-click='Search(" & m.MenuID & ")' ><i class='fa fa-search'></i></span>")
                            '    sb.Append("</div>")
                            'End If
                            If m.ColumnValue("ID_WebMenuTypes") <> 15 Then
                                sb.Append("<div class='input-group navbar-btn'>")
                                sb.Append("<input type=""text"" name=""SearchAll"" " &
                                  "placeholder='Search all columns'" &
                                  "class=""form-control"" ng-model=""filter[" & m.MenuID & "]." & "SearchAll" & """ ng-enter=""Search(" & m.MenuID & ", 1)""/>")
                                sb.Append("<span class='input-group-addon' mtype='3' ng-click='Search(" & m.MenuID & ", 1)' ><i class='fa fa-search'></i></span>")
                                sb.Append("</div>")
                            End If

                            Dim grpBtns As New List(Of String)
                            If CBool(m.ColumnValue("HasNew")) Then
                                sb.Append("<button type='button' class='btn btn-primary navbar-btn hidden-xs' ng-form-new mid='" & m.MenuID & "' mtype='3'><i class='fa fa-file-o'></i>New</button>")
                                grpBtns.Add("<a ng-form-new mid='" & m.MenuID & "' mtype='3'><i class='fa fa-file-o'></i>  New</a>")
                            End If
                            If m.ColumnValue("ID_WebMenuTypes") <> 15 Then
                                sb.Append("<button type='button' class='btn btn-primary navbar-btn hidden-xs' ng-click='Refresh()'><i class='fa fa-refresh'></i>Refresh</button>")
                                grpBtns.Add("<a ng-click='Refresh()'><i class='fa fa-refresh'></i>  Refresh</a>")
                            End If

                            If Not m.dtButtons Is Nothing Then
                                For Each btn In m.dtButtons.Select("IsVisible = 1 AND ID_WebMenuButtonTypes = 4", "SeqNo ASC")
                                    Select Case btn("Name").ToString.ToLower
                                        Case "delete"
                                            sb.Append("<button type='button' class='btn btn-primary navbar-btn hidden-xs' ng-batch-grid-delete " & If(IsNull(btn("ConfirmationDialog"), "") = "", "", "confirm=""" & HttpUtility.HtmlEncode(btn("ConfirmationDialog")) & """") & "  mid='" & m.MenuID & "'>Delete</button>")
                                            grpBtns.Add("<a ng-batch-grid-delete " & If(IsNull(btn("ConfirmationDialog"), "") = "", "", "confirm=""" & HttpUtility.HtmlEncode(btn("ConfirmationDialog")) & """") & " mid='" & m.MenuID & "'>  Delete</a>")
                                        Case "save"
                                            sb.Append("<button type='button' class='btn btn-primary navbar-btn hidden-xs' ng-batch-grid-save  mid='" & m.MenuID & "' btnid='" & btn("ID") & "'>Save</button>")
                                            grpBtns.Add("<a ng-batch-grid-save  mid='" & m.MenuID & "' btnid='" & btn("ID") & "'>  Save</a>")
                                        Case "generateexceltemplate"
                                            sb.Append("<button " & If(IsDBNull(btn("VisibleIf")), "", "ng-show='" & btn("VisibleIf").ToString & "'") & " type='button' class='btn btn-primary navbar-btn hidden-xs' ng-generate-excel-template " & If(IsNull(btn("ConfirmationDialog"), "") = "", "", "confirm=""" & HttpUtility.HtmlEncode(btn("ConfirmationDialog")) & """") & " btnid='" & btn("ID") & "' mid='" & btn("ID_WebMenus") & "' >" & IsNull(btn("Label"), btn("Name")) & "</button>")
                                            grpBtns.Add("<a ng-generate-excel-template " & If(IsNull(btn("ConfirmationDialog"), "") = "", "", "confirm=""" & HttpUtility.HtmlEncode(btn("ConfirmationDialog")) & """") & " btnid='" & btn("ID") & "' mid='" & btn("ID_WebMenus") & "' >  " & IsNull(btn("Label"), btn("Name")) & "</a>")
                                        Case "printgridreport"
                                            sb.Append("<button " & If(IsDBNull(btn("VisibleIf")), "", "ng-show='" & btn("VisibleIf").ToString & "'") & " type='button' class='btn btn-primary navbar-btn hidden-xs' ng-form-print-report btnid='" & btn("ID") & "' mid='" & btn("ID_WebMenus") & "' >" & IsNull(btn("Label"), btn("Name")) & "</button>")
                                            grpBtns.Add("<a ng-form-print-report btnid='" & btn("ID") & "' mid='" & btn("ID_WebMenus") & "' >  " & IsNull(btn("Label"), btn("Name")) & "</a>")
                                        Case Else
                                            sb.Append("<button type='button' class='btn btn-primary navbar-btn hidden-xs' ng-batch-grid-command mid='" & m.MenuID & "' btnid='" & btn("ID") & "'>" & IsNull(btn("Label"), btn("Name")) & "</button>")
                                            grpBtns.Add("<a ng-batch-grid-command mid='" & m.MenuID & "' btnid='" & btn("ID") & "'>  " & IsNull(btn("Label"), btn("Name")) & "</a>")
                                    End Select
                                Next
                            End If

                            'EXPORT TO EXCEL
                            If CBool(m.ColumnValue("WithExportToExcel")) Then
                                sb.Append("<button type='button' class='btn btn-primary navbar-btn hidden-xs' ng-export-to-excel mid='" & m.MenuID & "'><i class='fa fa-file-excel-o'></i>Export To Excel</button>")
                                grpBtns.Add("<a ng-export-to-excel mid='" & m.MenuID & "'><i class='fa fa-file-excel-o'></i>  Export To Excel</a>")
                            End If

                            'COLUMN SELECTION -TODO: LAGYAN BA NG SETTINGS?
                            If m.ColumnValue("ID_WebMenuTypes") <> 15 Then
                                sb.Append("<button type='button' class='btn btn-primary navbar-btn hidden-xs' ng-column-selection='" & m.MenuID & "'><i class='fa fa-columns'></i>Columns</button>")
                                grpBtns.Add("<a ng-column-selection='" & m.MenuID & "'><i class='fa fa-columns'></i>  Columns</a>")

                            End If

                            sb.Append("<div class='btn-group navbar-btn hidden-lg hidden-md'>")
                            sb.Append("<button type='button' class='btn btn-primary dropdown-toggle' data-toggle-dropdown>")
                            sb.Append("Actions <span class='caret'></span>")
                            sb.Append("</button>")
                            sb.Append("<ul class='dropdown-menu-xs-left dropdown-menu dropdown-menu-right'>")
                            For Each grpBtn As String In grpBtns
                                sb.Append("<li>")
                                sb.Append(grpBtn)
                                sb.Append("</li>")
                            Next
                            sb.Append("</ul>")
                            sb.Append("</div>")

                            sb.Append("</form>")


                            sb.Append("</div>")
                            sb.Append("</nav>")

                            If m.ColumnValue("ID_WebMenuTypes") = 15 Then
                                If m.dtColumns.Select("((ISNULL(ID_FilterWebMenuControlTypes,14) <> 14) AND (ISNULL(ID_FilterWebMenuControlTypes,0) <> 0))").Count > 0 Then
                                    sb.Append("<div class='grid-filter col-md-8 pull-right hide' id='grid-filter-" & m.MenuID & "' ng-form='filter_" & m.MenuID & "'>")

                                    sb.Append("<div class='filter-header row'>")
                                    sb.Append("<button type='button' class='btn btn-primary pull-right' ng-click='RefreshReport(" & m.MenuID & ")' ><i class='fa fa-search'></i>Refresh</button>")
                                    sb.Append("<button type='button' class='btn btn-primary pull-right' ng-click='clearFilter(" & m.MenuID & ")' ><i class='fa fa-refresh'></i>Clear</button>")
                                    sb.Append("</div>")

                                    sb.Append("<div class='filter-holder'>")
                                    For Each dr As DataRow In m.dtColumns.Select("((ISNULL(ID_FilterWebMenuControlTypes,14) <> 14) AND (ISNULL(ID_FilterWebMenuControlTypes,0) <> 0))", "FilterSeqNo ASC")
                                        sb.Append("<div class='row' " & If(CBool(IsNull(dr("IsRequired"), False)), "ng-class=""{ 'has-error' : filter_" & m.MenuID & "." & dr("Name") & ".$invalid && filter_" & m.MenuID & ".$submitted }"" ", "") & "><div class='col-sm-3'>" & IsNull(dr("Label"), dr("Name")) & "</div><div class='col-sm-9'>" & buildHTMLFilterControl(dr, True) & "</div></div>")
                                    Next
                                    sb.Append("</div>")
                                    sb.Append("</div>")
                                End If
                            Else
                                If m.dtColumns.Select("((ISNULL(ID_FilterWebMenuControlTypes,14) <> 14) AND (ISNULL(ID_FilterWebMenuControlTypes,0) <> 0))").Count > 0 Then
                                    sb.Append("<div class='grid-filter col-md-8 pull-right hide' id='grid-filter-" & m.MenuID & "'>")

                                    sb.Append("<div class='filter-header row'>")
                                    sb.Append("<button type='button' class='btn btn-primary pull-right' ng-click='Search(" & m.MenuID & ",0)' ><i class='fa fa-search'></i>Search</button>")
                                    sb.Append("<button type='button' class='btn btn-primary pull-right' ng-click='clearFilter(" & m.MenuID & ")' ><i class='fa fa-refresh'></i>Clear</button>")
                                    sb.Append("</div>")

                                    sb.Append("<div class='filter-holder'>")
                                    For Each dr As DataRow In m.dtColumns.Select("((ISNULL(ID_FilterWebMenuControlTypes,14) <> 14) AND (ISNULL(ID_FilterWebMenuControlTypes,0) <> 0))", "FilterSeqNo ASC")
                                        sb.Append("<div class='row'><div class='col-sm-3'>" & IsNull(dr("Label"), dr("Name")) & "</div><div class='col-sm-9'>" & buildHTMLFilterControl(dr) & "</div></div>")
                                    Next
                                    sb.Append("</div>")
                                    sb.Append("</div>")
                                End If
                            End If


                            sb.Append("<div class='clear'></div>")
                            Select Case m.ColumnValue("ID_WebMenuTypes")
                                Case 3, 17
                                    sb.Append("<div style='width:100%;" & If(IsDBNull(m.ColumnValue("Height")), "height:400px;", "height:" & m.ColumnValue("Height") & "px;") & "' class='grid' m-grid='gridOptions[" & m.MenuID & "]'></div>")
                                Case 18
                                    sb.Append("<tree-grid tree-data='treeViewOptions[" & m.MenuID & "]'></tree-grid>")
                                Case 15
                                    sb.Append("<div style='width:100%;" & If(IsDBNull(m.ColumnValue("Height")), "height:100%;", "height:" & m.ColumnValue("Height") & "px;") & "' class='grid'></div>")
                            End Select

                            sb.Append("</div>")
                        Next
                        sb.Append("</div>")
                        sb.Append("</div>")

                    Case 3, 12, 14
                        If Me.ColumnValue("ID_WebMenuTypes") <> 3 Then
                            sb.Append("<div class='modal-header'>")
                            sb.Append("<button type='button' class='close' ng-click='close()'>")
                            sb.Append("<span aria-hidden='true'>×</span><span class='sr-only'>Close</span>")
                            sb.Append("</button>")
                            sb.Append("<h4 class='modal-title'>" & Me.Name & "</h4>")
                            sb.Append("</div> ")
                            sb.Append("<div class='modal-body'>")
                        End If
                        sb.Append("<div class='ribbon'>")
                        sb.Append("<span class='ribbon-button-alignment'>")
                        ''TODO 
                        If Me.ColumnValue("HasBack") Then
                            sb.Append("<span class='btn btn-ribbon' ng-click='goPrevious()'><i class='fa fa-arrow-left'></i></span>")
                        End If
                        sb.Append("</span>")
                        sb.Append("<ol class='breadcrumb'>")
                        sb.Append(buildMenuRibbon(Me))
                        sb.Append("</ol>")
                        sb.Append("</div>")
                        sb.Append("<div class='content'>")
                        sb.Append("<div ng-form name='mainform'>")

                        sb.Append("<div class='button-holder'>")
                        sb.Append("<ol class='breadcrumb'>")
                        If CBool(Me.ColumnValue("HasNew")) And Me.ColumnValue("ID_WebMenuTypes") <> 14 Then
                            sb.Append("<li>")
                            sb.Append("<button type='button' " & If(IsDBNull(Me.ColumnValue("HasNewIf")), "", "ng-if='" & If(Me.ColumnValue("ID_WebMenuTypes") = 3, Me.ColumnValue("HasNewIf"), Me.ColumnValue("HasNewIf").ToString.Replace("Master", "Parent")) & "'") & " class='btn btn-primary' ng-form-new mid='" & Me.MenuID & "' mtype='" & Me.ColumnValue("ID_WebMenuTypes") & "'><i class='fa fa-file-o'></i>New</button>")
                            sb.Append("</li>")
                        End If
                        sb.Append("<li>")
                        sb.Append("<button type='button' class='btn btn-primary' ng-click='Refresh()'><i class='fa fa-refresh'></i>Refresh</button>")
                        sb.Append("</li>")
                        Dim hasGenerateButton As Boolean = False
                        If Not Me.dtButtons Is Nothing Then
                            For Each btn As DataRow In Me.dtButtons.Select("IsVisible = 1 AND ID_WebMenuButtonTypes IN (1,2)", "SeqNo DESC")
                                sb.Append("<li>")
                                Select Case btn("Name").ToString.ToLower
                                    Case "save", "submit"
                                        sb.Append("<button " & If(IsDBNull(btn("VisibleIf")), "", "ng-if='" & btn("VisibleIf").ToString & "'") & " type='button' class='btn btn-primary' accesskey='s' ng-form-save " & If(IsNull(btn("ConfirmationDialog"), "") = "", "", "confirm=""" & HttpUtility.HtmlEncode(btn("ConfirmationDialog")) & """") & " btnid='" & btn("ID") & "' ><i class='fa fa-save'></i>" & IsNull(btn("Label"), btn("Name")) & "</button>")
                                    Case "generatedate"
                                        Dim targetTab As Integer = 0
                                        For Each m As Menu In mCollection.GetChild(Me.MenuID).Where(Function(x) x.ColumnValue("IsVisible") = True)
                                            targetTab += 1
                                            If m.MenuID = CInt(btn("ID_TargetWebMenus")) Then Exit For
                                        Next
                                        sb.Append("<button " & If(IsDBNull(btn("VisibleIf")), "", "ng-if='" & btn("VisibleIf").ToString & "'") & " type='button' class='btn btn-primary' ng-form-generate " & If(IsNull(btn("ConfirmationDialog"), "") = "", "", "confirm=""" & HttpUtility.HtmlEncode(btn("ConfirmationDialog")) & """") & " btnid='" & btn("ID") & "' target-mid='" & IsNull(btn("ID_TargetWebMenus"), 0) & "' target-tab='" & (targetTab + Me.ColumnValue("TabsCount") - 1) & "' >" & IsNull(btn("Label"), btn("Name")) & "</button>")
                                        hasGenerateButton = True
                                    Case "publish"
                                        sb.Append("<button " & If(IsDBNull(btn("VisibleIf")), "", "ng-if='" & btn("VisibleIf").ToString & "'") & " type='button' class='btn btn-primary' ng-publish " & If(IsNull(btn("ConfirmationDialog"), "") = "", "", "confirm=""" & HttpUtility.HtmlEncode(btn("ConfirmationDialog")) & """") & " btnid='" & btn("ID") & "' >" & IsNull(btn("Label"), btn("Name")) & "</button>")
                                    Case "generateexceltemplate"
                                        sb.Append("<button " & If(IsDBNull(btn("VisibleIf")), "", "ng-if='" & btn("VisibleIf").ToString & "'") & " type='button' class='btn btn-primary' ng-generate-excel-template " & If(IsNull(btn("ConfirmationDialog"), "") = "", "", "confirm=""" & HttpUtility.HtmlEncode(btn("ConfirmationDialog")) & """") & " btnid='" & btn("ID") & "' mid='" & btn("ID_WebMenus") & "' >" & IsNull(btn("Label"), btn("Name")) & "</button>")
                                    Case "importexceltemplate"
                                        Dim targetTab As Integer = 0
                                        For Each m As Menu In mCollection.GetChild(Me.MenuID).Where(Function(x) x.ColumnValue("IsVisible") = True)
                                            targetTab += 1
                                            If m.MenuID = CInt(IsNull(btn("ID_TargetWebMenus"), 0)) Then Exit For
                                        Next
                                        sb.Append("<div " & If(IsDBNull(btn("VisibleIf")), "", "ng-if='" & btn("VisibleIf").ToString & "'") & " class='btn btn-primary btn-file-upload'>" & IsNull(btn("Label"), btn("Name")) & "<input type='file' ng-model='xxx' name='import-excel' file-input ng-file-select='onExcelSelect($files," & Me.MenuID & "," & btn("ID") & "," & IsNull(btn("ID_TargetWebMenus"), "0") & "," & (targetTab + Me.ColumnValue("TabsCount") - 1) & ")'></div>")
                                    Case "printreport"
                                        sb.Append("<button " & If(IsDBNull(btn("VisibleIf")), "", "ng-if='" & btn("VisibleIf").ToString & "'") & " type='button' class='btn btn-primary' ng-form-print-report btnid='" & btn("ID") & "' mid='" & btn("ID_WebMenus") & "' >" & IsNull(btn("Label"), btn("Name")) & "</button>")
                                    Case "async"
                                        sb.Append("<button " & If(IsDBNull(btn("VisibleIf")), "", "ng-if='" & btn("VisibleIf").ToString & "'") & " type='button' class='btn btn-primary' ng-form-async-command " & If(IsNull(btn("ConfirmationDialog"), "") = "", "", "confirm=""" & HttpUtility.HtmlEncode(btn("ConfirmationDialog")) & """") & " btnid='" & btn("ID") & "' >" & IsNull(btn("Label"), btn("Name")) & "</button>")
                                    Case Else
                                        sb.Append("<button " & If(IsDBNull(btn("VisibleIf")), "", "ng-if='" & btn("VisibleIf").ToString & "'") & " type='button' class='btn btn-primary' ng-form-special-command " & If(IsNull(btn("ConfirmationDialog"), "") = "", "", "confirm=""" & HttpUtility.HtmlEncode(btn("ConfirmationDialog")) & """") & " btnid='" & btn("ID") & "' >" & IsNull(btn("Label"), btn("Name")) & "</button>")
                                End Select
                                sb.Append("</li>")
                            Next
                        End If

                        sb.Append("</ol>")
                        sb.Append("</div>")
                        sb.Append("<div class='clear'></div>")

                        sb.Append("<div ng-model='tabs.activeTab' bs-tabs>")

                        For Each mTab As DataRow In Me.dtTabs.Select("", "SeqNo ASC")
                            sb.Append("<div title='" & mTab("Name") & "' name='master_" & mTab("ID") & "' ng-form  bs-pane>")

                            sb.Append("<div class='form_template form-horizontal' >")
                            sb.Append("<fieldset>")
                            sb.Append("<br/>")
                            'sb.Append("<legend>" & mTab("Name") & "</legend>")

                            If CInt(mTab("GetNumberOfColumns").ToString) >= 2 Then
                                Dim left As New Text.StringBuilder,
                                    right As New Text.StringBuilder
                                left.Append("<div class='col-sm-6'>")
                                right.Append("<div class='col-sm-6'>")
                                For Each dr As DataRow In Me.dtColumns.Select("IsVisible = 1 AND ISNULL(IsTitle,0) = 0 AND ID_WebMenuControlTypes IS NOT NULL AND ID_WebMenuTabs = " & mTab("ID"), "SeqNo ASC")
                                    If dr("Position") = 1 Then
                                        If IsNull(dr("VisibleIf"), "") <> "" AndAlso dr("ID_WebMenuControlTypes") <> 21 Then
                                            left.Append("<div class='form-group animate-show' ng-if='" & dr("VisibleIf") & "'>")
                                        ElseIf dr("ID_WebMenuControlTypes") = 21 Then
                                            left.Append("<div class='form-group' ng-if='false'>")
                                        Else
                                            left.Append("<div class='form-group' " & If(CBool(dr("HasValidation")), " ng-class=""{ 'has-error' : master_" & mTab("ID") & "." & dr("Name") & ".$invalid && mainform.$submitted }"" ", "") & ">")
                                        End If
                                        If Not IsDBNull(dr("HeaderText")) Then
                                            left.Append("<label class='col-md-12 control-label bold' style='text-align:center!important;padding-bottom:5px;font-size:14px;'>" & dr("HeaderText") & "</label>")
                                        End If
                                        left.Append("<label style='border-right:1px dashed #ddd;' class='" & If(dr("ID_WebMenuControlTypes") = 14, "control-label2 ", "control-label ") & " col-md-2 bold' " &
                                                        If(IsDBNull(dr("HelperText")), "", "data-trigger='click' data-type='success' data-title='" & addStripSlashes(dr("HelperText").ToString) & "' bs-tooltip") & ">" &
                                                        If(IsDBNull(dr("HelperText")), "", "<i class='fa fa-question-circle helper-text'></i> ") &
                                                        IsNull(dr("Label"), dr("Name")) & If(CBool(dr("IsRequired")) AndAlso IsDBNull(dr("RequiredIf")), "<span class='required'>* </span>", "") & "</label>")
                                        left.Append("<div class='col-md-10'>")
                                        left.Append(buildHTMLControl(dr))
                                        buildColumnValidation(dr, left)
                                        left.Append("</div>")
                                        left.Append("</div>")
                                    Else
                                        If IsNull(dr("VisibleIf"), "") <> "" AndAlso dr("ID_WebMenuControlTypes") <> 21 Then
                                            right.Append("<div class='form-group animate-show' ng-if='" & dr("VisibleIf") & "'>")
                                        ElseIf dr("ID_WebMenuControlTypes") = 21 Then
                                            right.Append("<div class='form-group' ng-if='false'>")
                                        Else
                                            right.Append("<div class='form-group' " & If(CBool(dr("HasValidation")), " ng-class=""{ 'has-error' : master_" & mTab("ID") & "." & dr("Name") & ".$invalid && mainform.$submitted }"" ", "") & ">")
                                        End If
                                        If Not IsDBNull(dr("HeaderText")) Then
                                            right.Append("<label class='col-md-12 control-label bold' style='text-align:center!important;padding-bottom:5px;font-size:14px;'>" & dr("HeaderText") & "</label>")
                                        End If
                                        right.Append("<label style='border-right:1px dashed #ddd;' class='" & If(dr("ID_WebMenuControlTypes") = 14, "control-label2 ", "control-label ") & "col-md-2 bold' " &
                                                        If(IsDBNull(dr("HelperText")), "", "data-trigger='click' data-type='success' data-title='" & addStripSlashes(dr("HelperText").ToString) & "' bs-tooltip") & ">" &
                                                        If(IsDBNull(dr("HelperText")), "", "<i class='fa fa-question-circle helper-text'></i> ") &
                                                        IsNull(dr("Label"), dr("Name")) & If(CBool(dr("IsRequired")) AndAlso IsDBNull(dr("RequiredIf")), "<span class='required'>* </span>", "") & "</label>")
                                        right.Append("<div class='col-md-10'>")
                                        right.Append(buildHTMLControl(dr))
                                        buildColumnValidation(dr, right)
                                        right.Append("</div>")
                                        right.Append("</div>")
                                    End If
                                Next
                                left.Append("</div>")
                                right.Append("</div>")
                                sb.Append(left.ToString)
                                sb.Append(right.ToString)
                            Else
                                sb.Append("<div class='col-sm-12'>")
                                For Each dr As DataRow In Me.dtColumns.Select("IsVisible = 1 AND ISNULL(IsTitle,0) = 0 AND ID_WebMenuControlTypes IS NOT NULL AND ID_WebMenuTabs = " & mTab("ID"), "SeqNo ASC")
                                    If IsNull(dr("VisibleIf"), "") <> "" AndAlso dr("ID_WebMenuControlTypes") <> 21 Then
                                        sb.Append("<div class='form-group animate-show' ng-if='" & dr("VisibleIf") & "'>")
                                    ElseIf dr("ID_WebMenuControlTypes") = 21 Then
                                        sb.Append("<div class='form-group' ng-if='false'>")
                                    Else
                                        sb.Append("<div class='form-group' " & If(CBool(dr("HasValidation")), " ng-class=""{ 'has-error' : master_" & mTab("ID") & "." & dr("Name") & ".$invalid && mainform.$submitted }"" ", "") & " >")
                                    End If
                                    If Not IsDBNull(dr("HeaderText")) Then
                                        sb.Append("<label class='col-md-12 control-label bold' style='text-align:center!important;padding-bottom:5px;font-size:14px;'>" & dr("HeaderText") & "</label>")
                                    End If
                                    sb.Append("<label style='border-right:1px dashed #ddd;' class='" & If(dr("ID_WebMenuControlTypes") = 14, "control-label2 ", "control-label ") & " col-md-2 bold' " &
                                                    If(IsDBNull(dr("HelperText")), "", "data-trigger='click' data-type='success' data-title='" & addStripSlashes(dr("HelperText").ToString) & "' bs-tooltip") & ">" &
                                                    If(IsDBNull(dr("HelperText")), "", "<i class='fa fa-question-circle helper-text'></i> ") &
                                                    IsNull(dr("Label"), dr("Name")) & If(CBool(dr("IsRequired")) AndAlso IsDBNull(dr("RequiredIf")), "<span class='required'>* </span>", "") & "</label>")
                                    sb.Append("<div class='col-md-10'>")
                                    sb.Append(buildHTMLControl(dr))
                                    buildColumnValidation(dr, sb)
                                    sb.Append("</div>")
                                    sb.Append("</div>")
                                Next
                                sb.Append("</div>")
                            End If

                            sb.Append("</fieldset> ")
                            sb.Append("</div>")
                            sb.Append("</div>")
                        Next


                        Dim detail As Integer = 1
                        For Each m As Menu In mCollection.GetChild(Me.MenuID).Where(Function(x) x.ColumnValue("IsVisible") = True)
                            'sb.append("<div title='" & m.Name & "' ng-show='" & If(hasGenerateButton, "true", If(IsDBNull(m.ColumnValue("VisibleIf")), "Master.ID > 0", "Master.ID > 0 && " & m.ColumnValue("VisibleIf"))) & "'  bs-pane>")
                            sb.Append("<div title='" & m.Name & "' " & If(IsDBNull(m.ColumnValue("VisibleIf")), "", "ng-show='{{" & m.ColumnValue("VisibleIf") & "}}'") & " name='detail-form-" & detail & "' select='loadTab(" & m.MenuID & ")' ng-form bs-pane>")
                            detail += 1

                            sb.Append("<div class='button-holder'>")
                            If CBool(m.ColumnValue("HasNew")) Then
                                Select Case m.ColumnValue("ID_WebMenuTypes")
                                    Case 12
                                        sb.Append("<button " & If(IsDBNull(m.ColumnValue("HasNewIf")), "", "ng-if='" & m.ColumnValue("HasNewIf") & "'") & " type='button' class='btn btn-primary' ng-form-new mid='" & m.MenuID & "' mtype='12' ><i class='fa fa-file-o'></i>New</button>")
                                    Case 14
                                        sb.Append("<button " & If(IsDBNull(m.ColumnValue("HasNewIf")), "", "ng-if='" & m.ColumnValue("HasNewIf") & "'") & " type='button' class='btn btn-primary' ng-add-new-row mid='" & m.MenuID & "' mtype='14' ><i class='fa fa-file-o'></i>New</button>")
                                End Select
                            End If
                            If m.ColumnValue("ID_WebMenuTypes") = 18 Then
                                sb.Append("<button type='button' class='btn btn-primary' ng-load-list mid='" & m.MenuID & "'><i class='fa fa-file-o'></i>Load List</button>")
                            End If
                            sb.Append("</div>")
                            sb.Append("<div style='width:99%;margin:0px auto;'>")
                            If m.ColumnValue("ID_WebMenuTypes") = 18 Then
                                sb.Append("<div style='width:100%;" & If(IsDBNull(m.ColumnValue("Height")), "height:400px;", "height:" & m.ColumnValue("Height") & "px;") & "' m-tree-view='treeOptions[" & m.MenuID & "]'></div>")
                            Else
                                sb.Append("<div style='width:100%;" & If(IsDBNull(m.ColumnValue("Height")), "height:400px;", "height:" & m.ColumnValue("Height") & "px;") & "' class='grid' m-grid='gridOptions[" & m.MenuID & "]'></div>")
                            End If
                            sb.Append("</div>")
                            sb.Append("</div>")
                        Next

                        sb.Append("</div>")

                        If Me.ColumnValue("ID_WebMenuTypes") <> 3 Then
                            sb.Append("</div>")
                        End If
                        sb.Append("</div>")
                        sb.Append("</div>")
                End Select
            Catch ex As Exception
                logError(ex, Me.MenuID)
            End Try
            Return sb.ToString
        End Function

        Public Function getDefaultFormData() As Dictionary(Of String, Object)

            If (defaultFormData.Count = 0 AndAlso Me.dtColumns.Select("IsVisible = 1 AND IsNull(IsTitle,0) = 0 AND Name <> 'ID' AND (ID_WebMenuControlTypes NOT IN (27) OR ID_WebMenuControlTypes IS NULL)").Count > 0) Or (defaultFormData.Count - 1 <> Me.dtColumns.Select("IsVisible = 1 AND IsNull(IsTitle,0) = 0 AND Name <> 'ID'  AND (ID_WebMenuControlTypes NOT IN (27) OR ID_WebMenuControlTypes IS NULL)").Count) Then
                Dim dt As DataTable = Me.dtColumns
                defaultFormData.Add("ID", 0)
                For Each dr As DataRow In Me.dtColumns.Select("IsVisible = 1 AND ISNULL(IsTitle,0) = 0 AND Name <> 'ID'  AND (ID_WebMenuControlTypes NOT IN (27) OR ID_WebMenuControlTypes IS NULL)")
                    'If dr("Name").ToString.ToLower = "id" Then
                    '    defaultFormData.Add(dr("Name"), 0)
                    'Else
                    If dr("colDatatype").ToString = "bit" Then
                        defaultFormData.Add(dr("Name"), If(IsDBNull(dr("DefaultValue")), dr("DefaultValue"), If(dr("DefaultValue").ToString = "1", "true", "false")))
                    Else
                        defaultFormData.Add(dr("Name"), dr("DefaultValue"))
                    End If

                    ' End If

                Next
            End If

            Return defaultFormData

        End Function

        Public Function getDefaultRowData() As Dictionary(Of String, Object)

            If (defaultRowData.Count = 0 AndAlso Me.dtColumns.Select("ShowInList = 1 AND IsNull(IsTitle,0) = 0 AND Name <> 'id'").Count > 0) Or (defaultRowData.Count - 1 <> Me.dtColumns.Select("ShowInList = 1 AND IsNull(IsTitle,0) = 0 AND Name <> 'id'").Count) Then
                Dim dt As DataTable = Me.dtColumns.Select("ShowInList = 1 AND ISNULL(IsTitle,0) = 0 AND Name <> 'id'").CopyToDataTable
                defaultRowData.Add("ID", 0)
                For Each dr As DataRow In dt.Rows
                    'If dr("Name").ToString.ToLower = "id" Then
                    '    defaultRowData.Add(dr("Name"), 0)
                    'Else
                    If dr("colDatatype").ToString = "bit" Then
                        defaultRowData.Add(dr("Name"), If(IsDBNull(dr("DefaultValue")), dr("DefaultValue"), If(dr("DefaultValue").ToString = "1", "true", "false")))
                    Else
                        defaultRowData.Add(dr("Name"), dr("DefaultValue"))
                    End If
                    'End If

                Next
            End If

            Return defaultRowData
        End Function

        Public Function getListColumns() As String
            If (ListColumns.Length = 0 AndAlso Me.dtColumns.Select("Name <> 'ID' AND ShowInList = 1 AND IsNull(isTitle,0) = 0 AND (ID_WebMenuControlTypes NOT IN(6, 27) OR ID_WebMenuControlTypes IS NULL)").Count > 0) Then
                Dim result As New Text.StringBuilder
                Dim cols As DataRow() = Me.dtColumns.Select("Name <> 'ID' AND ShowInList = 1 AND IsNull(isTitle,0) = 0 AND (ID_WebMenuControlTypes NOT IN(6) OR ID_WebMenuControlTypes IS NULL)")
                For x = 0 To cols.Length - 1
                    If Not IsDBNull(cols(x)("ID_WebMenuControlTypes")) Then
                        If cols(x)("ID_WebMenuControlTypes") = 4 Then
                            result.Append("convert(varchar(10),[" & cols(x)("Name") & "],101) as [" & cols(x)("Name") & "],")
                            'ElseIf cols(x)("ID_WebMenuControlTypes") = 5 Then
                            '    result.Append("cast([" & cols(x)("Name") & "] as Time) as [" & cols(x)("Name") & "],")
                        Else
                            result.Append("[" & cols(x)("Name") & "],")
                        End If
                    Else
                        result.Append("[" & cols(x)("Name") & "],")
                    End If

                    If Not IsDBNull(cols(x)("ID_WebMenuControlTypes")) Then
                        If cols(x)("ID_WebMenuControlTypes") = 23 Or cols(x)("ID_WebMenuControlTypes") = 2 Or cols(x)("ID_WebMenuControlTypes") = 17 Or cols(x)("ID_WebMenuControlTypes") = 35 Then
                            result.Append("[" & cols(x)("Name").ToString.Substring(3) & "],")
                        ElseIf cols(x)("ID_WebMenuControlTypes") = 25 Then
                            result.Append("[" & cols(x)("Name").ToString & "_GUID],")
                        End If
                    End If

                Next x
                ListColumns = "[ID]," & result.ToString.Trim(",")
            End If

            Return ListColumns
        End Function

        Public Function getFormColumns() As String
            If (FormColumns.Length = 0 AndAlso Me.dtColumns.Select("Name <> 'ID' AND IsVisible = 1 AND IsNull(isTitle,0) = 0 AND (ID_WebMenuControlTypes NOT IN(6, 27) OR ID_WebMenuControlTypes IS NULL)").Count > 0) Then
                Dim result As New Text.StringBuilder
                Dim cols As DataRow() = Me.dtColumns.Select("Name <> 'ID' AND IsVisible = 1 AND IsNull(isTitle,0) = 0 AND (ID_WebMenuControlTypes NOT IN(6) OR ID_WebMenuControlTypes IS NULL)")
                For x = 0 To cols.Length - 1
                    If Not IsDBNull(cols(x)("ID_WebMenuControlTypes")) Then
                        If cols(x)("ID_WebMenuControlTypes") = 4 Then
                            result.Append("convert(varchar(10),[" & cols(x)("Name") & "],101) as [" & cols(x)("Name") & "],")
                            'ElseIf cols(x)("ID_WebMenuControlTypes") = 5 Then
                            '    result.Append("cast([" & cols(x)("Name") & "] as Time) as [" & cols(x)("Name") & "],")
                        Else
                            result.Append("[" & cols(x)("Name") & "],")
                        End If
                    Else
                        result.Append("[" & cols(x)("Name") & "],")
                    End If
                    If Not IsDBNull(cols(x)("ID_WebMenuControlTypes")) Then
                        If cols(x)("ID_WebMenuControlTypes") = 23 Or cols(x)("ID_WebMenuControlTypes") = 2 Or cols(x)("ID_WebMenuControlTypes") = 17 Then
                            result.Append("[" & cols(x)("Name").ToString.Substring(3) & "],")
                        ElseIf cols(x)("ID_WebMenuControlTypes") = 25 Then
                            result.Append("[" & cols(x)("Name").ToString & "_GUID],")
                        End If
                    End If

                Next x
                FormColumns = "[ID]," & result.ToString.Trim(",")
            End If

            Return FormColumns
        End Function


        'Public Function GetRow(ByVal ID As Integer, Optional ByVal buttonID As Integer = 0, Optional ByVal parentID As Integer = 0, Optional ByVal SessionVariables As System.Web.SessionState.HttpSessionState = Nothing) As DataRow
        '    If mRow("DataSource").ToString = "" Then Return Nothing
        '    Dim dsView As New GSWEB.SQL.DataSource
        '    'mConnection.rID = ID
        '    dsView.SetCommandText("Select * FROM " + SetDataSourceFilter(Me, buttonID, parentID, SessionVariables) + " WHERE ID = " + ID.ToString)
        '    dsView.SetCommandType(CommandType.Text)
        '    Dim rslt As DataRow = Nothing
        '    Try
        '        If dsView.Connect Then
        '            Dim dt As DataTable = New DataTable
        '            dt.Load(dsView.ExecuteReader)
        '            For Each dr As DataRow In dt.Rows
        '                rslt = dr
        '                Exit For
        '            Next
        '        End If
        '    Catch ex As Exception
        '        Throw ex
        '    Finally
        '        dsView.Disconnect()
        '        dsView = Nothing
        '    End Try
        '    Return rslt
        'End Function


        Public Function dtTabs() As DataTable

            'If columnsCollection.Count > 0 AndAlso ClearMennu Then
            '    columnsCollection.Clear()
            'End If

            Dim query = From mTabs In TabsCollection _
                    Where mTabs.Item("ID_WebMenus") = Me.MenuID _
                    Order By mTabs.Item("SeqNo")

            If (query Is Nothing And Me.ColumnValue("TabsCount") > 0) OrElse query.Count <> Me.ColumnValue("TabsCount") Then LoadTabs(Me.MenuID)

            If query.Count > 0 Then
                Return query.CopyToDataTable
            Else
                Return Nothing
            End If

        End Function

        Public Function dtColumns() As DataTable

            'If columnsCollection.Count > 0 AndAlso ClearMennu Then
            '    columnsCollection.Clear()
            'End If

            Dim query = From mColumns In columnsCollection _
                    Where mColumns.Item("ID_WebMenus") = Me.MenuID _
                    Order By mColumns.Item("SeqNo")

            If (query Is Nothing And Me.ColumnValue("ColumnCount") > 0) OrElse query.Count <> Me.ColumnValue("ColumnCount") Then LoadColumns(Me.MenuID)

            'If (Not query Is Nothing OrElse Not query.Count = 0) AndAlso query.Count <> Me.ColumnValue("ColumnCount") Then
            '    LogEvent("Load Columns -> Menu ID : " & Me.MenuID & ", Query Count : " & query.Count & ", Column Count : " & Me.ColumnValue("ColumnCount"))
            'End If

            If query.Count > 0 Then
                Return query.CopyToDataTable
            Else
                Return Nothing
            End If

        End Function

        'Public Function dtSubDataSource() As DataTable
        '    Dim query = From mSubDataSource In menuSubDataSourceCollection _
        '        Where mSubDataSource.Item("ID_WebMenus") = Me.MenuID _
        '        Order By mSubDataSource.Item("SeqNo")

        '    If (query Is Nothing And Me.ColumnValue("SubDataSourceCount") > 0) OrElse query.Count <> Me.ColumnValue("SubDataSourceCount") Then LoadSubDataSource(Me.MenuID)

        '    If query.Count > 0 Then
        '        Return query.CopyToDataTable
        '    Else
        '        Return Nothing
        '    End If
        'End Function

        Public Function dtButtons() As DataTable

            'If buttonsCollection.Count > 0 AndAlso ClearMennu Then
            '    buttonsCollection.Clear()
            'End If

            Dim query = From mButtons In buttonsCollection _
                    Where mButtons.Item("ID_WebMenus") = Me.MenuID

            If (query Is Nothing And Me.ColumnValue("ButtonCount") > 0) OrElse query.Count <> Me.ColumnValue("ButtonCount") Then LoadButtons(Me.MenuID)

            If (Not query Is Nothing OrElse Not query.Count = 0) AndAlso query.Count <> Me.ColumnValue("ButtonCount") Then
                LogEvent("Load Buttons -> Menu ID : " & Me.MenuID & ", Query Count : " & query.Count & ", Button Count : " & Me.ColumnValue("ButtonCount"))
            End If

            If query.Count > 0 Then
                Return query.CopyToDataTable()
            Else
                Return Nothing
            End If

        End Function

        Public Function dtButtonValidations(ByVal ButtonID As Integer) As DataTable
            'If buttonsValCollection.Count > 0 AndAlso ClearMennu Then
            '    buttonsValCollection.Clear()
            'End If

            Dim query = From mButtonsVal In buttonsValCollection _
                    Where mButtonsVal.Item("ID_WebMenuButtons") = ButtonID

            Dim valCount As Integer = 0
            For Each dr As DataRow In buttonsCollection.GetObject(ButtonID).ToArray
                valCount = dr("ValidationCount")
            Next

            If (query Is Nothing And valCount > 0) OrElse query.Count <> valCount Then LoadButtonValidations(ButtonID)

            If query.Count > 0 Then
                Return query.CopyToDataTable
            Else
                Return Nothing
            End If

        End Function

        Public Function dtButtonParameters(ByVal ButtonID As Integer) As DataTable
            'If buttonsParamCollection.Count > 0 AndAlso ClearMennu Then
            '    buttonsParamCollection.Clear()
            'End If

            Dim query = From mButtonParam In buttonsParamCollection _
                    Where mButtonParam.Item("ID_WebMenuButtons") = ButtonID

            Dim paramCount As Integer = 0
            For Each dr As DataRow In buttonsCollection.GetObject(ButtonID).ToArray
                paramCount = dr("ParameterCount")
            Next

            If (query Is Nothing And paramCount > 0) OrElse query.Count <> paramCount Then LoadButtonParameters(ButtonID)

            If query.Count > 0 Then
                Return query.CopyToDataTable
            Else
                Return Nothing
            End If

        End Function

        Public Function dtColumnValidation(ByVal ColumnID As Integer) As DataTable
            'If buttonsParamCollection.Count > 0 AndAlso ClearMennu Then
            '    buttonsParamCollection.Clear()
            'End If

            Dim query = From mColumnValidation In columnsValCollection _
                    Where mColumnValidation.Item("ID_WebMenuColumns") = ColumnID

            Dim validationCount As Integer = 0
            For Each dr As DataRow In columnsCollection.GetObject(ColumnID).ToArray
                validationCount = dr("ValidationCount")
            Next

            If (query Is Nothing And validationCount > 0) OrElse query.Count <> validationCount Then LoadColumnValidation(ColumnID)

            If query.Count > 0 Then
                Return query.CopyToDataTable
            Else
                Return Nothing
            End If

        End Function

        'Public Function dtWidgets() As DataTable
        '    'If mWidgets.Count > 0 AndAlso ClearMennu Then
        '    '    mWidgets.Clear()
        '    'End If

        '    Dim query = From mWids In mWidgets _
        '           Where mWids.Item("ID_WebMenus") = Me.MenuID _
        '           Order By mWids.Item("SeqNo")

        '    If (query Is Nothing And Me.ColumnValue("WidsCount") > 0) OrElse query.Count <> Me.ColumnValue("WidsCount") Then LoadWidgets(Me.MenuID)

        '    If query.Count > 0 Then
        '        Return query.CopyToDataTable
        '    Else
        '        Return Nothing
        '    End If

        'End Function

        'Public Function dtLinks() As DataTable
        '    'If mLinks.Count > 0 AndAlso ClearMennu Then
        '    '    mLinks.Clear()
        '    'End If

        '    Dim query = From ml In mLinks _
        '           Where ml.Item("ID_WebMenus") = Me.MenuID _
        '           Order By ml.Item("SeqNo")

        '    If (query Is Nothing And Me.ColumnValue("LinksCount") > 0) OrElse query.Count <> Me.ColumnValue("LinksCount") Then LoadMenuLinks(Me.MenuID)

        '    If query.Count > 0 Then
        '        Return query.CopyToDataTable
        '    Else
        '        Return Nothing
        '    End If

        'End Function

        'Public Sub Refresh(Optional ByVal ViewFilter As String = "")
        '    If mRow("View").ToString = "" Then Exit Sub
        '    Try
        '        Dim dtRf As DataTable = Me.dtDataSource(ViewFilter)
        '    Catch ex As Exception
        '    Finally
        '        mConnection.Disconnect()
        '    End Try
        'End Sub



        ''PAG Access Rights Gustong nsa memory
        'Private Sub LoadAccessRights()
        '    Try
        '        AccessRights = getTable("SELECT ID_UserGroup FROM dbo.tUserGroupWebMenus WHERE ID_WebMenus = " + MenuID.ToString).AsEnumerable().Select(Function(r) CInt(r(0))).ToList()
        '    Catch ex As Exception
        '        Throw ex
        '    End Try
        'End Sub

        Public Function Save(connString As String, rID As Integer, master As Dictionary(Of String, Object), detail As DataSet, fileSummary As List(Of GSWEB.Utility.fileSummary), fileData As MultipartFormDataStreamProvider) As Integer
            Dim id As Integer = 0

            Using sqlConn As SqlClient.SqlConnection = New SqlClient.SqlConnection(connString)
                sqlConn.Open()
                Dim cols As List(Of String) = If(Me.TableColumns().Length > 0, Me.TableColumns().Split(",").ToList, New List(Of String)),
                    childCol As String = "ID_" & Me.ColumnValue("TableName").ToString.Substring(1)
                Dim transName As String = Me.MenuID & "_" & DateTime.Now.Ticks
                Dim trans As SqlClient.SqlTransaction = sqlConn.BeginTransaction(IsolationLevel.ReadCommitted, transName)
                Using sqlCommand As SqlClient.SqlCommand = New SqlClient.SqlCommand
                    Try
                        Dim cmdtxt As String = String.Empty
                        sqlCommand.Connection = sqlConn
                        sqlCommand.Transaction = trans
                        If Me.dtColumns.Select("ID_WebMenuControlTypes = 25 or ID_WebMenuControlTypes = 30 AND IsVisible = 1").Count = 0 OrElse fileSummary.Where(Function(x) x.mID = Me.MenuID And x.idx = rID).Count = 0 Then
                            cmdtxt = If(rID = 0, Me.GETFORMINSERTCommand(), Me.GETFORMUPDATECommand)
                        Else
                            Dim columns As String = getMenuColParameters(Me.MenuID, , False)
                            Dim fileUpload As String = Me.dtColumns.Select("ID_WebMenuControlTypes = 25 or ID_WebMenuControlTypes = 30 AND IsVisible = 1").CopyToDataTable.Rows(0)("Name") '' TODO: maraming columns na upload file 
                            If rID = 0 Then
                                cmdtxt = "INSERT INTO " + mRow("TableName").ToString + "(" + columns.Replace("@", "") + "," + fileUpload + "," + fileUpload + "_GUID)" + _
                                         " VALUES(" + columns + ",@" + fileUpload + "," + "@" + fileUpload + "_GUID); SELECT CAST(scope_identity() AS int)"
                            Else
                                Dim tmp As String() = columns.Split(",")
                                Dim str As New System.Text.StringBuilder

                                If columns.Length > 0 Then
                                    For Each col As String In tmp
                                        str.Append(col.Replace("@", "") + " = " + col + ",")
                                    Next
                                End If

                                str.Append(fileUpload + " = @" + fileUpload + ",")
                                str.Append(fileUpload + "_GUID = @" + fileUpload + "_GUID,")
                                cmdtxt = "UPDATE " + mRow("TableName").ToString + " SET " + str.ToString.Trim(",") + " WHERE ID = @ID; SELECT CAST(@ID AS int)"
                            End If
                        End If


                        sqlCommand.CommandText = cmdtxt

                        For Each col As String In cols
                            Dim datatype As System.Type = GetDataType(Me.dtColumns.Select("Name = '" + col + "'")(0).Item("colDataType").ToString)
                            Dim i As Object = master.Item(col)
                            If i Is Nothing OrElse IsDBNull(i) Then
                                sqlCommand.Parameters.AddWithValue("@" + col, System.DBNull.Value)
                            Else
                                'If i.ToString = "0" AndAlso CBool(Me.dtColumns.Select("Name = '" + col + "'")(0).Item("IsForeignKey")) Then
                                '    sqlCommand.Parameters.AddWithValue("@" + col, System.DBNull.Value)
                                'Else
                                If col.ToString.ToLower.Contains("time") Then
                                    'sqlCommand.Parameters.AddWithValue("@" + col, CTypeDynamic(Now.Date.ToShortDateString & " " & i.ToString, datatype))
                                    sqlCommand.Parameters.AddWithValue("@" + col, CTypeDynamic(i.ToString, datatype))
                                Else
                                    sqlCommand.Parameters.AddWithValue("@" + col, CTypeDynamic(i.ToString.Replace(Chr(160), Chr(32)), datatype))
                                End If
                            End If
                        Next

                        ''FILE UPLOAD 
                        ''PAG FILE UPLOAD KELANGAN "@columnName_GUID" na COLUMN.
                        If Me.dtColumns.Select("ID_WebMenuControlTypes = 25 or ID_WebMenuControlTypes = 30 AND IsVisible = 1").Count > 0 AndAlso fileSummary.Where(Function(x) x.mID = Me.MenuID And x.idx = rID).Count > 0 Then
                            Dim fileUpload As String = Me.dtColumns.Select("ID_WebMenuControlTypes = 25 or ID_WebMenuControlTypes = 30 AND IsVisible = 1").CopyToDataTable.Rows(0)("Name"),
                                fileGUID As String = Guid.NewGuid.ToString,
                                idx = fileSummary.FindIndex(Function(x) x.mID = Me.MenuID And x.idx = rID),
                                mfData As MultipartFileData = fileData.FileData.Item(idx),
                                uploadedFileInfo = New FileInfo(mfData.LocalFileName),
                                fileName As String = String.Empty
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
                            File.Move(mfData.LocalFileName, Path.Combine(HttpContext.Current.Server.MapPath("~/Upload/"), fileName))

                            Dim fileExtension As String = Path.GetExtension(fileName).ToLower,
                                extensions As String() = {".png", ".jpg", ".bmp", ".jpeg", ".gif", ".tiff"}
                            If extensions.Contains(fileExtension) Then
                                Dim tmpFile As String = Guid.NewGuid.ToString() & Path.GetExtension(fileName)
                                ResizeImage(Path.Combine(HttpContext.Current.Server.MapPath("~/Upload/"), fileName), Path.Combine(HttpContext.Current.Server.MapPath("~/Upload/"), tmpFile), 200)
                                File.Delete(Path.Combine(HttpContext.Current.Server.MapPath("~/Upload/"), fileName))
                                fileName = tmpFile
                            End If



                            sqlCommand.Parameters.AddWithValue("@" + fileUpload, master.Item(fileUpload))
                            sqlCommand.Parameters.AddWithValue("@" + fileUpload + "_GUID", fileName)
                        End If

                        If rID > 0 Then
                            sqlCommand.Parameters.AddWithValue("@ID", rID)
                        End If
                        id = sqlCommand.ExecuteScalar

                        ''INSERT OR UPDATE FOR DETAILED LOOKUP
                        'Dim cmdDL As String = String.Empty
                        'Dim menuDL As GSWEB.MenuCollection.Menu = mCollection.GetMenu(Me.MenuID)
                        'Dim colDL As New DataTable
                        'If menuDL.dtColumns.Select("ID_WebMenuControlTypes = 35 and IsVisible = 1").Length > 0 Then
                        '    colDL = menuDL.dtColumns.Select("ID_WebMenuControlTypes = 35 and IsVisible = 1").CopyToDataTable
                        'End If

                        'If colDL.Rows.Count > 0 Then
                        '    If rID = 0 Then
                        '        If colDL.Rows.Count > 0 Then
                        '            For Each DL In colDL.Rows
                        '                Dim v As String = String.Empty, val As New List(Of String)
                        '                v = master.Item(DL("Name")).ToString.Replace("[", "").Replace("]", "")
                        '                val = v.Split(",").ToList
                        '                cmdDL &= "INSERT INTO " & DL("TableName") & " (" & DL("Name") & ", ID_" & menuDL.TableName.ToString.Substring(1) & ")  VALUES"
                        '                For Each value In val
                        '                    value = Regex.Replace(value, "\r\n", String.Empty, RegexOptions.Compiled Or RegexOptions.Multiline)
                        '                    value = value.ToString.Replace(" ", "")
                        '                    cmdDL &= "(" & value & "," & id.ToString & "),"
                        '                Next
                        '                cmdDL = cmdDL.ToString.Substring(0, cmdDL.Length - 1) & ";"
                        '            Next
                        '        End If
                        '    Else
                        '        If colDL.Rows.Count > 0 Then
                        '            For Each DL In colDL.Rows
                        '                Dim v As String = String.Empty, val As New List(Of String)
                        '                Dim delcmd As String = String.Empty, inscmd As String = String.Empty
                        '                v = master.Item(DL("Name")).ToString.Replace("[", "").Replace("]", "")
                        '                v = Regex.Replace(v, "\r\n", String.Empty, RegexOptions.Compiled Or RegexOptions.Multiline).Replace(" ", "")
                        '                If v <> "" Then
                        '                    val = v.Split(",").ToList
                        '                End If
                        '                If val.Count > 0 Then
                        '                    delcmd = "DELETE FROM " & DL("TableName") & " WHERE " & DL("Name") & " NOT IN (" & String.Join(",", val) & ") AND ID_" & menuDL.TableName.ToString.Substring(1) & " = " & id.ToString & ";"
                        '                Else
                        '                    delcmd = "DELETE FROM " & DL("TableName") & " WHERE  ID_" & menuDL.TableName.ToString.Substring(1) & " = " & id.ToString & ";"
                        '                End If
                        '                Dim existDt As List(Of String) = getTable("SELECT " & DL("Name") & " FROM " & DL("TableName") & " WHERE ID_" & menuDL.TableName.ToString.Substring(1) & " = " & id.ToString & " ").Select().AsEnumerable.Select(Function(x) x(DL("Name")).ToString).ToList
                        '                Dim newID As New List(Of String)
                        '                For Each d In val
                        '                    If existDt.Where(Function(x) x = d).Count = 0 Then
                        '                        newID.Add(d)
                        '                    End If
                        '                Next
                        '                If newID.Count > 0 Then
                        '                    inscmd = "INSERT INTO " & DL("TableName") & " (" & DL("Name") & ", ID_" & menuDL.TableName.ToString.Substring(1) & ")  VALUES"
                        '                    For Each value In newID
                        '                        inscmd &= "(" & value & "," & id.ToString & "),"
                        '                    Next
                        '                    inscmd = inscmd.ToString.Substring(0, inscmd.Length - 1) & ";"
                        '                Else
                        '                    inscmd = ""
                        '                End If
                        '                cmdDL &= delcmd & inscmd
                        '            Next
                        '        End If
                        '    End If

                        '    If IsNull(cmdDL, "") <> "" Then
                        '        sqlCommand.CommandText = cmdDL
                        '        sqlCommand.ExecuteNonQuery()
                        '    End If
                        'End If

                        For Each m2 As GSWEB.MenuCollection.Menu In mCollection.GetChild(Me.MenuID)
                            Dim dt As DataTable = detail.Tables(m2.MenuID.ToString)
                            If m2.ColumnValue("ID_WebMenuTypes") = 18 Then

                            End If
                            Dim colsChild As List(Of String) = Nothing
                            If m2.TableColumns(True).Length > 0 Then
                                colsChild = m2.TableColumns(True).Split(",").ToList()
                            Else
                                Continue For
                            End If
                            For Each dr As DataRow In dt.Select
                                sqlCommand.Parameters.Clear()
                                cmdtxt = String.Empty
                                If m2.dtColumns.Select("(ID_WebMenuControlTypes = 25 or ID_WebMenuControlTypes = 30) AND ShowInList = 1").Count = 0 OrElse fileSummary.Where(Function(x) x.mID = m2.MenuID And x.idx = dt.Rows.IndexOf(dr)).Count = 0 Then
                                    cmdtxt = If(IsNull(dr("ID"), 0) = 0, m2.GETGRIDINSERTCommand(), m2.GETGRIDUPDATECommand)
                                Else
                                    Dim columns As String = getMenuColParameters(m2.MenuID, , True)
                                    Dim fileUpload As String = m2.dtColumns.Select("ID_WebMenuControlTypes = 25 or ID_WebMenuControlTypes = 30 AND ShowInList = 1").CopyToDataTable.Rows(0)("Name") '' TODO: maraming columns na upload file 
                                    If dr("ID") = 0 Then
                                        cmdtxt = "INSERT INTO " + m2.ColumnValue("TableName").ToString + "(" + columns.Replace("@", "") + "," + fileUpload + "," + fileUpload + "_GUID)" + _
                                                 " VALUES(" + columns + ",@" + fileUpload + ",@" + fileUpload + "_GUID); SELECT CAST(scope_identity() AS int)"
                                    Else
                                        Dim tmp As String() = columns.Split(",")
                                        Dim str As New System.Text.StringBuilder

                                        If columns.Length > 0 Then
                                            For Each col As String In tmp
                                                str.Append(col.Replace("@", "") + " = " + col + ",")
                                            Next
                                        End If

                                        str.Append(fileUpload + " = @" + fileUpload + ",")
                                        str.Append(fileUpload + "_GUID = @" + fileUpload + "_GUID,")
                                        cmdtxt = "UPDATE " + m2.ColumnValue("TableName").ToString + " SET " + str.ToString.Trim(",") + " WHERE ID = @ID; SELECT CAST(@ID AS int)"
                                    End If
                                End If

                                sqlCommand.CommandText = cmdtxt

                                'get Parent Column in Columns Collection kng meron -EMIL 20150610
                                '''''''''''''''''''''''''''''''''''''''''''''''''''
                                If m2.dtColumns.Columns.Contains("IsParentColumn") Then
                                    Dim drParentColumn As DataRow = m2.dtColumns.Select("IsParentColumn = 1").FirstOrDefault
                                    If Not drParentColumn Is Nothing Then childCol = drParentColumn.Item("Name").ToString
                                End If
                                '''''''''''''''''''''''''''''''''''''''''''''''''''
                                For Each col As String In colsChild.Where(Function(x) x <> childCol).ToList
                                    If dr(col).ToString = "" Then
                                        sqlCommand.Parameters.AddWithValue("@" & col, System.DBNull.Value)
                                    Else
                                        Dim dataType As System.Type = GetDataType(m2.dtColumns.Select("Name = '" + col + "'")(0).Item("colDataType").ToString)
                                        'sqlCommand.Parameters.AddWithValue("@" + col, CTypeDynamic(HttpUtility.HtmlEncode(dr(col).ToString.Replace(Chr(160), Chr(32))), dataType))
                                        sqlCommand.Parameters.AddWithValue("@" + col, CTypeDynamic(dr(col).ToString.Replace(Chr(160), Chr(32)), dataType))
                                    End If
                                Next

                                ''FILE UPLOAD 
                                If m2.dtColumns.Select("ID_WebMenuControlTypes = 25 or ID_WebMenuControlTypes = 30 AND ShowInList = 1").Count > 0 AndAlso fileSummary.Where(Function(x) x.mID = m2.MenuID And x.idx = dt.Rows.IndexOf(dr)).Count > 0 Then
                                    Dim fileUpload As String = m2.dtColumns.Select("ID_WebMenuControlTypes = 25 or ID_WebMenuControlTypes = 30 AND IsVisible = 1").CopyToDataTable.Rows(0)("Name"),
                                        fileGUID As String = Guid.NewGuid.ToString,
                                        idx = fileSummary.FindIndex(Function(x) x.mID = m2.MenuID And x.idx = dt.Rows.IndexOf(dr)),
                                        mfData As MultipartFileData = fileData.FileData.Item(idx),
                                        uploadedFileInfo = New FileInfo(mfData.LocalFileName),
                                        fileName As String = String.Empty
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
                                    File.Move(mfData.LocalFileName, Path.Combine(HttpContext.Current.Server.MapPath("~/Upload/"), fileName))

                                    sqlCommand.Parameters.AddWithValue("@" + fileUpload, dr(fileUpload))
                                    sqlCommand.Parameters.AddWithValue("@" + fileUpload + "_GUID", fileName)
                                End If

                                If IsNull(dr("ID"), 0) > 0 Then
                                    sqlCommand.Parameters.AddWithValue("@ID", dr("ID"))
                                End If

                                sqlCommand.Parameters.AddWithValue("@" + childCol, id)
                                sqlCommand.ExecuteNonQuery()
                            Next
                        Next
                        trans.Commit()
                    Catch ex As Exception
                        trans.Rollback()
                        logError(ex, Me.MenuID)
                        Throw ex
                    Finally
                        sqlConn.Close()
                        sqlConn.Dispose()
                    End Try
                End Using
            End Using
            Return id
        End Function

        Private Sub buildColumnValidation(dr As DataRow, sb As Text.StringBuilder)

            If CBool(dr("HasValidation")) Then
                If CBool(dr("IsRequired")) Then
                    sb.Append("<div class='note note-error' ng-show='master_" & dr("ID_WebMenuTabs") & "." & dr("Name") & ".$error.required && mainform.$submitted'>" & IsNull(dr("Label"), dr("Name")) & " is required.</div>")
                End If
                Dim dt As DataTable = Me.dtColumnValidation(dr("ID")) 'getTable("SELECT ID_ColumnValidation,ValidationMessage,FieldCompare FROM tWebMenuColumn_Validation WHERE ID_WebMenuColumns = " & dr("ID"))
                If Not dt Is Nothing Then
                    For Each tdr As DataRow In dt.Select
                        Select Case tdr("ID_ColumnValidation")
                            'Case 2 'alpha
                            'Case 3 'isnatural
                            '    sb.AppendLine("<p>" & IsNull(tdr("ValidationMessage"), dr("Name").ToString & " must be a valid number.") & "</p>")
                            'Case 4 'isnaturalnozero
                            '    sb.AppendLine("<p>" & IsNull(tdr("ValidationMessage"), dr("Name").ToString & " must be a valid non-zero number.") & "</p>")
                            'Case 5 'alphanumeric
                            '    sb.AppendLine("<p>" & IsNull(tdr("ValidationMessage"), dr("Name").ToString & " must be a valid alpha-numeric value.") & "</p>")
                            'Case 6 'email
                            '    sb.AppendLine("<p>" & IsNull(tdr("ValidationMessage"), dr("Name").ToString & " must be a valid e-mail address.") & "</p>")
                            'Case 7 'date
                            '    sb.Append("<div class='note note-error' ng-show='master_" & dr("ID_WebMenuTabs") & "." & dr("Name").ToString & ".$error.date && mainform.$submitted'>" & IsNull(tdr("ValidationMessage"), dr("Name").ToString & " must be a valid date.") & "</div>")
                            'Case 8 'time
                            '    sb.Append("<div class='note note-error' ng-show='master_" & dr("ID_WebMenuTabs") & "." & dr("Name").ToString & ".$error.time && mainform.$submitted'>" & IsNull(tdr("ValidationMessage"), dr("Name").ToString & " must be a valid time value.") & "</div>")
                            'Case 9 'datetime
                            '    sb.Append("<div class='note note-error'>" & IsNull(tdr("ValidationMessage"), dr("Name").ToString & " must be a valid date and time value.") & "</div>")
                            Case 1 'minlength
                                sb.Append("<div class='note note-error' ng-show='master_" & dr("ID_WebMenuTabs") & "." & dr("Name").ToString & ".$error.minlength && mainform.$submitted'>" & IsNull(tdr("ValidationMessage"), dr("Name").ToString) & "</div>")
                            Case 2 'maxlength
                                sb.Append("<div class='note note-error' ng-show='master_" & dr("ID_WebMenuTabs") & "." & dr("Name").ToString & ".$error.maxlength && mainform.$submitted'>" & IsNull(tdr("ValidationMessage"), dr("Name").ToString) & "</div>")
                            Case 3 'min date
                                sb.Append("<div class='note note-error' ng-show='master_" & dr("ID_WebMenuTabs") & "." & dr("Name").ToString & ".$error.mindate && mainform.$submitted'>" & IsNull(tdr("ValidationMessage"), dr("Name").ToString) & "</div>")
                            Case 4 'max date
                                sb.Append("<div class='note note-error' ng-show='master_" & dr("ID_WebMenuTabs") & "." & dr("Name").ToString & ".$error.maxdate && mainform.$submitted'>" & IsNull(tdr("ValidationMessage"), dr("Name").ToString) & "</div>")
                            Case 5 'min time
                                sb.Append("<div class='note note-error' ng-show='master_" & dr("ID_WebMenuTabs") & "." & dr("Name").ToString & ".$error.mintime && mainform.$submitted'>" & IsNull(tdr("ValidationMessage"), dr("Name").ToString) & "</div>")
                            Case 6 'max time
                                sb.Append("<div class='note note-error' ng-show='master_" & dr("ID_WebMenuTabs") & "." & dr("Name").ToString & ".$error.maxtime && mainform.$submitted'>" & IsNull(tdr("ValidationMessage"), dr("Name").ToString) & "</div>")
                                'Case 12 'datecompare
                                '    sb.AppendLine("<p>" & IsNull(tdr("ValidationMessage"), dr("Name").ToString & " must be less than " & tdr("FieldCompare").ToString & ".") & "</p>")
                                'Case 13 'regex
                                '    sb.AppendLine("<p>" & IsNull(tdr("ValidationMessage"), dr("Name").ToString & " must be in this format " & tdr("FieldCompare").ToString & ".") & "</p>")
                                'Case 14
                                '    sb.AppendLine("<p>" & IsNull(tdr("ValidationMessage"), dr("Name").ToString & " length must be less than " & tdr("FieldCompare").ToString & ".") & "</p>")
                                'Case 15 'textcompare
                                '    sb.AppendLine("<p>" & IsNull(tdr("ValidationMessage"), dr("Name").ToString & " must be equal to " & tdr("FieldCompare").ToString & ".") & "</p>")
                        End Select
                    Next
                End If
            End If

        End Sub

        Public Function dtSubDataSource() As DataTable
            Dim query = From mSubDataSource In menuSubDataSourceCollection _
                Where mSubDataSource.Item("ID_WebMenus") = Me.MenuID _
                Order By mSubDataSource.Item("SeqNo")

            If (query Is Nothing And Me.ColumnValue("SubDataSourceCount") > 0) OrElse query.Count <> Me.ColumnValue("SubDataSourceCount") Then LoadSubDataSource(Me.MenuID)

            If query.Count > 0 Then
                Return query.CopyToDataTable
            Else
                Return Nothing
            End If
        End Function


#End Region

#Region "Functions"

        Public Overloads Function Equals(ByVal other As Menu) As Boolean Implements IEquatable(Of Menu).Equals
            If Me.MenuID = other.MenuID Then
                Return True
            Else
                Return False
            End If
        End Function

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            Return MyBase.Equals(obj)
        End Function

        Public Function GETFORMINSERTCommand() As String
            If FormInsertCommand.Length = 0 Then
                Dim columns As String = getMenuColParameters(Me.MenuID, , False)
                FormInsertCommand = "INSERT INTO " + mRow("TableName").ToString + "(" + columns.Replace("@", "") + ")" + " VALUES(" + columns + "); SELECT CAST(scope_identity() AS int)"
            End If
            Return FormInsertCommand
        End Function

        Public Function GETFORMUPDATECommand() As String
            If FormUpdateCommand.Length = 0 Then
                Dim tmp As String = getMenuColParameters(Me.MenuID, , False)
                If tmp.Length = 0 Then
                    FormUpdateCommand = "SELECT CAST(@ID AS int)"
                Else
                    Dim columns As String() = tmp.Split(",")
                    Dim str As New System.Text.StringBuilder
                    For Each col As String In columns
                        str.Append(col.Replace("@", "") + " = " + col + ",")
                    Next
                    FormUpdateCommand = "UPDATE " + mRow("TableName").ToString + " SET " + str.ToString.Trim(",") + " WHERE ID = @ID; SELECT CAST(@ID AS int)"
                End If
            End If
            Return FormUpdateCommand
        End Function

        Public Function GETGRIDINSERTCommand() As String
            If GridInsertCommand.Length = 0 Then
                Dim columns As String = getMenuColParameters(Me.MenuID, , True)
                GridInsertCommand = "INSERT INTO " + mRow("TableName").ToString + "(" + columns.Replace("@", "") + ")" + " VALUES(" + columns + "); SELECT CAST(scope_identity() AS int)"
            End If
            Return GridInsertCommand
        End Function

        Public Function GETGRIDUPDATECommand() As String
            If GridUpdateCommand.Length = 0 Then
                Dim tmp As String = getMenuColParameters(Me.MenuID, , True)
                If tmp.Length = 0 Then
                    GridUpdateCommand = "SELECT CAST(@ID AS int)"
                Else
                    Dim columns As String() = tmp.Split(",")
                    Dim str As New System.Text.StringBuilder
                    For Each col As String In columns
                        str.Append(col.Replace("@", "") + " = " + col + ",")
                    Next
                    GridUpdateCommand = "UPDATE " + mRow("TableName").ToString + " SET " + str.ToString.Trim(",") + " WHERE ID = @ID; SELECT CAST(@ID AS int)"
                End If
            End If
            Return GridUpdateCommand
        End Function

        Private Function buildColumnDefinition(m As GSWEB.MenuCollection.Menu) As String
            Dim sb As New Text.StringBuilder,
                fixedColumns As Integer = IsNull(m.ColumnValue("FixedColumns"), 0)
            sb.Append("columnDefs:[")
            If CBool(IsNull(m.ColumnValue("HasOpen"), False)) Then
                sb.Append("{field:'$$'")
                If fixedColumns > 0 AndAlso IsNull(m.ColumnValue("FixedColumns"), 0) > 0 Then
                    sb.Append(",pinned:true")
                    fixedColumns -= 1
                End If
                sb.Append(",width:30,sortable:false,resizable:false,displayName:' ',cellTemplate:'<div class=\'m-grid-cell-contents\' " & If(m.ColumnValue("ID_WebMenuTypes") = 14, "ng-if=\'row.entity.ID > 0\'", "") & "><span><a ui-sref=\'" & buildAppStateName(m) & "({ ID_" & m.MenuID & ":row.entity.$$rID})\'><i class=\'fa fa-lg fa-fw fa-edit\'></i></a></span></div>'},")
                'sb.Append(",width:30,displayName:'',cellTemplate:'<div class=\'ngCellText\' " & If(m.ColumnValue("ID_WebMenuTypes") = 14, "", "ng-if=\'row.entity.ID > 0\'") & "><span><a ui-sref=\'" & buildAppStateName(m) & "({ ID_" & m.MenuID & ":row.entity.$$rID})\'><i class=\'fa fa-lg fa-fw fa-edit\'></i></a></span></div>'},")
            End If
            Select Case m.ColumnValue("ID_WebMenuTypes")
                Case 3, 12
                    For Each dr As DataRow In m.dtColumns.Select("ShowInList = 1 AND ID_WebMenuControlTypes NOT IN (27,21) AND ID_WebMenuControlTypes IS NOT NULL", "SeqNo ASC")
                        sb.Append("{field:'" & If(dr("Name").ToString.ToLower.Contains("id_") AndAlso dr("colDataType") = "int", dr("Name").ToString.Substring(3), dr("Name")) & "'" &
                                            If(dr("ID_WebMenuControlTypes") = 7, ",width:50", If(IsNull(dr("Width"), "").ToString = "", ",width:'*'", ",width:'" & dr("Width").ToString & "'")) &
                                            ",displayName:'" & addStripSlashes(IsNull(dr("Label"), dr("Name"))) & "'")
                        If fixedColumns > 0 AndAlso IsNull(m.ColumnValue("FixedColumns"), 0) > 0 Then
                            sb.Append(",pinned:true")
                            fixedColumns -= 1
                        End If
                        If dr("ID_WebMenuControlTypes") = 7 Then
                            sb.Append(",cellTemplate: '<div class=\'m-grid-cell-contents\'><img ng-src=\'Contents/Photos/{{row.entity." & dr("Name") & "}}\' alt=\'{{row.entity." & dr("Name") & "}}\' height=\'40\' width=\'40\'/></div>'")
                        ElseIf dr("ID_WebMenuControlTypes") = 6 Then
                            sb.Append(",cellTemplate:'<div class=\'m-grid-cell-contents\'><span><a ui-sref=\'{{row.entity.ID_WebMenus}}({ ID_{{row.entity.ID_WebMenus}}:row.entity.RefID})\'><i class=\'fa fa-lg fa-fw fa-edit\'></i></a></span></div>'")
                        ElseIf dr("ID_WebMenuControlTypes") = 25 Then
                            sb.Append(",cellTemplate:'<div class=\'m-grid-cell-contents\'><span download-file=\'{{row.entity." & dr("Name") & "_GUID}}\' filename=\'{{row.entity." & dr("Name") & "}}\' style=\'cursor:pointer;text-decoration:underline\' class=\'control-label\' ng-bind-html=\'row.entity." & dr("Name") & " | trustedHTML\'></span></div>'")
                        ElseIf dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("date") AndAlso dr("Name").ToString.ToLower.Contains("time") Then
                            sb.Append(",cellFilter:'date:\'MM/dd/yyyy hh:mm a\''")
                        ElseIf dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("date") Then
                            sb.Append(",headerCellTemplateUrl:'mgrid/headerCellTemplateDate.html'")
                            sb.Append(",cellFilter:'date:\'MM/dd/yyyy\''")
                        ElseIf dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("time") Then
                            sb.Append(",headerCellTemplateUrl:'mgrid/headerCellTemplateTime.html'")
                            sb.Append(",cellFilter:'date:\'hh:mm a\''")
                        ElseIf dr("colDataType") = "bit" Then
                            sb.Append(",cellTemplate:'<div class=\'m-grid-cell-contents\'><div style=\'margin-top:-3px!important;\' class=\'smart-form noselect for_checkbox material-switch\'>" &
                                                    "<input disabled id=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' type=\'checkbox\' name=\'" & dr("Name") & "\' " &
                                                    "class=\'form-checkbox\' ng-model=\'row.entity." & dr("Name") & "\' disabled/>" &
                                                    "<label for=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' class=\'label-default\'></label>" &
                                                    "</div></div>'")
                        ElseIf dr("colDataType") = "varchar" OrElse dr("colDataType") = "nvarchar" Then
                            sb.Append(",cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity." & dr("Name") & " | trustedHTML\'></span></div>'")
                        ElseIf dr("colDataType") = "varchar" OrElse dr("colDataType") = "nvarchar" Then
                            sb.Append(",cellTemplate:'<div class=\'m-grid-cell-contents\'><span class=\'control-label\' ng-bind-html=\'row.entity." & dr("Name") & " | trustedHTML\'></span></div>'")
                        End If

                        If CBool(IsNull(dr("IsGroup"), False)) Then
                            sb.Append(",grouping:{")
                            sb.Append("seqNo:" & IsNull(dr("GroupSeqNo"), 999) & ",")
                            ''TODO: AGGREGATE VALUES
                            sb.Append("}")

                        End If
                        sb.Append("},")
                    Next


                Case 14, 17
                    For Each dr As DataRow In m.dtColumns.Select("ShowInList = 1 AND ISNULL(IsTitle,0) = 0 AND ID_WebMenuControlTypes IS NOT NULL", "SeqNo ASC")
                        sb.Append("{field:'" & dr("Name") & "'" &
                                            If(dr("ID_WebMenuControlTypes") = 7, ",width:50", If(IsNull(dr("Width"), "").ToString = "", ",width:'*'", ",width:'" & dr("Width").ToString & "'")) &
                                            ",displayName:'" & addStripSlashes(IsNull(dr("Label"), dr("Name"))) & "'")
                        If fixedColumns > 0 AndAlso IsNull(m.ColumnValue("FixedColumns"), 0) > 0 Then
                            sb.Append(",pinned:true")
                            fixedColumns -= 1
                        End If

                        If dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("date") Then
                            sb.Append(",headerCellTemplateUrl:'mgrid/headerCellTemplateDate.html'")
                        ElseIf dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("time") Then
                            sb.Append(",headerCellTemplateUrl:'mgrid/headerCellTemplateTime.html'")
                        End If

                        Select Case dr("ID_WebMenuControlTypes")
                            Case 1 'TEXTBOX
                                sb.Append(",cellTemplate:'<div ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && appScope.mainform.$submitted }"" ", "") & ">" &
                                                    "<input type=\'text\' name=\'" & dr("Name") & "\' " &
                                                    "placeholder=\'" & IsNull(dr("Label"), dr("Name")) & "\' " &
                                                    If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                                    If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'!" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    buildHTMLDefinitionControlValidation(mCollection.GetMenu(dr("ID_WebMenus")), dr("ID")) &
                                                    " class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\'/></div>'")
                            Case 2 'DROPDOWN
                                sb.Append(",cellTemplate:'<div ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && appScope.mainform.$submitted }"" ", "") & ">" &
                                                    "<select name=\'" & dr("Name") & "\' ng-options=\'item.ID as item.Name for item in appScope.dropdown_source[" & dr("ID") & "]")
                                If Not IsDBNull(dr("JsEvents")) Then
                                Dim jsevents As String = dr("JsEvents")
                                    If jsevents.Split("=")(0).ToLower = "filterdatasource" Then
                                        sb.Append(" | filter: {" & jsevents.Split("=")(1).ToString & "}")
                                    ElseIf jsevents.Split("=")(0).ToLower = "orderby" Then
                                        sb.Append(" | orderBy: """ & jsevents.Split("=")(1).ToString & """")
                                    End If
                                End If
                                sb.Append("\'")
                                sb.Append(If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                                    If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'!" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    buildHTMLDefinitionControlValidation(mCollection.GetMenu(dr("ID_WebMenus")), dr("ID")))

                                sb.Append(" class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\'><option value>- Select -</option></select>'") '<i></i></label></div '                                                    "<label class=\'select\' style=\'width:100%;\'>" &
                                'sb.Append(",cellTemplate:'<div ng-form name=\'x\' ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && mainform.$submitted }"" >" &
                                '                     "<div " & If(IsDBNull(dr("Width")), "style=\'width:100%\' ", "style=\'width:" & dr("Width") & "px;\' ") &
                                '                     If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'" & dr("EnabledIf") & "\' "), " ng-disabled=\'true\' ") &
                                '                     "dropdown-append-to-body=\'true\' dropdown-on-select=""setID(row.entity,\'" & dr("Name") & "\',$item)"" dropdown=\'item.ID as item.Name for item in dropdown_source[" & dr("ID") & "]\' ng-model=\'row.entity." & dr("Name").ToString.Substring(3) & "\'></div>" &
                                '                     "<input type=\'hidden\' name=\'" & dr("Name") & "\' class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\' " &
                                '                     If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") & "/>" &
                                '                     "</div>'")
                                'sb.Append(",cellTemplate:'<button type=\'button\' class=\'btn btn-select btn-default\' " & If(IsDBNull(dr("Width")), "style=\'width:100%\' ", "style=\'width:" & dr("Width") & "px;\' ") &
                                '                      If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                '                      If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'" & dr("EnabledIf") & "\' "), " ng-disabled=\'true\' ") &
                                '                      "data-container=\'body\' data-animation=\'am-flip-x\' " &
                                '                      "ng-options=\'item.ID as item.Name for item in dropdown_source[" & dr("ID") & "]\' ng-model=\'row.entity." & dr("Name").ToString & "\' bs-select>" &
                                '                      "Action <span class=\'caret\'></span>" &
                                '                      "</button>'")
                            Case 3 'CHECKBOX
                                sb.Append(",cellTemplate:'<div class=\'smart-form noselect material-switch\'>" &
                                                    "<input id=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' type=\'checkbox\' name=\'" & dr("Name") & "\' " &
                                                    If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'!" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    "class=\'form-checkbox\' ng-model=\'row.entity." & dr("Name") & "\'/>" &
                                                    "<label for=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' class=\'" & If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "label-primary", "label-default"), "label-default") & "\'></label>" &
                                                    "</div>'")
                            Case 4 'DATE
                                sb.Append(",cellTemplate:'<div ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && appScope.mainform.$submitted }"" ", "") & ">" &
                                                    "<div class=\'input-group\'><input type=\'text\' name=\'" & dr("Name") & "\' date-format=\'MM/dd/yyyy\' " &
                                                    "placeholder=\'" & IsNull(dr("Label"), dr("Name")) & "\' " &
                                                    If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                                    If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'!" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    If(IsDBNull(dr("Width")), "", "style=\'width:" & dr("Width") & "px;\' ") &
                                                    buildHTMLDefinitionControlValidation(mCollection.GetMenu(dr("ID_WebMenus")), dr("ID")) &
                                                    " bs-datepicker data-container=\'body\' date-to-iso " &
                                                    "class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\'/>" &
                                                    "<span class=\'input-group-addon\'><i class=\'fa fa-calendar\'></i></span></div></div>'")
                            Case 5 'TIME
                                sb.Append(",cellTemplate:'<div ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && appScope.mainform.$submitted }"" ", "") & ">" &
                                                    "<div class=\'input-group\'><input type=\'text\' name=\'" & dr("Name") & "\' " &
                                                    "placeholder=\'" & IsNull(dr("Label"), dr("Name")) & "\' " &
                                                    If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                                    If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'!" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    If(IsDBNull(dr("Width")), "", "style=\'width:" & dr("Width") & "px;\' ") &
                                                    buildHTMLDefinitionControlValidation(mCollection.GetMenu(dr("ID_WebMenus")), dr("ID")) &
                                                    " bs-timepicker time-to-iso data-container=\'body\' " &
                                                    "class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\'/>" &
                                                    "<span class=\'input-group-addon\'><i class=\'fa fa-clock-o\'></i></span></div></div>'")
                            Case 7 'IMAGEFILE
                                sb.Append(",cellTemplate: '<div class=\'ngCellText\'><img ng-src=\'Contents/Photos/{{row.entity." & dr("Name") & "}}\' alt=\'{{row.entity." & dr("Name") & "}}\' height=\'40\' width=\'40\'/></div>'")
                            Case 8 'TEXTAREA
                                sb.Append(",cellTemplate:'<div ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && appScope.mainform.$submitted }"" ", "") & ">" &
                                                    "<textarea name=\'" & dr("Name") & "\' " &
                                                    "placeholder=\'" & IsNull(dr("Label"), dr("Name")) & "\' " &
                                                    If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                                    If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'!" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    If(IsDBNull(dr("Height")), "style=\'height:40px;\' ", "style=\'height:" & dr("Height") & "px;\' ") &
                                                    buildHTMLDefinitionControlValidation(mCollection.GetMenu(dr("ID_WebMenus")), dr("ID")) &
                                                    " class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\'></textarea></div>'")
                            Case 14 ' LABEL
                                If dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("date") AndAlso dr("Name").ToString.ToLower.Contains("time") Then
                                    sb.Append(",cellFilter:'date:\'MM/dd/yyyy hh:mm:ss a\''")
                                ElseIf dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("date") Then
                                    sb.Append(",cellFilter:'date:\'MM/dd/yyyy\''")
                                ElseIf dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("time") Then
                                    sb.Append(",cellFilter:'date:\'hh:mm:ss a\''")
                                ElseIf dr("colDataType") = "varchar" OrElse dr("colDataType") = "nvarchar" Then
                                    sb.Append(",cellTemplate:'<div class=\'ngCellText\'><span class=\'control-label\' ng-bind-html=\'row.entity." & dr("Name") & " | trustedHTML\'></span></div>'")
                                End If
                            Case 17 ' LOOKUP
                                sb.Append(",cellTemplate: '<div ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && appScope.mainform.$submitted }"" ", "") & "><div class=\'input-group\'>" &
                                          "<input target-name=\'row.entity." & dr("Name").ToString.Substring(3) & "\' target-value=\'row.entity." & dr("Name") & "\' target-source=\'appScope.lookup_source[" & dr("ID") & "]\' target-addon=\'" & dr("Name") & "_r{{row.$$uid}}_c{{column.$$uid}}\' type=\'text\' cid=\'" & dr("ID") & "\' " &
                                          "placeholder=\'" & IsNull(dr("Label"), dr("Name")) & "\' " &
                                          If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'!" & dr("EnabledIf") & "\' "), " disabled ") &
                                          " ng-form-lookup " &
                                          buildHTMLDefinitionControlValidation(mCollection.GetMenu(dr("ID_WebMenus")), dr("ID")) &
                                          " class=\'form-control\' ng-model=\'row.entity." & dr("Name").ToString.Substring(3) & "\'/><br>" &
                                          "<input type=\'hidden\' " &
                                          If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                          " name=\'" & dr("Name") & "\' ng-model=\'row.entity." & dr("Name") & "\' />" &
                                          "<span ID=\'" & dr("Name") & "_r{{row.$$uid}}_c{{column.$$uid}}\' class=\'input-group-addon\'><i class=\'fa fa-list\'></i></span></div></div>'")

                            Case 19 ' CURRENCY
                                sb.Append(",cellTemplate:'<div ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && appScope.mainform.$submitted }"" ", "") & ">" &
                                                    "<input type=\'text\' name=\'" & dr("Name") & "\' " &
                                                    "placeholder=\'" & IsNull(dr("Label"), dr("Name")) & "\' " &
                                                    If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                                    If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'!" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    buildHTMLDefinitionControlValidation(mCollection.GetMenu(dr("ID_WebMenus")), dr("ID")) &
                                                    " ng-currency-only fixed-decimal=\'2\' " &
                                                    "class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\'/></div>'")
                            Case 8 'HTML EDITOR
                                sb.Append(",cellTemplate:'<div ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && appScope.mainform.$submitted }"" ", "") & ">" &
                                                    "<textarea redactor name=\'" & dr("Name") & "\' " &
                                                    If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                                    If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'!" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    If(IsDBNull(dr("Width")), "", "style=\'width:" & dr("Width") & "px;\' ") &
                                                    If(IsDBNull(dr("Height")), "style=\'height:40px;\' ", "style=\'height:" & dr("Height") & "px;\' ") &
                                                    buildHTMLDefinitionControlValidation(mCollection.GetMenu(dr("ID_WebMenus")), dr("ID")) &
                                                    " class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\'></textarea></div>'")
                            Case 21 ' HIDDEN
                                sb.Append(",visible:false,cellTemplate:'<input type=\'hidden\' name=\'" & dr("Name") & "\' " &
                                                "class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\'/>'")
                            Case 23 ' AUTOCOMPLETE
                                sb.Append(",cellTemplate:'<div ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && appScope.mainform.$submitted }"" ", "") & ">" &
                                                    "<input type=\'text\' name=\'" & dr("Name").ToString.Substring(3) & "\' " &
                                                    "placeholder=\'" & IsNull(dr("Label"), dr("Name")) & "\' " &
                                                    If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'!" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    If(IsDBNull(dr("Width")), "", "style=\'width:" & dr("Width") & "px;\' ") &
                                                    "ng-change=""appScope.clearAutoComplete(row.entity,\'" & dr("Name") & "\')"" " &
                                                    "typeahead-append-to-body=\'true\' typeahead-on-select=""setID(row.entity,\'" & dr("Name") & "\',$item)"" " &
                                                    "typeahead=\'item as item.Name for item in appScope.autocomplete_source[" & dr("ID") & "] | filter:$viewValue | limitTo:10\' " &
                                                    "class=\'form-control\' ng-model=\'row.entity." & dr("Name").ToString.Substring(3) & "\'/>" &
                                                    "<input type=\'hidden\' name=\'" & dr("Name") & "\' " &
                                                    If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                                    "class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\'/></div>'")
                            Case 25 ' FILE UPLOAD
                                sb.Append(",cellTemplate:'<div class=\'smart-form input-group\' style=\'width:100%\' ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && appScope.mainform.$submitted }"" ", "") & ">" &
                                                    "<label for=\'file\' class=\'input input-file\'>" &
                                                    "<div class=\'button\'>" &
                                                    "<input type=\'file\' name=\'" & dr("Name") & "\' " &
                                                     If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                                     If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'!" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    "file-input ng-file-select=\'appScope.onFileSelect($files," & m.MenuID & ",""" & dr("Name") & """,row.$$rowIndex)\'" &
                                                    "ng-model=\'row.entity." & dr("Name") & "\'/>Browse" &
                                                    "</div>" & _
                                                    "<input type=\'text\' ng-model=\'row.entity." & dr("Name") & "\' placeholder=\'Select files...\' readonly>" & _
                                                    "</label>" & _
                                                    "<span class=\'input-group-addon\' ng-if=\'row.entity.ID > 0 && row.entity." & dr("Name") & " !== null\' download-file=\'{{row.entity." & dr("Name") & "_GUID}}\' filename=\'{{row.entity." & dr("Name") & "}}\'><i class=\'fa fa-download\'></i></span>" & _
                                                    "</div>'")
                            Case 26 ' RADIO BUTTON
                                sb.Append(",cellTemplate:'<div ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && appScope.mainform.$submitted }"" ", "") & ">" &
                                                    "<label ng-repeat=\'rdb in appScope.rdb_source[" & dr("ID") & "]\'>" &
                                                    "<input type=\'radio\' name=\'" & dr("Name") & "\' " &
                                                    If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                                    If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'!" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    "class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\' ng-value=\'rdb.ID\'/>" &
                                                    "{{rdb.Name}}</label></div>'")
                            Case 28 ' NUMERIC
                                sb.Append(",cellTemplate:'<div ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && appScope.mainform.$submitted }"" ", "") & ">" &
                                                    "<input type=\'text\' name=\'" & dr("Name") & "\' " &
                                                    "placeholder=\'" & IsNull(dr("Label"), dr("Name")) & "\' " &
                                                    If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                                    If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'!" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    buildHTMLDefinitionControlValidation(mCollection.GetMenu(dr("ID_WebMenus")), dr("ID")) &
                                                    " ng-numeric-only " &
                                                    "class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\'/></div>'")
                            Case 35 ' DETAILED LOOKUP
                                sb.Append(",cellTemplate: '<div ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && appScope.mainform.$submitted }"" ", "") & "><div class=\'input-group\'>" &
                                          "<input target-name=\'row.entity." & dr("Name").ToString.Substring(3) & "\' target-value=\'row.entity." & dr("Name") & "\' target-source=\'appScope.detailedlookup_source[" & dr("ID") & "]\' target-addon=\'" & dr("Name") & "_r{{row.$$uid}}_c{{column.$$uid}}\' type=\'text\' cid=\'" & dr("ID") & "\' " &
                                          "placeholder=\'" & IsNull(dr("Label"), dr("Name")) & "\' " &
                                          If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'!" & dr("EnabledIf") & "\' "), " disabled ") &
                                          " ng-form-detailed-lookup onkeyup=""return false"" onkeydown=""return false"" " &
                                          buildHTMLDefinitionControlValidation(mCollection.GetMenu(dr("ID_WebMenus")), dr("ID")) &
                                          " class=\'form-control\' ng-model=\'row.entity." & dr("Name").ToString.Substring(3) & "\'/><br>" &
                                          "<input type=\'hidden\' " &
                                          If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                          " name=\'" & dr("Name") & "\' ng-model=\'row.entity." & dr("Name") & "\' />" &
                                          "<span ID=\'" & dr("Name") & "_r{{row.$$uid}}_c{{column.$$uid}}\' class=\'input-group-addon\'><i class=\'fa fa-list\'></i></span></div>'")
                            Case 36 'DATETIME
                                sb.Append(",cellTemplate:'<div ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && appScope.mainform.$submitted }"" ", "") & ">" &
                                          "<div class=\'input-group\'><input onkeydown=\'if(event.keyCode != 8) return false\' type=\'text\' name=\'" & dr("Name") & "\' " &
                                          "placeholder=\'" & IsNull(dr("Label"), dr("Name")) & "\' " &
                                          If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                          If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'!" & dr("EnabledIf") & "\' "), " disabled ") &
                                          If(IsDBNull(dr("Width")), "", "style=\'width:" & dr("Width") & "px;\' ") &
                                          buildHTMLDefinitionControlValidation(mCollection.GetMenu(dr("ID_WebMenus")), dr("ID")) &
                                          " make-datetime-picker-detail ID=\'" & dr("Name") & "_r{{row.$$uid}}_c{{column.$$uid}}\' " &
                                          "class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\'/>" &
                                          "<span class=\'input-group-addon\'><i class=\'fa fa-calendar\'></i></span></div></div>'")

                        End Select


                        sb.Append("},")
                    Next
            End Select

            If CBool(IsNull(m.ColumnValue("HasDelete"), False)) Then
                sb.Append("{field:'$delete',width:20,sortable:false,resizable:false,displayName:' ',cellTemplate:'<div class=\'ngCellText\'><span><a class=\'row-delete\' ng-click=\'appScope.removeRow(" & m.MenuID & ",row,""" & addStripSlashes(IsNull(m.ColumnValue("DeleteConfirmation"), "")) & """)\' " & If(IsDBNull(m.ColumnValue("HasDeleteIf")), "", " ng-show=\'" & m.ColumnValue("HasDeleteIf") & "\' ") & " ><i class=\'fa fa-times\'></i></a></span></div>'},")
            End If

            sb.Append("],")
            Return sb.ToString
        End Function


        Private Function buildTreeViewColumnDefinition(m As GSWEB.MenuCollection.Menu) As String
            Dim sb As New Text.StringBuilder
            sb.Append("columnDefs:[")

            Select Case m.ColumnValue("ID_WebMenuTypes")
                Case 18
                    For Each dr As DataRow In m.dtColumns.Select("ShowInList = 1 AND ID_WebMenuControlTypes NOT IN (27,21) AND ID_WebMenuControlTypes IS NOT NULL", "SeqNo ASC").Skip(If(CBool(IsNull(m.ColumnValue("HasOpen"), False)), 0, 1))
                        sb.Append("{field:'" & If(dr("Name").ToString.ToLower.Contains("id_") AndAlso dr("colDataType") = "int", dr("Name").ToString.Substring(3), dr("Name")) & "'" &
                                            ",displayName:'" & addStripSlashes(IsNull(dr("Label"), dr("Name"))) & "'")

                        If dr("ID_WebMenuControlTypes") = 7 Then
                            sb.Append(",cellTemplate: '<img ng-src=\'Contents/Photos/{{row.entity." & dr("Name") & "}}\' alt=\'{{row.entity." & dr("Name") & "}}\' height=\'40\' width=\'40\'/>'")
                        ElseIf dr("ID_WebMenuControlTypes") = 6 Then
                            sb.Append(",cellTemplate:'<a ui-sref=\'{{row.entity.ID_WebMenus}}({ ID_{{row.entity.ID_WebMenus}}:row.entity.RefID})\'><i class=\'fa fa-lg fa-fw fa-edit\'></i></a>'")
                            'ElseIf dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("date") AndAlso dr("Name").ToString.ToLower.Contains("time") Then
                            '    sb.Append(",cellFilter:'date:\'MM/dd/yyyy hh:mm:ss a\''")
                            'ElseIf dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("date") Then
                            '    sb.Append(",headerCellTemplate:T.get('nggrid/headerCellTemplateDate.html')")
                            '    sb.Append(",cellFilter:'date:\'MM/dd/yyyy\''")
                            'ElseIf dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("time") Then
                            '    sb.Append(",headerCellTemplate:T.get('nggrid/headerCellTemplateTime.html')")
                            '    sb.Append(",cellFilter:'date:\'hh:mm:ss a\''")
                        ElseIf dr("colDataType") = "bit" Then
                            sb.Append(",cellTemplate:'<div class=\'smart-form noselect for_checkbox material-switch\'>" &
                                                    "<input disabled id=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' type=\'checkbox\' name=\'" & dr("Name") & "\' " &
                                                    "class=\'form-checkbox\' ng-model=\'row.entity." & dr("Name") & "\' disabled/>" &
                                                    "<label for=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' class=\'label-default\'></label>" &
                                                    "</div>'")
                        ElseIf dr("colDataType") = "varchar" OrElse dr("colDataType") = "nvarchar" Then
                            sb.Append(",cellTemplate:'<span class=\'control-label\' ng-bind-html=\'row.entity." & dr("Name") & " | trustedHTML\'></span>'")
                        End If
                        sb.Append("},")
                    Next


                Case 14, 17
                    For Each dr As DataRow In m.dtColumns.Select("ShowInList = 1 AND ISNULL(IsTitle,0) = 0 AND ID_WebMenuControlTypes IS NOT NULL", "SeqNo ASC")
                        sb.Append("{field:'" & dr("Name") & "'" &
                                            If(dr("ID_WebMenuControlTypes") = 7, ",width:50", If(IsNull(dr("Width"), "").ToString = "", ",width:'*'", ",width:'" & dr("Width").ToString & "'")) &
                                            ",displayName:'" & addStripSlashes(IsNull(dr("Label"), dr("Name"))) & "'")


                        If dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("date") Then
                            sb.Append(",headerCellTemplate:T.get('nggrid/headerCellTemplateDate.html')")
                        ElseIf dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("time") Then
                            sb.Append(",headerCellTemplate:T.get('nggrid/headerCellTemplateTime.html')")
                        End If

                        Select Case dr("ID_WebMenuControlTypes")
                            Case 1 'TEXTBOX
                                sb.Append(",cellTemplate:'<div ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && mainform.$submitted }"" ", "") & ">" &
                                                    "<input type=\'text\' name=\'" & dr("Name") & "\' " &
                                                    "placeholder=\'" & IsNull(dr("Label"), dr("Name")) & "\' " &
                                                    If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                                    If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    If(IsDBNull(dr("Width")), "", "style=\'width:" & dr("Width") & "px;\' ") &
                                                    "class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\'/></div>'")
                            Case 2 'DROPDOWN
                                sb.Append(",cellTemplate:'<div ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && mainform.$submitted }"" ", "") & ">" &
                                                    "<label class=\'select\' style=\'width:100%;\'>" &
                                                    "<select name=\'" & dr("Name") & "\' ng-options=\'item.ID as item.Name for item in dropdown_source[" & dr("ID") & "]\' " &
                                                    If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                                    If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    " class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\'><option value>- Select -</option></select><i></i></label></div>'")
                                'sb.Append(",cellTemplate:'<div ng-form name=\'x\' ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && mainform.$submitted }"" >" &
                                '                     "<div " & If(IsDBNull(dr("Width")), "style=\'width:100%\' ", "style=\'width:" & dr("Width") & "px;\' ") &
                                '                     If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'" & dr("EnabledIf") & "\' "), " ng-disabled=\'true\' ") &
                                '                     "dropdown-append-to-body=\'true\' dropdown-on-select=""setID(row.entity,\'" & dr("Name") & "\',$item)"" dropdown=\'item.ID as item.Name for item in dropdown_source[" & dr("ID") & "]\' ng-model=\'row.entity." & dr("Name").ToString.Substring(3) & "\'></div>" &
                                '                     "<input type=\'hidden\' name=\'" & dr("Name") & "\' class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\' " &
                                '                     If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") & "/>" &
                                '                     "</div>'")
                                'sb.Append(",cellTemplate:'<button type=\'button\' class=\'btn btn-select btn-default\' " & If(IsDBNull(dr("Width")), "style=\'width:100%\' ", "style=\'width:" & dr("Width") & "px;\' ") &
                                '                      If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                '                      If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'" & dr("EnabledIf") & "\' "), " ng-disabled=\'true\' ") &
                                '                      "data-container=\'body\' data-animation=\'am-flip-x\' " &
                                '                      "ng-options=\'item.ID as item.Name for item in dropdown_source[" & dr("ID") & "]\' ng-model=\'row.entity." & dr("Name").ToString & "\' bs-select>" &
                                '                      "Action <span class=\'caret\'></span>" &
                                '                      "</button>'")
                            Case 3 'CHECKBOX
                                sb.Append(",cellTemplate:'<div class=\'smart-form noselect material-switch\'>" &
                                                    "<input id=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' type=\'checkbox\' name=\'" & dr("Name") & "\' " &
                                                    If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    "class=\'form-checkbox\' ng-model=\'row.entity." & dr("Name") & "\'/>" &
                                                    "<label for=\'someSwitchOptionPrimary_r{{row.$$uid}}_c{{column.$$uid}}\' class=\'" & If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "label-primary", "label-default"), "label-default") & "\'></label>" &
                                                    "</div>'")

                            Case 4 'DATE
                                sb.Append(",cellTemplate:'<div ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && mainform.$submitted }"" ", "") & ">" &
                                                    "<div class=\'input-group\'><input type=\'text\' name=\'" & dr("Name") & "\' date-format=\'MM/dd/yyyy\' " &
                                                    "placeholder=\'" & IsNull(dr("Label"), dr("Name")) & "\' " &
                                                    If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                                    If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    If(IsDBNull(dr("Width")), "", "style=\'width:" & dr("Width") & "px;\' ") &
                                                    "bs-datepicker data-container=\'body\' date-to-iso " &
                                                    "class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\'/>" &
                                                    "<span class=\'input-group-addon\'><i class=\'fa fa-calendar\'></i></span></div></div>'")
                            Case 5 'TIME
                                sb.Append(",cellTemplate:'<div ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && mainform.$submitted }"" ", "") & ">" &
                                                    "<div class=\'input-group\'><input type=\'text\' name=\'" & dr("Name") & "\' " &
                                                    "placeholder=\'" & IsNull(dr("Label"), dr("Name")) & "\' " &
                                                    If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                                    If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    If(IsDBNull(dr("Width")), "", "style=\'width:" & dr("Width") & "px;\' ") &
                                                    "bs-timepicker data-container=\'body\' " &
                                                    "class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\'/>" &
                                                    "<span class=\'input-group-addon\'><i class=\'fa fa-clock-o\'></i></span></div></div>'")
                            Case 7 'IMAGEFILE
                                sb.Append(",cellTemplate: '<div class=\'ngCellText\'><img ng-src=\'Contents/Photos/{{row.entity." & dr("Name") & "}}\' alt=\'{{row.entity." & dr("Name") & "}}\' height=\'40\' width=\'40\'/></div>'")
                            Case 8 'TEXTAREA
                                sb.Append(",cellTemplate:'<div ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && mainform.$submitted }"" ", "") & ">" &
                                                    "<textarea name=\'" & dr("Name") & "\' " &
                                                    "placeholder=\'" & IsNull(dr("Label"), dr("Name")) & "\' " &
                                                    If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                                    If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    If(IsDBNull(dr("Width")), "", "style=\'width:" & dr("Width") & "px;\' ") &
                                                    If(IsDBNull(dr("Height")), "style=\'height:40px;\' ", "style=\'height:" & dr("Height") & "px;\' ") &
                                                    "class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\'></textarea></div>'")
                            Case 14 ' LABEL
                                If dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("date") AndAlso dr("Name").ToString.ToLower.Contains("time") Then
                                    sb.Append(",cellFilter:'date:\'MM/dd/yyyy hh:mm:ss a\''")
                                ElseIf dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("date") Then
                                    sb.Append(",cellFilter:'date:\'MM/dd/yyyy\''")
                                ElseIf dr("colDataType") = "datetime" AndAlso dr("Name").ToString.ToLower.Contains("time") Then
                                    sb.Append(",cellFilter:'date:\'hh:mm:ss a\''")
                                ElseIf dr("colDataType") = "varchar" OrElse dr("colDataType") = "nvarchar" Then
                                    sb.Append(",cellTemplate:'<div class=\'ngCellText\'><span class=\'control-label\' ng-bind-html=\'row.entity." & dr("Name") & " | trustedHTML\'></span></div>'")
                                End If
                            Case 17 ' LOOKUP

                            Case 19 ' CURRENCY
                                sb.Append(",cellTemplate:'<div ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && mainform.$submitted }"" ", "") & ">" &
                                                    "<input type=\'text\' name=\'" & dr("Name") & "\' " &
                                                    "placeholder=\'" & IsNull(dr("Label"), dr("Name")) & "\' " &
                                                    If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                                    If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    If(IsDBNull(dr("Width")), "", "style=\'width:" & dr("Width") & "px;\' ") &
                                                    "ng-currency-only fixed-decimal=\'2\' " &
                                                    "class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\'/></div>'")
                            Case 8 'HTML EDITOR
                                sb.Append(",cellTemplate:'<div ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && mainform.$submitted }"" ", "") & ">" &
                                                    "<textarea redactor name=\'" & dr("Name") & "\' " &
                                                    If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                                    If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    If(IsDBNull(dr("Width")), "", "style=\'width:" & dr("Width") & "px;\' ") &
                                                    If(IsDBNull(dr("Height")), "style=\'height:40px;\' ", "style=\'height:" & dr("Height") & "px;\' ") &
                                                    "class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\'></textarea></div>'")
                            Case 21 ' HIDDEN
                                sb.Append(",visible:false,cellTemplate:'<input type=\'hidden\' name=\'" & dr("Name") & "\' " &
                                                "class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\'/>'")
                            Case 23 ' AUTOCOMPLETE
                                sb.Append(",cellTemplate:'<div ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && mainform.$submitted }"" ", "") & ">" &
                                                    "<input type=\'text\' name=\'" & dr("Name").ToString.Substring(3) & "\' " &
                                                    "placeholder=\'" & IsNull(dr("Label"), dr("Name")) & "\' " &
                                                    If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    If(IsDBNull(dr("Width")), "", "style=\'width:" & dr("Width") & "px;\' ") &
                                                    "ng-change=""clearAutoComplete(row.entity,\'" & dr("Name") & "\')"" " &
                                                    "typeahead-append-to-body=\'true\' typeahead-on-select=""setID(row.entity,\'" & dr("Name") & "\',$item)"" " &
                                                    "typeahead=\'item as item.Name for item in autocomplete_source[" & dr("ID") & "] | filter:$viewValue | limitTo:10\' " &
                                                    "class=\'form-control\' ng-model=\'row.entity." & dr("Name").ToString.Substring(3) & "\'/>" &
                                                    "<input type=\'hidden\' name=\'" & dr("Name") & "\' " &
                                                    If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                                    "class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\'/></div>'")
                            Case 25 ' FILE UPLOAD
                                sb.Append(",cellTemplate:'<div class=\'smart-form input-group\' style=\'width:100%\' ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && mainform.$submitted }"" ", "") & ">" &
                                                    "<label for=\'file\' class=\'input input-file\'>" &
                                                    "<div class=\'button\'>" &
                                                    "<input type=\'file\' name=\'" & dr("Name") & "\' " &
                                                     If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                                    If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    If(IsDBNull(dr("Width")), "", "style=\'width:" & dr("Width") & "px;\' ") &
                                                    "file-input ng-file-select=\'onFileSelect($files," & m.MenuID & ",""" & dr("Name") & """,row.rowIndex)\'" &
                                                    "ng-model=\'row.entity." & dr("Name") & "\'/>Browse" &
                                                    "</div>" & _
                                                    "<input type=\'text\' ng-model=\'row.entity." & dr("Name") & "\' placeholder=\'Select files...\' readonly>" & _
                                                    "</label>" & _
                                                    "<span class=\'input-group-addon\' ng-if=\'row.entity.ID > 0 && row.entity." & dr("Name") & " !== null\' download-file=\'{{row.entity." & dr("Name") & "_GUID}}\' filename=\'{{row.entity." & dr("Name") & "}}\'><i class=\'fa fa-download\'></i></span>" & _
                                                    "</div>'")
                            Case 26 ' RADIO BUTTON
                                sb.Append(",cellTemplate:'<div ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && mainform.$submitted }"" ", "") & ">" &
                                                    "<label ng-repeat=\'rdb in rdb_source[" & dr("ID") & "]\'>" &
                                                    "<input type=\'radio\' name=\'" & dr("Name") & "\' " &
                                                    If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                                    If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    "class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\' ng-value=\'rdb.ID\'/>" &
                                                    "{{rdb.Name}}</label></div>'")
                            Case 28 ' NUMERIC
                                sb.Append(",cellTemplate:'<div ng-form name=\'x\' " & If(CBool(dr("HasValidation")), "ng-class=""{ \'has-error\' : x." & dr("Name") & ".$invalid && mainform.$submitted }"" ", "") & ">" &
                                                    "<input type=\'text\' name=\'" & dr("Name") & "\' " &
                                                    "placeholder=\'" & IsNull(dr("Label"), dr("Name")) & "\' " &
                                                    If(CBool(IsNull(dr("IsRequired"), False)), If(IsNull(dr("RequiredIf"), "") = "", " required ", " ng-required=\'" & dr("RequiredIf") & "\' "), "") &
                                                    If(CBool(IsNull(dr("IsEnabled"), False)), If(IsNull(dr("EnabledIf"), "") = "", "", " ng-disabled=\'" & dr("EnabledIf") & "\' "), " disabled ") &
                                                    If(IsDBNull(dr("Width")), "", "style=\'width:" & dr("Width") & "px;\' ") &
                                                    "ng-numeric-only " &
                                                    "class=\'form-control\' ng-model=\'row.entity." & dr("Name") & "\'/></div>'")

                        End Select


                        sb.Append("},")
                    Next
            End Select

            If CBool(IsNull(m.ColumnValue("HasDelete"), False)) Then
                sb.Append("{field:'ID',width:20,displayName:'',cellTemplate:'<div class=\'ngCellText\'><span><a class=\'row-delete\' ng-click=\'removeRow(" & m.MenuID & ",row)\' " & If(IsDBNull(m.ColumnValue("HasDeleteIf")), "", " ng-show=\'" & m.ColumnValue("HasDeleteIf") & "\' ") & " ><i class=\'fa fa-times\'></i></a></span></div>'},")
            End If

            sb.Append("],")
            Return sb.ToString
        End Function

#End Region

    End Class

End Namespace