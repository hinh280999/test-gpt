using System;
using WorkTimeCalculator.Models;
using WorkTimeCalculator.Services;
using Xunit;

namespace WorkTimeCalculator.Tests;

public class CalculationServiceTests
{
    [Fact]
    public void Calculate_ReturnsExpectedEndAndProgress()
    {
        var service = new CalculationService();
        var start = new DateTime(2024, 5, 1, 8, 0, 0);
        var result = service.Calculate(start, TimeSpan.FromHours(8), TimeSpan.FromMinutes(60), start.AddHours(4));

        Assert.Equal(new DateTime(2024, 5, 1, 17, 0, 0), result.End);
        Assert.Equal(9, result.WorkDuration.TotalHours + result.BreakDuration.TotalHours, 1);
        Assert.True(result.ProgressPercentage > 40 && result.ProgressPercentage < 50);
        Assert.Equal(TimeSpan.FromHours(5), result.Countdown);
        Assert.False(result.IsOvertime);
    }
}
