using System.ComponentModel.DataAnnotations;

namespace CompanyApi.DTOs;

// ================= RESPONSE DTOs =================

/// <summary>
/// Full employee view. Notice what is DIFFERENT from the Employee entity:
///  - no Salary (sensitive data stays inside the API)
///  - no Department object (avoids circular JSON); just DepartmentName instead
///  - an extra computed FullName field
/// </summary>
public class EmployeeDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
}

/// <summary>Small shape used when listing employees inside a department.</summary>
public class EmployeeSummaryDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

// ================= REQUEST DTOs =================

public class CreateEmployeeDto
{
    [Required, StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    // Salary can be WRITTEN by the client but is never READ back (not in EmployeeDto)
    [Range(0, 10_000_000)]
    public decimal Salary { get; set; }

    // Nullable + [Required] so a missing date gives a 400 error instead of silently becoming 01-01-0001
    [Required]
    public DateTime? HireDate { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "DepartmentId must be a valid department id.")]
    public int DepartmentId { get; set; }
}

public class UpdateEmployeeDto : CreateEmployeeDto
{
}
