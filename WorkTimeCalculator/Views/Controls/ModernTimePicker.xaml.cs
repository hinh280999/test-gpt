using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WorkTimeCalculator.Views.Controls;

public partial class ModernTimePicker : UserControl
{
    public static readonly DependencyProperty TimeProperty = DependencyProperty.Register(
        nameof(Time), typeof(DateTime), typeof(ModernTimePicker),
        new FrameworkPropertyMetadata(DateTime.Today, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTimeChanged));

    public ModernTimePicker()
    {
        InitializeComponent();
        Hours = new ObservableCollection<int>(Enumerable.Range(1, 12));
        Minutes = new ObservableCollection<int>(Enumerable.Range(0, 4).Select(i => i * 15));
    }

    public ObservableCollection<int> Hours { get; }

    public ObservableCollection<int> Minutes { get; }

    public DateTime Time
    {
        get => (DateTime)GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }

    public int SelectedHour
    {
        get => (int)GetValue(SelectedHourProperty);
        set => SetValue(SelectedHourProperty, value);
    }

    public static readonly DependencyProperty SelectedHourProperty = DependencyProperty.Register(
        nameof(SelectedHour), typeof(int), typeof(ModernTimePicker),
        new PropertyMetadata(8, OnPartChanged));

    public int SelectedMinute
    {
        get => (int)GetValue(SelectedMinuteProperty);
        set => SetValue(SelectedMinuteProperty, value);
    }

    public static readonly DependencyProperty SelectedMinuteProperty = DependencyProperty.Register(
        nameof(SelectedMinute), typeof(int), typeof(ModernTimePicker),
        new PropertyMetadata(0, OnPartChanged));

    public bool IsPm
    {
        get => (bool)GetValue(IsPmProperty);
        set => SetValue(IsPmProperty, value);
    }

    public static readonly DependencyProperty IsPmProperty = DependencyProperty.Register(
        nameof(IsPm), typeof(bool), typeof(ModernTimePicker),
        new PropertyMetadata(false, OnPartChanged));

    private static void OnTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ModernTimePicker picker)
        {
            return;
        }

        var time = (DateTime)e.NewValue;
        var hour = time.Hour;
        picker.IsPm = hour >= 12;
        var normalizedHour = hour % 12;
        picker.SelectedHour = normalizedHour == 0 ? 12 : normalizedHour;
        picker.SelectedMinute = time.Minute - time.Minute % 15;
    }

    private static void OnPartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not ModernTimePicker picker)
        {
            return;
        }

        var hour = picker.SelectedHour % 12;
        if (picker.IsPm)
        {
            hour += 12;
        }

        var newTime = picker.Time.Date.AddHours(hour).AddMinutes(picker.SelectedMinute);
        if (picker.Time.TimeOfDay != newTime.TimeOfDay)
        {
            picker.Time = newTime;
        }
    }
}
