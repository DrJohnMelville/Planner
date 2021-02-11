using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using Melville.IOC.IocContainers;
using Melville.IOC.IocContainers.ActivationStrategies.TypeActivation;
using Melville.MVVM.Asyncs;
using Melville.MVVM.Wpf.Clipboards;
using Melville.MVVM.Wpf.RootWindows;
using Melville.WpfAppFramework.StartupBases;
using Microsoft.Extensions.Configuration;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using Planner.Models.Appointments;
using Planner.Models.Appointments.SyncStructure;
using Planner.Models.Blobs;
using Planner.Models.HtmlGeneration;
using Planner.Models.Markdown;
using Planner.Models.Repositories;
using Planner.OutlookInterop;
using Planner.Wpf.Logins;
using Planner.Wpf.Notes;
using Planner.Wpf.Notes.Pasters;
using Planner.Wpf.PlannerPages;
using NoteCreator = Planner.Wpf.PlannerPages.NoteCreator;

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

        public override IIocService Create() => base.Create().CreateScope();

        protected override void RegisterWithIocContainer(IBindableIocService service)
        {
            service.AddLogging();
            RegisterReloadEvents(service);
            RegisterMainWindowWithView(service);
            RegisterNodaTimeClock(service);
            SetupConfiguration(service);
            SetupJsonSerialization(service);
            RegisterRepositories(service);
            RegisterNoteServer(service);
            RegisterMarkdownPasters(service);
            RegisterOutlookSync(service);
        }

        private void RegisterOutlookSync(IBindableIocService service)
        {
            service.Bind<IAppointmentSyncMonitor>().To<AppointmentSyncMonitor>().AsSingleton();
        }

        private void RegisterMarkdownPasters(IBindableIocService service)
        {
            service.Bind<IReadFromClipboard>().To<ReadFromClipboard>().AsSingleton();
            service.Bind<IMarkdownPaster>().To<CsvPaster>();
            service.Bind<IMarkdownPaster>().To<PngMarkdownPaster>();
            service.Bind<IMarkdownPaster>().To<FilePaster>();
            service.Bind<IMarkdownPaster>().To<StringPaster>().WithParameters(DataFormats.UnicodeText);
            service.Bind<IMarkdownPaster>().To<HtmlMarkdownPaster>();

            //if none of the text formats work, try for an image
            service.Bind<IMarkdownPaster>().To<ImageMarkdownPaster>();

            service.Bind<IMarkdownPaster>().To<CompositeMarkdownPaster>()
                .BlockSelfInjection().AsSingleton();
        }

        private void RegisterNoteServer(IBindableIocService service)
        {
            service.Bind<IEventBroadcast<NoteEditRequestEventArgs>>()
                .To<EventBroadcast<NoteEditRequestEventArgs>>().AsScoped();
            service.Bind<IEventBroadcast<PlannerNagivateRequestEventArgs>>()
                .To<EventBroadcast<PlannerNagivateRequestEventArgs>>().AsScoped();
            service.BindGeneric(typeof(IEventBroadcast<>), typeof(EventBroadcast<>),
                i=>i.AsSingleton());

            service.Bind<IMarkdownTranslator>().ToConstant(new MarkdownTranslator("/navToPage/","/Images/"));
            service.Bind<ITryNoteHtmlGenerator>().To<StaticFileGenerator>();
            service.Bind<ITryNoteHtmlGenerator>().To<BlobGenerator>();
            service.Bind<ITryNoteHtmlGenerator>().To<DailyJournalPageGenerator>();
            service.Bind<ITryNoteHtmlGenerator>().To<SearchResultPageGenerator>();
            service.Bind<ITryNoteHtmlGenerator>().To<DefaultTextGenerator>();
            
            service.Bind<INotesServer>().To<NotesServer>().FixResult(i=>i.Launch()).AsSingleton();
            service.Bind<ILinkRedirect>().To<RunNonPlannerUrlsInSystemBrowser>();
            service.Bind<ILinkRedirect>().To<EditNotification>();
            service.Bind<ILinkRedirect>().To<PlannerNavigateNotification>();
            service.Bind<ILinkRedirect>().To<OpenLocalFile>(); 
            service.Bind<ILinkRedirect>().To<CompositeLinkRedirect>().AsScoped().BlockSelfInjection();
        }

        private static void RegisterReloadEvents(IBindableIocService service)
        {
            service.Bind<IEventBroadcast<ClearCachesEventArgs>>()
                .To<EventBroadcast<ClearCachesEventArgs>>()
                .AsSingleton();
            service.Bind<IEventBroadcast<ClearCachesEventArgs>>()
                .To<DelayedEventBroadcast<ClearCachesEventArgs>>()
                .AsSingleton()
                .WhenConstructingType<ReloadingNavigator>();
        }

        private void SetupJsonSerialization(IBindableIocService service)
        {
            var options = new JsonSerializerOptions();
            options.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            options.IgnoreReadOnlyProperties = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.ReferenceHandler = ReferenceHandler.Preserve;
            service.Bind<JsonSerializerOptions>().ToConstant(options);
        }

        private static void RegisterRepositories(IBindableIocService service)
        {
            service.Bind<IRegisterRepositorySource>().To<RegisterRepositorySource>();
            service.BindGeneric(typeof(ICachedRepositorySource<>),typeof(LocalToRemoteRepositoryBridge<>));
            service.Bind<ILocalRepository<Appointment>>().To<AppointmentCachedRepository>().AsSingleton();
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
            service.RegisterHomeViewModel<LoginViewModel>();
            service.Bind<INavigationWindow>().To<NavigationWindow>().AsScoped();
            service.Bind<IPlannerNavigator>().To<NewWindowPlannerNavigator>()
                .AsScoped().BlockSelfInjection();
            service.Bind<IPlannerNavigator>().To<ReloadingNavigator>().AsScoped()
                .WhenConstructingType<NewWindowPlannerNavigator>();
            service.Bind<IPlannerNavigator>().To<PlannerNavigator>().AsScoped()
                .WhenConstructingType<ReloadingNavigator>();
            service.Bind<IRootNavigationWindow>()
                .And<Window>()
                .To<RootNavigationWindow>()
                .FixResult(ConfigRootWindow)
                .AsScoped();
            service.Bind<NoteCreator>().ToSelf().AsScoped();
            service.Bind<Func<(IRootNavigationWindow, IPlannerNavigator)>>().ToMethod(ioc => () =>
            {
                var scope  = GetRootService(ioc).CreateScope();
                return (scope.Get<IRootNavigationWindow>(), scope.Get<IPlannerNavigator>());
            });
            service.Bind<IStaWorker>().To<DispatcherStaWorker>();
        }

        private static void ConfigRootWindow(IRootNavigationWindow window)
        {
            window.SetWindowIconFromResource("Planner.WPF", "AppRoot/App.ico");
            if (window is RootNavigationWindow rnw) rnw.Title = "John Melville's Planner";
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
            service.Bind<IList<OutlookConnectionConfig>>()
                .To<List<OutlookConnectionConfig>>(ConstructorSelectors.DefaultConstructor)
                .InitializeFromConfiguration("Outlook")
                .AsSingleton();
            service.Bind<IList<TargetSite>>().To<List<TargetSite>>(ConstructorSelectors.DefaultConstructor)
                .InitializeFromConfiguration("Sites")
                .AsSingleton();
        }
    }
}