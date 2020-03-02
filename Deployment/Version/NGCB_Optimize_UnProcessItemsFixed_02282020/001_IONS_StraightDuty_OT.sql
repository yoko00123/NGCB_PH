ALTER	 TRIGGER dbo.rtOvertime ON dbo.tOvertime
FOR INSERT,UPDATE
AS
UPDATE tOvertime SET 
StartMinute = i.TempStartMinute+(DATEDIFF(Day,i.Workdate,(case when (i.FollowingDay = 1) then (dateadd(day,1,i.WorkDate)) else i.WorkDate end))*1440) 
,EndMinute  = CASE WHEN i.TempEndMinute < i.TempStartMinute THEN i.TempEndMinute + 1440 ELSE i.TempEndMinute END+(DATEDIFF(Day,i.Workdate,(case when (i.FollowingDay = 1) then (dateadd(day,1,i.WorkDate)) else i.WorkDate end))*1440) 
,DateTimeModified = GETDATE() 
FROM INSERTED i INNER JOIN tOvertime ON tOvertime.ID = i.ID


UPDATE dbo.tOvertime set
ComputedHours = (CASE WHEN (o.EndMinute - o.StartMinute) = 0 AND i.FollowingDay = 1 THEN 1440 ELSE o.EndMinute - o.StartMinute END )/60.00
FROM INSERTED i INNER JOIN tOvertime o ON o.ID = i.ID
GO

