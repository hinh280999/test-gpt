using System;
using System.ComponentModel.DataAnnotations;

namespace WorkTimeCalculator.Models;

/// <summary>
/// Stores configurable preferences for the application.
/// </summary>
public class ApplicationSettings
{
    [Range(1, 24)]
    public double StandardHoursPerDay { get; set; } = 8.0;

    [Range(1, 7)]
    public int WorkDaysPerWeek { get; set; } = 5;

    public TimeSpan DefaultStartTime { get; set; } = new(8, 0, 0);

    public TimeSpan DefaultBreakDuration { get; set; } = new(1, 0, 0);

    public bool EnableNotifications { get; set; } = true;

    public TimeSpan ReminderLeadTime { get; set; } = TimeSpan.FromMinutes(15);

    public bool DesktopNotifications { get; set; } = true;

    public bool SoundAlerts { get; set; } = false;

    public string Theme { get; set; } = "Light";

    public string AccentColor { get; set; } = "#2196F3";

    [Range(0.8, 1.5)]
    public double FontScale { get; set; } = 1.0;
}
