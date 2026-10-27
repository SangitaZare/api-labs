using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Dtos;

public class BudgetRequest
{
    [Required]
    public int CategoryId { get; set; }

    [Range(0.01, 999999999)]
    public decimal MonthlyLimit { get; set; }
}

public class BudgetResponse
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal MonthlyLimit { get; set; }
}

public class BudgetStatusItem
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal MonthlyLimit { get; set; }
    public decimal Spent { get; set; }
    public decimal Remaining { get; set; }
    public decimal PercentUsed { get; set; }
    public string Level { get; set; } = "OK";   // OK, Warning, Exceeded
}

public class BudgetStatusResult
{
    public string Month { get; set; } = string.Empty;
    public List<BudgetStatusItem> Budgets { get; set; } = new();
}
public class SpendRow
{
    public int CategoryId { get; set; }
    public decimal Spent { get; set; }
}