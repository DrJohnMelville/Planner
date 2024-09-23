using Planner.Models.Login;
using TokenServiceClient.Native;
using TokenServiceClient.Native.PersistentToken;

namespace Planner.CommonmUI.RepositoryMapping;

public static class WebLoginAlgorithm 
{
    public static async ValueTask<bool> LoginTo(
        this IRegisterRepositorySource register, TargetSite site)
    {
        if (string.IsNullOrWhiteSpace(site.Url))
        {
            register.UseLocalTestSource();
        }
        else
        {
            var loginAttempt = CapWebTokenFactory.CreateCapWebClient(
                site.Name, "Anonymous");
            if (!await loginAttempt.LoginAsync()) return false;
            register.UseWebSource(CreateDestinationClient(loginAttempt, site.Url));
        }

        return true;
    }

    private static HttpClient CreateDestinationClient(IPersistentAccessToken loginAttempt, string targetUrl)
    {
        var client = loginAttempt.AuthenticatedClient();
        client.BaseAddress = new Uri(targetUrl);
        return client;
    }

}