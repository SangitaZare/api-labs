using CompanyApi.Data;
using CompanyApi.DTOs;
using CompanyApi.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DepartmentsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/departments
    // Returns every department with its employee count.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAll()
    {
        var departments = await _context.Departments
            .AsNoTracking()                               // read-only query: no change tracking needed
            .OrderBy(d => d.Name)
            .Select(DepartmentMappings.ToDtoProjection)   // entity -> DTO, translated to SQL
            .ToListAsync();

        return Ok(departments);
    }

    // GET: api/departments/1
    // Returns one department together with its employees (the one-to-many in action).
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DepartmentDetailDto>> GetById(int id)
    {
        var department = await _context.Departments
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(DepartmentMappings.ToDetailProjection)
            .FirstOrDefaultAsync();

        if (department is null)
            return NotFound();

        return Ok(department);
    }

    // POST: api/departments
    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> Create(CreateDepartmentDto dto)
    {
        if (await _context.Departments.AnyAsync(d => d.Name == dto.Name))
            return Conflict(new { message = $"Department '{dto.Name}' already exists." });

        var department = dto.ToEntity();          // DTO -> entity
        _context.Departments.Add(department);
        await _context.SaveChangesAsync();        // INSERT runs here; department.Id is now filled in

        return CreatedAtAction(nameof(GetById), new { id = department.Id }, department.ToDto());
    }

    // PUT: api/departments/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateDepartmentDto dto)
    {
        var department = await _context.Departments.FindAsync(id);   // tracked entity
        if (department is null)
            return NotFound();

        if (await _context.Departments.AnyAsync(d => d.Name == dto.Name && d.Id != id))
            return Conflict(new { message = $"Department '{dto.Name}' already exists." });

        department.ApplyUpdate(dto);
        await _context.SaveChangesAsync();        // change tracker sends an UPDATE for changed columns only

        return NoContent();
    }

    // DELETE: api/departments/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department is null)
            return NotFound();

        // The relationship uses DeleteBehavior.Restrict, so the database would reject this
        // delete anyway. Checking first lets us return a clear message instead of a 500 error.
        if (await _context.Employees.AnyAsync(e => e.DepartmentId == id))
            return Conflict(new { message = "This department still has employees. Move or delete them first." });

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
