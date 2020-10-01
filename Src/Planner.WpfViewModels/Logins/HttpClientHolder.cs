using System;
using System.Net.Http;
using System.Threading.Tasks;
using TokenServiceClient.Native;
using TokenServiceClient.Native.PersistentToken;

namespace Planner.WpfViewModels.Logins
{
    public class HttpClientHolder
    {
        private HttpClient? client;

        public HttpClient GetClient() => client ??
                                         throw new InvalidOperationException("Cannot get client before loggingn in");
        public async Task<bool> Login(TargetSite site)
        {
            var loginAttempt = CapWebTokenFactory.CreateCapWebClient(site.Name, site.Secret);
            if (!await loginAttempt.LoginAsync()) return false;
            client = loginAttempt.AuthenticatedClient();
            client.BaseAddress = new Uri(site.Url);
            return true;
        }
    }
}