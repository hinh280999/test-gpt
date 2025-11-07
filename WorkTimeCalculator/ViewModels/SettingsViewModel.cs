using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WorkTimeCalculator.Models;

namespace WorkTimeCalculator.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly Action<LunchSettings> _saveCallback;
    private readonly Action _cancelCallback;

    [ObservableProperty]
    private DateTime? _lunchStart;

    [ObservableProperty]
    private DateTime? _lunchEnd;

    [ObservableProperty]
    private bool _isDarkTheme;

    [ObservableProperty]
    private string _validationMessage = string.Empty;

    public SettingsViewModel(LunchSettings source, Action<LunchSettings> saveCallback, Action cancelCallback)
    {
        _saveCallback = saveCallback;
        _cancelCallback = cancelCallback;
        SetSource(source);

        SaveCommand = new RelayCommand(Save);
        CancelCommand = new RelayCommand(Cancel);
    }

    public RelayCommand SaveCommand { get; }
    public RelayCommand CancelCommand { get; }

    public void SetSource(LunchSettings source)
    {
        LunchStart = DateTime.Today.Add(source.Start);
        LunchEnd = DateTime.Today.Add(source.End);
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

        ValidationMessage = string.Empty;
        _saveCallback(new LunchSettings(LunchStart.Value.TimeOfDay, LunchEnd.Value.TimeOfDay));
    }

    private void Cancel()
    {
        ValidationMessage = string.Empty;
        _cancelCallback();
    }
}
