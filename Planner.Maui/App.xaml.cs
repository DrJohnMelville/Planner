using Planner.Maui.Pages.Login;

namespace Planner.Maui;

public partial class App : Application, IAfterLoginOperation
{
    public App()
    {
        InitializeComponent();

        MainPage = new LoginPage(new LoginPageViewModel(this));
    }

    public void Do()
    {
        MainPage = new AppShell();
    }
}
