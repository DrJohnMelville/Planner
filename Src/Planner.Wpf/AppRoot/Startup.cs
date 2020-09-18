using System;
using System.Windows;
using Melville.IOC.IocContainers;
using Melville.MVVM.Wpf.RootWindows;
using Melville.MVVM.Wpf.ViewFrames;
using Melville.WpfAppFramework.StartupBases;
using Planner.Models.Tasks;
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
            service.Bind<IViewMappingConvention>().To<MapViewsToOwnAssembly>().AsSingleton();
            service.RegisterHomeViewModel<DailyTaskListViewModel>();
            service.Bind<IRootNavigationWindow>()
                .And<Window>()
                .To<RootNavigationWindow>()
                .AsSingleton();
            
            //Temporary binding until dailyPlannerPage is implemented
            service.Bind<IPlannerTaskFactory>().To<TemporaryPTF>();
        }
    }
    
    public class TemporaryPTF : IPlannerTaskFactory
    {
        public PlannerTask Task(string title)
        {
            return new PlannerTask() {Name = title};
        }
    }
}