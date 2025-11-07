using System;

namespace WorkTimeCalculator.Models;

public record LunchSettings(TimeSpan Start, TimeSpan End)
{
    public TimeSpan Duration => End - Start;
}
