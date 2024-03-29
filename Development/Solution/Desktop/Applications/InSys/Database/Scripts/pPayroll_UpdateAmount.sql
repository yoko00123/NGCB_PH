/****** Object:  StoredProcedure [dbo].[pPayroll_UpdateAmount]    Script Date: 08/03/2011 11:54:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[pPayroll_UpdateAmount]
  @ID_PayrollPeriod INT,
  @ID_Payroll INT = NULL,
  @ID_User INT = NULL
  --20110803
WITH ENCRYPTION
AS
SET NOCOUNT ON

DECLARE @StartDate DATETIME
DECLARE @EndDate DATETIME
DECLARE @TotalHours NUMERIC(19,9)
DECLARE @TotalAmount NUMERIC(19,2)
DECLARE @ID_Company INT
DECLARE @ID_Branch INT


SELECT 
	  @ID_Company = pp.ID_Company
	, @StartDate = ISNULL(pps.StartDate,pp.StartDate)
	, @EndDate = ISNULL(pps.EndDate,pp.EndDate)
FROM 	
	tPayrollPeriod pp 
	LEFT JOIN tEmployeeDailyScheduleView pps ON pp.ID_EmployeeDailyScheduleView = pps.[ID]
WHERE 
	(pp.[ID] = @ID_PayrollPeriod)

-- SELECT d.Days, d.Hours, d.Minutes, d.Amt, d.Adj, pr.Rate
UPDATE d
-- SET d.Amt = ROUND(d.Hours * p.HourlyRate * pr.Rate, 2)	
SET d.Amt = ROUND(
	CASE WHEN d.TotalHours > 0.00 THEN
		CASE WHEN (p.ID_PayrollScheme = 1) AND (d.ID_PayrollItem = 1) THEN
				CASE WHEN p.ID_PayrollFrequency BETWEEN 3 AND 4 THEN
					 p.MonthlyRate /
						CASE p.ID_PayrollFrequency
							WHEN 3 THEN 2		-- 3 is semi-monthly
							WHEN 4 THEN 1 END	-- 4 is monthly
					 
				ELSE
					-- compute like Daily payroll scheme employees...
					d.TotalHours *
					p.HourlyRate *
					
	--				CASE WHEN (p.ID_PayrollScheme = 1) AND (d.ID_PayrollItem BETWEEN 2 AND 6) THEN pr.Rate - 1 ELSE pr.Rate END
					ISNULL(pr.Rate,1)
				END
			--20081111 - set amount to 0 if payrollitem is leave  and scheme is monthly
			WHEN p.ID_PayrollScheme = 1 AND item.ID_PayrollItemCategory = 3 THEN
				0.00
			WHEN p.ID_PayrollScheme = 3 THEN 	
					d.TotalHours * 
					p.HourlyRate *
					ISNULL(pr.Rate,1)
		ELSE 
			d.TotalHours * 
			p.HourlyRate *
--			CASE WHEN (p.ID_PayrollScheme = 1) AND (d.ID_PayrollItem BETWEEN 2 AND 6) THEN pr.Rate - 1 ELSE pr.Rate END
			ISNULL(pr.Rate,1)
		END
	ELSE 0 END
+ (d.TotalHours * pr.Amt),2)
	-- less the leave (generic)
--	-
--	CASE WHEN (p.ID_PayrollScheme = 1) AND (d.ID_PayrollItem = 1) AND (lh.Hours IS NOT NULL) THEN
--		lh.Hours * p.HourlyRate
--	ELSE 0 END
FROM tPayroll_Detail d INNER JOIN
	tPayroll p ON d.ID_Payroll = p.ID INNER JOIN
	tPayrollItemRate pr ON p.ID_Parameter = pr.ID_Parameter AND d.ID_PayrollItem = pr.ID_PayrollItem 
--20081111 - set amount to 0 if payrollitem is leave  and scheme is monthly
INNER JOIN tPayrollItem item ON item.ID = d.ID_PayrollItem
/*
LEFT OUTER JOIN
	(	-- leave function for leave hours
		SELECT ID_Payroll, 1 AS ID_PayrollItem, SUM(Hours) AS Hours
		FROM dbo.fGetLeaveHours (@ID_PayrollPeriod, @ID_Payroll, @StartDate, @EndDate)
		GROUP BY ID_Payroll
	) lh ON d.ID_Payroll = lh.ID_Payroll AND d.ID_PayrollItem = lh.ID_PayrollItem
*/
WHERE (((p.ID_PayrollPeriod = @ID_PayrollPeriod) AND (p.IsProcessed = 0)) OR (p.ID = @ID_Payroll)) AND (p.IsBasicPay = 1)
AND (((p.TotalHoursWorked > 0 OR p.LeaveWithPay > 0) AND p.IsRequiredToLog = 1 ) OR p.IsRequiredToLog = 0)

-- payroll item setup...
UPDATE d
SET d.Amt = ISNULL(ROUND(tmp.Amt, 2), 0)
FROM tPayroll_Detail d INNER JOIN
	(
	SELECT d.ID,
		CASE WHEN (pis.ID_PayrollItemSetupOption IS NOT NULL) THEN
			CASE WHEN p.ID_PayrollScheme = 1 THEN -- monthly
				-- note: get monthly cola, then divide using frequency
				CASE pis.ID_PayrollItemSetupOption
				WHEN 1 THEN -- Affected by attendance (Houly)
					(pis.Amt * param.HoursPerDay * param.DaysPerYear) / 12
				WHEN 2 THEN -- Affected by attendance (Daily)
				(pis.Amt * param.DaysPerYear) / 12
				WHEN 3 then -- Affected by absences
				pis.Amt
				When 4 then -- Affected by attendance (COLA)
				(pis.amt * ISNULL(cola.[Days],0))
				END /
				CASE WHEN pis.ID_PayrollItemSetupOption NOT IN(3,4) then
					CASE WHEN p.ID_PayrollFrequency = 3 THEN 2
					WHEN p.ID_PayrollFrequency = 4 THEN 1 END
				else
					1
				END
				/* -
				-- less absenses
				(pis.Amt * 
				CASE pis.ID_PayrollItemSetupOption
				WHEN 1 THEN -- Affected by attendance (Houly)
					absenses.Hours
				WHEN 2 THEN -- Affected by attendance (Daily)
					absenses.Days
				END)*/
			WHEN p.ID_PayrollScheme = 2 OR p.ID_PayrollScheme = 3 THEN -- daily
				CASE pis.ID_PayrollItemSetupOption
				WHEN 1 THEN -- Affected by attendance (Houly)
					tmp.Hours * pis.Amt
				WHEN 2 THEN -- Affected by attendance (Daily)
					tmp.Days * pis.Amt
				WHEN 3 then -- Affected by absences
					pis.Amt
				When 4 then -- Affected by attendance (COLA)
					(pis.Amt * ISNULL(cola.[Days],0))- ((((pis.Amt/60.00)/8.00)* ISNULL(cola.tardy,0)) + (((pis.Amt/60.00)/8.00)* ISNULL(cola.UT,0)))
				END
				/* -
				-- less absenses
				(pis.Amt * 
				CASE pis.ID_PayrollItemSetupOption
				WHEN 1 THEN -- Affected by attendance (Houly)
					absenses.Hours
				WHEN 2 THEN -- Affected by attendance (Daily)
					absenses.Days
				END)*/
			END
			-
			-- less absenses
			CASE pis.ID_PayrollItemSetupOption
			when 1 then 
				(pis.Amt * absenses.Hours)
			when 2 then 
				(pis.Amt * absenses.Days)
			WHEN 3 then
				ISNULL(tkab.AbDays,0)  * ((pis.Amt*12)/param.DaysPerYear)
			when 4 then 
				0				
			END
		ELSE
			pis.Amt
		END  AS Amt
	FROM tPayroll p INNER JOIN
		tPayroll_Detail d ON p.ID = d.ID_Payroll INNER JOIN
		tParameter param ON p.ID_Parameter = param.ID INNER JOIN
		tPayrollItemSetup pis ON p.ID_Parameter = pis.ID_Parameter AND d.ID_PayrollItem = pis.ID_PayrollItem LEFT OUTER JOIN
		(
		SELECT p.ID AS ID_Payroll, SUM(d.Days) AS Days, SUM(d.TotalHours) AS Hours
		FROM tPayroll p INNER JOIN
			tPayroll_Detail d ON p.ID = d.ID_Payroll
		WHERE (((p.ID_PayrollPeriod = @ID_PayrollPeriod) AND (p.IsProcessed = 0)) OR (p.ID = @ID_Payroll)) AND (p.IsBasicPay = 1) AND
			-- ((d.ID_PayrollItem BETWEEN 1 AND 6) OR (d.ID_PayrollItem IN (SELECT ID FROM tPayrollItem WHERE ID_PayrollItemCategory = 3)))
			((d.ID_PayrollItem = 1) OR (d.ID_PayrollItem IN (SELECT ID FROM tPayrollItem WHERE ID_PayrollItemCategory = 3)))
		GROUP BY p.ID
		) tmp ON p.ID = tmp.ID_Payroll LEFT OUTER JOIN
		(
		SELECT p.ID AS ID_Payroll, SUM(d.Days) AS Days, SUM(d.TotalHours) AS Hours
		FROM tPayroll p INNER JOIN
			tPayroll_Detail d ON p.ID = d.ID_Payroll
		WHERE (((p.ID_PayrollPeriod = @ID_PayrollPeriod) AND (p.IsProcessed = 0)) OR (p.ID = @ID_Payroll)) AND (p.IsBasicPay = 1) AND
			(d.ID_PayrollItem BETWEEN 41 AND 42)
		GROUP BY p.ID
		) absenses ON p.ID = absenses.ID_Payroll
		LEFT OUTER JOIN
		(
		Select eds.ID_Employee,SUM(eds.Absences)AbDays
		from tEmployeeDailySchedule eds
		Where eds.Date BETWEEN @StartDate AND @EndDate
		AND Absences <> 0
		group By eds.ID_Employee
		)tkab ON tkab.ID_Employee = p.ID_Employee
		LEFT OUTER JOIN
		(
			Select eds.ID_Employee,SUM(CASE WHEN Absences = 0 then 1 WHEN Absences = .5 then .5 else 0 END)[Days],SUM(eds.tardy)tardy,SUM(eds.UT)UT
			from tEmployeeDailySchedule eds
			Where eds.Date Between @StartDate AND @EndDate AND eds.IsRd <> 1 --and eds.Reg <>0
			--AND eds.ID_Employee = 1111
			Group By eds.ID_Employee 
		)cola ON cola.ID_Employee = p.ID_Employee
	WHERE (((p.ID_PayrollPeriod = @ID_PayrollPeriod) AND (p.IsProcessed = 0)) OR (p.ID = @ID_Payroll)) AND (p.IsBasicPay = 1) AND
		(pis.ID_Employee IS NULL) AND (pis.Amt <> 0.00) AND (d.IsScheduled = 1) -- AND (tmp.Hours > 0.00) -- when tmp.Hours then no attendance at all, zero the amount
		AND (((p.TotalHoursWorked > 0 OR p.LeaveWithPay > 0) AND p.IsRequiredToLog = 1) OR p.IsRequiredToLog = 0)
	) tmp ON d.ID = tmp.ID

-- same as above, only difference is pis table points to employee...
-- here is the changes; try searching these:
-- WHERE (pis.ID_Employee IS NOT NULL)
-- tPayrollItemSetup pis ON p.ID_Employee = pis.ID_Employee


-- THis CODE IS FOR PAYROLL ITEM SETUP THAT POINTS TO PARAMETER,
-- SYSTEM MUST SEARCH FOR EMPLOYEE FIRST BEFORE THE PARAMETER

UPDATE d
SET d.Amt = ISNULL(ROUND(tmp.Amt, 2), 0)
FROM tPayroll_Detail d INNER JOIN
	(
	SELECT d.ID,
		CASE WHEN (pis.ID_PayrollItemSetupOption IS NOT NULL) THEN
			CASE WHEN p.ID_PayrollScheme = 1 THEN -- monthly
				-- note: get monthly cola, then divide using frequency
				CASE pis.ID_PayrollItemSetupOption
				WHEN 1 THEN -- Affected by attendance (Houly)
					(pis.Amt * param.HoursPerDay * param.DaysPerYear) / 12
				WHEN 2 THEN -- Affected by attendance (Daily)
				(pis.Amt * param.DaysPerYear) / 12
				WHEN 3 then -- Affected by absences
				pis.Amt
				When 4 then -- Affected by attendance (COLA)
				(pis.amt * ISNULL(cola.[Days],0))
				END /
				CASE WHEN pis.ID_PayrollItemSetupOption NOT IN(3,4) then
					CASE WHEN p.ID_PayrollFrequency = 3 THEN 2
					WHEN p.ID_PayrollFrequency = 4 THEN 1 END
				else
					1
				END
				/* -
				-- less absenses
				(pis.Amt * 
				CASE pis.ID_PayrollItemSetupOption
				WHEN 1 THEN -- Affected by attendance (Houly)
					absenses.Hours
				WHEN 2 THEN -- Affected by attendance (Daily)
					absenses.Days
				END)*/
			WHEN p.ID_PayrollScheme = 2 OR p.ID_PayrollScheme = 3  THEN -- daily
				CASE pis.ID_PayrollItemSetupOption
				WHEN 1 THEN -- Affected by attendance (Houly)
					tmp.Hours * pis.Amt
				WHEN 2 THEN -- Affected by attendance (Daily)
					tmp.Days * pis.Amt
				WHEN 3 then -- Affected by absences
					pis.Amt
				When 4 then -- Affected by attendance (COLA)
					(pis.Amt * ISNULL(cola.[Days],0))  - ((((pis.Amt/60.00)/8.00)* ISNULL(cola.tardy,0)) + (((pis.Amt/60.00)/8.00)* ISNULL(cola.UT,0)))
				END
				/* -
				-- less absenses
				(pis.Amt * 
				CASE pis.ID_PayrollItemSetupOption
				WHEN 1 THEN -- Affected by attendance (Houly)
					absenses.Hours
				WHEN 2 THEN -- Affected by attendance (Daily)
					absenses.Days
				END)*/
			END
			-
			-- less absenses
			CASE pis.ID_PayrollItemSetupOption
			when 1 then 
				(pis.Amt * absenses.Hours)
			when 2 then 
				(pis.Amt * absenses.Days)
			WHEN 3 then
				ISNULL(tkab.AbDays,0)  * ((pis.Amt*12)/param.DaysPerYear)
			when 4 then 
				0				
			END
		ELSE
			pis.Amt
		END AS Amt
	FROM tPayroll p INNER JOIN
		tPayroll_Detail d ON p.ID = d.ID_Payroll INNER JOIN
		tParameter param ON p.ID_Parameter = param.ID INNER JOIN
		tPayrollItemSetup pis ON p.ID_Employee = pis.ID_Employee AND d.ID_PayrollItem = pis.ID_PayrollItem LEFT OUTER JOIN
		(
		SELECT p.ID AS ID_Payroll, SUM(d.Days) AS Days, SUM(d.TotalHours) AS Hours
		FROM tPayroll p INNER JOIN
			tPayroll_Detail d ON p.ID = d.ID_Payroll
		WHERE (((p.ID_PayrollPeriod = @ID_PayrollPeriod) AND (p.IsProcessed = 0)) OR (p.ID = @ID_Payroll)) AND (p.IsBasicPay = 1) AND
			-- ((d.ID_PayrollItem BETWEEN 1 AND 6) OR (d.ID_PayrollItem IN (SELECT ID FROM tPayrollItem WHERE ID_PayrollItemCategory = 3)))
			((d.ID_PayrollItem = 1) OR (d.ID_PayrollItem IN (SELECT ID FROM tPayrollItem WHERE ID_PayrollItemCategory = 3)))
		GROUP BY p.ID
		) tmp ON p.ID = tmp.ID_Payroll LEFT OUTER JOIN
		(
		SELECT p.ID AS ID_Payroll, SUM(d.Days) AS Days, SUM(d.TotalHours) AS Hours
		FROM tPayroll p INNER JOIN
			tPayroll_Detail d ON p.ID = d.ID_Payroll
		WHERE (((p.ID_PayrollPeriod = @ID_PayrollPeriod) AND (p.IsProcessed = 0)) OR (p.ID = @ID_Payroll)) AND (p.IsBasicPay = 1) AND
			(d.ID_PayrollItem BETWEEN 41 AND 42)
		GROUP BY p.ID
		) absenses ON p.ID = absenses.ID_Payroll
		LEFT OUTER JOIN
		(
		Select eds.ID_Employee,SUM(eds.Absences)AbDays
		from tEmployeeDailySchedule eds
		Where eds.Date BETWEEN @StartDate AND @EndDate
		AND Absences <> 0
		group By eds.ID_Employee
		)tkab ON tkab.ID_Employee = p.ID_Employee
		LEFT OUTER JOIN
		(
			Select eds.ID_Employee,SUM(CASE WHEN Absences = 0 then 1 WHEN Absences = .5 then .5 else 0 END)[Days],SUM(eds.tardy)tardy,SUM(eds.UT)UT
			from tEmployeeDailySchedule eds
			Where eds.Date Between @StartDate AND @EndDate AND eds.IsRd <> 1 --and eds.Reg <>0
			--AND eds.ID_Employee = 1111
			Group By eds.ID_Employee 
		)cola ON cola.ID_Employee = p.ID_Employee
	WHERE (((p.ID_PayrollPeriod = @ID_PayrollPeriod) AND (p.IsProcessed = 0)) OR (p.ID = @ID_Payroll)) AND (p.IsBasicPay = 1) AND
		(pis.ID_Employee IS NOT NULL) AND (pis.Amt <> 0.00) AND (d.IsScheduled = 1) -- AND (tmp.Hours > 0.00) -- when tmp.Hours then no attendance at all, zero the amount
		AND (((p.TotalHoursWorked > 0 OR p.LeaveWithPay > 0) AND p.IsRequiredToLog = 1) OR p.IsRequiredToLog = 0)
	) tmp ON d.ID = tmp.ID
-- END payroll item setup...
--MEALALLOWANCE----------------------------------------------------------------------|
Update pd set Amt = Ma.MealAllowance from 
tPayroll_Detail pd 
left join tPAyroll p on pd.ID_Payroll = p.ID
inner join (select eds.ID_Employee  , SUM(eds.MealAllowance) MealAllowance from tEmployeeDailySchedule eds
inner join (Select ISNULL(edsv.StartDate,pp.StartDate) StartDate , ISNULL(edsv.EndDate,pp.EndDate) EndDate from 
			tPayrollPeriod pp 
			left join tEmployeeDailyScheduleView edsv on edsv.ID = pp.ID_EmployeeDailyScheduleView
			where pp.ID =@ID_PayrollPEriod
			) co on eds.Date between co.StartDate and co.EndDate
			
			group BY eds.ID_Employee
			
			
)MA ON ma.ID_Employee = p.ID_Employee
inner join tPayrollItemSetup pis on pis.ID_Employee = p.ID_Employee and pis.ID_PayrollItem = pd.ID_PayrollItem
where p.ID_PayrollPeriod = @ID_PayrollPeriod
and pd.ID_PayrollItem = 80 
--MEALALLOWANCE----------------------------------------------------------------------|



--ServiceCharge----------------------------------------------------------------------|
UPDATE pd
SET
pd.Amt = scd.TotalAmount * (scps.Percentage / 100.00)
FROM
tServiceCharge sc 
INNER JOIN tServiceCharge_Detail scd ON scd.ID_ServiceCharge = sc.ID
INNER JOIN tServiceChargePaymentSchedule scps ON scps.ID_ServiceCharge = sc.ID
INNER JOIN tPayrollPeriod pp ON pp.Year = scps.Year AND pp.ID_Month = scps.ID_Month AND pp.ID_PayrollSchedule = scps.ID_PayrollSchedule AND pp.ID_Company = sc.ID_Company
INNER JOIN tPayroll p ON p.ID_PayrollPeriod = pp.ID AND p.ID_Employee = scd.ID_Employee
INNER JOIN tPayroll_Detail pd ON pd.ID_Payroll = p.ID AND pd.ID_PayrollItem = 46
WHERE  (((p.ID_PayrollPeriod = @ID_PayrollPeriod) AND (p.IsProcessed = 0)) OR (p.ID = @ID_Payroll)) AND (p.IsBasicPay = 1)
AND (((p.TotalHoursWorked > 0 OR p.LeaveWithPay > 0) AND p.IsRequiredToLog = 1) OR p.IsRequiredToLog = 0)
--ServiceCharge----------------------------------------------------------------------|
-- start income and deductions
UPDATE d
--SET d.Amt = d.Amt + ISNULL(tmp.Amt, 0.0),
SET d.Adj = ISNULL(tmp.Amt, 0.0),
d.TaxAmount = ISNULL(tmp.TaxAmount,0)
FROM tPayroll_Detail d INNER JOIN
	tPayroll p ON d.ID_Payroll = p.ID INNER JOIN
	tPayrollPeriod pp ON p.ID_PayrollPeriod = pp.[ID] INNER JOIN
	(
		SELECT iad.Year, iad.ID_Month, iad.ID_PayrollSchedule, p.ID_Employee, iadd.ID_PayrollItem, SUM(iadd.Amount) AS Amt
		, SUM(iadd.TaxAmount) AS TaxAmount
		FROM tIncomeAndDeduction iad INNER JOIN
			tIncomeAndDeduction_Detail iadd ON iad.ID = iadd.ID_IncomeAndDeduction INNER JOIN
			tPayroll p ON iadd.ID_Employee = p.ID_Employee
		WHERE 
			(p.ID_PayrollPeriod = @ID_PayrollPeriod) 
			AND (p.IsProcessed = 0) 
			AND (iadd.IsVoided=0)
		GROUP BY iad.Year, iad.ID_Month, iad.ID_PayrollSchedule, p.ID_Employee, iadd.ID_PayrollItem
	) tmp ON pp.Year = tmp.Year AND pp.ID_Month = tmp.ID_Month AND pp.ID_PayrollSchedule = tmp.ID_PayrollSchedule AND p.ID_Employee = tmp.ID_Employee AND d.ID_PayrollItem = tmp.ID_PayrollItem
WHERE (p.ID_PayrollPeriod = @ID_PayrollPeriod) AND (p.IsProcessed = 0)
AND (((p.TotalHoursWorked > 0 OR p.LeaveWithPay > 0) AND p.IsRequiredToLog = 1) OR p.IsRequiredToLog = 0)
/*

*/
-- end income and deductions


-- start 13th month ------------------------------------------20071129
UPDATE d
SET d.Amt = d.Amt + ISNULL(tmp.Amt, 0.0)
FROM 
		tPayroll_Detail d 
INNER JOIN	tPayroll p ON d.ID_Payroll = p.ID 
INNER JOIN	tPayrollPeriod pp ON p.ID_PayrollPeriod = pp.[ID] 
INNER JOIN	(
		SELECT
			pp.ID ID_PayrollPeriod
			,tmd.ID_Employee
			,25 ID_PayrollItem
			,tmd.Amount AS Amt
		FROM
		tTMonth tm
		INNER JOIN tTMonth_Detail tmd ON tm.ID = tmd.ID_TMonth
		INNER JOIN tPayrollPeriod pp ON tm.ID_Company = pp.ID_Company AND tm.ID_Month = pp.ID_Month AND pp.Year = tm.Year AND pp.ID_PayrollSchedule = tm.ID_PayrollSchedule
		INNER JOIN tPayroll p ON pp.ID = p.ID_PayrollPeriod AND p.ID_Employee = tmd.ID_Employee
		WHERE  (p.ID_PayrollPeriod = @ID_PayrollPeriod) AND (p.IsProcessed = 0)

		) tmp ON 
			    pp.ID = tmp.ID_PayrollPeriod 
			AND p.ID_Employee = tmp.ID_Employee 
			AND d.ID_PayrollItem = tmp.ID_PayrollItem

WHERE (p.ID_PayrollPeriod = @ID_PayrollPeriod) AND (p.IsProcessed = 0)
AND (((p.TotalHoursWorked > 0 OR p.LeaveWithPay > 0) AND p.IsRequiredToLog = 1) OR p.IsRequiredToLog = 0)
-- end 13th month
--setLeaveConversionamount------------------------------------|
UPDATE d
SET d.Amt =tmp.TotalAmt
FROM tPayroll_Detail d 
INNER JOIN tPayroll p ON d.ID_Payroll = p.ID 
INNER JOIN tPayrollPeriod pp ON p.ID_PayrollPeriod = pp.[ID] 
INNER JOIN (
	SELECT
		lc.ID_LeavepayrollItem
		,lc.Year
		,lc.ID_Month
		,lc.ID_PayrollSchedule
		,lcd.ID_Employee
		, SUM(lcd.Amount) TotalAmt
	FROM
	tLeaveConversion_Detail lcd 
	INNER JOIN tLeaveConversion lc ON lc.ID = lcd.ID_LeaveConversion
	WHERE lc.ID_LeaveConversionType = 2 --converted
	GROUP BY  lc.ID_LeavepayrollItem,lc.Year,lc.ID_Month,lc.ID_PayrollSchedule,lcd.ID_Employee
	) tmp ON tmp.ID_Employee = p.ID_Employee
WHERE  pp.ID = @ID_PayrollPeriod AND
 tmp.Year = pp.Year AND tmp.ID_Month = pp.ID_Month AND tmp.ID_PayrollSchedule = pp.ID_PayrollSchedule AND
 d.ID_PayrollItem = CASE WHEN tmp.ID_LeavepayrollItem = 26 THEN 64 WHEN tmp.ID_LeavepayrollItem = 27 THEN 65 END
AND (((p.TotalHoursWorked > 0 OR p.LeaveWithPay > 0) AND p.IsRequiredToLog = 1) OR p.IsRequiredToLog = 0)

--setLeaveConversionamount------------------------------------|

--------------------------------------------------------------------------------------------------------------|
--PayrollOffset
/*
--SELECT pd.ID_PayrollItem ,p.id,(a.Hours * p.HourlyRate)
 UPDATE pd
SET pd.Amt  =(a.Hours * p.HourlyRate)
FROM tPayroll_Detail pd
INNER JOIN tPayroll p ON pd.ID_Payroll = p.ID 
INNER JOIN tPayrollPeriod pp ON p.ID_PayrollPeriod = pp.[ID]
INNER JOIN
(
	SELECT SUM(pod.Hours) Hours,po.ID_Employee,po.ID_payrollPeriod
	FROM tPayrollOffset_Detail pod
	INNER JOIN tPayrollOffset po ON po.ID = pod.ID_PayrollOffset
	WHERE po.ID_PayrollPeriod = @ID_PayrollPeriod
	GROUP BY po.ID_Employee,po.ID_PayrollPeriod
) a ON a.ID_Employee = p.ID_Employee AND a.ID_PayrollPeriod = pp.ID
WHERE p.ID_PayrollPeriod = @ID_PayrollPeriod
AND pd.ID_PayrollItem = 48
AND a.Hours > 0
AND (((p.TotalHoursWorked > 0 OR p.LeaveWithPay > 0) AND p.IsRequiredToLog = 1) OR p.IsRequiredToLog = 0)
*/
------------------------------------------------------------------------------------------------------------|
	UPDATE d
	SET d.Processed =1
	FROM tPayroll_Detail d 
	INNER JOIN tPayroll p ON d.ID_Payroll = p.ID
	INNER JOIN tPayrollPeriod pp ON p.ID_PayrollPeriod = pp.[ID] 
	INNER JOIN tPayrollItem i ON i.ID = d.ID_PayrollItem
	WHERE (((p.ID_PayrollPeriod = @ID_PayrollPeriod) AND (p.IsProcessed = 0)) OR (p.ID = @ID_Payroll)) AND (p.IsBasicPay = 1)
	AND 
	(
		d.Total > 0
		OR 
		((p.TotalHoursWorked = 0 AND p.LeaveWithPay = 0) AND p.IsRequiredToLog = 1)
	)
	AND i.ID_Income = 15

------------------------------------------------------------------------------------------------------------|


--PIECERATE-----------------------------------------------------------------------|
UPDATE pd
SET
pd.Amt = PR.TotalAmout

FROM

tPayroll_detail pd INNER JOIN

(Select 

	TotalAmout
	,PR.ID
		From

			
			(Select
			
			
			PR.ID_PayrollPeriod
			,PR.ID_Employee
			,SUM(PR.TotalAmount) as TotalAmout
			,PR.ID_Parameter
			,p.ID
			
			

			from

					(

					Select 
					prp.ID_PayrollPeriod
					,e.ID as ID_Employee
					,e.ID_Parameter
					,op.PricePerPiece
					,SUM(prpd.qty) TotalQtyDone

					,(op.PricePerPiece * SUM(prpd.qty)) as TotalAmount

					from

					tPieceRateProduction prp 

					INNER JOIN tPieceRateProduction_Detail prpd ON prpd.ID_PieceRateProduction = prp.ID
					INNER JOIN tOperationItem op ON op.ID = prpd.ID_Operationitem
					INNER JOIN tEmployee e ON e.ID = prpd.ID_Employee


					group by ID_PayrollPeriod, e.ID , op.PricePerPiece, e.ID_Parameter
					) as PR



			INNER JOIN tPayroll p ON p.ID_PayrollPeriod = PR.ID_PayrollPeriod AND p.ID_Employee = PR.ID_Employee

			WHERE 

 (((p.ID_PayrollPeriod = @ID_PayrollPeriod) AND (p.IsProcessed = 0)) OR (p.ID = @ID_Payroll)) AND (p.IsBasicPay = 1) AND (PR.ID_Parameter = 3) --PieceRate Parameter ID

			group by PR.ID_PayrollPeriod , PR.ID_Employee ,PR.ID_Parameter , p.ID

			)PR
			
) as PR ON PR.ID = pd.ID_Payroll

where  pd.ID_PayrollItem = 1





--ENDPIECERATE----------------------------------------------------------------------|
