using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Melville.INPC;
using Melville.MVVM.WaitingServices;
using Melville.MVVM.Wpf.DiParameterSources;
using Melville.MVVM.Wpf.RootWindows;
using Planner.WpfViewModels.PlannerPages;

namespace Planner.WpfViewModels.Logins
{
    public partial class LoginViewModel
    {
        public IList<TargetSite> Sites { get; }
        [AutoNotify] private TargetSite? currentSite;

        public LoginViewModel(IList<TargetSite> sites)
        {
            Sites = sites;
            currentSite = sites.FirstOrDefault();
        }

        public async Task LogIn([FromServices]HttpClientHolder holder, IWaitingService wait,
            INavigationWindow navigation, [FromServices]Func<DailyPlannerPageViewModel> factory)
        {
            if (CurrentSite == null) return;
            using (wait.WaitBlock("Logging In"))
            {
                try
                {
                    if (!await holder.Login(CurrentSite))
                    {
                        wait.ErrorMessage = "Login failed";
                        return;
                    }
                    navigation.NavigateTo(factory());
                }
                catch (Exception e)
                {
                    wait.ErrorMessage = e.Message;
                }
            }
        }
    }
}