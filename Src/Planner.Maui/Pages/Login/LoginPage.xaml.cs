using System.Windows.Input;
using Microsoft.Maui.Layouts;

namespace Planner.Maui.Pages.Login;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginPageViewModel vm)
    {
        BindingContext = vm;
        InitializeComponent();
        vm.SetNavigation(Navigation);
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        ((LoginPageViewModel)BindingContext).TryAutoLogin();
    }
}

