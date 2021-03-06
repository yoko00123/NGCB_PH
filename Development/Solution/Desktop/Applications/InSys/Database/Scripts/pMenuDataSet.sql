/****** Object:  StoredProcedure [dbo].[pMenuDataSet]    Script Date: 08/03/2011 12:00:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [dbo].[pMenuDataSet](@ID_Session INT)
--20110803
WITH RECOMPILE,ENCRYPTION
AS
IF OBJECT_ID(N'tempdb.dbo.#tmpMenu') IS NOT NULL
		DROP TABLE        #tmpMenu
IF OBJECT_ID(N'tempdb.dbo.#tmpMenuTab') IS NOT NULL
		DROP TABLE [dbo].[#tmpMenuTab]
IF OBJECT_ID(N'tempdb.dbo.#tmpMenuDetailTab') IS NOT NULL
	    DROP TABLE [dbo].[#tmpMenuDetailTab]
IF OBJECT_ID(N'tempdb.dbo.#tmpMenuTabField') IS NOT NULL
		DROP TABLE [dbo].[#tmpMenuTabField]
IF OBJECT_ID(N'tempdb.dbo.#tmpMenuDetailTabField') IS NOT NULL
		DROP TABLE [dbo].[#tmpMenuDetailTabField]
IF OBJECT_ID(N'tempdb.dbo.#tmpMenuDetailTabProperty') IS NOT NULL
		DROP TABLE [dbo].[#tmpMenuDetailTabProperty]
/*		
IF OBJECT_ID(N'tempdb.dbo.#tmpMenuChildTable') IS NOT NULL
		DROP TABLE [dbo].[#tmpMenuChildTable]
IF OBJECT_ID(N'tempdb.dbo.#tmpMenuChildTableRelation') IS NOT NULL
		DROP TABLE [dbo].[#tmpMenuChildTableRelation]
*/		
		
		
		
IF OBJECT_ID(N'tempdb.dbo.#tmpMenuButton') IS NOT NULL
		DROP TABLE [dbo].[#tmpMenuButton]
IF OBJECT_ID(N'tempdb.dbo.#tmpConstraints') IS NOT NULL
		DROP TABLE [dbo].[#tmpConstraints]
IF OBJECT_ID(N'tempdb.dbo.#tmpMenuSubDataSource') IS NOT NULL
		DROP TABLE [dbo].[#tmpMenuSubDataSource]

------------------------------------------------------------------------------------------
DECLARE @ID_UserGroup INT

SELECT @ID_UserGroup = u.ID_UserGroup 
FROM tUser u INNER JOIN tSession s ON u.ID = s.ID_User 
WHERE s.ID=@ID_Session


SELECT 
  ID
, Name
, DataSource
, BaseDataSource
, TableName

, ID_Menu
, SeqNo
, ImageFile
, ReportFile

, ID_MenuType
, Sort

, IsVisible
, AllowNew
, AllowOpen
, AllowDelete
, [ReadOnly]

, ReportTitle
, ReportSubTitle

, ColorRGB
, DarkColorRGB

, ID_Session
, AllowEdit
, ID_ListMenu
, ListMenu
, ListSource
, ListRowFieldHeader
, ListRowField
, ListRowCategoryHeader
, ListRowCategory
, IsUserData
, IsSpanView
, ListFixedFilter
, StatusTable
, [Description]
, SaveTrigger
	INTO #tmpMenu 
	FROM vzSessionMenu 
	WHERE (ID_Session=@ID_Session) 
	--and id = 16
	ORDER BY ID_MENU,SEQNO,NAME
	
---------------------------------------------------------------------------
SELECT 
	mt.ID_Menu,mt.ID, mt.Name, mt.HasTable,mt.Description,mt.ImageFile,mt.SeqNo 
	INTO #tmpMenuTab
	FROM tMenuTab mt 
	WHERE 
	--ID_Menu=9 
	--mt.ID_Menu = CASE (SELECT ID FROM #tmpMenu) WHEN 161 THEN 9 ELSE (SELECT ID FROM #tmpMenu) END
	mt.ID_Menu IN (SELECT ID FROM #tmpMenu)
	AND (IsActive=1) 
	AND ((NOT EXISTS (SELECT ID FROM tSystemApplicationMenuTab samt WHERE samt.ID_MenuTab=mt.ID)) OR (EXISTS (SELECT ID FROM vSystemApplicationMenuTab samt WHERE samt.IsActive=1 AND samt.ID_MenuTab=mt.ID))) 
  --AND ((NOT EXISTS (SELECT ID FROM tUserGroupMenuTab samt WHERE samt.ID_MenuTab=mt.ID)) OR (EXISTS (SELECT ID FROM vUserGroupMenuTab samt WHERE samt.IsActive=1 AND samt.ID_MenuTab=mt.ID))) 
  	AND ((NOT EXISTS (SELECT ID FROM vUserGroupMenuTab samt WHERE samt.ID_MenuTab=mt.ID AND samt.ID_UserGroup=@ID_UserGroup ))) --OR (EXISTS (SELECT ID FROM vUserGroupMenuTab samt WHERE samt.IsActive=1 AND samt.ID_MenuTab=mt.ID))) 


	ORDER BY mt.ID_Menu,mt.SeqNo,mt.ID
---------------------------------------------------------------------------------------------
--
SELECT * 
	INTO #tmpMenuDetailTab
	FROM tMenuDetailTab mdt 
	WHERE 
	--ID_Menu=9 
	mdt.ID_Menu IN (SELECT ID FROM #tmpMenu)
--	mdt.ID_Menu = CASE (SELECT ID FROM #tmpMenu) WHEN 161 THEN 9 ELSE (SELECT ID FROM #tmpMenu) END
	AND (IsActive=1) 
	AND ((NOT EXISTS (SELECT ID FROM tSystemApplicationMenuDetailTab samt WHERE samt.ID_MenuDetailTab=mdt.ID)) OR (EXISTS (SELECT ID FROM vSystemApplicationMenuDetailTab samt WHERE samt.IsActive=1 AND samt.ID_MenuDetailTab=mdt.ID))) 

--	AND ((NOT EXISTS (SELECT ID FROM tUserGroupMenuDetailTab samt WHERE samt.ID_MenuDetailTab=mdt.ID)) OR (EXISTS (SELECT ID FROM vUserGroupMenuDetailTab samt WHERE samt.IsActive=1 AND samt.ID_MenuDetailTab=mdt.ID))) 
    AND ((NOT EXISTS (SELECT ID FROM vUserGroupMenuDetailTab samt WHERE samt.ID_MenuDetailTab=mdt.ID AND samt.ID_UserGroup=@ID_UserGroup)))

	ORDER BY mdt.ID_Menu,mdt.SeqNo,mdt.ID
-----------------------------------
SELECT 
mtf.*
	--mtf.Name
	--,mtf.ID_MenuTab
	--,mtf.ID_SystemControlType
	--,mtf.ID_Menu
	--,mtf.EffectiveLabel
	--, mtf.Header
	--, mtf.ImageFile
	--,mtf.Description
	--, mtf.MenuTab
	--,ShowInBrowser
	--,mtf.ReadOnly
	--,mtf.Panel
	--,mtf.ShowInInfo
	--,mtf.ShowInList 
	--,mtf.MenuTabMenuID
	--,mtf.TableName
	--,mtf.StringFormat
	--,mtf.Sort
	INTO #tmpMenuTabField
	FROM vMenuTabField mtf 
	INNER JOIN tMenuTab mt ON mt.ID = mtf.ID_MenuTab 
	LEFT JOIN tMenu m ON m.ID = mtf.ID_Menu
/* 
	LEFT JOIN 
	(
		SELECT --TOP 100 PERCENT
		    sc.[name] [ColumnName]
		,	st.[name] [DataType]
		,	CASE WHEN st.[Name] = 'text' THEN 2147483647 ELSE sc.length End [Length]
		,	sc.xprec [Precision]
		,	sc.xscale [Scale]
		,	sc.iscomputed [Computed]
		, 	scm_def.[text] AS [DefaultValue]
		,	ISNULL(scm.[text], '') [Expression]
		,	sc.colstat & 1 [Identity]
		,	sc.IsNullable [AllowDBNull]
--		,	'' [Caption]
		,	CAST('' AS VARCHAR(50)) [StringFormat]
		,   sc.colorder SeqNo
		,    so.Name TableName
		 FROM	syscolumns sc
			INNER JOIN systypes st ON st.xtype = sc.xtype
			INNER JOIN sysobjects so ON so.id = sc.id
			LEFT OUTER JOIN syscomments scm ON so.id = scm.id AND sc.colorder = scm.number
		 LEFT OUTER JOIN dbo.syscomments scm_def ON sc.cdefault = scm_def.id
		 WHERE --(so.name = @ObjectName) AND 
		 sc.number=0
		 AND st.Name <> 'sysname'
	) st ON st.ColumnName = mtf.Name AND st.TableName = m.TableName

*/

	WHERE 
--	mtf.ID_MenuTab IN (SELECT ID FROM tMenuTab mt WHERE ID_Menu=9 AND (IsActive=1)) 
	mtf.ID_MenuTab IN (SELECT ID FROM #tmpMenuTab)
	AND (mtf.IsActive=1) 
--	AND (mtf.ShowInInfo=1) 
	ORDER BY mt.SeqNo,mt.ID, mtf.SeqNo,mtf.ID
-----------------------------------------------------------------------
SELECT 
mtf.*
--mtf.ID
--	,mtf.Name
--	,mtf.ID_MenuDetailTab
--	,mtf.ID_SystemControlType
--	,mtf.ID_Menu 
--	,mtf.EffectiveLabel
--	,mtf.Description
--	,mt.Name MenuDetailTab
--	,mtf.ShowInBrowser
--	,Formula
--	,mtf.ReadOnly
--	,ListKey
--	,Width
--	, mtf.IsGroup
--	,mtf.Text,mtf.Sort
--	,mtf.IsColumn
--	,mtf.ListColumn
--	,mtf.CopyFromList
--	,mtf.SeqNo 
--	,mt.ID_Menu MenuDetailTabMenuID
--	,mtf.ShowInInfo
--	,mtf.ImageFile
	INTO #tmpMenuDetailTabField
	FROM vMenuDetailTabField mtf 
	INNER JOIN tMenuDetailTab mt ON mt.ID = mtf.ID_MenuDetailTab 
	LEFT JOIN tMenu m ON m.ID = mtf.ID_Menu 
	WHERE 
	mtf.ID_MenuDetailTab IN (SELECT ID FROM #tmpMenuDetailTab) 
	AND (mtf.IsActive=1) 
	ORDER BY mt.SeqNo,mt.ID, mtf.SeqNo,mtf.ID
------------------------------------------------------------------------------------------------------------------
SELECT mtf.Name
,mtf.ID_MenuDetailTab
,mtf.Value 
		,mt.ID_Menu MenuDetailTabMenuID
	INTO #tmpMenuDetailTabProperty
	FROM tMenuDetailTabProperty mtf 
	INNER JOIN tMenuDetailTab mt ON mt.ID = mtf.ID_MenuDetailTab 
	WHERE 
--	mtf.ID_MenuDetailTab IN (SELECT ID FROM tMenuDetailTab mt WHERE ID_Menu=9 AND (IsActive=1)) 
	mtf.ID_MenuDetailTab IN (SELECT ID FROM #tmpMenuDetailTab) 
	AND (mtf.IsActive=1) 
	ORDER BY mtf.ID_MenuDetailTab,mt.SeqNo,mt.ID, mtf.SeqNo,mtf.ID
--------------------------------------------------------------------------------------------------------------
SELECT mb.* 
	INTO #tmpMenuButton
	FROM tMenuButton mb 
	WHERE 
	--ID_Menu=9 
	mb.ID_Menu IN (SELECT ID FROM #tmpMenu)
	AND (IsActive=1) 
	AND (
/*	(NOT EXISTS (SELECT ID 
						FROM vUserGroupMenuButton ugmbx 
						WHERE ugmbx.ID_MenuButton=mb.ID 
						AND ugmbx.ID_UserGroup <> @ID_UserGroup
						)) 
					OR */
					(EXISTS (SELECT ID FROM vUserGroupMenuButton ugmb 
					WHERE --ugmb.IsActive=1 AND 
					ugmb.ID_MenuButton=mb.ID 
					AND ugmb.ID_UserGroup=@ID_UserGroup 
					))
		OR mb.ID_MenuButtonType=1					
		OR @ID_UserGroup IN (1,2)			
		) 
--and 1=0
	ORDER BY mb.SeqNo,mb.ID
	-------------------------------------------------------------------
	--------------------------------------------------------------------
	-------------------------------------------------------------------
	--------------------------------------------------------------------
	-------------------------------------------------------------------
	--------------------------------------------------------------------
	-------------------------------------------------------------------
	--------------------------------------------------------------------

/*
SELECT * 
	INTO #tmpMenuChildTable
	FROM tMenuChildTable mdt 
	WHERE 
	mdt.ID_Menu IN (SELECT ID FROM #tmpMenu)
	AND (IsActive=1) 
--	AND ((NOT EXISTS (SELECT ID FROM tSystemApplicationMenuDetailTab samt WHERE samt.ID_MenuDetailTab=mdt.ID)) OR (EXISTS (SELECT ID FROM vSystemApplicationMenuDetailTab samt WHERE samt.IsActive=1 AND samt.ID_MenuDetailTab=mdt.ID))) 
	ORDER BY mdt.ID_Menu,mdt.SeqNo,mdt.ID
-----------------------------------

SELECT 
mtf.*
	INTO #tmpMenuChildTableRelation
	FROM vMenuChildTableRelation mtf 
	INNER JOIN tMenuChildTable mt ON mt.ID = mtf.ID_MenuChildTable 
	LEFT JOIN tMenu m ON m.ID = mtf.ID_Menu 
	WHERE 
	mtf.ID_MenuChildTable IN (SELECT ID FROM #tmpMenuChildTable) 
	--AND (mtf.IsActive=1) 
	ORDER BY mt.SeqNo,mt.ID--, mtf.SeqNo
	,mtf.ID
------------------------------------------------------------------------------------------------------------------
*/








	
---------------------------------------------------------------------------------------------------------
select o2.name TableName ,o1.Name ConstraintName,c.Text [Constraint] 
	INTO #tmpConstraints
	from SysObjects o1 
	INNER JOIN SysObjects o2 ON o1.parent_obj = o2.id 
	INNER JOIN SysComments c ON c.id = o1.id 
	WHERE o1.xtype = 'C'
------------------------------------------------------------------------------------------------------------\


INSERT INTO #tmpMenu 
(
  ID
, Name
, DataSource
, BaseDataSource
, TableName

, ID_Menu
, SeqNo
, ImageFile
, ReportFile

, ID_MenuType
, Sort

, IsVisible
, AllowNew
, AllowOpen
, AllowDelete
, [ReadOnly]

, ReportTitle
, ReportSubTitle

, ColorRGB
, DarkColorRGB

, ID_Session
, AllowEdit

, ID_ListMenu
, ListMenu
, ListSource
, ListRowFieldHeader
, ListRowField
, ListRowCategoryHeader
, ListRowCategory

, IsUserData
, IsSpanView
, ListFixedFilter

,SaveTrigger
)	
SELECT 
  m.ID
, m.Name
, m.DataSource
, m.BaseDataSource
, m.TableName

, m.ID_Menu
, m.SeqNo
, m.ImageFile
, m.ReportFile

, m.ID_MenuType
, m.Sort

, CAST(0 AS BIT) IsVisible
, CAST(0 AS BIT) AllowNew
, CAST(0 AS BIT) AllowOpen
, CAST(0 AS BIT) AllowDelete
, CAST(1 AS BIT) [ReadOnly]

, m.ReportTitle
, m.ReportSubTitle

, m.ColorRGB
, m.DarkColorRGB

, @ID_Session
, CAST(0 AS BIT) AllowEdit

, m.ID_ListMenu
, m.ListMenu
, m.ListSource
, m.ListRowFieldHeader
, m.ListRowField
, m.ListRowCategoryHeader
, m.ListRowCategory

, m.IsUserData
, m.IsSpanView
, m.ListFixedFilter

,SaveTrigger
FROM (SELECT DISTINCT ID_Menu 
		FROM 
		(	SELECT ID_Menu FROM #tmpMenuTabField mtf 
			UNION 
			SELECT ID_Menu FROM #tmpMenuDetailTabField mdtf 
			UNION 
			SELECT ID_DetailMenu FROM #tmpMenuDetailTab mdtf 
			UNION
			SELECT 442 -- FILE 
			) mft
		
		
		
		WHERE ID_Menu NOT IN (SELECT ID FROM #tmpMenu)) mtf
INNER JOIN vMenu m ON m.ID = mtf.ID_Menu



SELECT	s.* 
	INTO #tmpMenuSubDataSource
FROM dbo.tMenuSubDataSource s
INNER JOIN #tmpMenu m ON m.id = s.ID_Menu




-------------------------------------------------------------------------------------------------------------/
select * from #tmpMenu
select * from #tmpMenuTab
select * from #tmpMenuDetailTab
select * from #tmpMenuTabField
select * from #tmpMenuDetailTabField
select * from #tmpMenuDetailTabProperty
select * from #tmpMenuButton
select * from #tmpConstraints

SELECT DISTINCT ImageFile FROM #tmpMenu
UNION	SELECT			ImageFile FROM #tmpMenuTab
UNION	SELECT			ImageFile FROM #tmpMenuTab
UNION	SELECT			ImageFile FROM #tmpMenuDetailTabField
UNION	SELECT			ImageFile FROM #tmpMenuButton
UNION	SELECT			Name	  FROM tSystemImage

SELECT * FROM #tmpMenuSubDataSource

------------------------------------------------------------------------------
DROP TABLE #tmpMenu
DROP TABLE #tmpMenuTab
DROP TABLE #tmpMenuDetailTab
DROP TABLE #tmpMenuTabField
DROP TABLE #tmpMenuDetailTabField
DROP TABLE #tmpMenuDetailTabProperty
DROP TABLE #tmpMenuButton
DROP TABLE #tmpConstraints
DROP TABLE #tmpMenuSubDataSource
-------------------------------------------------------------------------------------------------------------
