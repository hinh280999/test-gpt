using System;

namespace WorkTimeCalculator.Models;

public class WorkEntry
{
    public DateTime Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime CalculatedEndTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public double TotalHours { get; set; }
}
