using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorkTimeCalculator.Models;

namespace WorkTimeCalculator.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly Action<LunchSettings, double, double> _saveCallback;
    private readonly Action _cancelCallback;

    [ObservableProperty]
    private DateTime? _lunchStart;

    [ObservableProperty]
    private DateTime? _lunchEnd;

    [ObservableProperty]
    private bool _isDarkTheme;

    [ObservableProperty]
    private string _validationMessage = string.Empty;

    [ObservableProperty]
    private double _workDurationHours = 8.0;

    [ObservableProperty]
    private double _selectedBreakMinutes = 90;

    public SettingsViewModel(LunchSettings source, double workDurationHours, double breakMinutes, Action<LunchSettings, double, double> saveCallback, Action cancelCallback)
    {
        _saveCallback = saveCallback;
        _cancelCallback = cancelCallback;
        SetSource(source, workDurationHours, breakMinutes);

        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
    }

    public RelayCommand SaveCommand { get; }
    public RelayCommand CancelCommand { get; }

    public void SetSource(LunchSettings source, double workDurationHours, double breakMinutes)
    {
        LunchStart = DateTime.Today.Add(source.Start);
        LunchEnd = DateTime.Today.Add(source.End);
        WorkDurationHours = workDurationHours;
        SelectedBreakMinutes = breakMinutes;
    }

    private void Save()
    {
        if (!LunchStart.HasValue || !LunchEnd.HasValue)
        {
            ValidationMessage = "Please provide both lunch start and end.";
            return;
        }

        if (LunchEnd <= LunchStart)
        {
            ValidationMessage = "Lunch end must be after lunch start.";
            return;
        }

        if (WorkDurationHours < 1 || WorkDurationHours > 12)
        {
            ValidationMessage = "Work duration must be between 1 and 12 hours.";
            return;
        }

        ValidationMessage = string.Empty;
        _saveCallback(new LunchSettings(LunchStart.Value.TimeOfDay, LunchEnd.Value.TimeOfDay), WorkDurationHours, SelectedBreakMinutes);
    }

    private void Cancel()
    {
        ValidationMessage = string.Empty;
        _cancelCallback();
    }
}
