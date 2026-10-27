namespace CompanyApi.Models;

/// <summary>
/// The "MANY" side of the relationship: each employee belongs to exactly one department.
/// </summary>
public class Employee
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    // Internal data: stored in the database but deliberately NOT returned by the API (see DTOs)
    public decimal Salary { get; set; }

    public DateTime HireDate { get; set; }

    // Foreign key property -> becomes the DepartmentId column with an FK constraint
    public int DepartmentId { get; set; }

    // Reference navigation property -> points to the "ONE" side
    public Department Department { get; set; } = null!;
}
