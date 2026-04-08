using System;
using Melville.IOC.AspNet.RegisterFromServiceCollection;
using Melville.IOC.IocContainers;
using Melville.IOC.TypeResolutionPolicy;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Planner.Web.CompositionRoot;
using Serilog;

namespace Planner.Web;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("In Main 3");
        var build = CreateHostBuilder(args).Build();
        Console.WriteLine("HostBuilder Built");
        build.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(
                new MelvilleServiceProviderFactory(true,
                PatchRegistryPolicyResolverWslBug))
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });


    private static void PatchRegistryPolicyResolverWslBug(IBindableIocService ioc)
    {
        // In WSL the IOC engine eagerly finds a concrete RegistryPolicyResolver which
        // is not registered in the IOC container.  The IOC infers this mapping which is
        // incorrect.  Here we refuse that mapping by explixitly mapping it to null.
        var bannedType = typeof(DataProtectionUtilityExtensions).Assembly
            .GetType("Microsoft.AspNetCore.DataProtection.IRegistryPolicyResolver")!;
        ioc.ConfigurePolicy<IPickBindingTargetSource>().Bind(bannedType, BindingPriority.KeepNew).Prohibit();
    }
}