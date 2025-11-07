using System;
using WorkTimeCalculator.Models;

namespace WorkTimeCalculator.Services;

public interface ICalculationService
{
    TimeCalculation Calculate(DateTime startTime, TimeSpan workDuration, TimeSpan breakDuration, DateTime now);
    double CalculateProgress(DateTime startTime, DateTime endTime, DateTime now);
    TimeSpan CalculateCountdown(DateTime endTime, DateTime now);
}

public class CalculationService : ICalculationService
{
    public TimeCalculation Calculate(DateTime startTime, TimeSpan workDuration, TimeSpan breakDuration, DateTime now)
    {
        var endTime = startTime.Add(workDuration).Add(breakDuration);
        var progress = CalculateProgress(startTime, endTime, now);
        var countdown = CalculateCountdown(endTime, now);
        var isOvertime = countdown < TimeSpan.Zero;
        return new TimeCalculation(startTime, endTime, workDuration, breakDuration, progress, countdown, isOvertime);
    }

    public double CalculateProgress(DateTime startTime, DateTime endTime, DateTime now)
    {
        if (now <= startTime)
        {
            return 0;
        }

        var total = (endTime - startTime).TotalMinutes;
        if (total <= 0)
        {
            return 0;
        }

        var elapsed = Math.Min(total, (now - startTime).TotalMinutes);
        return Math.Clamp(elapsed / total * 100, 0, 100);
    }

    public TimeSpan CalculateCountdown(DateTime endTime, DateTime now)
        => endTime - now;
}
