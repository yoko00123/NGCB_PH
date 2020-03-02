Option Explicit On
Option Strict On



Friend Class UserGroupInfo
    Inherits InfoSet

    Private myDT As New Database.Tables.tUserGroup(Connection)
    Private mtUserGroupMenu As DataTable
    'Private mtUserGroupMenu As New Database.Tables.tUserGroupMenu(Connection)
    Private mControl As New InSys.DataControl 'Private mControl As New nDB.UserGroupControl
    Private WithEvents mTVMenu As New MenuTreeView
    'Private mMenuGrid As DataGridView


    Public Sub New(ByVal pMenu As Integer, ByVal c As SqlClient.SqlConnection, ByVal pListing As DataTable, ByVal pID As Integer)
        MyBase.New(c, pListing, pID)
        With mDataset.Tables
            .Add(Table)
            '.Add(mtUserGroupMenu)
        End With

        'Dim pdc As DataColumn
        'Dim cdc As DataColumn
        'Dim rel As DataRelation


        InitControl(pMenu)
        'pdc = myDT.Columns(Database.Tables.tUserGroup.Field.ID)
        'cdc = mtUserGroupMenu.Columns(Database.Tables.tUserGroupMenu.Field.ID_UserGroup)
        'rel = mDataset.Relations.Add(pdc, cdc)

        mtUserGroupMenu = Me.mDataset.Tables("tUserGroupMenu")
        With mTVMenu
            .DataSource = nDB.MenuTable
            .DataDestination = mtUserGroupMenu
            .ValueMember = Database.Tables.tUserGroupMenu.Field.ID_Menu.ToString
        End With
        Me.AddControl(mTVMenu, "Menu")



        Me.NoGridTables = ""
        mtUserGroupMenu.DefaultView.RowFilter = "(ReportFile IS NULL) AND (DataSource IS NOT NULL)"

        AfterNew()

        'mMenuGrid = Me.GetDataGridView(mtUserGroupMenu)
        'With mMenuGrid
        '    .AllowUserToAddRows = False
        '    .AllowUserToDeleteRows = False
        '    .Columns(Database.Tables.tUserGroupMenu.Field.ID_UserGroup.ToString).ReadOnly = True
        'End With



    End Sub

#Region "LoadInfo"
    Public Overrides Sub LoadInfo(ByVal pID As Integer)
        'mtUserGroupMenu.ClearThenFill("ID_UserGroup=" & pID.ToString)
        'mtUserGroupDesignation.ClearThenFill("ID_UserGroup=" & pID.ToString)
        MyBase.LoadInfo(pID)
        mTVMenu.CheckNodes()
        'mTVDesignation.CheckNodes(mtUserGroupDesignation, Database.Tables.tUserGroupDesignation.Field.ID_Designation.ToString)
    End Sub

#End Region

    '#Region "SetDefaultValues"
    '    Protected Overrides Sub SetDefaultValues()
    '        Dim vID As Integer
    '        vID = CInt(myDT.Get(Database.Tables.tUserGroup.Field.ID))
    '        mtUserGroupMenu.Columns(Database.Tables.tUserGroupMenu.Field.ID_UserGroup).DefaultValue = vID
    '        mtUserGroupDesignation.Columns(Database.Tables.tUserGroupDesignation.Field.ID_UserGroup).DefaultValue = vID
    '    End Sub

    '#End Region

    Protected Overrides Function CanSave() As Boolean
        mTVMenu.EndEdit()
        'mTVDesignation.EndEdit(mtUserGroupDesignation, Database.Tables.tUserGroupDesignation.Field.ID_Designation.ToString)
        Return MyBase.CanSave()
    End Function

#Region "Overrides"
    Protected Overrides Property Table() As GSCOM.SQL.ZDataTable
        Get
            Return myDT
        End Get
        Set(ByVal value As GSCOM.SQL.ZDataTable)
            myDT = CType(value, Database.Tables.tUserGroup)
        End Set
    End Property


#End Region

    Private Sub mTVMenu_AfterSelect(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles mTVMenu.NodeMouseDoubleClick
        Dim xInfoSet As InfoSet
        Dim vID_Menu As Integer = CInt(e.Node.Name)
        Dim vID_UserGroupMenu As Integer
        Dim pMenu As Database.Menu
        pMenu = CType(612, Database.Menu)

        Dim dra() As DataRow
        dra = mtUserGroupMenu.Select("ID_Menu=" & vID_Menu)

        If dra.Length = 0 Then
            MsgBox("Must check this item first")
        Else
            vID_UserGroupMenu = CInt(dra(0)("ID"))
        End If
        xInfoSet = GetInfoSet(pMenu)
        If xInfoSet Is Nothing Then
            xInfoSet = ActiveModule.NewInfo(pMenu, Nothing, vID_UserGroupMenu)
        Else
            xInfoSet.LoadInfo(vID_UserGroupMenu)
        End If
        xInfoSet.ShowDialog()
    End Sub

End Class
