using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkTimeCalculator.Models;

namespace WorkTimeCalculator.Services;

public interface IDataService
{
    Task<WorkDay> GetOrCreateWorkDayAsync(DateTime date);
    Task<IReadOnlyList<WorkEntry>> GetEntriesAsync(DateTime from, DateTime to);
    Task<IReadOnlyList<WorkEntry>> GetRecentEntriesAsync(int days);
    Task SaveEntryAsync(WorkEntry entry);
    Task DeleteEntriesAsync(IEnumerable<Guid> entryIds);
    Task<IReadOnlyList<CategoryTag>> GetCategoriesAsync();
}
