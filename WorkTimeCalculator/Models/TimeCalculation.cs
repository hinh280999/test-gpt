using System;

namespace WorkTimeCalculator.Models;

/// <summary>
/// Represents the output of a time calculation for the quick planner.
/// </summary>
public record TimeCalculation(
    DateTime Start,
    DateTime End,
    TimeSpan WorkDuration,
    TimeSpan BreakDuration,
    double ProgressPercentage,
    TimeSpan Countdown,
    bool IsOvertime);
