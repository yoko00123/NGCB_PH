ALTER PROCEDURE dbo.pComputeHours @ID_DailyscheduleView INT

as



--DECLARE @ID_DailyscheduleView INT =	336



/**

	LJ 20141106, MDC

*/



--SELECT * FROM dbo.tEmployeeDailyScheduleView WHERE ID = 345



SET TRANSACTION ISOLATION LEVEL READ COMMITTED

SET NOCOUNT ON

SET XACT_ABORT ON



DECLARE @ID_EmployeeDailySchedule INT 



DECLARE @tbl AS TABLE (ID INT)

DECLARE @totalrow AS INT, @currow INT  = 0

DECLARE @percent INT = 0

DECLARE	@StartTime DATETIME = GETDATE(), @FinishTime VARCHAR(43) = 'unknown at this time'



INSERT INTO @tbl

        ( ID )

	--2015-03-31 ALVIN

	SELECT eds.ID FROM dbo.tEmployeeDailySchedule eds WITH(NOLOCK)

	INNER JOIN dbo.tEmployee e WITH(NOLOCK) ON e.ID = eds.ID_Employee

	CROSS APPLY(SELECT * FROM dbo.tEmployeeDailyScheduleView edsv WITH(NOLOCK) 

	--INNER JOIN dbo.fSessionEmployeeNew(@ID_Session) sen ON sen.ID_Employee = e.ID --ALVIN 20150327

	--LEFT OUTER JOIN dbo.tEmployeeProject ep WITH(NOLOCK) ON ep.ID = e.ID_EmployeeProject	

	WHERE (eds.Date BETWEEN edsv.StartDate AND edsv.EndDate)

	AND

	(e.ID_Company = edsv.ID_Company)

	AND (e.ID_PayrollFrequency = edsv.ID_PayrollFrequency)

	AND (e.ID_Branch = edsv.ID_Branch OR edsv.ID_Branch IS NULL)

	AND (e.ID_Department = edsv.ID_Department OR edsv.ID_Department IS NULL)

	AND (e.ID_Designation = edsv.ID_Designation OR edsv.ID_Designation IS NULL)

	AND (e.ID_CostCenter = edsv.ID_CostCenter OR edsv.ID_CostCenter IS NULL)

	AND (e.ID_EmployeeStatus = edsv.ID_EmployeeStatus OR edsv.ID_EmployeeStatus IS NULL) 

	--AND (ep.ID_OperationsCenter = edsv.ID_OperationsCenter OR Edsv.ID_OperationsCenter IS NULL)

	AND (eds.ID_employee = edsv.ID_Employee OR edsv.ID_Employee IS NULL)  

	--AND	(e.ID_EmployeeProject = edsv.ID_EmployeeProject OR edsv.ID_EmployeeProject IS NULL)

	--AND (e.ID_JobClass = edsv.ID_JobClass OR edsv.ID_JobClass IS NULL)

	--AND (eds.ID_JobClass = edsv.ID_JobClass OR edsv.ID_JobClass IS NULL) --20151026 KIM

	AND edsv.ID = @ID_DailyscheduleView)s

		WHERE eds.IsForComputation = 1

--OLD 

--SELECT 

--	eds.ID 

--	FROM  tEmployeeDailySchedule eds WITH(NOLOCK)

--	INNER JOIN tEmployee e WITH(NOLOCK) ON e.ID = eds.ID_Employee 

--	LEFT OUTER JOIN dbo.tEmployeeProject ep WITH(NOLOCK) ON ep.ID = e.ID_EmployeeProject

--	INNER JOIN dbo.tEmployeeDailyScheduleView edsv WITH(NOLOCK) ON eds.Date BETWEEN edsv.StartDate AND edsv.EndDate

--	INNER JOIN dbo.fSessionEmployeeNew(@ID_Session) sen ON sen.ID_Employee = e.ID --ALVIN 20150327

--	WHERE	 

--	(e.ID_Company = edsv.ID_Company)

--	AND (e.ID_PayrollFrequency = edsv.ID_PayrollFrequency)

--	AND (e.ID_Branch = edsv.ID_Branch OR edsv.ID_Branch IS NULL)

--	AND (e.ID_Department = edsv.ID_Department OR edsv.ID_Department IS NULL)

--	AND (e.ID_Designation = edsv.ID_Designation OR edsv.ID_Designation IS NULL)

--	AND (e.ID_CostCenter = edsv.ID_CostCenter OR edsv.ID_CostCenter IS NULL)

--	AND (e.ID_EmployeeStatus = edsv.ID_EmployeeStatus OR edsv.ID_EmployeeStatus IS NULL)

--	AND (ep.ID_OperationsCenter = edsv.ID_OperationsCenter OR Edsv.ID_OperationsCenter IS NULL)

--	--AND (per.ID_Gender = edsv.ID_Gender OR edsv.ID_Gender IS NULL)

--	AND (eds.ID_employee = edsv.ID_Employee OR edsv.ID_Employee IS NULL)

--	AND (edsv.ID = @ID_DailyscheduleView)

--	AND (eds.IsForComputation = 1)

--	AND	(e.ID_EmployeeProject = edsv.ID_EmployeeProject OR edsv.ID_EmployeeProject IS NULL)

--	--AND dbo.fEmployeeRights(@ID_Session, eds.ID_Employee) = 1 old 20150327

--	OPTION (ROBUST PLAN, FAST 30, FORCE ORDER, KEEPFIXED PLAN, MAXRECURSION 0)



DECLARE c CURSOR FAST_FORWARD FOR SELECT * FROM  @tbl

OPEN c



FETCH NEXT FROM c INTO @ID_EmployeeDailySchedule





SELECT @totalrow = COUNT(1) FROM @tbl



WHILE @@FETCH_STATUS = 0

BEGIN

	BEGIN TRY	

	BEGIN TRAN 

		SET @currow = @currow + 1

		SET @percent =  @currow * 100  / @totalrow

		SET NOCOUNT ON

		--RAISERROR('Now working with Record %d of %d, %d Percent. Estimated completion time is %s', 10, 1, @currow, @totalrow, @percent, @FinishTime) WITH NOWAIT

		RAISERROR('Now working with Record %d of %d, %d%%.', 10, 1, @currow, @totalrow, @percent, @FinishTime) WITH NOWAIT



		EXEC dbo.pEmployeeDailySchedule_ComputeHours @ID_EmployeeDailySchedule

		UPDATE dbo.tEmployeeDailySchedule SET IsForComputation = 0 WHERE ID = @ID_EmployeeDailySchedule -- testing :)

DECLARE @REGN INT, @IsActualAbsent INT, @LeaveWithPayn int, @ID_DayTypen INT
SELECT @REGN = REG, @IsActualAbsent = IsActualAbsent, @LeaveWithPayn = LeaveWithPay FROM dbo.tEmployeeDailySchedule WHERE ID = @ID_EmployeeDailySchedule AND LeaveWithPay = 0 AND ID_DayType = 1
IF @REGN = 0 AND @IsActualAbsent = 0 AND @LeaveWithPayn = 0 and @ID_DayTypen not in (7)
BEGIN

EXEC pEmployeeDailySchedule_ComputeHours @ID_EmployeeDailySchedule
END



		set @FinishTime = CONVERT(VARCHAR(40), DATEADD(SECOND, CAST(1 * DATEDIFF(SECOND, @StartTime, GETDATE()) / @currow AS INT), @StartTime), 120)



		COMMIT TRAN

	END TRY

	BEGIN CATCH  

		ROLLBACK TRAN

		--PRINT 'error catch'

		INSERT INTO dbo.tErrorCarrier

		        ( ErrorCode ,

		          ErrorMsg ,

		          line ,

		          timestamp ,

		          [Procedure],

				  ID_Module

		        )

		VALUES  (ERROR_NUMBER() , -- ErrorCode - varchar(50)

		          ERROR_MESSAGE() , -- ErrorMsg - text

		          ERROR_LINE() , -- line - int

		          GETDATE() , -- timestamp - datetime

		          'pEmployeeDailySchedule_ComputeHours',  -- Procedure - varchar(50),

				  @ID_EmployeeDailySchedule

		        )

	END catch  

	FETCH NEXT FROM c INTO @ID_EmployeeDailySchedule

END



CLOSE c

DEALLOCATE c