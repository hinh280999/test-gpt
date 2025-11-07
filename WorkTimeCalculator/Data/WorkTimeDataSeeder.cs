using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkTimeCalculator.Models;

namespace WorkTimeCalculator.Data;

public class WorkTimeDataSeeder : IDataSeeder
{
    private readonly IDbContextFactory<WorkTimeDbContext> _contextFactory;

    public WorkTimeDataSeeder(IDbContextFactory<WorkTimeDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task SeedAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();

        if (await context.Entries.AnyAsync())
        {
            return;
        }

        var today = DateTime.Today;
        for (var i = 0; i < 14; i++)
        {
            var date = today.AddDays(-i);
            var start = date.AddHours(8).AddMinutes(i % 4 * 15);
            var end = start.AddHours(8).AddMinutes(45);
            context.Entries.Add(new WorkEntry
            {
                Date = date,
                StartTime = start,
                EndTime = end,
                BreakMinutes = 60,
                Category = i % 3 switch
                {
                    0 => "Meeting",
                    1 => "Focus Work",
                    _ => "Admin"
                },
                Notes = "Seed entry",
                IsCalendarException = date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday
            });
        }

        await context.SaveChangesAsync();
    }
}
