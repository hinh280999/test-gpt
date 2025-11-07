using System;
using System.Threading.Tasks;

namespace WorkTimeCalculator.Services;

public interface INotificationService
{
    Task ScheduleReminderAsync(DateTime targetTime, TimeSpan leadTime, string message);
    Task CancelAllAsync();
}

public class NotificationService : INotificationService
{
    public Task ScheduleReminderAsync(DateTime targetTime, TimeSpan leadTime, string message)
    {
        // Desktop notifications would be implemented here; for now we no-op.
        return Task.CompletedTask;
    }

    public Task CancelAllAsync() => Task.CompletedTask;
}
