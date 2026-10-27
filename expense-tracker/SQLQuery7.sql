CREATE OR ALTER PROCEDURE dbo.usp_GetMonthlySpendByCategory
    @UserId int,
    @MonthStart date
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CategoryId, SUM(Amount) AS Spent
    FROM Expenses
    WHERE UserId = @UserId
      AND IsIncome = 0
      AND ExpenseDate >= @MonthStart
      AND ExpenseDate <  DATEADD(MONTH, 1, @MonthStart)
    GROUP BY CategoryId;
END