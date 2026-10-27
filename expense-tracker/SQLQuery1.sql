DECLARE @UserId int = (SELECT TOP 1 UserId FROM Expenses);

;WITH Numbers AS (
    SELECT TOP (100000) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
    FROM sys.all_objects a CROSS JOIN sys.all_objects b
)
INSERT INTO Expenses (Amount, Description, ExpenseDate, IsIncome, UserId, CategoryId)
SELECT
    CAST(ABS(CHECKSUM(NEWID())) % 500000 / 100.0 + 10 AS decimal(18,2)),
    CONCAT('SEED expense ', n),
    DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 1004, '2024-01-01'),
    0,
    @UserId,
    CASE n % 3 WHEN 0 THEN 1 WHEN 1 THEN 3 ELSE 4 END
FROM Numbers;

SELECT COUNT(*) AS TotalRows FROM Expenses;