using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkTimeCalculator.Models;

/// <summary>
/// Represents a single tracked work period with optional metadata.
/// </summary>
public class WorkEntry
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Duration in minutes for breaks that occurred within the entry.
    /// </summary>
    [Range(0, 720)]
    public int BreakMinutes { get; set; }

    [MaxLength(64)]
    public string Category { get; set; } = "Focus";

    [MaxLength(256)]
    public string? Notes { get; set; }

    /// <summary>
    /// Optional flag to identify weekend or holiday entries.
    /// </summary>
    public bool IsCalendarException { get; set; }

    [NotMapped]
    public TimeSpan Duration => EndTime - StartTime - TimeSpan.FromMinutes(BreakMinutes);
}
