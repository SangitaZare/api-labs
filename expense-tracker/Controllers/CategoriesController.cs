using ExpenseTracker.Data;
using ExpenseTracker.Dtos;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers;

[Route("api/categories")]
public class CategoriesController : ApiControllerBase
{
    private readonly AppDbContext _db;
    public CategoriesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.Categories
            .Where(c => c.UserId == CurrentUserId)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryResponse { Id = c.Id, Name = c.Name })
            .ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryRequest req)
    {
        var name = req.Name.Trim();
        if (await _db.Categories.AnyAsync(c => c.UserId == CurrentUserId && c.Name == name))
            return Conflict(new { message = "Category already exists." });

        var cat = new Category { Name = name, UserId = CurrentUserId };
        _db.Categories.Add(cat);
        await _db.SaveChangesAsync();
        return Ok(new CategoryResponse { Id = cat.Id, Name = cat.Name });
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cat = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id && c.UserId == CurrentUserId);
        if (cat == null) return NotFound();

        if (await _db.Expenses.AnyAsync(e => e.CategoryId == id))
            return Conflict(new { message = "Category has expenses and cannot be deleted." });

        _db.Categories.Remove(cat);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}