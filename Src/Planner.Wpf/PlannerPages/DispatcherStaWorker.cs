using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using Melville.MVVM.Asyncs;
using Melville.MVVM.Wpf.RootWindows;

namespace Planner.Wpf.PlannerPages
{
    public class DispatcherStaWorker:IStaWorker
    {
        private readonly Dispatcher dispatcher;

        public DispatcherStaWorker(RootNavigationWindow rootWin)
        {
            dispatcher = rootWin.Dispatcher;
        }

        public async Task<T> Run<T>(Func<Task<T>> action)
        {
            return await await dispatcher.InvokeAsync(action).Task;
        }
    }
}