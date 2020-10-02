using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Melville.MVVM.CSharpHacks;
using Melville.MVVM.Time;
using NodaTime;
using Planner.Models.Tasks;

namespace Planner.Models.Repositories
{
    public class PlannerTaskLocalToRemoteRepositoryBridge:ILocalPlannerTaskRepository
    {
        private readonly IPlannerTasRemotekRepository remote;
        private readonly IWallClock waiter;

        public PlannerTaskLocalToRemoteRepositoryBridge(IPlannerTasRemotekRepository remote, 
            IWallClock waiter)
        {
            this.remote = remote;
            this.waiter = waiter;
        }

        public PlannerTask CreateTask(string name, LocalDate date)
        {
            var ret = new PlannerTask(Guid.NewGuid()){Name = name, Date = date};
            remote.Add(ret);
            RegisterPropertyChangeNotifications(ret);
            return ret;
        }

        private void RegisterPropertyChangeNotifications(PlannerTask plannerTask) => 
            plannerTask.PropertyChanged += DelayedUpdateEvent;

        private void DelayedUpdateEvent(object? sender, EventArgs _)
        {
            if (sender is PlannerTask rpt) DelayedUpdate(rpt); 
        }
        private async void DelayedUpdate(PlannerTask plannerTask)
        {
            if (await ((IRemoteDatum) plannerTask).WaitForOverridingEvent(waiter, TimeSpan.FromSeconds(2)))
                remote.Update(plannerTask).FireAndForget();
        }

        public PlannerTaskList TasksForDate(LocalDate date)
        {
            var ret = new PlannerTaskList();
            RegisterItemRemovalNotification(ret);
            LoadTasksForDate(date, ret);
            return ret;
        }

        private void RegisterItemRemovalNotification(PlannerTaskList ret) => 
            ret.CollectionChanged += CollectionChanged;

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
                RemoveItems(e.OldItems.OfType<PlannerTask>());
        }

        private void RemoveItems(IEnumerable<PlannerTask> oldTasks)
        {
            foreach (var task in oldTasks)
            {
                remote.Delete(task);
            }
        }

        private async void LoadTasksForDate(LocalDate date, PlannerTaskList ret)
        {
            await foreach (var task in remote.TasksForDate(date))
            {
                RegisterPropertyChangeNotifications(task);
                ret.Add(task);
            }
        }
    }
}