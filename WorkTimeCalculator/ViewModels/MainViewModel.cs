using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorkTimeCalculator.Models;
using WorkTimeCalculator.Services;

namespace WorkTimeCalculator.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly WorkTimeCalculatorService _calculator = new();
    private readonly ObservableCollection<WorkEntry> _history = new();
    private readonly string _historyFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "WorkTimeCalculator", "history.csv");

    [ObservableProperty]
    private DateTime _selectedDate = DateTime.Today;

    [ObservableProperty]
    private DateTime? _startTime = DateTime.Today.AddHours(8);

    [ObservableProperty]
    private DateTime? _actualEndTime;

    [ObservableProperty]
    private string _recommendedEndTimeDisplay = "--:--";

    [ObservableProperty]
    private string _recommendationDetails = "Enter a start time to see your finish.";

    [ObservableProperty]
    private string _weeklySummary = "0 h";

    [ObservableProperty]
    private string _monthlySummary = "0 h";

    [ObservableProperty]
    private string _overtimeSummary = "On track";

    [ObservableProperty]
    private string _nextNotificationDisplay = "No reminder scheduled";

    [ObservableProperty]
    private double _reminderProgress = 0;

    [ObservableProperty]
    private string _historyFilter = string.Empty;

    [ObservableProperty]
    private int _selectedTabIndex;

    [ObservableProperty]
    private bool _isSettingsOpen;

    public SettingsViewModel Settings { get; }

    public ICollectionView FilteredHistory { get; }

    public ObservableCollection<DailySummary> RecentSummaries { get; } = new();

    public LunchSettings LunchSettings { get; private set; }

    public MainViewModel()
    {
        LunchSettings = new LunchSettings(TimeSpan.FromHours(12), TimeSpan.FromHours(13.5));
        Settings = new SettingsViewModel(LunchSettings, ApplySettings, CloseSettings);

        FilteredHistory = CollectionViewSource.GetDefaultView(_history);
        FilteredHistory.Filter = FilterHistory;

        SeedDemoHistory();
        UpdateSummaries();
        UpdateRecommendation();

        CalculateCommand = new RelayCommand(Calculate, CanCalculate);
        LogEntryCommand = new RelayCommand(LogEntry, CanLogEntry);
        ExportCommand = new RelayCommand(ExportHistory, () => _history.Any());
        RefreshHistoryCommand = new RelayCommand(UpdateSummaries);
        OpenSettingsCommand = new RelayCommand(OpenSettings);
    }

    public RelayCommand CalculateCommand { get; }
    public RelayCommand LogEntryCommand { get; }
    public RelayCommand ExportCommand { get; }
    public RelayCommand RefreshHistoryCommand { get; }
    public RelayCommand OpenSettingsCommand { get; }

    partial void OnStartTimeChanged(DateTime? value)
    {
        CalculateCommand.NotifyCanExecuteChanged();
        LogEntryCommand.NotifyCanExecuteChanged();
        UpdateRecommendation();
    }

    partial void OnActualEndTimeChanged(DateTime? value)
    {
        LogEntryCommand.NotifyCanExecuteChanged();
    }

    partial void OnHistoryFilterChanged(string value)
    {
        FilteredHistory.Refresh();
    }

    private bool FilterHistory(object obj)
    {
        if (obj is not WorkEntry entry)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(HistoryFilter))
        {
            return true;
        }

        var text = HistoryFilter.Trim();
        return entry.Date.ToString("d").Contains(text, StringComparison.CurrentCultureIgnoreCase)
            || entry.StartTime.ToString("t").Contains(text, StringComparison.CurrentCultureIgnoreCase)
            || entry.CalculatedEndTime.ToString("t").Contains(text, StringComparison.CurrentCultureIgnoreCase)
            || (entry.ActualEndTime?.ToString("t")?.Contains(text, StringComparison.CurrentCultureIgnoreCase) ?? false);
    }

    private void Calculate()
    {
        if (!StartTime.HasValue)
        {
            return;
        }

        var calculated = _calculator.CalculateEnd(SelectedDate, StartTime.Value.TimeOfDay, LunchSettings);
        RecommendedEndTimeDisplay = calculated.ToString("t");
        RecommendationDetails = $"8 hours focus + {LunchSettings.Duration.TotalMinutes / 60:0.##} h lunch";
        NextNotificationDisplay = $"Be ready to leave at {calculated.AddMinutes(-15):t}";
        ReminderProgress = 25;
    }

    private bool CanCalculate() => StartTime.HasValue;

    private void LogEntry()
    {
        if (!StartTime.HasValue || !_calculator.TryCalculateTotalHours(StartTime.Value.TimeOfDay, ActualEndTime?.TimeOfDay, LunchSettings, out var totalHours))
        {
            return;
        }

        var calculated = _calculator.CalculateEnd(SelectedDate, StartTime.Value.TimeOfDay, LunchSettings);
        var entry = new WorkEntry
        {
            Date = SelectedDate,
            StartTime = SelectedDate.Add(StartTime.Value.TimeOfDay),
            CalculatedEndTime = calculated,
            ActualEndTime = ActualEndTime.HasValue ? SelectedDate.Add(ActualEndTime.Value.TimeOfDay) : null,
            TotalHours = totalHours
        };

        _history.Insert(0, entry);
        UpdateSummaries();
        FilteredHistory.Refresh();
        ActualEndTime = null;
    }

    private bool CanLogEntry() => StartTime.HasValue && ActualEndTime.HasValue;

    private void ExportHistory()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_historyFilePath)!);
        using var writer = new StreamWriter(_historyFilePath);
        writer.WriteLine("Date,Start,CalculatedEnd,ActualEnd,TotalHours");
        foreach (var entry in _history.OrderByDescending(e => e.Date))
        {
            writer.WriteLine($"{entry.Date:d},{entry.StartTime:t},{entry.CalculatedEndTime:t},{entry.ActualEndTime:t},{entry.TotalHours:F2}");
        }
    }

    private void UpdateRecommendation()
    {
        if (!StartTime.HasValue)
        {
            RecommendedEndTimeDisplay = "--:--";
            RecommendationDetails = "Enter a start time to see your finish.";
            return;
        }

        var calculated = _calculator.CalculateEnd(SelectedDate, StartTime.Value.TimeOfDay, LunchSettings);
        RecommendedEndTimeDisplay = calculated.ToString("t");
        RecommendationDetails = $"8 hours work + {LunchSettings.Duration.TotalMinutes / 60:0.##} h lunch";
    }

    private void UpdateSummaries()
    {
        RecentSummaries.Clear();

        if (_history.Count == 0)
        {
            WeeklySummary = "0 h";
            MonthlySummary = "0 h";
            OvertimeSummary = "On track";
            return;
        }

        var today = DateTime.Today;
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
        var weeklyHours = _history.Where(h => h.Date >= startOfWeek).Sum(h => h.TotalHours);
        WeeklySummary = $"{weeklyHours:F1} h";

        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var monthlyHours = _history.Where(h => h.Date >= startOfMonth).Sum(h => h.TotalHours);
        MonthlySummary = $"{monthlyHours:F1} h";

        var expectedWeekly = 40;
        var overtime = weeklyHours - expectedWeekly;
        OvertimeSummary = overtime switch
        {
            > 0 => $"+{overtime:F1} h overtime",
            < 0 => $"{overtime:F1} h behind",
            _ => "On track"
        };
        UpdateRecentSummaries();
    }

    private void UpdateRecentSummaries()
    {
        var recentSummaries = _history
            .GroupBy(h => h.Date.Date)
            .OrderByDescending(g => g.Key)
            .Take(7)
            .Select(g => new DailySummary(g.Key, Math.Round(g.Sum(h => h.TotalHours), 2)));

        foreach (var summary in recentSummaries)
        {
            RecentSummaries.Add(summary);
        }
    }

    private void SeedDemoHistory()
    {
        var now = DateTime.Today;
        for (int i = 1; i <= 6; i++)
        {
            var date = now.AddDays(-i);
            var start = date.AddHours(8).AddMinutes(i * 3 % 30);
            var calculated = _calculator.CalculateEnd(date, start.TimeOfDay, LunchSettings);
            _history.Add(new WorkEntry
            {
                Date = date,
                StartTime = start,
                CalculatedEndTime = calculated,
                ActualEndTime = calculated.AddMinutes(-5),
                TotalHours = 8
            });
        }
    }

    private void ApplySettings(LunchSettings settings)
    {
        LunchSettings = settings;
        UpdateRecommendation();
        Settings.SetSource(settings);
        IsSettingsOpen = false;
    }

    private void CloseSettings()
    {
        IsSettingsOpen = false;
    }

    private void OpenSettings()
    {
        Settings.SetSource(LunchSettings);
        IsSettingsOpen = true;
    }
}

public record DailySummary(DateTime Date, double TotalHours)
{
    public string DateDisplay => Date.ToString("ddd, MMM d");

    public string TotalHoursDisplay => $"{TotalHours:F1} h";
}
