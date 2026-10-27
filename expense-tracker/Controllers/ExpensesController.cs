using ExpenseTracker.Data;
using ExpenseTracker.Dtos;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers;

[Route("api/expenses")]
public class ExpensesController : ApiControllerBase
{
    private readonly AppDbContext _db;
    public ExpensesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        DateTime? from, DateTime? to, int? categoryId,
        decimal? minAmount, decimal? maxAmount,
        int page = 1, int pageSize = 20)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _db.Expenses.AsNoTracking().Where(e => e.UserId == CurrentUserId);

        if (from.HasValue) query = query.Where(e => e.ExpenseDate >= from.Value);
        if (to.HasValue) query = query.Where(e => e.ExpenseDate <= to.Value);
        if (categoryId.HasValue) query = query.Where(e => e.CategoryId == categoryId.Value);
        if (minAmount.HasValue) query = query.Where(e => e.Amount >= minAmount.Value);
        if (maxAmount.HasValue) query = query.Where(e => e.Amount <= maxAmount.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(e => e.ExpenseDate).ThenByDescending(e => e.Id)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(e => new ExpenseResponse
            {
                Id = e.Id,
                Amount = e.Amount,
                Description = e.Description,
                ExpenseDate = e.ExpenseDate,
                IsIncome = e.IsIncome,
                CategoryId = e.CategoryId,
                CategoryName = e.Category.Name
            })
            .ToListAsync();

        return Ok(new PagedResult<ExpenseResponse>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var e = await _db.Expenses.AsNoTracking().Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == CurrentUserId);
        return e == null ? NotFound() : Ok(ToResponse(e));
    }

    [HttpPost]
    public async Task<IActionResult> Create(ExpenseRequest req)
    {
        if (!await _db.Categories.AnyAsync(c => c.Id == req.CategoryId && c.UserId == CurrentUserId))
            return BadRequest(new { message = "Category not found." });

        var e = new Expense
        {
            Amount = req.Amount,
            Description = req.Description,
            ExpenseDate = req.ExpenseDate,
            IsIncome = req.IsIncome,
            CategoryId = req.CategoryId,
            UserId = CurrentUserId
        };
        _db.Expenses.Add(e);
        await _db.SaveChangesAsync();
        await _db.Entry(e).Reference(x => x.Category).LoadAsync();
        return CreatedAtAction(nameof(GetById), new { id = e.Id }, ToResponse(e));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ExpenseRequest req)
    {
        var e = await _db.Expenses.FirstOrDefaultAsync(x => x.Id == id && x.UserId == CurrentUserId);
        if (e == null) return NotFound();

        if (!await _db.Categories.AnyAsync(c => c.Id == req.CategoryId && c.UserId == CurrentUserId))
            return BadRequest(new { message = "Category not found." });

        e.Amount = req.Amount; e.Description = req.Description;
        e.ExpenseDate = req.ExpenseDate; e.IsIncome = req.IsIncome;
        e.CategoryId = req.CategoryId;
        await _db.SaveChangesAsync();
        await _db.Entry(e).Reference(x => x.Category).LoadAsync();
        return Ok(ToResponse(e));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var e = await _db.Expenses.FirstOrDefaultAsync(x => x.Id == id && x.UserId == CurrentUserId);
        if (e == null) return NotFound();
        _db.Expenses.Remove(e);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static ExpenseResponse ToResponse(Expense e) => new()
    {
        Id = e.Id,
        Amount = e.Amount,
        Description = e.Description,
        ExpenseDate = e.ExpenseDate,
        IsIncome = e.IsIncome,
        CategoryId = e.CategoryId,
        CategoryName = e.Category.Name
    };
}