using Dapper;
using DapperNHibernateApi.Data;
using DapperNHibernateApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DapperNHibernateApi.Controllers;

/// <summary>
/// Day 15 - Data retrieval with DAPPER (micro-ORM).
/// We write the SQL ourselves; Dapper only turns the result rows into C# objects.
/// </summary>
[ApiController]
[Route("api/dapper")]
public class DapperEmployeesController : ControllerBase
{
    private readonly DapperContext _context;

    public DapperEmployeesController(DapperContext context)
    {
        _context = context;
    }

    // Shared SELECT for the employee endpoints.
    // Column names (or aliases) must match the EmployeeDto property names - that's how Dapper maps.
    private const string EmployeeSelect = @"
        SELECT e.Id,
               e.FirstName,
               e.LastName,
               e.FirstName + ' ' + e.LastName AS FullName,
               e.Email,
               e.HireDate,
               e.DepartmentId,
               d.Name AS DepartmentName
        FROM Employees e
        INNER JOIN Departments d ON d.Id = e.DepartmentId";

    // GET: api/dapper/employees
    // GET: api/dapper/employees?departmentId=1
    [HttpGet("employees")]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees([FromQuery] int? departmentId)
    {
        var sql = EmployeeSelect + @"
        WHERE (@DepartmentId IS NULL OR e.DepartmentId = @DepartmentId)
        ORDER BY e.LastName, e.FirstName";

        // Dapper opens and closes the connection automatically
        using var connection = _context.CreateConnection();

        // @DepartmentId is sent as a PARAMETER (safe from SQL injection)
        var employees = await connection.QueryAsync<EmployeeDto>(sql, new { DepartmentId = departmentId });

        return Ok(employees);
    }

    // GET: api/dapper/employees/1
    [HttpGet("employees/{id:int}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
    {
        var sql = EmployeeSelect + " WHERE e.Id = @Id";

        using var connection = _context.CreateConnection();
        var employee = await connection.QuerySingleOrDefaultAsync<EmployeeDto>(sql, new { Id = id });

        if (employee is null)
            return NotFound();

        return Ok(employee);
    }

    // GET: api/dapper/departments
    // Aggregate query: each department with its employee count (GROUP BY written by hand)
    [HttpGet("departments")]
    public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetDepartments()
    {
        const string sql = @"
            SELECT d.Id,
                   d.Name,
                   d.Location,
                   COUNT(e.Id) AS EmployeeCount
            FROM Departments d
            LEFT JOIN Employees e ON e.DepartmentId = d.Id
            GROUP BY d.Id, d.Name, d.Location
            ORDER BY d.Name";

        using var connection = _context.CreateConnection();
        var departments = await connection.QueryAsync<DepartmentDto>(sql);

        return Ok(departments);
    }
}
