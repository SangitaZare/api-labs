SET STATISTICS IO ON;
SET STATISTICS TIME ON;

DECLARE @UserId int = (SELECT TOP 1 UserId FROM Expenses);
EXEC dbo.usp_GetMonthlySpendByCategory @UserId, '2026-09-01';
EXEC dbo.usp_GetMonthlySpendByCategory @UserId, '2026-10-01';