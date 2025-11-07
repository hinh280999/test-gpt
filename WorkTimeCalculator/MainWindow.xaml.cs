using System.Windows;
using System.Windows.Input;

namespace WorkTimeCalculator;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += (_, _) => MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
    }
}
