using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorkTimeCalculator.Models;
using WorkTimeCalculator.Services;

namespace WorkTimeCalculator.ViewModels;

public partial class HistoryViewModel : BaseViewModel
{
    private const double DailyGoalHours = 8.0;
    private readonly IDataService _dataService;
    private readonly IExportService _exportService;

    public ObservableCollection<CalendarDayViewModel> CalendarDays { get; } = new();
    public ObservableCollection<HistoryEntryViewModel> Entries { get; } = new();
    public ObservableCollection<string> Categories { get; } = new() { "All" };

    [ObservableProperty]
    private DateTime _displayMonth;

    [ObservableProperty]
    private DateTime _rangeStart;

    [ObservableProperty]
    private DateTime _rangeEnd;

    [ObservableProperty]
    private string _selectedCategory = "All";

    [ObservableProperty]
    private bool _overtimeOnly;

    [ObservableProperty]
    private string? _statusMessage;

    [ObservableProperty]
    private double[] _weeklyTrend = Array.Empty<double>();

    [ObservableProperty]
    private (string Category, double Hours)[] _categoryDistribution = Array.Empty<(string, double)>();

    public HistoryViewModel(IDataService dataService, IExportService exportService)
    {
        _dataService = dataService;
        _exportService = exportService;

        var today = DateTime.Today;
        _displayMonth = new DateTime(today.Year, today.Month, 1);
        _rangeStart = today.AddDays(-30);
        _rangeEnd = today;

        RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        NextMonthCommand = new AsyncRelayCommand(NextMonthAsync);
        PreviousMonthCommand = new AsyncRelayCommand(PreviousMonthAsync);
        ExportSelectedCommand = new AsyncRelayCommand(ExportSelectedAsync);
        DeleteSelectedCommand = new AsyncRelayCommand(DeleteSelectedAsync);

        _ = InitializeAsync();
    }

    public IAsyncRelayCommand RefreshCommand { get; }
    public IAsyncRelayCommand NextMonthCommand { get; }
    public IAsyncRelayCommand PreviousMonthCommand { get; }
    public IAsyncRelayCommand ExportSelectedCommand { get; }
    public IAsyncRelayCommand DeleteSelectedCommand { get; }

    partial void OnDisplayMonthChanged(DateTime value)
    {
        _ = BuildCalendarAsync();
    }

    partial void OnSelectedCategoryChanged(string value)
    {
        _ = RefreshAsync();
    }

    partial void OnOvertimeOnlyChanged(bool value)
    {
        _ = RefreshAsync();
    }

    partial void OnRangeStartChanged(DateTime value)
    {
        if (RangeEnd < value)
        {
            RangeEnd = value;
        }

        _ = RefreshAsync();
    }

    partial void OnRangeEndChanged(DateTime value)
    {
        if (value < RangeStart)
        {
            RangeStart = value;
        }

        _ = RefreshAsync();
    }

    private async Task InitializeAsync()
    {
        var categories = await _dataService.GetCategoriesAsync();
        foreach (var category in categories)
        {
            if (!Categories.Contains(category.Name))
            {
                Categories.Add(category.Name);
            }
        }

        await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        var entries = await _dataService.GetEntriesAsync(RangeStart, RangeEnd);
        var filtered = entries.AsEnumerable();

        if (SelectedCategory != "All")
        {
            filtered = filtered.Where(e => e.Category == SelectedCategory);
        }

        if (OvertimeOnly)
        {
            filtered = filtered.Where(e => e.Duration.TotalHours > DailyGoalHours);
        }

        var ordered = filtered.OrderByDescending(e => e.Date).ThenBy(e => e.StartTime).ToList();

        Entries.Clear();
        foreach (var entry in ordered)
        {
            Entries.Add(new HistoryEntryViewModel(entry));
        }

        await BuildCalendarAsync(entries);
        BuildTrendData(entries);
        BuildCategoryDistribution(entries);

        StatusMessage = $"{Entries.Count} entries loaded";
    }

    private async Task BuildCalendarAsync()
    {
        var monthStart = new DateTime(DisplayMonth.Year, DisplayMonth.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
        var entries = await _dataService.GetEntriesAsync(monthStart, monthEnd);
        await BuildCalendarAsync(entries);
    }

    private Task BuildCalendarAsync(IReadOnlyCollection<WorkEntry> monthEntries)
    {
        CalendarDays.Clear();
        var monthStart = new DateTime(DisplayMonth.Year, DisplayMonth.Month, 1);
        var firstDay = monthStart.AddDays(-(int)monthStart.DayOfWeek);
        for (var i = 0; i < 42; i++)
        {
            var day = firstDay.AddDays(i);
            var total = monthEntries.Where(e => e.Date.Date == day.Date).Sum(e => e.Duration.TotalHours);
            CalendarDays.Add(new CalendarDayViewModel(day, Math.Round(total, 2), DailyGoalHours));
        }

        return Task.CompletedTask;
    }

    private void BuildTrendData(IReadOnlyCollection<WorkEntry> entries)
    {
        var today = DateTime.Today;
        var buckets = Enumerable.Range(0, 6)
            .Select(offset =>
            {
                var weekStart = today.AddDays(-7 * offset - (int)today.DayOfWeek);
                var weekEnd = weekStart.AddDays(6);
                var weekHours = entries.Where(e => e.Date >= weekStart && e.Date <= weekEnd)
                    .Sum(e => e.Duration.TotalHours);
                return Math.Round(weekHours, 2);
            })
            .Reverse()
            .ToArray();

        WeeklyTrend = buckets;
    }

    private void BuildCategoryDistribution(IReadOnlyCollection<WorkEntry> entries)
    {
        CategoryDistribution = entries
            .GroupBy(e => e.Category)
            .Select(g => (g.Key, Math.Round(g.Sum(e => e.Duration.TotalHours), 2)))
            .OrderByDescending(tuple => tuple.Item2)
            .ToArray();
    }

    private async Task NextMonthAsync()
    {
        DisplayMonth = DisplayMonth.AddMonths(1);
        await RefreshAsync();
    }

    private async Task PreviousMonthAsync()
    {
        DisplayMonth = DisplayMonth.AddMonths(-1);
        await RefreshAsync();
    }

    private async Task ExportSelectedAsync()
    {
        var selected = Entries.Where(e => e.IsSelected).Select(e => e.Entry).ToList();
        if (selected.Count == 0)
        {
            selected = Entries.Select(e => e.Entry).ToList();
        }

        var path = await _exportService.ExportAsync(selected, Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
        StatusMessage = $"Exported to {path}";
    }

    private async Task DeleteSelectedAsync()
    {
        var selectedIds = Entries.Where(e => e.IsSelected).Select(e => e.Entry.Id).ToList();
        if (selectedIds.Count == 0)
        {
            StatusMessage = "Select entries to delete.";
            return;
        }

        await _dataService.DeleteEntriesAsync(selectedIds);
        StatusMessage = "Entries deleted";
        await RefreshAsync();
    }
}
