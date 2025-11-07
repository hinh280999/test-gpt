using System;

namespace WorkTimeCalculator.Models;

public class WorkEntry
{
    public DateTime Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime CalculatedEndTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public double TotalHours { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public bool IsBreak { get; set; }
    public bool IsActive { get; set; }

    public string TimeRangeDisplay => IsBreak 
        ? $"{StartTime:t} → {ActualEndTime?.ToString("t") ?? CalculatedEndTime.ToString("t")}"
        : $"{StartTime:t} → {(ActualEndTime?.ToString("t") ?? CalculatedEndTime.ToString("t"))}";

    public string DurationDisplay => $"{TotalHours:F1}h";
}
