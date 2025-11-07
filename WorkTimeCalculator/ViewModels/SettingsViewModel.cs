using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorkTimeCalculator.Models;
using WorkTimeCalculator.Services;

namespace WorkTimeCalculator.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    private readonly INotificationService _notificationService;
    private readonly IExportService _exportService;
    private readonly IDataService _dataService;

    [ObservableProperty]
    private ApplicationSettings _settings = new();

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string? _statusMessage;

    public SettingsViewModel(INotificationService notificationService, IExportService exportService, IDataService dataService)
    {
        _notificationService = notificationService;
        _exportService = exportService;
        _dataService = dataService;

        SaveCommand = new AsyncRelayCommand(SaveAsync);
        ExportAllCommand = new AsyncRelayCommand(ExportAllAsync);
        ClearHistoryCommand = new AsyncRelayCommand(ClearHistoryAsync);
    }

    public IAsyncRelayCommand SaveCommand { get; }
    public IAsyncRelayCommand ExportAllCommand { get; }
    public IAsyncRelayCommand ClearHistoryCommand { get; }

    private async Task SaveAsync()
    {
        IsBusy = true;
        try
        {
            ValidateSettings();
            if (Settings.EnableNotifications)
            {
                await _notificationService.CancelAllAsync();
            }

            StatusMessage = "Settings saved";
        }
        catch (ValidationException ex)
        {
            StatusMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ExportAllAsync()
    {
        IsBusy = true;
        try
        {
            var entries = await _dataService.GetRecentEntriesAsync(365);
            var path = await _exportService.ExportAsync(entries, Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            StatusMessage = $"Data exported to {path}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ClearHistoryAsync()
    {
        IsBusy = true;
        try
        {
            var entries = await _dataService.GetRecentEntriesAsync(3650);
            await _dataService.DeleteEntriesAsync(entries.Select(e => e.Id));
            StatusMessage = "History cleared";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ValidateSettings()
    {
        var context = new ValidationContext(Settings);
        Validator.ValidateObject(Settings, context, validateAllProperties: true);
    }
}
