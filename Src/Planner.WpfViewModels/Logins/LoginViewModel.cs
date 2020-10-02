﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Melville.INPC;
using Melville.MVVM.WaitingServices;
using Melville.MVVM.Wpf.DiParameterSources;
using Melville.MVVM.Wpf.RootWindows;
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

        public async Task FakeDb(
            [FromServices] IRegisterRepositorySource registry,
            INavigationWindow navigation,
            [FromServices] Func<DailyPlannerPageViewModel> factory)
        {
          registry.UseLocalTestSource();
          navigation.NavigateTo(factory());
        }
        public async Task LogIn(
            IWaitingService wait,
            TargetSite currentSite, 
            [FromServices]IRegisterRepositorySource registry,
            INavigationWindow navigation, 
            [FromServices]Func<DailyPlannerPageViewModel> factory)
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
                    navigation.NavigateTo(factory());
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