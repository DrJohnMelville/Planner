using System.Windows.Input;
using Melville.MVVM.Maui.Commands;
using Melville.MVVM.WaitingServices;
using Planner.CommonmUI.RepositoryMapping;
using Planner.Models.Login;

namespace Planner.Maui.Pages.Login;

public partial class LoginPageViewModel(IList<TargetSite> sites)
{
    public IList<TargetSite> Sites { get; } = sites;
    private INavigation? navigation;
    public void SetNavigation(INavigation navigation) => this.navigation = navigation;
    public event EventHandler<EventArgs>? LoginSuccessful; 


    public void TryAutoLogin()
    {
    }

    private ICommand? loginCommand;
    public ICommand LoginCommand => loginCommand ??= InheritedCommandFactory.Create(Login);
    public async Task Login(TargetSite site, IShowProgress waitService,
        [FromServices] IRegisterRepositorySource reguster)
    {
        using var wait = waitService.ShowProgress("Logging in...");
        if (await reguster.LoginTo(site))
        {
            await (navigation?.PopModalAsync() ?? Task.CompletedTask);
            LoginSuccessful?.Invoke(this, EventArgs.Empty);
        }
    }
}