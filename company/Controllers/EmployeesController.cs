using CompanyApi.Data;
using CompanyApi.DTOs;
using CompanyApi.Mappings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CompanyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _context;

    public EmployeesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/employees                  -> all employees
    // GET: api/employees?departmentId=1   -> only employees of department 1
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll([FromQuery] int? departmentId)
    {
        var query = _context.Employees.AsNoTracking();

        if (departmentId.HasValue)
            query = query.Where(e => e.DepartmentId == departmentId.Value);

        var employees = await query
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .Select(EmployeeMappings.ToDtoProjection)   // includes JOIN for DepartmentName
            .ToListAsync();

        return Ok(employees);
    }

    // GET: api/employees/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeDto>> GetById(int id)
    {
        var employee = await _context.Employees
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(EmployeeMappings.ToDtoProjection)
            .FirstOrDefaultAsync();

        if (employee is null)
            return NotFound();

        return Ok(employee);
    }

    // POST: api/employees
    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create(CreateEmployeeDto dto)
    {
        if (!await _context.Departments.AnyAsync(d => d.Id == dto.DepartmentId))
            return BadRequest(new { message = $"Department {dto.DepartmentId} does not exist." });

        if (await _context.Employees.AnyAsync(e => e.Email == dto.Email))
            return Conflict(new { message = $"An employee with email '{dto.Email}' already exists." });

        var employee = dto.ToEntity();
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        // EXPLICIT LOADING: fetch the related Department so ToDto() can fill DepartmentName
        await _context.Entry(employee).Reference(e => e.Department).LoadAsync();

        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee.ToDto());
    }

    // PUT: api/employees/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateEmployeeDto dto)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee is null)
            return NotFound();

        if (!await _context.Departments.AnyAsync(d => d.Id == dto.DepartmentId))
            return BadRequest(new { message = $"Department {dto.DepartmentId} does not exist." });

        if (await _context.Employees.AnyAsync(e => e.Email == dto.Email && e.Id != id))
            return Conflict(new { message = $"An employee with email '{dto.Email}' already exists." });

        employee.ApplyUpdate(dto);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/employees/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee is null)
            return NotFound();

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
