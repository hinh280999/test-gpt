using CommunityToolkit.Mvvm.ComponentModel;
using WorkTimeCalculator.Models;

namespace WorkTimeCalculator.ViewModels;

public partial class HistoryEntryViewModel : BaseViewModel
{
    public HistoryEntryViewModel(WorkEntry entry)
    {
        Entry = entry;
    }

    public WorkEntry Entry { get; }

    [ObservableProperty]
    private bool _isSelected;

    public string DateDisplay => Entry.Date.ToString("d");
    public string StartDisplay => Entry.StartTime.ToString("t");
    public string EndDisplay => Entry.EndTime.ToString("t");
    public string BreakDisplay => $"{Entry.BreakMinutes} min";
    public string TotalDisplay => $"{Entry.Duration.TotalHours:0.##} h";
}
