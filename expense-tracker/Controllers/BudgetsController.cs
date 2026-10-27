using System.Globalization;
using ExpenseTracker.Data;
using ExpenseTracker.Dtos;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers;

[Route("api/budgets")]
public class BudgetsController : ApiControllerBase
{
    private readonly AppDbContext _db;
    public BudgetsController(AppDbContext db) => _db = db;

    // GET /api/budgets
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.Budgets.AsNoTracking()
            .Where(b => b.UserId == CurrentUserId)
            .OrderBy(b => b.Category.Name)
            .Select(b => new BudgetResponse
            {
                Id = b.Id,
                CategoryId = b.CategoryId,
                CategoryName = b.Category.Name,
                MonthlyLimit = b.MonthlyLimit
            })
            .ToListAsync();

        return Ok(items);
    }

    // POST /api/budgets  - creates a budget, or updates the limit if the category already has one
    [HttpPost]
    public async Task<IActionResult> Set(BudgetRequest req)
    {
        var category = await _db.Categories.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == req.CategoryId && c.UserId == CurrentUserId);
        if (category is null)
            return BadRequest(new { message = "Category not found." });

        var budget = await _db.Budgets
            .FirstOrDefaultAsync(b => b.CategoryId == req.CategoryId && b.UserId == CurrentUserId);

        var created = budget is null;
        if (budget is null)
        {
            budget = new Budget { UserId = CurrentUserId, CategoryId = req.CategoryId };
            _db.Budgets.Add(budget);
        }

        budget.MonthlyLimit = req.MonthlyLimit;
        await _db.SaveChangesAsync();

        var response = new BudgetResponse
        {
            Id = budget.Id,
            CategoryId = budget.CategoryId,
            CategoryName = category.Name,
            MonthlyLimit = budget.MonthlyLimit
        };

        return created ? StatusCode(201, response) : Ok(response);
    }

    // DELETE /api/budgets/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var budget = await _db.Budgets
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == CurrentUserId);
        if (budget is null) return NotFound();

        _db.Budgets.Remove(budget);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // GET /api/budgets/status?month=2026-10   (month is optional, defaults to this month)
    [HttpGet("status")]
    public async Task<IActionResult> Status(string? month)
    {
        DateTime start;
        if (string.IsNullOrWhiteSpace(month))
        {
            var now = DateTime.Now;
            start = new DateTime(now.Year, now.Month, 1);
        }
        else if (!DateTime.TryParseExact(month, "yyyy-MM", CultureInfo.InvariantCulture,
                     DateTimeStyles.None, out start))
        {
            return BadRequest(new { message = "Month must look like 2026-10." });
        }

        // Spending only (income is ignored), calculated by the stored procedure
        var spendRows = await _db.Database
            .SqlQuery<SpendRow>($"EXEC dbo.usp_GetMonthlySpendByCategory {CurrentUserId}, {start}")
            .ToListAsync();

        var spentByCategory = spendRows.ToDictionary(r => r.CategoryId, r => r.Spent);

        var budgets = await _db.Budgets.AsNoTracking()
            .Where(b => b.UserId == CurrentUserId)
            .Select(b => new { b.CategoryId, CategoryName = b.Category.Name, b.MonthlyLimit })
            .ToListAsync();

        var items = budgets.Select(b =>
        {
            var spent = spentByCategory.GetValueOrDefault(b.CategoryId);
            var percent = b.MonthlyLimit > 0 ? spent / b.MonthlyLimit * 100 : 0;
            return new BudgetStatusItem
            {
                CategoryId = b.CategoryId,
                CategoryName = b.CategoryName,
                MonthlyLimit = b.MonthlyLimit,
                Spent = spent,
                Remaining = b.MonthlyLimit - spent,
                PercentUsed = Math.Round(percent, 1),
                Level = percent >= 100 ? "Exceeded" : percent >= 80 ? "Warning" : "OK"
            };
        })
        .OrderByDescending(i => i.PercentUsed)
        .ToList();

        return Ok(new BudgetStatusResult { Month = start.ToString("yyyy-MM"), Budgets = items });
    }
}