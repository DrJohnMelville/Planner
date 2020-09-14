using System;
using System.Windows;
using Melville.IOC.IocContainers;
using Melville.MVVM.Wpf.RootWindows;
using Melville.WpfAppFramework.StartupBases;
using Planner.Wpf.TaskList;

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
            service.RegisterHomeViewModel<DailyTaskListViewModel>();
            service.Bind<IRootNavigationWindow>()
                .And<Window>()
                .To<RootNavigationWindow>()
                .AsSingleton();
        }
    }
}