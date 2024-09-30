using Melville.MVVM.Maui.Commands;
using Planner.Maui.Pages.Login;

namespace Planner.Maui;

public partial class AppShell
{
    public AppShell()
    {
        InitializeComponent();
        InheritedCommand.SetInheritedCommandParameter(this, Navigation);
    }
}
