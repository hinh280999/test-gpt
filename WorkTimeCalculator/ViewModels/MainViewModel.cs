using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorkTimeCalculator.Services;

namespace WorkTimeCalculator.ViewModels;

public partial class MainViewModel : BaseViewModel
{
    private readonly TodayViewModel _todayViewModel;
    private readonly HistoryViewModel _historyViewModel;
    private readonly AnalyticsViewModel _analyticsViewModel;
    private readonly SettingsViewModel _settingsViewModel;
    private readonly IExportService _exportService;
    private readonly IDataService _dataService;

    public MainViewModel(TodayViewModel todayViewModel, HistoryViewModel historyViewModel, AnalyticsViewModel analyticsViewModel, SettingsViewModel settingsViewModel, IExportService exportService, IDataService dataService)
    {
        _todayViewModel = todayViewModel;
        _historyViewModel = historyViewModel;
        _analyticsViewModel = analyticsViewModel;
        _settingsViewModel = settingsViewModel;
        _exportService = exportService;
        _dataService = dataService;

        Tabs = new ObservableCollection<NavigationTabViewModel>
        {
            new("Today", "CalendarToday", _todayViewModel),
            new("History", "History", _historyViewModel),
            new("Analytics", "ChartTimelineVariant", _analyticsViewModel),
            new("Settings", "Cog", _settingsViewModel)
        };

        SelectedTab = Tabs.First();

        ExportCommand = new AsyncRelayCommand(ExportAsync);
        NavigateCommand = new RelayCommand<string>(NavigateTo);
    }

    public ObservableCollection<NavigationTabViewModel> Tabs { get; }

    [ObservableProperty]
    private NavigationTabViewModel _selectedTab;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _statusMessage;

    public IAsyncRelayCommand ExportCommand { get; }
    public IRelayCommand<string> NavigateCommand { get; }

    private async Task ExportAsync()
    {
        IsBusy = true;
        try
        {
            var entries = await _dataService.GetRecentEntriesAsync(365);
            var path = await _exportService.ExportAsync(entries, Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            StatusMessage = $"Exported to {path}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void NavigateTo(string? tabTitle)
    {
        if (tabTitle is null)
        {
            return;
        }

        var target = Tabs.FirstOrDefault(t => string.Equals(t.Title, tabTitle, StringComparison.OrdinalIgnoreCase));
        if (target is not null)
        {
            SelectedTab = target;
        }
    }
}
