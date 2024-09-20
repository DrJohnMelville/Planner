using System.Windows.Input;
using Planner.Models.Login;

namespace Planner.Maui.Pages.Login;

public partial class LoginPageViewModel(IList<TargetSite> sites)
{
    public IList<TargetSite> Sites { get; } = sites;
    private INavigation? navigation;
    public void SetNavigation(INavigation navigation) => this.navigation = navigation;


    public void TryAutoLogin()
    {
    }

    private ICommand? loginCommand;
    public ICommand LoginCommand => loginCommand ??= new Command<TargetSite>(Login);
    public void Login(TargetSite site)
    {
        ;
    }
}