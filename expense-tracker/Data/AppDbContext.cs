using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Budget> Budgets => Set<Budget>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>().HasIndex(u => u.Email).IsUnique();
        b.Entity<User>().Property(u => u.Email).HasMaxLength(200);

        b.Entity<Category>().Property(c => c.Name).HasMaxLength(100);

        b.Entity<Expense>().Property(e => e.Amount).HasPrecision(18, 2);
        b.Entity<Budget>().Property(x => x.MonthlyLimit).HasPrecision(18, 2);
        b.Entity<Budget>().HasIndex(x => new { x.UserId, x.CategoryId }).IsUnique();
        // Avoid "multiple cascade paths" errors in SQL Server
        b.Entity<Expense>().HasOne(e => e.Category).WithMany(c => c.Expenses)
            .HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Restrict);
        b.Entity<Budget>().HasOne(x => x.Category).WithMany()
            .HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
    }
}