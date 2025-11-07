using System;
using System.Windows;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using WorkTimeCalculator.ViewModels;

namespace WorkTimeCalculator;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
    }

    private void DialogHost_DialogOpened(object? sender, DialogOpenedEventArgs eventArgs)
    {
        // DataContext is set via binding in XAML
    }

    private void DialogHost_DialogClosing(object? sender, DialogClosingEventArgs eventArgs)
    {
        // Dialog is closing, no action needed
    }

    private void SetToday_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.StartDateText = DateTime.Today.ToString("dd/MM/yyyy");
        }
    }

    private void SetNow_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.StartDateText = DateTime.Today.ToString("dd/MM/yyyy");
            vm.StartTimeText = DateTime.Now.ToString("HH:mm");
        }
    }

    private void AddEntry_Click(object sender, RoutedEventArgs e)
    {
        // TODO: Open dialog to add new entry
        MessageBox.Show("Add entry functionality coming soon!", "Work Time Calculator", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MenuEntry_Click(object sender, RoutedEventArgs e)
    {
        // TODO: Show context menu for entry
    }

    private void EditEntry_Click(object sender, RoutedEventArgs e)
    {
        // TODO: Edit entry functionality
    }
}
