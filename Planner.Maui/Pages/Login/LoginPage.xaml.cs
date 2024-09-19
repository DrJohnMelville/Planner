using System.Windows.Input;
using Melville.INPC;
using Planner.Models.Login;

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

public partial class LoginPageViewModel(IList<TargetSite> sites)
{
    public IList<TargetSite> Sites { get; } = sites;
    private INavigation navigation;
    public void SetNavigation(INavigation navigation) => this.navigation = navigation;


    public void TryAutoLogin()
    {
        ;
    } 
}