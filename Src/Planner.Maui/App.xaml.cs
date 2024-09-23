using Melville.INPC;
using Melville.MVVM.Maui.WaitingService;
using Planner.Maui.Pages.Login;

namespace Planner.Maui;

public partial class App
{
    public App(Func<AppShell> shell)
    { 
        InitializeComponent();
        MainPage = shell();
    }

}
