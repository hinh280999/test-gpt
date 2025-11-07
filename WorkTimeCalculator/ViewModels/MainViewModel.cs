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

    private string _startDateText = DateTime.Today.ToString("dd/MM/yyyy");
    private string _startTimeText = DateTime.Today.AddHours(8).ToString("HH:mm");
    private string _startDateTimeText = $"{DateTime.Today:dd/MM/yyyy} {DateTime.Today.AddHours(8):HH:mm}";
    private bool _isUpdatingFromDateTime = false;

    public string StartDateText
    {
        get => _startDateText;
        set
        {
            if (_startDateText != value)
            {
                _startDateText = value;
                OnPropertyChanged();
                UpdateStartDateTimeText();
                if (!_isUpdatingFromDateTime)
                {
                    ParseDateTime();
                }
            }
        }
    }

    public string StartTimeText
    {
        get => _startTimeText;
        set
        {
            if (_startTimeText != value)
            {
                _startTimeText = value;
                OnPropertyChanged();
                UpdateStartDateTimeText();
                if (!_isUpdatingFromDateTime)
                {
                    ParseDateTime();
                }
            }
        }
    }

    public string StartDateTimeText
    {
        get => _startDateTimeText;
        set
        {
            if (_startDateTimeText != value)
            {
                _startDateTimeText = value;
                OnPropertyChanged();
                if (!_isUpdatingFromDateTime)
                {
                    ParseStartDateTimeText();
                }
            }
        }
    }

    private void UpdateStartDateTimeText()
    {
        if (StartTime.HasValue)
        {
            var dateTime = StartTime.Value;
            if (dateTime.Date == DateTime.Today)
            {
                _startDateTimeText = dateTime.ToString("h:mm tt");
            }
            else
            {
                _startDateTimeText = $"{dateTime:dd/MM/yyyy} {dateTime:h:mm tt}";
            }
            OnPropertyChanged(nameof(StartDateTimeText));
        }
    }

    private void ParseStartDateTimeText()
    {
        if (string.IsNullOrWhiteSpace(_startDateTimeText))
        {
            return;
        }

        // Try to parse as full datetime first (dd/MM/yyyy HH:mm or dd/MM/yyyy h:mm tt)
        if (DateTime.TryParseExact(_startDateTimeText, "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out var fullDate))
        {
            _isUpdatingFromDateTime = true;
            try
            {
                SelectedDate = fullDate.Date;
                StartTime = fullDate;
                _startDateText = fullDate.ToString("dd/MM/yyyy");
                _startTimeText = fullDate.ToString("HH:mm");
                OnPropertyChanged(nameof(StartDateText));
                OnPropertyChanged(nameof(StartTimeText));
            }
            finally
            {
                _isUpdatingFromDateTime = false;
            }
            return;
        }

        // Try to parse as date with 12-hour format
        if (DateTime.TryParseExact(_startDateTimeText, "dd/MM/yyyy h:mm tt", null, System.Globalization.DateTimeStyles.None, out fullDate))
        {
            _isUpdatingFromDateTime = true;
            try
            {
                SelectedDate = fullDate.Date;
                StartTime = fullDate;
                _startDateText = fullDate.ToString("dd/MM/yyyy");
                _startTimeText = fullDate.ToString("HH:mm");
                OnPropertyChanged(nameof(StartDateText));
                OnPropertyChanged(nameof(StartTimeText));
            }
            finally
            {
                _isUpdatingFromDateTime = false;
            }
            return;
        }

        // Try to parse as time only (HH:mm or h:mm tt) - use today's date
        if (DateTime.TryParseExact(_startDateTimeText, "HH:mm", null, System.Globalization.DateTimeStyles.None, out var timeOnly))
        {
            _isUpdatingFromDateTime = true;
            try
            {
                StartTime = SelectedDate.Date.Add(timeOnly.TimeOfDay);
                _startTimeText = timeOnly.ToString("HH:mm");
                _startDateText = SelectedDate.ToString("dd/MM/yyyy");
                OnPropertyChanged(nameof(StartTimeText));
                OnPropertyChanged(nameof(StartDateText));
            }
            finally
            {
                _isUpdatingFromDateTime = false;
            }
            return;
        }

        // Try to parse as 12-hour time format
        if (DateTime.TryParseExact(_startDateTimeText, "h:mm tt", null, System.Globalization.DateTimeStyles.None, out timeOnly))
        {
            _isUpdatingFromDateTime = true;
            try
            {
                StartTime = SelectedDate.Date.Add(timeOnly.TimeOfDay);
                _startTimeText = timeOnly.ToString("HH:mm");
                _startDateText = SelectedDate.ToString("dd/MM/yyyy");
                OnPropertyChanged(nameof(StartTimeText));
                OnPropertyChanged(nameof(StartDateText));
            }
            finally
            {
                _isUpdatingFromDateTime = false;
            }
            return;
        }

        // Try generic parse
        if (DateTime.TryParse(_startDateTimeText, out var parsed))
        {
            _isUpdatingFromDateTime = true;
            try
            {
                SelectedDate = parsed.Date;
                StartTime = parsed;
                _startDateText = parsed.ToString("dd/MM/yyyy");
                _startTimeText = parsed.ToString("HH:mm");
                OnPropertyChanged(nameof(StartDateText));
                OnPropertyChanged(nameof(StartTimeText));
            }
            finally
            {
                _isUpdatingFromDateTime = false;
            }
        }
    }

    private void ParseDateTime()
    {
        // Parse date
        DateTime? parsedDate = null;
        if (DateTime.TryParseExact(_startDateText, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var date))
        {
            parsedDate = date;
        }
        else if (DateTime.TryParse(_startDateText, out date))
        {
            parsedDate = date;
        }

        // Parse time
        TimeSpan? parsedTime = null;
        if (TimeSpan.TryParse(_startTimeText, out var time))
        {
            parsedTime = time;
        }
        else if (DateTime.TryParseExact(_startTimeText, "HH:mm", null, System.Globalization.DateTimeStyles.None, out var timeOnly))
        {
            parsedTime = timeOnly.TimeOfDay;
        }

        // Update SelectedDate and StartTime
        _isUpdatingFromDateTime = true;
        try
        {
            if (parsedDate.HasValue)
            {
                SelectedDate = parsedDate.Value;
            }

            if (parsedTime.HasValue)
            {
                var dateToUse = parsedDate ?? SelectedDate;
                StartTime = dateToUse.Date.Add(parsedTime.Value);
            }
        }
        finally
        {
            _isUpdatingFromDateTime = false;
        }
    }

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

    [ObservableProperty]
    private double _workDurationHours = 8.0;

    [ObservableProperty]
    private double _selectedBreakMinutes = 90;

    [ObservableProperty]
    private double _todayProgress = 0;

    [ObservableProperty]
    private double _todayTotalHours = 0;

    [ObservableProperty]
    private double _weeklyProgressPercent = 0;

    [ObservableProperty]
    private double _weeklyHours = 0;

    [ObservableProperty]
    private double _monthlyHours = 0;

    [ObservableProperty]
    private int _daysWorkedThisMonth = 0;

    [ObservableProperty]
    private string _timeRemaining = "0h 0m";

    [ObservableProperty]
    private bool _notifyBeforeEnd = false;

    public SettingsViewModel Settings { get; }

    public ICollectionView FilteredHistory { get; }

    public ObservableCollection<DailySummary> RecentSummaries { get; } = new();

    public ObservableCollection<WorkEntry> TodayEntries { get; } = new();

    public LunchSettings LunchSettings { get; private set; }

    public MainViewModel()
    {
        LunchSettings = new LunchSettings(TimeSpan.FromHours(12), TimeSpan.FromHours(13.5));
        Settings = new SettingsViewModel(LunchSettings, WorkDurationHours, SelectedBreakMinutes, ApplySettings, CloseSettings);

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
        
        // Set break time to match lunch duration
        SelectedBreakMinutes = LunchSettings.Duration.TotalMinutes;
        
        // Initialize StartTime with SelectedDate
        if (!StartTime.HasValue)
        {
            StartTime = SelectedDate.AddHours(8);
        }
        
        // Initialize text fields
        _startDateText = SelectedDate.ToString("dd/MM/yyyy");
        _startTimeText = StartTime.Value.ToString("HH:mm");
        UpdateStartDateTimeText();
    }

    public RelayCommand CalculateCommand { get; }
    public RelayCommand LogEntryCommand { get; }
    public RelayCommand ExportCommand { get; }
    public RelayCommand RefreshHistoryCommand { get; }
    public RelayCommand OpenSettingsCommand { get; }

    partial void OnSelectedDateChanged(DateTime value)
    {
        // Update StartTime when SelectedDate changes - combine date with existing time
        if (StartTime.HasValue)
        {
            var timeOfDay = StartTime.Value.TimeOfDay;
            StartTime = value.Date.Add(timeOfDay);
        }
        else
        {
            StartTime = value.Date.AddHours(8);
        }
        
        // Update text fields only if not updating from text input
        if (!_isUpdatingFromDateTime)
        {
            _startDateText = value.ToString("dd/MM/yyyy");
            OnPropertyChanged(nameof(StartDateText));
            UpdateStartDateTimeText();
        }
        
        CalculateCommand.NotifyCanExecuteChanged();
        LogEntryCommand.NotifyCanExecuteChanged();
        UpdateRecommendation();
    }

    partial void OnStartTimeChanged(DateTime? value)
    {
        // Update time text field only if not updating from text input
        if (value.HasValue && !_isUpdatingFromDateTime)
        {
            _startTimeText = value.Value.ToString("HH:mm");
            OnPropertyChanged(nameof(StartTimeText));
            UpdateStartDateTimeText();
        }
        
        CalculateCommand.NotifyCanExecuteChanged();
        LogEntryCommand.NotifyCanExecuteChanged();
        UpdateRecommendation();
    }

    partial void OnActualEndTimeChanged(DateTime? value)
    {
        LogEntryCommand.NotifyCanExecuteChanged();
    }

    partial void OnWorkDurationHoursChanged(double value)
    {
        UpdateRecommendation();
        OnPropertyChanged(nameof(WorkDurationHoursDisplay));
    }

    partial void OnSelectedBreakMinutesChanged(double value)
    {
        UpdateRecommendation();
    }

    public string WorkDurationHoursDisplay => $"{WorkDurationHours:F1}h";

    public string TodayProgressDisplay => $"{TodayTotalHours:F1}h / {WorkDurationHours:F1}h";

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

        var breakDuration = TimeSpan.FromMinutes(SelectedBreakMinutes);
        var workDuration = TimeSpan.FromHours(WorkDurationHours);
        
        // Use StartTime directly if it has a valid date, otherwise combine with SelectedDate
        var start = StartTime.Value.Date == DateTime.Today || StartTime.Value.Date == SelectedDate
            ? StartTime.Value
            : SelectedDate.Date.Add(StartTime.Value.TimeOfDay);
        var lunchStart = start.Date.Add(LunchSettings.Start);
        var lunchEnd = start.Date.Add(LunchSettings.End);

        DateTime calculated;
        
        // If start time is after lunch, skip lunch break
        if (start >= lunchEnd)
        {
            calculated = start.Add(workDuration);
        }
        else
        {
            // Calculate with lunch break
            var adjusted = start.Add(workDuration);
            if (start <= lunchStart)
            {
                adjusted = adjusted.Add(breakDuration);
            }
            else if (start < lunchEnd)
            {
                var remainingLunch = lunchEnd - start;
                adjusted = adjusted.Add(remainingLunch > breakDuration ? breakDuration : remainingLunch);
            }
            calculated = adjusted;
        }

        RecommendedEndTimeDisplay = calculated.ToString("t");
        RecommendationDetails = $"{WorkDurationHours:F1} hours work + {SelectedBreakMinutes / 60:0.##} h break";
        NextNotificationDisplay = NotifyBeforeEnd ? $"Notify at {calculated.AddMinutes(-15):t}" : "No reminder scheduled";
        
        // Calculate time remaining
        var now = DateTime.Now;
        if (calculated > now)
        {
            var remaining = calculated - now;
            TimeRemaining = $"{(int)remaining.TotalHours}h {remaining.Minutes}m";
            TodayProgress = Math.Min(100, (now - start).TotalHours / WorkDurationHours * 100);
        }
        else
        {
            TimeRemaining = "0h 0m";
            TodayProgress = 100;
        }
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
            TimeRemaining = "0h 0m";
            TodayProgress = 0;
            return;
        }

        Calculate();
    }

    private void UpdateSummaries()
    {
        RecentSummaries.Clear();

        if (_history.Count == 0)
        {
            WeeklySummary = "0 h";
            MonthlySummary = "0 h";
            OvertimeSummary = "On track";
            WeeklyHours = 0;
            MonthlyHours = 0;
            WeeklyProgressPercent = 0;
            DaysWorkedThisMonth = 0;
            return;
        }

        var today = DateTime.Today;
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
        var weeklyHours = _history.Where(h => h.Date >= startOfWeek).Sum(h => h.TotalHours);
        WeeklyHours = weeklyHours;
        WeeklySummary = $"{weeklyHours:F1} h";

        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var monthlyHours = _history.Where(h => h.Date >= startOfMonth).Sum(h => h.TotalHours);
        MonthlyHours = monthlyHours;
        MonthlySummary = $"{monthlyHours:F1} h";
        
        DaysWorkedThisMonth = _history.Where(h => h.Date >= startOfMonth).Select(h => h.Date).Distinct().Count();

        var expectedWeekly = 40;
        WeeklyProgressPercent = Math.Min(100, (weeklyHours / expectedWeekly) * 100);
        
        var overtime = weeklyHours - expectedWeekly;
        OvertimeSummary = overtime switch
        {
            > 0 => $"+{overtime:F1} h overtime",
            < 0 => $"{overtime:F1} h behind",
            _ => "On track"
        };
        
        // Update today's entries
        UpdateTodayEntries();
        UpdateRecentSummaries();
    }

    private void UpdateTodayEntries()
    {
        TodayEntries.Clear();
        var todayEntries = _history.Where(h => h.Date.Date == DateTime.Today).OrderBy(h => h.StartTime);
        foreach (var entry in todayEntries)
        {
            TodayEntries.Add(entry);
        }
        
        TodayTotalHours = TodayEntries.Sum(e => e.TotalHours);
        TodayProgress = WorkDurationHours > 0 ? Math.Min(100, (TodayTotalHours / WorkDurationHours) * 100) : 0;
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

    private void ApplySettings(LunchSettings settings, double workDurationHours, double breakMinutes)
    {
        LunchSettings = settings;
        WorkDurationHours = workDurationHours;
        SelectedBreakMinutes = breakMinutes;
        UpdateRecommendation();
        Settings.SetSource(settings, WorkDurationHours, SelectedBreakMinutes);
        IsSettingsOpen = false;
    }

    private void CloseSettings()
    {
        IsSettingsOpen = false;
    }

    private void OpenSettings()
    {
        Settings.SetSource(LunchSettings, WorkDurationHours, SelectedBreakMinutes);
        IsSettingsOpen = true;
    }
}

public record DailySummary(DateTime Date, double TotalHours)
{
    public string DateDisplay => Date.ToString("ddd, MMM d");

    public string TotalHoursDisplay => $"{TotalHours:F1} h";
}
