using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Melville.INPC;
using Melville.MVVM.WaitingServices;
using Melville.MVVM.Wpf.DiParameterSources;
using Melville.MVVM.Wpf.KeyboardFacade;
using Melville.MVVM.Wpf.ViewFrames;
using Planner.CommonmUI.RepositoryMapping;
using Planner.Models.Login;
using Planner.Models.Time;
using Planner.Wpf.PlannerPages;

namespace Planner.Wpf.Logins;

[OnDisplayed(nameof(TryAutoLogin))]
public partial class LoginViewModel
{
    [FromConstructor]public IList<TargetSite> Sites { get; }
    [FromConstructor] private readonly IPlannerNavigator navigator;
        
    public  Task TryAutoLogin(
        IWaitingService wait,
        [FromServices] IRegisterRepositorySource registry,
        [FromServices] IUsersClock clock, 
        [FromServices] IKeyboardQuery keyboardQuery)
    {
#if DEBUG
        return Task.CompletedTask;
#else
            if ((keyboardQuery.Modifiers & ModifierKeys.Control) != 0 ||Sites.Count == 0) return Task.CompletedTask;
            return LogIn(wait, Sites[0], registry, clock);
#endif
    }

    public async Task LogIn(
        IWaitingService wait,
        TargetSite currentSite, 
        [FromServices]IRegisterRepositorySource registry,
        [FromServices] IUsersClock clock)
    {
        using (wait.WaitBlock("Logging In"))
        {
            try
            {
                if (!await registry.LoginTo(currentSite))
                {
                    wait.ErrorMessage = "Login failed";
                    return;
                }

                AfterLoginStartup(clock);
            }
            catch (Exception e)
            {
                wait.ErrorMessage = e.Message;
            }
        }
    }

    private void AfterLoginStartup(IUsersClock clock) => 
        navigator.ToDate(clock.CurrentDate());
}