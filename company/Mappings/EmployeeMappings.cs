using System.Linq.Expressions;
using CompanyApi.DTOs;
using CompanyApi.Models;

namespace CompanyApi.Mappings;

public static class EmployeeMappings
{
    // 1) Projection used inside LINQ queries.
    //    Reading e.Department.Name makes EF Core generate an INNER JOIN to Departments
    //    automatically - no .Include() needed.
    public static readonly Expression<Func<Employee, EmployeeDto>> ToDtoProjection =
        e => new EmployeeDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            FullName = e.FirstName + " " + e.LastName,
            Email = e.Email,
            HireDate = e.HireDate,
            DepartmentId = e.DepartmentId,
            DepartmentName = e.Department.Name
        };

    // 2) Mapping for an entity already in memory.
    //    The Department navigation must be loaded first, otherwise DepartmentName is empty.
    public static EmployeeDto ToDto(this Employee e) => new()
    {
        Id = e.Id,
        FirstName = e.FirstName,
        LastName = e.LastName,
        FullName = $"{e.FirstName} {e.LastName}",
        Email = e.Email,
        HireDate = e.HireDate,
        DepartmentId = e.DepartmentId,
        DepartmentName = e.Department?.Name ?? string.Empty
    };

    // 3) DTO -> Entity
    public static Employee ToEntity(this CreateEmployeeDto dto) => new()
    {
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        Email = dto.Email,
        Salary = dto.Salary,
        HireDate = dto.HireDate!.Value, // [Required] guarantees a value
        DepartmentId = dto.DepartmentId
    };

    public static void ApplyUpdate(this Employee e, UpdateEmployeeDto dto)
    {
        e.FirstName = dto.FirstName;
        e.LastName = dto.LastName;
        e.Email = dto.Email;
        e.Salary = dto.Salary;
        e.HireDate = dto.HireDate!.Value;
        e.DepartmentId = dto.DepartmentId; // changing the FK moves the employee to another department
    }
}
