using Planner.Maui.Pages.Login;

namespace Planner.Maui;

public partial class AppShell
{
    public AppShell(LoginPage page)
    {
        InitializeComponent();
        Navigation.PushModalAsync(page);
    }
}
