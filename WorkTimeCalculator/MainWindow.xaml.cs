using System.Windows;
using System.Windows.Input;
using WorkTimeCalculator.ViewModels;

namespace WorkTimeCalculator;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += (_, _) => MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
    }
}
