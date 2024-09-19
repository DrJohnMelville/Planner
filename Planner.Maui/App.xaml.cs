using Melville.INPC;
using Planner.Maui.Pages.Login;

namespace Planner.Maui;

public partial class App : Application
{
    public App(Func<AppShell> shell)
    { 
        InitializeComponent();
        MainPage = shell();
    }

}
