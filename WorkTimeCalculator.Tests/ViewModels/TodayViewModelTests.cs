using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using WorkTimeCalculator.Models;
using WorkTimeCalculator.Services;
using WorkTimeCalculator.ViewModels;
using Xunit;

namespace WorkTimeCalculator.Tests.ViewModels;

public class TodayViewModelTests
{
    [Fact]
    public async Task AddEntryCommand_SavesEntryAndRefreshes()
    {
        var categories = new List<CategoryTag> { new() { Name = "Focus Work" } } as IReadOnlyList<CategoryTag>;
        var entries = Array.Empty<WorkEntry>() as IReadOnlyList<WorkEntry>;

        var dataService = new Mock<IDataService>();
        dataService.Setup(s => s.GetCategoriesAsync()).ReturnsAsync(categories);
        dataService.Setup(s => s.GetEntriesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(entries);
        dataService.Setup(s => s.GetRecentEntriesAsync(It.IsAny<int>())).ReturnsAsync(entries);
        dataService.Setup(s => s.SaveEntryAsync(It.IsAny<WorkEntry>())).Returns(Task.CompletedTask).Verifiable();

        var calculationService = new Mock<ICalculationService>();
        calculationService.Setup(c => c.Calculate(It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(), It.IsAny<DateTime>()))
            .Returns((DateTime start, TimeSpan work, TimeSpan breaks, DateTime now) =>
                new TimeCalculation(start, start.Add(work).Add(breaks), work, breaks, 50, TimeSpan.FromHours(4), false));
        calculationService.Setup(c => c.CalculateCountdown(It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(TimeSpan.FromHours(4));
        calculationService.Setup(c => c.CalculateProgress(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<DateTime>())).Returns(50);

        var notificationService = new Mock<INotificationService>();
        notificationService.Setup(n => n.ScheduleReminderAsync(It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<string>())).Returns(Task.CompletedTask);

        var clock = new Mock<IClock>();
        clock.SetupGet(c => c.Now).Returns(new DateTime(2024, 5, 1, 8, 0, 0));

        var viewModel = new TodayViewModel(dataService.Object, calculationService.Object, notificationService.Object, clock.Object);
        await viewModel.InitializationTask;

        viewModel.EntryStartTime = new DateTime(2024, 5, 1, 8, 0, 0);
        viewModel.EntryEndTime = new DateTime(2024, 5, 1, 17, 0, 0);
        viewModel.EntryBreakMinutes = 60;
        viewModel.SelectedCategory = "Focus Work";

        await viewModel.AddEntryCommand.ExecuteAsync(null);

        dataService.Verify(s => s.SaveEntryAsync(It.Is<WorkEntry>(e => e.Category == "Focus Work" && e.BreakMinutes == 60)), Times.Once);
    }
}
