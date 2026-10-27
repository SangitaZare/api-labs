SELECT name, is_unique
FROM sys.indexes
WHERE object_id = OBJECT_ID('Budgets') AND name LIKE 'IX_Budgets%';