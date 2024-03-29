/****** Object:  StoredProcedure [dbo].[pPayroll_UpdateHour]    Script Date: 08/03/2011 11:55:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[pPayroll_UpdateHour]
  @ID_PayrollPeriod INT,
  @ID_Payroll INT = NULL
  --20110803
WITH ENCRYPTION
AS
SET NOCOUNT ON

DECLARE @StartDate DATETIME,@EndDate DATETIME
SELECT 
@StartDate = ISNULL(pps.StartDate,pp.StartDate)
, @EndDate = ISNULL(pps.EndDate,pp.EndDate)
FROM tPayrollPeriod pp LEFT JOIN
tEmployeeDailyScheduleView pps ON pp.ID_EmployeeDailyScheduleView = pps.[ID]
WHERE (pp.[ID] = @ID_PayrollPeriod)


CREATE TABLE #Emp
(
ID_Employee INT
,ID_Payroll INT
,IsRequiredToLog BIT
,ID_PayrollScheme INT
,ID_Parameter INT
,Code VARCHAR(500)
)
CREATE TABLE #Hours
(
ID INT IDENTITY(1,1)
,ID_Employee INT
,ID_PayrollItem INT
,TotalDays DECIMAL(18,9)
,TotalHours DECIMAL(18,9)
,TotalMinutes DECIMAL(18,9)
,Days DECIMAL(18,9) DEFAULT(0)
,Hours DECIMAL(18,9) DEFAULT(0)
,Minutes DECIMAL(18,9) DEFAULT(0)
)

INSERT INTO #Emp
SELECT
p.ID_Employee
,p.ID ID_Payroll
,p.IsRequiredToLog
,p.ID_PayrollScheme
,p.ID_Parameter
,e.Code
FROM
tPayroll p
INNER JOIN tEmployee e ON e.ID = p.ID_Employee
WHERE 
((p.ID_PayrollPeriod = @ID_PayrollPeriod AND p.IsProcessed = 0) OR p.ID = @ID_Payroll)
--((p.ID_PayrollPeriod = @ID_PayrollPeriod) OR p.ID = @ID_Payroll)
AND p.IsBasicPay = 1
--AND p.ID_Employee = 4

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
INSERT INTO #Hours(TotalDays,TotalHours,TotalMinutes,ID_PayrollItem,ID_Employee)
SELECT
SUM(Days) TotalDays
,SUM(Hours) TotalHours
,SUM(Minutes) TotalMinutes
,ID_PayrollItem
,ID_Employee
FROM
(
	--Regular Day
	SELECT 
--		CAST(SUM(ds.Days) AS DECIMAL(18,9)) Days,0 Hours,0 Minutes, eds.ID_DayType ID_PayrollItem,eds.ID_Employee
		--CAST(SUM(CASE WHEN emp.ID_PayrollScheme = 2 THEN 0 ELSE ds.Days END) AS DECIMAL(18,9)) Days
		CAST(SUM(CASE WHEN emp.ID_PayrollScheme = 2 THEN 0 ELSE CASE WHEN emp.ID_PayrollScheme = 1 THEN ds.Days ELSE ds.Days + CASE WHEN par.compressedWorkWeek = 1 THEN ((ds.WorkingHours % par.HoursPerDay) / par.HoursPerDay) ELSE 0 END END END) AS DECIMAL(18,9)) Days
		--CAST(SUM(CASE WHEN emp.ID_PayrollScheme = 2 THEN 0 ELSE ds.Days + ((ds.WorkingHours % par.HoursPerDay) / par.HoursPerDay) END) AS DECIMAL(18,9)) DAYS --ARVIN 20110519
		,SUM(CASE WHEN emp.ID_PayrollScheme = 2 THEN eds.REG + eds.EXT ELSE 0 END) Hours
		,0 Minutes, eds.ID_DayType ID_PayrollItem,eds.ID_Employee

	FROM tEmployeeDailySchedule eds 
	INNER JOIN tDailySchedule ds ON ds.ID = eds.ID_DailySchedule
	INNER JOIN #Emp emp ON emp.ID_Employee = eds.ID_Employee
	INNER JOIN dbo.tParameter par ON par.ID = emp.ID_Parameter
	WHERE eds.Date BETWEEN  @StartDate AND @EndDate
	AND emp.IsRequiredToLog = 1
	AND  ID_DayType =1 AND IsActualAbsent = 0
	GROUP BY eds.ID_DayType,eds.ID_Employee
	---LESS HALFDAY ABSENT IN DAILY /***deduct half day in computed days***/
	Union all
	Select   CAST(SUM(CASE WHEN emp.ID_PayrollScheme = 3 then eds.Absences * -1  Else 0 END) AS DECIMAL(18,9))Days,0 Hours,0 Minutes,eds.ID_DayType ID_PayrollItem,eds.ID_Employee
	from tEmployeeDailySchedule eds
	INNER JOIN #Emp emp ON emp.ID_Employee = eds.ID_Employee
	where eds.Date BETWEEN @StartDate AND @EndDate  
	AND emp.IsRequiredToLog = 1 AND ID_Daytype = 1 AND IsActualAbsent = 0
	GROUP BY eds.ID_DayType,eds.ID_Employee
	---LESS HALFDAY ABSENT IN DAILY /***deduct half day in computed days***/
	--RD/SH/LH/LHR/SHR
	UNION ALL
	SELECT 
		CAST(0 AS DECIMAL(18,9))Days, (SUM(eds.REG) + SUM(eds.EXT)) Hours, 0 Minutes, eds.ID_DayType ID_PayrollItem,eds.ID_Employee
	FROM tEmployeeDailySchedule eds 
	INNER JOIN #Emp emp ON emp.ID_Employee = eds.ID_Employee
	WHERE eds.Date BETWEEN  @StartDate AND @EndDate
	AND emp.IsRequiredToLog = 1
	AND emp.ID_PayrollScheme = 1
	AND  ID_DayType <> 1
	GROUP BY eds.ID_DayType,eds.ID_Employee
	--OT
	UNION ALL
	SELECT 
		CAST(0  AS DECIMAL(18,9)) Days, SUM(eds.OT) Hours, 0 Minutes, eds.ID_DayType+6 ID_PayrollItem,eds.ID_Employee
	FROM tEmployeeDailySchedule eds 
	INNER JOIN #Emp emp ON emp.ID_Employee = eds.ID_Employee
	WHERE eds.Date BETWEEN  @StartDate AND @EndDate
	AND emp.IsRequiredToLog = 1
	GROUP BY eds.ID_DayType,eds.ID_Employee
	--ND
	UNION ALL
	SELECT 
		CAST(0  AS DECIMAL(18,9)) Days, SUM(eds.ND) Hours, 0 Minutes, eds.ID_DayType+12 ID_PayrollItem,eds.ID_Employee
	FROM tEmployeeDailySchedule eds 
	INNER JOIN #Emp emp ON emp.ID_Employee = eds.ID_Employee
	WHERE eds.Date BETWEEN  @StartDate AND @EndDate
	AND emp.IsRequiredToLog = 1
	GROUP BY eds.ID_DayType,eds.ID_Employee
	--NDOT
	UNION ALL
	SELECT 
		CAST(0  AS DECIMAL(18,9)) Days, SUM(eds.NDOT) Hours, 0 Minutes, eds.ID_DayType+18 ID_PayrollItem,eds.ID_Employee
	FROM tEmployeeDailySchedule eds 
	INNER JOIN #Emp emp ON emp.ID_Employee = eds.ID_Employee
	WHERE eds.Date BETWEEN  @StartDate AND @EndDate
	AND emp.IsRequiredToLog = 1
	GROUP BY eds.ID_DayType,eds.ID_Employee
	--Absences
	UNION ALL
	SELECT 
		--CAST(SUM(ISNULL(eds.Absences,0)) AS DECIMAL(18,9)) Days,0 Hours, 0 Minutes, 41 ID_PayrollItem,eds.ID_Employee
		CAST(SUM(ISNULL(eds.Absences,0)) AS DECIMAL(18,9)) * CASE WHEN par.CompressedWorkWeek = 1 THEN (ds.WorkingHours / par.HoursPerDay) ELSE 1 END Days,0 Hours, 0 Minutes, 41 ID_PayrollItem,eds.ID_Employee --ARVIN 20110519
	FROM tEmployeeDailySchedule eds 
	INNER JOIN #Emp emp ON emp.ID_Employee = eds.ID_Employee
	INNER JOIN dbo.tDailySchedule ds ON ds.ID = eds.ID_DailySchedule
	INNER JOIN dbo.tParameter par ON par.ID = emp.ID_Parameter
	WHERE eds.Date BETWEEN  @StartDate AND @EndDate
	AND emp.IsRequiredToLog = 1
	AND emp.ID_PayrollScheme = 1
	GROUP BY eds.ID_Employee,ds.WorkingHours,par.HoursPerDay,par.CompressedWorkWeek
	--UT
	UNION ALL
	SELECT 
		CAST(0  AS DECIMAL(18,9)) Days,0 Hours, SUM(eds.UT) Minutes, (SELECT CAST(Value AS INT) FROM tSetting WHERE ([Name] = 'UndertimeID')) ID_PayrollItem,eds.ID_Employee
	FROM tEmployeeDailySchedule eds 
	INNER JOIN #Emp emp ON emp.ID_Employee = eds.ID_Employee
	WHERE eds.Date BETWEEN  @StartDate AND @EndDate
	AND emp.IsRequiredToLog = 1
	AND emp.ID_PayrollScheme <> 2
	GROUP BY eds.ID_Employee
	--Tardy
	UNION ALL
	SELECT 
		CAST(0  AS DECIMAL(18,9)) Days,0 Hours, SUM(eds.Tardy) Minutes, (SELECT CAST(Value AS INT) FROM tSetting WHERE ([Name] = 'TardinessID')) ID_PayrollItem,eds.ID_Employee
	FROM tEmployeeDailySchedule eds 
	INNER JOIN #Emp emp ON emp.ID_Employee = eds.ID_Employee
	WHERE eds.Date BETWEEN  @StartDate AND @EndDate
	AND emp.IsRequiredToLog = 1
	AND emp.ID_PayrollScheme <> 2
	GROUP BY eds.ID_Employee
	-- Leave
	UNION ALL
	SELECT  CAST(l.Days AS DECIMAL(18,9)) Days, 0 Hours, 0 Minutes, l.ID_PayrollItem,  emp.ID_Employee
	FROM dbo.fGetLeaveHours (@ID_PayrollPeriod, @ID_Payroll, @StartDate, @EndDate) l
	INNER JOIN tPayroll p ON p.ID = l.ID_Payroll
	INNER JOIN #Emp emp ON p.ID_Employee = emp.ID_Employee
	--WHERE emp.ID_PayrollScheme <> 1
	--Not Required To Log
	UNION ALL
	SELECT 
	CAST(CASE WHEN p.ID_PayrollScheme = 1 THEN SUM(Days) ELSE 0 END AS DECIMAL(18,9))
	Days
	,CASE WHEN p.ID_PayrollScheme = 1 THEN 0 ELSE COUNT(ID_DayType) * CAST(WorkingHours AS DECIMAL(18,9)) END
	 Hours
	,0 Minutes, ID_DayType AS ID_PayrollItem, tmp.ID_Employee	
	FROM
	(
		SELECT [Date], ID_Payroll, ID_Employee,dbo.fEmployeeDayType(ID_Employee,Date,IsRD)  ID_DayType,ISNULL(s.WorkingHours, 8.00) AS WorkingHours, ISNULL(s.Days,1) Days
		FROM
		(	SELECT tmp.[Date], tmp.ID_Payroll, tmp.ID_Employee,dbo.fGetEmployeeDailyScheduleIsRD ( tmp.ID_Employee, tmp.[Date] ) AS IsRD,
				dbo.fGetEmployeeDailyScheduleID ( tmp.ID_Employee, tmp.[Date] ) AS ID_Schedule -- get daily schedule of employee
			FROM 
			(	-- get all employee and date from payroll here...
				SELECT e.ID_Payroll, e.ID_Employee, c.[Date]
				FROM #Emp e 
				CROSS JOIN dbo.fCalendar(@StartDate, @EndDate) c
				WHERE e.IsRequiredToLog = 0
			) tmp 
		) tmp 
		LEFT OUTER JOIN tDailySchedule s ON tmp.ID_Schedule = s.ID
	) tmp
	LEFT JOIN tPayroll p ON p.ID_Employee = tmp.ID_Employee
	WHERE (tmp.ID_DayType = 1) -- exclude RD, SH, and LH
	AND p.ID_PayrollPeriod = @ID_PayrollPeriod
	GROUP BY  tmp.ID_Employee, WorkingHours, ID_DayType,p.ID_PayrollScheme 
	--LH/SH 
	--UNION ALL
	--SELECT
	--CAST(SUM(
	--CASE	 WHEN e.ID_PayrollScheme in (1,2,3) THEN
	--		--CASE WHEN h.ID_HolidayType = 5 THEN 
	--			--((ds.workingHours % par.hoursPerDay) / par.HoursPErDay)
	--		--ELSE 
	--			0
	--		--END 
	--	ELSE
	--		CASE 	WHEN h.ID_HolidayType = 5 THEN ds.Days
	--				WHEN h.ID_HolidayType = 3 AND h.IsWorking = 0 AND  ((eds.REG+eds.OT+eds.ND+eds.NDOT) = 0 ) THEN CAST(ds.Days AS DECIMAL(18,9))
	--		ELSE 0 END 
	--		--ELSE ds.Days + ((ds.WorkingHours % par.HoursPerDay) / par.HoursPerDay) End
	--END 
	--) AS DECIMAL(18,9)) Days
	-- , 0 Hours, 0 Minutes,1 ID_PayrollItem, e.ID_Employee
	--FROM
	--#Emp e 
	--INNER JOIN tEmployeeDailySchedule eds ON eds.ID_Employee = e.ID_Employee
	--INNER JOIN tPArameter par ON par.ID = e.ID_Parameter
	--INNER JOIN tDailySchedule ds ON ds.ID = eds.ID_DailySchedule
	--INNER JOIN tHoliday h ON h.Date= eds.Date
	--WHERE eds.Date BETWEEN @StartDate AND @EndDate 
	--AND e.IsRequiredToLog = 1
	--AND h.ID_HolidayType IN (3,5)
	--AND eds.IsRD <> 1
	--GROUP BY e.ID_Employee
	
	UNION ALL
	SELECT 
	CASE WHEN e.ID_PayrollScheme = 1 THEN 0 ELSE CAST(SUM(ds.DAYS) AS DECIMAL(18,2)) End Days, CASE WHEN ID_PayrollScheme = 1 THEN 0 ELSE CAST(SUM(ds.DAYS) AS DECIMAL(18,2)) * par.HoursPerDay END  Hours, 0 Minutes, eds.ID_DayType ID_PayrollItem,eds.ID_Employee
	FROM
	#Emp e 
	INNER JOIN tEmployeeDailySchedule eds ON eds.ID_Employee = e.ID_Employee
	INNER JOIN tPArameter par ON par.ID = e.ID_Parameter
	INNER JOIN tDailySchedule ds ON ds.ID = eds.ID_DailySchedule
	INNER JOIN tHoliday h ON h.Date= eds.Date
	WHERE eds.Date BETWEEN @StartDate AND @EndDate 
	AND e.IsRequiredToLog = 1
	AND h.ID_HolidayType IN (5)
	AND eds.IsRD <> 1
	GROUP BY eds.ID_Employee , e.ID_PayrollScheme , eds.ID_DayType, par.HoursPerDay
	
	UNION ALL --ARVIN 20110526
	SELECT 
	CASE WHEN e.ID_PayrollScheme = 1 THEN 0 ELSE CAST(SUM(ds.DAYS) AS DECIMAL(18,2)) + CASE WHEN par.CompressedWorkWeek = 1 THEN (((ds.WorkingHours % par.HoursPerDay) / par.HoursPerDay)) ELSE 0 END End Days, CASE WHEN ID_PayrollScheme = 1 THEN 0 ELSE CAST(SUM(ds.DAYS) AS DECIMAL(18,2)) + CASE WHEN par.CompressedWorkWeek = 1 THEN (((ds.WorkingHours % par.HoursPerDay) / par.HoursPerDay)) ELSE 0 END  * par.HoursPerDay END  Hours, 0 Minutes, 1 ID_PayrollItem,eds.ID_Employee
	FROM
	#Emp e 
	INNER JOIN tEmployeeDailySchedule eds ON eds.ID_Employee = e.ID_Employee
	INNER JOIN tPArameter par ON par.ID = e.ID_Parameter
	INNER JOIN tDailySchedule ds ON ds.ID = eds.ID_DailySchedule
	INNER JOIN tHoliday h ON h.Date= eds.Date
	WHERE eds.Date BETWEEN @StartDate AND @EndDate 
	AND e.IsRequiredToLog = 1
	AND h.ID_HolidayType IN (3,5)
	AND eds.IsRD <> 1 AND (Reg + OT + ND + NDOT ) > 0
	GROUP BY eds.ID_Employee , e.ID_PayrollScheme , eds.ID_DayType, par.HoursPerDay,ds.WorkingHours,par.CompressedWorkWeek
	
	UNION ALL --ARVIN 20110526 SLDaily
	SELECT 
	CASE WHEN e.ID_PayrollScheme = 1 THEN 0 ELSE CAST(SUM(ds.DAYS) AS DECIMAL(18,2)) End Days, CASE WHEN ID_PayrollScheme = 1 THEN 0 ELSE CAST(SUM(ds.DAYS) AS DECIMAL(18,2)) * par.HoursPerDay END  Hours, 0 Minutes, eds.ID_DayType ID_PayrollItem,eds.ID_Employee
	FROM
	#Emp e 
	INNER JOIN tEmployeeDailySchedule eds ON eds.ID_Employee = e.ID_Employee
	INNER JOIN tPArameter par ON par.ID = e.ID_Parameter
	INNER JOIN tDailySchedule ds ON ds.ID = eds.ID_DailySchedule
	INNER JOIN tHoliday h ON h.Date= eds.Date
	WHERE eds.Date BETWEEN @StartDate AND @EndDate 
	AND e.IsRequiredToLog = 1
	AND h.ID_HolidayType IN (3) AND (Reg + OT + ND + NDOT ) > 0
	AND eds.IsRD <> 1
	GROUP BY eds.ID_Employee , e.ID_PayrollScheme , eds.ID_DayType, par.HoursPerDay
	
/*	--SH - Monthly
	UNION ALL
	SELECT
	SUM(
	CASE WHEN h.ID_HolidayType = 3 AND  ((eds.REG+eds.OT+eds.ND+eds.NDOT) = 0 )   THEN CAST(ds.Days AS DECIMAL(18,9))
	ELSE 0 END 
	)  Days
	 , 0 Hours, 0 Minutes,41 ID_PayrollItem, e.ID_Employee
	FROM
	#Emp e 
	INNER JOIN tEmployeeDailySchedule eds ON eds.ID_Employee = e.ID_Employee
	INNER JOIN tDailySchedule ds ON ds.ID = eds.ID_DailySchedule
	INNER JOIN tHoliday h ON h.Date= eds.Date
	WHERE eds.Date BETWEEN @StartDate AND @EndDate 
	AND e.ID_PayrollScheme = 1
	AND e.IsRequiredToLog = 1
	AND h.ID_HolidayType = 3
	AND eds.IsRD <> 1
	GROUP BY e.ID_Employee*/
) a
GROUP BY
ID_PayrollItem
,ID_Employee

UPDATE h SET
h.Hours = FLOOR(h.TotalMinutes / 60.00)
,h.Minutes = h.TotalMinutes - (FLOOR(h.TotalMinutes / 60.00) * 60.00)
FROM #Hours h
INNER JOIN #Emp e ON e.ID_Employee = h.ID_Employee
INNER JOIN tParameter param ON param.ID = e.ID_Parameter
WHERE h.ID_PayrollItem IN (42,44)

UPDATE h SET
h.Days = FLOOR(h.TotalHours / param.HoursPerDay)
,h.Hours = h.TotalHours - (FLOOR(h.TotalHours / param.HoursPerDay) * param.HoursPerDay)
FROM #Hours h
INNER JOIN #Emp e ON e.ID_Employee = h.ID_Employee
INNER JOIN tParameter param ON param.ID = e.ID_Parameter
WHERE h.ID_PayrollItem <25 AND h.ID_PayrollItem <> 1

UPDATE h SET
h.Days = TotalDays
FROM #Hours h
INNER JOIN #Emp e ON e.ID_Employee = h.ID_Employee
INNER JOIN tParameter param ON param.ID = e.ID_Parameter
INNER JOIN tPayrollItem i ON i.ID = h.ID_PayrollItem
WHERE 
(h.ID_PayrollItem = 1 OR h.ID_PayrollItem = 41 OR i.ID_PayrollItemCategory = 3)
AND e.ID_PayrollScheme <> 2


UPDATE h SET
h.Days = FLOOR(h.TotalHours / param.HoursPerDay)
,h.Hours = h.TotalHours - (FLOOR(h.TotalHours / param.HoursPerDay) * param.HoursPerDay)
FROM #Hours h
INNER JOIN #Emp e ON e.ID_Employee = h.ID_Employee
INNER JOIN tParameter param ON param.ID = e.ID_Parameter
INNER JOIN tPayrollItem i ON i.ID = h.ID_PayrollItem
WHERE
 (h.ID_PayrollItem = 1 OR h.ID_PayrollItem = 41 OR i.ID_PayrollItemCategory = 3)
AND e.ID_PayrollScheme = 2 



---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
--PayrollFile
DELETE
FROM #Hours
WHERE ID_Employee IN
(SELECT DISTINCT emp.ID_Employee
FROM tPayrollFile pf 
INNER JOIN tPayrollFile_Detail pfd ON pf.ID = pfd.ID_PayrollFile
INNER JOIN #Emp emp ON emp.Code = pfd.EmployeeCode
INNER JOIN tPayrollPeriod pp ON pp.ID_Company = pf.ID_Company AND pp.Year = pf.Year AND pp.ID_Month = pf.ID_Month AND pp.ID_PayrollSchedule = pf.ID_payrollSchedule
AND  pp.ID = @ID_PayrollPeriod)
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
INSERT INTO #Hours(TotalHours,ID_PayrollItem,ID_Employee)
SELECT
		SUM(CASE 	WHEN i.ID =1 THEN pfd.REG
		WHEN i.ID =2 THEN  pfd.RD
		WHEN i.ID =3 THEN pfd.SH
		WHEN i.ID =4 THEN pfd.SHR
		WHEN i.ID =5 THEN pfd.LH
		WHEN i.ID =6 THEN pfd.LHR
		WHEN i.ID =7 THEN pfd.OT
		WHEN i.ID =8 THEN pfd.RDOT
		WHEN i.ID =9 THEN pfd.SHOT
		WHEN i.ID =10 THEN pfd.SHROT
		WHEN i.ID =11 THEN pfd.LHOT
		WHEN i.ID =12 THEN pfd.LHROT
		WHEN i.ID =13 THEN pfd.ND
		WHEN i.ID =14 THEN pfd.RDND
		WHEN i.ID =15 THEN pfd.SHND
		WHEN i.ID =16 THEN pfd.SHRND
		WHEN i.ID =17 THEN pfd.LHND
		WHEN i.ID =18 THEN pfd.LHRND
		WHEN i.ID =19 THEN pfd.NDOT
		WHEN i.ID =20 THEN pfd.RDNDOT
		WHEN i.ID =21 THEN pfd.SHNDOT
		WHEN i.ID =22 THEN pfd.SHRNDOT
		WHEN i.ID =23 THEN pfd.LHNDOT
		WHEN i.ID =24 THEN pfd.LHRNDOT

		WHEN i.ID =41 THEN CASE WHEN emp.ID_PayrollScheme = 2 THEN 0 ELSE (pfd.LWOP * 1.00) * param.hoursperday END 
		WHEN i.ID =42 THEN CASE WHEN emp.ID_PayrollScheme = 2 THEN 0 ELSE pfd.UT / 60.00 END 
		WHEN i.ID =44 THEN CASE WHEN emp.ID_PayrollScheme = 2 THEN 0 ELSE pfd.TARDY/60.00 END

		WHEN i.ID = 26 THEN (pfd.VL * 1.00) * param.hoursperday
		WHEN i.ID = 27 THEN (pfd.SL * 1.00) * param.hoursperday
		ELSE 0 END) TotalHours
		,i.ID ID_PayrollItem
		,emp.ID_Employee
FROM tPayrollFile pf 
INNER JOIN tPayrollFile_Detail pfd ON pf.ID = pfd.ID_PayrollFile
INNER JOIN #Emp emp ON emp.Code = pfd.EmployeeCode
INNER JOIN tParameter param ON param.ID = emp.ID_Parameter
INNER JOIN tPayrollPeriod pp ON pp.ID_Company = pf.ID_Company AND pp.Year = pf.Year AND pp.ID_Month = pf.ID_Month AND pp.ID_PayrollSchedule = pf.ID_payrollSchedule
CROSS JOIN tPayrollItem i
WHERE i.IsActive = 1 AND (i.ID < 25 OR i.ID IN(41,42,44,26,27))
AND  pp.ID = @ID_PayrollPeriod
GROUP BY i.ID, emp.ID_Employee
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
UPDATE h SET
h.Hours =FLOOR(TotalHours)
,h.Minutes = (TotalHours*60) - (FLOOR(TotalHours) * 60.00)
FROM #Hours h
INNER JOIN #Emp e ON e.ID_Employee = h.ID_Employee
INNER JOIN tPayrollFile_Detail pfd ON pfd.EmployeeCode = e.Code
INNER JOIN tPayrollFile pf ON pf.ID = pfd.ID_PayrollFile
INNER JOIN tParameter param ON param.ID = e.ID_Parameter
INNER JOIN tPayrollPeriod pp ON pp.ID_Company = pf.ID_Company AND pp.Year = pf.Year AND pp.ID_Month = pf.ID_Month AND pp.ID_PayrollSchedule = pf.ID_payrollSchedule
WHERE h.ID_PayrollItem IN (42,44)
AND  pp.ID = @ID_PayrollPeriod

UPDATE h SET
h.Days = FLOOR(h.TotalHours / param.HoursPerDay)
,h.Hours = h.TotalHours - (FLOOR(h.TotalHours / param.HoursPerDay) * param.HoursPerDay)
FROM #Hours h
INNER JOIN #Emp e ON e.ID_Employee = h.ID_Employee
INNER JOIN tPayrollFile_Detail pfd ON pfd.EmployeeCode = e.Code
INNER JOIN tPayrollFile pf ON pf.ID = pfd.ID_PayrollFile
INNER JOIN tParameter param ON param.ID = e.ID_Parameter
INNER JOIN tPayrollPeriod pp ON pp.ID_Company = pf.ID_Company AND pp.Year = pf.Year AND pp.ID_Month = pf.ID_Month AND pp.ID_PayrollSchedule = pf.ID_payrollSchedule
WHERE h.ID_PayrollItem <25 --AND h.ID_PayrollItem <> 1
AND  pp.ID = @ID_PayrollPeriod


UPDATE h SET
h.Days = TotalHours / param.HoursPerDay
FROM #Hours h
INNER JOIN #Emp e ON e.ID_Employee = h.ID_Employee
INNER JOIN tPayrollFile_Detail pfd ON pfd.EmployeeCode = e.Code
INNER JOIN tPayrollFile pf ON pf.ID = pfd.ID_PayrollFile
INNER JOIN tParameter param ON param.ID = e.ID_Parameter
INNER JOIn tPayrollItem i ON i.ID = h.ID_PayrollItem
INNER JOIN tPayrollPeriod pp ON pp.ID_Company = pf.ID_Company AND pp.Year = pf.Year AND pp.ID_Month = pf.ID_Month AND pp.ID_PayrollSchedule = pf.ID_payrollSchedule
WHERE  h.ID_PayrollItem = 41 OR i.ID_PayrollItemCategory = 3
AND  pp.ID = @ID_PayrollPeriod

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|

UPDATE pd SET
pd.Days = h.Days
,pd.Hours = h.Hours
,pd.Minutes = h.Minutes
,pd.TotalHours = (h.Days * par.HoursPerDay) + h.Hours + h.Minutes / CAST(60.00 AS DECIMAL(18,9))
FROM tPayroll_Detail pd
INNER JOIN tPayroll p ON pd.ID_Payroll = p.ID 
INNER JOIN #Emp e ON e.ID_Employee = p.ID_Employee
INNER JOIN #Hours h ON h.ID_Employee = e.ID_Employee AND h.ID_PayrollItem = pd.ID_PayrollItem
INNER JOIN tParameter par ON par.ID = e.ID_Parameter
WHERE ((p.ID_PayrollPeriod = @ID_PayrollPeriod AND p.IsProcessed = 0) OR p.ID = @ID_Payroll)
AND p.IsBasicPay = 1

---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
UPDATE p SET
TotalHoursWorked = a.HoursWorked
,LeaveWithPay = a.Leaves
FROM tPayroll p
INNER JOIN
(
	SELECT
	SUM(CASE WHEN i.ID_PayrollItemCategory = 3 THEN pd.TotalHours ELSE 0 END) Leaves
	,SUM(CASE WHEN i.ID < 25 THEN pd.TotalHours ELSE 0 END) HoursWorked
	,p.ID_Employee
	,p.ID ID_Payroll
	FROM tPayroll_Detail pd
	INNER JOIN tPayroll p ON p.ID = pd.ID_Payroll
	INNER JOIN tPayrollItem i ON i.ID = pd.ID_PayrollItem 
	WHERE ((p.ID_PayrollPeriod = @ID_PayrollPeriod AND p.IsProcessed = 0) OR p.ID = @ID_Payroll)
	AND p.IsBasicPay = 1
	GROUP BY p.ID_Employee,p.ID
)  a ON a.ID_Payroll = p.ID


---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------\


DROP TABLE #Hours
DROP TABLE #Emp
