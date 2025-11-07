using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorkTimeCalculator.Models;
using WorkTimeCalculator.Services;

namespace WorkTimeCalculator.ViewModels;

public partial class TodayViewModel : BaseViewModel
{
    private const double StandardWorkHours = 8.0;

    private readonly IDataService _dataService;
    private readonly ICalculationService _calculationService;
    private readonly INotificationService _notificationService;
    private readonly IClock _clock;
    private readonly DispatcherTimer _timer;

    public ObservableCollection<BreakOptionModel> BreakOptions { get; } = new();
    public ObservableCollection<TimeBlockViewModel> TodayEntries { get; } = new();
    public ObservableCollection<TimeBlockViewModel> RecentActivity { get; } = new();
    public ObservableCollection<CategoryTag> Categories { get; } = new();

    [ObservableProperty]
    private DateTime _selectedDate;

    [ObservableProperty]
    private DateTime _startTime;

    [ObservableProperty]
    private BreakOptionModel? _selectedBreakOption;

    [ObservableProperty]
    private int _customBreakMinutes = 60;

    [ObservableProperty]
    private DateTime _entryStartTime;

    [ObservableProperty]
    private DateTime _entryEndTime;

    [ObservableProperty]
    private int _entryBreakMinutes = 60;

    [ObservableProperty]
    private string _selectedCategory = "Focus Work";

    [ObservableProperty]
    private string? _entryNotes;

    [ObservableProperty]
    private bool _notificationsEnabled = true;

    [ObservableProperty]
    private string _recommendedEndTime = "--:--";

    [ObservableProperty]
    private string _calculationBreakdown = string.Empty;

    [ObservableProperty]
    private double _progressValue;

    [ObservableProperty]
    private string _countdownText = string.Empty;

    [ObservableProperty]
    private bool _isOvertime;

    [ObservableProperty]
    private double _weeklyHours;

    [ObservableProperty]
    private double _weeklyGoal = StandardWorkHours * 5;

    [ObservableProperty]
    private double _weeklyProgress;

    [ObservableProperty]
    private string _weeklyStatus = "On track";

    [ObservableProperty]
    private double _monthlyHours;

    [ObservableProperty]
    private double _monthlyAverage;

    [ObservableProperty]
    private int _daysWorked;

    [ObservableProperty]
    private string _overtimeStatus = string.Empty;

    [ObservableProperty]
    private string? _statusMessage;

    public TodayViewModel(IDataService dataService, ICalculationService calculationService, INotificationService notificationService, IClock clock)
    {
        _dataService = dataService;
        _calculationService = calculationService;
        _notificationService = notificationService;
        _clock = clock;

        SelectedDate = clock.Now.Date;
        _startTime = SelectedDate.AddHours(8);
        _entryStartTime = _startTime;
        _entryEndTime = _startTime.AddHours(9);

        BreakOptions.Add(new BreakOptionModel("30 min", TimeSpan.FromMinutes(30), false));
        BreakOptions.Add(new BreakOptionModel("1 hour", TimeSpan.FromHours(1), false));
        BreakOptions.Add(new BreakOptionModel("Custom", TimeSpan.FromHours(1), true));
        SelectedBreakOption = BreakOptions.First();

        AddEntryCommand = new AsyncRelayCommand(AddEntryAsync);
        StartNowCommand = new RelayCommand(SetStartToNow);
        StandardDayCommand = new RelayCommand(SetStandardDay);
        FlexibleDayCommand = new RelayCommand(SetFlexible);
        ApplyTemplateCommand = new RelayCommand<string>(ApplyTemplate);

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
        _timer.Tick += (_, _) => UpdateCalculation();
        _timer.Start();

        InitializationTask = InitializeAsync();
    }

    public IAsyncRelayCommand AddEntryCommand { get; }
    public IRelayCommand StartNowCommand { get; }
    public IRelayCommand StandardDayCommand { get; }
    public IRelayCommand FlexibleDayCommand { get; }
    public IRelayCommand<string> ApplyTemplateCommand { get; }

    public Task InitializationTask { get; }

    public TimeSpan SelectedBreakDuration => (SelectedBreakOption?.IsCustom ?? false)
        ? TimeSpan.FromMinutes(CustomBreakMinutes)
        : SelectedBreakOption?.Duration ?? TimeSpan.FromMinutes(CustomBreakMinutes);

    partial void OnSelectedDateChanged(DateTime value)
    {
        _ = RefreshAsync();
    }

    partial void OnStartTimeChanged(DateTime value)
    {
        UpdateCalculation();
    }

    partial void OnSelectedBreakOptionChanged(BreakOptionModel? value)
    {
        UpdateCalculation();
    }

    partial void OnCustomBreakMinutesChanged(int value)
    {
        if (value < 0)
        {
            CustomBreakMinutes = 0;
            return;
        }

        UpdateCalculation();
    }

    partial void OnNotificationsEnabledChanged(bool value)
    {
        if (value)
        {
            ScheduleNotification();
        }
        else
        {
            _ = _notificationService.CancelAllAsync();
        }
    }

    private async Task InitializeAsync()
    {
        var categories = await _dataService.GetCategoriesAsync();
        Categories.Clear();
        foreach (var category in categories)
        {
            Categories.Add(category);
        }

        if (Categories.Any())
        {
            SelectedCategory = Categories.First().Name;
        }

        await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        var dayEntries = await _dataService.GetEntriesAsync(SelectedDate, SelectedDate);
        TodayEntries.Clear();
        foreach (var entry in dayEntries)
        {
            TodayEntries.Add(new TimeBlockViewModel(entry));
        }

        var recent = await _dataService.GetRecentEntriesAsync(7);
        RecentActivity.Clear();
        foreach (var entry in recent)
        {
            RecentActivity.Add(new TimeBlockViewModel(entry));
        }

        await UpdateSummaryAsync();
        UpdateCalculation();
    }

    private async Task UpdateSummaryAsync()
    {
        var startOfWeek = SelectedDate.AddDays(-(int)SelectedDate.DayOfWeek);
        var weekEntries = await _dataService.GetEntriesAsync(startOfWeek, startOfWeek.AddDays(6));
        WeeklyHours = Math.Round(weekEntries.Sum(e => e.Duration.TotalHours), 2);
        WeeklyProgress = Math.Clamp(WeeklyHours / WeeklyGoal * 100, 0, 150);
        WeeklyStatus = WeeklyHours switch
        {
            var h when h >= WeeklyGoal => $"On track ({WeeklyHours:0.0}h)",
            var h when h >= WeeklyGoal - 5 => "Approaching goal",
            _ => "Needs attention"
        };

        var startOfMonth = new DateTime(SelectedDate.Year, SelectedDate.Month, 1);
        var monthEntries = await _dataService.GetEntriesAsync(startOfMonth, startOfMonth.AddMonths(1).AddDays(-1));
        MonthlyHours = Math.Round(monthEntries.Sum(e => e.Duration.TotalHours), 2);
        DaysWorked = monthEntries.Select(e => e.Date.Date).Distinct().Count();
        MonthlyAverage = DaysWorked == 0 ? 0 : Math.Round(MonthlyHours / DaysWorked, 2);
        var expected = WeeklyGoal / 5 * DateTime.DaysInMonth(SelectedDate.Year, SelectedDate.Month);
        var overtime = MonthlyHours - expected;
        OvertimeStatus = overtime >= 0 ? $"Overtime +{overtime:0.0}h" : $"Remaining {Math.Abs(overtime):0.0}h";
    }

    private async Task AddEntryAsync()
    {
        if (EntryEndTime <= EntryStartTime)
        {
            StatusMessage = "End time must be after start time.";
            return;
        }

        var entry = new WorkEntry
        {
            Date = SelectedDate,
            StartTime = SelectedDate.Date.Add(EntryStartTime.TimeOfDay),
            EndTime = SelectedDate.Date.Add(EntryEndTime.TimeOfDay),
            BreakMinutes = EntryBreakMinutes,
            Category = SelectedCategory,
            Notes = EntryNotes,
            IsCalendarException = SelectedDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday
        };

        await _dataService.SaveEntryAsync(entry);
        StatusMessage = "Entry saved";
        EntryNotes = string.Empty;
        await RefreshAsync();
    }

    private void UpdateCalculation()
    {
        var calculation = _calculationService.Calculate(SelectedDate.Date.Add(StartTime.TimeOfDay), TimeSpan.FromHours(StandardWorkHours), SelectedBreakDuration, _clock.Now);
        RecommendedEndTime = calculation.End.ToString("t");
        CalculationBreakdown = $"{calculation.WorkDuration.TotalHours:0.0} hours work + {calculation.BreakDuration.TotalHours:0.0} hour break";
        ProgressValue = calculation.ProgressPercentage;
        IsOvertime = calculation.IsOvertime;
        CountdownText = FormatCountdown(calculation.Countdown);
        if (NotificationsEnabled)
        {
            ScheduleNotification();
        }
    }

    private static string FormatCountdown(TimeSpan countdown)
    {
        var prefix = countdown >= TimeSpan.Zero ? "Leave in" : "Overtime";
        var span = countdown >= TimeSpan.Zero ? countdown : countdown.Negate();
        return $"{prefix} {span:hh\:mm}";
    }

    private void ScheduleNotification()
    {
        var target = SelectedDate.Date.Add(StartTime.TimeOfDay).AddHours(StandardWorkHours).Add(SelectedBreakDuration);
        _ = _notificationService.ScheduleReminderAsync(target, TimeSpan.FromMinutes(15), "Wrap up your workday");
    }

    private void SetStartToNow()
    {
        StartTime = SelectedDate.Date.Add(_clock.Now.TimeOfDay);
        EntryStartTime = StartTime;
        EntryEndTime = StartTime.AddHours(9);
    }

    private void SetStandardDay()
    {
        StartTime = SelectedDate.Date.AddHours(8);
        EntryStartTime = StartTime;
        EntryEndTime = StartTime.AddHours(9);
        SelectedBreakOption = BreakOptions.First(o => Math.Abs(o.Duration.TotalMinutes - 60) < 1);
    }

    private void SetFlexible()
    {
        StartTime = StartTime.AddMinutes(30);
        EntryStartTime = StartTime;
        EntryEndTime = StartTime.AddHours(8.5);
    }

    private void ApplyTemplate(string? template)
    {
        if (template is null)
        {
            return;
        }

        switch (template)
        {
            case "Full":
                EntryBreakMinutes = 60;
                EntryEndTime = EntryStartTime.AddHours(9);
                break;
            case "Half":
                EntryBreakMinutes = 30;
                EntryEndTime = EntryStartTime.AddHours(4.5);
                break;
            case "Short":
                EntryBreakMinutes = 30;
                EntryEndTime = EntryStartTime.AddHours(6.5);
                break;
            default:
                break;
        }
    }

    public record BreakOptionModel(string Label, TimeSpan Duration, bool IsCustom);
}
