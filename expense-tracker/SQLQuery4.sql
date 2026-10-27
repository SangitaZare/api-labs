SET STATISTICS IO ON;
SET STATISTICS TIME ON;

DECLARE @UserId int = (SELECT TOP 1 UserId FROM Expenses);

-- Query A: what GET /api/expenses does (filtered, newest first, page 1)
SELECT Id, Amount, Description, ExpenseDate, IsIncome, CategoryId
FROM Expenses
WHERE UserId = @UserId
  AND ExpenseDate >= '2025-01-01' AND ExpenseDate < '2026-01-01'
  AND CategoryId = 1
ORDER BY ExpenseDate DESC
OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY;

-- Query B: what the budget status does (monthly total per category)
SELECT CategoryId, SUM(Amount) AS Spent
FROM Expenses
WHERE UserId = @UserId AND IsIncome = 0
  AND ExpenseDate >= '2026-09-01' AND ExpenseDate < '2026-10-01'
GROUP BY CategoryId;