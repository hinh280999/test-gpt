using System;
using WorkTimeCalculator.Models;

namespace WorkTimeCalculator.Services;

public class WorkTimeCalculatorService
{
    private static readonly TimeSpan TargetWorkDuration = TimeSpan.FromHours(8);

    public DateTime CalculateEnd(DateTime date, TimeSpan startTime, LunchSettings lunchSettings)
    {
        var start = date.Add(startTime);
        var lunchStart = date.Add(lunchSettings.Start);
        var lunchEnd = date.Add(lunchSettings.End);

        // If start time is after lunch, skip lunch break
        if (start >= lunchEnd)
        {
            return start.Add(TargetWorkDuration);
        }

        var adjusted = start.Add(TargetWorkDuration);
        if (start <= lunchStart)
        {
            adjusted = adjusted.Add(lunchSettings.Duration);
        }
        else if (start < lunchEnd)
        {
            var remainingLunch = lunchEnd - start;
            adjusted = adjusted.Add(remainingLunch);
        }

        return adjusted;
    }

    public bool TryCalculateTotalHours(TimeSpan startTime, TimeSpan? actualEndTime, LunchSettings lunchSettings, out double totalHours)
    {
        totalHours = 0;
        if (!actualEndTime.HasValue)
        {
            return false;
        }

        var span = actualEndTime.Value - startTime;
        if (span <= TimeSpan.Zero)
        {
            return false;
        }

        var lunchOverlap = CalculateOverlap(startTime, actualEndTime.Value, lunchSettings.Start, lunchSettings.End);
        var worked = span - lunchOverlap;
        totalHours = Math.Round(worked.TotalHours, 2);
        return true;
    }

    private static TimeSpan CalculateOverlap(TimeSpan rangeStart, TimeSpan rangeEnd, TimeSpan lunchStart, TimeSpan lunchEnd)
    {
        var start = TimeSpan.FromTicks(Math.Max(rangeStart.Ticks, lunchStart.Ticks));
        var end = TimeSpan.FromTicks(Math.Min(rangeEnd.Ticks, lunchEnd.Ticks));
        return end > start ? end - start : TimeSpan.Zero;
    }
}
