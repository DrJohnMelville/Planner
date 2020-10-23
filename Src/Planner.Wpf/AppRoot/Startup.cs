using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Windows;
using CefSharp;
using Melville.IOC.IocContainers;
using Melville.IOC.IocContainers.ActivationStrategies.TypeActivation;
using Melville.MVVM.Wpf.Clipboards;
using Melville.MVVM.Wpf.MouseDragging;
using Melville.MVVM.Wpf.RootWindows;
using Melville.MVVM.Wpf.ViewFrames;
using Melville.WpfAppFramework.StartupBases;
using Microsoft.Extensions.Configuration;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Planner.Models.Blobs;
using Planner.Models.HtmlGeneration;
using Planner.Models.Repositories;
using Planner.Wpf.Notes;
using Planner.WpfViewModels.Logins;
using Planner.WpfViewModels.Notes;
using Planner.WpfViewModels.Notes.Pasters;
using Planner.WpfViewModels.PlannerPages;

namespace Planner.Wpf.AppRoot
{
    public class Startup: StartupBase
    {
        
        [STAThread]
        public static int Main(string[] commandLineArgs)
        {
            CefSharpRegistration.Initialize();
            ApplicationRootImplementation.Run(new Startup());
            return 0;
        }

        public override IIocService Create() => base.Create().CreateScope();

        protected override void RegisterWithIocContainer(IBindableIocService service)
        {
            service.AddLogging();
            RegisterMainWindowWithView(service);
            RegisterNodaTimeClock(service);
            SetupConfiguration(service);
            SetupJsonSerialization(service);
            RegisterRepositories(service);
            RegisterNoteServer(service);
            RegisterMarkdownPasters(service);
        }

        private void RegisterMarkdownPasters(IBindableIocService service)
        {
            service.Bind<IReadFromClipboard>().To<ReadFromClipboard>().AsSingleton();
            service.Bind<IMarkdownPaster>().To<CsvPaster>();
            service.Bind<IMarkdownPaster>().To<PngMarkdownPaster>();
            service.Bind<IMarkdownPaster>().To<HtmlMarkdownPaster>();
            service.Bind<IMarkdownPaster>().To<StringPaster>().WithParameters(DataFormats.UnicodeText);
            service.Bind<IMarkdownPaster>().To<CompositeMarkdownPaster>()
                .BlockSelfInjection().AsSingleton();
        }

        private void RegisterNoteServer(IBindableIocService service)
        {
            service.BindGeneric(typeof(IEventBroadcast<>), typeof(EventBroadcast<>),
                i=>i.AsSingleton());
            service.Bind<IHtmlContentOption>().To<StaticFiles>();
            service.Bind<IHtmlContentOption>().To<BlobContentOption>();
            service.Bind<IHtmlContentOption>().To<DailyJournalPageContent>();
            
            service.Bind<INotesServer>().To<NotesServer>().FixResult(i=>i.Launch()).AsSingleton();
            service.Bind<IRequestHandler>().To<WebNavigationRouter>();
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
            service.BindGeneric(typeof(ICachedRepositorySource<>),typeof(LocalToRemoteRepositoryBridge<>));
            service.BindGeneric(typeof(ILocalRepository<>), typeof(CachedRepository<>), 
                i=>i.AsSingleton());
        }
        
        private static void RegisterNodaTimeClock(IBindableIocService service)
        {
            service.Bind<IClock>().ToConstant(SystemClock.Instance);
        }

        private static void RegisterMainWindowWithView(IBindableIocService service)
        {
            service.Bind<INavigationHistory>().To<PlannerPageNavigationHistory>().AsScoped();
            service.Bind<IViewMappingConvention>().To<MapViewsToOwnAssembly>().AsSingleton();
            service.RegisterHomeViewModel<LoginViewModel>();
            service.Bind<INavigationWindow>().To<NavigationWindow>().AsScoped();
            service.Bind<IPlannerNavigator>().To<PlannerNavigator>().AsScoped();
            service.Bind<IPlannerNavigator>().To<NewWindowPlannerNavigator>()
                .AsScoped().BlockSelfInjection();
            service.Bind<IRootNavigationWindow>()
                .And<Window>()
                .To<RootNavigationWindow>()
                .AsScoped();
            service.Bind<Func<(IRootNavigationWindow, IPlannerNavigator)>>().ToMethod(ioc => () =>
            {
                var scope  = GetRootService(ioc).CreateScope();
                return (scope.Get<IRootNavigationWindow>(), scope.Get<IPlannerNavigator>());
            });
        }

        private static IIocService GetRootService(IIocService ioc)
        {
            var root = ioc;
            while (root.ParentScope != null && root.ParentScope != root) root = root.ParentScope;
            return root;
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