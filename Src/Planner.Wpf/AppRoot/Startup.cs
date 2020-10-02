using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Windows;
using Melville.IOC.IocContainers;
using Melville.IOC.IocContainers.ActivationStrategies.TypeActivation;
using Melville.MVVM.Wpf.RootWindows;
using Melville.MVVM.Wpf.ViewFrames;
using Melville.WpfAppFramework.StartupBases;
using Microsoft.Extensions.Configuration;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Planner.Models.Repositories;
using Planner.Models.Tasks;
using Planner.WpfViewModels.Logins;

namespace Planner.Wpf.AppRoot
{
    public class Startup: StartupBase
    {
        [STAThread]
        public static int Main(string[] commandLineArgs)
        {
            ApplicationRootImplementation.Run(new Startup());
            return 0;
        }
        protected override void RegisterWithIocContainer(IBindableIocService service)
        {
            service.AddLogging();
            RegisterMainWindowWithView(service);
            RegisterNodaTimeClock(service);
            SetupConfiguration(service);
            SetupJsonSerialization(service);
            RegisterRepositories(service);
        }

        private void SetupJsonSerialization(IBindableIocService service)
        {
            var options = new JsonSerializerOptions();
            options.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            options.IgnoreReadOnlyProperties = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            service.Bind<JsonSerializerOptions>().ToConstant(options);
        }

        private static void RegisterRepositories(IBindableIocService service)
        {
            service.Bind<IRegisterRepositorySource>().To<RegisterRepositorySource>();
            service.Bind<ILocalRepository<PlannerTask>>().To<PlannerTaskLocalToRemoteRepositoryBridge>();
            service.BindGeneric(typeof(ILocalRepository<>), typeof(CachedRepository<>),
                o => o.When(br => br.TypeBeingConstructed != br.DesiredType).AsSingleton());
        }

        private static void RegisterNodaTimeClock(IBindableIocService service)
        {
            service.Bind<IClock>().ToConstant(SystemClock.Instance);
        }

        private static void RegisterMainWindowWithView(IBindableIocService service)
        {
            service.Bind<IViewMappingConvention>().To<MapViewsToOwnAssembly>().AsSingleton();
            service.RegisterHomeViewModel<LoginViewModel>();
            service.Bind<IRootNavigationWindow>()
                .And<Window>()
                .To<RootNavigationWindow>()
                .AsSingleton();
        }
        
        private static void SetupConfiguration(IBindableIocService service)
        {
            service.AddConfigurationSources(i => i.AddUserSecrets<Startup>());
            service.Bind<IList<TargetSite>>().To<List<TargetSite>>(ConstructorSelectors.DefaultConstructor)
                .InitializeFromConfiguration("Sites")
                .AsSingleton();
        }
    }
}