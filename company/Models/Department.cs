namespace CompanyApi.Models;

/// <summary>
/// The "ONE" side of the relationship: a department has many employees.
/// Code-first: EF Core creates the Departments table from this class.
/// </summary>
public class Department
{
    // Primary key - EF Core treats a property named "Id" as the key by convention
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Location { get; set; }

    // Collection navigation property -> points to the "MANY" side
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
