using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Melville.Hacks;

namespace Planner.Wpf.PlannerPages
{
    public class DispatcherStaWorker:IStaWorker
    {
        private readonly Dispatcher dispatcher;

        // this has to take a Window and not a RootNavigationWindow because the IOC is setup to only
        // have one window per scope Can have any number of RootNavigationWindows.
        public DispatcherStaWorker(Window rootWin)
        {
            dispatcher = rootWin.Dispatcher;
        }

        public async Task<T> Run<T>(Func<Task<T>> action)
        {
            return await await dispatcher.InvokeAsync(action).Task;
        }
    }
}