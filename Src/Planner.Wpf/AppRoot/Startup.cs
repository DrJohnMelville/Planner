using System;
using System.Windows;
using Melville.IOC.IocContainers;
using Melville.MVVM.Wpf.RootWindows;
using Melville.MVVM.Wpf.ViewFrames;
using Melville.WpfAppFramework.StartupBases;
using NodaTime;
using Planner.Models.Repositories;
using Planner.Repository;
using Planner.WpfViewModels.PlannerPages;
using Planner.WpfViewModels.TaskList;

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
            
            //Temporary binding until dailyPlannerPage is implemented
            service.Bind<IPlannerTaskRepository>().To<TemporaryPTF>();
            service.Bind<IPlannerTaskRepository>().To<CachedTaskRepository>()
                .BlockSelfInjection().AsSingleton();
        }

        private static void RegisterNodaTimeClock(IBindableIocService service)
        {
            service.Bind<IClock>().ToConstant(SystemClock.Instance);
        }

        private static void RegisterMainWindowWithView(IBindableIocService service)
        {
            service.Bind<IViewMappingConvention>().To<MapViewsToOwnAssembly>().AsSingleton();
            service.RegisterHomeViewModel<DailyPlannerPageViewModel>();
            service.Bind<IRootNavigationWindow>()
                .And<Window>()
                .To<RootNavigationWindow>()
                .AsSingleton();
        }
    }
}