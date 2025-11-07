namespace WorkTimeCalculator.ViewModels;

public class NavigationTabViewModel : BaseViewModel
{
    public NavigationTabViewModel(string title, string icon, BaseViewModel content)
    {
        Title = title;
        Icon = icon;
        Content = content;
    }

    public string Title { get; }
    public string Icon { get; }
    public BaseViewModel Content { get; }
}
