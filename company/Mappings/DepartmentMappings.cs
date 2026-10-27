using System.Linq.Expressions;
using CompanyApi.DTOs;
using CompanyApi.Models;

namespace CompanyApi.Mappings;

public static class DepartmentMappings
{
    // =====================================================================
    // 1) ENTITY -> DTO INSIDE A QUERY (projection)
    //    These are Expressions, so EF Core translates them into SQL.
    //    Only the needed columns are read, and EmployeeCount becomes a
    //    COUNT subquery instead of loading every employee into memory.
    // =====================================================================

    public static readonly Expression<Func<Department, DepartmentDto>> ToDtoProjection =
        d => new DepartmentDto
        {
            Id = d.Id,
            Name = d.Name,
            Location = d.Location,
            EmployeeCount = d.Employees.Count
        };

    public static readonly Expression<Func<Department, DepartmentDetailDto>> ToDetailProjection =
        d => new DepartmentDetailDto
        {
            Id = d.Id,
            Name = d.Name,
            Location = d.Location,
            Employees = d.Employees
                .OrderBy(e => e.LastName)
                .Select(e => new EmployeeSummaryDto
                {
                    Id = e.Id,
                    FullName = e.FirstName + " " + e.LastName,
                    Email = e.Email
                })
                .ToList()
        };

    // =====================================================================
    // 2) ENTITY -> DTO FOR AN OBJECT ALREADY IN MEMORY
    //    (e.g. right after inserting it)
    // =====================================================================

    public static DepartmentDto ToDto(this Department d) => new()
    {
        Id = d.Id,
        Name = d.Name,
        Location = d.Location,
        EmployeeCount = d.Employees.Count
    };

    // =====================================================================
    // 3) DTO -> ENTITY (incoming requests)
    // =====================================================================

    public static Department ToEntity(this CreateDepartmentDto dto) => new()
    {
        Name = dto.Name,
        Location = dto.Location
    };

    public static void ApplyUpdate(this Department d, UpdateDepartmentDto dto)
    {
        d.Name = dto.Name;
        d.Location = dto.Location;
    }
}
