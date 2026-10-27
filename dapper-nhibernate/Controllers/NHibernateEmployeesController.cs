using DapperNHibernateApi.DTOs;
using DapperNHibernateApi.NHibernateData;
using Microsoft.AspNetCore.Mvc;
using NHibernate.Linq;

namespace DapperNHibernateApi.Controllers;

/// <summary>
/// Day 15 - Simple NHibernate queries for comparison with EF Core and Dapper.
/// Three query styles: LINQ, Get by primary key, and HQL.
/// </summary>
[ApiController]
[Route("api/nhibernate")]
public class NHibernateEmployeesController : ControllerBase
{
    // Written as NHibernate.ISession because ASP.NET Core also has an ISession (for session state)
    private readonly NHibernate.ISession _session;

    public NHibernateEmployeesController(NHibernate.ISession session)
    {
        _session = session;
    }

    // 1) LINQ QUERY - looks almost the same as EF Core
    // GET: api/nhibernate/employees
    // GET: api/nhibernate/employees?departmentId=1
    [HttpGet("employees")]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetEmployees([FromQuery] int? departmentId)
    {
        var query = _session.Query<NhEmployee>();

        if (departmentId.HasValue)
            query = query.Where(e => e.Department.Id == departmentId.Value);

        var employees = await query
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                FullName = e.FirstName + " " + e.LastName,
                Email = e.Email,
                HireDate = e.HireDate,
                DepartmentId = e.Department.Id,
                DepartmentName = e.Department.Name   // NHibernate adds the JOIN
            })
            .ToListAsync();

        return Ok(employees);
    }

    // 2) GET BY PRIMARY KEY - shows LAZY LOADING
    // GET: api/nhibernate/employees/1
    [HttpGet("employees/{id:int}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployee(int id)
    {
        // SQL query #1: loads only the employee row (Department is a lazy "proxy" for now)
        var employee = await _session.GetAsync<NhEmployee>(id);

        if (employee is null)
            return NotFound();

        // Reading employee.Department.Name inside ToDto triggers SQL query #2 (lazy load).
        // Watch the console: you will see TWO separate SELECT statements.
        return Ok(ToDto(employee));
    }

    // 3) HQL QUERY - NHibernate's own query language (uses CLASS names, not table names)
    // GET: api/nhibernate/employees/by-department/Engineering
    [HttpGet("employees/by-department/{departmentName}")]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetByDepartmentName(string departmentName)
    {
        var employees = await _session
            .CreateQuery(@"from NhEmployee e
                           join fetch e.Department d
                           where d.Name = :name
                           order by e.LastName")
            .SetParameter("name", departmentName)
            .ListAsync<NhEmployee>();

        // "join fetch" loaded the departments in the same query, so no extra lazy-load queries here
        return Ok(employees.Select(ToDto).ToList());
    }

    private static EmployeeDto ToDto(NhEmployee e) => new()
    {
        Id = e.Id,
        FirstName = e.FirstName,
        LastName = e.LastName,
        FullName = $"{e.FirstName} {e.LastName}",
        Email = e.Email,
        HireDate = e.HireDate,
        DepartmentId = e.Department.Id,
        DepartmentName = e.Department.Name
    };
}
