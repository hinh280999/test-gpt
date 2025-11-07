using System;
using WorkTimeCalculator.Models;

namespace WorkTimeCalculator.ViewModels;

public class TimeBlockViewModel : BaseViewModel
{
    public TimeBlockViewModel(WorkEntry entry)
    {
        Entry = entry;
    }

    public WorkEntry Entry { get; }

    public string DateDisplay => Entry.Date.ToString("ddd, MMM d");
    public string TimeRange => $"{Entry.StartTime:t} - {Entry.EndTime:t}";
    public string Duration => $"{Entry.Duration.TotalHours:0.##} h";
    public string Category => Entry.Category;
    public string Notes => Entry.Notes ?? string.Empty;
    public bool IsWeekend => Entry.IsCalendarException;
}
