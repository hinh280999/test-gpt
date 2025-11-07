using Microsoft.EntityFrameworkCore;
using WorkTimeCalculator.Models;

namespace WorkTimeCalculator.Data;

public class WorkTimeDbContext : DbContext
{
    public WorkTimeDbContext(DbContextOptions<WorkTimeDbContext> options)
        : base(options)
    {
    }

    public DbSet<WorkEntry> Entries => Set<WorkEntry>();
    public DbSet<WorkDay> WorkDays => Set<WorkDay>();
    public DbSet<CategoryTag> CategoryTags => Set<CategoryTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CategoryTag>().HasData(
            new CategoryTag { Name = "Meeting", Color = "#FF8E24AA", Icon = "AccountGroup" },
            new CategoryTag { Name = "Focus Work", Color = "#FF4CAF50", Icon = "Target" },
            new CategoryTag { Name = "Admin", Color = "#FFFFB300", Icon = "FileDocument" },
            new CategoryTag { Name = "Break", Color = "#FF26C6DA", Icon = "Coffee" }
        );
    }
}
