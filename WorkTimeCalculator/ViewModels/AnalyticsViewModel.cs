using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorkTimeCalculator.Services;

namespace WorkTimeCalculator.ViewModels;

public partial class AnalyticsViewModel : BaseViewModel
{
    private readonly IDataService _dataService;

    [ObservableProperty]
    private string _peakHoursDescription = "--";

    [ObservableProperty]
    private string _breakPatternDescription = "--";

    [ObservableProperty]
    private double[] _weeklyComparisons = Array.Empty<double>();

    [ObservableProperty]
    private (string Category, double Percentage)[] _categoryDistribution = Array.Empty<(string, double)>();

    [ObservableProperty]
    private string? _statusMessage;

    public AnalyticsViewModel(IDataService dataService)
    {
        _dataService = dataService;
        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        _ = RefreshAsync();
    }

    public IAsyncRelayCommand RefreshCommand { get; }

    private async Task RefreshAsync()
    {
        var start = DateTime.Today.AddDays(-60);
        var end = DateTime.Today;
        var entries = await _dataService.GetEntriesAsync(start, end);
        if (entries.Count == 0)
        {
            StatusMessage = "No analytics available yet.";
            return;
        }

        var groupedByHour = entries
            .SelectMany(e => Enumerable.Range(0, (int)Math.Ceiling(e.Duration.TotalHours))
                .Select(i => e.StartTime.AddHours(i).Hour))
            .GroupBy(h => h)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault();

        if (groupedByHour is not null)
        {
            PeakHoursDescription = $"Most productive around {groupedByHour.Key}:00";
        }

        var averageBreak = entries.Average(e => e.BreakMinutes);
        BreakPatternDescription = $"Average break {averageBreak:0} min";

        var currentWeekStart = end.AddDays(-(int)end.DayOfWeek);
        WeeklyComparisons = Enumerable.Range(0, 8)
            .Select(offset =>
            {
                var weekStart = currentWeekStart.AddDays(-7 * offset);
                var weekEnd = weekStart.AddDays(6);
                return entries.Where(e => e.Date >= weekStart && e.Date <= weekEnd)
                    .Sum(e => e.Duration.TotalHours);
            })
            .Reverse()
            .Select(hours => Math.Round(hours, 2))
            .ToArray();

        var totalHours = entries.Sum(e => e.Duration.TotalHours);
        CategoryDistribution = entries
            .GroupBy(e => e.Category)
            .Select(g => (g.Key, Math.Round(g.Sum(e => e.Duration.TotalHours) / totalHours * 100, 1)))
            .OrderByDescending(tuple => tuple.Item2)
            .ToArray();

        StatusMessage = "Analytics updated";
    }
}
