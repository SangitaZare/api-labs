using System.ComponentModel.DataAnnotations;

namespace CompanyApi.DTOs;

// ================= RESPONSE DTOs (what the API sends back) =================

/// <summary>Used in lists: department info plus how many employees it has.</summary>
public class DepartmentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public int EmployeeCount { get; set; }
}

/// <summary>Used for one department: includes its employees (the "many" side).</summary>
public class DepartmentDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public List<EmployeeSummaryDto> Employees { get; set; } = new();
}

// ================= REQUEST DTOs (what the client sends in) =================

public class CreateDepartmentDto
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Location { get; set; }
}

/// <summary>Same fields as create for now; a separate type lets them change independently later.</summary>
public class UpdateDepartmentDto : CreateDepartmentDto
{
}
