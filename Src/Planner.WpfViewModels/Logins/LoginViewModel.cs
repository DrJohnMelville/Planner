using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows.Input;
using Melville.MVVM.WaitingServices;
using Melville.MVVM.Wpf.DiParameterSources;
using Melville.MVVM.Wpf.KeyboardFacade;
using Melville.MVVM.Wpf.RootWindows;
using Melville.MVVM.Wpf.ViewFrames;
using NodaTime;
using Planner.Models.Time;
using Planner.WpfViewModels.PlannerPages;
using Serilog;
using TokenServiceClient.Native;
using TokenServiceClient.Native.PersistentToken;

namespace Planner.WpfViewModels.Logins
{
    public interface IRegisterRepositorySource
    {
        void UseWebSource(HttpClient authenticatedClient);
        void UseLocalTestSource();
    }
    [OnDisplayed(nameof(TryAutoLogin))]
    public partial class LoginViewModel
    {
        public IList<TargetSite> Sites { get; }
        private readonly IPlannerNavigator navigator;

        public LoginViewModel(IList<TargetSite> sites, IPlannerNavigator navigator)
        {
            Sites = sites;
            this.navigator = navigator;
        }

        public void FakeDb(
            [FromServices] IRegisterRepositorySource registry, 
            [FromServices] IUsersClock clock)
        {
          registry.UseLocalTestSource();
          navigator.ToDate(clock.CurrentDate());
        }

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
                    if (!await ConnectToWebRepository(currentSite, registry))
                    {
                        wait.ErrorMessage = "Login failed";
                        return;
                    }
                    navigator.ToDate(clock.CurrentDate());
                }
                catch (Exception e)
                {
                    wait.ErrorMessage = e.Message;
                }
            }
        }
        
        public async Task<bool> ConnectToWebRepository(TargetSite site, IRegisterRepositorySource register)
        {
            var loginAttempt = CapWebTokenFactory.CreateCapWebClient(site.Name, site.Secret);
            if (!await loginAttempt.LoginAsync()) return false;
            register.UseWebSource(CreateDestinationClient(loginAttempt, site.Url));
            return true;
        }

        private static HttpClient CreateDestinationClient(IPersistentAccessToken loginAttempt, string targetUrl)
        {
            var client = loginAttempt.AuthenticatedClient();
            client.BaseAddress = new Uri(targetUrl);
            return client;
        }
    }
}