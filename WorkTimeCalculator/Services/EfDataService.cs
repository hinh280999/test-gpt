using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkTimeCalculator.Data;
using WorkTimeCalculator.Models;

namespace WorkTimeCalculator.Services;

public class EfDataService : IDataService
{
    private readonly IDbContextFactory<WorkTimeDbContext> _contextFactory;

    public EfDataService(IDbContextFactory<WorkTimeDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<WorkDay> GetOrCreateWorkDayAsync(DateTime date)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var existing = await context.WorkDays
            .Include(d => d.Entries)
            .FirstOrDefaultAsync(d => d.Date == date.Date);

        if (existing is not null)
        {
            return existing;
        }

        var day = new WorkDay { Date = date.Date };
        context.WorkDays.Add(day);
        await context.SaveChangesAsync();
        return day;
    }

    public async Task<IReadOnlyList<WorkEntry>> GetEntriesAsync(DateTime from, DateTime to)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Entries
            .Where(e => e.Date >= from.Date && e.Date <= to.Date)
            .OrderByDescending(e => e.Date)
            .ThenBy(e => e.StartTime)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<WorkEntry>> GetRecentEntriesAsync(int days)
    {
        var end = DateTime.Today;
        var start = end.AddDays(-days);
        return await GetEntriesAsync(start, end);
    }

    public async Task SaveEntryAsync(WorkEntry entry)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var tracked = await context.Entries.FirstOrDefaultAsync(e => e.Id == entry.Id);
        if (tracked is null)
        {
            context.Entries.Add(entry);
        }
        else
        {
            context.Entry(tracked).CurrentValues.SetValues(entry);
        }

        await context.SaveChangesAsync();
    }

    public async Task DeleteEntriesAsync(IEnumerable<Guid> entryIds)
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        var ids = entryIds.ToArray();
        var entries = await context.Entries.Where(e => ids.Contains(e.Id)).ToListAsync();
        context.Entries.RemoveRange(entries);
        await context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<CategoryTag>> GetCategoriesAsync()
    {
        await using var context = await _contextFactory.CreateDbContextAsync();
        return await context.CategoryTags.OrderBy(c => c.Name).ToListAsync();
    }
}
