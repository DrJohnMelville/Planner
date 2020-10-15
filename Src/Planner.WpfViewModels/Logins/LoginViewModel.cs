using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Melville.MVVM.WaitingServices;
using Melville.MVVM.Wpf.DiParameterSources;
using Melville.MVVM.Wpf.RootWindows;
using NodaTime;
using Planner.Models.Time;
using Planner.WpfViewModels.PlannerPages;
using TokenServiceClient.Native;
using TokenServiceClient.Native.PersistentToken;

namespace Planner.WpfViewModels.Logins
{
    public interface IRegisterRepositorySource
    {
        void UseWebSource(HttpClient authenticatedClient);
        void UseLocalTestSource();
    }
    public partial class LoginViewModel
    {
        public IList<TargetSite> Sites { get; }

        public LoginViewModel(IList<TargetSite> sites)
        {
            Sites = sites;
        }

        public void FakeDb(
            [FromServices] IRegisterRepositorySource registry, 
            [FromServices] IPlannerNavigator nav,
            [FromServices] IClock clock)
        {
          registry.UseLocalTestSource();
          nav.ToDate(clock.CurrentDate());
        }
        public async Task LogIn(
            IWaitingService wait,
            TargetSite currentSite, 
            [FromServices]IRegisterRepositorySource registry,
            [FromServices] IPlannerNavigator nav,
            [FromServices] IClock clock)
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
                    nav.ToDate(clock.CurrentDate());
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