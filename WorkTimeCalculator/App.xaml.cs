using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorkTimeCalculator.Data;
using WorkTimeCalculator.Services;
using WorkTimeCalculator.ViewModels;

namespace WorkTimeCalculator;

public partial class App : Application
{
    private IHost? _host;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, configuration) =>
            {
                configuration.AddJsonFile("appsettings.json", optional: true);
            })
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton<IClock, SystemClock>();
                services.AddDbContextFactory<WorkTimeDbContext>(options =>
                    options.UseInMemoryDatabase("WorkTimeCalculator"));

                services.AddSingleton<IDataSeeder, WorkTimeDataSeeder>();
                services.AddSingleton<ICalculationService, CalculationService>();
                services.AddSingleton<INotificationService, NotificationService>();
                services.AddSingleton<IExportService, ExportService>();
                services.AddScoped<IDataService, EfDataService>();

                services.AddSingleton<TodayViewModel>();
                services.AddSingleton<HistoryViewModel>();
                services.AddSingleton<AnalyticsViewModel>();
                services.AddSingleton<SettingsViewModel>();
                services.AddSingleton<MainViewModel>();

                services.AddSingleton<MainWindow>();
            })
            .Build();

        _host.Start();

        using var scope = _host.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        seeder.SeedAsync().GetAwaiter().GetResult();

        var window = _host.Services.GetRequiredService<MainWindow>();
        window.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync(TimeSpan.FromSeconds(5));
            _host.Dispose();
        }

        base.OnExit(e);
    }
}
