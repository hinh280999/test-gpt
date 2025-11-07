namespace WorkTimeCalculator.Models;

/// <summary>
/// Represents a reusable tag for categorising work entries.
/// </summary>
public class CategoryTag
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#2196F3";
    public string Icon { get; set; } = "Briefcase";
}
