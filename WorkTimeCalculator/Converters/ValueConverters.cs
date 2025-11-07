using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WorkTimeCalculator.Converters;

public class BreakTimeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double breakMinutes && parameter is string paramStr && double.TryParse(paramStr, out var paramMinutes))
        {
            return Math.Abs(breakMinutes - paramMinutes) < 0.1;
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isChecked && isChecked && parameter is string paramStr && double.TryParse(paramStr, out var minutes))
        {
            return minutes;
        }
        return Binding.DoNothing;
    }
}

public class CountToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int count)
        {
            var invert = parameter is string str && str == "True";
            var visible = count == 0;
            return (invert ? !visible : visible) ? Visibility.Visible : Visibility.Collapsed;
        }
        return Visibility.Collapsed;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class NullToVisibilityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value != null && !string.IsNullOrWhiteSpace(value.ToString()) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class ProgressToDashArrayConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double progress && progress > 0)
        {
            var circumference = 2 * Math.PI * 54; // radius = 54 (120/2 - 12)
            var dashLength = circumference * (progress / 100.0);
            return new DoubleCollection(new[] { dashLength, circumference });
        }
        return new DoubleCollection(new[] { 0.0, 1000.0 });
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class AverageHoursConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double monthlyHours)
        {
            var daysWorked = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            var average = daysWorked > 0 ? monthlyHours / daysWorked : 0;
            return $"Average: {average:F1}h/day";
        }
        return "Average: 0.0h/day";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class BreakDurationConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // This is a placeholder - would need actual break duration in model
        return "1.5h";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class HoursToStatusColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double hours)
        {
            return hours switch
            {
                >= 6 and < 8 => new SolidColorBrush(Color.FromRgb(76, 175, 80)), // Green
                >= 8 and < 10 => new SolidColorBrush(Color.FromRgb(255, 152, 0)), // Orange
                >= 10 => new SolidColorBrush(Color.FromRgb(244, 67, 54)), // Red
                _ => new SolidColorBrush(Color.FromRgb(158, 158, 158)) // Gray
            };
        }
        return new SolidColorBrush(Color.FromRgb(158, 158, 158));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class HoursToProgressWidthConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double hours && parameter is string maxStr && double.TryParse(maxStr, out var max))
        {
            var progress = Math.Min(100, (hours / max) * 100);
            return progress;
        }
        return 0.0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class StatusIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double hours)
        {
            return hours switch
            {
                >= 6 and < 8 => "CheckCircle", // On target
                >= 8 and < 10 => "Alert", // Overtime warning
                >= 10 => "LightningBolt", // Excessive
                _ => "ClockOutline" // Under target
            };
        }
        return "ClockOutline";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class DateToDayOfWeekConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime date)
        {
            return date.ToString("ddd", culture);
        }
        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

