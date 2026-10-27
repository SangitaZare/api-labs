   SELECT name, type_desc
   FROM sys.indexes
   WHERE object_id = OBJECT_ID('Expenses') AND name LIKE 'IX_Expenses_User%';