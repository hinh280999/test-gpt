using System;
using System.Windows;
using System.Windows.Threading;

namespace WorkTimeCalculator;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        // Handle unhandled exceptions
        DispatcherUnhandledException += App_DispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show($"An unhandled exception occurred:\n\n{e.Exception.Message}\n\nStack trace:\n{e.Exception.StackTrace}", 
            "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        e.Handled = true;
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            MessageBox.Show($"An unhandled exception occurred:\n\n{ex.Message}\n\nStack trace:\n{ex.StackTrace}", 
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
