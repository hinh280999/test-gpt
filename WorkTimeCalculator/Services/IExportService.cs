using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using WorkTimeCalculator.Models;

namespace WorkTimeCalculator.Services;

public interface IExportService
{
    Task<string> ExportAsync(IEnumerable<WorkEntry> entries, string directory);
}

public class ExportService : IExportService
{
    public async Task<string> ExportAsync(IEnumerable<WorkEntry> entries, string directory)
    {
        Directory.CreateDirectory(directory);
        var filePath = Path.Combine(directory, $"worktime-export-{DateTime.Now:yyyyMMddHHmmss}.csv");
        await using var writer = new StreamWriter(filePath, false, Encoding.UTF8);
        await using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));
        await csv.WriteRecordsAsync(entries.Select(e => new ExportRow(e)));
        return filePath;
    }

    private record ExportRow
    {
        public ExportRow(WorkEntry entry)
        {
            Date = entry.Date.ToShortDateString();
            Start = entry.StartTime.ToShortTimeString();
            End = entry.EndTime.ToShortTimeString();
            BreakMinutes = entry.BreakMinutes;
            Category = entry.Category;
            Notes = entry.Notes ?? string.Empty;
            TotalHours = Math.Round(entry.Duration.TotalHours, 2);
        }

        public string Date { get; }
        public string Start { get; }
        public string End { get; }
        public int BreakMinutes { get; }
        public string Category { get; }
        public string Notes { get; }
        public double TotalHours { get; }
    }
}
