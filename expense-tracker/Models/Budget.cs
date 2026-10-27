namespace ExpenseTracker.Models;

public class Budget
{
    public int Id { get; set; }
    public decimal MonthlyLimit { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}