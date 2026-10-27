using CompanyApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyApi.Data;

/// <summary>
/// The DbContext is EF Core's session with the database:
/// it tracks entity changes, translates LINQ into SQL, and saves changes.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Each DbSet becomes a table
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Employee> Employees => Set<Employee>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---------- Departments table ----------
        modelBuilder.Entity<Department>(entity =>
        {
            entity.Property(d => d.Name)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(d => d.Location)
                  .HasMaxLength(100);

            entity.HasIndex(d => d.Name)
                  .IsUnique();
        });

        // ---------- Employees table ----------
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Salary).HasPrecision(18, 2);

            entity.HasIndex(e => e.Email)
                  .IsUnique();

            // ---------- ONE-TO-MANY RELATIONSHIP ----------
            // An Employee has ONE Department; a Department has MANY Employees;
            // they are linked by the DepartmentId foreign key.
            // (EF Core would discover this by convention, but writing it out
            //  makes the relationship explicit and lets us choose the delete behaviour.)
            entity.HasOne(e => e.Department)
                  .WithMany(d => d.Employees)
                  .HasForeignKey(e => e.DepartmentId)
                  .OnDelete(DeleteBehavior.Restrict); // can't delete a department that still has employees
        });

        // ---------- Seed data (inserted by the migration) ----------
        // HasData needs fixed values (no DateTime.Now), otherwise every migration would change it.
        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "Engineering", Location = "Pune" },
            new Department { Id = 2, Name = "Human Resources", Location = "Mumbai" },
            new Department { Id = 3, Name = "Finance", Location = "Bangalore" }
        );

        modelBuilder.Entity<Employee>().HasData(
            new Employee { Id = 1, FirstName = "Aarav", LastName = "Patil", Email = "aarav.patil@example.com", Salary = 650000m, HireDate = new DateTime(2023, 6, 1), DepartmentId = 1 },
            new Employee { Id = 2, FirstName = "Sneha", LastName = "Kulkarni", Email = "sneha.kulkarni@example.com", Salary = 820000m, HireDate = new DateTime(2022, 1, 15), DepartmentId = 1 },
            new Employee { Id = 3, FirstName = "Rohan", LastName = "Deshmukh", Email = "rohan.deshmukh@example.com", Salary = 450000m, HireDate = new DateTime(2024, 8, 1), DepartmentId = 1 },
            new Employee { Id = 4, FirstName = "Priya", LastName = "Joshi", Email = "priya.joshi@example.com", Salary = 550000m, HireDate = new DateTime(2021, 3, 10), DepartmentId = 2 },
            new Employee { Id = 5, FirstName = "Vikram", LastName = "Shinde", Email = "vikram.shinde@example.com", Salary = 900000m, HireDate = new DateTime(2020, 11, 20), DepartmentId = 3 }
        );
    }
}
