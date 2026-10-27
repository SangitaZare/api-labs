CREATE NONCLUSTERED INDEX IX_Expenses_User_Category_Date
ON Expenses (UserId, CategoryId, ExpenseDate DESC)
INCLUDE (Amount, Description, IsIncome);

CREATE NONCLUSTERED INDEX IX_Expenses_User_Date
ON Expenses (UserId, ExpenseDate)
INCLUDE (CategoryId, IsIncome, Amount);