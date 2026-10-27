SELECT OBJECT_NAME(object_id) AS ProcName, execution_count
FROM sys.dm_exec_procedure_stats
WHERE object_id = OBJECT_ID('dbo.usp_GetMonthlySpendByCategory');