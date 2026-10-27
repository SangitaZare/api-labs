namespace DapperNHibernateApi.NHibernateData;

// ---------------------------------------------------------------------------
// NHibernate entity classes.
//
// Rule: NHibernate needs every public property to be VIRTUAL
// (so it can create lazy-loading "proxy" objects).
// These classes map to the existing Departments and Employees tables
// in CompanyDb (created by EF Core migrations on Day 14).
// ---------------------------------------------------------------------------

/// <summary>"One" side: a department has many employees.</summary>
public class NhDepartment
{
    public virtual int Id { get; set; }
    public virtual string Name { get; set; } = string.Empty;
    public virtual string? Location { get; set; }

    public virtual IList<NhEmployee> Employees { get; set; } = new List<NhEmployee>();
}

/// <summary>"Many" side: each employee belongs to one department.</summary>
public class NhEmployee
{
    public virtual int Id { get; set; }
    public virtual string FirstName { get; set; } = string.Empty;
    public virtual string LastName { get; set; } = string.Empty;
    public virtual string Email { get; set; } = string.Empty;
    public virtual decimal Salary { get; set; }
    public virtual DateTime HireDate { get; set; }

    // NHibernate uses the object reference; the DepartmentId column is set in the mapping
    public virtual NhDepartment Department { get; set; } = null!;
}
