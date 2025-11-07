using System;
using WorkTimeCalculator.Models;

namespace WorkTimeCalculator.ViewModels;

public class CalendarDayViewModel : BaseViewModel
{
    public CalendarDayViewModel(DateTime date, double totalHours, double goalHours)
    {
        Date = date;
        TotalHours = totalHours;
        GoalHours = goalHours;
    }

    public DateTime Date { get; }
    public double TotalHours { get; }
    public double GoalHours { get; }

    public bool IsWeekend => Date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    public bool IsToday => Date.Date == DateTime.Today;
    public bool IsOvertime => TotalHours > GoalHours;
    public bool HasHours => TotalHours > 0.01;
}
