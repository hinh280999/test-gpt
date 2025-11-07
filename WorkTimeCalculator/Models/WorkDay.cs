using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WorkTimeCalculator.Models;

/// <summary>
/// Aggregates work entries for a single day and exposes convenience metrics.
/// </summary>
public class WorkDay
{
    [Key]
    public DateTime Date { get; set; }

    public ICollection<WorkEntry> Entries { get; set; } = new List<WorkEntry>();

    [Range(0, 1440)]
    public int PlannedMinutes { get; set; } = 8 * 60;

    public double TotalHours => Math.Round(Entries.Sum(e => e.Duration.TotalHours), 2);

    public double TotalBreakHours => Math.Round(Entries.Sum(e => e.BreakMinutes) / 60.0, 2);

    public bool IsWeekend => Date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

    public bool IsCompleted => TotalHours >= PlannedMinutes / 60.0;
}
