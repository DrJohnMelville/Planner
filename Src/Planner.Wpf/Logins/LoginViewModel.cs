using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Melville.INPC;
using Melville.MVVM.WaitingServices;
using Melville.MVVM.Wpf.DiParameterSources;
using Melville.MVVM.Wpf.KeyboardFacade;
using Melville.MVVM.Wpf.ViewFrames;
using Planner.CommonmUI.RepositoryMapping;
using Planner.Models.Login;
using Planner.Models.Time;
using Planner.Wpf.PlannerPages;
using TokenServiceClient.Native;
using TokenServiceClient.Native.PersistentToken;

namespace Planner.Wpf.Logins
{
    [OnDisplayed(nameof(TryAutoLogin))]
    public partial class LoginViewModel
    {
        [FromConstructor]public IList<TargetSite> Sites { get; }
        [FromConstructor] private readonly IPlannerNavigator navigator;
        
        public void FakeDb(
            [FromServices] IRegisterRepositorySource registry, 
            [FromServices] IUsersClock clock)
        {
          registry.UseLocalTestSource();
          AfterLoginStartup(clock);
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

                    AfterLoginStartup(clock);
                }
                catch (Exception e)
                {
                    wait.ErrorMessage = e.Message;
                }
            }
        }

        public async Task<bool> ConnectToWebRepository(TargetSite site, IRegisterRepositorySource register)
        {
            var loginAttempt = CapWebTokenFactory.CreateCapWebClient(
                site.Name, "Anonymous");
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

        private void AfterLoginStartup(IUsersClock clock)
        {
            navigator.ToDate(clock.CurrentDate());
        }
    }
}